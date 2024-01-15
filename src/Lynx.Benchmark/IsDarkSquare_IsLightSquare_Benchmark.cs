/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method        | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |-------------- |---------:|---------:|---------:|------:|----------:|------------:|
 *  | GetBit        | 48.69 ns | 0.211 ns | 0.197 ns |  1.00 |         - |          NA |
 *  | Lookup        | 40.66 ns | 0.122 ns | 0.114 ns |  0.84 |         - |          NA |
 *  | AntiDiagonals | 53.32 ns | 0.111 ns | 0.099 ns |  1.09 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method        | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |-------------- |---------:|---------:|---------:|------:|----------:|------------:|
 *  | GetBit        | 48.14 ns | 0.164 ns | 0.128 ns |  1.00 |         - |          NA |
 *  | Lookup        | 40.85 ns | 0.020 ns | 0.016 ns |  0.85 |         - |          NA |
 *  | AntiDiagonals | 52.64 ns | 0.048 ns | 0.040 ns |  1.09 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX
 *
 *  | Method        | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |-------------- |---------:|---------:|---------:|------:|----------:|------------:|
 *  | GetBit        | 76.90 ns | 0.209 ns | 0.174 ns |  1.00 |         - |          NA |
 *  | Lookup        | 60.36 ns | 0.273 ns | 0.228 ns |  0.78 |         - |          NA |
 *  | AntiDiagonals | 65.77 ns | 0.337 ns | 0.299 ns |  0.86 |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class IsDarkSquare_IsLightSquare_Benchmark : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    public bool GetBit()
    {
        bool result = false;
        for (int i = 0; i < 64; ++i)
        {
            result ^= IsLightSquare_GetBit(i);
        }

        return result;
    }

    [Benchmark]
    public bool Lookup()
    {
        bool result = false;
        for (int i = 0; i < 64; ++i)
        {
            result ^= IsLightSquare_Lookup(i);
        }

        return result;
    }

    [Benchmark]
    public bool AntiDiagonals()
    {
        bool result = false;
        for (int i = 0; i < 64; ++i)
        {
            result ^= IsLightSquare_AntiDiagonals(i);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsLightSquare_GetBit(int square)
    {
        return Masks.LightSquaresMask.GetBit(square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsLightSquare_Lookup(int square)
    {
        return ((Masks.LightSquaresMask >> square) & 1) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsLightSquare_AntiDiagonals(int square)
    {
        return (((9 * square) + 8) & 8) != 0;
    }
}
