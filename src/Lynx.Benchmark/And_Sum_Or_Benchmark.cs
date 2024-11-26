/*
 *
 *  BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.2.23502.2
 *    [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 *  | Method | data   | Mean           | Error       | StdDev      | Ratio | Allocated | Alloc Ratio |
 *  |------- |------- |---------------:|------------:|------------:|------:|----------:|------------:|
 *  | And    | 1      |       2.699 ns |   0.0026 ns |   0.0022 ns |  1.00 |         - |          NA |
 *  | Sum    | 1      |       2.512 ns |   0.0024 ns |   0.0022 ns |  0.93 |         - |          NA |
 *  | Or     | 1      |       2.509 ns |   0.0011 ns |   0.0009 ns |  0.93 |         - |          NA |
 *  |        |        |                |             |             |       |           |             |
 *  | And    | 10     |      32.310 ns |   0.0061 ns |   0.0047 ns |  1.00 |         - |          NA |
 *  | Sum    | 10     |      30.865 ns |   0.0096 ns |   0.0085 ns |  0.96 |         - |          NA |
 *  | Or     | 10     |      30.900 ns |   0.0336 ns |   0.0281 ns |  0.96 |         - |          NA |
 *  |        |        |                |             |             |       |           |             |
 *  | And    | 1000   |   3,304.787 ns |   1.7878 ns |   1.6723 ns |  1.00 |         - |          NA |
 *  | Sum    | 1000   |   3,218.845 ns |   1.5775 ns |   1.3984 ns |  0.97 |         - |          NA |
 *  | Or     | 1000   |   3,220.136 ns |   1.1314 ns |   0.9448 ns |  0.97 |         - |          NA |
 *  |        |        |                |             |             |       |           |             |
 *  | And    | 10000  |  32,959.613 ns |  13.5685 ns |  12.0281 ns |  1.00 |         - |          NA |
 *  | Sum    | 10000  |  32,028.850 ns |  22.3593 ns |  17.4567 ns |  0.97 |         - |          NA |
 *  | Or     | 10000  |  32,025.790 ns |  25.5429 ns |  19.9422 ns |  0.97 |         - |          NA |
 *  |        |        |                |             |             |       |           |             |
 *  | And    | 100000 | 331,090.201 ns | 563.3571 ns | 499.4014 ns |  1.00 |         - |          NA |
 *  | Sum    | 100000 | 318,408.463 ns | 229.8259 ns | 203.7346 ns |  0.96 |         - |          NA |
 *  | Or     | 100000 | 318,425.226 ns | 321.0552 ns | 300.3152 ns |  0.96 |         - |          NA |
 *
 *  BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 10 (10.0.20348.2031) (Hyper-V)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.2.23502.2
 *    [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 *  | Method | data   | Mean           | Error       | StdDev      | Ratio | Allocated | Alloc Ratio |
 *  |------- |------- |---------------:|------------:|------------:|------:|----------:|------------:|
 *  | And    | 1      |       2.343 ns |   0.0081 ns |   0.0076 ns |  1.00 |         - |          NA |
 *  | Sum    | 1      |       2.157 ns |   0.0059 ns |   0.0056 ns |  0.92 |         - |          NA |
 *  | Or     | 1      |       2.179 ns |   0.0118 ns |   0.0110 ns |  0.93 |         - |          NA |
 *  |        |        |                |             |             |       |           |             |
 *  | And    | 10     |      31.193 ns |   0.0102 ns |   0.0096 ns |  1.00 |         - |          NA |
 *  | Sum    | 10     |      34.008 ns |   0.0069 ns |   0.0065 ns |  1.09 |         - |          NA |
 *  | Or     | 10     |      34.014 ns |   0.0032 ns |   0.0026 ns |  1.09 |         - |          NA |
 *  |        |        |                |             |             |       |           |             |
 *  | And    | 1000   |   3,296.256 ns |   1.1472 ns |   1.0731 ns |  1.00 |         - |          NA |
 *  | Sum    | 1000   |   3,587.257 ns |   0.5209 ns |   0.4349 ns |  1.09 |         - |          NA |
 *  | Or     | 1000   |   3,587.079 ns |   0.4581 ns |   0.3825 ns |  1.09 |         - |          NA |
 *  |        |        |                |             |             |       |           |             |
 *  | And    | 10000  |  32,889.271 ns |   8.1494 ns |   7.6230 ns |  1.00 |         - |          NA |
 *  | Sum    | 10000  |  35,797.919 ns |  12.5595 ns |  11.1337 ns |  1.09 |         - |          NA |
 *  | Or     | 10000  |  35,790.512 ns |   8.5240 ns |   7.5563 ns |  1.09 |         - |          NA |
 *  |        |        |                |             |             |       |           |             |
 *  | And    | 100000 | 328,686.670 ns |  52.6873 ns |  46.7059 ns |  1.00 |         - |          NA |
 *  | Sum    | 100000 | 357,706.119 ns |  69.8567 ns |  58.3335 ns |  1.09 |         - |          NA |
 *  | Or     | 100000 | 357,728.441 ns | 129.5714 ns | 108.1980 ns |  1.09 |         - |          NA |
 *
 *  BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, macOS Monterey 12.6.9 (21G726) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100-rc.2.23502.2
 *    [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 *  | Method | data   | Mean           | Error         | StdDev         | Median         | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------- |------- |---------------:|--------------:|---------------:|---------------:|------:|--------:|----------:|------------:|
 *  | And    | 1      |       1.879 ns |     0.0383 ns |      0.0358 ns |       1.877 ns |  1.00 |    0.00 |         - |          NA |
 *  | Sum    | 1      |       1.839 ns |     0.0663 ns |      0.0886 ns |       1.820 ns |  1.00 |    0.06 |         - |          NA |
 *  | Or     | 1      |       2.343 ns |     0.0721 ns |      0.1963 ns |       2.296 ns |  1.24 |    0.11 |         - |          NA |
 *  |        |        |                |               |                |                |       |         |           |             |
 *  | And    | 10     |      26.677 ns |     0.5645 ns |      0.5281 ns |      26.688 ns |  1.00 |    0.00 |         - |          NA |
 *  | Sum    | 10     |      24.457 ns |     0.7485 ns |      2.1595 ns |      24.029 ns |  0.90 |    0.07 |         - |          NA |
 *  | Or     | 10     |      25.027 ns |     1.0066 ns |      2.9203 ns |      24.315 ns |  0.91 |    0.08 |         - |          NA |
 *  |        |        |                |               |                |                |       |         |           |             |
 *  | And    | 1000   |   3,264.525 ns |   128.0790 ns |    365.4167 ns |   3,213.228 ns |  1.00 |    0.00 |         - |          NA |
 *  | Sum    | 1000   |   2,626.122 ns |   100.4832 ns |    283.4147 ns |   2,578.095 ns |  0.82 |    0.13 |         - |          NA |
 *  | Or     | 1000   |   2,512.625 ns |    55.9297 ns |    163.1495 ns |   2,501.782 ns |  0.78 |    0.10 |         - |          NA |
 *  |        |        |                |               |                |                |       |         |           |             |
 *  | And    | 10000  |  30,062.442 ns |   600.2932 ns |    714.6063 ns |  30,029.103 ns |  1.00 |    0.00 |         - |          NA |
 *  | Sum    | 10000  |  29,958.601 ns | 1,616.3465 ns |  4,740.4645 ns |  28,839.835 ns |  0.90 |    0.12 |         - |          NA |
 *  | Or     | 10000  |  25,626.033 ns |   503.5787 ns |    599.4746 ns |  25,721.695 ns |  0.85 |    0.03 |         - |          NA |
 *  |        |        |                |               |                |                |       |         |           |             |
 *  | And    | 100000 | 238,530.814 ns | 4,363.8294 ns | 11,945.9125 ns | 233,501.724 ns |  1.00 |    0.00 |         - |          NA |
 *  | Sum    | 100000 | 221,279.707 ns | 2,918.0687 ns |  2,729.5632 ns | 221,003.871 ns |  0.85 |    0.04 |         - |          NA |
 *  | Or     | 100000 | 222,342.747 ns | 1,770.4176 ns |  1,569.4291 ns | 222,450.007 ns |  0.85 |    0.03 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class And_Sum_Or_Benchmark : BaseBenchmark
{
    public static IEnumerable<int> Data => [1, 10, 1_000, 10_000, 100_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public bool And(int data)
    {
        bool sum = false;
        for (int i = 0; i < data; ++i)
        {
            int a = i % 2;
            int b = i % 3;
            int c = i % 6;

            sum ^= (a == 0 && b == 0 && c == 0);
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool Sum(int data)
    {
        bool sum = false;
        for (int i = 0; i < data; ++i)
        {
            int a = i % 2;
            int b = i % 3;
            int c = i % 6;

            sum ^= (a + b + c == 0);
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool Or(int data)
    {
        bool sum = false;
        for (int i = 0; i < data; ++i)
        {
            int a = i % 2;
            int b = i % 3;
            int c = i % 6;

            sum ^= ((a | b | c) == 0);
        }

        return sum;
    }
}
