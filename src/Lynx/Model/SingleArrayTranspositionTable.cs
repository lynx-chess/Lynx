using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Model;

/// <summary>
/// Single array transposition table implementation
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SingleArrayTranspositionTable : ITranspositionTable
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public int Age { get; set; }

    private readonly TranspositionTableBucket[] _tt = [];
    public int SizeMBs { get; }

    public readonly ulong Length => (ulong)_tt.Length;

    public SingleArrayTranspositionTable()
    {
        _logger.Debug("Allocating Single Array TT");
        var sw = Stopwatch.StartNew();

        SizeMBs = Configuration.EngineSettings.TranspositionTableSize;
        var ttLength = CalculateLength(SizeMBs);

        bool exceptionThrown = false;
        while (SizeMBs > Constants.AbsoluteMinTTSize)
        {
            try
            {
                _tt = GC.AllocateArray<TranspositionTableBucket>((int)ttLength, pinned: true);
                break;
            }
            catch (OutOfMemoryException e)
            {
                exceptionThrown = true;
                _logger.Warn(e, "Out of memory exception when allocating TT array of size {ArraySize} ({ArraySizeMB} MB)", ttLength, SizeMBs);

                SizeMBs /= 2;
                ttLength = CalculateLength(SizeMBs);
            }
        }

        if (exceptionThrown)
        {
            _logger.Warn("Using only TT array of size {ArraySize} ({ArraySizeMB} MB)", ttLength, SizeMBs);
        }

        _logger.Info("Single Array TT allocation time:\t{0} ms", sw.ElapsedMilliseconds);
    }

    /// <summary>
    /// Multithreaded clearing of the transposition table
    /// </summary>
    public readonly void Clear()
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
    public readonly void PrefetchTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        if (Sse.IsSupported)
        {
            var index = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);

            unsafe
            {
                // Since _tt is a pinned array
                // This is no-op pinning as it does not influence the GC compaction
                // https://tooslowexception.com/pinned-object-heap-in-net-5/
                fixed (TranspositionTableBucket* ttPtr = _tt)
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

        return Bmi2.X64.IsSupported
            ? Bmi2.X64.MultiplyNoFlags(key, (ulong)_tt.Length)
            : (ulong)(((UInt128)key * (UInt128)_tt.Length) >> 64);
    }

#pragma warning disable S4144 // Methods should not have identical implementations

    /// <summary>
    /// Get a reference to a transposition table entry for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly ref TranspositionTableBucket ITranspositionTable.GetTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return ref _tt[ttIndex];
    }

    /// <summary>
    /// Get a readonly reference to a transposition table entry for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly ref readonly TranspositionTableBucket ITranspositionTable.GetTTEntryReadonly(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return ref _tt[ttIndex];
    }

#pragma warning restore S4144 // Methods should not have identical implementations

    /// <summary>
    /// Exact transposition table occupancy per mill (0-1000)
    /// </summary>
    public readonly int HashfullPermill() => (int)(1000L * (PopulatedItemsCount() / (double)Length));

    /// <summary>
    /// Orders of magnitude faster than <see cref="HashfullPermill"/>
    /// </summary>
    public readonly int HashfullPermillApprox()
    {
        int items = 0;

        for (int i = 0; i < 1_000; ++i)
        {
            unsafe
            {
                fixed (TranspositionTableBucket* ttPtr = _tt)
                {
                    var bucketPtr = ttPtr + i;
                    var bucket = (TranspositionTableElement*)bucketPtr;

                    for (int j = 0; j < Constants.TranspositionTableElementsPerBucket; ++j)
                    {
                        TranspositionTableElement entry = bucket[j];
                        if (entry.Key != default)
                        {
                            ++items;
                        }
                    }
                }
            }
        }

        //Console.WriteLine($"Real: {HashfullPermill(transpositionTable)}, estimated: {items}");
        return items / Constants.TranspositionTableElementsPerBucket;
    }

    internal static ulong CalculateLength(int sizeMBs)
    {
        var ttEntrySize = TranspositionTableElement.Size;
        var ttBucketSize = TranspositionTableBucket.Size;

        ulong sizeBytes = (ulong)sizeMBs * 1024ul * 1024ul;
        ulong ttLength = sizeBytes / ttBucketSize;
        var ttLengthMB = (double)ttLength / 1024 / 1024;

        if (ttLength > (ulong)Array.MaxLength)
        {
            throw new ConfigurationException($"Invalid TT Hash size: {ttLengthMB} MB, {ttLength} values (> Array.MaxLength, {Array.MaxLength})");
        }

        _logger.Info("Hash value:\t{0} MB", sizeMBs);
        _logger.Info("TT memory:\t{0} MB", (ttLengthMB * ttEntrySize).ToString("F"));
        _logger.Info("TT length:\t{0} items", ttLength);
        _logger.Info("TT bucket:\t{0} bytes", ttBucketSize);
        _logger.Info("TT entry:\t{0} bytes", ttEntrySize);

        return ttLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ulong PopulatedItemsCount()
    {
        ulong items = 0;
        for (int i = 0; i < _tt.Length; ++i)
        {
            unsafe
            {
                fixed (TranspositionTableBucket* ttPtr = _tt)
                {
                    var bucketPtr = ttPtr + i;
                    var bucket = (TranspositionTableElement*)bucketPtr;

                    for (int j = 0; j < Constants.TranspositionTableElementsPerBucket; ++j)
                    {
                        TranspositionTableElement entry = bucket[j];
                        if (entry.Key != default)
                        {
                            ++items;
                        }
                    }
                }
            }
        }

        return items;
    }

    [Obsolete("Only tests")]
    internal readonly ref TranspositionTableBucket Get(int index) => ref _tt[index];

    [Conditional("DEBUG")]
    private readonly void Stats()
    {
        int items = 0;
        for (int i = 0; i < _tt.Length; ++i)
        {
            unsafe
            {
                fixed (TranspositionTableBucket* ttPtr = _tt)
                {
                    var bucketPtr = ttPtr + i;
                    var bucket = (TranspositionTableElement*)bucketPtr;

                    for (int j = 0; j < Constants.TranspositionTableElementsPerBucket; ++j)
                    {
                        TranspositionTableElement entry = bucket[j];
                        if (entry.Key != default)
                        {
                            ++items;
                        }
                    }
                }
            }
        }
        _logger.Info("TT Occupancy:\t{0}% ({1}MB)",
            100 * PopulatedItemsCount() / (ulong)_tt.Length,
            _tt.Length * Marshal.SizeOf<TranspositionTableBucket>() / 1024 / 1024);
    }

    //[Conditional("DEBUG")]
    //private readonly void Print()
    //{
    //    Console.WriteLine("Single Array Transposition table content:");
    //    for (int i = 0; i < _tt.Length; ++i)
    //    {
    //        if (_tt[i].Key != default)
    //        {
    //            Console.WriteLine($"{i}: Key = {_tt[i].Key}, Depth: {_tt[i].Depth}, Score: {_tt[i].Score}, Move: {(_tt[i].Move != 0 ? ((Move)_tt[i].Move).UCIString() : "-")} {_tt[i].Type}");
    //        }
    //    }
    //    Console.WriteLine("");
    //}
}