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
        +100, +358, +372, +507, +1121, 0,
        -100, -358, -372, -507, -1121, 0
];

internal static readonly short[] EndGamePieceValues =
[
        +129, +427, +368, +746, +1424, 0,
        -129, -427, -368, -746, -1424, 0
];

internal static readonly short[] MiddleGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
         -24,    -23,    -15,    -12,     -5,     19,     22,    -15,
         -24,    -24,     -5,     12,     19,     25,     23,     10,
         -22,    -16,      4,     18,     26,     30,      1,     -2,
         -22,    -12,      2,     20,     28,     28,     -0,     -3,
         -21,    -18,     -3,      5,     14,     23,     15,      4,
         -24,    -20,    -20,    -15,     -8,     12,     11,    -22,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] EndGamePawnTable =
[
           0,      0,      0,      0,      0,      0,      0,      0,
          15,      9,      4,     -7,      8,      2,     -6,     -5,
          12,      9,     -1,    -14,     -8,     -9,     -8,     -8,
          26,     16,     -1,    -19,    -16,    -15,      3,      0,
          24,     15,     -2,    -16,    -15,    -12,      1,     -2,
          13,      6,     -4,    -12,     -5,     -7,     -7,     -8,
          17,      9,      8,     -6,     16,      6,     -3,     -2,
           0,      0,      0,      0,      0,      0,      0,      0,
];

internal static readonly short[] MiddleGameKnightTable =
[
        -143,    -21,    -53,    -30,    -12,    -14,     -9,    -80,
         -43,    -28,     -7,     14,     17,     27,     -9,     -1,
         -33,     -2,     14,     50,     54,     36,     30,     -5,
         -13,     22,     38,     52,     51,     53,     42,     16,
          -9,     21,     39,     39,     49,     52,     42,     14,
         -31,     -1,     14,     43,     52,     29,     23,     -6,
         -45,    -21,     -4,     13,     15,     22,     -7,     -1,
        -157,    -24,    -50,    -22,    -10,     -6,    -17,    -67,
];

internal static readonly short[] EndGameKnightTable =
[
         -55,    -38,     -6,     -4,     -4,    -26,    -31,    -80,
          -9,      5,      7,      2,      2,     -4,     -8,    -16,
          -8,      8,     25,     25,     22,      7,      2,     -7,
          11,     12,     37,     39,     43,     36,     17,     -2,
           9,     17,     37,     42,     43,     32,     22,      5,
         -10,     11,     16,     30,     22,      9,     -1,     -4,
         -14,      8,     -1,      4,     -2,    -10,     -9,    -20,
         -60,    -36,     -1,     -6,     -5,    -25,    -30,    -80,
];

internal static readonly short[] MiddleGameBishopTable =
[
         -13,     12,    -11,    -22,    -16,    -11,    -20,     14,
           4,     -1,      2,    -19,     -1,      2,     33,     -6,
          -9,      3,     -8,      4,     -9,      9,      3,     25,
         -10,     -8,     -2,     25,     22,    -12,      1,     -3,
         -17,     -3,    -11,     21,      9,     -9,     -7,      3,
           2,      1,      3,     -4,      3,      2,      5,     21,
           7,     10,      6,     -7,     -6,      5,     26,      5,
          10,     16,      2,    -36,    -19,    -17,      2,     -3,
];

internal static readonly short[] EndGameBishopTable =
[
          -1,     15,     -5,      4,     -0,      1,     -1,    -21,
           1,     -8,     -7,     -2,     -4,    -15,     -8,    -13,
          12,     10,      5,     -1,      7,      2,      3,      9,
          12,      3,      5,      4,      3,      6,      1,      5,
           7,      6,      3,      8,      1,      6,      2,      6,
          10,      0,     -0,     -4,      3,     -3,      0,      6,
          -8,    -10,    -17,     -3,     -6,    -10,     -8,     -7,
           2,    -10,     -4,      7,      8,      3,     -3,     -8,
];

internal static readonly short[] MiddleGameRookTable =
[
          -6,    -12,     -7,     -1,      8,      7,     10,      1,
         -30,    -19,    -13,    -10,      2,     10,     26,     -3,
         -33,    -21,    -22,    -11,      5,     12,     52,     25,
         -28,    -24,    -19,     -8,     -4,      8,     37,     16,
         -23,    -19,    -14,     -5,     -7,      8,     28,     12,
         -25,    -17,    -17,     -2,      4,     22,     51,     24,
         -28,    -28,     -8,     -3,      4,      8,     32,      3,
          -4,     -5,     -3,      8,     15,     11,     17,     12,
];

internal static readonly short[] EndGameRookTable =
[
           7,      4,      8,     -0,     -8,     -1,     -3,     -7,
          17,     18,     18,      8,     -1,     -5,     -8,      1,
          13,     10,     11,      4,     -8,    -11,    -23,    -18,
          15,     11,     12,      5,     -2,     -3,    -15,    -15,
          14,     11,     12,      3,     -0,     -8,    -12,    -11,
          13,     12,      3,     -4,    -11,    -15,    -23,    -12,
          20,     21,     14,      4,     -3,     -5,     -9,      0,
           2,     -1,      3,     -6,    -15,     -9,     -9,    -15,
];

internal static readonly short[] MiddleGameQueenTable =
[
         -14,    -13,    -14,      4,     -3,    -31,      5,      1,
          -4,    -11,      6,     -1,      2,      9,     26,     48,
         -11,     -6,     -9,     -9,    -12,      9,     34,     54,
         -13,    -19,    -17,     -7,     -8,     -3,     13,     25,
         -13,    -15,    -16,    -16,     -7,     -3,     12,     22,
          -8,     -4,    -14,    -11,     -6,      5,     21,     35,
         -18,    -21,      2,     10,      7,      6,     10,     36,
         -11,    -13,     -4,      6,     -0,    -35,    -14,     24,
];

internal static readonly short[] EndGameQueenTable =
[
         -23,    -16,     -4,    -11,    -17,    -15,    -31,      8,
         -14,     -9,    -25,     -3,     -4,    -20,    -49,     -5,
         -15,     -5,      6,      1,     21,     18,     -8,      3,
          -9,      9,      7,     12,     25,     35,     42,     29,
          -2,      4,     13,     23,     21,     31,     22,     39,
         -15,    -11,     13,     10,     12,     17,     16,     17,
          -9,     -3,    -20,    -21,    -14,    -16,    -34,      3,
         -14,    -13,    -10,     -6,     -9,     11,     14,     -3,
];

internal static readonly short[] MiddleGameKingTable =
[
           5,     39,     35,    -65,     18,    -55,     35,     24,
         -23,    -16,    -20,    -55,    -67,    -45,     -6,      3,
         -81,    -60,    -95,    -96,   -104,   -112,    -76,    -99,
         -96,    -79,   -104,   -151,   -145,   -122,   -121,   -153,
         -64,    -53,    -92,   -128,   -143,   -107,   -124,   -149,
         -79,    -38,    -87,    -91,    -80,    -90,    -68,    -89,
          58,     -9,    -22,    -42,    -46,    -30,     10,     12,
          17,     60,     45,    -47,     33,    -43,     50,     38,
];

internal static readonly short[] EndGameKingTable =
[
         -76,    -48,    -28,     -1,    -41,     -9,    -43,    -89,
         -21,     15,     24,     37,     43,     30,      8,    -28,
          -2,     41,     70,     81,     84,     74,     42,     11,
           1,     53,     91,    123,    121,     94,     65,     24,
          -9,     45,     89,    119,    123,     92,     67,     23,
          -0,     38,     70,     81,     79,     69,     41,      7,
         -46,     11,     26,     35,     37,     26,      3,    -31,
         -88,    -57,    -34,     -8,    -38,    -14,    -48,    -94,
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
