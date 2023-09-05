/*
 * PSQT based on https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
*/

using Lynx.Model;
using static Lynx.Model.BoardSquare;

namespace Lynx;

public static class EvaluationConstants
{
    public static readonly int[] MiddleGamePieceValues =
    {
        +82, +337, +365, +477, +1025, 0,
        -82, -337, -365, -477, -1025, 0,
    };

    public static readonly int[] EndGamePieceValues =
    {
        +94, +281, +297, +512, +936, 0,
        -94, -281, -297, -512, -936, 0
    };

    public static readonly int[] GamePhaseByPiece =
    {
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    };

    public static readonly int[] MiddleGamePawnTable =
    {
          0,   0,   0,   0,   0,   0,  0,   0,
         98, 134,  61,  95,  68, 126, 34, -11,
         -6,   7,  26,  31,  65,  56, 25, -20,
        -14,  13,   6,  21,  23,  12, 17, -23,
        -27,  -2,  -5,  12,  17,   6, 10, -25,
        -26,  -4,  -4, -10,   3,   3, 33, -12,
        -35,  -1, -20, -23, -15,  24, 38, -22,
          0,   0,   0,   0,   0,   0,  0,   0
    };

    public static readonly int[] EndGamePawnTable =
    {
          0,   0,   0,   0,   0,   0,   0,   0,
        178, 173, 158, 134, 147, 132, 165, 187,
         94, 100,  85,  67,  56,  53,  82,  84,
         32,  24,  13,   5,  -2,   4,  17,  17,
         13,   9,  -3,  -7,  -7,  -8,   3,  -1,
          4,   7,  -6,   1,   0,  -5,  -1,  -8,
         13,   8,   8,  10,  13,   0,   2,  -7,
          0,   0,   0,   0,   0,   0,   0,   0
    };

    public static readonly int[] MiddleGameKnightTable =
    {
        -167, -89, -34, -49,  61, -97, -15, -107,
         -73, -41,  72,  36,  23,  62,   7,  -17,
         -47,  60,  37,  65,  84, 129,  73,   44,
          -9,  17,  19,  53,  37,  69,  18,   22,
         -13,   4,  16,  13,  28,  19,  21,   -8,
         -23,  -9,  12,  10,  19,  17,  25,  -16,
         -29, -53, -12,  -3,  -1,  18, -14,  -19,
        -105, -21, -58, -33, -17, -28, -19,  -23
    };

    public static readonly int[] EndGameKnightTable =
    {
        -58, -38, -13, -28, -31, -27, -63, -99,
        -25,  -8, -25,  -2,  -9, -25, -24, -52,
        -24, -20,  10,   9,  -1,  -9, -19, -41,
        -17,   3,  22,  22,  22,  11,   8, -18,
        -18,  -6,  16,  25,  16,  17,   4, -18,
        -23,  -3,  -1,  15,  10,  -3, -20, -22,
        -42, -20, -10,  -5,  -2, -20, -23, -44,
        -29, -51, -23, -15, -22, -18, -50, -64
    };

    public static readonly int[] MiddleGameBishopTable =
    {
        -29,   4, -82, -37, -25, -42,   7,  -8,
        -26,  16, -18, -13,  30,  59,  18, -47,
        -16,  37,  43,  40,  35,  50,  37,  -2,
         -4,   5,  19,  50,  37,  37,   7,  -2,
         -6,  13,  13,  26,  34,  12,  10,   4,
          0,  15,  15,  15,  14,  27,  18,  10,
          4,  15,  16,   0,   7,  21,  33,   1,
        -33,  -3, -14, -21, -13, -12, -39, -21
    };

    public static readonly int[] EndGameBishopTable =
    {
        -14, -21, -11,  -8, -7,  -9, -17, -24,
         -8,  -4,   7, -12, -3, -13,  -4, -14,
          2,  -8,   0,  -1, -2,   6,   0,   4,
         -3,   9,  12,   9, 14,  10,   3,   2,
         -6,   3,  13,  19,  7,  10,  -3,  -9,
        -12,  -3,   8,  10, 13,   3,  -7, -15,
        -14, -18,  -7,  -1,  4,  -9, -15, -27,
        -23,  -9, -23,  -5, -9, -16,  -5, -17
    };

    public static readonly int[] MiddleGameRookTable =
    {
         32,  42,  32,  51, 63,  9,  31,  43,
         27,  32,  58,  62, 80, 67,  26,  44,
         -5,  19,  26,  36, 17, 45,  61,  16,
        -24, -11,   7,  26, 24, 35,  -8, -20,
        -36, -26, -12,  -1,  9, -7,   6, -23,
        -45, -25, -16, -17,  3,  0,  -5, -33,
        -44, -16, -20,  -9, -1, 11,  -6, -71,
        -19, -13,   1,  17, 16,  7, -37, -26
    };

    public static readonly int[] EndGameRookTable =
    {
        13, 10, 18, 15, 12,  12,   8,   5,
        11, 13, 13, 11, -3,   3,   8,   3,
         7,  7,  7,  5,  4,  -3,  -5,  -3,
         4,  3, 13,  1,  2,   1,  -1,   2,
         3,  5,  8,  4, -5,  -6,  -8, -11,
        -4,  0, -5, -1, -7, -12,  -8, -16,
        -6, -6,  0,  2, -9,  -9, -11,  -3,
        -9,  2,  3, -1, -5, -13,   4, -20
    };

    public static readonly int[] MiddleGameQueenTable =
    {
        -28,   0,  29,  12,  59,  44,  43,  45,
        -24, -39,  -5,   1, -16,  57,  28,  54,
        -13, -17,   7,   8,  29,  56,  47,  57,
        -27, -27, -16, -16,  -1,  17,  -2,   1,
         -9, -26,  -9, -10,  -2,  -4,   3,  -3,
        -14,   2, -11,  -2,  -5,   2,  14,   5,
        -35,  -8,  11,   2,   8,  15,  -3,   1,
         -1, -18,  -9,  10, -15, -25, -31, -50
    };

    public static readonly int[] EndGameQueenTable =
    {
         -9,  22,  22,  27,  27,  19,  10,  20,
        -17,  20,  32,  41,  58,  25,  30,   0,
        -20,   6,   9,  49,  47,  35,  19,   9,
          3,  22,  24,  45,  57,  40,  57,  36,
        -18,  28,  19,  47,  31,  34,  39,  23,
        -16, -27,  15,   6,   9,  17,  10,   5,
        -22, -23, -30, -16, -16, -23, -36, -32,
        -33, -28, -22, -43,  -5, -32, -20, -41
    };

    public static readonly int[] MiddleGameKingTable =
    {
        -65,  23,  16, -15, -56, -34,   2,  13,
         29,  -1, -20,  -7,  -8,  -4, -38, -29,
         -9,  24,   2, -16, -20,   6,  22, -22,
        -17, -20, -12, -27, -30, -25, -14, -36,
        -49,  -1, -27, -39, -46, -44, -33, -51,
        -14, -14, -22, -46, -44, -30, -15, -27,
          1,   7,  -8, -64, -43, -16,   9,   8,
        -15,  36,  12, -54,   8, -28,  24,  14
    };

    public static readonly int[] EndGameKingTable =
    {
        -74, -35, -18, -18, -11,  15,   4, -17,
        -12,  17,  14,  17,  17,  38,  23,  11,
         10,  17,  23,  15,  20,  45,  44,  13,
         -8,  22,  24,  27,  26,  33,  26,   3,
        -18,  -4,  21,  24,  27,  23,   9, -11,
        -19,  -3,  11,  21,  23,  16,   7,  -9,
        -27, -11,   4,  13,  14,   4,  -5, -17,
        -53, -34, -21, -11, -28, -14, -24, -43
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

    private static int MPS(BoardSquare square) => -MiddleGamePawnTable[_mirrorScore[(int)square]];
    private static int EPS(BoardSquare square) => -EndGamePawnTable[_mirrorScore[(int)square]];
    private static int MNS(BoardSquare square) => -MiddleGameKnightTable[_mirrorScore[(int)square]];
    private static int ENS(BoardSquare square) => -EndGameKnightTable[_mirrorScore[(int)square]];
    private static int MBS(BoardSquare square) => -MiddleGameBishopTable[_mirrorScore[(int)square]];
    private static int EBS(BoardSquare square) => -EndGameBishopTable[_mirrorScore[(int)square]];
    private static int MRS(BoardSquare square) => -MiddleGameRookTable[_mirrorScore[(int)square]];
    private static int ERS(BoardSquare square) => -EndGameRookTable[_mirrorScore[(int)square]];
    private static int MQS(BoardSquare square) => -MiddleGameQueenTable[_mirrorScore[(int)square]];
    private static int EQS(BoardSquare square) => -EndGameQueenTable[_mirrorScore[(int)square]];
    private static int MKS(BoardSquare square) => -MiddleGameKingTable[_mirrorScore[(int)square]];
    private static int EKS(BoardSquare square) => -EndGameKingTable[_mirrorScore[(int)square]];

    private static readonly int[] MiddleGamePawnTableBlack = new int[64]
    {
        MPS(a8), MPS(b8), MPS(c8), MPS(d8), MPS(e8), MPS(f8), MPS(g8), MPS(h8),
        MPS(a7), MPS(b7), MPS(c7), MPS(d7), MPS(e7), MPS(f7), MPS(g7), MPS(h7),
        MPS(a6), MPS(b6), MPS(c6), MPS(d6), MPS(e6), MPS(f6), MPS(g6), MPS(h6),
        MPS(a5), MPS(b5), MPS(c5), MPS(d5), MPS(e5), MPS(f5), MPS(g5), MPS(h5),
        MPS(a4), MPS(b4), MPS(c4), MPS(d4), MPS(e4), MPS(f4), MPS(g4), MPS(h4),
        MPS(a3), MPS(b3), MPS(c3), MPS(d3), MPS(e3), MPS(f3), MPS(g3), MPS(h3),
        MPS(a2), MPS(b2), MPS(c2), MPS(d2), MPS(e2), MPS(f2), MPS(g2), MPS(h2),
        MPS(a1), MPS(b1), MPS(c1), MPS(d1), MPS(e1), MPS(f1), MPS(g1), MPS(h1)
    };

    private static readonly int[] EndGamePawnTableBlack = new int[64]
    {
        EPS(a8), EPS(b8), EPS(c8), EPS(d8), EPS(e8), EPS(f8), EPS(g8), EPS(h8),
        EPS(a7), EPS(b7), EPS(c7), EPS(d7), EPS(e7), EPS(f7), EPS(g7), EPS(h7),
        EPS(a6), EPS(b6), EPS(c6), EPS(d6), EPS(e6), EPS(f6), EPS(g6), EPS(h6),
        EPS(a5), EPS(b5), EPS(c5), EPS(d5), EPS(e5), EPS(f5), EPS(g5), EPS(h5),
        EPS(a4), EPS(b4), EPS(c4), EPS(d4), EPS(e4), EPS(f4), EPS(g4), EPS(h4),
        EPS(a3), EPS(b3), EPS(c3), EPS(d3), EPS(e3), EPS(f3), EPS(g3), EPS(h3),
        EPS(a2), EPS(b2), EPS(c2), EPS(d2), EPS(e2), EPS(f2), EPS(g2), EPS(h2),
        EPS(a1), EPS(b1), EPS(c1), EPS(d1), EPS(e1), EPS(f1), EPS(g1), EPS(h1)
    };

    private static readonly int[] MiddleGameKnightTableBlack = new int[64]
    {
        MNS(a8), MNS(b8), MNS(c8), MNS(d8), MNS(e8), MNS(f8), MNS(g8), MNS(h8),
        MNS(a7), MNS(b7), MNS(c7), MNS(d7), MNS(e7), MNS(f7), MNS(g7), MNS(h7),
        MNS(a6), MNS(b6), MNS(c6), MNS(d6), MNS(e6), MNS(f6), MNS(g6), MNS(h6),
        MNS(a5), MNS(b5), MNS(c5), MNS(d5), MNS(e5), MNS(f5), MNS(g5), MNS(h5),
        MNS(a4), MNS(b4), MNS(c4), MNS(d4), MNS(e4), MNS(f4), MNS(g4), MNS(h4),
        MNS(a3), MNS(b3), MNS(c3), MNS(d3), MNS(e3), MNS(f3), MNS(g3), MNS(h3),
        MNS(a2), MNS(b2), MNS(c2), MNS(d2), MNS(e2), MNS(f2), MNS(g2), MNS(h2),
        MNS(a1), MNS(b1), MNS(c1), MNS(d1), MNS(e1), MNS(f1), MNS(g1), MNS(h1)
    };

    private static readonly int[] EndGameKnightTableBlack = new int[64]
    {
        ENS(a8), ENS(b8), ENS(c8), ENS(d8), ENS(e8), ENS(f8), ENS(g8), ENS(h8),
        ENS(a7), ENS(b7), ENS(c7), ENS(d7), ENS(e7), ENS(f7), ENS(g7), ENS(h7),
        ENS(a6), ENS(b6), ENS(c6), ENS(d6), ENS(e6), ENS(f6), ENS(g6), ENS(h6),
        ENS(a5), ENS(b5), ENS(c5), ENS(d5), ENS(e5), ENS(f5), ENS(g5), ENS(h5),
        ENS(a4), ENS(b4), ENS(c4), ENS(d4), ENS(e4), ENS(f4), ENS(g4), ENS(h4),
        ENS(a3), ENS(b3), ENS(c3), ENS(d3), ENS(e3), ENS(f3), ENS(g3), ENS(h3),
        ENS(a2), ENS(b2), ENS(c2), ENS(d2), ENS(e2), ENS(f2), ENS(g2), ENS(h2),
        ENS(a1), ENS(b1), ENS(c1), ENS(d1), ENS(e1), ENS(f1), ENS(g1), ENS(h1)
    };

    private static readonly int[] MiddleGameBishopTableBlack = new int[64]
    {
        MBS(a8), MBS(b8), MBS(c8), MBS(d8), MBS(e8), MBS(f8), MBS(g8), MBS(h8),
        MBS(a7), MBS(b7), MBS(c7), MBS(d7), MBS(e7), MBS(f7), MBS(g7), MBS(h7),
        MBS(a6), MBS(b6), MBS(c6), MBS(d6), MBS(e6), MBS(f6), MBS(g6), MBS(h6),
        MBS(a5), MBS(b5), MBS(c5), MBS(d5), MBS(e5), MBS(f5), MBS(g5), MBS(h5),
        MBS(a4), MBS(b4), MBS(c4), MBS(d4), MBS(e4), MBS(f4), MBS(g4), MBS(h4),
        MBS(a3), MBS(b3), MBS(c3), MBS(d3), MBS(e3), MBS(f3), MBS(g3), MBS(h3),
        MBS(a2), MBS(b2), MBS(c2), MBS(d2), MBS(e2), MBS(f2), MBS(g2), MBS(h2),
        MBS(a1), MBS(b1), MBS(c1), MBS(d1), MBS(e1), MBS(f1), MBS(g1), MBS(h1)
    };

    private static readonly int[] EndGameBishopTableBlack = new int[64]
    {
        EBS(a8), EBS(b8), EBS(c8), EBS(d8), EBS(e8), EBS(f8), EBS(g8), EBS(h8),
        EBS(a7), EBS(b7), EBS(c7), EBS(d7), EBS(e7), EBS(f7), EBS(g7), EBS(h7),
        EBS(a6), EBS(b6), EBS(c6), EBS(d6), EBS(e6), EBS(f6), EBS(g6), EBS(h6),
        EBS(a5), EBS(b5), EBS(c5), EBS(d5), EBS(e5), EBS(f5), EBS(g5), EBS(h5),
        EBS(a4), EBS(b4), EBS(c4), EBS(d4), EBS(e4), EBS(f4), EBS(g4), EBS(h4),
        EBS(a3), EBS(b3), EBS(c3), EBS(d3), EBS(e3), EBS(f3), EBS(g3), EBS(h3),
        EBS(a2), EBS(b2), EBS(c2), EBS(d2), EBS(e2), EBS(f2), EBS(g2), EBS(h2),
        EBS(a1), EBS(b1), EBS(c1), EBS(d1), EBS(e1), EBS(f1), EBS(g1), EBS(h1)
    };

    private static readonly int[] MiddleGameRookTableBlack = new int[64]
    {
        MRS(a8), MRS(b8), MRS(c8), MRS(d8), MRS(e8), MRS(f8), MRS(g8), MRS(h8),
        MRS(a7), MRS(b7), MRS(c7), MRS(d7), MRS(e7), MRS(f7), MRS(g7), MRS(h7),
        MRS(a6), MRS(b6), MRS(c6), MRS(d6), MRS(e6), MRS(f6), MRS(g6), MRS(h6),
        MRS(a5), MRS(b5), MRS(c5), MRS(d5), MRS(e5), MRS(f5), MRS(g5), MRS(h5),
        MRS(a4), MRS(b4), MRS(c4), MRS(d4), MRS(e4), MRS(f4), MRS(g4), MRS(h4),
        MRS(a3), MRS(b3), MRS(c3), MRS(d3), MRS(e3), MRS(f3), MRS(g3), MRS(h3),
        MRS(a2), MRS(b2), MRS(c2), MRS(d2), MRS(e2), MRS(f2), MRS(g2), MRS(h2),
        MRS(a1), MRS(b1), MRS(c1), MRS(d1), MRS(e1), MRS(f1), MRS(g1), MRS(h1)
    };

    private static readonly int[] EndGameRookTableBlack = new int[64]
    {
        ERS(a8), ERS(b8), ERS(c8), ERS(d8), ERS(e8), ERS(f8), ERS(g8), ERS(h8),
        ERS(a7), ERS(b7), ERS(c7), ERS(d7), ERS(e7), ERS(f7), ERS(g7), ERS(h7),
        ERS(a6), ERS(b6), ERS(c6), ERS(d6), ERS(e6), ERS(f6), ERS(g6), ERS(h6),
        ERS(a5), ERS(b5), ERS(c5), ERS(d5), ERS(e5), ERS(f5), ERS(g5), ERS(h5),
        ERS(a4), ERS(b4), ERS(c4), ERS(d4), ERS(e4), ERS(f4), ERS(g4), ERS(h4),
        ERS(a3), ERS(b3), ERS(c3), ERS(d3), ERS(e3), ERS(f3), ERS(g3), ERS(h3),
        ERS(a2), ERS(b2), ERS(c2), ERS(d2), ERS(e2), ERS(f2), ERS(g2), ERS(h2),
        ERS(a1), ERS(b1), ERS(c1), ERS(d1), ERS(e1), ERS(f1), ERS(g1), ERS(h1)
    };

    private static readonly int[] MiddleGameQueenTableBlack = new int[64]
    {
        MQS(a8), MQS(b8), MQS(c8), MQS(d8), MQS(e8), MQS(f8), MQS(g8), MQS(h8),
        MQS(a7), MQS(b7), MQS(c7), MQS(d7), MQS(e7), MQS(f7), MQS(g7), MQS(h7),
        MQS(a6), MQS(b6), MQS(c6), MQS(d6), MQS(e6), MQS(f6), MQS(g6), MQS(h6),
        MQS(a5), MQS(b5), MQS(c5), MQS(d5), MQS(e5), MQS(f5), MQS(g5), MQS(h5),
        MQS(a4), MQS(b4), MQS(c4), MQS(d4), MQS(e4), MQS(f4), MQS(g4), MQS(h4),
        MQS(a3), MQS(b3), MQS(c3), MQS(d3), MQS(e3), MQS(f3), MQS(g3), MQS(h3),
        MQS(a2), MQS(b2), MQS(c2), MQS(d2), MQS(e2), MQS(f2), MQS(g2), MQS(h2),
        MQS(a1), MQS(b1), MQS(c1), MQS(d1), MQS(e1), MQS(f1), MQS(g1), MQS(h1)
    };

    private static readonly int[] EndGameQueenTableBlack = new int[64]
    {
        EQS(a8), EQS(b8), EQS(c8), EQS(d8), EQS(e8), EQS(f8), EQS(g8), EQS(h8),
        EQS(a7), EQS(b7), EQS(c7), EQS(d7), EQS(e7), EQS(f7), EQS(g7), EQS(h7),
        EQS(a6), EQS(b6), EQS(c6), EQS(d6), EQS(e6), EQS(f6), EQS(g6), EQS(h6),
        EQS(a5), EQS(b5), EQS(c5), EQS(d5), EQS(e5), EQS(f5), EQS(g5), EQS(h5),
        EQS(a4), EQS(b4), EQS(c4), EQS(d4), EQS(e4), EQS(f4), EQS(g4), EQS(h4),
        EQS(a3), EQS(b3), EQS(c3), EQS(d3), EQS(e3), EQS(f3), EQS(g3), EQS(h3),
        EQS(a2), EQS(b2), EQS(c2), EQS(d2), EQS(e2), EQS(f2), EQS(g2), EQS(h2),
        EQS(a1), EQS(b1), EQS(c1), EQS(d1), EQS(e1), EQS(f1), EQS(g1), EQS(h1)
    };

    private static readonly int[] MiddleGameKingTableBlack = new int[64]
    {
        MKS(a8), MKS(b8), MKS(c8), MKS(d8), MKS(e8), MKS(f8), MKS(g8), MKS(h8),
        MKS(a7), MKS(b7), MKS(c7), MKS(d7), MKS(e7), MKS(f7), MKS(g7), MKS(h7),
        MKS(a6), MKS(b6), MKS(c6), MKS(d6), MKS(e6), MKS(f6), MKS(g6), MKS(h6),
        MKS(a5), MKS(b5), MKS(c5), MKS(d5), MKS(e5), MKS(f5), MKS(g5), MKS(h5),
        MKS(a4), MKS(b4), MKS(c4), MKS(d4), MKS(e4), MKS(f4), MKS(g4), MKS(h4),
        MKS(a3), MKS(b3), MKS(c3), MKS(d3), MKS(e3), MKS(f3), MKS(g3), MKS(h3),
        MKS(a2), MKS(b2), MKS(c2), MKS(d2), MKS(e2), MKS(f2), MKS(g2), MKS(h2),
        MKS(a1), MKS(b1), MKS(c1), MKS(d1), MKS(e1), MKS(f1), MKS(g1), MKS(h1)
    };

    private static readonly int[] EndGameKingTableBlack = new int[64]
    {
        EKS(a8), EKS(b8), EKS(c8), EKS(d8), EKS(e8), EKS(f8), EKS(g8), EKS(h8),
        EKS(a7), EKS(b7), EKS(c7), EKS(d7), EKS(e7), EKS(f7), EKS(g7), EKS(h7),
        EKS(a6), EKS(b6), EKS(c6), EKS(d6), EKS(e6), EKS(f6), EKS(g6), EKS(h6),
        EKS(a5), EKS(b5), EKS(c5), EKS(d5), EKS(e5), EKS(f5), EKS(g5), EKS(h5),
        EKS(a4), EKS(b4), EKS(c4), EKS(d4), EKS(e4), EKS(f4), EKS(g4), EKS(h4),
        EKS(a3), EKS(b3), EKS(c3), EKS(d3), EKS(e3), EKS(f3), EKS(g3), EKS(h3),
        EKS(a2), EKS(b2), EKS(c2), EKS(d2), EKS(e2), EKS(f2), EKS(g2), EKS(h2),
        EKS(a1), EKS(b1), EKS(c1), EKS(d1), EKS(e1), EKS(f1), EKS(g1), EKS(h1)
    };

    public static readonly int[][] MiddleGamePositionalTables =
    {
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
    };

    public static readonly int[][] EndGamePositionalTables =
    {
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
    };

    public static readonly int[,] MiddleGameTable = new int[12, 64];
    public static readonly int[,] EndGameTable = new int[12, 64];

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
    }

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

    public const int PVMoveScoreValue = 200_000;

    public const int TTMoveScoreValue = 190_000;

    /// <summary>
    /// For MVVLVA
    /// </summary>
    public const int CaptureMoveBaseScoreValue = 100_000;

    public const int FirstKillerMoveValue = 9_000;

    public const int SecondKillerMoveValue = 8_000;

    public const int PromotionMoveScoreValue = 7_000;

    /// <summary>
    /// Outside of the evaluation ranges (higher than any sensible evaluation, lower than <see cref="PositiveCheckmateDetectionLimit"/>)
    /// </summary>
    public const int NoHashEntry = 25_000;
}
