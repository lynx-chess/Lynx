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
                        +99, +334, +352, +441, +979, 0,
                        -99, -334, -352, -441, -979, 0
                ],
                [
                        +102, +368, +382, +518, +1153, 0,
                        -102, -368, -382, -518, -1153, 0
                ],
        ];

    internal static readonly short[][] EndGamePieceValues =
    [
            [
                        +128, +406, +350, +717, +1318, 0,
                        -128, -406, -350, -717, -1318, 0
                ],
                [
                        +121, +435, +375, +772, +1466, 0,
                        -121, -435, -375, -772, -1466, 0
                ],
        ];

    internal static readonly short[][] MiddleGamePawnTable =
    [
            [
                           0,      0,      0,      0,      0,      0,      0,      0,
                          15,     46,     36,     16,    -22,    -20,    -50,    -63,
                          16,     20,     30,     18,      7,     -3,    -41,    -51,
                           8,     16,     28,     34,     21,      6,    -29,    -46,
                          21,     26,     30,     40,     19,      2,    -28,    -49,
                          36,     37,     30,     12,     -2,    -11,    -49,    -71,
                          24,     62,     25,     12,    -13,    -19,    -58,    -68,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,      0,      0,      0,      0,
                         -31,    -31,    -23,    -14,     -5,     32,     46,      7,
                         -35,    -34,    -14,      3,     12,     24,     33,     19,
                         -30,    -21,     -2,     11,     23,     30,     11,      6,
                         -30,    -18,     -4,     13,     25,     29,     12,      8,
                         -34,    -30,    -12,     -4,      7,     23,     27,     15,
                         -32,    -30,    -27,    -17,     -8,     26,     38,      1,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
        ];

    internal static readonly short[][] EndGamePawnTable =
    [
            [
                           0,      0,      0,      0,      0,      0,      0,      0,
                           1,     -9,    -11,    -16,     -0,      1,     12,     25,
                           2,      3,     -8,    -22,    -14,    -12,      3,      9,
                          24,     17,     -4,    -22,    -12,    -10,     12,     14,
                          21,     14,     -6,    -19,    -12,     -7,      9,     13,
                           5,     -7,    -12,    -17,     -9,     -9,      4,      8,
                           2,    -12,     -3,      1,      4,      3,     18,     26,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,      0,      0,      0,      0,
                          20,     13,      6,     -7,     11,      3,    -10,    -12,
                          13,      6,     -1,    -10,     -6,     -6,    -14,    -13,
                          28,     16,     -1,    -18,    -15,    -14,      2,     -2,
                          25,     16,     -2,    -14,    -14,    -12,      1,     -5,
                          13,      5,     -4,     -9,     -4,     -6,    -13,    -14,
                          23,     14,     10,     -9,     19,      7,     -8,    -10,
                           0,      0,      0,      0,      0,      0,      0,      0,
                ],
        ];

    internal static readonly short[][] MiddleGameKnightTable =
    [
            [
                        -105,    -47,    -40,    -13,    -42,    -34,    -28,    -92,
                         -46,    -55,     10,     22,     16,     44,     -6,    -29,
                         -38,     15,     44,     67,     59,     21,     12,    -11,
                          11,     37,     54,     81,     75,     55,     54,      7,
                           0,     34,     59,     52,     66,     48,     45,     25,
                         -21,     -0,     37,     63,     59,     16,      7,    -27,
                         -23,    -13,     20,     13,     14,     12,    -19,    -33,
                        -155,    -60,    -52,    -33,    -28,    -44,    -32,   -126,
                ],
                [
                        -148,    -18,    -52,    -30,     -9,    -10,     -2,    -76,
                         -42,    -26,     -7,     14,     18,     28,     -5,      1,
                         -34,     -5,      9,     47,     51,     33,     27,     -6,
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
                         -67,      5,      1,    -16,      5,    -31,    -10,    -65,
                         -12,     19,      4,      0,     -2,     -7,     -3,    -23,
                           7,      0,      8,     19,     21,     13,     -2,    -11,
                           1,      7,     31,     25,     31,     37,      4,     -9,
                           8,     15,     29,     33,     37,     26,      5,    -15,
                          -2,     19,      1,     19,     19,     11,     -4,      3,
                         -16,      7,     -5,     11,     -4,     -9,      2,    -10,
                         -55,    -13,      9,     -1,     -4,     -8,    -11,    -78,
                ],
                [
                         -51,    -43,     -8,     -0,     -3,    -22,    -39,    -82,
                          -8,      4,     10,      6,      6,     -3,     -8,    -13,
                         -13,      8,     26,     27,     21,      6,      0,     -8,
                          10,     11,     37,     40,     44,     33,     18,     -1,
                           5,     15,     37,     43,     43,     34,     24,      7,
                         -14,      8,     16,     33,     22,      7,     -2,     -7,
                         -16,      8,      2,      7,      1,     -6,    -10,    -20,
                         -66,    -34,     -3,     -6,     -4,    -26,    -36,    -70,
                ],
        ];

    internal static readonly short[][] MiddleGameBishopTable =
    [
            [
                         -19,    -24,     -7,    -15,    -19,     -6,    -17,    -11,
                         -16,     46,     21,     -0,      2,      7,     -2,    -12,
                          -3,      7,     22,     28,      5,     -4,     -2,      4,
                          -0,      4,     18,     39,     36,    -16,     -3,     -8,
                         -19,     17,      5,     34,     16,     -6,      1,     -3,
                         -10,     17,     25,     14,      4,     -5,    -16,      1,
                         -21,     61,     37,     17,     -9,     -4,    -19,    -25,
                          -6,    -11,      2,    -37,    -18,     -8,    -15,    -74,
                ],
                [
                         -13,     14,    -10,    -19,    -12,     -8,    -16,     14,
                           6,     -1,      3,    -20,      1,      4,     38,     -6,
                         -10,     -0,    -12,      1,    -14,      7,     -2,     27,
                         -11,    -10,     -5,     22,     19,    -13,      3,     -1,
                         -18,     -5,    -14,     18,      9,    -10,     -7,      5,
                           1,     -4,     -0,     -7,     -0,      0,      2,     21,
                           8,      9,      5,     -8,     -5,      9,     31,      9,
                          10,     17,      3,    -34,    -16,    -17,      8,      4,
                ],
        ];

    internal static readonly short[][] EndGameBishopTable =
    [
            [
                          13,     12,      3,     -1,      7,      4,      3,      6,
                          19,    -17,    -10,    -10,     -7,    -13,     -1,     -9,
                          19,      7,    -11,    -15,    -10,      3,     -3,      8,
                          17,     -5,     -8,     -7,    -13,      5,      2,     15,
                          13,     -1,    -10,     -9,     -6,     -5,     -3,     10,
                          31,    -14,     -7,    -17,     -5,     -0,     -1,     -8,
                           4,    -20,    -22,    -17,     -6,     -9,      4,     13,
                          12,     13,     10,     16,      4,     -2,      6,     27,
                ],
                [
                          -2,     20,     -4,      6,     -1,      3,     -1,    -26,
                          -2,     -7,     -6,      2,     -1,    -14,     -5,    -10,
                          12,      8,      7,     -1,      9,      0,      2,     13,
                          12,      1,      4,      2,      2,      2,     -2,      2,
                           6,      2,      4,      7,     -2,      5,      0,      6,
                           7,      1,     -1,     -2,      2,     -5,     -0,     14,
                          -8,     -9,    -16,      1,     -3,     -7,     -6,     -8,
                           2,    -14,     -2,      5,     11,      7,     -6,    -16,
                ],
        ];

    internal static readonly short[][] MiddleGameRookTable =
    [
            [
                         -49,    -29,    -20,     11,      2,     10,     23,      7,
                          -5,      7,     15,     -6,     -2,      8,      1,    -18,
                          -2,     -8,    -33,      2,    -16,    -23,     16,     -6,
                         -14,     20,     -2,     -3,     -8,      3,     13,      3,
                           1,     13,      9,      4,    -14,     -8,    -11,     -1,
                           5,      8,     41,      2,    -11,     19,     18,     -4,
                          12,     16,      1,      5,     -3,     -2,      7,    -15,
                         -28,    -28,     -3,     21,      9,      8,     25,      8,
                ],
                [
                          -5,    -12,     -6,     -0,     10,      7,      1,      1,
                         -33,    -21,    -13,     -7,      4,     14,     38,      4,
                         -37,    -24,    -24,    -14,      4,     12,     51,     30,
                         -33,    -31,    -25,    -11,     -6,      7,     37,     14,
                         -27,    -25,    -19,     -8,     -7,      9,     34,     15,
                         -30,    -22,    -24,     -2,      1,     21,     52,     29,
                         -30,    -30,     -8,     -3,      6,     13,     44,     19,
                          -4,     -6,     -2,     10,     18,     13,     11,     20,
                ],
        ];

    internal static readonly short[][] EndGameRookTable =
    [
            [
                          22,     19,     26,     -3,     -6,    -18,    -22,     -2,
                           4,     17,     16,     10,     -4,     -9,     -2,     -0,
                           5,     12,     19,     -5,      1,     -4,    -18,    -11,
                           8,      1,      9,      7,     -5,    -10,    -17,    -14,
                          12,      8,      4,      1,      6,     -8,     -9,     -6,
                           4,      8,    -16,     -4,     -7,     -7,    -19,     -7,
                           8,     13,     15,      1,      2,     -5,     -8,      6,
                          10,     21,     12,     -7,     -4,    -17,    -25,     -8,
                ],
                [
                           9,      4,      7,     -0,     -7,      3,      8,    -14,
                          20,     19,     18,      7,     -1,     -4,    -10,      2,
                          15,      9,     11,      5,    -10,    -12,    -23,    -19,
                          17,     11,     12,      2,     -3,     -4,    -14,    -14,
                          14,      9,     12,      1,     -5,    -10,    -14,    -14,
                          15,     13,      6,     -4,    -11,    -18,    -24,    -10,
                          21,     22,     14,      4,     -4,     -6,     -9,     -5,
                           5,     -2,      4,     -7,    -17,     -7,      1,    -23,
                ],
        ];

    internal static readonly short[][] MiddleGameQueenTable =
    [
            [
                         -23,    -51,    -42,    -10,      2,      2,      8,     29,
                          -6,    -19,     21,     17,      7,     11,     28,     48,
                         -39,     -2,    -11,     -0,    -10,     -3,     29,     72,
                          -3,    -36,    -15,      5,     -1,     -3,     15,     39,
                          -5,    -11,    -15,      6,      1,      3,     12,     25,
                         -24,    -11,    -14,     -7,      6,     -7,     26,     47,
                         -44,    -47,     19,     25,     19,     -4,     21,     50,
                         -44,    -46,    -23,    -18,      6,     -9,    -10,     17,
                ],
                [
                         -15,    -11,    -11,      6,      1,    -32,     -0,     -8,
                          -5,     -9,      8,     -1,      5,     14,     28,     44,
                          -9,     -7,     -8,    -10,    -13,      8,     31,     47,
                         -12,    -16,    -15,     -8,     -6,     -1,     12,     23,
                         -13,    -14,    -15,    -16,     -6,     -1,     12,     21,
                          -8,     -5,    -14,    -10,     -9,      4,     17,     29,
                         -18,    -17,      3,     11,      9,     13,     12,     28,
                         -16,    -11,     -2,      8,      4,    -35,    -18,     25,
                ],
        ];

    internal static readonly short[][] EndGameQueenTable =
    [
            [
                         -43,     -7,     32,     -4,    -16,    -49,    -73,    -55,
                         -10,     -7,    -29,     -1,     -5,    -24,    -34,    -14,
                          38,      8,     33,      4,     27,     10,    -40,    -31,
                           2,     37,     18,     14,     19,     15,      6,     -5,
                           3,     22,     33,     20,     15,     -5,     -8,     15,
                          18,     21,     31,     15,     15,     34,     -7,     17,
                          15,     18,    -20,     -6,      3,     24,    -37,    -37,
                         -38,    -17,     15,     23,      4,     -1,     15,    -15,
                ],
                [
                         -15,    -16,     -7,    -14,    -18,     -8,    -11,     36,
                         -15,    -10,    -26,     -4,     -6,    -20,    -46,      6,
                         -23,     -9,      0,     -0,     18,     17,     -2,     15,
                         -16,     -2,      1,      7,     21,     33,     45,     34,
                          -8,     -5,      4,     17,     16,     31,     23,     43,
                         -21,    -16,      7,      7,     10,     13,     18,     20,
                         -10,     -5,    -22,    -24,    -19,    -25,    -30,     26,
                          -2,    -12,    -12,     -8,    -16,     11,     18,      7,
                ],
        ];

    internal static readonly short[][] MiddleGameKingTable =
    [
            [
                         298,    318,    304,    248,      0,      0,      0,      0,
                         275,    284,    279,    259,      0,      0,      0,      0,
                         220,    238,    202,    202,      0,      0,      0,      0,
                         205,    221,    199,    149,      0,      0,      0,      0,
                         227,    247,    209,    167,      0,      0,      0,      0,
                         208,    239,    196,    193,      0,      0,      0,      0,
                         315,    277,    253,    248,      0,      0,      0,      0,
                         287,    312,    292,    238,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,   -107,   -170,    -77,    -89,
                           0,      0,      0,      0,   -175,   -151,   -106,    -99,
                           0,      0,      0,      0,   -214,   -214,   -173,   -198,
                           0,      0,      0,      0,   -263,   -228,   -219,   -256,
                           0,      0,      0,      0,   -258,   -216,   -221,   -251,
                           0,      0,      0,      0,   -193,   -193,   -165,   -189,
                           0,      0,      0,      0,   -162,   -141,    -90,    -92,
                           0,      0,      0,      0,   -101,   -162,    -63,    -75,
                ],
        ];

    internal static readonly short[][] EndGameKingTable =
    [
            [
                        -100,    -70,    -49,    -33,      0,      0,      0,      0,
                         -48,    -13,     -3,      6,      0,      0,      0,      0,
                         -28,     14,     42,     53,      0,      0,      0,      0,
                         -26,     27,     63,     95,      0,      0,      0,      0,
                         -33,     18,     62,     92,      0,      0,      0,      0,
                         -24,     14,     45,     56,      0,      0,      0,      0,
                         -63,    -14,      4,      8,      0,      0,      0,      0,
                        -104,    -75,    -51,    -33,      0,      0,      0,      0,
                ],
                [
                           0,      0,      0,      0,    -25,      2,    -32,    -79,
                           0,      0,      0,      0,     53,     39,     15,    -22,
                           0,      0,      0,      0,     93,     80,     44,     15,
                           0,      0,      0,      0,    129,     99,     68,     28,
                           0,      0,      0,      0,    131,     98,     70,     27,
                           0,      0,      0,      0,     88,     75,     44,     11,
                           0,      0,      0,      0,     48,     37,     10,    -25,
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
