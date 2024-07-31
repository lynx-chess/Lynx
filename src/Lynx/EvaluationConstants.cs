using Lynx.Model;

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
                        +99, +333, +351, +441, +1001, 0,
                        -99, -333, -351, -441, -1001, 0
                ],
                [
                        +102, +367, +382, +518, +1151, 0,
                        -102, -367, -382, -518, -1151, 0
                ],
        ];

    internal static readonly short[][] EndGamePieceValues =
    [
            [
                        +127, +404, +348, +715, +1328, 0,
                        -127, -404, -348, -715, -1328, 0
                ],
                [
                        +121, +436, +376, +772, +1462, 0,
                        -121, -436, -376, -772, -1462, 0
                ],
        ];

    internal static readonly short[][] MiddleGamePawnTable =
    [
            [
                           0,      0,      0,      0,      0,      0,      0,      0,
                          14,     46,     35,     15,    -23,    -20,    -48,    -63,
                          16,     21,     29,     16,      7,     -3,    -39,    -51,
                           8,     17,     27,     34,     21,      6,    -28,    -46,
                          20,     26,     30,     40,     19,      1,    -27,    -49,
                          36,     38,     29,     11,     -2,    -11,    -47,    -71,
                          24,     63,     24,     12,    -14,    -20,    -56,    -68,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,      0,      0,      0,      0,
                         -31,    -31,    -22,    -13,     -5,     32,     46,      7,
                         -34,    -34,    -14,      3,     12,     24,     33,     19,
                         -30,    -21,     -2,     12,     23,     30,     11,      6,
                         -30,    -18,     -4,     13,     25,     29,     11,      8,
                         -34,    -30,    -12,     -4,      7,     23,     26,     15,
                         -31,    -30,    -27,    -17,     -8,     26,     38,      1,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
        ];

    internal static readonly short[][] EndGamePawnTable =
    [
            [
                           0,      0,      0,      0,      0,      0,      0,      0,
                           3,    -10,     -9,    -15,      1,      1,     10,     27,
                           3,      1,     -8,    -22,    -15,    -12,     -1,     10,
                          26,     16,     -3,    -22,    -12,    -10,     10,     15,
                          23,     13,     -6,    -18,    -12,     -7,      7,     14,
                           6,    -10,    -11,    -16,    -10,    -10,      1,      9,
                           4,    -13,     -2,      2,      5,      3,     16,     29,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,      0,      0,      0,      0,
                          19,     14,      5,     -8,     11,      3,     -9,    -12,
                          13,      7,     -1,    -10,     -6,     -6,    -12,    -14,
                          28,     16,     -1,    -18,    -16,    -14,      3,     -3,
                          24,     16,     -2,    -14,    -14,    -12,      1,     -5,
                          12,      6,     -4,     -9,     -4,     -6,    -12,    -14,
                          22,     15,      9,     -9,     19,      7,     -7,    -10,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
        ];

    internal static readonly short[][] MiddleGameKnightTable =
    [
            [
                        -106,    -47,    -40,    -13,    -42,    -34,    -28,    -91,
                         -46,    -55,     10,     21,     14,     43,     -6,    -29,
                         -37,     15,     44,     67,     58,     22,     13,    -10,
                          12,     37,     54,     81,     75,     56,     55,      7,
                           1,     33,     59,     52,     65,     48,     46,     25,
                         -20,      1,     37,     62,     58,     16,      8,    -26,
                         -23,    -14,     19,     11,     12,     12,    -19,    -33,
                        -155,    -60,    -51,    -33,    -29,    -44,    -31,   -127,
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
                         -66,      6,      2,    -15,      5,    -31,    -10,    -65,
                         -12,     20,      5,      1,     -1,     -6,     -3,    -22,
                           7,     -0,      7,     19,     21,     12,     -3,    -12,
                           1,      7,     30,     24,     31,     36,      3,    -10,
                           8,     15,     29,     32,     37,     25,      3,    -15,
                          -2,     18,     -0,     19,     19,     11,     -6,      3,
                         -15,      8,     -4,     12,     -3,     -9,      2,    -10,
                         -54,    -13,      9,     -1,     -4,     -8,    -11,    -76,
                ],
                [
                         -51,    -43,     -8,     -0,     -3,    -23,    -39,    -82,
                          -8,      4,     10,      5,      5,     -3,     -8,    -13,
                         -13,      8,     26,     27,     21,      6,      1,     -8,
                          11,     12,     38,     40,     44,     33,     18,     -1,
                           6,     15,     37,     43,     43,     34,     24,      8,
                         -14,      8,     16,     33,     22,      8,     -1,     -7,
                         -16,      8,      2,      7,      1,     -6,    -10,    -20,
                         -66,    -35,     -4,     -6,     -5,    -26,    -36,    -71,
                ],
        ];

    internal static readonly short[][] MiddleGameBishopTable =
    [
            [
                         -19,    -23,     -8,    -16,    -19,     -6,    -16,    -11,
                         -15,     46,     20,     -2,      1,      7,     -2,    -11,
                          -3,      7,     22,     26,      4,     -4,     -1,      5,
                          -0,      5,     18,     39,     36,    -16,     -2,     -7,
                         -19,     18,      5,     34,     16,     -6,      2,     -2,
                          -9,     18,     25,     12,      3,     -5,    -15,      2,
                         -21,     60,     36,     16,    -10,     -5,    -19,    -25,
                          -6,    -10,      2,    -37,    -18,     -7,    -15,    -73,
                ],
                [
                         -13,     14,     -9,    -19,    -12,     -8,    -16,     14,
                           6,     -1,      3,    -19,      1,      4,     38,     -6,
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
                          14,     12,      4,      0,      7,      5,      3,      6,
                          19,    -17,     -9,     -9,     -6,    -13,     -1,     -9,
                          19,      6,    -11,    -14,    -10,      2,     -5,      8,
                          17,     -5,     -8,     -8,    -14,      4,      1,     14,
                          13,     -2,    -11,     -9,     -7,     -6,     -5,     10,
                          31,    -15,     -7,    -17,     -5,     -1,     -3,     -8,
                           4,    -20,    -21,    -16,     -5,     -8,      4,     14,
                          13,     13,     11,     16,      5,     -2,      6,     27,
                ],
                [
                          -2,     20,     -4,      6,     -1,      3,     -1,    -26,
                          -2,     -7,     -6,      1,     -1,    -14,     -5,    -10,
                          12,      8,      7,     -1,     10,      1,      3,     13,
                          12,      1,      5,      3,      2,      2,     -1,      2,
                           6,      2,      4,      7,     -2,      6,      1,      6,
                           6,      1,     -1,     -2,      2,     -5,      0,     14,
                          -8,     -9,    -16,      1,     -3,     -7,     -6,     -8,
                           2,    -14,     -3,      5,     11,      6,     -6,    -16,
                ],
        ];

    internal static readonly short[][] MiddleGameRookTable =
    [
            [
                         -48,    -29,    -20,     11,      3,     10,     23,      7,
                          -5,      7,     14,     -7,     -2,      8,      1,    -18,
                          -1,     -8,    -34,      2,    -17,    -23,     17,     -6,
                         -14,     20,     -3,     -3,     -8,      4,     14,      3,
                           1,     13,      8,      4,    -15,     -8,    -11,     -0,
                           5,      8,     40,      1,    -12,     18,     19,     -3,
                          12,     16,     -0,      4,     -5,     -2,      7,    -15,
                         -28,    -27,     -2,     21,      9,      9,     25,      8,
                ],
                [
                          -5,    -12,     -6,     -0,     10,      7,      1,      1,
                         -33,    -20,    -13,     -7,      4,     14,     38,      4,
                         -37,    -24,    -24,    -14,      4,     12,     51,     30,
                         -33,    -31,    -25,    -11,     -6,      7,     36,     14,
                         -27,    -25,    -19,     -8,     -7,      9,     34,     15,
                         -30,    -22,    -24,     -2,      1,     21,     52,     29,
                         -30,    -30,     -8,     -3,      6,     13,     44,     19,
                          -4,     -5,     -2,     10,     18,     13,     11,     20,
                ],
        ];

    internal static readonly short[][] EndGameRookTable =
    [
            [
                          22,     20,     26,     -2,     -5,    -18,    -22,     -1,
                           4,     17,     16,     10,     -3,     -9,     -2,     -0,
                           5,     12,     19,     -5,      1,     -4,    -18,    -11,
                           8,      1,      8,      7,     -5,    -11,    -18,    -14,
                          12,      7,      4,      1,      6,     -9,    -10,     -7,
                           4,      8,    -16,     -4,     -7,     -7,    -20,     -8,
                           8,     13,     15,      2,      2,     -5,     -8,      6,
                          10,     22,     12,     -7,     -4,    -17,    -24,     -8,
                ],
                [
                           9,      4,      7,     -0,     -7,      2,      8,    -14,
                          20,     19,     18,      7,     -1,     -4,    -10,      2,
                          15,      9,     11,      5,    -10,    -11,    -22,    -18,
                          17,     11,     12,      2,     -3,     -3,    -14,    -14,
                          14,      9,     13,      1,     -4,     -9,    -14,    -14,
                          15,     13,      6,     -4,    -11,    -18,    -24,    -10,
                          21,     22,     14,      4,     -4,     -6,     -9,     -4,
                           5,     -2,      4,     -7,    -17,     -7,      0,    -23,
                ],
        ];

    internal static readonly short[][] MiddleGameQueenTable =
    [
            [
                         -33,    -60,    -48,    -15,     -3,     -1,      4,     22,
                         -12,    -20,     19,     16,      7,     11,     28,     44,
                         -41,     -0,     -8,      5,     -5,      1,     31,     70,
                          -4,    -32,     -9,     13,      7,      4,     20,     39,
                          -5,     -7,     -9,     14,      8,     10,     16,     24,
                         -25,     -9,    -10,     -1,     11,     -2,     29,     45,
                         -49,    -47,     19,     26,     20,     -3,     21,     47,
                         -54,    -54,    -30,    -21,      2,    -12,    -14,     10,
                ],
                [
                         -14,    -11,    -10,      7,      1,    -31,      0,     -8,
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
                         -39,     -3,     34,     -1,    -13,    -49,    -72,    -54,
                          -9,     -7,    -28,      0,     -5,    -23,    -35,    -15,
                          36,      6,     33,      4,     27,      9,    -42,    -33,
                          -2,     36,     18,     16,     21,     14,      4,     -8,
                           0,     20,     33,     22,     17,     -6,    -10,     14,
                          14,     18,     32,     15,     15,     33,     -9,     15,
                          14,     18,    -19,     -6,      3,     24,    -38,    -39,
                         -36,    -14,     18,     25,      6,     -0,     15,    -13,
                ],
                [
                         -14,    -16,     -7,    -14,    -18,     -8,    -10,     36,
                         -15,    -10,    -26,     -4,     -6,    -20,    -46,      7,
                         -22,     -9,     -0,     -1,     18,     17,     -2,     16,
                         -16,     -2,      0,      6,     20,     33,     46,     35,
                          -7,     -5,      4,     17,     16,     31,     23,     43,
                         -20,    -16,      7,      7,     10,     13,     19,     21,
                         -10,     -5,    -22,    -25,    -20,    -25,    -30,     26,
                          -2,    -12,    -13,     -8,    -16,     11,     19,      7,
                ],
        ];

    internal static readonly short[][] MiddleGameKingTable =
    [
            [
                         311,    331,    317,    260,      0,      0,      0,      0,
                         288,    297,    291,    271,      0,      0,      0,      0,
                         233,    251,    215,    214,      0,      0,      0,      0,
                         218,    233,    211,    162,      0,      0,      0,      0,
                         240,    260,    221,    179,      0,      0,      0,      0,
                         221,    252,    208,    205,      0,      0,      0,      0,
                         328,    290,    266,    260,      0,      0,      0,      0,
                         300,    325,    306,    252,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,   -104,   -166,    -74,    -85,
                           0,      0,      0,      0,   -172,   -148,   -102,    -96,
                           0,      0,      0,      0,   -211,   -210,   -169,   -195,
                           0,      0,      0,      0,   -259,   -224,   -216,   -253,
                           0,      0,      0,      0,   -255,   -213,   -218,   -248,
                           0,      0,      0,      0,   -189,   -189,   -162,   -186,
                           0,      0,      0,      0,   -159,   -137,    -87,    -89,
                           0,      0,      0,      0,    -97,   -158,    -60,    -72,
                ],
        ];

    internal static readonly short[][] EndGameKingTable =
    [
            [
                         -94,    -63,    -43,    -27,      0,      0,      0,      0,
                         -42,     -7,      3,     12,      0,      0,      0,      0,
                         -22,     19,     48,     59,      0,      0,      0,      0,
                         -20,     32,     69,    101,      0,      0,      0,      0,
                         -27,     24,     68,     98,      0,      0,      0,      0,
                         -18,     20,     51,     62,      0,      0,      0,      0,
                         -57,     -8,     10,     15,      0,      0,      0,      0,
                         -98,    -69,    -45,    -26,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,    -27,     -0,    -34,    -81,
                           0,      0,      0,      0,     51,     38,     13,    -24,
                           0,      0,      0,      0,     91,     78,     43,     14,
                           0,      0,      0,      0,    127,     98,     66,     26,
                           0,      0,      0,      0,    129,     97,     68,     25,
                           0,      0,      0,      0,     87,     74,     42,     10,
                           0,      0,      0,      0,     46,     35,      9,    -26,
                           0,      0,      0,      0,    -23,     -3,    -37,    -85,
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
