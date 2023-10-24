namespace Lynx.Model;
public readonly struct GameState
{
    public readonly sbyte CapturedPiece;

    public readonly byte Castle;

    public readonly BoardSquare EnPassant;

    public readonly long ZobristKey;

    public GameState(sbyte capturedPiece, byte castle, BoardSquare enpassant, long zobristKey)
    {
        CapturedPiece = capturedPiece;
        Castle = castle;
        EnPassant = enpassant;
        ZobristKey = zobristKey;
    }
}
