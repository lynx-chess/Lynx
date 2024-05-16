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
        +99, +398, +365, +491, +1114, 0,
        -99, -398, -365, -491, -1114, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +166, +416, +381, +738, +1369, 0,
        -166, -416, -381, -738, -1369, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -25,    -23,    -12,    -12,    -1,     32,     30,     -13,
        -27,    -26,    -4,     13,     21,     28,     22,     7,
        -26,    -16,    3,      18,     25,     29,     0,      -6,
        -27,    -12,    1,      19,     27,     27,     -1,     -7,
        -25,    -19,    -3,     3,      14,     24,     15,     1,
        -26,    -20,    -18,    -18,    -5,     25,     19,     -20,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        11,     18,     7,      43,     16,     -6,     -2,     -19,
        9,      18,     1,      2,      -10,    -14,    -5,     -21,
        20,     20,     -6,     -21,    -25,    -25,    3,      -13,
        18,     19,     -7,     -17,    -22,    -21,    1,      -17,
        10,     12,     -4,     4,      -4,     -13,    -4,     -22,
        13,     15,     7,      51,     29,     -5,     0,      -17,
        0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -144,   -21,    -50,    -30,    -12,    -21,    -10,    -96,
        -45,    -29,    -3,     16,     18,     24,     -16,    -19,
        -29,    1,      21,     56,     60,     40,     32,     -6,
        -11,    24,     42,     59,     57,     57,     43,     16,
        -8,     24,     44,     46,     56,     56,     43,     15,
        -26,    3,      20,     48,     58,     33,     26,     -7,
        -45,    -21,    1,      15,     17,     19,     -14,    -19,
        -155,   -22,    -47,    -19,    -10,    -11,    -17,    -90,
];

internal static readonly short[] EndGameKnightTable =
[
        -73,    -60,    -17,    -9,     -10,    -26,    -50,    -105,
        -23,    4,      10,     11,     11,     7,      -6,     -18,
        -13,    15,     39,     37,     36,     23,     12,     -11,
        8,      20,     48,     49,     53,     48,     27,     -4,
        7,      23,     46,     52,     53,     44,     31,     2,
        -18,    19,     31,     42,     33,     23,     8,      -9,
        -29,    7,      3,      13,     6,      0,      -9,     -27,
        -90,    -58,    -15,    -15,    -11,    -30,    -46,    -100,
];

internal static readonly short[] MiddleGameBishopTable =
[
        -17,    20,     2,      -15,    -11,    -14,    -16,    1,
        8,      4,      6,      -17,    3,      1,      28,     -12,
        -5,     5,      -3,     3,      -7,     13,     4,      27,
        -7,     -6,     -6,     23,     19,     -18,    2,      -2,
        -15,    -1,     -13,    19,     6,      -14,    -6,     4,
        6,      4,      8,      -4,     6,      7,      6,      23,
        10,     15,     10,     -5,     -2,     4,      21,     -3,
        6,      19,     15,     -30,    -15,    -18,    5,      -14,
];

internal static readonly short[] EndGameBishopTable =
[
        -19,    3,      -20,    1,      -4,     7,      -5,     -34,
        -10,    -8,     -1,     5,      7,      -4,     1,      -19,
        11,     17,     10,     9,      19,     9,      10,     11,
        12,     11,     11,     -1,     -6,     11,     9,      5,
        8,      14,     11,     2,      -7,     12,     10,     7,
        10,     8,      4,      6,      11,     4,      8,      7,
        -19,    -11,    -12,    4,      5,      2,      1,      -11,
        -13,    -17,    -15,    8,      7,      8,      -7,     -22,
];

internal static readonly short[] MiddleGameRookTable =
[
        -4,     -10,    -3,     5,      16,     6,      7,      -3,
        -26,    -18,    -14,    -12,    0,      2,      14,     -4,
        -27,    -20,    -23,    -13,    2,      9,      50,     28,
        -19,    -20,    -17,    -6,     -4,     8,      39,     23,
        -14,    -16,    -12,    -4,     -7,     6,      29,     18,
        -19,    -15,    -18,    -3,     2,      17,     47,     28,
        -25,    -28,    -9,     -5,     1,      1,      20,     3,
        -2,     -3,     2,      14,     23,     11,     16,     10,
];

internal static readonly short[] EndGameRookTable =
[
        -2,     0,      4,      -5,     -13,    -1,     -2,     -7,
        13,     21,     20,     9,      1,      4,      5,      1,
        10,     13,     15,     8,      -2,     -5,     -16,    -16,
        9,      7,      13,     7,      -1,     -1,     -19,    -19,
        9,      7,      12,     6,      2,      -4,     -14,    -14,
        9,      15,     8,      2,      -6,     -8,     -18,    -13,
        17,     26,     16,     6,      -1,     3,      0,      0,
        -4,     -5,     -1,     -8,     -19,    -8,     -12,    -17,
];

internal static readonly short[] MiddleGameQueenTable =
[
        -11,    -7,     -3,     13,     6,      -30,    3,      3,
        -1,     -11,    7,      -3,     2,      5,      23,     53,
        -8,     -7,     -10,    -11,    -14,    6,      32,     56,
        -11,    -20,    -21,    -10,    -12,    -9,     9,      24,
        -10,    -16,    -21,    -19,    -11,    -8,     9,      22,
        -6,     -4,     -16,    -13,    -7,     3,      18,     38,
        -14,    -21,    3,      10,     7,      2,      7,      39,
        -9,     -7,     8,      16,     9,      -33,    -8,     29,
];

internal static readonly short[] EndGameQueenTable =
[
        -30,    -24,    -15,    -15,    -23,    -4,     -26,    5,
        -23,    -7,     -24,    0,      0,      -11,    -41,    -15,
        -18,    -1,     12,     8,      30,     25,     -1,     1,
        -17,    8,      12,     20,     31,     42,     44,     27,
        -12,    2,      19,     31,     24,     38,     21,     34,
        -17,    -8,     19,     18,     20,     24,     22,     12,
        -19,    -3,     -20,    -17,    -11,    -9,     -29,    -4,
        -19,    -22,    -23,    -10,    -17,    10,     3,      -12,
];

internal static readonly short[] MiddleGameKingTable =
[
        18,     47,     27,     -71,    11,     -60,    36,     43,
        -24,    -19,    -33,    -70,    -81,    -58,    -13,    14,
        -91,    -71,    -106,   -108,   -121,   -128,   -87,    -100,
        -104,   -101,   -120,   -149,   -149,   -143,   -141,   -162,
        -57,    -69,    -103,   -126,   -152,   -129,   -138,   -153,
        -79,    -38,    -91,    -101,   -92,    -100,   -72,    -87,
        65,     -9,     -34,    -58,    -63,    -43,    5,      24,
        29,     76,     40,     -54,    25,     -47,    52,     60,
];

internal static readonly short[] EndGameKingTable =
[
        -76,    -52,    -31,    -13,    -46,    -13,    -41,    -92,
        -9,     13,     19,     29,     37,     28,     11,     -24,
        20,     46,     60,     69,     72,     66,     47,     27,
        24,     63,     83,     96,     94,     88,     76,     49,
        11,     55,     82,     94,     98,     86,     79,     48,
        18,     45,     63,     71,     70,     62,     48,     24,
        -35,    16,     28,     34,     36,     29,     11,     -22,
        -71,    -54,    -26,    -5,     -33,    -9,     -39,    -91,
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
