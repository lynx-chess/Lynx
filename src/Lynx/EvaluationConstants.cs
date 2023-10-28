/*
 * PSQT based on https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
*/

using Lynx.Model;

namespace Lynx;

public static class EvaluationConstants
{
    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

    /// <summary>
    /// 30k games, 16+0.16, UHO_XXL_+0.90_+1.19.epd
    /// Retained (W,D,L) = (791773, 1127929, 793778) positions.
    /// </summary>
    public const int EvalNormalizationCoefficient = 78;

    public static readonly double[] As = [-44.54789428, 284.90322556, -305.65458204, 143.86777995];

    public static readonly double[] Bs = [-21.08101051, 127.81742295, -160.22340655, 128.53122955];

    public static readonly int[] MiddleGamePieceValues =
    [
            +77, +303, +284, +389, +898, 0,
            -77, -303, -284, -389, -898, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +107, +342, +311, +645, +1047, 0,
            -107, -342, -311, -645, -1047, 0
    ];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -24, -16, -13, -12, -11, 22, 27, -10,
            -20, -17, 0, 4, 12, 22, 22, 3,
            -16, -8, 2, 15, 21, 25, 12, -4,
            -14, -7, 0, 16, 19, 24, 10, -7,
            -18, -19, -3, 2, 10, 22, 18, 1,
            -20, -17, -18, -10, -8, 21, 22, -13,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            12, 7, 8, -8, 9, 5, -3, -10,
            10, 8, -4, -13, -5, -2, -6, -9,
            21, 16, 0, -17, -11, -9, 2, 0,
            20, 15, -1, -16, -10, -8, 2, 0,
            11, 7, -3, -12, -3, -2, -5, -9,
            14, 8, 10, -6, 17, 7, -1, -7,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -129, -18, -48, -31, -17, -16, -15, -63,
            -31, -27, 4, 13, 11, 16, -7, -13,
            -16, 3, 21, 37, 42, 30, 23, -3,
            -4, 22, 25, 39, 41, 40, 36, 16,
            -2, 24, 30, 36, 40, 34, 38, 16,
            -14, 8, 20, 34, 37, 30, 23, -8,
            -34, -17, 8, 12, 12, 11, -9, -5,
            -127, -21, -40, -19, -6, -16, -15, -61,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -45, -52, -8, -8, -8, -8, -40, -94,
            -27, -3, 4, 10, 11, -1, -10, -20,
            -16, 18, 23, 31, 26, 18, 11, -4,
            -2, 16, 41, 42, 46, 42, 27, 1,
            -1, 18, 37, 42, 46, 44, 27, 0,
            -16, 9, 20, 34, 28, 15, 10, -17,
            -19, 5, 5, 9, 12, 0, -9, -34,
            -57, -48, -4, -9, -8, -14, -46, -85,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            0, -4, -5, -25, -12, -15, -6, 2,
            -13, 7, 3, -4, 0, -1, 20, 6,
            -5, 2, 2, 2, 2, 8, 8, 15,
            -12, 2, 0, 16, 13, -7, 5, 2,
            -6, 4, -4, 21, 14, -5, 6, 1,
            2, 5, 2, 0, 1, 10, 5, 14,
            9, 11, 9, -5, 0, 4, 18, -2,
            8, 0, -4, -33, -21, -16, -11, -11,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -10, 9, -18, 6, 1, -1, -2, -12,
            5, -8, -2, -1, 1, -3, -5, -18,
            7, 11, 4, 4, 5, 3, 2, 9,
            10, 7, 6, -2, -4, 5, 10, 12,
            3, 5, 4, -1, -7, 2, 1, 13,
            8, 3, 2, 1, 6, -3, 7, 5,
            -11, -8, -7, 0, 1, -2, -10, -5,
            -9, -3, -8, 4, 8, -1, 10, -3,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -5, -8, -5, 2, 6, 0, -7, -6,
            -24, -10, -16, -15, -4, 3, 3, -43,
            -16, 4, -13, 3, 8, 12, 34, 2,
            -15, -1, -7, 9, 8, 19, 35, 5,
            -17, -7, 3, 10, 11, 20, 32, 18,
            -15, -3, -7, 7, 5, 17, 33, 10,
            -22, -16, -8, -5, -1, 10, 13, -24,
            -3, -7, -5, 2, 8, 2, -1, 4,
    ];

    public static readonly int[] EndGameRookTable =
    [
            3, 5, 7, 1, -2, 4, 4, -4,
            14, 11, 10, 10, -4, 0, 1, 17,
            1, 0, 5, -2, -6, -5, -14, -6,
            1, -2, 8, 0, -3, -3, -16, -6,
            1, 3, 4, -2, -4, -4, -14, -7,
            3, 2, 3, -6, -6, -10, -14, -8,
            14, 12, 11, 3, 0, -4, -1, 15,
            1, 1, 8, 1, -4, 0, 6, -12,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -7, -15, -12, 4, -8, -35, 0, 0,
            -6, -9, 7, 3, 4, 9, 12, 28,
            -14, 0, 3, -3, -1, 6, 26, 35,
            -6, -13, -13, -15, -5, 5, 14, 19,
            -9, -8, -12, -16, -11, 3, 10, 14,
            -10, 7, 1, -3, -5, 8, 17, 20,
            -10, -5, 8, 7, 8, 6, 14, 19,
            -6, -8, -11, 5, -5, -29, -22, 16,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -3, 0, -4, -27, 1, 12, 2, 31,
            -14, -5, -31, -19, -12, -17, -43, 3,
            -8, -11, -16, -7, 1, 10, 13, 25,
            -11, -1, -4, 15, 9, 25, 32, 52,
            -16, -7, -4, 10, 19, 28, 41, 55,
            -1, -31, -15, -5, 7, 7, 17, 49,
            1, -19, -31, -21, -19, -17, -46, 25,
            -11, -8, 1, -28, 2, 12, 25, 11,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            13, 35, -1, -78, -17, -73, 13, 32,
            5, -12, -49, -79, -76, -60, -10, 6,
            -45, -64, -50, -77, -72, -63, -40, -62,
            -110, -26, -63, -84, -72, -74, -49, -84,
            -47, -31, -26, -86, -92, -56, -63, -105,
            -20, -14, -59, -75, -67, -72, -40, -58,
            27, -5, -41, -79, -69, -58, -6, 11,
            7, 39, 4, -75, -18, -68, 18, 34,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -70, -30, -8, 13, -18, 14, -15, -65,
            -17, 14, 34, 41, 44, 36, 16, -10,
            7, 39, 44, 57, 57, 50, 33, 17,
            22, 39, 59, 66, 65, 56, 45, 22,
            2, 35, 51, 65, 69, 56, 48, 28,
            12, 27, 46, 55, 54, 51, 34, 16,
            -9, 14, 29, 41, 42, 35, 13, -13,
            -62, -32, -10, 13, -18, 7, -19, -64,
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
