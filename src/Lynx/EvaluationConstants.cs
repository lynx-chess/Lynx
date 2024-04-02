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
        +83, +268, +243, +334, +747, 0,
        -83, -268, -243, -334, -747, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +76, +258, +234, +457, +847, 0,
        -76, -258, -234, -457, -847, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -19,    -20,    -12,    -17,    -7,     27,     27,     -10,
        -19,    -20,    -9,     -4,     10,     26,     24,     7,
        -16,    -12,    -2,     9,      14,     26,     5,      -4,
        -17,    -7,     0,      14,     18,     26,     3,      -5,
        -16,    -11,    -7,     -7,     5,      25,     21,     1,
        -18,    -17,    -15,    -14,    -6,     24,     20,     -19,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        10,     7,      2,      -3,     7,      1,      -8,     -6,
        9,      7,      -1,     -5,     -3,     -5,     -10,    -9,
        16,     11,     -1,     -12,    -8,     -8,     0,      -3,
        16,     11,     0,      -11,    -7,     -8,     1,      -2,
        9,      6,      0,      -3,     -1,     -6,     -8,     -7,
        9,      9,      9,      -6,     11,     2,      -5,     0,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -81,    -14,    -40,    -20,    -12,    -14,    -5,     -74,
        -40,    -28,    -2,     16,     12,     12,     -14,    -20,
        -17,    7,      18,     44,     47,     29,     26,     -8,
        -6,     20,     36,     47,     42,     44,     24,     10,
        1,      18,     36,     36,     43,     45,     25,     9,
        -21,    7,      19,     40,     39,     27,     22,     -11,
        -33,    -19,    8,      14,     10,     15,     -14,    -22,
        -108,   -19,    -40,    -23,    -10,    -7,     -12,    -63,
];

internal static readonly short[] EndGameKnightTable =
[
        -42,    -38,    -1,     -7,     -8,     -14,    -39,    -60,
        -10,    12,     7,      3,      3,      -2,     -4,     -9,
        -9,     10,     18,     22,     17,     10,     6,      -4,
        9,      15,     30,     28,     34,     30,     20,     3,
        2,      13,     30,     33,     31,     26,     17,     4,
        -7,     10,     13,     21,     19,     7,      2,      -3,
        -13,    6,      2,      4,      5,      0,      -1,     -16,
        -40,    -35,    1,      -6,     -9,     -24,    -34,    -59,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -22,    -14,    1,      -14,    -14,    -6,     -11,    -22,
        12,     1,      5,      -10,    8,      1,      22,     -10,
        -3,     13,     -2,     5,      0,      14,     6,      17,
        -2,     -1,     3,      20,     21,     -9,     6,      3,
        -8,     6,      0,      25,     6,      -6,     -1,     9,
        8,      7,      8,      3,      10,     6,      7,      12,
        3,      9,      4,      3,      2,      3,      13,     -8,
        -11,    -4,     13,     -33,    -4,     -10,    -1,     -30,
];

internal static readonly short[] EndGameBishopTable =
[
        -5,     4,      -13,    3,      4,      -2,     -3,     -5,
        -3,     -6,     0,      2,      -1,     -3,     -8,     -4,
        7,      3,      3,      1,      6,      -1,     2,      1,
        6,      6,      2,      -3,     -5,     5,      5,      7,
        8,      9,      3,      -5,     -3,     6,      10,     7,
        1,      7,      1,      -3,     4,      1,      2,      6,
        -1,     -8,     -4,     1,      1,      -1,     -5,     -5,
        -6,     -7,     -11,    7,      3,      -2,     -1,     0,
];

internal static readonly short[] MiddleGameRookTable =
[
        2,      -2,     3,      6,      17,     11,     -5,     -4,
        -17,    -12,    -8,     -1,     8,      9,      11,     -22,
        -19,    -14,    -16,    -1,     5,      11,     20,     2,
        -13,    -14,    -7,     9,      9,      11,     7,      14,
        -10,    -8,     -8,     -1,     6,      10,     16,     5,
        -9,     -11,    -9,     0,      5,      20,     30,     6,
        -18,    -13,    -7,     -3,     4,      3,      1,      -7,
        5,      0,      4,      12,     19,     11,     5,      -1,
];

internal static readonly short[] EndGameRookTable =
[
        -4,     -4,     -3,     -6,     -14,    -7,     0,      -4,
        14,     11,     10,     0,      -5,     -6,     -1,     9,
        11,     12,     11,     2,      -5,     -4,     -5,     2,
        9,      9,      10,     2,      1,      3,      2,      -6,
        9,      8,      10,     5,      1,      2,      -1,     -2,
        7,      8,      6,      -2,     -7,     -7,     -8,     0,
        13,     12,     7,      -1,     -5,     -3,     6,      2,
        -7,     -7,     -3,     -10,    -18,    -10,    -2,     -6,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -2,     -4,     -3,     18,     6,      -25,    -17,    18,
        -12,    -13,    9,      2,      9,      11,     20,     30,
        -13,    -4,     -3,     -9,     2,      14,     20,     32,
        -9,     -17,    -9,     -7,     -4,     -6,     3,      11,
        -10,    -18,    -12,    -8,     -4,     -3,     3,      19,
        -9,     -1,     -3,     -2,     8,      10,     11,     16,
        -15,    -17,    9,      17,     14,     4,      2,      18,
        -7,     -5,     4,      21,     9,      -22,    -30,    11,
];

internal static readonly short[] EndGameQueenTable =
[
        -9,     -9,     -10,    -31,    -17,    -9,     20,     8,
        5,      8,      -17,    -7,     -10,    -19,    -28,    -14,
        -6,     -2,     6,      8,      8,      4,      2,      6,
        -5,     14,     13,     21,     23,     28,     29,     23,
        -4,     15,     15,     20,     13,     27,     21,     12,
        -8,     -5,     3,      0,      -8,     3,      0,      13,
        4,      9,      -18,    -27,    -21,    -18,    -27,    -7,
        -6,     -9,     -10,    -29,    -13,    -8,     24,     25,
];

internal static readonly short[] MiddleGameKingTable =
[
        -35,    16,     -4,     -77,    2,      -73,    16,     10,
        -39,    -45,    -53,    -65,    -62,    -57,    -24,    -8,
        -80,    -47,    -65,    -68,    -65,    -83,    -75,    -89,
        -49,    -39,    -49,    -40,    -59,    -74,    -87,    -81,
        -54,    -43,    -39,    -55,    -64,    -66,    -95,    -85,
        -79,    -44,    -61,    -56,    -61,    -72,    -66,    -68,
        -13,    -37,    -56,    -60,    -57,    -48,    -9,     -1,
        -13,    33,     10,     -61,    13,     -66,    28,     24,
];

internal static readonly short[] EndGameKingTable =
[
        -6,     -27,    -13,    1,      -26,    -3,     -27,    -54,
        10,     18,     17,     20,     21,     19,     6,      -13,
        21,     27,     31,     35,     36,     36,     28,     16,
        11,     31,     37,     38,     43,     41,     41,     20,
        13,     27,     34,     43,     44,     40,     41,     20,
        22,     27,     30,     32,     34,     31,     25,     11,
        -3,     14,     19,     19,     19,     15,     2,      -15,
        -26,    -33,    -18,    -4,     -32,    -5,     -31,    -61,
];

#pragma warning restore IDE0055

    /// <summary>
    /// 12x64
    /// </summary>
    public static readonly int[][] PackedPSQT = new int[12][];

    /// <summary>
    /// <see cref="Constants.AbsoluteMaxDepth"/> x <see cref="Constants.MaxNumberOfPossibleMovesInAPosition"/>
    /// </summary>
    public static readonly int[][] LMRReductions = new int[Configuration.EngineSettings.MaxDepth][];

    /// <summary>
    /// [0, 4, 136, 276, 424, 580, 744, 916, 1096, 1284, 1480, 1684, 1896, 1896, 1896, 1896, ...]
    /// </summary>
    public static readonly int[] HistoryBonus = new int[Configuration.EngineSettings.MaxDepth];

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

        for (int searchDepth = 1; searchDepth < Configuration.EngineSettings.MaxDepth; ++searchDepth)    // Depth > 0 or we'd be in QSearch
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
