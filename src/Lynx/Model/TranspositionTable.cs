using System.Runtime.InteropServices;

namespace Lynx.Model;

public enum NodeType
{
    Unknown,    // Making it 0 instead of -1 because of default struct initialization
    Exact,
    Alpha,
    Beta
}

public struct TranspositionTableElement
{
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
    public int Score { get; set; }

    /// <summary>
    /// Best move found in a position. May not exist if the position failed low (score <= alpha)
    /// </summary>
    public Move? Move { get; set; }

    /// <summary>
    /// How deep the recorded search went
    /// </summary>
    public int Depth { get; set; }

    public void Clear()
    {
        Key = default;
        Score = default;
        Depth = default;
        Move = null;
        Type = NodeType.Unknown;
    }
}

public static class TranspositionTableExtensions
{
    public static int TranspositionTableArrayLength => Configuration.Hash / Marshal.SizeOf(typeof(TranspositionTableElement));

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
    /// <param name="transpositionTable"></param>
    /// <param name="position"></param>
    /// <param name="maxDepth"></param>
    /// <param name="depth">Ply</param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    public static int ProbeHash(this TranspositionTable transpositionTable, Position position, int maxDepth, int depth, int alpha, int beta)
    {
        var entry = transpositionTable[TranspositionTableIndex(position, transpositionTable)];

        if (position.UniqueIdentifier != entry.Key)
        {
            return EvaluationConstants.NoHashEntry;
        }

        if (entry.Depth >= maxDepth)
        {
            // We want to translate the checkmate position relative to the saved node to our root position from which we're searching
            // If the recorded score is a checkmate in 3 and we are at depth 5, we want to read checkmate in 8
            var score = RecalculateMateScores(entry.Score, depth);

            return entry.Type switch
            {
                NodeType.Exact => score,
                NodeType.Alpha when score <= alpha => alpha,
                NodeType.Beta when score >= beta => beta,
                _ => EvaluationConstants.NoHashEntry
            };
        }
        else
        {
            // TODO return only best move candidate?
        }

        return EvaluationConstants.NoHashEntry;
    }

    /// <summary>
    /// Adds a <see cref="TranspositionTableElement"/> to the transposition tabke
    /// </summary>
    /// <param name="transpositionTable"></param>
    /// <param name="position"></param>
    /// <param name="maxDepth"></param>
    /// <param name="ply">Ply</param>
    /// <param name="move"></param>
    /// <param name="eval"></param>
    /// <param name="nodeType"></param>
    public static void RecordHash(this TranspositionTable transpositionTable, Position position, int maxDepth, int ply, Move? move, int eval, NodeType nodeType)
    {
        ref var entry = ref transpositionTable[TranspositionTableIndex(position, transpositionTable)];

        // We want to store the distance to the checkmate position relative to the current node, independently from the root
        // If the evaluated score is a checkmate in 8 and we're at depth 5, we want to store checkmate value in 3
        var score = RecalculateMateScores(eval, -ply);

        entry.Key = position.UniqueIdentifier;
        entry.Score = score;
        entry.Depth = maxDepth;
        entry.Move = move;
        entry.Type = nodeType;
    }

    internal static int TranspositionTableIndex(Position position, TranspositionTable transpositionTable) =>
        (int)(position.UniqueIdentifier % transpositionTable.Length);

    /// <summary>
    /// If playing side is giving checkmate, decrease checkmate score (increase n in checkmate in n moves) due to being searching at a given depth already when this position is found.
    /// The opposite if the playing side is getting checkmated.
    /// Logic for when to pass +depth or -depth for the desired effect in https://www.talkchess.com/forum3/viewtopic.php?f=7&t=74411 and https://talkchess.com/forum3/viewtopic.php?p=861852#p861852
    /// </summary>
    /// <param name="score"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    internal static int RecalculateMateScores(int score, int depth) => score +
        score switch
        {
            > EvaluationConstants.PositiveCheckmateDetectionLimit => -EvaluationConstants.DepthCheckmateFactor * depth,
            < EvaluationConstants.NegativeCheckmateDetectionLimit => EvaluationConstants.DepthCheckmateFactor * depth,
            _ => 0
        };

    internal static void Print(this TranspositionTable transpositionTable)
    {
        Console.WriteLine("Transposition table:");
        for (int i = 0; i < transpositionTable.Length; ++i)
        {
            if (transpositionTable[i].Key != default)
            {
                Console.WriteLine($"{i}: Key = {transpositionTable[i].Key}, Depth: {transpositionTable[i].Depth}, Score: {transpositionTable[i].Score}, Move: {transpositionTable[i].Move?.ToMoveString() ?? "-"} {transpositionTable[i].Type}");
            }
        }
        Console.WriteLine("");
    }
}
