using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

/// <summary>
/// Interface for transposition table implementations
/// </summary>
public interface ITranspositionTable
//public readonly struct ITranspositionTable
{
    int Age { get; protected set; }

    /// <summary>
    /// Request size of the transposition table in MB
    /// </summary>
    int RequestedSizeMBs { get; }

    /// <summary>
    /// Size of the transposition table in MB
    /// </summary>
    int SizeMBs { get; }

    /// <summary>
    /// Length of the transposition table (total number of entries)
    /// </summary>
    ulong Length { get; }

    void BumpAge()
    {
        // Circular buffer
        Age = (Age + 1) % TranspositionTableElement.MaxAge;
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
    bool ProbeHash(Position position, int halfMovesWithoutCaptureOrPawnMove, int ply, out TTProbeResult result)
    {
        ref readonly var bucketEntry = ref GetTTEntryReadonly(position, halfMovesWithoutCaptureOrPawnMove);

        unsafe
        {
            fixed (TranspositionTableBucket* bucketPointer = &bucketEntry)
            {
                var bucket = (TranspositionTableElement*)bucketPointer;
                var key = GenerateTTKey(position.UniqueIdentifier);

                // We simply take the first entry
                for (int i = 0; i < Constants.TranspositionTableElementsPerBucket; ++i)
                {
                    ref var entry = ref bucket[i];

                    if (key == entry.Key)
                    {
                        // We want to translate the checkmate position relative to the saved node to our root position from which we're searching
                        // If the recorded score is a checkmate in 3 and we are at depth 5, we want to read checkmate in 8
                        var recalculatedScore = RecalculateMateScores(entry.Score, ply);

                        result = new TTProbeResult(recalculatedScore, entry.Move, entry.Type, entry.StaticEval, entry.Depth, entry.WasPv);

                        return true;
                    }
                }
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
    void RecordHash(Position position, int halfMovesWithoutCaptureOrPawnMove, int staticEval, int depth, int ply, int score, NodeType nodeType, bool wasPv, Move? move = null)
    {
        Debug.Assert(nodeType != NodeType.Alpha || move is null, "Assertion failed", "There's no 'best move' on fail-lows, so TT one won't be overridden");

        //var ttIndex = CalculateTTIndex(position.UniqueIdentifier, halfMovesWithoutCaptureOrPawnMove);

        ref TranspositionTableBucket bucketEntry = ref GetTTEntry(position, halfMovesWithoutCaptureOrPawnMove);

        unsafe
        {
            fixed (TranspositionTableBucket* bucketPointer = &bucketEntry)
            {
                var bucket = (TranspositionTableElement*)bucketPointer;

                ref TranspositionTableElement entry = ref bucket[0];

                var newKey = GenerateTTKey(position.UniqueIdentifier);

#if DEBUG
                int bucketIndex = 0;
#endif

                if (entry.Key != newKey && entry.Key != 0)
                {
                    int minValue = CalculateBucketEntryWeight(entry, Age);

                    for (int i = 1; i < Constants.TranspositionTableElementsPerBucket; ++i)
                    {
                        ref var candidateEntry = ref bucket[i];

                        // Bucket policy to discard very old entries

                        // Always take an empty entry, or one that corresponds to the same position
                        if (candidateEntry.Key == newKey || candidateEntry.Key == 0)
                        {
                            entry = ref candidateEntry;

#if DEBUG
                            bucketIndex = i;
#endif

                            break;
                        }

                        // Otherwise, take the entry with the lowest weight (calculated based on depth and age), aka the 'worst' entry
                        var value = CalculateBucketEntryWeight(candidateEntry, Age);

                        if (value < minValue)
                        {
                            minValue = value;
                            entry = ref candidateEntry;

#if DEBUG
                            bucketIndex = i;
#endif
                        }
                    }
                }

                var wasPvInt = wasPv ? 1 : 0;

                // This calculation allows to account for the circular buffer, i.e. with Age being back to 0 and entry.Age being 29, delta is +3 instead of -3
                // Comparing ageDelta > 0 is equivalent to Age != entry.Age, but it also allows more detailed comparisons
                var ageDelta = (Age - entry.Age + TranspositionTableElement.MaxAge + 1) & TranspositionTableElement.MaxAge;

                bool shouldReplace =
                    entry.Key != newKey                                                         // Different key: collision or no actual entry
                    || nodeType == NodeType.Exact                                               // Entering PV data
                    || ageDelta > Configuration.EngineSettings.TTReplacement_AgeOffset          // High age diff
                    || depth                                                                    // Higher depth
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

                entry.Update(newKey, recalculatedScore, staticEval, depth, nodeType, wasPvInt, move, Age);


                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                static int CalculateBucketEntryWeight(TranspositionTableElement entry, int ttAge)
                {
                    // This calculation allows to account for the circular buffer, i.e. with Age being back to 0 and entry.Age being 29, delta is +3 instead of -3
                    // Comparing ageDelta > 0 is equivalent to Age != entry.Age, but it also allows more detailed comparisons
                    var ageDelta = (ttAge - entry.Age + TranspositionTableElement.MaxAge + 1) & TranspositionTableElement.MaxAge;

                    // depth - age formula from SP
                    return entry.Depth - (2 * ageDelta);
                }
            }
        }
    }

    /// <summary>
    /// Save only the static evaluation for a position in the transposition table
    /// </summary>
    /// <param name="position">The position to save static eval for</param>
    /// <param name="halfMovesWithoutCaptureOrPawnMove">Half moves without capture or pawn move counter</param>
    /// <param name="staticEval">Static evaluation of the position</param>
    /// <param name="wasPv">Whether this position was part of the principal variation</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void SaveStaticEval(Position position, int halfMovesWithoutCaptureOrPawnMove, int staticEval, bool wasPv)
    {
        ref var bucketEntry = ref GetTTEntry(position, halfMovesWithoutCaptureOrPawnMove);

        unsafe
        {
            fixed (TranspositionTableBucket* bucketPointer = &bucketEntry)
            {
                var bucket = (TranspositionTableElement*)bucketPointer;
                ref TranspositionTableElement firstEntry = ref bucket[0];

                // Extra key checks here (right before saving) failed for MT in https://github.com/lynx-chess/Lynx/pull/1566
                firstEntry.Update(GenerateTTKey(position.UniqueIdentifier), EvaluationConstants.NoScore, staticEval, depth: 0, NodeType.Unknown, wasPv ? 1 : 0, move: null, Age);
            }
        }
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
    protected ref TranspositionTableBucket GetTTEntry(Position position, int halfMovesWithoutCaptureOrPawnMove);

    /// <summary>
    /// Get a readonly reference to a transposition table entry for the given position
    /// </summary>
    protected ref readonly TranspositionTableBucket GetTTEntryReadonly(Position position, int halfMovesWithoutCaptureOrPawnMove);

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
}