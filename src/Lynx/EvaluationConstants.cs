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

[4503s] Epoch 50000 (11.1368 eps), error 0.0884277, LR 1
public static readonly int[] MiddleGamePieceValues =
[
        +103, +380, +357, +502, +1126, 0,
        -103, -380, -357, -502, -1126, 0
];

public static readonly int[] EndGamePieceValues =
[
        +142, +457, +402, +845, +1432, 0,
        -142, -457, -402, -845, -1432, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -28,    -24,    -20,    -12,    -10,    25,     32,     -15,
        -24,    -23,    -7,     5,      13,     27,     25,     7,
        -19,    -11,    5,      18,     25,     33,     10,     -4,
        -19,    -9,     2,      17,     25,     29,     9,      -5,
        -22,    -21,    -6,     1,      12,     22,     18,     4,
        -28,    -23,    -22,    -19,    -10,    20,     22,     -18,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        15,     11,     9,      -15,    7,      5,      -3,     -11,
        12,     10,     -1,     -13,    -6,     -4,     -6,     -13,
        27,     18,     -1,     -20,    -15,    -12,    4,      -3,
        25,     19,     -2,     -16,    -13,    -9,     3,      -5,
        12,     9,      -3,     -12,    -3,     -3,     -4,     -13,
        17,     12,     10,     -11,    11,     9,      -1,     -9,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -151,   -21,    -62,    -42,    -22,    -28,    -8,     -94,
        -48,    -27,    0,      15,     13,     17,     -16,    -14,
        -15,    4,      19,     58,     57,     39,     34,     1,
        -3,     28,     45,     56,     57,     60,     51,     23,
        -5,     30,     44,     48,     55,     53,     54,     21,
        -22,    7,      17,     48,     56,     35,     30,     -4,
        -42,    -24,    1,      13,     14,     11,     -9,     -14,
        -171,   -24,    -60,    -37,    -16,    -19,    -11,    -85,
];

public static readonly int[] EndGameKnightTable =
[
        -69,    -83,    -10,    -12,    -10,    -24,    -72,    -77,
        -22,    3,      13,     10,     14,     3,      -9,     -18,
        -22,    16,     40,     41,     38,     20,     13,     -18,
        -2,     22,     48,     53,     58,     51,     30,     -4,
        0,      23,     48,     55,     60,     51,     36,     5,
        -21,    19,     30,     43,     36,     21,     11,     -10,
        -28,    2,      6,      12,     6,      4,      -13,    -23,
        -60,    -81,    -7,     -15,    -12,    -28,    -77,    -90,
];

public static readonly int[] MiddleGameBishopTable =
[
        -16,    6,      -9,     -32,    -26,    -19,    -33,    -7,
        6,      3,      11,     -18,    1,      -7,     28,     -9,
        -4,     10,     -2,     8,      -4,     16,     10,     30,
        -6,     4,      2,      29,     25,     -13,    9,      1,
        -7,     4,      -7,     24,     14,     -7,     2,      10,
        3,      10,     5,      -1,     6,      10,     12,     23,
        8,      11,     12,     -9,     -2,     -3,     23,     -1,
        5,      15,     0,      -46,    -25,    -23,    -6,     -11,
];

public static readonly int[] EndGameBishopTable =
[
        -8,     14,     -24,    6,      4,      -3,     10,     -20,
        1,      -5,     -4,     4,      3,      -3,     -2,     -14,
        15,     16,     6,      0,      12,     3,      7,      10,
        11,     6,      4,      -2,     -9,     8,      9,      7,
        4,      9,      6,      -1,     -8,     4,      7,      7,
        13,     5,      4,      2,      8,      -3,     5,      10,
        -9,     -7,     -11,    4,      -1,     -2,     -3,     -12,
        -8,     -10,    -18,    6,      7,      -1,     4,      -9,
];

public static readonly int[] MiddleGameRookTable =
[
        -9,     -16,    -13,    -7,     4,      -7,     2,      -5,
        -32,    -17,    -16,    -15,    -6,     0,      16,     -7,
        -33,    -12,    -16,    -5,     4,      12,     57,     25,
        -26,    -17,    -10,    3,      4,      18,     56,     27,
        -19,    -11,    -7,     10,     5,      19,     51,     29,
        -27,    -9,     -14,    -1,     4,      19,     54,     33,
        -26,    -22,    -10,    -11,    -4,     -5,     18,     -3,
        -6,     -11,    -10,    -1,     11,     -1,     8,      7,
];

public static readonly int[] EndGameRookTable =
[
        3,      4,      9,      1,      -9,     4,      0,      -9,
        19,     20,     20,     9,      -1,     0,      -6,     4,
        11,     6,      9,      1,      -7,     -4,     -20,    -17,
        12,     13,     12,     5,      0,      1,      -17,    -17,
        10,     10,     13,     2,      1,      -3,     -16,    -15,
        13,     11,     7,      -2,     -8,     -8,     -22,    -13,
        21,     21,     14,     6,      -3,     0,      -3,     4,
        -3,     1,      6,      -3,     -13,    -2,     -4,     -17,
];

public static readonly int[] MiddleGameQueenTable =
[
        -11,    -21,    -14,    0,      -9,     -49,    -5,     -12,
        -6,     -11,    7,      0,      4,      0,      23,     44,
        -10,    -1,     -10,    -6,     -5,     10,     36,     57,
        -6,     -14,    -12,    -4,     -1,     8,      19,     31,
        -11,    -9,     -12,    -11,    1,      4,      21,     30,
        -7,     -3,     -12,    -10,    -3,     6,      25,     43,
        -17,    -15,    4,      5,      6,      1,      6,      38,
        -14,    -23,    -9,     -2,     -11,    -52,    -24,    25,
];

public static readonly int[] EndGameQueenTable =
[
        -19,    -13,    -17,    -27,    -11,    4,      -20,    40,
        -17,    -10,    -30,    -11,    -12,    -21,    -54,    9,
        -9,     -9,     5,      -5,     9,      20,     -2,     11,
        -15,    6,      0,      11,     17,     30,     42,     39,
        -4,     1,      3,      23,     12,     32,     24,     50,
        -15,    -11,    11,     5,      9,      20,     21,     22,
        -8,     -8,     -26,    -16,    -15,    -22,    -41,    9,
        -9,     -5,     -11,    -13,    -5,     21,     22,     11,
];

public static readonly int[] MiddleGameKingTable =
[
        13,     47,     17,     -87,    5,      -73,    40,     52,
        -1,     -12,    -34,    -70,    -79,    -56,    -3,     23,
        -83,    -67,    -98,    -94,    -101,   -114,   -76,    -100,
        -90,    -78,    -101,   -122,   -121,   -120,   -122,   -165,
        -65,    -60,    -88,    -114,   -121,   -115,   -122,   -164,
        -71,    -40,    -90,    -93,    -89,    -100,   -78,    -96,
        64,     -11,    -30,    -64,    -66,    -50,    8,      31,
        38,     70,     32,     -80,    10,     -67,    50,     63,
];

public static readonly int[] EndGameKingTable =
[
        -75,    -42,    -21,    3,      -50,    -5,     -37,    -92,
        -24,    15,     26,     38,     43,     32,     11,     -28,
        7,      41,     57,     65,     69,     63,     42,     23,
        12,     49,     70,     82,     81,     77,     63,     39,
        0,      44,     68,     80,     84,     75,     65,     38,
        3,      37,     56,     65,     64,     58,     44,     21,
        -40,    11,     26,     36,     39,     30,     7,      -30,
        -92,    -57,    -29,    -2,     -42,    -8,     -40,    -97,
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
