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
            +106, +431, +394, +568, +1249, 0,
            -106, -431, -394, -568, -1249, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +158, +451, +410, +848, +1425, 0,
            -158, -451, -410, -848, -1425, 0
    ];

    public static readonly int[] GamePhaseByPiece =
[
    0, 1, 1, 2, 4, 0,
    0, 1, 1, 2, 4, 0
];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -32, -20, -11, -10, 2, 33, 37, -12,
            -30, -27, 1, 16, 24, 31, 27, 8,
            -27, -16, 4, 24, 30, 35, 4, -5,
            -27, -15, 2, 26, 31, 31, -1, -11,
            -32, -24, -2, 6, 17, 22, 19, 2,
            -36, -21, -20, -26, -10, 21, 23, -22,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            19, 16, 9, 6, 8, -1, -4, -13,
            13, 18, -1, -9, -9, -11, -3, -14,
            25, 22, -4, -26, -24, -21, 6, -6,
            24, 20, -9, -27, -26, -19, 2, -7,
            14, 10, -12, -24, -13, -13, -6, -16,
            24, 15, 11, 74, 41, -1, -3, -9,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -172, -33, -60, -41, -24, -27, -24, -111,
            -51, -30, 2, 19, 21, 27, -10, -16,
            -35, 4, 29, 61, 66, 46, 42, -5,
            -7, 31, 46, 65, 64, 66, 45, 17,
            -5, 27, 49, 52, 61, 65, 44, 12,
            -28, 8, 28, 53, 65, 40, 34, -5,
            -48, -13, 13, 18, 18, 20, -6, -19,
            -186, -35, -56, -32, -23, -19, -31, -99,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -73, -53, -15, -7, -8, -28, -47, -103,
            -25, -3, 13, 10, 11, 6, -12, -26,
            -5, 15, 34, 38, 39, 22, 5, -15,
            9, 22, 53, 54, 57, 47, 24, -2,
            8, 29, 48, 54, 55, 41, 32, 0,
            -17, 15, 22, 44, 32, 18, 4, -15,
            -33, -3, 1, 15, 7, 2, -19, -22,
            -81, -61, -15, -14, -9, -21, -51, -100,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -17, 12, -5, -26, -17, -27, -16, 5,
            3, 10, 7, -16, 3, 2, 38, -8,
            -5, 8, 1, 0, -5, 16, 6, 33,
            -7, -11, -8, 20, 17, -18, 3, -3,
            -15, -2, -13, 18, 5, -12, -2, 7,
            8, 8, 10, -7, 6, 9, 11, 31,
            7, 23, 11, -6, -4, 0, 32, 5,
            13, 12, 8, -35, -24, -30, -7, -8,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -15, 12, -22, 6, -6, 9, -6, -34,
            -7, -7, -6, 3, 4, -8, -3, -15,
            13, 18, 8, 11, 17, 9, 7, 6,
            15, 14, 12, 0, -3, 11, 6, 7,
            12, 12, 9, 4, -8, 6, 2, 0,
            12, 5, 0, 6, 7, 0, 1, 3,
            -16, -15, -15, 7, 7, 4, -3, -15,
            -12, -11, -23, 10, 15, 10, 1, -21,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -9, -17, -15, -9, 7, -5, 1, -7,
            -34, -14, -17, -14, -3, -1, 19, -8,
            -33, -17, -22, -11, 4, 15, 58, 29,
            -27, -19, -14, -9, -1, 8, 45, 31,
            -20, -14, -8, 3, -2, 13, 38, 28,
            -23, -12, -7, 4, 10, 27, 64, 42,
            -27, -28, -10, -10, -2, 1, 30, -1,
            -3, -6, -4, 5, 17, 3, 11, 14,
    ];

    public static readonly int[] EndGameRookTable =
    [
            6, 9, 16, 5, -5, 5, 6, -3,
            13, 22, 23, 11, 1, 8, 3, 4,
            9, 9, 12, 6, -3, -6, -17, -19,
            10, 5, 13, 8, -3, -1, -17, -23,
            4, 3, 10, 1, -3, -10, -22, -24,
            5, 8, -1, -7, -13, -17, -32, -27,
            16, 26, 19, 10, -1, 4, -6, 4,
            -3, 0, 8, -2, -14, -4, -5, -23,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -16, -22, -19, 1, -13, -43, -8, -5,
            0, -9, 9, 1, 6, 8, 20, 57,
            -8, -2, -5, -5, -9, 10, 37, 60,
            -10, -21, -14, -14, -8, -2, 18, 31,
            -11, -10, -16, -18, -7, 2, 10, 23,
            -3, 1, -9, -6, 0, 10, 28, 43,
            -9, -21, 5, 14, 10, 2, 18, 49,
            -9, -25, -11, 4, -9, -49, -24, 14,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -23, -8, -2, -19, -2, 10, -27, 12,
            -22, -16, -27, -1, -3, -8, -29, -14,
            -13, -9, 1, -1, 26, 24, -4, 10,
            -10, 9, -7, 20, 21, 36, 39, 28,
            -4, -4, 4, 24, 18, 23, 24, 39,
            -27, -19, 7, 6, 14, 17, 12, 13,
            -15, -7, -19, -19, -7, 1, -40, -8,
            -14, -9, -6, -16, 7, 29, 18, -12,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            41, 57, 25, -77, 0, -74, 37, 59,
            -6, -10, -34, -81, -88, -63, -9, 21,
            -90, -75, -121, -127, -135, -141, -98, -111,
            -114, -107, -135, -185, -184, -170, -175, -210,
            -48, -74, -112, -156, -167, -139, -146, -172,
            -39, -4, -92, -106, -93, -101, -64, -71,
            109, 11, -19, -58, -59, -37, 23, 46,
            57, 94, 48, -62, 20, -47, 68, 88,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -102, -65, -40, -15, -55, -15, -52, -106,
            -31, 1, 15, 29, 37, 25, 4, -33,
            0, 32, 52, 64, 66, 62, 41, 18,
            7, 50, 76, 96, 93, 84, 71, 48,
            -7, 44, 78, 98, 104, 89, 76, 45,
            -2, 40, 71, 85, 84, 75, 57, 30,
            -37, 21, 41, 56, 57, 47, 22, -14,
            -75, -50, -18, 15, -19, 8, -33, -88,
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
