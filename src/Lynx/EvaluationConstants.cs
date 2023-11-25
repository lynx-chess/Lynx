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
            +98, +360, +343, +474, +1078, 0,
            -98, -360, -343, -474, -1078, 0
    ];

    public static readonly int[] EndGamePieceValues =
    [
            +132, +419, +363, +783, +1299, 0,
            -132, -419, -363, -783, -1299, 0
    ];

    public static readonly int[] MiddleGamePawnTable =
    [
            0,      0,      0,      0,      0,      0,      0,      0,
            -26,    -23,    -22,    -14,    -12,    24,     34,     -6,
            -23,    -21,    -10,    2,      12,     30,     28,     12,
            -17,    -8,     4,      15,     23,     33,     13,     0,
            -17,    -7,     -1,     11,     20,     31,     10,     -2,
            -19,    -21,    -8,     -4,     10,     26,     21,     7,
            -26,    -23,    -22,    -19,    -13,    23,     24,     -12,
            0,      0,      0,      0,      0,      0,      0,      0,
    ];

    public static readonly int[] EndGamePawnTable =
    [
            0,      0,      0,      0,      0,      0,      0,      0,
            14,     12,     9,      -12,    3,      7,      -1,     -12,
            11,     9,      -1,     -14,    -6,     -3,     -8,     -13,
            29,     20,     0,      -19,    -12,    -10,    4,      -3,
            25,     18,     0,      -17,    -12,    -9,     2,      -6,
            14,     9,      -3,     -12,    -2,     -2,     -5,     -13,
            18,     13,     9,      -5,     14,     10,     2,      -10,
            0,      0,      0,      0,      0,      0,      0,      0,
    ];

    public static readonly int[] MiddleGameKnightTable =
    [
            -139,   -21,    -58,    -41,    -22,    -29,    -7,     -74,
            -39,    -26,    -3,     13,     10,     19,     -5,     -8,
            -15,    4,      14,     60,     50,     36,     29,     2,
            -6,     20,     40,     51,     50,     56,     51,     25,
            -8,     26,     39,     44,     50,     47,     54,     23,
            -16,    5,      16,     46,     52,     31,     29,     -7,
            -53,    -27,    -1,     12,     12,     12,     -14,    -14,
            -152,   -18,    -58,    -43,    -16,    -21,    -10,    -70,
    ];

    public static readonly int[] EndGameKnightTable =
    [
            -57,    -79,    -15,    -15,    -9,     -16,    -65,    -93,
            -27,    5,      7,      3,      16,     6,      -15,    -17,
            -25,    13,     41,     32,     38,     20,     18,     -8,
            4,      16,     49,     50,     59,     53,     33,     -1,
            -2,     26,     48,     51,     62,     55,     42,     7,
            -22,    17,     30,     42,     36,     19,     8,      -17,
            -11,    6,      1,      6,      6,      -4,     -14,    -27,
            -49,    -91,    -6,     -9,     -13,    -28,    -68,    -98,
    ];

    public static readonly int[] MiddleGameBishopTable =
    [
            -12,    -3,     -5,     -23,    -26,    -20,    -42,    -10,
            2,      4,      9,      -13,    -3,     -8,     26,     -10,
            -2,     4,      1,      8,      0,      13,     6,      25,
            -15,    6,      3,      37,     26,     -9,     8,      7,
            -7,     3,      -6,     24,     17,     -9,     3,      7,
            0,      12,     7,      4,      9,      11,     8,      27,
            11,     9,      14,     -9,     3,      1,      25,     -14,
            3,      13,     -1,     -46,    -34,    -20,    -1,     -12,
    ];

    public static readonly int[] EndGameBishopTable =
    [
            -10,    18,     -26,    -2,     3,      -3,     13,     -13,
            3,      -8,     -7,     1,      2,      -2,     -2,     -17,
            14,     18,     5,      -1,     12,     0,      8,      15,
            8,      5,      5,      -4,     -13,    1,      8,      8,
            -3,     6,      8,      1,      -7,     8,      5,      4,
            14,     3,      0,      -3,     7,      -1,     11,     5,
            -7,     -11,    -8,     4,      -4,     -3,     -11,    1,
            0,      -4,     -16,    6,      9,      -5,     0,      -13,
    ];

    public static readonly int[] MiddleGameRookTable =
    [
            -10,    -16,    -13,    -7,     4,      -6,     -2,     -1,
            -36,    -19,    -19,    -18,    -12,    2,      15,     -26,
            -30,    -12,    -17,    -1,     8,      5,      53,     25,
            -27,    -16,    -7,     12,     10,     27,     63,     23,
            -14,    -9,     -8,     13,     4,      22,     51,     18,
            -24,    -5,     -21,    4,      -2,     20,     49,     34,
            -25,    -17,    -9,     -5,     -5,     -3,     24,     1,
            -6,     -12,    -11,    -1,     8,      -2,     3,      4,
    ];

    public static readonly int[] EndGameRookTable =
    [
            2,      3,      6,      2,      -9,     -1,     0,      -16,
            20,     15,     17,     5,      -3,     -6,     -8,     8,
            10,     7,      9,      1,      -8,     1,      -15,    -8,
            13,     16,     11,     2,      -2,     1,      -16,    -17,
            8,      9,      13,     1,      1,      -2,     -11,    -8,
            14,     10,     8,      -2,     -8,     -9,     -17,    -10,
            20,     18,     12,     2,      -1,     0,      -4,     4,
            -2,     1,      4,      -4,     -12,    -4,     -2,     -21,
    ];

    public static readonly int[] MiddleGameQueenTable =
    [
            -7,     -21,    -9,     -1,     -7,     -54,    15,     -4,
            -17,    -14,    8,      1,      -1,     -3,     16,     42,
            -17,    0,      -7,     -6,     -7,     11,     36,     51,
            -8,     -10,    -13,    -8,     6,      12,     20,     24,
            -15,    -6,     -11,    -11,    4,      1,      22,     29,
            2,      -1,     -8,     -7,     -7,     8,      19,     42,
            -20,    -9,     4,      5,      6,      4,      0,      21,
            -7,     -6,     -7,     -2,     -13,    -41,    -26,    54,
    ];

    public static readonly int[] EndGameQueenTable =
    [
            -18,    -14,    -20,    -25,    -13,    6,      -25,    50,
            5,      -2,     -32,    -16,    -4,     -19,    -42,    -6,
            5,      -11,    0,      -9,     6,      9,      1,      21,
            -16,    0,      2,      13,     7,      30,     39,     54,
            0,      -10,    0,      17,     12,     35,     29,     58,
            -26,    -17,    2,      5,      8,      13,     23,     23,
            -2,     -12,    -31,    -15,    -22,    -33,    -36,    21,
            -7,     -22,    -14,    -14,    0,      17,     13,     10,
    ];

    public static readonly int[] MiddleGameKingTable =
    [
            18,     23,     2,      -89,    0,      -77,    34,     38,
            -24,    -15,    -38,    -62,    -76,    -63,    -7,     16,
            -94,    -55,    -77,    -82,    -95,    -101,   -66,    -87,
            -29,    -29,    -68,    -95,    -90,    -109,   -86,    -137,
            -12,    -18,    -58,    -84,    -112,   -95,    -117,   -145,
            -52,    -39,    -58,    -81,    -75,    -100,   -73,    -105,
            44,     -11,    -40,    -63,    -71,    -53,    3,      25,
            18,     53,     26,     -75,    7,      -74,    42,     51,
    ];

    public static readonly int[] EndGameKingTable =
    [
            -86,    -30,    -19,    -6,     -49,    -8,     -36,    -89,
            -20,    12,     21,     28,     34,     29,     9,      -29,
            11,     32,     43,     52,     59,     54,     33,     17,
            2,      34,     57,     63,     65,     66,     52,     27,
            -13,    30,     55,     63,     70,     65,     57,     34,
            1,      28,     45,     54,     52,     52,     35,     19,
            -33,    12,     25,     29,     34,     26,     4,      -33,
            -71,    -47,    -33,    -6,     -44,    -8,     -37,    -94,
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

    public const int PromotionMoveScoreValue = 131_072;

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
