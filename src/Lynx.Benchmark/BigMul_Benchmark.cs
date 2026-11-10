/* 
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Benchmark;

public class BigMul_Benchmark : BaseBenchmark
{
    private nuint _ttLength;

    private ulong[] _hashes = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Parse once, outside the measured region
        var real = Engine._benchmarkFens.Select(fen => new Position(fen).UniqueIdentifier).ToArray();

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
            result ^= Bmi2.X64.MultiplyNoFlags(hash, _ttLength);
        }

        return result;
    }
}
