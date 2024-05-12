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
        +109, +399, +366, +492, +1127, 0,
        -109, -399, -366, -492, -1127, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +133, +439, +392, +768, +1413, 0,
        -133, -439, -392, -768, -1413, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -27,    -24,    -16,    -12,    -3,     29,     32,     -11,
        -27,    -25,    -7,     9,      17,     29,     27,     9,
        -26,    -16,    1,      17,     26,     31,     5,      -3,
        -26,    -12,    0,      19,     27,     28,     5,      -4,
        -24,    -19,    -5,     2,      13,     25,     20,     3,
        -27,    -21,    -20,    -16,    -6,     23,     22,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        13,     12,     7,      -12,    6,      1,      -5,     -8,
        11,     12,     0,      -12,    -7,     -5,     -4,     -9,
        26,     19,     1,      -19,    -14,    -10,    7,      -1,
        23,     19,     0,      -15,    -13,    -8,     5,      -3,
        12,     9,      -2,     -11,    -3,     -4,     -4,     -10,
        15,     13,     9,      -11,    15,     3,      -2,     -4,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -146,   -24,    -52,    -32,    -15,    -22,    -12,    -95,
        -45,    -28,    -5,     15,     16,     24,     -13,    -14,
        -29,    0,      18,     57,     60,     40,     33,     -4,
        -11,    25,     42,     59,     58,     58,     44,     17,
        -8,     24,     44,     46,     56,     57,     44,     17,
        -27,    2,      18,     48,     57,     33,     26,     -5,
        -45,    -18,    1,      14,     15,     18,     -13,    -15,
        -164,   -26,    -50,    -22,    -11,    -13,    -20,    -85,
];

internal static readonly short[] EndGameKnightTable =
[
        -63,    -61,    -11,    -12,    -12,    -28,    -52,    -82,
        -16,    2,      14,     9,      9,      6,      -11,    -21,
        -13,    15,     36,     36,     34,     17,     10,     -12,
        6,      19,     48,     49,     54,     48,     24,     -5,
        5,      25,     46,     53,     54,     42,     29,     -1,
        -14,    19,     27,     40,     32,     19,     7,      -8,
        -25,    3,      6,      11,     4,      1,      -10,    -25,
        -66,    -58,    -7,     -16,    -14,    -28,    -50,    -80,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -17,    15,     -2,     -15,    -13,    -17,    -22,    3,
        7,      2,      7,      -18,    2,      1,      28,     -4,
        -6,     6,      -4,     3,      -9,     14,     5,      28,
        -5,     -6,     -6,     23,     20,     -18,    2,      -2,
        -14,    -1,     -13,    20,     5,      -14,    -6,     5,
        4,      5,      7,      -4,     4,      8,      7,      24,
        10,     13,     11,     -5,     -3,     4,      19,     1,
        9,      19,     11,     -31,    -14,    -22,    1,      -13,
];

internal static readonly short[] EndGameBishopTable =
[
        -7,     16,     -13,    3,      -1,     4,      0,      -25,
        1,      -6,     -3,     5,      2,      -9,     -3,     -15,
        15,     14,     7,      2,      11,     3,      7,      10,
        13,     8,      6,      -4,     -8,     7,      6,      8,
        9,      10,     6,      -1,     -10,    6,      8,      10,
        11,     4,      0,      -1,     5,      -2,     4,      8,
        -9,     -8,     -14,    3,      0,      -4,     -2,     -6,
        -7,     -12,    -9,     8,      6,      6,      -3,     -10,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -9,     -5,     0,      11,     1,      6,      -4,
        -25,    -16,    -12,    -11,    1,      5,      19,     -1,
        -27,    -18,    -22,    -12,    6,      12,     52,     31,
        -25,    -19,    -17,    -5,     -4,     11,     39,     22,
        -17,    -16,    -12,    -3,     -6,     10,     32,     16,
        -21,    -14,    -18,    -3,     3,      21,     49,     28,
        -22,    -25,    -9,     -6,     3,      2,      24,     4,
        -3,     -4,     -1,     9,      18,     5,      13,     7,
];

internal static readonly short[] EndGameRookTable =
[
        7,      3,      6,      -3,     -13,    3,      1,      -2,
        17,     20,     18,     8,      -2,     -2,     -3,     2,
        14,     11,     11,     6,      -9,     -10,    -21,    -17,
        16,     11,     12,     4,      0,      -2,     -14,    -13,
        15,     10,     12,     2,      1,      -6,     -12,    -9,
        14,     12,     5,      -3,     -10,    -14,    -20,    -11,
        20,     22,     14,     4,      -5,     -3,     -5,     2,
        2,      -2,     2,      -10,    -20,    -6,     -7,     -10,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -12,    -9,     -6,     9,      2,      -33,    5,      -4,
        -2,     -11,    8,      -2,     3,      5,      23,     61,
        -8,     -6,     -9,     -9,     -12,    7,      33,     58,
        -13,    -19,    -18,    -9,     -10,    -4,     11,     24,
        -11,    -16,    -19,    -20,    -11,    -5,     10,     22,
        -8,     -3,     -14,    -10,    -6,     5,      20,     39,
        -15,    -21,    4,      11,     9,      4,      7,      45,
        -9,     -9,     6,      11,     6,      -37,    -17,    24,
];

internal static readonly short[] EndGameQueenTable =
[
        -21,    -21,    -10,    -14,    -21,    -9,     -31,    14,
        -13,    -10,    -28,    -2,     -3,     -15,    -40,    -5,
        -16,    -5,     5,      -1,     21,     20,     -2,     3,
        -9,     8,      5,      10,     24,     34,     44,     36,
        -4,     3,      14,     24,     23,     32,     26,     44,
        -9,     -14,    11,     5,      11,     18,     19,     18,
        -8,     -2,     -23,    -22,    -16,    -16,    -28,    6,
        -12,    -15,    -21,    -9,     -16,    12,     17,     0,
];

internal static readonly short[] MiddleGameKingTable =
[
        18,     49,     27,     -72,    10,     -59,    35,     39,
        -9,     -10,    -21,    -61,    -73,    -51,    -3,     17,
        -71,    -50,    -88,    -91,    -102,   -111,   -73,    -91,
        -89,    -78,    -95,    -129,   -124,   -120,   -122,   -164,
        -64,    -51,    -84,    -105,   -120,   -106,   -127,   -153,
        -76,    -22,    -76,    -86,    -77,    -89,    -63,    -83,
        69,     -3,     -23,    -53,    -55,    -37,    12,     25,
        31,     74,     40,     -53,    23,     -47,    51,     55,
];

internal static readonly short[] EndGameKingTable =
[
        -86,    -55,    -25,    3,      -36,    -9,     -49,    -107,
        -30,    14,     26,     39,     47,     33,     7,      -42,
        -4,     39,     64,     75,     79,     71,     43,     6,
        1,      52,     81,     102,    101,    87,     65,     26,
        -7,     43,     79,     98,     103,    86,     68,     24,
        -1,     35,     62,     75,     73,     66,     41,     2,
        -53,    10,     28,     38,     40,     29,     2,      -45,
        -95,    -67,    -31,    -5,     -31,    -12,    -52,    -113,
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
