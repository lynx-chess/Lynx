/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method          | command              | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |---------------- |--------------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | Sequential      | go infinite          |  2.320 us | 0.0149 us | 0.0125 us |  2.324 us |  1.00 |    0.00 |      - |     304 B |        1.00 |
 *  | Parallell       | go infinite          |  3.742 us | 0.0617 us | 0.0960 us |  3.726 us |  1.63 |    0.04 | 0.0229 |    2246 B |        7.39 |
 *  | CapturingGroups | go infinite          |  1.566 us | 0.0136 us | 0.0120 us |  1.564 us |  0.67 |    0.01 | 0.0172 |    1504 B |        4.95 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go in(...) d2d4 [33] |  4.523 us | 0.0590 us | 0.0552 us |  4.520 us |  1.00 |    0.00 | 0.0076 |     752 B |        1.00 |
 *  | Parallell       | go in(...) d2d4 [33] |  4.644 us | 0.0928 us | 0.2150 us |  4.569 us |  1.02 |    0.05 | 0.0305 |    2696 B |        3.59 |
 *  | CapturingGroups | go in(...) d2d4 [33] |  1.771 us | 0.0095 us | 0.0089 us |  1.769 us |  0.39 |    0.01 | 0.0172 |    1504 B |        2.00 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go wt(...)c 500 [43] |  4.898 us | 0.0408 us | 0.0382 us |  4.892 us |  1.00 |    0.00 | 0.0153 |    1288 B |        1.00 |
 *  | Parallell       | go wt(...)c 500 [43] |  4.861 us | 0.0968 us | 0.2085 us |  4.825 us |  1.00 |    0.06 | 0.0381 |    3232 B |        2.51 |
 *  | CapturingGroups | go wt(...)c 500 [43] |  3.421 us | 0.0183 us | 0.0162 us |  3.420 us |  0.70 |    0.01 | 0.0420 |    3712 B |        2.88 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go wt(...)onder [50] |  5.160 us | 0.0963 us | 0.0989 us |  5.188 us |  1.00 |    0.00 | 0.0153 |    1288 B |        1.00 |
 *  | Parallell       | go wt(...)onder [50] |  5.444 us | 0.1133 us | 0.3120 us |  5.377 us |  1.05 |    0.07 |      - |    3231 B |        2.51 |
 *  | CapturingGroups | go wt(...)onder [50] |  4.137 us | 0.0347 us | 0.0325 us |  4.134 us |  0.80 |    0.02 | 0.0534 |    4768 B |        3.70 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go in(...)c 100 [66] |  6.498 us | 0.1193 us | 0.1116 us |  6.519 us |  1.00 |    0.00 | 0.0153 |    1496 B |        1.00 |
 *  | Parallell       | go in(...)c 100 [66] |  5.864 us | 0.1165 us | 0.2769 us |  5.842 us |  0.90 |    0.05 |      - |    3440 B |        2.30 |
 *  | CapturingGroups | go in(...)c 100 [66] |  3.992 us | 0.0281 us | 0.0263 us |  3.994 us |  0.61 |    0.01 | 0.0534 |    4768 B |        3.19 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go i(...) 500 [118]  |  8.075 us | 0.1394 us | 0.1304 us |  8.074 us |  1.00 |    0.00 | 0.0305 |    2696 B |        1.00 |
 *  | Parallell       | go i(...) 500 [118]  |  7.440 us | 0.2449 us | 0.6908 us |  7.410 us |  0.89 |    0.06 |      - |    4640 B |        1.72 |
 *  | CapturingGroups | go i(...) 500 [118]  |  6.376 us | 0.1268 us | 0.2083 us |  6.391 us |  0.79 |    0.03 | 0.1068 |    9208 B |        3.42 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go i(...) 500 [149]  |  9.341 us | 0.1475 us | 0.1380 us |  9.395 us |  1.00 |    0.00 | 0.0305 |    3208 B |        1.00 |
 *  | Parallell       | go i(...) 500 [149]  | 10.759 us | 0.3415 us | 1.0015 us | 10.735 us |  1.18 |    0.10 | 0.0610 |    5152 B |        1.61 |
 *  | CapturingGroups | go i(...) 500 [149]  |  6.404 us | 0.1268 us | 0.1735 us |  6.386 us |  0.68 |    0.02 | 0.1068 |    9208 B |        2.87 |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *
 *  | Method          | command              | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |---------------- |--------------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Sequential      | go infinite          | 11.963 us | 0.2364 us | 0.5336 us | 12.196 us |  1.00 |    0.00 | 0.0305 |      - |     304 B |        1.00 |
 *  | Parallell       | go infinite          | 13.380 us | 0.2647 us | 0.4774 us | 13.381 us |  1.14 |    0.08 | 0.3357 |      - |    2248 B |        7.39 |
 *  | CapturingGroups | go infinite          |  4.590 us | 0.5742 us | 1.6929 us |  3.362 us |  0.30 |    0.10 | 0.2403 |      - |    1504 B |        4.95 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go in(...) d2d4 [33] | 12.361 us | 0.2459 us | 0.2300 us | 12.355 us |  1.00 |    0.00 | 0.1068 |      - |     752 B |        1.00 |
 *  | Parallell       | go in(...) d2d4 [33] | 14.285 us | 0.2796 us | 0.3635 us | 14.407 us |  1.16 |    0.04 | 0.4272 |      - |    2696 B |        3.59 |
 *  | CapturingGroups | go in(...) d2d4 [33] |  4.477 us | 0.2931 us | 0.8268 us |  4.434 us |  0.31 |    0.03 | 0.2289 |      - |    1504 B |        2.00 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)c 500 [43] | 12.391 us | 0.2343 us | 0.3915 us | 12.453 us |  1.00 |    0.00 | 0.1831 |      - |    1288 B |        1.00 |
 *  | Parallell       | go wt(...)c 500 [43] | 18.218 us | 0.3643 us | 0.3741 us | 18.360 us |  1.47 |    0.06 | 0.4883 |      - |    3232 B |        2.51 |
 *  | CapturingGroups | go wt(...)c 500 [43] | 12.109 us | 0.2193 us | 0.2052 us | 12.067 us |  0.98 |    0.05 | 0.5951 |      - |    3712 B |        2.88 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)onder [50] | 12.658 us | 0.2429 us | 0.4907 us | 12.774 us |  1.00 |    0.00 | 0.1221 |      - |    1288 B |        1.00 |
 *  | Parallell       | go wt(...)onder [50] | 16.124 us | 0.3212 us | 0.4062 us | 16.247 us |  1.29 |    0.06 | 0.4883 |      - |    3232 B |        2.51 |
 *  | CapturingGroups | go wt(...)onder [50] | 12.405 us | 0.2424 us | 0.2149 us | 12.401 us |  1.00 |    0.05 | 0.7629 |      - |    4768 B |        3.70 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go in(...)c 100 [66] | 13.037 us | 0.2603 us | 0.6992 us | 12.912 us |  1.00 |    0.00 | 0.2289 |      - |    1496 B |        1.00 |
 *  | Parallell       | go in(...)c 100 [66] | 18.527 us | 0.4227 us | 1.2397 us | 18.162 us |  1.43 |    0.13 | 0.5493 |      - |    3440 B |        2.30 |
 *  | CapturingGroups | go in(...)c 100 [66] | 11.905 us | 0.1449 us | 0.1355 us | 11.948 us |  0.86 |    0.04 | 0.7629 |      - |    4768 B |        3.19 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go i(...) 500 [118]  | 12.834 us | 0.2485 us | 0.2203 us | 12.918 us |  1.00 |    0.00 | 0.2441 |      - |    2697 B |        1.00 |
 *  | Parallell       | go i(...) 500 [118]  | 19.906 us | 0.3877 us | 0.8179 us | 19.796 us |  1.56 |    0.11 | 0.7324 |      - |    4640 B |        1.72 |
 *  | CapturingGroups | go i(...) 500 [118]  | 13.421 us | 0.2632 us | 0.4746 us | 13.466 us |  1.02 |    0.05 | 1.4648 | 0.0458 |    9208 B |        3.41 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go i(...) 500 [149]  | 18.008 us | 0.3546 us | 0.7402 us | 18.110 us |  1.00 |    0.00 | 0.4883 |      - |    3208 B |        1.00 |
 *  | Parallell       | go i(...) 500 [149]  | 19.783 us | 0.3932 us | 1.1469 us | 19.641 us |  1.09 |    0.06 | 0.8240 |      - |    5152 B |        1.61 |
 *  | CapturingGroups | go i(...) 500 [149]  | 13.983 us | 0.3050 us | 0.8896 us | 14.015 us |  0.78 |    0.05 | 1.4648 |      - |    9208 B |        2.87 |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method          | command              | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |---------------- |--------------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Sequential      | go infinite          |  2.120 us | 0.0053 us | 0.0044 us |  2.121 us |  1.00 |    0.00 | 0.0153 |      - |     304 B |        1.00 |
 *  | Parallell       | go infinite          |  3.240 us | 0.0508 us | 0.0475 us |  3.227 us |  1.53 |    0.02 | 0.1335 |      - |    2244 B |        7.38 |
 *  | CapturingGroups | go infinite          |  1.300 us | 0.0029 us | 0.0027 us |  1.300 us |  0.61 |    0.00 | 0.0896 |      - |    1504 B |        4.95 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go in(...) d2d4 [33] |  3.947 us | 0.0633 us | 0.1075 us |  3.932 us |  1.00 |    0.00 | 0.0305 |      - |     753 B |        1.00 |
 *  | Parallell       | go in(...) d2d4 [33] |  4.054 us | 0.0775 us | 0.0829 us |  4.052 us |  1.03 |    0.03 | 0.1602 |      - |    2696 B |        3.58 |
 *  | CapturingGroups | go in(...) d2d4 [33] |  1.431 us | 0.0036 us | 0.0032 us |  1.432 us |  0.36 |    0.01 | 0.0896 |      - |    1504 B |        2.00 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)c 500 [43] |  4.600 us | 0.0673 us | 0.0630 us |  4.589 us |  1.00 |    0.00 | 0.0763 |      - |    1299 B |        1.00 |
 *  | Parallell       | go wt(...)c 500 [43] |  4.312 us | 0.0842 us | 0.1034 us |  4.284 us |  0.94 |    0.02 | 0.1907 |      - |    3232 B |        2.49 |
 *  | CapturingGroups | go wt(...)c 500 [43] |  2.566 us | 0.0095 us | 0.0080 us |  2.563 us |  0.56 |    0.01 | 0.2213 |      - |    3712 B |        2.86 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)onder [50] |  5.741 us | 0.1142 us | 0.3086 us |  5.766 us |  1.00 |    0.00 | 0.0763 |      - |    1292 B |        1.00 |
 *  | Parallell       | go wt(...)onder [50] |  4.679 us | 0.0929 us | 0.2528 us |  4.589 us |  0.82 |    0.07 | 0.1907 |      - |    3232 B |        2.50 |
 *  | CapturingGroups | go wt(...)onder [50] |  3.060 us | 0.0149 us | 0.0140 us |  3.059 us |  0.57 |    0.03 | 0.2823 | 0.0038 |    4768 B |        3.69 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go in(...)c 100 [66] | 18.036 us | 0.7603 us | 2.1813 us | 17.759 us |  1.00 |    0.00 | 0.0916 |      - |    1527 B |        1.00 |
 *  | Parallell       | go in(...)c 100 [66] |  5.976 us | 0.1191 us | 0.2294 us |  5.915 us |  0.34 |    0.05 | 0.2060 |      - |    3440 B |        2.25 |
 *  | CapturingGroups | go in(...)c 100 [66] |  3.016 us | 0.0162 us | 0.0151 us |  3.017 us |  0.18 |    0.02 | 0.2823 | 0.0038 |    4768 B |        3.12 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go i(...) 500 [118]  | 31.281 us | 0.6169 us | 0.7576 us | 31.541 us |  1.00 |    0.00 | 0.1221 |      - |    2756 B |        1.00 |
 *  | Parallell       | go i(...) 500 [118]  |  7.199 us | 0.1530 us | 0.4342 us |  7.053 us |  0.25 |    0.02 | 0.2747 |      - |    4640 B |        1.68 |
 *  | CapturingGroups | go i(...) 500 [118]  | 10.805 us | 1.9426 us | 5.7278 us |  6.438 us |  0.20 |    0.01 | 0.5493 | 0.0153 |    9214 B |        3.34 |
 *  |                 |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential      | go i(...) 500 [149]  | 33.093 us | 0.2194 us | 0.1945 us | 33.077 us |  1.00 |    0.00 | 0.1831 |      - |    3271 B |        1.00 |
 *  | Parallell       | go i(...) 500 [149]  |  7.872 us | 0.1574 us | 0.3032 us |  7.755 us |  0.24 |    0.01 | 0.3052 |      - |    5152 B |        1.58 |
 *  | CapturingGroups | go i(...) 500 [149]  | 13.289 us | 1.2020 us | 3.5441 us | 11.204 us |  0.33 |    0.02 | 0.5493 | 0.0153 |    9224 B |        2.82 |
 *
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
