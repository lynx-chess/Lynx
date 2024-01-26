using Lynx.Model;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx;

public static class Attacks
{
    private static readonly BitBoard[] _bishopOccupancyMasks;
    private static readonly BitBoard[] _rookOccupancyMasks;

    /// <summary>
    /// [64 (Squares), 512 (Occupancies)]
    /// Use <see cref="BishopAttacks(int, BitBoard)"/>
    /// </summary>
    private static readonly BitBoard[][] _bishopAttacks;

    /// <summary>
    /// [64 (Squares), 4096 (Occupancies)]
    /// Use <see cref="RookAttacks(int, BitBoard)"/>
    /// </summary>
    private static readonly BitBoard[][] _rookAttacks;

    private static readonly ulong[] _pextAttacks;
    private static readonly ulong[] _pextBishopOffset;
    private static readonly ulong[] _pextRookOffset;

    /// <summary>
    /// [2 (B|W), 64 (Squares)]
    /// </summary>
    public static BitBoard[][] PawnAttacks { get; }
    public static BitBoard[] KnightAttacks { get; }
    public static BitBoard[] KingAttacks { get; }

    static Attacks()
    {
        KingAttacks = AttackGenerator.InitializeKingAttacks();
        PawnAttacks = AttackGenerator.InitializePawnAttacks();
        KnightAttacks = AttackGenerator.InitializeKnightAttacks();

        (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopMagicAttacks();
        (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookMagicAttacks();

        if (Bmi2.X64.IsSupported)
        {
            _pextAttacks = new ulong[5248 + 102400];
            _pextBishopOffset = new ulong[64];
            _pextRookOffset = new ulong[64];

            InitializeBishopAndRookPextAttacks();
        }
        else
        {
            _pextAttacks = [];
            _pextBishopOffset = [];
            _pextRookOffset = [];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard BishopAttacks(int squareIndex, BitBoard occupancy)
    {
        return Bmi2.X64.IsSupported
            ? _pextAttacks[_pextBishopOffset[squareIndex] + Bmi2.X64.ParallelBitExtract(occupancy, _bishopOccupancyMasks[squareIndex])]
            : MagicNumbersBishopAttacks(squareIndex, occupancy);
    }

    /// <summary>
    /// Get Bishop attacks assuming current board occupancy
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard MagicNumbersBishopAttacks(int squareIndex, BitBoard occupancy)
    {
        var occ = occupancy & _bishopOccupancyMasks[squareIndex];
        occ *= Constants.BishopMagicNumbers[squareIndex];
        occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

        return _bishopAttacks[squareIndex][occ];
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
        return Bmi2.IsSupported
            ? _pextAttacks[_pextRookOffset[squareIndex] + Bmi2.X64.ParallelBitExtract(occupancy, _rookOccupancyMasks[squareIndex])]
            : MagicNumbersRookAttacks(squareIndex, occupancy);
    }

    public static BitBoard MagicNumbersRookAttacks(int squareIndex, BitBoard occupancy)
    {
        var occ = occupancy & _rookOccupancyMasks[squareIndex];
        occ *= Constants.RookMagicNumbers[squareIndex];
        occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

        return _rookAttacks[squareIndex][occ];
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
    public static bool IsSquareAttackedBySide(int squaredIndex, Position position, Side sideToMove) =>
        IsSquareAttacked(squaredIndex, sideToMove, position.PieceBitBoards, position.OccupancyBitBoards);

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

        return (PawnAttacks[oppositeColorIndex][squareIndex] & pawnBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKnights(int squareIndex, BitBoard knightBitBoard)
    {
        return (KnightAttacks[squareIndex] & knightBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKing(int squareIndex, BitBoard kingBitBoard)
    {
        return (KingAttacks[squareIndex] & kingBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByBishops(int squareIndex, BitBoard bishopBitBoard, BitBoard occupancy, out BitBoard bishopAttacks)
    {
        bishopAttacks = BishopAttacks(squareIndex, occupancy);
        return (bishopAttacks & bishopBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByRooks(int squareIndex, BitBoard rookBitBoard, BitBoard occupancy, out BitBoard rookAttacks)
    {
        rookAttacks = RookAttacks(squareIndex, occupancy);
        return (rookAttacks & rookBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByQueens(BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard queenBitBoard)
    {
        var queenAttacks = QueenAttacks(rookAttacks, bishopAttacks);
        return (queenAttacks & queenBitBoard) != default;
    }

    /// <summary>
    /// Taken from Leorik (https://github.com/lithander/Leorik/blob/master/Leorik.Core/Slider/Pext.cs)
    /// Based on https://www.chessprogramming.org/BMI2#PEXT_Bitboards
    /// </summary>
    private static void InitializeBishopAndRookPextAttacks()
    {
        ulong index = 0;

        // Bishop-Attacks
        for (int square = 0; square < 64; square++)
        {
            _pextBishopOffset[square] = index;
            ulong bishopMask = _bishopOccupancyMasks[square];

            ulong patterns = 1UL << BitOperations.PopCount(bishopMask);

            for (ulong i = 0; i < patterns; i++)
            {
                ulong occupation = Bmi2.X64.ParallelBitDeposit(i, bishopMask);
                _pextAttacks[index++] = MagicNumbersBishopAttacks(square, occupation);
            }
        }

        // Rook-Attacks
        for (int square = 0; square < 64; square++)
        {
            _pextRookOffset[square] = index;
            ulong rookMask = _rookOccupancyMasks[square];
            ulong patterns = 1UL << BitOperations.PopCount(rookMask);

            for (ulong i = 0; i < patterns; i++)
            {
                ulong occupation = Bmi2.X64.ParallelBitDeposit(i, rookMask);
                _pextAttacks[index++] = MagicNumbersRookAttacks(square, occupation);
            }
        }
    }
}
