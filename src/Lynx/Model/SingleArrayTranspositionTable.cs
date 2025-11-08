using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Model;

/// <summary>
/// Single array transposition table implementation (from main branch)
/// </summary>
public readonly struct SingleArrayTranspositionTable : ITranspositionTable
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly TranspositionTableElement[] _tt = [];
    public int SizeMBs { get; }

    public ulong Length => (ulong)_tt.Length;

    public SingleArrayTranspositionTable()
    {
        _logger.Debug("Allocating Single Array TT");
        var sw = Stopwatch.StartNew();

        SizeMBs = Configuration.EngineSettings.TranspositionTableSize;

        var ttLength = CalculateLength(SizeMBs);
        _tt = GC.AllocateArray<TranspositionTableElement>((int)ttLength, pinned: true);

        _logger.Info("Single Array TT allocation time:\t{0} ms", sw.ElapsedMilliseconds);
    }

    /// <summary>
    /// Multithreaded clearing of the transposition table
    /// </summary>
    public void Clear()
    {
        var threadCount = Configuration.EngineSettings.Threads;

        _logger.Debug("Zeroing Single Array TT using {ThreadCount} thread(s)", threadCount);
        var sw = Stopwatch.StartNew();

        var tt = _tt;
        var ttLength = tt.Length;
        var sizePerThread = ttLength / threadCount;

        // Instead of just doing Array.Clear(_tt):
        Parallel.For(0, threadCount, i =>
        {
            var start = i * sizePerThread;
            var length = (i == threadCount - 1)
                ? ttLength - start
                : sizePerThread;

            Array.Clear(tt, start, length);
        });

        _logger.Info("Single Array TT clearing/zeroing time:\t{0} ms", sw.ElapsedMilliseconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrefetchTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        if (Sse.IsSupported)
        {
            var index = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);

            unsafe
            {
                // Since _tt is a pinned array
                // This is no-op pinning as it does not influence the GC compaction
                // https://tooslowexception.com/pinned-object-heap-in-net-5/
                fixed (TranspositionTableElement* ttPtr = _tt)
                {
                    Sse.Prefetch0(ttPtr + index);
                }
            }
        }
    }

    /// <summary>
    /// 'Fixed-point multiplication trick', see https://lemire.me/blog/2016/06/27/a-fast-alternative-to-the-modulo-reduction/
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ulong CalculateTTIndex(ulong positionUniqueIdentifier, int halfMovesWithoutCaptureOrPawnMove)
    {
        var key = positionUniqueIdentifier ^ ZobristTable.HalfMovesWithoutCaptureOrPawnMoveHash(halfMovesWithoutCaptureOrPawnMove);

        return (ulong)(((UInt128)key * (UInt128)_tt.Length) >> 64);
    }

    #pragma warning disable S4144 // Methods should not have identical implementations

    /// <summary>
    /// Get a reference to a transposition table entry for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ref TranspositionTableElement ITranspositionTable.GetTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return ref _tt[ttIndex];
    }

    /// <summary>
    /// Get a readonly reference to a transposition table entry for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ref readonly TranspositionTableElement ITranspositionTable.GetTTEntryReadonly(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return ref _tt[ttIndex];
    }

    #pragma warning restore S4144 // Methods should not have identical implementations

    /// <summary>
    /// Exact transposition table occupancy per mill (0-1000)
    /// </summary>
    public int HashfullPermill() => (int)(1000L * (PopulatedItemsCount() / (double)Length));

    /// <summary>
    /// Orders of magnitude faster than <see cref="HashfullPermill"/>
    /// </summary>
    public readonly int HashfullPermillApprox()
    {
        int items = 0;

        for (int i = 0; i < 1_000; ++i)
        {
            if (_tt[i].Key != default)
            {
                ++items;
            }
        }

        //Console.WriteLine($"Real: {HashfullPermill(transpositionTable)}, estimated: {items}");
        return items;
    }

    internal static ulong CalculateLength(int sizeMBs)
    {
        var ttEntrySize = TranspositionTableElement.Size;

        ulong sizeBytes = (ulong)sizeMBs * 1024ul * 1024ul;
        ulong ttLength = sizeBytes / ttEntrySize;
        var ttLengthMB = (double)ttLength / 1024 / 1024;

        if (ttLength > (ulong)Array.MaxLength)
        {
            throw new ArgumentException($"Invalid TT Hash size: {ttLengthMB} MB, {ttLength} values (> Array.MaxLength, {Array.MaxLength})");
        }

        _logger.Info("Hash value:\t{0} MB", sizeMBs);
        _logger.Info("TT memory:\t{0} MB", (ttLengthMB * ttEntrySize).ToString("F"));
        _logger.Info("TT length:\t{0} items", ttLength);
        _logger.Info("TT entry:\t{0} bytes", ttEntrySize);

        return ttLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ulong PopulatedItemsCount()
    {
        ulong items = 0;
        for (int i = 0; i < _tt.Length; ++i)
        {
            if (_tt[i].Key != default)
            {
                ++items;
            }
        }

        return items;
    }

    [Obsolete("Only tests")]
    internal ref TranspositionTableElement Get(int index) => ref _tt[index];

    [Conditional("DEBUG")]
    private void Stats()
    {
        ulong items = 0;
        for (int i = 0; i < _tt.Length; ++i)
        {
            if (_tt[i].Key != default)
            {
                ++items;
            }
        }
        _logger.Info("Single Array TT Occupancy:\t{0}% ({1}MB)",
            100 * PopulatedItemsCount() / (ulong)_tt.Length,
            (ulong)_tt.Length * (ulong)Marshal.SizeOf<TranspositionTableElement>() / 1024 / 1024);
    }

    [Conditional("DEBUG")]
    private readonly void Print()
    {
        Console.WriteLine("Single Array Transposition table content:");
        for (int i = 0; i < _tt.Length; ++i)
        {
            if (_tt[i].Key != default)
            {
                Console.WriteLine($"{i}: Key = {_tt[i].Key}, Depth: {_tt[i].Depth}, Score: {_tt[i].Score}, Move: {(_tt[i].Move != 0 ? ((Move)_tt[i].Move).UCIString() : "-")} {_tt[i].Type}");
            }
        }
        Console.WriteLine("");
    }
}