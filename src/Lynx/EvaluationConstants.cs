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
        +102, +385, +349, +503, +1105, 0,
        -102, -385, -349, -503, -1105, 0
];

public static readonly int[] EndGamePieceValues =
[
        +152, +499, +454, +916, +1574, 0,
        -152, -499, -454, -916, -1574, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -33,    -23,    -12,    -14,    0,      25,     31,     -20,
        -30,    -26,    -3,     14,     22,     29,     25,     6,
        -26,    -16,    4,      22,     30,     34,     2,      -2,
        -24,    -12,    3,      26,     32,     31,     1,      -4,
        -29,    -18,    -2,     7,      17,     21,     17,     1,
        -33,    -19,    -18,    -12,    4,      18,     19,     -28,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        15,     13,     5,      -14,    6,      2,      -4,     -9,
        13,     14,     0,      -14,    -8,     -7,     -1,     -9,
        29,     21,     2,      -22,    -19,    -13,    10,     0,
        26,     20,     -2,     -17,    -16,    -9,     7,      -2,
        14,     11,     -4,     -12,    -3,     -3,     -1,     -10,
        18,     13,     10,     -17,    9,      7,      0,      -5,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -155,   -38,    -58,    -43,    -22,    -26,    -27,    -99,
        -50,    -27,    -2,     18,     20,     24,     -11,    -19,
        -34,    3,      26,     60,     66,     46,     41,     -1,
        -8,     31,     47,     67,     66,     67,     49,     22,
        -3,     24,     51,     52,     64,     66,     48,     18,
        -31,    6,      24,     52,     63,     38,     32,     0,
        -46,    -12,    6,      16,     17,     17,     -8,     -20,
        -172,   -42,    -56,    -30,    -19,    -17,    -36,    -96,
];

public static readonly int[] EndGameKnightTable =
[
        -79,    -57,    -14,    -12,    -9,     -30,    -48,    -97,
        -20,    -3,     18,     9,      10,     8,      -14,    -20,
        -8,     15,     36,     39,     38,     18,     7,      -17,
        8,      21,     52,     52,     55,     48,     23,     -5,
        9,      29,     49,     53,     55,     42,     29,     -1,
        -13,    21,     27,     47,     34,     19,     6,      -13,
        -33,    -1,     5,      13,     6,      4,      -17,    -22,
        -85,    -56,    -10,    -14,    -10,    -24,    -47,    -88,
];

public static readonly int[] MiddleGameBishopTable =
[
        -19,    14,     -10,    -26,    -16,    -23,    -20,    7,
        7,      5,      7,      -18,    3,      -2,     32,     -7,
        -6,     7,      -3,     2,      -7,     14,     10,     33,
        -3,     -9,     -8,     19,     18,     -18,    4,      1,
        -13,    0,      -14,    18,     4,      -10,    -5,     9,
        5,      6,      8,      -7,     4,      6,      11,     28,
        9,      18,     13,     -5,     -4,     -1,     23,     2,
        15,     14,     5,      -38,    -18,    -25,    -6,     -12,
];

public static readonly int[] EndGameBishopTable =
[
        -11,    20,     -22,    6,      -5,     4,      -4,     -39,
        -4,     -5,     -4,     6,      2,      -10,    -2,     -16,
        15,     17,     9,      7,      15,     7,      3,      6,
        14,     11,     10,     1,      -1,     10,     5,      1,
        10,     11,     8,      5,      -9,     3,      5,      3,
        15,     5,      1,      3,      7,      0,      2,      6,
        -16,    -12,    -19,    2,      1,      0,      0,      -13,
        -11,    -14,    -18,    6,      9,      6,      -1,     -22,
];

public static readonly int[] MiddleGameRookTable =
[
        -6,     -16,    -15,    -10,    7,      -6,     3,      -7,
        -30,    -12,    -13,    -13,    -1,     0,      20,     -4,
        -31,    -16,    -17,    -10,    8,      16,     60,     31,
        -28,    -18,    -13,    -8,     1,      7,      44,     29,
        -19,    -13,    -7,     2,      0,      13,     33,     26,
        -26,    -13,    -9,     1,      8,      24,     58,     36,
        -29,    -27,    -8,     -9,     -1,     0,      25,     -1,
        -5,     -8,     -7,     3,      16,     -1,     11,     8,
];

public static readonly int[] EndGameRookTable =
[
        6,      9,      16,     4,      -6,     6,      7,      -1,
        14,     21,     21,     10,     -2,     3,      -4,     2,
        8,      6,      9,      3,      -10,    -12,    -25,    -27,
        11,     6,      11,     7,      -6,     -2,     -16,    -24,
        10,     7,      12,     1,      -4,     -11,    -17,    -20,
        10,     13,     1,      -7,     -13,    -16,    -25,    -19,
        20,     23,     16,     5,      -5,     0,      -6,     6,
        0,      0,      9,      -1,     -13,    -1,     -5,     -13,
];

public static readonly int[] MiddleGameQueenTable =
[
        -19,    -22,    -22,    -3,     -13,    -38,    1,      -3,
        5,      -7,     8,      0,      4,      6,      22,     53,
        -3,     -3,     -5,     -5,     -8,     11,     39,     59,
        -6,     -18,    -13,    -6,     -8,     0,      18,     30,
        -8,     -11,    -15,    -15,    -7,     1,      10,     24,
        -2,     -2,     -11,    -9,     -2,     5,      25,     40,
        -10,    -21,    4,      11,     7,      0,      12,     41,
        -14,    -29,    -14,    1,      -9,     -50,    -20,    15,
];

public static readonly int[] EndGameQueenTable =
[
        -24,    -13,    1,      -10,    -5,     -4,     -38,    2,
        -26,    -18,    -27,    -2,     0,      -17,    -44,    -5,
        -18,    -5,     -1,     -1,     20,     21,     -13,    9,
        -14,    7,      -6,     11,     25,     35,     41,     31,
        0,      1,      9,      23,     20,     26,     26,     46,
        -17,    -11,    11,     9,      15,     24,     17,     22,
        -15,    -7,     -21,    -19,    -7,     -4,     -37,    4,
        -9,     -5,     -1,     -11,    4,      30,     19,     -8,
];

public static readonly int[] MiddleGameKingTable =
[
        53,     65,     33,     -77,    7,      -63,    47,     72,
        4,      -4,     -28,    -71,    -83,    -55,    -2,     30,
        -85,    -77,    -123,   -127,   -133,   -145,   -94,    -100,
        -136,   -135,   -163,   -205,   -197,   -181,   -183,   -215,
        -86,    -109,   -150,   -188,   -195,   -153,   -184,   -193,
        -73,    -36,    -114,   -127,   -105,   -117,   -81,    -86,
        105,    4,      -25,    -59,    -62,    -41,    16,     41,
        66,     90,     42,     -63,    21,     -48,    65,     87,
];

public static readonly int[] EndGameKingTable =
[
        -90,    -54,    -25,    4,      -38,    -2,     -41,    -98,
        -21,    17,     31,     47,     54,     37,     15,     -21,
        5,      46,     69,     82,     84,     76,     51,     25,
        17,     63,     91,     113,    107,    96,     80,     53,
        2,      53,     87,     107,    111,    92,     83,     47,
        2,      39,     67,     81,     78,     68,     50,     21,
        -50,    9,      29,     43,     44,     33,     9,      -28,
        -103,   -64,    -29,    -2,     -33,    -7,     -47,    -104,
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
