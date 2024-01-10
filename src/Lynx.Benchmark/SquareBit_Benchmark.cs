/*
 *
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

file static class BitBoardHelpers
{
    public static readonly ulong[] SquareBitsArray =
        Enum.GetValuesAsUnderlyingType<BoardSquare>()
            .OfType<int>()
            .Select(square => 1UL << square)
            .ToArray();

    public static ulong CaculateSquareBit(int boardSquare)
    {
        return 1UL << boardSquare;
    }
}

public class SquareBit_Benchmark : BaseBenchmark
{
    [Params(1, 10, 100, 1_000, 10_000)]
    public int Size { get; set; }

    private BoardSquare[] _squares = null!;

    [GlobalSetup]
    public void Setup()
    {
        _squares = Enumerable.Range((int)BoardSquare.a8, (int)BoardSquare.h1).
            Select(_ => (BoardSquare)Random.Shared.Next((int)BoardSquare.a8, (int)BoardSquare.h1))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public ulong Calculated()
    {
        ulong count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var square in _squares)
            {
                count += BitBoardHelpers.CaculateSquareBit((int)square);
            }
        }

        return count;
    }

    [Benchmark]
    public ulong Array()
    {
        ulong count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var square in _squares)
            {
                count += BitBoardHelpers.SquareBitsArray[(int)square];
            }
        }

        return count;
    }
}
