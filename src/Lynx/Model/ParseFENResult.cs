using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ParseFENResult
{
    public readonly int HalfMoveClock;
    public readonly int FullMoveCounter;
    public readonly BoardSquare EnPassant;

    public readonly CastlingData CastlingData;

    public readonly Side Side;
    public readonly byte Castle;

    public ParseFENResult(
        Side side,
        byte castle,
        BoardSquare enPassant,
        CastlingData castlingData,
        int halfMoveClock,
        int fullMoveCounter)
    {
        Side = side;
        Castle = castle;
        EnPassant = enPassant;
        CastlingData = castlingData;
        HalfMoveClock = halfMoveClock;
        FullMoveCounter = fullMoveCounter;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
