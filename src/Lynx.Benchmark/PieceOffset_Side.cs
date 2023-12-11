/*
 *
 *  BenchmarkDotNet v0.13.11, Windows 11 (10.0.22621.2715/22H2/2022Update/SunValley2)
 *  11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
 *
 *
 *  | Method           | iterations | Mean           | Error       | StdDev        | Median         | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------------- |----------- |---------------:|------------:|--------------:|---------------:|------:|--------:|----------:|------------:|
 *  | Array            | 1          |      0.1565 ns |   0.0286 ns |     0.0500 ns |      0.1599 ns |  1.00 |    0.00 |         - |          NA |
 *  | ThirdConditional | 1          |      0.0064 ns |   0.0100 ns |     0.0205 ns |      0.0000 ns |  0.08 |    0.24 |         - |          NA |
 *  | Branchless       | 1          |      0.0395 ns |   0.0239 ns |     0.0224 ns |      0.0447 ns |  0.32 |    0.17 |         - |          NA |
 *  |                  |            |                |             |               |                |       |         |           |             |
 *  | Array            | 10         |      3.2247 ns |   0.0613 ns |     0.0573 ns |      3.2107 ns |  1.00 |    0.00 |         - |          NA |
 *  | ThirdConditional | 10         |      2.7925 ns |   0.0813 ns |     0.1403 ns |      2.7540 ns |  0.84 |    0.04 |         - |          NA |
 *  | Branchless       | 10         |      2.3830 ns |   0.0739 ns |     0.0617 ns |      2.3891 ns |  0.74 |    0.02 |         - |          NA |
 *  |                  |            |                |             |               |                |       |         |           |             |
 *  | Array            | 1000       |    450.1945 ns |   8.9648 ns |    24.5410 ns |    441.3498 ns |  1.00 |    0.00 |         - |          NA |
 *  | ThirdConditional | 1000       |    273.4925 ns |   1.8315 ns |     1.6236 ns |    273.4851 ns |  0.56 |    0.02 |         - |          NA |
 *  | Branchless       | 1000       |    273.0186 ns |   1.4802 ns |     1.3122 ns |    273.1652 ns |  0.56 |    0.02 |         - |          NA |
 *  |                  |            |                |             |               |                |       |         |           |             |
 *  | Array            | 10000      |  4,282.7045 ns |  38.5052 ns |    34.1339 ns |  4,280.9650 ns |  1.00 |    0.00 |         - |          NA |
 *  | ThirdConditional | 10000      |  2,658.4250 ns |   7.7976 ns |     6.9123 ns |  2,659.5709 ns |  0.62 |    0.01 |         - |          NA |
 *  | Branchless       | 10000      |  2,658.7172 ns |   8.1112 ns |     6.7733 ns |  2,656.3129 ns |  0.62 |    0.01 |         - |          NA |
 *  |                  |            |                |             |               |                |       |         |           |             |
 *  | Array            | 100000     | 43,869.2741 ns | 854.9116 ns | 1,111.6266 ns | 43,578.9551 ns |  1.00 |    0.00 |         - |          NA |
 *  | ThirdConditional | 100000     | 26,762.0113 ns | 284.1472 ns |   251.8891 ns | 26,657.0923 ns |  0.60 |    0.02 |         - |          NA |
 *  | Branchless       | 100000     | 26,599.6741 ns |  61.2763 ns |    54.3198 ns | 26,605.4535 ns |  0.60 |    0.01 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

/// <summary>
/// <see cref="Utils.PieceOffset(int)"/>
/// </summary>
public static class PieceOffsetBySideImplementations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ThirdConditional(Side side) => side == Side.White ? 0 : 6;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Branchless(Side side) => 6 - (6 * (int)side);

    public static readonly int[] Array = new[] { 6, 0 };
}

public class PieceOffset_Side : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Array(int iterations)
    {
        var result = 0;
        for (int i = 0; i < iterations; ++i)
        {
            result += PieceOffsetBySideImplementations.Array[(int)Side.Black];
            result += PieceOffsetBySideImplementations.Array[(int)Side.White];
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int ThirdConditional(int iterations)
    {
        var result = 0;
        for (int i = 0; i < iterations; ++i)
        {
            result += PieceOffsetBySideImplementations.ThirdConditional(Side.Black);
            result += PieceOffsetBySideImplementations.ThirdConditional(Side.White);
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
            result += PieceOffsetBySideImplementations.Branchless(Side.Black);
            result += PieceOffsetBySideImplementations.Branchless(Side.White);
        }

        return result;
    }
}
