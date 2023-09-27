/*
 * PSQT based on https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
*/

using Lynx.Model;

namespace Lynx;

public static class EvaluationConstants
{
    public static readonly int[] MiddleGamePieceValues =
    [
            +56, +295, +329, +389, +885, 0,
            -56, -295, -329, -389, -885, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +160, +327, +346, +615, +1171, 0,
            -160, -327, -346, -615, -1171, 0
    ];

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 5, 12, 5, 20, 54, 53, 22,
            0, 2, 18, 24, 30, 38, 40, 25,
            2, 10, 23, 36, 40, 45, 25, 17,
            4, 12, 21, 38, 41, 40, 22, 14,
            1, 5, 18, 20, 29, 34, 35, 23,
            0, 7, 8, 2, 17, 47, 45, 16,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -44, -47, -51, -66, -55, -59, -61, -66,
            -47, -47, -57, -64, -60, -61, -58, -63,
            -35, -42, -56, -73, -69, -66, -51, -56,
            -37, -42, -58, -69, -68, -63, -53, -57,
            -45, -47, -59, -63, -58, -58, -58, -64,
            -43, -47, -48, -66, -49, -55, -59, -64,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -110, -12, -35, -17, -8, 2, -5, -62,
            -25, -12, 10, 18, 22, 36, 10, 9,
            -13, 13, 28, 51, 51, 37, 41, 9,
            7, 34, 42, 54, 55, 57, 42, 23,
            9, 29, 44, 45, 53, 55, 41, 19,
            -9, 17, 28, 43, 48, 33, 34, 8,
            -23, 1, 17, 16, 20, 29, 11, 6,
            -125, -14, -30, -11, -7, 8, -10, -54,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -46, -34, -4, -2, -2, -19, -32, -64,
            -12, 2, 15, 15, 12, 5, -8, -17,
            -1, 15, 28, 32, 33, 20, 7, -9,
            9, 19, 42, 43, 44, 37, 20, 1,
            9, 24, 39, 44, 44, 34, 25, 4,
            -6, 16, 22, 38, 31, 20, 8, -4,
            -19, 2, 7, 16, 9, 5, -9, -16,
            -45, -37, -2, -4, -2, -16, -33, -61,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -36, -4, -23, -28, -27, -26, -15, -17,
            -7, -1, 7, -12, 2, 10, 26, -6,
            -11, 7, 4, 15, 5, 13, 7, 18,
            -14, -1, 12, 30, 30, 7, 8, -12,
            -19, 5, 6, 29, 22, 8, 3, -6,
            -1, 6, 11, 8, 14, 7, 8, 15,
            -6, 9, 9, -4, -3, 10, 20, 3,
            -12, -4, -11, -34, -32, -26, -7, -26,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -14, 5, -22, 2, -6, -6, -11, -32,
            -9, -1, 3, 10, 12, -3, -2, -20,
            6, 19, 24, 23, 28, 23, 10, -2,
            9, 21, 27, 28, 26, 26, 16, 1,
            8, 19, 25, 31, 24, 23, 14, 0,
            6, 9, 18, 21, 24, 20, 6, -1,
            -19, -4, -5, 11, 13, 6, 1, -23,
            -15, -13, -17, 3, 5, -3, -6, -19,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            6, -2, -1, 5, 14, 19, 18, 10,
            -14, -1, -4, -4, 4, 17, 32, 12,
            -13, -1, -7, 0, 11, 18, 56, 36,
            -10, -5, 0, 1, 9, 16, 44, 35,
            -6, 0, 2, 9, 6, 17, 37, 32,
            -8, -1, 2, 6, 11, 25, 55, 40,
            -12, -11, -1, -2, 3, 16, 38, 12,
            8, 4, 5, 15, 20, 23, 23, 21,
    ];

    public static readonly int[] EndGameRookTable =
    [
            9, 11, 17, 8, 2, 4, 6, -2,
            14, 19, 20, 14, 5, 4, -1, 3,
            10, 8, 11, 8, -1, -4, -14, -15,
            11, 8, 11, 9, 1, 1, -9, -14,
            10, 8, 13, 6, 3, -1, -9, -11,
            10, 12, 4, 0, -3, -6, -14, -11,
            18, 21, 17, 10, 3, 3, -3, 7,
            4, 5, 12, 4, -3, -1, 0, -11,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -20, -24, -21, 1, -15, -27, -4, -15,
            -7, -7, 6, -1, 4, 15, 18, 35,
            -6, 1, -2, 1, -4, 10, 30, 43,
            -7, -10, -3, 0, 3, 7, 20, 23,
            -10, -2, -5, -5, 3, 8, 13, 16,
            -5, 1, -4, -3, 0, 10, 22, 30,
            -15, -15, 3, 8, 6, 10, 16, 30,
            -14, -26, -16, 2, -12, -35, -17, -6,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -11, 5, 18, -8, 16, 6, -24, 7,
            -13, 4, 0, 23, 18, 4, -14, -16,
            -9, 9, 36, 35, 54, 44, 8, -4,
            -3, 30, 32, 61, 64, 58, 42, 17,
            9, 24, 43, 67, 64, 53, 35, 31,
            -10, 7, 44, 44, 53, 44, 27, 7,
            -7, 12, 7, 10, 17, 15, -17, -10,
            -1, 10, 21, -3, 24, 29, 18, 7,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            2, 22, 9, -80, -18, -72, 13, 11,
            -19, -16, -32, -70, -75, -46, -7, 1,
            -75, -62, -100, -102, -107, -109, -73, -90,
            -99, -96, -117, -148, -144, -135, -131, -161,
            -78, -83, -106, -136, -140, -119, -129, -157,
            -64, -27, -91, -99, -87, -92, -61, -80,
            55, -8, -29, -63, -63, -39, 5, 10,
            16, 49, 21, -72, -13, -60, 25, 24,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -48, -26, -11, 15, -20, 9, -20, -51,
            -7, 17, 27, 40, 44, 31, 14, -6,
            8, 34, 51, 60, 62, 56, 40, 25,
            12, 43, 63, 77, 75, 68, 57, 40,
            3, 37, 60, 74, 77, 67, 58, 39,
            4, 28, 49, 59, 58, 52, 38, 22,
            -28, 11, 25, 38, 38, 29, 11, -11,
            -59, -38, -17, 9, -15, 5, -24, -57,
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

    public const int PVMoveScoreValue = 200_000;

    public const int TTMoveScoreValue = 190_000;

    /// <summary>
    /// For MVVLVA
    /// </summary>
    public const int CaptureMoveBaseScoreValue = 100_000;

    public const int FirstKillerMoveValue = 9_000;

    public const int SecondKillerMoveValue = 8_000;

    public const int PromotionMoveScoreValue = 7_000;

    /// <summary>
    /// Outside of the evaluation ranges (higher than any sensible evaluation, lower than <see cref="PositiveCheckmateDetectionLimit"/>)
    /// </summary>
    public const int NoHashEntry = 25_000;

    /// <summary>
    /// Evaluation to be returned when there's one single legal move
    /// </summary>
    public const int SingleMoveEvaluation = 200;
}
