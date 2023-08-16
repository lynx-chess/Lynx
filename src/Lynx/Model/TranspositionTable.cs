using NLog;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

public enum NodeType : byte
{
    Unknown,    // Making it 0 instead of -1 because of default struct initialization
    Exact,
    Alpha,
    Beta
}

public struct TranspositionTableElement
{
    private byte _depth;
    //private byte _age;
    private short _score;

    /// <summary>
    /// Full Zobrist key
    /// </summary>
    public long Key { get; set; }

    /// <summary>
    /// Node (position) type:
    /// <see cref="NodeType.Exact"/>: == <see cref="Score"/>,
    /// <see cref="NodeType.Alpha"/>: &lt;= <see cref="Score"/>,
    /// <see cref="NodeType.Beta"/>: &gt;= <see cref="Score"/>
    /// </summary>
    public NodeType Type { get; set; }

    /// <summary>
    /// Position evaluation
    /// </summary>
    public int Score { readonly get => _score; set => _score = (short)value; }

    /// <summary>
    /// Best move found in a position. 0 if the position failed low (score <= alpha)
    /// </summary>
    public Move Move { get; set; }

    /// <summary>
    /// How deep the recorded search went. For us this numberis targetDepth - ply
    /// </summary>
    public int Depth { readonly get => _depth; set => _depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1))[0]; }

    //public int Age { readonly get => _age; set => _age = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1))[0]; }

    public void Clear()
    {
        Key = 0;
        Score = 0;
        Depth = 0;
        Move = 0;
        Type = NodeType.Unknown;
    }
}

public static class TranspositionTableExtensions
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static (int Length, int Mask) CalculateLength(int size)
    {
        var ttSize = (int)BitOperations.RoundUpToPowerOf2((uint)(size / Marshal.SizeOf(typeof(TranspositionTableElement))));

        _logger.Info("Choosing TT size of {0}MB for user selected {1}MB", ttSize / 1024 / 1024, size);
        _logger.Debug("TT size: ({0})MB, {1}", ttSize / 1024 / 1024, ttSize.ToString("X"));
        _logger.Debug("TT mask: {0}", (ttSize - 1).ToString("X"));

        return (ttSize, ttSize - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int TranspositionTableIndex(Position position, TranspositionTable transpositionTable) =>
        (int)(position.UniqueIdentifier & (transpositionTable.Length - 1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ClearTranspositionTable(this TranspositionTable transpositionTable)
    {
        foreach (var element in transpositionTable)
        {
            element.Clear();
        }
    }

    /// <summary>
    /// Checks the transposition table and, if there's a eval value that can be deducted from it of there's a previously recorded <paramref name="position"/>, it's returned. <see cref="EvaluationConstants.NoHashEntry"/> is returned otherwise
    /// </summary>
    /// <param name="tt"></param>
    /// <param name="position"></param>
    /// <param name="targetDepth"></param>
    /// <param name="ply">Ply</param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Evaluation, Move BestMove) ProbeHash(this TranspositionTable tt, int ttMask, Position position, int targetDepth, int ply, int alpha, int beta)
    {
        if (!Configuration.EngineSettings.TranspositionTableEnabled)
        {
            return (EvaluationConstants.NoHashEntry, default);
        }

        ref var entry = ref tt[position.UniqueIdentifier & ttMask];

        if (position.UniqueIdentifier != entry.Key)
        {
            return (EvaluationConstants.NoHashEntry, default);
        }

        var eval = EvaluationConstants.NoHashEntry;

        if (entry.Depth >= (targetDepth - ply))
        {
            // We want to translate the checkmate position relative to the saved node to our root position from which we're searching
            // If the recorded score is a checkmate in 3 and we are at depth 5, we want to read checkmate in 8
            var score = RecalculateMateScores(entry.Score, ply);

            eval = entry.Type switch
            {
                NodeType.Exact => score,
                NodeType.Alpha when score <= alpha => alpha,
                NodeType.Beta when score >= beta => beta,
                _ => EvaluationConstants.NoHashEntry
            };
        }

        return (eval, entry.Move);
    }

    /// <summary>
    /// Adds a <see cref="TranspositionTableElement"/> to the transposition tabke
    /// </summary>
    /// <param name="tt"></param>
    /// <param name="position"></param>
    /// <param name="targetDepth"></param>
    /// <param name="ply">Ply</param>
    /// <param name="eval"></param>
    /// <param name="nodeType"></param>
    /// <param name="move"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RecordHash(this TranspositionTable tt, int ttMask, Position position, int targetDepth, int ply, int eval, NodeType nodeType, Move? move = 0)
    {
        if (!Configuration.EngineSettings.TranspositionTableEnabled)
        {
            return;
        }

        ref var entry = ref tt[position.UniqueIdentifier & ttMask];

        //if (entry.Key != default && entry.Key != position.UniqueIdentifier)
        //{
        //    _logger.Warn("TT collision");
        //}

        // We want to store the distance to the checkmate position relative to the current node, independently from the root
        // If the evaluated score is a checkmate in 8 and we're at depth 5, we want to store checkmate value in 3
        var score = RecalculateMateScores(eval, -ply);

        entry.Key = position.UniqueIdentifier;
        entry.Score = score;
        entry.Depth = targetDepth - ply;
        entry.Move = move ?? 0;
        entry.Type = nodeType;
    }

    /// <summary>
    /// If playing side is giving checkmate, decrease checkmate score (increase n in checkmate in n moves) due to being searching at a given depth already when this position is found.
    /// The opposite if the playing side is getting checkmated.
    /// Logic for when to pass +depth or -depth for the desired effect in https://www.talkchess.com/forum3/viewtopic.php?f=7&t=74411 and https://talkchess.com/forum3/viewtopic.php?p=861852#p861852
    /// </summary>
    /// <param name="score"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int RecalculateMateScores(int score, int depth) => score +
            score switch
            {
                > EvaluationConstants.PositiveCheckmateDetectionLimit => -EvaluationConstants.CheckmateDepthFactor * depth,
                < EvaluationConstants.NegativeCheckmateDetectionLimit => EvaluationConstants.CheckmateDepthFactor * depth,
                _ => 0
            };

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
    /// TT occupancy per mill
    /// </summary>
    /// <param name="transpositionTable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HashfullPermill(this TranspositionTable transpositionTable) => transpositionTable.Length > 0
        ? (int)(1000L * transpositionTable.PopulatedItemsCount() / transpositionTable.LongLength)
        : 0;

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
                Console.WriteLine($"{i}: Key = {transpositionTable[i].Key}, Depth: {transpositionTable[i].Depth}, Score: {transpositionTable[i].Score}, Move: {(transpositionTable[i].Move != 0 ? transpositionTable[i].Move.ToMoveString() : "-")} {transpositionTable[i].Type}");
            }
        }
        Console.WriteLine("");
    }
}