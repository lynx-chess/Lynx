﻿/*
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method            | sq1Array  | sq2Array  | Mean          | Error      | StdDev     | Ratio  | RatioSD | Allocated | Alloc Ratio |
 *  |------------------ |---------- |---------- |--------------:|-----------:|-----------:|-------:|--------:|----------:|------------:|
 *  | OnTheFly          | Int32[10] | Int32[10] |    199.543 ns |  2.4357 ns |  2.2783 ns |  1.000 |    0.02 |         - |          NA |
 *  | Lookup            | Int32[10] | Int32[10] |     64.915 ns |  0.1788 ns |  0.1396 ns |  0.325 |    0.00 |         - |          NA |
 *  | LookupDoubleArray | Int32[10] | Int32[10] |     68.922 ns |  0.6394 ns |  0.5668 ns |  0.345 |    0.00 |         - |          NA |
 *  | OnTheFly          | Int32[1]  | Int32[1]  |      2.012 ns |  0.0198 ns |  0.0175 ns |  0.010 |    0.00 |         - |          NA |
 *  | Lookup            | Int32[1]  | Int32[1]  |      1.380 ns |  0.0131 ns |  0.0116 ns |  0.007 |    0.00 |         - |          NA |
 *  | LookupDoubleArray | Int32[1]  | Int32[1]  |      1.683 ns |  0.0056 ns |  0.0044 ns |  0.008 |    0.00 |         - |          NA |
 *  | OnTheFly          | Int32[30] | Int32[30] |  1,735.268 ns | 16.5898 ns | 13.8532 ns |  8.697 |    0.12 |         - |          NA |
 *  | Lookup            | Int32[30] | Int32[30] |    592.591 ns |  2.6362 ns |  2.2014 ns |  2.970 |    0.03 |         - |          NA |
 *  | LookupDoubleArray | Int32[30] | Int32[30] |    593.276 ns |  0.7367 ns |  0.6152 ns |  2.974 |    0.03 |         - |          NA |
 *  | OnTheFly          | Int32[5]  | Int32[5]  |     50.485 ns |  0.3470 ns |  0.3076 ns |  0.253 |    0.00 |         - |          NA |
 *  | Lookup            | Int32[5]  | Int32[5]  |     18.613 ns |  0.2392 ns |  0.2238 ns |  0.093 |    0.00 |         - |          NA |
 *  | LookupDoubleArray | Int32[5]  | Int32[5]  |     18.617 ns |  0.1388 ns |  0.1298 ns |  0.093 |    0.00 |         - |          NA |
 *  | OnTheFly          | Int32[64] | Int32[64] | 10,995.924 ns | 19.2642 ns | 17.0772 ns | 55.112 |    0.62 |         - |          NA |
 *  | Lookup            | Int32[64] | Int32[64] |  2,623.785 ns | 23.7809 ns | 22.2446 ns | 13.151 |    0.18 |         - |          NA |
 *  | LookupDoubleArray | Int32[64] | Int32[64] |  2,608.702 ns |  5.1225 ns |  4.2775 ns | 13.075 |    0.15 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2655) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method            | sq1Array  | sq2Array  | Mean         | Error     | StdDev    | Ratio  | RatioSD | Allocated | Alloc Ratio |
 *  |------------------ |---------- |---------- |-------------:|----------:|----------:|-------:|--------:|----------:|------------:|
 *  | OnTheFly          | Int32[10] | Int32[10] |   182.940 ns | 0.6388 ns | 0.4987 ns |  1.000 |    0.00 |         - |          NA |
 *  | Lookup            | Int32[10] | Int32[10] |    64.521 ns | 0.1274 ns | 0.0994 ns |  0.353 |    0.00 |         - |          NA |
 *  | LookupDoubleArray | Int32[10] | Int32[10] |    70.411 ns | 0.7159 ns | 0.6697 ns |  0.385 |    0.00 |         - |          NA |
 *  | OnTheFly          | Int32[1]  | Int32[1]  |     2.649 ns | 0.0030 ns | 0.0025 ns |  0.014 |    0.00 |         - |          NA |
 *  | Lookup            | Int32[1]  | Int32[1]  |     1.412 ns | 0.0025 ns | 0.0022 ns |  0.008 |    0.00 |         - |          NA |
 *  | LookupDoubleArray | Int32[1]  | Int32[1]  |     1.723 ns | 0.0040 ns | 0.0035 ns |  0.009 |    0.00 |         - |          NA |
 *  | OnTheFly          | Int32[30] | Int32[30] | 2,102.563 ns | 4.0566 ns | 3.3874 ns | 11.493 |    0.04 |         - |          NA |
 *  | Lookup            | Int32[30] | Int32[30] |   572.458 ns | 0.4641 ns | 0.4114 ns |  3.129 |    0.01 |         - |          NA |
 *  | LookupDoubleArray | Int32[30] | Int32[30] |   582.855 ns | 0.5000 ns | 0.4432 ns |  3.186 |    0.01 |         - |          NA |
 *  | OnTheFly          | Int32[5]  | Int32[5]  |    48.996 ns | 0.4039 ns | 0.3581 ns |  0.268 |    0.00 |         - |          NA |
 *  | Lookup            | Int32[5]  | Int32[5]  |    16.870 ns | 0.0700 ns | 0.0655 ns |  0.092 |    0.00 |         - |          NA |
 *  | LookupDoubleArray | Int32[5]  | Int32[5]  |    19.678 ns | 0.2548 ns | 0.2383 ns |  0.108 |    0.00 |         - |          NA |
 *  | OnTheFly          | Int32[64] | Int32[64] | 9,905.736 ns | 9.2500 ns | 8.1999 ns | 54.148 |    0.15 |         - |          NA |
 *  | Lookup            | Int32[64] | Int32[64] | 2,567.974 ns | 2.3026 ns | 2.0412 ns | 14.037 |    0.04 |         - |          NA |
 *  | LookupDoubleArray | Int32[64] | Int32[64] | 2,585.791 ns | 1.5930 ns | 1.3302 ns | 14.135 |    0.04 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *
 *  | Method            | sq1Array  | sq2Array  | Mean          | Error      | StdDev     | Ratio  | RatioSD | Allocated | Alloc Ratio |
 *  |------------------ |---------- |---------- |--------------:|-----------:|-----------:|-------:|--------:|----------:|------------:|
 *  | OnTheFly          | Int32[10] | Int32[10] |   203.9593 ns |  0.8891 ns |  0.7882 ns |  1.000 |    0.01 |         - |          NA |
 *  | Lookup            | Int32[10] | Int32[10] |    55.4638 ns |  1.1100 ns |  1.7281 ns |  0.272 |    0.01 |         - |          NA |
 *  | LookupDoubleArray | Int32[10] | Int32[10] |    70.1269 ns |  1.4349 ns |  1.4093 ns |  0.344 |    0.01 |         - |          NA |
 *  | OnTheFly          | Int32[1]  | Int32[1]  |     1.4242 ns |  0.0068 ns |  0.0053 ns |  0.007 |    0.00 |         - |          NA |
 *  | Lookup            | Int32[1]  | Int32[1]  |     0.8779 ns |  0.0392 ns |  0.0550 ns |  0.004 |    0.00 |         - |          NA |
 *  | LookupDoubleArray | Int32[1]  | Int32[1]  |     1.0344 ns |  0.0223 ns |  0.0186 ns |  0.005 |    0.00 |         - |          NA |
 *  | OnTheFly          | Int32[30] | Int32[30] | 1,525.1495 ns | 21.4802 ns | 19.0416 ns |  7.478 |    0.09 |         - |          NA |
 *  | Lookup            | Int32[30] | Int32[30] |   504.5326 ns |  2.3276 ns |  1.8172 ns |  2.474 |    0.01 |         - |          NA |
 *  | LookupDoubleArray | Int32[30] | Int32[30] |   588.3407 ns |  2.1516 ns |  1.7967 ns |  2.885 |    0.01 |         - |          NA |
 *  | OnTheFly          | Int32[5]  | Int32[5]  |    54.8104 ns |  0.1373 ns |  0.1146 ns |  0.269 |    0.00 |         - |          NA |
 *  | Lookup            | Int32[5]  | Int32[5]  |    13.2023 ns |  0.0737 ns |  0.0576 ns |  0.065 |    0.00 |         - |          NA |
 *  | LookupDoubleArray | Int32[5]  | Int32[5]  |    19.4769 ns |  0.0990 ns |  0.0878 ns |  0.095 |    0.00 |         - |          NA |
 *  | OnTheFly          | Int32[64] | Int32[64] | 7,658.2115 ns | 58.9772 ns | 52.2818 ns | 37.548 |    0.28 |         - |          NA |
 *  | Lookup            | Int32[64] | Int32[64] | 2,434.3557 ns |  8.8069 ns |  8.2380 ns | 11.936 |    0.06 |         - |          NA |
 *  | LookupDoubleArray | Int32[64] | Int32[64] | 2,618.9650 ns |  3.5368 ns |  2.9533 ns | 12.841 |    0.05 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class ChebyshevDistance_Benchmark : BaseBenchmark
{
    private readonly Random _rnd = new(1234);

    public IEnumerable<object[]> Data()
    {
        yield return new object[] { Enumerable.Range(0, 1).Select(_ => _rnd.Next(0, 64)).ToArray(), Enumerable.Range(0, 1).Select(_ => _rnd.Next(0, 64)).ToArray() };
        yield return new object[] { Enumerable.Range(0, 5).Select(_ => _rnd.Next(0, 64)).ToArray(), Enumerable.Range(0, 5).Select(_ => _rnd.Next(0, 64)).ToArray() };
        yield return new object[] { Enumerable.Range(0, 10).Select(_ => _rnd.Next(0, 64)).ToArray(), Enumerable.Range(0, 10).Select(_ => _rnd.Next(0, 64)).ToArray() };
        yield return new object[] { Enumerable.Range(0, 30).Select(_ => _rnd.Next(0, 64)).ToArray(), Enumerable.Range(0, 30).Select(_ => _rnd.Next(0, 64)).ToArray() };
        yield return new object[] { Enumerable.Range(0, 64).Select(_ => _rnd.Next(0, 64)).ToArray(), Enumerable.Range(0, 64).Select(_ => _rnd.Next(0, 64)).ToArray() };
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int OnTheFly(int[] sq1Array, int[] sq2Array)
    {
        var result = 0;

        for (int sq1 = 0; sq1 < sq1Array.Length; ++sq1)
        {
            for (int sq2 = 0; sq2 < sq2Array.Length; ++sq2)
            {
                result += ChebyshevDistanceOnTheFly(sq1Array[sq1], sq2Array[sq2]);
            }
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int Lookup(int[] sq1Array, int[] sq2Array)
    {
        var result = 0;

        for (int sq1 = 0; sq1 < sq1Array.Length; ++sq1)
        {
            for (int sq2 = 0; sq2 < sq2Array.Length; ++sq2)
            {
                result += ChebyshevDistanceLookup(sq1Array[sq1], sq2Array[sq2]);
            }
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int LookupDoubleArray(int[] sq1Array, int[] sq2Array)
    {
        var result = 0;

        for (int sq1 = 0; sq1 < sq1Array.Length; ++sq1)
        {
            for (int sq2 = 0; sq2 < sq2Array.Length; ++sq2)
            {
                result += ChebyshevDistanceLookupDoubleArray(sq1Array[sq1], sq2Array[sq2]);
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ChebyshevDistanceOnTheFly(int square1, int square2)
    {
        var xDelta = Math.Abs(Lynx.Constants.File[square1] - Lynx.Constants.File[square2]);
        var yDelta = Math.Abs(Lynx.Constants.Rank[square1] - Lynx.Constants.Rank[square2]);

        return xDelta >= yDelta
            ? xDelta
            : yDelta;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ChebyshevDistanceLookup(int square1, int square2)
    {
        const int square1Offset = 64;

        return Constants.ChebyshevDistance[(square1 * square1Offset) + square2];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ChebyshevDistanceLookupDoubleArray(int square1, int square2)
    {
        return Constants.ChebyshevDistanceDoubleArray[square1][square2];
    }

    public static class Constants
    {
        /// <summary>
        /// 64x64
        /// </summary>
        public static ReadOnlySpan<int> ChebyshevDistance =>
        [
            0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7,
            2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7,
            3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7,
            4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7,
            5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7,
            6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7,
            7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7,
            1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 6, 6, 6, 6, 6, 6, 6, 7,
            1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6,
            3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6,
            4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6,
            5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6,
            6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6,
            7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 6, 6, 6, 6, 6, 6,
            2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7,
            2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6,
            2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5,
            4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5,
            6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5,
            7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5,
            3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7,
            3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6,
            3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4,
            5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4,
            6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4,
            7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7,
            4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6,
            4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5,
            4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3,
            5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3,
            6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3,
            7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3,
            5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7,
            5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6,
            5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4,
            5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2,
            6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2,
            7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2,
            6, 6, 6, 6, 6, 6, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7,
            6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6,
            6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5,
            6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4,
            6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3,
            6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2,
            6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1,
            7, 6, 6, 6, 6, 6, 6, 6, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1,
            7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6,
            7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5,
            7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4,
            7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3,
            7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2,
            7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0,
        ];

        /// <summary>
        /// 64x64
        /// </summary>
        public static readonly int[][] ChebyshevDistanceDoubleArray =
        [
            [0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7],
            [1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
            [2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
            [3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
            [4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
            [5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
            [6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
            [7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
            [1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 6, 6, 6, 6, 6, 6, 6, 7],
            [1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6],
            [2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
            [3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
            [4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
            [5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
            [6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
            [7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 6, 6, 6, 6, 6, 6],
            [2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7],
            [2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6],
            [2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5],
            [3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5],
            [4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5],
            [5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5],
            [6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5],
            [7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5],
            [3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7],
            [3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6],
            [3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5],
            [3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4],
            [4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4],
            [5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4],
            [6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4],
            [7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4],
            [4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7],
            [4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6],
            [4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5],
            [4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4],
            [4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3],
            [5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3],
            [6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3],
            [7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3],
            [5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7],
            [5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6],
            [5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5],
            [5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4],
            [5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3],
            [5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2],
            [6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2],
            [7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2],
            [6, 6, 6, 6, 6, 6, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7],
            [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6],
            [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5],
            [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4],
            [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3],
            [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2],
            [6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1],
            [7, 6, 6, 6, 6, 6, 6, 6, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1],
            [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7],
            [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6],
            [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5],
            [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4],
            [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3],
            [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2],
            [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1],
            [7, 7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0]
        ];
    }
}
