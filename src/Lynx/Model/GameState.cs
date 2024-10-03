namespace Lynx.Model;
public readonly struct GameState
{
    public readonly long ZobristKey;

    public readonly int IncrementalEvaluation;

    public readonly int IncrementalPhase;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public GameState(long zobristKey, int incrementalEvaluation, int incrementalPhase, BoardSquare enpassant, byte castle)
    {
        ZobristKey = zobristKey;
        IncrementalEvaluation = incrementalEvaluation;
        IncrementalPhase = incrementalPhase;
        EnPassant = enpassant;
        Castle = castle;
    }
}
