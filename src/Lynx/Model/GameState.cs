namespace Lynx.Model;
public readonly struct GameState(int capturedPiece, int castle, BoardSquare enpassant, long zobristKey)
{
    public readonly int CapturedPiece = capturedPiece;

    public readonly int Castle = castle;

    public readonly BoardSquare EnPassant = enpassant;

    public readonly long ZobristKey = zobristKey;
}
