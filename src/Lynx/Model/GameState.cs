namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct GameState
{
    public readonly ulong ZobristKey;

    public readonly ulong KingPawnKey;

    public readonly int IncremetalEvalAccumulator;

    public readonly int KingPawnStructureEval;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public readonly bool IsIncrementalEval;

    public readonly bool IsSameKingPawnStructure;

    public GameState(
        ulong zobristKey, ulong kingPawnKey,
        int incrementalEvalAccumulator, int kingPawnStructureEval,
        BoardSquare enpassant, byte castle,
        bool isIncrementalEval, bool isSameKingPawnStructure)
    {
        ZobristKey = zobristKey;
        KingPawnKey = kingPawnKey;
        IncremetalEvalAccumulator = incrementalEvalAccumulator;
        KingPawnStructureEval = kingPawnStructureEval;
        EnPassant = enpassant;
        Castle = castle;
        IsIncrementalEval = isIncrementalEval;
        IsSameKingPawnStructure = isSameKingPawnStructure;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
