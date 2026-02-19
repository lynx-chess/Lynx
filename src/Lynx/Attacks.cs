using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx;

public static class Attacks
{
    private static readonly Bitboard[] _bishopOccupancyMasks;
    private static readonly Bitboard[] _rookOccupancyMasks;

    /// <summary>
    /// [64 (Squares), 512 (Occupancies)]
    /// Use <see cref="BishopAttacks(int, Bitboard)"/>
    /// </summary>
    private static readonly Bitboard[][] _bishopAttacks;

    /// <summary>
    /// [64 (Squares), 4096 (Occupancies)]
    /// Use <see cref="RookAttacks(int, Bitboard)"/>
    /// </summary>
    private static readonly Bitboard[][] _rookAttacks;

    private static readonly ulong[] _pextAttacks;
    private static readonly ulong[] _pextBishopOffset;
    private static readonly ulong[] _pextRookOffset;

    /// <summary>
    /// [2 (B|W), 64 (Squares)]
    /// </summary>
    public static Bitboard[][] PawnAttacks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public static Bitboard[] KnightAttacks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public static Bitboard[] KingAttacks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

#pragma warning disable CA1810 // Initialize reference type static fields inline
    static Attacks()
#pragma warning restore CA1810 // Initialize reference type static fields inline
    {
        KingAttacks = AttackGenerator.InitializeKingAttacks();
        PawnAttacks = AttackGenerator.InitializePawnAttacks();
        KnightAttacks = AttackGenerator.InitializeKnightAttacks();

        (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopMagicAttacks();
        (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookMagicAttacks();

        if (Bmi2.X64.IsSupported)
        {
            _pextAttacks = GC.AllocateArray<Bitboard>(5248 + 102400, pinned: true);
            _pextBishopOffset = GC.AllocateArray<Bitboard>(64, pinned: true);
            _pextRookOffset = GC.AllocateArray<Bitboard>(64, pinned: true);

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
    public static Bitboard BishopAttacks(int squareIndex, Bitboard occupancy)
    {
        return Bmi2.X64.IsSupported
            ? _pextAttacks[_pextBishopOffset[squareIndex] + Bmi2.X64.ParallelBitExtract(occupancy, _bishopOccupancyMasks[squareIndex])]
            : MagicNumbersBishopAttacks(squareIndex, occupancy);
    }

    /// <summary>
    /// Get Bishop attacks assuming current board occupancy
    /// </summary>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard MagicNumbersBishopAttacks(int squareIndex, Bitboard occupancy)
    {
        var occ = occupancy & _bishopOccupancyMasks[squareIndex];
        occ *= Constants.BishopMagicNumbers[squareIndex];
        occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

        return _bishopAttacks[squareIndex][occ];
    }

    /// <summary>
    /// Get Rook attacks assuming current board occupancy
    /// </summary>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard RookAttacks(int squareIndex, Bitboard occupancy)
    {
        return Bmi2.IsSupported
            ? _pextAttacks[_pextRookOffset[squareIndex] + Bmi2.X64.ParallelBitExtract(occupancy, _rookOccupancyMasks[squareIndex])]
            : MagicNumbersRookAttacks(squareIndex, occupancy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard MagicNumbersRookAttacks(int squareIndex, Bitboard occupancy)
    {
        var occ = occupancy & _rookOccupancyMasks[squareIndex];
        occ *= Constants.RookMagicNumbers[squareIndex];
        occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

        return _rookAttacks[squareIndex][occ];
    }

    /// <summary>
    /// Get Queen attacks assuming current board occupancy
    /// Use <see cref="QueenAttacks(Bitboard, Bitboard)"/> if rook and bishop attacks are already calculated
    /// </summary>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard QueenAttacks(int squareIndex, Bitboard occupancy)
    {
        return QueenAttacks(
            RookAttacks(squareIndex, occupancy),
            BishopAttacks(squareIndex, occupancy));
    }

    /// <summary>
    /// Get Queen attacks having rook and bishop attacks pre-calculated
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard QueenAttacks(Bitboard rookAttacks, Bitboard bishopAttacks) => rookAttacks | bishopAttacks;

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
