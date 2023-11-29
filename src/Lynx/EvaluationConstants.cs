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
        +101, +382, +350, +498, +1093, 0,
        -101, -382, -350, -498, -1093, 0
];

public static readonly int[] EndGamePieceValues =
[
        +152, +489, +439, +901, +1555, 0,
        -152, -489, -439, -901, -1555, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -31,    -21,    -13,    -7,     -4,     22,     27,     -17,
        -29,    -21,    -4,     12,     19,     24,     21,     5,
        -22,    -14,    6,      22,     29,     31,     4,      -4,
        -21,    -9,     4,      23,     29,     30,     3,      -6,
        -25,    -18,    -2,     5,      14,     19,     16,     1,
        -31,    -20,    -18,    -13,    -5,     17,     17,     -23,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        12,     10,     7,      -9,     5,      4,      -3,     -11,
        9,      10,     -1,     -15,    -8,     -6,     -3,     -11,
        24,     19,     -1,     -22,    -18,    -13,    8,      -1,
        22,     18,     -3,     -18,    -17,    -10,    6,      -3,
        11,     8,      -5,     -14,    -4,     -2,     -2,     -12,
        14,     12,     10,     0,      11,     8,      0,      -8,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -104,   -34,    -39,    -33,    -21,    -26,    -21,    -73,
        -48,    -31,    -3,     13,     14,     17,     -15,    -22,
        -30,    2,      19,     46,     51,     38,     29,     -4,
        -7,     29,     43,     58,     58,     60,     47,     18,
        -4,     26,     47,     48,     57,     56,     46,     18,
        -27,    2,      18,     38,     46,     32,     22,     -7,
        -50,    -26,    -5,     12,     13,     8,      -14,    -19,
        -103,   -36,    -45,    -35,    -21,    -22,    -28,    -69,
];

public static readonly int[] EndGameKnightTable =
[
        -97,    -69,    -23,    -16,    -12,    -28,    -62,    -109,
        -21,    0,      15,     10,     14,     8,      -12,    -21,
        -12,    17,     42,     45,     42,     23,     13,     -14,
        8,      24,     55,     57,     60,     52,     27,     -5,
        5,      31,     53,     57,     63,     50,     34,     0,
        -16,    23,     32,     51,     42,     24,     10,     -12,
        -26,    3,      6,      15,     9,      5,      -10,    -23,
        -106,   -67,    -17,    -13,    -9,     -25,    -59,    -96,
];

public static readonly int[] MiddleGameBishopTable =
[
        11,     1,      -10,    -20,    -18,    -22,    -11,    17,
        5,      3,      4,      -18,    -2,     -6,     26,     -11,
        -6,     7,      -4,     2,      -8,     10,     7,      27,
        -3,     -4,     -6,     21,     19,     -15,    3,      -1,
        -12,    -2,     -13,    16,     7,      -12,    -4,     4,
        2,      6,      2,      -5,     3,      4,      10,     22,
        9,      12,     6,      -9,     -5,     -2,     19,     -4,
        24,     4,      0,      -28,    -17,    -27,    3,      11,
];

public static readonly int[] EndGameBishopTable =
[
        -28,    13,     -24,    -1,     -3,     2,      -5,     -38,
        -3,     -6,     -4,     5,      5,      -5,     -1,     -17,
        15,     17,     8,      4,      12,     6,      9,      11,
        11,     8,      8,      -1,     -5,     6,      6,      8,
        7,      10,     7,      3,      -9,     6,      6,      7,
        14,     5,      2,      2,      7,      2,      4,      8,
        -15,    -10,    -15,    3,      3,      1,      -1,     -9,
        -15,    -7,     -16,    1,      4,      3,      0,      -21,
];

public static readonly int[] MiddleGameRookTable =
[
        -12,    -16,    -13,    -6,     5,      -6,     5,      -7,
        -33,    -16,    -16,    -16,    -4,     -2,     10,     -12,
        -31,    -11,    -16,    -6,     8,      9,      54,     23,
        -19,    -11,    -9,     1,      6,      15,     47,     24,
        -14,    -7,     -3,     9,      2,      13,     37,     19,
        -25,    -5,     -11,    2,      4,      20,     53,     25,
        -28,    -25,    -8,     -8,     -3,     -2,     18,     -2,
        -7,     -10,    -7,     2,      13,     -1,     10,     2,
];

public static readonly int[] EndGameRookTable =
[
        6,      7,      12,     2,      -5,     6,      3,      -4,
        16,     20,     20,     10,     0,      4,      0,      9,
        9,      5,      9,      3,      -10,    -7,     -21,    -18,
        9,      7,      10,     4,      -4,     0,      -14,    -17,
        8,      6,      11,     -1,     -1,     -6,     -13,    -14,
        11,     9,      2,      -6,     -11,    -11,    -20,    -12,
        20,     23,     15,     5,      -2,     1,      -1,     6,
        -1,     1,      6,      -2,     -12,    -1,     -4,     -12,
];

public static readonly int[] MiddleGameQueenTable =
[
        -5,     -14,    -14,    0,      -5,     -18,    7,      -8,
        -2,     -5,     9,      0,      3,      8,      18,     16,
        -2,     0,      -5,     -6,     -7,     7,      36,     50,
        -8,     -7,     -13,    -5,     -5,     1,      14,     30,
        -7,     -7,     -13,    -15,    -5,     -2,     15,     22,
        -2,     1,      -10,    -9,     -3,     5,      22,     37,
        -11,    -17,    5,      9,      7,      3,      5,      6,
        -15,    -16,    -11,    1,      -9,     -28,    -16,    -3,
];

public static readonly int[] EndGameQueenTable =
[
        -29,    -19,    -7,     -17,    -17,    -31,    -28,    10,
        -12,    -17,    -31,    -5,     -4,     -22,    -47,    28,
        -16,    -9,     2,      -5,     13,     21,     -8,     8,
        -6,     2,      5,      11,     22,     34,     48,     27,
        1,      4,      9,      22,     21,     32,     23,     48,
        -15,    -13,    8,      8,      12,     21,     19,     23,
        -13,    -10,    -28,    -20,    -13,    -17,    -31,    32,
        -5,     -14,    -10,    -10,    0,      6,      16,     18,
];

public static readonly int[] MiddleGameKingTable =
[
        46,     54,     18,     -49,    4,      -62,    43,     63,
        34,     7,      -25,    -72,    -83,    -58,    -4,     28,
        -78,    -62,    -95,    -109,   -119,   -125,   -80,    -98,
        -115,   -102,   -113,   -161,   -152,   -139,   -136,   -179,
        -79,    -66,    -110,   -147,   -159,   -129,   -142,   -170,
        -61,    -39,    -79,    -107,   -91,    -106,   -71,    -90,
        74,     9,      -21,    -60,    -64,    -45,    11,     36,
        63,     74,     34,     -38,    16,     -55,    56,     76,
];

public static readonly int[] EndGameKingTable =
[
        -90,    -50,    -22,    -9,     -43,    -3,     -37,    -96,
        -28,    10,     30,     44,     51,     37,     15,     -23,
        10,     41,     62,     74,     79,     69,     46,     24,
        17,     57,     81,     98,     96,     86,     72,     44,
        4,      47,     80,     96,     100,    85,     74,     41,
        5,      39,     59,     74,     71,     65,     46,     19,
        -37,    8,      27,     40,     42,     32,     9,      -28,
        -104,   -55,    -28,    -15,    -37,    -6,     -42,    -102,
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
