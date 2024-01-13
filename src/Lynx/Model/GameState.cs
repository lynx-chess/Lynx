namespace Lynx.Model;
public readonly struct GameState
{
    public readonly long ZobristKey;

    public readonly byte Castle;

    public readonly BoardSquare EnPassant;

    public GameState(long zobristKey, byte castle, BoardSquare enpassant)
    {
        ZobristKey = zobristKey;
        Castle = castle;
        EnPassant = enpassant;
    }
}
