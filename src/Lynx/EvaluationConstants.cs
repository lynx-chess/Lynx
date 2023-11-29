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
        +96, +365, +334, +477, +1039, 0,
        -96, -365, -334, -477, -1039, 0
];

public static readonly int[] EndGamePieceValues =
[
        +149, +478, +427, +879, +1530, 0,
        -149, -478, -427, -879, -1530, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -29,    -17,    -11,    -6,     -4,     18,     19,     -17,
        -26,    -15,    -2,     12,     17,     19,     16,     2,
        -18,    -11,    8,      23,     28,     28,     1,      -5,
        -18,    -8,     5,      23,     28,     26,     0,      -6,
        -24,    -15,    -2,     6,      12,     14,     11,     -1,
        -29,    -19,    -16,    -8,     -6,     14,     12,     -20,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        10,     9,      9,      -5,     4,      5,      -1,     -10,
        7,      8,      -1,     -15,    -8,     -4,     -3,     -10,
        20,     18,     -2,     -22,    -18,    -11,    8,      0,
        18,     17,     -2,     -17,    -17,    -9,     6,      -2,
        8,      7,      -4,     -14,    -5,     -2,     -1,     -11,
        11,     11,     11,     1,      11,     9,      1,      -8,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -71,    -34,    -34,    -30,    -21,    -24,    -21,    -58,
        -44,    -36,    1,      11,     11,     12,     -19,    -23,
        -28,    3,      18,     39,     42,     34,     22,     -5,
        -5,     28,     42,     53,     54,     55,     44,     15,
        -1,     27,     45,     45,     53,     52,     43,     16,
        -26,    1,      17,     32,     38,     29,     17,     -7,
        -46,    -26,    -4,     10,     12,     2,      -20,    -18,
        -73,    -37,    -30,    -31,    -19,    -23,    -27,    -55,
];

public static readonly int[] EndGameKnightTable =
[
        -108,   -67,    -22,    -18,    -14,    -28,    -59,    -108,
        -23,    2,      10,     12,     16,     7,      -12,    -23,
        -11,    16,     41,     45,     45,     26,     15,     -15,
        5,      25,     54,     58,     61,     52,     32,     -4,
        2,      32,     53,     58,     63,     49,     36,     -2,
        -14,    20,     33,     51,     43,     26,     12,     -15,
        -25,    -1,     4,      14,     9,      5,      -10,    -24,
        -106,   -63,    -24,    -12,    -10,    -24,    -53,    -93,
];

public static readonly int[] MiddleGameBishopTable =
[
        17,     -2,     -9,     -15,    -16,    -20,    -12,    17,
        2,      2,      2,      -17,    -2,     -7,     22,     -9,
        -5,     6,      -3,     3,      -6,     9,      7,      23,
        -3,     -3,     -5,     17,     17,     -13,    2,      -2,
        -8,     -1,     -11,    14,     7,      -11,    -2,     2,
        1,      6,      0,      -4,     2,      2,      9,      20,
        8,      9,      4,      -9,     -6,     -3,     17,     -4,
        24,     1,      -2,     -18,    -14,    -24,    0,      14,
];

public static readonly int[] EndGameBishopTable =
[
        -27,    6,      -23,    -2,     -4,     2,      -7,     -36,
        -6,     -8,     -1,     6,      6,      0,      0,      -17,
        15,     18,     8,      4,      10,     6,      10,     14,
        12,     9,      8,      0,      -3,     7,      9,      8,
        8,      11,     7,      3,      -7,     7,      10,     9,
        15,     7,      5,      3,      7,      2,      7,      10,
        -15,    -9,     -13,    5,      3,      0,      -1,     -8,
        -10,    -4,     -14,    -4,     2,      1,      3,      -14,
];

public static readonly int[] MiddleGameRookTable =
[
        -13,    -13,    -10,    -3,     5,      -4,     5,      -7,
        -34,    -19,    -16,    -13,    -3,     -2,     6,      -14,
        -26,    -3,     -12,    0,      7,      8,      46,     15,
        -14,    -4,     -4,     7,      6,      16,     38,     23,
        -10,    2,      -1,     11,     4,      13,     34,     19,
        -23,    1,      -6,     3,      4,      18,     47,     18,
        -28,    -23,    -8,     -6,     -2,     -2,     8,      -2,
        -9,     -9,     -5,     4,      12,     0,      11,     -1,
];

public static readonly int[] EndGameRookTable =
[
        4,      5,      11,     0,      -5,     6,      1,      -4,
        17,     19,     19,     8,      0,      2,      2,      11,
        7,      2,      7,      0,      -7,     -7,     -17,    -15,
        6,      3,      7,      1,      -4,     -2,     -11,    -16,
        4,      1,      9,      -2,     -2,     -5,     -11,    -12,
        9,      5,      0,      -5,     -10,    -9,     -18,    -10,
        21,     20,     14,     4,      -3,     0,      4,      8,
        -1,     1,      5,      -3,     -12,    0,      -5,     -9,
];

public static readonly int[] MiddleGameQueenTable =
[
        -1,     -8,     -10,    1,      -1,     -7,     5,      -1,
        -1,     -5,     9,      0,      3,      9,      14,     1,
        1,      0,      -3,     -6,     -6,     5,      32,     40,
        -6,     -3,     -13,    -6,     -7,     0,      13,     26,
        -4,     -5,     -13,    -15,    -6,     -4,     12,     19,
        0,      1,      -8,     -10,    -4,     4,      19,     28,
        -7,     -12,    6,      8,      6,      2,      2,      -8,
        -7,     -9,     -8,     3,      -7,     -16,    -8,     -5,
];

public static readonly int[] EndGameQueenTable =
[
        -28,    -22,    -12,    -17,    -21,    -38,    -19,    -9,
        -13,    -16,    -29,    -7,     -4,     -28,    -42,    34,
        -12,    -7,     4,      -3,     11,     23,     -5,     15,
        2,      8,      11,     17,     24,     33,     48,     30,
        6,      11,     14,     25,     23,     34,     30,     46,
        -10,    -10,    8,      8,      13,     20,     19,     32,
        -8,     -13,    -30,    -19,    -12,    -15,    -30,    36,
        -14,    -17,    -15,    -13,    -1,     -9,     3,      10,
];

public static readonly int[] MiddleGameKingTable =
[
        31,     39,     2,      -40,    -5,     -63,    33,     52,
        35,     2,      -30,    -78,    -85,    -62,    -11,    21,
        -75,    -65,    -86,    -106,   -114,   -120,   -79,    -101,
        -98,    -87,    -97,    -145,   -134,   -127,   -113,   -170,
        -74,    -51,    -85,    -131,   -142,   -112,   -117,   -159,
        -58,    -36,    -64,    -101,   -87,    -103,   -71,    -90,
        54,     7,      -21,    -67,    -66,    -48,    5,      29,
        49,     54,     19,     -26,    9,      -55,    46,     63,
];

public static readonly int[] EndGameKingTable =
[
        -85,    -44,    -18,    -20,    -42,    -5,     -35,    -94,
        -26,    8,      28,     42,     48,     35,     15,     -23,
        9,      40,     56,     70,     74,     65,     43,     22,
        15,     53,     74,     91,     89,     79,     64,     39,
        5,      43,     73,     89,     93,     79,     67,     37,
        3,      36,     53,     70,     67,     60,     43,     17,
        -32,    5,      24,     38,     39,     31,     7,      -29,
        -97,    -48,    -25,    -23,    -38,    -8,     -40,    -99,
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
