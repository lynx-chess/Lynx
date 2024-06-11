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
        +100, +356, +374, +506, +1122, 0,
        -100, -356, -374, -506, -1122, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +128, +428, +366, +746, +1424, 0,
        -128, -428, -366, -746, -1424, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
         -24,    -23,    -16,    -12,     -5,     18,     22,    -15,
         -24,    -24,     -5,     12,     19,     26,     23,      9,
         -22,    -15,      5,     19,     27,     31,      3,     -2,
         -22,    -11,      2,     21,     29,     28,      1,     -3,
         -22,    -18,     -3,      5,     14,     23,     16,      4,
         -24,    -21,    -20,    -15,     -8,     12,     11,    -23,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
          15,      9,      3,     -8,      7,      1,     -7,     -4,
          12,      9,     -1,    -13,     -8,     -8,     -8,     -7,
          27,     16,     -1,    -19,    -16,    -14,      3,      1,
          24,     15,     -2,    -16,    -15,    -12,      1,     -2,
          14,      6,     -4,    -12,     -5,     -7,     -7,     -8,
          17,      9,      7,     -6,     15,      5,     -4,     -1,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -141,    -20,    -52,    -29,    -11,    -14,     -8,    -79,
         -42,    -28,     -6,     15,     17,     27,     -8,     -1,
         -32,     -2,     13,     49,     54,     35,     30,     -5,
         -13,     20,     36,     50,     50,     52,     40,     15,
          -9,     20,     38,     38,     48,     50,     40,     14,
         -30,     -0,     13,     43,     51,     29,     23,     -6,
         -45,    -20,     -4,     14,     16,     22,     -7,     -1,
        -156,    -23,    -49,    -21,     -9,     -5,    -16,    -66,
];

internal static readonly short[] EndGameKnightTable =
[
         -55,    -38,     -5,     -4,     -4,    -26,    -31,    -80,
          -8,      5,      7,      1,      1,     -5,     -8,    -16,
          -8,      8,     26,     26,     22,      7,      2,     -8,
          12,     13,     38,     40,     43,     37,     17,     -2,
           9,     18,     37,     43,     44,     33,     21,      5,
         -10,     11,     16,     30,     22,      9,     -1,     -4,
         -14,      9,     -1,      3,     -3,    -10,     -8,    -21,
         -61,    -36,     -1,     -6,     -5,    -25,    -31,    -80,
];

internal static readonly short[] MiddleGameBishopTable =
[
         -13,     14,    -10,    -21,    -15,    -10,    -19,     15,
           5,     -0,      3,    -18,      0,      2,     34,     -5,
          -8,      3,     -8,      3,    -10,      9,      3,     24,
         -10,     -9,     -4,     24,     20,    -14,     -1,     -4,
         -17,     -5,    -12,     19,      7,    -11,     -8,      2,
           2,      2,      3,     -4,      2,      2,      5,     20,
           8,     11,      7,     -6,     -5,      5,     26,      7,
          11,     17,      3,    -36,    -18,    -16,      2,     -3,
];

internal static readonly short[] EndGameBishopTable =
[
           0,     15,     -5,      3,     -0,      1,     -1,    -20,
           1,     -8,     -7,     -3,     -5,    -15,     -8,    -12,
          12,     10,      5,     -1,      7,      2,      2,      9,
          12,      3,      6,      4,      3,      6,      1,      5,
           7,      6,      4,      8,      1,      5,      2,      6,
          10,      0,     -0,     -4,      3,     -3,      0,      6,
          -8,    -10,    -18,     -4,     -7,    -10,     -8,     -7,
           2,    -10,     -4,      7,      8,      2,     -3,     -8,
];

internal static readonly short[] MiddleGameRookTable =
[
          -5,    -11,     -7,     -0,      9,      7,     10,      2,
         -35,    -27,    -15,    -10,      2,     10,     24,     -6,
         -32,    -20,    -21,     -9,      6,     12,     51,     25,
         -27,    -23,    -18,     -6,     -3,      8,     36,     17,
         -22,    -17,    -13,     -3,     -6,      8,     28,     12,
         -24,    -16,    -16,     -0,      5,     22,     50,     25,
         -33,    -36,    -10,     -1,      4,      9,     31,      1,
          -3,     -4,     -2,      9,     16,     12,     18,     13,
];

internal static readonly short[] EndGameRookTable =
[
          10,      8,     12,      4,     -4,      2,      0,     -4,
           7,     10,      9,     -0,     -9,    -11,    -14,     -6,
          16,     12,     14,      6,     -5,     -8,    -19,    -15,
          17,     14,     14,      7,      1,     -0,    -12,    -13,
          16,     13,     14,      5,      3,     -5,     -9,     -9,
          15,     15,      6,     -2,     -8,    -12,    -20,     -9,
           8,     11,      4,     -4,    -10,    -12,    -17,    -10,
           5,      2,      7,     -2,    -12,     -6,     -7,    -12,
];

internal static readonly short[] MiddleGameQueenTable =
[
         -12,    -12,    -13,      4,     -2,    -29,      7,      2,
          -4,    -11,      6,     -1,      2,      9,     26,     49,
         -10,     -6,     -9,     -9,    -11,      8,     33,     54,
         -13,    -19,    -17,     -7,     -8,     -3,     11,     24,
         -13,    -16,    -17,    -16,     -8,     -4,     10,     21,
          -8,     -4,    -14,    -11,     -6,      5,     20,     35,
         -17,    -21,      2,     10,      7,      5,      9,     36,
         -10,    -12,     -3,      6,      0,    -35,    -12,     25,
];

internal static readonly short[] EndGameQueenTable =
[
         -24,    -17,     -4,    -13,    -17,    -16,    -33,      7,
         -14,     -9,    -26,     -4,     -5,    -21,    -49,     -6,
         -15,     -5,      6,      1,     21,     18,     -7,      3,
          -9,      9,      8,     13,     26,     36,     44,     31,
          -1,      5,     14,     24,     22,     31,     24,     41,
         -14,    -11,     14,     10,     13,     18,     18,     17,
          -8,     -2,    -21,    -21,    -14,    -17,    -34,      4,
         -15,    -14,    -11,     -7,    -10,     10,     11,     -4,
];

internal static readonly short[] MiddleGameKingTable =
[
           6,     39,     35,    -64,     18,    -54,     35,     24,
         -22,    -13,    -18,    -52,    -64,    -44,     -5,      4,
         -84,    -60,    -95,    -96,   -104,   -112,    -76,    -99,
        -100,    -83,   -106,   -152,   -147,   -125,   -126,   -157,
         -70,    -58,    -95,   -130,   -144,   -111,   -129,   -153,
         -82,    -39,    -88,    -92,    -81,    -91,    -68,    -89,
          59,     -8,    -20,    -40,    -44,    -29,     11,     13,
          17,     59,     44,    -47,     32,    -43,     50,     38,
];

internal static readonly short[] EndGameKingTable =
[
         -72,    -45,    -25,      4,    -36,     -4,    -40,    -85,
         -25,     12,     21,     33,     39,     27,      5,    -32,
          -6,     37,     66,     77,     80,     70,     37,      6,
          -3,     50,     88,    120,    118,     91,     62,     20,
         -12,     41,     85,    116,    120,     89,     64,     19,
          -5,     34,     66,     78,     75,     65,     36,      1,
         -50,      8,     23,     32,     33,     23,     -0,    -35,
         -83,    -54,    -31,     -3,    -32,     -8,    -44,    -90,
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
