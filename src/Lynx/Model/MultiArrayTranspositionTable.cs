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

    private readonly int _ttArrayCount;
    private readonly TranspositionTableElement[][] _tt = [];
    public int SizeMBs { get; }
    public ulong Length { get; }

    public MultiArrayTranspositionTable()
    {
        _logger.Debug("Allocating Multi-Array TT");
        var sw = Stopwatch.StartNew();

        SizeMBs = Configuration.EngineSettings.TranspositionTableSize;

        Length = CalculateLength(SizeMBs);

        (var fullArrayCount, var itemsLeft) = Math.DivRem(Length, (ulong)Constants.MaxTTArrayLength);

        Debug.Assert(fullArrayCount <= int.MaxValue);
        Debug.Assert(itemsLeft <= int.MaxValue);

        _ttArrayCount = (int)fullArrayCount
            + (itemsLeft == 0
                ? 0
                : 1);

        if (_ttArrayCount > 1)
        {
            _logger.Info("Multi-Array TT arrays:\t{0}", _ttArrayCount);
        }

        if (_ttArrayCount > Constants.MaxTTArrayLength)
        {
            var ttLengthGB = (double)Length / 1024 / 1024 / 1024;
            throw new ArgumentException($"Invalid TT Hash size: {ttLengthGB} GB, {Length} values (> {Constants.MaxTTArrayLength})");
        }

        _tt = GC.AllocateArray<TranspositionTableElement[]>(_ttArrayCount, pinned: true);
        for (int i = 0; i < (int)fullArrayCount; ++i)
        {
            _tt[i] = GC.AllocateArray<TranspositionTableElement>(Constants.MaxTTArrayLength, pinned: true);
        }

        if (itemsLeft != 0)
        {
            _tt[_ttArrayCount - 1] = GC.AllocateArray<TranspositionTableElement>((int)itemsLeft, pinned: true);
        }

        _logger.Info("Multi-Array TT allocation time:\t{0} ms", sw.ElapsedMilliseconds);
    }

    /// <summary>
    /// Multithreaded clearing of the transposition table
    /// </summary>
    public void Clear()
    {
        var threadsPerTTArray = Configuration.EngineSettings.Threads / _ttArrayCount;

        var sw = Stopwatch.StartNew();

        if (threadsPerTTArray >= 1)
        {
            ParallelTTArrayZeroing(threadsPerTTArray);
        }
        else
        {
            SequentialTTArrayZeroing();
        }

        _logger.Info("Multi-Array TT zeroing completed in:\t{0} ms", sw.ElapsedMilliseconds);
    }

    private void ParallelTTArrayZeroing(int threadsPerTTArray)
    {
        _logger.Info("Multi-Array TT zeroing using {ZeroingThreadCount} (out of {TotalThreadCount}) thread(s), using {PerTTThreadCount} thread per TT array",
            _ttArrayCount * threadsPerTTArray, Configuration.EngineSettings.Threads, threadsPerTTArray);

        var localTTRef = _tt;

        // Instead of just doing foreach (var tt in _tt)
        Parallel.For(0, _ttArrayCount, ttArrayIndex =>
        {
            var tt = localTTRef[ttArrayIndex];

            var ttLength = tt.Length;
            var sizePerThread = ttLength / threadsPerTTArray;

            // Instead of just doing Array.Clear(_tt):
            Parallel.For(0, threadsPerTTArray, threadIndex =>
            {
                var start = threadIndex * sizePerThread;
                var length = (threadIndex == threadsPerTTArray - 1)
                    ? ttLength - start
                    : sizePerThread;

                Array.Clear(tt, start, length);
            });
        });
    }

    private void SequentialTTArrayZeroing()
    {
        var threadCount = Configuration.EngineSettings.Threads;
        _logger.Info("Multi-Array TT zeroing using {ThreadCount} thread(s), one TT array at a time", threadCount);

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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrefetchTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        if (Sse.IsSupported)
        {
            (var ttIndex, var entryIndex) = CalculateTTIndexes(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
            Debug.Assert(ttIndex < _ttArrayCount);
            Debug.Assert(entryIndex < _tt[ttIndex].Length);

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

        return (ulong)(((UInt128)key * (UInt128)Length) >> 64);
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

#pragma warning disable S4144 // Methods should not have identical implementations

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

#pragma warning restore S4144 // Methods should not have identical implementations

    /// <summary>
    /// Exact TT occupancy per mill
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
            if (_tt[0][i].Key != default)
            {
                ++items;
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
            _logger.Info("More than one TT array will be used for TT Hash size: {RequestedHashSize} GB, {TTLength} values (> {TTArraySizeGBs} GB, {MaxTTArrayLength} items",
                (ttLengthMB * ttEntrySize / 1024).ToString("F2"), ttLength, Constants.TTArraySizeGBs.ToString("F2"), Constants.MaxTTArrayLength);
        }
        else
        {
            _logger.Warn("Multi-Array TT used, but single TT array expected for TT Hash size of {RequestedHashSize} GB and {TTLength} values. Max values are {TTArraySizeGBs} GB, {MaxTTArrayLength} items",
                (ttLengthMB * ttEntrySize / 1024).ToString("F2"),ttLength, Constants.TTArraySizeGBs.ToString("F2"), Constants.MaxTTArrayLength);
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
            100 * PopulatedItemsCount() / Length,
            (long)Length * Marshal.SizeOf<TranspositionTableElement>() / 1024 / 1024);
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
                    Console.WriteLine($"Array {i}, item {j} (total item index: {(i * Constants.MaxTTArrayLength) + j}): "
                    + $"Key = {entry.Key}, Depth: {entry.Depth}, Score: {entry.Score}, Move: {(entry.Move != 0 ? ((Move)entry.Move).UCIString() : " - ")} {entry.Type}");
                }
            }
        }
        Console.WriteLine("");
    }
}
