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
        +103, +400, +370, +496, +1135, 0,
        -103, -400, -370, -496, -1135, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +131, +439, +393, +769, +1413, 0,
        -131, -439, -393, -769, -1413, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -23,    -23,    -15,    -10,    -4,     31,     35,     -8,
        -29,    -30,    -11,    6,      10,     24,     19,     4,
        -23,    -12,    3,      17,     27,     33,     9,      0,
        -23,    -8,     1,      20,     29,     31,     10,     -1,
        -28,    -25,    -9,     -1,     6,      20,     13,     -2,
        -23,    -20,    -18,    -13,    -5,     27,     25,     -16,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        12,     11,     8,      -12,    7,      2,      -5,     -10,
        11,     11,     0,      -11,    -6,     -6,     -4,     -11,
        26,     19,     1,      -19,    -15,    -11,    7,      -1,
        24,     18,     -1,     -16,    -14,    -8,     5,      -3,
        11,     10,     -3,     -10,    -3,     -4,     -3,     -11,
        15,     12,     10,     -11,    17,     5,      -2,     -7,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -147,   -24,    -49,    -30,    -11,    -21,    -11,    -96,
        -45,    -28,    -3,     15,     17,     25,     -12,    -17,
        -29,    1,      19,     57,     60,     40,     34,     -4,
        -11,    26,     44,     61,     60,     60,     46,     17,
        -8,     24,     47,     48,     59,     59,     46,     17,
        -28,    2,      18,     48,     57,     33,     27,     -5,
        -45,    -18,    1,      14,     15,     17,     -12,    -19,
        -165,   -27,    -48,    -19,    -8,     -11,    -19,    -90,
];

internal static readonly short[] EndGameKnightTable =
[
        -62,    -59,    -11,    -11,    -9,     -25,    -54,    -81,
        -17,    2,      13,     9,      8,      5,      -13,    -21,
        -13,    15,     37,     35,     33,     17,     9,      -13,
        6,      19,     48,     48,     53,     46,     24,     -5,
        4,      24,     46,     52,     53,     42,     28,     -1,
        -14,    18,     27,     39,     31,     19,     6,      -8,
        -25,    2,      6,      11,     4,      0,      -12,    -25,
        -66,    -57,    -7,     -15,    -12,    -25,    -50,    -79,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -18,    15,     -2,     -13,    -9,     -16,    -20,    2,
        3,      2,      7,      -18,    3,      0,      28,     -10,
        -9,     5,      -5,     3,      -8,     14,     4,      27,
        -5,     -6,     -5,     24,     21,     -16,    3,      -2,
        -14,    -1,     -13,    21,     6,      -13,    -6,     5,
        3,      5,      6,      -5,     5,      7,      7,      22,
        6,      13,     11,     -6,     -3,     3,      19,     -5,
        8,      20,     11,     -29,    -10,    -23,    0,      -15,
];

internal static readonly short[] EndGameBishopTable =
[
        -9,     16,     -12,    3,      -1,     7,      -1,     -26,
        0,      -7,     -3,     6,      2,      -9,     -4,     -15,
        14,     14,     7,      2,      11,     4,      6,      9,
        12,     8,      6,      -3,     -6,     7,      6,      8,
        8,      10,     6,      0,      -9,     7,      8,      9,
        11,     4,      0,      0,      6,      -1,     4,      7,
        -9,     -9,     -14,    3,      1,      -3,     -3,     -7,
        -8,     -14,    -8,     8,      6,      8,      -2,     -10,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -4,     1,      15,     2,      7,      -5,
        -25,    -17,    -13,    -11,    1,      5,      19,     -3,
        -28,    -20,    -23,    -12,    5,      11,     50,     30,
        -26,    -20,    -17,    -7,     -4,     11,     40,     21,
        -18,    -15,    -11,    -5,     -6,     9,      31,     15,
        -23,    -15,    -19,    -5,     2,      20,     48,     27,
        -23,    -25,    -10,    -8,     2,      2,      26,     3,
        -3,     -4,     -1,     11,     21,     6,      14,     8,
];

internal static readonly short[] EndGameRookTable =
[
        5,      3,      7,      -1,     -10,    5,      0,      -3,
        15,     19,     17,     7,      -3,     -4,     -5,     1,
        13,     11,     12,     6,      -8,     -10,    -20,    -17,
        14,     10,     12,     4,      -1,     -2,     -14,    -13,
        14,     9,      11,     3,      0,      -7,     -12,    -10,
        13,     13,     5,      -3,     -9,     -14,    -19,    -12,
        18,     21,     14,     3,      -5,     -4,     -7,     1,
        0,      -3,     3,      -8,     -17,    -3,     -7,     -12,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -12,    -9,     -6,     8,      3,      -31,    7,      -6,
        -3,     -10,    8,      -2,     4,      6,      23,     60,
        -8,     -7,     -9,     -10,    -12,    6,      32,     57,
        -14,    -18,    -18,    -10,    -11,    -4,     11,     23,
        -13,    -16,    -19,    -20,    -11,    -5,     9,      21,
        -8,     -4,     -15,    -11,    -6,     3,      20,     39,
        -15,    -20,    3,      9,      8,      3,      7,      45,
        -10,    -10,    5,      10,     7,      -37,    -17,    23,
];

internal static readonly short[] EndGameQueenTable =
[
        -23,    -23,    -9,     -9,     -14,    -7,     -33,    14,
        -15,    -11,    -28,    -1,     -3,     -16,    -42,    -7,
        -17,    -4,     5,      0,      23,     22,     0,      4,
        -10,    6,      6,      12,     27,     37,     44,     35,
        -4,     3,      15,     25,     24,     33,     28,     44,
        -11,    -13,    12,     7,      14,     21,     20,     18,
        -11,    -4,     -23,    -20,    -16,    -14,    -26,    5,
        -14,    -15,    -18,    -4,     -10,    16,     15,     0,
];

internal static readonly short[] MiddleGameKingTable =
[
        30,     55,     28,     -74,    10,     -60,    39,     50,
        4,      -14,    -32,    -71,    -84,    -59,    -10,    24,
        -74,    -64,    -104,   -106,   -117,   -128,   -87,    -94,
        -96,    -90,    -110,   -143,   -141,   -137,   -134,   -170,
        -68,    -66,    -98,    -123,   -141,   -125,   -141,   -160,
        -79,    -34,    -94,    -103,   -95,    -107,   -80,    -90,
        71,     -11,    -37,    -65,    -69,    -48,    3,      30,
        39,     76,     38,     -57,    19,     -51,    52,     64,
];

internal static readonly short[] EndGameKingTable =
[
        -72,    -46,    -19,    7,      -31,    -1,     -37,    -89,
        -18,    17,     27,     39,     46,     34,     12,     -24,
        7,      39,     57,     67,     71,     64,     43,     21,
        10,     50,     72,     87,     86,     79,     65,     39,
        2,      42,     70,     83,     88,     77,     68,     37,
        9,      34,     55,     67,     65,     57,     41,     17,
        -38,    13,     28,     37,     39,     29,     7,      -27,
        -82,    -57,    -25,    -1,     -27,    -5,     -41,    -95,
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
