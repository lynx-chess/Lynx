namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly ref struct ParseFENResult
{
#pragma warning disable S3887 // Mutable, non-private fields should not be "readonly"
    public readonly BitBoard[] PieceBitBoards;
    public readonly BitBoard[] OccupancyBitBoards;
    public readonly int[] Board;
#pragma warning restore S3887 // Mutable, non-private fields should not be "readonly"

    public readonly int HalfMoveClock;
    //public readonly int FullMoveCounter;
    public readonly BoardSquare EnPassant;

    public readonly int WhiteKingSideRook;
    public readonly int WhiteQueenSideRook;
    public readonly int BlackKingSideRook;
    public readonly int BlackQueenSideRook;

    public readonly Side Side;
    public readonly byte Castle;

    public ParseFENResult(
        BitBoard[] pieceBitBoards,
        BitBoard[] occupancyBitBoards,
        int[] board,
        Side side,
        byte castle,
        BoardSquare enPassant,
        int whiteKingSideRook,
        int whiteQueenSideRook,
        int blackKingSideRook,
        int blackQueenSideRook,
        int halfMoveClock)
    {
        PieceBitBoards = pieceBitBoards;
        OccupancyBitBoards = occupancyBitBoards;
        Board = board;
        Side = side;
        Castle = castle;
        EnPassant = enPassant;

        WhiteKingSideRook = whiteKingSideRook;
        WhiteQueenSideRook = whiteQueenSideRook;
        BlackKingSideRook = blackKingSideRook;
        BlackQueenSideRook = blackQueenSideRook;

        HalfMoveClock = halfMoveClock;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
