using Microsoft.Extensions.ObjectPool;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public sealed class GameState
{
    public ulong ZobristKey;
    public ulong KingPawnKey;
    public ulong NonPawnWhiteKey;
    public ulong NonPawnBlackKey;
    public ulong MinorKey;
    public ulong MajorKey;
    public int IncrementalEvalAccumulator;
    public int IncrementalPhaseAccumulator;
    public BoardSquare EnPassant;
    public byte Castle;
    public bool IsIncrementalEval;

    public void Populate(Position position)
    {
        ZobristKey = position.UniqueIdentifier;

        KingPawnKey = position.KingPawnUniqueIdentifier;
        NonPawnWhiteKey = position.NonPawnHash[(int)Side.White];
        NonPawnBlackKey = position.NonPawnHash[(int)Side.Black];
        MinorKey = position.MinorHash;
        MajorKey = position.MajorHash;

        EnPassant = position.EnPassant;
        Castle = position.Castle;
        IncrementalEvalAccumulator = position.IncrementalEvalAccumulator;
        IncrementalPhaseAccumulator = position.IncrementalPhaseAccumulator;

        // We also save a copy of _isIncrementalEval, so that current move doesn't affect 'sibling' moves exploration
        IsIncrementalEval = position.IsIncrementalEval;
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
