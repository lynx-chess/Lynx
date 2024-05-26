using Lynx.Model;

#pragma warning disable IDE1006 // Naming Styles

namespace Lynx;

public static class EvaluationConstants
{
    /// <summary>
    /// 101192 games, 20+0.2, UHO_XXL_+0.90_+1.19.epd
    /// Retained (W,D,L) = (2456211, 5250072, 2453504) positions.
    /// </summary>
    public const int EvalNormalizationCoefficient = 142;

    public static readonly double[] As = [-19.12346211, 118.16335522, -65.13621477, 108.37887346];

    public static readonly double[] Bs = [-5.65666195, 33.69404882, -34.39300793, 99.06591582];

#pragma warning disable IDE0055 // Discard formatting in this region

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

internal static readonly short[] MiddleGamePieceValues =
[
        +103, +392, +168, +208, +1113, 0,
        -103, -392, -168, -208, -1113, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +129, +445, +205, +381, +1427, 0,
        -129, -445, -205, -381, -1427, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -25,    -22,    -16,    -4,     -2,     26,     28,     -13,
        -25,    -27,    -7,     10,     18,     25,     20,     9,
        -24,    -16,    3,      18,     25,     30,     1,      -4,
        -24,    -12,    1,      19,     27,     27,     -1,     -5,
        -22,    -21,    -5,     3,      13,     21,     13,     4,
        -26,    -20,    -20,    -9,     -5,     21,     17,     -21,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     8,      4,      -10,    7,      1,      -8,     -7,
        12,     9,      -0,     -12,    -6,     -7,     -7,     -8,
        27,     16,     -0,     -19,    -14,    -14,    4,      0,
        24,     15,     -1,     -15,    -13,    -11,    2,      -2,
        13,     7,      -3,     -10,    -3,     -5,     -5,     -9,
        17,     9,      8,      -8,     15,     4,      -5,     -4,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -147,   -22,    -51,    -32,    -12,    -21,    -10,    -98,
        -45,    -27,    -3,     15,     17,     24,     -13,    -17,
        -28,    2,      19,     57,     59,     38,     34,     -4,
        -10,    25,     44,     60,     59,     60,     44,     18,
        -6,     25,     46,     47,     58,     58,     45,     17,
        -26,    4,      20,     48,     57,     32,     28,     -6,
        -46,    -18,    1,      14,     16,     19,     -12,    -19,
        -164,   -24,    -49,    -23,    -10,    -11,    -17,    -88,
];

internal static readonly short[] EndGameKnightTable =
[
        -65,    -50,    -11,    -10,    -9,     -27,    -42,    -87,
        -19,    1,      13,     8,      8,      5,      -11,    -21,
        -14,    13,     35,     35,     32,     17,     7,      -14,
        6,      18,     46,     48,     51,     45,     23,     -8,
        2,      23,     46,     51,     52,     41,     27,     -2,
        -17,    17,     25,     40,     32,     18,     5,      -10,
        -25,    3,      6,      11,     4,      0,      -11,    -24,
        -71,    -46,    -6,     -12,    -10,    -25,    -42,    -87,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -20,    13,     -3,     -23,    -12,    -13,    -21,    -6,
        -1,     1,      7,      -16,    2,      1,      24,     -21,
        -12,    10,     -2,     11,     1,      15,     12,     20,
        -13,    -5,     3,      33,     30,     -6,     2,      -4,
        -21,    -1,     -5,     29,     17,     -2,     -7,     1,
        -2,     10,     10,     4,      14,     9,      14,     15,
        1,      12,     11,     -5,     -3,     4,      17,     -11,
        4,      17,     10,     -36,    -15,    -19,    1,      -21,
];

internal static readonly short[] EndGameBishopTable =
[
        -9,     8,      -6,     2,      -1,     5,      -7,     -25,
        -6,     -9,     -6,     4,      1,      -12,    -5,     -18,
        6,      11,     11,     5,      14,     8,      5,      3,
        5,      5,      12,     11,     10,     12,     4,      -2,
        1,      8,      11,     15,     7,      12,     5,      -1,
        3,      2,      6,      3,      9,      4,      4,      -1,
        -15,    -11,    -17,    2,      -1,     -5,     -3,     -11,
        -6,     -16,    -6,     6,      7,      5,      -8,     -13,
];

internal static readonly short[] MiddleGameRookTable =
[
        -2,     -9,     -3,     4,      17,     6,      9,      -1,
        -27,    -17,    -11,    -9,     4,      7,      21,     -4,
        -30,    -19,    -22,    -10,    6,      10,     50,     26,
        -26,    -23,    -19,    -9,     -5,     6,      35,     17,
        -21,    -18,    -14,    -5,     -8,     5,      27,     13,
        -23,    -15,    -17,    -2,     5,      20,     49,     26,
        -25,    -26,    -6,     -2,     6,      5,      27,     2,
        -0,     -2,     2,      14,     25,     11,     17,     11,
];

internal static readonly short[] EndGameRookTable =
[
        6,      4,      6,      -2,     -10,    5,      -1,     -3,
        15,     18,     18,     9,      -1,     -2,     -5,     1,
        12,     10,     12,     5,      -6,     -9,     -21,    -17,
        14,     11,     12,     6,      -1,     -0,     -13,    -14,
        13,     10,     13,     4,      1,      -5,     -10,    -10,
        12,     12,     4,      -3,     -9,     -13,    -21,    -12,
        18,     21,     14,     5,      -3,     -2,     -6,     1,
        1,      -2,     2,      -7,     -17,    -4,     -8,     -12,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -8,     -9,     -9,     7,      2,      -29,    10,     5,
        -2,     -10,    7,      -3,     2,      6,      23,     50,
        -9,     -6,     -9,     -10,    -11,    6,      34,     56,
        -11,    -19,    -19,    -9,     -9,     -5,     11,     26,
        -10,    -15,    -18,    -18,    -9,     -5,     10,     23,
        -6,     -3,     -15,    -12,    -6,     4,      20,     37,
        -15,    -20,    3,      9,      7,      2,      7,      37,
        -6,     -9,     2,      9,      5,      -36,    -12,    28,
];

internal static readonly short[] EndGameQueenTable =
[
        -27,    -19,    -6,     -9,     -15,    -11,    -34,    6,
        -17,    -9,     -25,    1,      -2,     -16,    -46,    -7,
        -16,    -5,     6,      2,      22,     20,     -8,     2,
        -10,    8,      9,      13,     26,     37,     44,     29,
        -4,     5,      15,     25,     22,     34,     25,     39,
        -16,    -11,    14,     12,     15,     19,     18,     14,
        -11,    -3,     -21,    -17,    -12,    -10,    -31,    2,
        -18,    -15,    -13,    -3,     -8,     16,     13,     -4,
];

internal static readonly short[] MiddleGameKingTable =
[
        27,     52,     27,     -77,    6,      -61,    41,     51,
        -12,    -16,    -36,    -74,    -87,    -59,    -10,    19,
        -83,    -70,    -109,   -111,   -119,   -127,   -84,    -96,
        -108,   -97,    -118,   -153,   -150,   -140,   -138,   -162,
        -77,    -70,    -105,   -133,   -149,   -124,   -142,   -159,
        -82,    -46,    -99,    -107,   -95,    -106,   -76,    -87,
        72,     -9,     -38,    -66,    -71,    -48,    4,      27,
        40,     77,     39,     -61,    18,     -51,    55,     65,
];

internal static readonly short[] EndGameKingTable =
[
        -71,    -45,    -19,    7,      -31,    -1,     -37,    -89,
        -12,    18,     27,     39,     46,     33,     12,     -23,
        10,     42,     59,     69,     72,     64,     44,     22,
        15,     54,     75,     90,     88,     81,     68,     39,
        5,      45,     72,     86,     91,     78,     70,     39,
        11,     39,     58,     68,     66,     59,     43,     18,
        -38,    13,     29,     37,     40,     29,     8,      -26,
        -83,    -56,    -27,    -0,     -27,    -5,     -42,    -95,
];

#pragma warning restore IDE0055

    /// <summary>
    /// 12x64
    /// </summary>
    public static readonly int[][] PackedPSQT = new int[12][];

    /// <summary>
    /// <see cref="Constants.AbsoluteMaxDepth"/> x <see cref="Constants.MaxNumberOfPossibleMovesInAPosition"/>
    /// </summary>
    public static readonly int[][] LMRReductions = new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin][];

    /// <summary>
    /// [0, 4, 136, 276, 424, 580, 744, 916, 1096, 1284, 1480, 1684, 1896, 1896, 1896, 1896, ...]
    /// </summary>
    public static readonly int[] HistoryBonus = new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin];

    static EvaluationConstants()
    {
        short[] middleGamePawnTableBlack = MiddleGamePawnTable.Select((_, index) => (short)-MiddleGamePawnTable[index ^ 56]).ToArray();
        short[] endGamePawnTableBlack = EndGamePawnTable.Select((_, index) => (short)-EndGamePawnTable[index ^ 56]).ToArray();

        short[] middleGameKnightTableBlack = MiddleGameKnightTable.Select((_, index) => (short)-MiddleGameKnightTable[index ^ 56]).ToArray();
        short[] endGameKnightTableBlack = EndGameKnightTable.Select((_, index) => (short)-EndGameKnightTable[index ^ 56]).ToArray();

        short[] middleGameBishopTableBlack = MiddleGameBishopTable.Select((_, index) => (short)-MiddleGameBishopTable[index ^ 56]).ToArray();
        short[] endGameBishopTableBlack = EndGameBishopTable.Select((_, index) => (short)-EndGameBishopTable[index ^ 56]).ToArray();

        short[] middleGameRookTableBlack = MiddleGameRookTable.Select((_, index) => (short)-MiddleGameRookTable[index ^ 56]).ToArray();
        short[] endGameRookTableBlack = EndGameRookTable.Select((_, index) => (short)-EndGameRookTable[index ^ 56]).ToArray();

        short[] middleGameQueenTableBlack = MiddleGameQueenTable.Select((_, index) => (short)-MiddleGameQueenTable[index ^ 56]).ToArray();
        short[] EndGameQueenTableBlack = EndGameQueenTable.Select((_, index) => (short)-EndGameQueenTable[index ^ 56]).ToArray();

        short[] middleGameKingTableBlack = MiddleGameKingTable.Select((_, index) => (short)-MiddleGameKingTable[index ^ 56]).ToArray();
        short[] endGameKingTableBlack = EndGameKingTable.Select((_, index) => (short)-EndGameKingTable[index ^ 56]).ToArray();

        short[][] mgPositionalTables =
        [
            MiddleGamePawnTable,
            MiddleGameKnightTable,
            MiddleGameBishopTable,
            MiddleGameRookTable,
            MiddleGameQueenTable,
            MiddleGameKingTable,

            middleGamePawnTableBlack,
            middleGameKnightTableBlack,
            middleGameBishopTableBlack,
            middleGameRookTableBlack,
            middleGameQueenTableBlack,
            middleGameKingTableBlack
        ];

        short[][] egPositionalTables =
        [
            EndGamePawnTable,
            EndGameKnightTable,
            EndGameBishopTable,
            EndGameRookTable,
            EndGameQueenTable,
            EndGameKingTable,

            endGamePawnTableBlack,
            endGameKnightTableBlack,
            endGameBishopTableBlack,
            endGameRookTableBlack,
            EndGameQueenTableBlack,
            endGameKingTableBlack
        ];

        for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
        {
            PackedPSQT[piece] = new int[64];
            for (int sq = 0; sq < 64; ++sq)
            {
                PackedPSQT[piece][sq] = Utils.Pack(
                    (short)(MiddleGamePieceValues[piece] + mgPositionalTables[piece][sq]),
                    (short)(EndGamePieceValues[piece] + egPositionalTables[piece][sq]));
            }
        }

        for (int searchDepth = 1; searchDepth < Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin; ++searchDepth)    // Depth > 0 or we'd be in QSearch
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

    #pragma warning disable IDE0055 // Discard formatting in this region

    /// <summary>
    /// MVV LVA [attacker,victim] 12x11
    /// Original based on
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/move_ordering_intro/bbc.c#L2406
    ///             (Victims)   Pawn Knight Bishop  Rook   Queen  King
    /// (Attackers)
    ///       Pawn              105    205    305    405    505    0
    ///     Knight              104    204    304    404    504    0
    ///     Bishop              103    203    303    403    503    0
    ///       Rook              102    202    302    402    502    0
    ///      Queen              101    201    301    401    501    0
    ///       King              100    200    300    400    500    0
    /// </summary>
    public static readonly int[][] MostValueableVictimLeastValuableAttacker =
    [         //    P     N     B     R      Q  K    p    n      b    r      q          k
        /* P */ [   0,    0,    0,    0,     0, 0,  1500, 4000, 4500, 5500, 11500 ], // 0],
        /* N */ [   0,    0,    0,    0,     0, 0,  1400, 3900, 4400, 5400, 11400 ], // 0],
        /* B */ [   0,    0,    0,    0,     0, 0,  1300, 3800, 4300, 5300, 11300 ], // 0],
        /* R */ [   0,    0,    0,    0,     0, 0,  1200, 3700, 4200, 5200, 11200 ], // 0],
        /* Q */ [   0,    0,    0,    0,     0, 0,  1100, 3600, 4100, 5100, 11100 ], // 0],
        /* K */ [   0,    0,    0,    0,     0, 0,  1000, 3500, 4001, 5000, 11000 ], // 0],
        /* p */ [1500, 4000, 4500, 5500, 11500, 0,     0,    0,    0,    0,     0 ], // 0],
        /* n */ [1400, 3900, 4400, 5400, 11400, 0,     0,    0,    0,    0,     0 ], // 0],
        /* b */ [1300, 3800, 4300, 5300, 11300, 0,     0,    0,    0,    0,     0 ], // 0],
        /* r */ [1200, 3700, 4200, 5200, 11200, 0,     0,    0,    0,    0,     0 ], // 0],
        /* q */ [1100, 3600, 4100, 5100, 11100, 0,     0,    0,    0,    0,     0 ], // 0],
        /* k */ [1000, 3500, 4001, 5000, 11000, 0,     0,    0,    0,    0,     0 ], // 0]
    ];

    public static readonly int[] MVV_PieceValues =
    [
        1000, 3500, 4000, 5000, 11000, 0,
        1000, 3500, 4000, 5000, 11000, 0,
        0
    ];

#pragma warning restore IDE0055

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

#pragma warning restore IDE1006 // Naming Styles
