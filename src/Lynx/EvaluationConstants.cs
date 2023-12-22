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
        +97, +378, +363, +448, +1032, 0,
        -97, -378, -363, -448, -1032, 0
];

public static readonly int[] EndGamePieceValues =
[
        +150, +485, +503, +899, +1797, 0,
        -150, -485, -503, -899, -1797, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -24,    -22,    -13,    -23,    -6,     30,     34,     -10,
        -25,    -22,    -7,     3,      14,     22,     30,     11,
        -22,    -12,    3,      15,     26,     31,     7,      0,
        -21,    -8,     0,      16,     26,     26,     5,      -1,
        -23,    -17,    -5,     -4,     10,     19,     22,     5,
        -24,    -19,    -18,    -27,    -11,    23,     23,     -17,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        13,     12,     7,      -14,    6,      2,      -4,     -12,
        11,     12,     0,      -12,    -7,     -4,     -4,     -11,
        28,     19,     -1,     -22,    -19,    -13,    7,      -2,
        25,     18,     -2,     -16,    -16,    -9,     4,      -4,
        12,     9,      -4,     -10,    -2,     -2,     -3,     -12,
        16,     13,     10,     -15,    18,     6,      -1,     -9,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -150,   -24,    -61,    -41,    -19,    -21,    -9,     -94,
        -42,    -25,    -2,     6,      12,     23,     -13,    -11,
        -24,    5,      18,     53,     53,     34,     40,     3,
        -5,     30,     42,     54,     59,     60,     49,     25,
        -3,     28,     44,     41,     56,     56,     51,     23,
        -22,    8,      18,     43,     50,     28,     33,     2,
        -44,    -16,    -1,     5,      10,     16,     -11,    -14,
        -169,   -28,    -58,    -32,    -16,    -13,    -18,    -88,
];

public static readonly int[] EndGameKnightTable =
[
        -75,    -73,    -12,    -13,    -11,    -30,    -67,    -95,
        -24,    -2,     15,     14,     13,     9,      -13,    -22,
        -14,    16,     41,     41,     41,     24,     9,      -16,
        6,      20,     54,     56,     58,     52,     26,     -7,
        4,      27,     53,     59,     60,     49,     31,     -1,
        -18,    19,     30,     48,     39,     26,     6,      -12,
        -28,    1,      5,      18,     8,      1,      -14,    -25,
        -79,    -73,    -7,     -15,    -12,    -27,    -63,    -94,
];

public static readonly int[] MiddleGameBishopTable =
[
        -22,    10,     -24,    -30,    -30,    -31,    -30,    -6,
        12,     4,      12,     -26,    -6,     0,      28,     -5,
        -1,     14,     -5,     9,      -7,     9,      12,     37,
        -6,     6,      7,      23,     24,     -5,     15,     3,
        -13,    10,     -2,     18,     11,     -2,     7,      10,
        10,     14,     6,      -2,     6,      1,      13,     32,
        13,     14,     14,     -14,    -10,    4,      19,     4,
        4,      12,     -9,     -46,    -33,    -35,    -4,     -21,
];

public static readonly int[] EndGameBishopTable =
[
        -36,    4,      -40,    -6,     -14,    -15,    -11,    -53,
        -21,    -12,    -1,     9,      13,     -8,     -7,     -34,
        2,      17,     25,     21,     31,     21,     9,      -3,
        5,      17,     27,     28,     24,     27,     14,     -6,
        -1,     18,     26,     34,     21,     25,     13,     -6,
        0,      4,      16,     20,     24,     18,     3,      -6,
        -35,    -14,    -12,    9,      11,     1,      -4,     -32,
        -29,    -25,    -29,    -2,     -3,     -7,     -13,    -41,
];

public static readonly int[] MiddleGameRookTable =
[
        -5,     -17,    -14,    -7,     7,      -3,     3,      -4,
        -27,    -14,    -19,    -22,    -9,     -3,     17,     -3,
        -28,    -14,    -22,    -11,    3,      4,      60,     35,
        -19,    -14,    -9,     -5,     3,      13,     55,     34,
        -14,    -6,     -4,     0,      -2,     10,     44,     29,
        -21,    -11,    -15,    -7,     -1,     16,     56,     35,
        -24,    -24,    -13,    -15,    -9,     -6,     26,     2,
        -1,     -8,     -8,     3,      15,     2,      11,     9,
];

public static readonly int[] EndGameRookTable =
[
        3,      7,      14,     3,      -6,     4,      5,      -6,
        14,     20,     22,     12,     1,      2,      -4,     2,
        8,      6,      11,     4,      -9,     -8,     -23,    -23,
        10,     8,      9,      5,      -4,     -2,     -19,    -22,
        10,     6,      11,     2,      -2,     -7,     -17,    -17,
        10,     12,     2,      -4,     -10,    -12,    -23,    -16,
        19,     22,     17,     7,      -1,     1,      -6,     4,
        -3,     -1,     7,      -2,     -14,    -3,     -5,     -15,
];

public static readonly int[] MiddleGameQueenTable =
[
        -18,    -26,    -27,    -5,     -19,    -48,    1,      -1,
        4,      -5,     5,      -12,    -5,     2,      22,     55,
        1,      1,      -10,    -11,    -13,    3,      43,     71,
        0,      -4,     -9,     -5,     -5,     4,      27,     41,
        -2,     0,      -9,     -15,    -6,     2,      24,     37,
        4,      3,      -14,    -17,    -11,    2,      27,     50,
        -10,    -14,    1,      0,      0,      -2,     8,      40,
        -11,    -27,    -17,    -3,     -13,    -55,    -24,    26,
];

public static readonly int[] EndGameQueenTable =
[
        -47,    -28,    0,      -24,    -9,     -20,    -61,    -24,
        -52,    -20,    -23,    16,     10,     -12,    -58,    -47,
        -46,    -6,     32,     33,     52,     43,     -17,    -41,
        -39,    9,      33,     61,     75,     64,     38,     -4,
        -24,    4,      41,     76,     73,     60,     17,     11,
        -45,    -14,    44,     51,     54,     45,     17,     -21,
        -49,    -14,    -15,    3,      3,      -1,     -41,    -32,
        -38,    -18,    -6,     -14,    -2,     17,     -2,     -36,
];

public static readonly int[] MiddleGameKingTable =
[
        41,     56,     29,     -86,    5,      -69,    46,     62,
        -1,     -10,    -32,    -76,    -90,    -58,    -4,     28,
        -87,    -75,    -121,   -125,   -136,   -142,   -88,    -103,
        -127,   -133,   -150,   -191,   -178,   -169,   -165,   -193,
        -95,    -96,    -140,   -170,   -182,   -152,   -169,   -189,
        -84,    -46,    -116,   -126,   -107,   -121,   -79,    -94,
        89,     0,      -35,    -68,    -72,    -47,    11,     37,
        57,     86,     43,     -71,    16,     -58,    60,     76,
];

public static readonly int[] EndGameKingTable =
[
        -87,    -50,    -25,    4,      -45,    -3,     -40,    -95,
        -23,    15,     28,     43,     51,     35,     13,     -25,
        6,      42,     64,     76,     80,     71,     47,     25,
        16,     61,     85,     102,    98,     90,     75,     47,
        4,      49,     82,     98,     103,    89,     78,     46,
        5,      39,     63,     77,     73,     66,     47,     20,
        -49,    8,      29,     40,     42,     31,     8,      -29,
        -101,   -63,    -32,    -3,     -36,    -7,     -45,    -101,
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
