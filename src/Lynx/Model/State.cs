using System.Runtime.CompilerServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

partial class Position
{
    private sealed class State
    {
        public ulong UniqueIdentifier { get; set; }
        public ulong KingPawnUniqueIdentifier { get; set; }
#pragma warning disable S3887 // Mutable, non-private fields should not be "readonly"
        public ulong[] NonPawnHash { get; set; }
#pragma warning restore S3887 // Mutable, non-private fields should not be "readonly"
        public ulong MinorHash { get; set; }
        public ulong MajorHash { get; set; }

        public int IncrementalEvalAccumulator { get; set; }
        public int IncrementalPhaseAccumulator { get; set; }

        public BoardSquare EnPassant { get; set; } = BoardSquare.noSquare;

        public byte Castle { get; set; }

        /// <summary>
        /// We save it so that current move doesn't affect 'sibling' moves exploration
        /// </summary>
        public bool IsIncrementalEval;

        public State()
        {
            NonPawnHash = new ulong[2];
        }

        public State(State original)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetupFromPrevious(State previous)
        {
            UniqueIdentifier = previous.UniqueIdentifier;
            KingPawnUniqueIdentifier = previous.KingPawnUniqueIdentifier;
            NonPawnHash[(int)Side.White] = previous.NonPawnHash[(int)Side.White];
            NonPawnHash[(int)Side.Black] = previous.NonPawnHash[(int)Side.Black];
            MinorHash = previous.MinorHash;
            MajorHash = previous.MajorHash;

            IncrementalEvalAccumulator = previous.IncrementalEvalAccumulator;
            IncrementalPhaseAccumulator = previous.IncrementalPhaseAccumulator;

            Castle = previous.Castle;

            IsIncrementalEval = previous.IsIncrementalEval;
        }
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
