namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly ref struct CastlingData
{
    public readonly ulong WhiteKingsideFreeSquares;
    public readonly ulong WhiteQueensideFreeSquares;
    public readonly ulong BlackKingsideFreeSquares;
    public readonly ulong BlackQueensideFreeSquares;

    public readonly ulong WhiteKingsideNonAttackedSquares;
    public readonly ulong WhiteQueensideNonAttackedSquares;
    public readonly ulong BlackKingsideNonAttackedSquares;
    public readonly ulong BlackQueensideNonAttackedSquares;

    public readonly int WhiteKingsideRook;
    public readonly int WhiteQueensideRook;
    public readonly int BlackKingsideRook;
    public readonly int BlackQueensideRook;

    public CastlingData(
        int whiteKingsideRook,
        int whiteQueensideRook,
        int blackKingsideRook,
        int blackQueensideRook,

        ulong whiteKingsideFreeSquares,
        ulong whiteQueensideFreeSquares,
        ulong blackKingsideFreeSquares,
        ulong blackQueensideFreeSquares,

        ulong whiteKingsideNonAttackedSquares,
        ulong whiteQueensideNonAttackedSquares,
        ulong blackKingsideNonAttackedSquares,
        ulong blackQueensideNonAttackedSquares)
    {
        WhiteKingsideRook = whiteKingsideRook;
        WhiteQueensideRook = whiteQueensideRook;
        BlackKingsideRook = blackKingsideRook;
        BlackQueensideRook = blackQueensideRook;

        WhiteKingsideFreeSquares = whiteKingsideFreeSquares;
        WhiteQueensideFreeSquares = whiteQueensideFreeSquares;
        BlackKingsideFreeSquares = blackKingsideFreeSquares;
        BlackQueensideFreeSquares = blackQueensideFreeSquares;

        WhiteKingsideNonAttackedSquares = whiteKingsideNonAttackedSquares;
        WhiteQueensideNonAttackedSquares = whiteQueensideNonAttackedSquares;
        BlackKingsideNonAttackedSquares = blackKingsideNonAttackedSquares;
        BlackQueensideNonAttackedSquares = blackQueensideNonAttackedSquares;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
