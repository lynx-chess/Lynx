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

#pragma warning disable IDE0055 // Discard formatting in this region

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

public static readonly int[] MiddleGamePieceValues =
[
        +103, +386, +357, +475, +1084, 0,
        -103, -386, -357, -475, -1084, 0
];

public static readonly int[] EndGamePieceValues =
[
        +149, +485, +434, +843, +1560, 0,
        -149, -485, -434, -843, -1560, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -26,    -23,    -15,    -9,     -3,     29,     31,     -15,
        -27,    -25,    -5,     11,     18,     27,     24,     7,
        -27,    -15,    3,      18,     27,     30,     3,      -6,
        -26,    -11,    1,      19,     27,     27,     2,      -7,
        -24,    -19,    -3,     4,      14,     23,     17,     1,
        -27,    -21,    -20,    -14,    -6,     22,     20,     -22,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        12,     12,     6,      -15,    6,      2,      -3,     -12,
        11,     13,     0,      -14,    -7,     -5,     -2,     -11,
        28,     20,     0,      -22,    -18,    -12,    8,      -1,
        25,     19,     -2,     -17,    -16,    -9,     6,      -3,
        12,     10,     -4,     -13,    -2,     -3,     -2,     -11,
        15,     12,     9,      -14,    16,     6,      -1,     -9,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -153,   -23,    -52,    -32,    -12,    -21,    -9,     -98,
        -46,    -27,    -3,     15,     18,     25,     -14,    -17,
        -28,    1,      20,     57,     61,     42,     35,     -3,
        -11,    25,     43,     60,     60,     61,     46,     18,
        -8,     24,     45,     48,     57,     59,     47,     17,
        -26,    3,      20,     49,     60,     34,     28,     -4,
        -48,    -19,    0,      15,     17,     19,     -11,    -18,
        -168,   -24,    -49,    -21,    -9,     -12,    -17,    -91,
];

public static readonly int[] EndGameKnightTable =
[
        -72,    -61,    -13,    -13,    -10,    -28,    -55,    -93,
        -22,    -2,     15,     9,      10,     7,      -13,    -21,
        -14,    16,     38,     38,     37,     18,     10,     -15,
        7,      20,     52,     53,     56,     50,     25,     -6,
        4,      27,     51,     55,     58,     47,     31,     0,
        -18,    19,     27,     45,     35,     20,     7,      -11,
        -28,    2,      5,      13,     5,      0,      -15,    -25,
        -78,    -60,    -9,     -17,    -11,    -26,    -53,    -93,
];

public static readonly int[] MiddleGameBishopTable =
[
        -15,    16,     -2,     -15,    -10,    -16,    -23,    2,
        8,      3,      7,      -18,    1,      -1,     29,     -12,
        -6,     5,      -4,     3,      -8,     14,     5,      28,
        -7,     -7,     -6,     23,     19,     -18,    2,      -2,
        -16,    -2,     -15,    17,     6,      -13,    -6,     5,
        5,      5,      7,      -5,     6,      7,      8,      24,
        10,     15,     12,     -6,     -3,     4,      21,     -2,
        10,     20,     12,     -29,    -13,    -21,    1,      -12,
];

public static readonly int[] EndGameBishopTable =
[
        -14,    18,     -13,    3,      -3,     6,      1,      -31,
        -5,     -8,     -4,     5,      4,      -10,    -2,     -16,
        14,     16,     8,      3,      12,     4,      7,      10,
        13,     8,      7,      -2,     -6,     6,      4,      5,
        9,      10,     6,      4,      -10,    5,      4,      5,
        12,     4,      0,      1,      5,      -1,     4,      7,
        -15,    -11,    -17,    3,      1,      -3,     -2,     -11,
        -7,     -12,    -7,     7,      8,      9,      -2,     -22,
];

public static readonly int[] MiddleGameRookTable =
[
        -4,     -10,    -5,     2,      15,     3,      10,     0,
        -27,    -17,    -14,    -14,    -2,     3,      21,     0,
        -29,    -19,    -23,    -13,    4,      9,      55,     31,
        -26,    -21,    -18,    -12,    -7,     8,      44,     21,
        -20,    -17,    -12,    -5,     -10,    7,      33,     16,
        -24,    -16,    -19,    -5,     1,      19,     50,     30,
        -25,    -28,    -9,     -7,     -1,     1,      27,     4,
        -2,     -3,     1,      12,     23,     8,      17,     12,
];

public static readonly int[] EndGameRookTable =
[
        6,      3,      7,      -2,     -11,    4,      0,      -5,
        15,     20,     21,     11,     0,      -2,     -6,     2,
        11,     8,      12,     5,      -9,     -11,    -23,    -20,
        15,     10,     12,     6,      -2,     -2,     -16,    -15,
        14,     9,      12,     2,      -1,     -9,     -13,    -11,
        12,     13,     3,      -4,     -11,    -16,    -22,    -14,
        20,     23,     16,     5,      -3,     -2,     -8,     2,
        1,      -3,     2,      -8,     -18,    -4,     -9,     -15,
];

public static readonly int[] MiddleGameQueenTable =
[
        -15,    -10,    -5,     9,      3,      -32,    14,     1,
        0,      -10,    7,      -2,     2,      6,      22,     51,
        -8,     -5,     -10,    -9,     -14,    6,      35,     59,
        -12,    -20,    -20,    -9,     -11,    -5,     11,     25,
        -12,    -15,    -20,    -20,    -10,    -6,     9,      22,
        -5,     -3,     -16,    -13,    -8,     3,      20,     39,
        -15,    -21,    3,      9,      7,      3,      6,      37,
        -11,    -11,    5,      11,     6,      -39,    -12,    28,
];

public static readonly int[] EndGameQueenTable =
[
        -27,    -24,    -10,    -9,     -18,    -11,    -46,    6,
        -22,    -13,    -28,    0,      -2,     -17,    -48,    -5,
        -17,    -7,     4,      -1,     23,     23,     -8,     3,
        -11,    8,      5,      12,     28,     40,     49,     35,
        -2,     2,      14,     26,     26,     34,     27,     48,
        -20,    -14,    15,     13,     19,     22,     23,     18,
        -15,    -6,     -23,    -18,    -12,    -10,    -32,    6,
        -15,    -18,    -17,    -2,     -9,     21,     13,     -9,
];

public static readonly int[] MiddleGameKingTable =
[
        -41,    39,     15,     -87,    -3,     -74,    28,     -18,
        -19,    60,     39,     -2,     -16,    14,     65,     7,
        -98,    2,      -44,    -47,    -61,    -65,    -16,    -116,
        -136,   -47,    -65,    -108,   -100,   -88,    -85,    -206,
        -101,   -14,    -56,    -87,    -102,   -72,    -88,    -198,
        -93,    30,     -36,    -47,    -31,    -44,    -6,     -108,
        67,     70,     38,     8,      3,      26,     81,     16,
        -26,    66,     27,     -71,    9,      -63,    42,     -4,
];

public static readonly int[] EndGameKingTable =
[
        -8,     -23,    4,      30,     -10,    23,     -15,    -19,
        5,      -35,    -22,    -8,     0,      -15,    -37,    2,
        33,     -8,     14,     26,     30,     20,     -4,     51,
        43,     9,      34,     51,     47,     39,     24,     73,
        31,     -1,     31,     47,     52,     37,     27,     72,
        32,     -11,    13,     26,     23,     15,     -4,     46,
        -19,    -41,    -21,    -10,    -8,     -19,    -42,    -1,
        -20,    -35,    -3,     23,     -5,     19,     -19,    -24,
];

#pragma warning restore IDE0055

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
