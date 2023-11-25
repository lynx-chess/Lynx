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
        +103, +370, +358, +503, +1178, 0,
        -103, -370, -358, -503, -1178, 0
];

public static readonly int[] EndGamePieceValues =
[
        +125, +380, +323, +715, +1144, 0,
        -125, -380, -323, -715, -1144, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -21,    -28,    -26,    -16,    -17,    26,     32,     -6,
        -17,    -24,    -11,    -2,     5,      26,     26,     9,
        -14,    -7,     2,      12,     19,     30,     18,     -3,
        -16,    -7,     0,      10,     16,     29,     21,     -2,
        -18,    -23,    -10,    -4,     5,      25,     23,     11,
        -26,    -27,    -26,    -21,    -16,    26,     28,     -7,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     11,     9,      -17,    7,      10,     1,      -11,
        11,     10,     1,      -12,    0,      -2,     -10,    -15,
        25,     19,     0,      -15,    -7,     -9,     -1,     -6,
        25,     19,     -1,     -14,    -7,     -8,     -2,     -7,
        12,     9,      0,      -11,    0,      -1,     -10,    -16,
        16,     10,     10,     -11,    7,      12,     0,      -12,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -154,   -12,    -67,    -47,    -17,    -23,    -2,     -95,
        -44,    -29,    0,      14,     10,     12,     -18,    0,
        -10,    7,      15,     48,     48,     36,     29,     4,
        -2,     31,     41,     47,     50,     49,     54,     31,
        -1,     29,     39,     47,     49,     46,     57,     28,
        -13,    9,      16,     45,     46,     35,     28,     -1,
        -34,    -33,    1,      14,     11,     14,     -14,    -5,
        -137,   -12,    -57,    -41,    -17,    -38,    0,      -85,
];

public static readonly int[] EndGameKnightTable =
[
        -38,    -84,    -9,     -8,     -17,    -29,    -78,    -51,
        -23,    7,      7,      7,      7,      -2,     -7,     -26,
        -31,    17,     38,     41,     35,     21,     16,     -14,
        -4,     20,     45,     49,     56,     53,     38,     5,
        -6,     15,     44,     50,     61,     52,     42,     13,
        -32,    16,     29,     38,     35,     21,     21,     -15,
        -34,    3,      3,      9,      8,      -1,     -6,     -31,
        -42,    -88,    -7,     -14,    -11,    -22,    -75,    -85,
];

public static readonly int[] MiddleGameBishopTable =
[
        -17,    3,      -8,     -45,    -31,    -16,    -32,    -31,
        10,     1,      8,      -15,    0,      -3,     26,     -2,
        -6,     9,      0,      7,      -2,     17,     15,     23,
        -6,     12,     9,      33,     26,     -7,     10,     13,
        -1,     8,      2,      29,     21,     -4,     6,      11,
        0,      7,      2,      3,      2,      16,     11,     21,
        9,      5,      10,     -12,    1,      -6,     27,     -3,
        1,      10,     -5,     -46,    -33,    -15,    -19,    -24,
];

public static readonly int[] EndGameBishopTable =
[
        -2,     1,      -25,    9,      5,      -13,    13,     11,
        4,      -6,     -4,     0,      -3,     -5,     -6,     -23,
        10,     9,      1,      -3,     7,      -4,     5,      10,
        7,      2,      2,      -6,     -11,    5,      13,     9,
        -1,     4,      3,      -6,     -8,     5,      13,     11,
        6,      8,      3,      -2,     9,      -6,     7,      11,
        0,      -6,     -5,     3,      -5,     -4,     -10,    -18,
        -6,     -3,     -26,    4,      4,      -17,    13,     7,
];

public static readonly int[] MiddleGameRookTable =
[
        -8,     -16,    -14,    -6,     2,      -2,     7,      0,
        -26,    -18,    -17,    -13,    -8,     -6,     8,      -18,
        -31,    -11,    -17,    4,      2,      12,     54,     14,
        -16,    -12,    -6,     15,     12,     33,     66,     33,
        -19,    -13,    -4,     14,     11,     31,     73,     35,
        -27,    -10,    -22,    -1,     -1,     13,     52,     21,
        -22,    -19,    -17,    -10,    -10,    -3,     -3,     -12,
        -5,     -14,    -14,    -6,     2,      0,      4,      7,
];

public static readonly int[] EndGameRookTable =
[
        -6,     -3,     3,      -4,     -16,    -5,     -8,     -27,
        19,     15,     15,     6,      -3,     -2,     -6,     5,
        12,     8,      12,     -2,     -6,     2,      -17,    -6,
        7,      14,     11,     4,      5,      4,      -14,    -12,
        9,      13,     12,     4,      4,      4,      -16,    -14,
        13,     11,     14,     3,      -2,     0,      -17,    -6,
        18,     19,     13,     5,      -1,     -4,     4,      5,
        -7,     -1,     3,      -3,     -11,    -6,     -7,     -24,
];

public static readonly int[] MiddleGameQueenTable =
[
        -6,     -26,    -9,     2,      -11,    -62,    -16,    -3,
        -17,    -15,    4,      -1,     3,      -3,     24,     45,
        -18,    -1,     -8,     -6,     -3,     8,      32,     54,
        -6,     -8,     -7,     1,      15,     17,     23,     35,
        -11,    -8,     -6,     1,      16,     16,     32,     39,
        -13,    -4,     -7,     -8,     -8,     6,      27,     42,
        -22,    -12,    2,      -2,     3,      5,      15,     35,
        -5,     -24,    -7,     -2,     -17,    -62,    -33,    11,
];

public static readonly int[] EndGameQueenTable =
[
        -9,     0,      -24,    -42,    -11,    15,     18,     64,
        -5,     -5,     -31,    -21,    -27,    -28,    -64,    14,
        -2,     -13,    -2,     -10,    -2,     19,     13,     32,
        -10,    4,      2,      8,      2,      25,     45,     60,
        -6,     2,      2,      10,     -1,     25,     30,     53,
        -9,     -8,     1,      -4,     3,      22,     19,     49,
        5,      -6,     -31,    -21,    -27,    -41,    -68,    26,
        -16,    -4,     -23,    -33,    -10,    11,     31,     59,
];

public static readonly int[] MiddleGameKingTable =
[
        -67,    14,     -21,    -102,   -3,     -88,    28,     18,
        -2,     -31,    -29,    -54,    -57,    -51,    -3,     15,
        -55,    -24,    -29,    -19,    -35,    -66,    -62,    -106,
        -22,    16,     1,      7,      -15,    -34,    -62,    -112,
        -30,    26,     1,      4,      -20,    -36,    -51,    -121,
        -53,    -29,    -42,    -26,    -46,    -67,    -70,    -107,
        -14,    -35,    -39,    -65,    -64,    -52,    -3,     11,
        -48,    6,      -17,    -111,   -8,     -94,    27,     21,
];

public static readonly int[] EndGameKingTable =
[
        -46,    -28,    -14,    0,      -53,    -8,     -29,    -81,
        -11,    15,     18,     23,     24,     23,     6,      -30,
        8,      24,     33,     35,     38,     42,     30,     21,
        3,      23,     37,     37,     44,     46,     44,     23,
        -2,     22,     39,     38,     45,     44,     41,     22,
        6,      30,     34,     35,     40,     40,     30,     19,
        -8,     16,     19,     23,     27,     23,     4,      -30,
        -43,    -29,    -13,    3,      -54,    -7,     -29,    -86,
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
