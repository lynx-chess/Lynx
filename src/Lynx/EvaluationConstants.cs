/*
 * PSQT based on https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
*/

using Lynx.Model;

namespace Lynx;

public static class EvaluationConstants
{
    /// <summary>
    /// 30k games, 16+0.16, UHO_XXL_+0.90_+1.19.epd
    /// Retained (W,D,L) = (791773, 1127929, 793778) positions.
    /// </summary>
    public const int EvalNormalizationCoefficient = 78;

    public static readonly double[] As = [-44.54789428, 284.90322556, -305.65458204, 143.86777995];

    public static readonly double[] Bs = [-21.08101051, 127.81742295, -160.22340655, 128.53122955];

    public static readonly int[] MiddleGamePieceValues =
    [
            +49, +270, +255, +347, +790, 0,
            -49, -270, -255, -347, -790, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +165, +346, +320, +644, +1119, 0,
            -165, -346, -320, -644, -1119, 0
    ];

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 7, 15, 14, 23, 42, 46, 9,
            2, 5, 21, 33, 39, 44, 41, 28,
            5, 12, 26, 39, 45, 48, 25, 22,
            6, 15, 26, 42, 46, 45, 24, 21,
            3, 10, 22, 29, 35, 39, 36, 24,
            0, 10, 11, 15, 26, 36, 37, 3,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -46, -48, -53, -67, -53, -56, -60, -63,
            -48, -47, -57, -67, -62, -62, -57, -63,
            -36, -42, -55, -72, -70, -66, -50, -57,
            -39, -42, -58, -69, -69, -63, -52, -58,
            -47, -49, -60, -65, -59, -59, -58, -64,
            -44, -48, -50, -69, -50, -52, -57, -60,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -106, -22, -36, -25, -11, -13, -14, -65,
            -30, -14, 4, 18, 20, 22, -3, -8,
            -19, 8, 24, 48, 53, 38, 34, 4,
            0, 28, 39, 53, 53, 53, 40, 21,
            3, 23, 41, 43, 51, 52, 40, 18,
            -17, 10, 22, 42, 51, 32, 28, 5,
            -27, -3, 10, 17, 17, 17, -1, -9,
            -118, -25, -35, -16, -9, -7, -20, -64,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -46, -30, 0, 2, 4, -11, -24, -59,
            -4, 8, 23, 17, 17, 16, 0, -4,
            5, 22, 36, 38, 38, 23, 15, -2,
            16, 26, 48, 48, 50, 45, 27, 7,
            17, 31, 45, 49, 50, 40, 31, 10,
            1, 25, 29, 44, 35, 24, 15, 1,
            -13, 10, 14, 20, 15, 14, -2, -5,
            -50, -30, 3, 1, 4, -6, -23, -53,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -19, 5, -12, -24, -17, -22, -19, 0,
            -1, -2, 0, -18, -3, -7, 17, -10,
            -10, 0, -7, -4, -11, 5, 2, 18,
            -7, -12, -11, 8, 7, -18, -3, -4,
            -15, -5, -16, 7, -3, -13, -9, 1,
            -2, -1, 0, -10, -2, -1, 3, 15,
            1, 7, 4, -9, -9, -6, 11, -4,
            5, 5, -2, -33, -19, -23, -10, -14,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -3, 19, -11, 9, 1, 8, 1, -24,
            1, 1, 1, 9, 6, -3, 3, -7,
            15, 17, 11, 9, 15, 10, 7, 9,
            14, 13, 12, 5, 4, 12, 8, 6,
            12, 12, 10, 8, -2, 7, 8, 6,
            15, 8, 5, 6, 10, 5, 6, 9,
            -7, -4, -9, 6, 6, 5, 5, -5,
            -3, -5, -8, 9, 11, 9, 4, -11,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            7, 1, 1, 5, 17, 8, 14, 7,
            -10, 3, 2, 3, 11, 12, 26, 9,
            -10, 1, 0, 5, 17, 23, 54, 34,
            -8, -1, 3, 6, 12, 17, 43, 32,
            -2, 2, 7, 14, 12, 21, 36, 31,
            -6, 2, 5, 13, 18, 29, 53, 38,
            -9, -7, 6, 6, 11, 12, 30, 11,
            9, 6, 7, 14, 23, 11, 20, 18,
    ];

    public static readonly int[] EndGameRookTable =
    [
            14, 16, 22, 13, 6, 14, 15, 9,
            20, 25, 25, 17, 9, 12, 7, 11,
            16, 14, 16, 12, 3, 1, -8, -9,
            18, 14, 18, 15, 6, 8, -2, -7,
            17, 15, 19, 10, 7, 2, -2, -4,
            17, 19, 11, 5, 1, -1, -8, -4,
            24, 26, 21, 14, 6, 10, 6, 14,
            10, 10, 17, 9, 1, 9, 7, 0,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -14, -16, -16, -2, -10, -27, 1, -2,
            3, -6, 6, -1, 3, 4, 15, 38,
            -2, -2, -4, -4, -6, 7, 28, 42,
            -5, -13, -9, -4, -6, -1, 13, 21,
            -6, -8, -11, -11, -5, 0, 7, 17,
            -2, -1, -8, -6, -2, 3, 17, 28,
            -7, -15, 3, 7, 5, 0, 9, 29,
            -10, -21, -11, 1, -7, -36, -15, 11,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -12, -3, 6, -2, 2, 3, -22, 7,
            -13, -8, -13, 4, 5, -7, -26, 2,
            -7, 2, 5, 5, 20, 20, -4, 12,
            -4, 11, 1, 13, 23, 30, 35, 28,
            6, 6, 12, 22, 20, 24, 25, 39,
            -6, -2, 14, 12, 16, 23, 18, 21,
            -5, 1, -9, -8, 1, 3, -21, 9,
            -1, 2, 5, -2, 9, 27, 19, 0,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            19, 27, 4, -74, -15, -64, 14, 32,
            -17, -23, -40, -70, -79, -59, -21, 2,
            -80, -74, -108, -110, -115, -123, -87, -91,
            -117, -116, -136, -166, -160, -149, -150, -173,
            -81, -97, -126, -154, -159, -129, -151, -157,
            -72, -45, -101, -110, -95, -103, -77, -81,
            55, -17, -38, -62, -63, -49, -8, 10,
            27, 45, 10, -65, -4, -54, 27, 43,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -58, -32, -11, 10, -20, 5, -22, -63,
            -8, 19, 29, 40, 46, 34, 17, -8,
            10, 40, 56, 65, 67, 61, 43, 25,
            19, 52, 72, 87, 83, 75, 64, 44,
            9, 45, 69, 83, 86, 73, 66, 41,
            8, 35, 55, 65, 62, 56, 43, 22,
            -29, 13, 28, 37, 39, 31, 13, -13,
            -67, -39, -14, 5, -17, 2, -27, -67,
    ];

    public static readonly int[] MiddleGamePawnTableBlack = MiddleGamePawnTable.Select((_, index) => -MiddleGamePawnTable[index ^ 56]).ToArray();
    public static readonly int[] EndGamePawnTableBlack = EndGamePawnTable.Select((_, index) => -EndGamePawnTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameKnightTableBlack = MiddleGameKnightTable.Select((_, index) => -MiddleGameKnightTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameKnightTableBlack = EndGameKnightTable.Select((_, index) => -EndGameKnightTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameBishopTableBlack = MiddleGameBishopTable.Select((_, index) => -MiddleGameBishopTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameBishopTableBlack = EndGameBishopTable.Select((_, index) => -EndGameBishopTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameRookTableBlack = MiddleGameRookTable.Select((_, index) => -MiddleGameRookTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameRookTableBlack = EndGameRookTable.Select((_, index) => -EndGameRookTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameQueenTableBlack = MiddleGameQueenTable.Select((_, index) => -MiddleGameQueenTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameQueenTableBlack = EndGameQueenTable.Select((_, index) => -EndGameQueenTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameKingTableBlack = MiddleGameKingTable.Select((_, index) => -MiddleGameKingTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameKingTableBlack = EndGameKingTable.Select((_, index) => -EndGameKingTable[index ^ 56]).ToArray();

    /// <summary>
    /// [12][64]
    /// </summary>
    public static readonly int[][] MiddleGamePositionalTables =
    [
        MiddleGamePawnTable,
        MiddleGameKnightTable,
        MiddleGameBishopTable,
        MiddleGameRookTable,
        MiddleGameQueenTable,
        MiddleGameKingTable,

        MiddleGamePawnTableBlack,
        MiddleGameKnightTableBlack,
        MiddleGameBishopTableBlack,
        MiddleGameRookTableBlack,
        MiddleGameQueenTableBlack,
        MiddleGameKingTableBlack
    ];

    /// <summary>
    /// [12][64]
    /// </summary>
    public static readonly int[][] EndGamePositionalTables =
    [
        EndGamePawnTable,
        EndGameKnightTable,
        EndGameBishopTable,
        EndGameRookTable,
        EndGameQueenTable,
        EndGameKingTable,

        EndGamePawnTableBlack,
        EndGameKnightTableBlack,
        EndGameBishopTableBlack,
        EndGameRookTableBlack,
        EndGameQueenTableBlack,
        EndGameKingTableBlack
    ];

    public static readonly int[,] MiddleGameTable = new int[12, 64];
    public static readonly int[,] EndGameTable = new int[12, 64];

    static EvaluationConstants()
    {
        for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
        {
            for (int sq = 0; sq < 64; ++sq)
            {
                MiddleGameTable[piece, sq] = MiddleGamePieceValues[piece] + MiddleGamePositionalTables[piece][sq];
                EndGameTable[piece, sq] = EndGamePieceValues[piece] + EndGamePositionalTables[piece][sq];
            }
        }
    }

    /// <summary>
    /// MVV LVA [attacker,victim] [12,64]
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/move_ordering_intro/bbc.c#L2406
    ///             (Victims)   Pawn Knight Bishop  Rook   Queen  King
    /// (Attackers)
    ///       Pawn              105    205    305    405    505    605
    ///     Knight              104    204    304    404    504    604
    ///     Bishop              103    203    303    403    503    603
    ///       Rook              102    202    302    402    502    602
    ///      Queen              101    201    301    401    501    601
    ///       King              100    200    300    400    500    600
    /// </summary>
    public static readonly int[,] MostValueableVictimLeastValuableAttacker =
    {
        { 105, 205, 305, 405, 505, 605, 105, 205, 305, 405, 505, 605 },
        { 104, 204, 304, 404, 504, 604, 104, 204, 304, 404, 504, 604 },
        { 103, 203, 303, 403, 503, 603, 103, 203, 303, 403, 503, 603 },
        { 102, 202, 302, 402, 502, 602, 102, 202, 302, 402, 502, 602 },
        { 101, 201, 301, 401, 501, 601, 101, 201, 301, 401, 501, 601 },
        { 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600 },
        { 105, 205, 305, 405, 505, 605, 105, 205, 305, 405, 505, 605 },
        { 104, 204, 304, 404, 504, 604, 104, 204, 304, 404, 504, 604 },
        { 103, 203, 303, 403, 503, 603, 103, 203, 303, 403, 503, 603 },
        { 102, 202, 302, 402, 502, 602, 102, 202, 302, 402, 502, 602 },
        { 101, 201, 301, 401, 501, 601, 101, 201, 301, 401, 501, 601 },
        { 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600 }
    };

    /// <summary>
    /// Base absolute checkmate evaluation value. Actual absolute evaluations are lower than this one by a number of <see cref="Position.DepthCheckmateFactor"/>
    /// </summary>
    public const int CheckMateBaseEvaluation = 30_000;

    /// <summary>
    /// This value combined with <see cref="PositiveCheckmateDetectionLimit"/> and <see cref="NegativeCheckmateDetectionLimit"/> should allows mates up to in <see cref="Constants.AbsoluteMaxDepth"/> moves.
    /// </summary>
    public const int CheckmateDepthFactor = 10;

    /// <summary>
    /// Minimum evaluation for a position to be White checkmate
    /// </summary>
    public const int PositiveCheckmateDetectionLimit = 27_000; // CheckMateBaseEvaluation - (Constants.AbsoluteMaxDepth + 45) * DepthCheckmateFactor;

    /// <summary>
    /// Minimum evaluation for a position to be Black checkmate
    /// </summary>
    public const int NegativeCheckmateDetectionLimit = -27_000; // -CheckMateBaseEvaluation + (Constants.AbsoluteMaxDepth + 45) * DepthCheckmateFactor;

    public const int PVMoveScoreValue = 4_194_304;

    public const int TTMoveScoreValue = 2_097_152;

    /// <summary>
    /// For MVVLVA
    /// </summary>
    public const int CaptureMoveBaseScoreValue = 1_048_576;

    public const int FirstKillerMoveValue = 524_288;

    public const int SecondKillerMoveValue = 262_144;

    public const int PromotionMoveScoreValue = 131_072;

    public const int MaxHistoryMoveValue = 4_096;

    /// <summary>
    /// Negative offset to ensure history move scores don't reach other move ordering values
    /// </summary>
    public const int BaseMoveScore = int.MinValue / 2;

    /// <summary>
    /// Outside of the evaluation ranges (higher than any sensible evaluation, lower than <see cref="PositiveCheckmateDetectionLimit"/>)
    /// </summary>
    public const int NoHashEntry = 25_000;

    /// <summary>
    /// Evaluation to be returned when there's one single legal move
    /// </summary>
    public const int SingleMoveEvaluation = 200;
}
