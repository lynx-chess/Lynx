using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Model;

/// <summary>
/// Single array transposition table implementation (from main branch)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct TranspositionTable
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly TranspositionTableElement[] _tt = [];
    public int SizeMBs { get; }

    public ulong Length => (ulong)_tt.Length;

    public TranspositionTable()
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
                _tt = GC.AllocateArray<TranspositionTableElement>((int)ttLength, pinned: true);
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
        ref readonly var entry = ref GetTTEntryReadonly(position, halfMovesWithoutCaptureOrPawnMove);

        var key = GenerateTTKey(position.UniqueIdentifier);

        if (key != entry.Key)
        {
            result = default;
            return false;
        }

        // We want to translate the checkmate position relative to the saved node to our root position from which we're searching
        // If the recorded score is a checkmate in 3 and we are at depth 5, we want to read checkmate in 8
        var recalculatedScore = RecalculateMateScores(entry.Score, ply);

        result = new TTProbeResult(recalculatedScore, entry.Move, entry.Type, entry.StaticEval, entry.Depth, entry.WasPv);

        return true;
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

        ref var entry = ref GetTTEntry(position, halfMovesWithoutCaptureOrPawnMove);

        var newKey = GenerateTTKey(position.UniqueIdentifier);

        var wasPvInt = wasPv ? 1 : 0;

        bool shouldReplace =
            entry.Key != newKey                 // Different key: collision or no actual entry
            || nodeType == NodeType.Exact       // Entering PV data
            || depth                            // Higher depth
                    + Configuration.EngineSettings.TTReplacement_DepthOffset
                    + (Configuration.EngineSettings.TTReplacement_TTPVDepthOffset * wasPvInt)
                >= entry.Depth;

        if (!shouldReplace)
        {
            return;
        }

        // We want to store the distance to the checkmate position relative to the current node, independently from the root
        // If the evaluated score is a checkmate in 8 and we're at depth 5, we want to store checkmate value in 3
        var recalculatedScore = RecalculateMateScores(score, -ply);

        entry.Update(newKey, recalculatedScore, staticEval, depth, nodeType, wasPvInt, move);
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
        ref var entry = ref GetTTEntry(position, halfMovesWithoutCaptureOrPawnMove);

        // Extra key checks here (right before saving) failed for MT in https://github.com/lynx-chess/Lynx/pull/1566
        entry.Update(GenerateTTKey(position.UniqueIdentifier), EvaluationConstants.NoScore, staticEval, depth: 0, NodeType.Unknown, wasPv ? 1 : 0, move: null);
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

        return Bmi2.X64.IsSupported
            ? Bmi2.X64.MultiplyNoFlags(key, (ulong)_tt.Length)
            : (ulong)(((UInt128)key * (UInt128)_tt.Length) >> 64);
    }

#pragma warning disable S4144 // Methods should not have identical implementations

    /// <summary>
    /// Get a reference to a transposition table entry for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref TranspositionTableElement GetTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return ref _tt[ttIndex];
    }

    /// <summary>
    /// Get a readonly reference to a transposition table entry for the given position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref readonly TranspositionTableElement GetTTEntryReadonly(Position position, int halfMovesWithoutCaptureOrPawnMove)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);
        return ref _tt[ttIndex];
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
    static int RecalculateMateScores(int score, int ply)
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
            throw new ConfigurationException($"Invalid TT Hash size: {ttLengthMB} MB, {ttLength} values (> Array.MaxLength, {Array.MaxLength})");
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