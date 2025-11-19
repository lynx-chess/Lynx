namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public struct PositionState
{
    public ulong UniqueIdentifier;
    public ulong KingPawnUniqueIdentifier;
#pragma warning disable S3887 // Mutable, non-private fields should not be "readonly"
    public readonly ulong[] NonPawnHash;
#pragma warning restore S3887 // Mutable, non-private fields should not be "readonly"
    public ulong MinorHash;
    public ulong MajorHash;

    public int IncrementalEvalAccumulator;
    public int IncrementalPhaseAccumulator;

    public BoardSquare EnPassant;

    public byte Castle;

    /// <summary>
    /// We save it so that current move doesn't affect 'sibling' moves exploration
    /// </summary>
    public bool IsIncrementalEval;

    public PositionState()
    {
        NonPawnHash = new ulong[2];
    }

    public PositionState(PositionState original)
    {
        UniqueIdentifier = original.UniqueIdentifier;
        KingPawnUniqueIdentifier = original.KingPawnUniqueIdentifier;
        NonPawnHash = [original.NonPawnHash[0], original.NonPawnHash[1]];
        MinorHash = original.MinorHash;
        MajorHash = original.MajorHash;
        IncrementalEvalAccumulator = original.IncrementalEvalAccumulator;
        IncrementalPhaseAccumulator = original.IncrementalPhaseAccumulator;
        EnPassant = original.EnPassant;
        Castle = original.Castle;
        IsIncrementalEval = original.IsIncrementalEval;
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
