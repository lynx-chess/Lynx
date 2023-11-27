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
        +102, +381, +357, +502, +1118, 0,
        -102, -381, -357, -502, -1118, 0
];

public static readonly int[] EndGamePieceValues =
[
        +145, +465, +412, +861, +1467, 0,
        -145, -465, -412, -861, -1467, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -28,    -22,    -19,    -11,    -7,     25,     32,     -15,
        -25,    -22,    -6,     7,      16,     27,     27,     8,
        -19,    -11,    6,      19,     27,     34,     9,      -3,
        -20,    -8,     2,      19,     26,     30,     7,      -5,
        -22,    -19,    -5,     2,      14,     23,     18,     3,
        -28,    -21,    -20,    -16,    -8,     20,     21,     -20,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     11,     9,      -16,    6,      4,      -4,     -12,
        11,     10,     -2,     -14,    -7,     -5,     -5,     -12,
        27,     19,     -1,     -22,    -16,    -12,    5,      -3,
        24,     18,     -2,     -16,    -14,    -10,    4,      -5,
        13,     9,      -4,     -13,    -4,     -4,     -4,     -13,
        16,     11,     9,      -11,    12,     8,      -1,     -9,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -160,   -23,    -60,    -42,    -22,    -28,    -10,    -96,
        -47,    -26,    -2,     15,     16,     19,     -13,    -15,
        -17,    4,      20,     58,     58,     41,     35,     1,
        -6,     28,     45,     58,     59,     61,     51,     23,
        -5,     30,     45,     49,     56,     56,     53,     22,
        -23,    9,      19,     49,     59,     36,     32,     -4,
        -43,    -21,    1,      14,     15,     12,     -10,    -16,
        -167,   -26,    -60,    -36,    -17,    -18,    -14,    -82,
];

public static readonly int[] EndGameKnightTable =
[
        -63,    -79,    -13,    -13,    -9,     -24,    -70,    -83,
        -22,    3,      15,     11,     13,     8,      -11,    -19,
        -20,    16,     42,     40,     40,     21,     14,     -17,
        2,      22,     50,     53,     58,     52,     30,     -2,
        3,      25,     48,     55,     61,     51,     35,     4,
        -19,    19,     31,     45,     37,     21,     9,      -10,
        -29,    3,      6,      12,     6,      1,      -14,    -22,
        -66,    -78,    -5,     -13,    -11,    -26,    -73,    -94,
];

public static readonly int[] MiddleGameBishopTable =
[
        -14,    7,      -9,     -28,    -25,    -20,    -40,    -2,
        6,      4,      10,     -19,    1,      -6,     28,     -11,
        -4,     10,     -3,     6,      -5,     14,     8,      30,
        -5,     1,      -1,     27,     22,     -15,    7,      1,
        -10,    2,      -10,    21,     12,     -9,     0,      10,
        2,      9,      6,      -2,     6,      8,      11,     23,
        8,      12,     12,     -8,     -3,     -3,     23,     -5,
        9,      14,     1,      -47,    -27,    -25,    -5,     -11,
];

public static readonly int[] EndGameBishopTable =
[
        -9,     17,     -25,    3,      2,      -3,     8,      -30,
        -1,     -6,     -3,     6,      3,      -6,     -1,     -12,
        15,     16,     7,      2,      12,     4,      8,      9,
        11,     7,      6,      -1,     -9,     9,      8,      5,
        6,      11,     7,      1,      -7,     5,      5,      6,
        14,     6,      4,      2,      7,      -1,     5,      10,
        -11,    -9,     -12,    4,      -1,     -1,     -2,     -11,
        -9,     -9,     -17,    8,      8,      2,      5,      -16,
];

public static readonly int[] MiddleGameRookTable =
[
        -10,    -16,    -14,    -8,     5,      -7,     2,      -5,
        -33,    -16,    -16,    -15,    -6,     0,      17,     -7,
        -32,    -14,    -18,    -5,     7,      12,     57,     29,
        -28,    -16,    -10,    2,      4,      15,     53,     27,
        -19,    -12,    -8,     9,      3,      18,     51,     26,
        -26,    -8,     -13,    2,      6,      20,     55,     33,
        -26,    -21,    -7,     -8,     -3,     -2,     23,     0,
        -6,     -10,    -9,     0,      12,     -1,     9,      7,
];

public static readonly int[] EndGameRookTable =
[
        4,      4,      10,     1,      -8,     4,      3,      -7,
        17,     20,     20,     9,      -1,     0,      -6,     3,
        11,     7,      9,      0,      -8,     -6,     -21,    -19,
        12,     12,     11,     4,      -3,     2,      -18,    -19,
        10,     9,      12,     1,      0,      -5,     -19,    -15,
        12,     10,     5,      -4,     -10,    -10,    -23,    -13,
        19,     21,     13,     5,      -4,     0,      -5,     3,
        -3,     0,      6,      -2,     -13,    -2,     -5,     -17,
];

public static readonly int[] MiddleGameQueenTable =
[
        -14,    -22,    -16,    -1,     -9,     -47,    -2,     -3,
        -4,     -11,    8,      1,      4,      2,      21,     47,
        -6,     -2,     -8,     -5,     -5,     9,      36,     58,
        -7,     -13,    -13,    -4,     -2,     6,      19,     31,
        -11,    -12,    -12,    -13,    1,      3,      17,     29,
        -6,     -2,     -10,    -9,     -4,     6,      24,     45,
        -16,    -16,    4,      7,      6,      1,      6,      37,
        -9,     -23,    -8,     -2,     -10,    -53,    -22,    31,
];

public static readonly int[] EndGameQueenTable =
[
        -20,    -11,    -12,    -21,    -10,    2,      -24,    26,
        -19,    -8,     -30,    -9,     -10,    -21,    -50,    2,
        -12,    -7,     4,      -4,     11,     20,     -4,     12,
        -15,    6,      0,      10,     19,     32,     42,     37,
        -1,     2,      5,      25,     15,     31,     27,     46,
        -16,    -12,    10,     7,      10,     21,     22,     15,
        -11,    -6,     -25,    -15,    -14,    -18,    -40,    8,
        -11,    -5,     -9,     -10,    -3,     24,     22,     3,
];

public static readonly int[] MiddleGameKingTable =
[
        27,     51,     20,     -86,    5,      -72,    42,     57,
        -3,     -12,    -33,    -69,    -80,    -57,    -2,     26,
        -83,    -70,    -105,   -102,   -110,   -124,   -79,    -96,
        -96,    -93,    -116,   -140,   -141,   -132,   -130,   -172,
        -75,    -69,    -101,   -133,   -138,   -124,   -135,   -170,
        -74,    -37,    -92,    -97,    -91,    -106,   -77,    -97,
        73,     -6,     -31,    -62,    -65,    -47,    10,     33,
        44,     75,     35,     -76,    12,     -62,    54,     70,
];

public static readonly int[] EndGameKingTable =
[
        -81,    -45,    -22,    3,      -47,    -3,     -37,    -92,
        -23,    15,     27,     40,     45,     33,     13,     -26,
        5,      41,     59,     69,     73,     67,     44,     23,
        12,     53,     76,     88,     87,     81,     66,     42,
        1,      46,     73,     87,     89,     79,     69,     40,
        5,      37,     58,     68,     66,     61,     45,     22,
        -43,    10,     28,     37,     39,     31,     8,      -29,
        -93,    -59,    -29,    -2,     -40,    -7,     -42,    -99,
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
