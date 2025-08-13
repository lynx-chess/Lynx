using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using ParseResult = (ulong[] PieceBitBoards, ulong[] OccupancyBitBoards, int[] board, Lynx.Model.Side Side, byte Castle, Lynx.Model.BoardSquare EnPassant, int HalfMoveClock);

namespace Lynx.Benchmark;

#pragma warning disable S112, S6667 // General or reserved exceptions should never be thrown

public partial class ParseFEN_tsoj_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
    [
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.CmkTestPositionFEN,
        Constants.ComplexPositionFEN,
        Constants.KillerTestPositionFEN,
        Constants.TTPositionFEN,
        "r1b1k1n1/1p1p1p1p/n1n1n1n1/1n1n1n1n/n1n1n1n1/1n1n1n1n/P1P1P1P1/R1B1K1N1 w Qq - 1024 1024"
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Original(string fen) => ParseFEN_FENParser_Original.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Improved1(string fen) => ParseFEN_FENParser_tsoj.ParseFEN(fen);

    public static class ParseFEN_FENParser_Original
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
            var occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
            var board = ArrayPool<int>.Shared.Rent(64);
            Array.Fill(board, (int)Piece.None);

            bool success;
            Side side;
            byte castle = 0;
            int halfMoveClock = 0/*, fullMoveCounter = 1*/;
            BoardSquare enPassant = BoardSquare.noSquare;

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

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

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
                ? (pieceBitBoards, occupancyBitBoards, board, side, castle, enPassant, halfMoveClock/*, fullMoveCounter*/)
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
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

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

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece + 6];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
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
        private static byte ParseCastlingRights(ReadOnlySpan<char> castling)
        {
            byte castle = 0;

            for (int i = 0; i < castling.Length; ++i)
            {
                castle |= castling[i] switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new LynxException($"Unrecognized castling char: {castling[i]}")
                };
            }

            return castle;
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

    public static class ParseFEN_FENParser_tsoj
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
            var occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
            var board = ArrayPool<int>.Shared.Rent(64);
            Array.Fill(board, (int)Piece.None);

            bool success;
            Side side;
            byte castle = 0;
            int halfMoveClock = 0/*, fullMoveCounter = 1*/;
            BoardSquare enPassant = BoardSquare.noSquare;

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

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

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
                ? (pieceBitBoards, occupancyBitBoards, board, side, castle, enPassant, halfMoveClock)
                : throw new LynxException($"Error parsing {fen.ToString()}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards, int[] board)
        {
            Span<char> transformedFen = stackalloc char[128];
            int index = 0;

            foreach (var ch in fen)
            {
                if (ch == '/')
                {
                    continue;
                }

                if (char.IsDigit(ch))
                {
                    var count = ch - '0';
                    for (int i = 0; i < count; ++i)
                    {
                        transformedFen[index++] = '1';
                    }

                    continue;
                }

                transformedFen[index++] = ch;
            }

            Debug.Assert(transformedFen[64] == ' ');

            for (int rank = 7; rank >= 0; --rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    var squareIndex = BitBoardExtensions.SquareIndex(7 - rank, file);

                    var piece = transformedFen[squareIndex] switch
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
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(squareIndex);
                        board[squareIndex] = (int)piece;
                    }

                }
            }

            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece + 6];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
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
        private static byte ParseCastlingRights(ReadOnlySpan<char> castling)
        {
            byte castle = 0;

            for (int i = 0; i < castling.Length; ++i)
            {
                castle |= castling[i] switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new LynxException($"Unrecognized castling char: {castling[i]}")
                };
            }

            return castle;
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
}

#pragma warning restore S112, S6667 // General or reserved exceptions should never be thrown
