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
        +102, +381, +355, +502, +1115, 0,
        -102, -381, -355, -502, -1115, 0
];

public static readonly int[] EndGamePieceValues =
[
        +146, +472, +420, +871, +1486, 0,
        -146, -472, -420, -871, -1486, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -29,    -22,    -17,    -11,    -6,     25,     32,     -16,
        -26,    -22,    -6,     9,      17,     28,     27,     7,
        -20,    -12,    6,      20,     27,     34,     7,      -3,
        -20,    -9,     3,      19,     28,     30,     6,      -4,
        -23,    -20,    -4,     2,      14,     23,     18,     3,
        -29,    -21,    -20,    -17,    -8,     20,     21,     -21,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     12,     8,      -16,    6,      4,      -4,     -11,
        11,     11,     -1,     -14,    -7,     -5,     -5,     -11,
        28,     19,     -1,     -21,    -16,    -12,    6,      -2,
        24,     19,     -2,     -16,    -15,    -9,     4,      -4,
        13,     10,     -4,     -12,    -3,     -3,     -3,     -12,
        17,     12,     10,     -11,    14,     8,      -1,     -8,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -155,   -25,    -59,    -41,    -22,    -26,    -11,    -95,
        -45,    -27,    -2,     15,     17,     21,     -11,    -16,
        -20,    4,      21,     61,     60,     42,     37,     0,
        -7,     28,     44,     59,     61,     62,     51,     22,
        -5,     29,     46,     49,     58,     57,     53,     21,
        -22,    8,      20,     49,     59,     36,     31,     -4,
        -45,    -19,    1,      15,     17,     13,     -11,    -16,
        -171,   -28,    -58,    -35,    -16,    -17,    -16,    -87,
];

public static readonly int[] EndGameKnightTable =
[
        -67,    -76,    -13,    -13,    -9,     -25,    -69,    -88,
        -23,    1,      15,     9,      12,     8,      -13,    -19,
        -19,    14,     40,     38,     39,     21,     12,     -17,
        4,      19,     51,     53,     57,     51,     28,     -4,
        4,      27,     49,     54,     60,     50,     33,     1,
        -20,    18,     30,     45,     36,     20,     8,      -10,
        -28,    2,      5,      11,     4,      0,      -14,    -24,
        -68,    -77,    -7,     -14,    -12,    -27,    -71,    -90,
];

public static readonly int[] MiddleGameBishopTable =
[
        -16,    7,      -8,     -27,    -23,    -20,    -28,    -1,
        6,      4,      9,      -18,    1,      -4,     30,     -10,
        -4,     10,     -2,     6,      -4,     15,     8,      31,
        -6,     0,      -2,     27,     23,     -14,    7,      2,
        -10,    2,      -10,    21,     11,     -10,    0,      10,
        5,      9,      7,      -2,     7,      8,      11,     26,
        9,      13,     13,     -7,     -1,     -1,     24,     -4,
        10,     15,     2,      -44,    -24,    -24,    -4,     -12,
];

public static readonly int[] EndGameBishopTable =
[
        -10,    18,     -26,    3,      0,      -2,     3,      -30,
        -2,     -6,     -4,     5,      3,      -8,     -1,     -14,
        14,     16,     8,      2,      13,     4,      7,      10,
        12,     7,      7,      -1,     -8,     8,      6,      4,
        6,      11,     8,      3,      -8,     6,      3,      5,
        12,     4,      2,      2,      8,      0,      5,      8,
        -12,    -9,     -13,    4,      -1,     -1,     -4,     -9,
        -8,     -11,    -17,    5,      7,      2,      2,      -16,
];

public static readonly int[] MiddleGameRookTable =
[
        -10,    -17,    -14,    -8,     5,      -6,     2,      -5,
        -33,    -17,    -16,    -15,    -6,     0,      16,     -7,
        -33,    -14,    -18,    -6,     6,      12,     57,     29,
        -26,    -16,    -11,    0,      3,      16,     53,     26,
        -19,    -12,    -10,    7,      1,      16,     45,     22,
        -28,    -10,    -14,    0,      6,      21,     56,     34,
        -26,    -22,    -9,     -9,     -4,     -3,     22,     -1,
        -6,     -10,    -10,    0,      12,     -2,     10,     7,
];

public static readonly int[] EndGameRookTable =
[
        5,      6,      11,     2,      -8,     4,      3,      -7,
        17,     20,     20,     9,      -1,     0,      -4,     3,
        11,     6,      10,     1,      -9,     -6,     -22,    -20,
        12,     12,     11,     4,      -3,     -1,     -18,    -20,
        11,     8,      13,     1,      -1,     -5,     -18,    -15,
        13,     10,     4,      -3,     -11,    -11,    -23,    -14,
        20,     21,     14,     5,      -3,     0,      -4,     4,
        -2,     0,      7,      -2,     -13,    -2,     -5,     -17,
];

public static readonly int[] MiddleGameQueenTable =
[
        -15,    -21,    -16,    -1,     -9,     -45,    1,      0,
        -2,     -10,    8,      1,      3,      4,      19,     47,
        -7,     -1,     -7,     -6,     -6,     10,     37,     59,
        -7,     -15,    -14,    -5,     -3,     5,      17,     28,
        -11,    -11,    -13,    -13,    -2,     1,      17,     26,
        -3,     -1,     -10,    -9,     -4,     6,      24,     43,
        -16,    -16,    5,      7,      7,      0,      5,      37,
        -8,     -22,    -9,     -1,     -9,     -51,    -18,    34,
];

public static readonly int[] EndGameQueenTable =
[
        -21,    -12,    -10,    -19,    -8,     1,      -28,    18,
        -19,    -10,    -31,    -7,     -5,     -20,    -45,    0,
        -12,    -8,     3,      -4,     13,     19,     -4,     8,
        -15,    6,      0,      11,     20,     33,     45,     40,
        0,      1,      6,      22,     18,     31,     27,     50,
        -18,    -13,    8,      7,      13,     21,     22,     17,
        -11,    -7,     -26,    -16,    -13,    -14,    -37,    8,
        -13,    -7,     -8,     -8,     -2,     26,     17,     1,
];

public static readonly int[] MiddleGameKingTable =
[
        33,     52,     22,     -85,    6,      -69,    43,     59,
        -2,     -10,    -31,    -70,    -82,    -58,    -3,     25,
        -81,    -66,    -110,   -106,   -117,   -127,   -82,    -95,
        -103,   -98,    -121,   -156,   -150,   -141,   -136,   -176,
        -70,    -74,    -108,   -142,   -152,   -131,   -148,   -173,
        -79,    -41,    -94,    -107,   -92,    -111,   -76,    -97,
        79,     -3,     -30,    -62,    -64,    -47,    10,     34,
        49,     79,     37,     -71,    13,     -60,    56,     73,
];

public static readonly int[] EndGameKingTable =
[
        -82,    -47,    -22,    4,      -46,    -3,     -38,    -93,
        -21,    15,     28,     41,     48,     35,     13,     -25,
        6,      42,     61,     72,     75,     68,     46,     23,
        14,     54,     78,     93,     90,     84,     68,     44,
        0,      47,     76,     90,     94,     82,     73,     42,
        6,      38,     60,     72,     68,     63,     45,     22,
        -45,    10,     28,     38,     41,     31,     8,      -29,
        -95,    -60,    -30,    -3,     -37,    -6,     -42,    -100,
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
