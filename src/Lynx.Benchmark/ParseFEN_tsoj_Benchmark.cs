/*
 *  BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.304
 *    [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
 *
 *  | Method             | fen                  | Mean     | Error   | StdDev  | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |--------------------- |---------:|--------:|--------:|------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original  | 8/k7/(...)- 0 1 [39] | 328.1 ns | 0.66 ns | 0.62 ns |  1.00 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | 8/k7/(...)- 0 1 [39] | 487.3 ns | 1.80 ns | 1.59 ns |  1.49 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |        |        |           |             |
 *  | ParseFEN_Original  | r1b1k(...) 1024 [88] | 440.1 ns | 1.75 ns | 1.64 ns |  1.00 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | r1b1k(...) 1024 [88] | 598.2 ns | 1.37 ns | 1.22 ns |  1.36 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |        |        |           |             |
 *  | ParseFEN_Original  | r2q1r(...)- 0 9 [68] | 401.8 ns | 1.29 ns | 1.21 ns |  1.00 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | r2q1r(...)- 0 9 [68] | 566.1 ns | 2.95 ns | 2.61 ns |  1.41 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |        |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 404.0 ns | 1.20 ns | 1.07 ns |  1.00 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | r3k2r(...)- 0 1 [68] | 571.8 ns | 2.13 ns | 2.00 ns |  1.42 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |        |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 406.8 ns | 1.91 ns | 1.60 ns |  1.00 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | r3k2r(...)- 0 1 [68] | 566.0 ns | 2.15 ns | 1.91 ns |  1.39 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |        |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)6 0 1 [67] | 404.7 ns | 2.34 ns | 2.19 ns |  1.00 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | rnbqk(...)6 0 1 [67] | 591.5 ns | 1.56 ns | 1.38 ns |  1.46 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |        |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)- 0 1 [56] | 380.8 ns | 1.12 ns | 0.99 ns |  1.00 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | rnbqk(...)- 0 1 [56] | 563.9 ns | 1.38 ns | 1.22 ns |  1.48 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |        |        |           |             |
 *  | ParseFEN_Original  | rq2k2(...)- 0 1 [71] | 404.3 ns | 1.63 ns | 1.52 ns |  1.00 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | rq2k2(...)- 0 1 [71] | 567.5 ns | 1.22 ns | 1.09 ns |  1.40 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.3932) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.304
 *    [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
 *
 *  | Method             | fen                  | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |--------------------- |---------:|--------:|--------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original  | 8/k7/(...)- 0 1 [39] | 332.1 ns | 1.62 ns | 1.52 ns |  1.00 |    0.01 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | 8/k7/(...)- 0 1 [39] | 450.0 ns | 1.55 ns | 1.37 ns |  1.35 |    0.01 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |         |        |        |           |             |
 *  | ParseFEN_Original  | r1b1k(...) 1024 [88] | 434.7 ns | 2.22 ns | 2.08 ns |  1.00 |    0.01 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | r1b1k(...) 1024 [88] | 533.7 ns | 2.13 ns | 1.99 ns |  1.23 |    0.01 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |         |        |        |           |             |
 *  | ParseFEN_Original  | r2q1r(...)- 0 9 [68] | 407.7 ns | 2.11 ns | 1.97 ns |  1.00 |    0.01 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | r2q1r(...)- 0 9 [68] | 502.4 ns | 4.84 ns | 4.29 ns |  1.23 |    0.01 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |         |        |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 407.9 ns | 2.91 ns | 2.72 ns |  1.00 |    0.01 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | r3k2r(...)- 0 1 [68] | 464.3 ns | 4.15 ns | 3.88 ns |  1.14 |    0.01 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |         |        |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 366.7 ns | 2.68 ns | 2.51 ns |  1.00 |    0.01 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | r3k2r(...)- 0 1 [68] | 486.2 ns | 4.02 ns | 3.76 ns |  1.33 |    0.01 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |         |        |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)6 0 1 [67] | 390.2 ns | 4.70 ns | 4.39 ns |  1.00 |    0.02 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | rnbqk(...)6 0 1 [67] | 519.9 ns | 5.06 ns | 4.48 ns |  1.33 |    0.02 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |         |        |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)- 0 1 [56] | 379.4 ns | 2.60 ns | 2.43 ns |  1.00 |    0.01 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | rnbqk(...)- 0 1 [56] | 480.3 ns | 1.53 ns | 1.28 ns |  1.27 |    0.01 | 0.0343 | 0.0010 |     584 B |        1.00 |
 *  |                    |                      |          |         |         |       |         |        |        |           |             |
 *  | ParseFEN_Original  | rq2k2(...)- 0 1 [71] | 384.7 ns | 5.61 ns | 5.25 ns |  1.00 |    0.02 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *  | ParseFEN_tsoj      | rq2k2(...)- 0 1 [71] | 467.2 ns | 2.13 ns | 1.88 ns |  1.21 |    0.02 | 0.0348 | 0.0010 |     584 B |        1.00 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using ParseResult = (ulong[] PieceBitboards, ulong[] OccupancyBitboards, int[] board, Lynx.Model.Side Side, byte Castle, Lynx.Model.BoardSquare EnPassant, int HalfMoveClock);

namespace Lynx.Benchmark;

#pragma warning disable S112, S6667 // General or reserved exceptions should never be thrown

public class ParseFEN_tsoj_Benchmark : BaseBenchmark
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
    public ParseResult ParseFEN_tsoj(string fen) => ParseFEN_FENParser_tsoj.ParseFEN(fen);

    public static class ParseFEN_FENParser_Original
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitboards = ArrayPool<Bitboard>.Shared.Rent(12);
            var occupancyBitboards = ArrayPool<Bitboard>.Shared.Rent(3);
            var board = ArrayPool<int>.Shared.Rent(64);
            Array.Fill(board, (int)Piece.None);

            bool success;
            Side side;
            byte castle = 0;
            int halfMoveClock = 0/*, fullMoveCounter = 1*/;
            BoardSquare enPassant = BoardSquare.noSquare;

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

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitboards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                //if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                //{
                //    _logger.Debug("No full move counter detected");
                //}

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
                ? (pieceBitboards, occupancyBitboards, board, side, castle, enPassant, halfMoveClock/*, fullMoveCounter*/)
                : throw new LynxException($"Error parsing {fen.ToString()}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ParseBoard(ReadOnlySpan<char> fen, Bitboard[] pieceBitboards, Bitboard[] occupancyBitboards, int[] board)
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
            PopulateOccupancies(pieceBitboards, occupancyBitboards);

            static void ParseBoardSection(Bitboard[] pieceBitboards, int[] board, int rankIndex, ReadOnlySpan<char> boardfenSection)
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

            static void PopulateOccupancies(Bitboard[] pieceBitboards, Bitboard[] occupancyBitboards)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    occupancyBitboards[(int)Side.White] |= pieceBitboards[piece];
                    occupancyBitboards[(int)Side.Black] |= pieceBitboards[piece + 6];
                }

                occupancyBitboards[(int)Side.Both] = occupancyBitboards[(int)Side.White] | occupancyBitboards[(int)Side.Black];
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
        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, Bitboard[] PieceBitboards, Side side)
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

                var pawnBitboard = side == Side.White
                    ? PieceBitboards[(int)Piece.p]
                    : PieceBitboards[(int)Piece.P];

                if (!pawnBitboard.GetBit(pawnSquare))
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

            var pieceBitboards = ArrayPool<Bitboard>.Shared.Rent(12);
            var occupancyBitboards = ArrayPool<Bitboard>.Shared.Rent(3);
            var board = ArrayPool<int>.Shared.Rent(64);
            Array.Fill(board, (int)Piece.None);

            bool success;
            Side side;
            byte castle = 0;
            int halfMoveClock = 0/*, fullMoveCounter = 1*/;
            BoardSquare enPassant = BoardSquare.noSquare;

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

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitboards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                //if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                //{
                //    _logger.Debug("No full move counter detected");
                //}

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
                ? (pieceBitboards, occupancyBitboards, board, side, castle, enPassant, halfMoveClock)
                : throw new LynxException($"Error parsing {fen.ToString()}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ParseBoard(ReadOnlySpan<char> fen, Bitboard[] pieceBitboards, Bitboard[] occupancyBitboards, int[] board)
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
                    var squareIndex = BitboardExtensions.SquareIndex(7 - rank, file);

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
                        pieceBitboards[(int)piece] = pieceBitboards[(int)piece].SetBit(squareIndex);
                        board[squareIndex] = (int)piece;
                    }
                }
            }

            PopulateOccupancies(pieceBitboards, occupancyBitboards);

            static void PopulateOccupancies(Bitboard[] pieceBitboards, Bitboard[] occupancyBitboards)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    occupancyBitboards[(int)Side.White] |= pieceBitboards[piece];
                    occupancyBitboards[(int)Side.Black] |= pieceBitboards[piece + 6];
                }

                occupancyBitboards[(int)Side.Both] = occupancyBitboards[(int)Side.White] | occupancyBitboards[(int)Side.Black];
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
        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, Bitboard[] PieceBitboards, Side side)
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

                var pawnBitboard = side == Side.White
                    ? PieceBitboards[(int)Piece.p]
                    : PieceBitboards[(int)Piece.P];

                if (!pawnBitboard.GetBit(pawnSquare))
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
