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
        +103, +379, +360, +504, +1138, 0,
        -103, -379, -360, -504, -1138, 0
];

public static readonly int[] EndGamePieceValues =
[
        +137, +440, +384, +817, +1369, 0,
        -137, -440, -384, -817, -1369, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -26,    -24,    -21,    -14,    -13,    25,     31,     -13,
        -22,    -22,    -8,     3,      10,     26,     26,     8,
        -17,    -9,     5,      17,     23,     33,     12,     -4,
        -18,    -7,     1,      15,     22,     30,     14,     -4,
        -20,    -21,    -6,     0,      10,     25,     20,     7,
        -27,    -24,    -22,    -19,    -13,    21,     24,     -14,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        15,     12,     10,     -16,    8,      8,      -2,     -11,
        12,     10,     1,      -12,    -3,     -3,     -6,     -13,
        27,     19,     -1,     -18,    -12,    -11,    3,      -4,
        26,     19,     -2,     -15,    -11,    -8,     1,      -5,
        13,     9,      -2,     -11,    -1,     -2,     -6,     -14,
        17,     12,     11,     -13,    12,     11,     0,      -10,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -147,   -17,    -64,    -46,    -23,    -29,    -6,     -97,
        -45,    -27,    -2,     14,     12,     14,     -17,    -10,
        -14,    5,      17,     53,     54,     39,     33,     1,
        -3,     30,     43,     52,     55,     55,     54,     26,
        -2,     30,     41,     49,     53,     50,     54,     25,
        -18,    9,      17,     46,     51,     36,     30,     -4,
        -40,    -23,    1,      13,     13,     10,     -9,     -10,
        -163,   -20,    -59,    -35,    -17,    -23,    -7,     -84,
];

public static readonly int[] EndGameKnightTable =
[
        -61,    -86,    -10,    -10,    -10,    -26,    -73,    -70,
        -22,    4,      12,     9,      10,     3,      -11,    -22,
        -25,    16,     39,     42,     38,     21,     14,     -15,
        -1,     22,     48,     53,     58,     52,     33,     -2,
        0,      18,     48,     52,     61,     52,     38,     6,
        -28,    18,     30,     42,     36,     21,     15,     -12,
        -33,    -1,     4,      12,     7,      2,      -12,    -28,
        -60,    -85,    -9,     -14,    -12,    -27,    -76,    -90,
];

public static readonly int[] MiddleGameBishopTable =
[
        -17,    4,      -9,     -38,    -28,    -19,    -30,    -18,
        8,      2,      8,      -17,    0,      -4,     27,     -8,
        -4,     9,      -2,     6,      -4,     15,     11,     28,
        -6,     6,      4,      30,     25,     -11,    9,      4,
        -7,     5,      -3,     24,     15,     -7,     3,      8,
        2,      9,      3,      1,      4,      11,     11,     20,
        8,      8,      12,     -10,    -2,     -3,     24,     -1,
        5,      13,     -3,     -46,    -28,    -21,    -12,    -21,
];

public static readonly int[] EndGameBishopTable =
[
        -5,     10,     -25,    7,      3,      -5,     9,      -10,
        1,      -6,     -5,     3,      0,      -7,     -4,     -18,
        12,     13,     4,      0,      10,     0,      5,      8,
        9,      4,      3,      -6,     -10,    7,      10,     8,
        2,      7,      4,      -2,     -9,     4,      10,     8,
        11,     5,      3,      -1,     7,      -4,     4,      11,
        -7,     -8,     -10,    3,      -2,     -3,     -6,     -15,
        -7,     -7,     -21,    5,      4,      -8,     9,      -3,
];

public static readonly int[] MiddleGameRookTable =
[
        -9,     -17,    -13,    -6,     4,      -6,     4,      -4,
        -29,    -16,    -16,    -14,    -6,     -4,     14,     -14,
        -31,    -13,    -17,    -2,     3,      11,     56,     20,
        -20,    -11,    -7,     8,      6,      23,     58,     29,
        -19,    -11,    -7,     10,     8,      24,     58,     31,
        -28,    -10,    -17,    -2,     2,      17,     54,     28,
        -25,    -20,    -14,    -10,    -7,     -2,     13,     -4,
        -5,     -12,    -12,    -2,     8,      -1,     7,      6,
];

public static readonly int[] EndGameRookTable =
[
        0,      1,      7,      -1,     -12,    1,      -3,     -15,
        17,     17,     17,     8,      -3,     -1,     -6,     4,
        10,     7,      10,     0,      -7,     -3,     -21,    -14,
        8,      11,     9,      3,      1,      1,      -16,    -15,
        9,      10,     12,     3,      0,      -2,     -17,    -16,
        13,     11,     9,      -1,     -6,     -7,     -20,    -12,
        18,     19,     14,     4,      -3,     -3,     -2,     4,
        -5,     0,      5,      -3,     -13,    -4,     -6,     -20,
];

public static readonly int[] MiddleGameQueenTable =
[
        -6,     -24,    -12,    2,      -10,    -51,    -7,     -11,
        -9,     -10,    6,      0,      4,      0,      23,     45,
        -12,    0,      -8,     -5,     -3,     9,      35,     57,
        -6,     -12,    -11,    -2,     3,      12,     23,     33,
        -9,     -9,     -10,    -6,     5,      8,      24,     32,
        -8,     -2,     -8,     -8,     -5,     7,      26,     41,
        -20,    -13,    4,      3,      5,      1,      11,     37,
        -5,     -23,    -8,     -2,     -13,    -56,    -29,    16,
];

public static readonly int[] EndGameQueenTable =
[
        -22,    -4,     -17,    -33,    -10,    6,      -6,     50,
        -12,    -12,    -29,    -13,    -16,    -25,    -57,    8,
        -9,     -13,    3,      -7,     7,      21,     3,      16,
        -15,    6,      3,      12,     15,     29,     39,     45,
        -7,     3,      5,      17,     10,     31,     29,     51,
        -14,    -11,    6,      1,      9,      21,     18,     37,
        -1,     -10,    -28,    -19,    -20,    -27,    -54,    12,
        -16,    -6,     -15,    -16,    -3,     16,     25,     34,
];

public static readonly int[] MiddleGameKingTable =
[
        -1,     39,     11,     -91,    5,      -76,    38,     44,
        4,      -18,    -31,    -65,    -73,    -54,    -2,     23,
        -77,    -52,    -79,    -77,    -83,    -101,   -74,    -105,
        -80,    -53,    -74,    -83,    -94,    -94,    -105,   -145,
        -62,    -38,    -64,    -82,    -91,    -88,    -100,   -152,
        -67,    -40,    -84,    -79,    -78,    -94,    -78,    -98,
        45,     -18,    -35,    -65,    -66,    -50,    3,      24,
        19,     55,     25,     -87,    5,      -75,    43,     53,
];

public static readonly int[] EndGameKingTable =
[
        -71,    -40,    -22,    1,      -53,    -7,     -36,    -91,
        -22,    13,     23,     33,     37,     29,     9,      -29,
        4,      35,     51,     57,     59,     57,     39,     23,
        11,     42,     60,     69,     71,     67,     58,     33,
        0,      37,     61,     69,     72,     65,     57,     33,
        3,      37,     50,     57,     57,     54,     39,     20,
        -34,    11,     24,     32,     35,     28,     6,      -31,
        -85,    -51,    -28,    -2,     -47,    -7,     -37,    -96,
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
