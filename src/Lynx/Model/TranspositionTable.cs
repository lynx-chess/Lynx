using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Model;
public readonly struct TranspositionTable
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly TranspositionTableElement[] _tt = [];

#pragma warning disable CA1051 // Do not declare visible instance fields
    public readonly int Size;
#pragma warning restore CA1051 // Do not declare visible instance fields

    public TranspositionTable()
    {
        Size = Configuration.EngineSettings.TranspositionTableSize;

        var ttLength = CalculateLength(Size);
        _tt = GC.AllocateArray<TranspositionTableElement>(ttLength, pinned: true);
    }

    public void Clear()
    {
        Array.Clear(_tt);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrefetchTTEntry(Position position)
    {
        if (Sse.IsSupported)
        {
            var index = CalculateTTIndex(position.UniqueIdentifier);

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
    public readonly ulong CalculateTTIndex(ulong positionUniqueIdentifier) => (ulong)(((UInt128)positionUniqueIdentifier * (UInt128)_tt.Length) >> 64);

    /// <summary>
    /// Checks the transposition table and, if there's a eval value that can be deducted from it of there's a previously recorded <paramref name="position"/>, it's returned. <see cref="EvaluationConstants.NoHashEntry"/> is returned otherwise
    /// </summary>
    /// <param name="ply">Ply</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Score, ShortMove BestMove, NodeType NodeType, int StaticEval, int Depth, bool WasPv) ProbeHash(Position position, int ply)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier);
        var entry = _tt[ttIndex];

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
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier);
        ref var entry = ref _tt[ttIndex];

        //if (entry.Key != default && entry.Key != position.UniqueIdentifier)
        //{
        //    _logger.Warn("TT collision");
        //}

        bool shouldReplace =
            entry.Key == 0                                      // No actual entry
            || (position.UniqueIdentifier >> 48) != entry.Key   // Different key: collision
            || nodeType == NodeType.Exact                       // Entering PV data
            || depth
                //+ Configuration.EngineSettings.TTReplacement_DepthOffset
                + (Configuration.EngineSettings.TTReplacement_TTPVDepthOffset * (wasPv ? 1 : 0)) >= entry.Depth;           // Higher depth

        if (!shouldReplace)
        {
            return;
        }

        // We want to store the distance to the checkmate position relative to the current node, independently from the root
        // If the evaluated score is a checkmate in 8 and we're at depth 5, we want to store checkmate value in 3
        var recalculatedScore = RecalculateMateScores(score, -ply);

        entry.Update(position.UniqueIdentifier, recalculatedScore, staticEval, depth, nodeType, wasPvInt, move);
    }

    /// <summary>
    /// Exact TT occupancy per mill
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int HashfullPermill() => _tt.Length > 0
        ? (int)(1000L * PopulatedItemsCount() / _tt.LongLength)
        : 0;

    /// <summary>
    /// Orders of magnitude faster than <see cref="HashfullPermill(TranspositionTableElement[])"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int HashfullPermillApprox()
    {
        int items = 0;

        if (_tt.Length >= 1_000)
        {
            for (int i = 0; i < 1_000; ++i)
            {
                if (_tt[i].Key != default)
                {
                    ++items;
                }
            }
        }

        //Console.WriteLine($"Real: {HashfullPermill(transpositionTable)}, estimated: {items}");
        return items;
    }

    internal static int CalculateLength(int size)
    {
        var ttEntrySize = TranspositionTableElement.Size;

        ulong sizeBytes = (ulong)size * 1024ul * 1024ul;
        ulong ttLength = sizeBytes / ttEntrySize;
        var ttLengthMb = (double)ttLength / 1024 / 1024;

        if (ttLength > (ulong)Array.MaxLength)
        {
            throw new ArgumentException($"Invalid transpositon table (Hash) size: {ttLengthMb}Mb, {ttLength} values (> Array.MaxLength, {Array.MaxLength})");
        }

        _logger.Info("Hash value:\t{0} MB", size);
        _logger.Info("TT memory:\t{0} MB", (ttLengthMb * ttEntrySize).ToString("F"));
        _logger.Info("TT length:\t{0} items", ttLength);
        _logger.Info("TT entry:\t{0} bytes", ttEntrySize);

        return (int)ttLength;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly int PopulatedItemsCount()
    {
        int items = 0;
        for (int i = 0; i < _tt.Length; ++i)
        {
            if (_tt[i].Key != default)
            {
                ++items;
            }
        }

        return items;
    }

    [Conditional("DEBUG")]
    private void Stats()
    {
        int items = 0;
        for (int i = 0; i < _tt.Length; ++i)
        {
            if (_tt[i].Key != default)
            {
                ++items;
            }
        }
        _logger.Info("TT Occupancy:\t{0}% ({1}MB)",
            100 * PopulatedItemsCount() / _tt.Length,
            _tt.Length * Marshal.SizeOf<TranspositionTableElement>() / 1024 / 1024);
    }

    [Conditional("DEBUG")]
    private readonly void Print()
    {
        Console.WriteLine("Transposition table content:");
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
