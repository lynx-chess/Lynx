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
        +101, +392, +172, +488, +1244, 0,
        -101, -392, -172, -488, -1244, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +129, +443, +205, +777, +1558, 0,
        -129, -443, -205, -777, -1558, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -24,    -23,    -14,    -6,     -1,     28,     28,     -12,
        -25,    -25,    -6,     11,     18,     26,     22,     9,
        -24,    -16,    3,      17,     25,     29,     0,      -3,
        -24,    -12,    1,      19,     26,     27,     -1,     -4,
        -22,    -19,    -4,     5,      14,     23,     15,     3,
        -25,    -21,    -19,    -10,    -4,     22,     18,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     9,      5,      -9,     7,      1,      -7,     -7,
        12,     9,      -1,     -12,    -6,     -7,     -7,     -8,
        26,     16,     -1,     -19,    -15,    -14,    3,      0,
        24,     15,     -2,     -15,    -14,    -11,    1,      -2,
        13,     6,      -3,     -11,    -3,     -5,     -6,     -9,
        16,     9,      9,      -8,     16,     4,      -4,     -4,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -146,   -23,    -50,    -30,    -13,    -21,    -10,    -99,
        -45,    -27,    -3,     17,     18,     23,     -14,    -17,
        -28,    1,      20,     56,     60,     40,     34,     -4,
        -11,    24,     44,     59,     59,     60,     44,     17,
        -7,     24,     46,     47,     57,     58,     45,     16,
        -26,    4,      20,     49,     58,     34,     27,     -6,
        -47,    -19,    0,      16,     17,     18,     -13,    -19,
        -163,   -25,    -48,    -21,    -10,    -12,    -18,    -89,
];

internal static readonly short[] EndGameKnightTable =
[
        -66,    -58,    -12,    -11,    -9,     -26,    -52,    -87,
        -18,    1,      14,     9,      9,      6,      -9,     -19,
        -13,    15,     36,     36,     33,     18,     9,      -13,
        7,      20,     47,     49,     53,     47,     24,     -6,
        4,      25,     47,     52,     53,     42,     28,     -0,
        -15,    18,     27,     40,     32,     19,     6,      -9,
        -24,    4,      6,      11,     5,      1,      -11,    -23,
        -71,    -57,    -7,     -13,    -10,    -25,    -50,    -86,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -13,    14,     -2,     -16,    -10,    -15,    -21,    4,
        6,      1,      5,      -18,    0,      -2,     26,     -13,
        -7,     3,      -7,     5,      -8,     10,     3,      24,
        -9,     -8,     -4,     23,     21,     -14,    0,      -3,
        -16,    -4,     -12,    19,     8,      -10,    -8,     4,
        4,      2,      4,      -3,     5,      3,      6,      20,
        9,      13,     9,      -6,     -4,     1,      18,     -3,
        11,     18,     11,     -31,    -12,    -20,    2,      -11,
];

internal static readonly short[] EndGameBishopTable =
[
        -2,     13,     -10,    1,      -2,     6,      -1,     -18,
        -1,     -8,     -7,     -0,     -2,     -13,    -3,     -11,
        11,     9,      6,      -0,     9,      3,      3,      9,
        11,     2,      6,      5,      4,      7,      1,      5,
        6,      5,      5,      9,      2,      6,      2,      5,
        9,      -0,     -0,     -3,     4,      -1,     1,      6,
        -10,    -11,    -18,    -2,     -4,     -7,     -3,     -4,
        0,      -13,    -8,     5,      6,      6,      -3,     -6,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -4,     3,      15,     3,      7,      -2,
        -27,    -17,    -13,    -11,    1,      3,      17,     -4,
        -29,    -20,    -22,    -11,    4,      9,      49,     27,
        -23,    -21,    -17,    -7,     -3,     8,      37,     19,
        -18,    -15,    -13,    -4,     -6,     7,      28,     14,
        -22,    -15,    -19,    -3,     3,      19,     47,     26,
        -24,    -26,    -9,     -5,     2,      1,      24,     2,
        -3,     -4,     1,      12,     22,     8,      14,     9,
];

internal static readonly short[] EndGameRookTable =
[
        4,      2,      6,      -3,     -11,    2,      -0,     -4,
        16,     18,     18,     8,      -1,     -2,     -4,     2,
        13,     11,     12,     5,      -6,     -10,    -20,    -16,
        14,     11,     13,     6,      -0,     -0,     -13,    -13,
        14,     10,     13,     4,      2,      -6,     -10,    -9,
        12,     13,     4,      -3,     -10,    -13,    -20,    -11,
        19,     22,     14,     4,      -3,     -2,     -5,     2,
        -1,     -3,     1,      -9,     -18,    -5,     -8,     -13,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -9,     -9,     -6,     8,      3,      -29,    12,     12,
        -1,     -12,    5,      -4,     1,      4,      21,     51,
        -10,    -8,     -11,    -11,    -13,    5,      32,     54,
        -13,    -20,    -19,    -11,    -11,    -5,     10,     23,
        -13,    -17,    -19,    -19,    -10,    -5,     8,      21,
        -7,     -5,     -16,    -12,    -6,     3,      18,     36,
        -15,    -21,    1,      8,      6,      1,      6,      38,
        -7,     -10,    4,      11,     6,      -35,    -11,    34,
];

internal static readonly short[] EndGameQueenTable =
[
        -26,    -22,    -14,    -12,    -19,    -13,    -38,    3,
        -18,    -10,    -26,    -2,     -4,     -18,    -47,    -5,
        -17,    -5,     7,      2,      23,     20,     -9,     4,
        -10,    7,      12,     23,     35,     38,     42,     30,
        -4,     4,      18,     33,     31,     34,     23,     40,
        -16,    -12,    16,     12,     15,     19,     17,     16,
        -12,    -5,     -21,    -20,    -14,    -14,    -33,    3,
        -18,    -18,    -20,    -7,     -13,    13,     10,     -9,
];

internal static readonly short[] MiddleGameKingTable =
[
        22,     47,     24,     -77,    7,      -63,    37,     45,
        -14,    -18,    -36,    -73,    -86,    -60,    -11,    17,
        -86,    -71,    -109,   -110,   -117,   -126,   -85,    -98,
        -109,   -99,    -118,   -154,   -150,   -140,   -139,   -164,
        -79,    -72,    -107,   -134,   -150,   -124,   -143,   -161,
        -84,    -47,    -100,   -106,   -95,    -106,   -77,    -89,
        70,     -11,    -40,    -65,    -70,    -49,    3,      24,
        35,     72,     35,     -62,    18,     -54,    50,     59,
];

internal static readonly short[] EndGameKingTable =
[
        -70,    -45,    -19,    6,      -32,    -1,     -36,    -88,
        -11,    18,     28,     40,     47,     34,     13,     -22,
        12,     43,     60,     70,     73,     65,     45,     23,
        16,     55,     76,     91,     89,     82,     69,     40,
        7,      46,     74,     87,     93,     80,     72,     40,
        12,     40,     59,     69,     67,     60,     44,     19,
        -37,    14,     30,     38,     41,     30,     9,      -24,
        -81,    -55,    -25,    -0,     -28,    -4,     -40,    -93,
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
