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
        +104, +391, +363, +487, +1118, 0,
        -104, -391, -363, -487, -1118, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +136, +445, +400, +776, +1424, 0,
        -136, -445, -400, -776, -1424, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -25,    -22,    -14,    -10,    -2,     28,     32,     -13,
        -26,    -24,    -6,     9,      17,     28,     25,     7,
        -25,    -15,    2,      17,     25,     30,     3,      -6,
        -25,    -11,    0,      18,     27,     28,     2,      -7,
        -23,    -17,    -4,     3,      13,     24,     19,     1,
        -25,    -20,    -19,    -14,    -4,     23,     22,     -21,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        11,     11,     6,      -12,    6,      3,      -5,     -10,
        9,      11,     0,      -12,    -6,     -5,     -4,     -10,
        24,     18,     1,      -19,    -14,    -10,    8,      -1,
        22,     18,     -1,     -14,    -12,    -7,     6,      -3,
        10,     8,      -3,     -10,    -2,     -3,     -3,     -11,
        13,     11,     9,      -12,    15,     6,      -2,     -7,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -146,   -22,    -50,    -30,    -12,    -19,    -9,     -97,
        -45,    -28,    -3,     16,     18,     22,     -14,    -16,
        -27,    2,      20,     56,     58,     40,     34,     -4,
        -10,    25,     44,     59,     58,     59,     44,     18,
        -7,     24,     45,     46,     56,     58,     45,     17,
        -26,    4,      20,     49,     56,     34,     28,     -5,
        -46,    -19,    1,      15,     17,     17,     -13,    -17,
        -163,   -24,    -47,    -20,    -9,     -9,     -17,    -87,
];

internal static readonly short[] EndGameKnightTable =
[
        -66,    -59,    -12,    -11,    -11,    -28,    -53,    -88,
        -18,    1,      13,     8,      8,      6,      -10,    -20,
        -14,    15,     35,     36,     34,     18,     9,      -13,
        7,      19,     48,     50,     53,     48,     25,     -7,
        4,      24,     47,     53,     54,     43,     29,     0,
        -16,    18,     26,     41,     34,     18,     6,      -10,
        -25,    3,      6,      11,     4,      1,      -11,    -25,
        -70,    -57,    -7,     -15,    -11,    -27,    -50,    -85,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -18,    14,     -2,     -15,    -11,    -14,    -22,    0,
        7,      2,      6,      -17,    2,      -2,     27,     -12,
        -6,     6,      -4,     3,      -9,     12,     4,      27,
        -7,     -6,     -5,     22,     19,     -17,    2,      -1,
        -15,    -1,     -12,    18,     5,      -12,    -6,     5,
        5,      5,      7,      -4,     4,      6,      7,      22,
        9,      13,     11,     -5,     -3,     1,      19,     -3,
        7,      18,     11,     -30,    -13,    -19,    2,      -15,
];

internal static readonly short[] EndGameBishopTable =
[
        -11,    13,     -14,    2,      -2,     3,      -2,     -28,
        -3,     -7,     -3,     4,      2,      -9,     -4,     -15,
        12,     13,     7,      2,      12,     4,      7,      9,
        12,     7,      6,      -2,     -5,     7,      6,      6,
        8,      10,     6,      1,      -7,     7,      8,      7,
        10,     4,      1,      0,      6,      0,      4,      6,
        -13,    -10,    -14,    2,      1,      -2,     -3,     -10,
        -8,     -14,    -10,    6,      6,      5,      -5,     -14,
];

internal static readonly short[] MiddleGameRookTable =
[
        -3,     -9,     -3,     3,      15,     6,      9,      -1,
        -25,    -16,    -13,    -11,    1,      2,      17,     -3,
        -28,    -19,    -22,    -12,    2,      9,      50,     28,
        -23,    -20,    -17,    -8,     -4,     9,      37,     20,
        -18,    -15,    -12,    -5,     -7,     8,      29,     15,
        -21,    -15,    -18,    -4,     1,      19,     48,     27,
        -23,    -25,    -9,     -5,     3,      1,      23,     3,
        -2,     -3,     1,      12,     22,     11,     17,     10,
];

internal static readonly short[] EndGameRookTable =
[
        4,      1,      5,      -4,     -12,    1,      -2,     -5,
        16,     18,     18,     7,      -2,     -2,     -4,     2,
        12,     10,     12,     5,      -6,     -9,     -20,    -16,
        14,     11,     13,     7,      0,      0,      -12,    -13,
        13,     10,     13,     5,      2,      -5,     -10,    -9,
        12,     13,     4,      -3,     -9,     -13,    -20,    -11,
        19,     21,     14,     3,      -4,     -2,     -5,     2,
        -1,     -4,     1,      -9,     -19,    -7,     -9,     -13,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -12,    -9,     -5,     10,     4,      -28,    10,     2,
        -2,     -10,    7,      -3,     2,      5,      22,     49,
        -9,     -6,     -9,     -10,    -13,    7,      33,     55,
        -12,    -19,    -18,    -9,     -10,    -5,     10,     25,
        -11,    -15,    -18,    -18,    -8,     -5,     9,      22,
        -6,     -3,     -15,    -12,    -6,     4,      20,     37,
        -15,    -20,    3,      10,     8,      2,      6,      37,
        -10,    -10,    5,      12,     7,      -35,    -12,    25,
];

internal static readonly short[] EndGameQueenTable =
[
        -23,    -20,    -11,    -12,    -17,    -11,    -34,    9,
        -16,    -9,     -24,    1,      -1,     -15,    -45,    -6,
        -15,    -4,     7,      2,      24,     21,     -8,     4,
        -9,     9,      9,      14,     28,     39,     45,     31,
        -2,     6,      16,     26,     23,     34,     25,     40,
        -15,    -11,    15,     12,     15,     20,     19,     16,
        -10,    -3,     -20,    -18,    -12,    -11,    -31,    3,
        -14,    -16,    -17,    -6,     -10,    15,     13,     -2,
];

internal static readonly short[] MiddleGameKingTable =
[
        22,     49,     26,     -76,    8,      -61,    38,     45,
        -10,    -12,    -29,    -68,    -79,    -54,    -6,     20,
        -77,    -59,    -96,    -99,    -109,   -114,   -75,    -89,
        -100,   -88,    -105,   -143,   -135,   -126,   -127,   -155,
        -67,    -60,    -93,    -122,   -136,   -111,   -131,   -150,
        -74,    -36,    -89,    -96,    -85,    -95,    -68,    -80,
        76,     -5,     -33,    -60,    -64,    -44,    8,      28,
        36,     74,     38,     -61,    19,     -51,    51,     58,
];

internal static readonly short[] EndGameKingTable =
[
        -68,    -44,    -18,    7,      -30,    0,      -35,    -84,
        -12,    16,     24,     38,     43,     31,     11,     -22,
        8,      37,     53,     62,     66,     57,     38,     18,
        12,     48,     67,     82,     78,     71,     60,     34,
        2,      39,     64,     78,     81,     69,     63,     33,
        9,      34,     52,     62,     60,     51,     37,     13,
        -39,    11,     26,     35,     37,     28,     7,      -25,
        -80,    -54,    -25,    0,      -27,    -4,     -39,    -89,
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
