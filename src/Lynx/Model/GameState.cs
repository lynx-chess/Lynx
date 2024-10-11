namespace Lynx.Model;
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
