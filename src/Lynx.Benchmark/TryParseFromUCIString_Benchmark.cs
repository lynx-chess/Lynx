/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method | positionCommand      | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |------- |--------------------- |-----------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Array  | position startpos    |   2.455 us | 0.0400 us | 0.0354 us |  1.00 |    0.00 | 0.3204 | 0.0267 |   26.4 KB |        1.00 |
 *  | Span   | position startpos    |   2.370 us | 0.0387 us | 0.0362 us |  0.97 |    0.02 | 0.3204 | 0.0267 |   26.4 KB |        1.00 |
 *  | Memory | position startpos    |   2.562 us | 0.0312 us | 0.0291 us |  1.04 |    0.02 | 0.3357 | 0.0267 |  27.45 KB |        1.04 |
 *  |        |                      |            |           |           |       |         |        |        |           |             |
 *  | Array  | posi(...)b7b6 [193]  |  13.653 us | 0.0590 us | 0.0552 us |  1.00 |    0.00 | 0.4425 | 0.0305 |  37.23 KB |        1.00 |
 *  | Span   | posi(...)b7b6 [193]  |  10.376 us | 0.0397 us | 0.0352 us |  0.76 |    0.00 | 0.3204 | 0.0153 |   26.4 KB |        0.71 |
 *  | Memory | posi(...)b7b6 [193]  |  11.528 us | 0.0307 us | 0.0288 us |  0.84 |    0.00 | 0.3357 | 0.0153 |  27.45 KB |        0.74 |
 *  |        |                      |            |           |           |       |         |        |        |           |             |
 *  | Array  | posi(...)f3g3 [353]  |  22.541 us | 0.1237 us | 0.1157 us |  1.00 |    0.00 | 0.5493 | 0.0305 |  46.48 KB |        1.00 |
 *  | Span   | posi(...)f3g3 [353]  |  15.777 us | 0.1128 us | 0.1000 us |  0.70 |    0.01 | 0.3052 |      - |   26.4 KB |        0.57 |
 *  | Memory | posi(...)f3g3 [353]  |  15.823 us | 0.0742 us | 0.0658 us |  0.70 |    0.00 | 0.3357 |      - |  27.45 KB |        0.59 |
 *  |        |                      |            |           |           |       |         |        |        |           |             |
 *  | Array  | posi(...)h3f1 [2984] | 141.219 us | 0.7872 us | 0.7364 us |  1.00 |    0.00 | 1.9531 |      - | 174.64 KB |        1.00 |
 *  | Span   | posi(...)h3f1 [2984] |  87.617 us | 0.3918 us | 0.3664 us |  0.62 |    0.00 | 0.2441 |      - |  26.42 KB |        0.15 |
 *  | Memory | posi(...)h3f1 [2984] |  85.608 us | 0.4678 us | 0.4376 us |  0.61 |    0.00 | 0.2441 |      - |  27.47 KB |        0.16 |
 *  |        |                      |            |           |           |       |         |        |        |           |             |
 *  | Array  | posi(...)g4g8 [979]  |  50.867 us | 0.3176 us | 0.2971 us |  1.00 |    0.00 | 0.9155 | 0.0610 |  79.39 KB |        1.00 |
 *  | Span   | posi(...)g4g8 [979]  |  33.572 us | 0.2398 us | 0.2126 us |  0.66 |    0.01 | 0.3052 |      - |  26.42 KB |        0.33 |
 *  | Memory | posi(...)g4g8 [979]  |  34.014 us | 0.0978 us | 0.0816 us |  0.67 |    0.00 | 0.3052 |      - |  27.47 KB |        0.35 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method | positionCommand      | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |------- |--------------------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | Array  | position startpos    |   1.507 us | 0.0294 us | 0.0382 us |  1.00 |    0.00 |  1.6136 | 0.1450 |   26.4 KB |        1.00 |
 *  | Span   | position startpos    |   1.498 us | 0.0267 us | 0.0297 us |  1.00 |    0.03 |  1.6136 | 0.1450 |   26.4 KB |        1.00 |
 *  | Memory | position startpos    |   1.485 us | 0.0216 us | 0.0202 us |  0.99 |    0.03 |  1.6804 | 0.1526 |  27.45 KB |        1.04 |
 *  |        |                      |            |           |           |       |         |         |        |           |             |
 *  | Array  | posi(...)b7b6 [193]  |  11.474 us | 0.1085 us | 0.0906 us |  1.00 |    0.00 |  2.2736 | 0.2136 |  37.23 KB |        1.00 |
 *  | Span   | posi(...)b7b6 [193]  |   9.287 us | 0.1016 us | 0.0848 us |  0.81 |    0.01 |  1.6022 | 0.1373 |   26.4 KB |        0.71 |
 *  | Memory | posi(...)b7b6 [193]  |   9.224 us | 0.0622 us | 0.0582 us |  0.80 |    0.01 |  1.6785 | 0.1526 |  27.45 KB |        0.74 |
 *  |        |                      |            |           |           |       |         |         |        |           |             |
 *  | Array  | posi(...)f3g3 [353]  |  18.725 us | 0.1280 us | 0.1134 us |  1.00 |    0.00 |  2.8381 | 0.2441 |  46.48 KB |        1.00 |
 *  | Span   | posi(...)f3g3 [353]  |  13.885 us | 0.1101 us | 0.1030 us |  0.74 |    0.01 |  1.6022 | 0.1373 |   26.4 KB |        0.57 |
 *  | Memory | posi(...)f3g3 [353]  |  14.013 us | 0.0392 us | 0.0367 us |  0.75 |    0.01 |  1.6785 | 0.1526 |  27.45 KB |        0.59 |
 *  |        |                      |            |           |           |       |         |         |        |           |             |
 *  | Array  | posi(...)h3f1 [2984] | 122.097 us | 0.4894 us | 0.4578 us |  1.00 |    0.00 | 10.6201 | 0.9766 | 174.67 KB |        1.00 |
 *  | Span   | posi(...)h3f1 [2984] |  79.757 us | 0.2419 us | 0.2145 us |  0.65 |    0.00 |  1.5869 | 0.1221 |  26.43 KB |        0.15 |
 *  | Memory | posi(...)h3f1 [2984] |  78.569 us | 0.3416 us | 0.3196 us |  0.64 |    0.00 |  1.5869 | 0.1221 |  27.47 KB |        0.16 |
 *  |        |                      |            |           |           |       |         |         |        |           |             |
 *  | Array  | posi(...)g4g8 [979]  |  46.771 us | 0.2859 us | 0.2388 us |  1.00 |    0.00 |  4.8218 | 0.4272 |  79.41 KB |        1.00 |
 *  | Span   | posi(...)g4g8 [979]  |  32.601 us | 0.1414 us | 0.1181 us |  0.70 |    0.00 |  1.5869 | 0.1221 |  26.43 KB |        0.33 |
 *  | Memory | posi(...)g4g8 [979]  |  33.635 us | 0.1927 us | 0.1609 us |  0.72 |    0.00 |  1.6479 | 0.1221 |  27.47 KB |        0.35 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method | positionCommand      | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |------- |--------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | Array  | position startpos    |   3.264 us | 0.1672 us | 0.4929 us |   3.240 us |  1.00 |    0.00 |  4.2992 | 0.2251 |  26.41 KB |        1.00 |
 *  | Span   | position startpos    |   3.554 us | 0.3368 us | 0.9717 us |   3.288 us |  1.12 |    0.35 |  4.2877 | 0.1984 |  26.41 KB |        1.00 |
 *  | Memory | position startpos    |   4.048 us | 0.2852 us | 0.8363 us |   3.959 us |  1.27 |    0.34 |  4.4708 | 0.2594 |  27.46 KB |        1.04 |
 *  |        |                      |            |           |           |            |       |         |         |        |           |             |
 *  | Array  | posi(...)b7b6 [193]  |  20.277 us | 0.5705 us | 1.6277 us |  20.053 us |  1.00 |    0.00 |  6.0425 | 0.4578 |  37.24 KB |        1.00 |
 *  | Span   | posi(...)b7b6 [193]  |  14.177 us | 0.4286 us | 1.2019 us |  13.935 us |  0.70 |    0.09 |  4.2725 | 0.3662 |  26.41 KB |        0.71 |
 *  | Memory | posi(...)b7b6 [193]  |  14.574 us | 0.3810 us | 1.0930 us |  14.334 us |  0.72 |    0.08 |  4.4708 | 0.3357 |  27.46 KB |        0.74 |
 *  |        |                      |            |           |           |            |       |         |         |        |           |             |
 *  | Array  | posi(...)f3g3 [353]  |  36.367 us | 1.4771 us | 4.2618 us |  36.469 us |  1.00 |    0.00 |  7.5684 | 0.6714 |   46.5 KB |        1.00 |
 *  | Span   | posi(...)f3g3 [353]  |  21.418 us | 0.7132 us | 1.9523 us |  20.880 us |  0.60 |    0.09 |  4.2725 | 0.3662 |  26.41 KB |        0.57 |
 *  | Memory | posi(...)f3g3 [353]  |  20.732 us | 0.6461 us | 1.8643 us |  20.500 us |  0.58 |    0.10 |  4.4556 | 0.3357 |  27.46 KB |        0.59 |
 *  |        |                      |            |           |           |            |       |         |         |        |           |             |
 *  | Array  | posi(...)h3f1 [2984] | 209.581 us | 4.1877 us | 4.6546 us | 209.782 us |  1.00 |    0.00 | 28.3203 | 2.4414 | 174.74 KB |        1.00 |
 *  | Span   | posi(...)h3f1 [2984] | 109.983 us | 2.0527 us | 3.8555 us | 108.617 us |  0.54 |    0.02 |  4.1504 | 0.2441 |  26.44 KB |        0.15 |
 *  | Memory | posi(...)h3f1 [2984] | 110.365 us | 1.9742 us | 2.8313 us | 109.601 us |  0.53 |    0.02 |  4.3945 | 0.2441 |  27.49 KB |        0.16 |
 *  |        |                      |            |           |           |            |       |         |         |        |           |             |
 *  | Array  | posi(...)g4g8 [979]  |  71.956 us | 1.3943 us | 1.8614 us |  71.579 us |  1.00 |    0.00 | 12.9395 | 0.8545 |  79.44 KB |        1.00 |
 *  | Span   | posi(...)g4g8 [979]  |  40.538 us | 0.7434 us | 1.5843 us |  40.107 us |  0.58 |    0.03 |  4.2725 | 0.3052 |  26.44 KB |        0.33 |
 *  | Memory | posi(...)g4g8 [979]  |  42.430 us | 0.8433 us | 1.3855 us |  42.319 us |  0.59 |    0.03 |  4.4556 | 0.3052 |  27.49 KB |        0.35 |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

/// <summary>
/// Extension of <see cref="ParseFENBenchmark_Benchmark"/>, but other (previous) stuff is tested there
/// </summary>
public class TryParseFromUCIString_Benchmark : BaseBenchmark
{
    private static readonly Move[] _movePool = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

    public static IEnumerable<string> Data =>
    [
        "position startpos",    // No moves
        "position startpos moves d2d4 d7d5 b1c3 g8f6 f2f3 c7c5 e2e3 b8c6 a2a3 e7e6 f1e2 c8d7 d4c5 f8c5 h2h3 f6h5 h1h2 d8h4 e1d2 h4f2 c3d5 e6d5 d2c3 c5e3 c1d2 c6a5 a3a4 e3d4 c3b4 d4c5 b4a5 a7a6 a1a2 b7b6",    // 17 moves
        "position startpos moves d2d4 d7d5 g1f3 g8f6 e2e3 b8c6 f1e2 c8f5 b1c3 f6e4 e1g1 e4c3 b2c3 e7e6 a1b1 a8b8 c1b2 b7b5 f3e5 c6e5 d4e5 f8c5 e2f3 e8g8 f1e1 d8h4 g2g4 f5g6 b1c1 f7f6 e5f6 h4f6 g1g2 g6e4 f3e4 f6f2 g2h1 d5e4 d1e2 f2f3 e2f3 e4f3 e3e4 f3f2 e1f1 f8f4 c1d1 f4e4 g4g5 e4g4 b2c1 b8f8 a2a3 e6e5 h2h3 g4g3 c1b2 f8f3 d1d8 g8f7 d8d7 f7e6 d7c7 g3h3 h1g2 f3g3",    // 36 moves
        "position startpos moves g1f3 e7e6 e2e4 b7b6 d2d4 g8f6 e4e5 f6d5 c2c4 f8b4 b1d2 d5e7 a2a3 b4d2 c1d2 d7d6 d2c3 d6e5 f3e5 c8b7 f1e2 e8g8 e2f3 b7f3 d1f3 b8d7 a1d1 f7f6 e5c6 e7c6 f3c6 f8e8 e1g1 d7f8 f1e1 d8d7 c6f3 f8g6 f3b7 a7a5 c4c5 a8b8 b7e4 d7b5 d4d5 e6e5 d5d6 b5c5 d6d7 e8d8 e1e3 g6f4 g2g3 f4h3 g1g2 h3g5 e4a4 g5f7 e3d3 f7d6 d3d5 c5c4 a4c4 d6c4 b2b4 a5a4 d1c1 c4d6 c3e5 d8d7 e5d6 d7d6 d5d6 c7d6 c1c6 d6d5 c6d6 b8c8 d6d5 c8c3 b4b5 c3b3 d5d4 b3a3 d4d6 a3a1 d6b6 a4a3 b6b8 g8f7 b5b6 a1b1 b8a8 b1b6 a8a3 b6b8 a3a7 f7g8 h2h4 b8e8 g3g4 e8d8 g2f3 d8d3 f3e4 d3d8 e4f4 d8d4 f4g3 d4d8 h4h5 d8f8 g3f4 f8b8 f4f3 b8b3 f3g2 b3b8 a7c7 b8a8 g2f3 a8a3 f3f4 a3a4 f4g3 a4a8 c7b7 a8f8 g3f4 f8c8 f4e4 c8c4 e4f3 c4c3 f3g2 c3c8 f2f4 c8c2 g2f3 c2c3 f3e4 c3c4 e4f5 c4c5 f5e6 c5c6 e6d5 c6c8 h5h6 g7h6 d5e6 c8c4 e6f5 c4c6 b7a7 c6d6 a7c7 d6d4 c7b7 d4a4 b7e7 a4a6 e7e6 a6e6 f5e6 g8g7 f4f5 h6h5 g4h5 h7h6 e6e7 g7g8 e7f6 g8f8 f6e6 f8g7 e6e7 g7h8 f5f6 h8g8 f6f7 g8h8 f7f8Q h8h7 f8f4 h7g8 f4g4 g8h8 e7f8 h8h7 g4g8",  // 96 movws
        Constants.LongPositionCommand, // 296 moves
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public TryParseFromUCIString_Benchmark_Game Array(string positionCommand)
    {
        return ParseGame(positionCommand, _movePool);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Game Span(string positionCommand)
    {
        var game = new Game(Constants.InitialPositionFEN);
        game.ParsePositionCommand(positionCommand);

        return game;
    }

    private static TryParseFromUCIString_Benchmark_Game ParseGame(ReadOnlySpan<char> positionCommandSpan, Move[] movePool)
    {
        try
        {
            // We divide the position command in these two sections:
            // "position startpos                       ||"
            // "position startpos                       || moves e2e4 e7e5"
            // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  ||"
            // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  || moves e2e4 e7e5"
            Span<Range> items = stackalloc Range[2];
            positionCommandSpan.Split(items, "moves", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var initialPositionSection = positionCommandSpan[items[0]];

            // We divide in these two parts
            // "position startpos ||"       <-- If "fen" doesn't exist in the section
            // "position || (fen) 8/8/8/8/8/8/8/8 w - - 0 1"  <-- If "fen" does exist
            Span<Range> initialPositionParts = stackalloc Range[2];
            initialPositionSection.Split(initialPositionParts, "fen", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            ReadOnlySpan<char> fen = initialPositionSection[initialPositionParts[0]].Length == PositionCommand.Id.Length   // "position" o "position startpos"
                ? initialPositionSection[initialPositionParts[1]]
                : Constants.InitialPositionFEN.AsSpan();

            var movesSection = positionCommandSpan[items[1]];

            Span<Range> moves = stackalloc Range[(movesSection.Length / 5) + 1]; // Number of potential half-moves provided in the string
            movesSection.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return new TryParseFromUCIString_Benchmark_Game(fen, movesSection, moves, movePool);
        }
        catch (Exception)
        {
#pragma warning disable S112 // General or reserved exceptions should never be thrown
            throw new($"Error parsing position command '{positionCommandSpan}'");
#pragma warning restore S112 // General or reserved exceptions should never be thrown
        }
    }

    public sealed class TryParseFromUCIString_Benchmark_Game
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#if DEBUG
        public List<Move> MoveHistory { get; }
#endif

        public HashSet<ulong> PositionHashHistory { get; }

        public int HalfMovesWithoutCaptureOrPawnMove { get; set; }

#pragma warning disable RCS1169, S2933, S4487, IDE0044, IDE0052, RCS1170 // Readonly, not used
        public Position CurrentPosition { get; private set; }

        private Position _gameInitialPosition;
#pragma warning restore RCS1169, S2933, S4487, IDE0044, IDE0052, RCS1170 // Readonly, not used

        public TryParseFromUCIString_Benchmark_Game(ReadOnlySpan<char> fen)
        {
            CurrentPosition = new Position(Constants.InitialPositionFEN);
            var parsedFen = FENParser.ParseFEN(fen);
            CurrentPosition.PopulateFrom(parsedFen);

            if (!CurrentPosition.IsValid())
            {
                _logger.Warn($"Invalid position detected: {fen}");
            }

            PositionHashHistory = new(1024) { CurrentPosition.UniqueIdentifier };
            HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;
            _gameInitialPosition = new Position(CurrentPosition);
#if DEBUG
            MoveHistory = new(1024);
#endif
        }

        public TryParseFromUCIString_Benchmark_Game(ReadOnlySpan<char> fen, ReadOnlySpan<char> rawMoves, Span<Range> rangeSpan, Move[] movePool) : this(fen)
        {
            Span<BitBoard> attacks = stackalloc BitBoard[12];
            Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
            var evaluationContext = new EvaluationContext(attacks, attacksBySide);

            for (int i = 0; i < rangeSpan.Length; ++i)
            {
                if (rangeSpan[i].Start.Equals(rangeSpan[i].End))
                {
                    break;
                }
                var moveString = rawMoves[rangeSpan[i]];

                var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition, ref evaluationContext, movePool);

                if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
                {
                    _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen.ToString(), rawMoves.ToString(), moveString.ToString());
                    break;
                }

                MakeMove(parsedMove.Value);
            }

            _gameInitialPosition = new Position(CurrentPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MakeMove(Move moveToPlay)
        {
            CurrentPosition.MakeMove(moveToPlay);

            if (CurrentPosition.WasProduceByAValidMove())
            {
#if DEBUG
                MoveHistory.Add(moveToPlay);
#endif
            }
            else
            {
                _logger.Warn("Error trying to play {0}", moveToPlay.UCIString());
                CurrentPosition.UnmakeMove(moveToPlay);
            }

            PositionHashHistory.Add(CurrentPosition.UniqueIdentifier);
            HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveToPlay, HalfMovesWithoutCaptureOrPawnMove);
        }
    }
}
