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
        +103, +392, +362, +487, +1114, 0,
        -103, -392, -362, -487, -1114, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +133, +443, +398, +775, +1425, 0,
        -133, -443, -398, -775, -1425, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -25,    -22,    -14,    -9,     -2,     29,     31,     -14,
        -26,    -24,    -4,     10,     19,     29,     26,     7,
        -25,    -15,    3,      18,     26,     29,     3,      -5,
        -25,    -11,    1,      20,     27,     27,     1,      -6,
        -23,    -17,    -3,     4,      14,     25,     19,     2,
        -26,    -20,    -18,    -13,    -4,     23,     20,     -22,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        12,     10,     6,      -13,    5,      1,      -6,     -9,
        10,     10,     1,      -13,    -6,     -5,     -5,     -9,
        25,     17,     -1,     -19,    -15,    -14,    4,      -1,
        23,     16,     -2,     -15,    -14,    -10,    2,      -3,
        12,     8,      -2,     -11,    -3,     -4,     -4,     -10,
        14,     10,     9,      -12,    14,     5,      -3,     -6,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -146,   -23,    -51,    -31,    -13,    -21,    -11,    -98,
        -46,    -29,    -4,     15,     16,     23,     -15,    -18,
        -29,    0,      19,     56,     59,     39,     33,     -5,
        -11,    24,     43,     59,     58,     58,     44,     17,
        -8,     24,     45,     46,     57,     57,     44,     16,
        -27,    3,      19,     48,     57,     33,     27,     -6,
        -47,    -20,    0,      14,     16,     18,     -14,    -19,
        -163,   -26,    -49,    -21,    -10,    -12,    -19,    -89,
];

internal static readonly short[] EndGameKnightTable =
[
        -67,    -58,    -12,    -11,    -10,    -27,    -53,    -87,
        -18,    1,      13,     8,      8,      5,      -10,    -20,
        -13,    15,     35,     35,     32,     17,     9,      -13,
        7,      20,     47,     48,     52,     47,     24,     -6,
        4,      24,     47,     51,     52,     42,     28,     0,
        -15,    18,     26,     40,     32,     18,     6,      -9,
        -24,    4,      6,      11,     4,      0,      -11,    -24,
        -72,    -57,    -7,     -14,    -11,    -26,    -50,    -84,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -19,    14,     -3,     -15,    -12,    -16,    -21,    -1,
        7,      2,      7,      -18,    2,      -1,     27,     -13,
        -6,     5,      -5,     3,      -8,     13,     4,      27,
        -7,     -7,     -5,     23,     20,     -17,    2,      -2,
        -15,    -1,     -13,    18,     6,      -13,    -6,     5,
        5,      5,      7,      -5,     5,      6,      7,      22,
        9,      13,     10,     -6,     -3,     3,      19,     -3,
        6,      18,     11,     -30,    -14,    -21,    1,      -16,
];

internal static readonly short[] EndGameBishopTable =
[
        -10,    14,     -13,    3,      -1,     5,      -1,     -27,
        -2,     -7,     -3,     5,      3,      -9,     -3,     -14,
        13,     14,     7,      2,      11,     4,      7,      10,
        13,     8,      7,      -3,     -6,     7,      6,      7,
        9,      11,     6,      1,      -8,     7,      8,      8,
        11,     5,      1,      0,      6,      0,      5,      7,
        -12,    -9,     -14,    3,      2,      -2,     -2,     -9,
        -7,     -13,    -9,     7,      7,      7,      -3,     -13,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -4,     2,      14,     3,      7,      -2,
        -26,    -17,    -13,    -12,    0,      3,      17,     -4,
        -29,    -20,    -22,    -12,    3,      9,      50,     27,
        -24,    -20,    -17,    -7,     -4,     8,      38,     19,
        -19,    -15,    -12,    -4,     -7,     7,      29,     14,
        -22,    -16,    -18,    -4,     2,      19,     48,     27,
        -24,    -26,    -9,     -6,     2,      2,      23,     2,
        -3,     -4,     0,      12,     22,     8,      15,     9,
];

internal static readonly short[] EndGameRookTable =
[
        4,      2,      5,      -3,     -11,    2,      -1,     -4,
        16,     19,     18,     8,      -1,     -2,     -4,     3,
        13,     11,     12,     5,      -7,     -10,    -20,    -16,
        15,     11,     13,     6,      0,      0,      -13,    -13,
        14,     10,     13,     4,      1,      -6,     -10,    -9,
        13,     13,     4,      -3,     -10,    -13,    -20,    -11,
        19,     22,     14,     4,      -4,     -3,     -5,     2,
        -1,     -3,     1,      -9,     -18,    -5,     -8,     -13,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -13,    -10,    -5,     9,      3,      -30,    8,      2,
        -2,     -10,    7,      -3,     2,      6,      22,     50,
        -9,     -6,     -9,     -10,    -12,    7,      34,     55,
        -12,    -19,    -18,    -9,     -9,     -6,     10,     24,
        -12,    -15,    -19,    -18,    -8,     -5,     9,      22,
        -6,     -4,     -15,    -12,    -5,     4,      20,     37,
        -15,    -19,    3,      10,     7,      3,      7,      37,
        -10,    -10,    5,      11,     6,      -36,    -13,    25,
];

internal static readonly short[] EndGameQueenTable =
[
        -24,    -21,    -12,    -12,    -17,    -11,    -34,    8,
        -17,    -10,    -25,    -1,     -2,     -17,    -47,    -8,
        -16,    -4,     6,      1,      22,     19,     -8,     3,
        -10,    8,      9,      13,     26,     38,     44,     31,
        -3,     5,      15,     25,     22,     33,     24,     39,
        -16,    -11,    14,     11,     13,     19,     18,     15,
        -11,    -3,     -21,    -19,    -13,    -13,    -32,    3,
        -16,    -17,    -18,    -7,     -11,    16,     13,     -3,
];

internal static readonly short[] MiddleGameKingTable =
[
        24,     50,     26,     -75,    9,      -61,    39,     48,
        -12,    -16,    -35,    -72,    -84,    -58,    -10,    19,
        -82,    -69,    -107,   -108,   -116,   -125,   -83,    -96,
        -107,   -97,    -116,   -151,   -146,   -138,   -137,   -162,
        -74,    -70,    -104,   -131,   -146,   -122,   -141,   -158,
        -81,    -44,    -97,    -104,   -92,    -104,   -74,    -87,
        73,     -8,     -36,    -63,    -68,    -46,    5,      27,
        38,     75,     38,     -59,    20,     -51,    53,     62,
];

internal static readonly short[] EndGameKingTable =
[
        -71,    -45,    -20,    5,      -33,    -2,     -38,    -89,
        -13,    17,     27,     39,     46,     33,     12,     -23,
        10,     42,     59,     68,     72,     64,     43,     22,
        15,     53,     75,     90,     88,     80,     68,     39,
        5,      45,     72,     86,     91,     78,     70,     39,
        11,     38,     57,     68,     66,     58,     42,     18,
        -38,    13,     28,     37,     39,     29,     8,      -25,
        -82,    -56,    -26,    -2,     -29,    -6,     -41,    -94,
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
