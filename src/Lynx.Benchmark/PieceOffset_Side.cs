/*
 * Pretty much inconclusive
 *
 *  |     Method | iterations |           Mean |       Error |        StdDev |         Median | Ratio | RatioSD | Allocated |
 *  |----------- |----------- |---------------:|------------:|--------------:|---------------:|------:|--------:|----------:|
 *  |      Array |          1 |      2.4108 ns |   0.0344 ns |     0.0287 ns |      2.4078 ns | 1.000 |    0.00 |         - |
 *  | Branchless |          1 |      0.0001 ns |   0.0001 ns |     0.0001 ns |      0.0000 ns | 0.000 |    0.00 |         - |
 *  |            |            |                |             |               |                |       |         |           |
 *  |      Array |         10 |     11.6373 ns |   0.0723 ns |     0.0641 ns |     11.6089 ns |  1.00 |    0.00 |         - |
 *  | Branchless |         10 |      3.5845 ns |   0.0423 ns |     0.0353 ns |      3.5764 ns |  0.31 |    0.00 |         - |
 *  |            |            |                |             |               |                |       |         |           |
 *  |      Array |       1000 |    811.3248 ns |   6.3741 ns |     5.6504 ns |    808.2047 ns |  1.00 |    0.00 |         - |
 *  | Branchless |       1000 |    347.1858 ns |   0.7916 ns |     0.7405 ns |    347.0516 ns |  0.43 |    0.00 |         - |
 *  |            |            |                |             |               |                |       |         |           |
 *  |      Array |      10000 |  7,940.8334 ns |  21.7690 ns |    20.3628 ns |  7,939.5035 ns |  1.00 |    0.00 |         - |
 *  | Branchless |      10000 |  3,420.9737 ns |  48.5945 ns |    40.5786 ns |  3,401.1097 ns |  0.43 |    0.00 |         - |
 *  |            |            |                |             |               |                |       |         |           |
 *  |      Array |     100000 | 79,020.9464 ns | 118.8401 ns |   105.3487 ns | 79,027.0752 ns |  1.00 |    0.00 |         - |
 *  | Branchless |     100000 | 34,815.7739 ns | 632.6429 ns | 1,248.7754 ns | 34,251.8158 ns |  0.46 |    0.02 |         - |
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
