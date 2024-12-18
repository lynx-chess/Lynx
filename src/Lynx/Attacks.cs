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
            _pextAttacks = GC.AllocateArray<BitBoard>(5248 + 102400, pinned: true);
            _pextBishopOffset = GC.AllocateArray<BitBoard>(64, pinned: true);
            _pextRookOffset = GC.AllocateArray<BitBoard>(64, pinned: true);

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
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
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
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard RookAttacks(int squareIndex, BitBoard occupancy)
    {
        return Bmi2.IsSupported
            ? _pextAttacks[_pextRookOffset[squareIndex] + Bmi2.X64.ParallelBitExtract(occupancy, _rookOccupancyMasks[squareIndex])]
            : MagicNumbersRookAttacks(squareIndex, occupancy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard QueenAttacks(BitBoard rookAttacks, BitBoard bishopAttacks) => rookAttacks | bishopAttacks;

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
