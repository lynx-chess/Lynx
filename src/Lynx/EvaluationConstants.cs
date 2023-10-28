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
            +84, +341, +313, +450, +978, 0,
            -84, -341, -313, -450, -978, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +126, +359, +327, +676, +1149, 0,
            -126, -359, -327, -676, -1149, 0
    ];

    public static readonly int[] GamePhaseByPiece =
[
    0, 1, 1, 2, 4, 0,
    0, 1, 1, 2, 4, 0
];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -26, -16, -8, -8, 2, 27, 30, -9,
            -23, -21, 1, 13, 19, 25, 21, 7,
            -21, -13, 4, 19, 24, 28, 3, -4,
            -21, -12, 2, 21, 25, 25, 0, -9,
            -25, -19, -2, 5, 13, 17, 15, 2,
            -29, -16, -15, -20, -8, 17, 18, -18,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            15, 13, 7, 4, 7, -1, -3, -10,
            10, 14, -1, -7, -7, -9, -3, -11,
            20, 17, -3, -21, -20, -17, 4, -5,
            19, 16, -7, -22, -21, -16, 2, -6,
            11, 8, -10, -19, -10, -11, -5, -13,
            19, 12, 9, 58, 32, -1, -2, -7,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -136, -26, -47, -32, -18, -21, -19, -87,
            -39, -23, 3, 16, 18, 22, -7, -12,
            -27, 4, 24, 49, 53, 38, 34, -3,
            -5, 26, 37, 53, 51, 53, 36, 14,
            -3, 22, 40, 42, 50, 53, 36, 11,
            -22, 7, 23, 43, 53, 33, 28, -3,
            -37, -9, 11, 15, 16, 17, -3, -14,
            -147, -27, -44, -24, -17, -14, -23, -77,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -58, -42, -12, -5, -6, -22, -37, -82,
            -20, -1, 11, 9, 9, 6, -9, -20,
            -4, 13, 28, 31, 32, 18, 4, -11,
            7, 18, 43, 43, 46, 38, 20, -1,
            7, 23, 39, 43, 45, 33, 26, 1,
            -13, 13, 18, 36, 26, 15, 4, -11,
            -26, -2, 2, 12, 6, 2, -15, -17,
            -64, -48, -11, -10, -7, -16, -40, -79,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -14, 9, -5, -21, -14, -22, -13, 3,
            2, 7, 5, -13, 2, 1, 30, -7,
            -4, 5, 0, 0, -4, 12, 5, 26,
            -6, -9, -7, 15, 13, -15, 2, -3,
            -12, -2, -11, 14, 3, -10, -2, 5,
            6, 6, 7, -6, 4, 7, 8, 24,
            5, 18, 8, -5, -4, 0, 25, 4,
            10, 9, 6, -28, -20, -25, -6, -7,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -12, 9, -18, 5, -5, 7, -4, -27,
            -6, -6, -4, 2, 3, -6, -2, -12,
            10, 14, 6, 9, 14, 7, 6, 5,
            12, 11, 10, 0, -2, 9, 5, 6,
            9, 9, 7, 3, -7, 5, 1, 0,
            10, 4, 0, 5, 6, 0, 1, 3,
            -13, -12, -12, 6, 5, 3, -2, -12,
            -10, -9, -18, 8, 12, 8, 1, -17,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -7, -13, -11, -7, 6, -3, 1, -5,
            -27, -11, -13, -11, -2, 0, 16, -6,
            -26, -13, -17, -9, 3, 12, 46, 23,
            -21, -15, -11, -7, -1, 7, 36, 25,
            -15, -11, -6, 2, -2, 11, 30, 23,
            -18, -9, -5, 3, 8, 22, 51, 33,
            -21, -22, -7, -8, -1, 1, 24, -1,
            -2, -5, -3, 4, 14, 2, 9, 12,
    ];

    public static readonly int[] EndGameRookTable =
    [
            5, 7, 12, 4, -4, 4, 5, -2,
            10, 18, 18, 8, 1, 6, 2, 3,
            7, 7, 10, 5, -3, -5, -13, -15,
            8, 4, 10, 6, -2, -1, -13, -18,
            4, 2, 8, 1, -2, -8, -17, -19,
            4, 6, -1, -5, -11, -14, -25, -22,
            13, 20, 15, 8, -1, 3, -5, 3,
            -3, 0, 6, -2, -11, -3, -4, -18,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -12, -17, -15, 2, -10, -33, -5, -3,
            1, -7, 7, 1, 5, 7, 16, 46,
            -5, -1, -3, -4, -7, 8, 30, 49,
            -8, -16, -11, -11, -6, -1, 15, 25,
            -8, -7, -12, -14, -5, 2, 9, 19,
            -2, 1, -7, -4, 1, 8, 23, 35,
            -6, -16, 5, 11, 8, 2, 15, 40,
            -6, -19, -8, 4, -7, -38, -19, 12,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -19, -7, -2, -16, -2, 7, -22, 8,
            -18, -13, -22, -1, -2, -7, -24, -12,
            -11, -8, 1, 0, 21, 20, -3, 7,
            -8, 8, -5, 17, 18, 29, 31, 22,
            -4, -3, 4, 20, 16, 19, 19, 31,
            -22, -15, 6, 6, 12, 14, 10, 9,
            -13, -6, -15, -15, -5, 1, -32, -8,
            -12, -7, -6, -13, 5, 23, 14, -10,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            12, 24, -1, -82, -21, -80, 9, 26,
            -26, -29, -48, -85, -91, -71, -28, -4,
            -93, -81, -118, -122, -129, -133, -99, -109,
            -112, -106, -129, -168, -167, -156, -160, -188,
            -59, -80, -110, -146, -154, -132, -138, -158,
            -52, -24, -95, -106, -95, -102, -72, -78,
            66, -12, -36, -67, -68, -51, -3, 16,
            24, 54, 17, -70, -5, -59, 33, 49,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -106, -76, -56, -36, -68, -37, -66, -109,
            -49, -23, -13, -2, 5, -5, -22, -51,
            -25, 1, 17, 26, 28, 25, 8, -10,
            -19, 15, 36, 52, 49, 42, 32, 14,
            -30, 11, 38, 54, 58, 46, 36, 12,
            -26, 7, 32, 43, 42, 35, 21, -1,
            -54, -8, 8, 20, 21, 13, -7, -36,
            -84, -65, -38, -12, -40, -18, -51, -95,
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
