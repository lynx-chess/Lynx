/*
 * Span Split()
 *
 *  BenchmarkDotNet v0.13.8, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 * | Method            | fen                  | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
 * |------------------ |--------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 * | ParseFEN_Original | 8/k7/(...)- 0 1 [39] | 3.202 us | 0.0196 us | 0.0163 us |  1.00 | 0.1564 |   2.89 KB |        1.00 |
 * | ParseFEN_Current  | 8/k7/(...)- 0 1 [39] | 3.057 us | 0.0182 us | 0.0170 us |  0.96 | 0.1411 |   2.64 KB |        0.91 |
 * |                   |                      |          |           |           |       |        |           |             |
 * | ParseFEN_Original | r2q1r(...)- 0 9 [68] | 3.897 us | 0.0267 us | 0.0249 us |  1.00 | 0.1678 |   3.09 KB |        1.00 |
 * | ParseFEN_Current  | r2q1r(...)- 0 9 [68] | 3.816 us | 0.0194 us | 0.0162 us |  0.98 | 0.1526 |   2.84 KB |        0.92 |
 * |                   |                      |          |           |           |       |        |           |             |
 * | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 3.793 us | 0.0254 us | 0.0238 us |  1.00 | 0.1640 |   3.01 KB |        1.00 |
 * | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 3.775 us | 0.0258 us | 0.0242 us |  1.00 | 0.1450 |   2.75 KB |        0.91 |
 * |                   |                      |          |           |           |       |        |           |             |
 * | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 3.740 us | 0.0281 us | 0.0263 us |  1.00 | 0.1640 |   3.01 KB |        1.00 |
 * | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 3.598 us | 0.0329 us | 0.0308 us |  0.96 | 0.1488 |   2.75 KB |        0.91 |
 * |                   |                      |          |           |           |       |        |           |             |
 * | ParseFEN_Original | rnbqk(...)6 0 1 [67] | 3.709 us | 0.0204 us | 0.0191 us |  1.00 | 0.1640 |      3 KB |        1.00 |
 * | ParseFEN_Current  | rnbqk(...)6 0 1 [67] | 3.536 us | 0.0131 us | 0.0117 us |  0.95 | 0.1488 |   2.73 KB |        0.91 |
 * |                   |                      |          |           |           |       |        |           |             |
 * | ParseFEN_Original | rnbqk(...)- 0 1 [56] | 2.680 us | 0.0106 us | 0.0099 us |  1.00 | 0.1450 |    2.7 KB |        1.00 |
 * | ParseFEN_Current  | rnbqk(...)- 0 1 [56] | 2.519 us | 0.0198 us | 0.0186 us |  0.94 | 0.1335 |   2.44 KB |        0.90 |
 * |                   |                      |          |           |           |       |        |           |             |
 * | ParseFEN_Original | rq2k2(...)- 0 1 [71] | 4.194 us | 0.0120 us | 0.0113 us |  1.00 | 0.1678 |   3.09 KB |        1.00 |
 * | ParseFEN_Current  | rq2k2(...)- 0 1 [71] | 3.973 us | 0.0358 us | 0.0299 us |  0.95 | 0.1526 |   2.84 KB |        0.92 |
 *
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method            | fen                  | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------ |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | ParseFEN_Original | 8/k7/(...)- 0 1 [39] | 3.387 us | 0.0395 us | 0.0370 us |  1.00 |    0.00 | 0.1564 |   2.89 KB |        1.00 |
 *  | ParseFEN_Current  | 8/k7/(...)- 0 1 [39] | 3.407 us | 0.0453 us | 0.0424 us |  1.01 |    0.01 | 0.1411 |   2.64 KB |        0.91 |
 *  |                   |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original | r2q1r(...)- 0 9 [68] | 4.398 us | 0.0509 us | 0.0476 us |  1.00 |    0.00 | 0.1678 |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | r2q1r(...)- 0 9 [68] | 4.234 us | 0.0457 us | 0.0427 us |  0.96 |    0.01 | 0.1526 |   2.84 KB |        0.92 |
 *  |                   |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 4.225 us | 0.0416 us | 0.0369 us |  1.00 |    0.00 | 0.1602 |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 4.180 us | 0.0653 us | 0.0610 us |  0.99 |    0.02 | 0.1450 |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 4.145 us | 0.0532 us | 0.0497 us |  1.00 |    0.00 | 0.1602 |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 4.069 us | 0.0577 us | 0.0540 us |  0.98 |    0.02 | 0.1450 |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)6 0 1 [67] | 4.138 us | 0.0531 us | 0.0497 us |  1.00 |    0.00 | 0.1602 |      3 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)6 0 1 [67] | 4.036 us | 0.0347 us | 0.0325 us |  0.98 |    0.01 | 0.1450 |   2.73 KB |        0.91 |
 *  |                   |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)- 0 1 [56] | 3.234 us | 0.0249 us | 0.0233 us |  1.00 |    0.00 | 0.1450 |    2.7 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)- 0 1 [56] | 2.900 us | 0.0244 us | 0.0203 us |  0.90 |    0.01 | 0.1335 |   2.44 KB |        0.90 |
 *  |                   |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original | rq2k2(...)- 0 1 [71] | 4.619 us | 0.0578 us | 0.0541 us |  1.00 |    0.00 | 0.1678 |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | rq2k2(...)- 0 1 [71] | 4.428 us | 0.0667 us | 0.0624 us |  0.96 |    0.01 | 0.1526 |   2.84 KB |        0.92 |
 *
 *  BenchmarkDotNet v0.13.8, macOS Monterey 12.6.8 (21G725) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *
 *
 *  | Method            | fen                  | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |------------------ |--------------------- |---------:|----------:|----------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original | 8/k7/(...)- 0 1 [39] | 3.159 us | 0.0514 us | 0.0401 us | 3.152 us |  1.00 |    0.00 | 0.4692 |      - |   2.89 KB |        1.00 |
 *  | ParseFEN_Current  | 8/k7/(...)- 0 1 [39] | 2.909 us | 0.0357 us | 0.0279 us | 2.902 us |  0.92 |    0.01 | 0.4311 |      - |   2.64 KB |        0.91 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | r2q1r(...)- 0 9 [68] | 3.799 us | 0.0560 us | 0.0468 us | 3.806 us |  1.00 |    0.00 | 0.5035 | 0.0038 |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | r2q1r(...)- 0 9 [68] | 4.017 us | 0.0774 us | 0.1527 us | 3.984 us |  1.06 |    0.03 | 0.4578 |      - |   2.84 KB |        0.92 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 4.536 us | 0.1328 us | 0.3894 us | 4.419 us |  1.00 |    0.00 | 0.4883 |      - |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 4.226 us | 0.1678 us | 0.4946 us | 4.381 us |  0.94 |    0.12 | 0.4425 |      - |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 4.442 us | 0.1151 us | 0.3393 us | 4.448 us |  1.00 |    0.00 | 0.4883 |      - |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 4.095 us | 0.1417 us | 0.4020 us | 3.977 us |  0.92 |    0.10 | 0.4425 |      - |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)6 0 1 [67] | 3.449 us | 0.0976 us | 0.2815 us | 3.349 us |  1.00 |    0.00 | 0.4883 |      - |      3 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)6 0 1 [67] | 3.565 us | 0.0983 us | 0.2805 us | 3.514 us |  1.04 |    0.11 | 0.4463 |      - |   2.74 KB |        0.91 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)- 0 1 [56] | 2.571 us | 0.0508 us | 0.0978 us | 2.550 us |  1.00 |    0.00 | 0.4387 |      - |    2.7 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)- 0 1 [56] | 2.892 us | 0.0535 us | 0.0549 us | 2.895 us |  1.12 |    0.04 | 0.3967 |      - |   2.44 KB |        0.90 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | rq2k2(...)- 0 1 [71] | 4.324 us | 0.0791 us | 0.0661 us | 4.318 us |  1.00 |    0.00 | 0.5035 |      - |    3.1 KB |        1.00 |
 *  | ParseFEN_Current  | rq2k2(...)- 0 1 [71] | 3.931 us | 0.0783 us | 0.1072 us | 3.934 us |  0.91 |    0.03 | 0.4578 |      - |   2.84 KB |        0.92 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Text.RegularExpressions;

using ParseResult = (bool Success, ulong[] PieceBitBoards, ulong[] OccupancyBitBoards, Lynx.Model.Side Side, byte Castle, Lynx.Model.BoardSquare EnPassant,
            int HalfMoveClock, int FullMoveCounter);

namespace Lynx.Benchmark;
public partial class ParseFENBenchmark : BaseBenchmark
{
    public static IEnumerable<string> Data => new[] {
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.CmkTestPositionFEN,
        Constants.ComplexPositionFEN,
        Constants.KillerTestPositionFEN,
        Constants.TTPositionFEN
    };

    [Benchmark(Baseline =true)]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Original(string fen) => ParseFEN_FENParser_Original.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Current(string fen) => ParseFEN_FENParser_Improved.ParseFEN(fen);

    public static partial class ParseFEN_FENParser_Original
    {
        [GeneratedRegex("(?<=^|\\/)[P|N|B|R|Q|K|p|n|b|r|q|k|\\d]{1,8}", RegexOptions.Compiled)]
        private static partial Regex RanksRegex();

        private static readonly Regex _ranksRegex = RanksRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ParseResult ParseFEN(string fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12] {
                default, default, default, default,
                default, default, default, default,
                default, default, default, default};

            var occupancyBitBoards = new BitBoard[3] { default, default, default };

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                MatchCollection matches;
                (matches, success) = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedString = fen[(matches[^1].Index + matches[^1].Length)..];
                var parts = unparsedString.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(parts[0]);

                castle = ParseCastlingRights(parts[1]);

                (enPassant, success) = ParseEnPassant(parts[2], pieceBitBoards, side);

                if (parts.Length < 4 || !int.TryParse(parts[3], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (parts.Length < 5 || !int.TryParse(parts[4], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen);
                _logger.Error(e.Message);
                success = false;
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        private static (MatchCollection Matches, bool Success) ParseBoard(string fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;

            var rankIndex = 0;
            var matches = _ranksRegex.Matches(fen);

            if (matches.Count != 8)
            {
                return (matches, false);
            }

            foreach (var match in matches)
            {
                var fileIndex = 0;
                foreach (var ch in ((Group)match).Value)
                {
                    if (Constants.PiecesByChar.TryGetValue(ch, out Piece piece))
                    {
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else if (int.TryParse($"{ch}", out int emptySquares))
                    {
                        fileIndex += emptySquares;
                    }
                    else
                    {
                        _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, ((Group)match).Value);
                        success = false;
                        break;
                    }
                }

                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                ++rankIndex;
            }

            return (matches, success);

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                var limit = (int)Piece.K;
                for (int piece = (int)Piece.P; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                }

                limit = (int)Piece.k;
                for (int piece = (int)Piece.p; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
            }
        }

        private static Side ParseSide(string sideString)
        {
#pragma warning disable S3358 // Ternary operators should not be nested
            bool isWhite = sideString.Equals("w", StringComparison.OrdinalIgnoreCase);

            return isWhite || sideString.Equals("b", StringComparison.OrdinalIgnoreCase)
                ? isWhite ? Side.White : Side.Black
                : throw new($"Unrecognized side: {sideString}");
#pragma warning restore S3358 // Ternary operators should not be nested
        }

        private static byte ParseCastlingRights(string castleString)
        {
            byte castle = 0;

            foreach (var ch in castleString)
            {
                castle |= ch switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {ch}")
                };
            }

            return castle;
        }

        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(string enPassantString, BitBoard[] PieceBitBoards, Side side)
        {
            bool success = true;
            BoardSquare enPassant = BoardSquare.noSquare;

            if (Enum.TryParse(enPassantString, ignoreCase: true, out BoardSquare result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant >> 3);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    _logger.Error("Invalid en passant square: {0}", enPassantString);
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
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantString, side, pawnSquare);
                }
            }
            else if (enPassantString != "-")
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantString);
            }

            return (enPassant, success);
        }
    }

    public static partial class ParseFEN_FENParser_Improved
    {
        [GeneratedRegex("(?<=^|\\/)[P|N|B|R|Q|K|p|n|b|r|q|k|\\d]{1,8}", RegexOptions.Compiled)]
        private static partial Regex RanksRegex();

        private static readonly Regex _ranksRegex = RanksRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static (bool Success, BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, byte Castle, BoardSquare EnPassant,
            int HalfMoveClock, int FullMoveCounter) ParseFEN(string fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12] {
                default, default, default, default,
                default, default, default, default,
                default, default, default, default};

            var occupancyBitBoards = new BitBoard[3] { default, default, default };

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                MatchCollection matches;
                (matches, success) = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedString = fen[(matches[^1].Index + matches[^1].Length)..].AsSpan();
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedString.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedString, parts[0]);

                castle = ParseCastlingRights(unparsedString, parts[1]);

                (enPassant, success) = ParseEnPassant(unparsedString, parts[2], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedString[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (partsLength < 5 || !int.TryParse(unparsedString[parts[4]], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen);
                _logger.Error(e.Message);
                success = false;
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        private static (MatchCollection Matches, bool Success) ParseBoard(string fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;

            var rankIndex = 0;
            var matches = _ranksRegex.Matches(fen);

            if (matches.Count != 8)
            {
                return (matches, false);
            }

            foreach (var match in matches)
            {
                var fileIndex = 0;
                foreach (var ch in ((Group)match).Value)
                {
                    if (Constants.PiecesByChar.TryGetValue(ch, out Piece piece))
                    {
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else if (int.TryParse($"{ch}", out int emptySquares))
                    {
                        fileIndex += emptySquares;
                    }
                    else
                    {
                        _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, ((Group)match).Value);
                        success = false;
                        break;
                    }
                }

                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                ++rankIndex;
            }

            return (matches, success);

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                var limit = (int)Piece.K;
                for (int piece = (int)Piece.P; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                }

                limit = (int)Piece.k;
                for (int piece = (int)Piece.p; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
            }
        }

        private static Side ParseSide(ReadOnlySpan<char> unparsedString, Range sideRange)
        {
            var sidePart = unparsedString[sideRange];
#pragma warning disable S3358 // Ternary operators should not be nested
            bool isWhite = sidePart[0].Equals('w');

            return isWhite || sidePart[0].Equals('b')
                ? isWhite ? Side.White : Side.Black
                : throw new($"Unrecognized side: {sidePart}");
#pragma warning restore S3358 // Ternary operators should not be nested
        }

        private static byte ParseCastlingRights(ReadOnlySpan<char> unparsedString, Range castleRange)
        {
            byte castle = 0;

            foreach (var ch in unparsedString[castleRange])
            {
                castle |= ch switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {ch}")
                };
            }

            return castle;
        }

        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> unparsedString, Range enPassantRange, BitBoard[] PieceBitBoards, Side side)
        {
            var enPassantPart = unparsedString[enPassantRange];
            bool success = true;
            BoardSquare enPassant = BoardSquare.noSquare;

            if (Enum.TryParse(enPassantPart, ignoreCase: true, out BoardSquare result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant >> 3);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    _logger.Error("Invalid en passant square: {0}", enPassantPart.ToString());
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
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantPart.ToString(), side, pawnSquare);
                }
            }
            else if (enPassantPart[0] != '-')
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantPart.ToString());
            }

            return (enPassant, success);
        }
    }
}
