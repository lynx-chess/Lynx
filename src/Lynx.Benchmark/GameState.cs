using Lynx.Model;

namespace Lynx.Benchmark;

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
        IncrementalEvalAccumulator = position._state.IncrementalEvalAccumulator;
        IncrementalPhaseAccumulator = position._state.IncrementalPhaseAccumulator;

        // We also save a copy of _isIncrementalEval, so that current move doesn't affect 'sibling' moves exploration
        IsIncrementalEval = position._state.IsIncrementalEval;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
