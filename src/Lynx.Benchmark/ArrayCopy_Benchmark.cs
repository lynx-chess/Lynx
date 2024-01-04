/*
 * Since we're copying such small arrays, there's little effect in trying to optimize it
 * https://devblogs.microsoft.com/dotnet/hardware-intrinsics-in-net-core/
 *
 *  |             Method |       array |      Mean |    Error |    StdDev |    Median | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
 *  |------------------- |------------ |----------:|---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  |       ArrayDotCopy | UInt64[124] | 128.86 ns | 2.637 ns |  4.549 ns | 129.23 ns |  1.00 |    0.00 | 0.4857 |    1016 B |        1.00 |
 *  |         ManualLoop | UInt64[124] | 162.40 ns | 3.156 ns |  3.241 ns | 163.05 ns |  1.26 |    0.06 | 0.4857 |    1016 B |        1.00 |
 *  |     ManualLoopSpan | UInt64[124] | 166.54 ns | 2.453 ns |  2.410 ns | 167.72 ns |  1.29 |    0.05 | 0.4857 |    1016 B |        1.00 |
 *  | UnrolledManualLoop | UInt64[124] | 174.80 ns | 3.512 ns |  4.567 ns | 173.76 ns |  1.35 |    0.04 | 0.4857 |    1016 B |        1.00 |
 *  |       ArrayDotCopy |  UInt64[12] |  29.47 ns | 1.571 ns |  4.632 ns |  28.37 ns |  0.23 |    0.04 | 0.0573 |     120 B |        0.12 |
 *  |         ManualLoop |  UInt64[12] |  36.80 ns | 4.377 ns | 12.906 ns |  31.83 ns |  0.22 |    0.06 | 0.0574 |     120 B |        0.12 |
 *  |     ManualLoopSpan |  UInt64[12] |  34.07 ns | 1.933 ns |  5.546 ns |  33.67 ns |  0.26 |    0.04 | 0.0573 |     120 B |        0.12 |
 *  | UnrolledManualLoop |  UInt64[12] |  38.37 ns | 3.198 ns |  9.073 ns |  36.80 ns |  0.29 |    0.06 | 0.0573 |     120 B |        0.12 |
 *  |       ArrayDotCopy |   UInt64[3] |  21.73 ns | 2.012 ns |  5.901 ns |  20.82 ns |  0.14 |    0.02 | 0.0229 |      48 B |        0.05 |
 *  |         ManualLoop |   UInt64[3] |  18.51 ns | 1.880 ns |  5.513 ns |  17.67 ns |  0.14 |    0.05 | 0.0229 |      48 B |        0.05 |
 *  |     ManualLoopSpan |   UInt64[3] |  18.48 ns | 1.498 ns |  4.347 ns |  18.22 ns |  0.13 |    0.03 | 0.0229 |      48 B |        0.05 |
 *  | UnrolledManualLoop |   UInt64[3] |  19.03 ns | 1.289 ns |  3.761 ns |  18.86 ns |  0.16 |    0.03 | 0.0229 |      48 B |        0.05 |
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;
public class ArrayCopy_Benchmark : BaseBenchmark
{
    public static IEnumerable<ulong[]> Data => new List<ulong[]> {
        new ulong[]{ 1, 2, 3 },
        new ulong[]{ 1, 2, 3, 4, 5, 6 ,7 ,8 ,9 ,10, 11, 12 },
        new ulong[]{
            01, 02, 03, 04, 05, 06 ,07 ,08 ,09 ,10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 45, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58 ,59 ,60,
            61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
            81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 00,
            01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15 ,16, 17 ,18, 19, 20,
            21, 22, 23, 24 },
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ulong[] ArrayDotCopy(ulong[] array)
    {
        return ArrayDotCopyImpl(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong[] ManualLoop(ulong[] array)
    {
        return ManualLoopImpl(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong[] ManualLoopSpan(ulong[] array)
    {
        return ManualLoopImplSpan(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong[] UnrolledManualLoop(ulong[] array)
    {
        return UnrolledManualLoopImpl(array);
    }

    private static ulong[] ArrayDotCopyImpl(ulong[] array)
    {
        var result = new ulong[array.Length];
        Array.Copy(array, result, array.Length);

        return result;
    }

    private static ulong[] ManualLoopImpl(ulong[] array)
    {
        var result = new ulong[array.Length];
        for (int i = 0; i < array.Length; ++i)
        {
            result[i] = array[i];
        }

        return result;
    }

    private static ulong[] ManualLoopImplSpan(ReadOnlySpan<ulong> array)
    {
        var result = new ulong[array.Length];
        for (int i = 0; i < array.Length; ++i)
        {
            result[i] = array[i];
        }

        return result;
    }

    private static unsafe ulong[] UnrolledManualLoopImpl(ReadOnlySpan<ulong> array)
    {
        var result = new ulong[array.Length];

        int i = 0;
        int lastBlockIndex = array.Length - (array.Length % 4);

        // Pin source so we can elide the bounds checks
        fixed (ulong* pSource = array)
        {
            while (i < lastBlockIndex)
            {
                result[i] = pSource[i];
                result[i + 1] = pSource[i + 1];
                result[i + 2] = pSource[i + 2];
                result[i + 3] = pSource[i + 3];
                i += 4;
            }
            while (i < array.Length)
            {
                result[i] = pSource[i];
                ++i;
            }
        }

        return result;
    }
}
