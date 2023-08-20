using Lynx.Model;
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
    public static bool AreAnySquaresAttackedBySide(int squaredIndex1, int squaredIndex2, Position position, Side sideToMove) =>
        AreAnySquaresAttacked(squaredIndex1, squaredIndex2, sideToMove, position.PieceBitBoards, position.OccupancyBitBoards);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquaredAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var offset = Utils.PieceOffset(sideToMove);

        // I tried to order them from most to least likely
        return
            IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition)
            || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
            || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
            || IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
            || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
            || IsSquareAttackedByKing(squareIndex, offset, piecePosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreAnySquaresAttacked(int squareIndex1, int squareIndex2, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var offset = Utils.PieceOffset(sideToMove);

        // I tried to order them from most to least likely
        return
            AreAnySquaresAttackedByPawns(squareIndex1, squareIndex2, sideToMove, offset, piecePosition)
            || AreAnySquaresAttackedByKnights(squareIndex1, squareIndex2, offset, piecePosition)
            || IsSquareAttackedByBishops(squareIndex1, offset, piecePosition, occupancy, out var bishopAttacks1)
            || IsSquareAttackedByBishops(squareIndex2, offset, piecePosition, occupancy, out var bishopAttacks2)
            || IsSquareAttackedByRooks(squareIndex1, offset, piecePosition, occupancy, out var rookAttacks1)
            || IsSquareAttackedByRooks(squareIndex2, offset, piecePosition, occupancy, out var rookAttacks2)
            || AreAnySquaresAttackedByQueens(offset, bishopAttacks1, rookAttacks1, bishopAttacks2, rookAttacks2, piecePosition)
            || AreAnySquaresAttackedByKing(squareIndex1, squareIndex2, offset, piecePosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareInCheck(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var offset = Utils.PieceOffset(sideToMove);

        // I tried to order them from most to least likely
        return
            IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
            || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
            || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
            || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
            || IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByPawns(int squareIndex, Side sideToMove, int offset, BitBoard[] pieces)
    {
        var oppositeColorIndex = ((int)sideToMove + 1) % 2;

        return (PawnAttacks[oppositeColorIndex, squareIndex] & pieces[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AreAnySquaresAttackedByPawns(int squareIndex1, int squareIndex2, Side sideToMove, int offset, BitBoard[] pieces)
    {
        var oppositeColorIndex = ((int)sideToMove + 1) % 2;

        return ((PawnAttacks[oppositeColorIndex, squareIndex1] & pieces[offset]) != default)
            || ((PawnAttacks[oppositeColorIndex, squareIndex2] & pieces[offset]) != default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
    {
        return (KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AreAnySquaresAttackedByKnights(int squareIndex1, int squareIndex2, int offset, BitBoard[] piecePosition)
    {
        var knight = (int)Piece.N + offset;

        return ((KnightAttacks[squareIndex1] & piecePosition[knight]) != default)
            || ((KnightAttacks[squareIndex2] & piecePosition[knight]) != default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
    {
        return (KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AreAnySquaresAttackedByKing(int squareIndex1, int squareIndex2, int offset, BitBoard[] piecePosition)
    {
        var king = (int)Piece.K + offset;

        return ((KingAttacks[squareIndex1] & piecePosition[king]) != default)
            || ((KingAttacks[squareIndex2] & piecePosition[king]) != default);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AreAnySquaresAttackedByQueens(int offset, BitBoard bishopAttacks1, BitBoard rookAttacks1, BitBoard bishopAttacks2, BitBoard rookAttacks2, BitBoard[] piecePosition)
    {
        var queen = (int)Piece.Q + offset;

        return ((QueenAttacks(rookAttacks1, bishopAttacks1) & piecePosition[queen]) != default)
            || ((QueenAttacks(rookAttacks2, bishopAttacks2) & piecePosition[queen]) != default);
    }
}
