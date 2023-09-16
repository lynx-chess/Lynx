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
 *  | Method            | fen                  | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |------------------ |--------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 *  | ParseFEN_Original | 8/k7/(...)- 0 1 [39] | 3.213 us | 0.0239 us | 0.0212 us |  1.00 | 0.1564 |   2.89 KB |        1.00 |
 *  | ParseFEN_Current  | 8/k7/(...)- 0 1 [39] | 2.954 us | 0.0121 us | 0.0113 us |  0.92 | 0.1411 |   2.64 KB |        0.91 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | r2q1r(...)- 0 9 [68] | 3.958 us | 0.0159 us | 0.0141 us |  1.00 | 0.1678 |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | r2q1r(...)- 0 9 [68] | 3.768 us | 0.0168 us | 0.0149 us |  0.95 | 0.1526 |   2.84 KB |        0.92 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 3.725 us | 0.0107 us | 0.0100 us |  1.00 | 0.1640 |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 3.681 us | 0.0186 us | 0.0165 us |  0.99 | 0.1488 |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 3.722 us | 0.0224 us | 0.0210 us |  1.00 | 0.1640 |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 3.586 us | 0.0226 us | 0.0212 us |  0.96 | 0.1488 |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)6 0 1 [67] | 3.722 us | 0.0205 us | 0.0192 us |  1.00 | 0.1640 |      3 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)6 0 1 [67] | 3.529 us | 0.0126 us | 0.0118 us |  0.95 | 0.1488 |   2.73 KB |        0.91 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)- 0 1 [56] | 2.771 us | 0.0131 us | 0.0116 us |  1.00 | 0.1450 |    2.7 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)- 0 1 [56] | 2.506 us | 0.0268 us | 0.0251 us |  0.90 | 0.1335 |   2.44 KB |        0.90 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | rq2k2(...)- 0 1 [71] | 4.214 us | 0.0269 us | 0.0252 us |  1.00 | 0.1678 |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | rq2k2(...)- 0 1 [71] | 3.852 us | 0.0158 us | 0.0140 us |  0.91 | 0.1526 |   2.84 KB |        0.92 |
 *
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method            | fen                  | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |------------------ |--------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 *  | ParseFEN_Original | 8/k7/(...)- 0 1 [39] | 2.973 us | 0.0133 us | 0.0118 us |  1.00 | 0.1564 |   2.89 KB |        1.00 |
 *  | ParseFEN_Current  | 8/k7/(...)- 0 1 [39] | 2.793 us | 0.0095 us | 0.0088 us |  0.94 | 0.1411 |   2.64 KB |        0.91 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | r2q1r(...)- 0 9 [68] | 3.772 us | 0.0103 us | 0.0086 us |  1.00 | 0.1678 |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | r2q1r(...)- 0 9 [68] | 3.597 us | 0.0079 us | 0.0070 us |  0.95 | 0.1526 |   2.84 KB |        0.92 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 3.502 us | 0.0114 us | 0.0101 us |  1.00 | 0.1640 |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 3.468 us | 0.0127 us | 0.0118 us |  0.99 | 0.1488 |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 3.449 us | 0.0090 us | 0.0084 us |  1.00 | 0.1640 |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 3.349 us | 0.0135 us | 0.0126 us |  0.97 | 0.1488 |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)6 0 1 [67] | 3.460 us | 0.0135 us | 0.0126 us |  1.00 | 0.1640 |      3 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)6 0 1 [67] | 3.244 us | 0.0120 us | 0.0112 us |  0.94 | 0.1488 |   2.73 KB |        0.91 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)- 0 1 [56] | 2.666 us | 0.0093 us | 0.0087 us |  1.00 | 0.1450 |    2.7 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)- 0 1 [56] | 2.415 us | 0.0066 us | 0.0062 us |  0.91 | 0.1335 |   2.44 KB |        0.90 |
 *  |                   |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original | rq2k2(...)- 0 1 [71] | 3.705 us | 0.0107 us | 0.0100 us |  1.00 | 0.1678 |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | rq2k2(...)- 0 1 [71] | 3.629 us | 0.0099 us | 0.0093 us |  0.98 | 0.1526 |   2.84 KB |        0.92 |
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
 *  | ParseFEN_Original | 8/k7/(...)- 0 1 [39] | 2.961 us | 0.0187 us | 0.0146 us | 2.960 us |  1.00 |    0.00 | 0.4692 |      - |   2.89 KB |        1.00 |
 *  | ParseFEN_Current  | 8/k7/(...)- 0 1 [39] | 2.835 us | 0.0082 us | 0.0069 us | 2.836 us |  0.96 |    0.00 | 0.4311 |      - |   2.64 KB |        0.91 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | r2q1r(...)- 0 9 [68] | 3.664 us | 0.0093 us | 0.0082 us | 3.665 us |  1.00 |    0.00 | 0.5035 | 0.0038 |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | r2q1r(...)- 0 9 [68] | 4.565 us | 0.2938 us | 0.8476 us | 4.247 us |  1.05 |    0.06 | 0.4616 |      - |   2.84 KB |        0.92 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 3.787 us | 0.0612 us | 0.0543 us | 3.788 us |  1.00 |    0.00 | 0.4883 |      - |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 3.408 us | 0.0664 us | 0.0952 us | 3.398 us |  0.89 |    0.03 | 0.4463 |      - |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | r3k2r(...)- 0 1 [68] | 3.531 us | 0.0662 us | 0.0517 us | 3.529 us |  1.00 |    0.00 | 0.4883 |      - |   3.01 KB |        1.00 |
 *  | ParseFEN_Current  | r3k2r(...)- 0 1 [68] | 3.695 us | 0.0705 us | 0.0839 us | 3.697 us |  1.05 |    0.04 | 0.4463 |      - |   2.75 KB |        0.91 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)6 0 1 [67] | 3.696 us | 0.0711 us | 0.0630 us | 3.680 us |  1.00 |    0.00 | 0.4883 |      - |      3 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)6 0 1 [67] | 4.701 us | 0.0833 us | 0.0780 us | 4.725 us |  1.27 |    0.03 | 0.4463 |      - |   2.74 KB |        0.91 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | rnbqk(...)- 0 1 [56] | 2.664 us | 0.0237 us | 0.0221 us | 2.661 us |  1.00 |    0.00 | 0.4387 |      - |    2.7 KB |        1.00 |
 *  | ParseFEN_Current  | rnbqk(...)- 0 1 [56] | 2.664 us | 0.0372 us | 0.0311 us | 2.661 us |  1.00 |    0.01 | 0.3967 |      - |   2.44 KB |        0.90 |
 *  |                   |                      |          |           |           |          |       |         |        |        |           |             |
 *  | ParseFEN_Original | rq2k2(...)- 0 1 [71] | 3.847 us | 0.0350 us | 0.0292 us | 3.838 us |  1.00 |    0.00 | 0.5035 |      - |   3.09 KB |        1.00 |
 *  | ParseFEN_Current  | rq2k2(...)- 0 1 [71] | 3.399 us | 0.0445 us | 0.0394 us | 3.378 us |  0.88 |    0.01 | 0.4616 | 0.0038 |   2.84 KB |        0.92 |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;
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

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Original(string fen) => ParseFEN_FENParser_Original.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Improved1(string fen) => ParseFEN_FENParser_Improved1.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Base2(string fen) => ParseFEN_FENParser_Base2.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_NoRegex(string fen) => ParseFEN_FENParser_NoRegex.ParseFEN(fen);

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

    public static partial class ParseFEN_FENParser_Improved1
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

    public static partial class ParseFEN_FENParser_Base2
    {
        [GeneratedRegex("(?<=^|\\/)[P|N|B|R|Q|K|p|n|b|r|q|k|\\d]{1,8}", RegexOptions.Compiled)]
        private static partial Regex RanksRegex();

        private static readonly Regex _ranksRegex = RanksRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static (bool Success, BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, byte Castle, BoardSquare EnPassant,
            int HalfMoveClock, int FullMoveCounter) ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12];
            var occupancyBitBoards = new BitBoard[3];

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                MatchCollection matches;
                (matches, success) = ParseBoard(fen.ToString(), pieceBitBoards, occupancyBitBoards);

                var unparsedStringAsSpan = fen[(matches[^1].Index + matches[^1].Length)..];
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedStringAsSpan[parts[0]]);

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen.ToString());
                _logger.Error(e.Message);
                success = false;
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Side ParseSide(ReadOnlySpan<char> side)
        {
            return side[0] switch
            {
                'w' or 'W' => Side.White,
                'b' or 'B' => Side.Black,
                _ => throw new($"Unrecognized side: {side}")
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
                    _ => throw new($"Unrecognized castling char: {castling[i]}")
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

    public static class ParseFEN_FENParser_NoRegex
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static (bool Success, BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, byte Castle, BoardSquare EnPassant,
            int HalfMoveClock, int FullMoveCounter) ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12];
            var occupancyBitBoards = new BitBoard[3];

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                success = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedStringAsSpan[parts[0]]);

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen.ToString());
                _logger.Error(e.Message);
                success = false;
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;
            var rankIndex = 0;
            var end = fen.IndexOf('/');

            while (success && end != -1)
            {
                var match = fen[..end];

                ParseBoardSection(pieceBitBoards, ref success, rankIndex, match);
                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                fen = fen[(end + 1)..];
                end = fen.IndexOf('/');
                ++rankIndex;
            }

            ParseBoardSection(pieceBitBoards, ref success, rankIndex, fen[..fen.IndexOf(' ')]);
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            return success;

            static void ParseBoardSection(ulong[] pieceBitBoards, ref bool success, int rankIndex, ReadOnlySpan<char> boardfenSection)
            {
                int fileIndex = 0;

                foreach (var ch in boardfenSection)
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
                        _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, boardfenSection.ToString());
                        success = false;
                        break;
                    }
                }
            }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Side ParseSide(ReadOnlySpan<char> side)
        {
            return side[0] switch
            {
                'w' or 'W' => Side.White,
                'b' or 'B' => Side.Black,
                _ => throw new($"Unrecognized side: {side}")
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
                    _ => throw new($"Unrecognized castling char: {castling[i]}")
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
