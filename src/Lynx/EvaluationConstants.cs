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
        +98, +393, +359, +486, +1101, 0,
        -98, -393, -359, -486, -1101, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +161, +415, +377, +737, +1362, 0,
        -161, -415, -377, -737, -1362, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -31,    -7,     -14,    -13,    -2,     29,     40,     -18,
        -34,    -12,    -8,     10,     19,     21,     30,     1,
        -33,    -3,     0,      14,     23,     25,     9,      -13,
        -33,    1,      -1,     16,     25,     22,     9,      -13,
        -31,    -6,     -6,     1,      11,     18,     23,     -4,
        -31,    -5,     -19,    -19,    -7,     22,     30,     -25,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        16,     16,     1,      42,     15,     -9,     -5,     -14,
        13,     15,     -2,     4,      -11,    -18,    -7,     -17,
        24,     17,     -8,     -20,    -26,    -29,    0,      -9,
        21,     15,     -8,     -16,    -22,    -24,    -2,     -13,
        14,     9,      -7,     5,      -5,     -17,    -5,     -18,
        17,     13,     4,      52,     28,     -7,     -2,     -13,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -142,   -21,    -51,    -31,    -13,    -21,    -10,    -95,
        -46,    -27,    -3,     16,     17,     25,     -16,    -18,
        -29,    0,      20,     55,     58,     38,     31,     -6,
        -11,    24,     42,     58,     56,     57,     42,     15,
        -7,     24,     44,     46,     55,     55,     43,     14,
        -26,    2,      19,     48,     57,     32,     25,     -8,
        -46,    -18,    0,      15,     16,     19,     -12,    -19,
        -153,   -22,    -48,    -20,    -11,    -11,    -18,    -88,
];

internal static readonly short[] EndGameKnightTable =
[
        -72,    -57,    -16,    -9,     -9,     -26,    -48,    -104,
        -22,    4,      11,     11,     12,     8,      -5,     -17,
        -13,    15,     39,     37,     36,     23,     12,     -11,
        8,      21,     49,     49,     54,     49,     28,     -3,
        7,      23,     46,     53,     54,     45,     32,     4,
        -18,    19,     32,     42,     34,     23,     9,      -9,
        -27,    7,      3,      13,     8,      0,      -8,     -27,
        -90,    -56,    -14,    -13,    -10,    -30,    -44,    -103,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -16,    17,     2,      -18,    -11,    -15,    -15,    1,
        7,      4,      4,      -17,    1,      1,      28,     -13,
        -6,     3,      -4,     1,      -8,     12,     2,      26,
        -9,     -7,     -7,     22,     18,     -18,    1,      -3,
        -15,    -2,     -14,    18,     5,      -14,    -6,     3,
        5,      2,      7,      -6,     5,      6,      4,      23,
        9,      15,     9,      -6,     -3,     4,      21,     -3,
        6,      17,     14,     -32,    -15,    -18,    5,      -13,
];

internal static readonly short[] EndGameBishopTable =
[
        -18,    3,      -21,    2,      -3,     6,      -5,     -33,
        -9,     -8,     -1,     4,      6,      -5,     1,      -19,
        10,     17,     8,      7,      16,     8,      9,      11,
        13,     10,     10,     -3,     -7,     10,     9,      6,
        8,      13,     9,      1,      -8,     11,     9,      8,
        10,     7,      3,      4,      10,     3,      8,      7,
        -19,    -10,    -13,    4,      4,      2,      1,      -11,
        -12,    -16,    -15,    8,      7,      7,      -7,     -21,
];

internal static readonly short[] MiddleGameRookTable =
[
        -3,     -10,    -2,     5,      16,     6,      6,      -2,
        -26,    -16,    -13,    -11,    1,      2,      14,     -3,
        -28,    -19,    -22,    -12,    2,      9,      47,     27,
        -19,    -19,    -16,    -5,     -3,     8,      37,     22,
        -15,    -15,    -11,    -3,     -6,     7,      27,     17,
        -19,    -16,    -17,    -2,     2,      17,     45,     27,
        -24,    -25,    -8,     -3,     3,      2,      22,     5,
        -2,     -3,     3,      14,     23,     11,     14,     10,
];

internal static readonly short[] EndGameRookTable =
[
        -2,     0,      4,      -4,     -12,    -1,     -2,     -7,
        13,     21,     21,     10,     2,      5,      6,      1,
        10,     13,     15,     9,      0,      -5,     -15,    -16,
        9,      7,      13,     8,      0,      0,      -19,    -20,
        8,      7,      13,     7,      2,      -4,     -14,    -14,
        9,      15,     8,      3,      -5,     -7,     -18,    -13,
        17,     25,     16,     7,      0,      3,      0,      0,
        -5,     -5,     0,      -7,     -17,    -8,     -12,    -17,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -9,     -8,     -5,     12,     4,      -31,    2,      4,
        0,      -10,    6,      -3,     1,      4,      23,     51,
        -9,     -8,     -11,    -12,    -15,    5,      30,     55,
        -11,    -20,    -21,    -10,    -12,    -9,     9,      24,
        -10,    -16,    -21,    -18,    -10,    -9,     8,      22,
        -6,     -5,     -16,    -14,    -8,     2,      17,     37,
        -13,    -18,    2,      10,     7,      2,      8,      40,
        -8,     -8,     7,      15,     8,      -34,    -10,    29,
];

internal static readonly short[] EndGameQueenTable =
[
        -31,    -25,    -14,    -14,    -21,    -3,     -24,    4,
        -23,    -7,     -24,    0,      0,      -10,    -41,    -14,
        -18,    -1,     11,     7,      29,     25,     0,      1,
        -17,    8,      11,     19,     29,     41,     44,     27,
        -14,    2,      18,     30,     23,     38,     22,     34,
        -18,    -9,     18,     17,     20,     23,     22,     12,
        -20,    -5,     -19,    -18,    -11,    -8,     -29,    -4,
        -21,    -21,    -22,    -10,    -16,    11,     3,      -14,
];

internal static readonly short[] MiddleGameKingTable =
[
        17,     46,     28,     -70,    12,     -59,    36,     42,
        -25,    -18,    -31,    -69,    -80,    -55,    -10,    15,
        -92,    -71,    -107,   -106,   -116,   -127,   -85,    -98,
        -111,   -105,   -121,   -149,   -149,   -142,   -140,   -164,
        -66,    -74,    -105,   -125,   -152,   -128,   -138,   -154,
        -82,    -43,    -91,    -101,   -89,    -99,    -71,    -86,
        64,     -10,    -34,    -58,    -62,    -42,    7,      24,
        24,     74,     40,     -54,    25,     -48,    51,     58,
];

internal static readonly short[] EndGameKingTable =
[
        -75,    -53,    -31,    -12,    -46,    -12,    -40,    -93,
        -9,     13,     19,     29,     37,     28,     11,     -25,
        20,     47,     60,     69,     72,     66,     47,     27,
        26,     64,     84,     97,     94,     88,     76,     49,
        13,     57,     82,     94,     99,     86,     79,     49,
        19,     46,     63,     72,     70,     61,     47,     24,
        -34,    16,     28,     35,     37,     29,     10,     -22,
        -70,    -54,    -26,    -4,     -32,    -9,     -38,    -92,
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
