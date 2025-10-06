using NUnit.Framework;
using Lynx.Model;

namespace Lynx.Test;

[TestFixture(Category = Categories.PerftFRC, Explicit = true)]
public class PerftFRCTest : BaseTest
{
    public PerftFRCTest()
    {
        Configuration.EngineSettings.IsChess960 = true;
    }

    /// <summary>
    /// https://github.com/toanth/motors/blob/main/gears/src/games/chess/perft_tests.rs
    /// </summary>
    [TestCase("2r1kr2/8/8/8/8/8/8/1R2K1R1 w GBfc - 0 1", 1, 22)]
    [TestCase("2r1kr2/8/8/8/8/8/8/1R2K1R1 w GBfc - 0 1", 2, 501)]
    [TestCase("2r1kr2/8/8/8/8/8/8/1R2K1R1 w GBfc - 0 1", 3, 11459)]
    [TestCase("rkr5/8/8/8/8/8/8/5RKR w HFca - 0 1", 1, 22)]
    [TestCase("rkr5/8/8/8/8/8/8/5RKR w HFca - 0 1", 2, 442)]
    [TestCase("rkr5/8/8/8/8/8/8/5RKR w HFca - 0 1", 3, 10217)]
    [TestCase("2r3kr/8/8/8/8/8/8/2KRR3 w h - 3 2", 1, 3)]
    [TestCase("2r3kr/8/8/8/8/8/8/2KRR3 w h - 3 2", 2, 72)]
    [TestCase("2r3kr/8/8/8/8/8/8/2KRR3 w h - 3 2", 3, 1371)]
    [TestCase("5rkr/8/8/8/8/8/8/RKR5 w CAhf - 0 1", 1, 22)]
    [TestCase("5rkr/8/8/8/8/8/8/RKR5 w CAhf - 0 1", 2, 442)]
    [TestCase("5rkr/8/8/8/8/8/8/RKR5 w CAhf - 0 1", 3, 10206)]
    [TestCase("3rkr2/8/8/8/8/8/8/R3K2R w HAfd - 0 1", 1, 20)]
    [TestCase("3rkr2/8/8/8/8/8/8/R3K2R w HAfd - 0 1", 2, 452)]
    [TestCase("3rkr2/8/8/8/8/8/8/R3K2R w HAfd - 0 1", 3, 9873)]
    [TestCase("4k3/8/8/8/8/8/8/4KR2 w F - 0 1", 1, 14)]
    [TestCase("4k3/8/8/8/8/8/8/4KR2 w F - 0 1", 2, 47)]
    [TestCase("4k3/8/8/8/8/8/8/4KR2 w F - 0 1", 3, 781)]
    [TestCase("4kr2/8/8/8/8/8/8/4K3 w f - 0 1", 1, 3)]
    [TestCase("4kr2/8/8/8/8/8/8/4K3 w f - 0 1", 2, 42)]
    [TestCase("4kr2/8/8/8/8/8/8/4K3 w f - 0 1", 3, 246)]
    [TestCase("4k3/8/8/8/8/8/8/2R1K3 w C - 0 1", 1, 16)]
    [TestCase("4k3/8/8/8/8/8/8/2R1K3 w C - 0 1", 2, 71)]
    [TestCase("4k3/8/8/8/8/8/8/2R1K3 w C - 0 1", 3, 1277)]
    [TestCase("2r1k3/8/8/8/8/8/8/4K3 w c - 0 1", 1, 5)]
    [TestCase("2r1k3/8/8/8/8/8/8/4K3 w c - 0 1", 2, 80)]
    [TestCase("2r1k3/8/8/8/8/8/8/4K3 w c - 0 1", 3, 448)]
    public void MotorsFRC(string fen, int depth, long expectedNumberOfNodes)
    {
        Configuration.EngineSettings.IsChess960 = true;

        Validate(fen, depth, expectedNumberOfNodes);
    }

    /// <summary>
    /// https://github.com/toanth/motors/blob/main/gears/src/games/chess/perft_tests.rs
    /// Only meant to make sure DFRC works assuming Chess960 and normal chess movegen already works.
    /// </summary>
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 1, 32)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 2, 1093)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 3, 34210)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 4, 1187103)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 5, 37188628)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 6, 1308319545, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 1, 31)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 2, 841)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 3, 23877)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 4, 711547)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 5, 20894205)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 6, 644033568, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    public void MotorsDFRC(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    /// <summary>
    /// https://github.com/toanth/motors/blob/main/gears/src/games/chess/perft_tests.rs
    /// Tricky to parse X-FENs
    /// </summary>
    [TestCase("r1rkrqnb/1b6/2n5/ppppppp1/PPPPPP2/B1NQ4/6PP/1K1RR1NB w Kkq - 8 14", 1, 42)]
    [TestCase("r1rkrqnb/1b6/2n5/ppppppp1/PPPPPP2/B1NQ4/6PP/1K1RR1NB w Kkq - 8 14", 2, 1620)]
    [TestCase("r1rkrqnb/1b6/2n5/ppppppp1/PPPPPP2/B1NQ4/6PP/1K1RR1NB w Kkq - 8 14", 3, 67391)]
    [TestCase("r1rkrqnb/1b6/2n5/ppppppp1/PPPPPP2/B1NQ4/6PP/1K1RR1NB w Kkq - 8 14", 4, 2592441)]
    [TestCase("r1rkrqnb/1b6/2n5/ppppppp1/PPPPPP2/B1NQ4/6PP/1K1RR1NB w Kkq - 8 14", 5, 107181922, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    [TestCase("rrkrn1r1/2P2P2/8/p3pP2/8/8/4R2p/1RR1KR1R w KCdq e6 90 99", 1, 53)]
    [TestCase("rrkrn1r1/2P2P2/8/p3pP2/8/8/4R2p/1RR1KR1R w KCdq e6 90 99", 2, 1349)]
    [TestCase("rrkrn1r1/2P2P2/8/p3pP2/8/8/4R2p/1RR1KR1R w KCdq e6 90 99", 3, 63522)]
    [TestCase("rrkrn1r1/2P2P2/8/p3pP2/8/8/4R2p/1RR1KR1R w KCdq e6 90 99", 4, 1754940)]
    [TestCase("rrkrn1r1/2P2P2/8/p3pP2/8/8/4R2p/1RR1KR1R w KCdq e6 90 99", 5, 80364051, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    [TestCase("rr2k1r1/p1p4P/1p3P2/8/1P6/3p4/7P/2RK1R1R w Kk - 0 1", 1, 29)]
    [TestCase("rr2k1r1/p1p4P/1p3P2/8/1P6/3p4/7P/2RK1R1R w Kk - 0 1", 2, 522)]
    [TestCase("rr2k1r1/p1p4P/1p3P2/8/1P6/3p4/7P/2RK1R1R w Kk - 0 1", 3, 14924)]
    [TestCase("rr2k1r1/p1p4P/1p3P2/8/1P6/3p4/7P/2RK1R1R w Kk - 0 1", 4, 277597)]
    [TestCase("rr2k1r1/p1p4P/1p3P2/8/1P6/3p4/7P/2RK1R1R w Kk - 0 1", 5, 8098755, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    [TestCase("8/2k5/8/8/8/8/8/RR1K1R1R w KB - 0 1", 1, 38)]
    [TestCase("8/2k5/8/8/8/8/8/RR1K1R1R w KB - 0 1", 2, 174)]
    [TestCase("8/2k5/8/8/8/8/8/RR1K1R1R w KB - 0 1", 3, 7530)]
    [TestCase("8/2k5/8/8/8/8/8/RR1K1R1R w KB - 0 1", 4, 35038)]
    [TestCase("8/2k5/8/8/8/8/8/RR1K1R1R w KB - 0 1", 5, 1620380)]
    [TestCase("8/2k5/8/8/8/8/8/RR1K1R1R w KB - 0 1", 6, 7173240, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    // pins
    [TestCase("1nbqkbnr/ppp1pppp/8/r2pP2K/8/8/PPPP1PPP/RNBQ1BNR w k d6 0 2", 1, 31)]
    [TestCase("1nbqkbnr/ppp1pppp/8/r2pP2K/8/8/PPPP1PPP/RNBQ1BNR w k d6 0 2", 2, 927)]
    [TestCase("1nbqkbnr/ppp1pppp/8/r2pP2K/8/8/PPPP1PPP/RNBQ1BNR w k d6 0 2", 3, 26832)]
    [TestCase("1nbqkbnr/ppp1pppp/8/r2pP2K/8/8/PPPP1PPP/RNBQ1BNR w k d6 0 2", 4, 813632)]
    [TestCase("1nbqkbnr/ppp1pppp/8/r2pP2K/8/8/PPPP1PPP/RNBQ1BNR w k d6 0 2", 5, 23977743, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    [TestCase("2k5/3q4/8/8/3B4/3K1B1r/8/8 w - - 0 1", 1, 7)]
    [TestCase("2k5/3q4/8/8/3B4/3K1B1r/8/8 w - - 0 1", 2, 211)]
    [TestCase("2k5/3q4/8/8/3B4/3K1B1r/8/8 w - - 0 1", 3, 4246)]
    [TestCase("2k5/3q4/8/8/3B4/3K1B1r/8/8 w - - 0 1", 4, 138376)]
    [TestCase("2k5/3q4/8/8/3B4/3K1B1r/8/8 w - - 0 1", 5, 2611571)]
    [TestCase("2k5/3q4/8/8/3B4/3K1B1r/8/8 w - - 0 1", 6, 85530145, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    // castling through check
    [TestCase("2r3kr/8/8/8/8/8/8/RK2R3 w Qk - 0 1", 1, 21)]
    [TestCase("2r3kr/8/8/8/8/8/8/RK2R3 w Qk - 0 1", 2, 447)]
    [TestCase("2r3kr/8/8/8/8/8/8/RK2R3 w Qk - 0 1", 3, 9933)]
    [TestCase("2r3kr/8/8/8/8/8/8/RK2R3 w Qk - 0 1", 4, 226424)]
    [TestCase("2r3kr/8/8/8/8/8/8/RK2R3 w Qk - 0 1", 5, 5338161)]
    [TestCase("2r3kr/8/8/8/8/8/8/RK2R3 w Qk - 0 1", 6, 126787151, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    public void MotorsXFENS(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    [TestCase("rbnnbqkr/pppppppp/8/8/8/2P5/PP1PPPPP/RBNNBQKR b KQkq - 0 1", 5, 6112904)]
    [TestCase("4r1kr/8/pppppppp/8/8/PPPPPPPP/8/RK2R3 b EAhe - 0 1", 4, 193643)]
    [TestCase("4r1kr/8/pppppppp/8/8/PPPPPPPP/8/RK2R3 b EAhe - 0 1", 6, 89392187, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    [TestCase("4r1kr/8/pppppppp/8/8/PPPPPPPP/8/RK5R b HAhe - 0 1", 4, 198885)]
    [TestCase("4r1kr/8/pppppppp/8/8/PPPPPPPP/8/RK5R b HAhe - 0 1", 6, 93963364, Category = Categories.PerftFRCExhaustive, Explicit = true)]
    [TestCase("rnkrbbqn/pppp1ppp/8/4p3/4P3/3P4/PPP2PPP/NNQRKRBB b FDda - 0 1", 5, 8791662)]
    [TestCase("rnkrbbqn/pppp1ppp/8/4p3/4P3/3P4/PPP2PPP/NNQRKRBB b KQkq - 0 1", 5, 8791662)]
    public void LynxFRCTests(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    [Explicit]
    [Category(Categories.LongRunning)]
    [TestCase("rnkrbbqn/pppp1ppp/8/4p3/4P3/3P4/PPP2PPP/NNQRKRBB b KQkq - 0 1", 15)]
    [TestCase("rnkrbbqn/pppp1ppp/8/4p3/4P3/3P4/PPP2PPP/NNQRKRBB b FDda - 0 1", 15)]
    [TestCase("bnqbrnkr/p1p2pp1/1p6/4p3/4N2p/3P4/PPPB1PPP/RQ1K1RNB w kq e6 1 1", 17)]
    [TestCase("bnqbrnkr/p1p2pp1/1p6/4p3/4N2p/3P4/PPPB1PPP/RQ1K1RNB w he e6 1 1", 17)]
    public void XFENParsingWhenFRCEnabled(string fen, int depth)
    {
        var engine = GetEngine();

        engine.AdjustPosition($"position fen {fen}");

        Assert.DoesNotThrow(() => engine.BestMove(new($"go depth {depth}")));
    }

    private static void Validate(string fen, int depth, long expectedNumberOfNodes)
    {
        Assert.AreEqual(expectedNumberOfNodes, Perft.PerftRecursiveImpl(new Position(fen), depth, default));
    }
}
