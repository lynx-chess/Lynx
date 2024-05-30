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
        +101, +137, +174, +209, +1115, 0,
        -101, -137, -174, -209, -1115, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +128, +179, +209, +382, +1426, 0,
        -128, -179, -209, -382, -1426, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -24,    -23,    -15,    -6,     -1,     26,     28,     -11,
        -25,    -25,    -5,     11,     17,     26,     22,     9,
        -23,    -16,    4,      18,     26,     30,     1,      -2,
        -23,    -12,    2,      20,     28,     28,     -0,     -4,
        -22,    -19,    -3,     4,      13,     23,     14,     3,
        -24,    -21,    -19,    -10,    -4,     19,     17,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        15,     10,     5,      -9,     7,      2,      -6,     -5,
        13,     9,      -1,     -12,    -6,     -8,     -7,     -7,
        27,     17,     -0,     -19,    -15,    -14,    4,      1,
        25,     16,     -2,     -15,    -14,    -11,    1,      -1,
        14,     6,      -3,     -11,    -3,     -6,     -6,     -8,
        18,     10,     9,      -8,     15,     5,      -3,     -2,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -139,   -17,    -50,    -29,    -9,     -20,    -5,     -85,
        -41,    -26,    -5,     15,     17,     21,     -14,    -11,
        -30,    -0,     16,     51,     55,     37,     31,     -3,
        -11,    23,     39,     53,     52,     54,     42,     17,
        -7,     22,     40,     41,     50,     53,     42,     16,
        -28,    2,      16,     44,     53,     31,     25,     -4,
        -44,    -19,    -2,     14,     16,     15,     -13,    -12,
        -155,   -19,    -49,    -21,    -7,     -12,    -13,    -75,
];

internal static readonly short[] EndGameKnightTable =
[
        -56,    -40,    -6,     -4,     -4,     -21,    -33,    -77,
        -10,    4,      7,      3,      4,      -0,     -5,     -11,
        -9,     7,      25,     26,     22,     7,      1,      -9,
        11,     12,     37,     39,     43,     36,     17,     -3,
        8,      17,     36,     42,     43,     32,     21,     4,
        -12,    10,     15,     30,     22,     8,      -1,     -5,
        -15,    7,      -1,     6,      -0,     -5,     -6,     -15,
        -61,    -38,    -1,     -6,     -5,     -20,    -32,    -77,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -11,    16,     -6,     -19,    -13,    -17,    -20,    5,
        7,      1,      5,      -19,    0,      -3,     26,     -12,
        -6,     4,      -7,     5,      -9,     10,     4,      25,
        -8,     -7,     -2,     25,     22,     -13,    1,      -2,
        -15,    -3,     -11,    20,     9,      -9,     -7,     4,
        4,      3,      5,      -3,     4,      3,      6,      21,
        10,     13,     8,      -7,     -5,     -1,     18,     -2,
        13,     19,     6,      -34,    -16,    -23,    2,      -11,
];

internal static readonly short[] EndGameBishopTable =
[
        -2,     14,     -7,     3,      -1,     7,      -1,     -17,
        -0,     -9,     -7,     -0,     -3,     -12,    -3,     -10,
        11,     9,      5,      -1,     8,      2,      2,      9,
        11,     2,      5,      4,      3,      6,      1,      4,
        6,      5,      3,      8,      1,      5,      2,      5,
        8,      -1,     -1,     -4,     3,      -2,     0,      5,
        -9,     -11,    -18,    -2,     -4,     -6,     -3,     -4,
        1,      -12,    -5,     7,      7,      8,      -3,     -6,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -5,     1,      12,     2,      6,      -4,
        -28,    -17,    -11,    -8,     4,      4,      20,     -4,
        -31,    -19,    -20,    -9,     6,      12,     50,     26,
        -26,    -22,    -18,    -7,     -4,     7,      36,     17,
        -21,    -17,    -13,    -3,     -6,     7,      27,     12,
        -23,    -15,    -16,    0,      5,      21,     49,     26,
        -26,    -26,    -7,     -2,     5,      2,      26,     1,
        -2,     -3,     -1,     10,     19,     6,      14,     8,
];

internal static readonly short[] EndGameRookTable =
[
        6,      4,      8,      -0,     -10,    4,      0,      -2,
        17,     18,     18,     9,      -1,     -1,     -4,     2,
        13,     10,     12,     4,      -6,     -10,    -21,    -17,
        15,     11,     12,     5,      -1,     -2,     -14,    -14,
        14,     11,     12,     3,      0,      -7,     -11,    -10,
        13,     13,     4,      -3,     -10,    -13,    -22,    -11,
        19,     21,     14,     4,      -2,     -1,     -6,     2,
        2,      -1,     3,      -6,     -17,    -4,     -7,     -11,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -10,    -10,    -11,    5,      -0,     -34,    7,      2,
        -2,     -9,     7,      -1,     3,      4,      23,     50,
        -9,     -5,     -7,     -8,     -11,    9,      34,     56,
        -11,    -18,    -16,    -7,     -7,     -2,     13,     26,
        -11,    -14,    -16,    -15,    -7,     -2,     11,     23,
        -6,     -2,     -13,    -10,    -5,     6,      21,     37,
        -15,    -19,    3,      10,     8,      1,      7,      37,
        -8,     -10,    -1,     7,      3,      -39,    -15,    25,
];

internal static readonly short[] EndGameQueenTable =
[
        -23,    -17,    -5,     -10,    -16,    -10,    -32,    9,
        -14,    -9,     -24,    -1,     -3,     -15,    -46,    -6,
        -14,    -4,     6,      2,      21,     18,     -8,     2,
        -9,     10,     9,      13,     26,     36,     43,     30,
        -2,     5,      15,     24,     21,     31,     23,     39,
        -15,    -11,    14,     11,     13,     17,     17,     15,
        -9,     -3,     -20,    -18,    -12,    -11,    -32,    2,
        -15,    -14,    -12,    -4,     -10,    15,     14,     -3,
];

internal static readonly short[] MiddleGameKingTable =
[
        18,     48,     27,     -78,    3,      -61,    37,     39,
        -15,    -10,    -27,    -65,    -74,    -50,    -4,     14,
        -74,    -56,    -96,    -98,    -105,   -109,   -69,    -91,
        -94,    -79,    -107,   -154,   -148,   -123,   -118,   -151,
        -63,    -52,    -96,    -134,   -146,   -109,   -121,   -146,
        -74,    -34,    -89,    -95,    -84,    -89,    -61,    -82,
        69,     -3,     -29,    -55,    -58,    -37,    12,     22,
        31,     74,     40,     -60,    16,     -50,    52,     53,
];

internal static readonly short[] EndGameKingTable =
[
        -80,    -51,    -24,    2,      -36,    -7,     -43,    -99,
        -22,    15,     25,     38,     44,     31,     10,     -30,
        -2,     40,     67,     78,     80,     70,     41,     11,
        2,      53,     88,     116,    114,    90,     64,     25,
        -8,     44,     86,     113,    117,    89,     67,     24,
        -0,     37,     67,     78,     75,     65,     40,     7,
        -48,    10,     28,     37,     38,     27,     5,      -33,
        -92,    -62,    -31,    -5,     -32,    -11,    -47,    -104,
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
