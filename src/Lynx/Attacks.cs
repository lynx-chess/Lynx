using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx;

public static unsafe class Attacks
{
    /// <summary>
    /// Stores both the mask and the attack for a certain piece type
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    private struct PEXTEntry
    {
        public Bitboard Mask;
        public Bitboard* Attacks;
    }

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable S1450 // Private fields only used as local variables in methods should become local variables
    /// <summary>
    /// Backs up bishop and rook arrays: [0..63] bishop, [64..127] rook
    /// </summary>
    private static readonly PEXTEntry[] _PEXTEntries;
#pragma warning restore S1450 // Private fields only used as local variables in methods should become local variables
    private static readonly PEXTEntry* _bishopPEXTEntries;
    private static readonly PEXTEntry* _rookPEXTEntries;

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

    private static readonly ulong[] _PEXTAttacks;
    private static readonly ulong[] _PEXTBishopOffset;
    private static readonly ulong[] _PEXTRookOffset;
#pragma warning restore IDE1006 // Naming Styles

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

        (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopMagicAttacks();
        (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookMagicAttacks();

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
