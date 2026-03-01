/*
 *
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
 *  AMD Ryzen 9 9955HX 2.50GHz, 1 CPU, 32 logical and 16 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
 *  
 *  
 *  | Method             | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
 *  | Naive              | 1.743 us | 0.0325 us | 0.0713 us | 1.724 us |  1.00 |    0.06 |         - |          NA |
 *  | ManuallyUnrolled   | 1.266 us | 0.0212 us | 0.0199 us | 1.271 us |  0.73 |    0.03 |         - |          NA |
 *  | Vectorized_128     | 4.293 us | 0.0837 us | 0.1174 us | 4.245 us |  2.47 |    0.12 |         - |          NA |
 *  | Vectorized_128_tmp | 4.301 us | 0.0860 us | 0.1506 us | 4.222 us |  2.47 |    0.13 |         - |          NA |
 *  | Vectorized_256     | 4.257 us | 0.0851 us | 0.1659 us | 4.195 us |  2.45 |    0.14 |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Lynx.Benchmark;

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
    public short Naive()
    {
        for (int i = 0; i < QuietHistoryLength; ++i)
        {
            _quietHistory[i] = (short)(_quietHistory[i] * 3 / 4);
        }

        return _quietHistory.Last();
    }

    [Benchmark]
    public short ManuallyUnrolled()
    {
        ManuallyUnroll();

        return _quietHistory.Last();
    }

    [Benchmark]
    public unsafe short Vectorized_128()
    {
        if (Vector128.IsHardwareAccelerated && QuietHistoryLength >= Vector128<byte>.Count)
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

    [Benchmark]
    public unsafe short Vectorized_128_tmp()
    {
        if (Vector128.IsHardwareAccelerated && QuietHistoryLength >= Vector128<byte>.Count)
        {
            nuint oneVectorAwayFromEnd = (nuint)(QuietHistoryLength - Vector128<int>.Count);
            nuint elementOffset = 0;
            Vector128<short> loaded;

            ref short quietHistoryRef = ref MemoryMarshal.GetReference(_quietHistory);

            for (; elementOffset <= oneVectorAwayFromEnd; elementOffset += (nuint)Vector128<short>.Count)
            {
                loaded = Vector128.LoadUnsafe(ref quietHistoryRef, elementOffset);
                var tmp = loaded * 3;
                loaded = tmp / 4;

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

    [Benchmark]
    public unsafe short Vectorized_256()
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
