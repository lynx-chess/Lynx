using BenchmarkDotNet.Attributes;
using System.Threading.Channels;

namespace Lynx.Benchmark;

public class BenchUCI : BaseBenchmark
{
    private readonly Channel<string> _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(100_000) { SingleReader = true, SingleWriter = false });

    [Benchmark]
    public async Task<(int, long)> Bench_DefaultDepth()
    {
        return await OpenBench.Bench(Configuration.EngineSettings.BenchDepth, _channel);
    }
}
