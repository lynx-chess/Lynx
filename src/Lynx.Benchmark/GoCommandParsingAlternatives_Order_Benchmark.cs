/*
 *
BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
| Method      | command              | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------ |--------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| Naive       | go bi(...)onder [49] | 923.3 ns | 14.84 ns | 13.88 ns |  1.00 |    0.02 | 0.0019 |     272 B |        1.00 |
| Wtime_First | go bi(...)onder [49] | 888.8 ns |  5.96 ns |  5.58 ns |  0.96 |    0.02 | 0.0029 |     272 B |        1.00 |
|             |                      |          |          |          |       |         |        |           |             |
| Naive       | go po(...)c 500 [49] | 892.1 ns |  9.54 ns |  8.46 ns |  1.00 |    0.01 | 0.0019 |     272 B |        1.00 |
| Wtime_First | go po(...)c 500 [49] | 907.3 ns | 11.62 ns | 10.87 ns |  1.02 |    0.02 | 0.0019 |     272 B |        1.00 |
|             |                      |          |          |          |       |         |        |           |             |
| Naive       | go wt(...)c 500 [42] | 905.7 ns | 12.89 ns | 12.06 ns |  1.00 |    0.02 | 0.0019 |     272 B |        1.00 |
| Wtime_First | go wt(...)c 500 [42] | 951.4 ns | 12.96 ns | 12.12 ns |  1.05 |    0.02 | 0.0019 |     272 B |        1.00 |
|             |                      |          |          |          |       |         |        |           |             |
| Naive       | go wt(...)onder [49] | 916.4 ns | 11.50 ns | 10.76 ns |  1.00 |    0.02 | 0.0019 |     272 B |        1.00 |
| Wtime_First | go wt(...)onder [49] | 908.7 ns | 13.53 ns | 12.66 ns |  0.99 |    0.02 | 0.0029 |     272 B |        1.00 |
 *
 */

using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Utilities;
using NLog;

namespace Lynx.Benchmark;

/// <summary>
/// Implementation chosen from <see cref="GoCommandParsingAlternatives_Benchmark"/>
/// </summary>
public class GoCommandParsingAlternatives_Order_Benchmark : BaseBenchmark
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [Params(1, 10, 100, 1000)]
    public int Count { get; set; }

    [ParamsSource(nameof(Data))]
    public string? Command { get; set; }

    public static IEnumerable<string> Data =>
    [
        "go wtime 7000 winc 500 btime 8000 binc 500",
        "go ponder wtime 7000 winc 500 btime 8000 binc 500",
        "go wtime 7000 winc 500 btime 8000 binc 500 ponder",
        "go binc 500 winc 500 btime 8000 wtime 7000 ponder",
    ];

    [Benchmark(Baseline = true)]
    public void Naive()
    {
        var command = Command!;

        for (int i = 0; i < Count; ++i)
        {
            ParseNaive(command);
        }
    }

    [Benchmark]
    public void Wtime_First()
    {
        var command = Command!;

        for (int i = 0; i < Count; ++i)
        {
            ParseWtime_First(command);
        }
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
