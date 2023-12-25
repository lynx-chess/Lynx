/*
 */

using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark;

public partial class GoCommandParsingAlternatives : BaseBenchmark
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
    public async Task Sequential(string command)
    {
        await Task.Run(() => ParseSequentially(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task Parallell(string command)
    {
        await ParseInParallel(command);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task CapturingGroups(string command)
    {
        await Task.Run(() => ParseRegexCapturingGroups(command));
    }

    [GeneratedRegex("(?<=searchmoves).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex SearchMovesRegex();

    [GeneratedRegex("(?<=wtime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex WhiteTimeRegex();

    [GeneratedRegex("(?<=btime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex BlackTimeRegex();

    [GeneratedRegex("(?<=winc).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex WhiteIncrementRegex();

    [GeneratedRegex("(?<=binc).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex BlackIncrementRegex();

    [GeneratedRegex("(?<=movestogo).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex MovesToGoRegex();

    [GeneratedRegex("(?<=depth).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex DepthRegex();

    [GeneratedRegex("(?<=nodes).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex NodesRegex();

    [GeneratedRegex("(?<=mate).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex MateRegex();

    [GeneratedRegex("(?<=movetime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex MoveTimeRegex();

    [GeneratedRegex(@"(?<wtime>(?<=wtime\s+)\d+)|(?<btime>(?<=btime\s+)\d+)|(?<winc>(?<=winc\s+)\d+)|(?<binc>(?<=binc\s+)\d+)|(?<movestogo>(?<=movestogo\s+)\d+)|(?<depth>(?<=depth\s+)\d+)|(?<movetime>(?<=movetime\s+)\d+)|(?<infinite>infinite)|(?<ponder>ponder)")]
    private static partial Regex CapturingGroups();

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

    private void ParseSequentially(string command)
    {
        var match = SearchMovesRegex().Match(command);
        SearchMoves = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        match = WhiteTimeRegex().Match(command);
        if (int.TryParse(match.Value, out var value))
        {
            WhiteTime = value;
        }

        match = BlackTimeRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            BlackTime = value;
        }

        match = WhiteIncrementRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            WhiteIncrement = value;
        }

        match = BlackIncrementRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            BlackIncrement = value;
        }

        match = MovesToGoRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            MovesToGo = value;
        }

        match = DepthRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            Depth = value;
        }

        match = NodesRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            Nodes = value;
        }

        match = MateRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            Mate = value;
        }

        match = MoveTimeRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            MoveTime = value;
        }

        Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase);

        Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase);
    }

    private async Task ParseInParallel(string command)
    {
        var taskList = new List<Task>
            {
                Task.Run(() =>
                {
                    var match = SearchMovesRegex().Match(command);

                    SearchMoves = Enumerable.ToList<string>(match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                }),
                Task.Run(() =>
                {
                    var match = WhiteTimeRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = BlackTimeRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = WhiteIncrementRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = BlackIncrementRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = MovesToGoRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MovesToGo = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = DepthRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Depth = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = NodesRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Nodes = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = MateRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Mate = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = MoveTimeRegex().Match(command);

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

    private void ParseRegexCapturingGroups(string command)
    {
        foreach (var match in CapturingGroups().Matches(command).Cast<Match>())
        {
            for (int i = 1; i < match.Groups.Count; ++i)
            {
                var group = match.Groups[i];
                if (group.Success)
                {
                    switch (group.Name)
                    {
                        case "wtime":
                            WhiteTime = int.Parse(group.Value);
                            break;
                        case "btime":
                            BlackTime = int.Parse(group.Value);
                            break;
                        case "winc":
                            WhiteIncrement = int.Parse(group.Value);
                            break;
                        case "binc":
                            BlackIncrement = int.Parse(group.Value);
                            break;
                        case "movestogo":
                            MovesToGo = int.Parse(group.Value);
                            break;
                        case "infinite":
                            Infinite = true;
                            break;
                        case "ponder":
                            Ponder = true;
                            break;
                        case "depth":
                            Depth = int.Parse(group.Value);
                            break;
                        case "movetime":
                            MoveTime = int.Parse(group.Value);
                            break;
                    }

                    break;
                }
            }
        }
    }
}
