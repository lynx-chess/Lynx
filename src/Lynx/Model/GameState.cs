namespace Lynx.Model;
public readonly struct GameState
{
    public readonly long ZobristKey;

    public readonly BoardSquare EnPassant;

    public readonly bool Quiet;

    public readonly byte Castle;

    public GameState(long zobristKey, BoardSquare enpassant, byte castle)
    {
        ZobristKey = zobristKey;
        EnPassant = enpassant;
        Castle = castle;
        Quiet = false;
    }

    public GameState(long zobristKey, BoardSquare enpassant, bool isQuiet, byte castle)
    {
        ZobristKey = zobristKey;
        EnPassant = enpassant;
        Castle = castle;
        Quiet = isQuiet;
    }
}
