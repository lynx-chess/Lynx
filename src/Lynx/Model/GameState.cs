namespace Lynx.Model;
public readonly struct GameState
{
    public readonly int CapturedPiece;

    public readonly int Castle;

    public readonly BoardSquare EnPassant;

    public readonly long ZobristKey;

    // TODO: save full Zobrist key?

    public GameState(int capturedPiece, int castle, BoardSquare enpassant, long zobristKey)
    {
        CapturedPiece = capturedPiece;
        Castle = castle;
        EnPassant = enpassant;
        ZobristKey = zobristKey;
    }
}