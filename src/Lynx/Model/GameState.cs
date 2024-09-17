namespace Lynx.Model;
public readonly struct GameState
{
    public readonly long ZobristKey;

    public readonly int CapturedPiece;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public GameState(long zobristKey, int capturedPiece, BoardSquare enpassant, byte castle)
    {
        ZobristKey = zobristKey;
        CapturedPiece = capturedPiece;
        EnPassant = enpassant;
        Castle = castle;
    }
}
