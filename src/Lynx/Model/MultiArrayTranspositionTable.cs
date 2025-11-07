using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Model;

/// <summary>
/// Multi-array transposition table implementation (current branch)
/// </summary>
public readonly struct MultiArrayTranspositionTable : ITranspositionTable
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly ulong _totalTTLength;
    private readonly int _ttArrayCount;
    private readonly TranspositionTableElement[][] _tt = [];

    private readonly int _size;
    public int Size => _size;

    public ulong Length => _totalTTLength;

    public MultiArrayTranspositionTable()
    {
        _logger.Debug("Allocating Multi-Array TT");
        var sw = Stopwatch.StartNew();

        _size = Configuration.EngineSettings.TranspositionTableSize;

        _totalTTLength = CalculateLength(_size);

        ulong maxArrayLength = (ulong)Constants.MaxTTArrayLength;

        ulong fullArrayCount = (_totalTTLength / maxArrayLength);
        int itemsLeft = (int)(_totalTTLength % maxArrayLength);

        var totalArrayCount = fullArrayCount
            + (itemsLeft == 0
            ? 0UL
            : 1UL);

        if (totalArrayCount > 1)
        {
            _logger.Info("Multi-Array TT arrays:\t{0}", totalArrayCount);
        }

        if (totalArrayCount > maxArrayLength)
        {
            var ttLengthGB = (double)_totalTTLength / 1024 / 1024 / 1024;
            throw new ArgumentException($"Invalid transposition table (Hash) size: {ttLengthGB}GB, {_totalTTLength} values (> Array.MaxLength, {maxArrayLength})");
        }

        _ttArrayCount = (int)totalArrayCount;

        _tt = GC.AllocateArray<TranspositionTableElement[]>(_ttArrayCount, pinned: true);
        for (int i = 0; i < (int)fullArrayCount; ++i)
        {
            _tt[i] = GC.AllocateArray<TranspositionTableElement>(Constants.MaxTTArrayLength, pinned: true);
        }

        if (itemsLeft != 0)
        {
            _tt[_ttArrayCount - 1] = GC.AllocateArray<TranspositionTableElement>(itemsLeft, pinned: true);
        }


        _logger.Info("Multi-Array TT allocation time:\t{0} ms", sw.ElapsedMilliseconds);
    }

    /// <summary>
    /// Multithreaded clearing of the transposition table
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        var threadCount = Configuration.EngineSettings.Threads;

        _logger.Debug("Zeroing Multi-Array TT using {ThreadCount} thread(s)", threadCount);
        var sw = Stopwatch.StartNew();

        // TODO: better division of work, it's probably better
        // not to go from sub-tt into sub-tt
        foreach (var tt in _tt)
        {
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
        }

        _logger.Info("Multi-Array TT clearing/zeroing time:\t{0} ms", sw.ElapsedMilliseconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrefetchTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        if (Sse.IsSupported)
        {
            (var ttIndex, var entryIndex) = CalculateTTIndexes(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
            Debug.Assert(ttIndex < _ttArrayCount);
            Debug.Assert(entryIndex < Constants.MaxTTArrayLength);

            unsafe
            {
                // Since _tt is a pinned array
                // This is no-op pinning as it does not influence the GC compaction
                // https://tooslowexception.com/pinned-object-heap-in-net-5/
                fixed (TranspositionTableElement* ttPtr = _tt[ttIndex])
                {
                    Sse.Prefetch0(ttPtr + entryIndex);
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

        return (ulong)(((UInt128)key * (UInt128)_totalTTLength) >> 64);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly (int, int) CalculateTTIndexes(ulong positionUniqueIdentifier, int halfMovesWithoutCaptureOrPawnMove)
    {
        var globalIndex = CalculateTTIndex(positionUniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);

        ulong maxTTArrayLength = (ulong)Constants.MaxTTArrayLength;

        var ttIndex = (int)(globalIndex / maxTTArrayLength);
        var itemIndex = (int)(globalIndex % maxTTArrayLength);

        return (ttIndex, itemIndex);
    }

    /// <summary>
    /// Get a reference to a transposition table entry for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ref TranspositionTableElement ITranspositionTable.GetTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        (var ttIndex, var entryIndex) = CalculateTTIndexes(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return ref _tt[ttIndex][entryIndex];
    }

    /// <summary>
    /// Get a readonly reference to a transposition table entry for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ref readonly TranspositionTableElement ITranspositionTable.GetTTEntryReadonly(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        (var ttIndex, var entryIndex) = CalculateTTIndexes(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return ref _tt[ttIndex][entryIndex];
    }

    /// <summary>
    /// Exact TT occupancy per mill
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int HashfullPermill() => _tt.Length > 0
        ? (int)(1000L * (PopulatedItemsCount() / (ulong)_tt.LongLength))
        : 0;

    /// <summary>
    /// Orders of magnitude faster than <see cref="HashfullPermill(TranspositionTableElement[])"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int HashfullPermillApprox()
    {
        int items = 0;

        if (_tt[0].Length >= 1_000)
        {
            for (int i = 0; i < 1_000; ++i)
            {
                if (_tt[0][i].Key != default)
                {
                    ++items;
                }
            }
        }

        //Console.WriteLine($"Real: {HashfullPermill(transpositionTable)}, estimated: {items}");
        return items;
    }

    internal static ulong CalculateLength(int size)
    {
        var ttEntrySize = TranspositionTableElement.Size;

        ulong sizeBytes = (ulong)size * 1024ul * 1024ul;
        ulong ttLength = sizeBytes / ttEntrySize;
        var ttLengthMB = (double)ttLength / 1024 / 1024;

        if (ttLength > (ulong)Constants.MaxTTArrayLength)
        {
            _logger.Info($"More than one TT array will be used for transposition table (Hash) size: {ttLengthMB * ttEntrySize / 1024}GB, {ttLength} values (> Array.MaxLength, {Constants.MaxTTArrayLength})");
        }

        _logger.Info("Hash value:\t{0} MB", size);
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
            for (int j = 0; j < _tt[i].Length; ++j)
            {
                if (_tt[i][j].Key != default)
                {
                    ++items;
                }
            }
        }

        return items;
    }

    [Obsolete("Only tests")]
    internal ref TranspositionTableElement Get(int index) => ref _tt[0][index];

    [Conditional("DEBUG")]
    private void Stats()
    {
        int items = 0;
        for (int i = 0; i < _tt.Length; ++i)
        {
            for (int j = 0; j < _tt[i].Length; ++j)
            {
                if (_tt[i][j].Key != default)
                {
                    ++items;
                }
            }
        }
        _logger.Info("TT Occupancy:\t{0}% ({1}MB)",
            100 * PopulatedItemsCount() / _totalTTLength,
            (long)_totalTTLength * Marshal.SizeOf<TranspositionTableElement>() / 1024 / 1024);
    }

    [Conditional("DEBUG")]
    private readonly void Print()
    {
        Console.WriteLine("Transposition table content:");
        for (int i = 0; i < _tt.Length; ++i)
        {
            for (int j = 0; j < _tt[i].Length; ++j)
            {
                if (_tt[i][j].Key != default)
                {
                    var entry = _tt[i][j];
                    Console.WriteLine($"Array {i}, item {j} (total item index: {i * Constants.MaxTTArrayLength + j}): "
                    + $"Key = {entry.Key}, Depth: {entry.Depth}, Score: {entry.Score}, Move: {(entry.Move != 0 ? ((Move)entry.Move).UCIString() : " - ")} {entry.Type}");
                }
            }
        }
        Console.WriteLine("");
    }
}
