namespace Lynx.Model;
public readonly struct GameState(sbyte capturedPiece, byte castle, BoardSquare enpassant, long zobristKey)
{
    public readonly sbyte CapturedPiece = capturedPiece;

    public readonly byte Castle = castle;

    public readonly BoardSquare EnPassant = enpassant;

    public readonly long ZobristKey = zobristKey;
}
