/*
 *
 * BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 * AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 * .NET SDK 8.0.401
 *   [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 * | Method          | command              | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 * |---------------- |--------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
 * | Sequential      | go infinite          | 1,889.8 ns |  22.33 ns |  19.80 ns | 1,883.3 ns |  1.00 |    0.01 |      - |     272 B |        1.00 |
 * | Parallel        | go infinite          | 3,257.4 ns |  63.33 ns | 132.20 ns | 3,232.4 ns |  1.72 |    0.07 | 0.0191 |    1826 B |        6.71 |
 * | CapturingGroups | go infinite          | 1,577.6 ns |   5.56 ns |   4.34 ns | 1,577.9 ns |  0.83 |    0.01 | 0.0172 |    1504 B |        5.53 |
 * | NoRegex         | go infinite          |   868.0 ns |  10.13 ns |   9.47 ns |   864.2 ns |  0.46 |    0.01 | 0.0029 |     272 B |        1.00 |
 * |                 |                      |            |           |           |            |       |         |        |           |             |
 * | Sequential      | go wt(...)c 500 [42] | 4,330.4 ns |  52.75 ns |  49.34 ns | 4,327.7 ns |  1.00 |    0.02 | 0.0076 |    1248 B |        1.00 |
 * | Parallel        | go wt(...)c 500 [42] | 4,263.0 ns |  82.61 ns | 234.34 ns | 4,185.8 ns |  0.98 |    0.05 | 0.0305 |    2808 B |        2.25 |
 * | CapturingGroups | go wt(...)c 500 [42] | 3,883.5 ns |  22.45 ns |  21.00 ns | 3,885.4 ns |  0.90 |    0.01 | 0.0534 |    4800 B |        3.85 |
 * | NoRegex         | go wt(...)c 500 [42] | 1,011.2 ns |   6.44 ns |   6.02 ns | 1,010.5 ns |  0.23 |    0.00 | 0.0019 |     272 B |        0.22 |
 * |                 |                      |            |           |           |            |       |         |        |           |             |
 * | Sequential      | go wt(...)00000 [78] | 5,401.7 ns | 101.28 ns |  94.74 ns | 5,400.7 ns |  1.00 |    0.02 | 0.0153 |    1728 B |        1.00 |
 * | Parallel        | go wt(...)00000 [78] | 5,129.1 ns | 101.88 ns | 125.11 ns | 5,122.6 ns |  0.95 |    0.03 | 0.0381 |    3288 B |        1.90 |
 * | CapturingGroups | go wt(...)00000 [78] | 5,515.9 ns | 105.29 ns | 103.41 ns | 5,489.9 ns |  1.02 |    0.03 | 0.0839 |    7064 B |        4.09 |
 * | NoRegex         | go wt(...)00000 [78] | 1,179.3 ns |  10.16 ns |   9.01 ns | 1,179.2 ns |  0.22 |    0.00 | 0.0019 |     272 B |        0.16 |
 * |                 |                      |            |           |           |            |       |         |        |           |             |
 * | Sequential      | go wt(...)go 40 [62] | 5,020.1 ns |  67.46 ns |  63.10 ns | 5,036.3 ns |  1.00 |    0.02 | 0.0153 |    1488 B |        1.00 |
 * | Parallel        | go wt(...)go 40 [62] | 4,559.2 ns |  90.55 ns | 190.99 ns | 4,537.0 ns |  0.91 |    0.04 | 0.0305 |    3048 B |        2.05 |
 * | CapturingGroups | go wt(...)go 40 [62] | 5,236.4 ns |  62.20 ns |  55.14 ns | 5,242.3 ns |  1.04 |    0.02 | 0.0839 |    7032 B |        4.73 |
 * | NoRegex         | go wt(...)go 40 [62] | 1,089.1 ns |   4.17 ns |   3.90 ns | 1,088.7 ns |  0.22 |    0.00 | 0.0019 |     272 B |        0.18 |
 *
 *
 * BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2655) (Hyper-V)
 * AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 * .NET SDK 8.0.401
 *   [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 * | Method          | command              | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 * |---------------- |--------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 * | Sequential      | go infinite          | 1,810.8 ns |   8.83 ns |   8.26 ns | 1,814.1 ns |  1.00 |    0.01 | 0.0153 |      - |     272 B |        1.00 |
 * | Parallel        | go infinite          | 2,747.5 ns |  32.83 ns |  30.71 ns | 2,747.9 ns |  1.52 |    0.02 | 0.1068 |      - |    1823 B |        6.70 |
 * | CapturingGroups | go infinite          | 1,387.9 ns |  13.33 ns |  12.47 ns | 1,381.1 ns |  0.77 |    0.01 | 0.0896 |      - |    1504 B |        5.53 |
 * | NoRegex         | go infinite          |   796.1 ns |   5.17 ns |   4.83 ns |   794.8 ns |  0.44 |    0.00 | 0.0162 |      - |     272 B |        1.00 |
 * |                 |                      |            |           |           |            |       |         |        |        |           |             |
 * | Sequential      | go wt(...)c 500 [42] | 3,916.6 ns |  27.85 ns |  26.06 ns | 3,916.1 ns |  1.00 |    0.01 | 0.0687 |      - |    1249 B |        1.00 |
 * | Parallel        | go wt(...)c 500 [42] | 3,910.5 ns |  74.81 ns |  69.98 ns | 3,889.4 ns |  1.00 |    0.02 | 0.1678 |      - |    2808 B |        2.25 |
 * | CapturingGroups | go wt(...)c 500 [42] | 2,986.8 ns |  22.93 ns |  21.45 ns | 2,979.8 ns |  0.76 |    0.01 | 0.2861 | 0.0038 |    4800 B |        3.84 |
 * | NoRegex         | go wt(...)c 500 [42] |   962.8 ns |   1.75 ns |   1.55 ns |   963.2 ns |  0.25 |    0.00 | 0.0153 |      - |     272 B |        0.22 |
 * |                 |                      |            |           |           |            |       |         |        |        |           |             |
 * | Sequential      | go wt(...)00000 [78] | 5,817.6 ns | 115.51 ns | 192.99 ns | 5,799.9 ns |  1.00 |    0.05 | 0.0992 |      - |    1731 B |        1.00 |
 * | Parallel        | go wt(...)00000 [78] | 4,679.0 ns |  92.75 ns | 227.52 ns | 4,543.7 ns |  0.81 |    0.05 | 0.1907 |      - |    3288 B |        1.90 |
 * | CapturingGroups | go wt(...)00000 [78] | 4,446.6 ns |  40.87 ns |  38.23 ns | 4,442.5 ns |  0.77 |    0.03 | 0.4196 | 0.0076 |    7065 B |        4.08 |
 * | NoRegex         | go wt(...)00000 [78] | 1,051.2 ns |   5.68 ns |   5.32 ns | 1,053.6 ns |  0.18 |    0.01 | 0.0153 |      - |     272 B |        0.16 |
 * |                 |                      |            |           |           |            |       |         |        |        |           |             |
 * | Sequential      | go wt(...)go 40 [62] | 4,540.9 ns |  22.86 ns |  55.65 ns | 4,545.7 ns |  1.00 |    0.02 | 0.0763 |      - |    1489 B |        1.00 |
 * | Parallel        | go wt(...)go 40 [62] | 4,339.3 ns |  82.89 ns | 167.44 ns | 4,294.7 ns |  0.96 |    0.04 | 0.1755 |      - |    3048 B |        2.05 |
 * | CapturingGroups | go wt(...)go 40 [62] | 4,279.5 ns |  74.97 ns |  73.63 ns | 4,260.6 ns |  0.94 |    0.02 | 0.4196 | 0.0076 |    7033 B |        4.72 |
 * | NoRegex         | go wt(...)go 40 [62] | 1,003.3 ns |   1.81 ns |   1.60 ns | 1,002.7 ns |  0.22 |    0.00 | 0.0153 |      - |     272 B |        0.18 |
 *
 *
 * BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
 * Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 * .NET SDK 8.0.401
 *   [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *   DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *
 * | Method          | command              | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 * |---------------- |--------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 * | Sequential      | go infinite          | 1,386.9 ns |  13.19 ns |  11.01 ns | 1,381.0 ns |  1.00 |    0.01 | 0.0420 |      - |     272 B |        1.00 |
 * | Parallel        | go infinite          | 3,580.8 ns | 101.24 ns | 296.93 ns | 3,552.0 ns |  2.58 |    0.21 | 0.2899 |      - |    1832 B |        6.74 |
 * | CapturingGroups | go infinite          | 1,225.3 ns |  16.07 ns |  19.73 ns | 1,218.6 ns |  0.88 |    0.02 | 0.2403 |      - |    1504 B |        5.53 |
 * | NoRegex         | go infinite          |   797.5 ns |  11.16 ns |   9.32 ns |   797.6 ns |  0.58 |    0.01 | 0.0429 |      - |     272 B |        1.00 |
 * |                 |                      |            |           |           |            |       |         |        |        |           |             |
 * | Sequential      | go wt(...)c 500 [42] | 3,723.5 ns |  62.03 ns |  48.43 ns | 3,721.0 ns |  1.00 |    0.02 | 0.1984 |      - |    1248 B |        1.00 |
 * | Parallel        | go wt(...)c 500 [42] | 4,551.0 ns |  97.53 ns | 284.49 ns | 4,503.9 ns |  1.22 |    0.08 | 0.4425 |      - |    2808 B |        2.25 |
 * | CapturingGroups | go wt(...)c 500 [42] | 2,127.0 ns |  10.58 ns |  14.13 ns | 2,123.0 ns |  0.57 |    0.01 | 0.7668 | 0.0114 |    4800 B |        3.85 |
 * | NoRegex         | go wt(...)c 500 [42] |   860.7 ns |  16.97 ns |  17.42 ns |   855.2 ns |  0.23 |    0.01 | 0.0420 |      - |     272 B |        0.22 |
 * |                 |                      |            |           |           |            |       |         |        |        |           |             |
 * | Sequential      | go wt(...)00000 [78] | 4,767.2 ns | 195.39 ns | 576.10 ns | 5,088.8 ns |  1.02 |    0.19 | 0.2747 |      - |    1728 B |        1.00 |
 * | Parallel        | go wt(...)00000 [78] | 5,383.0 ns | 114.51 ns | 328.56 ns | 5,304.6 ns |  1.15 |    0.18 | 0.5264 |      - |    3288 B |        1.90 |
 * | CapturingGroups | go wt(...)00000 [78] | 4,288.4 ns |  74.53 ns |  58.19 ns | 4,270.0 ns |  0.92 |    0.13 | 1.1292 | 0.0153 |    7064 B |        4.09 |
 * | NoRegex         | go wt(...)00000 [78] |   905.3 ns |   7.45 ns |   6.96 ns |   902.6 ns |  0.19 |    0.03 | 0.0420 |      - |     272 B |        0.16 |
 * |                 |                      |            |           |           |            |       |         |        |        |           |             |
 * | Sequential      | go wt(...)go 40 [62] | 4,767.8 ns |  34.09 ns |  26.61 ns | 4,770.5 ns |  1.00 |    0.01 | 0.2136 |      - |    1488 B |        1.00 |
 * | Parallel        | go wt(...)go 40 [62] | 4,821.8 ns |  50.13 ns |  44.44 ns | 4,808.8 ns |  1.01 |    0.01 | 0.4883 |      - |    3048 B |        2.05 |
 * | CapturingGroups | go wt(...)go 40 [62] | 3,102.9 ns |  42.35 ns |  37.54 ns | 3,107.2 ns |  0.65 |    0.01 | 1.1253 | 0.0267 |    7032 B |        4.73 |
 * | NoRegex         | go wt(...)go 40 [62] |   907.2 ns |  17.28 ns |  17.75 ns |   904.3 ns |  0.19 |    0.00 | 0.0420 |      - |     272 B |        0.18 |
 *
 *
 * BenchmarkDotNet v0.14.0, macOS Ventura 13.6.9 (22G830) [Darwin 22.6.0]
 * Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 * .NET SDK 8.0.401
 *   [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 * | Method          | command              | Mean      | Error     | StdDev     | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 * |---------------- |--------------------- |----------:|----------:|-----------:|---------:|------:|--------:|-------:|----------:|------------:|
 * | Sequential      | go infinite          |  58.04 us | 13.959 us |  37.259 us | 42.16 us |  1.43 |    2.04 |      - |     272 B |        1.00 |
 * | Parallel        | go infinite          | 177.16 us | 67.495 us | 194.738 us | 84.03 us |  4.38 |    8.46 | 0.2441 |    1831 B |        6.73 |
 * | CapturingGroups | go infinite          | 112.30 us | 27.751 us |  77.358 us | 75.78 us |  2.77 |    4.08 | 0.1831 |    1504 B |        5.53 |
 * | NoRegex         | go infinite          |  32.00 us | 10.268 us |  27.759 us | 19.85 us |  0.79 |    1.31 |      - |     272 B |        1.00 |
 * |                 |                      |           |           |            |          |       |         |        |           |             |
 * | Sequential      | go wt(...)c 500 [42] | 129.38 us | 42.167 us | 123.003 us | 55.16 us |  2.41 |    3.51 | 0.1221 |    1249 B |        1.00 |
 * | Parallel        | go wt(...)c 500 [42] |  60.64 us |  1.570 us |   4.376 us | 60.84 us |  1.13 |    0.92 | 0.3662 |    2808 B |        2.25 |
 * | CapturingGroups | go wt(...)c 500 [42] |  36.44 us |  2.684 us |   7.829 us | 37.74 us |  0.68 |    0.58 | 0.7324 |    4800 B |        3.84 |
 * | NoRegex         | go wt(...)c 500 [42] |  22.26 us |  1.077 us |   3.055 us | 21.99 us |  0.41 |    0.34 | 0.0305 |     272 B |        0.22 |
 * |                 |                      |           |           |            |          |       |         |        |           |             |
 * | Sequential      | go wt(...)00000 [78] |  38.51 us |  2.774 us |   8.137 us | 38.37 us |  1.05 |    0.34 | 0.2747 |    1728 B |        1.00 |
 * | Parallel        | go wt(...)00000 [78] |  48.18 us |  0.947 us |   1.297 us | 48.00 us |  1.31 |    0.32 | 0.4883 |    3288 B |        1.90 |
 * | CapturingGroups | go wt(...)00000 [78] |  40.83 us |  1.883 us |   5.462 us | 40.64 us |  1.11 |    0.31 | 1.1292 |    7065 B |        4.09 |
 * | NoRegex         | go wt(...)00000 [78] |  33.89 us |  2.446 us |   7.174 us | 33.60 us |  0.92 |    0.30 |      - |     272 B |        0.16 |
 * |                 |                      |           |           |            |          |       |         |        |           |             |
 * | Sequential      | go wt(...)go 40 [62] |  30.76 us |  3.056 us |   8.913 us | 30.54 us |  1.10 |    0.50 | 0.2136 |    1488 B |        1.00 |
 * | Parallel        | go wt(...)go 40 [62] |  34.94 us |  0.690 us |   1.744 us | 34.76 us |  1.25 |    0.43 | 0.4883 |    3048 B |        2.05 |
 * | CapturingGroups | go wt(...)go 40 [62] |  33.42 us |  2.963 us |   8.597 us | 34.12 us |  1.19 |    0.52 | 1.0986 |    7032 B |        4.73 |
 * | NoRegex         | go wt(...)go 40 [62] |  18.27 us |  1.997 us |   5.887 us | 17.60 us |  0.65 |    0.31 | 0.0381 |     272 B |        0.18 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.UCI.Commands.GUI;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Diagnostics.Utilities;
using NLog;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark;

public partial class GoCommandParsingAlternatives_Benchmark : BaseBenchmark
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static IEnumerable<string> Data => [
            "go infinite",
            "go wtime 7000 winc 500 btime 8000 binc 500",
            "go wtime 7000 winc 500 btime 8000 binc 500 ponder movestogo 40",
            "go wtime 7000 winc 500 btime 8000 binc 500 movestogo 40 depth 14 nodes 1000000",
        ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public async Task Sequential(string command)
    {
        await Task.Run(() => ParseSequentially(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task Parallel(string command)
    {
        await ParseInParallel(command);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task CapturingGroups(string command)
    {
        await Task.Run(() => ParseRegexCapturingGroups(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex(string command)
    {
        await Task.Run(() => ParseNoRegex(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex_DictionaryAction(string command)
    {
        await Task.Run(() => ParseNoRegex_DictionaryAction(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex_DictionaryActionAndMemoryValues(string command)
    {
        await Task.Run(() => ParseNoRegex_DictionaryActionAndMemoryValues(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex_DictionaryActionAndMemoryValues2(string command)
    {
        await Task.Run(() => ParseNoRegex_DictionaryActionAndMemoryValues_2(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex_ReadOnlySpanComparison(string command)
    {
        await Task.Run(() => ParseNoRegex_ReadOnlySpanComparison(command));
    }

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

    [GeneratedRegex("(?<=movetime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex MoveTimeRegex();

    [GeneratedRegex("(?<=depth).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex DepthRegex();

    [GeneratedRegex("(?<=nodes).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex NodesRegex();

    [GeneratedRegex("(?<=mate).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex MateRegex();

    [GeneratedRegex("(?<=searchmoves).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex SearchMovesRegex();

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
        var match = WhiteTimeRegex().Match(command);
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

        match = MoveTimeRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            MoveTime = value;
        }

        match = DepthRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            Depth = value;
        }

        Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase);

        Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase);

        //match = NodesRegex().Match(command);
        //if (int.TryParse(match.Value, out value))
        //{
        //    Nodes = value;
        //}

        //match = MateRegex().Match(command);
        //if (int.TryParse(match.Value, out value))
        //{
        //    Mate = value;
        //}

        //var match = SearchMovesRegex().Match(command);
        //SearchMoves = [.. match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)];ç
    }

    private async Task ParseInParallel(string command)
    {
        var taskList = new List<Task>
            {
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
                    var match = MoveTimeRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MoveTime = value;
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
                Task.Run(() => Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase)),
                Task.Run(() => Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase)),
                //Task.Run(() =>
                //{
                //    var match = NodesRegex().Match(command);

                //    if(int.TryParse(match.Value, out var value))
                //    {
                //        Nodes = value;
                //    }
                //}),
                //Task.Run(() =>
                //{
                //    var match = MateRegex().Match(command);

                //    if(int.TryParse(match.Value, out var value))
                //    {
                //        Mate = value;
                //    }
                //}),
                //Task.Run(() =>
                //{
                //    var match = SearchMovesRegex().Match(command);

                //    SearchMoves = Enumerable.ToList<string>(match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                //}),
            };

        await Task.WhenAll(taskList);
    }

    private void ParseRegexCapturingGroups(string command)
    {
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
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
                        case "movetime":
                            MoveTime = int.Parse(group.Value);
                            break;
                        case "depth":
                            Depth = int.Parse(group.Value);
                            break;
                        case "infinite":
                            Infinite = true;
                            break;
                        case "ponder":
                            Ponder = true;
                            break;
                            //case "nodes":
                            //    Nodes = int.Parse(group.Value);
                            //    break;
                            //case "mate":
                            //    Nodes = int.Parse(group.Value);
                            //    break;
                            //case "searchmoves":
                            //    Nodes = int.Parse(group.Value);
                            //    break;
                    }

                    break;
                }
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
    }

    private void ParseNoRegex(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            switch (commandAsSpan[ranges[i]])
            {
                case "wtime":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            WhiteTime = value;
                        }

                        break;
                    }
                case "btime":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            BlackTime = value;
                        }

                        break;
                    }
                case "winc":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            WhiteIncrement = value;
                        }

                        break;
                    }
                case "binc":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            BlackIncrement = value;
                        }

                        break;
                    }
                case "movestogo":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            MovesToGo = value;
                        }

                        break;
                    }
                case "movetime":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            MoveTime = value;
                        }

                        break;
                    }
                case "depth":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            Depth = value;
                        }

                        break;
                    }
                case "infinite":
                    {
                        Infinite = true;
                        break;
                    }
                case "ponder":
                    {
                        Ponder = true;
                        break;
                    }
                case "nodes":
                    {
                        _logger.Warn("nodes not supported in go command, it will be safely ignored");
                        ++i;
                        break;
                    }
                case "mate":
                    {
                        _logger.Warn("mate not supported in go command, it will be safely ignored");
                        ++i;
                        break;
                    }
                case "searchmoves":
                    {
                        const string message = "searchmoves not supported in go command";
                        _logger.Error(message);
                        throw new InvalidDataException(message);
                    }
                default:
                    {
                        _logger.Warn("{0} not supported in go command", commandAsSpan[ranges[i]].ToString());
                        break;
                    }
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    private static readonly Dictionary<string, Action<GoCommandParsingAlternatives_Benchmark, int>> _commandActions = new Dictionary<string, Action<GoCommandParsingAlternatives_Benchmark, int>>
    {
        ["wtime"] = (command, value) => command.WhiteTime = value,
        ["btime"] = (command, value) => command.BlackTime = value,
        ["winc"] = (command, value) => command.WhiteIncrement = value,
        ["binc"] = (command, value) => command.BlackIncrement = value,
        ["movestogo"] = (command, value) => command.MovesToGo = value,
        ["movetime"] = (command, value) => command.MoveTime = value,
        ["depth"] = (command, value) => command.Depth = value
    };

    private void ParseNoRegex_DictionaryAction(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]].ToString();
            if (_commandActions.TryGetValue(key, out var action))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    action(this, value);
                }
            }
            else
            {
                switch (key)
                {
                    case "infinite":
                        Infinite = true;
                        break;
                    case "ponder":
                        Ponder = true;
                        break;
                    case "nodes":
                        _logger.Warn("nodes not supported in go command, it will be safely ignored");
                        ++i;
                        break;
                    case "mate":
                        _logger.Warn("mate not supported in go command, it will be safely ignored");
                        ++i;
                        break;
                    case "searchmoves":
                        const string message = "searchmoves not supported in go command";
                        _logger.Error(message);
                        throw new NotImplementedException(message);
                    default:
                        _logger.Warn("{0} not supported in go command, attempting to continue command parsing", key);
                        break;
                }
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    private static readonly ReadOnlyMemory<char> InfiniteMemory = "infinite".AsMemory();
    private static readonly ReadOnlyMemory<char> PonderMemory = "ponder".AsMemory();
    private static readonly ReadOnlyMemory<char> NodesMemory = "nodes".AsMemory();
    private static readonly ReadOnlyMemory<char> MateMemory = "mate".AsMemory();
    private static readonly ReadOnlyMemory<char> SearchMovesMemory = "searchmoves".AsMemory();

    private static readonly Dictionary<ReadOnlyMemory<char>, Action<GoCommandParsingAlternatives_Benchmark, int>> _commandActions2 = new()
    {
        ["wtime".AsMemory()] = (command, value) => command.WhiteTime = value,
        ["btime".AsMemory()] = (command, value) => command.BlackTime = value,
        ["winc".AsMemory()] = (command, value) => command.WhiteIncrement = value,
        ["binc".AsMemory()] = (command, value) => command.BlackIncrement = value,
        ["movestogo".AsMemory()] = (command, value) => command.MovesToGo = value,
        ["movetime".AsMemory()] = (command, value) => command.MoveTime = value,
        ["depth".AsMemory()] = (command, value) => command.Depth = value
    };

    private void ParseNoRegex_DictionaryActionAndMemoryValues(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];
            if (_commandActions2.TryGetValue(key.ToString().AsMemory(), out var action))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    action(this, value);
                }
            }
            else if (key.Equals(InfiniteMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;
            }
            else if (key.Equals(NodesMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchMovesMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new NotImplementedException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command, attempting to continue command parsing", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    private static readonly Dictionary<string, Action<GoCommandParsingAlternatives_Benchmark, int>> _commandActions3 = new()
    {
        ["wtime"] = (command, value) => command.WhiteTime = value,
        ["btime"] = (command, value) => command.BlackTime = value,
        ["winc"] = (command, value) => command.WhiteIncrement = value,
        ["binc"] = (command, value) => command.BlackIncrement = value,
        ["movestogo"] = (command, value) => command.MovesToGo = value,
        ["movetime"] = (command, value) => command.MoveTime = value,
        ["depth"] = (command, value) => command.Depth = value
    };

    private void ParseNoRegex_DictionaryActionAndMemoryValues_2(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];
            if (_commandActions3.TryGetValue(key.ToString(), out var action))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    action(this, value);
                }
            }
            else if (key.Equals(InfiniteMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;
            }
            else if (key.Equals(NodesMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchMovesMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new NotImplementedException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command, attempting to continue command parsing", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

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

    private void ParseNoRegex_ReadOnlySpanComparison(string command)
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
}
