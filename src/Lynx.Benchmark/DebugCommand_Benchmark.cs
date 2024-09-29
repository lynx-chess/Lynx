/*
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.19045.3448/22H2/2022Update)
 *  Intel Core i7-5500U CPU 2.40GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23463.5
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method      | command   | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------ |---------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | StringSplit | debug off | 77.09 ns | 1.584 ns | 1.824 ns |  1.00 |    0.00 | 0.0497 |     104 B |        1.00 |
 *  | SpanSplit   | debug off | 44.06 ns | 0.591 ns | 0.524 ns |  0.57 |    0.01 |      - |         - |        0.00 |
 *  | SpanSplit2  | debug off | 46.36 ns | 0.739 ns | 0.617 ns |  0.60 |    0.02 |      - |         - |        0.00 |
 *  |             |           |          |          |          |       |         |        |           |             |
 *  | StringSplit | debug on  | 73.92 ns | 1.373 ns | 1.217 ns |  1.00 |    0.00 | 0.0497 |     104 B |        1.00 |
 *  | SpanSplit   | debug on  | 38.15 ns | 0.732 ns | 0.649 ns |  0.52 |    0.01 |      - |         - |        0.00 |
 *  | SpanSplit2  | debug on  | 37.50 ns | 0.574 ns | 0.537 ns |  0.51 |    0.01 |      - |         - |        0.00 |
 *  |             |           |          |          |          |       |         |        |           |             |
 *  | StringSplit | debug onf | 79.91 ns | 1.378 ns | 1.587 ns |  1.00 |    0.00 | 0.0497 |     104 B |        1.00 |
 *  | SpanSplit   | debug onf | 48.67 ns | 0.638 ns | 0.533 ns |  0.61 |    0.01 |      - |         - |        0.00 |
 *  | SpanSplit2  | debug onf | 40.53 ns | 0.550 ns | 0.429 ns |  0.51 |    0.01 |      - |         - |        0.0  | // I forgot a !sign, hence the diff here
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.UCI.Commands;

namespace Lynx.Benchmark;
public class DebugCommand_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
    [
        "debug on",
        "debug off",
        "debug onf",
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public bool StringSplit(string command) => DebugCommandBenchmark_DebugCommandStringSplit.Parse(command);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool SpanSplit(string command) => DebugCommandBenchmark_DebugCommandSpanSplit.Parse(command);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool SpanSplit2(string command) => DebugCommandBenchmark_DebugCommandSpanSplit2.Parse(command);

    public sealed class DebugCommandBenchmark_DebugCommandStringSplit : IGUIBaseCommand
    {
        public const string Id = "debug";

        public static bool Parse(string command)
        {
            const string on = "on";
            const string off = "off";

            var state = command.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];

            return state.Equals(on, StringComparison.OrdinalIgnoreCase)
                || (!state.Equals(off, StringComparison.OrdinalIgnoreCase)
                    && Configuration.IsDebug);
        }
    }

    public sealed class DebugCommandBenchmark_DebugCommandSpanSplit : IGUIBaseCommand
    {
        public const string Id = "debug";

        public static bool Parse(ReadOnlySpan<char> command)
        {
            const string on = "on";
            const string off = "off";

            Span<Range> items = stackalloc Range[2];
            command.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries);

            return command[items[1]].Equals(on, StringComparison.OrdinalIgnoreCase)
                || (!command[items[1]].Equals(off, StringComparison.OrdinalIgnoreCase)
                    && Configuration.IsDebug);
        }
    }

    public sealed class DebugCommandBenchmark_DebugCommandSpanSplit2 : IGUIBaseCommand
    {
        public const string Id = "debug";

        public static bool Parse(ReadOnlySpan<char> command)
        {
            const string on = "on";
            const string off = "off";

            Span<Range> items = stackalloc Range[2];
            command.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries);

            var debugValue = command[items[1]];

            return debugValue.Equals(on, StringComparison.OrdinalIgnoreCase)
                || (!debugValue.Equals(off, StringComparison.OrdinalIgnoreCase)
                    && Configuration.IsDebug);
        }
    }
}
