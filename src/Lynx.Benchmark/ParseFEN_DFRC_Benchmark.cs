using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

#pragma warning disable S112, S6667 // General or reserved exceptions should never be thrown

public class ParseFEN_DFRC_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data => FRC.AllPositions;

    [Benchmark(Baseline = true)]
    public ulong ParseFEN_DFR_Original()
    {
        var pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
        var occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
        var board = ArrayPool<int>.Shared.Rent(64);

        ulong result = 0;
        foreach (var frcPosition in FRC.AllPositions)
        {
            Array.Clear(pieceBitBoards);
            Array.Clear(occupancyBitBoards);
            Array.Fill(board, (int)Piece.None);

            result += (ulong)ParseCastling(frcPosition, pieceBitBoards, occupancyBitBoards, board);
        }

        return result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int ParseCastling(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards, int[] board)
        {
            ParseBoard(fen, pieceBitBoards, occupancyBitBoards, board);

            var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
            Span<Range> parts = stackalloc Range[5];
            var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

            if (partsLength < 3)
            {
                throw new LynxException($"Error parsing second half (after board) of fen {fen}");
            }

            return ParseFEN_DFR_FENParser_Original.ParseDFRCCastlingRights(unparsedStringAsSpan[parts[1]], pieceBitBoards, out var _);
        }
    }

    [Benchmark]
    public ulong ParseFEN_DFR_PrecalculatedKingsideQueensideSquares()
    {
        var pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
        var occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
        var board = ArrayPool<int>.Shared.Rent(64);

        ulong result = 0;
        foreach (var frcPosition in FRC.AllPositions)
        {
            Array.Clear(pieceBitBoards);
            Array.Clear(occupancyBitBoards);
            Array.Fill(board, (int)Piece.None);

            result += (ulong)ParseCastling(frcPosition, pieceBitBoards, occupancyBitBoards, board);
        }

        return result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int ParseCastling(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards, int[] board)
        {
            ParseBoard(fen, pieceBitBoards, occupancyBitBoards, board);

            var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
            Span<Range> parts = stackalloc Range[5];
            var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

            if (partsLength < 3)
            {
                throw new LynxException($"Error parsing second half (after board) of fen {fen}");
            }

            return ParseFEN_DFR_FENParser_PrecalculatedKingsideQueensideSquares.ParseDFRCCastlingRights(unparsedStringAsSpan[parts[1]], pieceBitBoards, out var _);
        }
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

    static class ParseFEN_DFR_FENParser_Original
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ParseDFRCCastlingRights(ReadOnlySpan<char> castlingChars, ReadOnlySpan<BitBoard> pieceBitboards, out byte castlingRights)
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

            return whiteKingsideRook + (10 * whiteQueensideRook) + (100 * blackKingsideRook) + (1000 * blackQueensideRook);
        }
    }

    static class ParseFEN_DFR_FENParser_PrecalculatedKingsideQueensideSquares
    {
        private static readonly BitBoard[] _queensideSquares = GC.AllocateArray<BitBoard>(64, pinned: true);
        private static readonly BitBoard[] _kingsideSquares = GC.AllocateArray<BitBoard>(64, pinned: true);

        static ParseFEN_DFR_FENParser_PrecalculatedKingsideQueensideSquares()
        {
            for (int square = (int)BoardSquare.a1; square <= (int)BoardSquare.h1; ++square)
            {
                PopulateSquares(square);
            }

            for (int square = (int)BoardSquare.a8; square <= (int)BoardSquare.a8; ++square)
            {
                PopulateSquares(square);
            }

            static void PopulateSquares(int square)
            {
                var file = Constants.File[square];

                for (int f = file + 1; f < 8; ++f)
                {
                    _queensideSquares[square].SetBit(square);
                }

                for (int f = file - 1; f >= 0; --f)
                {
                    _kingsideSquares[square].SetBit(square);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ParseDFRCCastlingRights(ReadOnlySpan<char> castlingChars, ReadOnlySpan<BitBoard> pieceBitboards, out byte castlingRights)
        {
            // X-FEN uses KQkq notation when not ambiguous, with the letters referring to "the outermost rook of the affected side"

            int whiteKingsideRook = CastlingData.DefaultValues, whiteQueensideRook = CastlingData.DefaultValues, blackKingsideRook = CastlingData.DefaultValues, blackQueensideRook = CastlingData.DefaultValues;

            Debug.Assert(pieceBitboards[(int)Piece.K] != 0);
            Debug.Assert(pieceBitboards[(int)Piece.k] != 0);

            var whiteKing = pieceBitboards[(int)Piece.K].GetLS1BIndex();
            var blackKing = pieceBitboards[(int)Piece.k].GetLS1BIndex();

            var whiteRooks = pieceBitboards[(int)Piece.R];
            var blackRooks = pieceBitboards[(int)Piece.r];

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

                            whiteKingsideRook = FindNearestQueensideRook(whiteRooks, whiteKing);

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

                            whiteQueensideRook = FindNearestKingsideRook(whiteRooks, whiteKing);

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

                            blackKingsideRook = FindNearestQueensideRook(blackRooks, blackKing);

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

                            blackQueensideRook = FindNearestKingsideRook(blackRooks, blackKing);

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

            return whiteKingsideRook + (10 * whiteQueensideRook) + (100 * blackKingsideRook) + (1000 * blackQueensideRook);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindNearestQueensideRook(BitBoard rooks, int kingSquare)
        {
            var east = rooks & _queensideSquares[kingSquare];

            return east == 0
                ? CastlingData.DefaultValues
                : east.GetLS1BIndex();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindNearestKingsideRook(BitBoard rooks, int kingSquare)
        {
            var west = rooks & _kingsideSquares[kingSquare];

            if (west == 0)
            {
                return CastlingData.DefaultValues;
            }

            // Find the closest rook to the king (highest index among west squares)
            int rookSquare = CastlingData.DefaultValues;
            while (west != 0)
            {
                west.WithoutLS1B(out var sq);

                if (sq > rookSquare)
                {
                    rookSquare = sq;
                }
            }

            return rookSquare;
        }
    }
}

#pragma warning restore S112, S6667 // General or reserved exceptions should never be thrown
