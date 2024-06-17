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

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

internal static readonly short[] MiddleGamePieceValues =
[
        +93, +355, +328, +512, +1096, 0,
        -93, -355, -328, -512, -1096, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +130, +341, +205, +745, +1494, 0,
        -130, -341, -205, -745, -1494, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
         -16,    -21,     -9,     -5,      0,     24,     24,     -8,
         -22,    -20,     -9,      6,     16,     20,     24,      9,
         -18,    -24,     -1,     10,     16,     25,     -6,      1,
         -18,    -18,     -5,     12,     19,     22,     -4,     -1,
         -19,    -16,     -7,      1,     14,     17,     17,      4,
         -17,    -18,    -13,     -6,     -1,     18,     14,    -15,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
          13,      9,      2,    -12,      8,      2,     -6,     -7,
          12,      7,     -0,    -11,     -6,     -6,     -8,     -7,
          25,     17,     -0,    -19,    -15,    -13,      5,      0,
          23,     16,     -1,    -16,    -14,    -10,      3,     -2,
          13,      4,     -3,    -10,     -4,     -4,     -7,     -8,
          15,      9,      6,    -11,     14,      5,     -3,     -4,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -129,    -18,    -45,    -20,     -1,    -10,     -4,    -67,
         -36,    -21,     -3,     18,     21,     25,     -0,      5,
         -29,     -5,      1,     33,     38,     23,     26,      2,
          -7,     11,     24,     44,     43,     46,     32,     24,
          -4,     10,     24,     33,     39,     44,     33,     25,
         -27,     -4,      4,     26,     38,     17,     21,      1,
         -39,    -17,     -5,     17,     19,     19,     -2,      3,
        -142,    -20,    -44,    -14,     -3,     -4,    -14,    -57,
];

internal static readonly short[] EndGameKnightTable =
[
         -37,    -24,     -8,     -3,     -3,    -22,    -20,    -58,
          -6,      2,     -0,     -5,     -5,    -10,     -5,     -7,
          -7,      0,     20,     22,     19,      3,     -4,     -4,
          12,      7,     33,     34,     38,     32,     14,      4,
           9,     11,     33,     37,     38,     28,     18,     10,
         -10,      3,     11,     26,     18,      5,     -6,      0,
         -10,      4,     -8,     -3,     -9,    -15,     -9,    -11,
         -42,    -25,     -5,     -6,     -5,    -21,    -20,    -59,
];

internal static readonly short[] MiddleGameBishopTable =
[
         -10,     19,    -14,    -24,    -21,    -15,    -17,     27,
          14,     -8,      6,    -16,     -1,     -0,     27,      3,
         -11,      2,    -10,      7,     -6,      7,     -0,     19,
         -15,    -10,     -1,     26,     23,    -14,      1,     -2,
         -21,     -4,     -9,     21,     11,    -12,     -5,      4,
           1,      4,      1,     -1,      5,      1,      1,     17,
          18,      3,      7,     -6,     -6,      4,     18,     11,
          12,     25,     -2,    -39,    -24,    -22,      2,      4,
];

internal static readonly short[] EndGameBishopTable =
[
          -7,      7,    -11,     -1,     -8,     -2,     -9,    -27,
          -7,     -9,     -4,     -0,      3,    -13,     -9,    -18,
           4,     11,     12,      8,     18,     10,      4,      6,
           5,      5,     13,     13,     11,     15,      4,     -1,
          -1,      8,     11,     16,     10,     14,      6,     -0,
           2,      0,      6,      7,     13,      7,      1,      2,
         -17,    -12,    -13,      0,     -0,     -7,     -9,    -13,
          -5,    -17,     -8,      2,      0,     -1,    -10,    -15,
];

internal static readonly short[] MiddleGameRookTable =
[
         -10,    -11,     -7,     -0,     10,      5,     10,     -3,
         -34,    -21,    -15,     -9,      1,     10,     23,     -7,
         -36,    -19,    -21,    -10,      4,     12,     53,     24,
         -28,    -20,    -13,     -2,      2,      7,     42,     17,
         -24,    -15,    -10,     -1,     -4,      6,     32,     12,
         -28,    -15,    -16,     -3,      1,     21,     52,     22,
         -32,    -30,    -10,     -1,      4,      8,     32,     -1,
          -8,     -4,     -3,     10,     17,      9,     17,      8,
];

internal static readonly short[] EndGameRookTable =
[
           4,      1,      5,     -3,    -12,     -2,     -6,    -10,
          16,     18,     17,      5,     -3,     -6,     -8,      0,
          15,     11,     12,      6,     -6,     -9,    -21,    -17,
          16,     13,     14,      7,      2,      1,    -12,    -13,
          15,     12,     14,      5,      3,     -3,     -9,     -9,
          14,     14,      5,     -2,     -8,    -12,    -21,    -11,
          19,     21,     12,      1,     -6,     -6,     -9,     -0,
          -1,     -5,      0,     -9,    -18,    -10,    -11,    -18,
];

internal static readonly short[] MiddleGameQueenTable =
[
         -13,     -7,    -12,      4,     -1,    -27,      7,      3,
          -2,    -15,      1,     -2,     -0,      5,     22,     51,
          -9,     -6,    -12,    -12,    -14,      7,     32,     56,
          -8,    -17,    -16,     -4,     -6,     -1,     17,     29,
         -10,    -13,    -15,    -15,     -7,     -3,     14,     26,
          -7,     -4,    -19,    -17,    -11,      3,     19,     39,
         -15,    -25,     -3,      7,      4,      1,      5,     37,
         -12,     -7,     -5,      4,     -0,    -33,    -12,     24,
];

internal static readonly short[] EndGameQueenTable =
[
         -36,    -29,     -9,    -19,    -22,    -24,    -40,     -6,
         -29,    -12,    -23,     -5,     -4,    -18,    -49,    -17,
         -22,     -3,     14,     14,     34,     27,     -5,     -3,
         -18,     12,     18,     25,     42,     48,     48,     26,
          -7,      6,     24,     37,     39,     45,     30,     36,
         -21,    -10,     24,     27,     28,     27,     19,      8,
         -22,     -6,    -17,    -19,    -14,    -13,    -34,     -8,
         -26,    -24,    -13,    -11,    -15,      3,      5,    -15,
];

internal static readonly short[] MiddleGameKingTable =
[
          11,     41,     36,    -62,     21,    -54,     36,     30,
         -21,    -21,    -25,    -58,    -75,    -51,     -9,      2,
         -84,    -68,   -102,   -109,   -116,   -122,    -82,   -102,
        -103,    -87,   -109,   -146,   -146,   -130,   -128,   -156,
         -71,    -62,    -95,   -125,   -147,   -115,   -134,   -151,
         -84,    -46,    -98,   -106,    -92,   -103,    -74,    -93,
          54,    -17,    -29,    -47,    -54,    -36,      6,     11,
          19,     60,     45,    -45,     34,    -43,     50,     42,
];

internal static readonly short[] EndGameKingTable =
[
         -74,    -47,    -25,      1,    -38,     -7,    -41,    -88,
         -19,     19,     27,     39,     45,     33,     10,    -25,
           2,     44,     71,     81,     84,     74,     44,     13,
           5,     56,     90,    118,    115,     92,     66,     26,
          -5,     48,     87,    115,    118,     90,     68,     25,
           3,     42,     72,     82,     79,     69,     43,      9,
         -43,     16,     30,     38,     39,     29,      6,    -29,
         -84,    -55,    -30,     -6,    -34,    -11,    -44,    -93,
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
