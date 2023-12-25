/*
 *
 *  |                  Method |              command |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
 *  |------------------------ |--------------------- |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  |        TraditionalRegex |          go infinite | 13.82 us | 0.465 us | 1.371 us | 14.32 us |  1.00 |    0.00 | 1.2817 |   2.62 KB |        1.00 |
 *  | GeneratedAttributeRegex |          go infinite | 11.02 us | 0.249 us | 0.733 us | 11.21 us |  0.81 |    0.11 | 1.1597 |   2.37 KB |        0.91 |
 *  |                         |                      |          |          |          |          |       |         |        |           |             |
 *  |        TraditionalRegex | go in(...) d2d4 [33] | 15.86 us | 0.384 us | 1.100 us | 16.12 us |  1.00 |    0.00 | 1.5106 |   3.05 KB |        1.00 |
 *  | GeneratedAttributeRegex | go in(...) d2d4 [33] | 14.59 us | 0.488 us | 1.437 us | 15.04 us |  0.93 |    0.11 | 1.3733 |   2.82 KB |        0.92 |
 *  |                         |                      |          |          |          |          |       |         |        |           |             |
 *  |        TraditionalRegex |  go i(...) 500 [137] | 26.19 us | 0.621 us | 1.820 us | 26.68 us |  1.00 |    0.00 | 2.5635 |   5.21 KB |        1.00 |
 *  | GeneratedAttributeRegex |  go i(...) 500 [137] | 22.58 us | 0.497 us | 1.467 us | 22.57 us |  0.87 |    0.10 | 2.4414 |   4.98 KB |        0.96 |
 *  |                         |                      |          |          |          |          |       |         |        |           |             |
 *  |        TraditionalRegex | go in(...)c 100 [66] | 20.35 us | 0.406 us | 0.995 us | 20.45 us |  1.00 |    0.00 | 1.8616 |   3.78 KB |        1.00 |
 *  | GeneratedAttributeRegex | go in(...)c 100 [66] | 17.66 us | 0.347 us | 0.541 us | 17.73 us |  0.86 |    0.04 | 1.7395 |   3.55 KB |        0.94 |
 *  |                         |                      |          |          |          |          |       |         |        |           |             |
 *  |        TraditionalRegex |  go i(...) 500 [117] | 24.21 us | 0.479 us | 0.731 us | 24.11 us |  1.00 |    0.00 | 2.4414 |   4.95 KB |        1.00 |
 *  | GeneratedAttributeRegex |  go i(...) 500 [117] | 20.73 us | 0.409 us | 0.560 us | 20.72 us |  0.86 |    0.04 | 2.3193 |   4.72 KB |        0.95 |
 *
 */

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute' - testing that

using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark;

public partial class GoCommandParsing_GeneratedAttribute : BaseBenchmark
{
    public static IEnumerable<string> Data => new[] {
            "go infinite",
            "go infinite searchmoves e2e4 d2d4",
            "go wtime 7000 wince 500 btime 8000 binc 500",
            "go wtime 7000 wince 500 btime 8000 binc 500 ponder",
            "go infinite searchmoves e2e4 d2d4 wtime 10000 btime 10000 winc 100",
            "go infinite searchmoves e2e4 d2d4 wtime 10000 btime 10000 winc 100 binc 100 movestogo 10 depth 50 mate 10 movetime 500",
            "go infinite searchmoves e2e4 d2d4 a2a4 a2a3 b2b4 b2b3 wtime 10000 btime 10000 winc 100 binc 100 movestogo 10 depth 50 mate 10 nodes 1000 movetime 500"
        };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public async Task TraditionalRegex(string command)
    {
        await Task.Run(() => ParseTraditionalRegex(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task GeneratedAttributeRegex(string command)
    {
        await ParseGeneratedAttributeRegex(command);
    }

    private readonly Regex _searchMovesRegex = new(
        "(?<=searchmoves).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _whiteTimeRegex = new(
        "(?<=wtime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _blackTimeRegex = new(
        "(?<=btime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _whiteIncrementRegex = new(
        "(?<=winc).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _blackIncrementRegex = new(
        "(?<=binc).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _movesToGoRegex = new(
        "(?<=movestogo).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _depthRegex = new(
        "(?<=depth).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _nodesRegex = new(
        "(?<=nodes).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _mateRegex = new(
        "(?<=mate).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _moveTimeRegex = new(
        "(?<=movetime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string GoSubcommands = "searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite";

    [GeneratedRegex(@$"(?<=searchmoves).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SearchMoveRegex();

    [GeneratedRegex(@$"(?<=wtime).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WhiteTimeRegex();

    [GeneratedRegex(@$"(?<=btime).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex BlackTimeRegex();

    [GeneratedRegex(@$"(?<=winc).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WhiteIncrementRegex();

    [GeneratedRegex(@$"(?<=binc).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex BlackIncrementRegex();

    [GeneratedRegex(@$"(?<=movestogo).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MovesToGoRegex();

    [GeneratedRegex(@$"(?<=depth).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DepthRegex();

    [GeneratedRegex(@$"(?<=nodes).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex NodesRegex();

    [GeneratedRegex(@$"(?<=mate).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MateRegex();

    [GeneratedRegex(@$"(?<=movetime).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MoveTimeRegex();

    private static readonly Regex _searchMovesRegexAttribute = SearchMoveRegex();
    private static readonly Regex _whiteTimeRegexAttribute = WhiteTimeRegex();
    private static readonly Regex _blackTimeRegexAttribute = BlackTimeRegex();
    private static readonly Regex _whiteIncrementRegexAttribute = WhiteIncrementRegex();
    private static readonly Regex _blackIncrementRegexAttribute = BlackIncrementRegex();
    private static readonly Regex _movesToGoRegexAttribute = MovesToGoRegex();
    private static readonly Regex _depthRegexAttribute = DepthRegex();
    private static readonly Regex _nodesRegexAttribute = NodesRegex();
    private static readonly Regex _mateRegexAttribute = MateRegex();
    private static readonly Regex _moveTimeRegexAttribute = MoveTimeRegex();

    public List<string> SearchMoves { get; private set; } = default!;
    public int WhiteTime { get; private set; } = default!;
    public int BlackTime { get; private set; } = default!;
    public int WhiteIncrement { get; private set; } = default!;
    public int BlackIncrement { get; private set; } = default!;
    public int MovesToGo { get; private set; } = default!;
    public int Depth { get; private set; } = default!;
    public int Nodes { get; private set; } = default!;
    public int Mate { get; private set; } = default!;
    public int MoveTime { get; private set; } = default!;
    public bool Infinite { get; private set; } = default!;
    public bool Ponder { get; private set; } = default!;

    private async Task ParseTraditionalRegex(string command)
    {
        var taskList = new List<Task>
            {
                Task.Run(() =>
                {
                    var match = _searchMovesRegex.Match(command);

                    SearchMoves = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                }),
                Task.Run(() =>
                {
                    var match = _whiteTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _blackTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _whiteIncrementRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _blackIncrementRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _movesToGoRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MovesToGo = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _depthRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Depth = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _nodesRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Nodes = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _mateRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Mate = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _moveTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MoveTime = value;
                    }
                }),
                Task.Run(() => Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase)),
                Task.Run(() => Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase))
            };

        await Task.WhenAll(taskList);
    }

    private async Task ParseGeneratedAttributeRegex(string command)
    {
        var taskList = new List<Task>
            {
                Task.Run(() =>
                {
                    var match = _searchMovesRegexAttribute.Match(command);

                    SearchMoves = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                }),
                Task.Run(() =>
                {
                    var match = _whiteTimeRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _blackTimeRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _whiteIncrementRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _blackIncrementRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _movesToGoRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MovesToGo = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _depthRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Depth = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _nodesRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Nodes = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _mateRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Mate = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _moveTimeRegexAttribute.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MoveTime = value;
                    }
                }),
                Task.Run(() => Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase)),
                Task.Run(() => Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase))
            };

        await Task.WhenAll(taskList);
    }
}

#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
