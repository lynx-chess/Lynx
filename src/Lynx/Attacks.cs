using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

using static Lynx.PinnedArrays;

namespace Lynx;

public static unsafe class Attacks
{
    /// <summary>
    /// Stores both the mask and the attack pointer for every square
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    private struct PEXTEntry
    {
        public Bitboard Mask;
        public Bitboard* Attacks;
    }

    /// <summary>
    /// Stores mask, magic, shift and attack pointer for every square.
    /// See <see cref="AttackGenerator.InitializeBishopAndRookMagicAttacks"/>
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    internal struct MagicEntry
    {
        public Bitboard Mask;
        public Bitboard Magic;
        public Bitboard* Attacks;
        public int Shift;
    }

    private static readonly PEXTEntry* _bishopPEXTEntries;
    private static readonly PEXTEntry* _rookPEXTEntries;

    private static readonly MagicEntry* _bishopMagicEntries;
    private static readonly MagicEntry* _rookMagicEntries;

    public static readonly Bitboard* KnightAttacks;
    public static readonly Bitboard* KingAttacks;

    /// <summary>
    /// [2 x 64], index : (side x 64) + square
    /// </summary>
    private static readonly Bitboard* _pawnAttacks;

    /// <summary>
    /// [64 x 64]: index: (square1 x 64) + square2
    /// </summary>
    private static readonly Bitboard* _raysBetween;

#pragma warning disable CA1810 // Initialize reference type static fields inline
    static Attacks()
#pragma warning restore CA1810 // Initialize reference type static fields inline
    {
        KingAttacks = CopyToPinnedArray(AttackGenerator.InitializeKingAttacks());
        KnightAttacks = CopyToPinnedArray(AttackGenerator.InitializeKnightAttacks());
        _pawnAttacks = FlattenToPinnedArray(AttackGenerator.InitializePawnAttacks(), stride: 64);

        var (magicAttacks, magicEntries) = AttackGenerator.InitializeBishopAndRookMagicAttacks();
        KeepAlive(magicAttacks);

        _bishopMagicEntries = KeepAlive(magicEntries);
        _rookMagicEntries = _bishopMagicEntries + 64;

        if (Bmi2.X64.IsSupported)
        {
            _bishopPEXTEntries = InitializeBishopAndRookPEXTAttacks();
            _rookPEXTEntries = _bishopPEXTEntries + 64;
        }
        else
        {
            _bishopPEXTEntries = null;
            _rookPEXTEntries = null;
        }

        _raysBetween = AllocatePinnedArray(64 * 64, out Bitboard[] raysBetween);

        for (int square1 = 0; square1 < 64; ++square1)
        {
            for (var square2 = 0; square2 < 64; ++square2)
            {
                if (square1 == square2)
                {
                    continue;
                }

                var occupancy = 1UL << square1 | 1UL << square2;

                int file1 = Constants.File(square1);
                int file2 = Constants.File(square2);
                int rank1 = Constants.Rank(square1);
                int rank2 = Constants.Rank(square2);

                if (file1 == file2 || rank1 == rank2)
                {
                    raysBetween[(square1 * 64) + square2] = RookAttacks(square1, occupancy) & RookAttacks(square2, occupancy);
                }
                else if (file1 - rank1 == file2 - rank2
                    || file1 + rank1 == file2 + rank2)
                {
                    raysBetween[(square1 * 64) + square2] = BishopAttacks(square1, occupancy) & BishopAttacks(square2, occupancy);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard PawnAttacks(int side, int square)
    {
        Debug.Assert((uint)side < 2);
        Debug.Assert((uint)square < 64);

        return _pawnAttacks[(side * 64) + square];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard* PawnAttacks(int side)
    {
        Debug.Assert((uint)side < 2);

        return _pawnAttacks + (side * 64);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard RaysBetween(int square1, int square2)
    {
        Debug.Assert((uint)square1 < 64);
        Debug.Assert((uint)square2 < 64);

        return _raysBetween[(square1 * 64) + square2];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard BishopAttacks(int squareIndex, Bitboard occupancy)
    {
        Debug.Assert((uint)squareIndex < 64);

        if (Bmi2.X64.IsSupported)
        {
            var entry = _bishopPEXTEntries + squareIndex;
            return entry->Attacks[Bmi2.X64.ParallelBitExtract(occupancy, entry->Mask)];
        }

        return MagicNumbersBishopAttacks(squareIndex, occupancy);
    }

    /// <summary>
    /// Get Bishop attacks assuming current board occupancy
    /// </summary>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard MagicNumbersBishopAttacks(int squareIndex, Bitboard occupancy)
    {
        Debug.Assert((uint)squareIndex < 64);

        var entry = _bishopMagicEntries + squareIndex;

        return entry->Attacks[((occupancy & entry->Mask) * entry->Magic) >> entry->Shift];
    }

    /// <summary>
    /// Get Rook attacks assuming current board occupancy
    /// </summary>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard RookAttacks(int squareIndex, Bitboard occupancy)
    {
        Debug.Assert((uint)squareIndex < 64);

        if (Bmi2.X64.IsSupported)
        {
            var entry = _rookPEXTEntries + squareIndex;
            return entry->Attacks[Bmi2.X64.ParallelBitExtract(occupancy, entry->Mask)];
        }

        return MagicNumbersRookAttacks(squareIndex, occupancy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard MagicNumbersRookAttacks(int squareIndex, Bitboard occupancy)
    {
        Debug.Assert((uint)squareIndex < 64);

        var entry = _rookMagicEntries + squareIndex;

        return entry->Attacks[((occupancy & entry->Mask) * entry->Magic) >> entry->Shift];
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
    /// Taken from Leorik (https://github.com/lithander/Leorik/blob/master/Leorik.Core/Slider/PEXT.cs)
    /// Based on https://www.chessprogramming.org/BMI2#PEXT_Bitboards
    /// </summary>
    private static PEXTEntry* InitializeBishopAndRookPEXTAttacks()
    {
        var attacksPointer = AllocatePinnedArray(5248 + 102400, out Bitboard[] attacks);
        var entriesPointer = AllocatePinnedArray(128, out PEXTEntry[] entries);

        var bishopOccupancyMasks = AttackGenerator.InitializeBishopOccupancy();
        var rookOccupancyMasks = AttackGenerator.InitializeRookOccupancy();

        ulong index = 0;

        // Bishop-Attacks
        for (int square = 0; square < 64; square++)
        {
            ulong bishopMask = bishopOccupancyMasks[square];
            entries[square] = new PEXTEntry { Mask = bishopMask, Attacks = attacksPointer + index };

            ulong patterns = 1UL << BitOperations.PopCount(bishopMask);

            for (ulong i = 0; i < patterns; i++)
            {
                ulong occupation = Bmi2.X64.ParallelBitDeposit(i, bishopMask);
                attacks[index++] = MagicNumbersBishopAttacks(square, occupation);
            }
        }

        // Rook-Attacks
        for (int square = 0; square < 64; square++)
        {
            ulong rookMask = rookOccupancyMasks[square];
            entries[64 + square] = new PEXTEntry { Mask = rookMask, Attacks = attacksPointer + index };

            ulong patterns = 1UL << BitOperations.PopCount(rookMask);

            for (ulong i = 0; i < patterns; i++)
            {
                ulong occupation = Bmi2.X64.ParallelBitDeposit(i, rookMask);
                attacks[index++] = MagicNumbersRookAttacks(square, occupation);
            }
        }

        Debug.Assert(index == (ulong)attacks.Length);

        return entriesPointer;
    }
}
