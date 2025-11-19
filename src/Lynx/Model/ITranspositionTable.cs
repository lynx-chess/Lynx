using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

/// <summary>
/// Interface for transposition table implementations
/// </summary>
public interface ITranspositionTable
//public readonly struct ITranspositionTable
{
    /// <summary>
    /// Size of the transposition table in MB
    /// </summary>
    int SizeMBs { get; }

    /// <summary>
    /// Length of the transposition table (total number of entries)
    /// </summary>
    ulong Length { get; }

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
    bool ProbeHash(Position position, int halfMovesWithoutCaptureOrPawnMove, int ply, out TTProbeResult result)
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
    void RecordHash(Position position, int halfMovesWithoutCaptureOrPawnMove, int staticEval, int depth, int ply, int score, NodeType nodeType, bool wasPv, Move? move = null)
    {
        Debug.Assert(nodeType != NodeType.Alpha || move is null, "Assertion failed", "There's no 'best move' on fail-lows, so TT one won't be overriden");

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
    void SaveStaticEval(Position position, int halfMovesWithoutCaptureOrPawnMove, int staticEval, bool wasPv, NodeType nodeType = NodeType.Unknown)
    {
        ref var entry = ref GetTTEntry(position, halfMovesWithoutCaptureOrPawnMove);

        // Extra key checks here (right before saving) failed for MT in https://github.com/lynx-chess/Lynx/pull/1566
        entry.Update(GenerateTTKey(position.UniqueIdentifier), EvaluationConstants.NoScore, staticEval, depth: 0, nodeType, wasPv ? 1 : 0, null);
    }

    /// <summary>
    /// Multithreaded clearing of the transposition table
    /// </summary>
    void Clear();

    /// <summary>
    /// Prefetch transposition table entry for performance optimization
    /// </summary>
    void PrefetchTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove);

    /// <summary>
    /// Approximate transposition table occupancy per mill (0-1000)
    /// Orders of magnitude faster than HashfullPermill()
    /// </summary>
    /// <returns>Approximate occupancy in per mill</returns>
    int HashfullPermillApprox();

    /// <summary>
    /// Get a reference to a transposition table entry for the given position
    /// </summary>
    protected ref TranspositionTableElement GetTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove);

    /// <summary>
    /// Get a readonly reference to a transposition table entry for the given position
    /// </summary>
    protected ref readonly TranspositionTableElement GetTTEntryReadonly(Position position, int halfMovesWithoutCaptureOrPawnMove);

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
        if (score > EvaluationConstants.PositiveCheckmateDetectionLimit)
        {
            return score - ply;
        }
        else if (score < EvaluationConstants.NegativeCheckmateDetectionLimit && score != EvaluationConstants.NoScore)
        {
            return score + ply;
        }

        return score;
    }
}