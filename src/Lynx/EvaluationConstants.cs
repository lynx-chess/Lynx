﻿using Lynx.Model;

#pragma warning disable IDE1006 // Naming Styles

namespace Lynx;

public static class EvaluationConstants
{
    /// <summary>
    /// 18110 games, 20+0.2, 8moves_v3.epd
    /// Retained (W,D,L) = (443253, 1225563, 445851) positions.
    /// </summary>
    public const int EvalNormalizationCoefficient = 90;

    public static readonly double[] As = [-8.04766150, 80.50520447, -50.49187611, 68.23641282];

    public static readonly double[] Bs = [-2.50118978, 19.26481735, -6.98361835, 49.93026650];

#pragma warning disable IDE0055 // Discard formatting in this region

    public const int PSQTBucketCount = 2;

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

internal static readonly short[][] MiddleGamePieceValues =
[
        [
                +97, +332, +350, +451, +999, 0,
                -97, -332, -350, -451, -999, 0
        ],
        [
                +103, +367, +382, +517, +1150, 0,
                -103, -367, -382, -517, -1150, 0
        ],
];

internal static readonly short[][] EndGamePieceValues =
[
        [
                +128, +405, +348, +708, +1326, 0,
                -128, -405, -348, -708, -1326, 0
        ],
        [
                +121, +436, +375, +773, +1463, 0,
                -121, -436, -375, -773, -1463, 0
        ],
];

internal static readonly short[][] MiddleGamePawnTable =
[
        [
                   0,      0,      0,      0,      0,      0,      0,      0,
                  17,     50,     39,     14,    -21,    -19,    -47,    -66,
                  18,     21,     29,     12,      9,     -1,    -38,    -54,
                   8,     16,     26,     28,     22,      7,    -27,    -49,
                  21,     26,     28,     34,     20,      2,    -26,    -52,
                  37,     37,     29,      6,     -1,    -10,    -45,    -74,
                  27,     66,     28,     10,    -12,    -19,    -55,    -71,
                   0,      0,      0,      0,      0,      0,      0,      0,
        ],
        [
                   0,      0,      0,      0,      0,      0,      0,      0,
                 -31,    -32,    -23,    -14,     -5,     32,     46,      7,
                 -34,    -35,    -14,      3,     12,     25,     33,     19,
                 -30,    -21,     -2,     12,     23,     30,     12,      7,
                 -30,    -18,     -4,     13,     25,     29,     12,      8,
                 -34,    -30,    -13,     -4,      7,     23,     27,     15,
                 -31,    -31,    -27,    -17,     -8,     26,     38,      1,
                   0,      0,      0,      0,      0,      0,      0,      0,
        ],
];

internal static readonly short[][] EndGamePawnTable =
[
        [
                   0,      0,      0,      0,      0,      0,      0,      0,
                   3,    -11,    -11,    -15,      1,      1,     10,     28,
                   3,      1,     -9,    -21,    -15,    -13,     -1,     11,
                  25,     16,     -4,    -22,    -12,    -10,     10,     16,
                  23,     12,     -6,    -18,    -12,     -7,      7,     15,
                   5,    -10,    -12,    -15,    -10,    -10,      1,     10,
                   4,    -14,     -3,      3,      5,      3,     16,     30,
                   0,      0,      0,      0,      0,      0,      0,      0,
        ],
        [
                   0,      0,      0,      0,      0,      0,      0,      0,
                  19,     14,      5,     -8,     12,      4,     -9,    -12,
                  12,      7,     -1,    -10,     -5,     -6,    -12,    -13,
                  27,     16,     -1,    -18,    -15,    -14,      3,     -2,
                  24,     16,     -2,    -15,    -14,    -12,      2,     -5,
                  12,      6,     -4,    -10,     -3,     -5,    -11,    -14,
                  22,     15,      9,    -10,     19,      7,     -7,    -10,
                   0,      0,      0,      0,      0,      0,      0,      0,
        ],
];

internal static readonly short[][] MiddleGameKnightTable =
[
        [
                -106,    -47,    -40,    -12,    -43,    -35,    -28,    -93,
                 -46,    -56,     10,     20,     14,     44,     -5,    -28,
                 -37,     15,     44,     67,     59,     22,     13,    -10,
                  12,     37,     54,     81,     75,     56,     55,      9,
                   1,     33,     59,     52,     66,     49,     46,     26,
                 -20,      1,     38,     62,     58,     15,      9,    -26,
                 -24,    -14,     19,     11,     12,     12,    -19,    -32,
                -156,    -60,    -51,    -32,    -28,    -44,    -32,   -127,
        ],
        [
                -148,    -18,    -52,    -30,     -9,    -10,     -2,    -76,
                 -42,    -26,     -7,     14,     18,     28,     -5,      1,
                 -34,     -5,      9,     47,     51,     33,     27,     -6,
                 -15,     19,     35,     47,     48,     51,     41,     14,
                 -11,     19,     36,     36,     47,     50,     41,     12,
                 -34,     -4,      8,     40,     47,     26,     20,     -6,
                 -47,    -20,     -4,     14,     17,     25,     -1,      2,
                -143,    -21,    -46,    -19,     -6,     -1,    -10,    -64,
        ],
];

internal static readonly short[][] EndGameKnightTable =
[
        [
                 -66,      5,      2,    -16,      5,    -30,    -10,    -64,
                 -12,     20,      5,      1,     -1,     -6,     -3,    -23,
                   7,     -0,      7,     19,     21,     12,     -3,    -12,
                   1,      7,     30,     24,     31,     36,      3,    -10,
                   8,     15,     29,     32,     37,     25,      3,    -15,
                  -2,     18,      0,     19,     19,     11,     -6,      3,
                 -15,      7,     -4,     12,     -2,     -9,      2,    -10,
                 -54,    -13,      9,     -1,     -4,     -8,    -11,    -76,
        ],
        [
                 -51,    -43,     -8,     -0,     -3,    -23,    -39,    -82,
                  -8,      4,     10,      5,      5,     -3,     -8,    -13,
                 -13,      8,     26,     27,     21,      6,      1,     -8,
                  11,     12,     38,     40,     44,     33,     18,     -1,
                   6,     15,     37,     43,     43,     34,     24,      8,
                 -14,      8,     16,     33,     22,      7,     -2,     -7,
                 -15,      8,      2,      7,      1,     -6,    -10,    -20,
                 -66,    -34,     -3,     -6,     -5,    -26,    -36,    -71,
        ],
];

internal static readonly short[][] MiddleGameBishopTable =
[
        [
                 -20,    -23,     -9,    -15,    -19,     -7,    -17,    -12,
                 -14,     45,     20,     -1,      1,      7,     -2,    -11,
                  -3,      7,     22,     27,      4,     -3,     -1,      6,
                  -0,      4,     18,     39,     37,    -15,     -2,     -7,
                 -18,     17,      6,     34,     16,     -6,      2,     -2,
                  -9,     18,     25,     12,      3,     -5,    -15,      3,
                 -20,     59,     36,     15,    -10,     -5,    -19,    -24,
                  -7,     -9,      1,    -37,    -18,     -8,    -14,    -74,
        ],
        [
                 -13,     14,     -9,    -19,    -12,     -8,    -16,     14,
                   5,     -1,      3,    -19,      1,      4,     38,     -6,
                 -10,     -0,    -12,      2,    -14,      7,     -2,     27,
                 -11,    -10,     -5,     22,     19,    -13,      2,     -1,
                 -18,     -5,    -14,     18,      9,    -10,     -7,      5,
                   1,     -5,     -0,     -7,     -0,      0,      2,     21,
                   8,      9,      5,     -8,     -4,      9,     31,      9,
                  10,     17,      3,    -34,    -16,    -17,      8,      4,
        ],
];

internal static readonly short[][] EndGameBishopTable =
[
        [
                  14,     12,      4,     -0,      7,      5,      3,      7,
                  19,    -16,     -9,     -9,     -6,    -13,     -1,     -9,
                  19,      6,    -12,    -14,    -10,      2,     -5,      8,
                  17,     -6,     -9,     -8,    -14,      4,      1,     14,
                  13,     -2,    -11,     -9,     -7,     -6,     -4,     10,
                  31,    -16,     -7,    -17,     -5,     -1,     -3,     -8,
                   4,    -19,    -21,    -16,     -5,     -8,      4,     14,
                  13,     13,     11,     16,      5,     -2,      6,     28,
        ],
        [
                  -2,     19,     -4,      6,     -1,      3,     -1,    -26,
                  -2,     -7,     -6,      1,     -1,    -14,     -5,    -10,
                  12,      8,      7,     -1,     10,      1,      3,     13,
                  12,      1,      5,      2,      2,      2,     -1,      2,
                   6,      2,      4,      7,     -2,      6,      1,      6,
                   7,      1,     -1,     -2,      2,     -5,      0,     14,
                  -8,     -9,    -16,      1,     -3,     -7,     -6,     -8,
                   2,    -14,     -3,      5,     11,      6,     -6,    -16,
        ],
];

internal static readonly short[][] MiddleGameRookTable =
[
        [
                 -57,    -31,    -20,      9,      4,      9,     22,      1,
                  -7,      6,     15,     -6,      0,     10,      3,    -18,
                  -3,     -8,    -33,      6,    -12,    -21,     18,     -7,
                 -16,     17,     -1,     -0,     -2,      6,     15,      3,
                  -0,     12,      9,      6,    -10,     -8,     -9,     -0,
                   5,      8,     40,      3,     -9,     18,     18,     -5,
                  11,     16,      2,      5,     -2,     -1,      8,    -15,
                 -36,    -28,     -3,     20,     11,      7,     23,      2,
        ],
        [
                  -5,    -12,     -7,     -0,     10,      8,      2,      2,
                 -33,    -21,    -14,     -8,      4,     15,     38,      4,
                 -37,    -25,    -24,    -14,      4,     12,     51,     31,
                 -33,    -31,    -25,    -12,     -7,      7,     37,     15,
                 -27,    -25,    -20,     -9,     -8,      9,     35,     15,
                 -31,    -22,    -25,     -3,      1,     21,     52,     30,
                 -30,    -30,     -8,     -3,      6,     13,     44,     19,
                  -4,     -5,     -3,      9,     18,     14,     11,     21,
        ],
];

internal static readonly short[][] EndGameRookTable =
[
        [
                  26,     22,     26,     -1,     -6,    -17,    -21,      3,
                   5,     18,     15,      9,     -5,     -9,     -3,      0,
                   6,     12,     18,     -8,     -2,     -4,    -18,    -10,
                   9,      2,      7,      4,     -8,    -11,    -17,    -13,
                  13,      7,      2,     -2,      4,     -9,    -10,     -6,
                   4,      8,    -17,     -6,     -9,     -7,    -19,     -7,
                   9,     13,     13,      0,      1,     -5,     -8,      6,
                  14,     23,     12,     -5,     -4,    -15,    -23,     -4,
        ],
        [
                   8,      3,      7,     -0,     -7,      2,      8,    -15,
                  20,     19,     19,      8,     -1,     -4,    -10,      1,
                  15,      9,     12,      6,     -9,    -12,    -23,    -19,
                  17,     12,     13,      2,     -2,     -3,    -15,    -14,
                  14,     10,     13,      2,     -4,     -9,    -14,    -14,
                  15,     13,      6,     -4,    -11,    -18,    -24,    -11,
                  21,     22,     15,      5,     -4,     -6,    -10,     -5,
                   4,     -2,      4,     -7,    -17,     -7,      0,    -24,
        ],
];

internal static readonly short[][] MiddleGameQueenTable =
[
        [
                 -34,    -60,    -48,    -14,     -4,     -1,      5,     23,
                 -11,    -20,     19,     15,      6,     11,     29,     46,
                 -42,     -0,     -8,      5,     -5,      1,     31,     70,
                  -4,    -32,     -7,     13,      7,      4,     20,     39,
                  -6,     -7,     -9,     13,      9,     10,     16,     25,
                 -26,    -10,    -10,     -2,     10,     -2,     29,     46,
                 -49,    -48,     18,     25,     19,     -3,     22,     49,
                 -54,    -53,    -30,    -21,      1,    -12,    -14,     12,
        ],
        [
                 -14,    -11,    -11,      7,      1,    -31,      0,     -8,
                  -5,     -9,      8,     -1,      5,     14,     28,     45,
                  -9,     -7,     -8,    -10,    -13,      8,     31,     47,
                 -12,    -17,    -16,     -8,     -7,     -2,     12,     23,
                 -13,    -15,    -15,    -17,     -6,     -2,     12,     21,
                  -8,     -5,    -14,    -10,     -9,      4,     17,     29,
                 -18,    -17,      3,     11,      9,     13,     12,     28,
                 -15,    -11,     -1,      8,      4,    -35,    -18,     25,
        ],
];

internal static readonly short[][] EndGameQueenTable =
[
        [
                 -40,     -3,     34,     -2,    -13,    -48,    -72,    -56,
                 -10,     -7,    -27,      2,     -4,    -23,    -36,    -16,
                  36,      6,     33,      3,     27,     10,    -41,    -33,
                  -2,     35,     17,     15,     20,     14,      4,     -8,
                   1,     20,     32,     21,     17,     -5,     -9,     14,
                  15,     18,     32,     16,     16,     35,     -8,     15,
                  14,     18,    -18,     -3,      4,     25,    -37,    -40,
                 -37,    -16,     18,     24,      6,     -1,     15,    -15,
        ],
        [
                 -15,    -16,     -7,    -14,    -18,     -8,    -10,     37,
                 -15,    -10,    -27,     -5,     -6,    -20,    -46,      7,
                 -22,     -9,     -0,     -1,     18,     17,     -2,     16,
                 -16,     -2,      0,      5,     20,     33,     46,     35,
                  -7,     -6,      4,     16,     16,     31,     23,     43,
                 -20,    -16,      7,      6,     10,     13,     19,     21,
                  -9,     -5,    -23,    -25,    -20,    -26,    -30,     26,
                  -2,    -12,    -13,     -9,    -16,     11,     19,      7,
        ],
];

internal static readonly short[][] MiddleGameKingTable =
[
        [
                 330,    351,    334,    276,      0,      0,      0,      0,
                 309,    321,    313,    292,      0,      0,      0,      0,
                 252,    273,    238,    236,      0,      0,      0,      0,
                 233,    251,    231,    180,      0,      0,      0,      0,
                 255,    277,    239,    199,      0,      0,      0,      0,
                 240,    274,    230,    227,      0,      0,      0,      0,
                 347,    313,    286,    280,      0,      0,      0,      0,
                 317,    345,    322,    266,      0,      0,      0,      0,
        ],
        [
                   0,      0,      0,      0,   -108,   -171,    -78,    -90,
                   0,      0,      0,      0,   -176,   -153,   -107,   -101,
                   0,      0,      0,      0,   -215,   -215,   -174,   -199,
                   0,      0,      0,      0,   -260,   -227,   -220,   -256,
                   0,      0,      0,      0,   -256,   -215,   -221,   -251,
                   0,      0,      0,      0,   -193,   -194,   -167,   -190,
                   0,      0,      0,      0,   -163,   -142,    -91,    -93,
                   0,      0,      0,      0,   -101,   -163,    -64,    -76,
        ],
];

internal static readonly short[][] EndGameKingTable =
[
        [
                 -98,    -68,    -47,    -30,      0,      0,      0,      0,
                 -46,    -11,     -1,      8,      0,      0,      0,      0,
                 -26,     15,     44,     55,      0,      0,      0,      0,
                 -23,     29,     65,     97,      0,      0,      0,      0,
                 -31,     20,     63,     94,      0,      0,      0,      0,
                 -22,     15,     46,     58,      0,      0,      0,      0,
                 -62,    -13,      6,     10,      0,      0,      0,      0,
                -102,    -74,    -49,    -30,      0,      0,      0,      0,
        ],
        [
                   0,      0,      0,      0,    -26,      1,    -32,    -79,
                   0,      0,      0,      0,     52,     39,     15,    -22,
                   0,      0,      0,      0,     92,     80,     44,     15,
                   0,      0,      0,      0,    128,     98,     67,     27,
                   0,      0,      0,      0,    130,     97,     69,     26,
                   0,      0,      0,      0,     88,     75,     43,     11,
                   0,      0,      0,      0,     48,     36,     10,    -25,
                   0,      0,      0,      0,    -21,     -2,    -35,    -83,
        ],
];

#pragma warning restore IDE0055

    /// <summary>
    /// 2x12x64
    /// </summary>
    public static readonly int[][][] PackedPSQT = new int[2][][];

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
        short[][][] mgPositionalTables =
        [
            MiddleGamePawnTable,
            MiddleGameKnightTable,
            MiddleGameBishopTable,
            MiddleGameRookTable,
            MiddleGameQueenTable,
            MiddleGameKingTable
        ];

        short[][][] egPositionalTables =
        [
            EndGamePawnTable,
            EndGameKnightTable,
            EndGameBishopTable,
            EndGameRookTable,
            EndGameQueenTable,
            EndGameKingTable
        ];

        for (int bucket = 0; bucket < PSQTBucketCount; ++bucket)
        {
            PackedPSQT[bucket] = new int[12][];
            for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
            {
                PackedPSQT[bucket][piece] = new int[64];
                PackedPSQT[bucket][piece + 6] = new int[64];

                for (int sq = 0; sq < 64; ++sq)
                {
                    PackedPSQT[bucket][piece][sq] = Utils.Pack(
                        (short)(MiddleGamePieceValues[bucket][piece] + mgPositionalTables[piece][bucket][sq]),
                        (short)(EndGamePieceValues[bucket][piece] + egPositionalTables[piece][bucket][sq]));

                    PackedPSQT[bucket][piece + 6][sq] = Utils.Pack(
                        (short)(MiddleGamePieceValues[bucket][piece + 6] - mgPositionalTables[piece][bucket][sq ^ 56]),
                        (short)(EndGamePieceValues[bucket][piece + 6] - egPositionalTables[piece][bucket][sq ^ 56]));
                }
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

    public const int CounterMoveValue = 65_536;

    // Revisit bad capture pruning in NegaMax.cs if order changes and promos aren't the lowest before bad captures
    public const int PromotionMoveScoreValue = 32_768;

    public const int BadCaptureMoveBaseScoreValue = 16_384;

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

    public const int ContinuationHistoryPlyCount = 1;
}

#pragma warning restore IDE1006 // Naming Styles
