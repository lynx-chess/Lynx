/*
 * More or less the same
 *
 *  BenchmarkDotNet v0.13.7, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *
 *
 *  |         Method | iterations |           Mean |      Error |    StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------- |----------- |---------------:|-----------:|----------:|------:|--------:|----------:|------------:|
 *  | LessAndGreater |          1 |      0.2141 ns |  0.0041 ns | 0.0039 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |          1 |      0.2109 ns |  0.0053 ns | 0.0047 ns |  0.99 |    0.02 |         - |          NA |
 *  |                |            |                |            |           |       |         |           |             |
 *  | LessAndGreater |         10 |      8.4003 ns |  0.0065 ns | 0.0058 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |         10 |      6.2649 ns |  0.0708 ns | 0.0662 ns |  0.75 |    0.01 |         - |          NA |
 *  |                |            |                |            |           |       |         |           |             |
 *  | LessAndGreater |       1000 |    807.6876 ns |  0.1891 ns | 0.1769 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |       1000 |    811.9714 ns |  0.1302 ns | 0.1154 ns |  1.01 |    0.00 |         - |          NA |
 *  |                |            |                |            |           |       |         |           |             |
 *  | LessAndGreater |      10000 |  8,041.9955 ns |  1.0335 ns | 0.9667 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |      10000 |  8,040.8007 ns |  0.3681 ns | 0.2874 ns |  1.00 |    0.00 |         - |          NA |
 *  |                |            |                |            |           |       |         |           |             |
 *  | LessAndGreater |     100000 | 80,327.6609 ns | 10.4212 ns | 9.2382 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |     100000 | 80,336.0808 ns |  9.0273 ns | 8.0025 ns |  1.00 |    0.00 |         - |          NA |
 *
 *  BenchmarkDotNet v0.13.7, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *
 *
 *  |         Method | iterations |           Mean |       Error |      StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------- |----------- |---------------:|------------:|------------:|------:|--------:|----------:|------------:|
 *  | LessAndGreater |          1 |      0.0080 ns |   0.0048 ns |   0.0045 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |          1 |      0.3615 ns |   0.0071 ns |   0.0066 ns | 78.39 |   91.65 |         - |          NA |
 *  |                |            |                |             |             |       |         |           |             |
 *  | LessAndGreater |         10 |      7.5929 ns |   0.0218 ns |   0.0193 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |         10 |      7.5130 ns |   0.0219 ns |   0.0204 ns |  0.99 |    0.00 |         - |          NA |
 *  |                |            |                |             |             |       |         |           |             |
 *  | LessAndGreater |       1000 |    681.5867 ns |   1.7133 ns |   1.6026 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |       1000 |    680.6993 ns |   0.9369 ns |   0.7823 ns |  1.00 |    0.00 |         - |          NA |
 *  |                |            |                |             |             |       |         |           |             |
 *  | LessAndGreater |      10000 |  6,737.0490 ns |  13.3187 ns |  11.8067 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |      10000 |  6,742.4636 ns |  15.3367 ns |  14.3459 ns |  1.00 |    0.00 |         - |          NA |
 *  |                |            |                |             |             |       |         |           |             |
 *  | LessAndGreater |     100000 | 67,442.2046 ns | 219.1631 ns | 205.0053 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |     100000 | 67,137.2410 ns | 142.3049 ns | 126.1496 ns |  1.00 |    0.00 |         - |          NA |
 *
 *  BenchmarkDotNet v0.13.7, macOS Monterey 12.6.8 (21G725) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *
 *
 *  |         Method | iterations |           Mean |         Error |        StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------- |----------- |---------------:|--------------:|--------------:|------:|--------:|----------:|------------:|
 *  | LessAndGreater |          1 |      0.6071 ns |     0.0619 ns |     0.1814 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |          1 |      0.9989 ns |     0.0959 ns |     0.2826 ns |  1.75 |    0.60 |         - |          NA |
 *  |                |            |                |               |               |       |         |           |             |
 *  | LessAndGreater |         10 |     10.0495 ns |     0.2383 ns |     0.4975 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |         10 |      9.5443 ns |     0.2428 ns |     0.7158 ns |  0.99 |    0.10 |         - |          NA |
 *  |                |            |                |               |               |       |         |           |             |
 *  | LessAndGreater |       1000 |    697.7671 ns |    13.9922 ns |    25.9354 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |       1000 |    664.8828 ns |    10.7921 ns |     9.5669 ns |  0.93 |    0.04 |         - |          NA |
 *  |                |            |                |               |               |       |         |           |             |
 *  | LessAndGreater |      10000 |  6,522.8467 ns |    54.0407 ns |    45.1264 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |      10000 |  6,692.5055 ns |   105.3049 ns |    98.5023 ns |  1.02 |    0.02 |         - |          NA |
 *  |                |            |                |               |               |       |         |           |             |
 *  | LessAndGreater |     100000 | 57,475.5212 ns |   982.9700 ns |   919.4708 ns |  1.00 |    0.00 |         - |          NA |
 *  |            Abs |     100000 | 66,456.0062 ns | 1,149.4625 ns | 1,018.9685 ns |  1.16 |    0.03 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class CheckmateDetectionLimits_Benchmark : BaseBenchmark
{
    public static IEnumerable<int> Data => [1, 10, 1_000, 10_000, 100_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public bool LessAndGreater(int iterations)
    {
        bool result = false;

        for (int i = 0; i < iterations; ++i)
        {
            result ^= (iterations < EvaluationConstants.PositiveCheckmateDetectionLimit && iterations > -EvaluationConstants.PositiveCheckmateDetectionLimit);
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool Abs(int iterations)
    {
        bool result = false;

        for (int i = 0; i < iterations; ++i)
        {
            result ^= (Math.Abs(iterations) < EvaluationConstants.PositiveCheckmateDetectionLimit);
        }

        return result;
    }
}
