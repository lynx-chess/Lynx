namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct GameState
{
    public readonly ulong ZobristKey;

    public readonly ulong PawnKey;

    public readonly int IncremetalEvalAccumulator;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public readonly bool IsIncrementalEval;

    public GameState(ulong zobristKey, ulong pawnKey, int incrementalEvalAccumulator, BoardSquare enpassant, byte castle, bool isIncrementalEval)
    {
        ZobristKey = zobristKey;
        PawnKey = pawnKey;
        IncremetalEvalAccumulator = incrementalEvalAccumulator;
        EnPassant = enpassant;
        Castle = castle;
        IsIncrementalEval = isIncrementalEval;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
