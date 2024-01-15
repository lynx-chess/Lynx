using Lynx.Model;

namespace Lynx;

public static class EvaluationConstants
{
    /// <summary>
    /// 26799 games, 20+0.2, UHO_XXL_+0.90_+1.19.epd
    /// Retained (W,D,L) = (695493, 1473130, 695796) positions.
    /// </summary>
    public const int EvalNormalizationCoefficient = 90;

    public static readonly double[] As = [-22.42003433, 164.85211203, -163.90358958, 112.35539471];

    public static readonly double[] Bs = [-11.46115172, 75.58863284, -86.52557504, 85.45272407];

#pragma warning disable IDE0055 // Discard formatting in this region

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

public static readonly int[] MiddleGamePieceValues =
[
        +102, +384, +359, +476, +1087, 0,
        -102, -384, -359, -476, -1087, 0
];

public static readonly int[] EndGamePieceValues =
[
        +149, +482, +435, +843, +1560, 0,
        -149, -482, -435, -843, -1560, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -26,    -23,    -15,    -10,    -3,     28,     31,     -14,
        -27,    -25,    -5,     10,     18,     27,     25,     7,
        -26,    -15,    4,      18,     27,     31,     3,      -5,
        -26,    -11,    1,      19,     28,     28,     2,      -6,
        -24,    -20,    -4,     4,      14,     23,     17,     2,
        -26,    -21,    -20,    -15,    -6,     22,     20,     -22,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        12,     11,     6,      -15,    5,      2,      -4,     -12,
        11,     12,     -1,     -15,    -8,     -6,     -3,     -11,
        28,     20,     -1,     -22,    -18,    -12,    8,      -1,
        24,     19,     -2,     -17,    -16,    -9,     5,      -3,
        12,     10,     -4,     -13,    -3,     -3,     -2,     -12,
        15,     12,     9,      -14,    16,     6,      -1,     -9,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -151,   -21,    -50,    -30,    -10,    -19,    -8,     -95,
        -44,    -25,    -1,     17,     19,     26,     -12,    -15,
        -29,    1,      21,     53,     60,     43,     36,     -2,
        -14,    18,     39,     52,     57,     60,     48,     17,
        -12,    17,     41,     44,     56,     59,     48,     17,
        -27,    3,      21,     47,     60,     36,     30,     -3,
        -46,    -17,    1,      16,     18,     21,     -9,     -15,
        -168,   -23,    -48,    -19,    -7,     -11,    -16,    -90,
];

public static readonly int[] EndGameKnightTable =
[
        -70,    -58,    -10,    -10,    -7,     -25,    -52,    -91,
        -19,    0,      17,     12,     13,     10,     -11,    -19,
        -13,    16,     37,     38,     36,     18,     10,     -14,
        6,      17,     47,     47,     52,     44,     22,     -8,
        2,      24,     46,     49,     54,     42,     28,     -2,
        -18,    19,     26,     45,     35,     21,     7,      -10,
        -26,    3,      7,      16,     8,      2,      -12,    -22,
        -75,    -57,    -5,     -14,    -8,     -22,    -50,    -90,
];

public static readonly int[] MiddleGameBishopTable =
[
        -16,    15,     -3,     -15,    -10,    -17,    -24,    1,
        7,      3,      6,      -19,    1,      -1,     28,     -13,
        -7,     5,      -4,     2,      -8,     13,     4,      28,
        -8,     -7,     -7,     22,     20,     -18,    2,      -3,
        -17,    -2,     -15,    17,     6,      -13,    -6,     4,
        4,      5,      7,      -6,     5,      6,      7,      23,
        9,      14,     11,     -7,     -4,     3,      20,     -3,
        9,      19,     10,     -29,    -13,    -22,    1,      -14,
];

public static readonly int[] EndGameBishopTable =
[
        -14,    18,     -13,    3,      -3,     6,      1,      -31,
        -5,     -8,     -4,     5,      4,      -10,    -2,     -16,
        14,     16,     9,      3,      13,     4,      7,      10,
        13,     8,      7,      -1,     -6,     7,      4,      6,
        9,      10,     7,      4,      -10,    5,      4,      5,
        11,     4,      0,      1,      5,      0,      4,      6,
        -16,    -11,    -16,    3,      1,      -3,     -2,     -11,
        -7,     -12,    -7,     7,      8,      9,      -2,     -22,
];

public static readonly int[] MiddleGameRookTable =
[
        -4,     -10,    -4,     2,      15,     3,      10,     0,
        -27,    -16,    -14,    -14,    -2,     3,      21,     1,
        -28,    -18,    -22,    -13,    5,      11,     56,     32,
        -26,    -21,    -17,    -12,    -6,     9,      45,     21,
        -20,    -16,    -12,    -5,     -9,     8,      34,     17,
        -23,    -15,    -18,    -5,     2,      20,     51,     31,
        -24,    -28,    -8,     -7,     0,      2,      28,     5,
        -2,     -3,     1,      13,     23,     8,      18,     12,
];

public static readonly int[] EndGameRookTable =
[
        7,      4,      8,      -2,     -10,    4,      0,      -5,
        15,     21,     21,     11,     1,      -1,     -5,     2,
        11,     9,      12,     5,      -8,     -11,    -22,    -20,
        15,     11,     12,     7,      -2,     -2,     -15,    -15,
        14,     10,     13,     3,      0,      -8,     -12,    -11,
        12,     13,     3,      -4,     -11,    -15,    -21,    -14,
        20,     23,     17,     6,      -2,     -2,     -7,     3,
        1,      -2,     3,      -8,     -18,    -4,     -9,     -14,
];

public static readonly int[] MiddleGameQueenTable =
[
        -15,    -9,     -5,     9,      3,      -31,    15,     1,
        1,      -10,    7,      -2,     3,      5,      22,     52,
        -8,     -5,     -8,     -8,     -13,    7,      35,     59,
        -12,    -19,    -19,    -9,     -10,    -5,     12,     26,
        -11,    -14,    -19,    -19,    -9,     -5,     11,     22,
        -4,     -3,     -15,    -12,    -7,     4,      21,     40,
        -15,    -21,    3,      10,     7,      3,      7,      38,
        -11,    -10,    5,      11,     6,      -38,    -14,    28,
];

public static readonly int[] EndGameQueenTable =
[
        -26,    -24,    -9,     -8,     -17,    -11,    -45,    7,
        -21,    -12,    -27,    1,      -2,     -16,    -48,    -5,
        -16,    -6,     4,      0,      24,     23,     -7,     3,
        -11,    9,      7,      13,     29,     41,     49,     36,
        -2,     3,      15,     28,     26,     35,     27,     48,
        -19,    -14,    16,     13,     20,     23,     24,     18,
        -14,    -5,     -22,    -17,    -11,    -9,     -31,    6,
        -14,    -18,    -16,    -2,     -8,     21,     15,     -8,
];

public static readonly int[] MiddleGameKingTable =
[
        34,     55,     30,     -72,    12,     -58,    44,     57,
        -3,     -13,    -33,    -74,    -89,    -59,    -7,     23,
        -82,    -72,    -116,   -119,   -133,   -138,   -88,    -101,
        -121,   -120,   -137,   -181,   -173,   -160,   -158,   -190,
        -85,    -87,    -129,   -159,   -174,   -144,   -160,   -182,
        -77,    -43,    -108,   -119,   -103,   -116,   -78,    -91,
        84,     -2,     -34,    -65,    -70,    -46,    9,      33,
        49,     83,     43,     -56,    25,     -47,    59,     71,
];

public static readonly int[] EndGameKingTable =
[
        -85,    -48,    -22,    4,      -36,    -3,     -41,    -96,
        -20,    17,     30,     43,     51,     36,     14,     -24,
        7,      44,     65,     77,     81,     71,     47,     25,
        17,     60,     85,     103,    99,     90,     75,     47,
        5,      50,     83,     99,     103,    88,     78,     46,
        5,      41,     64,     78,     74,     66,     47,     20,
        -45,    10,     30,     41,     44,     33,     9,      -27,
        -98,    -61,    -29,    -3,     -31,    -7,     -45,    -101,
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

    /// <summary>
    /// 12x64
    /// </summary>
    public static readonly int[][] MiddleGameTable = new int[12][];

    /// <summary>
    /// 12x64
    /// </summary>
    public static readonly int[][] EndGameTable = new int[12][];

    /// <summary>
    /// <see cref="Constants.AbsoluteMaxDepth"/> x <see cref="Constants.MaxNumberOfPossibleMovesInAPosition"/>
    /// </summary>
    public static readonly int[][] LMRReductions = new int[Constants.AbsoluteMaxDepth][];

    public static readonly int[] HistoryBonus = new int[Constants.AbsoluteMaxDepth];

    static EvaluationConstants()
    {
        for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
        {
            MiddleGameTable[piece] = new int[64];
            EndGameTable[piece] = new int[64];
            for (int sq = 0; sq < 64; ++sq)
            {
                MiddleGameTable[piece][sq] = MiddleGamePieceValues[piece] + MiddleGamePositionalTables[piece][sq];
                EndGameTable[piece][sq] = EndGamePieceValues[piece] + EndGamePositionalTables[piece][sq];
            }
        }

        for (int searchDepth = 1; searchDepth < Constants.AbsoluteMaxDepth; ++searchDepth)    // Depth > 0 or we'd be in QSearch
        {
            LMRReductions[searchDepth] = new int[Constants.MaxNumberOfPossibleMovesInAPosition];

            for (int movesSearchedCount = 1; movesSearchedCount < Constants.MaxNumberOfPossibleMovesInAPosition; ++movesSearchedCount) // movesSearchedCount > 0 or we wouldn't be applying LMR
            {
                LMRReductions[searchDepth][movesSearchedCount] = Convert.ToInt32(Math.Round(
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
    public static readonly int[][] MostValueableVictimLeastValuableAttacker =
    [
        [105, 205, 305, 405, 505, 605, 105, 205, 305, 405, 505, 605],
        [104, 204, 304, 404, 504, 604, 104, 204, 304, 404, 504, 604],
        [103, 203, 303, 403, 503, 603, 103, 203, 303, 403, 503, 603],
        [102, 202, 302, 402, 502, 602, 102, 202, 302, 402, 502, 602],
        [101, 201, 301, 401, 501, 601, 101, 201, 301, 401, 501, 601],
        [100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600],
        [105, 205, 305, 405, 505, 605, 105, 205, 305, 405, 505, 605],
        [104, 204, 304, 404, 504, 604, 104, 204, 304, 404, 504, 604],
        [103, 203, 303, 403, 503, 603, 103, 203, 303, 403, 503, 603],
        [102, 202, 302, 402, 502, 602, 102, 202, 302, 402, 502, 602],
        [101, 201, 301, 401, 501, 601, 101, 201, 301, 401, 501, 601],
        [100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600]
    ];

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

    #region Move ordering

    public const int GoodCaptureMoveBaseScoreValue = 1_048_576;

    public const int FirstKillerMoveValue = 524_288;

    public const int SecondKillerMoveValue = 262_144;

    public const int ThirdKillerMoveValue = 131_072;

    // Revisit bad capture pruning in NegaMax.cs if order changes and promos aren't the lowest before bad captures
    public const int PromotionMoveScoreValue = 65_536;

    public const int BadCaptureMoveBaseScoreValue = 32_768;

    //public const int MaxHistoryMoveValue => Configuration.EngineSettings.MaxHistoryMoveValue;

    /// <summary>
    /// Negative offset to ensure history move scores don't reach other move ordering values
    /// </summary>
    public const int BaseMoveScore = int.MinValue / 2;

    #endregion

    /// <summary>
    /// Outside of the evaluation ranges (higher than any sensible evaluation, lower than <see cref="PositiveCheckmateDetectionLimit"/>)
    /// </summary>
    public const int NoHashEntry = 25_000;

    /// <summary>
    /// Evaluation to be returned when there's one single legal move
    /// </summary>
    public const int SingleMoveEvaluation = 200;
}
