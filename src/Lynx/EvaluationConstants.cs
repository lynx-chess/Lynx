/*
 * PSQT based on https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
*/

using Lynx.Model;

namespace Lynx;

public static class EvaluationConstants
{
    public static readonly int[] MiddleGamePieceValues =
    [
            +55, +299, +285, +389, +881, 0,
            -55, -299, -285, -389, -881, 0
    ];
    public static readonly int[] EndGamePieceValues =
    [
            +161, +324, +300, +612, +1047, 0,
            -161, -324, -300, -612, -1047, 0
    ];

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];
    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -1, 9, 15, 16, 24, 45, 49, 13,
            1, 4, 22, 33, 38, 44, 41, 27,
            3, 11, 26, 40, 44, 48, 26, 18,
            4, 13, 25, 42, 45, 45, 24, 15,
            2, 8, 23, 28, 36, 39, 38, 25,
            -1, 10, 11, 15, 21, 38, 41, 8,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -45, -48, -52, -68, -55, -56, -61, -65,
            -47, -47, -58, -67, -62, -62, -58, -63,
            -36, -42, -56, -73, -69, -66, -51, -56,
            -38, -42, -58, -69, -68, -64, -53, -57,
            -46, -48, -60, -65, -59, -59, -58, -64,
            -43, -48, -50, -67, -48, -53, -59, -62,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -116, -17, -37, -23, -11, -13, -11, -74,
            -30, -15, 7, 20, 21, 25, 0, -5,
            -19, 9, 27, 50, 53, 40, 36, 3,
            1, 29, 39, 53, 52, 54, 39, 19,
            3, 25, 41, 44, 50, 53, 38, 15,
            -14, 12, 26, 44, 52, 35, 31, 3,
            -28, -3, 16, 19, 20, 21, 2, -7,
            -132, -19, -34, -16, -10, -7, -16, -65,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -41, -30, 0, 2, 2, -11, -27, -57,
            -7, 6, 19, 15, 15, 12, -3, -10,
            3, 20, 31, 35, 34, 21, 11, -5,
            14, 24, 45, 46, 48, 41, 25, 6,
            14, 29, 43, 46, 48, 37, 29, 9,
            -1, 21, 25, 41, 32, 21, 12, 0,
            -15, 6, 11, 16, 12, 11, -5, -9,
            -41, -33, 2, 0, 2, -8, -28, -55,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -18, 2, -10, -25, -18, -25, -17, -3,
            -4, 1, -1, -17, -3, -5, 21, -11,
            -9, 0, -5, -5, -9, 6, -1, 18,
            -10, -14, -11, 8, 6, -19, -3, -7,
            -17, -7, -15, 7, -3, -14, -7, -1,
            0, 0, 1, -10, -1, 0, 2, 15,
            -1, 10, 2, -9, -9, -5, 16, -2,
            3, 3, 0, -30, -22, -28, -12, -13,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -5, 12, -14, 7, -1, 6, -3, -21,
            -1, -3, -2, 5, 4, -6, -2, -9,
            11, 13, 7, 7, 11, 6, 5, 5,
            11, 11, 8, 2, 0, 8, 5, 5,
            11, 9, 7, 5, -3, 5, 3, 3,
            11, 5, 2, 4, 6, 2, 2, 5,
            -9, -7, -11, 4, 4, 2, 0, -10,
            -5, -7, -12, 7, 10, 8, 1, -10,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            7, 1, 2, 6, 18, 9, 14, 9,
            -11, 3, 2, 3, 11, 13, 29, 8,
            -11, 1, -2, 6, 17, 24, 56, 35,
            -7, -2, 2, 6, 12, 18, 45, 35,
            -3, 2, 6, 14, 10, 21, 38, 31,
            -6, 2, 6, 14, 18, 31, 55, 39,
            -9, -8, 5, 5, 11, 13, 33, 9,
            9, 7, 8, 15, 24, 13, 18, 21,
    ];

    public static readonly int[] EndGameRookTable =
    [
            12, 14, 18, 11, 3, 10, 11, 3,
            17, 21, 22, 14, 6, 9, 3, 8,
            12, 11, 13, 9, 1, -2, -11, -12,
            14, 10, 14, 11, 4, 4, -6, -11,
            12, 10, 15, 8, 5, 1, -6, -7,
            13, 15, 7, 1, -2, -4, -11, -7,
            20, 23, 18, 11, 4, 8, 3, 11,
            8, 7, 13, 6, -2, 5, 5, -5,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -9, -14, -12, 3, -8, -28, 0, -1,
            2, -5, 8, 2, 6, 7, 16, 42,
            -4, 0, -2, -1, -5, 9, 29, 45,
            -6, -14, -9, -9, -5, 1, 14, 23,
            -8, -7, -11, -13, -4, 2, 8, 17,
            -3, 1, -6, -4, 1, 7, 20, 30,
            -7, -15, 5, 10, 8, 2, 13, 34,
            -7, -18, -8, 3, -6, -35, -19, 8,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -16, -6, -1, -13, -1, 5, -25, 5,
            -13, -10, -18, 1, 0, -8, -24, -9,
            -10, -7, 1, -1, 18, 16, -5, 5,
            -6, 9, -4, 15, 17, 26, 29, 21,
            4, 1, 7, 20, 17, 20, 21, 34,
            -13, -10, 8, 5, 12, 15, 13, 15,
            -6, -3, -13, -14, -5, 1, -27, -1,
            -5, -3, 0, -9, 8, 24, 21, 2,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            14, 26, 3, -73, -16, -69, 13, 28,
            -15, -22, -41, -74, -80, -61, -21, 1,
            -73, -67, -103, -106, -110, -115, -83, -90,
            -95, -93, -113, -148, -147, -137, -135, -161,
            -69, -82, -105, -137, -142, -122, -133, -153,
            -60, -33, -93, -100, -91, -97, -72, -79,
            54, -15, -37, -64, -66, -50, -7, 10,
            23, 46, 12, -65, -8, -56, 26, 41,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -54, -29, -10, 12, -19, 8, -21, -60,
            -9, 17, 29, 41, 45, 35, 18, -8,
            7, 35, 51, 61, 62, 58, 42, 24,
            11, 42, 63, 77, 76, 68, 58, 40,
            2, 37, 60, 74, 77, 67, 59, 38,
            3, 29, 50, 59, 59, 53, 40, 21,
            -28, 12, 27, 38, 39, 32, 14, -13,
            -62, -39, -14, 7, -15, 4, -25, -65,
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
