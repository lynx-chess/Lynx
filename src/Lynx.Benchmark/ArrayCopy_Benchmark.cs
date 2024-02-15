/*
 * Since we're copying such small arrays, there's little effect in trying to optimize it
 * https://devblogs.microsoft.com/dotnet/hardware-intrinsics-in-net-core/
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.200
 *    [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *
 *  | Method             | array       | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------- |------------ |-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | ArrayDotCopy       | UInt64[124] |  87.670 ns | 1.3851 ns | 1.2956 ns |  1.00 |    0.00 | 0.0120 |    1016 B |        1.00 |
 *  | ManualLoop         | UInt64[124] | 157.961 ns | 2.1946 ns | 1.9455 ns |  1.80 |    0.03 | 0.0119 |    1016 B |        1.00 |
 *  | ManualLoopSpan     | UInt64[124] | 122.212 ns | 1.9715 ns | 1.8441 ns |  1.39 |    0.03 | 0.0119 |    1016 B |        1.00 |
 *  | UnrolledManualLoop | UInt64[124] | 117.234 ns | 1.1972 ns | 0.9997 ns |  1.34 |    0.02 | 0.0119 |    1016 B |        1.00 |
 *  | BlockCopy          | UInt64[124] | 100.794 ns | 1.7532 ns | 1.5542 ns |  1.15 |    0.02 | 0.0120 |    1016 B |        1.00 |
 *  | ArrayDotCopy       | UInt64[12]  |  18.267 ns | 0.1581 ns | 0.1479 ns |  0.21 |    0.00 | 0.0014 |     120 B |        0.12 |
 *  | ManualLoop         | UInt64[12]  |  20.709 ns | 0.1286 ns | 0.1140 ns |  0.24 |    0.00 | 0.0014 |     120 B |        0.12 |
 *  | ManualLoopSpan     | UInt64[12]  |  18.663 ns | 0.2058 ns | 0.1926 ns |  0.21 |    0.00 | 0.0014 |     120 B |        0.12 |
 *  | UnrolledManualLoop | UInt64[12]  |  19.069 ns | 0.1913 ns | 0.1696 ns |  0.22 |    0.00 | 0.0014 |     120 B |        0.12 |
 *  | BlockCopy          | UInt64[12]  |  27.888 ns | 0.2316 ns | 0.2166 ns |  0.32 |    0.01 | 0.0014 |     120 B |        0.12 |
 *  | ArrayDotCopy       | UInt64[3]   |  12.570 ns | 0.1376 ns | 0.1220 ns |  0.14 |    0.00 | 0.0006 |      48 B |        0.05 |
 *  | ManualLoop         | UInt64[3]   |   9.899 ns | 0.0816 ns | 0.0764 ns |  0.11 |    0.00 | 0.0006 |      48 B |        0.05 |
 *  | ManualLoopSpan     | UInt64[3]   |  11.081 ns | 0.0956 ns | 0.0847 ns |  0.13 |    0.00 | 0.0006 |      48 B |        0.05 |
 *  | UnrolledManualLoop | UInt64[3]   |  11.842 ns | 0.0754 ns | 0.0630 ns |  0.14 |    0.00 | 0.0006 |      48 B |        0.05 |
 *  | BlockCopy          | UInt64[3]   |  22.510 ns | 0.1301 ns | 0.1217 ns |  0.26 |    0.00 | 0.0006 |      48 B |        0.05 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2227) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.200
 *    [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *
 *  | Method             | array       | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------- |------------ |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | ArrayDotCopy       | UInt64[124] | 46.395 ns | 0.9933 ns | 1.7397 ns |  1.00 |    0.00 | 0.0607 |    1016 B |        1.00 |
 *  | ManualLoop         | UInt64[124] | 78.095 ns | 1.6117 ns | 1.9186 ns |  1.66 |    0.07 | 0.0607 |    1016 B |        1.00 |
 *  | ManualLoopSpan     | UInt64[124] | 78.240 ns | 0.8534 ns | 0.7983 ns |  1.68 |    0.04 | 0.0607 |    1016 B |        1.00 |
 *  | UnrolledManualLoop | UInt64[124] | 73.794 ns | 0.6488 ns | 0.6069 ns |  1.58 |    0.04 | 0.0607 |    1016 B |        1.00 |
 *  | BlockCopy          | UInt64[124] | 51.263 ns | 0.6510 ns | 0.5436 ns |  1.10 |    0.03 | 0.0607 |    1016 B |        1.00 |
 *  | ArrayDotCopy       | UInt64[12]  | 12.321 ns | 0.1434 ns | 0.1271 ns |  0.26 |    0.01 | 0.0072 |     120 B |        0.12 |
 *  | ManualLoop         | UInt64[12]  | 10.971 ns | 0.1965 ns | 0.1838 ns |  0.24 |    0.01 | 0.0072 |     120 B |        0.12 |
 *  | ManualLoopSpan     | UInt64[12]  | 12.028 ns | 0.2842 ns | 0.2519 ns |  0.26 |    0.01 | 0.0072 |     120 B |        0.12 |
 *  | UnrolledManualLoop | UInt64[12]  | 12.213 ns | 0.0595 ns | 0.0528 ns |  0.26 |    0.01 | 0.0072 |     120 B |        0.12 |
 *  | BlockCopy          | UInt64[12]  | 17.839 ns | 0.1025 ns | 0.0909 ns |  0.38 |    0.01 | 0.0072 |     120 B |        0.12 |
 *  | ArrayDotCopy       | UInt64[3]   |  9.584 ns | 0.0435 ns | 0.0363 ns |  0.21 |    0.01 | 0.0029 |      48 B |        0.05 |
 *  | ManualLoop         | UInt64[3]   |  7.244 ns | 0.2108 ns | 0.1869 ns |  0.16 |    0.01 | 0.0029 |      48 B |        0.05 |
 *  | ManualLoopSpan     | UInt64[3]   |  7.573 ns | 0.0313 ns | 0.0262 ns |  0.16 |    0.00 | 0.0029 |      48 B |        0.05 |
 *  | UnrolledManualLoop | UInt64[3]   |  8.000 ns | 0.0229 ns | 0.0191 ns |  0.17 |    0.00 | 0.0029 |      48 B |        0.05 |
 *  | BlockCopy          | UInt64[3]   | 15.352 ns | 0.0910 ns | 0.0852 ns |  0.33 |    0.01 | 0.0029 |      48 B |        0.05 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Sonoma 14.2.1 (23C71) [Darwin 23.2.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.200
 *    [Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
 *
 *  | Method             | array       | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------- |------------ |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | ArrayDotCopy       | UInt64[124] | 49.995 ns | 1.0477 ns | 1.9158 ns |  1.00 |    0.00 | 0.1619 |    1016 B |        1.00 |
 *  | ManualLoop         | UInt64[124] | 80.462 ns | 0.3586 ns | 0.3355 ns |  1.56 |    0.07 | 0.1619 |    1016 B |        1.00 |
 *  | ManualLoopSpan     | UInt64[124] | 81.766 ns | 0.5302 ns | 0.4140 ns |  1.60 |    0.07 | 0.1619 |    1016 B |        1.00 |
 *  | UnrolledManualLoop | UInt64[124] | 58.113 ns | 0.1795 ns | 0.1401 ns |  1.13 |    0.05 | 0.1619 |    1016 B |        1.00 |
 *  | BlockCopy          | UInt64[124] | 54.379 ns | 1.0284 ns | 0.8029 ns |  1.06 |    0.06 | 0.1619 |    1016 B |        1.00 |
 *  | ArrayDotCopy       | UInt64[12]  |  9.104 ns | 0.0499 ns | 0.0389 ns |  0.18 |    0.01 | 0.0191 |     120 B |        0.12 |
 *  | ManualLoop         | UInt64[12]  | 10.054 ns | 0.0303 ns | 0.0283 ns |  0.20 |    0.01 | 0.0191 |     120 B |        0.12 |
 *  | ManualLoopSpan     | UInt64[12]  | 10.689 ns | 0.0222 ns | 0.0185 ns |  0.21 |    0.01 | 0.0191 |     120 B |        0.12 |
 *  | UnrolledManualLoop | UInt64[12]  |  9.004 ns | 0.0185 ns | 0.0164 ns |  0.18 |    0.01 | 0.0191 |     120 B |        0.12 |
 *  | BlockCopy          | UInt64[12]  | 14.652 ns | 0.0471 ns | 0.0418 ns |  0.29 |    0.01 | 0.0191 |     120 B |        0.12 |
 *  | ArrayDotCopy       | UInt64[3]   |  6.209 ns | 0.0193 ns | 0.0171 ns |  0.12 |    0.01 | 0.0076 |      48 B |        0.05 |
 *  | ManualLoop         | UInt64[3]   |  5.144 ns | 0.0204 ns | 0.0160 ns |  0.10 |    0.00 | 0.0076 |      48 B |        0.05 |
 *  | ManualLoopSpan     | UInt64[3]   |  5.650 ns | 0.0218 ns | 0.0193 ns |  0.11 |    0.00 | 0.0076 |      48 B |        0.05 |
 *  | UnrolledManualLoop | UInt64[3]   |  5.941 ns | 0.0186 ns | 0.0165 ns |  0.12 |    0.00 | 0.0076 |      48 B |        0.05 |
 *  | BlockCopy          | UInt64[3]   | 11.939 ns | 0.0801 ns | 0.0669 ns |  0.23 |    0.01 | 0.0076 |      48 B |        0.05 |
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

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong[] BlockCopy(ulong[] array)
    {
        return BlockCopyImpl(array);
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

    private static ulong[] BlockCopyImpl(ulong[] array)
    {
        const int ulongSize = sizeof(ulong);
        var result = new ulong[array.Length];
        Buffer.BlockCopy(array, 0, result, 0, ulongSize * array.Length);

        return result;
    }
}
