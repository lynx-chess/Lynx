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
 *
 *
 *  With ints, so that we can have a more handy big array, it also proves Array.Copy is the best option
 *
 *   BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *   AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *   .NET SDK 8.0.401
 *     [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *     DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *   | Method             | array        | Mean          | Error       | StdDev      | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *   |------------------- |------------- |--------------:|------------:|------------:|-------:|--------:|-------:|----------:|------------:|
 *   | ArrayDotCopy       | Int32[124]   |     51.656 ns |   0.4764 ns |   0.4456 ns |   1.00 |    0.01 | 0.0062 |     520 B |        1.00 |
 *   | ManualLoop         | Int32[124]   |     89.307 ns |   0.7565 ns |   0.6706 ns |   1.73 |    0.02 | 0.0062 |     520 B |        1.00 |
 *   | ManualLoopSpan     | Int32[124]   |     88.945 ns |   1.4542 ns |   1.2144 ns |   1.72 |    0.03 | 0.0062 |     520 B |        1.00 |
 *   | UnrolledManualLoop | Int32[124]   |     83.226 ns |   0.8523 ns |   0.7555 ns |   1.61 |    0.02 | 0.0062 |     520 B |        1.00 |
 *   | ArrayDotCopy       | Int32[12]    |     13.969 ns |   0.1242 ns |   0.1037 ns |   0.27 |    0.00 | 0.0008 |      72 B |        0.14 |
 *   | ManualLoop         | Int32[12]    |     13.871 ns |   0.1933 ns |   0.1714 ns |   0.27 |    0.00 | 0.0008 |      72 B |        0.14 |
 *   | ManualLoopSpan     | Int32[12]    |     14.783 ns |   0.1452 ns |   0.1358 ns |   0.29 |    0.00 | 0.0008 |      72 B |        0.14 |
 *   | UnrolledManualLoop | Int32[12]    |     15.562 ns |   0.1366 ns |   0.1278 ns |   0.30 |    0.00 | 0.0008 |      72 B |        0.14 |
 *   | ArrayDotCopy       | Int32[17664] |  5,240.555 ns |  49.8248 ns |  44.1684 ns | 101.46 |    1.18 | 0.8392 |   70680 B |      135.92 |
 *   | ManualLoop         | Int32[17664] |  9,286.429 ns |  82.7457 ns |  73.3519 ns | 179.79 |    2.03 | 0.8392 |   70680 B |      135.92 |
 *   | ManualLoopSpan     | Int32[17664] |  9,669.222 ns | 189.1164 ns | 225.1296 ns | 187.20 |    4.54 | 0.8392 |   70680 B |      135.92 |
 *   | UnrolledManualLoop | Int32[17664] | 10,398.810 ns | 196.1951 ns | 233.5563 ns | 201.32 |    4.73 | 0.8392 |   70680 B |      135.92 |
 *   | ArrayDotCopy       | Int32[3]     |     11.900 ns |   0.0360 ns |   0.0281 ns |   0.23 |    0.00 | 0.0005 |      40 B |        0.08 |
 *   | ManualLoop         | Int32[3]     |      8.877 ns |   0.0730 ns |   0.0683 ns |   0.17 |    0.00 | 0.0005 |      40 B |        0.08 |
 *   | ManualLoopSpan     | Int32[3]     |      9.912 ns |   0.1269 ns |   0.1187 ns |   0.19 |    0.00 | 0.0005 |      40 B |        0.08 |
 *   | UnrolledManualLoop | Int32[3]     |     11.036 ns |   0.0729 ns |   0.0682 ns |   0.21 |    0.00 | 0.0005 |      40 B |        0.08 |
 *
 *
 *   BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2582) (Hyper-V)
 *   AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *   .NET SDK 8.0.401
 *     [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *     DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *   | Method             | array        | Mean         | Error      | StdDev     | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *   |------------------- |------------- |-------------:|-----------:|-----------:|-------:|--------:|-------:|----------:|------------:|
 *   | ArrayDotCopy       | Int32[124]   |    27.995 ns |  0.2050 ns |  0.1712 ns |   1.00 |    0.01 | 0.0311 |     520 B |        1.00 |
 *   | ManualLoop         | Int32[124]   |    65.708 ns |  1.2098 ns |  0.9445 ns |   2.35 |    0.04 | 0.0310 |     520 B |        1.00 |
 *   | ManualLoopSpan     | Int32[124]   |    66.921 ns |  0.1534 ns |  0.1360 ns |   2.39 |    0.02 | 0.0310 |     520 B |        1.00 |
 *   | UnrolledManualLoop | Int32[124]   |    63.406 ns |  0.9769 ns |  0.9138 ns |   2.27 |    0.03 | 0.0310 |     520 B |        1.00 |
 *   | ArrayDotCopy       | Int32[12]    |    10.917 ns |  0.0792 ns |  0.0661 ns |   0.39 |    0.00 | 0.0043 |      72 B |        0.14 |
 *   | ManualLoop         | Int32[12]    |     9.716 ns |  0.0328 ns |  0.0291 ns |   0.35 |    0.00 | 0.0043 |      72 B |        0.14 |
 *   | ManualLoopSpan     | Int32[12]    |    11.320 ns |  0.0966 ns |  0.0904 ns |   0.40 |    0.00 | 0.0043 |      72 B |        0.14 |
 *   | UnrolledManualLoop | Int32[12]    |    11.529 ns |  0.1798 ns |  0.1501 ns |   0.41 |    0.01 | 0.0043 |      72 B |        0.14 |
 *   | ArrayDotCopy       | Int32[17664] | 3,241.467 ns | 10.0601 ns |  8.4006 ns | 115.79 |    0.75 | 4.2000 |   70680 B |      135.92 |
 *   | ManualLoop         | Int32[17664] | 7,181.827 ns | 52.7879 ns | 44.0803 ns | 256.55 |    2.16 | 4.1962 |   70680 B |      135.92 |
 *   | ManualLoopSpan     | Int32[17664] | 7,167.770 ns | 24.2930 ns | 21.5351 ns | 256.05 |    1.70 | 4.1962 |   70680 B |      135.92 |
 *   | UnrolledManualLoop | Int32[17664] | 7,774.437 ns | 24.8035 ns | 19.3650 ns | 277.72 |    1.79 | 4.1962 |   70680 B |      135.92 |
 *   | ArrayDotCopy       | Int32[3]     |     9.579 ns |  0.0545 ns |  0.0510 ns |   0.34 |    0.00 | 0.0024 |      40 B |        0.08 |
 *   | ManualLoop         | Int32[3]     |     6.004 ns |  0.0525 ns |  0.0439 ns |   0.21 |    0.00 | 0.0024 |      40 B |        0.08 |
 *   | ManualLoopSpan     | Int32[3]     |     7.494 ns |  0.0281 ns |  0.0249 ns |   0.27 |    0.00 | 0.0024 |      40 B |        0.08 |
 *   | UnrolledManualLoop | Int32[3]     |     7.782 ns |  0.0954 ns |  0.0893 ns |   0.28 |    0.00 | 0.0024 |      40 B |        0.08 |
 *
 *
 *   BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
 *   Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *   .NET SDK 8.0.401
 *     [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *     DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *
 *   | Method             | array        | Mean         | Error      | StdDev     | Ratio  | RatioSD | Gen0    | Allocated | Alloc Ratio |
 *   |------------------- |------------- |-------------:|-----------:|-----------:|-------:|--------:|--------:|----------:|------------:|
 *   | ArrayDotCopy       | Int32[124]   |    26.723 ns |  0.5761 ns |  0.9304 ns |   1.00 |    0.05 |  0.0829 |     520 B |        1.00 |
 *   | ManualLoop         | Int32[124]   |    66.869 ns |  1.3761 ns |  1.2198 ns |   2.51 |    0.09 |  0.0829 |     520 B |        1.00 |
 *   | ManualLoopSpan     | Int32[124]   |    78.039 ns |  3.0398 ns |  8.9629 ns |   2.92 |    0.35 |  0.0829 |     520 B |        1.00 |
 *   | UnrolledManualLoop | Int32[124]   |    56.156 ns |  1.5939 ns |  4.6998 ns |   2.10 |    0.19 |  0.0829 |     520 B |        1.00 |
 *   | ArrayDotCopy       | Int32[12]    |     8.170 ns |  0.2039 ns |  0.3929 ns |   0.31 |    0.02 |  0.0115 |      72 B |        0.14 |
 *   | ManualLoop         | Int32[12]    |     9.283 ns |  0.2300 ns |  0.3843 ns |   0.35 |    0.02 |  0.0115 |      72 B |        0.14 |
 *   | ManualLoopSpan     | Int32[12]    |    10.267 ns |  0.1315 ns |  0.1165 ns |   0.38 |    0.01 |  0.0115 |      72 B |        0.14 |
 *   | UnrolledManualLoop | Int32[12]    |     8.112 ns |  0.2255 ns |  0.6612 ns |   0.30 |    0.03 |  0.0115 |      72 B |        0.14 |
 *   | ArrayDotCopy       | Int32[17664] | 2,376.202 ns | 46.6252 ns | 38.9341 ns |  89.02 |    3.28 | 11.2343 |   70680 B |      135.92 |
 *   | ManualLoop         | Int32[17664] | 6,998.381 ns | 46.8786 ns | 39.1458 ns | 262.19 |    8.84 | 11.2305 |   70680 B |      135.92 |
 *   | ManualLoopSpan     | Int32[17664] | 6,991.966 ns | 38.1943 ns | 31.8939 ns | 261.95 |    8.79 | 11.2305 |   70680 B |      135.92 |
 *   | UnrolledManualLoop | Int32[17664] | 4,896.139 ns | 24.9988 ns | 22.1608 ns | 183.43 |    6.16 | 11.2305 |   70680 B |      135.92 |
 *   | ArrayDotCopy       | Int32[3]     |     6.177 ns |  0.0629 ns |  0.0526 ns |   0.23 |    0.01 |  0.0064 |      40 B |        0.08 |
 *   | ManualLoop         | Int32[3]     |     5.029 ns |  0.0739 ns |  0.0617 ns |   0.19 |    0.01 |  0.0064 |      40 B |        0.08 |
 *   | ManualLoopSpan     | Int32[3]     |     5.593 ns |  0.1144 ns |  0.1070 ns |   0.21 |    0.01 |  0.0064 |      40 B |        0.08 |
 *   | UnrolledManualLoop | Int32[3]     |     6.072 ns |  0.0562 ns |  0.0439 ns |   0.23 |    0.01 |  0.0064 |      40 B |        0.08 |
 *
 *
 *   BenchmarkDotNet v0.14.0, macOS Ventura 13.6.9 (22G830) [Darwin 22.6.0]
 *   Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *   .NET SDK 8.0.401
 *     [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *     DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *   | Method             | array        | Mean         | Error      | StdDev     | Ratio  | RatioSD | Gen0    | Allocated | Alloc Ratio |
 *   |------------------- |------------- |-------------:|-----------:|-----------:|-------:|--------:|--------:|----------:|------------:|
 *   | ArrayDotCopy       | Int32[124]   |     49.04 ns |   1.031 ns |   1.188 ns |   1.00 |    0.03 |  0.0829 |     520 B |        1.00 |
 *   | ManualLoop         | Int32[124]   |    137.23 ns |   2.761 ns |   3.068 ns |   2.80 |    0.09 |  0.0827 |     520 B |        1.00 |
 *   | ManualLoopSpan     | Int32[124]   |     85.26 ns |   1.748 ns |   1.795 ns |   1.74 |    0.05 |  0.0829 |     520 B |        1.00 |
 *   | UnrolledManualLoop | Int32[124]   |    103.01 ns |   1.054 ns |   0.934 ns |   2.10 |    0.05 |  0.0829 |     520 B |        1.00 |
 *   | ArrayDotCopy       | Int32[12]    |     20.57 ns |   0.354 ns |   0.314 ns |   0.42 |    0.01 |  0.0115 |      72 B |        0.14 |
 *   | ManualLoop         | Int32[12]    |     24.61 ns |   0.398 ns |   0.372 ns |   0.50 |    0.01 |  0.0115 |      72 B |        0.14 |
 *   | ManualLoopSpan     | Int32[12]    |     20.55 ns |   0.328 ns |   0.307 ns |   0.42 |    0.01 |  0.0115 |      72 B |        0.14 |
 *   | UnrolledManualLoop | Int32[12]    |     22.97 ns |   0.291 ns |   0.272 ns |   0.47 |    0.01 |  0.0115 |      72 B |        0.14 |
 *   | ArrayDotCopy       | Int32[17664] |  5,402.33 ns | 103.396 ns | 178.352 ns | 110.22 |    4.41 | 11.2305 |   70680 B |      135.92 |
 *   | ManualLoop         | Int32[17664] | 17,481.15 ns | 292.803 ns | 300.688 ns | 356.65 |   10.19 | 11.2305 |   70680 B |      135.92 |
 *   | ManualLoopSpan     | Int32[17664] |  9,415.87 ns | 150.920 ns | 117.828 ns | 192.10 |    5.02 | 11.2305 |   70680 B |      135.92 |
 *   | UnrolledManualLoop | Int32[17664] | 12,928.91 ns | 205.501 ns | 171.603 ns | 263.77 |    6.98 | 11.2305 |   70680 B |      135.92 |
 *   | ArrayDotCopy       | Int32[3]     |     19.00 ns |   0.282 ns |   0.250 ns |   0.39 |    0.01 |  0.0063 |      40 B |        0.08 |
 *   | ManualLoop         | Int32[3]     |     15.95 ns |   0.181 ns |   0.160 ns |   0.33 |    0.01 |  0.0063 |      40 B |        0.08 |
 *   | ManualLoopSpan     | Int32[3]     |     15.47 ns |   0.244 ns |   0.228 ns |   0.32 |    0.01 |  0.0063 |      40 B |        0.08 |
 *   | UnrolledManualLoop | Int32[3]     |     16.75 ns |   0.186 ns |   0.165 ns |   0.34 |    0.01 |  0.0063 |      40 B |        0.08 |
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;
public class ArrayCopy_Benchmark : BaseBenchmark
{
    public static IEnumerable<int[]> Data => [
        [1, 2, 3],
        [1, 2, 3, 4, 5, 6 ,7 ,8 ,9 ,10, 11, 12],
        [
            01, 02, 03, 04, 05, 06 ,07 ,08 ,09 ,10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 45, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58 ,59 ,60,
            61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
            81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 00,
            01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15 ,16, 17 ,18, 19, 20,
            21, 22, 23, 24
        ],
        EvaluationConstants.PackedPSQT
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int[] ArrayDotCopy(int[] array)
    {
        return ArrayDotCopyImpl(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] ManualLoop(int[] array)
    {
        return ManualLoopImpl(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] ManualLoopSpan(int[] array)
    {
        return ManualLoopImplSpan(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] UnrolledManualLoop(int[] array)
    {
        return UnrolledManualLoopImpl(array);
    }

    private static int[] ArrayDotCopyImpl(int[] array)
    {
        var result = new int[array.Length];
        Array.Copy(array, result, array.Length);

        return result;
    }

    private static int[] ManualLoopImpl(int[] array)
    {
        var result = new int[array.Length];
        for (int i = 0; i < array.Length; ++i)
        {
            result[i] = array[i];
        }

        return result;
    }

    private static int[] ManualLoopImplSpan(ReadOnlySpan<int> array)
    {
        var result = new int[array.Length];
        for (int i = 0; i < array.Length; ++i)
        {
            result[i] = array[i];
        }

        return result;
    }

    private static unsafe int[] UnrolledManualLoopImpl(ReadOnlySpan<int> array)
    {
        var result = new int[array.Length];

        int i = 0;
        int lastBlockIndex = array.Length - (array.Length % 4);

        // Pin source so we can elide the bounds checks
        fixed (int* pSource = array)
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
