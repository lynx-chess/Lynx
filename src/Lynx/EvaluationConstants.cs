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
        +103, +386, +359, +476, +1084, 0,
        -103, -386, -359, -476, -1084, 0
];

public static readonly int[] EndGamePieceValues =
[
        +149, +483, +434, +844, +1564, 0,
        -149, -483, -434, -844, -1564, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -26,    -23,    -15,    -10,    -3,     28,     31,     -15,
        -27,    -25,    -5,     11,     19,     27,     24,     7,
        -27,    -15,    3,      18,     27,     30,     3,      -6,
        -26,    -12,    1,      19,     27,     27,     2,      -7,
        -25,    -20,    -4,     4,      14,     23,     17,     1,
        -27,    -21,    -20,    -14,    -5,     22,     20,     -22,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        12,     12,     6,      -15,    6,      2,      -4,     -12,
        11,     13,     0,      -14,    -7,     -5,     -3,     -11,
        28,     20,     -1,     -23,    -18,    -12,    8,      -1,
        25,     19,     -2,     -18,    -16,    -9,     5,      -3,
        12,     10,     -4,     -13,    -2,     -3,     -2,     -12,
        15,     12,     9,      -14,    17,     6,      -1,     -9,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -155,   -23,    -52,    -33,    -12,    -22,    -9,     -99,
        -47,    -28,    -4,     15,     18,     24,     -14,    -17,
        -29,    0,      20,     56,     60,     41,     35,     -3,
        -12,    24,     42,     59,     59,     60,     46,     17,
        -8,     23,     44,     47,     57,     58,     47,     17,
        -26,    3,      20,     48,     59,     34,     28,     -4,
        -48,    -20,    -1,     14,     16,     19,     -12,    -18,
        -170,   -25,    -50,    -21,    -10,    -13,    -18,    -92,
];

public static readonly int[] EndGameKnightTable =
[
        -71,    -61,    -12,    -12,    -9,     -27,    -55,    -93,
        -21,    -2,     15,     10,     11,     8,      -12,    -20,
        -13,    16,     39,     37,     37,     18,     10,     -14,
        8,      18,     50,     51,     54,     48,     24,     -5,
        5,      25,     49,     53,     56,     44,     30,     1,
        -17,    19,     27,     45,     36,     21,     7,      -10,
        -28,    2,      5,      14,     6,      0,      -14,    -24,
        -77,    -59,    -8,     -16,    -10,    -24,    -52,    -92,
];

public static readonly int[] MiddleGameBishopTable =
[
        -16,    15,     -3,     -15,    -11,    -17,    -24,    1,
        7,      3,      6,      -19,    1,      -2,     28,     -13,
        -7,     4,      -5,     2,      -9,     13,     4,      28,
        -8,     -8,     -7,     22,     19,     -19,    2,      -3,
        -17,    -3,     -15,    16,     6,      -13,    -7,     4,
        4,      4,      6,      -6,     5,      6,      7,      23,
        9,      14,     11,     -7,     -4,     3,      20,     -3,
        9,      19,     11,     -30,    -14,    -22,    1,      -13,
];

public static readonly int[] EndGameBishopTable =
[
        -14,    19,     -12,    4,      -2,     6,      1,      -31,
        -4,     -8,     -4,     5,      4,      -10,    -2,     -16,
        15,     16,     9,      3,      13,     4,      8,      10,
        13,     9,      7,      -1,     -6,     7,      4,      6,
        9,      10,     7,      4,      -10,    5,      4,      5,
        12,     4,      0,      1,      6,      0,      4,      7,
        -15,    -11,    -16,    3,      1,      -2,     -1,     -11,
        -7,     -11,    -7,     7,      9,      10,     -1,     -22,
];

public static readonly int[] MiddleGameRookTable =
[
        -4,     -10,    -4,     2,      15,     3,      10,     0,
        -27,    -16,    -14,    -14,    -2,     3,      21,     0,
        -29,    -18,    -22,    -13,    4,      10,     55,     31,
        -26,    -21,    -18,    -12,    -7,     8,      44,     21,
        -20,    -16,    -12,    -5,     -9,     7,      33,     16,
        -23,    -16,    -18,    -5,     2,      20,     51,     30,
        -24,    -28,    -8,     -7,     0,      1,      28,     4,
        -2,     -3,     1,      13,     23,     8,      17,     12,
];

public static readonly int[] EndGameRookTable =
[
        6,      4,      7,      -2,     -11,    4,      0,      -5,
        15,     20,     21,     11,     0,      -2,     -6,     2,
        11,     8,      12,     5,      -9,     -11,    -23,    -20,
        15,     10,     12,     6,      -2,     -2,     -16,    -15,
        14,     9,      12,     2,      -1,     -9,     -13,    -11,
        12,     13,     3,      -4,     -11,    -16,    -22,    -14,
        19,     23,     16,     5,      -3,     -2,     -8,     2,
        1,      -3,     2,      -8,     -18,    -4,     -9,     -15,
];

public static readonly int[] MiddleGameQueenTable =
[
        -14,    -9,     -5,     9,      3,      -31,    15,     2,
        1,      -10,    8,      -2,     3,      6,      22,     51,
        -7,     -5,     -9,     -9,     -13,    6,      35,     59,
        -12,    -19,    -19,    -9,     -11,    -5,     11,     26,
        -12,    -15,    -20,    -19,    -10,    -6,     10,     22,
        -5,     -3,     -16,    -13,    -8,     4,      20,     39,
        -15,    -20,    3,      9,      7,      3,      7,      37,
        -10,    -10,    5,      11,     6,      -38,    -11,    28,
];

public static readonly int[] EndGameQueenTable =
[
        -27,    -24,    -10,    -9,     -18,    -11,    -46,    6,
        -22,    -13,    -28,    0,      -2,     -18,    -48,    -6,
        -18,    -7,     4,      0,      24,     23,     -9,     3,
        -12,    8,      6,      12,     29,     40,     49,     34,
        -2,     2,      14,     27,     26,     34,     26,     47,
        -20,    -14,    16,     13,     20,     22,     23,     17,
        -15,    -6,     -24,    -18,    -12,    -10,    -32,    6,
        -15,    -18,    -17,    -2,     -8,     20,     13,     -10,
];

public static readonly int[] MiddleGameKingTable =
[
        34,     55,     30,     -71,    12,     -58,    44,     57,
        -3,     -12,    -33,    -74,    -88,    -58,    -7,     23,
        -82,    -71,    -115,   -119,   -133,   -137,   -88,    -101,
        -120,   -120,   -138,   -182,   -173,   -161,   -158,   -191,
        -85,    -87,    -129,   -160,   -174,   -144,   -161,   -183,
        -77,    -43,    -109,   -119,   -103,   -116,   -78,    -91,
        84,     -2,     -34,    -64,    -70,    -46,    9,      32,
        49,     82,     43,     -55,    25,     -47,    58,     70,
];

public static readonly int[] EndGameKingTable =
[
        -85,    -48,    -22,    4,      -36,    -3,     -41,    -96,
        -21,    17,     30,     43,     51,     36,     14,     -23,
        7,      44,     65,     77,     81,     71,     47,     25,
        17,     60,     85,     103,    99,     90,     75,     47,
        5,      50,     83,     98,     103,    88,     78,     46,
        5,      40,     64,     78,     74,     66,     47,     19,
        -45,    10,     31,     41,     44,     33,     9,      -27,
        -97,    -61,    -29,    -3,     -31,    -7,     -45,    -101,
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
