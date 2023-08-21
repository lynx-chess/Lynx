﻿using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx;

public static class Attacks
{
    private static readonly BitBoard[] _bishopOccupancyMasks;
    private static readonly BitBoard[] _rookOccupancyMasks;

    /// <summary>
    /// [64 (Squares), 512 (Occupancies)]
    /// Use <see cref="BishopAttacks(int, BitBoard)"/>
    /// </summary>
    private static readonly BitBoard[,] _bishopAttacks;

    /// <summary>
    /// [64 (Squares), 4096 (Occupancies)]
    /// Use <see cref="RookAttacks(int, BitBoard)"/>
    /// </summary>
    private static readonly BitBoard[,] _rookAttacks;

    /// <summary>
    /// [2 (B|W), 64 (Squares)]
    /// </summary>
    public static BitBoard[,] PawnAttacks { get; }
    public static BitBoard[] KnightAttacks { get; }
    public static BitBoard[] KingAttacks { get; }

    static Attacks()
    {
        KingAttacks = AttackGenerator.InitializeKingAttacks();
        PawnAttacks = AttackGenerator.InitializePawnAttacks();
        KnightAttacks = AttackGenerator.InitializeKnightAttacks();

        (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopAttacks();
        (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookAttacks();
    }

    /// <summary>
    /// Get Bishop attacks assuming current board occupancy
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard BishopAttacks(int squareIndex, BitBoard occupancy)
    {
        var occ = occupancy & _bishopOccupancyMasks[squareIndex];
        occ *= Constants.BishopMagicNumbers[squareIndex];
        occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

        return _bishopAttacks[squareIndex, occ];
    }

    /// <summary>
    /// Get Rook attacks assuming current board occupancy
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard RookAttacks(int squareIndex, BitBoard occupancy)
    {
        var occ = occupancy & _rookOccupancyMasks[squareIndex];
        occ *= Constants.RookMagicNumbers[squareIndex];
        occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

        return _rookAttacks[squareIndex, occ];
    }

    /// <summary>
    /// Get Queen attacks assuming current board occupancy
    /// Use <see cref="QueenAttacks(BitBoard, BitBoard)"/> if rook and bishop attacks are already calculated
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard QueenAttacks(int squareIndex, BitBoard occupancy)
    {
        return QueenAttacks(
            RookAttacks(squareIndex, occupancy),
            BishopAttacks(squareIndex, occupancy));
    }

    /// <summary>
    /// Get Queen attacks having rook and bishop attacks pre-calculated
    /// </summary>
    /// <param name="rookAttacks"></param>
    /// <param name="bishopAttacks"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard QueenAttacks(BitBoard rookAttacks, BitBoard bishopAttacks)
    {
        return rookAttacks | bishopAttacks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquaredAttackedBySide(int squaredIndex, Position position, Side sideToMove) =>
        IsSquaredAttacked(squaredIndex, sideToMove, position.PieceBitBoards, position.OccupancyBitBoards);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquaredAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
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

        return (PawnAttacks[oppositeColorIndex, squareIndex] & pieces[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
    {
        return (KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
    {
        return (KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard bishopAttacks)
    {
        bishopAttacks = BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
        return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard rookAttacks)
    {
        rookAttacks = RookAttacks(squareIndex, occupancy[(int)Side.Both]);
        return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard[] piecePosition)
    {
        var queenAttacks = QueenAttacks(rookAttacks, bishopAttacks);
        return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
    }
}
