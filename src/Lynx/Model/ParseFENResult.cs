using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ParseFENResult
{
#pragma warning disable S3887 // Mutable, non-private fields should not be "readonly"
    public readonly Bitboard[] PieceBitboards;
    public readonly Bitboard[] OccupancyBitboards;
    public readonly int[] Board;
#pragma warning restore S3887 // Mutable, non-private fields should not be "readonly"

    public readonly int HalfMoveClock;
    //public readonly int FullMoveCounter;
    public readonly BoardSquare EnPassant;

    public readonly CastlingData CastlingData;

    public readonly Side Side;
    public readonly byte Castle;

    public ParseFENResult(
        Bitboard[] pieceBitboards,
        Bitboard[] occupancyBitboards,
        int[] board,
        Side side,
        byte castle,
        BoardSquare enPassant,
        CastlingData castlingData,
        int halfMoveClock)
    {
        PieceBitboards = pieceBitboards;
        OccupancyBitboards = occupancyBitboards;
        Board = board;
        Side = side;
        Castle = castle;
        EnPassant = enPassant;
        CastlingData = castlingData;
        HalfMoveClock = halfMoveClock;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
