/*
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method             | Mean     | Error    | StdDev   | Allocated |
 *  |------------------- |---------:|---------:|---------:|----------:|
 *  | Bench_DefaultDepth | 512.4 ms | 10.17 ms | 28.18 ms | 265.16 MB |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2113) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method             | Mean     | Error   | StdDev   | Gen0      | Gen1      | Gen2      | Allocated |
 *  |------------------- |---------:|--------:|---------:|----------:|----------:|----------:|----------:|
 *  | Bench_DefaultDepth | 410.0 ms | 8.12 ms | 16.03 ms | 5000.0000 | 1000.0000 | 1000.0000 | 265.18 MB |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.6.9 (21G726) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method             | Mean    | Error    | StdDev   | Gen0       | Gen1      | Gen2      | Allocated |
 *  |------------------- |--------:|---------:|---------:|-----------:|----------:|----------:|----------:|
 *  | Bench_DefaultDepth | 1.139 s | 0.1229 s | 0.3526 s | 13000.0000 | 2000.0000 | 1000.0000 | 265.21 MB |
 */

using BenchmarkDotNet.Attributes;
using System.Threading.Channels;

namespace Lynx.Benchmark;

public class UCI_Benchmark : BaseBenchmark
{
    private readonly Channel<string> _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(100_000) { SingleReader = true, SingleWriter = false });

    [Benchmark]
    public (int, long) Bench_DefaultDepth()
    {
        var engine = new Engine(_channel.Writer);
        return engine.Bench(Configuration.EngineSettings.BenchDepth);
    }
}
