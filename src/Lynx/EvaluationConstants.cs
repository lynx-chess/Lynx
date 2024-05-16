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
        +105, +398, +366, +489, +1113, 0,
        -105, -398, -366, -489, -1113, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +133, +439, +391, +762, +1419, 0,
        -133, -439, -391, -762, -1419, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -27,    -22,    -13,    -6,     0,      30,     33,     -12,
        -30,    -24,    -4,     13,     19,     26,     26,     8,
        -32,    -16,    2,      17,     24,     25,     4,      -7,
        -31,    -12,    0,      19,     26,     23,     2,      -7,
        -27,    -18,    -2,     5,      15,     22,     19,     2,
        -27,    -20,    -17,    -11,    -3,     23,     22,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        11,     11,     6,      -12,    7,      2,      -5,     -11,
        10,     10,     -1,     -14,    -7,     -5,     -5,     -11,
        25,     18,     -1,     -21,    -16,    -11,    6,      -1,
        22,     17,     -2,     -17,    -15,    -8,     4,      -3,
        10,     8,      -4,     -12,    -3,     -4,     -4,     -11,
        13,     12,     9,      -11,    16,     5,      -2,     -7,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -149,   -18,    -46,    -24,    -8,     -20,    -7,     -99,
        -44,    -28,    -4,     16,     17,     27,     -13,    -16,
        -29,    -1,     18,     55,     60,     39,     32,     -5,
        -12,    24,     41,     58,     57,     57,     43,     15,
        -9,     22,     44,     46,     56,     56,     43,     15,
        -27,    1,      18,     48,     57,     32,     25,     -6,
        -43,    -18,    0,      15,     16,     21,     -14,    -18,
        -164,   -20,    -45,    -14,    -4,     -11,    -15,    -89,
];

internal static readonly short[] EndGameKnightTable =
[
        -61,    -59,    -13,    -14,    -12,    -27,    -52,    -81,
        -16,    1,      13,     8,      8,      4,      -13,    -21,
        -13,    14,     35,     35,     32,     16,     9,      -13,
        6,      18,     48,     48,     53,     47,     24,     -5,
        5,      25,     46,     51,     52,     42,     28,     0,
        -15,    17,     25,     39,     31,     18,     6,      -8,
        -25,    3,      6,      11,     3,      -1,     -12,    -26,
        -67,    -57,    -9,     -17,    -14,    -26,    -50,    -80,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -21,    14,     2,      -9,     -6,     -14,    -20,    -1,
        6,      1,      6,      -18,    2,      2,      27,     -7,
        -6,     3,      -6,     2,      -10,    12,     3,      27,
        -7,     -8,     -8,     21,     19,     -20,    0,      -4,
        -14,    -3,     -15,    18,     4,      -16,    -9,     3,
        5,      3,      6,      -6,     4,      6,      5,      22,
        10,     12,     10,     -6,     -3,     5,      18,     -2,
        5,      20,     16,     -25,    -7,     -18,    4,      -17,
];

internal static readonly short[] EndGameBishopTable =
[
        -8,     15,     -11,    3,      -1,     6,      -1,     -25,
        0,      -6,     -3,     6,      2,      -10,    -3,     -16,
        14,     14,     7,      2,      11,     3,      7,      10,
        13,     8,      6,      -4,     -8,     7,      6,      9,
        9,      10,     6,      -1,     -9,     6,      8,      10,
        10,     4,      0,      0,      5,      -2,     4,      7,
        -10,    -8,     -14,    3,      1,      -4,     -2,     -7,
        -6,     -13,    -7,     8,      6,      7,      -3,     -10,
];

internal static readonly short[] MiddleGameRookTable =
[
        -12,    -11,    4,      9,      18,     -3,     6,      -13,
        -26,    -15,    -5,     -5,     5,      4,      20,     -1,
        -27,    -16,    -16,    -4,     9,      7,      45,     26,
        -26,    -17,    -12,    0,      1,      7,      35,     15,
        -18,    -13,    -7,     1,      -2,     6,      25,     9,
        -21,    -13,    -13,    1,      2,      13,     39,     19,
        -22,    -20,    -2,     0,      5,      0,      28,     4,
        -13,    -6,     7,      18,     23,     2,      12,     -1,
];

internal static readonly short[] EndGameRookTable =
[
        1,      2,      5,      -1,     -9,     2,      0,      -6,
        16,     19,     18,     10,     0,      -2,     -5,     0,
        14,     11,     12,     7,      -6,     -8,     -21,    -17,
        16,     11,     13,     6,      1,      0,      -14,    -12,
        15,     10,     12,     5,      2,      -5,     -11,    -8,
        13,     13,     5,      -1,     -7,     -12,    -19,    -10,
        19,     21,     14,     6,      -2,     -3,     -8,     0,
        -3,     -3,     0,      -7,     -15,    -6,     -8,     -15,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -11,    -4,     3,      17,     11,     -27,    12,     -3,
        1,      -12,    7,      -2,     3,      7,      23,     66,
        -8,     -7,     -12,    -12,    -15,    4,      31,     56,
        -14,    -22,    -22,    -13,    -15,    -9,     7,      21,
        -13,    -19,    -23,    -23,    -14,    -10,    5,      19,
        -8,     -5,     -16,    -14,    -8,     1,      17,     37,
        -12,    -23,    3,      11,     8,      7,      6,      48,
        -8,     -4,     14,     19,     16,     -32,    -10,    26,
];

internal static readonly short[] EndGameQueenTable =
[
        -25,    -28,    -19,    -15,    -24,    -10,    -37,    10,
        -16,    -9,     -27,    -2,     -4,     -15,    -42,    -11,
        -16,    -5,     6,      2,      25,     22,     0,      5,
        -8,     9,      8,      14,     29,     41,     46,     37,
        -2,     5,      18,     27,     26,     37,     30,     45,
        -9,     -12,    13,     8,      14,     21,     22,     18,
        -11,    -1,     -21,    -22,    -16,    -16,    -26,    3,
        -17,    -23,    -28,    -10,    -20,    11,     9,      -3,
];

internal static readonly short[] MiddleGameKingTable =
[
        21,     47,     26,     -71,    13,     -59,    37,     43,
        -12,    -21,    -37,    -74,    -87,    -60,    -12,    16,
        -78,    -68,    -107,   -106,   -118,   -130,   -86,    -97,
        -100,   -91,    -110,   -142,   -141,   -136,   -133,   -170,
        -74,    -67,    -99,    -119,   -138,   -123,   -139,   -158,
        -82,    -38,    -93,    -102,   -93,    -105,   -75,    -90,
        67,     -12,    -38,    -65,    -68,    -46,    4,      25,
        32,     74,     40,     -51,    27,     -47,    53,     60,
];

internal static readonly short[] EndGameKingTable =
[
        -69,    -43,    -17,    7,      -31,    -2,     -37,    -87,
        -13,    19,     28,     40,     46,     33,     13,     -22,
        9,      41,     58,     68,     72,     65,     43,     22,
        13,     51,     73,     87,     86,     79,     65,     40,
        5,      43,     71,     83,     89,     78,     69,     38,
        11,     36,     56,     67,     65,     58,     41,     18,
        -36,    14,     29,     37,     39,     29,     8,      -26,
        -79,    -55,    -24,    -2,     -28,    -6,     -41,    -94,
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
