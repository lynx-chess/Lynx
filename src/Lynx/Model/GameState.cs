namespace Lynx.Model;
public readonly struct GameState
{
    public readonly long ZobristKey;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public GameState(long zobristKey, BoardSquare enpassant, byte castle)
    {
        ZobristKey = zobristKey;
        EnPassant = enpassant;
        Castle = castle;
    }
}
