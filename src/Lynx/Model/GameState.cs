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

    public readonly int IncrementalEvalAccumulator;

    public readonly int IncrementalPhaseAccumulator;

    public readonly int WhiteKing;

    public readonly int BlackKing;

    public readonly int WhiteKingBucket;

    public readonly int BlackKingBucket;

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

        WhiteKing = position.WhiteKing;
        BlackKing = position.BlackKing;
        WhiteKingBucket = position.WhiteKingBucket;
        BlackKingBucket = position.BlackKingBucket;

        // We also save a copy of _isIncrementalEval, so that current move doesn't affect 'sibling' moves exploration
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
