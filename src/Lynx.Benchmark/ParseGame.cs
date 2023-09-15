/*
 * Span Split()
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark;
public partial class ParseGameBenchmark : BaseBenchmark
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
    public Game ParseGame_Original(string fen) => ParseGame_OriginalClass.ParseGame(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Game ParseGame_Current(string fen) => ParseGame_ImprovedClass.ParseGame(fen);

    public static partial class ParseGame_OriginalClass
    {
        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        [GeneratedRegex("(?<=fen).+?(?=moves|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex FenRegex();

        [GeneratedRegex("(?<=moves).+?(?=$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex MovesRegex();

        private static readonly Regex _fenRegex = FenRegex();
        private static readonly Regex _movesRegex = MovesRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Game ParseGame(string positionCommand)
        {
            try
            {
                var items = positionCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                bool isInitialPosition = string.Equals(items.ElementAtOrDefault(1), StartPositionString, StringComparison.OrdinalIgnoreCase);

                var initialPosition = isInitialPosition
                        ? Constants.InitialPositionFEN
                        : _fenRegex.Match(positionCommand).Value.Trim();

                if (string.IsNullOrEmpty(initialPosition))
                {
                    _logger.Error("Error parsing position command '{0}': no initial position found", positionCommand);
                }

                var moves = _movesRegex.Match(positionCommand).Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

                return new Game(initialPosition, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                return new Game();
            }
        }
    }

        public static partial class ParseGame_ImprovedClass
        {
        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        [GeneratedRegex("(?<=fen).+?(?=moves|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex FenRegex();

        [GeneratedRegex("(?<=moves).+?(?=$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex MovesRegex();

        private static readonly Regex _fenRegex = FenRegex();
        private static readonly Regex _movesRegex = MovesRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Game ParseGame(string positionCommand)
            {
                try
                {
                    var positionCommandSpan = positionCommand.AsSpan();
                    Span<Range> items = stackalloc Range[3];    // Leaving 'everything else' in the third one
                    positionCommandSpan.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries);
                    bool isInitialPosition = positionCommandSpan[items[1]].Equals(StartPositionString, StringComparison.OrdinalIgnoreCase);

                    var initialPosition = isInitialPosition
                            ? Constants.InitialPositionFEN
                            : _fenRegex.Match(positionCommand).Value.Trim();

                    if (string.IsNullOrEmpty(initialPosition))
                    {
                        _logger.Error("Error parsing position command '{0}': no initial position found", positionCommand);
                    }

                    Span<Range> moves = stackalloc Range[250];
                    var movesRegexResult = _movesRegex.Match(positionCommand).ValueSpan;
                    movesRegexResult.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries);

                    return new Game(initialPosition, movesRegexResult, moves);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                    return new Game();
                }
            }
        }
    }
