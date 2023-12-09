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
        0,
        1,
        1,
        2,
        4,
        0,
        0,
        1,
        1,
        2,
        4,
        0
    ];

    public static readonly int[] MiddleGamePieceValues =
    [
            +102,
        +382,
        +352,
        +500,
        +1102,
        0,
        -102,
        -382,
        -352,
        -500,
        -1102,
        0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +149,
        +484,
        +435,
        +892,
        +1531,
        0,
        -149,
        -484,
        -435,
        -892,
        -1531,
        0
    ];

    public static readonly int[] MiddleGamePawnTable =
    [
            0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        -31,
        -22,
        -15,
        -11,
        -4,
        25,
        31,
        -18,
        -29,
        -24,
        -5,
        11,
        19,
        27,
        26,
        7,
        -23,
        -14,
        5,
        20,
        29,
        34,
        5,
        -3,
        -23,
        -10,
        3,
        21,
        30,
        30,
        4,
        -4,
        -26,
        -18,
        -3,
        4,
        15,
        22,
        18,
        1,
        -31,
        -20,
        -20,
        -15,
        -7,
        19,
        20,
        -25,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        15,
        12,
        7,
        -15,
        6,
        3,
        -4,
        -11,
        12,
        12,
        0,
        -14,
        -7,
        -6,
        -3,
        -11,
        28,
        20,
        0,
        -22,
        -18,
        -13,
        8,
        -1,
        24,
        19,
        -2,
        -17,
        -16,
        -10,
        5,
        -4,
        13,
        10,
        -4,
        -13,
        -3,
        -3,
        -2,
        -11,
        17,
        12,
        9,
        -15,
        17,
        7,
        -1,
        -7,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -155,
        -30,
        -62,
        -43,
        -21,
        -26,
        -16,
        -97,
        -46,
        -25,
        -2,
        16,
        18,
        23,
        -13,
        -17,
        -27,
        4,
        22,
        60,
        62,
        43,
        38,
        -1,
        -8,
        28,
        45,
        62,
        63,
        63,
        50,
        22,
        -4,
        27,
        48,
        50,
        60,
        61,
        51,
        21,
        -25,
        6,
        21,
        51,
        61,
        36,
        31,
        -2,
        -48,
        -16,
        1,
        15,
        17,
        17,
        -10,
        -18,
        -170,
        -33,
        -59,
        -33,
        -19,
        -17,
        -24,
        -91,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -73,
        -70,
        -11,
        -12,
        -9,
        -27,
        -63,
        -93,
        -21,
        -1,
        15,
        9,
        12,
        10,
        -12,
        -20,
        -13,
        17,
        40,
        39,
        38,
        20,
        11,
        -14,
        8,
        21,
        52,
        53,
        57,
        51,
        26,
        -5,
        5,
        28,
        51,
        55,
        59,
        47,
        32,
        0,
        -17,
        20,
        29,
        46,
        36,
        22,
        7,
        -10,
        -27,
        2,
        5,
        13,
        6,
        2,
        -14,
        -24,
        -79,
        -70,
        -7,
        -14,
        -10,
        -25,
        -60,
        -93,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -13,
        12,
        -10,
        -26,
        -20,
        -21,
        -27,
        3,
        8,
        5,
        8,
        -18,
        1,
        -2,
        30,
        -11,
        -5,
        8,
        -3,
        4,
        -6,
        14,
        8,
        32,
        -4,
        -4,
        -4,
        25,
        21,
        -16,
        5,
        2,
        -13,
        1,
        -12,
        19,
        7,
        -10,
        -3,
        8,
        6,
        8,
        8,
        -5,
        6,
        7,
        10,
        27,
        9,
        16,
        13,
        -7,
        -4,
        1,
        22,
        -2,
        12,
        15,
        3,
        -42,
        -23,
        -27,
        -5,
        -12,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -14,
        19,
        -24,
        5,
        -2,
        2,
        2,
        -31,
        -4,
        -7,
        -3,
        6,
        5,
        -9,
        -1,
        -15,
        15,
        16,
        9,
        4,
        14,
        5,
        8,
        10,
        13,
        9,
        8,
        -1,
        -5,
        8,
        5,
        5,
        8,
        10,
        8,
        5,
        -9,
        6,
        4,
        5,
        13,
        4,
        1,
        2,
        7,
        1,
        4,
        7,
        -14,
        -10,
        -16,
        3,
        2,
        -1,
        0,
        -10,
        -8,
        -11,
        -17,
        8,
        9,
        6,
        0,
        -21,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -8,
        -16,
        -13,
        -9,
        6,
        -6,
        2,
        -6,
        -30,
        -13,
        -14,
        -15,
        -3,
        0,
        19,
        -6,
        -31,
        -15,
        -18,
        -7,
        8,
        11,
        60,
        31,
        -24,
        -18,
        -12,
        -3,
        3,
        13,
        50,
        26,
        -18,
        -12,
        -5,
        4,
        -1,
        14,
        40,
        23,
        -24,
        -11,
        -12,
        1,
        6,
        23,
        56,
        33,
        -28,
        -24,
        -8,
        -8,
        -2,
        -2,
        25,
        0,
        -5,
        -8,
        -8,
        2,
        15,
        -1,
        11,
        7,
    ];

    public static readonly int[] EndGameRookTable =
    [
            6,
        8,
        13,
        4,
        -6,
        6,
        6,
        -4,
        16,
        20,
        20,
        9,
        -1,
        2,
        -4,
        4,
        9,
        7,
        10,
        3,
        -9,
        -9,
        -23,
        -21,
        12,
        10,
        11,
        5,
        -4,
        -1,
        -17,
        -20,
        12,
        8,
        12,
        2,
        -1,
        -7,
        -14,
        -15,
        12,
        13,
        2,
        -6,
        -11,
        -13,
        -21,
        -15,
        21,
        23,
        16,
        5,
        -3,
        1,
        -5,
        5,
        0,
        1,
        8,
        -1,
        -13,
        -1,
        -4,
        -14,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -16,
        -20,
        -20,
        -2,
        -11,
        -41,
        5,
        0,
        0,
        -8,
        9,
        -1,
        3,
        3,
        23,
        49,
        -5,
        -1,
        -5,
        -5,
        -8,
        10,
        39,
        62,
        -9,
        -14,
        -13,
        -3,
        -4,
        2,
        17,
        29,
        -9,
        -9,
        -13,
        -14,
        -4,
        1,
        15,
        26,
        -2,
        0,
        -12,
        -9,
        -3,
        7,
        24,
        42,
        -15,
        -18,
        5,
        9,
        7,
        0,
        7,
        36,
        -12,
        -22,
        -10,
        0,
        -9,
        -49,
        -20,
        27,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -24,
        -15,
        -1,
        -13,
        -8,
        -3,
        -36,
        12,
        -19,
        -14,
        -28,
        -1,
        -1,
        -14,
        -47,
        0,
        -17,
        -7,
        0,
        -5,
        18,
        21,
        -10,
        4,
        -11,
        4,
        0,
        5,
        22,
        35,
        45,
        35,
        -1,
        -1,
        9,
        21,
        19,
        28,
        23,
        48,
        -19,
        -15,
        11,
        9,
        14,
        21,
        21,
        19,
        -12,
        -8,
        -25,
        -17,
        -11,
        -7,
        -32,
        10,
        -11,
        -8,
        -8,
        -6,
        2,
        29,
        21,
        -6,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            44,
        57,
        27,
        -83,
        6,
        -66,
        45,
        65,
        1,
        -8,
        -29,
        -70,
        -83,
        -58,
        -3,
        28,
        -81,
        -68,
        -113,
        -117,
        -129,
        -135,
        -86,
        -99,
        -119,
        -121,
        -139,
        -181,
        -171,
        -160,
        -159,
        -190,
        -82,
        -86,
        -129,
        -160,
        -174,
        -144,
        -162,
        -181,
        -76,
        -40,
        -106,
        -116,
        -100,
        -114,
        -77,
        -90,
        89,
        2,
        -30,
        -60,
        -64,
        -45,
        12,
        37,
        59,
        85,
        40,
        -67,
        17,
        -55,
        60,
        79,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -87,
        -50,
        -24,
        4,
        -43,
        -2,
        -40,
        -96,
        -22,
        16,
        29,
        43,
        51,
        36,
        14,
        -24,
        6,
        43,
        64,
        76,
        80,
        71,
        47,
        25,
        16,
        60,
        84,
        102,
        98,
        89,
        75,
        47,
        3,
        49,
        82,
        98,
        103,
        88,
        78,
        45,
        5,
        39,
        63,
        77,
        73,
        66,
        47,
        20,
        -47,
        9,
        30,
        40,
        43,
        32,
        9,
        -28,
        -100,
        -62,
        -30,
        -3,
        -35,
        -6,
        -44,
        -101,
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

    public static readonly int[] HistoryBonus = new int[Constants.AbsoluteMaxDepth];

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

            HistoryBonus[searchDepth] = Math.Min(
                Configuration.EngineSettings.History_MaxMoveRawBonus,
                (4 * searchDepth * searchDepth) + (120 * searchDepth) - 120);   // Sirius, originally from Berserk
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

    public const int MinEval = NegativeCheckmateDetectionLimit + 1;

    public const int MaxEval = PositiveCheckmateDetectionLimit - 1;

    public const int PVMoveScoreValue = 4_194_304;

    public const int TTMoveScoreValue = 2_097_152;

    /// <summary>
    /// For MVVLVA
    /// </summary>
    public const int CaptureMoveBaseScoreValue = 1_048_576;

    public const int FirstKillerMoveValue = 524_288;

    public const int SecondKillerMoveValue = 262_144;

    public const int ThirdKillerMoveValue = 131_072;

    public const int PromotionMoveScoreValue = 65_536;

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
