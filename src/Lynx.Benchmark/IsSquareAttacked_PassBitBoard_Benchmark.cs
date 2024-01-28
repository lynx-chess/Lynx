/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                             | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |----------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | IsSquaredAttacked_PassBitBoards    | 1.420 us | 0.0077 us | 0.0072 us |  1.00 |         - |          NA |
 *  | IsSquareAttacked_PassPieceBitBoard | 1.453 us | 0.0089 us | 0.0083 us |  1.02 |         - |          NA |
 *  | IsSquareAttacked_Position          | 1.530 us | 0.0120 us | 0.0112 us |  1.08 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2227) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                             | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |----------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | IsSquaredAttacked_PassBitBoards    | 1.457 us | 0.0053 us | 0.0044 us |  1.00 |         - |          NA |
 *  | IsSquareAttacked_PassPieceBitBoard | 1.447 us | 0.0097 us | 0.0086 us |  0.99 |         - |          NA |
 *  | IsSquareAttacked_Position          | 1.464 us | 0.0038 us | 0.0036 us |  1.01 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                             | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------------------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
 *  | IsSquaredAttacked_PassBitBoards    | 2.120 us | 0.0408 us | 0.1110 us | 2.087 us |  1.00 |    0.00 |         - |          NA |
 *  | IsSquareAttacked_PassPieceBitBoard | 2.126 us | 0.0346 us | 0.0425 us | 2.120 us |  0.99 |    0.05 |         - |          NA |
 *  | IsSquareAttacked_Position          | 1.989 us | 0.0394 us | 0.0498 us | 1.984 us |  0.93 |    0.06 |         - |          NA |
 *
 *  --------------------------------------------------------------------------------------------------------------------------------
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                            | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |---------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | IsSquareInCheck_PassBitBoards     | 1.573 us | 0.0059 us | 0.0053 us |  1.00 |         - |          NA |
 *  | IsSquareInCheck_PassPieceBitBoard | 1.670 us | 0.0070 us | 0.0065 us |  1.06 |         - |          NA |
 *  | IsSquareInCheck_Position          | 1.553 us | 0.0088 us | 0.0082 us |  0.99 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2227) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                            | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |---------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | IsSquareInCheck_PassBitBoards     | 1.555 us | 0.0089 us | 0.0079 us |  1.00 |         - |          NA |
 *  | IsSquareInCheck_PassPieceBitBoard | 1.644 us | 0.0028 us | 0.0025 us |  1.06 |         - |          NA |
 *  | IsSquareInCheck_Position          | 1.550 us | 0.0017 us | 0.0016 us |  1.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                            | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------------------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
 *  | IsSquareInCheck_PassBitBoards     | 2.459 us | 0.1088 us | 0.3067 us | 2.321 us |  1.00 |    0.00 |         - |          NA |
 *  | IsSquareInCheck_PassPieceBitBoard | 2.965 us | 0.1559 us | 0.4596 us | 2.891 us |  1.23 |    0.24 |         - |          NA |
 *  | IsSquareInCheck_Position          | 2.378 us | 0.0473 us | 0.1220 us | 2.349 us |  0.98 |    0.13 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class IsSquareAttacked_PassBitBoard_Benchmark : BaseBenchmark
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
    public bool IsSquaredAttacked_PassBitBoards()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (Attacks_PassingArray.IsSquareAttacked(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public bool IsSquareAttacked_PassPieceBitBoard()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (Attacks_PassBitBoard.IsSquareAttacked(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public bool IsSquareAttacked_Position()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (position.IsSquareAttacked(squareIndex, position.Side))
                {
                    b = true;
                }
            }
        }

        return b;
    }
}

public class IsSquareInCheck_PassBitBoard_Benchmark : BaseBenchmark
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
    public bool IsSquareInCheck_PassBitBoards()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                var sideToMoveInt = (int)position.Side;
                var offset = Utils.PieceOffset(sideToMoveInt);

                if (Attacks_PassingArray.IsSquareInCheck(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public bool IsSquareInCheck_PassPieceBitBoard()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (Attacks_PassBitBoard.IsSquareInCheck(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public bool IsSquareInCheck_Position()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (position.IsInCheck())
                {
                    b = true;
                }
            }
        }

        return b;
    }
}

file static class Attacks_PassingArray
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);

        // I tried to order them from most to least likely
        return
            IsSquareAttackedByPawns(squareIndex, sideToMoveInt, offset, piecePosition)
            || IsSquareAttackedByKing(squareIndex, offset, piecePosition)
            || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
            || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
            || IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
            || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareInCheck(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);

        // I tried to order them from most to least likely
        return
            IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
            || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
            || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
            || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
            || IsSquareAttackedByPawns(squareIndex, sideToMoveInt, offset, piecePosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByPawns(int squareIndex, int sideToMove, int offset, BitBoard[] pieces)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & pieces[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
    {
        return (Attacks.KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
    {
        return (Attacks.KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard bishopAttacks)
    {
        bishopAttacks = Attacks.BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
        return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard rookAttacks)
    {
        rookAttacks = Attacks.RookAttacks(squareIndex, occupancy[(int)Side.Both]);
        return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard[] piecePosition)
    {
        var queenAttacks = Attacks.QueenAttacks(rookAttacks, bishopAttacks);
        return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
    }
}

file static class Attacks_PassBitBoard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);
        var bothSidesOccupancy = occupancy[(int)Side.Both];

        // I tried to order them from most to least likely - not tested
        return
            IsSquareAttackedByPawns(squareIndex, sideToMoveInt, piecePosition[offset])
            || IsSquareAttackedByKing(squareIndex, piecePosition[(int)Piece.K + offset])
            || IsSquareAttackedByKnights(squareIndex, piecePosition[(int)Piece.N + offset])
            || IsSquareAttackedByBishops(squareIndex, piecePosition[(int)Piece.B + offset], bothSidesOccupancy, out var bishopAttacks)
            || IsSquareAttackedByRooks(squareIndex, piecePosition[(int)Piece.R + offset], bothSidesOccupancy, out var rookAttacks)
            || IsSquareAttackedByQueens(bishopAttacks, rookAttacks, piecePosition[(int)Piece.Q + offset]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareInCheck(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);
        var bothSidesOccupancy = occupancy[(int)Side.Both];

        // I tried to order them from most to least likely- not tested
        return
            IsSquareAttackedByRooks(squareIndex, piecePosition[(int)Piece.R + offset], bothSidesOccupancy, out var rookAttacks)
            || IsSquareAttackedByBishops(squareIndex, piecePosition[(int)Piece.B + offset], bothSidesOccupancy, out var bishopAttacks)
            || IsSquareAttackedByQueens(bishopAttacks, rookAttacks, piecePosition[(int)Piece.Q + offset])
            || IsSquareAttackedByKnights(squareIndex, piecePosition[(int)Piece.N + offset])
            || IsSquareAttackedByPawns(squareIndex, sideToMoveInt, piecePosition[offset]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByPawns(int squareIndex, int sideToMove, BitBoard pawnBitBoard)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & pawnBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKnights(int squareIndex, BitBoard knightBitBoard)
    {
        return (Attacks.KnightAttacks[squareIndex] & knightBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKing(int squareIndex, BitBoard kingBitBoard)
    {
        return (Attacks.KingAttacks[squareIndex] & kingBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByBishops(int squareIndex, BitBoard bishopBitBoard, BitBoard occupancy, out BitBoard bishopAttacks)
    {
        bishopAttacks = Attacks.BishopAttacks(squareIndex, occupancy);
        return (bishopAttacks & bishopBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByRooks(int squareIndex, BitBoard rookBitBoard, BitBoard occupancy, out BitBoard rookAttacks)
    {
        rookAttacks = Attacks.RookAttacks(squareIndex, occupancy);
        return (rookAttacks & rookBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByQueens(BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard queenBitBoard)
    {
        var queenAttacks = Attacks.QueenAttacks(rookAttacks, bishopAttacks);
        return (queenAttacks & queenBitBoard) != default;
    }
}