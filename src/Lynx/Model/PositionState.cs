using System.Runtime.CompilerServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

partial class Position
{
    private sealed class State
    {
        public ulong UniqueIdentifier
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public ulong KingPawnUniqueIdentifier
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

#pragma warning disable S3887 // Mutable, non-private fields should not be "readonly"
        public ulong[] NonPawnHash
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
#pragma warning restore S3887 // Mutable, non-private fields should not be "readonly"

        public ulong MinorHash
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public ulong MajorHash
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public int IncrementalEvalAccumulator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public int IncrementalPhaseAccumulator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public BoardSquare EnPassant
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        } = BoardSquare.noSquare;

        public byte Castle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        /// <summary>
        /// We save it so that current move doesn't affect 'sibling' moves exploration
        /// </summary>
        public bool IsIncrementalEval
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

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
