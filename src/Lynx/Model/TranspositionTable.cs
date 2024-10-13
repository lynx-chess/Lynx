using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

public enum NodeType : byte
{
    Unknown,    // Making it 0 instead of -1 because of default struct initialization
    Exact,
    /// <summary>
    /// Upper bound
    /// </summary>
    Alpha,
    /// <summary>
    /// Lower bound
    /// </summary>
    Beta
}

public struct TranspositionTableElement
{
    private ushort _key;

    private ShortMove _move;

    private short _score;

    private short _staticEval;

    private byte _depth;

    private NodeType _type;

    /// <summary>
    /// 16 MSB of Position's Zobrist key
    /// </summary>
    public readonly ushort Key => _key;

    /// <summary>
    /// Best move found in the position. 0 if the search failed low (score <= alpha)
    /// </summary>
    public readonly ShortMove Move => _move;

    /// <summary>
    /// Position's score
    /// </summary>
    public readonly int Score => _score;

    /// <summary>
    /// Position's static evaluation
    /// </summary>
    public readonly int StaticEval => _staticEval;

    /// <summary>
    /// How deep the recorded search went. For us this numberis targetDepth - ply
    /// </summary>
    public readonly int Depth => _depth;

    /// <summary>
    /// Node (position) type:
    /// <see cref="NodeType.Exact"/>: == <see cref="Score"/>,
    /// <see cref="NodeType.Alpha"/>: &lt;= <see cref="Score"/>,
    /// <see cref="NodeType.Beta"/>: &gt;= <see cref="Score"/>
    /// </summary>
    public readonly NodeType Type => _type;

    /// <summary>
    /// Struct size in bytes
    /// </summary>
    public static ulong Size => (ulong)Marshal.SizeOf(typeof(TranspositionTableElement));

    public void Update(ulong key, int score, int staticEval, int depth, NodeType nodeType, Move? move)
    {
        _key = (ushort)key;
        _score = (short)score;
        _staticEval = (short)staticEval;
        _depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref depth, 1))[0];
        _type = nodeType;
        _move = move != null ? (ShortMove)move : Move;    // Suggested by cj5716 instead of 0. https://github.com/lynx-chess/Lynx/pull/462
    }
}

public static class TranspositionTableExtensions
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static int CalculateLength(int size)
    {
        var ttEntrySize = TranspositionTableElement.Size;

        ulong sizeBytes = (ulong)size * 1024ul * 1024ul;
        ulong ttLength = sizeBytes / ttEntrySize;
        var ttLengthMb = (double)ttLength / 1024 / 1024;

        if (ttLength > (ulong)Array.MaxLength)
        {
            throw new ArgumentException($"Invalid transpositon table (Hash) size: {ttLengthMb}Mb, {ttLength} values (> Array.MaxLength, {Array.MaxLength})");
        }

        var mask = ttLength - 1;

        _logger.Info("Hash value:\t{0} MB", size);
        _logger.Info("TT memory:\t{0} MB", (ttLengthMb * ttEntrySize).ToString("F"));
        _logger.Info("TT length:\t{0} items", ttLength);
        _logger.Info("TT entry:\t{0} bytes", ttEntrySize);
        _logger.Info("TT mask:\t{0}", mask.ToString("X"));

        return (int)ttLength;
    }

    /// <summary>
    /// 'Fixed-point multiplication trick', see https://lemire.me/blog/2016/06/27/a-fast-alternative-to-the-modulo-reduction/
    /// </summary>
    /// <param name="positionUniqueIdentifier"></param>
    /// <param name="ttLength"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong CalculateTTIndex(ulong positionUniqueIdentifier, int ttLength) => (ulong)(((UInt128)positionUniqueIdentifier * (UInt128)ttLength) >> 64);

    /// <summary>
    /// Checks the transposition table and, if there's a eval value that can be deducted from it of there's a previously recorded <paramref name="position"/>, it's returned. <see cref="EvaluationConstants.NoHashEntry"/> is returned otherwise
    /// </summary>
    /// <param name="tt"></param>
    /// <param name="position"></param>
    /// <param name="depth"></param>
    /// <param name="ply">Ply</param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Score, ShortMove BestMove, NodeType NodeType, int RawScore, int StaticEval, int EntryDepth) ProbeHash(this TranspositionTable tt, Position position, int depth, int ply, int alpha, int beta)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, tt.Length);
        ref var entry = ref tt[ttIndex];

        if ((ushort)position.UniqueIdentifier != entry.Key)
        {
            return (EvaluationConstants.NoHashEntry, default, default, default, default, default);
        }

        var type = entry.Type;
        var rawScore = entry.Score;
        var score = EvaluationConstants.NoHashEntry;

        if (entry.Depth >= depth)
        {
            var recalculatedScore = RecalculateMateScores(rawScore, ply);

            if (type == NodeType.Exact
                || (type == NodeType.Alpha && recalculatedScore <= alpha)
                || (type == NodeType.Beta && recalculatedScore >= beta))
            {
                // We want to translate the checkmate position relative to the saved node to our root position from which we're searching
                // If the recorded score is a checkmate in 3 and we are at depth 5, we want to read checkmate in 8
                score = recalculatedScore;
            }
        }

        return (score, entry.Move, entry.Type, rawScore, entry.StaticEval, entry.Depth);
    }

    /// <summary>
    /// Adds a <see cref="TranspositionTableElement"/> to the transposition tabke
    /// </summary>
    /// <param name="tt"></param>
    /// <param name="position"></param>
    /// <param name="depth"></param>
    /// <param name="ply">Ply</param>
    /// <param name="score"></param>
    /// <param name="nodeType"></param>
    /// <param name="move"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RecordHash(this TranspositionTable tt, Position position, int staticEval, int depth, int ply, int score, NodeType nodeType, Move? move = null)
    {
        var ttIndex = CalculateTTIndex(position.UniqueIdentifier, tt.Length);
        ref var entry = ref tt[ttIndex];

        //if (entry.Key != default && entry.Key != position.UniqueIdentifier)
        //{
        //    _logger.Warn("TT collision");
        //}

        bool shouldReplace =
            entry.Key == 0                                      // No actual entry
            || (position.UniqueIdentifier >> 48) != entry.Key   // Different key: collision
            || nodeType == NodeType.Exact                       // Entering PV data
            || depth >= entry.Depth;                            // Higher depth

        if (!shouldReplace)
        {
            return;
        }

        // We want to store the distance to the checkmate position relative to the current node, independently from the root
        // If the evaluated score is a checkmate in 8 and we're at depth 5, we want to store checkmate value in 3
        var recalculatedScore = RecalculateMateScores(score, -ply);

        entry.Update(position.UniqueIdentifier, recalculatedScore, staticEval, depth, nodeType, move);
    }

    /// <summary>
    /// If playing side is giving checkmate, decrease checkmate score (increase n in checkmate in n moves) due to being searching at a given depth already when this position is found.
    /// The opposite if the playing side is getting checkmated.
    /// Logic for when to pass +depth or -depth for the desired effect in https://www.talkchess.com/forum3/viewtopic.php?f=7&t=74411 and https://talkchess.com/forum3/viewtopic.php?p=861852#p861852
    /// </summary>
    /// <param name="score"></param>
    /// <param name="ply"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int RecalculateMateScores(int score, int ply)
    {
        if (score > EvaluationConstants.PositiveCheckmateDetectionLimit)
        {
            return score - (EvaluationConstants.CheckmateDepthFactor * ply);
        }
        else if (score < EvaluationConstants.NegativeCheckmateDetectionLimit)
        {
            return score + (EvaluationConstants.CheckmateDepthFactor * ply);
        }

        return score;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopulatedItemsCount(this TranspositionTable transpositionTable)
    {
        int items = 0;
        for (int i = 0; i < transpositionTable.Length; ++i)
        {
            if (transpositionTable[i].Key != default)
            {
                ++items;
            }
        }

        return items;
    }

    /// <summary>
    /// Exact TT occupancy per mill
    /// </summary>
    /// <param name="transpositionTable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HashfullPermill(this TranspositionTable transpositionTable) => transpositionTable.Length > 0
        ? (int)(1000L * transpositionTable.PopulatedItemsCount() / transpositionTable.LongLength)
        : 0;

    /// <summary>
    /// Orders of magnitude faster than <see cref="HashfullPermill(TranspositionTableElement[])"/>
    /// </summary>
    /// <param name="transpositionTable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HashfullPermillApprox(this TranspositionTable transpositionTable)
    {
        int items = 0;
        for (int i = 0; i < 1000; ++i)
        {
            if (transpositionTable[i].Key != default)
            {
                ++items;
            }
        }

        //Console.WriteLine($"Real: {HashfullPermill(transpositionTable)}, estimated: {items}");
        return items;
    }

    [Conditional("DEBUG")]
    internal static void Stats(this TranspositionTable transpositionTable)
    {
        int items = 0;
        for (int i = 0; i < transpositionTable.Length; ++i)
        {
            if (transpositionTable[i].Key != default)
            {
                ++items;
            }
        }
        _logger.Info("TT Occupancy:\t{0}% ({1}MB)",
            100 * transpositionTable.PopulatedItemsCount() / transpositionTable.Length,
            transpositionTable.Length * Marshal.SizeOf(typeof(TranspositionTableElement)) / 1024 / 1024);
    }

    [Conditional("DEBUG")]
    internal static void Print(this TranspositionTable transpositionTable)
    {
        Console.WriteLine("Transposition table content:");
        for (int i = 0; i < transpositionTable.Length; ++i)
        {
            if (transpositionTable[i].Key != default)
            {
                Console.WriteLine($"{i}: Key = {transpositionTable[i].Key}, Depth: {transpositionTable[i].Depth}, Score: {transpositionTable[i].Score}, Move: {(transpositionTable[i].Move != 0 ? ((Move)transpositionTable[i].Move).UCIString() : "-")} {transpositionTable[i].Type}");
            }
        }
        Console.WriteLine("");
    }
}