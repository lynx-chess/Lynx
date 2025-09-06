namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly ref struct CastlingData
{
    public readonly int WhiteKingSideRook;
    public readonly int WhiteQueenSideRook;
    public readonly int BlackKingSideRook;
    public readonly int BlackQueenSideRook;

    public CastlingData(
        int whiteKingSideRook,
        int whiteQueenSideRook,
        int blackKingSideRook,
        int blackQueenSideRook)
    {
        WhiteKingSideRook = whiteKingSideRook;
        WhiteQueenSideRook = whiteQueenSideRook;
        BlackKingSideRook = blackKingSideRook;
        BlackQueenSideRook = blackQueenSideRook;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
