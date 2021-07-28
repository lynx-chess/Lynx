using NUnit.Framework;
using Lynx.Model;

namespace Lynx.Test
{
    /// <summary>
    /// https://www.chessprogramming.org/Perft_Results
    /// </summary>
    public class PerftTest
    {
        [TestCase(Constants.InitialPositionFEN, 1, 20)]
        [TestCase(Constants.InitialPositionFEN, 2, 400)]
        [TestCase(Constants.InitialPositionFEN, 3, 8_902)]
        [TestCase(Constants.InitialPositionFEN, 4, 197_281)]
        [TestCase(Constants.InitialPositionFEN, 5, 4_865_609, Category = "LongRunning", Explicit = true)]
        [TestCase(Constants.InitialPositionFEN, 6, 119_060_324, Category = "LongRunning", Explicit = true)]
        [TestCase(Constants.InitialPositionFEN, 7, 3_195_901_860, Category = "ExtraLongRunning", Explicit = true)]
        [TestCase(Constants.InitialPositionFEN, 8, 84_998_978_956, Category = "ExtraLongRunning", Explicit = true)]
        [TestCase(Constants.InitialPositionFEN, 9, 2_439_530_234_167, Category = "ExtraLongRunning", Explicit = true)]
        public void InitialPosition(string fen, int depth, long expectedNumberOfNodes)
        {
            Validate(fen, depth, expectedNumberOfNodes);
        }

        [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 1, 48)]
        [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 2, 2_039)]
        [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 3, 97_862)]
        [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 4, 4_085_603, Category = "LongRunning", Explicit = true)]
        [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 5, 193_690_690, Category = "LongRunning", Explicit = true)]
        [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 6, 8_031_647_685, Category = "ExtraLongRunning", Explicit = true)]
        public void Position2(string fen, int depth, long expectedNumberOfNodes)
        {
            Validate(fen, depth, expectedNumberOfNodes);
        }

        [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 1, 14)]
        [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 2, 191)]
        [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 3, 2_812)]
        [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 4, 43_238)]
        [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 5, 674_624)]
        [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 6, 11_030_083, Category = "LongRunning", Explicit = true)]
        [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 7, 178_633_661, Category = "LongRunning", Explicit = true)]
        [TestCase("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -", 8, 3_009_794_393, Category = "ExtraLongRunning", Explicit = true)]
        public void Position3(string fen, int depth, long expectedNumberOfNodes)
        {
            Validate(fen, depth, expectedNumberOfNodes);
        }

        [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 1, 6)]
        [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 2, 264)]
        [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 3, 9_467)]
        [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 4, 422_333)]
        [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 5, 15_833_292, Category = "LongRunning", Explicit = true)]
        [TestCase("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 6, 706_045_033, Category = "ExtraLongRunning", Explicit = true)]
        public void Position4(string fen, int depth, long expectedNumberOfNodes)
        {
            Validate(fen, depth, expectedNumberOfNodes);
        }

        [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 1, 6)]
        [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 2, 264)]
        [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 3, 9_467)]
        [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 4, 422_333)]
        [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 5, 15_833_292, Category = "LongRunning", Explicit = true)]
        [TestCase("r2q1rk1/pP1p2pp/Q4n2/bbp1p3/Np6/1B3NBn/pPPP1PPP/R3K2R b KQ - 0 1", 6, 706_045_033, Category = "ExtraLongRunning", Explicit = true)]
        public void Position4_mirrored(string fen, int depth, long expectedNumberOfNodes)
        {
            Validate(fen, depth, expectedNumberOfNodes);
        }

        [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 1, 44)]
        [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 2, 1_486)]
        [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 3, 62_379)]
        [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 4, 2_103_487, Category = "LongRunning", Explicit = true)]
        [TestCase("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 5, 89_941_194, Category = "LongRunning", Explicit = true)]
        public void Position5(string fen, int depth, long expectedNumberOfNodes)
        {
            Validate(fen, depth, expectedNumberOfNodes);
        }

        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 1, 46)]
        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 2, 2_079)]
        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 3, 89_890)]
        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 4, 3_894_594, Category = "LongRunning", Explicit = true)]
        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 5, 164_075_551, Category = "LongRunning", Explicit = true)]
        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 6, 6_923_051_137, Category = "ExtraLongRunning", Explicit = true)]
        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 7, 287_188_994_746, Category = "ExtraLongRunning", Explicit = true)]
        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 8, 11_923_589_843_526, Category = "ExtraLongRunning", Explicit = true)]
        [TestCase("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 9, 490_154_852_788_714, Category = "ExtraLongRunning", Explicit = true)]
        public void Position6(string fen, int depth, long expectedNumberOfNodes)
        {
            Validate(fen, depth, expectedNumberOfNodes);
        }

        private static void Validate(string fen, int depth, long expectedNumberOfNodes)
        {
            Assert.AreEqual(expectedNumberOfNodes, Perft.ResultsImpl(new Position(fen), depth, default));
        }
    }
}
