using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Lynx.TunableEvalParameters;

namespace Lynx;

public static class EvaluationPSQTs
{
    public const int PSQTBucketCount = 23;

#pragma warning disable S4663 // Comments should not be empty - comes from C++
    public static ReadOnlySpan<int> PSQTBucketLayout =>
    [
        15, 16, 17, 18, 19, 20, 21, 22,
        15, 16, 17, 18, 19, 20, 21, 22,
        15, 16, 17, 18, 19, 20, 21, 22,
        15, 16, 17, 18, 19, 20, 21, 22,
        15, 8, 9, 10, 11, 12, 13, 14,
        15, 8, 9, 10, 11, 12, 13, 14,
        0, 8, 9, 10, 11, 12, 13, 14,
        0, 1, 2, 3, 4, 5, 6, 7, //
    ];
#pragma warning restore S4663 // Comments should not be empty

    /// <summary>
    /// 12 x 64
    /// </summary>
    internal static readonly int[] _packedPSQT = GC.AllocateArray<int>(12 * 64, pinned: true);

    static EvaluationPSQTs()
    {
        short[][] mgPositionalTables =
        [
            MiddleGamePawnTable,
            MiddleGameKnightTable,
            MiddleGameBishopTable,
            MiddleGameRookTable,
            MiddleGameQueenTable,
            MiddleGameKingTable
        ];

        short[][] egPositionalTables =
        [
            EndGamePawnTable,
            EndGameKnightTable,
            EndGameBishopTable,
            EndGameRookTable,
            EndGameQueenTable,
            EndGameKingTable
        ];

        for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
        {
            for (int sq = 0; sq < 64; ++sq)
            {
                ref var whitePieceEntry = ref PSQT(piece, sq);
                ref var blackPieceEntry = ref PSQT(piece + 6, sq);

                whitePieceEntry = Utils.Pack(
                    (short)(MiddleGamePieceValues[piece] + mgPositionalTables[piece][sq]),
                    (short)(EndGamePieceValues[piece] + egPositionalTables[piece][sq]));

                blackPieceEntry = Utils.Pack(
                    (short)(MiddleGamePieceValues[piece + 6] - mgPositionalTables[piece][sq ^ 56]),
                    (short)(EndGamePieceValues[piece + 6] - egPositionalTables[piece][sq ^ 56]));
            }
        }
    }

    /// <summary>
    /// [12][64]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref int PSQT(int piece, int square)
    {
        const int pieceOffset = 64;

        var index = (piece * pieceOffset)
            + square;

        Debug.Assert(index >= 0 && index < _packedPSQT.Length);

        return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_packedPSQT), index);
    }
}
