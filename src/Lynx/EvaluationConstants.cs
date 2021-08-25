/*
 * Based on BBC
 * https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/evaluate_positional_scores/bbc.c
*/

using Lynx.Model;
using static Lynx.Model.BoardSquare;

namespace Lynx
{
    public static class EvaluationConstants
    {
        public static readonly int[] MaterialScore = new int[12]
        {
            +100,
            +300,
            +350,
            +500,
            +900,
            1_000_000,
            -100,
            -300,
            -350,
            -500,
            -900,
            -1_000_000,
        };

        private static readonly int[] PawnPositionalScore = new int[64]
        {
            90,  90,  90,  90,  90,  90,  90,  90,
            30,  30,  30,  40,  40,  30,  30,  30,
            20,  20,  20,  30,  30,  30,  20,  20,
            10,  10,  10,  20,  20,  10,  10,  10,
             5,   5,  10,  20,  20,   5,   5,   5,
             0,   0,   0,   5,   5,   0,   0,   0,
             0,   0,   0, -10, -10,   0,   0,   0,
             0,   0,   0,   0,   0,   0,   0,   0
        };

        private static readonly int[] KnightPositionalScore = new int[64]
        {
            -10,   0,   0,   0,   0,   0,   0,  -10,
            -5,   0,   0,  10,  10,   0,   0,  -5,
            -5,   5,  20,  20,  20,  20,   5,  -5,
            -5,  10,  20,  30,  30,  20,  10,  -5,
            -5,  10,  20,  30,  30,  20,  10,  -5,
            -5,   5,  20,  10,  10,  20,   5,  -5,
            -5,   0,   0,   0,   0,   0,   0,  -5,
            -10, -10,   0,   0,   0,   0, -10,  -10
         };

        private static readonly int[] BishopPositionalScore = new int[64]
        {
             0,   0,   0,   0,   0,   0,   0,   0,
             0,   0,   0,   0,   0,   0,   0,   0,
             0,   0,   0,  10,  10,   0,   0,   0,
             0,   0,  10,  20,  20,  10,   0,   0,
             0,   0,  10,  20,  20,  10,   0,   0,
             0,  10,   0,   0,   0,   0,  10,   0,
             0,  30,   0,   0,   0,   0,  30,   0,
             0,   0, -10,   0,   0, -10,   0,   0
        };

        private static readonly int[] RookPositionalScore = new int[64]
        {
            50,  50,  50,  50,  50,  50,  50,  50,
            50,  50,  50,  50,  50,  50,  50,  50,
             0,   0,  10,  20,  20,  10,   0,   0,
             0,   0,  10,  20,  20,  10,   0,   0,
             0,   0,  10,  20,  20,  10,   0,   0,
             0,   0,  10,  20,  20,  10,   0,   0,
             0,   0,  10,  20,  20,  10,   0,   0,
             0,   0,   0,  20,  20,   0,   0,   0
        };

        private static readonly int[] QueenPositionalScore = new int[64]
        {
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0
        };

        /// <summary>
        /// Could have two: one for opening/middle game, avoiding the center, and another one for endgames, seeking the center
        /// </summary>
        private static readonly int[] KingPositionalScore = new int[64]
        {
             0,   0,   0,   0,   0,   0,   0,   0,
             0,   0,   5,   5,   5,   5,   0,   0,
             0,   5,   5,  10,  10,   5,   5,   0,
             0,   5,  10,  20,  20,  10,   5,   0,
             0,   5,  10,  20,  20,  10,   5,   0,
             0,   0,   5,  10,  10,   5,   0,   0,
             0,   5,   5,  -5,  -5,   0,   5,   0,
             0,   0,   10,  0, -15,   0,  15,   0
        };

        private static readonly int[] MirrorScore = new int[64]
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

        private static int PPS(BoardSquare square) => -PawnPositionalScore[MirrorScore[(int)square]];
        private static int NPS(BoardSquare square) => -KnightPositionalScore[MirrorScore[(int)square]];
        private static int BPS(BoardSquare square) => -BishopPositionalScore[MirrorScore[(int)square]];
        private static int RPS(BoardSquare square) => -RookPositionalScore[MirrorScore[(int)square]];
        private static int KPS(BoardSquare square) => -KingPositionalScore[MirrorScore[(int)square]];

        private static readonly int[] PawnPositionalScore_Black = new int[64]
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

        private static readonly int[] KnightPositionalScore_Black = new int[64]
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

        private static readonly int[] BishopPositionalScore_Black = new int[64]
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

        private static readonly int[] RookPositionalScore_Black = new int[64]
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

        private static readonly int[] KingPositionalScore_Black = new int[64]
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

        public static readonly int[][] PositionalScore = new int[12][]
        {
            PawnPositionalScore,
            KnightPositionalScore,
            BishopPositionalScore,
            RookPositionalScore,
            QueenPositionalScore,
            KingPositionalScore,

            PawnPositionalScore_Black,
            KnightPositionalScore_Black,
            BishopPositionalScore_Black,
            RookPositionalScore_Black,
            QueenPositionalScore,
            KingPositionalScore_Black,
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
    }
}
