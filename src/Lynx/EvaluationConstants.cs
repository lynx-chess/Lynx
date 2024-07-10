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
        +49, +151, +141, +211, +491, 0,
        -49, -151, -141, -211, -491, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +104, +318, +344, +632, +1228, 0,
        -104, -318, -344, -632, -1228, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
         -12,    -22,    -14,    -15,    -12,     16,     19,      1,
         -14,    -20,    -11,      3,     11,     21,     16,     15,
          -8,     -6,      1,     10,     16,     27,     14,      8,
          -9,     -2,      2,     11,     15,     26,     11,      5,
         -15,    -13,    -16,     -8,     -3,     20,     10,      2,
         -16,    -18,    -24,    -17,     -9,     14,      6,    -15,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
           6,      9,     -0,     -2,      8,      5,     -8,    -14,
           4,      4,     -2,    -14,     -7,     -3,    -12,    -14,
          19,     14,      1,    -17,    -10,     -7,      1,     -1,
          21,     15,     -0,    -13,     -7,     -9,      3,     -0,
          10,      4,     -0,    -10,     -2,     -7,    -11,    -10,
          15,     12,     13,    -11,     18,      9,     -1,      0,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
         -98,    -10,    -29,    -31,    -13,    -19,     -2,    -49,
         -26,    -37,     -7,     11,     14,      9,     -4,     -7,
         -20,      3,     12,     43,     41,     25,     19,      1,
         -10,     20,     27,     43,     38,     42,     40,     12,
          -3,     12,     33,     23,     36,     36,     37,     13,
         -26,     -4,      7,     35,     31,     17,      7,     -1,
         -32,     -8,      1,      7,      1,      9,    -12,     -3,
        -100,    -17,    -37,    -13,     -5,    -10,    -16,    -52,
];

internal static readonly short[] EndGameKnightTable =
[
         -37,    -31,    -10,      4,    -11,    -25,    -22,    -56,
          -8,     13,     -5,     -1,     -0,     -3,    -15,     -8,
         -14,      5,     19,     27,     18,      7,      5,     -2,
          10,     11,     37,     35,     41,     36,     25,      5,
           4,      9,     32,     40,     41,     35,     22,      8,
         -10,     -1,      7,     25,     20,     -4,     -0,    -11,
         -13,     -5,     -8,     -2,     -4,    -12,     -6,    -23,
         -26,    -30,     -6,     -8,    -14,    -28,    -31,    -50,
];

internal static readonly short[] MiddleGameBishopTable =
[
          -2,     14,     -9,    -20,    -20,     -3,     -6,     -3,
          -1,     -1,      5,    -17,     -2,      1,     25,      1,
          -9,      2,     -6,      8,     -9,      3,     -4,     16,
           1,     -2,      2,     24,     24,     -5,      2,      1,
          -8,      4,     -7,     19,      4,     -0,     -1,      9,
           3,      0,      0,     -1,     -0,    -10,      2,      9,
           4,      7,      8,     -4,    -10,     -4,      9,      4,
          11,     13,      2,    -35,    -13,    -11,      5,    -16,
];

internal static readonly short[] EndGameBishopTable =
[
          14,      8,     -5,      8,      5,     -1,      4,     -2,
          18,     -8,     -5,     -5,     -4,    -14,    -13,      3,
          10,      4,      1,     -5,      0,     -4,     -4,      8,
           8,     -1,     -3,     -2,     -0,     -2,     -5,     10,
           6,     -1,     -2,      1,      1,    -10,     -1,      2,
          13,     -3,     -4,     -9,     -7,     -6,    -11,      9,
           3,     -6,    -16,     -6,    -11,    -10,    -11,     -2,
          21,     -2,      1,     10,      6,      5,      4,     16,
];

internal static readonly short[] MiddleGameRookTable =
[
         -12,     -9,     -8,     -4,     -1,     -2,      7,      2,
         -27,    -15,    -11,     -5,      1,      7,     19,      1,
         -27,    -17,    -11,      1,      3,      8,     26,     11,
         -26,    -15,    -11,      8,      4,     15,     36,     10,
         -16,     -9,     -7,     -3,      6,      3,     40,     16,
         -13,     -4,     -6,      2,     11,     20,     33,     12,
         -18,    -21,    -12,     -4,     -0,      2,     12,     -2,
          -7,     -7,     -6,      1,      6,     -1,     14,     -1,
];

internal static readonly short[] EndGameRookTable =
[
           2,     -2,      3,     -2,     -7,     -4,     -8,    -12,
          18,     12,     10,      1,     -7,    -10,     -8,      1,
          18,     13,      9,     -2,     -7,     -2,     -6,     -3,
          17,     11,     12,      2,      2,      5,     -5,     -1,
          15,     10,     11,      3,     -1,      8,     -7,     -9,
          15,      7,      5,     -7,    -14,    -13,    -13,     -4,
          14,     14,     10,     -5,     -8,    -14,    -13,      2,
          -3,     -4,      1,     -8,    -14,     -7,    -10,    -12,
];

internal static readonly short[] MiddleGameQueenTable =
[
          -1,     -4,     -8,      3,     -0,    -22,    -16,     19,
          -3,     -2,      5,     -3,      3,     11,     15,     34,
         -13,     -9,     -2,     -9,     -3,      2,     18,     25,
         -11,    -15,    -14,     -2,     -2,      0,     14,     13,
          -6,    -13,    -11,    -11,     -8,      0,      9,     10,
          -8,     -5,     -3,     -5,     -3,      2,      9,     11,
         -11,    -11,      7,     12,      6,      5,      8,     28,
           0,     -7,      1,      5,      4,    -21,    -23,      8,
];

internal static readonly short[] EndGameQueenTable =
[
          -9,     -8,     -7,    -15,    -16,    -22,      8,      6,
          -2,     -5,    -22,    -12,    -16,    -31,    -36,     11,
           6,      6,      1,      7,      6,     19,      9,     29,
           2,      8,     12,     10,     18,     25,     31,     47,
          -6,      7,      8,     16,     12,     19,     24,     40,
          -2,     -6,     -5,     -8,     -6,      3,      4,     37,
          10,      0,    -27,    -35,    -24,    -36,    -39,     11,
          -7,     -1,    -12,    -17,    -25,    -30,     11,     25,
];

internal static readonly short[] MiddleGameKingTable =
[
          -8,     -4,    -17,    -59,    -20,    -76,    -11,    -23,
           9,    -11,    -22,    -62,    -67,    -55,    -24,    -33,
         -13,    -32,    -61,    -70,    -92,    -90,    -65,    -78,
         -44,      4,    -38,    -92,    -78,    -64,    -45,    -82,
          -9,     -0,    -31,    -51,    -74,    -59,    -48,    -65,
         -24,     -8,    -18,    -33,    -36,    -60,    -29,    -38,
          53,     19,     -3,    -13,    -19,    -11,     19,     16,
          14,     50,     36,    -31,     37,    -24,     38,     26,
];

internal static readonly short[] EndGameKingTable =
[
         -46,    -30,    -17,     -9,    -37,    -12,    -29,    -71,
         -21,      8,      9,     18,     23,     16,      3,    -18,
          -8,     20,     43,     50,     56,     48,     22,      2,
           2,     28,     60,     88,     87,     59,     34,     10,
          -6,     28,     58,     85,     86,     55,     30,      2,
          -3,     17,     38,     44,     45,     38,      9,    -15,
         -36,     -3,     10,     14,     13,      6,    -11,    -34,
         -56,    -40,    -28,    -13,    -49,    -22,    -42,    -82,
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
