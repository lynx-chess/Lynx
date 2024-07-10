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
        +52, +164, +155, +226, +524, 0,
        -52, -164, -155, -226, -524, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +105, +333, +357, +654, +1272, 0,
        -105, -333, -357, -654, -1272, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
         -12,    -22,    -14,    -14,    -12,     16,     19,      1,
         -14,    -20,    -12,      3,     11,     21,     15,     14,
          -8,     -7,      1,     10,     16,     27,     14,      8,
          -9,     -3,      2,     11,     16,     27,     11,      5,
         -15,    -13,    -16,     -8,     -2,     21,      9,      2,
         -16,    -18,    -24,    -16,     -9,     14,      6,    -15,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
           6,      9,      0,     -1,      8,      5,     -9,    -14,
           4,      4,     -2,    -14,     -7,     -2,    -12,    -15,
          19,     14,      1,    -17,    -10,     -7,      0,     -1,
          20,     15,     -0,    -13,     -7,     -9,      3,     -0,
          10,      4,      0,     -9,     -1,     -8,    -11,    -10,
          16,     12,     14,    -10,     18,      9,     -1,      1,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -106,    -10,    -28,    -31,    -13,    -19,     -2,    -51,
         -26,    -39,     -6,     11,     14,      8,     -5,     -8,
         -20,      4,     12,     44,     41,     25,     19,      0,
          -9,     20,     27,     43,     38,     42,     40,     13,
          -3,     12,     33,     23,     35,     37,     37,     14,
         -26,     -3,      7,     35,     31,     17,      8,     -2,
         -33,     -7,      2,      7,      1,      8,    -13,     -5,
         -97,    -17,    -36,    -12,     -5,    -10,    -16,    -55,
];

internal static readonly short[] EndGameKnightTable =
[
         -41,    -29,    -10,      5,    -11,    -24,    -22,    -61,
          -9,     16,     -5,     -1,     -1,     -4,    -15,     -8,
         -12,      6,     19,     26,     17,      6,      5,     -2,
          12,     13,     36,     34,     40,     36,     26,      6,
           6,     11,     33,     40,     40,     34,     22,      8,
          -9,     -0,      7,     24,     19,     -5,     -0,     -8,
         -12,     -4,     -9,     -3,     -3,    -11,     -4,    -21,
         -30,    -28,     -6,     -8,    -14,    -28,    -31,    -53,
];

internal static readonly short[] MiddleGameBishopTable =
[
          -1,     15,    -10,    -21,    -20,     -3,     -6,     -2,
           0,     -1,      5,    -18,     -2,     -1,     24,      2,
          -8,      2,     -7,      8,    -10,      3,     -5,     16,
           2,     -3,      2,     24,     24,     -5,      2,      2,
          -6,      4,     -6,     19,      4,     -1,     -1,     10,
           4,     -1,     -0,     -2,     -1,    -10,      2,     10,
           6,      7,      7,     -5,    -10,     -5,      8,      5,
          11,     12,      2,    -36,    -14,    -11,      5,    -14,
];

internal static readonly short[] EndGameBishopTable =
[
          12,      6,     -6,      7,      3,     -2,      3,     -4,
          17,     -8,     -4,     -5,     -4,    -13,    -13,      4,
           9,      5,      2,     -4,      0,     -4,     -3,      6,
           7,      0,     -2,     -2,      1,     -0,     -4,      9,
           8,      0,     -0,      1,      2,     -8,     -0,      3,
          13,     -2,     -2,     -8,     -6,     -5,    -10,      8,
           4,     -5,    -16,     -6,    -11,     -9,    -11,     -3,
          19,     -4,      1,     10,      5,      4,      3,     13,
];

internal static readonly short[] MiddleGameRookTable =
[
         -12,     -9,     -7,     -4,     -0,     -2,      7,      3,
         -26,    -15,    -11,     -5,      1,      5,     17,      0,
         -26,    -17,    -10,      2,      3,      8,     24,     10,
         -24,    -14,     -9,     10,      7,     16,     36,     10,
         -15,     -8,     -5,     -2,      8,      3,     39,     15,
         -13,     -5,     -6,      2,     11,     18,     30,     10,
         -18,    -19,    -13,     -4,     -1,      1,     10,     -3,
          -7,     -7,     -6,      1,      6,     -1,     14,     -1,
];

internal static readonly short[] EndGameRookTable =
[
          -0,     -4,      0,     -5,    -10,     -8,    -12,    -15,
          20,     13,     11,      1,     -8,    -10,     -7,      2,
          20,     14,     11,     -1,     -7,     -2,     -5,     -3,
          18,     12,     14,      3,      2,      6,     -5,     -0,
          16,     11,     13,      3,      1,     10,     -6,     -8,
          16,      8,      6,     -6,    -14,    -12,    -11,     -3,
          16,     15,     11,     -4,     -8,    -14,    -10,      4,
          -5,     -7,     -1,    -11,    -17,    -11,    -13,    -15,
];

internal static readonly short[] MiddleGameQueenTable =
[
           0,     -4,     -9,      2,     -1,    -23,    -17,     22,
          -3,     -1,      3,     -5,      1,      9,     15,     28,
         -14,    -10,     -3,    -10,     -4,      1,     17,     22,
         -12,    -16,    -14,      2,      1,      1,     14,     13,
          -6,    -13,    -11,     -8,     -5,      1,      7,     10,
          -9,     -6,     -3,     -6,     -3,      2,      8,     10,
          -9,     -7,      6,     11,      5,      5,      6,     28,
           4,     -7,      2,      4,      4,    -20,    -22,     14,
];

internal static readonly short[] EndGameQueenTable =
[
         -13,     -9,     -9,    -18,    -20,    -26,      7,      6,
           0,     -3,    -18,    -10,    -15,    -30,    -43,     13,
           8,     10,      3,     12,      9,     19,      7,     27,
           5,     12,     16,     12,     18,     25,     30,     46,
          -2,     10,     12,     17,     14,     20,     25,     41,
           2,     -3,     -3,     -5,     -6,      4,      3,     33,
          11,     -2,    -24,    -34,    -23,    -36,    -43,      3,
         -10,     -2,    -14,    -20,    -30,    -38,      8,     22,
];

internal static readonly short[] MiddleGameKingTable =
[
          -8,     -5,    -17,    -62,    -19,    -76,    -10,    -22,
           9,    -12,    -26,    -65,    -69,    -56,    -24,    -31,
         -19,    -39,    -66,    -75,    -94,    -91,    -66,    -80,
         -59,     -6,    -48,    -89,    -76,    -68,    -51,    -87,
         -23,     -6,    -37,    -56,    -77,    -62,    -55,    -63,
         -26,    -14,    -22,    -37,    -39,    -61,    -30,    -38,
          52,     17,     -6,    -16,    -21,    -12,     19,     16,
          15,     50,     35,    -33,     36,    -24,     38,     27,
];

internal static readonly short[] EndGameKingTable =
[
         -44,    -30,    -18,    -10,    -39,    -13,    -29,    -69,
         -20,      7,      9,     19,     23,     16,      2,    -18,
          -6,     22,     45,     51,     56,     49,     22,      3,
           8,     31,     63,     88,     87,     60,     36,     12,
          -1,     31,     60,     87,     87,     56,     32,      3,
          -3,     19,     39,     44,     45,     38,      9,    -16,
         -38,     -4,      9,     13,     12,      5,    -13,    -35,
         -55,    -41,    -30,    -14,    -52,    -23,    -44,    -82,
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
