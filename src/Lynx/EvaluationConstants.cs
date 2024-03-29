﻿using Lynx.Model;

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
        +103, +386, +357, +475, +1084, 0,
        -103, -386, -357, -475, -1084, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +149, +485, +434, +843, +1560, 0,
        -149, -485, -434, -843, -1560, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -26,    -23,    -15,    -9,     -3,     29,     31,     -15,
        -27,    -25,    -5,     11,     18,     27,     24,     7,
        -27,    -15,    3,      18,     27,     30,     3,      -6,
        -26,    -11,    1,      19,     27,     27,     2,      -7,
        -24,    -19,    -3,     4,      14,     23,     17,     1,
        -27,    -21,    -20,    -14,    -5,     22,     20,     -22,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        12,     12,     6,      -15,    6,      2,      -3,     -12,
        11,     13,     0,      -14,    -7,     -5,     -2,     -11,
        28,     20,     0,      -22,    -18,    -12,    8,      -1,
        25,     19,     -2,     -17,    -16,    -9,     6,      -3,
        12,     10,     -4,     -13,    -2,     -3,     -2,     -11,
        15,     12,     9,      -14,    16,     6,      -1,     -9,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -153,   -22,    -52,    -32,    -12,    -21,    -9,     -98,
        -46,    -27,    -3,     15,     18,     25,     -14,    -17,
        -28,    1,      20,     57,     61,     42,     35,     -3,
        -11,    25,     43,     60,     60,     61,     46,     18,
        -8,     24,     45,     48,     57,     59,     47,     17,
        -26,    3,      20,     49,     60,     34,     28,     -4,
        -48,    -19,    0,      15,     17,     19,     -11,    -18,
        -168,   -24,    -49,    -21,    -9,     -12,    -17,    -91,
];

internal static readonly short[] EndGameKnightTable =
[
        -72,    -61,    -13,    -13,    -10,    -28,    -55,    -93,
        -22,    -2,     15,     9,      10,     7,      -13,    -21,
        -14,    16,     38,     38,     37,     18,     10,     -15,
        7,      20,     52,     53,     56,     50,     25,     -6,
        4,      27,     51,     55,     58,     47,     31,     0,
        -18,    19,     27,     45,     35,     20,     7,      -11,
        -28,    2,      5,      13,     5,      0,      -15,    -25,
        -78,    -60,    -9,     -17,    -11,    -26,    -53,    -93,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -15,    16,     -2,     -15,    -10,    -16,    -23,    2,
        8,      3,      7,      -18,    1,      -1,     29,     -12,
        -6,     5,      -4,     3,      -8,     14,     5,      28,
        -7,     -7,     -6,     23,     19,     -18,    2,      -2,
        -16,    -2,     -15,    17,     6,      -13,    -6,     5,
        5,      5,      7,      -5,     6,      7,      8,      24,
        10,     15,     12,     -6,     -3,     4,      21,     -2,
        10,     20,     12,     -29,    -13,    -21,    1,      -12,
];

internal static readonly short[] EndGameBishopTable =
[
        -14,    18,     -13,    3,      -3,     6,      1,      -31,
        -5,     -8,     -4,     5,      4,      -10,    -2,     -16,
        14,     16,     8,      3,      12,     4,      7,      10,
        13,     8,      7,      -2,     -6,     6,      4,      5,
        9,      10,     6,      4,      -10,    5,      4,      5,
        12,     4,      0,      1,      5,      -1,     4,      7,
        -15,    -11,    -17,    3,      1,      -3,     -2,     -11,
        -7,     -12,    -7,     7,      8,      9,      -2,     -22,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -5,     2,      15,     3,      10,     0,
        -27,    -17,    -14,    -14,    -2,     3,      21,     0,
        -29,    -19,    -23,    -13,    4,      9,      55,     31,
        -27,    -21,    -18,    -12,    -7,     8,      44,     21,
        -20,    -17,    -12,    -5,     -10,    7,      33,     16,
        -24,    -16,    -19,    -5,     1,      19,     50,     30,
        -25,    -28,    -9,     -7,     -1,     1,      27,     4,
        -2,     -3,     1,      12,     23,     8,      17,     12,
];

internal static readonly short[] EndGameRookTable =
[
        6,      3,      7,      -2,     -11,    4,      0,      -5,
        15,     20,     21,     11,     0,      -2,     -6,     2,
        11,     8,      12,     5,      -9,     -11,    -23,    -20,
        15,     10,     12,     6,      -2,     -2,     -16,    -15,
        14,     9,      12,     2,      -1,     -9,     -13,    -11,
        12,     13,     3,      -4,     -11,    -16,    -22,    -14,
        20,     23,     16,     5,      -3,     -2,     -8,     2,
        1,      -3,     2,      -8,     -18,    -4,     -9,     -15,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -15,    -10,    -5,     9,      3,      -32,    14,     1,
        0,      -10,    7,      -2,     2,      6,      22,     51,
        -8,     -5,     -10,    -9,     -14,    6,      35,     59,
        -12,    -20,    -19,    -9,     -11,    -5,     11,     25,
        -12,    -15,    -20,    -20,    -10,    -6,     9,      22,
        -5,     -3,     -16,    -13,    -8,     3,      20,     39,
        -15,    -21,    3,      9,      7,      3,      6,      37,
        -11,    -11,    5,      11,     6,      -39,    -12,    28,
];

internal static readonly short[] EndGameQueenTable =
[
        -27,    -24,    -10,    -9,     -19,    -11,    -46,    6,
        -22,    -13,    -28,    0,      -2,     -17,    -48,    -5,
        -17,    -7,     4,      -1,     23,     23,     -8,     3,
        -11,    8,      5,      12,     28,     40,     49,     35,
        -2,     2,      14,     26,     26,     34,     27,     48,
        -20,    -14,    15,     13,     19,     22,     23,     18,
        -15,    -6,     -23,    -18,    -12,    -10,    -32,    6,
        -15,    -18,    -17,    -2,     -9,     21,     13,     -9,
];

internal static readonly short[] MiddleGameKingTable =
[
        34,     55,     30,     -72,    12,     -58,    44,     57,
        -3,     -12,    -33,    -74,    -88,    -58,    -7,     23,
        -83,    -71,    -116,   -119,   -133,   -137,   -88,    -100,
        -120,   -119,   -138,   -181,   -172,   -160,   -157,   -190,
        -85,    -87,    -128,   -159,   -174,   -144,   -161,   -182,
        -77,    -42,    -108,   -119,   -103,   -116,   -78,    -92,
        83,     -2,     -34,    -64,    -69,    -46,    9,      32,
        49,     82,     43,     -56,    25,     -47,    58,     70,
];

internal static readonly short[] EndGameKingTable =
[
        -85,    -49,    -22,    4,      -36,    -3,     -41,    -96,
        -21,    16,     30,     43,     51,     36,     14,     -24,
        7,      44,     65,     77,     81,     71,     47,     25,
        17,     60,     85,     102,    98,     90,     75,     47,
        5,      50,     82,     98,     103,    88,     78,     46,
        6,      40,     64,     78,     74,     66,     47,     20,
        -45,    11,     31,     41,     44,     33,     9,      -27,
        -97,    -61,    -29,    -3,     -31,    -7,     -45,    -101,
];

#pragma warning restore IDE0055

    /// <summary>
    /// 12x64
    /// </summary>
    public static readonly int[][] PackedPSQT = new int[12][];

    /// <summary>
    /// <see cref="Constants.AbsoluteMaxDepth"/> x <see cref="Constants.MaxNumberOfPossibleMovesInAPosition"/>
    /// </summary>
    public static readonly int[][] LMRReductions = new int[Configuration.EngineSettings.MaxDepth][];

    /// <summary>
    /// [0, 4, 136, 276, 424, 580, 744, 916, 1096, 1284, 1480, 1684, 1896, 1896, 1896, 1896, ...]
    /// </summary>
    public static readonly int[] HistoryBonus = new int[Configuration.EngineSettings.MaxDepth];

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

        for (int searchDepth = 1; searchDepth < Configuration.EngineSettings.MaxDepth; ++searchDepth)    // Depth > 0 or we'd be in QSearch
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
