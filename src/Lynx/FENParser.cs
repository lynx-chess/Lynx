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
    public static ParseFENResult ParseFEN(ReadOnlySpan<char> fen)
    {
        fen = fen.Trim();

        // Arrays will be be returned as part of Position cleanaup
        var pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
        var occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
        var board = ArrayPool<int>.Shared.Rent(64);
        Array.Fill(board, (int)Piece.None);

        bool success;
        Side side;
        byte castlingRights = 0;
        int halfMoveClock = 0/*, fullMoveCounter = 1*/;
        BoardSquare enPassant = BoardSquare.noSquare;
        CastlingData castlingData;

        try
        {
            ParseBoard(fen, pieceBitBoards, occupancyBitBoards, board);

            var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
            Span<Range> parts = stackalloc Range[5];
            var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

            if (partsLength < 3)
            {
                throw new LynxException($"Error parsing second half (after board) of fen {fen}");
            }

            side = ParseSide(unparsedStringAsSpan[parts[0]]);

            castlingData = !Configuration.EngineSettings.IsChess960
                    ? ParseStandardChessCastlingRights(unparsedStringAsSpan[parts[1]], pieceBitBoards, out castlingRights)
                    : ParseDFRCCastlingRights(unparsedStringAsSpan[parts[1]], pieceBitBoards, out castlingRights);

            (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

            if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
            {
                _logger.Debug("No half move clock detected");
            }

            //if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
            //{
            //    _logger.Debug("No full move counter detected");
            //}

            if (pieceBitBoards[(int)Piece.K].CountBits() != 1
                || pieceBitBoards[(int)Piece.k].CountBits() != 1)
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
            ? new(pieceBitBoards, occupancyBitBoards, board, side, castlingRights, enPassant,
                castlingData,
                halfMoveClock/*, fullMoveCounter*/)
            : throw new LynxException($"Error parsing {fen.ToString()}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards, int[] board)
    {
        var rankIndex = 0;
        var end = fen.IndexOf('/');

        while (end != -1)
        {
            var match = fen[..end];

            ParseBoardSection(pieceBitBoards, board, rankIndex, match);

            fen = fen[(end + 1)..];
            end = fen.IndexOf('/');
            ++rankIndex;
        }

        ParseBoardSection(pieceBitBoards, board, rankIndex, fen[..fen.IndexOf(' ')]);

        // Populate occupancies
        for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
        {
            occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
            occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece + 6];
        }

        occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ParseBoardSection(BitBoard[] pieceBitBoards, int[] board, int rankIndex, ReadOnlySpan<char> boardfenSection)
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

                    _ => Piece.None
                };

                if (piece != Piece.None)
                {
                    var square = BitBoardExtensions.SquareIndex(rankIndex, fileIndex);
                    pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(square);
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
            _ => throw new LynxException($"Unrecognized side: {side}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CastlingData ParseStandardChessCastlingRights(ReadOnlySpan<char> castlingChars, ReadOnlySpan<BitBoard> pieceBitboards, out byte castlingRights)
    {
        if (castlingChars.ContainsAny(_DFRCCastlingRightsChars))
        {
            _logger.Warn("DFRC position detected without UCI_Chess960 set. Enabling it as a fallback");
            Configuration.EngineSettings.IsChess960 = true;

            return ParseDFRCCastlingRights(castlingChars, pieceBitboards, out castlingRights);
        }

        castlingRights = 0;
        for (int i = 0; i < castlingChars.Length; ++i)
        {
            castlingRights |= castlingChars[i] switch
            {
                'K' => (byte)CastlingRights.WK,
                'Q' => (byte)CastlingRights.WQ,
                'k' => (byte)CastlingRights.BK,
                'q' => (byte)CastlingRights.BQ,
                '-' => castlingRights,
                _ => throw new LynxException($"Unrecognized castling char: {castlingChars[i]}")
            };
        }

        return new CastlingData(
            Constants.InitialWhiteKingsideRookSquare, Constants.InitialWhiteQueensideRookSquare,
            Constants.InitialBlackKingsideRookSquare, Constants.InitialBlackQueensideRookSquare);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CastlingData ParseDFRCCastlingRights(ReadOnlySpan<char> castlingChars, ReadOnlySpan<BitBoard> pieceBitboards, out byte castlingRights)
    {
        // X-FEN uses KQkq notation when not ambiguous, with the letters referring to "the outermost rook of the affected side"

        int whiteKingsideRook = CastlingData.DefaultValues, whiteQueensideRook = CastlingData.DefaultValues, blackKingsideRook = CastlingData.DefaultValues, blackQueensideRook = CastlingData.DefaultValues;

        Debug.Assert(pieceBitboards[(int)Piece.K] != 0);
        Debug.Assert(pieceBitboards[(int)Piece.k] != 0);

        var whiteKing = pieceBitboards[(int)Piece.K].GetLS1BIndex();
        var blackKing = pieceBitboards[(int)Piece.k].GetLS1BIndex();

        castlingRights = 0;

        for (int i = 0; i < castlingChars.Length; ++i)
        {
            var ch = castlingChars[i];
            switch (ch)
            {
                case 'K':
                    {
                        Debug.Assert(whiteKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white kingside rooks detected {whiteKingsideRook}");

                        castlingRights |= (byte)CastlingRights.WK;

                        for (int potentialRookSquareIndex = Constants.InitialWhiteKingsideRookSquare; potentialRookSquareIndex > whiteKing; --potentialRookSquareIndex)
                        {
                            if (pieceBitboards[(int)Piece.R].GetBit(potentialRookSquareIndex))
                            {
                                whiteKingsideRook = potentialRookSquareIndex;
                                break;
                            }
                        }

                        if (whiteKingsideRook == CastlingData.DefaultValues)
                        {
                            throw new LynxException($"Invalid castling rights: {ch} specified, but no rook found");
                        }

                        break;
                    }
                case 'Q':
                    {
                        Debug.Assert(whiteQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white queenside rooks detected {whiteQueensideRook}");

                        castlingRights |= (byte)CastlingRights.WQ;

                        for (int potentialRookSquareIndex = Constants.InitialWhiteQueensideRookSquare; potentialRookSquareIndex < whiteKing; ++potentialRookSquareIndex)
                        {
                            if (pieceBitboards[(int)Piece.R].GetBit(potentialRookSquareIndex))
                            {
                                whiteQueensideRook = potentialRookSquareIndex;
                                break;
                            }
                        }

                        if (whiteQueensideRook == CastlingData.DefaultValues)
                        {
                            throw new LynxException($"Invalid castling rights: {ch} specified, but no rook found");
                        }

                        break;
                    }
                case 'k':
                    {
                        Debug.Assert(blackKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black kingside rooks detected {blackKingsideRook}");

                        castlingRights |= (byte)CastlingRights.BK;

                        for (int potentialRookSquareIndex = Constants.InitialBlackKingsideRookSquare; potentialRookSquareIndex > blackKing; --potentialRookSquareIndex)
                        {
                            if (pieceBitboards[(int)Piece.r].GetBit(potentialRookSquareIndex))
                            {
                                blackKingsideRook = potentialRookSquareIndex;
                                break;
                            }
                        }

                        if (blackKingsideRook == CastlingData.DefaultValues)
                        {
                            throw new LynxException($"Invalid castling rights: {ch} specified, but no rook found");
                        }

                        break;
                    }
                case 'q':
                    {
                        Debug.Assert(blackQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black queenside rooks detected {blackQueensideRook}");

                        castlingRights |= (byte)CastlingRights.BQ;

                        for (int potentialRookSquareIndex = Constants.InitialBlackQueensideRookSquare; potentialRookSquareIndex < blackKing; ++potentialRookSquareIndex)
                        {
                            if (pieceBitboards[(int)Piece.r].GetBit(potentialRookSquareIndex))
                            {
                                blackQueensideRook = potentialRookSquareIndex;
                                break;
                            }
                        }

                        if (blackQueensideRook == CastlingData.DefaultValues)
                        {
                            throw new LynxException($"Invalid castling rights: {ch} specified, but no rook found");
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
                            var square = BitBoardExtensions.SquareIndex(rank: 7, file: ch - 'A');

                            if (square < whiteKing)
                            {
                                Debug.Assert(whiteQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white queenside rooks detected {whiteQueensideRook}");
                                castlingRights |= (byte)CastlingRights.WQ;
                                whiteQueensideRook = square;
                            }
                            else if (square > whiteKing)
                            {
                                Debug.Assert(whiteKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white kingside rooks detected {whiteKingsideRook}");
                                castlingRights |= (byte)CastlingRights.WK;
                                whiteKingsideRook = square;
                            }
                            else
                            {
                                throw new LynxException($"Invalid castle character {ch}: it matches white king square ({square})");
                            }

                            break;
                        }
                        else if (ch >= 'a' && ch <= 'h')
                        {
                            var square = BitBoardExtensions.SquareIndex(rank: 0, file: ch - 'a');

                            if (square < blackKing)
                            {
                                Debug.Assert(blackQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black queenside rooks detected {blackQueensideRook}");
                                castlingRights |= (byte)CastlingRights.BQ;
                                blackQueensideRook = square;
                            }
                            else if (square > blackKing)
                            {
                                Debug.Assert(blackKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black kingside rooks detected {blackKingsideRook}");
                                castlingRights |= (byte)CastlingRights.BK;
                                blackKingsideRook = square;
                            }
                            else
                            {
                                throw new LynxException($"Invalid castle character {ch}: it matches black king square ({square})");
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
    private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, BitBoard[] PieceBitBoards, Side side)
    {
        bool success = true;
        BoardSquare enPassant = BoardSquare.noSquare;

        if (Enum.TryParse(enPassantSpan, ignoreCase: true, out BoardSquare result))
        {
            enPassant = result;

            var rank = 1 + ((int)enPassant >> 3);
            if (rank != 3 && rank != 6)
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
            }

            // Check that there's an actual pawn to be captured
            var pawnOffset = side == Side.White
                ? +8
                : -8;

            var pawnSquare = (int)enPassant + pawnOffset;

            var pawnBitBoard = side == Side.White
                ? PieceBitBoards[(int)Piece.p]
                : PieceBitBoards[(int)Piece.P];

            if (!pawnBitBoard.GetBit(pawnSquare))
            {
                success = false;
                _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
            }
        }
        else if (enPassantSpan[0] != '-')
        {
            success = false;
            _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
        }

        return (enPassant, success);
    }
}
