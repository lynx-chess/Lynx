/*
 *
 *  TODO: add results
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Benchmark;

/// <summary>
/// Benchmarks <see cref="ZobristTable.PieceHash(int, int)"/> table layout alternatives:
/// the original jagged <c>ulong[][]</c> vs a flat <c>ulong[]</c> with bounds-checked access
/// vs a flat <c>ulong[]</c> with <see cref="Unsafe.Add"/> (what is currently implemented).
/// The <c>CurrentPieceHash</c> method calls <see cref="ZobristTable.PieceHash"/> directly to
/// confirm it matches the <c>FlatUnsafe</c> baseline.
/// </summary>
public class ZobristHash_PieceTable_Benchmark : BaseBenchmark
{
    public static IEnumerable<Position> Data =>
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    private readonly ulong[][] _jaggedTable;
    private readonly ulong[] _flatTable;

    public ZobristHash_PieceTable_Benchmark()
    {
        _jaggedTable = InitializeJagged(new LynxRandom(int.MaxValue));
        _flatTable = InitializeFlat(new LynxRandom(int.MaxValue));
    }

    /// <summary>
    /// Original layout: <c>_table[square][piece]</c> — two pointer dereferences per lookup.
    /// </summary>
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ulong Jagged(Position position)
    {
        ulong hash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitboards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var square);
                hash ^= _jaggedTable[square][pieceIndex];
            }
        }

        return hash;
    }

    /// <summary>
    /// Flat layout: <c>_table[square * 12 + piece]</c> — single bounds-checked array access.
    /// </summary>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong Flat(Position position)
    {
        ulong hash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitboards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var square);
                hash ^= _flatTable[(square * 12) + pieceIndex];
            }
        }

        return hash;
    }

    /// <summary>
    /// Flat layout with <see cref="Unsafe.Add"/> on a pinned array — no bounds check.
    /// This is what <see cref="ZobristTable.PieceHash"/> currently uses.
    /// </summary>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong FlatUnsafe(Position position)
    {
        ulong hash = 0;
        ref var tableRef = ref MemoryMarshal.GetArrayDataReference(_flatTable);

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitboards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var square);
                hash ^= Unsafe.Add(ref tableRef, square * 12 + pieceIndex);
            }
        }

        return hash;
    }

    /// <summary>
    /// Calls the production <see cref="ZobristTable.PieceHash"/> directly (currently flat + unsafe).
    /// </summary>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong CurrentPieceHash(Position position)
    {
        ulong hash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitboards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var square);
                hash ^= ZobristTable.PieceHash(square, pieceIndex);
            }
        }

        return hash;
    }

    private static ulong[][] InitializeJagged(LynxRandom random)
    {
        var table = new ulong[64][];

        for (int square = 0; square < 64; ++square)
        {
            table[square] = new ulong[12];

            for (int piece = 0; piece < 12; ++piece)
            {
                table[square][piece] = random.NextUInt64();
            }
        }

        return table;
    }

    private static ulong[] InitializeFlat(LynxRandom random)
    {
        var table = GC.AllocateArray<ulong>(64 * 12, pinned: true);

        for (int square = 0; square < 64; ++square)
        {
            for (int piece = 0; piece < 12; ++piece)
            {
                table[square * 12 + piece] = random.NextUInt64();
            }
        }

        return table;
    }
}
