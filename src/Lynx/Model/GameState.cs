namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct GameState
{
    public readonly ulong ZobristKey;

    public readonly ulong KingPawnKey;

    public readonly int IncremetalEvalAccumulator;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public readonly bool IsIncrementalEval;

    /// <summary>
    /// Null moves
    /// </summary>
    /// <param name="zobristKey"></param>
    /// <param name="enpassant"></param>
    public GameState(ulong zobristKey, BoardSquare enpassant)
    {
        ZobristKey = zobristKey;
        EnPassant = enpassant;
    }

    public GameState(ulong zobristKey, ulong kingPawnKey, int incrementalEvalAccumulator, BoardSquare enpassant, byte castle, bool isIncrementalEval)
    {
        ZobristKey = zobristKey;
        KingPawnKey = kingPawnKey;
        IncremetalEvalAccumulator = incrementalEvalAccumulator;
        EnPassant = enpassant;
        Castle = castle;
        IsIncrementalEval = isIncrementalEval;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
