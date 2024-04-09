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
        +105, +392, +363, +488, +1116, 0,
        -105, -392, -363, -488, -1116, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +135, +443, +399, +774, +1424, 0,
        -135, -443, -399, -774, -1424, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -26,    -22,    -15,    -10,    -2,     29,     31,     -15,
        -26,    -24,    -6,     9,      18,     28,     26,     7,
        -25,    -15,    3,      17,     25,     31,     4,      -5,
        -25,    -11,    1,      19,     27,     28,     3,      -6,
        -24,    -17,    -4,     3,      14,     25,     19,     1,
        -26,    -20,    -19,    -14,    -5,     23,     21,     -22,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        11,     11,     6,      -12,    6,      2,      -5,     -10,
        9,      10,     0,      -13,    -7,     -6,     -5,     -11,
        24,     17,     -1,     -20,    -16,    -12,    6,      -2,
        22,     17,     -2,     -16,    -14,    -9,     4,      -4,
        10,     8,      -3,     -11,    -3,     -4,     -4,     -11,
        13,     11,     9,      -12,    15,     5,      -2,     -6,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -145,   -22,    -50,    -31,    -12,    -20,    -10,    -98,
        -45,    -28,    -3,     16,     17,     24,     -14,    -17,
        -28,    2,      20,     57,     60,     40,     34,     -4,
        -10,    25,     44,     59,     59,     59,     45,     18,
        -7,     25,     46,     47,     57,     58,     45,     17,
        -26,    4,      20,     49,     58,     34,     28,     -5,
        -46,    -19,    0,      15,     17,     19,     -13,    -18,
        -163,   -25,    -48,    -20,    -9,     -11,    -18,    -88,
];

internal static readonly short[] EndGameKnightTable =
[
        -67,    -59,    -12,    -11,    -10,    -27,    -53,    -87,
        -18,    1,      13,     8,      8,      5,      -10,    -20,
        -14,    15,     35,     35,     32,     17,     9,      -14,
        7,      19,     47,     48,     52,     46,     24,     -7,
        4,      24,     46,     51,     52,     42,     28,     0,
        -16,    18,     26,     40,     32,     18,     5,      -10,
        -25,    4,      5,      11,     4,      0,      -11,    -24,
        -71,    -57,    -7,     -14,    -11,    -26,    -50,    -85,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -18,    14,     -3,     -15,    -12,    -16,    -22,    0,
        7,      2,      7,      -18,    2,      -1,     27,     -12,
        -6,     5,      -4,     3,      -8,     13,     4,      27,
        -8,     -6,     -5,     23,     20,     -17,    2,      -2,
        -15,    -1,     -13,    18,     6,      -13,    -6,     5,
        5,      5,      7,      -4,     6,      6,      7,      22,
        9,      13,     11,     -6,     -3,     3,      19,     -3,
        7,      18,     11,     -30,    -14,    -21,    1,      -15,
];

internal static readonly short[] EndGameBishopTable =
[
        -11,    13,     -14,    3,      -2,     4,      -2,     -27,
        -3,     -7,     -3,     4,      2,      -9,     -3,     -15,
        12,     13,     7,      2,      11,     3,      6,      9,
        12,     7,      6,      -3,     -6,     6,      5,      6,
        8,      10,     6,      0,      -8,     6,      7,      7,
        10,     4,      0,      0,      5,      -1,     4,      6,
        -13,    -10,    -14,    2,      1,      -3,     -3,     -10,
        -7,     -14,    -9,     7,      7,      6,      -4,     -13,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -4,     2,      14,     4,      7,      -2,
        -26,    -17,    -13,    -12,    0,      3,      17,     -3,
        -29,    -20,    -22,    -12,    3,      10,     50,     27,
        -24,    -21,    -17,    -8,     -4,     9,      38,     19,
        -18,    -15,    -13,    -5,     -6,     8,      29,     14,
        -22,    -16,    -18,    -4,     2,      19,     48,     27,
        -24,    -26,    -9,     -6,     2,      2,      23,     2,
        -2,     -4,     0,      12,     22,     8,      15,     9,
];

internal static readonly short[] EndGameRookTable =
[
        4,      2,      6,      -3,     -11,    3,      0,      -4,
        16,     19,     18,     8,      -1,     -2,     -4,     3,
        14,     11,     12,     5,      -7,     -9,     -20,    -16,
        15,     11,     13,     6,      0,      -1,     -13,    -13,
        15,     11,     13,     4,      1,      -6,     -10,    -9,
        13,     14,     5,      -3,     -10,    -13,    -20,    -11,
        19,     22,     14,     4,      -4,     -2,     -4,     2,
        0,      -3,     1,      -9,     -18,    -5,     -8,     -12,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -12,    -10,    -5,     9,      3,      -30,    9,      2,
        -2,     -10,    7,      -3,     2,      6,      22,     49,
        -9,     -6,     -9,     -10,    -12,    7,      34,     55,
        -12,    -19,    -18,    -9,     -9,     -5,     11,     24,
        -12,    -16,    -19,    -18,    -8,     -5,     9,      22,
        -6,     -4,     -15,    -12,    -5,     4,      20,     37,
        -16,    -20,    3,      10,     7,      3,      6,      36,
        -10,    -10,    4,      11,     6,      -36,    -13,    25,
];

internal static readonly short[] EndGameQueenTable =
[
        -24,    -20,    -11,    -11,    -16,    -10,    -34,    9,
        -16,    -9,     -25,    0,      -2,     -16,    -45,    -7,
        -15,    -4,     6,      2,      23,     20,     -8,     4,
        -9,     9,      9,      14,     27,     38,     44,     31,
        -2,     5,      16,     25,     22,     34,     25,     40,
        -15,    -11,    14,     12,     14,     20,     19,     15,
        -10,    -3,     -21,    -18,    -12,    -11,    -31,    3,
        -15,    -16,    -17,    -6,     -9,     16,     14,     -3,
];

internal static readonly short[] MiddleGameKingTable =
[
        25,     50,     26,     -74,    9,      -61,    39,     48,
        -11,    -16,    -34,    -72,    -84,    -58,    -10,    19,
        -82,    -67,    -105,   -107,   -116,   -124,   -83,    -96,
        -105,   -95,    -115,   -150,   -145,   -138,   -137,   -162,
        -71,    -69,    -103,   -130,   -146,   -123,   -141,   -158,
        -79,    -43,    -96,    -104,   -93,    -104,   -75,    -87,
        72,     -9,     -37,    -63,    -68,    -47,    4,      26,
        38,     75,     38,     -59,    20,     -51,    52,     62,
];

internal static readonly short[] EndGameKingTable =
[
        -72,    -46,    -20,    5,      -33,    -2,     -38,    -89,
        -13,    18,     27,     39,     46,     33,     13,     -23,
        10,     42,     59,     68,     72,     64,     44,     22,
        15,     53,     75,     90,     88,     80,     68,     39,
        5,      45,     72,     86,     91,     78,     70,     39,
        11,     39,     57,     68,     66,     59,     43,     17,
        -38,    13,     28,     37,     39,     29,     8,      -26,
        -82,    -56,    -26,    -2,     -29,    -6,     -42,    -94,
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
