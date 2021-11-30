/*
 * Pretty much inconclusive
 *
 *  |           Method | iterations |           Mean |         Error |        StdDev |         Median | Ratio | RatioSD | Allocated |
 *  |----------------- |----------- |---------------:|--------------:|--------------:|---------------:|------:|--------:|----------:|
 *  | ThirdConditional |          1 |      0.0000 ns |     0.0000 ns |     0.0000 ns |      0.0000 ns |     ? |       ? |         - |
 *  |       Branchless |          1 |      0.0053 ns |     0.0116 ns |     0.0108 ns |      0.0000 ns |     ? |       ? |         - |
 *  |                  |            |                |               |               |                |       |         |           |
 *  | ThirdConditional |         10 |      3.6113 ns |     0.0294 ns |     0.0275 ns |      3.6002 ns |  1.00 |    0.00 |         - |
 *  |       Branchless |         10 |      3.6180 ns |     0.0275 ns |     0.0229 ns |      3.6204 ns |  1.00 |    0.01 |         - |
 *  |                  |            |                |               |               |                |       |         |           |
 *  | ThirdConditional |       1000 |    382.4481 ns |    16.1206 ns |    45.9929 ns |    359.7989 ns |  1.00 |    0.00 |         - |
 *  |       Branchless |       1000 |    347.2697 ns |     1.0720 ns |     0.8952 ns |    347.3704 ns |  0.85 |    0.04 |         - |
 *  |                  |            |                |               |               |                |       |         |           |
 *  | ThirdConditional |      10000 |  3,395.3388 ns |    10.5327 ns |     9.3370 ns |  3,391.9155 ns |  1.00 |    0.00 |         - |
 *  |       Branchless |      10000 |  3,397.4954 ns |    10.6733 ns |     8.3330 ns |  3,397.0459 ns |  1.00 |    0.00 |         - |
 *  |                  |            |                |               |               |                |       |         |           |
 *  | ThirdConditional |     100000 | 34,608.9655 ns |   504.2024 ns |   580.6402 ns | 34,499.9786 ns |  1.00 |    0.00 |         - |
 *  |       Branchless |     100000 | 39,079.1962 ns | 1,431.5247 ns | 4,220.8828 ns | 38,997.9370 ns |  1.15 |    0.06 |         - |
 *
 */

using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

/// <summary>
/// <see cref="Utils.PieceOffset(int)"/>
/// </summary>
public static class PieceOffsetByBooleanImplementations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ThirdConditional(bool isWhite) => isWhite ? 0 : 6;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Branchless(bool isWhite) => 6 - (6 * Unsafe.As<bool, byte>(ref isWhite));
}
public class PieceOffset_Boolean : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int ThirdConditional(int iterations)
    {
        var result = 0;
        for (int i = 0; i < iterations; ++i)
        {
            result += PieceOffsetByBooleanImplementations.ThirdConditional(true);
            result += PieceOffsetByBooleanImplementations.ThirdConditional(false);
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int Branchless(int iterations)
    {
        var result = 0;
        for (int i = 0; i < iterations; ++i)
        {
            result += PieceOffsetByBooleanImplementations.Branchless(true);
            result += PieceOffsetByBooleanImplementations.Branchless(false);
        }

        return result;
    }
}