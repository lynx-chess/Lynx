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
        +98, +355, +373, +506, +1121, 0,
        -98, -355, -373, -506, -1121, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +123, +428, +362, +747, +1425, 0,
        -123, -428, -362, -747, -1425, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
         -22,    -22,    -14,    -12,     -6,     19,     23,    -13,
         -25,    -26,     -6,      9,     15,     24,     21,      8,
         -21,    -13,      5,     19,     28,     31,      6,     -2,
         -21,     -9,      3,     21,     30,     29,      5,     -2,
         -22,    -21,     -5,      2,     10,     20,     13,      2,
         -22,    -20,    -19,    -15,     -9,     13,     12,    -20,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
          16,     11,      5,     -7,      8,      3,     -5,     -2,
          10,      6,     -3,    -15,    -10,    -10,    -10,     -9,
          27,     17,     -1,    -20,    -16,    -14,      4,      1,
          24,     15,     -3,    -16,    -15,    -12,      2,     -1,
          12,      3,     -6,    -13,     -7,     -8,     -9,    -10,
          18,     11,      9,     -5,     17,      7,     -1,      1,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -141,    -18,    -51,    -27,     -9,    -12,     -7,    -78,
         -41,    -27,     -5,     17,     19,     28,     -7,      1,
         -34,     -4,     12,     49,     51,     32,     26,     -7,
         -13,     19,     35,     49,     49,     50,     40,     13,
         -10,     19,     37,     38,     48,     49,     41,     13,
         -33,     -3,     11,     42,     49,     26,     20,     -8,
         -44,    -19,     -2,     16,     17,     23,     -6,      1,
        -156,    -21,    -48,    -19,     -8,     -3,    -15,    -66,
];

internal static readonly short[] EndGameKnightTable =
[
         -56,    -38,     -5,     -3,     -3,    -25,    -31,    -80,
          -9,      6,     10,      4,      4,     -3,     -7,    -16,
          -9,      7,     26,     27,     22,      6,     -1,     -9,
          11,     12,     37,     39,     43,     35,     16,     -3,
           7,     16,     37,     42,     44,     32,     19,      4,
         -11,     11,     16,     31,     22,      8,     -3,     -5,
         -15,      8,      1,      6,     -0,     -8,     -7,    -20,
         -60,    -35,     -0,     -6,     -5,    -25,    -30,    -79,
];

internal static readonly short[] MiddleGameBishopTable =
[
         -12,     14,     -8,    -19,    -14,     -9,    -19,     15,
           5,      2,      4,    -17,      2,      3,     36,     -5,
          -9,      0,     -9,      3,    -12,      6,     -2,     24,
          -9,    -10,     -4,     22,     19,    -15,      0,     -4,
         -17,     -5,    -13,     19,      7,    -11,     -7,      3,
           2,     -2,      2,     -5,     -0,     -1,      1,     20,
           8,     13,      8,     -5,     -4,      6,     28,      7,
          12,     17,      4,    -34,    -16,    -15,      2,     -3,
];

internal static readonly short[] EndGameBishopTable =
[
           1,     17,     -3,      5,      2,      3,      1,    -19,
           3,     -7,     -6,     -1,     -2,    -14,     -7,    -10,
          13,      8,      4,     -2,      6,      1,     -0,     11,
          13,      0,      3,      0,     -1,      2,     -1,      6,
           7,      3,      1,      4,     -3,      2,     -1,      7,
          10,     -2,     -2,     -4,      1,     -4,     -2,      7,
          -6,     -9,    -16,     -2,     -4,     -8,     -7,     -5,
           3,     -9,     -2,      9,      9,      4,     -1,     -6,
];

internal static readonly short[] MiddleGameRookTable =
[
          -4,    -10,     -5,      1,     10,      8,     11,      3,
         -29,    -17,    -11,     -8,      4,     12,     29,     -1,
         -33,    -22,    -23,    -11,      3,      9,     47,     23,
         -29,    -26,    -23,    -11,     -7,      5,     35,     15,
         -23,    -21,    -17,     -7,     -8,      6,     26,     11,
         -26,    -18,    -18,     -2,      1,     18,     47,     22,
         -26,    -26,     -6,     -1,      6,     10,     35,      5,
          -2,     -3,     -0,     11,     17,     13,     19,     14,
];

internal static readonly short[] EndGameRookTable =
[
           7,      5,      9,      1,     -7,     -1,     -2,     -7,
          17,     19,     19,      9,     -1,     -5,     -9,      0,
          14,     10,     12,      4,     -8,    -12,    -23,    -17,
          15,     11,     12,      4,     -3,     -5,    -17,    -15,
          14,     10,     11,      2,     -2,    -10,    -14,    -12,
          14,     13,      4,     -4,    -10,    -15,    -24,    -11,
          20,     22,     15,      4,     -3,     -5,    -10,     -0,
           2,     -1,      4,     -5,    -14,     -9,     -9,    -14,
];

internal static readonly short[] MiddleGameQueenTable =
[
         -12,    -11,    -12,      5,     -1,    -29,      7,      3,
          -3,     -9,      8,      0,      4,     10,     27,     51,
         -12,     -8,    -10,    -10,    -14,      4,     29,     53,
         -12,    -20,    -18,     -9,     -8,     -4,     11,     24,
         -13,    -16,    -17,    -17,     -7,     -3,     10,     22,
          -9,     -6,    -16,    -11,     -9,      1,     16,     33,
         -16,    -19,      4,     12,      9,      7,     12,     38,
         -10,    -11,     -2,      8,      1,    -34,    -15,     25,
];

internal static readonly short[] EndGameQueenTable =
[
         -23,    -17,     -3,    -11,    -15,    -16,    -32,      8,
         -14,     -9,    -25,     -3,     -4,    -21,    -48,     -5,
         -13,     -4,      7,      2,     21,     19,     -8,      5,
          -9,      8,      7,     11,     23,     33,     41,     31,
          -1,      4,     12,     22,     19,     28,     21,     39,
         -13,    -11,     14,     10,     13,     18,     17,     18,
          -9,     -3,    -21,    -21,    -14,    -17,    -34,      3,
         -14,    -13,    -11,     -6,     -9,     10,     15,     -2,
];

internal static readonly short[] MiddleGameKingTable =
[
           7,     40,     36,    -65,     19,    -55,     35,     24,
         -20,    -14,    -20,    -54,    -66,    -44,     -5,      5,
         -84,    -64,    -98,    -97,   -105,   -114,    -79,    -99,
         -99,    -79,   -104,   -150,   -145,   -122,   -120,   -156,
         -67,    -54,    -92,   -129,   -142,   -109,   -124,   -152,
         -81,    -44,    -91,    -91,    -82,    -92,    -70,    -90,
          59,     -9,    -21,    -41,    -45,    -29,     11,     14,
          16,     59,     46,    -46,     34,    -42,     51,     38,
];

internal static readonly short[] EndGameKingTable =
[
         -77,    -48,    -28,     -1,    -41,     -9,    -43,    -91,
         -23,     15,     24,     37,     43,     30,      8,    -29,
          -2,     40,     69,     81,     83,     73,     39,     10,
           0,     52,     90,    122,    120,     92,     63,     23,
          -9,     43,     88,    119,    123,     91,     65,     22,
          -1,     37,     70,     81,     78,     68,     39,      5,
         -47,     11,     26,     35,     37,     26,      3,    -33,
         -89,    -57,    -34,     -9,    -38,    -14,    -48,    -96,
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
