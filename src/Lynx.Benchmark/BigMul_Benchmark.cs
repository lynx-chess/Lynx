/*
 * 
 * 
 *  ; Lynx.Benchmark.BigMul_Benchmark.BigMul() and Lynx.Benchmark.BigMul_Benchmark.Manual()
 *         push      rbx
 *         sub       rsp,10
 *         xor       eax,eax
 *         mov       [rsp+8],rax
 *         xor       eax,eax
 *         mov       r8,[rcx+8]
 *         mov       r10d,[r8+8]
 *         test      r10d,r10d
 *         jle       short M00_L01
 *         add       r8,10
 *         nop
 *  M00_L00:
 *         mov       rdx,[r8]
 *         mov       r9,[rcx+10]
 *         lea       r11,[rsp+8]
 *         mulx      rdx,rbx,r9
 *         mov       [r11],rbx
 *         xor       rax,rdx
 *         add       r8,8
 *         dec       r10d
 *         jne       short M00_L00
 *  M00_L01:
 *         add       rsp,10
 *         pop       rbx
 *         ret
 *  ; Total bytes of code 70
 *  
 *  
 *   ; Lynx.Benchmark.BigMul_Benchmark.Intrinsics()
 *         xor       eax,eax
 *         mov       r8,[rcx+8]
 *         mov       r10d,[r8+8]
 *         test      r10d,r10d
 *         jle       short M00_L01
 *         mov       rcx,[rcx+10]
 *         add       r8,10
 *         nop       word ptr [rax+rax]
 *  M00_L00:
 *         mov       rdx,[r8]
 *         mulx      rdx,rdx,rcx
 *         xor       rax,rdx
 *         add       r8,8
 *         dec       r10d
 *         jne       short M00_L00
 *  M00_L01:
 *         ret
 *  ; Total bytes of code 53
 * 
 * 
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
 *  AMD Ryzen 9 9955HX 2.50GHz, 1 CPU, 32 logical and 16 physical cores
 *  .NET SDK 11.0.100
 *    [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v4
 *  
 *  | Method     | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------- |---------:|---------:|---------:|---------:|------:|--------:|----------:|------------:|
 *  | Manual     | 39.20 ns | 0.794 ns | 1.189 ns | 38.54 ns |  1.00 |    0.04 |         - |          NA |
 *  | BigMul     | 38.18 ns | 0.313 ns | 0.244 ns | 38.14 ns |  0.97 |    0.03 |         - |          NA |
 *  | Intrinsics | 34.48 ns | 0.704 ns | 1.288 ns | 34.44 ns |  0.88 |    0.04 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.404
 *    [Host]     : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v3
 *  
 *  | Method     | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |----------- |---------:|---------:|---------:|------:|----------:|------------:|
 *  | Manual     | 95.26 ns | 0.291 ns | 0.273 ns |  1.00 |         - |          NA |
 *  | BigMul     | 95.63 ns | 0.400 ns | 0.334 ns |  1.00 |         - |          NA |
 *  | Intrinsics | 74.59 ns | 0.148 ns | 0.131 ns |  0.78 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
 *  AMD EPYC 9V45 4.29GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.404
 *    [Host]     : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
 *  
 *  | Method     | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |----------- |---------:|---------:|---------:|------:|----------:|------------:|
 *  | Manual     | 46.16 ns | 0.485 ns | 0.430 ns |  1.00 |         - |          NA |
 *  | BigMul     | 45.44 ns | 0.188 ns | 0.146 ns |  0.98 |         - |          NA |
 *  | Intrinsics | 40.25 ns | 0.279 ns | 0.233 ns |  0.87 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.404
 *    [Host]     : .NET 10.0.15 (10.0.15, 10.0.1526.53008), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.15 (10.0.15, 10.0.1526.53008), Arm64 RyuJIT armv8.0-a
 *  
 *  | Method     | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
 *  | Manual     | 69.31 ns | 2.400 ns | 6.962 ns |  1.01 |    0.14 |         - |          NA |
 *  | BigMul     | 64.78 ns | 2.632 ns | 7.721 ns |  0.94 |    0.15 |         - |          NA |
 *  | Intrinsics |       NA |       NA |       NA |     ? |       ? |        NA |           ? |
 *  
 * 
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Benchmark;

[DisassemblyDiagnoser]
public class BigMul_Benchmark : BaseBenchmark
{
    private nuint _ttLength;

    private ulong[] _hashes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _hashes = [.. Engine._benchmarkFens.Select(fen => new Position(fen).UniqueIdentifier)];

        // Runtime value, not const, so the JIT can't specialize on it
        _ttLength = 0x0000000000100000;
    }

    [Benchmark(Baseline = true)]
    public ulong Manual()
    {
        ulong result = 0;

        foreach (var hash in _hashes)
        {
            result ^= (ulong)(((UInt128)hash * (UInt128)_ttLength) >> 64);
        }

        return result;
    }

    [Benchmark]
    public ulong BigMul()
    {
        ulong result = 0;

        foreach (var hash in _hashes)
        {
            result ^= Math.BigMul(hash, _ttLength, out var _);
        }

        return result;
    }

    [Benchmark]
    public ulong Intrinsics()
    {
        ulong result = 0;

        foreach (var hash in _hashes)
        {
            result ^= Bmi2.X64.IsSupported
                ? Bmi2.X64.MultiplyNoFlags(hash, _ttLength)
                : Math.BigMul(hash, _ttLength, out var _);
        }

        return result;
    }
}
