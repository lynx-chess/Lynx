/*
 *
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *  
 *  | Method           | position            | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |----------------- |-------------------- |---------:|---------:|---------:|------:|----------:|------------:|
 *  | Jagged           | Lynx.Model.Position | 29.56 ns | 0.057 ns | 0.048 ns |  1.00 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 29.48 ns | 0.054 ns | 0.045 ns |  1.00 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 30.68 ns | 0.074 ns | 0.062 ns |  1.04 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 30.69 ns | 0.043 ns | 0.038 ns |  1.04 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 30.66 ns | 0.026 ns | 0.023 ns |  1.04 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 28.33 ns | 0.014 ns | 0.012 ns |  0.96 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 26.57 ns | 0.038 ns | 0.032 ns |  0.90 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 26.46 ns | 0.029 ns | 0.026 ns |  0.90 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 26.45 ns | 0.016 ns | 0.015 ns |  0.89 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 26.46 ns | 0.041 ns | 0.036 ns |  0.90 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 26.46 ns | 0.021 ns | 0.017 ns |  0.90 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 25.65 ns | 0.024 ns | 0.020 ns |  0.87 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 25.73 ns | 0.068 ns | 0.060 ns |  0.87 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 25.78 ns | 0.022 ns | 0.020 ns |  0.87 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 25.77 ns | 0.020 ns | 0.017 ns |  0.87 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 25.78 ns | 0.019 ns | 0.017 ns |  0.87 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 27.11 ns | 0.300 ns | 0.266 ns |  0.92 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 27.92 ns | 0.267 ns | 0.208 ns |  0.94 |         - |          NA |
 *  
 *  
 *   BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32370/24H2/2024Update/HudsonValley) (Hyper-V)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.79GHz), 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *  
 *  | Method           | position            | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------------- |-------------------- |---------:|---------:|---------:|---------:|------:|--------:|----------:|------------:|
 *  | Jagged           | Lynx.Model.Position | 31.94 ns | 0.253 ns | 0.236 ns | 31.94 ns |  1.00 |    0.01 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 30.89 ns | 0.110 ns | 0.092 ns | 30.90 ns |  0.97 |    0.01 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 31.17 ns | 0.096 ns | 0.085 ns | 31.19 ns |  0.98 |    0.01 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 38.21 ns | 2.532 ns | 7.466 ns | 35.89 ns |  1.20 |    0.23 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 30.78 ns | 0.080 ns | 0.066 ns | 30.79 ns |  0.96 |    0.01 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 30.67 ns | 0.157 ns | 0.139 ns | 30.62 ns |  0.96 |    0.01 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 27.95 ns | 0.580 ns | 0.645 ns | 27.91 ns |  0.88 |    0.02 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 27.22 ns | 0.263 ns | 0.246 ns | 27.15 ns |  0.85 |    0.01 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 27.00 ns | 0.416 ns | 0.348 ns | 26.96 ns |  0.85 |    0.01 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 27.18 ns | 0.245 ns | 0.204 ns | 27.12 ns |  0.85 |    0.01 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 28.82 ns | 0.581 ns | 1.147 ns | 28.49 ns |  0.90 |    0.04 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 26.30 ns | 0.257 ns | 0.228 ns | 26.21 ns |  0.82 |    0.01 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 26.38 ns | 0.104 ns | 0.092 ns | 26.35 ns |  0.83 |    0.01 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 26.79 ns | 0.488 ns | 0.715 ns | 26.61 ns |  0.84 |    0.02 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 26.99 ns | 0.369 ns | 0.345 ns | 26.93 ns |  0.85 |    0.01 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 34.15 ns | 0.401 ns | 0.376 ns | 34.14 ns |  1.07 |    0.01 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 34.08 ns | 0.491 ns | 0.459 ns | 33.89 ns |  1.07 |    0.02 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 32.14 ns | 0.066 ns | 0.058 ns | 32.14 ns |  1.01 |    0.01 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *  
 *  | Method           | position            | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |----------------- |-------------------- |---------:|---------:|---------:|------:|----------:|------------:|
 *  | Jagged           | Lynx.Model.Position | 50.59 ns | 0.408 ns | 0.361 ns |  1.00 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 43.06 ns | 0.557 ns | 0.465 ns |  0.85 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 42.80 ns | 0.392 ns | 0.347 ns |  0.85 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 42.43 ns | 0.148 ns | 0.124 ns |  0.84 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 42.54 ns | 0.283 ns | 0.251 ns |  0.84 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 41.50 ns | 0.247 ns | 0.207 ns |  0.82 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 24.06 ns | 0.145 ns | 0.135 ns |  0.48 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 33.10 ns | 0.286 ns | 0.267 ns |  0.65 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 32.71 ns | 0.192 ns | 0.170 ns |  0.65 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 24.54 ns | 0.293 ns | 0.274 ns |  0.49 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 32.46 ns | 0.221 ns | 0.196 ns |  0.64 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 23.79 ns | 0.198 ns | 0.175 ns |  0.47 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 27.91 ns | 0.244 ns | 0.229 ns |  0.55 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 31.62 ns | 0.127 ns | 0.113 ns |  0.63 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 23.76 ns | 0.128 ns | 0.113 ns |  0.47 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 31.21 ns | 0.154 ns | 0.144 ns |  0.62 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 31.56 ns | 0.205 ns | 0.192 ns |  0.62 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 31.20 ns | 0.178 ns | 0.167 ns |  0.62 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *  
 *  | Method           | position            | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------------- |-------------------- |---------:|---------:|---------:|---------:|------:|--------:|----------:|------------:|
 *  | Jagged           | Lynx.Model.Position | 30.20 ns | 0.623 ns | 1.258 ns | 30.08 ns |  1.00 |    0.06 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 29.48 ns | 0.618 ns | 1.177 ns | 29.17 ns |  0.98 |    0.06 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 29.58 ns | 0.635 ns | 1.339 ns | 29.18 ns |  0.98 |    0.06 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 33.00 ns | 0.695 ns | 1.389 ns | 32.26 ns |  1.09 |    0.06 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 32.95 ns | 0.693 ns | 1.416 ns | 32.02 ns |  1.09 |    0.06 |         - |          NA |
 *  | Jagged           | Lynx.Model.Position | 29.85 ns | 0.242 ns | 0.237 ns | 29.82 ns |  0.99 |    0.04 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 24.14 ns | 0.504 ns | 0.828 ns | 24.23 ns |  0.80 |    0.04 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 27.19 ns | 0.564 ns | 0.973 ns | 27.06 ns |  0.90 |    0.05 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 23.80 ns | 0.487 ns | 0.744 ns | 23.53 ns |  0.79 |    0.04 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 27.45 ns | 0.571 ns | 1.058 ns | 27.55 ns |  0.91 |    0.05 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 27.31 ns | 0.565 ns | 0.989 ns | 27.26 ns |  0.91 |    0.05 |         - |          NA |
 *  | Flat             | Lynx.Model.Position | 23.44 ns | 0.481 ns | 0.763 ns | 22.99 ns |  0.78 |    0.04 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 27.33 ns | 0.565 ns | 0.792 ns | 27.49 ns |  0.91 |    0.05 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 22.56 ns | 0.463 ns | 0.665 ns | 22.63 ns |  0.75 |    0.04 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 22.55 ns | 0.460 ns | 0.615 ns | 22.70 ns |  0.75 |    0.04 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 22.52 ns | 0.461 ns | 0.599 ns | 22.67 ns |  0.75 |    0.04 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 22.30 ns | 0.458 ns | 0.510 ns | 22.58 ns |  0.74 |    0.03 |         - |          NA |
 *  | FlatUnsafe       | Lynx.Model.Position | 26.46 ns | 0.549 ns | 0.917 ns | 26.45 ns |  0.88 |    0.05 |         - |          NA |
 *  
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Benchmark;

/// <summary>
/// Benchmarks <see cref="ZobristTable.PieceHash(int, int)"/> table layout alternatives:
/// the original jagged <c>ulong[][]</c> vs a flat <c>ulong[]</c> with bounds-checked access
/// vs a flat <c>ulong[]</c> with <see cref="Unsafe.Add"/> (what is currently implemented).
/// The <c>CurrentPieceHash</c> method calls <see cref="ZobristTable.PieceHash"/> directly to
/// confirm it matches the <c>FlatUnsafe</c> baseline.
/// </summary>
public class ZobristHash_PieceTable_Benchmark : BaseBenchmark
{
    public static IEnumerable<Position> Data =>
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    private readonly ulong[][] _jaggedTable;
    private readonly ulong[] _flatTable;

    public ZobristHash_PieceTable_Benchmark()
    {
        _jaggedTable = InitializeJagged(new LynxRandom(int.MaxValue));
        _flatTable = InitializeFlat(new LynxRandom(int.MaxValue));
    }

    /// <summary>
    /// Original layout: <c>_table[square][piece]</c> — two pointer dereferences per lookup.
    /// </summary>
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ulong Jagged(Position position)
    {
        ulong hash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitboards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var square);
                hash ^= _jaggedTable[square][pieceIndex];
            }
        }

        return hash;
    }

    /// <summary>
    /// Flat layout: <c>_table[square * 12 + piece]</c> — single bounds-checked array access.
    /// </summary>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong Flat(Position position)
    {
        ulong hash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitboards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var square);
                hash ^= _flatTable[(square * 12) + pieceIndex];
            }
        }

        return hash;
    }

    /// <summary>
    /// Flat layout with <see cref="Unsafe.Add"/> on a pinned array — no bounds check.
    /// This is what <see cref="ZobristTable.PieceHash"/> currently uses.
    /// </summary>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong FlatUnsafe(Position position)
    {
        ulong hash = 0;
        ref var tableRef = ref MemoryMarshal.GetArrayDataReference(_flatTable);

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitboards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var square);
                hash ^= Unsafe.Add(ref tableRef, square * 12 + pieceIndex);
            }
        }

        return hash;
    }

    private static ulong[][] InitializeJagged(LynxRandom random)
    {
        var table = new ulong[64][];

        for (int square = 0; square < 64; ++square)
        {
            table[square] = new ulong[12];

            for (int piece = 0; piece < 12; ++piece)
            {
                table[square][piece] = random.NextUInt64();
            }
        }

        return table;
    }

    private static ulong[] InitializeFlat(LynxRandom random)
    {
        var table = GC.AllocateArray<ulong>(64 * 12, pinned: true);

        for (int square = 0; square < 64; ++square)
        {
            for (int piece = 0; piece < 12; ++piece)
            {
                table[square * 12 + piece] = random.NextUInt64();
            }
        }

        return table;
    }
}
