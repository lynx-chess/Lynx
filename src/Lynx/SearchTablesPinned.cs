using System.Diagnostics;
using System.Runtime.CompilerServices;

using static Lynx.PinnedArrays;

namespace Lynx;

/// <summary>
/// Pinned copies of the tables ready during search and move ordering use in the hot paths, exposed as pointers.
/// This converts the address in a JIT constant, baked directly into the compiled code (JIT knows both the index and the length), avoiding bound checks
/// </summary>
public static unsafe class SearchTablesPinned
{
    public static readonly int DepthCount = EvaluationConstants.HistoryBonus.Length;

    public static readonly int* HistoryBonus = CopyToPinnedArray(EvaluationConstants.HistoryBonus);

    public static readonly int* HistoryMalus = CopyToPinnedArray(EvaluationConstants.HistoryMalus);

    public static readonly int* SEEPieceValues = CopyToPinnedArray(SEE.PieceValues.ToArray());

    private static readonly int* _lmrReductions =
        FlattenToPinnedArray(
            [.. EvaluationConstants.LMRReductions[0].Select(row => row ?? []),
                .. EvaluationConstants.LMRReductions[1].Select(row => row ?? [])],
            stride: Constants.MaxNumberOfPseudolegalMovesInAPosition);

    private static readonly int* _mostValuableVictimLeastValuableAttacker = FlattenToPinnedArray(EvaluationConstants.MostValuableVictimLeastValuableAttacker, stride: 12);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int QuietLMRReduction(int depth, int movesSearched)
    {
        Debug.Assert((uint)depth < (uint)DepthCount);
        Debug.Assert((uint)movesSearched < Constants.MaxNumberOfPseudolegalMovesInAPosition);

        return _lmrReductions[(depth * Constants.MaxNumberOfPseudolegalMovesInAPosition) + movesSearched];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NoisyLMRReduction(int depth, int movesSearched)
    {
        Debug.Assert((uint)depth < (uint)DepthCount);
        Debug.Assert((uint)movesSearched < Constants.MaxNumberOfPseudolegalMovesInAPosition);

        return _lmrReductions[((DepthCount + depth) * Constants.MaxNumberOfPseudolegalMovesInAPosition) + movesSearched];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MostValuableVictimLeastValuableAttacker(int piece, int capturedPiece)
    {
        Debug.Assert((uint)piece < 12);
        Debug.Assert((uint)capturedPiece < 12);

        return _mostValuableVictimLeastValuableAttacker[(piece * 12) + capturedPiece];
    }
}
