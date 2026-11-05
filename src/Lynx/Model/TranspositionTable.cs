using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Model;

/// <summary>
/// Transposition table
/// Every engine receives its own instance of this struct, but they all point to the same native memory block.
/// [ age (<see cref="Alignment"/> bytes) | <see cref="Length"/> buckets (<see cref="SizeMBs"/> MB) ]
/// Each bucket holds <see cref="Constants.TranspositionTableEntriesPerBucket"/> entries, see <see cref="TranspositionTableBucket"/>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct TranspositionTable
{
    private const nuint Alignment = 64;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly byte* _nativeMemoryPointer;

    private readonly TranspositionTableBucket* _buckets;

    private readonly int* _age;

    public int RequestedSizeMBs { get; }

    public int SizeMBs { get; }

    /// <summary>
    /// Number of buckets
    /// </summary>
    public nuint Length { get; }

    public TranspositionTable()
    {
        Debug.Assert(Constants.TranspositionTableEntriesPerBucket * sizeof(TranspositionTableElement) <= Constants.TranspositionTableBucketSize);
        Debug.Assert(Alignment % Constants.TranspositionTableBucketSize == 0);

        _logger.Debug("Allocating TT");
        var sw = Stopwatch.StartNew();

        RequestedSizeMBs = Configuration.EngineSettings.TranspositionTableSize;
        SizeMBs = RequestedSizeMBs;
        var ttLength = CalculateLength(SizeMBs);
        var totalSizeBytes = ttLength * (nuint)sizeof(TranspositionTableBucket);

        bool exceptionThrown = false;
        do
        {
            try
            {
                _nativeMemoryPointer = (byte*)NativeMemory.AlignedAlloc(Alignment + totalSizeBytes, Alignment);
                _age = (int*)_nativeMemoryPointer;
                _buckets = (TranspositionTableBucket*)(_nativeMemoryPointer + Alignment);

                *_age = 0;
                break;
            }
            catch (OutOfMemoryException e)
            {
                exceptionThrown = true;
                _logger.Warn(e, "Out of memory exception when allocating TT array of size {ArraySize} ({ArraySizeMB} MB)", ttLength, SizeMBs);

                SizeMBs /= 2;
                ttLength = CalculateLength(SizeMBs);
                totalSizeBytes = ttLength * (nuint)sizeof(TranspositionTableBucket);
            }
        } while (SizeMBs > Constants.AbsoluteMinTTSize);

        Length = ttLength;

        if (exceptionThrown)
        {
            _logger.Warn("Using only TT array of size {ArraySize} ({ArraySizeMB} MB)", ttLength, SizeMBs);
        }

        _logger.Info("TT allocation time:\t{0} ms", sw.ElapsedMilliseconds);
    }

    /// <summary>
    /// Checks the transposition table and, if there's an eval value that can be deducted from it
    /// for a previously recorded position, it's returned. Returns false otherwise.
    /// </summary>
    /// <param name="position">The position to probe</param>
    /// <param name="halfMovesWithoutCaptureOrPawnMove">Half moves without capture or pawn move counter</param>
    /// <param name="ply">Current ply (distance from root)</param>
    /// <param name="result">The probe result if found</param>
    /// <returns>True if a valid entry was found, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ProbeHash(Position position, int halfMovesWithoutCaptureOrPawnMove, int ply, out TTProbeResult result)
    {
        TranspositionTableElement* bucket = GetBucket(position, halfMovesWithoutCaptureOrPawnMove);

        var key = GenerateTTKey(position.UniqueIdentifier);

        for (int i = 0; i < Constants.TranspositionTableEntriesPerBucket; ++i)
        {
            TranspositionTableElement* entry = bucket + i;

            if (key == entry->Key)
            {
                // We want to translate the checkmate position relative to the saved node to our root position from which we're searching
                // If the recorded score is a checkmate in 3 and we are at depth 5, we want to read checkmate in 8
                var recalculatedScore = RecalculateMateScores(entry->Score, ply);

                result = new TTProbeResult(recalculatedScore, entry->Move, entry->Type, entry->StaticEval, entry->Depth, entry->WasPv);

                return true;
            }
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Adds a TranspositionTableElement to the transposition table
    /// </summary>
    /// <param name="position">The position to record</param>
    /// <param name="halfMovesWithoutCaptureOrPawnMove">Half moves without capture or pawn move counter</param>
    /// <param name="staticEval">Static evaluation of the position</param>
    /// <param name="depth">Search depth</param>
    /// <param name="ply">Current ply (distance from root)</param>
    /// <param name="score">Position score</param>
    /// <param name="nodeType">Type of node (Alpha, Beta, Exact)</param>
    /// <param name="wasPv">Whether this position was part of the principal variation</param>
    /// <param name="move">Best move found (null for fail-low nodes)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RecordHash(Position position, int halfMovesWithoutCaptureOrPawnMove, int staticEval, int depth, int ply, int score, NodeType nodeType, bool wasPv, Move? move = null)
    {
        Debug.Assert(nodeType != NodeType.Alpha || move is null, "Assertion failed", "There's no 'best move' on fail-lows, so TT one won't be overridden");

        TranspositionTableElement* bucket = GetBucket(position, halfMovesWithoutCaptureOrPawnMove);

        var newKey = GenerateTTKey(position.UniqueIdentifier);
        var age = *_age;

        TranspositionTableElement* entry = SelectEntry(bucket, newKey, age);

        var wasPvInt = wasPv ? 1 : 0;

        bool shouldReplace =
            entry->Key != newKey                // Different key: collision or no actual entry, entry is the least valuable one in the bucket
            || nodeType == NodeType.Exact       // Entering PV data
            || entry->Age != age                // Entry from a previous search
            || depth                            // Higher depth
                    + Configuration.EngineSettings.TTReplacement_DepthOffset
                    + (Configuration.EngineSettings.TTReplacement_TTPVDepthOffset * wasPvInt)
                >= entry->Depth;

        if (!shouldReplace)
        {
            return;
        }

        // We want to store the distance to the checkmate position relative to the current node, independently from the root
        // If the evaluated score is a checkmate in 8 and we're at depth 5, we want to store checkmate value in 3
        var recalculatedScore = RecalculateMateScores(score, -ply);

        entry->Update(newKey, recalculatedScore, staticEval, depth, nodeType, wasPvInt, move, age);
    }

    /// <summary>
    /// Save only the static evaluation for a position in the transposition table
    /// </summary>
    /// <param name="position">The position to save static eval for</param>
    /// <param name="halfMovesWithoutCaptureOrPawnMove">Half moves without capture or pawn move counter</param>
    /// <param name="staticEval">Static evaluation of the position</param>
    /// <param name="wasPv">Whether this position was part of the principal variation</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SaveStaticEval(Position position, int halfMovesWithoutCaptureOrPawnMove, int staticEval, bool wasPv)
    {
        TranspositionTableElement* bucket = GetBucket(position, halfMovesWithoutCaptureOrPawnMove);

        var key = GenerateTTKey(position.UniqueIdentifier);
        var age = *_age;

        TranspositionTableElement* entry = SelectEntry(bucket, key, age);

        // Extra key checks here (right before saving) failed for MT in https://github.com/lynx-chess/Lynx/pull/1566
        entry->Update(key, EvaluationConstants.NoScore, staticEval, depth: 0, NodeType.Unknown, wasPv ? 1 : 0, move: null, age);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static TranspositionTableElement* SelectEntry(TranspositionTableElement* bucket, ushort key, int age)
    {
        TranspositionTableElement* selectedEntry = bucket;
        int minValue = int.MaxValue;

        for (int i = 0; i < Constants.TranspositionTableEntriesPerBucket; ++i)
        {
            TranspositionTableElement* candidate = bucket + i;

            // Same position or empty entry
            // In empty entries, some FP: positions with Key == 0 and only static eval saved. Hopefully rare enough?
            if (candidate->Key == key
                || (candidate->Key == 0 && candidate->Type == NodeType.Unknown))
            {
                return candidate;
            }

            // Equivalent of:
            // relativeAge = age - candidate->Age;
            // if (relativeAge < 0) relativeAge += MaxAge
            // No need to worry about negative numbers as long as TranspositionTableElement.MaxAge is a power of two 
            var relativeAge = (age - candidate->Age) & TranspositionTableElement.AgeMask;

            // Formula from Stormphrax
            var value = candidate->Depth - (2 * relativeAge);

            if (value < minValue)
            {
                minValue = value;
                selectedEntry = candidate;
            }
        }

        return selectedEntry;
    }

    public int Age
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => *_age;
    }

    /// <summary>
    /// Maakes <see cref="Age"/> a circular buffer [0, <see cref="TranspositionTableElement.MaxAge"/> - 1]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BumpAge() => *_age = (*_age + 1) & TranspositionTableElement.AgeMask;

    /// <summary>
    /// Multithreaded clearing of the transposition table
    /// </summary>
    public void Clear()
    {
        var threadCount = Configuration.EngineSettings.Threads;

        _logger.Debug("Zeroing TT using {ThreadCount} thread(s)", threadCount);
        var sw = Stopwatch.StartNew();

        var tt = _buckets;
        var ttLength = Length;
        var sizePerThread = ttLength / (nuint)threadCount;

        // Instead of just doing _tt.Clear():
        Parallel.For(0, threadCount, i =>
        {
            var start = (nuint)i * sizePerThread;
            var length = (i == threadCount - 1)
                ? ttLength - start
                : sizePerThread;

            var ttPtr = tt + start;
            var bytesToClear = (nuint)sizeof(TranspositionTableBucket) * length;

            NativeMemory.Clear(ttPtr, bytesToClear);
        });

        *_age = 0;

        _logger.Info("TT clearing/zeroing time using {0} threads:\t{1} ms", threadCount, sw.ElapsedMilliseconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrefetchTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        if (Sse.IsSupported)
        {
            var index = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);

            Sse.Prefetch0(_buckets + index);
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
            ? Bmi2.X64.MultiplyNoFlags(key, Length)
            : (ulong)(((UInt128)key * (UInt128)Length) >> 64);
    }

#pragma warning disable S4144 // Methods should not have identical implementations

    /// <summary>
    /// Get a pointer to the first entry of the bucket for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TranspositionTableElement* GetBucket(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return (TranspositionTableElement*)&_buckets[(nint)ttIndex];
    }

    /// <summary>
    /// Use lowest 16 bits of the position unique identifier as the key
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ushort GenerateTTKey(ulong positionUniqueIdentifier)
        => (ushort)positionUniqueIdentifier;

    /// <summary>
    /// If playing side is giving checkmate, decrease checkmate score (increase n in checkmate in n moves) due to being searching at a given depth already when this position is found.
    /// The opposite if the playing side is getting checkmated.
    /// Logic for when to pass +depth or -depth for the desired effect in https://www.talkchess.com/forum3/viewtopic.php?f=7&t=74411 and https://talkchess.com/forum3/viewtopic.php?p=861852#p861852
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int RecalculateMateScores(int score, int ply)
    {
#pragma warning disable MA0071 // Avoid using redundant else
        if (score > EvaluationConstants.PositiveCheckmateDetectionLimit)
        {
            return score - ply;
        }
        else if (score < EvaluationConstants.NegativeCheckmateDetectionLimit && score != EvaluationConstants.NoScore)
        {
            return score + ply;
        }
#pragma warning restore MA0071 // Avoid using redundant else

        return score;
    }

#pragma warning restore S4144 // Methods should not have identical implementations

    /// <summary>
    /// Exact transposition table occupancy per mill (0-1000)
    /// </summary>
    public int HashfullPermill() => (int)(1000L * (PopulatedItemsCount() / ((double)Length * Constants.TranspositionTableEntriesPerBucket)));

    /// <summary>
    /// Orders of magnitude faster than <see cref="HashfullPermill"/>
    /// </summary>
    public readonly int HashfullPermillApprox()
    {
        int items = 0;

        for (int i = 0; i < 1_000; ++i)
        {
            var bucket = (TranspositionTableElement*)&_buckets[i];

            for (int j = 0; j < Constants.TranspositionTableEntriesPerBucket; ++j)
            {
                if (bucket[j].Key != default)
                {
                    ++items;
                }
            }
        }

        //Console.WriteLine($"Real: {HashfullPermill(transpositionTable)}, estimated: {items}");
        return items / Constants.TranspositionTableEntriesPerBucket;
    }

    /// <summary>
    /// Number of buckets that fit in <paramref name="sizeMBs"/>
    /// </summary>
    internal static nuint CalculateLength(int sizeMBs)
    {
        nuint ttEntrySize = (nuint)sizeof(TranspositionTableElement);
        nuint ttBucketSize = (nuint)sizeof(TranspositionTableBucket);

        nuint sizeBytes = (nuint)sizeMBs * 1024 * 1024;
        nuint ttLength = sizeBytes / ttBucketSize;
        double ttLengthMB = (double)(ttLength * ttBucketSize) / (1024 * 1024);

        _logger.Info("Hash value:\t{0} MB", sizeMBs);
        _logger.Info("TT memory:\t{0} MB", ttLengthMB.ToString("F"));
        _logger.Info("TT length:\t{0} buckets, {1} entries", ttLength, ttLength * Constants.TranspositionTableEntriesPerBucket);
        _logger.Info("TT bucket:\t{0} bytes, {1} entries", ttBucketSize, Constants.TranspositionTableEntriesPerBucket);
        _logger.Info("TT entry:\t{0} bytes", ttEntrySize);

        return ttLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ulong PopulatedItemsCount()
    {
        ulong items = 0;
        for (nuint i = 0; i < Length; ++i)
        {
            var bucket = (TranspositionTableElement*)&_buckets[i];

            for (int j = 0; j < Constants.TranspositionTableEntriesPerBucket; ++j)
            {
                if (bucket[j].Key != default)
                {
                    ++items;
                }
            }
        }

        return items;
    }

    [Obsolete("Only tests")]
    internal TranspositionTableElement* Get(int bucketIndex, int entryIndex = 0)
    {
        Debug.Assert((uint)entryIndex < Constants.TranspositionTableEntriesPerBucket);

        return (TranspositionTableElement*)&_buckets[bucketIndex] + entryIndex;
    }

    [Conditional("DEBUG")]
    private void Stats()
    {
        _logger.Info("TT Occupancy:\t{0}% ({1}MB)",
            100 * PopulatedItemsCount() / (Length * Constants.TranspositionTableEntriesPerBucket),
            Length * (nuint)sizeof(TranspositionTableBucket) / 1024 / 1024);
    }

    [Conditional("DEBUG")]
    private readonly void Print()
    {
        Console.WriteLine("Transposition table content:");
        for (nuint i = 0; i < Length; ++i)
        {
            var bucket = (TranspositionTableElement*)&_buckets[i];

            for (int j = 0; j < Constants.TranspositionTableEntriesPerBucket; ++j)
            {
                ref var entry = ref bucket[j];

                if (entry.Key != default)
                {
                    Console.WriteLine($"{i}.{j}: Key = {entry.Key}, Depth: {entry.Depth}, Score: {entry.Score}, Move: {(entry.Move != 0 ? ((Move)entry.Move).UCIString() : "-")} {entry.Type}, Age: {entry.Age}");
                }
            }
        }
        Console.WriteLine("");
    }

    public void Release()
    {
        if (_nativeMemoryPointer != null)
        {
            NativeMemory.AlignedFree(_nativeMemoryPointer);
        }
    }
}