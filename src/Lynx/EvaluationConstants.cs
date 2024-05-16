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
        +113, +390, +360, +485, +1107, 0,
        -113, -390, -360, -485, -1107, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +144, +440, +393, +769, +1415, 0,
        -144, -440, -393, -769, -1415, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -31,    -11,    -17,    -13,    -4,     27,     41,     -21,
        -31,    -13,    -8,     6,      16,     26,     35,     0,
        -31,    -4,     0,      13,     23,     29,     13,     -12,
        -30,    0,      -1,     15,     24,     26,     13,     -13,
        -29,    -7,     -5,     -1,     12,     23,     29,     -6,
        -31,    -9,     -21,    -17,    -7,     21,     31,     -28,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        8,      16,     5,      -11,    5,      3,      4,      -12,
        7,      15,     -2,     -13,    -8,     -5,     3,      -13,
        21,     21,     -3,     -22,    -17,    -12,    12,     -5,
        18,     20,     -4,     -17,    -16,    -9,     10,     -7,
        8,      13,     -5,     -11,    -4,     -5,     5,      -14,
        11,     17,     7,      -11,    15,     6,      7,      -9,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -146,   -23,    -52,    -32,    -13,    -21,    -11,    -99,
        -46,    -29,    -4,     15,     17,     23,     -16,    -18,
        -29,    1,      19,     56,     59,     40,     33,     -5,
        -12,    24,     43,     58,     57,     58,     44,     17,
        -8,     24,     44,     46,     56,     57,     44,     16,
        -27,    2,      19,     48,     57,     33,     27,     -6,
        -47,    -19,    0,      14,     15,     17,     -13,    -19,
        -163,   -25,    -49,    -22,    -10,    -12,    -18,    -88,
];

internal static readonly short[] EndGameKnightTable =
[
        -65,    -58,    -11,    -10,    -9,     -26,    -53,    -86,
        -17,    3,      13,     9,      9,      6,      -10,    -19,
        -14,    15,     36,     36,     33,     17,     9,      -14,
        8,      20,     48,     48,     52,     47,     24,     -6,
        5,      25,     47,     51,     52,     42,     29,     0,
        -15,    18,     26,     40,     32,     18,     6,      -10,
        -24,    4,      6,      12,     5,      0,      -11,    -23,
        -70,    -55,    -7,     -13,    -11,    -25,    -49,    -84,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -17,    14,     -2,     -15,    -12,    -16,    -21,    0,
        7,      3,      6,      -17,    2,      -1,     27,     -12,
        -6,     5,      -4,     3,      -7,     13,     4,      27,
        -8,     -6,     -5,     22,     19,     -17,    3,      -1,
        -15,    -1,     -13,    18,     6,      -12,    -6,     5,
        4,      4,      7,      -4,     6,      7,      7,      22,
        9,      14,     11,     -5,     -3,     3,      20,     -3,
        8,      18,     11,     -30,    -13,    -20,    2,      -14,
];

internal static readonly short[] EndGameBishopTable =
[
        -10,    14,     -13,    3,      -1,     4,      -1,     -27,
        -2,     -7,     -4,     4,      1,      -9,     -3,     -14,
        12,     13,     6,      1,      10,     2,      6,      9,
        13,     7,      5,      -5,     -8,     6,      5,      6,
        8,      10,     5,      -1,     -10,    5,      7,      7,
        10,     4,      -1,     -1,     4,      -1,     3,      6,
        -12,    -9,     -15,    2,      0,      -4,     -2,     -10,
        -7,     -12,    -9,     7,      7,      7,      -4,     -12,
];

internal static readonly short[] MiddleGameRookTable =
[
        -3,     -10,    -4,     2,      14,     4,      7,      -2,
        -26,    -16,    -13,    -12,    0,      4,      16,     -3,
        -30,    -21,    -22,    -13,    3,      10,     50,     27,
        -25,    -21,    -17,    -9,     -4,     10,     38,     19,
        -19,    -15,    -13,    -6,     -7,     9,      29,     14,
        -22,    -17,    -19,    -4,     2,      20,     48,     27,
        -23,    -25,    -9,     -6,     2,      2,      22,     3,
        -2,     -4,     0,      11,     21,     9,      14,     10,
];

internal static readonly short[] EndGameRookTable =
[
        4,      1,      5,      -4,     -12,    3,      -1,     -4,
        15,     18,     17,     7,      -2,     -3,     -4,     2,
        13,     11,     11,     5,      -7,     -10,    -21,    -17,
        14,     10,     12,     5,      -2,     -2,     -14,    -14,
        14,     9,      12,     3,      1,      -7,     -11,    -9,
        12,     13,     4,      -3,     -10,    -14,    -20,    -12,
        19,     21,     14,     3,      -4,     -3,     -4,     2,
        -1,     -4,     1,      -9,     -19,    -5,     -9,     -13,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -12,    -10,    -6,     9,      2,      -30,    8,      3,
        -1,     -10,    7,      -3,     2,      6,      22,     48,
        -10,    -6,     -9,     -10,    -12,    7,      33,     54,
        -12,    -19,    -17,    -9,     -9,     -5,     10,     24,
        -12,    -16,    -18,    -18,    -8,     -5,     9,      22,
        -6,     -4,     -14,    -12,    -5,     4,      19,     37,
        -15,    -18,    3,      10,     7,      3,      7,      36,
        -9,     -10,    4,      11,     6,      -36,    -14,    27,
];

internal static readonly short[] EndGameQueenTable =
[
        -23,    -20,    -10,    -11,    -15,    -9,     -33,    9,
        -16,    -8,     -25,    0,      -1,     -16,    -44,    -5,
        -13,    -4,     5,      1,      22,     21,     -7,     5,
        -9,     9,      7,      12,     26,     38,     44,     31,
        -3,     5,      14,     25,     21,     35,     25,     39,
        -15,    -11,    14,     11,     13,     20,     19,     16,
        -10,    -4,     -20,    -19,    -13,    -11,    -30,    5,
        -15,    -15,    -16,    -6,     -9,     17,     13,     -4,
];

internal static readonly short[] MiddleGameKingTable =
[
        23,     50,     27,     -73,    10,     -60,    39,     48,
        -10,    -15,    -33,    -70,    -82,    -57,    -9,     19,
        -82,    -66,    -104,   -105,   -116,   -124,   -83,    -95,
        -105,   -95,    -114,   -149,   -143,   -138,   -137,   -164,
        -71,    -72,    -104,   -129,   -147,   -125,   -143,   -158,
        -80,    -45,    -95,    -103,   -92,    -104,   -75,    -87,
        70,     -10,    -37,    -63,    -67,    -47,    4,      26,
        35,     73,     37,     -59,    20,     -52,    52,     61,
];

internal static readonly short[] EndGameKingTable =
[
        -72,    -45,    -19,    5,      -32,    -3,     -38,    -89,
        -12,    18,     27,     40,     46,     33,     13,     -23,
        10,     42,     59,     69,     72,     64,     44,     22,
        15,     54,     75,     90,     87,     81,     68,     39,
        5,      45,     72,     86,     91,     78,     71,     39,
        11,     39,     58,     68,     66,     59,     42,     17,
        -37,    14,     29,     37,     39,     29,     8,      -26,
        -82,    -55,    -26,    -1,     -29,    -6,     -42,    -94,
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
