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
    public readonly Side Side;
    public readonly byte Castle;

    public ParseFENResult(
        BitBoard[] pieceBitBoards,
        BitBoard[] occupancyBitBoards,
        int[] board,
        Side side,
        byte castle,
        BoardSquare enPassant,
        int halfMoveClock)
    {
        PieceBitBoards = pieceBitBoards;
        OccupancyBitBoards = occupancyBitBoards;
        Board = board;
        Side = side;
        Castle = castle;
        EnPassant = enPassant;
        HalfMoveClock = halfMoveClock;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
