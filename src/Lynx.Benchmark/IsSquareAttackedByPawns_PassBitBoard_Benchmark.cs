/*
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                                    | Mean     | Error   | StdDev  | Ratio | Allocated | Alloc Ratio |
 *  |------------------------------------------ |---------:|--------:|--------:|------:|----------:|------------:|
 *  | IsSquaredAttackedByPawns_PassBitBoards    | 538.3 ns | 5.07 ns | 4.74 ns |  1.00 |         - |          NA |
 *  | IsSquaredAttackedByPawns_PassPawnBitBoard | 501.8 ns | 0.79 ns | 0.62 ns |  0.93 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2227) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                                    | Mean     | Error   | StdDev  | Ratio | Allocated | Alloc Ratio |
 *  |------------------------------------------ |---------:|--------:|--------:|------:|----------:|------------:|
 *  | IsSquaredAttackedByPawns_PassBitBoards    | 503.7 ns | 1.63 ns | 1.45 ns |  1.00 |         - |          NA |
 *  | IsSquaredAttackedByPawns_PassPawnBitBoard | 501.9 ns | 0.56 ns | 0.53 ns |  1.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX
 *
 *  | Method                                    | Mean     | Error   | StdDev  | Ratio | Allocated | Alloc Ratio |
 *  |------------------------------------------ |---------:|--------:|--------:|------:|----------:|------------:|
 *  | IsSquaredAttackedByPawns_PassBitBoards    | 802.2 ns | 5.70 ns | 4.45 ns |  1.00 |         - |          NA |
 *  | IsSquaredAttackedByPawns_PassPawnBitBoard | 729.2 ns | 9.26 ns | 8.21 ns |  0.91 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class IsSquareAttackedByPawns_PassBitBoard_Benchmark : BaseBenchmark
{
    private readonly Position[] _positions =
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    [Benchmark(Baseline = true)]
    public bool IsSquaredAttackedByPawns_PassBitBoards()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                var sideToMoveInt = (int)position.Side;
                var offset = Utils.PieceOffset(sideToMoveInt);

                if (IsSquaredAttackedByPawns_PassBitBoards(squareIndex, sideToMoveInt, offset, position.PieceBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public bool IsSquaredAttackedByPawns_PassPawnBitBoard()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                var sideToMoveInt = (int)position.Side;
                var offset = Utils.PieceOffset(sideToMoveInt);

                if (IsSquaredAttackedByPawns_PassPawnBitBoard(squareIndex, sideToMoveInt, position.PieceBitBoards[offset]))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquaredAttackedByPawns_PassBitBoards(int squareIndex, int sideToMove, int offset, BitBoard[] pieces)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & pieces[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquaredAttackedByPawns_PassPawnBitBoard(int squareIndex, int sideToMove, BitBoard pawnBitBoard)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & pawnBitBoard) != default;
    }
}
