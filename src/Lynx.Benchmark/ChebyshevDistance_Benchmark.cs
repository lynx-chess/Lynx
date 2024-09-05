﻿using BenchmarkDotNet.Attributes;
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
                result += ChebyshevDistanceOnTheFly(sq1Array[sq1], sq2Array[sq2]);
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
    }
}
