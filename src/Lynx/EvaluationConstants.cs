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
            +73, +287, +260, +375, +818, 0,
            -73, -287, -260, -375, -818, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +121, +359, +327, +665, +1142, 0,
            -121, -359, -327, -665, -1142, 0
    ];

    public static readonly int[] GamePhaseByPiece =
[
    0, 1, 1, 2, 4, 0,
    0, 1, 1, 2, 4, 0
];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -22, -16, -7, -10, 2, 21, 24, -12,
            -20, -18, 0, 13, 19, 24, 20, 7,
            -18, -11, 4, 17, 23, 27, 3, 1,
            -18, -9, 3, 19, 25, 24, 0, -2,
            -22, -15, -3, 5, 12, 16, 12, 1,
            -25, -16, -15, -23, -4, 13, 14, -20,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            12, 12, 5, 5, 8, -1, -2, -8,
            8, 13, 0, -7, -8, -9, -3, -11,
            18, 15, -3, -20, -19, -16, 4, -5,
            16, 14, -7, -20, -20, -15, 2, -7,
            9, 6, -9, -18, -9, -10, -4, -12,
            16, 10, 7, 57, 32, -2, -2, -6,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -116, -28, -42, -31, -16, -19, -20, -72,
            -36, -20, -1, 14, 15, 18, -9, -14,
            -25, 3, 19, 44, 49, 34, 30, -1,
            -6, 22, 35, 49, 49, 49, 35, 16,
            -3, 19, 37, 38, 47, 48, 35, 14,
            -22, 5, 18, 39, 47, 28, 23, 0,
            -33, -9, 5, 12, 12, 13, -6, -15,
            -122, -31, -41, -23, -15, -13, -27, -70,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -60, -42, -13, -8, -6, -23, -34, -80,
            -18, -2, 12, 7, 8, 8, -8, -15,
            -4, 12, 29, 29, 31, 17, 6, -10,
            7, 16, 40, 40, 43, 37, 18, -2,
            7, 22, 36, 40, 41, 31, 24, -1,
            -12, 13, 19, 34, 25, 15, 4, -11,
            -24, 0, 3, 12, 6, 2, -13, -15,
            -73, -44, -12, -11, -7, -15, -33, -74,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -14, 11, -7, -20, -12, -17, -15, 5,
            4, 4, 5, -13, 2, -1, 23, -5,
            -5, 5, -2, 0, -5, 10, 7, 24,
            -2, -7, -7, 14, 13, -14, 2, 0,
            -10, -1, -11, 13, 3, -8, -3, 7,
            4, 5, 5, -6, 2, 4, 8, 22,
            7, 14, 9, -5, -4, -2, 18, 1,
            11, 10, 3, -29, -15, -19, -4, -9,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -12, 13, -16, 3, -5, 5, -2, -30,
            -6, -4, -3, 3, 3, -6, 0, -12,
            11, 15, 7, 8, 14, 8, 4, 6,
            12, 9, 10, 2, -1, 9, 5, 4,
            7, 9, 7, 4, -7, 4, 4, 1,
            11, 4, 1, 4, 6, 0, 2, 4,
            -12, -10, -12, 5, 4, 3, 0, -8,
            -10, -10, -15, 6, 10, 6, 1, -20,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -5, -12, -11, -7, 5, -4, 1, -6,
            -23, -10, -11, -10, -2, -1, 12, -5,
            -23, -12, -14, -8, 4, 10, 42, 21,
            -19, -13, -10, -6, 0, 5, 32, 21,
            -12, -9, -5, 2, 1, 10, 26, 21,
            -17, -8, -5, 2, 7, 19, 46, 30,
            -19, -20, -6, -6, -1, 0, 20, 2,
            -2, -5, -4, 4, 13, 1, 10, 9,
    ];

    public static readonly int[] EndGameRookTable =
    [
            3, 7, 12, 3, -4, 4, 6, 1,
            10, 17, 17, 8, 0, 6, 4, 4,
            7, 7, 10, 5, -3, -4, -12, -14,
            8, 5, 10, 7, -2, 1, -11, -16,
            5, 4, 9, 1, -3, -8, -15, -17,
            5, 8, 1, -4, -9, -12, -23, -19,
            13, 20, 15, 7, -1, 3, -4, 3,
            -3, 0, 6, -2, -11, -1, -5, -14,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -14, -16, -16, -2, -10, -29, -3, -3,
            4, -5, 7, 0, 4, 4, 15, 40,
            -2, -2, -4, -4, -6, 8, 28, 44,
            -4, -12, -9, -4, -5, -1, 13, 23,
            -4, -7, -10, -10, -4, 2, 9, 19,
            1, 0, -7, -5, 0, 5, 20, 31,
            -4, -14, 4, 9, 6, 1, 11, 33,
            -8, -20, -9, 3, -5, -34, -11, 14,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -16, -8, 2, -8, -2, 1, -21, 5,
            -20, -13, -19, -1, 0, -9, -28, -3,
            -11, -2, 2, 2, 18, 18, -6, 10,
            -10, 5, -3, 10, 19, 28, 32, 24,
            -5, -1, 5, 17, 13, 18, 17, 30,
            -17, -10, 7, 8, 12, 17, 11, 12,
            -15, -6, -14, -12, -3, -1, -28, 0,
            -10, -5, -4, -8, 2, 20, 8, -13,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            15, 23, 0, -79, -19, -70, 9, 27,
            -26, -28, -43, -75, -84, -64, -27, -4,
            -95, -83, -115, -118, -125, -132, -96, -103,
            -129, -122, -143, -175, -170, -159, -166, -189,
            -67, -91, -125, -154, -160, -131, -145, -151,
            -58, -34, -96, -107, -91, -99, -71, -74,
            63, -13, -34, -59, -61, -45, -3, 15,
            28, 50, 15, -64, 0, -52, 33, 48,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -73, -43, -23, -6, -35, -7, -32, -75,
            -16, 9, 17, 28, 35, 24, 10, -18,
            10, 35, 50, 59, 61, 56, 38, 21,
            21, 54, 73, 88, 84, 76, 66, 48,
            8, 48, 75, 90, 94, 79, 71, 43,
            11, 42, 66, 77, 74, 66, 51, 30,
            -21, 24, 39, 49, 49, 41, 23, -4,
            -52, -30, -5, 18, -8, 12, -18, -60,
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
