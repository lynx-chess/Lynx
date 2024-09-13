/*
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *  
 *  | Method          | command              | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |---------------- |--------------------- |-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | Sequential      | go infinite          | 1,851.4 ns |  24.09 ns |  21.36 ns |  1.00 |    0.02 |      - |     272 B |        1.00 |
 *  | Parallel        | go infinite          | 3,311.1 ns |  65.93 ns | 115.47 ns |  1.79 |    0.06 | 0.0191 |    1829 B |        6.72 |
 *  | CapturingGroups | go infinite          | 1,561.6 ns |  14.24 ns |  13.32 ns |  0.84 |    0.01 | 0.0172 |    1504 B |        5.53 |
 *  | NoRegex         | go infinite          |   877.1 ns |   4.71 ns |   4.18 ns |  0.47 |    0.01 | 0.0029 |     304 B |        1.12 |
 *  |                 |                      |            |           |           |       |         |        |           |             |
 *  | Sequential      | go wt(...)c 500 [42] | 4,135.0 ns |  39.83 ns |  37.26 ns |  1.00 |    0.01 | 0.0076 |    1248 B |        1.00 |
 *  | Parallel        | go wt(...)c 500 [42] | 4,391.0 ns |  86.56 ns | 193.60 ns |  1.06 |    0.05 | 0.0305 |    2808 B |        2.25 |
 *  | CapturingGroups | go wt(...)c 500 [42] | 3,907.0 ns |  24.50 ns |  22.92 ns |  0.94 |    0.01 | 0.0534 |    4800 B |        3.85 |
 *  | NoRegex         | go wt(...)c 500 [42] | 1,065.4 ns |  12.57 ns |  11.76 ns |  0.26 |    0.00 | 0.0019 |     304 B |        0.24 |
 *  |                 |                      |            |           |           |       |         |        |           |             |
 *  | Sequential      | go wt(...)00000 [78] | 5,388.7 ns |  93.92 ns |  87.85 ns |  1.00 |    0.02 | 0.0153 |    1728 B |        1.00 |
 *  | Parallel        | go wt(...)00000 [78] | 4,942.9 ns | 119.01 ns | 331.75 ns |  0.92 |    0.06 |      - |    3288 B |        1.90 |
 *  | CapturingGroups | go wt(...)00000 [78] | 5,425.3 ns |  87.01 ns |  81.39 ns |  1.01 |    0.02 | 0.0839 |    7064 B |        4.09 |
 *  | NoRegex         | go wt(...)00000 [78] | 1,185.1 ns |  11.68 ns |  10.93 ns |  0.22 |    0.00 | 0.0019 |     304 B |        0.18 |
 *  |                 |                      |            |           |           |       |         |        |           |             |
 *  | Sequential      | go wt(...)go 40 [62] | 4,940.5 ns |  57.45 ns |  53.74 ns |  1.00 |    0.01 | 0.0153 |    1488 B |        1.00 |
 *  | Parallel        | go wt(...)go 40 [62] | 4,628.8 ns |  91.57 ns | 208.54 ns |  0.94 |    0.04 | 0.0305 |    3048 B |        2.05 |
 *  | CapturingGroups | go wt(...)go 40 [62] | 5,256.5 ns |  92.27 ns |  86.31 ns |  1.06 |    0.02 | 0.0839 |    7032 B |        4.73 |
 *  | NoRegex         | go wt(...)go 40 [62] | 1,127.6 ns |   5.74 ns |   5.09 ns |  0.23 |    0.00 | 0.0019 |     304 B |        0.20 |
 *  
 *  
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2655) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *  
 *  | Method          | command              | Mean       | Error    | StdDev    | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |---------------- |--------------------- |-----------:|---------:|----------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Sequential      | go infinite          | 1,779.1 ns |  3.47 ns |   3.24 ns | 1,779.0 ns |  1.00 |    0.00 | 0.0153 |      - |     272 B |        1.00 |
 *  | Parallel        | go infinite          | 2,777.8 ns | 31.02 ns |  29.01 ns | 2,781.8 ns |  1.56 |    0.02 | 0.1068 |      - |    1826 B |        6.71 |
 *  | CapturingGroups | go infinite          | 1,388.3 ns |  7.47 ns |   6.62 ns | 1,387.1 ns |  0.78 |    0.00 | 0.0896 |      - |    1504 B |        5.53 |
 *  | NoRegex         | go infinite          |   859.2 ns |  7.75 ns |   7.25 ns |   862.1 ns |  0.48 |    0.00 | 0.0181 |      - |     304 B |        1.12 |
 *  |                 |                      |            |          |           |            |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)c 500 [42] | 3,742.4 ns | 14.00 ns |  11.69 ns | 3,740.5 ns |  1.00 |    0.00 | 0.0687 |      - |    1249 B |        1.00 |
 *  | Parallel        | go wt(...)c 500 [42] | 3,845.1 ns | 75.53 ns | 154.29 ns | 3,798.3 ns |  1.03 |    0.04 | 0.1678 |      - |    2808 B |        2.25 |
 *  | CapturingGroups | go wt(...)c 500 [42] | 3,064.8 ns | 42.70 ns |  39.94 ns | 3,054.1 ns |  0.82 |    0.01 | 0.2861 | 0.0038 |    4800 B |        3.84 |
 *  | NoRegex         | go wt(...)c 500 [42] |   964.5 ns |  1.91 ns |   1.78 ns |   964.3 ns |  0.26 |    0.00 | 0.0172 |      - |     304 B |        0.24 |
 *  |                 |                      |            |          |           |            |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)00000 [78] | 5,824.0 ns | 89.81 ns |  84.01 ns | 5,829.1 ns |  1.00 |    0.02 | 0.0992 |      - |    1731 B |        1.00 |
 *  | Parallel        | go wt(...)00000 [78] | 4,583.4 ns | 90.61 ns | 117.82 ns | 4,562.7 ns |  0.79 |    0.02 | 0.1907 |      - |    3288 B |        1.90 |
 *  | CapturingGroups | go wt(...)00000 [78] | 4,658.1 ns | 52.94 ns |  49.52 ns | 4,665.6 ns |  0.80 |    0.01 | 0.4196 | 0.0076 |    7065 B |        4.08 |
 *  | NoRegex         | go wt(...)00000 [78] | 1,090.3 ns |  1.63 ns |   1.53 ns | 1,090.1 ns |  0.19 |    0.00 | 0.0172 |      - |     304 B |        0.18 |
 *  |                 |                      |            |          |           |            |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)go 40 [62] | 4,757.1 ns | 34.79 ns |  32.54 ns | 4,760.3 ns |  1.00 |    0.01 | 0.0839 |      - |    1490 B |        1.00 |
 *  | Parallel        | go wt(...)go 40 [62] | 4,214.8 ns | 83.13 ns | 214.59 ns | 4,130.0 ns |  0.89 |    0.05 | 0.1755 |      - |    3048 B |        2.05 |
 *  | CapturingGroups | go wt(...)go 40 [62] | 4,933.6 ns | 98.56 ns | 156.33 ns | 4,935.6 ns |  1.04 |    0.03 | 0.4120 |      - |    7036 B |        4.72 |
 *  | NoRegex         | go wt(...)go 40 [62] | 1,029.6 ns |  2.05 ns |   1.92 ns | 1,029.6 ns |  0.22 |    0.00 | 0.0172 |      - |     304 B |        0.20 |
 *  
 *  
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *  
 *  | Method          | command              | Mean        | Error     | StdDev      | Median      | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |---------------- |--------------------- |------------:|----------:|------------:|------------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Sequential      | go infinite          |  2,563.0 ns |  68.07 ns |   194.20 ns |  2,594.2 ns |  1.01 |    0.11 | 0.0420 |      - |     272 B |        1.00 |
 *  | Parallel        | go infinite          |  6,524.4 ns | 218.46 ns |   644.12 ns |  6,525.5 ns |  2.56 |    0.32 | 0.2747 |      - |    1801 B |        6.62 |
 *  | CapturingGroups | go infinite          |  2,853.6 ns |  90.50 ns |   263.99 ns |  2,871.2 ns |  1.12 |    0.14 | 0.2365 |      - |    1504 B |        5.53 |
 *  | NoRegex         | go infinite          |  1,576.4 ns |  63.31 ns |   185.67 ns |  1,580.6 ns |  0.62 |    0.09 | 0.0458 |      - |     304 B |        1.12 |
 *  |                 |                      |             |           |             |             |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)c 500 [42] |  6,177.2 ns | 214.98 ns |   623.69 ns |  6,147.0 ns |  1.01 |    0.14 | 0.1831 |      - |    1248 B |        1.00 |
 *  | Parallel        | go wt(...)c 500 [42] |  9,026.4 ns | 362.03 ns | 1,044.55 ns |  8,861.3 ns |  1.48 |    0.22 | 0.4272 |      - |    2791 B |        2.24 |
 *  | CapturingGroups | go wt(...)c 500 [42] |  8,281.2 ns | 689.25 ns | 2,032.27 ns |  8,554.8 ns |  1.35 |    0.36 | 0.7629 |      - |    4800 B |        3.85 |
 *  | NoRegex         | go wt(...)c 500 [42] |  1,252.3 ns | 126.21 ns |   372.14 ns |  1,034.0 ns |  0.20 |    0.06 | 0.0477 |      - |     304 B |        0.24 |
 *  |                 |                      |             |           |             |             |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)00000 [78] |  6,045.3 ns | 366.11 ns | 1,079.48 ns |  5,998.1 ns |  1.03 |    0.26 | 0.2747 |      - |    1728 B |        1.00 |
 *  | Parallel        | go wt(...)00000 [78] |  5,884.0 ns | 182.39 ns |   529.14 ns |  5,700.4 ns |  1.00 |    0.20 | 0.5264 |      - |    3288 B |        1.90 |
 *  | CapturingGroups | go wt(...)00000 [78] |  5,372.3 ns | 333.60 ns |   983.63 ns |  5,047.7 ns |  0.92 |    0.23 | 1.1292 | 0.0229 |    7064 B |        4.09 |
 *  | NoRegex         | go wt(...)00000 [78] |    969.3 ns |  17.59 ns |    43.47 ns |    954.3 ns |  0.17 |    0.03 | 0.0477 |      - |     304 B |        0.18 |
 *  |                 |                      |             |           |             |             |       |         |        |        |           |             |
 *  | Sequential      | go wt(...)go 40 [62] |  8,626.6 ns | 432.39 ns | 1,274.90 ns |  8,814.2 ns |  1.03 |    0.23 | 0.2136 |      - |    1488 B |        1.00 |
 *  | Parallel        | go wt(...)go 40 [62] | 10,027.7 ns | 671.07 ns | 1,957.54 ns | 10,271.9 ns |  1.19 |    0.31 | 0.4578 |      - |    3038 B |        2.04 |
 *  | CapturingGroups | go wt(...)go 40 [62] | 11,519.3 ns | 505.27 ns | 1,489.81 ns | 11,364.9 ns |  1.37 |    0.29 | 1.0986 |      - |    7032 B |        4.73 |
 *  | NoRegex         | go wt(...)go 40 [62] |  1,992.7 ns | 111.59 ns |   329.02 ns |  2,012.0 ns |  0.24 |    0.06 | 0.0458 |      - |     304 B |        0.20 |
 *  
 *  
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.6.9 (22G830) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *  
 *  | Method          | command              | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |---------------- |--------------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | Sequential      | go infinite          |  9.999 us | 0.7285 us | 2.1480 us |  8.866 us |  1.04 |    0.30 | 0.0305 |     272 B |        1.00 |
 *  | Parallel        | go infinite          | 22.309 us | 0.4408 us | 0.4329 us | 22.267 us |  2.32 |    0.43 | 0.2441 |    1832 B |        6.74 |
 *  | CapturingGroups | go infinite          |  6.927 us | 0.3132 us | 0.9088 us |  7.076 us |  0.72 |    0.16 | 0.2365 |    1504 B |        5.53 |
 *  | NoRegex         | go infinite          |  4.618 us | 0.2672 us | 0.7879 us |  4.309 us |  0.48 |    0.12 | 0.0458 |     304 B |        1.12 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go wt(...)c 500 [42] | 23.630 us | 0.5262 us | 1.4841 us | 23.580 us |  1.00 |    0.10 | 0.1831 |    1248 B |        1.00 |
 *  | Parallel        | go wt(...)c 500 [42] | 28.021 us | 0.5383 us | 1.4181 us | 27.497 us |  1.19 |    0.11 | 0.4272 |    2808 B |        2.25 |
 *  | CapturingGroups | go wt(...)c 500 [42] | 23.949 us | 0.4764 us | 1.2383 us | 23.868 us |  1.02 |    0.10 | 0.7629 |    4800 B |        3.85 |
 *  | NoRegex         | go wt(...)c 500 [42] |  1.558 us | 0.0311 us | 0.0829 us |  1.529 us |  0.07 |    0.01 | 0.0477 |     304 B |        0.24 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go wt(...)00000 [78] | 23.294 us | 0.7628 us | 2.1764 us | 23.710 us |  1.01 |    0.17 | 0.2747 |    1728 B |        1.00 |
 *  | Parallel        | go wt(...)00000 [78] | 38.876 us | 0.7743 us | 1.0336 us | 38.887 us |  1.69 |    0.23 | 0.4883 |    3288 B |        1.90 |
 *  | CapturingGroups | go wt(...)00000 [78] | 21.552 us | 2.4262 us | 7.1156 us | 24.289 us |  0.94 |    0.33 | 1.1292 |    7064 B |        4.09 |
 *  | NoRegex         | go wt(...)00000 [78] |  1.991 us | 0.0444 us | 0.1244 us |  1.980 us |  0.09 |    0.01 | 0.0477 |     304 B |        0.18 |
 *  |                 |                      |           |           |           |           |       |         |        |           |             |
 *  | Sequential      | go wt(...)go 40 [62] | 23.190 us | 0.7036 us | 1.9846 us | 23.545 us |  1.01 |    0.14 | 0.2136 |    1488 B |        1.00 |
 *  | Parallel        | go wt(...)go 40 [62] | 28.145 us | 0.5608 us | 0.8731 us | 28.175 us |  1.22 |    0.14 | 0.4883 |    3048 B |        2.05 |
 *  | CapturingGroups | go wt(...)go 40 [62] | 24.677 us | 0.4849 us | 0.5389 us | 24.544 us |  1.07 |    0.12 | 1.0986 |    7032 B |        4.73 |
 *  | NoRegex         | go wt(...)go 40 [62] |  1.758 us | 0.0340 us | 0.0442 us |  1.751 us |  0.08 |    0.01 | 0.0477 |     304 B |        0.20 |
 *
 */

using BenchmarkDotNet.Attributes;
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
    }


    private void ParseNoRegex(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < rangesLength; i++)
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
            };
        }
    }
}
