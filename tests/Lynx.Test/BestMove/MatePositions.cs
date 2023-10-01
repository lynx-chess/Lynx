namespace Lynx.Test.BestMove;

public static class MatePositions
{
    public static readonly object[] Mates_in_1 = new object[]
    {
        new object[]{ "8/8/8/N1N5/3N1Q2/8/1bpPPP2/2k1K2R w K - 0 1                      ", new[] { Constants.WhiteShortCastle }, "" },
        new object[]{ "8/8/8/4N1N1/3B3N/8/3PPPpb/R3K1k1 w Q - 0 1                       ", new[] { Constants.WhiteLongCastle }, "" },
        new object[]{ "8/8/1p2k3/3bn3/3K4/8/7r/1q6 b - - 0 1                            ", new[] { "b1d3" }, ""},
        new object[]{ "r1bq1r2/pp2p3/4N2k/3pPppP/1b1p2Q1/2N5/PP3PP1/R1B1K2R w KQ g6 0 1 ", new[] { "h5g6" }, "https://www.chessgames.com/perl/chessgame?gid=1242924" },
        new object[]{ "5r2/1n6/2p3k1/b1Pb3p/P2PpPpP/4P1P1/1P4KR/4q3 b - f3 0 1          ", new[] { "e4f3" }, "https://www.chessgames.com/perl/chessgame?gid=1886010" }
    };

    public static readonly object[] Mates_in_2 = new object[]
    {
        new object[]{ "5k2/Q7/8/5K2/8/8/8/8 w - - 0 1                                   ", new[] { "f5f6" }, "https://gameknot.com/chess-puzzle.pl?pz=114463" },
        new object[]{ "8/pN3R2/1b2k1K1/n4R2/pp1p4/3B1P1n/3B1PNP/3r3Q w - -              ", new[] { "d2f4" }, "https://gameknot.com/chess-puzzle.pl?pz=114463" },
        new object[]{ "KQ4R1/8/8/8/4N3/8/5p2/6bk w - -                                  ", new[] { "b8b2" }, "https://gameknot.com/chess-puzzle.pl?pz=1" },
        new object[]{ "8/8/8/8/8/3n1N2/8/3Q1K1k w - -                                   ", new[] { "d1b1" }, "https://gameknot.com/chess-puzzle.pl?pz=1669"},
        new object[]{ "RNBKRNRQ/PPPPPPPP/8/pppppppp/rnbqkbnr/8/8/8 b - -                ", new[] { "g4h6" }, "https://gameknot.com/chess-puzzle.pl?pz=1630" }
    };

    public static readonly object[] Mates_in_3 = new object[]
    {
        new object[]{ "4rqk1/3R1prR/p1p5/1p2PQp1/5p2/1P6/P1B2PP1/6K1 w - -              ", new[] { "f5h3" }, "https://gameknot.com/chess-puzzle.pl?pz=111285" },
        new object[]{ "1b6/2p2N1K/1p2Bp1p/3Pp2R/4kp1p/1N6/p1P1PPb1/r1R4r w - -          ", new[] { "h7h6" }, "https://gameknot.com/chess-puzzle.pl?pz=251481"},
        new object[]{ "5B2/3Q4/8/7p/6N1/6k1/8/5K2 w - -                                 ", new[] { "f8h6" }, "https://gameknot.com/chess-puzzle.pl?pz=248898"},
        new object[]{ "nb5B/1N1Q4/6RK/3pPP2/RrrkN3/2pP3b/qpP1PP2/3n4 w - -              ", new[] { "g6g3" }, "https://gameknot.com/chess-puzzle.pl?pz=228148"},
        new object[]{ "3R3N/2pR1p2/4k3/N1P5/1q4PK/2B4B/8/8 w - - 0 1                    ", new[] { "c5c6" }, "http://talkchess.com/forum3/viewtopic.php?f=7&t=78428&p=908885"}
    };

    public static readonly object[] Mates_in_4 = new object[]
    {
        new object[]{ "5k2/R7/8/5K2/8/8/8/8 w - - 0 1       ", new[] { "f5e6" }, "" },
        new object[]{ "6k1/1R6/5K2/3p1N2/1P3n2/8/8/3r4 w - -", new[] { "f5h6" }, "https://gameknot.com/chess-puzzle.pl?pz=260253" }
    };

    /// <summary>
    /// http://www.talkchess.com/forum3/viewtopic.php?f=7&t=78583
    /// </summary>
    public static readonly object[] Mates_in_4_Collection = new object[]
    {
        new object[]{ "n7/3p1p2/NpkNp1p1/1p2P3/3Q4/6B1/b7/4K3 w - - 0 1                 ", new[] { "d4g1" }, ""},
        new object[]{ "K6Q/1p6/pPq4P/P2p2P1/4pP1N/7k/n5R1/1n2BB2 w - - 0 1              ", new[] { "f1a6" }, ""},
        new object[]{ "8/8/8/3p4/1N1P2Kp/Bkp5/5Q2/4N3 w - - 0 1                         ", new[] { "f2f8" }, ""},
        new object[]{ "4K3/8/pp2B3/P7/4kpR1/2P1p3/4Q3/2r5 w - - 0 1                     ", new[] { "g4g2" }, ""},
        new object[]{ "8/Q5p1/2Bpq3/KPn2pR1/3kpP1p/1P1p3b/3B1b2/7n w - - 0 1            ", new[] { "g5g6" }, ""},
        new object[]{ "2Q3Kn/p7/8/3NkPp1/p7/P1Np1pPr/1P1p4/1b2b2r w - - 0 1             ", new[] { "b2b4" }, ""},
        new object[]{ "4b1N1/2pr2pR/2rp1p2/1Q1B1kp1/2pP4/4P1K1/1n3PN1/R7 w - - 0 1      ", new[] { "a1g1" }, ""},
        new object[]{ "r1b5/1p1n3p/4p2K/p5Bp/3k4/Q4B2/2P5/5n2 w - - 0 1                 ", new[] { "g5e7" }, ""},
        new object[]{ "4N3/1p6/3B2nN/pPpk4/4R1Pp/P1p2P2/K1P1b3/3n4 w - - 0 1            ", new[] { "d6h2" }, ""},
        new object[]{ "3r1n2/2bp2p1/1p1kp1Pp/4N2p/Pp2B2Q/7P/1P3P1B/K7 w - - 0 1         ", new[] { "e4g2", "e4d3", "e4h1", "e4f3", "e4a8" }, ""},
        new object[]{ "3K4/8/6R1/p3k1pp/2bp1R1p/n1b1P3/2p2N2/2B4B w - - 0 1             ", new[] { "g6g5", "c1d2" }, ""},
        new object[]{ "5n2/r2p1p2/p1PBkn2/p3Ppp1/6b1/1N1pP3/1N1P3K/2R5 w - - 0 1        ", new[] { "b2a4" }, ""},
        new object[]{ "8/1Qp5/2K5/1R5p/7P/5k2/8/8 w - - 0 1                             ", new[] { "b5a5" }, ""},
        new object[]{ "8/8/8/p7/R4B2/8/5k2/1B5K w - - 0 1                               ", new[] { "b1f5" }, ""},
        new object[]{ "2N5/8/N1B5/4k2p/8/7K/8/2Q5 w - - 0 1                             ", new[] { "h3h4" }, ""},
        new object[]{ "8/1R6/8/3k3N/N5R1/8/8/4K3 w - - 0 1                              ", new[] { "g4g7" }, ""},
        new object[]{ "8/1R4p1/3kN3/5PR1/8/6K1/8/8 w - - 0 1                            ", new[] { "f5f6" }, ""},
        new object[]{ "8/8/2K3p1/4N2B/7k/5p2/Q7/8 w - - 0 1                             ", new[] { "a2b1" }, ""},
        new object[]{ "1n6/2Bp4/p2N4/N1k5/2p1P3/R1P5/1P6/1K1b4 w - - 0 1                ", new[] { "a3a4" }, ""},
        new object[]{ "2r5/2p3n1/5R2/4k1PP/3R2p1/2n1P3/1Q2P3/4b2K w - - 0 1             ", new[] { "b2b7" }, ""},
        new object[]{ "8/8/1p6/1P1P1B2/k7/1NK5/2N5/8 w - - 0 1                          ", new[] { "c2e3" }, ""},
        new object[]{ "4nKn1/1Q6/2p2P2/1p6/6r1/2P1B3/1N1Np1b1/4k3 w - - 0 1             ", new[] { "b7f7" }, ""},
        new object[]{ "8/np1N1N2/4p3/1p1b4/1b2kp2/p1R5/p3P3/n1Q2K2 w - - 0 1            ", new[] { "c3g3" }, ""},
        new object[]{ "bK1N2N1/5Qnr/2p4p/2pnk3/8/1p1BPP2/6rp/7q w - - 0 1               ", new[] { "d3c4" }, ""},
        new object[]{ "1QB3n1/BK4P1/4Pp1r/p2kb2p/7P/5Rp1/1PP2Pp1/8 w - - 0 1            ", new[] { "e6e7" }, ""},
        new object[]{ "n7/3rp1NK/b4pP1/1r6/4kb2/p4R2/qNP2R1P/1n5B w - - 0 1             ", new[] { "f3f4" }, ""},
        new object[]{ "8/1pp5/2b5/4N3/2Q2B2/k1p4r/p5rp/K1R5 w - - 0 1                   ", new[] { "f4h6" }, ""},
        new object[]{ "2k5/K2pR3/RB1p1r2/1p4N1/1p4N1/1P6/6n1/3Br2q w - - 0 1            ", new[] { "a6a2" }, ""},
        new object[]{ "4b3/3q4/8/4P3/4BP1p/1R1Pk2p/4N3/4K2R w - - 0 1                   ", new[] { "e4a8" }, ""},
        new object[]{ "1KB1n3/Q4p1N/1Npk3q/r1p5/2p2p1b/R1p4b/2P2P2/4R3 w - - 0 1        ", new[] { "a3a1" }, ""},
        new object[]{ "8/8/8/4p3/4p3/2k5/2P1K3/1R1R4 w - - 0 1                          ", new[] { "b1b5" }, ""},
        new object[]{ "4k3/5RR1/8/1n6/8/7p/7K/8 w - - 0 1                               ", new[] { "g7h7" }, ""},
        new object[]{ "8/8/8/3R4/8/5K2/4n2p/7k w - - 0 1                                ", new[] { "d5d2" }, ""},
        new object[]{ "8/8/3K4/6p1/3k3P/1R1P1R2/8/8 w - - 0 1                           ", new[] { "f3h3" }, ""},
        new object[]{ "R7/8/1p4K1/1Pp4P/1P1p2pk/2pR4/P5P1/8 w - - 0 1                   ", new[] { "a8a3" }, ""},
        new object[]{ "7R/8/8/4N3/1p5p/4K2p/8/4k3 w - - 0 1                             ", new[] { "h8c8" }, ""},
        new object[]{ "6R1/8/8/3N4/p5p1/3K2p1/8/3k4 w - - 0 1                           ", new[] { "g8e8" }, ""},
        new object[]{ "3k4/5R2/8/8/2K5/B3N3/8/8 w - - 0 1                               ", new[] { "e3f5" }, ""},
        new object[]{ "3k4/7R/8/8/K2B1N2/8/8/8 w - - 0 1                                ", new[] { "f4d3" }, ""},
        new object[]{ "3k4/6R1/8/8/1N1B4/3K4/8/8 w - - 0 1                              ", new[] { "d4e3" }, ""},
        new object[]{ "3k4/7R/8/8/1N1B4/2K5/8/8 w - - 0 1                               ", new[] { "b4d3" }, ""}
    };

    public static readonly object[] Mates_in_5 = new object[]
    {
        new object[]{ "4r3/4PpPk/5P1P/3R2K1/P2P4/2P5/8/8 w - -                          ", new[] { "d5d8" }, "https://gameknot.com/chess-puzzle.pl?pz=253500" },
        new object[]{ "r1b1r3/5p1k/p1qbpp2/1p1p4/5P2/PP2PR2/2PN2PP/R2Q2K1 w - -         ", new[] { "f3g3" }, "https://gameknot.com/chess-puzzle.pl?pz=196756" }
    };

    public static readonly object[] Mates_in_6 = new object[]
    {
        new object[]{ "1r4k1/5q2/3bp3/3p4/3P2n1/4P3/P1Q1KP2/1N4R1 b - -                 ", new[] { "f7f2" }, "https://gameknot.com/chess-puzzle.pl?pz=111491" },
        new object[]{ "1r6/5pkp/1n1Rr1pN/p1p1P1Q1/1b2qB1P/6P1/5P1K/3R4 w - -            ", new[] { "g5f6" }, "https://gameknot.com/chess-puzzle.pl?pz=145711" }
    };

    public static readonly object[] Mates_in_7 = new object[]
    {
        new object[]{ "4r1k1/1p3p1p/rb1R2p1/pQ6/P1p1q3/2P3RP/1P3PP1/6K1 b - -           ", new[] { "b6f2" }, "https://gameknot.com/chess-puzzle.pl?pz=228984" },    // info depth 14 seldepth 28 multipv 1 score cp 1405 nodes 202487382 nps 511294 time 395029 pv b6f2 g1h2 a6d6 g3g4 e4e1 b5e8 e1e8 g4c4 d6d2 b2b3 e8e5 g2g3

        new object[]{ "1k2r3/ppN3pp/5n2/8/6P1/P1p1pK1P/1PPr4/1R2RN2 b - -               ", new[] { "d2f2" }, "https://gameknot.com/chess-puzzle.pl?pz=117353" }     // info depth 12 seldepth 26 multipv 1 score cp 160 nodes 102915997 nps 439966 time 232918 pv d2f2 f3g3 f6e4 g3h4 f2f6 c7a6 b7a6 b2c3 b8c8 g4g5 f6f4 h4h5 e4c3, not reachable any more after adding RFP
    };

    public static readonly object[] Mates_in_11 = new object[]
    {
        new object[]{ "6r1/p1pq1p1p/1p1p1Qnk/3PrR2/2n1P1PP/P1P5/4R3/6K1 w - -           ", new[] { "f5h5" }, "https://talkchess.com/forum3/viewtopic.php?f=7&t=82648" },
    };
}
