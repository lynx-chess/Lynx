/*
 * Based on BBC
 * https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/evaluate_positional_scores/bbc.c
*/

using Lynx.Model;
using static Lynx.Model.BoardSquare;

namespace Lynx;

public static class EvaluationConstants
{
    public static readonly int[] MaterialScore = new int[12]
    {
        +100,
        +300,
        +350,
        +500,
        +1_000,
        +1_000_000,
        -100,
        -300,
        -350,
        -500,
        -1_000,
        -1_000_000,
    };

    private static readonly int[] _pawnPositionalScore = new int[64]
    {
        90,  90,  90,  90,  90,  90,  90,  90,
        40,  40,  40,  50,  50,  40,  40,  40,
        20,  20,  20,  30,  30,  30,  20,  20,
        10,  10,  10,  20,  20,  10,  10,  10,
        0,   0,  10,  20,  20,   0,   0,   0,
        0,   0,   0,  10,  10,   0,   0,   0,
        0,   0,   0, -10, -10,   0,   0,   0,
        0,   0,   0,   0,   0,   0,   0,   0
    };

    private static readonly int[] _knightPositionalScore = new int[64]
    {
        -60,    -20,    -10,    -3,     -3,     -10,    -20,    -60,
        -15,    -5,      0,     10,     10,     0,      -5,     -15,
        -5,     5,      20,     20,     20,     20,     5,      -5,
        -5,     10,     20,     30,     30,     20,     10,     -5,
        -5,     10,     20,     30,     30,     20,     10,     -5,
        -5,     5,      20,     10,     10,     20,     5,      -5,
        -15,    -5,      0,      5,      5,      0,     -5,     -15,
        -30,    -20,    -10,    -5,     -5,     -10,    -20,    -30
     };

    private static readonly int[] _bishopPositionalScore = new int[64]
    {
        -0,   -10,    -10,     -10,    -10,    -10,    -10,    0,
        -5,    10,      0,     0,      0,      0,      10,     -5,
        -5,    0,      20,     10,     10,     20,     0,      -5,
        0,     5,      10,     30,     30,     10,     5,      0,
        0,     5,      10,     30,     30,     10,     5,      0,
        -5,    0,      20,     10,     10,     20,     0,      -5,
        -5,     10,     0,      0,      0,      0,     10,    -5,
        0,     -10,    -20,    -10,    -10,    -20,    -10,    0
    };

    private static readonly int[] _rookPositionalScore = new int[64]
    {
        50,  50,  50,  50,  50,  50,  50,  50,
        70,  70,  70,  70,  70,  70,  70,  70,
        0,   0,  10,  20,  20,  10,   0,   0,
        0,   0,  10,  20,  20,  10,   0,   0,
        0,   0,  10,  20,  20,  10,   0,   0,
        0,   0,  10,  20,  20,  10,   0,   0,
        0,   0,  10,  20,  20,  10,   0,   0,
        5,   5,   5,  20,  20,   5,   5,   5
    };

    private static readonly int[] _queenPositionalScore = new int[64]
    {
        -10,   -10,    5,      5,      5,      5,      -10,    -10,
        -10,   5,      5,      5,      5,      5,      5,      -10,
        5,     5,      10,     10,     10,     5,      5,      5,
        5,     5,      10,     20,     20,     10,     5,      5,
        5,     5,      10,     20,     20,     10,     5,      5,
        5,     5,      5,      10,     10,     5,      5,      5,
        -10,   5,      5,      5,      5,      5,      5,      -10,
        -10,   -10,    -5,     0,      0,      -5,     -10,    -10
    };

    private static readonly int[] _kingPositionalScore = new int[64]
    {
        -50,   -50,    -50,    -50,    -50,    -50,    -50,    -50,
        -50,   -50,    -50,    -50,    -50,    -50,    -50,    -50,
        -50,   -50,    -50,    -50,    -50,    -50,    -50,    -50,
        -50,   -50,    -50,    -50,    -50,    -50,    -50,    -50,
        -50,   -50,    -50,    -50,    -50,    -50,    -50,    -50,
        -50,   -50,    -50,    -50,    -50,    -50,    -50,    -50,
        +15,    +20,   -10,    -20,    -20,    -30,    +30,    +25,
        +20,    +20,   +20,    -10,    +10,    -40,    +30,    +30
    };

    private static readonly int[] _kingEndgamePositionalScore = new int[64]
    {
        0,     5,      10,     12,     12,     10,     5,      0,
        5,     10,     15,     20,     20,     15,     10,     5,
        10,    15,     20,     30,     30,     20,     15,     10,
        15,    20,     30,     50,     50,     30,     20,     15,
        15,    20,     30,     50,     50,     30,     20,     15,
        10,    15,     20,     30,     30,     20,     15,     10,
        5,     10,     15,     20,     20,     15,     10,     5,
        0,     5,      10,     12,     12,     10,     5,      0
    };

    private static readonly int[] _mirrorScore = new int[64]
    {
        (int)a1, (int)b1, (int)c1, (int)d1, (int)e1, (int)f1, (int)g1, (int)h1,
        (int)a2, (int)b2, (int)c2, (int)d2, (int)e2, (int)f2, (int)g2, (int)h2,
        (int)a3, (int)b3, (int)c3, (int)d3, (int)e3, (int)f3, (int)g3, (int)h3,
        (int)a4, (int)b4, (int)c4, (int)d4, (int)e4, (int)f4, (int)g4, (int)h4,
        (int)a5, (int)b5, (int)c5, (int)d5, (int)e5, (int)f5, (int)g5, (int)h5,
        (int)a6, (int)b6, (int)c6, (int)d6, (int)e6, (int)f6, (int)g6, (int)h6,
        (int)a7, (int)b7, (int)c7, (int)d7, (int)e7, (int)f7, (int)g7, (int)h7,
        (int)a8, (int)b8, (int)c8, (int)d8, (int)e8, (int)f8, (int)g8, (int)h8
    };

    private static int PPS(BoardSquare square) => -_pawnPositionalScore[_mirrorScore[(int)square]];
    private static int NPS(BoardSquare square) => -_knightPositionalScore[_mirrorScore[(int)square]];
    private static int BPS(BoardSquare square) => -_bishopPositionalScore[_mirrorScore[(int)square]];
    private static int RPS(BoardSquare square) => -_rookPositionalScore[_mirrorScore[(int)square]];
    private static int QPS(BoardSquare square) => -_queenPositionalScore[_mirrorScore[(int)square]];
    private static int KPS(BoardSquare square) => -_kingPositionalScore[_mirrorScore[(int)square]];
    private static int KEPS(BoardSquare square) => -_kingEndgamePositionalScore[_mirrorScore[(int)square]];

    private static readonly int[] _pawnPositionalScore_Black = new int[64]
    {
        PPS(a8), PPS(b8), PPS(c8), PPS(d8), PPS(e8), PPS(f8), PPS(g8), PPS(h8),
        PPS(a7), PPS(b7), PPS(c7), PPS(d7), PPS(e7), PPS(f7), PPS(g7), PPS(h7),
        PPS(a6), PPS(b6), PPS(c6), PPS(d6), PPS(e6), PPS(f6), PPS(g6), PPS(h6),
        PPS(a5), PPS(b5), PPS(c5), PPS(d5), PPS(e5), PPS(f5), PPS(g5), PPS(h5),
        PPS(a4), PPS(b4), PPS(c4), PPS(d4), PPS(e4), PPS(f4), PPS(g4), PPS(h4),
        PPS(a3), PPS(b3), PPS(c3), PPS(d3), PPS(e3), PPS(f3), PPS(g3), PPS(h3),
        PPS(a2), PPS(b2), PPS(c2), PPS(d2), PPS(e2), PPS(f2), PPS(g2), PPS(h2),
        PPS(a1), PPS(b1), PPS(c1), PPS(d1), PPS(e1), PPS(f1), PPS(g1), PPS(h1)
    };

    private static readonly int[] _knightPositionalScore_Black = new int[64]
    {
        NPS(a8), NPS(b8), NPS(c8), NPS(d8), NPS(e8), NPS(f8), NPS(g8), NPS(h8),
        NPS(a7), NPS(b7), NPS(c7), NPS(d7), NPS(e7), NPS(f7), NPS(g7), NPS(h7),
        NPS(a6), NPS(b6), NPS(c6), NPS(d6), NPS(e6), NPS(f6), NPS(g6), NPS(h6),
        NPS(a5), NPS(b5), NPS(c5), NPS(d5), NPS(e5), NPS(f5), NPS(g5), NPS(h5),
        NPS(a4), NPS(b4), NPS(c4), NPS(d4), NPS(e4), NPS(f4), NPS(g4), NPS(h4),
        NPS(a3), NPS(b3), NPS(c3), NPS(d3), NPS(e3), NPS(f3), NPS(g3), NPS(h3),
        NPS(a2), NPS(b2), NPS(c2), NPS(d2), NPS(e2), NPS(f2), NPS(g2), NPS(h2),
        NPS(a1), NPS(b1), NPS(c1), NPS(d1), NPS(e1), NPS(f1), NPS(g1), NPS(h1)
    };

    private static readonly int[] _bishopPositionalScore_Black = new int[64]
    {
        BPS(a8), BPS(b8), BPS(c8), BPS(d8), BPS(e8), BPS(f8), BPS(g8), BPS(h8),
        BPS(a7), BPS(b7), BPS(c7), BPS(d7), BPS(e7), BPS(f7), BPS(g7), BPS(h7),
        BPS(a6), BPS(b6), BPS(c6), BPS(d6), BPS(e6), BPS(f6), BPS(g6), BPS(h6),
        BPS(a5), BPS(b5), BPS(c5), BPS(d5), BPS(e5), BPS(f5), BPS(g5), BPS(h5),
        BPS(a4), BPS(b4), BPS(c4), BPS(d4), BPS(e4), BPS(f4), BPS(g4), BPS(h4),
        BPS(a3), BPS(b3), BPS(c3), BPS(d3), BPS(e3), BPS(f3), BPS(g3), BPS(h3),
        BPS(a2), BPS(b2), BPS(c2), BPS(d2), BPS(e2), BPS(f2), BPS(g2), BPS(h2),
        BPS(a1), BPS(b1), BPS(c1), BPS(d1), BPS(e1), BPS(f1), BPS(g1), BPS(h1)
    };

    private static readonly int[] _rookPositionalScore_Black = new int[64]
    {
        RPS(a8), RPS(b8), RPS(c8), RPS(d8), RPS(e8), RPS(f8), RPS(g8), RPS(h8),
        RPS(a7), RPS(b7), RPS(c7), RPS(d7), RPS(e7), RPS(f7), RPS(g7), RPS(h7),
        RPS(a6), RPS(b6), RPS(c6), RPS(d6), RPS(e6), RPS(f6), RPS(g6), RPS(h6),
        RPS(a5), RPS(b5), RPS(c5), RPS(d5), RPS(e5), RPS(f5), RPS(g5), RPS(h5),
        RPS(a4), RPS(b4), RPS(c4), RPS(d4), RPS(e4), RPS(f4), RPS(g4), RPS(h4),
        RPS(a3), RPS(b3), RPS(c3), RPS(d3), RPS(e3), RPS(f3), RPS(g3), RPS(h3),
        RPS(a2), RPS(b2), RPS(c2), RPS(d2), RPS(e2), RPS(f2), RPS(g2), RPS(h2),
        RPS(a1), RPS(b1), RPS(c1), RPS(d1), RPS(e1), RPS(f1), RPS(g1), RPS(h1)
    };

    private static readonly int[] _queenPositionalScore_Black = new int[64]
    {
        QPS(a8), QPS(b8), QPS(c8), QPS(d8), QPS(e8), QPS(f8), QPS(g8), QPS(h8),
        QPS(a7), QPS(b7), QPS(c7), QPS(d7), QPS(e7), QPS(f7), QPS(g7), QPS(h7),
        QPS(a6), QPS(b6), QPS(c6), QPS(d6), QPS(e6), QPS(f6), QPS(g6), QPS(h6),
        QPS(a5), QPS(b5), QPS(c5), QPS(d5), QPS(e5), QPS(f5), QPS(g5), QPS(h5),
        QPS(a4), QPS(b4), QPS(c4), QPS(d4), QPS(e4), QPS(f4), QPS(g4), QPS(h4),
        QPS(a3), QPS(b3), QPS(c3), QPS(d3), QPS(e3), QPS(f3), QPS(g3), QPS(h3),
        QPS(a2), QPS(b2), QPS(c2), QPS(d2), QPS(e2), QPS(f2), QPS(g2), QPS(h2),
        QPS(a1), QPS(b1), QPS(c1), QPS(d1), QPS(e1), QPS(f1), QPS(g1), QPS(h1)
    };

    private static readonly int[] _kingPositionalScore_Black = new int[64]
    {
        KPS(a8), KPS(b8), KPS(c8), KPS(d8), KPS(e8), KPS(f8), KPS(g8), KPS(h8),
        KPS(a7), KPS(b7), KPS(c7), KPS(d7), KPS(e7), KPS(f7), KPS(g7), KPS(h7),
        KPS(a6), KPS(b6), KPS(c6), KPS(d6), KPS(e6), KPS(f6), KPS(g6), KPS(h6),
        KPS(a5), KPS(b5), KPS(c5), KPS(d5), KPS(e5), KPS(f5), KPS(g5), KPS(h5),
        KPS(a4), KPS(b4), KPS(c4), KPS(d4), KPS(e4), KPS(f4), KPS(g4), KPS(h4),
        KPS(a3), KPS(b3), KPS(c3), KPS(d3), KPS(e3), KPS(f3), KPS(g3), KPS(h3),
        KPS(a2), KPS(b2), KPS(c2), KPS(d2), KPS(e2), KPS(f2), KPS(g2), KPS(h2),
        KPS(a1), KPS(b1), KPS(c1), KPS(d1), KPS(e1), KPS(f1), KPS(g1), KPS(h1)
    };

    private static readonly int[] _kingEndgamePositionalScore_Black = new int[64]
    {
        KEPS(a8), KEPS(b8), KEPS(c8), KEPS(d8), KEPS(e8), KEPS(f8), KEPS(g8), KEPS(h8),
        KEPS(a7), KEPS(b7), KEPS(c7), KEPS(d7), KEPS(e7), KEPS(f7), KEPS(g7), KEPS(h7),
        KEPS(a6), KEPS(b6), KEPS(c6), KEPS(d6), KEPS(e6), KEPS(f6), KEPS(g6), KEPS(h6),
        KEPS(a5), KEPS(b5), KEPS(c5), KEPS(d5), KEPS(e5), KEPS(f5), KEPS(g5), KEPS(h5),
        KEPS(a4), KEPS(b4), KEPS(c4), KEPS(d4), KEPS(e4), KEPS(f4), KEPS(g4), KEPS(h4),
        KEPS(a3), KEPS(b3), KEPS(c3), KEPS(d3), KEPS(e3), KEPS(f3), KEPS(g3), KEPS(h3),
        KEPS(a2), KEPS(b2), KEPS(c2), KEPS(d2), KEPS(e2), KEPS(f2), KEPS(g2), KEPS(h2),
        KEPS(a1), KEPS(b1), KEPS(c1), KEPS(d1), KEPS(e1), KEPS(f1), KEPS(g1), KEPS(h1)
    };

    public static readonly int[][] PositionalScore = new int[12][]
    {
        _pawnPositionalScore,
        _knightPositionalScore,
        _bishopPositionalScore,
        _rookPositionalScore,
        _queenPositionalScore,
        _kingPositionalScore,

        _pawnPositionalScore_Black,
        _knightPositionalScore_Black,
        _bishopPositionalScore_Black,
        _rookPositionalScore_Black,
        _queenPositionalScore_Black,
        _kingPositionalScore_Black,
    };

    public static readonly int[][] EndgamePositionalScore = new int[12][]
    {
        Array.Empty<int>(),
        Array.Empty<int>(),
        Array.Empty<int>(),
        Array.Empty<int>(),
        Array.Empty<int>(),
        _kingEndgamePositionalScore,

        Array.Empty<int>(),
        Array.Empty<int>(),
        Array.Empty<int>(),
        Array.Empty<int>(),
        Array.Empty<int>(),
        _kingEndgamePositionalScore_Black
    };

    /// <summary>
    /// MVV LVA [attacker,victim]
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
    public static readonly int[,] MostValueableVictimLeastValuableAttacker = new int[12, 12]
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

    public const int FirstKillerMoveValue = 9_000;

    public const int SecondKillerMoveValue = 8_000;

    public const int PVMoveScoreValue = 200_000;

    public const int TTMoveScoreValue = 190_000;

    public const int PromotionMoveScoreValue = 150_000;

    /// <summary>
    /// For MVVLVA
    /// </summary>
    public const int CaptureMoveBaseScoreValue = 100_000;

    /// <summary>
    /// Outside of the evaluation ranges (higher than any sensible evaluation, lower than <see cref="PositiveCheckmateDetectionLimit"/>)
    /// </summary>
    public const int NoHashEntry = 25_000;
}
