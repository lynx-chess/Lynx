namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct GameState
{
    public readonly ulong ZobristKey;

    public readonly ulong KingPawnKey;

    public readonly ulong NonPawnWhiteKey;

    public readonly ulong NonPawnBlackKey;

    public readonly ulong MinorKey;

    public readonly ulong MajorKey;

    public readonly int IncremetalEvalAccumulator;

    public readonly int IncrementalPhaseAccumulator;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public readonly bool IsIncrementalEval;

    public GameState(Position position)
    {
        ZobristKey = position.UniqueIdentifier;

        KingPawnKey = position.KingPawnUniqueIdentifier;
        NonPawnWhiteKey = position.NonPawnHash[(int)Side.White];
        NonPawnBlackKey = position.NonPawnHash[(int)Side.Black];
        MinorKey = position.MinorHash;
        MajorKey = position.MajorHash;

        EnPassant = position.EnPassant;
        Castle = position.Castle;
        IncrementalEvalAccumulator = position._incrementalEvalAccumulator;
        IncrementalPhaseAccumulator = position._incrementalPhaseAccumulator;
        IsIncrementalEval = position._isIncrementalEval;
    }
}

public readonly struct NullMoveGameState
{
    public readonly ulong ZobristKey;

    public readonly BoardSquare EnPassant;

    public NullMoveGameState(Position position)
    {
        ZobristKey = position.UniqueIdentifier;
        EnPassant = position.EnPassant;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
