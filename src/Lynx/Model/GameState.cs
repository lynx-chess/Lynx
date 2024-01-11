namespace Lynx.Model;
public readonly struct GameState
{
    public readonly long ZobristKey;

    public readonly byte Castle;

    public readonly int CapturedPiece;

    public readonly BoardSquare EnPassant;

    public GameState(long zobristKey, int capturedPiece, byte castle, BoardSquare enpassant)
    {
        CapturedPiece = capturedPiece;
        Castle = castle;
        EnPassant = enpassant;
        ZobristKey = zobristKey;
    }
}
