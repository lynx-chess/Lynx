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
                        +97, +332, +350, +451, +998, 0,
                        -97, -332, -350, -451, -998, 0
                ],
                [
                        +103, +367, +382, +517, +1150, 0,
                        -103, -367, -382, -517, -1150, 0
                ],
        ];

        internal static readonly short[][] EndGamePieceValues =
        [
                [
                        +127, +404, +348, +707, +1327, 0,
                        -127, -404, -348, -707, -1327, 0
                ],
                [
                        +121, +436, +376, +773, +1464, 0,
                        -121, -436, -376, -773, -1464, 0
                ],
        ];

        internal static readonly short[][] MiddleGamePawnTable =
        [
                [
                           0,      0,      0,      0,      0,      0,      0,      0,
                          16,     49,     38,     14,    -22,    -19,    -47,    -66,
                          18,     21,     29,     12,      9,     -1,    -38,    -54,
                           9,     16,     26,     28,     21,      7,    -27,    -48,
                          21,     25,     29,     34,     20,      2,    -26,    -52,
                          37,     37,     29,      6,      0,     -9,    -45,    -73,
                          26,     66,     27,      9,    -13,    -19,    -55,    -71,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,      0,      0,      0,      0,
                         -31,    -31,    -23,    -13,     -5,     32,     46,      7,
                         -34,    -35,    -14,      3,     12,     25,     33,     19,
                         -30,    -21,     -2,     12,     24,     30,     12,      7,
                         -30,    -18,     -4,     13,     26,     29,     12,      8,
                         -34,    -30,    -13,     -4,      7,     23,     27,     15,
                         -31,    -30,    -27,    -17,     -8,     26,     38,      1,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
        ];

        internal static readonly short[][] EndGamePawnTable =
        [
                [
                           0,      0,      0,      0,      0,      0,      0,      0,
                           3,    -11,    -10,    -15,      1,      1,     10,     27,
                           3,      1,     -8,    -21,    -15,    -13,     -1,     11,
                          26,     17,     -3,    -22,    -12,    -11,      9,     15,
                          23,     13,     -6,    -18,    -11,     -7,      6,     15,
                           6,    -10,    -12,    -15,    -10,    -10,      0,      9,
                           4,    -14,     -3,      3,      5,      3,     16,     29,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,      0,      0,      0,      0,
                          19,     14,      5,     -8,     11,      4,     -9,    -12,
                          12,      6,     -2,    -10,     -6,     -6,    -12,    -13,
                          27,     16,     -1,    -18,    -15,    -13,      4,     -2,
                          24,     16,     -2,    -15,    -14,    -12,      2,     -5,
                          12,      6,     -4,    -10,     -3,     -5,    -11,    -14,
                          22,     14,      9,    -10,     19,      7,     -7,    -10,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
        ];

        internal static readonly short[][] MiddleGameKnightTable =
        [
                [
                        -105,    -48,    -40,    -12,    -44,    -37,    -30,    -93,
                         -47,    -57,     10,     20,     14,     44,     -5,    -28,
                         -38,     15,     43,     68,     58,     22,     14,    -11,
                          12,     37,     54,     81,     75,     56,     55,      8,
                           1,     33,     60,     51,     66,     49,     47,     26,
                         -20,      1,     37,     63,     59,     16,      9,    -26,
                         -24,    -13,     19,     11,     12,     12,    -19,    -32,
                        -155,    -61,    -50,    -32,    -28,    -45,    -33,   -125,
                ],
                [
                        -148,    -18,    -52,    -30,     -9,    -10,     -2,    -76,
                         -42,    -26,     -7,     14,     18,     28,     -5,      1,
                         -34,     -5,      9,     47,     50,     33,     27,     -6,
                         -15,     19,     35,     47,     48,     51,     41,     15,
                         -11,     19,     36,     36,     47,     50,     41,     12,
                         -34,     -4,      8,     40,     47,     26,     20,     -6,
                         -47,    -20,     -4,     14,     17,     25,     -1,      2,
                        -143,    -21,    -46,    -19,     -6,     -1,    -10,    -64,
                ],
        ];

        internal static readonly short[][] EndGameKnightTable =
        [
                [
                         -67,      6,      1,    -16,      5,    -29,     -9,    -64,
                         -12,     20,      5,      1,     -1,     -5,     -2,    -21,
                           7,     -0,      7,     19,     21,     12,     -4,    -12,
                           1,      7,     30,     23,     30,     35,      3,    -10,
                           8,     15,     29,     32,     37,     24,      3,    -15,
                          -2,     18,      0,     19,     19,     11,     -6,      3,
                         -15,      7,     -4,     12,     -2,     -8,      3,     -9,
                         -55,    -12,      9,     -1,     -4,     -7,    -10,    -76,
                ],
                [
                         -51,    -43,     -8,     -0,     -3,    -23,    -38,    -82,
                          -8,      4,     10,      5,      5,     -3,     -8,    -13,
                         -13,      8,     26,     27,     21,      6,      1,     -8,
                          11,     12,     37,     40,     44,     33,     18,     -1,
                           6,     15,     37,     43,     43,     34,     24,      8,
                         -14,      8,     16,     33,     22,      7,     -1,     -7,
                         -15,      8,      2,      7,      1,     -6,    -10,    -20,
                         -66,    -35,     -3,     -6,     -5,    -26,    -36,    -71,
                ],
        ];

        internal static readonly short[][] MiddleGameBishopTable =
        [
                [
                         -20,    -22,    -10,    -14,    -20,     -7,    -18,    -13,
                         -14,     45,     20,     -2,      1,      7,     -2,    -14,
                          -3,      9,     21,     27,      4,     -4,     -1,      6,
                          -0,      5,     18,     39,     37,    -16,     -2,     -6,
                         -17,     17,      5,     35,     16,     -6,      2,     -2,
                          -9,     18,     25,     12,      3,     -6,    -14,      3,
                         -21,     60,     36,     15,    -11,     -5,    -19,    -24,
                          -7,     -8,      1,    -37,    -18,     -9,    -14,    -76,
                ],
                [
                         -13,     14,     -9,    -19,    -12,     -8,    -16,     14,
                           5,     -2,      3,    -19,      1,      4,     38,     -6,
                         -10,     -0,    -12,      2,    -14,      7,     -2,     27,
                         -11,    -10,     -5,     22,     19,    -13,      2,     -1,
                         -18,     -5,    -14,     18,      9,    -10,     -7,      5,
                           1,     -4,     -0,     -6,     -0,      0,      2,     21,
                           8,      9,      5,     -8,     -4,      9,     31,      9,
                          10,     17,      3,    -34,    -16,    -17,      7,      4,
                ],
        ];

        internal static readonly short[][] EndGameBishopTable =
        [
                [
                          14,     12,      4,     -0,      8,      5,      4,      7,
                          18,    -17,    -10,     -9,     -6,    -12,     -1,     -7,
                          19,      6,    -11,    -15,    -10,      2,     -5,      7,
                          17,     -5,     -9,     -8,    -14,      4,      1,     14,
                          13,     -2,    -10,     -9,     -7,     -6,     -5,     10,
                          30,    -15,     -8,    -17,     -5,     -1,     -3,     -8,
                           4,    -20,    -21,    -17,     -5,     -8,      5,     14,
                          13,     12,     11,     16,      4,     -1,      6,     29,
                ],
                [
                          -2,     20,     -4,      6,     -1,      3,     -2,    -26,
                          -2,     -7,     -6,      1,     -1,    -14,     -5,    -10,
                          12,      8,      7,     -1,     10,      1,      3,     13,
                          12,      1,      5,      2,      3,      2,     -1,      2,
                           5,      2,      4,      7,     -2,      6,      1,      6,
                           7,      1,     -1,     -2,      2,     -5,      0,     14,
                          -8,     -9,    -16,      1,     -3,     -7,     -6,     -8,
                           2,    -14,     -3,      5,     11,      6,     -6,    -17,
                ],
        ];

        internal static readonly short[][] MiddleGameRookTable =
        [
                [
                         -58,    -31,    -19,      8,      4,      8,     22,      1,
                          -9,      7,     16,     -6,      1,     10,      3,    -17,
                          -4,     -8,    -33,      6,    -13,    -22,     18,     -6,
                         -16,     16,     -0,     -1,     -3,      5,     16,      3,
                          -1,     12,     10,      6,    -10,     -8,     -8,      0,
                           5,      8,     40,      2,     -9,     17,     18,     -5,
                          11,     16,      2,      5,     -2,     -1,      7,    -15,
                         -35,    -27,     -2,     20,     11,      7,     23,      2,
                ],
                [
                          -5,    -12,     -7,     -0,     10,      8,      2,      1,
                         -33,    -21,    -14,     -8,      4,     15,     39,      5,
                         -37,    -25,    -24,    -14,      4,     12,     51,     31,
                         -33,    -31,    -25,    -12,     -7,      7,     37,     15,
                         -27,    -25,    -20,     -9,     -8,      9,     34,     15,
                         -31,    -22,    -25,     -3,      1,     21,     52,     30,
                         -30,    -30,     -9,     -3,      6,     13,     44,     19,
                          -4,     -5,     -3,      9,     18,     14,     11,     20,
                ],
        ];

        internal static readonly short[][] EndGameRookTable =
        [
                [
                          26,     21,     25,     -1,     -6,    -17,    -20,      2,
                           5,     16,     14,      8,     -6,     -9,     -1,      1,
                           6,     12,     18,     -8,     -2,     -4,    -18,    -10,
                          10,      2,      7,      4,     -8,    -11,    -17,    -14,
                          13,      8,      2,     -2,      3,     -8,    -11,     -7,
                           4,      8,    -17,     -6,     -9,     -6,    -19,     -7,
                           8,     12,     13,      0,      1,     -3,     -6,      7,
                          14,     22,     11,     -5,     -5,    -15,    -22,     -4,
                ],
                [
                           8,      3,      7,     -0,     -7,      2,      7,    -14,
                          21,     19,     19,      8,     -0,     -4,    -11,      1,
                          15,      9,     12,      6,     -9,    -12,    -23,    -19,
                          17,     12,     13,      2,     -2,     -3,    -14,    -14,
                          14,      9,     13,      2,     -4,     -9,    -14,    -14,
                          15,     13,      6,     -4,    -11,    -18,    -24,    -11,
                          21,     22,     15,      5,     -4,     -6,    -10,     -5,
                           4,     -2,      4,     -7,    -17,     -8,     -0,    -23,
                ],
        ];

        internal static readonly short[][] MiddleGameQueenTable =
        [
                [
                         -34,    -60,    -49,    -16,     -5,     -3,      5,     23,
                         -11,    -20,     19,     15,      5,     10,     28,     46,
                         -41,     -0,     -8,      5,     -5,      0,     33,     71,
                          -4,    -31,     -6,     13,      8,      5,     21,     40,
                          -5,     -7,     -8,     13,      9,     10,     17,     26,
                         -25,    -10,     -9,     -3,     11,     -3,     29,     46,
                         -49,    -47,     19,     24,     19,     -3,     21,     49,
                         -55,    -53,    -30,    -21,      0,    -13,    -16,     11,
                ],
                [
                         -14,    -10,    -10,      7,      1,    -31,     -0,     -7,
                          -5,     -9,      8,     -1,      5,     14,     28,     45,
                          -9,     -7,     -8,    -10,    -13,      8,     31,     47,
                         -12,    -17,    -16,     -8,     -7,     -2,     12,     23,
                         -13,    -15,    -15,    -17,     -6,     -2,     12,     21,
                          -8,     -5,    -14,    -10,     -9,      4,     17,     29,
                         -18,    -17,      3,     11,      9,     13,     12,     29,
                         -15,    -11,     -1,      8,      4,    -35,    -17,     26,
                ],
        ];

        internal static readonly short[][] EndGameQueenTable =
        [
                [
                         -39,     -2,     35,     -1,    -12,    -46,    -69,    -55,
                         -11,     -8,    -28,      1,     -4,    -21,    -34,    -15,
                          36,      6,     32,      2,     28,     10,    -43,    -35,
                          -2,     35,     16,     15,     19,     12,      2,    -11,
                          -0,     19,     31,     22,     16,     -5,    -11,     11,
                          14,     19,     30,     16,     16,     36,     -9,     14,
                          12,     16,    -20,     -5,      5,     27,    -35,    -40,
                         -35,    -16,     18,     23,      7,      2,     20,    -13,
                ],
                [
                         -15,    -17,     -7,    -14,    -19,     -8,    -11,     36,
                         -15,    -10,    -27,     -5,     -6,    -20,    -46,      6,
                         -22,     -9,     -0,     -1,     18,     18,     -1,     17,
                         -16,     -2,      0,      6,     21,     34,     46,     35,
                          -7,     -5,      4,     16,     16,     31,     23,     44,
                         -20,    -16,      7,      7,     10,     13,     19,     21,
                          -9,     -5,    -22,    -25,    -20,    -26,    -30,     26,
                          -2,    -12,    -13,     -9,    -16,     11,     18,      6,
                ],
        ];

        internal static readonly short[][] MiddleGameKingTable =
        [
                [
                         135,    139,    130,    102,      0,      0,      0,      0,
                         122,    131,    124,    112,      0,      0,      0,      0,
                          93,    102,     79,     81,      0,      0,      0,      0,
                          72,     86,     80,     44,      0,      0,      0,      0,
                          87,     98,     81,     57,      0,      0,      0,      0,
                          81,    105,     79,     77,      0,      0,      0,      0,
                         155,    132,    111,    107,      0,      0,      0,      0,
                         135,    145,    126,     95,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,    -34,    -60,    -13,    -13,
                           0,      0,      0,      0,    -75,    -63,    -42,    -28,
                           0,      0,      0,      0,   -120,   -120,    -90,    -88,
                           0,      0,      0,      0,   -167,   -131,   -123,   -133,
                           0,      0,      0,      0,   -165,   -126,   -123,   -125,
                           0,      0,      0,      0,   -116,   -109,    -85,    -89,
                           0,      0,      0,      0,    -71,    -65,    -37,    -27,
                           0,      0,      0,      0,    -29,    -57,    -12,     -8,
                ],
        ];

        internal static readonly short[][] EndGameKingTable =
        [
                [
                         -59,    -39,    -24,    -16,      0,      0,      0,      0,
                         -28,     -7,     -1,      5,      0,      0,      0,      0,
                         -15,      9,     26,     31,      0,      0,      0,      0,
                         -13,     20,     41,     59,      0,      0,      0,      0,
                         -17,     15,     41,     57,      0,      0,      0,      0,
                         -12,     10,     29,     35,      0,      0,      0,      0,
                         -37,     -8,      4,      7,      0,      0,      0,      0,
                         -61,    -44,    -27,    -16,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,    -13,      0,    -17,    -42,
                           0,      0,      0,      0,     28,     23,      9,    -13,
                           0,      0,      0,      0,     54,     48,     26,      7,
                           0,      0,      0,      0,     77,     58,     42,     15,
                           0,      0,      0,      0,     76,     58,     43,     14,
                           0,      0,      0,      0,     55,     46,     25,      5,
                           0,      0,      0,      0,     26,     23,      6,    -16,
                           0,      0,      0,      0,    -10,     -2,    -20,    -47,
                ],
        ];

        internal static readonly short[][] MiddleGameEnemyKingTable =
        [
                [
                         -17,    -36,    -27,      3,    140,    166,    115,    134,
                           5,    -23,    -31,      0,    140,    152,    137,    127,
                         -16,     12,     36,     11,    152,    157,    147,    134,
                          42,     37,     21,     56,    170,    128,    116,    110,
                          14,     43,     39,     41,    167,    126,    121,     87,
                          -3,    -11,     16,     12,    154,    130,    139,    162,
                         -41,    -16,    -15,      1,    142,    138,    121,    117,
                         -22,    -23,     -9,     16,    144,    160,    101,    118,
                ],
                [
                        -149,   -163,   -155,   -123,     -1,     38,     -7,      4,
                        -156,   -138,   -133,   -130,     33,     16,    -10,      1,
                         -91,   -122,   -113,    -98,     21,     19,      8,     43,
                        -113,   -114,    -96,    -93,     12,     30,     36,     66,
                        -106,   -131,   -110,    -94,     12,     22,     33,     72,
                         -89,   -104,    -91,    -87,     -3,     13,      6,     25,
                        -131,   -126,   -118,   -118,     19,      2,    -20,     -7,
                        -122,   -148,   -147,   -120,     -3,     33,    -20,     -5,
                ],
        ];

        internal static readonly short[][] EndGameEnemyKingTable =
        [
                [
                          27,     13,     15,     11,      5,      6,     32,     42,
                           7,      3,      0,     -5,    -23,    -18,     -8,     10,
                          18,     -7,    -20,    -22,    -40,    -33,    -24,     -5,
                          14,     -4,    -20,    -41,    -56,    -40,    -23,     -9,
                          23,     -3,    -23,    -40,    -56,    -39,    -28,     -9,
                          21,     -0,    -14,    -19,    -36,    -27,    -25,    -18,
                          24,      3,     -4,     -3,    -22,    -13,     -6,      9,
                          34,     20,      8,     10,      4,     11,     30,     39,
                ],
                [
                          49,     39,     26,     15,     14,     -3,     12,     35,
                          30,      6,      1,     -3,    -24,    -16,     -5,     10,
                           4,     -6,    -19,    -26,    -38,    -30,    -15,     -7,
                           3,    -16,    -29,    -38,    -49,    -40,    -25,    -11,
                           2,    -11,    -25,    -37,    -53,    -38,    -24,    -10,
                          -1,    -11,    -22,    -28,    -31,    -28,    -16,     -1,
                          24,      7,     -2,     -6,    -21,    -14,     -4,     10,
                          43,     35,     27,     14,     13,     -3,     13,     36,
                ],
        ];

#pragma warning restore IDE0055

    /// <summary>
    /// 2x14x64
    /// </summary>
    public static readonly int[][][] PackedPSQT = new int[2][][];

    public const int WhiteEnemyKingTableIndex = 12;

    public const int BlackEnemyKingTableIndex = 13;

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
            PackedPSQT[bucket] = new int[14][];
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

            PackedPSQT[bucket][WhiteEnemyKingTableIndex] = new int[64];
            PackedPSQT[bucket][BlackEnemyKingTableIndex] = new int[64];

            for (int sq = 0; sq < 64; ++sq)
            {
                PackedPSQT[bucket][WhiteEnemyKingTableIndex][sq] = Utils.Pack(
                    MiddleGameEnemyKingTable[bucket][sq],
                    EndGameEnemyKingTable[bucket][sq]);

                PackedPSQT[bucket][BlackEnemyKingTableIndex][sq] = Utils.Pack(
                    (short)(-MiddleGameEnemyKingTable[bucket][sq ^ 56]),
                    (short)(-EndGameEnemyKingTable[bucket][sq ^ 56]));
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
