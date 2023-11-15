/*
 * PSQT based on https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
*/

using Lynx.Model;

namespace Lynx;

public static class EvaluationConstants
{
    /// <summary>
    /// 21454 games, 16+0.16, UHO_XXL_+0.90_+1.19.epd
    /// Retained (W,D,L) = (486074, 1108791, 487345) positions.
    /// </summary>
    public const int EvalNormalizationCoefficient = 138;

    public static readonly double[] As = [-25.59900221, 175.23377472, -145.09355540, 133.49051930];

    public static readonly double[] Bs = [-14.14613328, 84.98205725, -101.16332276, 120.88906952];

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

    public static readonly int[] MiddleGamePieceValues =
    [
            +73, +275, +249, +359, +788, 0,
            -73, -275, -249, -359, -788, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +109, +356, +324, +653, +1123, 0,
            -109, -356, -324, -653, -1123, 0
    ];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -24, -17, -9, -10, 0, 18, 22, -14,
            -22, -19, -2, 10, 15, 20, 17, 4,
            -19, -12, 3, 15, 21, 24, 1, -2,
            -17, -9, 2, 18, 23, 22, 0, -3,
            -21, -13, -2, 5, 12, 15, 12, 0,
            -24, -14, -13, -9, 2, 13, 13, -20,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            10, 9, 3, -11, 3, 1, -3, -7,
            8, 9, -1, -11, -6, -6, -1, -7,
            20, 14, 1, -16, -14, -10, 6, 0,
            18, 14, -2, -13, -12, -7, 4, -2,
            10, 8, -3, -9, -2, -3, -1, -8,
            12, 9, 7, -13, 6, 4, -1, -4,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -111, -27, -42, -31, -16, -19, -20, -71,
            -36, -19, -2, 13, 14, 17, -9, -14,
            -24, 2, 18, 42, 47, 33, 29, -1,
            -6, 22, 33, 47, 47, 47, 34, 15,
            -3, 17, 36, 37, 45, 46, 34, 13,
            -22, 4, 17, 36, 45, 27, 22, -1,
            -33, -9, 4, 11, 12, 12, -6, -15,
            -123, -30, -41, -22, -14, -13, -26, -69,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -57, -41, -10, -8, -7, -21, -35, -69,
            -15, -2, 13, 7, 7, 6, -10, -15,
            -6, 11, 25, 27, 27, 13, 5, -12,
            6, 15, 37, 37, 39, 34, 16, -4,
            6, 21, 35, 38, 39, 29, 21, -1,
            -9, 15, 19, 33, 24, 14, 4, -9,
            -24, -1, 4, 9, 4, 3, -12, -16,
            -61, -40, -7, -10, -7, -17, -33, -63,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -14, 10, -7, -19, -12, -16, -14, 5,
            5, 4, 5, -13, 2, -2, 23, -5,
            -4, 5, -2, 1, -5, 10, 7, 23,
            -2, -6, -6, 14, 12, -13, 3, 1,
            -9, 0, -10, 12, 3, -8, -3, 7,
            4, 4, 5, -5, 3, 4, 8, 20,
            6, 13, 9, -4, -3, -1, 17, 1,
            11, 10, 3, -27, -13, -18, -4, -9,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -8, 14, -16, 4, -4, 3, -3, -28,
            -3, -4, -3, 4, 1, -8, -2, -11,
            11, 12, 6, 4, 10, 5, 2, 4,
            10, 8, 7, 0, -1, 7, 3, 1,
            7, 7, 5, 4, -7, 2, 3, 2,
            10, 3, 0, 2, 5, 0, 1, 4,
            -12, -9, -14, 2, 1, 0, 0, -10,
            -8, -10, -13, 4, 6, 4, -1, -16,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -5, -11, -11, -7, 5, -4, 2, -5,
            -22, -9, -10, -10, -1, 0, 14, -3,
            -22, -12, -13, -7, 5, 11, 42, 22,
            -20, -13, -9, -6, 0, 5, 31, 20,
            -14, -10, -5, 1, 0, 9, 23, 18,
            -19, -10, -7, 0, 6, 17, 41, 26,
            -21, -19, -6, -6, -1, 0, 18, -1,
            -4, -6, -5, 2, 11, -1, 7, 6,
    ];

    public static readonly int[] EndGameRookTable =
    [
            5, 7, 12, 3, -4, 4, 5, -1,
            10, 15, 15, 8, -1, 2, -2, 2,
            6, 4, 6, 2, -7, -8, -18, -19,
            8, 4, 8, 5, -4, -1, -11, -17,
            7, 5, 9, 1, -2, -7, -12, -14,
            7, 10, 1, -5, -9, -11, -17, -13,
            14, 17, 12, 4, -3, 0, -4, 4,
            0, 0, 7, -1, -9, -1, -3, -9,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -13, -15, -16, -2, -9, -27, 1, -2,
            4, -5, 6, 0, 3, 5, 15, 38,
            -2, -2, -3, -3, -6, 8, 28, 42,
            -4, -12, -9, -4, -5, 0, 13, 22,
            -6, -8, -11, -11, -5, 1, 7, 17,
            -1, -1, -8, -6, -1, 4, 18, 28,
            -7, -15, 3, 8, 5, 0, 9, 29,
            -10, -20, -10, 1, -6, -35, -14, 11,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -18, -10, 0, -8, -4, -3, -28, 1,
            -19, -14, -19, -2, -1, -13, -32, -4,
            -13, -4, -1, -2, 14, 14, -10, 6,
            -11, 5, -5, 7, 17, 24, 29, 22,
            0, 0, 6, 16, 14, 18, 18, 33,
            -12, -8, 8, 6, 10, 17, 12, 15,
            -11, -6, -15, -14, -5, -3, -27, 3,
            -7, -4, -1, -8, 3, 21, 13, -6,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            19, 27, 4, -74, -15, -64, 14, 32,
            -16, -22, -39, -70, -79, -59, -21, 2,
            -80, -74, -107, -110, -114, -123, -86, -91,
            -116, -115, -135, -165, -160, -149, -150, -173,
            -81, -97, -126, -154, -158, -128, -150, -157,
            -71, -45, -101, -110, -94, -103, -77, -81,
            56, -17, -37, -62, -63, -49, -8, 10,
            28, 45, 11, -64, -4, -54, 27, 43,
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
            -66, -39, -14, 5, -17, 2, -26, -67,
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
