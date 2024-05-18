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
        +105, +399, +365, +493, +1129, 0,
        -105, -399, -365, -493, -1129, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +128, +440, +392, +769, +1408, 0,
        -128, -440, -392, -769, -1408, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -26,    -25,    -16,    -9,     -3,     29,     29,     -11,
        -27,    -27,    -7,     10,     17,     26,     23,     11,
        -25,    -17,    1,      18,     26,     29,     1,      -1,
        -25,    -13,    0,      20,     27,     27,     1,      -2,
        -23,    -21,    -5,     3,      13,     23,     16,     5,
        -26,    -23,    -20,    -13,    -6,     23,     19,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        15,     10,     5,      -12,    7,      1,      -7,     -7,
        13,     9,      -1,     -12,    -6,     -7,     -7,     -8,
        27,     17,     0,      -19,    -15,    -13,    4,      0,
        25,     16,     -1,     -15,    -14,    -10,    2,      -2,
        14,     7,      -3,     -10,    -3,     -5,     -6,     -8,
        17,     11,     9,      -11,    16,     4,      -4,     -4,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -147,   -24,    -51,    -30,    -14,    -21,    -11,    -98,
        -45,    -28,    -5,     15,     16,     25,     -12,    -17,
        -29,    0,      18,     56,     60,     39,     33,     -4,
        -11,    25,     42,     59,     58,     58,     44,     17,
        -8,     23,     44,     46,     56,     56,     44,     16,
        -27,    2,      18,     48,     57,     32,     26,     -5,
        -46,    -18,    0,      14,     15,     19,     -12,    -18,
        -164,   -26,    -50,    -20,    -9,     -11,    -19,    -88,
];

internal static readonly short[] EndGameKnightTable =
[
        -62,    -59,    -12,    -12,    -10,    -27,    -53,    -82,
        -17,    0,      13,     8,      8,      5,      -13,    -22,
        -13,    14,     35,     34,     32,     16,     8,      -13,
        5,      18,     47,     48,     52,     46,     23,     -5,
        4,      23,     45,     51,     52,     41,     27,     -1,
        -15,    17,     26,     39,     31,     18,     6,      -9,
        -25,    2,      5,      11,     4,      0,      -12,    -26,
        -66,    -58,    -8,     -16,    -12,    -26,    -51,    -82,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -17,    15,     -2,     -14,    -10,    -16,    -21,    1,
        6,      2,      7,      -17,    2,      3,      28,     -7,
        -6,     5,      -4,     2,      -8,     14,     5,      28,
        -6,     -6,     -6,     24,     20,     -17,    2,      -1,
        -13,    -1,     -13,    20,     6,      -14,    -6,     5,
        5,      5,      7,      -5,     6,      8,      7,      24,
        9,      13,     11,     -5,     -2,     5,      20,     -1,
        8,      20,     12,     -29,    -11,    -20,    2,      -14,
];

internal static readonly short[] EndGameBishopTable =
[
        -8,     15,     -12,    4,      -1,     6,      -1,     -25,
        0,      -7,     -3,     5,      2,      -10,    -3,     -15,
        14,     13,     6,      1,      10,     3,      6,      9,
        13,     7,      6,      -4,     -7,     6,      5,      8,
        8,      10,     5,      -1,     -10,    6,      7,      10,
        10,     3,      -1,     -1,     5,      -2,     3,      7,
        -10,    -8,     -14,    3,      1,      -3,     -2,     -7,
        -6,     -13,    -8,     8,      7,      7,      -3,     -11,
];

internal static readonly short[] MiddleGameRookTable =
[
        -5,     -10,    -5,     2,      14,     3,      6,      -4,
        -26,    -17,    -13,    -11,    1,      5,      19,     -2,
        -28,    -19,    -23,    -12,    5,      11,     50,     30,
        -25,    -20,    -18,    -6,     -4,     10,     38,     21,
        -18,    -16,    -12,    -4,     -6,     8,      29,     14,
        -22,    -15,    -19,    -4,     2,      19,     48,     27,
        -24,    -26,    -10,    -6,     3,      3,      26,     3,
        -4,     -4,     -1,     11,     21,     7,      14,     7,
];

internal static readonly short[] EndGameRookTable =
[
        5,      3,      7,      -1,     -10,    5,      1,      -3,
        16,     19,     18,     8,      -2,     -2,     -3,     1,
        12,     10,     11,     5,      -8,     -10,    -21,    -18,
        14,     10,     12,     4,      0,      -1,     -14,    -13,
        14,     10,     12,     3,      1,      -6,     -12,    -10,
        13,     12,     4,      -3,     -10,    -14,    -20,    -12,
        19,     21,     14,     4,      -4,     -3,     -6,     2,
        0,      -2,     3,      -8,     -17,    -3,     -7,     -11,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -11,    -9,     -5,     10,     4,      -30,    8,      -5,
        -2,     -11,    8,      -1,     4,      7,      24,     62,
        -7,     -6,     -9,     -10,    -12,    7,      33,     58,
        -13,    -19,    -19,    -10,    -11,    -4,     11,     24,
        -11,    -16,    -20,    -20,    -11,    -5,     9,      22,
        -8,     -3,     -14,    -11,    -5,     4,      20,     38,
        -15,    -21,    4,      11,     9,      5,      8,      46,
        -9,     -9,     7,      12,     8,      -35,    -14,    25,
];

internal static readonly short[] EndGameQueenTable =
[
        -24,    -22,    -11,    -11,    -16,    -6,     -33,    14,
        -15,    -10,    -28,    -2,     -3,     -15,    -41,    -7,
        -17,    -6,     5,      0,      22,     21,     -1,     3,
        -10,    7,      7,      12,     27,     37,     44,     36,
        -4,     3,      15,     24,     24,     33,     28,     44,
        -11,    -14,    11,     6,      13,     20,     20,     20,
        -10,    -2,     -22,    -20,    -15,    -15,    -26,    4,
        -15,    -16,    -20,    -5,     -11,    16,     14,     0,
];

internal static readonly short[] MiddleGameKingTable =
[
        27,     51,     26,     -74,    9,      -61,    38,     48,
        -6,     -16,    -32,    -72,    -85,    -59,    -11,    18,
        -74,    -63,    -104,   -106,   -115,   -128,   -85,    -93,
        -95,    -91,    -110,   -144,   -142,   -137,   -133,   -168,
        -70,    -65,    -99,    -122,   -139,   -122,   -139,   -158,
        -79,    -35,    -91,    -101,   -92,    -104,   -75,    -87,
        76,     -8,     -35,    -63,    -67,    -46,    4,      27,
        41,     77,     39,     -56,    21,     -50,    52,     64,
];

internal static readonly short[] EndGameKingTable =
[
        -70,    -45,    -19,    7,      -31,    -2,     -36,    -88,
        -15,    18,     27,     39,     46,     34,     13,     -22,
        8,      40,     58,     68,     71,     64,     43,     21,
        11,     51,     73,     87,     86,     79,     65,     39,
        3,      42,     71,     83,     89,     77,     69,     38,
        10,     35,     56,     67,     65,     58,     41,     18,
        -38,    13,     28,     37,     40,     29,     8,      -25,
        -82,    -57,    -26,    -1,     -27,    -5,     -41,    -95,
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
