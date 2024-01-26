/*

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
    public bool IsSquaredAttacked_PassPawnBitBoard()
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
    public bool IsSquaredInCheck_PassPawnBitBoard()
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