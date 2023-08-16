/*
 *

 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class TranspositionTableIndex : BaseBenchmark
{
    public static IEnumerable<int> Data => new int[]
    {
        2 * 1024 * 1024,
        4 * 1024 * 1024,
        8 * 1024 * 1024,
        16 * 1024 * 1024,
        32 * 1024 * 1024,
        64 * 1024 * 1024,
        128 * 1024 * 1024,
        256 * 1024 * 1024,
        512 * 1024 * 1024,
        1024 * 1024 * 1024
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public long Modulo(int size)
    {
        var mask = size - 1;

        long total = 0;
        for (long i = 0; i < 10_000L; ++i)
        {
            total += i % size;
        }

        return total + mask;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long And(int size)
    {
        var mask = size - 1;

        long total = 0;
        for (long i = 0; i < 10_000L; ++i)
        {
            total += i & mask;
        }

        return total + mask;
    }
}
