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
 *  | IsSquareAttacked_PassBitBoards     | 1.411 us | 0.0073 us | 0.0065 us |  1.00 |         - |          NA |
 *  | IsSquareAttacked_PassPieceBitBoard | 1.447 us | 0.0070 us | 0.0062 us |  1.03 |         - |          NA |
 *  | IsSquareInCheck_PassBitBoards      | 1.572 us | 0.0077 us | 0.0068 us |  1.11 |         - |          NA |
 *  | IsSquareInCheck_PassPieceBitBoard  | 1.669 us | 0.0069 us | 0.0057 us |  1.18 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2227) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                             | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------------------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | IsSquareAttacked_PassBitBoards     | 1.522 us | 0.0294 us | 0.0245 us |  1.00 |    0.00 |         - |          NA |
 *  | IsSquareAttacked_PassPieceBitBoard | 1.536 us | 0.0037 us | 0.0035 us |  1.01 |    0.02 |         - |          NA |
 *  | IsSquareInCheck_PassBitBoards      | 1.668 us | 0.0019 us | 0.0017 us |  1.10 |    0.02 |         - |          NA |
 *  | IsSquareInCheck_PassPieceBitBoard  | 1.769 us | 0.0065 us | 0.0061 us |  1.16 |    0.02 |         - |          NA |
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
 *  | IsSquareAttacked_PassBitBoards     | 2.506 us | 0.0802 us | 0.2262 us | 2.512 us |  1.00 |    0.00 |         - |          NA |
 *  | IsSquareAttacked_PassPieceBitBoard | 2.858 us | 0.1562 us | 0.4507 us | 2.721 us |  1.14 |    0.19 |         - |          NA |
 *  | IsSquareInCheck_PassBitBoards      | 3.329 us | 0.1635 us | 0.4666 us | 3.217 us |  1.34 |    0.23 |         - |          NA |
 *  | IsSquareInCheck_PassPieceBitBoard  | 3.714 us | 0.1368 us | 0.4011 us | 3.684 us |  1.48 |    0.20 |         - |          NA |
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
                var sideToMoveInt = (int)position.Side;
                var offset = Utils.PieceOffset(sideToMoveInt);

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
                var sideToMoveInt = (int)position.Side;
                var offset = Utils.PieceOffset(sideToMoveInt);

                if (Attacks_PassBitBoard.IsSquareAttacked(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
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
                var sideToMoveInt = (int)position.Side;
                var offset = Utils.PieceOffset(sideToMoveInt);

                if (Attacks_PassBitBoard.IsSquareInCheck(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
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