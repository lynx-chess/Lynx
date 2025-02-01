/*
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.3091) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.102
 *    [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
 *
 *  | Method   | Mean     | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |--------- |---------:|---------:|---------:|------:|-------:|----------:|------------:|
 *  | Original | 41.69 us | 0.139 us | 0.123 us |  1.00 | 2.9907 |  49.63 KB |        1.00 |
 *  | BitBoard | 41.26 us | 0.234 us | 0.195 us |  0.99 | 2.9907 |  49.63 KB |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.102
 *    [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
 *
 *  | Method   | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |--------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | Original | 30.15 us | 0.376 us | 0.333 us |  1.00 |    0.02 | 8.0872 |  49.64 KB |        1.00 |
 *  | BitBoard | 29.31 us | 0.405 us | 0.338 us |  0.97 |    0.02 | 8.0872 |  49.64 KB |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.2 (22H313) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.102
 *    [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
 *
 *  | Method   | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |--------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | Original | 53.74 us | 0.864 us | 0.766 us |  1.00 |    0.02 | 8.0566 |  49.64 KB |        1.00 |
 *  | BitBoard | 55.06 us | 1.050 us | 1.078 us |  1.02 |    0.02 | 8.0566 |  49.64 KB |        1.00 |
 *
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class PawnIslandsBenchmark : BaseBenchmark
{
    private static readonly string[] Data = Engine._benchmarkFens;

    [Benchmark(Baseline = true)]
    public int Original()
    {
        var sum = 0;

        foreach (var fen in Engine._benchmarkFens)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitBoards;
            BitBoard whitePawns = pieces[(int)Piece.P];
            sum += Original(whitePawns);
        }

        return sum;
    }

    [Benchmark]
    public int Original_SkipLocalsInit()
    {
        var sum = 0;

        foreach (var fen in Engine._benchmarkFens)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitBoards;
            BitBoard whitePawns = pieces[(int)Piece.P];
            sum += Original_SkipLocalsInit(whitePawns);
        }

        return sum;
    }

    [Benchmark]
    public int BitBoard()
    {
        var sum = 0;

        foreach (var fen in Data)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitBoards;
            BitBoard whitePawns = pieces[(int)Piece.P];
            sum += BitBoard(whitePawns);
        }

        return sum;
    }

    [Benchmark]
    public int BitBoard_NoFileArray()
    {
        var sum = 0;

        foreach (var fen in Data)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitBoards;
            BitBoard whitePawns = pieces[(int)Piece.P];
            sum += BitBoard_NoFileArray(whitePawns);
        }

        return sum;
    }

    private static int Original(BitBoard pawns)
    {
        const int n = 1;

        Span<int> files = stackalloc int[8];

        while (pawns != default)
        {
            var squareIndex = pawns.GetLS1BIndex();
            pawns.ResetLS1B();

            files[Constants.File[squareIndex]] = n;
        }

        var islandCount = 0;
        var isIsland = false;

        for (int file = 0; file < files.Length; ++file)
        {
            if (files[file] == n)
            {
                if (!isIsland)
                {
                    isIsland = true;
                    ++islandCount;
                }
            }
            else
            {
                isIsland = false;
            }
        }

        return islandCount;
    }

    [SkipLocalsInit]
    private static int Original_SkipLocalsInit(BitBoard pawns)
    {
        const int n = 1;

        Span<int> files = stackalloc int[8];
        files.Clear();

        while (pawns != default)
        {
            var squareIndex = pawns.GetLS1BIndex();
            pawns.ResetLS1B();

            files[Constants.File[squareIndex]] = n;
        }

        var islandCount = 0;
        var isIsland = false;

        for (int file = 0; file < files.Length; ++file)
        {
            if (files[file] == n)
            {
                if (!isIsland)
                {
                    isIsland = true;
                    ++islandCount;
                }
            }
            else
            {
                isIsland = false;
            }
        }

        return islandCount;
    }

    private static int BitBoard(BitBoard pawns)
    {
        int pawnFileBitBoard = 0;

        while (pawns != default)
        {
            var squareIndex = pawns.GetLS1BIndex();
            pawns.ResetLS1B();

            // BitBoard.SetBit equivalent but for byte instead of ulong
            pawnFileBitBoard |= (1 << Constants.File[squareIndex]);
        }

        return PawnIslandsCount[pawnFileBitBoard];
    }

    private static int BitBoard_NoFileArray(BitBoard pawns)
    {
        int pawnFileBitBoard = 0;

        while (pawns != default)
        {
            var squareIndex = pawns.GetLS1BIndex();
            pawns.ResetLS1B();

            // BitBoard.SetBit equivalent but for byte instead of ulong
            pawnFileBitBoard |= (1 << (squareIndex % 8));
        }

        return PawnIslandsCount[pawnFileBitBoard];
    }

    private static ReadOnlySpan<int> PawnIslandsCount =>
    [
        0, 1, 1, 1, 1, 2, 1, 1, 1, 2,
        2, 2, 1, 2, 1, 1, 1, 2, 2, 2,
        2, 3, 2, 2, 1, 2, 2, 2, 1, 2,
        1, 1, 1, 2, 2, 2, 2, 3, 2, 2,
        2, 3, 3, 3, 2, 3, 2, 2, 1, 2,
        2, 2, 2, 3, 2, 2, 1, 2, 2, 2,
        1, 2, 1, 1, 1, 2, 2, 2, 2, 3,
        2, 2, 2, 3, 3, 3, 2, 3, 2, 2,
        2, 3, 3, 3, 3, 4, 3, 3, 2, 3,
        3, 3, 2, 3, 2, 2, 1, 2, 2, 2,
        2, 3, 2, 2, 2, 3, 3, 3, 2, 3,
        2, 2, 1, 2, 2, 2, 2, 3, 2, 2,
        1, 2, 2, 2, 1, 2, 1, 1, 1, 2,
        2, 2, 2, 3, 2, 2, 2, 3, 3, 3,
        2, 3, 2, 2, 2, 3, 3, 3, 3, 4,
        3, 3, 2, 3, 3, 3, 2, 3, 2, 2,
        2, 3, 3, 3, 3, 4, 3, 3, 3, 4,
        4, 4, 3, 4, 3, 3, 2, 3, 3, 3,
        3, 4, 3, 3, 2, 3, 3, 3, 2, 3,
        2, 2, 1, 2, 2, 2, 2, 3, 2, 2,
        2, 3, 3, 3, 2, 3, 2, 2, 2, 3,
        3, 3, 3, 4, 3, 3, 2, 3, 3, 3,
        2, 3, 2, 2, 1, 2, 2, 2, 2, 3,
        2, 2, 2, 3, 3, 3, 2, 3, 2, 2,
        1, 2, 2, 2, 2, 3, 2, 2, 1, 2,
        2, 2, 1, 2, 1,
        1
    ];
}
