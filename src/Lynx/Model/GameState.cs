namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct GameState
{
    public readonly ulong ZobristKey;

    public readonly ulong KingPawnKey;

    public readonly ulong NonPawnWhiteKey;

    public readonly ulong NonPawnBlackKey;

    public readonly ulong MinorKey;

    public readonly BitBoard WhitePawnAttacks;
    public readonly BitBoard BlackPawnAttacks;

    public readonly BitBoard WhiteKnightAttacks;
    public readonly BitBoard BlackKnightAttacks;

    public readonly BitBoard WhiteBishopAttacks;
    public readonly BitBoard BlackBishopAttacks;

    public readonly BitBoard WhiteRookAttacks;
    public readonly BitBoard BlackRookAttacks;

    public readonly BitBoard WhiteQueenAttacks;
    public readonly BitBoard BlackQueenAttacks;

    public readonly BitBoard WhiteKingAttacks;
    public readonly BitBoard BlackKingAttacks;

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

        var attacks = position._attacks;

        WhitePawnAttacks = attacks[(int)Piece.P];
        WhiteKnightAttacks = attacks[(int)Piece.N];
        WhiteBishopAttacks = attacks[(int)Piece.B];
        WhiteRookAttacks = attacks[(int)Piece.R];
        WhiteQueenAttacks = attacks[(int)Piece.Q];
        WhiteKingAttacks = attacks[(int)Piece.K];

        BlackPawnAttacks = attacks[(int)Piece.p];
        BlackKnightAttacks = attacks[(int)Piece.n];
        BlackBishopAttacks = attacks[(int)Piece.b];
        BlackRookAttacks = attacks[(int)Piece.r];
        BlackQueenAttacks = attacks[(int)Piece.q];
        BlackKingAttacks = attacks[(int)Piece.k];

        EnPassant = position.EnPassant;
        Castle = position.Castle;
        IncrementalEvalAccumulator = position._incrementalEvalAccumulator;
        IncrementalPhaseAccumulator = position._incrementalPhaseAccumulator;

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
