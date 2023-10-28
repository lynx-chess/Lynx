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
            +101, +415, +381, +536, +1235, 0,
            -101, -415, -381, -536, -1235, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +114, +290, +280, +584, +856, 0,
            -114, -290, -280, -584, -856, 0
    ];

    public static readonly int[] GamePhaseByPiece =
[
    0, 1, 1, 2, 4, 0,
    0, 1, 1, 2, 4, 0
];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -33, -10, -18, -9, -18, 37, 35, -17,
            -21, -18, 5, 8, 14, 28, 28, -3,
            -19, -8, 5, 29, 24, 27, 4, -16,
            -23, -13, 4, 25, 20, 27, 0, -18,
            -21, -23, 0, 3, 7, 26, 22, -3,
            -35, -18, -22, -15, -21, 32, 26, -21,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            16, 7, 13, 6, 18, -5, -8, -8,
            10, 11, -7, -5, -7, -9, -6, -11,
            18, 14, -6, -21, -18, -17, 1, -2,
            18, 13, -8, -25, -16, -17, 2, -3,
            5, 7, -10, -23, -10, -14, -10, -15,
            24, 12, 17, 47, 45, -3, -1, -5,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -200, -12, -50, -29, -7, -31, -7, -79,
            -38, -62, 13, 18, 25, 22, -11, -23,
            -19, 14, 32, 41, 65, 44, 47, -5,
            -6, 36, 29, 45, 55, 50, 40, 6,
            -6, 28, 35, 45, 52, 46, 35, 17,
            -13, 6, 32, 47, 47, 46, 46, -2,
            -46, -57, 11, 20, 22, 37, -5, 6,
            -187, -16, -79, -41, 0, -39, -5, -47,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -5, -48, -5, -5, -6, -4, -51, -68,
            -19, 9, -2, 11, 9, 3, -3, -28,
            -13, 22, 24, 31, 21, 15, -5, -14,
            -2, 15, 41, 45, 38, 35, 22, 6,
            -2, 13, 39, 44, 36, 38, 23, -10,
            -16, 4, 17, 31, 25, 14, 1, -18,
            -28, 9, 9, 15, 12, -12, -10, -59,
            -30, -44, 11, -3, -7, -16, -44, -87,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -25, -3, 1, -29, -20, -15, -69, -1,
            -11, 20, -2, -5, 0, 6, 42, -19,
            -9, 15, 9, -6, 6, 28, 14, 6,
            -19, 6, -8, 19, 16, -10, -4, 3,
            11, -1, -5, 14, 9, -8, 11, 7,
            3, 23, 11, 1, 3, 28, 13, 18,
            -17, 21, 7, -6, 4, 15, 44, -28,
            -28, 10, 1, -23, 1, -9, -26, -17,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -11, -1, -20, 10, 6, 4, 12, -28,
            2, -11, 4, -1, 8, 0, -9, -21,
            8, 1, 4, 10, 5, -4, 0, 10,
            11, 5, 9, 1, -1, 9, 6, 5,
            0, 5, 10, 5, -5, 7, -8, 6,
            3, -1, 5, 6, 10, -2, 6, -1,
            -1, -8, -4, 10, 10, 0, -16, 1,
            -9, -10, -14, 5, 1, -6, 11, 2,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -2, -4, -1, 12, 19, 15, -20, -4,
            -28, 2, -21, -2, 3, 26, -15, -62,
            -32, -6, -9, -7, 3, 28, 18, -14,
            -25, -15, -10, -1, 6, 24, 23, -19,
            -27, -22, -7, 11, 5, 34, 25, -2,
            -25, -7, 0, -1, 4, 38, 36, -1,
            -23, 2, 1, 7, 25, 29, 22, -45,
            9, 4, 4, 12, 17, 22, -19, 8,
    ];

    public static readonly int[] EndGameRookTable =
    [
            7, 8, 11, 3, -5, 2, 3, -9,
            17, 11, 17, 9, -6, 2, 13, 22,
            10, 11, -1, 3, -4, -11, -5, -4,
            4, 2, 12, 1, -3, -8, -10, 1,
            4, 5, 9, -4, -4, -10, -17, -9,
            2, -1, 0, 0, -10, -16, -15, -12,
            12, 8, 8, 7, -9, -5, -5, 17,
            -4, 6, 7, 0, -1, -6, 13, -21,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -8, -19, -9, 15, -15, -52, 0, -8,
            -26, -37, 14, 9, 16, 5, 16, 18,
            -11, -1, -1, -1, 5, 6, 28, 41,
            -4, -32, -11, -22, -9, 3, -7, 2,
            -5, -33, -16, -18, -7, -2, -1, 0,
            -8, 19, -4, 9, -4, 13, 13, 28,
            -23, -22, 21, 14, 24, 30, 5, 61,
            12, -2, 7, 14, -9, -22, -3, 18,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -5, 5, 1, -22, 40, 30, 1, 26,
            -7, 23, -27, -13, -17, 5, -17, 2,
            -4, -25, -19, -9, -5, 10, 19, 15,
            -13, 15, -13, 12, 9, 12, 50, 48,
            -13, 6, -12, -7, -5, 6, 52, 60,
            -9, -59, -23, -18, 2, 13, 16, 39,
            -2, -13, -35, -14, -16, -11, -9, -4,
            -37, -12, -9, -23, 20, 26, 15, 11,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            -31, 23, -13, -85, -14, -75, 10, 20,
            -1, -8, -55, -97, -78, -66, -23, -16,
            -67, -31, -56, -81, -84, -82, -54, -93,
            -102, -18, -56, -117, -103, -99, -75, -102,
            -33, 0, -47, -86, -99, -87, -125, -144,
            15, -7, -44, -77, -87, -91, -54, -54,
            -15, -16, -27, -94, -75, -63, -4, 12,
            -45, 33, 6, -71, -8, -65, 20, 20,
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

    public static readonly int[,] LMRReductions = new int[Constants.AbsoluteMaxDepth, Constants.MaxNumberOfPossibleMovesInAPosition];

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

        for (int searchDepth = 1; searchDepth < Constants.AbsoluteMaxDepth; ++searchDepth)    // Depth > 0 or we'd be in QSearch
        {
            for (int movesSearchedCount = 1; movesSearchedCount < Constants.MaxNumberOfPossibleMovesInAPosition; ++movesSearchedCount) // movesSearchedCount > 0 or we wouldn't be applying LMR
            {
                LMRReductions[searchDepth, movesSearchedCount] = Convert.ToInt32(Math.Round(
                    Configuration.EngineSettings.LMR_Base + (Math.Log(movesSearchedCount) * Math.Log(searchDepth) / Configuration.EngineSettings.LMR_Divisor)));
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

    //public const int MaxHistoryMoveValue => Configuration.EngineSettings.MaxHistoryMoveValue;

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
