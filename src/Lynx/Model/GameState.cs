namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct GameState
{
    public readonly ulong ZobristKey;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public GameState(ulong zobristKey, BoardSquare enpassant, byte castle)
    {
        ZobristKey = zobristKey;
        EnPassant = enpassant;
        Castle = castle;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
