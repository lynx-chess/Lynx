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
        +110, +402, +370, +491, +1135, 0,
        -110, -402, -370, -491, -1135, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +133, +438, +391, +769, +1402, 0,
        -133, -438, -391, -769, -1402, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -28,    -25,    -17,    -6,     -1,     41,     39,     -4,
        -28,    -26,    -8,     7,      16,     28,     24,     7,
        -28,    -17,    0,      15,     23,     29,     3,      -5,
        -27,    -13,    -2,     17,     25,     26,     2,      -6,
        -25,    -20,    -6,     1,      12,     25,     18,     1,
        -27,    -22,    -21,    -10,    -2,     35,     29,     -12,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        13,     13,     8,      -14,    5,      -1,     -5,     -12,
        11,     12,     1,      -12,    -6,     -5,     -3,     -10,
        26,     19,     1,      -19,    -15,    -11,    7,      -1,
        23,     18,     -1,     -15,    -14,    -8,     5,      -3,
        12,     10,     -2,     -10,    -2,     -4,     -3,     -10,
        15,     13,     10,     -13,    13,     2,      -3,     -8,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -147,   -26,    -52,    -29,    -15,    -9,     -13,    -91,
        -47,    -30,    -7,     16,     18,     33,     -5,     -8,
        -32,    -3,     16,     54,     57,     38,     32,     -5,
        -14,    22,     40,     56,     56,     55,     42,     14,
        -11,    21,     42,     44,     54,     54,     42,     14,
        -30,    0,      16,     45,     55,     31,     25,     -7,
        -47,    -20,    -2,     15,     16,     27,     -5,     -8,
        -161,   -28,    -49,    -19,    -11,    0,      -21,    -80,
];

internal static readonly short[] EndGameKnightTable =
[
        -62,    -58,    -11,    -12,    -9,     -30,    -52,    -83,
        -16,    2,      14,     8,      8,      2,      -14,    -23,
        -13,    15,     36,     35,     34,     17,     9,      -13,
        6,      19,     48,     49,     53,     47,     24,     -5,
        5,      24,     46,     51,     53,     41,     28,     -1,
        -14,    18,     27,     40,     32,     19,     7,      -8,
        -25,    2,      6,      10,     4,      -4,     -13,    -28,
        -68,    -56,    -7,     -16,    -11,    -29,    -50,    -82,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -21,    13,     -5,     -14,    -12,    -7,     -13,    7,
        3,      -1,     5,      -18,    2,      8,      35,     -2,
        -9,     2,      -7,     0,      -11,    11,     2,      25,
        -10,    -10,    -9,     20,     17,     -21,    -1,     -4,
        -17,    -5,     -17,    17,     3,      -17,    -9,     3,
        1,      2,      4,      -7,     2,      5,      5,      21,
        7,      11,     9,      -5,     -2,     12,     27,     5,
        5,      18,     8,      -30,    -13,    -12,    12,     -11,
];

internal static readonly short[] EndGameBishopTable =
[
        -8,     15,     -11,    3,      -1,     2,      -3,     -28,
        0,      -6,     -3,     4,      1,      -12,    -5,     -18,
        14,     13,     6,      2,      11,     4,      7,      9,
        13,     8,      6,      -4,     -7,     6,      6,      8,
        9,      10,     5,      -1,     -10,    6,      7,      9,
        11,     4,      -1,     0,      5,      -1,     4,      7,
        -10,    -9,     -14,    2,      0,      -7,     -4,     -10,
        -7,     -13,    -6,     8,      7,      2,      -5,     -12,
];

internal static readonly short[] MiddleGameRookTable =
[
        -7,     -12,    -6,     2,      12,     15,     15,     -3,
        -27,    -20,    -15,    -13,    -1,     13,     25,     2,
        -30,    -21,    -25,    -15,    2,      9,      51,     31,
        -28,    -22,    -20,    -10,    -7,     7,      38,     21,
        -20,    -19,    -16,    -8,     -9,     5,      30,     15,
        -24,    -17,    -21,    -7,     0,      18,     48,     29,
        -25,    -28,    -12,    -7,     1,      11,     33,     8,
        -6,     -6,     -2,     11,     19,     19,     21,     8,
];

internal static readonly short[] EndGameRookTable =
[
        6,      3,      7,      -2,     -9,     2,      -2,     -6,
        16,     20,     18,     8,      -2,     -5,     -6,     0,
        13,     11,     12,     6,      -8,     -9,     -21,    -18,
        15,     11,     13,     5,      0,      -1,     -14,    -13,
        15,     10,     12,     4,      1,      -5,     -12,    -10,
        13,     13,     5,      -3,     -9,     -13,    -20,    -13,
        19,     22,     15,     4,      -4,     -6,     -9,     0,
        1,      -2,     3,      -8,     -16,    -7,     -10,    -14,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -14,    -10,    -6,     12,     4,      -20,    17,     -5,
        -5,     -13,    6,      0,      4,      15,     28,     59,
        -10,    -9,     -11,    -12,    -14,    5,      31,     57,
        -16,    -22,    -21,    -14,    -13,    -8,     9,      21,
        -15,    -18,    -23,    -23,    -13,    -8,     7,      19,
        -11,    -5,     -16,    -12,    -7,     2,      18,     38,
        -18,    -23,    2,      12,     9,      13,     13,     45,
        -12,    -10,    6,      15,     7,      -25,    -3,     23,
];

internal static readonly short[] EndGameQueenTable =
[
        -22,    -22,    -10,    -15,    -16,    -12,    -38,    15,
        -13,    -9,     -28,    -5,     -5,     -21,    -41,    -3,
        -15,    -4,     6,      0,      22,     23,     1,      4,
        -7,     8,      6,      13,     27,     38,     45,     38,
        -1,     4,      15,     26,     23,     34,     29,     47,
        -7,     -14,    12,     7,      14,     21,     23,     20,
        -8,     -1,     -22,    -23,    -17,    -20,    -27,    6,
        -12,    -16,    -19,    -9,     -11,    11,     8,      0,
];

internal static readonly short[] MiddleGameKingTable =
[
        13,     48,     31,     -81,    3,      -63,    36,     29,
        -8,     -8,     -20,    -61,    -73,    -44,    5,      17,
        -73,    -51,    -92,    -94,    -104,   -113,   -71,    -91,
        -92,    -83,    -102,   -132,   -130,   -127,   -123,   -165,
        -66,    -55,    -90,    -110,   -128,   -112,   -127,   -154,
        -75,    -21,    -80,    -90,    -80,    -89,    -59,    -85,
        76,     1,      -21,    -54,    -57,    -32,    19,     26,
        34,     79,     47,     -62,    13,     -53,    50,     46,
];

internal static readonly short[] EndGameKingTable =
[
        -64,    -41,    -20,    10,     -30,    -1,     -35,    -79,
        -14,    17,     25,     37,     44,     30,     9,      -20,
        8,      37,     55,     65,     69,     61,     40,     21,
        10,     49,     71,     85,     83,     77,     62,     39,
        3,      40,     68,     81,     86,     75,     65,     37,
        9,      33,     53,     65,     63,     54,     38,     17,
        -37,    12,     26,     36,     37,     26,     4,      -24,
        -78,    -55,    -28,    1,      -25,    -4,     -39,    -86,
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
