/*
 *
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.7462/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *  
 *  | Method             | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | Naive              | 3.423 us | 0.0032 us | 0.0027 us |  1.00 |         - |          NA |
 *  | TemporaryVariable  | 2.936 us | 0.0047 us | 0.0044 us |  0.86 |         - |          NA |
 *  | ManuallyVectorized | 2.991 us | 0.0026 us | 0.0022 us |  0.87 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *  
 *  | Method             | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | Naive              | 3.399 us | 0.0026 us | 0.0021 us |  1.00 |         - |          NA |
 *  | TemporaryVariable  | 2.905 us | 0.0051 us | 0.0045 us |  0.85 |         - |          NA |
 *  | ManuallyVectorized | 2.981 us | 0.0020 us | 0.0018 us |  0.88 |         - |          NA |
 *  
 *  
 *   BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
 *  
 *  | Method             | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | Naive              | 2.939 us | 0.0314 us | 0.0279 us |  1.00 |         - |          NA |
 *  | TemporaryVariable  | 2.717 us | 0.0088 us | 0.0082 us |  0.92 |         - |          NA |
 *  | ManuallyVectorized | 2.902 us | 0.0097 us | 0.0091 us |  0.99 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.2 (24G325) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.101
 *    [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a
 *  | Method             | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
 *  | Naive              | 2.898 us | 0.0653 us | 0.1895 us | 2.819 us |  1.00 |    0.09 |         - |          NA |
 *  | TemporaryVariable  | 2.923 us | 0.0848 us | 0.2488 us | 2.922 us |  1.01 |    0.11 |         - |          NA |
 *  | ManuallyVectorized | 2.494 us | 0.0715 us | 0.2085 us | 2.483 us |  0.86 |    0.09 |         - |          NA |
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
        fixed (short* ttPtr = _quietHistory)
        {
            for (int i = 0; i < QuietHistoryLength; i += 4)
            {
                var start = ttPtr + 4 * i;

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
        fixed (short* ttPtr = _quietHistory)
        {
            for (int i = 0; i < QuietHistoryLength; i += 4)
            {
                var start = ttPtr + 4 * i;

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
