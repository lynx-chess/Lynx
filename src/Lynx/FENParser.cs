using Lynx.Model;
using NLog;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

public static class FENParser
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#pragma warning disable IDE1006 // Naming Styles
    private static readonly SearchValues<char> _DFRCCastlingRightsChars =
#pragma warning restore IDE1006 // Naming Styles
        SearchValues.Create(
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h');

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParseFENResult ParseFEN(ReadOnlySpan<char> fen, Span<Bitboard> pieceBitboards, Span<Bitboard> occupancyBitboards, Span<int> board)
    {
        Debug.Assert(pieceBitboards.Length == Position.PieceBitboardsLength);
        Debug.Assert(occupancyBitboards.Length == Position.OccupancyBitboardsLength);
        Debug.Assert(board.Length == Position.BoardLength);

        fen = fen.Trim();

        pieceBitboards.Clear();
        occupancyBitboards.Clear();
        board.Fill((int)Piece.None);

        bool success;
        Side side;
        byte castlingRights = 0;
        int halfMoveClock = 0;
        int fullMoveCounter = 1;
        BoardSquare enPassant = BoardSquare.noSquare;
        CastlingData castlingData;

        try
        {
            ParseBoard(fen, pieceBitboards, occupancyBitboards, board);

            var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
            Span<Range> parts = stackalloc Range[5];
            var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

            if (partsLength < 3)
            {
                throw new LynxException($"Error parsing second half (after board) of fen {fen}");
            }

            side = ParseSide(unparsedStringAsSpan[parts[0]]);

            castlingData = !Configuration.EngineSettings.IsChess960
                    ? ParseStandardChessCastlingRights(unparsedStringAsSpan[parts[1]], pieceBitboards, out castlingRights)
                    : ParseDFRCCastlingRights(unparsedStringAsSpan[parts[1]], pieceBitboards, out castlingRights);

            (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitboards, side);

            if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
            {
                _logger.Debug("No half move clock detected");
            }

            halfMoveClock = Math.Clamp(halfMoveClock, 0, Constants.MaxNumberOfHalfMovesInAGame);
            if (halfMoveClock >= Constants.MaxNumberOfHalfMovesInAGame - Configuration.EngineSettings.MaxDepth - Constants.ArrayDepthMargin)
            {
                halfMoveClock = 0;
                _logger.Warn("Half move clock too close to its maximum ({0}), resetting it to 0 to avoid issues during searches", Constants.MaxNumberOfHalfMovesInAGame);
            }

            if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
            {
                _logger.Debug("No full move counter detected");
            }

            fullMoveCounter = Math.Clamp(fullMoveCounter, 1, Constants.MaxNumberOfMovesInAGame);

            if (pieceBitboards[(int)Piece.K].CountBits() != 1
                || pieceBitboards[(int)Piece.k].CountBits() != 1)
            {
                throw new LynxException("Missing or extra kings");
            }
        }
#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both - meh
        catch (Exception e)
        {
            _logger.Error(e, "Error parsing FEN {Fen}", fen.ToString());
            success = false;
            throw;
        }
#pragma warning restore S2139 // Exceptions should be either logged or rethrown but not both

        return success
            ? new(side, castlingRights, enPassant,
                castlingData,
                halfMoveClock, fullMoveCounter)
            : throw new LynxException($"Error parsing {fen}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ParseBoard(ReadOnlySpan<char> fen, Span<Bitboard> pieceBitboards, Span<Bitboard> occupancyBitboards, Span<int> board)
    {
        var rankIndex = 0;
        var end = fen.IndexOf('/');

        while (end != -1)
        {
            var match = fen[..end];

            ParseBoardSection(pieceBitboards, board, rankIndex, match);

            fen = fen[(end + 1)..];
            end = fen.IndexOf('/');
            ++rankIndex;
        }

        ParseBoardSection(pieceBitboards, board, rankIndex, fen[..fen.IndexOf(' ')]);

        // Populate occupancies
        for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
        {
            occupancyBitboards[(int)Side.White] |= pieceBitboards[piece];
            occupancyBitboards[(int)Side.Black] |= pieceBitboards[piece + 6];
        }

        occupancyBitboards[(int)Side.Both] = occupancyBitboards[(int)Side.White] | occupancyBitboards[(int)Side.Black];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ParseBoardSection(Span<Bitboard> pieceBitboards, Span<int> board, int rankIndex, ReadOnlySpan<char> boardfenSection)
        {
            int fileIndex = 0;

            foreach (var ch in boardfenSection)
            {
                var piece = ch switch
                {
                    'P' => Piece.P,
                    'N' => Piece.N,
                    'B' => Piece.B,
                    'R' => Piece.R,
                    'Q' => Piece.Q,
                    'K' => Piece.K,

                    'p' => Piece.p,
                    'n' => Piece.n,
                    'b' => Piece.b,
                    'r' => Piece.r,
                    'q' => Piece.q,
                    'k' => Piece.k,

                    _ => Piece.None,
                };

                if (piece != Piece.None)
                {
                    var square = BitboardExtensions.SquareIndex(rankIndex, fileIndex);
                    pieceBitboards[(int)piece] = pieceBitboards[(int)piece].SetBit(square);
                    board[square] = (int)piece;
                    ++fileIndex;
                }
                else
                {
                    fileIndex += ch - '0';
                    Debug.Assert(fileIndex >= 1 && fileIndex <= 8, $"Error parsing char {ch} in fen {boardfenSection.ToString()}");
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Side ParseSide(ReadOnlySpan<char> side)
    {
        return side[0] switch
        {
            'w' or 'W' => Side.White,
            'b' or 'B' => Side.Black,
            _ => throw new LynxException($"Unrecognized side: {side}"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CastlingData ParseStandardChessCastlingRights(ReadOnlySpan<char> castlingChars, ReadOnlySpan<Bitboard> pieceBitboards, out byte castlingRights)
    {
        if (castlingChars.ContainsAny(_DFRCCastlingRightsChars))
        {
            _logger.Warn("DFRC position detected without UCI_Chess960 set. Enabling it as a fallback");
            Configuration.EngineSettings.IsChess960 = true;

            return ParseDFRCCastlingRights(castlingChars, pieceBitboards, out castlingRights);
        }

        var isWhiteKingOnInitialSquare = pieceBitboards[(int)Piece.K].GetBit(Constants.InitialWhiteKingSquare);
        var isBlackKingOnInitialSquare = pieceBitboards[(int)Piece.k].GetBit(Constants.InitialBlackKingSquare);

        castlingRights = 0;

        for (int i = 0; i < castlingChars.Length; ++i)
        {
            var ch = castlingChars[i];
            switch (ch)
            {
                case 'K':
                    {
                        if (isWhiteKingOnInitialSquare && pieceBitboards[(int)Piece.R].GetBit(Constants.InitialWhiteKingsideRookSquare))
                        {
                            castlingRights |= (byte)CastlingRights.WK;
                        }
                        else
                        {
                            _logger.Warn("Castling right {CastlingRight} ignored: no white king on e1 or no white rook on h1. Set UCI_Chess960 if this is a DFRC position", ch);
                        }

                        break;
                    }
                case 'Q':
                    {
                        if (isWhiteKingOnInitialSquare && pieceBitboards[(int)Piece.R].GetBit(Constants.InitialWhiteQueensideRookSquare))
                        {
                            castlingRights |= (byte)CastlingRights.WQ;
                        }
                        else
                        {
                            _logger.Warn("Castling right {CastlingRight} ignored: no white king on e1 or no white rook on a1. Set UCI_Chess960 if this is a DFRC position", ch);
                        }

                        break;
                    }
                case 'k':
                    {
                        if (isBlackKingOnInitialSquare && pieceBitboards[(int)Piece.r].GetBit(Constants.InitialBlackKingsideRookSquare))
                        {
                            castlingRights |= (byte)CastlingRights.BK;
                        }
                        else
                        {
                            _logger.Warn("Castling right {CastlingRight} ignored: no black king on e8 or no black rook on h8. Set UCI_Chess960 if this is a DFRC position", ch);
                        }

                        break;
                    }
                case 'q':
                    {
                        if (isBlackKingOnInitialSquare && pieceBitboards[(int)Piece.r].GetBit(Constants.InitialBlackQueensideRookSquare))
                        {
                            castlingRights |= (byte)CastlingRights.BQ;
                        }
                        else
                        {
                            _logger.Warn("Castling right {CastlingRight} ignored: no black king on e8 or no black rook on a8. Set UCI_Chess960 if this is a DFRC position", ch);
                        }

                        break;
                    }
                case '-':
                    {
                        break;
                    }
                default:
                    {
                        throw new LynxException($"Unrecognized castling char: {ch}");
                    }
            }
        }

        return new CastlingData(
            Constants.InitialWhiteKingsideRookSquare, Constants.InitialWhiteQueensideRookSquare,
            Constants.InitialBlackKingsideRookSquare, Constants.InitialBlackQueensideRookSquare);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CastlingData ParseDFRCCastlingRights(ReadOnlySpan<char> castlingChars, ReadOnlySpan<Bitboard> pieceBitboards, out byte castlingRights)
    {
        // X-FEN uses KQkq notation when not ambiguous, with the letters referring to "the outermost rook of the affected side"

        int whiteKingsideRook = CastlingData.DefaultValues, whiteQueensideRook = CastlingData.DefaultValues, blackKingsideRook = CastlingData.DefaultValues, blackQueensideRook = CastlingData.DefaultValues;

        Debug.Assert(pieceBitboards[(int)Piece.K] != 0);
        Debug.Assert(pieceBitboards[(int)Piece.k] != 0);

        var whiteKing = pieceBitboards[(int)Piece.K].GetLS1BIndex();
        var blackKing = pieceBitboards[(int)Piece.k].GetLS1BIndex();

        var isWhiteKingOnBackRank = Constants.Rank(whiteKing) == 0;
        var isBlackKingOnBackRank = Constants.Rank(blackKing) == 7;

        castlingRights = 0;

        for (int i = 0; i < castlingChars.Length; ++i)
        {
            var ch = castlingChars[i];
            switch (ch)
            {
                case 'K':
                    {
                        Debug.Assert(whiteKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white kingside rooks detected {whiteKingsideRook}");

                        if (isWhiteKingOnBackRank)
                        {
                            for (int potentialRookSquareIndex = Constants.InitialWhiteKingsideRookSquare; potentialRookSquareIndex > whiteKing; --potentialRookSquareIndex)
                            {
                                if (pieceBitboards[(int)Piece.R].GetBit(potentialRookSquareIndex))
                                {
                                    whiteKingsideRook = potentialRookSquareIndex;
                                    break;
                                }
                            }
                        }

                        if (whiteKingsideRook != CastlingData.DefaultValues)
                        {
                            castlingRights |= (byte)CastlingRights.WK;
                        }
                        else
                        {
                            _logger.Warn("Castling right {CastlingRight} ignored: white king not on the first rank or no white rook on its kingside", ch);
                        }

                        break;
                    }
                case 'Q':
                    {
                        Debug.Assert(whiteQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white queenside rooks detected {whiteQueensideRook}");

                        if (isWhiteKingOnBackRank)
                        {
                            for (int potentialRookSquareIndex = Constants.InitialWhiteQueensideRookSquare; potentialRookSquareIndex < whiteKing; ++potentialRookSquareIndex)
                            {
                                if (pieceBitboards[(int)Piece.R].GetBit(potentialRookSquareIndex))
                                {
                                    whiteQueensideRook = potentialRookSquareIndex;
                                    break;
                                }
                            }
                        }

                        if (whiteQueensideRook != CastlingData.DefaultValues)
                        {
                            castlingRights |= (byte)CastlingRights.WQ;
                        }
                        else
                        {
                            _logger.Warn("Castling right {CastlingRight} ignored: white king not on the first rank or no white rook on its queenside", ch);
                        }

                        break;
                    }
                case 'k':
                    {
                        Debug.Assert(blackKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black kingside rooks detected {blackKingsideRook}");

                        if (isBlackKingOnBackRank)
                        {
                            for (int potentialRookSquareIndex = Constants.InitialBlackKingsideRookSquare; potentialRookSquareIndex > blackKing; --potentialRookSquareIndex)
                            {
                                if (pieceBitboards[(int)Piece.r].GetBit(potentialRookSquareIndex))
                                {
                                    blackKingsideRook = potentialRookSquareIndex;
                                    break;
                                }
                            }
                        }

                        if (blackKingsideRook != CastlingData.DefaultValues)
                        {
                            castlingRights |= (byte)CastlingRights.BK;
                        }
                        else
                        {
                            _logger.Warn("Castling right {CastlingRight} ignored: black king not on the eighth rank or no black rook on its kingside", ch);
                        }

                        break;
                    }
                case 'q':
                    {
                        Debug.Assert(blackQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black queenside rooks detected {blackQueensideRook}");

                        if (isBlackKingOnBackRank)
                        {
                            for (int potentialRookSquareIndex = Constants.InitialBlackQueensideRookSquare; potentialRookSquareIndex < blackKing; ++potentialRookSquareIndex)
                            {
                                if (pieceBitboards[(int)Piece.r].GetBit(potentialRookSquareIndex))
                                {
                                    blackQueensideRook = potentialRookSquareIndex;
                                    break;
                                }
                            }
                        }

                        if (blackQueensideRook != CastlingData.DefaultValues)
                        {
                            castlingRights |= (byte)CastlingRights.BQ;
                        }
                        else
                        {
                            _logger.Warn("Castling right {CastlingRight} ignored: black king not on the eighth rank or no black rook on its queenside", ch);
                        }

                        break;
                    }
                case '-':
                    {
                        //castle |= (byte)CastlingRights.None;
                        break;
                    }
                default:
                    {
                        if (ch >= 'A' && ch <= 'H')
                        {
                            var square = BitboardExtensions.SquareIndex(rank: 7, file: ch - 'A');

                            if (square == whiteKing)
                            {
                                throw new LynxException($"Invalid castle character {ch}: it matches white king square ({square})");
                            }

                            if (!isWhiteKingOnBackRank || !pieceBitboards[(int)Piece.R].GetBit(square))
                            {
                                _logger.Warn("Castling right {CastlingRight} ignored: white king not on the first rank or no white rook on {Square}", ch, (BoardSquare)square);
                                break;
                            }

                            if (square < whiteKing)
                            {
                                Debug.Assert(whiteQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white queenside rooks detected {whiteQueensideRook}");
                                castlingRights |= (byte)CastlingRights.WQ;
                                whiteQueensideRook = square;
                            }
                            else
                            {
                                Debug.Assert(whiteKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white kingside rooks detected {whiteKingsideRook}");
                                castlingRights |= (byte)CastlingRights.WK;
                                whiteKingsideRook = square;
                            }

                            break;
                        }

                        if (ch >= 'a' && ch <= 'h')
                        {
                            var square = BitboardExtensions.SquareIndex(rank: 0, file: ch - 'a');

                            if (square == blackKing)
                            {
                                throw new LynxException($"Invalid castle character {ch}: it matches black king square ({square})");
                            }

                            if (!isBlackKingOnBackRank || !pieceBitboards[(int)Piece.r].GetBit(square))
                            {
                                _logger.Warn("Castling right {CastlingRight} ignored: black king not on the eighth rank or no black rook on {Square}", ch, (BoardSquare)square);
                                break;
                            }

                            if (square < blackKing)
                            {
                                Debug.Assert(blackQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black queenside rooks detected {blackQueensideRook}");
                                castlingRights |= (byte)CastlingRights.BQ;
                                blackQueensideRook = square;
                            }
                            else
                            {
                                Debug.Assert(blackKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black kingside rooks detected {blackKingsideRook}");
                                castlingRights |= (byte)CastlingRights.BK;
                                blackKingsideRook = square;
                            }

                            break;
                        }

                        throw new LynxException($"Unrecognized castle character: {ch}");
                    }
            }
        }

        return new CastlingData(whiteKingsideRook, whiteQueensideRook, blackKingsideRook, blackQueensideRook);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, ReadOnlySpan<Bitboard> PieceBitboards, Side side)
    {
        bool success = true;
        BoardSquare enPassant = BoardSquare.noSquare;

        // Enum.TryParse is too slow
        if (TryParseEnPassantSquare(enPassantSpan, out var parsedSquare))
        {
            enPassant = parsedSquare;

            if (enPassant != BoardSquare.noSquare)
            {
                // Check that there's an actual pawn to be captured
                var pawnOffset = ((int)side << 4) - 8; // side == Side.White ? +8 : -8
                var pawnSquare = (int)enPassant + pawnOffset;

                var pawnBitboard = PieceBitboards[(int)Piece.P + Utils.PieceOffset(Utils.OppositeSide((int)side))];

                if (!pawnBitboard.GetBit(pawnSquare))
                {
                    success = false;
                    enPassant = BoardSquare.noSquare;
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
                }
            }
        }
        else if (enPassantSpan.Length != 1 || enPassantSpan[0] != '-')
        {
            success = false;
            _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
        }

        return (enPassant, success);

        /// <summary>
        /// Fast alternative to Enum.TryParse
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool TryParseEnPassantSquare(ReadOnlySpan<char> enPassantSpan, out BoardSquare square)
        {
            if (enPassantSpan.Length == 1 && enPassantSpan[0] == '-')
            {
                square = BoardSquare.noSquare;
                return true;
            }

            if (enPassantSpan.Length != 2)
            {
                square = BoardSquare.noSquare;
                return false;
            }

            // Normalize to lowercase without branching
            // https://blog.cloudflare.com/the-oldest-trick-in-the-ascii-book/
            // Lowercase ASCII is uppercase ASCII + 0x20
            var fileChar = (char)(enPassantSpan[0] | 0x20);
            int file = fileChar - 'a'; // 0-7
            int rank = enPassantSpan[1] - '0';  // 1-8

            // Only ranks 3 and 6 are legal en passant target squares.
            if ((uint)file >= 8 || (rank != 3 && rank != 6))
            {
                square = BoardSquare.noSquare;
                _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());

                return false;
            }

            square = (BoardSquare)BitboardExtensions.SquareIndex(8 - rank, file);
            return true;
        }
    }
}
