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

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

    public static readonly int[] MiddleGamePieceValues =
    [
            +53, +168, +164, +224, +633, 0,
            -53, -168, -164, -224, -633, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +128, +428, +381, +766, +1271, 0,
            -128, -428, -381, -766, -1271, 0
    ];

    public static readonly int[] MiddleGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            -17, -16, -16, -7, -15, 17, 21, 1,
            -13, -15, -3, -1, 7, 16, 15, 8,
            -10, -2, 0, 9, 19, 19, 17, 2,
            -7, -2, -1, 8, 15, 21, 13, -3,
            -13, -18, -5, -2, 8, 20, 12, 2,
            -14, -18, -19, -11, -10, 18, 18, -6,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0, 0, 0, 0, 0, 0, 0, 0,
            10, 7, 10, -7, 15, 15, 3, -19,
            11, 5, -2, -18, -3, 3, -6, -15,
            19, 18, -1, -17, -10, -7, -3, -8,
            18, 15, -2, -15, -8, -10, -2, -6,
            11, 2, -4, -14, -4, 3, -7, -12,
            14, 8, 9, -7, 14, 14, 1, -13,
            0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -116, -14, -57, -16, -8, -3, -11, -47,
            -16, -19, 3, 12, 10, 14, -2, -14,
            -7, 5, 17, 37, 36, 22, 14, 0,
            4, 23, 24, 30, 33, 32, 33, 19,
            3, 23, 26, 30, 32, 26, 34, 20,
            -12, 12, 15, 32, 30, 24, 20, -1,
            -17, -23, 3, 11, 6, 6, -21, -2,
            -78, -13, -47, -22, 0, -10, -8, -82,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -88, -46, -5, -9, -12, -10, -38, -154,
            -38, -14, 7, 15, 15, -1, -9, -10,
            -28, 21, 27, 42, 31, 29, 25, -4,
            -11, 20, 48, 52, 59, 52, 39, 7,
            -8, 18, 43, 54, 64, 59, 43, 6,
            -33, 9, 30, 39, 36, 27, 24, -18,
            -29, 12, 3, 10, 18, 3, 3, -29,
            -106, -56, -10, -4, -13, -12, -54, -135,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            13, 2, -7, -27, -22, -9, 13, 2,
            -11, 5, 4, -2, -2, -3, 12, 14,
            -9, 1, 4, 4, 0, 2, 7, 15,
            -6, 7, 5, 16, 16, -1, 7, 11,
            -4, 8, 4, 18, 24, -5, 6, 4,
            -1, -2, 2, 4, -2, 5, 5, 11,
            -7, 6, 0, -8, -2, 1, 13, 5,
            7, -2, -11, -41, -38, -7, -22, 2,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -15, 6, -22, 5, -1, -5, 1, 1,
            5, -9, -6, -7, -2, -1, -3, -16,
            8, 15, 3, 2, 4, 2, 6, 7,
            5, 4, 2, -8, -8, 2, 20, 21,
            4, 7, 1, -4, -14, 7, 8, 26,
            11, 8, 1, 0, 4, -2, 3, 10,
            -4, -7, -1, -6, 2, -8, -9, -13,
            -12, 0, -12, 6, 9, -6, 18, -19,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -9, -12, -8, -1, -1, -2, 0, -9,
            -14, -19, -16, -13, -12, 0, -2, -18,
            -15, 5, -13, 11, 3, 5, 27, 6,
            -7, 2, -2, 14, 7, 16, 35, 10,
            -15, 2, 7, 8, 12, 26, 34, 22,
            -12, 1, -4, 7, 2, 7, 26, -1,
            -12, -21, -9, -13, -10, 6, 4, 20,
            -10, -14, -10, -3, 1, 0, 14, -2,
    ];

    public static readonly int[] EndGameRookTable =
    [
            -1, 2, 2, -4, -3, 4, 7, 0,
            10, 12, 8, 9, -1, -3, 5, 17,
            0, -4, 8, -10, 0, 5, -14, 7,
            -4, -1, 10, -2, 4, 2, -18, -3,
            0, 2, -1, -2, 1, 4, -8, -4,
            0, 0, 5, -5, -5, -4, -12, 3,
            9, 13, 11, 2, 7, -7, -2, -2,
            -1, -2, 8, -2, -2, 1, 1, -2,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -16, -2, -13, -1, -11, -32, -5, 10,
            -3, -3, 2, 2, 1, 7, 9, 17,
            -8, 0, 4, -4, 0, -3, 16, 35,
            -3, -5, -10, -8, -4, 9, 11, 23,
            7, 1, -7, -13, -15, 9, 15, 20,
            -14, 2, 7, -4, -4, 3, 25, 16,
            -6, -3, 5, -2, 3, 7, 24, 14,
            -9, -5, -12, 2, -7, -31, -16, 19,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            12, -6, -11, -30, 0, 12, 39, 48,
            -3, -18, -31, -22, -22, -28, -57, 21,
            -19, -7, -13, -6, -5, 22, 38, 40,
            -21, -11, -1, 8, 13, 30, 47, 62,
            -51, -13, -12, 14, 36, 26, 45, 51,
            -8, -19, -26, -12, -4, 13, 18, 58,
            5, -12, -31, -18, -20, -42, -85, 40,
            0, -4, -8, -33, -7, -14, 23, 46,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            -8, 32, -11, -62, -24, -93, 7, 21,
            7, -19, -47, -66, -62, -48, -6, 8,
            2, -47, -11, -20, -14, -9, -16, -19,
            -58, 36, 36, 28, 32, 7, 15, -25,
            -22, 47, 89, 19, 38, 42, 13, -27,
            -15, 3, -35, -16, -12, -26, -10, -38,
            -5, -1, -52, -68, -53, -48, -4, 11,
            -13, 21, -17, -80, -27, -84, 14, 28,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -52, -20, 11, 1, -37, 17, -11, -69,
            -3, 25, 31, 35, 35, 32, 15, -14,
            6, 48, 43, 52, 47, 37, 26, 4,
            17, 40, 55, 56, 55, 46, 39, 14,
            4, 36, 46, 55, 51, 40, 39, 11,
            31, 44, 47, 49, 46, 45, 27, 8,
            13, 25, 35, 38, 38, 32, 13, -15,
            -64, -8, 3, 3, -38, 6, -16, -69,
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
