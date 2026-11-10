using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Benchmark;

public class BigMul_Benchmark : BaseBenchmark
{
    private const nuint TTLength = 0x0000000000100000;

    public static IEnumerable<Position> Data => Engine._benchmarkFens.Select(fen => new Position(fen));

    [Benchmark(Baseline = true)]
    public ulong Manual()
    {
        ulong result = 0;

        foreach (var position in Data)
        {
            result ^= (ulong)(((UInt128)position.UniqueIdentifier * (UInt128)TTLength) >> 64);
        }

        return result;
    }

    [Benchmark]
    public ulong BigMul()
    {
        ulong result = 0;

        foreach (var position in Data)
        {
            result ^= Math.BigMul(position.UniqueIdentifier, TTLength, out var _);
        }

        return result;
    }

    [Benchmark]
    public ulong Intrinsics()
    {
        ulong result = 0;

        foreach (var position in Data)
        {
            result ^= Bmi2.X64.MultiplyNoFlags(position.UniqueIdentifier, TTLength);
        }

        return result;
    }
}
