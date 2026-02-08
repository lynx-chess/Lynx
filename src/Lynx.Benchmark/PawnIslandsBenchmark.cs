/*
 *  BenchmarkDotNet v0.15.4, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method                    | Mean     | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |-------------------------- |---------:|---------:|---------:|------:|-------:|----------:|------------:|
 *  | Original                  | 39.19 us | 0.446 us | 0.418 us |  1.00 | 3.4790 |   57.6 KB |        1.00 |
 *  | Original_SkipLocalsInit   | 39.53 us | 0.288 us | 0.270 us |  1.01 | 3.4790 |   57.6 KB |        1.00 |
 *  | Bitboard                  | 36.27 us | 0.154 us | 0.136 us |  0.93 | 3.4790 |   57.6 KB |        1.00 |
 *  | Bitboard_NoFileArray      | 37.03 us | 0.205 us | 0.182 us |  0.95 | 3.4790 |   57.6 KB |        1.00 |
 *  | Bitboard_NoPawnCountArray | 36.81 us | 0.172 us | 0.152 us |  0.94 | 3.4790 |   57.6 KB |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.15.4, Windows 11 (10.0.26100.6584/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method                    | Mean     | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |-------------------------- |---------:|---------:|---------:|------:|-------:|----------:|------------:|
 *  | Original                  | 40.07 us | 0.312 us | 0.276 us |  1.00 | 3.4790 |   57.6 KB |        1.00 |
 *  | Original_SkipLocalsInit   | 40.49 us | 0.574 us | 0.537 us |  1.01 | 3.4790 |   57.6 KB |        1.00 |
 *  | Bitboard                  | 39.41 us | 0.304 us | 0.285 us |  0.98 | 3.4790 |   57.6 KB |        1.00 |
 *  | Bitboard_NoFileArray      | 38.50 us | 0.232 us | 0.194 us |  0.96 | 3.4790 |   57.6 KB |        1.00 |
 *  | Bitboard_NoPawnCountArray | 38.82 us | 0.320 us | 0.299 us |  0.97 | 3.4790 |   57.6 KB |        1.00 |
 *
 *
 *   BenchmarkDotNet v0.15.4, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), Arm64 RyuJIT armv8.0-a
 *
 *  | Method                    | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |-------------------------- |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | Original                  | 25.01 us | 0.498 us | 1.356 us | 24.84 us |  1.00 |    0.08 | 9.3994 |   57.6 KB |        1.00 |
 *  | Original_SkipLocalsInit   | 23.79 us | 0.458 us | 0.580 us | 23.93 us |  0.95 |    0.05 | 9.3994 |   57.6 KB |        1.00 |
 *  | Bitboard                  | 22.83 us | 0.453 us | 1.120 us | 22.31 us |  0.92 |    0.07 | 9.3994 |   57.6 KB |        1.00 |
 *  | Bitboard_NoFileArray      | 22.90 us | 0.457 us | 1.050 us | 22.29 us |  0.92 |    0.06 | 9.3994 |   57.6 KB |        1.00 |
 *  | Bitboard_NoPawnCountArray | 22.98 us | 0.458 us | 1.014 us | 22.79 us |  0.92 |    0.06 | 9.3994 |   57.6 KB |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.15.4, macOS Ventura 13.7.6 (22H625) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method                    | Mean     | Error    | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |-------------------------- |---------:|---------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | Original                  | 66.75 us | 3.647 us | 10.637 us |  1.02 |    0.22 | 9.3994 |   57.6 KB |        1.00 |
 *  | Original_SkipLocalsInit   | 57.40 us | 1.487 us |  4.339 us |  0.88 |    0.15 | 9.3994 |   57.6 KB |        1.00 |
 *  | Bitboard                  | 58.56 us | 1.531 us |  4.343 us |  0.90 |    0.15 | 9.3994 |   57.6 KB |        1.00 |
 *  | Bitboard_NoFileArray      | 67.81 us | 3.107 us |  8.964 us |  1.04 |    0.21 | 9.3994 |   57.6 KB |        1.00 |
 *  | Bitboard_NoPawnCountArray | 66.54 us | 2.602 us |  7.254 us |  1.02 |    0.19 | 9.3994 |   57.6 KB |        1.00 |
*/

using BenchmarkDotNet.Attributes;
using Lynx.ConstantsGenerator;
using Lynx.Model;
using System.Numerics;
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
            var pieces = FENParser.ParseFEN(fen).PieceBitboards;
            Bitboard whitePawns = pieces[(int)Piece.P];
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
            var pieces = FENParser.ParseFEN(fen).PieceBitboards;
            Bitboard whitePawns = pieces[(int)Piece.P];
            sum += Original_SkipLocalsInit(whitePawns);
        }

        return sum;
    }

    [Benchmark]
    public int Bitboard()
    {
        var sum = 0;

        foreach (var fen in Data)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitboards;
            Bitboard whitePawns = pieces[(int)Piece.P];
            sum += Bitboard(whitePawns);
        }

        return sum;
    }

    [Benchmark]
    public int Bitboard_NoFileArray()
    {
        var sum = 0;

        foreach (var fen in Data)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitboards;
            Bitboard whitePawns = pieces[(int)Piece.P];
            sum += Bitboard_NoFileArray(whitePawns);
        }

        return sum;
    }

    [Benchmark]
    public int Bitboard_NoPawnCountArray()
    {
        var sum = 0;

        foreach (var fen in Data)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitboards;
            Bitboard whitePawns = pieces[(int)Piece.P];
            sum += Bitboard_NoPawnCountArray(whitePawns);
        }

        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Original(Bitboard pawns)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Original_SkipLocalsInit(Bitboard pawns)
    {
        const int n = 1;

        Span<int> files = stackalloc int[8];
        files.Clear();

        while (pawns != default)
        {
            pawns = pawns.WithoutLS1B(out var squareIndex);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Bitboard(Bitboard pawns)
    {
        int pawnFileBitboard = 0;

        while (pawns != default)
        {
            pawns = pawns.WithoutLS1B(out var squareIndex);

            // Bitboard.SetBit equivalent but for byte instead of ulong
            pawnFileBitboard |= (1 << Constants.File[squareIndex]);
        }

        return PawnIslandsGenerator.PawnIslandsCount[pawnFileBitboard];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Bitboard_NoFileArray(Bitboard pawns)
    {
        int pawnFileBitboard = 0;

        while (pawns != 0)
        {
            pawns = pawns.WithoutLS1B(out var squareIndex);

            // Bitboard.SetBit equivalent but for byte instead of ulong
            pawnFileBitboard |= (1 << (squareIndex % 8));
        }

        return PawnIslandsGenerator.PawnIslandsCount[pawnFileBitboard];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Bitboard_NoPawnCountArray(Bitboard pawns)
    {
        byte pawnFileBitboard = 0;

        while (pawns != 0)
        {
            pawns = pawns.WithoutLS1B(out var squareIndex);

            // Bitboard.SetBit equivalent but for byte instead of ulong
            pawnFileBitboard |= (byte)(1 << (squareIndex % 8));
        }

        int shifted = pawnFileBitboard << 1;

        // Treat shifted’s MSB as 0 implicitly
        int starts = pawnFileBitboard & (~shifted);

        return BitOperations.PopCount((uint)starts);
    }
}
