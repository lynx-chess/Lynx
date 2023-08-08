namespace Lynx.Model;
public readonly struct GameState
{
    public readonly int CapturedPiece;

    public readonly byte Castle;

    public readonly BoardSquare EnPassant;

    public readonly long ZobristKey;

    // TODO: save full Zobrist key?

    public GameState(int capturedPiece, byte castle, BoardSquare enpassant, long zobristKey)
    {
        CapturedPiece = capturedPiece;
        Castle = castle;
        EnPassant = enpassant;
        ZobristKey = zobristKey;
    }
}
