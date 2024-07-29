namespace Lynx;
using static Utils;

#pragma warning disable IDE0055 // Discard formatting in this region

public static partial class EvaluationConstants
{
    public static readonly short[] MiddleGamePieceValues =
    [
            +97, +356, +373, +507, +1123, 0,
            -97, -356, -373, -507, -1123, 0
    ];

    public static readonly short[] EndGamePieceValues =
    [
            +123, +427, +363, +747, +1425, 0,
            -123, -427, -363, -747, -1425, 0
    ];

    public static readonly short[] MiddleGamePawnTable =
    [
               0,      0,      0,      0,      0,      0,      0,      0,
             -21,    -22,    -14,    -12,     -6,     20,     23,    -12,
             -24,    -26,     -6,      9,     15,     23,     20,      8,
             -20,    -14,      5,     18,     27,     30,      4,     -1,
             -20,    -10,      3,     20,     29,     29,      3,     -2,
             -22,    -21,     -5,      2,     10,     20,     13,      2,
             -21,    -19,    -19,    -15,     -9,     14,     12,    -19,
               0,      0,      0,      0,      0,      0,      0,      0,
    ];

    public static readonly short[] EndGamePawnTable =
    [
               0,      0,      0,      0,      0,      0,      0,      0,
              16,     11,      5,     -7,      8,      3,     -5,     -3,
              10,      6,     -3,    -15,    -10,    -10,    -10,     -9,
              27,     17,     -1,    -20,    -16,    -15,      4,      1,
              24,     16,     -3,    -16,    -15,    -12,      2,     -1,
              12,      3,     -6,    -13,     -7,     -8,     -9,    -10,
              18,     11,      9,     -5,     17,      7,     -1,      0,
               0,      0,      0,      0,      0,      0,      0,      0,
    ];

    public static readonly short[] MiddleGameKnightTable =
    [
            -142,    -19,    -51,    -28,    -10,    -13,     -7,    -79,
             -42,    -27,     -5,     16,     18,     28,     -8,      0,
             -35,     -4,     12,     50,     52,     33,     26,     -7,
             -13,     21,     37,     51,     51,     52,     43,     14,
             -10,     21,     39,     39,     50,     51,     43,     13,
             -33,     -3,     12,     43,     49,     26,     20,     -8,
             -45,    -20,     -2,     15,     17,     23,     -6,      0,
            -157,    -22,    -49,    -20,     -8,     -4,    -15,    -67,
    ];

    public static readonly short[] EndGameKnightTable =
    [
             -56,    -38,     -5,     -3,     -3,    -25,    -31,    -80,
              -9,      6,     10,      4,      4,     -3,     -7,    -16,
              -8,      7,     26,     27,     22,      7,     -1,     -9,
              10,     11,     37,     38,     43,     35,     15,     -3,
               7,     16,     36,     42,     43,     32,     19,      4,
             -10,     11,     16,     31,     22,      8,     -3,     -5,
             -15,      8,      1,      7,     -0,     -8,     -8,    -20,
             -60,    -35,      0,     -6,     -5,    -25,    -30,    -79,
    ];

    public static readonly short[] MiddleGameBishopTable =
    [
             -13,     13,     -9,    -20,    -15,    -10,    -21,     14,
               4,      1,      4,    -17,      1,      3,     36,     -6,
              -9,     -0,     -9,      4,    -12,      6,     -2,     26,
              -9,     -8,     -2,     23,     21,    -13,      2,     -3,
             -17,     -3,    -11,     20,      9,    -10,     -5,      3,
               2,     -2,      2,     -4,     -0,     -1,      0,     21,
               7,     12,      7,     -6,     -4,      5,     28,      6,
              11,     16,      3,    -35,    -17,    -16,      2,     -3,
    ];

    public static readonly short[] EndGameBishopTable =
    [
               1,     17,     -3,      5,      2,      3,      1,    -19,
               3,     -7,     -6,     -0,     -2,    -14,     -7,    -10,
              13,      8,      4,     -2,      6,      1,      0,     10,
              13,      0,      3,      0,     -1,      2,     -1,      6,
               7,      3,      1,      4,     -3,      3,     -1,      7,
              10,     -2,     -2,     -4,      1,     -4,     -2,      7,
              -6,     -9,    -16,     -2,     -4,     -8,     -7,     -5,
               3,     -8,     -2,      9,      9,      4,     -1,     -6,
    ];

    public static readonly short[] MiddleGameRookTable =
    [
              -4,    -10,     -6,      0,      9,      8,     11,      2,
             -29,    -18,    -11,     -8,      4,     12,     29,     -2,
             -33,    -22,    -23,    -12,      3,      9,     48,     24,
             -29,    -26,    -23,    -11,     -6,      6,     37,     16,
             -24,    -21,    -17,     -7,     -8,      7,     28,     12,
             -26,    -18,    -19,     -2,      0,     19,     48,     22,
             -26,    -27,     -7,     -2,      5,     10,     35,      5,
              -2,     -4,     -1,     10,     17,     12,     19,     14,
    ];

    public static readonly short[] EndGameRookTable =
    [
               7,      5,      9,      1,     -7,     -1,     -3,     -6,
              17,     19,     19,      9,     -1,     -5,     -9,      0,
              14,     10,     12,      4,     -8,    -12,    -23,    -18,
              15,     11,     12,      4,     -3,     -5,    -17,    -16,
              14,     10,     11,      2,     -2,    -10,    -15,    -12,
              14,     13,      4,     -3,    -10,    -15,    -24,    -11,
              20,     22,     15,      4,     -3,     -5,    -10,     -0,
               2,     -1,      4,     -5,    -14,     -9,     -9,    -14,
    ];

    public static readonly short[] MiddleGameQueenTable =
    [
             -13,    -12,    -13,      5,     -2,    -30,      6,      2,
              -4,    -10,      7,     -0,      3,     11,     28,     50,
             -12,     -8,    -10,    -10,    -14,      5,     30,     53,
             -12,    -20,    -17,     -8,     -8,     -3,     13,     26,
             -12,    -16,    -16,    -16,     -6,     -3,     12,     23,
              -9,     -6,    -16,    -11,     -9,      1,     17,     33,
             -16,    -19,      3,     12,      8,      7,     12,     38,
             -11,    -12,     -2,      7,      1,    -35,    -15,     25,
    ];

    public static readonly short[] EndGameQueenTable =
    [
             -22,    -16,     -3,    -11,    -15,    -15,    -32,      8,
             -14,     -9,    -25,     -2,     -4,    -21,    -49,     -5,
             -13,     -4,      7,      2,     22,     19,     -8,      5,
             -10,      7,      6,     11,     23,     33,     40,     28,
              -2,      3,     12,     22,     19,     29,     20,     38,
             -13,    -10,     14,     11,     13,     18,     16,     18,
              -9,     -3,    -21,    -21,    -13,    -17,    -34,      3,
             -13,    -13,    -10,     -6,     -9,     11,     15,     -2,
    ];

    public static readonly short[] MiddleGameKingTable =
    [
               6,     39,     36,    -64,     19,    -55,     35,     24,
             -21,    -15,    -20,    -54,    -67,    -44,     -5,      5,
             -83,    -65,    -99,    -97,   -106,   -115,    -79,    -99,
             -99,    -80,   -104,   -151,   -145,   -122,   -121,   -155,
             -66,    -54,    -92,   -129,   -143,   -108,   -125,   -151,
             -81,    -44,    -91,    -91,    -82,    -92,    -70,    -90,
              59,     -9,    -21,    -42,    -46,    -29,     11,     14,
              16,     60,     46,    -46,     34,    -42,     51,     38,
    ];

    public static readonly short[] EndGameKingTable =
    [
             -77,    -49,    -28,     -1,    -41,     -9,    -43,    -91,
             -23,     15,     24,     37,     43,     30,      8,    -29,
              -2,     40,     69,     81,     84,     73,     40,     10,
               0,     52,     90,    122,    120,     92,     63,     23,
             -10,     43,     88,    119,    123,     91,     65,     22,
              -1,     37,     70,     81,     78,     68,     39,      5,
             -47,     11,     26,     35,     37,     26,      3,    -33,
             -88,    -57,    -34,     -9,    -38,    -14,    -48,    -96,
    ];

    public static readonly TaperedEvaluationTerm IsolatedPawnPenalty = Pack(-19, -14);

    public static readonly TaperedEvaluationTerm OpenFileRookBonus = Pack(45, 6);

    public static readonly TaperedEvaluationTerm SemiOpenFileRookBonus = Pack(15, 8);

    public static readonly TaperedEvaluationTerm QueenMobilityBonus = Pack(3, 8);

    public static readonly TaperedEvaluationTerm SemiOpenFileKingPenalty = Pack(-30, 18);

    public static readonly TaperedEvaluationTerm OpenFileKingPenalty = Pack(-96, 15);

    public static readonly TaperedEvaluationTerm KingShieldBonus = Pack(23, -11);

    public static readonly TaperedEvaluationTerm BishopPairBonus = Pack(30, 81);

    public static readonly TaperedEvaluationTerm PieceProtectedByPawnBonus = Pack(7, 11);

    public static readonly TaperedEvaluationTerm PieceAttackedByPawnPenalty = Pack(-45, -18);

    public static readonly TaperedEvaluationTermByRank PassedPawnBonus =
    [
        Pack(0, 0),
        Pack(2, 11),
        Pack(-11, 20),
        Pack(-12, 47),
        Pack(18, 81),
        Pack(63, 161),
        Pack(102, 225),
        Pack(0, 0)
    ];

    public static readonly TaperedEvaluationTermByCount27 VirtualKingMobilityBonus =
    [
        Pack(0, 0),
        Pack(0, 0),
        Pack(0, 0),
        Pack(37, -5),
        Pack(51, -8),
        Pack(24, 24),
        Pack(22, 13),
        Pack(19, 3),
        Pack(15, 6),
        Pack(11, 5),
        Pack(9, 9),
        Pack(2, 13),
        Pack(0, 9),
        Pack(-5, 12),
        Pack(-15, 15),
        Pack(-26, 18),
        Pack(-35, 15),
        Pack(-46, 12),
        Pack(-52, 10),
        Pack(-60, 3),
        Pack(-51, -5),
        Pack(-46, -13),
        Pack(-44, -24),
        Pack(-39, -34),
        Pack(-45, -44),
        Pack(-22, -65),
        Pack(-62, -72),
        Pack(-36, -90)
    ];

    public static readonly TaperedEvaluationTermByCount8 KnightMobilityBonus =
    [
        Pack(0, 0),
        Pack(25, -4),
        Pack(34, 5),
        Pack(40, 5),
        Pack(44, 11),
        Pack(42, 20),
        Pack(41, 22),
        Pack(43, 24),
        Pack(55, 17)
    ];

    public static readonly TaperedEvaluationTermByCount14 BishopMobilityBonus =
    [
        Pack(-198, -154),
        Pack(0, 0),
        Pack(12, 2),
        Pack(21, 41),
        Pack(36, 57),
        Pack(42, 72),
        Pack(58, 92),
        Pack(67, 102),
        Pack(76, 114),
        Pack(76, 121),
        Pack(82, 126),
        Pack(85, 125),
        Pack(87, 126),
        Pack(119, 118),
        Pack(0, 0)
    ];

    public static readonly TaperedEvaluationTermByCount14 RookMobilityBonus =
    [
        Pack(0, 0),
        Pack(7, 31),
        Pack(12, 34),
        Pack(16, 42),
        Pack(14, 52),
        Pack(21, 55),
        Pack(24, 62),
        Pack(29, 66),
        Pack(30, 78),
        Pack(34, 85),
        Pack(38, 87),
        Pack(41, 89),
        Pack(41, 93),
        Pack(55, 92),
        Pack(50, 90)
    ];
}

#pragma warning restore IDE0055
