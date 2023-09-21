/*
 * PSQT based on https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
*/

using Lynx.Model;

namespace Lynx;

public static class EvaluationConstants
{
    public static readonly int[] MiddleGamePieceValues =
    [
        51, 264, 296, 347, 794, 00,
        -51, -264, -296, -347, -794, 00
    ];

    public static readonly int[] EndGamePieceValues =
    [
        165, 352, 367, 647, 1218, 00,
        -165, -352, -367, -647, -1218, 00
    ];

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

    public static readonly int[] MiddleGamePawnTable =
    [
0, 0, 0, 0, 0, 0, 0, 0,
1, 4, 13, 3, 20, 50, 50, 18,
1, 3, 16, 25, 31, 38, 39, 25,
4, 11, 24, 35, 41, 45, 24, 21,
6, 14, 22, 38, 42, 40, 23, 21,
2, 7, 17, 21, 29, 34, 32, 22,
1, 6, 8, 2, 22, 45, 41, 13,
0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
0, 0, 0, 0, 0, 0, 0, 0,
-46, -46, -52, -66, -53, -59, -61, -65,
-48, -47, -56, -65, -61, -61, -57, -63,
-36, -42, -56, -73, -70, -67, -50, -57,
-39, -43, -58, -69, -69, -63, -52, -58,
-46, -48, -59, -63, -58, -58, -57, -64,
-44, -47, -49, -67, -50, -55, -58, -63,
0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
-97, -15, -32, -18, -6, 5, -6, -52,
-23, -9, 8, 18, 22, 34, 10, 8,
-12, 14, 25, 52, 52, 37, 41, 11,
8, 34, 44, 56, 57, 58, 45, 27,
10, 29, 46, 45, 56, 55, 45, 24,
-10, 17, 26, 43, 48, 33, 33, 11,
-20, 2, 12, 16, 19, 27, 11, 6,
-109, -18, -29, -11, -4, 10, -13, -51,
    ];

    public static readonly int[] EndGameKnightTable =
    [
-54, -38, -6, -5, -3, -23, -33, -69,
-12, 2, 17, 13, 11, 7, -9, -14,
-2, 14, 31, 32, 33, 20, 8, -9,
8, 18, 41, 42, 43, 38, 20, -1,
9, 24, 39, 43, 43, 34, 24, 3,
-6, 18, 24, 38, 31, 20, 8, -6,
-19, 2, 8, 16, 9, 5, -10, -15,
-58, -37, -3, -6, -3, -17, -30, -62,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
-35, 2, -22, -24, -23, -20, -14, -10,
-1, 0, 12, -11, 6, 11, 27, -1,
-7, 10, 5, 20, 8, 16, 13, 22,
-8, 6, 16, 34, 35, 11, 13, -6,
-14, 11, 9, 33, 26, 13, 7, 1,
2, 8, 13, 11, 16, 9, 12, 18,
-1, 9, 14, 0, 1, 13, 20, 5,
-8, 2, -8, -32, -25, -18, 0, -23,
    ];

    public static readonly int[] EndGameBishopTable =
    [
-14, 11, -20, 3, -5, -5, -7, -37,
-7, 2, 6, 13, 14, -1, 1, -19,
9, 22, 27, 25, 31, 26, 11, 2,
11, 22, 30, 31, 30, 29, 19, 1,
8, 22, 27, 34, 25, 25, 18, 3,
9, 12, 21, 23, 27, 22, 9, 3,
-17, -1, -3, 13, 13, 8, 5, -20,
-14, -12, -15, 4, 6, -2, -4, -22,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
7, -2, -1, 5, 14, 19, 20, 8,
-12, -1, -3, -4, 5, 16, 30, 12,
-12, -1, -5, -1, 12, 17, 54, 35,
-10, -3, 1, 2, 10, 15, 42, 33,
-4, 1, 3, 9, 9, 17, 35, 32,
-8, 0, 2, 6, 11, 24, 53, 39,
-12, -10, 0, -1, 4, 15, 37, 14,
8, 4, 5, 15, 20, 22, 25, 19,
    ];

    public static readonly int[] EndGameRookTable =
    [
10, 14, 20, 10, 4, 7, 9, 3,
17, 22, 23, 16, 7, 7, 2, 6,
13, 11, 14, 10, 1, 0, -12, -13,
15, 11, 14, 13, 3, 5, -5, -11,
14, 11, 16, 8, 4, 0, -6, -8,
14, 16, 8, 3, -1, -3, -12, -8,
21, 23, 19, 12, 5, 5, 0, 9,
6, 7, 14, 6, -1, 2, 1, -5,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
-23, -24, -23, -4, -16, -25, 0, -16,
-3, -7, 6, -3, 2, 13, 18, 32,
-2, 0, -4, -1, -4, 9, 30, 41,
-4, -7, -2, 4, 3, 7, 19, 22,
-6, -3, -4, -3, 3, 7, 13, 17,
-3, 0, -6, -4, -1, 7, 20, 28,
-13, -15, 2, 7, 5, 8, 14, 25,
-16, -28, -18, 0, -12, -33, -10, -1,
    ];

    public static readonly int[] EndGameQueenTable =
    [
18, 31, 48, 28, 44, 27, 1, 33,
11, 30, 27, 50, 47, 29, 6, 19,
16, 41, 64, 64, 80, 71, 32, 27,
21, 56, 61, 84, 94, 86, 71, 48,
35, 52, 72, 92, 90, 82, 61, 60,
21, 39, 74, 75, 81, 75, 56, 38,
16, 38, 35, 40, 46, 40, 11, 24,
26, 38, 49, 29, 49, 57, 41, 28,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
6, 23, 10, -83, -19, -68, 14, 14,
-21, -17, -30, -64, -73, -44, -6, 2,
-83, -69, -104, -105, -110, -116, -76, -93,
-121, -120, -140, -165, -158, -148, -146, -175,
-91, -99, -128, -153, -157, -126, -147, -162,
-76, -39, -100, -108, -91, -97, -66, -82,
56, -9, -30, -59, -60, -38, 5, 10,
20, 48, 20, -73, -10, -59, 26, 25,
    ];

    public static readonly int[] EndGameKingTable =
    [
-51, -28, -12, 14, -19, 8, -20, -53,
-6, 19, 27, 39, 45, 30, 14, -7,
12, 39, 56, 65, 66, 60, 41, 26,
21, 53, 73, 87, 83, 75, 63, 46,
11, 45, 70, 83, 86, 73, 66, 42,
10, 34, 55, 65, 62, 54, 40, 23,
-29, 12, 26, 37, 38, 28, 10, -11,
-63, -38, -17, 9, -15, 4, -24, -58,
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
