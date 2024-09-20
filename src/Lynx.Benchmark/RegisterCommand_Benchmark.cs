/*
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.19045.3448/22H2/2022Update)
 *  Intel Core i7-5500U CPU 2.40GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23463.5
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method          | command              | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |---------------- |--------------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | StringSplit     | register late        | 172.04 ns |  3.535 ns |  6.191 ns | 170.74 ns |  1.00 |    0.00 | 0.1683 |     352 B |        1.00 |
 *  | SpanSplit       | register late        | 106.20 ns |  1.335 ns |  1.115 ns | 105.79 ns |  0.62 |    0.02 | 0.0842 |     176 B |        0.50 |
 *  | SpanSplitStruct | register late        |  97.06 ns |  1.711 ns |  1.517 ns |  97.47 ns |  0.57 |    0.02 | 0.0650 |     136 B |        0.39 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | StringSplit     | regis(...)74324 [98] | 726.40 ns | 27.308 ns | 72.890 ns | 692.78 ns |  1.00 |    0.00 | 0.8717 |    1824 B |        1.00 |
 *  | SpanSplit       | regis(...)74324 [98] | 338.85 ns |  6.824 ns |  8.123 ns | 339.86 ns |  0.43 |    0.05 | 0.3748 |     784 B |        0.43 |
 *  | SpanSplitStruct | regis(...)74324 [98] | 311.02 ns |  6.152 ns |  5.453 ns | 310.14 ns |  0.42 |    0.04 | 0.3557 |     744 B |        0.41 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | StringSplit     | regis(...)74324 [41] | 336.63 ns |  6.454 ns |  6.038 ns | 338.06 ns |  1.00 |    0.00 | 0.3328 |     696 B |        1.00 |
 *  | SpanSplit       | regis(...)74324 [41] | 199.10 ns |  4.035 ns |  3.577 ns | 199.03 ns |  0.59 |    0.01 | 0.1147 |     240 B |        0.34 |
 *  | SpanSplitStruct | regis(...)74324 [41] | 204.13 ns |  4.136 ns |  3.667 ns | 202.78 ns |  0.61 |    0.02 | 0.0956 |     200 B |        0.29 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | StringSplit     | regis(...)74324 [39] | 333.95 ns |  6.689 ns |  8.459 ns | 333.40 ns |  1.00 |    0.00 | 0.3290 |     688 B |        1.00 |
 *  | SpanSplit       | regis(...)74324 [39] | 200.44 ns |  3.881 ns |  4.313 ns | 199.69 ns |  0.60 |    0.02 | 0.1147 |     240 B |        0.35 |
 *  | SpanSplitStruct | regis(...)74324 [39] | 193.11 ns |  2.168 ns |  2.028 ns | 193.30 ns |  0.58 |    0.02 | 0.0956 |     200 B |        0.29 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.UCI.Commands;
using System.Text;

namespace Lynx.Benchmark;
public class RegisterCommand_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
    [
        "register late",
        "register name Stefan MK code 4359874324",
        "register name Lynx 0.16.0 code 4359874324",
        "register name Lynx 0.16.0 by eduherminio, check https://github.com/lync-chess/lynx code 4359874324",
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public RegisterCommandBenchmark_RegisterCommandStringSplit StringSplit(string command) => new(command);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public RegisterCommandBenchmark_RegisterCommandSpanSplit SpanSplit(string command) => new(command);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public RegisterCommandBenchmark_RegisterCommandSpanSplitStruct SpanSplitStruct(string command) => new(command);

    public sealed class RegisterCommandBenchmark_RegisterCommandStringSplit : IGUIBaseCommand
    {
        public const string Id = "register";

        public bool Later { get; }

        public string Name { get; } = string.Empty;

        public string Code { get; } = string.Empty;

        public RegisterCommandBenchmark_RegisterCommandStringSplit(string command)
        {
            var items = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (string.Equals("later", items[1], StringComparison.OrdinalIgnoreCase))
            {
                Later = true;
                return;
            }

            var sb = new StringBuilder();

            foreach (var item in items[1..])
            {
                if (string.Equals("name", item, StringComparison.OrdinalIgnoreCase))
                {
                    Code = sb.ToString().TrimEnd();
                    sb.Clear();
                }
                else if (string.Equals("code", item, StringComparison.OrdinalIgnoreCase))
                {
                    Name = sb.ToString().TrimEnd();
                    sb.Clear();
                }
                else
                {
                    sb.Append(item);
                    sb.Append(' ');
                }
            }

            if (string.IsNullOrEmpty(Name))
            {
                Name = sb.ToString().TrimEnd();
            }
            else
            {
                Code = sb.ToString().TrimEnd();
            }
        }
    }

    public sealed class RegisterCommandBenchmark_RegisterCommandSpanSplit : IGUIBaseCommand
    {
        public const string Id = "register";

        public bool Later { get; }

        public string Name { get; } = string.Empty;

        public string Code { get; } = string.Empty;

        public RegisterCommandBenchmark_RegisterCommandSpanSplit(ReadOnlySpan<char> command)
        {
            const string later = "later";
            const string name = "name";
            const string code = "code";

            Span<Range> items = stackalloc Range[6];
            var itemsLength = command.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (command[items[1]].Equals(later, StringComparison.OrdinalIgnoreCase))
            {
                Later = true;
                return;
            }

            var sb = new StringBuilder();

            for (int i = 1; i < itemsLength; ++i)
            {
                var item = command[items[i]];
                if (item.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    Code = sb.ToString();
                    sb.Clear();
                }
                else if (item.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    Name = sb.ToString();
                    sb.Clear();
                }
                else
                {
                    sb.Append(item);
                    sb.Append(' ');
                }
            }

            if (string.IsNullOrEmpty(Name))
            {
                Name = sb.ToString();
            }
            else
            {
                Code = sb.ToString();
            }
        }
    }

    public readonly struct RegisterCommandBenchmark_RegisterCommandSpanSplitStruct
    {
        public const string Id = "register";

        public bool Later { get; }

        public string Name { get; } = string.Empty;

        public string Code { get; } = string.Empty;

        public RegisterCommandBenchmark_RegisterCommandSpanSplitStruct(ReadOnlySpan<char> command)
        {
            const string later = "later";
            const string name = "name";
            const string code = "code";

            Span<Range> items = stackalloc Range[6];
            var itemsLength = command.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (command[items[1]].Equals(later, StringComparison.OrdinalIgnoreCase))
            {
                Later = true;
                return;
            }

            var sb = new StringBuilder();

            for (int i = 1; i < itemsLength; ++i)
            {
                var item = command[items[i]];
                if (item.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    Code = sb.ToString();
                    sb.Clear();
                }
                else if (item.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    Name = sb.ToString();
                    sb.Clear();
                }
                else
                {
                    sb.Append(item);
                    sb.Append(' ');
                }
            }

            if (string.IsNullOrEmpty(Name))
            {
                Name = sb.ToString();
            }
            else
            {
                Code = sb.ToString();
            }
        }
    }
}