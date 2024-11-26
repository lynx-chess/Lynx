/*
*
*   BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
*   AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
*   .NET SDK 8.0.403
*     [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
*     DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
*
*   | Method               | position            | Mean     | Error   | StdDev  | Ratio | Allocated | Alloc Ratio |
*   |--------------------- |-------------------- |---------:|--------:|--------:|------:|----------:|------------:|
*   | Original             | Lynx.Model.Position | 488.1 ns | 2.21 ns | 1.85 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 487.8 ns | 0.63 ns | 0.52 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 487.4 ns | 0.74 ns | 0.69 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 487.2 ns | 1.50 ns | 1.41 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 488.2 ns | 1.29 ns | 1.20 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 487.3 ns | 1.37 ns | 1.22 ns |  1.00 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 500.1 ns | 0.48 ns | 0.37 ns |  1.02 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 511.9 ns | 3.72 ns | 3.48 ns |  1.05 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 510.0 ns | 4.19 ns | 3.50 ns |  1.04 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 509.2 ns | 5.03 ns | 4.71 ns |  1.04 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 508.8 ns | 3.91 ns | 3.66 ns |  1.04 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 510.7 ns | 5.53 ns | 5.17 ns |  1.05 |         - |          NA |
*
*
*   BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2700) (Hyper-V)
*   AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
*   .NET SDK 8.0.403
*     [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
*     DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
*
*   | Method               | position            | Mean     | Error   | StdDev  | Ratio | Allocated | Alloc Ratio |
*   |--------------------- |-------------------- |---------:|--------:|--------:|------:|----------:|------------:|
*   | Original             | Lynx.Model.Position | 479.4 ns | 1.04 ns | 0.92 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 484.1 ns | 3.37 ns | 2.99 ns |  1.01 |         - |          NA |
*   | Original             | Lynx.Model.Position | 478.5 ns | 0.21 ns | 0.19 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 479.0 ns | 0.51 ns | 0.43 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 478.6 ns | 0.32 ns | 0.28 ns |  1.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 479.1 ns | 0.46 ns | 0.38 ns |  1.00 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 512.0 ns | 2.83 ns | 2.65 ns |  1.07 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 494.5 ns | 1.67 ns | 1.48 ns |  1.03 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 512.9 ns | 1.29 ns | 1.14 ns |  1.07 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 511.1 ns | 3.70 ns | 3.28 ns |  1.07 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 511.3 ns | 2.44 ns | 2.29 ns |  1.07 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 511.7 ns | 2.78 ns | 2.60 ns |  1.07 |         - |          NA |
*
*   BenchmarkDotNet v0.14.0, macOS Sonoma 14.7 (23H124) [Darwin 23.6.0]
*   Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
*   .NET SDK 8.0.403
*     [Host]     : .NET 8.0.10 (8.0.1024.46610), Arm64 RyuJIT AdvSIMD
*     DefaultJob : .NET 8.0.10 (8.0.1024.46610), Arm64 RyuJIT AdvSIMD
*
*   | Method               | position            | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
*   |--------------------- |-------------------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
*   | Original             | Lynx.Model.Position | 513.8 ns | 10.07 ns |  9.89 ns |  1.00 |    0.03 |         - |          NA |
*   | Original             | Lynx.Model.Position | 516.8 ns |  9.99 ns |  9.81 ns |  1.01 |    0.03 |         - |          NA |
*   | Original             | Lynx.Model.Position | 503.2 ns |  5.54 ns |  4.63 ns |  0.98 |    0.02 |         - |          NA |
*   | Original             | Lynx.Model.Position | 527.7 ns | 10.10 ns | 18.21 ns |  1.03 |    0.04 |         - |          NA |
*   | Original             | Lynx.Model.Position | 503.8 ns |  9.88 ns | 10.98 ns |  0.98 |    0.03 |         - |          NA |
*   | Original             | Lynx.Model.Position | 502.6 ns |  8.76 ns |  7.76 ns |  0.98 |    0.02 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 507.6 ns |  8.67 ns |  7.24 ns |  0.99 |    0.02 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 504.6 ns |  8.71 ns |  8.15 ns |  0.98 |    0.02 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 502.6 ns |  9.72 ns | 10.80 ns |  0.98 |    0.03 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 496.5 ns |  5.41 ns |  4.79 ns |  0.97 |    0.02 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 504.1 ns |  9.53 ns | 10.20 ns |  0.98 |    0.03 |         - |          NA |
*   | Branchless           | Lynx.Model.Position | 501.3 ns |  8.60 ns |  8.05 ns |  0.98 |    0.02 |         - |          NA |
*
*    BenchmarkDotNet v0.14.0, macOS Ventura 13.7 (22H123) [Darwin 22.6.0]
*   Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
*   .NET SDK 8.0.403
*     [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
*     DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
*
*   | Method               | position            | Mean     | Error   | StdDev  | Ratio | RatioSD | Allocated | Alloc Ratio |
*   |--------------------- |-------------------- |---------:|--------:|--------:|------:|--------:|----------:|------------:|
*   | Original             | Lynx.Model.Position | 367.8 ns | 1.31 ns | 1.23 ns |  1.00 |    0.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 367.6 ns | 2.10 ns | 1.97 ns |  1.00 |    0.01 |         - |          NA |
*   | Original             | Lynx.Model.Position | 368.2 ns | 1.57 ns | 1.39 ns |  1.00 |    0.00 |         - |          NA |
*   | Original             | Lynx.Model.Position | 368.1 ns | 2.22 ns | 2.08 ns |  1.00 |    0.01 |         - |          NA |
*   | Original             | Lynx.Model.Position | 366.2 ns | 1.97 ns | 1.74 ns |  1.00 |    0.01 |         - |          NA |
*   | Original             | Lynx.Model.Position | 366.2 ns | 2.10 ns | 1.75 ns |  1.00 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt | Lynx.Model.Position | 441.4 ns | 2.44 ns | 2.17 ns |  1.20 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt | Lynx.Model.Position | 444.0 ns | 6.94 ns | 6.82 ns |  1.21 |    0.02 |         - |          NA |
*   | GetLS1BIndexAndPopIt | Lynx.Model.Position | 443.7 ns | 3.32 ns | 2.95 ns |  1.21 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt | Lynx.Model.Position | 442.8 ns | 1.53 ns | 1.36 ns |  1.20 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt | Lynx.Model.Position | 440.7 ns | 2.60 ns | 2.31 ns |  1.20 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt | Lynx.Model.Position | 443.6 ns | 4.64 ns | 3.87 ns |  1.21 |    0.01 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class GetBit_Benchmark : BaseBenchmark
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

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public bool Original(Position position)
    {
        bool result = true;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            for (int sq = 0; sq < 64; ++sq)
            {
                var square = bitboard.GetBit_Original(sq);
                result ^= square;
            }
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool Branchless(Position position)
    {
        bool result = true;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            for (int sq = 0; sq < 64; ++sq)
            {
                var square = bitboard.GetBit_Branchless(sq);
                result ^= square;
            }
        }

        return result;
    }
}

internal static class BitBoardExtensions_GetBit_Benchmark
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit_Original(this BitBoard board, int squareIndex)
    {
        return (board & (1UL << squareIndex)) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit_Branchless(this BitBoard board, int squareIndex)
    {
        var value = (byte)(board & (1UL << squareIndex));

        return Unsafe.As<byte, bool>(ref value);
    }
}