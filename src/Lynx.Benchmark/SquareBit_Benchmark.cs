/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method     | Size  | Mean          | Error        | StdDev       | Ratio | Allocated | Alloc Ratio |
 *  |----------- |------ |--------------:|-------------:|-------------:|------:|----------:|------------:|
 *  | Calculated | 1     |      30.14 ns |     0.063 ns |     0.056 ns |  1.00 |         - |          NA |
 *  | Array      | 1     |      38.67 ns |     0.272 ns |     0.255 ns |  1.28 |         - |          NA |
 *  |            |       |               |              |              |       |           |             |
 *  | Calculated | 10    |     306.84 ns |     1.408 ns |     1.248 ns |  1.00 |         - |          NA |
 *  | Array      | 10    |     400.81 ns |     2.386 ns |     2.115 ns |  1.31 |         - |          NA |
 *  |            |       |               |              |              |       |           |             |
 *  | Calculated | 100   |   3,062.41 ns |    14.661 ns |    12.996 ns |  1.00 |         - |          NA |
 *  | Array      | 100   |   3,936.57 ns |     3.119 ns |     2.435 ns |  1.28 |         - |          NA |
 *  |            |       |               |              |              |       |           |             |
 *  | Calculated | 1000  |  30,053.25 ns |   106.769 ns |    94.648 ns |  1.00 |         - |          NA |
 *  | Array      | 1000  |  39,305.42 ns |    35.967 ns |    30.034 ns |  1.31 |         - |          NA |
 *  |            |       |               |              |              |       |           |             |
 *  | Calculated | 10000 | 300,547.86 ns |   633.632 ns |   494.698 ns |  1.00 |         - |          NA |
 *  | Array      | 10000 | 394,684.22 ns | 2,329.190 ns | 1,944.979 ns |  1.31 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method     | Size  | Mean          | Error      | StdDev     | Ratio | Allocated | Alloc Ratio |
 *  |----------- |------ |--------------:|-----------:|-----------:|------:|----------:|------------:|
 *  | Calculated | 1     |      30.24 ns |   0.048 ns |   0.045 ns |  1.00 |         - |          NA |
 *  | Array      | 1     |      39.32 ns |   0.041 ns |   0.036 ns |  1.30 |         - |          NA |
 *  |            |       |               |            |            |       |           |             |
 *  | Calculated | 10    |     306.08 ns |   1.168 ns |   0.912 ns |  1.00 |         - |          NA |
 *  | Array      | 10    |     399.60 ns |   0.401 ns |   0.335 ns |  1.31 |         - |          NA |
 *  |            |       |               |            |            |       |           |             |
 *  | Calculated | 100   |   3,028.29 ns |   1.937 ns |   1.617 ns |  1.00 |         - |          NA |
 *  | Array      | 100   |   3,939.18 ns |   5.354 ns |   4.746 ns |  1.30 |         - |          NA |
 *  |            |       |               |            |            |       |           |             |
 *  | Calculated | 1000  |  30,028.22 ns |  39.488 ns |  36.937 ns |  1.00 |         - |          NA |
 *  | Array      | 1000  |  39,277.77 ns |  30.761 ns |  27.269 ns |  1.31 |         - |          NA |
 *  |            |       |               |            |            |       |           |             |
 *  | Calculated | 10000 | 300,133.21 ns | 608.569 ns | 539.480 ns |  1.00 |         - |          NA |
 *  | Array      | 10000 | 392,942.21 ns | 223.963 ns | 198.537 ns |  1.31 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method     | Size  | Mean          | Error        | StdDev       | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------- |------ |--------------:|-------------:|-------------:|------:|--------:|----------:|------------:|
 *  | Calculated | 1     |      30.21 ns |     0.251 ns |     0.235 ns |  1.00 |    0.00 |         - |          NA |
 *  | Array      | 1     |      30.70 ns |     0.337 ns |     0.316 ns |  1.02 |    0.01 |         - |          NA |
 *  |            |       |               |              |              |       |         |           |             |
 *  | Calculated | 10    |     308.05 ns |     2.942 ns |     2.752 ns |  1.00 |    0.00 |         - |          NA |
 *  | Array      | 10    |     308.23 ns |     3.475 ns |     3.251 ns |  1.00 |    0.02 |         - |          NA |
 *  |            |       |               |              |              |       |         |           |             |
 *  | Calculated | 100   |   3,018.77 ns |    34.015 ns |    31.818 ns |  1.00 |    0.00 |         - |          NA |
 *  | Array      | 100   |   3,030.89 ns |    23.458 ns |    21.942 ns |  1.00 |    0.01 |         - |          NA |
 *  |            |       |               |              |              |       |         |           |             |
 *  | Calculated | 1000  |  29,715.88 ns |   220.408 ns |   195.386 ns |  1.00 |    0.00 |         - |          NA |
 *  | Array      | 1000  |  30,241.26 ns |   336.921 ns |   281.344 ns |  1.02 |    0.01 |         - |          NA |
 *  |            |       |               |              |              |       |         |           |             |
 *  | Calculated | 10000 | 299,686.96 ns | 3,597.358 ns | 3,364.971 ns |  1.00 |    0.00 |         - |          NA |
 *  | Array      | 10000 | 301,357.55 ns | 3,036.923 ns | 2,535.968 ns |  1.01 |    0.02 |         - |          NA |
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
