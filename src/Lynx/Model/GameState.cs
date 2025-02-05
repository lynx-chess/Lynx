namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct GameState
{
    public readonly ulong ZobristKey;

    public readonly ulong KingPawnKey;

    public readonly int IncremetalEvalAccumulator;

    public readonly int IncrementalAdditionalEvalAccumulator;

    public readonly int IncrementalPhaseAccumulator;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public readonly bool IsIncrementalEval;

    public readonly bool IsIncrementalAdditionalEval;

    public GameState(ulong zobristKey, ulong kingPawnKey,
        int incrementalEvalAccumulator, int incrementalAdditionalEvalAccumulator, int incrementalPhaseAccumulator,
        BoardSquare enpassant, byte castle, bool isIncrementalEval, bool isIncrementalAdditionalEval)
    {
        ZobristKey = zobristKey;
        KingPawnKey = kingPawnKey;
        IncremetalEvalAccumulator = incrementalEvalAccumulator;
        IncrementalAdditionalEvalAccumulator = incrementalAdditionalEvalAccumulator;
        IncrementalPhaseAccumulator = incrementalPhaseAccumulator;
        EnPassant = enpassant;
        Castle = castle;
        IsIncrementalEval = isIncrementalEval;
        IsIncrementalAdditionalEval = isIncrementalAdditionalEval;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
