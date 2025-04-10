using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Model;
public readonly struct TranspositionTable
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly ulong _totalTTLength;
    private readonly int _ttArrayCount;
    private readonly TranspositionTableElement[][] _tt = [];

#pragma warning disable CA1051 // Do not declare visible instance fields
    public readonly int Size;
#pragma warning restore CA1051 // Do not declare visible instance fields

    public ulong Length => _totalTTLength;

    public TranspositionTable()
    {
        Size = Configuration.EngineSettings.TranspositionTableSize;

        var ttLength = CalculateLength(Size);
        _totalTTLength = ttLength;

        ulong fullArrayCount = (ttLength / (ulong)Array.MaxLength);
        int itemsLeft = (int)(ttLength % (ulong)Array.MaxLength);

        var totalArrayCount = fullArrayCount
            + (itemsLeft == 0
            ? 0UL
            : 1UL);

        if (totalArrayCount > (ulong)Array.MaxLength)
        {
            var ttLengthGB = (double)ttLength / 1024 / 1024 / 1024;
            throw new ArgumentException($"Invalid transpositon table (Hash) size: {ttLengthGB}GB, {ttLength} values (> Array.MaxLength, {Array.MaxLength})");
        }

        _ttArrayCount = (int)totalArrayCount;

        _tt = GC.AllocateArray<TranspositionTableElement[]>(_ttArrayCount, pinned: true);
        for (int i = 0; i < (int)fullArrayCount; ++i)
        {
            _tt[i] = GC.AllocateArray<TranspositionTableElement>(Array.MaxLength, pinned: true);
        }

        if (itemsLeft != 0)
        {
            _tt[_ttArrayCount - 1] = GC.AllocateArray<TranspositionTableElement>(itemsLeft, pinned: true);
        }
    }

    /// <summary>
    /// Multithreaded clearing of the transposition table
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        _logger.Debug("Clearing TT");
        var sw = Stopwatch.StartNew();

        // TODO: better division of work, it's probably better
        // not to go from sub-tt into sub-tt
        foreach (var tt in _tt)
        {
            var ttLength = tt.Length;
            var threadCount = Configuration.EngineSettings.Threads;
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

        _logger.Info("TT clearing time:\t{0} ms", sw.ElapsedMilliseconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrefetchTTEntry(Position position)
    {
        if (Sse.IsSupported)
        {
            (var ttIndex, var entryIndex) = CalculateTTIndexes(position.UniqueIdentifier);

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
    public readonly ulong CalculateTTIndex(ulong positionUniqueIdentifier) => (ulong)(((UInt128)positionUniqueIdentifier * (UInt128)_totalTTLength) >> 64);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly (int, int) CalculateTTIndexes(ulong positionUniqueIdentifier)
    {
        var globalIndex = CalculateTTIndex(positionUniqueIdentifier);

        var ttIndex = (int)(globalIndex / (ulong)Array.MaxLength);
        var itemIndex = (int)(globalIndex % (ulong)Array.MaxLength);

        return (ttIndex, itemIndex);
    }

    /// <summary>
    /// Checks the transposition table and, if there's a eval value that can be deducted from it of there's a previously recorded <paramref name="position"/>, it's returned. <see cref="EvaluationConstants.NoHashEntry"/> is returned otherwise
    /// </summary>
    /// <param name="ply">Ply</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Score, ShortMove BestMove, NodeType NodeType, int StaticEval, int Depth, bool WasPv) ProbeHash(Position position, int ply)
    {
        (var ttIndex, var entryIndex) = CalculateTTIndexes(position.UniqueIdentifier);
        var entry = _tt[ttIndex][entryIndex];

        if ((ushort)position.UniqueIdentifier != entry.Key)
        {
            return (EvaluationConstants.NoHashEntry, default, default, EvaluationConstants.NoHashEntry, default, default);
        }

        // We want to translate the checkmate position relative to the saved node to our root position from which we're searching
        // If the recorded score is a checkmate in 3 and we are at depth 5, we want to read checkmate in 8
        var recalculatedScore = RecalculateMateScores(entry.Score, ply);

        return (recalculatedScore, entry.Move, entry.Type, entry.StaticEval, entry.Depth, entry.WasPv);
    }

    /// <summary>
    /// Adds a <see cref="TranspositionTableElement"/> to the transposition tabke
    /// </summary>
    /// <param name="ply">Ply</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RecordHash(Position position, int staticEval, int depth, int ply, int score, NodeType nodeType, bool wasPv, Move? move = null)
    {
        (var ttIndex, var entryIndex) = CalculateTTIndexes(position.UniqueIdentifier);
        ref var entry = ref _tt[ttIndex][entryIndex];

        //if (entry.Key != default && entry.Key != position.UniqueIdentifier)
        //{
        //    _logger.Warn("TT collision");
        //}

        var wasPvInt = wasPv ? 1 : 0;

        bool shouldReplace =
            entry.Key == 0                                      // No actual entry
            || (position.UniqueIdentifier >> 48) != entry.Key   // Different key: collision
            || nodeType == NodeType.Exact                       // Entering PV data
            || depth
                //+ Configuration.EngineSettings.TTReplacement_DepthOffset
                + (Configuration.EngineSettings.TTReplacement_TTPVDepthOffset * wasPvInt) >= entry.Depth    // Higher depth
                ;
        // || entry.Type == NodeType.None not needed, since depth condition is always true for those cases

        if (!shouldReplace)
        {
            return;
        }

        // We want to store the distance to the checkmate position relative to the current node, independently from the root
        // If the evaluated score is a checkmate in 8 and we're at depth 5, we want to store checkmate value in 3
        var recalculatedScore = RecalculateMateScores(score, -ply);

        entry.Update(position.UniqueIdentifier, recalculatedScore, staticEval, depth, nodeType, wasPvInt, move);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SaveStaticEval(Position position, int staticEval, bool wasPv)
    {
        (var ttIndex, var entryIndex) = CalculateTTIndexes(position.UniqueIdentifier);
        ref var entry = ref _tt[ttIndex][entryIndex];

        // Extra key checks here (right before saving) failed for MT in https://github.com/lynx-chess/Lynx/pull/1566
        entry.Update(position.UniqueIdentifier, EvaluationConstants.NoHashEntry, staticEval, depth: -1, NodeType.None, wasPv ? 1 : 0, null);
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

        if (ttLength > (ulong)Array.MaxLength)
        {
            _logger.Info($"More than one TT array will be used for transpositon table (Hash) size: {ttLengthMB * ttEntrySize / 1024}GB, {ttLength} values (> Array.MaxLength, {Array.MaxLength})");
        }

        _logger.Info("Hash value:\t{0} MB", size);
        _logger.Info("TT memory:\t{0} MB", (ttLengthMB * ttEntrySize).ToString("F"));
        _logger.Info("TT length:\t{0} items", ttLength);
        _logger.Info("TT entry:\t{0} bytes", ttEntrySize);

        return ttLength;
    }

    /// <summary>
    /// If playing side is giving checkmate, decrease checkmate score (increase n in checkmate in n moves) due to being searching at a given depth already when this position is found.
    /// The opposite if the playing side is getting checkmated.
    /// Logic for when to pass +depth or -depth for the desired effect in https://www.talkchess.com/forum3/viewtopic.php?f=7&t=74411 and https://talkchess.com/forum3/viewtopic.php?p=861852#p861852
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int RecalculateMateScores(int score, int ply)
    {
        if (score > EvaluationConstants.PositiveCheckmateDetectionLimit)
        {
            return score - ply;
        }
        else if (score < EvaluationConstants.NegativeCheckmateDetectionLimit)
        {
            return score + ply;
        }

        return score;
    }

¡   [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            100 * PopulatedItemsCount() / (ulong)_tt.Length,
            _tt.Length * Marshal.SizeOf<TranspositionTableElement>() / 1024 / 1024);
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
                    Console.WriteLine($"{i}: Key = {entry.Key}, Depth: {entry.Depth}, Score: entry.Score}, Move: {(entry.Move != 0 ? ((Move)entry.Move).UCIString() : " - ")} {_tt[i].Type}");
                }
            }
        }
        Console.WriteLine("");
    }
}
