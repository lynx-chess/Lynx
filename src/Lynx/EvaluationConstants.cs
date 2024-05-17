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
        +100, +392, +362, +489, +1116, 0,
        -100, -392, -362, -489, -1116, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +128, +443, +399, +775, +1423, 0,
        -128, -443, -399, -775, -1423, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -24,    -24,    -14,    -8,     -1,     29,     29,     -12,
        -23,    -26,    -4,     12,     19,     27,     23,     10,
        -24,    -18,    4,      18,     26,     30,     1,      -3,
        -24,    -14,    2,      20,     29,     28,     0,      -4,
        -21,    -19,    -3,     5,      15,     24,     16,     5,
        -25,    -22,    -18,    -12,    -3,     24,     18,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     10,     5,      -12,    7,      1,      -7,     -7,
        13,     10,     0,      -11,    -5,     -6,     -6,     -7,
        27,     17,     0,      -19,    -14,    -13,    4,      1,
        25,     16,     -1,     -15,    -13,    -10,    2,      -1,
        14,     7,      -3,     -9,     -2,     -5,     -5,     -7,
        16,     10,     9,      -12,    16,     5,      -4,     -4,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -146,   -23,    -50,    -30,    -12,    -21,    -10,    -97,
        -46,    -28,    -3,     15,     17,     24,     -14,    -18,
        -28,    1,      20,     56,     60,     40,     34,     -4,
        -11,    25,     44,     59,     59,     59,     45,     17,
        -7,     25,     45,     47,     57,     58,     45,     16,
        -26,    3,      19,     48,     57,     33,     27,     -6,
        -47,    -19,    1,      14,     16,     19,     -13,    -18,
        -163,   -25,    -48,    -21,    -9,     -11,    -18,    -87,
];

internal static readonly short[] EndGameKnightTable =
[
        -67,    -57,    -12,    -11,    -10,    -27,    -52,    -88,
        -19,    1,      13,     8,      9,      5,      -10,    -20,
        -14,    14,     35,     35,     32,     17,     8,      -14,
        7,      19,     47,     48,     52,     46,     24,     -7,
        4,      24,     46,     51,     52,     42,     28,     0,
        -16,    17,     26,     40,     32,     18,     5,      -10,
        -24,    3,      5,      11,     5,      0,      -11,    -24,
        -71,    -56,    -7,     -14,    -11,    -26,    -49,    -87,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -17,    14,     -2,     -15,    -12,    -15,    -21,    0,
        7,      2,      6,      -18,    2,      0,      27,     -12,
        -6,     5,      -4,     3,      -7,     13,     5,      27,
        -8,     -6,     -5,     23,     20,     -17,    3,      -1,
        -15,    -1,     -13,    19,     6,      -13,    -5,     5,
        5,      5,      7,      -4,     6,      6,      7,      22,
        9,      14,     10,     -5,     -3,     4,      19,     -2,
        7,      18,     11,     -30,    -13,    -20,    1,      -16,
];

internal static readonly short[] EndGameBishopTable =
[
        -11,    13,     -14,    3,      -2,     4,      -2,     -27,
        -2,     -7,     -3,     4,      2,      -10,    -3,     -15,
        12,     13,     6,      1,      10,     3,      6,      8,
        13,     7,      6,      -3,     -6,     6,      6,      6,
        8,      10,     5,      1,      -8,     7,      7,      7,
        9,      4,      0,      -1,     5,      -1,     3,      5,
        -13,    -10,    -14,    3,      1,      -3,     -2,     -9,
        -7,     -14,    -9,     7,      7,      6,      -3,     -13,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -4,     2,      14,     3,      7,      -3,
        -26,    -17,    -13,    -12,    0,      3,      17,     -4,
        -29,    -20,    -22,    -12,    3,      9,      48,     27,
        -25,    -19,    -16,    -6,     -3,     8,      37,     18,
        -20,    -14,    -12,    -4,     -6,     7,      29,     13,
        -22,    -16,    -18,    -4,     2,      18,     47,     26,
        -24,    -26,    -9,     -5,     2,      2,      23,     2,
        -3,     -4,     0,      12,     22,     8,      14,     9,
];

internal static readonly short[] EndGameRookTable =
[
        4,      2,      5,      -3,     -11,    3,      -1,     -4,
        16,     18,     18,     8,      -1,     -2,     -4,     2,
        13,     10,     12,     5,      -6,     -10,    -20,    -16,
        15,     11,     12,     6,      0,      -1,     -13,    -13,
        14,     10,     13,     4,      1,      -6,     -10,    -9,
        13,     13,     4,      -3,     -10,    -13,    -21,    -11,
        19,     22,     14,     4,      -4,     -3,     -5,     2,
        -1,     -4,     1,      -8,     -18,    -5,     -8,     -13,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -13,    -10,    -6,     9,      2,      -30,    7,      2,
        -2,     -10,    7,      -3,     2,      6,      22,     49,
        -9,     -6,     -9,     -10,    -12,    7,      33,     55,
        -13,    -19,    -20,    -10,    -10,    -6,     10,     24,
        -12,    -16,    -19,    -18,    -9,     -6,     9,      22,
        -6,     -4,     -15,    -12,    -5,     4,      20,     37,
        -15,    -20,    3,      10,     7,      3,      7,      37,
        -11,    -11,    4,      11,     6,      -36,    -12,    24,
];

internal static readonly short[] EndGameQueenTable =
[
        -24,    -20,    -11,    -11,    -16,    -11,    -32,    8,
        -17,    -10,    -25,    0,      -2,     -16,    -46,    -8,
        -16,    -5,     6,      2,      23,     20,     -8,     3,
        -8,     9,      11,     15,     28,     39,     45,     31,
        -2,     6,      17,     26,     23,     34,     26,     40,
        -16,    -11,    15,     12,     14,     20,     18,     15,
        -11,    -4,     -20,    -18,    -12,    -12,    -31,    2,
        -16,    -16,    -17,    -5,     -10,    17,     13,     -2,
];

internal static readonly short[] MiddleGameKingTable =
[
        26,     50,     27,     -74,    10,     -60,    40,     49,
        -11,    -15,    -33,    -72,    -84,    -56,    -8,     20,
        -82,    -68,    -106,   -107,   -115,   -124,   -82,    -95,
        -108,   -98,    -116,   -152,   -146,   -138,   -137,   -163,
        -76,    -70,    -105,   -131,   -147,   -123,   -142,   -159,
        -83,    -46,    -98,    -105,   -93,    -105,   -75,    -88,
        73,     -9,     -37,    -64,    -69,    -47,    5,      26,
        38,     74,     37,     -60,    20,     -52,    52,     61,
];

internal static readonly short[] EndGameKingTable =
[
        -71,    -46,    -20,    5,      -33,    -2,     -38,    -89,
        -12,    18,     27,     39,     46,     33,     12,     -23,
        10,     43,     59,     69,     72,     64,     44,     22,
        16,     54,     75,     90,     88,     81,     68,     39,
        6,      45,     73,     87,     92,     79,     71,     39,
        12,     39,     58,     68,     67,     59,     43,     18,
        -38,    13,     29,     38,     40,     30,     8,      -26,
        -82,    -56,    -26,    -1,     -28,    -5,     -41,    -94,
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
