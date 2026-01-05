/*
 *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9955HX 2.50GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


| Method                                                   | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|--------------------------------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
| Naive                                                    | 2.302 us | 0.0108 us | 0.0090 us |  1.00 |         - |          NA |
| TemporaryVariable                                        | 1.324 us | 0.0136 us | 0.0127 us |  0.58 |         - |          NA |
| ManuallyVectorized                                       | 1.328 us | 0.0213 us | 0.0199 us |  0.58 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariable                  | 1.111 us | 0.0042 us | 0.0035 us |  0.48 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed          | 1.229 us | 0.0111 us | 0.0093 us |  0.53 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2         | 1.162 us | 0.0232 us | 0.0347 us |  0.50 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 1.345 us | 0.0031 us | 0.0026 us |  0.58 |         - |          NA |


BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 3.01GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3

| Method                                                   | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|--------------------------------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
| Naive                                                    | 3.353 us | 0.0333 us | 0.0312 us |  1.00 |         - |          NA |
| TemporaryVariable                                        | 2.874 us | 0.0239 us | 0.0224 us |  0.86 |         - |          NA |
| ManuallyVectorized                                       | 2.948 us | 0.0259 us | 0.0242 us |  0.88 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariable                  | 2.683 us | 0.0359 us | 0.0336 us |  0.80 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed          | 2.755 us | 0.0250 us | 0.0234 us |  0.82 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2         | 2.296 us | 0.0164 us | 0.0153 us |  0.68 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 2.361 us | 0.0178 us | 0.0166 us |  0.70 |         - |          NA |


BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.7462/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3

| Method                                                   | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|--------------------------------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
| Naive                                                    | 3.428 us | 0.0040 us | 0.0033 us |  1.00 |         - |          NA |
| TemporaryVariable                                        | 2.940 us | 0.0030 us | 0.0028 us |  0.86 |         - |          NA |
| ManuallyVectorized                                       | 3.000 us | 0.0046 us | 0.0036 us |  0.88 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariable                  | 2.737 us | 0.0382 us | 0.0339 us |  0.80 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed          | 2.774 us | 0.0070 us | 0.0066 us |  0.81 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2         | 2.337 us | 0.0031 us | 0.0027 us |  0.68 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 2.323 us | 0.0086 us | 0.0071 us |  0.68 |         - |          NA |


BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3

| Method                                                   | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------------------------------------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
| Naive                                                    | 4.767 us | 0.2842 us | 0.8015 us | 4.545 us |  1.02 |    0.23 |         - |          NA |
| TemporaryVariable                                        | 3.864 us | 0.0765 us | 0.1545 us | 3.851 us |  0.83 |    0.13 |         - |          NA |
| ManuallyVectorized                                       | 3.855 us | 0.0770 us | 0.1800 us | 3.829 us |  0.83 |    0.13 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariable                  | 3.914 us | 0.0774 us | 0.1158 us | 3.908 us |  0.84 |    0.13 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed          | 3.258 us | 0.0647 us | 0.0988 us | 3.290 us |  0.70 |    0.10 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2         | 2.454 us | 0.0469 us | 0.0576 us | 2.466 us |  0.53 |    0.08 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 2.554 us | 0.0466 us | 0.0864 us | 2.568 us |  0.55 |    0.08 |         - |          NA |


 BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.2 (24G325) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a

| Method                                                   | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------------------------------------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
| Naive                                                    | 3.214 us | 0.0919 us | 0.2694 us | 3.250 us |  1.01 |    0.13 |         - |          NA |
| TemporaryVariable                                        | 3.077 us | 0.1160 us | 0.3420 us | 3.039 us |  0.96 |    0.14 |         - |          NA |
| ManuallyVectorized                                       | 2.147 us | 0.0452 us | 0.1252 us | 2.121 us |  0.67 |    0.07 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariable                  | 1.958 us | 0.0672 us | 0.1983 us | 1.897 us |  0.61 |    0.08 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed          | 1.959 us | 0.0707 us | 0.2083 us | 1.892 us |  0.61 |    0.09 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2         | 1.740 us | 0.0641 us | 0.1880 us | 1.701 us |  0.55 |    0.08 |         - |          NA |
| ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 1.589 us | 0.0520 us | 0.1532 us | 1.515 us |  0.50 |    0.07 |         - |          NA |
 * 
*/

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class HistoryAging_Vectorization_Benchmark : BaseBenchmark
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
    public void Naive()
    {
        for (int i = 0; i < QuietHistoryLength; ++i)
        {
            _quietHistory[i] = (short)(_quietHistory[i] * 3 / 4);
        }
    }

    [Benchmark]
    public void TemporaryVariable()
    {
        for (int i = 0; i < QuietHistoryLength; ++i)
        {
            int tmp = _quietHistory[i] * 3;
            _quietHistory[i] = (short)(tmp / 4);
        }
    }

    [Benchmark]
    public void ManuallyVectorized()
    {
        for (int i = 0; i < QuietHistoryLength; i += 4)
        {
            _quietHistory[i] = (short)(_quietHistory[i] * 3 / 4);
            _quietHistory[i + 1] = (short)(_quietHistory[i + 1] * 3 / 4);
            _quietHistory[i + 2] = (short)(_quietHistory[i + 2] * 3 / 4);
            _quietHistory[i + 3] = (short)(_quietHistory[i + 3] * 3 / 4);
        }
    }

    [Benchmark]
    public void ManuallyVectorizedWithTemporaryVariable()
    {
        for (int i = 0; i < QuietHistoryLength; i += 4)
        {
            int tmp1 = _quietHistory[i] * 3;
            int tmp2 = _quietHistory[i + 1] * 3;
            int tmp3 = _quietHistory[i + 2] * 3;
            int tmp4 = _quietHistory[i + 3] * 3;

            _quietHistory[i] = (short)(tmp1 / 4);
            _quietHistory[i + 1] = (short)(tmp2 / 4);
            _quietHistory[i + 2] = (short)(tmp3 / 4);
            _quietHistory[i + 3] = (short)(tmp4 / 4);
        }
    }

    [Benchmark]
    public unsafe void ManuallyVectorizedWithTemporaryVariableAndFixed()
    {
        fixed (short* histPtr = _quietHistory)
        {
            for (int i = 0; i < QuietHistoryLength; i += 4)
            {
                short* start = histPtr + i;

                int tmp1 = *start * 3;
                int tmp2 = *(start + 1) * 3;
                int tmp3 = *(start + 2) * 3;
                int tmp4 = *(start + 3) * 3;

                _quietHistory[i] = (short)(tmp1 / 4);
                _quietHistory[i + 1] = (short)(tmp2 / 4);
                _quietHistory[i + 2] = (short)(tmp3 / 4);
                _quietHistory[i + 3] = (short)(tmp4 / 4);
            }
        }
    }

    [Benchmark]
    public unsafe void ManuallyVectorizedWithTemporaryVariableAndFixed2()
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

    [Benchmark]
    public unsafe void ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8()
    {
        fixed (short* histPtr = _quietHistory)
        {
            for (int i = 0; i < QuietHistoryLength; i += 8)
            {
                short* start = histPtr + i;

                short* h2 = start + 1;
                short* h3 = start + 2;
                short* h4 = start + 3;
                short* h5 = start + 4;
                short* h6 = start + 5;
                short* h7 = start + 6;
                short* h8 = start + 7;

                int tmp1 = *start * 3;
                int tmp2 = *h2 * 3;
                int tmp3 = *h3 * 3;
                int tmp4 = *h4 * 3;
                int tmp5 = *h5 * 3;
                int tmp6 = *h6 * 3;
                int tmp7 = *h7 * 3;
                int tmp8 = *h8 * 3;

                *start = (short)(tmp1 / 4);
                *h2 = (short)(tmp2 / 4);
                *h3 = (short)(tmp3 / 4);
                *h4 = (short)(tmp4 / 4);
                *h5 = (short)(tmp5 / 4);
                *h6 = (short)(tmp6 / 4);
                *h7 = (short)(tmp7 / 4);
                *h8 = (short)(tmp8 / 4);
            }
        }
    }
}
