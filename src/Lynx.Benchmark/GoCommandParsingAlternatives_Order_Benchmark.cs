using BenchmarkDotNet.Attributes;
using NLog;

namespace Lynx.Benchmark;

/// <summary>
/// Implementation chosen from <see cref="GoCommandParsingAlternatives_Benchmark"/>
/// </summary>
public class GoCommandParsingAlternatives_Order_Benchmark : BaseBenchmark
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static IEnumerable<string> Data =>
    [
        "go wtime 7000 winc 500 btime 8000 binc 500",
        "go ponder wtime 7000 winc 500 btime 8000 binc 500",
        "go wtime 7000 winc 500 btime 8000 binc 500 ponder",
        "go binc 500 winc 500 btime 8000 wtime 7000 ponder",
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public async Task Naive(string command)
    {
        await Task.Run(() => ParseNaive(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task Wtime_First(string command)
    {
        await Task.Run(() => ParseWtime_First(command));
    }

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
    private static ReadOnlySpan<char> WtimeSpan => "wtime".AsSpan();
    private static ReadOnlySpan<char> BtimeSpan => "btime".AsSpan();
    private static ReadOnlySpan<char> WincSpan => "winc".AsSpan();
    private static ReadOnlySpan<char> BincSpan => "binc".AsSpan();
    private static ReadOnlySpan<char> MovestogoSpan => "movestogo".AsSpan();
    private static ReadOnlySpan<char> MovetimeSpan => "movetime".AsSpan();
    private static ReadOnlySpan<char> DepthSpan => "depth".AsSpan();
    private static ReadOnlySpan<char> InfiniteSpan => "infinite".AsSpan();
    private static ReadOnlySpan<char> PonderSpan => "ponder".AsSpan();
    private static ReadOnlySpan<char> NodesSpan => "nodes".AsSpan();
    private static ReadOnlySpan<char> MateSpan => "mate".AsSpan();
    private static ReadOnlySpan<char> SearchmovesSpan => "searchmoves".AsSpan();

    private void ParseNaive(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];

            if (key.Equals(WtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteTime = value;
                }
            }
            else if (key.Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackTime = value;
                }
            }
            else if (key.Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteIncrement = value;
                }
            }
            else if (key.Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackIncrement = value;
                }
            }
            else if (key.Equals(MovestogoSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MovesToGo = value;
                }
            }
            else if (key.Equals(MovetimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MoveTime = value;
                }
            }
            else if (key.Equals(DepthSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    Depth = value;
                }
            }
            else if (key.Equals(InfiniteSpan, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderSpan, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;
            }
            else if (key.Equals(NodesSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchmovesSpan, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new InvalidDataException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    private void ParseWtime_First(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];

            if (key.Equals(WtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteTime = value;
                }

                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteIncrement = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackIncrement = value;
                    }
                }
            }
            else if (key.Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackTime = value;
                }
            }
            else if (key.Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteIncrement = value;
                }
            }
            else if (key.Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackIncrement = value;
                }
            }
            else if (key.Equals(MovestogoSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MovesToGo = value;
                }
            }
            else if (key.Equals(MovetimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MoveTime = value;
                }
            }
            else if (key.Equals(DepthSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    Depth = value;
                }
            }
            else if (key.Equals(InfiniteSpan, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderSpan, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;
                int value;

                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteIncrement = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackIncrement = value;
                    }
                }
            }
            else if (key.Equals(NodesSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchmovesSpan, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new InvalidDataException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }
}
