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
        +102, +382, +352, +500, +1101, 0,
        -102, -382, -352, -500, -1101, 0
];

public static readonly int[] EndGamePieceValues =
[
        +149, +485, +436, +893, +1533, 0,
        -149, -485, -436, -893, -1533, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -31,    -23,    -15,    -11,    -4,     25,     31,     -18,
        -29,    -24,    -5,     11,     19,     27,     26,     7,
        -23,    -14,    5,      20,     29,     34,     5,      -3,
        -23,    -10,    2,      21,     29,     30,     3,      -5,
        -26,    -18,    -3,     4,      15,     22,     18,     1,
        -31,    -20,    -20,    -16,    -7,     18,     20,     -25,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        15,     12,     7,      -15,    6,      3,      -3,     -10,
        12,     13,     0,      -14,    -7,     -6,     -3,     -11,
        28,     20,     0,      -22,    -17,    -13,    8,      -1,
        25,     19,     -2,     -16,    -15,    -9,     5,      -4,
        13,     10,     -4,     -13,    -2,     -3,     -2,     -11,
        17,     12,     10,     -15,    16,     7,      -1,     -7,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -154,   -31,    -61,    -44,    -21,    -26,    -16,    -97,
        -46,    -25,    -2,     16,     18,     23,     -12,    -17,
        -28,    4,      22,     60,     62,     43,     38,     -1,
        -8,     28,     45,     62,     63,     64,     50,     21,
        -4,     27,     48,     49,     60,     61,     50,     21,
        -25,    6,      21,     51,     61,     36,     31,     -1,
        -48,    -16,    1,      15,     17,     17,     -10,    -18,
        -171,   -33,    -59,    -33,    -19,    -17,    -24,    -91,
];

public static readonly int[] EndGameKnightTable =
[
        -74,    -70,    -12,    -12,    -10,    -28,    -64,    -94,
        -21,    -1,     15,     9,      11,     9,      -13,    -21,
        -13,    17,     39,     38,     37,     19,     10,     -14,
        8,      20,     52,     53,     56,     50,     25,     -6,
        4,      27,     51,     55,     58,     46,     31,     0,
        -17,    20,     28,     45,     36,     21,     7,      -11,
        -28,    2,      5,      13,     5,      1,      -14,    -24,
        -79,    -69,    -7,     -15,    -10,    -26,    -60,    -94,
];

public static readonly int[] MiddleGameBishopTable =
[
        -14,    12,     -10,    -26,    -20,    -21,    -27,    3,
        8,      4,      9,      -18,    1,      -2,     30,     -11,
        -5,     8,      -3,     4,      -6,     14,     7,      32,
        -5,     -5,     -4,     25,     20,     -16,    5,      1,
        -13,    0,      -13,    18,     7,      -11,    -3,     8,
        5,      8,      8,      -4,     6,      7,      10,     27,
        9,      16,     13,     -7,     -4,     1,      22,     -2,
        12,     15,     3,      -42,    -24,    -27,    -4,     -13,
];

public static readonly int[] EndGameBishopTable =
[
        -14,    18,     -24,    4,      -2,     1,      1,      -32,
        -4,     -6,     -4,     6,      5,      -10,    -1,     -15,
        15,     16,     9,      4,      13,     5,      7,      10,
        13,     8,      7,      -1,     -5,     7,      5,      5,
        8,      10,     7,      4,      -10,    6,      4,      4,
        12,     4,      0,      1,      6,      1,      4,      6,
        -15,    -10,    -17,    3,      2,      -2,     0,      -11,
        -8,     -11,    -18,    8,      9,      5,      0,      -21,
];

public static readonly int[] MiddleGameRookTable =
[
        -8,     -16,    -13,    -9,     6,      -6,     2,      -5,
        -31,    -13,    -14,    -14,    -4,     0,      19,     -6,
        -31,    -15,    -18,    -7,     8,      12,     60,     32,
        -24,    -18,    -12,    -3,     3,      13,     51,     26,
        -18,    -11,    -6,     4,      -1,     14,     40,     23,
        -24,    -11,    -12,    0,      6,      23,     57,     33,
        -28,    -24,    -8,     -8,     -2,     -2,     24,     0,
        -5,     -8,     -8,     2,      15,     -1,     11,     8,
];

public static readonly int[] EndGameRookTable =
[
        6,      8,      13,     3,      -6,     6,      6,      -4,
        15,     20,     20,     9,      -1,     2,      -4,     3,
        9,      7,      10,     3,      -9,     -10,    -23,    -22,
        11,     9,      11,     5,      -4,     -2,     -17,    -20,
        11,     8,      12,     1,      -2,     -8,     -15,    -16,
        11,     12,     2,      -6,     -11,    -13,    -22,    -16,
        20,     22,     16,     5,      -3,     1,      -5,     4,
        0,      0,      8,      -1,     -13,    -1,     -5,     -14,
];

public static readonly int[] MiddleGameQueenTable =
[
        -16,    -21,    -20,    -3,     -12,    -42,    4,      -1,
        0,      -8,     8,      -1,     2,      2,      22,     49,
        -5,     -2,     -6,     -5,     -9,     9,      38,     61,
        -9,     -15,    -14,    -4,     -5,     1,      16,     29,
        -9,     -9,     -14,    -14,    -5,     1,      14,     25,
        -3,     0,      -12,    -9,     -4,     6,      24,     42,
        -16,    -18,    5,      9,      7,      -1,     8,      35,
        -12,    -23,    -10,    -1,     -9,     -49,    -21,    28,
];

public static readonly int[] EndGameQueenTable =
[
        -24,    -14,    -1,     -13,    -7,     -3,     -36,    13,
        -19,    -13,    -28,    -1,     -1,     -14,    -47,    0,
        -17,    -7,     1,      -4,     18,     22,     -10,    5,
        -11,    4,      0,      6,      22,     36,     45,     36,
        -1,     -2,     9,      22,     19,     29,     24,     48,
        -19,    -14,    11,     9,      14,     22,     22,     19,
        -11,    -7,     -24,    -17,    -11,    -7,     -33,    10,
        -12,    -7,     -8,     -5,     2,      28,     22,     -7,
];

public static readonly int[] MiddleGameKingTable =
[
        44,     58,     27,     -83,    6,      -66,    45,     65,
        2,      -8,     -29,    -70,    -83,    -57,    -3,     28,
        -82,    -68,    -114,   -117,   -129,   -135,   -87,    -99,
        -119,   -122,   -140,   -182,   -172,   -160,   -158,   -191,
        -83,    -86,    -130,   -161,   -174,   -143,   -164,   -181,
        -76,    -40,    -107,   -116,   -100,   -114,   -78,    -90,
        89,     2,      -30,    -60,    -64,    -45,    12,     37,
        59,     85,     40,     -67,    17,     -55,    60,     79,
];

public static readonly int[] EndGameKingTable =
[
        -87,    -50,    -24,    4,      -42,    -2,     -40,    -96,
        -22,    16,     29,     43,     51,     36,     14,     -24,
        6,      43,     65,     77,     80,     71,     47,     25,
        16,     60,     85,     102,    98,     90,     75,     47,
        3,      49,     82,     98,     103,    88,     78,     45,
        5,      40,     63,     77,     74,     66,     47,     20,
        -47,    9,      30,     40,     43,     32,     9,      -28,
        -100,   -62,    -30,    -3,     -35,    -6,     -44,    -101,
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
