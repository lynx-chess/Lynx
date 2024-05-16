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
        +112, +395, +363, +486, +1123, 0,
        -112, -395, -363, -486, -1123, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +132, +440, +392, +767, +1410, 0,
        -132, -440, -392, -767, -1410, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -29,    -28,    -18,    -14,    -2,     27,     39,     -13,
        -29,    -29,    -8,     6,      20,     28,     35,     11,
        -28,    -19,    -1,     14,     27,     30,     12,     -1,
        -28,    -15,    -2,     16,     28,     27,     11,     -2,
        -26,    -22,    -7,     0,      15,     24,     29,     5,
        -29,    -25,    -23,    -18,    -5,     21,     28,     -21,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        13,     13,     8,      -10,    5,      2,      -7,     -10,
        12,     12,     1,      -12,    -8,     -6,     -8,     -11,
        26,     19,     1,      -19,    -16,    -12,    3,      -2,
        24,     19,     0,      -15,    -15,    -9,     1,      -4,
        12,     10,     -2,     -10,    -5,     -4,     -7,     -11,
        16,     14,     10,     -11,    14,     5,      -5,     -7,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -149,   -23,    -51,    -31,    -14,    -23,    -11,    -99,
        -45,    -28,    -5,     15,     16,     23,     -17,    -19,
        -28,    0,      19,     56,     60,     40,     34,     -4,
        -11,    25,     42,     59,     58,     59,     45,     17,
        -8,     23,     45,     47,     56,     57,     45,     17,
        -26,    2,      19,     48,     58,     33,     26,     -4,
        -45,    -18,    0,      14,     15,     17,     -16,    -20,
        -166,   -25,    -49,    -20,    -10,    -14,    -18,    -90,
];

internal static readonly short[] EndGameKnightTable =
[
        -62,    -59,    -12,    -12,    -10,    -26,    -53,    -79,
        -17,    1,      13,     8,      8,      5,      -12,    -21,
        -14,    13,     35,     34,     32,     16,     8,      -14,
        5,      17,     47,     47,     52,     45,     22,     -6,
        4,      24,     45,     50,     52,     41,     27,     -1,
        -16,    17,     25,     39,     30,     18,     6,      -10,
        -26,    2,      5,      11,     4,      0,      -11,    -25,
        -66,    -57,    -8,     -16,    -12,    -25,    -51,    -80,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -17,    15,     -2,     -14,    -12,    -18,    -23,    -1,
        6,      1,      6,      -19,    1,      0,      25,     -8,
        -7,     5,      -4,     3,      -8,     13,     4,      28,
        -6,     -7,     -6,     22,     20,     -17,    2,      -1,
        -15,    -2,     -14,    20,     5,      -14,    -7,     5,
        4,      5,      7,      -5,     5,      7,      7,      23,
        9,      13,     10,     -6,     -4,     3,      16,     -4,
        9,      19,     11,     -30,    -12,    -22,    2,      -17,
];

internal static readonly short[] EndGameBishopTable =
[
        -9,     15,     -12,    4,      0,      7,      0,      -25,
        -1,     -7,     -3,     5,      2,      -9,     -3,     -15,
        14,     13,     6,      1,      10,     3,      6,      9,
        12,     7,      6,      -4,     -8,     6,      5,      7,
        8,      9,      5,      -1,     -10,    6,      7,      9,
        10,     3,      -1,     -1,     5,      -2,     3,      7,
        -10,    -9,     -15,    3,      1,      -4,     -2,     -7,
        -7,     -13,    -7,     9,      7,      7,      -3,     -10,
];

internal static readonly short[] MiddleGameRookTable =
[
        -2,     -9,     -4,     1,      16,     2,      10,     -3,
        -24,    -17,    -14,    -12,    2,      3,      16,     -3,
        -27,    -19,    -23,    -12,    7,      12,     53,     32,
        -25,    -20,    -17,    -7,     -2,     11,     43,     23,
        -17,    -16,    -12,    -5,     -5,     10,     33,     18,
        -21,    -15,    -19,    -4,     3,      20,     48,     29,
        -22,    -25,    -10,    -7,     3,      1,      20,     2,
        -1,     -3,     0,      11,     23,     6,      18,     8,
];

internal static readonly short[] EndGameRookTable =
[
        4,      2,      7,      -1,     -11,    5,      0,      -3,
        15,     19,     18,     8,      -3,     -3,     -4,     2,
        12,     11,     11,     5,      -10,    -11,    -22,    -17,
        15,     10,     12,     4,      -2,     -3,     -16,    -14,
        14,     10,     11,     3,      -1,     -7,     -13,    -10,
        12,     12,     4,      -4,     -11,    -14,    -21,    -12,
        19,     21,     14,     4,      -5,     -3,     -5,     2,
        0,      -3,     3,      -8,     -18,    -3,     -8,     -12,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -12,    -9,     -5,     8,      3,      -32,    10,     0,
        -2,     -12,    7,      -3,     2,      4,      21,     61,
        -8,     -6,     -9,     -10,    -11,    6,      32,     58,
        -13,    -19,    -19,    -10,    -12,    -5,     9,      23,
        -12,    -16,    -21,    -20,    -12,    -6,     8,      21,
        -8,     -3,     -15,    -12,    -6,     3,      19,     38,
        -16,    -22,    3,      10,     8,      2,      2,      44,
        -9,     -9,     7,      11,     6,      -39,    -14,    26,
];

internal static readonly short[] EndGameQueenTable =
[
        -22,    -21,    -10,    -9,     -16,    -7,     -36,    10,
        -14,    -10,    -28,    -2,     -2,     -13,    -41,    -1,
        -16,    -5,     5,      -1,     21,     21,     -3,     2,
        -10,    7,      6,      11,     27,     37,     43,     35,
        -3,     3,      16,     24,     25,     33,     26,     42,
        -10,    -14,    12,     7,      13,     20,     19,     19,
        -8,     -2,     -23,    -20,    -15,    -13,    -23,    7,
        -13,    -16,    -20,    -4,     -10,    18,     14,     -1,
];

internal static readonly short[] MiddleGameKingTable =
[
        38,     59,     28,     -86,    4,      -58,    42,     57,
        2,      -18,    -38,    -81,    -90,    -71,    -12,    21,
        -75,    -65,    -113,   -128,   -136,   -131,   -88,    -95,
        -102,   -102,   -130,   -180,   -168,   -143,   -135,   -168,
        -71,    -80,    -119,   -155,   -159,   -127,   -141,   -159,
        -78,    -41,    -105,   -121,   -106,   -108,   -76,    -87,
        82,     -14,    -41,    -75,    -74,    -54,    3,      30,
        50,     84,     41,     -70,    15,     -47,    57,     73,
];

internal static readonly short[] EndGameKingTable =
[
        -71,    -46,    -18,    3,      -32,    -2,     -36,    -87,
        -16,    18,     26,     37,     45,     37,     14,     -21,
        7,      39,     57,     68,     72,     65,     44,     23,
        11,     50,     72,     89,     87,     80,     65,     39,
        2,      42,     70,     84,     89,     78,     69,     38,
        8,      34,     55,     67,     66,     59,     42,     19,
        -40,    13,     27,     36,     39,     32,     9,      -25,
        -84,    -58,    -27,    -4,     -28,    -5,     -40,    -94,
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
