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
        +101, +137, +173, +210, +1115, 0,
        -101, -137, -173, -210, -1115, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +129, +179, +209, +382, +1425, 0,
        -129, -179, -209, -382, -1425, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -24,    -23,    -15,    -5,     -1,     27,     28,     -12,
        -25,    -25,    -5,     11,     18,     26,     22,     9,
        -23,    -16,    4,      18,     25,     30,     1,      -2,
        -23,    -12,    1,      20,     27,     28,     -0,     -3,
        -22,    -19,    -3,     4,      14,     23,     15,     4,
        -24,    -21,    -20,    -9,     -4,     21,     17,     -20,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     9,      4,      -10,    7,      1,      -7,     -7,
        12,     9,      -1,     -12,    -6,     -7,     -7,     -8,
        26,     16,     -1,     -19,    -15,    -14,    3,      0,
        24,     15,     -2,     -15,    -14,    -11,    1,      -2,
        13,     6,      -4,     -10,    -3,     -5,     -6,     -8,
        17,     9,      8,      -9,     15,     4,      -4,     -4,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -139,   -17,    -50,    -28,    -8,     -19,    -4,     -88,
        -42,    -26,    -5,     15,     17,     21,     -15,    -13,
        -30,    -1,     17,     51,     56,     37,     31,     -4,
        -12,    22,     38,     53,     52,     54,     42,     16,
        -8,     21,     40,     40,     50,     52,     42,     15,
        -29,    2,      16,     44,     54,     31,     25,     -5,
        -44,    -20,    -2,     14,     16,     16,     -14,    -14,
        -156,   -20,    -48,    -20,    -6,     -12,    -12,    -78,
];

internal static readonly short[] EndGameKnightTable =
[
        -55,    -38,    -6,     -3,     -3,     -19,    -34,    -76,
        -9,     5,      8,      4,      5,      -0,     -4,     -11,
        -9,     8,      25,     27,     23,     7,      2,      -8,
        11,     12,     38,     40,     43,     37,     17,     -2,
        8,      17,     37,     42,     44,     32,     21,     5,
        -11,    11,     16,     31,     23,     9,      -1,     -5,
        -15,    8,      0,      7,      1,      -5,     -6,     -15,
        -60,    -36,    -1,     -5,     -4,     -18,    -33,    -75,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -10,    16,     -5,     -18,    -10,    -16,    -20,    4,
        7,      1,      5,      -18,    1,      -2,     26,     -14,
        -6,     4,      -6,     5,      -7,     11,     4,      25,
        -8,     -8,     -3,     24,     22,     -13,    1,      -2,
        -15,    -3,     -11,    20,     9,      -9,     -7,     4,
        4,      3,      5,      -2,     6,      4,      6,      21,
        9,      13,     8,      -6,     -4,     1,      18,     -4,
        14,     20,     7,      -32,    -13,    -22,    1,      -11,
];

internal static readonly short[] EndGameBishopTable =
[
        -1,     14,     -6,     3,      -0,     8,      -0,     -17,
        -0,     -8,     -7,     1,      -2,     -12,    -3,     -10,
        11,     10,     6,      0,      9,      3,      3,      9,
        11,     3,      6,      6,      4,      7,      1,      5,
        6,      6,      4,      10,     2,      6,      2,      5,
        9,      0,      0,      -2,     4,      -1,     1,      5,
        -9,     -10,    -17,    -1,     -4,     -6,     -3,     -4,
        1,      -11,    -5,     7,      8,      8,      -2,     -5,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -9,     -4,     3,      16,     5,      8,      -2,
        -28,    -17,    -11,    -9,     4,      6,      20,     -5,
        -31,    -20,    -21,    -11,    6,      11,     50,     25,
        -27,    -23,    -19,    -9,     -5,     6,      35,     16,
        -22,    -18,    -14,    -5,     -8,     5,      26,     12,
        -24,    -16,    -17,    -1,     5,      21,     49,     25,
        -26,    -26,    -7,     -2,     5,      4,      26,     1,
        -2,     -3,     1,      13,     24,     10,     16,     10,
];

internal static readonly short[] EndGameRookTable =
[
        6,      3,      7,      -2,     -10,    5,      -1,     -3,
        15,     18,     18,     8,      -1,     -2,     -5,     1,
        12,     10,     12,     5,      -6,     -9,     -21,    -17,
        14,     11,     13,     6,      -0,     -0,     -13,    -14,
        13,     10,     13,     4,      1,      -5,     -10,    -10,
        12,     12,     4,      -3,     -9,     -13,    -21,    -12,
        18,     21,     14,     4,      -2,     -2,     -6,     1,
        1,      -2,     2,      -7,     -17,    -3,     -8,     -12,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -10,    -10,    -10,    5,      1,      -31,    8,      1,
        -3,     -11,    6,      -2,     3,      5,      22,     49,
        -10,    -6,     -8,     -9,     -11,    8,      33,     55,
        -12,    -19,    -18,    -8,     -9,     -4,     11,     24,
        -12,    -15,    -18,    -17,    -8,     -4,     10,     22,
        -7,     -3,     -14,    -11,    -4,     5,      20,     36,
        -16,    -20,    2,      10,     7,      2,      6,      36,
        -8,     -10,    0,      7,      4,      -37,    -13,    25,
];

internal static readonly short[] EndGameQueenTable =
[
        -26,    -19,    -6,     -8,     -15,    -10,    -33,    8,
        -16,    -9,     -24,    -1,     -3,     -15,    -46,    -7,
        -16,    -5,     5,      1,      21,     19,     -8,     3,
        -10,    8,      9,      13,     27,     37,     44,     30,
        -3,     4,      15,     25,     22,     32,     25,     40,
        -16,    -11,    13,     11,     13,     18,     18,     15,
        -11,    -3,     -20,    -18,    -12,    -11,    -31,    3,
        -17,    -16,    -13,    -1,     -8,     17,     13,     -3,
];

internal static readonly short[] MiddleGameKingTable =
[
        27,     52,     27,     -77,    5,      -62,    41,     51,
        -13,    -17,    -36,    -73,    -85,    -60,    -12,    18,
        -84,    -70,    -108,   -111,   -118,   -126,   -85,    -97,
        -108,   -98,    -117,   -153,   -150,   -140,   -138,   -162,
        -77,    -71,    -106,   -133,   -149,   -124,   -142,   -159,
        -83,    -47,    -100,   -107,   -96,    -106,   -77,    -89,
        72,     -11,    -40,    -65,    -70,    -49,    3,      25,
        40,     77,     38,     -62,    16,     -52,    54,     65,
];

internal static readonly short[] EndGameKingTable =
[
        -71,    -45,    -19,    7,      -31,    -1,     -37,    -89,
        -12,    18,     27,     39,     45,     33,     13,     -23,
        10,     43,     59,     69,     72,     64,     44,     22,
        15,     54,     76,     90,     89,     81,     68,     39,
        6,      45,     73,     87,     92,     79,     71,     39,
        12,     39,     58,     69,     67,     59,     43,     18,
        -38,    13,     29,     37,     40,     30,     8,      -25,
        -83,    -57,    -26,    -0,     -27,    -5,     -42,    -95,
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
