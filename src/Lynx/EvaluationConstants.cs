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
        +101, +393, +163, +488, +1114, 0,
        -101, -393, -163, -488, -1114, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +129, +443, +205, +777, +1425, 0,
        -129, -443, -205, -777, -1425, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -23,    -22,    -14,    -8,     -2,     28,     29,     -12,
        -23,    -25,    -6,     11,     18,     25,     21,     11,
        -24,    -15,    4,      19,     26,     30,     2,      -3,
        -23,    -11,    2,      20,     28,     28,     1,      -4,
        -21,    -19,    -4,     4,      14,     23,     14,     5,
        -24,    -20,    -19,    -12,    -5,     22,     18,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     9,      4,      -9,     7,      1,      -8,     -7,
        12,     9,      -0,     -12,    -6,     -7,     -7,     -8,
        27,     16,     -1,     -19,    -15,    -14,    3,      0,
        24,     15,     -1,     -15,    -13,    -11,    1,      -2,
        13,     6,      -3,     -10,    -3,     -5,     -6,     -9,
        16,     9,      9,      -8,     16,     4,      -4,     -4,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -147,   -23,    -48,    -30,    -13,    -18,    -10,    -98,
        -46,    -27,    -4,     15,     16,     23,     -13,    -18,
        -27,    1,      19,     56,     59,     39,     33,     -2,
        -11,    25,     44,     59,     59,     59,     45,     17,
        -7,     24,     45,     47,     58,     58,     45,     16,
        -25,    4,      19,     48,     57,     33,     27,     -4,
        -47,    -19,    -0,     14,     16,     17,     -12,    -19,
        -165,   -25,    -47,    -20,    -10,    -9,     -18,    -89,
];

internal static readonly short[] EndGameKnightTable =
[
        -66,    -58,    -12,    -11,    -9,     -27,    -52,    -87,
        -18,    1,      14,     10,     10,     6,      -10,    -19,
        -13,    15,     37,     36,     33,     19,     9,      -14,
        7,      20,     48,     49,     53,     47,     24,     -6,
        4,      25,     47,     52,     53,     43,     28,     -0,
        -16,    18,     27,     41,     33,     20,     6,      -10,
        -24,    4,      6,      12,     6,      1,      -10,    -23,
        -71,    -58,    -6,     -13,    -10,    -26,    -50,    -86,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -31,    6,      -8,     -21,    -15,    -20,    -27,    -13,
        -5,     3,      8,      -14,    9,      5,      26,     -20,
        -15,    7,      1,      14,     2,      17,     8,      18,
        -16,    -5,     5,      33,     30,     -5,     1,      -9,
        -23,    -1,     -4,     30,     18,     -1,     -7,     -3,
        -5,     7,      13,     7,      16,     12,     10,     14,
        -4,     14,     13,     -1,     4,      9,      19,     -9,
        -6,     10,     6,      -35,    -18,    -24,    -4,     -28,
];

internal static readonly short[] EndGameBishopTable =
[
        -12,    7,      -12,    -1,     -4,     4,      -8,     -28,
        -7,     -9,     -6,     4,      -0,     -11,    -3,     -20,
        5,      12,     13,     8,      17,     11,     8,      2,
        5,      7,      15,     15,     13,     16,     7,      -2,
        0,      10,     13,     18,     11,     15,     7,      -1,
        3,      3,      7,      5,      12,     7,      6,      -1,
        -17,    -11,    -16,    2,      -1,     -4,     -3,     -13,
        -9,     -19,    -11,    3,      4,      4,      -9,     -15,
];

internal static readonly short[] MiddleGameRookTable =
[
        -3,     -10,    -2,     4,      15,     7,      7,      -2,
        -26,    -16,    -14,    -12,    1,      3,      18,     -3,
        -28,    -19,    -23,    -11,    4,      9,      48,     27,
        -23,    -20,    -17,    -7,     -3,     8,      37,     19,
        -18,    -16,    -13,    -4,     -6,     7,      28,     14,
        -21,    -15,    -18,    -3,     2,      19,     47,     27,
        -24,    -25,    -9,     -5,     3,      1,      24,     2,
        -2,     -4,     2,      13,     23,     11,     15,     10,
];

internal static readonly short[] EndGameRookTable =
[
        4,      2,      5,      -3,     -11,    1,      -1,     -4,
        16,     18,     18,     8,      -1,     -2,     -4,     2,
        13,     10,     12,     5,      -7,     -10,    -20,    -17,
        14,     11,     13,     6,      -1,     -1,     -13,    -14,
        14,     10,     13,     4,      1,      -6,     -10,    -10,
        12,     13,     4,      -3,     -10,    -13,    -21,    -11,
        19,     21,     14,     4,      -4,     -2,     -5,     2,
        -1,     -3,     1,      -9,     -18,    -7,     -8,     -13,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -11,    -10,    -3,     12,     4,      -27,    8,      3,
        -2,     -10,    6,      -3,     3,      6,      23,     50,
        -9,     -6,     -9,     -10,    -12,    7,      33,     56,
        -12,    -19,    -19,    -9,     -9,     -5,     11,     25,
        -11,    -15,    -19,    -18,    -8,     -5,     10,     23,
        -6,     -4,     -15,    -12,    -5,     4,      19,     37,
        -15,    -20,    3,      10,     7,      3,      8,      37,
        -9,     -9,     7,      14,     7,      -33,    -12,    26,
];

internal static readonly short[] EndGameQueenTable =
[
        -25,    -20,    -13,    -13,    -17,    -12,    -33,    8,
        -16,    -8,     -23,    2,      -1,     -16,    -45,    -7,
        -15,    -3,     7,      3,      23,     20,     -7,     3,
        -8,     10,     11,     15,     27,     39,     45,     31,
        -3,     6,      17,     26,     23,     34,     25,     40,
        -15,    -10,    15,     13,     15,     19,     19,     15,
        -11,    -2,     -19,    -17,    -11,    -11,    -31,    3,
        -16,    -16,    -19,    -7,     -10,    15,     13,     -3,
];

internal static readonly short[] MiddleGameKingTable =
[
        23,     48,     27,     -75,    8,      -59,    38,     47,
        -14,    -15,    -35,    -73,    -85,    -58,    -10,    19,
        -83,    -69,    -107,   -109,   -116,   -125,   -84,    -96,
        -108,   -97,    -116,   -152,   -148,   -138,   -138,   -162,
        -77,    -70,    -104,   -132,   -148,   -123,   -141,   -160,
        -81,    -46,    -98,    -104,   -93,    -104,   -75,    -87,
        72,     -8,     -38,    -64,    -68,    -47,    5,      26,
        36,     74,     39,     -59,    20,     -50,    52,     61,
];

internal static readonly short[] EndGameKingTable =
[
        -70,    -45,    -20,    6,      -32,    -2,     -37,    -88,
        -12,    18,     27,     40,     46,     33,     13,     -23,
        11,     43,     59,     69,     72,     64,     44,     22,
        15,     54,     75,     90,     89,     81,     68,     39,
        6,      45,     73,     86,     92,     79,     71,     39,
        11,     39,     58,     68,     67,     59,     43,     18,
        -38,    13,     29,     38,     40,     30,     8,      -25,
        -82,    -56,    -27,    -1,     -28,    -5,     -41,    -94,
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
