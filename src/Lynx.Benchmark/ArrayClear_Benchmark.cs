using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class ArrayClear_Benchmark : BaseBenchmark
{
#pragma warning disable S2365 // Properties should not make collection or array copies
    public static IEnumerable<int[]> Data => [
        [1, 2, 3],
        [.. Enumerable.Range(0,12)],
        [.. Enumerable.Range(0,128)],
        [.. Enumerable.Range(0,192)],
        [.. Enumerable.Range(0,256)],
        [.. Enumerable.Range(0,256)],
        [.. Enumerable.Range(0,512)],
        [.. Enumerable.Range(0,1024)],
        EvaluationPSQTs._packedPSQT
    ];
#pragma warning restore S2365 // Properties should not make collection or array copies

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int[] ArrayDotClear(int[] array)
    {
        Array.Clear(array);

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] ManualLoop(int[] array)
    {
        for (int i = 0; i < array.Length; ++i)
        {
            array[i] = 0;
        }

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] SpanClear(int[] array)
    {
        array.AsSpan().Clear();

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] UnrolledManualLoop(int[] array)
    {
        int i = 0;
        int lastBlockIndex = array.Length - (array.Length % 4);

        // Pin source so we can elide the bounds checks
        unsafe
        {
            fixed (int* pSource = array)
            {
                while (i < lastBlockIndex)
                {
                    array[i] = 0;
                    array[i + 1] = 0;
                    array[i + 2] = 0;
                    array[i + 3] = 0;
                    i += 4;
                }
                while (i < array.Length)
                {
                    array[i] = 0;
                    ++i;
                }
            }
        }

        return array;
    }
}
