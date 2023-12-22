/*
 * PSQT based on https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
*/

using Lynx.Model;

namespace Lynx;

public static class EvaluationConstants
{
    /// <summary>
    /// 21454 games, 16+0.16, UHO_XXL_+0.90_+1.19.epd
    /// Retained (W,D,L) = (486074, 1108791, 487345) positions.
    /// </summary>
    public const int EvalNormalizationCoefficient = 138;

    public static readonly double[] As = [-25.59900221, 175.23377472, -145.09355540, 133.49051930];

    public static readonly double[] Bs = [-14.14613328, 84.98205725, -101.16332276, 120.88906952];

    public static readonly int[] GamePhaseByPiece =
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

public static readonly int[] MiddleGamePieceValues =
[
        +103, +387, +386, +492, +1125, 0,
        -103, -387, -386, -492, -1125, 0
];

public static readonly int[] EndGamePieceValues =
[
        +149, +487, +460, +852, +1559, 0,
        -149, -487, -460, -852, -1559, 0
];

public static readonly int[] MiddleGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        -26,    -22,    -14,    -13,    -5,     27,     31,     -15,
        -25,    -24,    -5,     10,     18,     27,     24,     9,
        -24,    -13,    6,      20,     29,     33,     6,      -5,
        -24,    -9,     3,      21,     29,     29,     4,      -6,
        -22,    -19,    -3,     3,      14,     23,     17,     3,
        -26,    -20,    -19,    -18,    -8,     21,     20,     -22,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] EndGamePawnTable =
[
        0,      0,      0,      0,      0,      0,      0,      0,
        13,     12,     7,      -13,    7,      2,      -3,     -11,
        12,     13,     1,      -14,    -7,     -5,     -2,     -11,
        29,     21,     0,      -22,    -17,    -12,    8,      0,
        25,     20,     -1,     -16,    -15,    -8,     6,      -3,
        13,     11,     -3,     -12,    -2,     -2,     -1,     -11,
        16,     13,     9,      -12,    17,     6,      -1,     -8,
        0,      0,      0,      0,      0,      0,      0,      0,
];

public static readonly int[] MiddleGameKnightTable =
[
        -154,   -24,    -50,    -32,    -13,    -19,    -10,    -99,
        -46,    -27,    -4,     12,     16,     24,     -14,    -18,
        -27,    1,      19,     58,     60,     41,     35,     0,
        -10,    27,     44,     61,     61,     62,     48,     18,
        -7,     26,     46,     49,     59,     60,     49,     18,
        -24,    3,      19,     49,     59,     34,     28,     -2,
        -48,    -18,    -1,     12,     15,     17,     -12,    -18,
        -170,   -25,    -48,    -20,    -11,    -11,    -18,    -92,
];

public static readonly int[] EndGameKnightTable =
[
        -72,    -62,    -12,    -12,    -9,     -29,    -57,    -93,
        -22,    -2,     15,     11,     11,     8,      -13,    -22,
        -15,    16,     38,     38,     37,     17,     9,      -16,
        6,      19,     52,     52,     55,     50,     24,     -8,
        4,      27,     51,     55,     57,     46,     30,     -1,
        -19,    19,     27,     46,     36,     19,     6,      -12,
        -28,    2,      5,      14,     6,      1,      -15,    -25,
        -77,    -61,    -8,     -16,    -10,    -26,    -55,    -93,
];

public static readonly int[] MiddleGameBishopTable =
[
        -27,    9,      -10,    -20,    -18,    -22,    -30,    -13,
        -3,     5,      9,      -16,    7,      3,      28,     -18,
        -14,    8,      3,      11,     1,      20,     9,      22,
        -13,    -5,     3,      34,     30,     -9,     2,      -8,
        -22,    0,      -6,     29,     18,     -4,     -6,     -2,
        -3,     9,      14,     3,      15,     13,     11,     17,
        -2,     16,     14,     -4,     3,      8,      21,     -8,
        -1,     14,     4,      -34,    -20,    -26,    -6,     -26,
];

public static readonly int[] EndGameBishopTable =
[
        -20,    14,     -15,    2,      -4,     2,      -4,     -38,
        -9,     -10,    -3,     8,      4,      -9,     -4,     -23,
        8,      17,     15,     9,      20,     10,     10,     3,
        7,      11,     15,     11,     7,      15,     7,      -1,
        3,      13,     14,     16,     2,      13,     7,      0,
        6,      5,      6,      7,      13,     6,      7,      0,
        -21,    -13,    -16,    6,      1,      -1,     -4,     -18,
        -14,    -16,    -10,    5,      7,      6,      -6,     -28,
];

public static readonly int[] MiddleGameRookTable =
[
        -2,     -9,     -1,     6,      17,     8,      10,     1,
        -29,    -17,    -12,    -12,    0,      5,      23,     -2,
        -31,    -19,    -22,    -12,    5,      10,     56,     30,
        -30,    -24,    -20,    -14,    -8,     7,      43,     18,
        -23,    -19,    -15,    -6,     -10,    6,      32,     14,
        -25,    -17,    -18,    -4,     3,      20,     52,     30,
        -27,    -28,    -7,     -4,     2,      3,      30,     3,
        0,      -2,     5,      16,     25,     13,     18,     13,
];

public static readonly int[] EndGameRookTable =
[
        5,      3,      7,      -3,     -10,    3,      -1,     -6,
        14,     20,     21,     12,     1,      -1,     -6,     1,
        10,     8,      13,     6,      -7,     -10,    -22,    -21,
        15,     11,     13,     7,      -2,     -1,     -15,    -15,
        14,     10,     13,     3,      0,      -7,     -12,    -11,
        11,     13,     3,      -3,     -9,     -14,    -21,    -15,
        19,     22,     16,     7,      -1,     -1,     -8,     2,
        0,      -3,     1,      -9,     -18,    -5,     -11,    -16,
];

public static readonly int[] MiddleGameQueenTable =
[
        -18,    -14,    -5,     8,      1,      -30,    9,      -10,
        -7,     -10,    10,     1,      7,      10,     23,     43,
        -12,    -3,     -5,     -3,     -7,     12,     37,     53,
        -16,    -20,    -15,    -3,     -5,     -1,     10,     21,
        -15,    -15,    -15,    -13,    -4,     -1,     10,     17,
        -9,     -2,     -11,    -6,     -1,     9,      23,     35,
        -23,    -20,    6,      13,     12,     7,      8,      30,
        -15,    -14,    5,      10,     4,      -38,    -14,    19,
];

public static readonly int[] EndGameQueenTable =
[
        -31,    -25,    -13,    -6,     -17,    -10,    -46,    6,
        -22,    -15,    -26,    4,      1,      -14,    -46,    -6,
        -22,    -8,     4,      -1,     26,     28,     -3,     1,
        -17,    6,      5,      14,     31,     43,     50,     33,
        -7,     0,      14,     28,     27,     37,     28,     46,
        -24,    -15,    15,     14,     23,     28,     29,     16,
        -16,    -8,     -22,    -14,    -8,     -6,     -30,    5,
        -19,    -20,    -21,    -1,     -7,     22,     10,     -11,
];

public static readonly int[] MiddleGameKingTable =
[
        36,     56,     34,     -73,    10,     -57,    45,     59,
        -5,     -14,    -36,    -77,    -91,    -62,    -10,    21,
        -83,    -73,    -118,   -123,   -137,   -141,   -91,    -101,
        -121,   -120,   -139,   -182,   -174,   -163,   -157,   -189,
        -85,    -87,    -129,   -161,   -176,   -145,   -161,   -181,
        -78,    -45,    -111,   -122,   -106,   -119,   -81,    -92,
        82,     -5,     -37,    -67,    -71,    -49,    6,      30,
        51,     84,     46,     -57,    22,     -46,    59,     73,
];

public static readonly int[] EndGameKingTable =
[
        -85,    -48,    -22,    5,      -36,    -2,     -41,    -96,
        -21,    16,     29,     43,     50,     35,     13,     -24,
        7,      43,     64,     76,     80,     71,     47,     25,
        16,     59,     84,     102,    98,     89,     74,     46,
        4,      49,     82,     97,     102,    87,     77,     45,
        5,      40,     63,     77,     74,     66,     47,     19,
        -45,    10,     30,     41,     43,     32,     8,      -28,
        -98,    -61,    -30,    -3,     -31,    -6,     -45,    -101,
];

    public static readonly int[] MiddleGamePawnTableBlack = MiddleGamePawnTable.Select((_, index) => -MiddleGamePawnTable[index ^ 56]).ToArray();
    public static readonly int[] EndGamePawnTableBlack = EndGamePawnTable.Select((_, index) => -EndGamePawnTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameKnightTableBlack = MiddleGameKnightTable.Select((_, index) => -MiddleGameKnightTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameKnightTableBlack = EndGameKnightTable.Select((_, index) => -EndGameKnightTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameBishopTableBlack = MiddleGameBishopTable.Select((_, index) => -MiddleGameBishopTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameBishopTableBlack = EndGameBishopTable.Select((_, index) => -EndGameBishopTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameRookTableBlack = MiddleGameRookTable.Select((_, index) => -MiddleGameRookTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameRookTableBlack = EndGameRookTable.Select((_, index) => -EndGameRookTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameQueenTableBlack = MiddleGameQueenTable.Select((_, index) => -MiddleGameQueenTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameQueenTableBlack = EndGameQueenTable.Select((_, index) => -EndGameQueenTable[index ^ 56]).ToArray();

    public static readonly int[] MiddleGameKingTableBlack = MiddleGameKingTable.Select((_, index) => -MiddleGameKingTable[index ^ 56]).ToArray();
    public static readonly int[] EndGameKingTableBlack = EndGameKingTable.Select((_, index) => -EndGameKingTable[index ^ 56]).ToArray();

    /// <summary>
    /// [12][64]
    /// </summary>
    public static readonly int[][] MiddleGamePositionalTables =
    [
        MiddleGamePawnTable,
        MiddleGameKnightTable,
        MiddleGameBishopTable,
        MiddleGameRookTable,
        MiddleGameQueenTable,
        MiddleGameKingTable,

        MiddleGamePawnTableBlack,
        MiddleGameKnightTableBlack,
        MiddleGameBishopTableBlack,
        MiddleGameRookTableBlack,
        MiddleGameQueenTableBlack,
        MiddleGameKingTableBlack
    ];

    /// <summary>
    /// [12][64]
    /// </summary>
    public static readonly int[][] EndGamePositionalTables =
    [
        EndGamePawnTable,
        EndGameKnightTable,
        EndGameBishopTable,
        EndGameRookTable,
        EndGameQueenTable,
        EndGameKingTable,

        EndGamePawnTableBlack,
        EndGameKnightTableBlack,
        EndGameBishopTableBlack,
        EndGameRookTableBlack,
        EndGameQueenTableBlack,
        EndGameKingTableBlack
    ];

    public static readonly int[,] MiddleGameTable = new int[12, 64];
    public static readonly int[,] EndGameTable = new int[12, 64];

    public static readonly int[,] LMRReductions = new int[Constants.AbsoluteMaxDepth, Constants.MaxNumberOfPossibleMovesInAPosition];

    public static readonly int[] HistoryBonus = new int[Constants.AbsoluteMaxDepth];

    static EvaluationConstants()
    {
        for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
        {
            for (int sq = 0; sq < 64; ++sq)
            {
                MiddleGameTable[piece, sq] = MiddleGamePieceValues[piece] + MiddleGamePositionalTables[piece][sq];
                EndGameTable[piece, sq] = EndGamePieceValues[piece] + EndGamePositionalTables[piece][sq];
            }
        }

        for (int searchDepth = 1; searchDepth < Constants.AbsoluteMaxDepth; ++searchDepth)    // Depth > 0 or we'd be in QSearch
        {
            for (int movesSearchedCount = 1; movesSearchedCount < Constants.MaxNumberOfPossibleMovesInAPosition; ++movesSearchedCount) // movesSearchedCount > 0 or we wouldn't be applying LMR
            {
                LMRReductions[searchDepth, movesSearchedCount] = Convert.ToInt32(Math.Round(
                    Configuration.EngineSettings.LMR_Base + (Math.Log(movesSearchedCount) * Math.Log(searchDepth) / Configuration.EngineSettings.LMR_Divisor)));
            }

            HistoryBonus[searchDepth] = Math.Min(
                Configuration.EngineSettings.History_MaxMoveRawBonus,
                (4 * searchDepth * searchDepth) + (120 * searchDepth) - 120);   // Sirius, originally from Berserk
        }
    }

    /// <summary>
    /// MVV LVA [attacker,victim] [12,64]
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/move_ordering_intro/bbc.c#L2406
    ///             (Victims)   Pawn Knight Bishop  Rook   Queen  King
    /// (Attackers)
    ///       Pawn              105    205    305    405    505    605
    ///     Knight              104    204    304    404    504    604
    ///     Bishop              103    203    303    403    503    603
    ///       Rook              102    202    302    402    502    602
    ///      Queen              101    201    301    401    501    601
    ///       King              100    200    300    400    500    600
    /// </summary>
    public static readonly int[,] MostValueableVictimLeastValuableAttacker =
    {
        { 105, 205, 305, 405, 505, 605, 105, 205, 305, 405, 505, 605 },
        { 104, 204, 304, 404, 504, 604, 104, 204, 304, 404, 504, 604 },
        { 103, 203, 303, 403, 503, 603, 103, 203, 303, 403, 503, 603 },
        { 102, 202, 302, 402, 502, 602, 102, 202, 302, 402, 502, 602 },
        { 101, 201, 301, 401, 501, 601, 101, 201, 301, 401, 501, 601 },
        { 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600 },
        { 105, 205, 305, 405, 505, 605, 105, 205, 305, 405, 505, 605 },
        { 104, 204, 304, 404, 504, 604, 104, 204, 304, 404, 504, 604 },
        { 103, 203, 303, 403, 503, 603, 103, 203, 303, 403, 503, 603 },
        { 102, 202, 302, 402, 502, 602, 102, 202, 302, 402, 502, 602 },
        { 101, 201, 301, 401, 501, 601, 101, 201, 301, 401, 501, 601 },
        { 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600 }
    };

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

    /// <summary>
    /// For MVVLVA
    /// </summary>
    public const int CaptureMoveBaseScoreValue = 1_048_576;

    public const int FirstKillerMoveValue = 524_288;

    public const int SecondKillerMoveValue = 262_144;

    public const int ThirdKillerMoveValue = 131_072;

    public const int PromotionMoveScoreValue = 65_536;

    //public const int MaxHistoryMoveValue => Configuration.EngineSettings.MaxHistoryMoveValue;

    /// <summary>
    /// Negative offset to ensure history move scores don't reach other move ordering values
    /// </summary>
    public const int BaseMoveScore = int.MinValue / 2;

    /// <summary>
    /// Outside of the evaluation ranges (higher than any sensible evaluation, lower than <see cref="PositiveCheckmateDetectionLimit"/>)
    /// </summary>
    public const int NoHashEntry = 25_000;

    /// <summary>
    /// Evaluation to be returned when there's one single legal move
    /// </summary>
    public const int SingleMoveEvaluation = 200;
}
