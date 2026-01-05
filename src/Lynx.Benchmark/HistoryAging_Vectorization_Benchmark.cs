/*
 *
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
 *  AMD Ryzen 9 9955HX 2.50GHz, 1 CPU, 32 logical and 16 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
 *  
 *  | Method                                                   | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
 *  | Naive                                                    | 2.283 us | 0.0076 us | 0.0074 us | 2.281 us |  1.00 |    0.00 |         - |          NA |
 *  | TemporaryVariable                                        | 1.345 us | 0.0261 us | 0.0391 us | 1.324 us |  0.59 |    0.02 |         - |          NA |
 *  | ManuallyVectorized                                       | 1.336 us | 0.0258 us | 0.0336 us | 1.319 us |  0.59 |    0.01 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariable                  | 1.120 us | 0.0103 us | 0.0115 us | 1.114 us |  0.49 |    0.01 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed          | 1.233 us | 0.0133 us | 0.0111 us | 1.229 us |  0.54 |    0.00 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2         | 1.130 us | 0.0018 us | 0.0015 us | 1.130 us |  0.49 |    0.00 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 1.344 us | 0.0066 us | 0.0055 us | 1.342 us |  0.59 |    0.00 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.79GHz), 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
 *  
 *  | Method                                                   | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Naive                                                    | 3.400 us | 0.0354 us | 0.0313 us |  1.00 |    0.01 |         - |          NA |
 *  | TemporaryVariable                                        | 2.735 us | 0.0517 us | 0.0634 us |  0.80 |    0.02 |         - |          NA |
 *  | ManuallyVectorized                                       | 2.592 us | 0.0032 us | 0.0029 us |  0.76 |    0.01 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariable                  | 2.352 us | 0.0011 us | 0.0010 us |  0.69 |    0.01 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed          | 2.266 us | 0.0018 us | 0.0015 us |  0.67 |    0.01 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2         | 1.842 us | 0.0036 us | 0.0034 us |  0.54 |    0.00 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 1.945 us | 0.0007 us | 0.0005 us |  0.57 |    0.01 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.7462/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *  
 *  | Method                                                   | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | Naive                                                    | 3.243 us | 0.0031 us | 0.0024 us |  1.00 |         - |          NA |
 *  | TemporaryVariable                                        | 3.048 us | 0.0050 us | 0.0044 us |  0.94 |         - |          NA |
 *  | ManuallyVectorized                                       | 3.022 us | 0.0180 us | 0.0160 us |  0.93 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariable                  | 2.701 us | 0.0042 us | 0.0035 us |  0.83 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed          | 2.803 us | 0.0023 us | 0.0020 us |  0.86 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2         | 2.369 us | 0.0285 us | 0.0252 us |  0.73 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 2.377 us | 0.0049 us | 0.0044 us |  0.73 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *  
 *  | Method                                                   | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Naive                                                    | 3.227 us | 0.0615 us | 0.0604 us |  1.00 |    0.03 |         - |          NA |
 *  | TemporaryVariable                                        | 2.887 us | 0.0571 us | 0.1400 us |  0.89 |    0.05 |         - |          NA |
 *  | ManuallyVectorized                                       | 3.069 us | 0.0604 us | 0.0806 us |  0.95 |    0.03 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariable                  | 3.213 us | 0.0640 us | 0.1016 us |  1.00 |    0.04 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed          | 2.511 us | 0.0502 us | 0.0930 us |  0.78 |    0.03 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2         | 1.968 us | 0.0392 us | 0.0717 us |  0.61 |    0.02 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 2.065 us | 0.0411 us | 0.0782 us |  0.64 |    0.03 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.2 (24G325) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a
 *  
 *  | Method                                                   | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
 *  | Naive                                                    | 3.214 us | 0.0919 us | 0.2694 us | 3.250 us |  1.01 |    0.13 |         - |          NA |
 *  | TemporaryVariable                                        | 3.077 us | 0.1160 us | 0.3420 us | 3.039 us |  0.96 |    0.14 |         - |          NA |
 *  | ManuallyVectorized                                       | 2.147 us | 0.0452 us | 0.1252 us | 2.121 us |  0.67 |    0.07 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariable                  | 1.958 us | 0.0672 us | 0.1983 us | 1.897 us |  0.61 |    0.08 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed          | 1.959 us | 0.0707 us | 0.2083 us | 1.892 us |  0.61 |    0.09 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2         | 1.740 us | 0.0641 us | 0.1880 us | 1.701 us |  0.55 |    0.08 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 1.589 us | 0.0520 us | 0.1532 us | 1.515 us |  0.50 |    0.07 |         - |          NA |
 *  
 *  
 *   BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.2 (24G325) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a
 *  
 *  | Method                                                   | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
 *  | Naive                                                    | 3.605 us | 0.1856 us | 0.5415 us | 3.616 us |  1.02 |    0.22 |         - |          NA |
 *  | TemporaryVariable                                        | 3.095 us | 0.1029 us | 0.2985 us | 3.123 us |  0.88 |    0.16 |         - |          NA |
 *  | ManuallyVectorized                                       | 2.276 us | 0.0730 us | 0.2107 us | 2.241 us |  0.65 |    0.12 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariable                  | 1.898 us | 0.0620 us | 0.1769 us | 1.836 us |  0.54 |    0.10 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed          | 1.872 us | 0.0626 us | 0.1766 us | 1.832 us |  0.53 |    0.10 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2         | 1.585 us | 0.0331 us | 0.0960 us | 1.558 us |  0.45 |    0.07 |         - |          NA |
 *  | ManuallyVectorizedWithTemporaryVariableAndFixed2_Length8 | 1.354 us | 0.0258 us | 0.0254 us | 1.345 us |  0.38 |    0.06 |         - |          NA |
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
