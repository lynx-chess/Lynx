using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

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

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable S1450 // Private fields only used as local variables in methods should become local variables

    /// <summary>
    /// Backs up bishop and rook arrays of <see cref="PEXTEntry"/>: [0..63] bishop, [64..127] rook
    /// </summary>
    private static readonly PEXTEntry[] _PEXTEntries;
    private static readonly PEXTEntry* _bishopPEXTEntries;
    private static readonly PEXTEntry* _rookPEXTEntries;

    /// <summary>
    /// Backs up bishop and rook arrays of <see cref="MagicEntry"/>: [0..63] bishop, [64..127] rook
    /// </summary>
    private static readonly MagicEntry[] _magicEntries;
    private static readonly MagicEntry* _bishopMagicEntries;
    private static readonly MagicEntry* _rookMagicEntries;

    #pragma warning restore S1450 // Private fields only used as local variables in methods should become local variables

    /// <summary>
    /// Packed bishop (5248) and rook (102400) magic attacks
    /// Use <see cref="BishopAttacks(int, Bitboard)"/> and <see cref="RookAttacks(int, Bitboard)"/>
    /// </summary>
    private static readonly Bitboard[] _magicAttacks;

    private static readonly Bitboard[] _bishopOccupancyMasks;
    private static readonly Bitboard[] _rookOccupancyMasks;

    private static readonly ulong[] _PEXTAttacks;
    private static readonly ulong[] _PEXTBishopOffset;
    private static readonly ulong[] _PEXTRookOffset;

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


    /// <summary>
    /// 64 x 64
    /// </summary>
#pragma warning disable S2386, S3887 // Mutable fields should not be "public static"
    public static readonly Bitboard[][] RaysBetween;
#pragma warning restore S2386, S3887 // Mutable fields should not be "public static"

#pragma warning disable CA1810 // Initialize reference type static fields inline
    static Attacks()
#pragma warning restore CA1810 // Initialize reference type static fields inline
    {
        KingAttacks = AttackGenerator.InitializeKingAttacks();
        PawnAttacks = AttackGenerator.InitializePawnAttacks();
        KnightAttacks = AttackGenerator.InitializeKnightAttacks();

        _bishopOccupancyMasks = AttackGenerator.InitializeBishopOccupancy();
        _rookOccupancyMasks = AttackGenerator.InitializeRookOccupancy();

        (_magicAttacks, _magicEntries) = AttackGenerator.InitializeBishopAndRookMagicAttacks();
        _bishopMagicEntries = (MagicEntry*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(_magicEntries));
        _rookMagicEntries = _bishopMagicEntries + 64;

        if (Bmi2.X64.IsSupported)
        {
            _PEXTAttacks = GC.AllocateArray<Bitboard>(5248 + 102400, pinned: true);
            _PEXTBishopOffset = GC.AllocateArray<Bitboard>(64, pinned: true);
            _PEXTRookOffset = GC.AllocateArray<Bitboard>(64, pinned: true);

            InitializeBishopAndRookPEXTAttacks();

            _PEXTEntries = GC.AllocateArray<PEXTEntry>(128, pinned: true);
            _bishopPEXTEntries = (PEXTEntry*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(_PEXTEntries));
            _rookPEXTEntries = _bishopPEXTEntries + 64;

            var PEXTAttacks = (Bitboard*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(_PEXTAttacks));
            for (int square = 0; square < 64; ++square)
            {
                _bishopPEXTEntries[square] = new PEXTEntry { Mask = _bishopOccupancyMasks[square], Attacks = PEXTAttacks + _PEXTBishopOffset[square] };
                _rookPEXTEntries[square] = new PEXTEntry { Mask = _rookOccupancyMasks[square], Attacks = PEXTAttacks + _PEXTRookOffset[square] };
            }
        }
        else
        {
            _PEXTAttacks = [];
            _PEXTBishopOffset = [];
            _PEXTRookOffset = [];
            _PEXTEntries = [];
            _bishopPEXTEntries = null;
            _rookPEXTEntries = null;
        }

        RaysBetween = new Bitboard[64][];

        for (int square1 = 0; square1 < 64; ++square1)
        {
            RaysBetween[square1] = new Bitboard[64];

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
                    RaysBetween[square1][square2] = RookAttacks(square1, occupancy) & RookAttacks(square2, occupancy);
                }
                else if (file1 - rank1 == file2 - rank2
                    || file1 + rank1 == file2 + rank2)
                {
                    RaysBetween[square1][square2] = BishopAttacks(square1, occupancy) & BishopAttacks(square2, occupancy);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard BishopAttacks(int squareIndex, Bitboard occupancy)
    {
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
    private static void InitializeBishopAndRookPEXTAttacks()
    {
        ulong index = 0;

        // Bishop-Attacks
        for (int square = 0; square < 64; square++)
        {
            _PEXTBishopOffset[square] = index;
            ulong bishopMask = _bishopOccupancyMasks[square];

            ulong patterns = 1UL << BitOperations.PopCount(bishopMask);

            for (ulong i = 0; i < patterns; i++)
            {
                ulong occupation = Bmi2.X64.ParallelBitDeposit(i, bishopMask);
                _PEXTAttacks[index++] = MagicNumbersBishopAttacks(square, occupation);
            }
        }

        // Rook-Attacks
        for (int square = 0; square < 64; square++)
        {
            _PEXTRookOffset[square] = index;
            ulong rookMask = _rookOccupancyMasks[square];
            ulong patterns = 1UL << BitOperations.PopCount(rookMask);

            for (ulong i = 0; i < patterns; i++)
            {
                ulong occupation = Bmi2.X64.ParallelBitDeposit(i, rookMask);
                _PEXTAttacks[index++] = MagicNumbersRookAttacks(square, occupation);
            }
        }
    }
}
