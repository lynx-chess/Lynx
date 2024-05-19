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
        +101, +79, +362, +487, +1115, 0,
        -101, -79, -362, -487, -1115, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +128, +94, +399, +776, +1424, 0,
        -128, -94, -399, -776, -1424, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -24,    -24,    -14,    -9,     -2,     28,     28,     -12,
        -25,    -26,    -6,     10,     18,     25,     21,     9,
        -24,    -17,    3,      18,     26,     29,     0,      -3,
        -24,    -13,    1,      19,     27,     26,     -1,     -4,
        -22,    -19,    -4,     4,      13,     22,     15,     4,
        -24,    -22,    -19,    -12,    -5,     23,     18,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        15,     10,     6,      -12,    7,      2,      -6,     -6,
        13,     10,     0,      -11,    -5,     -6,     -6,     -7,
        27,     17,     0,      -18,    -14,    -13,    4,      1,
        25,     16,     -1,     -14,    -13,    -10,    2,      -1,
        14,     7,      -3,     -10,    -2,     -5,     -5,     -8,
        17,     10,     10,     -11,    16,     5,      -3,     -3,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        48,     111,    24,     44,     62,     54,     124,    95,
        88,     47,     -48,    -30,    -28,    -21,    60,     117,
        46,     -44,    -144,   -108,   -104,   -124,   -11,    70,
        64,     -20,    -120,   -105,   -105,   -105,   0,      92,
        67,     -20,    -119,   -117,   -107,   -106,   0,      91,
        48,     -41,    -145,   -115,   -107,   -131,   -18,    69,
        87,     55,     -45,    -30,    -29,    -26,    62,     116,
        30,     109,    26,     53,     65,     63,     116,    106,
];

internal static readonly short[] EndGameKnightTable =
[
        150,    93,     71,     72,     73,     56,     97,     129,
        131,    84,     -37,    -42,    -41,    -45,    73,     130,
        70,     -35,    -148,   -148,   -151,   -166,   -41,    70,
        90,     -30,    -136,   -135,   -131,   -137,   -26,    77,
        87,     -26,    -137,   -132,   -131,   -141,   -22,    83,
        67,     -32,    -157,   -143,   -151,   -165,   -44,    74,
        125,    87,     -44,    -39,    -45,    -50,    72,     126,
        145,    93,     76,     69,     72,     57,     100,    130,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -19,    14,     -3,     -16,    -12,    -16,    -22,    -1,
        6,      2,      6,      -18,    1,      -1,     27,     -13,
        -6,     5,      -5,     2,      -8,     12,     4,      26,
        -8,     -7,     -6,     23,     19,     -17,    2,      -2,
        -15,    -2,     -13,    19,     6,      -13,    -6,     4,
        5,      4,      7,      -5,     5,      6,      7,      22,
        8,      13,     10,     -6,     -3,     3,      19,     -3,
        6,      18,     11,     -31,    -14,    -21,    1,      -16,
];

internal static readonly short[] EndGameBishopTable =
[
        -10,    13,     -14,    3,      -1,     4,      -2,     -27,
        -2,     -7,     -3,     4,      2,      -9,     -3,     -15,
        12,     13,     6,      2,      11,     3,      6,      9,
        13,     7,      6,      -3,     -6,     6,      6,      7,
        8,      10,     6,      1,      -8,     7,      7,      7,
        10,     4,      0,      -1,     5,      -1,     4,      6,
        -13,    -10,    -14,    3,      1,      -2,     -2,     -9,
        -7,     -14,    -9,     7,      7,      6,      -3,     -13,
];

internal static readonly short[] MiddleGameRookTable =
[
        -3,     -9,     -3,     3,      15,     4,      8,      -2,
        -25,    -16,    -13,    -11,    1,      4,      18,     -3,
        -28,    -19,    -22,    -11,    4,      10,     50,     28,
        -23,    -19,    -16,    -6,     -2,     9,      38,     20,
        -18,    -14,    -12,    -3,     -5,     8,      29,     15,
        -21,    -15,    -18,    -3,     3,      19,     48,     27,
        -23,    -25,    -8,     -5,     3,      2,      24,     3,
        -2,     -3,     1,      13,     23,     9,      15,     10,
];

internal static readonly short[] EndGameRookTable =
[
        4,      2,      5,      -3,     -11,    2,      -1,     -4,
        16,     18,     18,     8,      -1,     -2,     -4,     2,
        13,     10,     12,     5,      -7,     -10,    -20,    -17,
        14,     11,     12,     6,      0,      -1,     -13,    -13,
        14,     10,     12,     4,      1,      -6,     -10,    -10,
        12,     13,     4,      -3,     -10,    -14,    -21,    -11,
        19,     21,     14,     4,      -4,     -3,     -5,     2,
        -1,     -4,     1,      -9,     -18,    -5,     -8,     -13,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -13,    -10,    -6,     9,      2,      -31,    8,      2,
        -2,     -10,    7,      -3,     2,      5,      22,     50,
        -9,     -6,     -9,     -10,    -12,    7,      33,     55,
        -12,    -19,    -19,    -9,     -9,     -6,     10,     24,
        -12,    -16,    -19,    -18,    -9,     -6,     9,      22,
        -6,     -4,     -15,    -12,    -6,     4,      20,     37,
        -15,    -20,    3,      10,     7,      3,      7,      37,
        -11,    -10,    4,      11,     6,      -36,    -14,    25,
];

internal static readonly short[] EndGameQueenTable =
[
        -25,    -21,    -12,    -12,    -17,    -10,    -34,    8,
        -17,    -10,    -25,    0,      -2,     -17,    -46,    -8,
        -16,    -4,     6,      2,      22,     19,     -8,     3,
        -9,     9,      9,      13,     26,     38,     44,     30,
        -3,     5,      16,     25,     22,     33,     25,     39,
        -16,    -11,    14,     11,     14,     19,     18,     15,
        -11,    -3,     -21,    -19,    -13,    -12,    -31,    2,
        -16,    -17,    -18,    -6,     -10,    16,     14,     -3,
];

internal static readonly short[] MiddleGameKingTable =
[
        25,     49,     26,     -75,    9,      -61,    39,     48,
        -12,    -16,    -34,    -72,    -84,    -57,    -9,     19,
        -82,    -68,    -106,   -107,   -115,   -124,   -82,    -95,
        -107,   -96,    -115,   -150,   -146,   -137,   -137,   -162,
        -75,    -69,    -103,   -130,   -146,   -121,   -140,   -158,
        -80,    -44,    -97,    -104,   -92,    -103,   -74,    -87,
        73,     -8,     -37,    -63,    -68,    -46,    5,      27,
        37,     75,     38,     -59,    21,     -51,    52,     62,
];

internal static readonly short[] EndGameKingTable =
[
        -71,    -45,    -19,    5,      -32,    -1,     -37,    -88,
        -12,    18,     28,     40,     46,     34,     13,     -22,
        11,     43,     60,     69,     72,     65,     44,     23,
        16,     54,     75,     90,     89,     81,     68,     40,
        6,      45,     73,     87,     92,     79,     71,     39,
        12,     39,     58,     69,     67,     59,     43,     18,
        -38,    13,     29,     38,     40,     30,     8,      -25,
        -81,    -56,    -26,    -1,     -28,    -5,     -41,    -94,
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
