namespace Lynx.Model;
public readonly struct GameState
{
    public readonly long ZobristKey;

    public readonly byte Castle;

    public readonly sbyte CapturedPiece;

    public readonly BoardSquare EnPassant;

    public GameState(sbyte capturedPiece, byte castle, BoardSquare enpassant, long zobristKey)
    {
        CapturedPiece = capturedPiece;
        Castle = castle;
        EnPassant = enpassant;
        ZobristKey = zobristKey;
    }
}
