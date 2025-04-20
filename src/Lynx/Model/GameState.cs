using System.Buffers;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct GameState : IDisposable
{
    public readonly ulong ZobristKey;

    public readonly ulong KingPawnKey;

    public readonly ulong NonPawnWhiteKey;

    public readonly ulong NonPawnBlackKey;

    #region PieceKeys

    public readonly ulong[] PieceKey;

    #endregion

    public readonly int IncremetalEvalAccumulator;

    public readonly int IncrementalPhaseAccumulator;

    public readonly BoardSquare EnPassant;

    public readonly byte Castle;

    public readonly bool IsIncrementalEval;

    public GameState(ulong zobristKey, ulong kingPawnKey, ulong nonPawnWhiteKey, ulong nonPawnBlackKey, ulong[] pieceKey,
        int incrementalEvalAccumulator, int incrementalPhaseAccumulator, BoardSquare enpassant, byte castle, bool isIncrementalEval)
    {
        ZobristKey = zobristKey;
        KingPawnKey = kingPawnKey;
        NonPawnWhiteKey = nonPawnWhiteKey;
        NonPawnBlackKey = nonPawnBlackKey;
        PieceKey = ArrayPool<ulong>.Shared.Rent(12);
        Array.Copy(pieceKey, PieceKey, 12);

        IncremetalEvalAccumulator = incrementalEvalAccumulator;
        IncrementalPhaseAccumulator = incrementalPhaseAccumulator;
        EnPassant = enpassant;
        Castle = castle;
        IsIncrementalEval = isIncrementalEval;
    }

    public void Dispose()
    {
        ArrayPool<BitBoard>.Shared.Return(PieceKey, clearArray: true);
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
