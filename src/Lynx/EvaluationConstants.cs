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
        +101, +393, +173, +200, +547, 0,
        -101, -393, -173, -200, -547, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +129, +445, +206, +374, +737, 0,
        -129, -445, -206, -374, -737, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -24,    -23,    -14,    -8,     -2,     27,     28,     -12,
        -25,    -24,    -5,     11,     19,     26,     22,     9,
        -22,    -15,    4,      19,     26,     31,     1,      -2,
        -22,    -12,    2,      20,     27,     28,     0,      -3,
        -22,    -18,    -4,     4,      14,     23,     15,     4,
        -24,    -21,    -19,    -12,    -5,     21,     17,     -20,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        14,     9,      4,      -8,     7,      1,      -7,     -7,
        12,     9,      -0,     -12,    -6,     -7,     -7,     -8,
        26,     16,     -1,     -19,    -15,    -14,    3,      0,
        24,     15,     -2,     -15,    -14,    -11,    1,      -2,
        14,     6,      -3,     -10,    -3,     -5,     -6,     -8,
        17,     9,      8,      -7,     16,     4,      -4,     -4,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -147,   -23,    -52,    -32,    -14,    -23,    -10,    -99,
        -45,    -27,    -3,     15,     17,     23,     -15,    -18,
        -28,    2,      20,     57,     60,     40,     34,     -5,
        -9,     25,     45,     60,     60,     60,     45,     17,
        -6,     25,     46,     48,     58,     59,     45,     17,
        -26,    5,      19,     49,     58,     33,     27,     -6,
        -47,    -19,    -0,     14,     15,     17,     -14,    -20,
        -163,   -26,    -49,    -23,    -11,    -13,    -18,    -89,
];

internal static readonly short[] EndGameKnightTable =
[
        -66,    -49,    -11,    -10,    -8,     -26,    -44,    -87,
        -19,    1,      13,     9,      9,      6,      -10,    -20,
        -14,    13,     35,     35,     32,     16,     7,      -14,
        5,      18,     46,     48,     51,     45,     23,     -8,
        2,      23,     46,     51,     52,     41,     27,     -2,
        -16,    16,     25,     40,     32,     17,     5,      -10,
        -25,    3,      6,      12,     5,      1,      -11,    -24,
        -71,    -46,    -6,     -12,    -9,     -25,    -43,    -87,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -10,    16,     -6,     -17,    -11,    -16,    -20,    3,
        7,      1,      5,      -20,    -0,     -2,     26,     -13,
        -7,     4,      -8,     5,      -8,     9,      4,      25,
        -8,     -7,     -3,     24,     22,     -13,    1,      -2,
        -15,    -3,     -12,    20,     9,      -9,     -7,     5,
        4,      3,      3,      -4,     5,      3,      6,      21,
        9,      13,     8,      -8,     -5,     0,      18,     -3,
        14,     20,     7,      -31,    -13,    -21,    1,      -12,
];

internal static readonly short[] EndGameBishopTable =
[
        -3,     13,     -7,     2,      -1,     6,      -1,     -18,
        -1,     -9,     -7,     1,      -2,     -13,    -4,     -11,
        11,     9,      6,      -0,     9,      2,      2,      7,
        10,     2,      6,      5,      3,      6,      0,      3,
        5,      5,      4,      9,      1,      5,      1,      4,
        8,      -1,     0,      -2,     4,      -2,     -0,     4,
        -10,    -11,    -18,    -1,     -4,     -7,     -4,     -5,
        -0,     -12,    -5,     6,      7,      7,      -3,     -6,
];

internal static readonly short[] MiddleGameRookTable =
[
        -2,     -9,     -3,     5,      17,     5,      8,      -2,
        -28,    -17,    -11,    -9,     4,      6,      20,     -5,
        -31,    -19,    -21,    -10,    6,      12,     50,     26,
        -26,    -23,    -18,    -7,     -3,     7,      37,     17,
        -21,    -17,    -14,    -4,     -6,     7,      28,     13,
        -23,    -15,    -16,    -1,     5,      21,     49,     26,
        -26,    -26,    -7,     -2,     6,      4,      27,     1,
        -1,     -2,     2,      15,     25,     11,     16,     10,
];

internal static readonly short[] EndGameRookTable =
[
        5,      4,      7,      -2,     -10,    5,      -1,     -3,
        16,     18,     18,     9,      -0,     -1,     -5,     2,
        12,     10,     12,     5,      -6,     -9,     -20,    -17,
        14,     11,     13,     6,      -1,     -0,     -13,    -14,
        14,     11,     13,     4,      1,      -6,     -10,    -10,
        12,     13,     4,      -2,     -9,     -13,    -21,    -12,
        18,     21,     14,     5,      -2,     -1,     -6,     1,
        1,      -2,     2,      -8,     -17,    -4,     -8,     -12,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -16,    -14,    -10,    3,      1,      -30,    8,      -6,
        -9,     -10,    8,      1,      6,      10,     24,     43,
        -14,    -5,     -4,     -3,     -5,     12,     35,     49,
        -15,    -18,    -13,    -3,     -4,     -0,     11,     20,
        -15,    -15,    -13,    -11,    -3,     1,      10,     17,
        -11,    -2,     -10,    -4,     2,      10,     22,     31,
        -22,    -20,    4,      13,     11,     7,      9,      31,
        -14,    -13,    0,      6,      3,      -36,    -14,    18,
];

internal static readonly short[] EndGameQueenTable =
[
        -28,    -21,    -10,    -3,     -15,    -9,     -39,    5,
        -19,    -12,    -22,    3,      1,      -13,    -43,    -8,
        -21,    -6,     7,      2,      27,     26,     -4,     2,
        -17,    5,      10,     22,     35,     40,     42,     27,
        -9,     1,      16,     33,     31,     35,     23,     37,
        -20,    -13,    16,     12,     18,     25,     22,     14,
        -13,    -7,     -19,    -15,    -9,     -8,     -30,    -0,
        -20,    -18,    -17,    2,      -8,     16,     8,      -8,
];

internal static readonly short[] MiddleGameKingTable =
[
        27,     52,     28,     -77,    5,      -62,    40,     50,
        -11,    -18,    -36,    -74,    -86,    -60,    -12,    18,
        -84,    -70,    -108,   -111,   -119,   -127,   -85,    -96,
        -106,   -97,    -117,   -153,   -150,   -140,   -137,   -160,
        -77,    -70,    -105,   -133,   -149,   -124,   -141,   -157,
        -83,    -47,    -100,   -107,   -96,    -107,   -77,    -88,
        72,     -11,    -39,    -65,    -70,    -49,    3,      25,
        40,     76,     39,     -61,    17,     -52,    54,     64,
];

internal static readonly short[] EndGameKingTable =
[
        -71,    -45,    -19,    7,      -30,    -0,     -37,    -88,
        -12,    18,     28,     40,     46,     34,     13,     -22,
        11,     43,     60,     69,     73,     65,     45,     23,
        16,     55,     76,     91,     89,     82,     69,     40,
        6,      46,     74,     87,     92,     79,     71,     40,
        12,     40,     59,     69,     67,     60,     44,     19,
        -38,    13,     29,     38,     40,     30,     9,      -24,
        -82,    -56,    -26,    0,      -26,    -4,     -41,    -94,
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
