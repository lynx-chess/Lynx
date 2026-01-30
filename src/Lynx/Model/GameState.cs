using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[StructLayout(LayoutKind.Sequential)]
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
        IncrementalEvalAccumulator = position.IncrementalEvalAccumulator;
        IncrementalPhaseAccumulator = position.IncrementalPhaseAccumulator;

        // We also save a copy of _isIncrementalEval, so that current move doesn't affect 'sibling' moves exploration
        IsIncrementalEval = position.IsIncrementalEval;
    }
}

[StructLayout(LayoutKind.Sequential)]
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
