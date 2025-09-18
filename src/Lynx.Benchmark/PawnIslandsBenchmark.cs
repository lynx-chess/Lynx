/*
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.3091) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.102
 *    [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
 *
 *  | Method                  | Mean     | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |------------------------ |---------:|---------:|---------:|------:|-------:|----------:|------------:|
 *  | Original                | 42.39 us | 0.246 us | 0.218 us |  1.00 | 2.9907 |  49.63 KB |        1.00 |
 *  | Original_SkipLocalsInit | 43.48 us | 0.287 us | 0.255 us |  1.03 | 2.9907 |  49.63 KB |        1.00 |
 *  | BitBoard                | 41.62 us | 0.388 us | 0.363 us |  0.98 | 2.9907 |  49.63 KB |        1.00 |
 *  | BitBoard_NoFileArray    | 40.30 us | 0.294 us | 0.275 us |  0.95 | 2.9907 |  49.63 KB |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.2 (22H313) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.102
 *    [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
 *
 *  | Method                  | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------------ |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | Original                | 55.11 us | 0.977 us | 0.816 us |  1.00 |    0.02 | 8.0566 |  49.64 KB |        1.00 |
 *  | Original_SkipLocalsInit | 55.39 us | 0.533 us | 0.445 us |  1.01 |    0.02 | 8.0566 |  49.64 KB |        1.00 |
 *  | BitBoard                | 54.91 us | 0.704 us | 0.624 us |  1.00 |    0.02 | 8.0566 |  49.64 KB |        1.00 |
 *  | BitBoard_NoFileArray    | 56.32 us | 0.862 us | 0.806 us |  1.02 |    0.02 | 8.0566 |  49.64 KB |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.102
 *    [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
 *  | Method                  | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------------ |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | Original                | 29.08 us | 0.084 us | 0.075 us |  1.00 |    0.00 | 8.0872 |  49.64 KB |        1.00 |
 *  | Original_SkipLocalsInit | 29.09 us | 0.107 us | 0.089 us |  1.00 |    0.00 | 8.0872 |  49.64 KB |        1.00 |
 *  | BitBoard                | 28.24 us | 0.227 us | 0.189 us |  0.97 |    0.01 | 8.0872 |  49.64 KB |        1.00 |
 *  | BitBoard_NoFileArray    | 30.33 us | 0.605 us | 1.044 us |  1.04 |    0.04 | 8.0872 |  49.64 KB |        1.00 |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.ConstantsGenerator;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class PawnIslandsBenchmark : BaseBenchmark
{
#pragma warning disable IDE1006 // Naming Styles
    private static readonly string[] Data = Engine._benchmarkFens;
#pragma warning restore IDE1006 // Naming Styles

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

        return PawnIslandsGenerator.PawnIslandsCount[pawnFileBitBoard];
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

        return PawnIslandsGenerator.PawnIslandsCount[pawnFileBitBoard];
    }
}
