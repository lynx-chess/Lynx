/*
 *
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
 *  AMD Ryzen 9 9955HX 2.50GHz, 1 CPU, 32 logical and 16 physical cores
 *  .NET SDK 10.0.101
 *    [Host]    : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
 *    Scalar    : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT
 *    Vector128 : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v2
 *    Vector256 : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
 *  
 *  IterationTime=250ms  MaxIterationCount=20  WarmupCount=1
 *  
 *  | Method         | Job       | Mean     | StdDev    | Ratio | Code Size | Allocated | Alloc Ratio |
 *  |--------------- |---------- |---------:|----------:|------:|----------:|----------:|------------:|
 *  | Vectorized     | Scalar    | 1.165 us | 0.0072 us |  1.00 |     825 B |         - |          NA |
 *  | Vectorized     | Vector128 | 4.072 us | 0.0255 us |  3.50 |     780 B |         - |          NA |
 *  | Vectorized     | Vector256 | 4.214 us | 0.1248 us |  3.62 |   1,107 B |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Lynx_SIMD.Benchmark;

public class HistoryAging_Vectorized_Benchmark : BaseBenchmark
{
    /// <summary>
    /// 12 * 64 * 2 * 2 
    /// </summary>
    private const int QuietHistoryLength = 3_072;

    private readonly short[] _quietHistory = GC.AllocateArray<short>(QuietHistoryLength, pinned: true);

    [GlobalSetup]
    public void Setup()
    {
        for (int i = 0; i < QuietHistoryLength; ++i)
        {
            _quietHistory[i] = (short)Random.Shared.Next();
        }
    }

    [Benchmark(Baseline = true)]
    public unsafe short Vectorized()
    {
        if (Vector256.IsHardwareAccelerated && QuietHistoryLength >= Vector256<byte>.Count)
        {
            nuint oneVectorAwayFromEnd = (nuint)(QuietHistoryLength - Vector256<int>.Count);
            nuint elementOffset = 0;
            Vector256<short> loaded;

            ref short quietHistoryRef = ref MemoryMarshal.GetReference(_quietHistory);

            for (; elementOffset <= oneVectorAwayFromEnd; elementOffset += (nuint)Vector256<short>.Count)
            {
                loaded = Vector256.LoadUnsafe(ref quietHistoryRef, elementOffset);
                loaded = loaded * 3 / 4;

                loaded.StoreUnsafe(ref quietHistoryRef, elementOffset);
            }

            for (var i = elementOffset; i < QuietHistoryLength; i++)
            {
                _quietHistory[i] = (short)(_quietHistory[i] * 3 / 4);
            }
        }
        else if (Vector128.IsHardwareAccelerated && QuietHistoryLength >= Vector128<byte>.Count)
        {
            nuint oneVectorAwayFromEnd = (nuint)(QuietHistoryLength - Vector128<int>.Count);
            nuint elementOffset = 0;
            Vector128<short> loaded;

            ref short quietHistoryRef = ref MemoryMarshal.GetReference(_quietHistory);

            for (; elementOffset <= oneVectorAwayFromEnd; elementOffset += (nuint)Vector128<short>.Count)
            {
                loaded = Vector128.LoadUnsafe(ref quietHistoryRef, elementOffset);
                loaded = loaded * 3 / 4;

                loaded.StoreUnsafe(ref quietHistoryRef, elementOffset);
            }

            for (var i = elementOffset; i < QuietHistoryLength; i++)
            {
                _quietHistory[i] = (short)(_quietHistory[i] * 3 / 4);
            }
        }
        else
        {
            ManuallyUnroll();
        }

        return _quietHistory.Last();
    }

    private unsafe void ManuallyUnroll()
    {
        fixed (short* histPtr = _quietHistory)
        {
            for (int i = 0; i < QuietHistoryLength; i += 4)
            {
                short* start = histPtr + i;

                short* h2 = start + 1;
                short* h3 = start + 2;
                short* h4 = start + 3;

                int tmp1 = *start * 3;
                int tmp2 = *h2 * 3;
                int tmp3 = *h3 * 3;
                int tmp4 = *h4 * 3;

                *start = (short)(tmp1 / 4);
                *h2 = (short)(tmp2 / 4);
                *h3 = (short)(tmp3 / 4);
                *h4 = (short)(tmp4 / 4);
            }
        }
    }
}
