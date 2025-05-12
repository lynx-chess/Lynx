/*
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.203
 *    [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean          | Error      | StdDev     | Ratio  | RatioSD | Allocated | Alloc Ratio |
 *  |------------------- |------------- |--------------:|-----------:|-----------:|-------:|--------:|----------:|------------:|
 *  | ArrayDotClear      | Int32[1024]  |     73.253 ns |  0.1952 ns |  0.1731 ns |   1.00 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[1024]  |    325.412 ns |  1.8664 ns |  1.6545 ns |   4.44 |    0.02 |         - |          NA |
 *  | SpanClear          | Int32[1024]  |     71.808 ns |  0.1970 ns |  0.1645 ns |   0.98 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[1024]  |    245.387 ns |  0.7368 ns |  0.6152 ns |   3.35 |    0.01 |         - |          NA |
 *  | ArrayDotClear      | Int32[128]   |      8.481 ns |  0.0327 ns |  0.0273 ns |   0.12 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[128]   |     46.562 ns |  0.2042 ns |  0.1810 ns |   0.64 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[128]   |      6.910 ns |  0.0099 ns |  0.0077 ns |   0.09 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[128]   |     30.550 ns |  0.1739 ns |  0.1357 ns |   0.42 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[12]    |      4.110 ns |  0.0533 ns |  0.0472 ns |   0.06 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[12]    |      4.445 ns |  0.0379 ns |  0.0354 ns |   0.06 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[12]    |      2.185 ns |  0.0080 ns |  0.0071 ns |   0.03 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[12]    |      2.962 ns |  0.0239 ns |  0.0212 ns |   0.04 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[192]   |     10.936 ns |  0.0274 ns |  0.0214 ns |   0.15 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[192]   |     66.357 ns |  0.4920 ns |  0.4603 ns |   0.91 |    0.01 |         - |          NA |
 *  | SpanClear          | Int32[192]   |      9.374 ns |  0.0482 ns |  0.0451 ns |   0.13 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[192]   |     45.610 ns |  0.1350 ns |  0.1263 ns |   0.62 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[256]   |     13.502 ns |  0.1163 ns |  0.1088 ns |   0.18 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[256]   |     13.417 ns |  0.0558 ns |  0.0466 ns |   0.18 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[256]   |     86.252 ns |  0.0978 ns |  0.0764 ns |   1.18 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[256]   |     86.313 ns |  0.4253 ns |  0.3978 ns |   1.18 |    0.01 |         - |          NA |
 *  | SpanClear          | Int32[256]   |     11.906 ns |  0.0770 ns |  0.0721 ns |   0.16 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[256]   |     11.860 ns |  0.0882 ns |  0.0782 ns |   0.16 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[256]   |     59.734 ns |  0.0527 ns |  0.0411 ns |   0.82 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[256]   |     60.305 ns |  0.5809 ns |  0.4850 ns |   0.82 |    0.01 |         - |          NA |
 *  | ArrayDotClear      | Int32[35328] |  1,573.356 ns |  3.4968 ns |  3.2709 ns |  21.48 |    0.07 |         - |          NA |
 *  | ManualLoop         | Int32[35328] | 11,034.036 ns | 60.5747 ns | 56.6616 ns | 150.63 |    0.82 |         - |          NA |
 *  | SpanClear          | Int32[35328] |  1,571.257 ns |  1.3757 ns |  1.1488 ns |  21.45 |    0.05 |         - |          NA |
 *  | UnrolledManualLoop | Int32[35328] |  8,280.911 ns | 38.9324 ns | 34.5125 ns | 113.05 |    0.52 |         - |          NA |
 *  | ArrayDotClear      | Int32[3]     |      3.595 ns |  0.0166 ns |  0.0147 ns |   0.05 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[3]     |      1.252 ns |  0.0106 ns |  0.0089 ns |   0.02 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[3]     |      1.868 ns |  0.0083 ns |  0.0078 ns |   0.03 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[3]     |      2.262 ns |  0.0093 ns |  0.0087 ns |   0.03 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[512]   |     27.214 ns |  0.0358 ns |  0.0318 ns |   0.37 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[512]   |    165.548 ns |  0.4347 ns |  0.3854 ns |   2.26 |    0.01 |         - |          NA |
 *  | SpanClear          | Int32[512]   |     26.876 ns |  0.0114 ns |  0.0089 ns |   0.37 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[512]   |    125.648 ns |  0.1363 ns |  0.1138 ns |   1.72 |    0.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.3328) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.203
 *    [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean          | Error      | StdDev     | Ratio  | RatioSD | Allocated | Alloc Ratio |
 *  |------------------- |------------- |--------------:|-----------:|-----------:|-------:|--------:|----------:|------------:|
 *  | ArrayDotClear      | Int32[1024]  |     44.758 ns |  0.0723 ns |  0.0641 ns |   1.00 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[1024]  |    323.886 ns |  0.5023 ns |  0.4453 ns |   7.24 |    0.01 |         - |          NA |
 *  | SpanClear          | Int32[1024]  |     44.681 ns |  0.0355 ns |  0.0297 ns |   1.00 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[1024]  |    244.347 ns |  0.3589 ns |  0.3182 ns |   5.46 |    0.01 |         - |          NA |
 *  | ArrayDotClear      | Int32[128]   |      7.225 ns |  0.0235 ns |  0.0209 ns |   0.16 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[128]   |     46.698 ns |  0.0627 ns |  0.0489 ns |   1.04 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[128]   |      5.627 ns |  0.0086 ns |  0.0077 ns |   0.13 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[128]   |     30.587 ns |  0.2381 ns |  0.1859 ns |   0.68 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[12]    |      4.481 ns |  0.0396 ns |  0.0370 ns |   0.10 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[12]    |      5.176 ns |  0.0330 ns |  0.0309 ns |   0.12 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[12]    |      2.774 ns |  0.0058 ns |  0.0048 ns |   0.06 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[12]    |      3.219 ns |  0.0036 ns |  0.0034 ns |   0.07 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[192]   |     11.222 ns |  0.0213 ns |  0.0199 ns |   0.25 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[192]   |     66.533 ns |  0.1232 ns |  0.1092 ns |   1.49 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[192]   |      9.593 ns |  0.0082 ns |  0.0073 ns |   0.21 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[192]   |     45.487 ns |  0.0943 ns |  0.0882 ns |   1.02 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[256]   |     13.694 ns |  0.0282 ns |  0.0250 ns |   0.31 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[256]   |     13.699 ns |  0.0452 ns |  0.0423 ns |   0.31 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[256]   |     86.018 ns |  0.1258 ns |  0.1050 ns |   1.92 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[256]   |     86.386 ns |  0.1913 ns |  0.1597 ns |   1.93 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[256]   |     12.107 ns |  0.0497 ns |  0.0465 ns |   0.27 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[256]   |     12.101 ns |  0.0406 ns |  0.0360 ns |   0.27 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[256]   |     59.713 ns |  0.0569 ns |  0.0532 ns |   1.33 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[256]   |     59.889 ns |  0.1090 ns |  0.0967 ns |   1.34 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[35328] |  1,504.808 ns |  4.4247 ns |  4.1388 ns |  33.62 |    0.10 |         - |          NA |
 *  | ManualLoop         | Int32[35328] | 10,978.707 ns | 17.4823 ns | 16.3530 ns | 245.29 |    0.49 |         - |          NA |
 *  | SpanClear          | Int32[35328] |  1,501.981 ns |  1.2901 ns |  1.1436 ns |  33.56 |    0.05 |         - |          NA |
 *  | UnrolledManualLoop | Int32[35328] |  8,235.559 ns | 16.2635 ns | 15.2128 ns | 184.00 |    0.42 |         - |          NA |
 *  | ArrayDotClear      | Int32[3]     |      3.867 ns |  0.0044 ns |  0.0039 ns |   0.09 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[3]     |      1.566 ns |  0.0025 ns |  0.0023 ns |   0.03 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[3]     |      2.184 ns |  0.0033 ns |  0.0031 ns |   0.05 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[3]     |      2.574 ns |  0.0059 ns |  0.0055 ns |   0.06 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[512]   |     24.671 ns |  0.0139 ns |  0.0116 ns |   0.55 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[512]   |    165.043 ns |  0.2158 ns |  0.1802 ns |   3.69 |    0.01 |         - |          NA |
 *  | SpanClear          | Int32[512]   |     24.903 ns |  0.0222 ns |  0.0197 ns |   0.56 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[512]   |    125.328 ns |  0.1398 ns |  0.1167 ns |   2.80 |    0.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.5 (22H527) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.203
 *    [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean           | Error       | StdDev      | Median         | Ratio  | RatioSD | Allocated | Alloc Ratio |
 *  |------------------- |------------- |---------------:|------------:|------------:|---------------:|-------:|--------:|----------:|------------:|
 *  | ArrayDotClear      | Int32[1024]  |     74.1540 ns |   0.3934 ns |   0.3285 ns |     74.2151 ns |   1.00 |    0.01 |         - |          NA |
 *  | ManualLoop         | Int32[1024]  |    243.3785 ns |   1.3384 ns |   1.2520 ns |    243.2868 ns |   3.28 |    0.02 |         - |          NA |
 *  | SpanClear          | Int32[1024]  |     77.1307 ns |   0.7063 ns |   0.5514 ns |     77.0695 ns |   1.04 |    0.01 |         - |          NA |
 *  | UnrolledManualLoop | Int32[1024]  |    304.8825 ns |   1.7642 ns |   1.4732 ns |    304.5986 ns |   4.11 |    0.03 |         - |          NA |
 *  | ArrayDotClear      | Int32[128]   |      7.5294 ns |   0.1637 ns |   0.1531 ns |      7.4765 ns |   0.10 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[128]   |     35.2633 ns |   0.2275 ns |   0.2128 ns |     35.2221 ns |   0.48 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[128]   |      5.9512 ns |   0.1581 ns |   0.1479 ns |      5.9935 ns |   0.08 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[128]   |     38.4365 ns |   0.2177 ns |   0.1929 ns |     38.4939 ns |   0.52 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[12]    |      3.1212 ns |   0.1154 ns |   0.1023 ns |      3.1128 ns |   0.04 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[12]    |      3.6263 ns |   0.1038 ns |   0.0867 ns |      3.6306 ns |   0.05 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[12]    |      2.3189 ns |   0.1064 ns |   0.1384 ns |      2.2623 ns |   0.03 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[12]    |      2.2009 ns |   0.0670 ns |   0.0627 ns |      2.2234 ns |   0.03 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[192]   |     10.0350 ns |   0.2535 ns |   0.2490 ns |      9.9321 ns |   0.14 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[192]   |     51.5392 ns |   1.0570 ns |   1.0381 ns |     51.2017 ns |   0.70 |    0.01 |         - |          NA |
 *  | SpanClear          | Int32[192]   |      9.0217 ns |   0.2444 ns |   0.3946 ns |      9.1316 ns |   0.12 |    0.01 |         - |          NA |
 *  | UnrolledManualLoop | Int32[192]   |     60.4999 ns |   1.2822 ns |   2.3767 ns |     60.3848 ns |   0.82 |    0.03 |         - |          NA |
 *  | ArrayDotClear      | Int32[256]   |     13.2144 ns |   0.3233 ns |   0.4316 ns |     13.2835 ns |   0.18 |    0.01 |         - |          NA |
 *  | ArrayDotClear      | Int32[256]   |     12.3956 ns |   0.2972 ns |   0.4068 ns |     12.4638 ns |   0.17 |    0.01 |         - |          NA |
 *  | ManualLoop         | Int32[256]   |     70.1878 ns |   1.4252 ns |   1.4636 ns |     70.5997 ns |   0.95 |    0.02 |         - |          NA |
 *  | ManualLoop         | Int32[256]   |     70.7239 ns |   1.9979 ns |   5.5362 ns |     68.3824 ns |   0.95 |    0.07 |         - |          NA |
 *  | SpanClear          | Int32[256]   |     11.4469 ns |   0.2737 ns |   0.6007 ns |     11.2654 ns |   0.15 |    0.01 |         - |          NA |
 *  | SpanClear          | Int32[256]   |     11.4647 ns |   0.1822 ns |   0.1704 ns |     11.4302 ns |   0.15 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[256]   |     80.5930 ns |   1.6774 ns |   2.6605 ns |     80.1326 ns |   1.09 |    0.04 |         - |          NA |
 *  | UnrolledManualLoop | Int32[256]   |     86.8711 ns |   1.2739 ns |   1.1916 ns |     87.2698 ns |   1.17 |    0.02 |         - |          NA |
 *  | ArrayDotClear      | Int32[35328] |  2,998.5011 ns |  50.5352 ns |  47.2706 ns |  2,980.1289 ns |  40.44 |    0.64 |         - |          NA |
 *  | ManualLoop         | Int32[35328] | 10,193.6437 ns | 202.9928 ns | 548.8030 ns | 10,142.5544 ns | 137.47 |    7.38 |         - |          NA |
 *  | SpanClear          | Int32[35328] |  2,956.3379 ns |  58.8605 ns | 103.0894 ns |  2,986.5002 ns |  39.87 |    1.38 |         - |          NA |
 *  | UnrolledManualLoop | Int32[35328] | 11,778.8237 ns | 250.8805 ns | 723.8477 ns | 11,736.4111 ns | 158.85 |    9.74 |         - |          NA |
 *  | ArrayDotClear      | Int32[3]     |      2.3688 ns |   0.1095 ns |   0.2405 ns |      2.3619 ns |   0.03 |    0.00 |         - |          NA |
 *  | ManualLoop         | Int32[3]     |      0.9583 ns |   0.0730 ns |   0.0647 ns |      0.9420 ns |   0.01 |    0.00 |         - |          NA |
 *  | SpanClear          | Int32[3]     |      0.8669 ns |   0.0844 ns |   0.1889 ns |      0.8067 ns |   0.01 |    0.00 |         - |          NA |
 *  | UnrolledManualLoop | Int32[3]     |      1.9682 ns |   0.0738 ns |   0.0690 ns |      1.9582 ns |   0.03 |    0.00 |         - |          NA |
 *  | ArrayDotClear      | Int32[512]   |     48.5060 ns |   1.0091 ns |   1.3471 ns |     48.1785 ns |   0.65 |    0.02 |         - |          NA |
 *  | ManualLoop         | Int32[512]   |    127.9750 ns |   2.4706 ns |   2.3110 ns |    128.2702 ns |   1.73 |    0.03 |         - |          NA |
 *  | SpanClear          | Int32[512]   |     49.5148 ns |   0.9447 ns |   1.4984 ns |     49.1219 ns |   0.67 |    0.02 |         - |          NA |
 *  | UnrolledManualLoop | Int32[512]   |    167.7616 ns |   3.3340 ns |   4.4508 ns |    167.8591 ns |   2.26 |    0.06 |         - |          NA |
 *
 */
using BenchmarkDotNet.Attributes;

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
