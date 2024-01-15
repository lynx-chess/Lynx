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
