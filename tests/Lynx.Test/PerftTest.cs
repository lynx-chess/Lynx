using NUnit.Framework;
using Lynx.Model;

namespace Lynx.Test;

/// <summary>
/// https://www.chessprogramming.org/Perft_Results
/// </summary>
[TestFixture(Category = Categories.Perft, Explicit = true)]
public class PerftTest
{
    [TestCase(Constants.InitialPositionFEN, 1, 20, Ignore = "Included in PerftTestSuite")]
    [TestCase(Constants.InitialPositionFEN, 2, 400, Ignore = "Included in PerftTestSuite")]
    [TestCase(Constants.InitialPositionFEN, 3, 8_902, Ignore = "Included in PerftTestSuite")]
    [TestCase(Constants.InitialPositionFEN, 4, 197_281, Ignore = "Included in PerftTestSuite")]
    [TestCase(Constants.InitialPositionFEN, 5, 4_865_609, Ignore = "Included in PerftTestSuite")]
    [TestCase(Constants.InitialPositionFEN, 6, 119_060_324, Ignore = "Included in PerftTestSuite")]
    [TestCase(Constants.InitialPositionFEN, 7, 3_195_901_860, Category = Categories.TooLong, Explicit = true)]
    [TestCase(Constants.InitialPositionFEN, 8, 84_998_978_956, Category = Categories.TooLong, Explicit = true)]
    [TestCase(Constants.InitialPositionFEN, 9, 2_439_530_234_167, Category = Categories.TooLong, Explicit = true)]
    public void InitialPosition(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    /// <summary>
    /// aka <see cref="Constants.TrickyTestPositionFEN"/>
    /// </summary>
    /// <param name="expectedNumberOfNodes"></param>
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 1, 48, Ignore = "Included in PerftTestSuite")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 2, 2_039, Ignore = "Included in PerftTestSuite")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 3, 97_862, Ignore = "Included in PerftTestSuite")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 4, 4_085_603, Ignore = "Included in PerftTestSuite")]
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 5, 193_690_690, Ignore = "Included in PerftTestSuite")]                                                      // 2m 30s
    [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 6, 8_031_647_685, Category = Categories.TooLong, Explicit = true)]    // 24 min
    public void Position2(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 1, 14)]
    [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 2, 191)]
    [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 3, 2_812)]
    [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 4, 43_238)]
    [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 5, 674_624)]
    [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 6, 11_030_083)]
    [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 7, 178_633_661)]
    [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 8, 3_009_794_393, Category = Categories.TooLong, Explicit = true)]   // 8 m 10 s
    public void Position3(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 1, 6)]
    [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 2, 264)]
    [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 3, 9_467)]
    [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 4, 422_333)]
    [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 5, 15_833_292)]
    [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 6, 706_045_033, Category = Categories.TooLong, Explicit = true)]    // 9m 6s
    public void Position4(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 1, 6)]
    [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 2, 264)]
    [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 3, 9_467)]
    [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 4, 422_333)]
    [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 5, 15_833_292)]
    [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 6, 706_045_033, Category = Categories.TooLong, Explicit = true)]
    public void Position4_mirrored(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 1, 44)]
    [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 2, 1_486)]
    [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 3, 62_379)]
    [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 4, 2_103_487)]
    [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 5, 89_941_194)]
    public void Position5(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 1, 46)]
    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 2, 2_079)]
    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 3, 89_890)]
    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 4, 3_894_594)]
    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 5, 164_075_551)]
    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 6, 6_923_051_137, Category = Categories.TooLong, Explicit = true)]
    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 7, 287_188_994_746, Category = Categories.TooLong, Explicit = true)]
    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 8, 11_923_589_843_526, Category = Categories.TooLong, Explicit = true)]
    [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 9, 490_154_852_788_714, Category = Categories.TooLong, Explicit = true)]
    public void Position6(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    /// <summary>
    /// By Martin Sedlak
    /// http://talkchess.com/forum3/viewtopic.php?t=47318
    /// </summary>
    [TestCase("8/5bk1/8/2Pp4/8/1K6/8/8 w - d6 0 1           ", 6, 824064, Description = "avoid illegal en passant capture")]
    [TestCase("8/8/1k6/8/2pP4/8/5BK1/8 b - d3 0 1           ", 6, 824064, Description = "avoid illegal en passant capture")]
    [TestCase("8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1          ", 6, 1440467, Description = "en passant capture checks opponent")]
    [TestCase("8/5k2/8/2Pp4/2B5/1K6/8/8 w - d6 0 1          ", 6, 1440467, Description = "en passant capture checks opponent")]
    [TestCase("5k2/8/8/8/8/8/8/4K2R w K - 0 1               ", 6, 661072, Description = "short castling gives check")]
    [TestCase("4k2r/8/8/8/8/8/8/5K2 b k - 0 1               ", 6, 661072, Description = "short castling gives check")]
    [TestCase("3k4/8/8/8/8/8/8/R3K3 w Q - 0 1               ", 6, 803711, Description = "long castling gives check")]
    [TestCase("r3k3/8/8/8/8/8/8/3K4 b q - 0 1               ", 6, 803711, Description = "long castling gives check")]
    [TestCase("r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1    ", 4, 1274206, Description = "castling (including losing cr due to rook capture)")]
    [TestCase("r3k2r/7b/8/8/8/8/1B4BQ/R3K2R b KQkq - 0 1    ", 4, 1274206, Description = "castling (including losing cr due to rook capture)")]
    [TestCase("r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1     ", 4, 1720476, Description = "castling prevented")]
    [TestCase("r3k2r/8/5Q2/8/8/3q4/8/R3K2R w KQkq - 0 1     ", 4, 1720476, Description = "castling prevented")]
    [TestCase("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1            ", 6, 3821001, Description = "promote out of check")]
    [TestCase("3K4/8/8/8/8/8/4p3/2k2R2 b - - 0 1            ", 6, 3821001, Description = "promote out of check")]
    [TestCase("8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1          ", 5, 1004658, Description = "discovered check")]
    [TestCase("5K2/8/1Q6/2N5/8/1p2k3/8/8 w - - 0 1          ", 5, 1004658, Description = "discovered check")]
    [TestCase("4k3/1P6/8/8/8/8/K7/8 w - - 0 1               ", 6, 217342, Description = "promote to give check")]
    [TestCase("8/k7/8/8/8/8/1p6/4K3 b - - 0 1               ", 6, 217342, Description = "promote to give check")]
    [TestCase("8/P1k5/K7/8/8/8/8/8 w - - 0 1                ", 6, 92683, Description = "underpromote to check")]
    [TestCase("8/8/8/8/8/k7/p1K5/8 b - - 0 1                ", 6, 92683, Description = "underpromote to check")]
    [TestCase("K1k5/8/P7/8/8/8/8/8 w - - 0 1                ", 6, 2217, Description = "self stalemate")]
    [TestCase("8/8/8/8/8/p7/8/k1K5 b - - 0 1                ", 6, 2217, Description = "self stalemate")]
    [TestCase("8/k1P5/8/1K6/8/8/8/8 w - - 0 1               ", 7, 567584, Description = "stalemate/checkmate")]
    [TestCase("8/8/8/8/1k6/8/K1p5/8 b - - 0 1               ", 7, 567584, Description = "stalemate/checkmate")]
    [TestCase("8/8/2k5/5q2/5n2/8/5K2/8 b - - 0 1            ", 4, 23527, Description = "double check")]
    [TestCase("8/5k2/8/5N2/5Q2/2K5/8/8 w - - 0 1            ", 4, 23527, Description = "double check")]
    public void MartinSedlakPositions(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    /// <summary>
    /// By John Merlino
    /// https://talkchess.com/forum3/viewtopic.php?topic_view=threads&p=509159&t=47318#p509159
    /// </summary>
    [TestCase("r3k2r/8/8/8/3pPp2/8/8/R3K1RR b KQkq e3 0 1                           ", 6, 485_647_607, Category = Categories.TooLong, Explicit = true)]  // 6m 30s
    [TestCase("8/7p/p5pb/4k3/P1pPn3/8/P5PP/1rB2RK1 b - d3 0 28                      ", 4, 67_197, Description = "resolving check by en-passant capture of the checking pawn")]
    [TestCase("8/7p/p5pb/4k3/P1pPn3/8/P5PP/1rB2RK1 b - d3 0 28                      ", 6, 38_633_283, Description = "resolving check by en-passant capture of the checking pawn")]
    [TestCase("8/3K4/2p5/p2b2r1/5k2/8/8/1q6 b - - 1 67                              ", 7, 493_407_574)]  // 5m 50s
    [TestCase("rnbqkb1r/ppppp1pp/7n/4Pp2/8/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 3        ", 6, 244_063_299)]  // 3m 45s
    [TestCase("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -                                     ", 8, 8_103_790)]
    [TestCase("n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - -                                  ", 6, 71_179_139)]   // 1m 12s
    [TestCase("r3k2r/p6p/8/B7/1pp1p3/3b4/P6P/R3K2R w KQkq -                         ", 6, 77_054_993)]   // 1m 25s
    [TestCase("8/5p2/8/2k3P1/p3K3/8/1P6/8 b - -                                     ", 8, 64_451_405)]   // 1m 20s
    [TestCase("r3k2r/pb3p2/5npp/n2p4/1p1PPB2/6P1/P2N1PBP/R3K2R w KQkq -             ", 5, 29_179_893)]   // 1m 25s
    public void JohnMerlinoPositions(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    /// <summary>
    /// By kz04px
    /// https://github.com/kz04px/libchess/blob/71c3543da7ad64387b618e889feef5406f1ee851/tests/perft.cpp
    /// https://github.com/kz04px/rawr/blob/1c97b8da895f919c1eb7af215b902599c09532b3/tests/perft_extra.rs
    /// </summary>
    // EP
    [TestCase("8/8/8/8/1k1PpN1R/8/8/4K3 b - d3 0 1  ", 1, 9, 193)]
    [TestCase("8/8/8/8/1k1Ppn1R/8/8/4K3 b - d3 0 1  ", 1, 17, 220)]
    [TestCase("4k3/8/8/2PpP3/8/8/8/4K3 w - d6 0 1   ", 1, 9, 47, 376)]
    [TestCase("4k3/8/8/8/2pPp3/8/8/4K3 b - d3 0 1   ", 1, 9, 47, 376)]
    [TestCase("r3k2r/p2pqpb1/bn2pnp1/2pPN3/1p2P3/2N2Q1p/PPPBBPPP/R4K1R w kq c6 0 2", 1, 46)]
    // EP - pinned diagonal
    [TestCase("4k3/b7/8/2Pp4/8/8/8/6K1 w - d6 0 1   ", 1, 5, 45)]
    [TestCase("4k3/7b/8/4pP2/8/8/8/1K6 w - e6 0 1   ", 1, 5, 45)]
    [TestCase("6k1/8/8/8/2pP4/8/B7/3K4 b - d3 0 1   ", 1, 5, 45)]
    [TestCase("1k6/8/8/8/4Pp2/8/7B/4K3 b - e3 0 1   ", 1, 5, 45)]
    [TestCase("4k3/b7/8/1pP5/8/8/8/6K1 w - b6 0 1   ", 1, 6, 52)]
    [TestCase("4k3/7b/8/5Pp1/8/8/8/1K6 w - g6 0 1   ", 1, 6, 51)]
    [TestCase("6k1/8/8/8/1Pp5/8/B7/4K3 b - b3 0 1   ", 1, 6, 52)]
    [TestCase("1k6/8/8/8/5pP1/8/7B/4K3 b - g3 0 1   ", 1, 6, 51)]
    [TestCase("4k3/K7/8/1pP5/8/8/8/6b1 w - b6 0 1   ", 1, 6, 66)]
    [TestCase("4k3/7K/8/5Pp1/8/8/8/1b6 w - g6 0 1   ", 1, 6, 60)]
    [TestCase("6B1/8/8/8/1Pp5/8/k7/4K3 b - b3 0 1   ", 1, 6, 66)]
    [TestCase("1B6/8/8/8/5pP1/8/7k/4K3 b - g3 0 1   ", 1, 6, 60)]
    [TestCase("4k3/b7/8/2Pp4/3K4/8/8/8 w - d6 0 1   ", 1, 5, 44)]
    [TestCase("4k3/8/1b6/2Pp4/3K4/8/8/8 w - d6 0 1  ", 1, 6, 59)]
    [TestCase("4k3/8/b7/1Pp5/2K5/8/8/8 w - c6 0 1   ", 1, 6, 49)]
    [TestCase("4k3/8/7b/5pP1/5K2/8/8/8 w - f6 0 1   ", 1, 6, 49)]
    [TestCase("4k3/7b/8/4pP2/4K3/8/8/8 w - e6 0 1   ", 1, 5, 44)]
    [TestCase("4k3/8/6b1/4pP2/4K3/8/8/8 w - e6 0 1  ", 1, 6, 53)]
    [TestCase("4k3/8/3K4/1pP5/8/q7/8/8 w - b6 0 1   ", 1, 5, 114)]
    [TestCase("7k/4K3/8/1pP5/8/q7/8/8 w - b6 0 1    ", 1, 8, 171)]
    // EP - double check
    [TestCase("4k3/2rn4/8/2K1pP2/8/8/8/8 w - e6 0 1 ", 1, 4, 75)]
    // EP - pinned horizontal
    [TestCase("4k3/8/8/K2pP2r/8/8/8/8 w - d6 0 1    ", 1, 6, 94)]
    [TestCase("4k3/8/8/K2pP2q/8/8/8/8 w - d6 0 1    ", 1, 6, 130)]
    [TestCase("4k3/8/8/r2pP2K/8/8/8/8 w - d6 0 1    ", 1, 6, 87)]
    [TestCase("4k3/8/8/q2pP2K/8/8/8/8 w - d6 0 1    ", 1, 6, 129)]
    [TestCase("8/8/8/8/1k1Pp2R/8/8/4K3 b - d3 0 1   ", 1, 8, 125)]
    [TestCase("8/8/8/8/1R1Pp2k/8/8/4K3 b - d3 0 1   ", 1, 6, 87)]
    // EP - pinned vertical
    [TestCase("k7/8/4r3/3pP3/8/8/8/4K3 w - d6 0 1   ", 1, 5, 70)]
    [TestCase("k3K3/8/8/3pP3/8/8/8/4r3 w - d6 0 1   ", 1, 6, 91)]
    // EP - in check
    [TestCase("4k3/8/8/4pP2/3K4/8/8/8 w - e6 0 1    ", 1, 9, 49)]
    [TestCase("8/8/8/4k3/5Pp1/8/8/3K4 b - f3 0 1    ", 1, 9, 50)]
    [TestCase("2b1k3/8/8/2Pp4/8/7K/8/8 w - - 0 1    ", 1, 4, 52)]
    [TestCase("2b1k3/8/8/2Pp4/8/7K/8/8 w - d6 0 1   ", 1, 4, 52)]
    [TestCase("4k3/r6K/8/2Pp4/8/8/8/8 w - - 0 1     ", 1, 4, 77)]
    [TestCase("4k3/r6K/8/2Pp4/8/8/8/8 w - d6 0 1    ", 1, 4, 77)]
    [TestCase("2K1k3/8/8/2Pp4/8/7b/8/8 w - - 0 1    ", 1, 3, 37)]
    [TestCase("2K1k3/8/8/2Pp4/8/7b/8/8 w - d6 0 1   ", 1, 3, 37)]
    [TestCase("2K1k3/8/8/2Pp4/8/7q/8/8 w - - 0 1    ", 1, 3, 79)]
    [TestCase("2K1k3/8/8/2Pp4/8/7q/8/8 w - d6 0 1   ", 1, 3, 79)]
    [TestCase("8/8/7k/8/2pP4/8/8/2B1K3 b - - 0 1    ", 1, 4, 52)]
    [TestCase("8/8/7k/8/2pP4/8/8/2B1K3 b - d3 0 1   ", 1, 4, 52)]
    [TestCase("8/8/7k/8/2pP4/8/8/2Q1K3 b - - 0 1    ", 1, 4, 76)]
    [TestCase("8/8/7k/8/2pP4/8/8/2Q1K3 b - d3 0 1   ", 1, 4, 76)]
    [TestCase("8/8/8/8/2pP4/8/R6k/4K3 b - - 0 1     ", 1, 4, 77)]
    [TestCase("8/8/8/8/2pP4/8/R6k/4K3 b - d3 0 1    ", 1, 4, 77)]
    // EP - block check
    [TestCase("4k3/8/K6r/3pP3/8/8/8/8 w - d6 0 1    ", 1, 6, 109)]
    [TestCase("4k3/8/K6q/3pP3/8/8/8/8 w - d6 0 1    ", 1, 6, 151)]
    [TestCase("4kb2/8/8/3pP3/8/K7/8/8 w - d6 0 1    ", 1, 5, 55)]
    [TestCase("4kq2/8/8/3pP3/8/K7/8/8 w - d6 0 1    ", 1, 5, 100)]
    [TestCase("4k3/8/r6K/3pP3/8/8/8/8 w - d6 0 1    ", 1, 6, 107)]
    [TestCase("4k3/8/q6K/3pP3/8/8/8/8 w - d6 0 1    ", 1, 6, 149)]
    [TestCase("3k1K2/8/8/3pP3/8/b7/8/8 w - d6 0 1   ", 1, 4, 44)]
    [TestCase("3k1K2/8/8/3pP3/8/q7/8/8 w - d6 0 1   ", 1, 4, 100)]
    [TestCase("8/8/8/8/3Pp3/k6R/8/4K3 b - d3 0 1    ", 1, 6, 109)]
    [TestCase("8/8/8/8/3Pp3/k6Q/8/4K3 b - d3 0 1    ", 1, 6, 151)]
    [TestCase("8/8/k7/8/3Pp3/8/8/4KB2 b - d3 0 1    ", 1, 5, 55)]
    [TestCase("8/8/k7/8/3Pp3/8/8/4KQ2 b - d3 0 1    ", 1, 5, 100)]
    // Double checks
    [TestCase("4k3/8/4r3/8/8/8/3p4/4K3 w - - 0 1    ", 1, 4, 80, 320)]
    [TestCase("4k3/8/4q3/8/8/8/3b4/4K3 w - - 0 1    ", 1, 4, 143, 496)]
    // Pins
    [TestCase("4k3/8/8/8/1b5b/8/3Q4/4K3 w - - 0 1   ", 1, 3, 54, 1256)]
    [TestCase("4k3/8/8/8/1b5b/8/3R4/4K3 w - - 0 1   ", 1, 3, 54, 836)]
    [TestCase("4k3/8/8/8/1b5b/2Q5/5P2/4K3 w - - 0 1 ", 1, 6, 98, 2274)]
    [TestCase("4k3/8/8/8/1b5b/2R5/5P2/4K3 w - - 0 1 ", 1, 4, 72, 1300)]
    [TestCase("4k3/8/8/8/1b2r3/8/3Q4/4K3 w - - 0 1  ", 1, 3, 66, 1390)]
    [TestCase("4k3/8/8/8/1b2r3/8/3QP3/4K3 w - - 0 1 ", 1, 6, 119, 2074)]
    public void Kz04pxPositions(string fen, long _, long nodesD1, long nodesD2 = 0, long nodesD3 = 0)
    {
        Validate(fen, 1, nodesD1);

        if (nodesD2 != 0)
        {
            Validate(fen, 2, nodesD2);
        }

        if (nodesD3 != 0)
        {
            Validate(fen, 3, nodesD3);
        }
    }

    /// <summary>
    /// martinn = Motor's author
    /// </summary>
    // Motor vs Avalanche
    [TestCase("6r1/2q2pp1/1PB2k2/3P1P2/5Q1B/8/6K1/7R b - - 0 1", 1, 1)]
    [TestCase("6r1/2q2pp1/1PB2k2/3P1P2/5Q1B/8/6K1/7R b - - 0 1", 2, 48)]
    [TestCase("6r1/2q2pp1/1PB2k2/3P1P2/5Q1B/8/6K1/7R b - - 0 1", 3, 1_060)]
    [TestCase("6r1/2q2pp1/1PB2k2/3P1P2/5Q1B/8/6K1/7R b - - 0 1", 4, 42_723)]
    [TestCase("6r1/2q2pp1/1PB2k2/3P1P2/5Q1B/8/6K1/7R b - - 0 1", 5, 981_168)]
    [TestCase("6r1/2q2pp1/1PB2k2/3P1P2/5Q1B/8/6K1/7R b - - 0 1", 6, 37_765_954)]
    [TestCase("6r1/2q2pp1/1PB2k2/3P1P2/5Q1B/8/6K1/7R b - - 0 1", 7, 891_192_699)]   // ~40s
    public void MartinnPositions(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    [Test]
    public void PositionWithHighestKnownNumberOfMoves() => Validate("R6R/3Q4/1Q4Q1/4Q3/2Q4Q/Q4Q2/pp1Q4/kBNN1KB1 w - - 0 1", 1, 218);

    private static void Validate(string fen, int depth, long expectedNumberOfNodes)
    {
        Assert.AreEqual(expectedNumberOfNodes, Perft.PerftRecursiveImpl(new Position(fen), depth, default));
    }
}
