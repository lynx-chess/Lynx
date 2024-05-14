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
        +111, +399, +367, +493, +1131, 0,
        -111, -399, -367, -493, -1131, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +133, +439, +392, +768, +1415, 0,
        -133, -439, -392, -768, -1415, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -28,    -24,    -16,    -8,     -3,     30,     32,     -14,
        -28,    -25,    -6,     11,     18,     29,     27,     8,
        -27,    -16,    2,      19,     26,     31,     5,      -4,
        -27,    -12,    0,      21,     27,     28,     4,      -5,
        -25,    -19,    -4,     5,      14,     25,     20,     2,
        -28,    -22,    -20,    -12,    -6,     24,     22,     -22,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        13,     13,     7,      -13,    7,      3,      -3,     -9,
        11,     12,     0,      -13,    -6,     -5,     -4,     -10,
        26,     19,     1,      -20,    -15,    -11,    7,      -1,
        23,     18,     -1,     -16,    -14,    -8,     5,      -3,
        12,     10,     -2,     -11,    -3,     -3,     -3,     -10,
        15,     13,     10,     -12,    16,     6,      -1,     -6,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -146,   -23,    -50,    -29,    -12,    -20,    -11,    -97,
        -45,    -28,    -4,     16,     17,     25,     -12,    -16,
        -29,    1,      19,     57,     61,     41,     34,     -3,
        -11,    25,     43,     59,     58,     59,     45,     17,
        -8,     24,     45,     47,     57,     57,     45,     17,
        -27,    3,      19,     49,     58,     34,     27,     -4,
        -44,    -18,    1,      15,     16,     19,     -12,    -17,
        -165,   -26,    -49,    -19,    -8,     -11,    -19,    -88,
];

internal static readonly short[] EndGameKnightTable =
[
        -62,    -59,    -11,    -12,    -9,     -26,    -53,    -81,
        -16,    2,      13,     9,      9,      5,      -12,    -21,
        -13,    14,     36,     35,     33,     17,     9,      -13,
        6,      18,     48,     48,     53,     47,     24,     -5,
        4,      24,     46,     51,     53,     42,     28,     -1,
        -14,    18,     26,     39,     32,     18,     7,      -9,
        -25,    3,      6,      12,     4,      0,      -11,    -26,
        -66,    -58,    -7,     -15,    -12,    -25,    -50,    -80,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -18,    15,     -2,     -14,    -10,    -16,    -21,    1,
        6,      1,      7,      -18,    2,      1,      28,     -7,
        -7,     5,      -4,     3,      -8,     14,     4,      27,
        -6,     -7,     -6,     23,     20,     -18,    2,      -2,
        -14,    -2,     -14,    19,     5,      -14,    -7,     5,
        4,      5,      7,      -4,     5,      8,      7,      23,
        9,      13,     11,     -5,     -3,     5,      19,     -2,
        8,      19,     11,     -29,    -11,    -21,    2,      -15,
];

internal static readonly short[] EndGameBishopTable =
[
        -8,     15,     -12,    4,      -1,     7,      -1,     -26,
        -1,     -7,     -3,     5,      2,      -10,    -3,     -15,
        14,     13,     6,      1,      11,     3,      6,      9,
        12,     7,      5,      -4,     -8,     6,      5,      8,
        8,      9,      5,      -1,     -10,    6,      7,      9,
        10,     3,      -1,     -1,     5,      -2,     3,      7,
        -10,    -9,     -15,    3,      1,      -4,     -2,     -8,
        -7,     -13,    -8,     8,      7,      8,      -3,     -10,
];

internal static readonly short[] MiddleGameRookTable =
[
        -5,     -10,    -5,     2,      14,     3,      7,      -4,
        -26,    -18,    -13,    -11,    1,      5,      18,     -3,
        -29,    -20,    -23,    -12,    5,      11,     51,     30,
        -26,    -21,    -18,    -7,     -5,     10,     39,     20,
        -19,    -17,    -13,    -5,     -7,     8,      31,     15,
        -22,    -15,    -19,    -4,     1,      20,     48,     27,
        -24,    -26,    -10,    -6,     2,      3,      24,     3,
        -4,     -4,     -1,     11,     21,     7,      14,     7,
];

internal static readonly short[] EndGameRookTable =
[
        5,      3,      7,      -2,     -10,    5,      1,      -3,
        16,     19,     18,     7,      -2,     -3,     -4,     2,
        13,     11,     11,     5,      -9,     -10,    -21,    -17,
        14,     10,     12,     4,      -1,     -2,     -14,    -13,
        14,     10,     11,     3,      0,      -6,     -12,    -9,
        13,     12,     4,      -4,     -10,    -14,    -20,    -12,
        19,     21,     14,     3,      -5,     -4,     -6,     2,
        1,      -3,     3,      -8,     -17,    -3,     -7,     -11,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -10,    -8,     -3,     11,     5,      -30,    7,      -3,
        -2,     -12,    7,      -1,     3,      5,      22,     62,
        -7,     -7,     -10,    -11,    -13,    5,      32,     57,
        -13,    -20,    -22,    -13,    -13,    -7,     9,      24,
        -12,    -17,    -21,    -22,    -13,    -8,     8,      22,
        -8,     -4,     -15,    -12,    -6,     3,      20,     39,
        -15,    -21,    3,      10,     8,      4,      5,      46,
        -7,     -8,     8,      13,     9,      -35,    -14,    26,
];

internal static readonly short[] EndGameQueenTable =
[
        -28,    -25,    -15,    -16,    -20,    -8,     -31,    11,
        -16,    -10,    -30,    -4,     -3,     -13,    -38,    -7,
        -17,    -4,     4,      -1,     22,     26,     2,      5,
        -10,    8,      8,      14,     28,     42,     49,     37,
        -4,     4,      16,     26,     26,     40,     32,     45,
        -11,    -14,    12,     7,      13,     25,     24,     20,
        -11,    -3,     -24,    -21,    -16,    -13,    -23,    5,
        -20,    -19,    -26,    -10,    -16,    15,     14,     -2,
];

internal static readonly short[] MiddleGameKingTable =
[
        27,     52,     26,     -74,    9,      -61,    38,     48,
        -5,     -16,    -32,    -72,    -85,    -61,    -11,    18,
        -73,    -63,    -104,   -107,   -117,   -128,   -86,    -95,
        -94,    -91,    -109,   -144,   -142,   -138,   -135,   -169,
        -68,    -65,    -99,    -122,   -140,   -124,   -141,   -158,
        -78,    -34,    -91,    -101,   -92,    -105,   -76,    -88,
        76,     -8,     -34,    -64,    -67,    -47,    4,      26,
        42,     77,     39,     -55,    21,     -50,    52,     64,
];

internal static readonly short[] EndGameKingTable =
[
        -70,    -45,    -19,    7,      -31,    -2,     -37,    -88,
        -15,    18,     27,     39,     46,     34,     13,     -22,
        8,      40,     58,     68,     71,     64,     43,     22,
        11,     51,     72,     87,     86,     79,     65,     39,
        3,      42,     70,     83,     89,     78,     69,     37,
        10,     35,     56,     67,     65,     58,     42,     18,
        -38,    14,     28,     37,     39,     29,     8,      -25,
        -82,    -56,    -25,    -1,     -27,    -5,     -41,    -94,
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
