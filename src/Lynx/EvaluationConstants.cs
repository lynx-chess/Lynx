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
        +99, +393, +365, +489, +1117, 0,
        -99, -393, -365, -489, -1117, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +132, +442, +399, +775, +1427, 0,
        -132, -442, -399, -775, -1427, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -21,    -19,    -12,    -7,     0,      31,     36,     -9,
        -28,    -28,    -9,     5,      12,     24,     20,     4,
        -23,    -13,    2,      16,     26,     30,     6,      -4,
        -23,    -9,     1,      18,     27,     28,     6,      -5,
        -26,    -23,    -8,     -2,     7,      19,     13,     -2,
        -22,    -16,    -15,    -11,    -2,     27,     25,     -17,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        11,     12,     7,      -13,    6,      2,      -4,     -9,
        8,      9,      -1,     -13,    -7,     -6,     -6,     -11,
        24,     18,     -1,     -21,    -15,    -12,    6,      -2,
        21,     17,     -3,     -16,    -14,    -10,    4,      -4,
        10,     7,      -4,     -11,    -3,     -4,     -5,     -12,
        14,     12,     10,     -12,    16,     6,      -1,     -6,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -147,   -23,    -51,    -31,    -12,    -21,    -10,    -99,
        -46,    -29,    -4,     15,     17,     22,     -15,    -19,
        -28,    1,      21,     56,     60,     40,     34,     -5,
        -11,    25,     44,     61,     60,     60,     45,     17,
        -8,     25,     46,     47,     58,     58,     46,     16,
        -27,    3,      20,     48,     57,     34,     28,     -6,
        -48,    -20,    0,      14,     16,     17,     -13,    -20,
        -165,   -26,    -48,    -20,    -9,     -12,    -18,    -91,
];

internal static readonly short[] EndGameKnightTable =
[
        -67,    -58,    -11,    -10,    -9,     -26,    -53,    -86,
        -18,    2,      14,     9,      9,      5,      -10,    -19,
        -13,    15,     36,     36,     33,     18,     9,      -13,
        8,      20,     48,     48,     52,     47,     25,     -6,
        5,      25,     47,     52,     53,     43,     28,     0,
        -15,    18,     27,     40,     33,     19,     6,      -9,
        -24,    4,      6,      12,     5,      1,      -11,    -23,
        -70,    -56,    -6,     -14,    -11,    -26,    -50,    -84,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -18,    15,     -2,     -13,    -11,    -15,    -23,    -1,
        6,      3,      8,      -17,    4,      -1,     28,     -15,
        -7,     7,      -3,     5,      -6,     15,     5,      27,
        -6,     -6,     -4,     25,     22,     -15,    4,      -1,
        -15,    0,      -12,    21,     8,      -11,    -4,     5,
        4,      5,      8,      -3,     7,      8,      8,      22,
        7,      14,     12,     -5,     -2,     3,      20,     -5,
        7,      18,     12,     -30,    -12,    -21,    0,      -17,
];

internal static readonly short[] EndGameBishopTable =
[
        -11,    14,     -14,    3,      -1,     4,      -1,     -27,
        -1,     -7,     -3,     5,      3,      -8,     -3,     -13,
        13,     14,     7,      3,      12,     4,      7,      9,
        13,     8,      7,      -2,     -5,     7,      6,      7,
        9,      11,     7,      2,      -7,     8,      8,      8,
        10,     5,      2,      1,      7,      0,      5,      7,
        -12,    -10,    -14,    3,      2,      -2,     -3,     -9,
        -7,     -13,    -9,     7,      7,      7,      -2,     -12,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -4,     3,      15,     3,      7,      -2,
        -26,    -17,    -13,    -12,    0,      3,      18,     -3,
        -28,    -19,    -22,    -12,    4,      10,     50,     28,
        -24,    -20,    -16,    -7,     -3,     10,     39,     20,
        -18,    -14,    -12,    -4,     -6,     8,      30,     16,
        -21,    -15,    -18,    -4,     3,      19,     49,     28,
        -23,    -25,    -9,     -7,     2,      1,      24,     3,
        -2,     -4,     0,      12,     22,     8,      15,     10,
];

internal static readonly short[] EndGameRookTable =
[
        3,      1,      5,      -4,     -12,    2,      -1,     -5,
        16,     18,     17,     7,      -2,     -3,     -5,     2,
        13,     11,     12,     5,      -7,     -10,    -21,    -17,
        14,     11,     12,     5,      -1,     -1,     -13,    -13,
        14,     10,     12,     4,      1,      -6,     -11,    -10,
        12,     13,     4,      -3,     -10,    -14,    -21,    -12,
        18,     21,     14,     3,      -5,     -3,     -6,     1,
        -1,     -4,     1,      -9,     -19,    -6,     -9,     -13,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -12,    -9,     -6,     8,      2,      -31,    7,      2,
        -2,     -10,    7,      -4,     2,      5,      22,     49,
        -10,    -7,     -9,     -9,     -11,    7,      34,     55,
        -13,    -19,    -19,    -8,     -10,    -5,     11,     24,
        -12,    -16,    -18,    -18,    -8,     -5,     9,      22,
        -7,     -4,     -14,    -11,    -6,     4,      20,     37,
        -15,    -20,    3,      9,      7,      2,      7,      36,
        -11,    -11,    4,      11,     6,      -37,    -16,    25,
];

internal static readonly short[] EndGameQueenTable =
[
        -24,    -22,    -10,    -11,    -16,    -10,    -34,    8,
        -17,    -10,    -25,    1,      -2,     -16,    -46,    -7,
        -15,    -3,     7,      2,      24,     21,     -7,     3,
        -9,     9,      10,     14,     28,     39,     45,     31,
        -2,     6,      16,     26,     23,     35,     26,     40,
        -15,    -10,    15,     12,     15,     20,     19,     15,
        -11,    -3,     -21,    -17,    -13,    -11,    -31,    2,
        -15,    -16,    -17,    -6,     -9,     17,     15,     -2,
];

internal static readonly short[] MiddleGameKingTable =
[
        26,     51,     27,     -74,    9,      -61,    39,     48,
        -7,     -15,    -35,    -72,    -84,    -58,    -9,     22,
        -83,    -68,    -105,   -107,   -117,   -125,   -84,    -96,
        -106,   -95,    -115,   -150,   -145,   -137,   -136,   -162,
        -72,    -69,    -102,   -131,   -146,   -123,   -141,   -158,
        -79,    -44,    -97,    -104,   -94,    -105,   -76,    -88,
        72,     -9,     -38,    -64,    -69,    -47,    5,      29,
        36,     75,     38,     -59,    20,     -51,    52,     62,
];

internal static readonly short[] EndGameKingTable =
[
        -72,    -46,    -20,    5,      -33,    -2,     -38,    -89,
        -14,    17,     27,     39,     46,     33,     12,     -24,
        10,     42,     59,     68,     72,     64,     44,     22,
        15,     53,     75,     90,     88,     81,     68,     39,
        5,      45,     72,     86,     91,     78,     70,     39,
        11,     38,     57,     68,     66,     58,     43,     17,
        -39,    13,     29,     37,     39,     29,     7,      -27,
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
