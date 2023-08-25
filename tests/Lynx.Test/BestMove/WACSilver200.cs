// WACData source: http://www.talkchess.com/forum3/viewtopic.php?f=2&t=67469",

using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test.BestMove;

public class WACSilver200 : BaseTest
{
    [Explicit]
    [Category(nameof(WinningAtChess_10seconds))]
    [TestCaseSource(typeof(WACData), nameof(WACData.Data))]
    /// <summary>
    /// 10s, see first case of <see cref="TimeManagementTest"/>
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="bestMove"></param>
    public async Task WinningAtChess_10seconds(string fen, string bestMove, string id)
    {
        await VerifyBestMove(fen, bestMove, id, new GoCommand($"go btime {2_000} wtime {2_000} winc {11_111} binc {11_111} movestogo {1}"));
    }

    [Explicit]
    [Category(nameof(WinningAtChess_DefaultSearchDepth))]
    [TestCaseSource(typeof(WACData), nameof(WACData.Data))]
    /// <summary>
    /// 10s, see first case of <see cref="TimeManagementTest"/>
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="bestMove"></param>
    public async Task WinningAtChess_DefaultSearchDepth(string fen, string bestMove, string id)
    {
        await VerifyBestMove(fen, bestMove, id, new GoCommand($"go depth {DefaultSearchDepth}"));
    }

    private static async Task VerifyBestMove(string fen, string bestMove, string id, GoCommand goCommand)
    {
        var engine = GetEngine(fen);

        var bestResult = await engine.BestMove(goCommand);

        var bestMoveArray = bestMove.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (bestMoveArray.Length == 1)
        {
            var expectedMove = bestMoveArray[0].TrimEnd('+');
            Assert.AreEqual(expectedMove, bestResult.BestMove.ToEPDString(), $"id {id} depth {bestResult.Depth} seldepth {bestResult.Depth} nodes {bestResult.Nodes}");
        }
        else if (bestMoveArray.Length == 2)
        {
            var bestResultGot = bestResult.BestMove.ToEPDString();
            Assert.True(
                bestMoveArray[0].TrimEnd('+') == bestResultGot
                || bestMoveArray[1].TrimEnd('+') == bestResultGot
                , $"id {id} Expected {bestMove} but got {bestResultGot} " +
                $"depth {bestResult.Depth} seldepth {bestResult.Depth} nodes {bestResult.Nodes}");
        }
        else
        {
            Assert.Fail();
        }
    }

    private static class WACData
    {
        public static object[] Data = new object[]
        {
            new object[] {"5rk1/1ppb3p/p1pb4/6q1/3P1p1r/2P1R2P/PP1BQ1P1/5RKN w - -              ", "Rg3", "WAC.003"},
            new object[] {"r1bq2rk/pp3pbp/2p1p1pQ/7P/3P4/2PB1N2/PP3PPR/2KR4 w - -               ", "Qxh7+", "WAC.004"},
            new object[] {"5k2/6pp/p1qN4/1p1p4/3P4/2PKP2Q/PP3r2/3R4 b - -                       ", "Qc4+", "WAC.005"},
            new object[] {"7k/p7/1R5K/6r1/6p1/6P1/8/8 w - -                                     ", "Rb7", "WAC.006"},
            new object[] {"rnbqkb1r/pppp1ppp/8/4P3/6n1/7P/PPPNPPP1/R1BQKBNR b KQkq -            ", "Ne3", "WAC.007"},
            new object[] {"r4q1k/p2bR1rp/2p2Q1N/5p2/5p2/2P5/PP3PPP/R5K1 w - -                   ", "Rf7", "WAC.008"},
            new object[] {"2br2k1/2q3rn/p2NppQ1/2p1P3/Pp5R/4P3/1P3PPP/3R2K1 w - -               ", "Rxh7", "WAC.010"},
            new object[] {"r1b1kb1r/3q1ppp/pBp1pn2/8/Np3P2/5B2/PPP3PP/R2Q1RK1 w kq -            ", "Bxc6", "WAC.011"},
            new object[] {"4k1r1/2p3r1/1pR1p3/3pP2p/3P2qP/P4N2/1PQ4P/5R1K b - -                 ", "Qxf3+", "WAC.012"},
            new object[] {"5rk1/pp4p1/2n1p2p/2Npq3/2p5/6P1/P3P1BP/R4Q1K w - -                   ", "Qxf8+", "WAC.013"},
            new object[] {"r2rb1k1/pp1q1p1p/2n1p1p1/2bp4/5P2/PP1BPR1Q/1BPN2PP/R5K1 w - -        ", "Qxh7+", "WAC.014"},
            new object[] {"r4rk1/ppp2ppp/2n5/2bqp3/8/P2PB3/1PP1NPPP/R2Q1RK1 w - -               ", "Nc3", "WAC.016"},
            new object[] {"1k5r/pppbn1pp/4q1r1/1P3p2/2NPp3/1QP5/P4PPP/R1B1R1K1 w - -            ", "Ne5", "WAC.017"},
            new object[] {"R7/P4k2/8/8/8/8/r7/6K1 w - -                                         ", "Rh8", "WAC.018"},
            new object[] {"r1b2rk1/ppbn1ppp/4p3/1QP4q/3P4/N4N2/5PPP/R1B2RK1 w - -               ", "c6", "WAC.019"},
            new object[] {"r2qkb1r/1ppb1ppp/p7/4p3/P1Q1P3/2P5/5PPP/R1B2KNR b kq -               ", "Bb5", "WAC.020"},
            new object[] {"5rk1/1b3p1p/pp3p2/3n1N2/1P6/P1qB1PP1/3Q3P/4R1K1 w - -                ", "Qh6", "WAC.021"},
            new object[] {"r1bqk2r/ppp1nppp/4p3/n5N1/2BPp3/P1P5/2P2PPP/R1BQK2R w KQkq -         ", "Nxf7", "WAC.022"},
            new object[] {"r3nrk1/2p2p1p/p1p1b1p1/2NpPq2/3R4/P1N1Q3/1PP2PPP/4R1K1 w - -         ", "g4", "WAC.023"},
            new object[] {"6k1/1b1nqpbp/pp4p1/5P2/1PN5/4Q3/P5PP/1B2B1K1 b - -                   ", "Bd4", "WAC.024"},
            new object[] {"3R1rk1/8/5Qpp/2p5/2P1p1q1/P3P3/1P2PK2/8 b - -                        ", "Qh4+", "WAC.025"},
            new object[] {"3r2k1/1p1b1pp1/pq5p/8/3NR3/2PQ3P/PP3PP1/6K1 b - -                    ", "Bf5", "WAC.026"},
            new object[] {"7k/pp4np/2p3p1/3pN1q1/3P4/Q7/1r3rPP/2R2RK1 w - -                     ", "Qf8+", "WAC.027"},
            new object[] {"1r1r2k1/4pp1p/2p1b1p1/p3R3/RqBP4/4P3/1PQ2PPP/6K1 b - -               ", "Qe1+", "WAC.028"},
            new object[] {"r2q2k1/pp1rbppp/4pn2/2P5/1P3B2/6P1/P3QPBP/1R3RK1 w - -               ", "c6", "WAC.029"},
            new object[] {"1r3r2/4q1kp/b1pp2p1/5p2/pPn1N3/6P1/P3PPBP/2QRR1K1 w - -              ", "Nxd6", "WAC.030"},
            new object[] {"6k1/p4p1p/1p3np1/2q5/4p3/4P1N1/PP3PPP/3Q2K1 w - -                    ", "Qd8+", "WAC.032"},
            new object[] {"7k/1b1r2p1/p6p/1p2qN2/3bP3/3Q4/P5PP/1B1R3K b - -                     ", "Bg1", "WAC.034"},
            new object[] {"r3r2k/2R3pp/pp1q1p2/8/3P3R/7P/PP3PP1/3Q2K1 w - -                     ", "Rxh7+", "WAC.035"},
            new object[] {"3r4/2p1rk2/1pQq1pp1/7p/1P1P4/P4P2/6PP/R1R3K1 b - -                   ", "Re1+", "WAC.036"},
            new object[] {"2r5/2rk2pp/1pn1pb2/pN1p4/P2P4/1N2B3/nPR1KPPP/3R4 b - -               ", "Nxd4+", "WAC.037"},
            new object[] {"r1br2k1/pp2bppp/2nppn2/8/2P1PB2/2N2P2/PqN1B1PP/R2Q1R1K w - -         ", "Na4", "WAC.039"},
            new object[] {"3r1r1k/1p4pp/p4p2/8/1PQR4/6Pq/P3PP2/2R3K1 b - -                      ", "Rc8", "WAC.040"},
            new object[] {"r1b1r1k1/pp1n1pbp/1qp3p1/3p4/1B1P4/Q3PN2/PP2BPPP/R4RK1 w - -         ", "Ba5", "WAC.042"},
            new object[] {"3rb1k1/pq3pbp/4n1p1/3p4/2N5/2P2QB1/PP3PPP/1B1R2K1 b - -              ", "dxc4", "WAC.044"},
            new object[] {"7k/2p1b1pp/8/1p2P3/1P3r2/2P3Q1/1P5P/R4qBK b - -                      ", "Qxa1", "WAC.045"},
            new object[] {"r1bqr1k1/pp1nb1p1/4p2p/3p1p2/3P4/P1N1PNP1/1PQ2PP1/3RKB1R w K -       ", "Nb5", "WAC.046"},
            new object[] {"r1b2rk1/pp2bppp/2n1pn2/q5B1/2BP4/2N2N2/PP2QPPP/2R2RK1 b - -          ", "Nxd4", "WAC.047"},
            new object[] {"1rbq1rk1/p1p1bppp/2p2n2/8/Q1BP4/2N5/PP3PPP/R1B2RK1 b - -             ", "Rb4", "WAC.048"},
            new object[] {"k4r2/1R4pb/1pQp1n1p/3P4/5p1P/3P2P1/r1q1R2K/8 w - -                   ", "Rxb6+", "WAC.050"},
            new object[] {"r1bq1r2/pp4k1/4p2p/3pPp1Q/3N1R1P/2PB4/6P1/6K1 w - -                  ", "Rg4+", "WAC.051"},
            new object[] {"6k1/6p1/p7/3Pn3/5p2/4rBqP/P4RP1/5QK1 b - -                           ", "Re1", "WAC.053"},
            new object[] {"r3kr2/1pp4p/1p1p4/7q/4P1n1/2PP2Q1/PP4P1/R1BB2K1 b q -                ", "Qh1+", "WAC.054"},
            new object[] {"r3r1k1/pp1q1pp1/4b1p1/3p2B1/3Q1R2/8/PPP3PP/4R1K1 w - -               ", "Qxg7+", "WAC.055"},
            new object[] {"r1bqk2r/pppp1ppp/5n2/2b1n3/4P3/1BP3Q1/PP3PPP/RNB1K1NR b KQkq -       ", "Bxf2+", "WAC.056"},
            new object[] {"r3q1kr/ppp5/3p2pQ/8/3PP1b1/5R2/PPP3P1/5RK1 w - -                     ", "Rf8+", "WAC.057"},
            new object[] {"8/8/2R5/1p2qp1k/1P2r3/2PQ2P1/5K2/8 w - -                             ", "Qd1+", "WAC.058"},
            new object[] {"r1b2rk1/2p1qnbp/p1pp2p1/5p2/2PQP3/1PN2N1P/PB3PP1/3R1RK1 w - -        ", "Nd5", "WAC.059"},
            new object[] {"6r1/3Pn1qk/p1p1P1rp/2Q2p2/2P5/1P4P1/P3R2P/5RK1 b - -                 ", "Rxg3+", "WAC.062"},
            new object[] {"r1brnbk1/ppq2pp1/4p2p/4N3/3P4/P1PB1Q2/3B1PPP/R3R1K1 w - -            ", "Nxf7", "WAC.063"},
            new object[] {"8/6pp/3q1p2/3n1k2/1P6/3NQ2P/5PP1/6K1 w - -", "g4+                    ", "WAC.064"},
            new object[] {"1r1r1qk1/p2n1p1p/bp1Pn1pQ/2pNp3/2P2P1N/1P5B/P6P/3R1RK1 w - -         ", "Ne7+", "WAC.065"},
            new object[] {"1k1r2r1/ppq5/1bp4p/3pQ3/8/2P2N2/PP4P1/R4R1K b - -                    ", "Qxe5", "WAC.066"},
            new object[] {"3r2k1/p2q4/1p4p1/3rRp1p/5P1P/6PK/P3R3/3Q4 w - -                      ", "Rxd5", "WAC.067"},
            new object[] {"6k1/5ppp/1q6/2b5/8/2R1pPP1/1P2Q2P/7K w - -                           ", "Qxe3", "WAC.068"},
            new object[] {"2kr3r/pppq1ppp/3p1n2/bQ2p3/1n1PP3/1PN1BN1P/1PP2PP1/2KR3R b - -       ", "Na2+", "WAC.070"},
            new object[] {"2kr3r/pp1q1ppp/5n2/1Nb5/2Pp1B2/7Q/P4PPP/1R3RK1 w - -                 ", "Nxa7+", "WAC.071"},
            new object[] {"r3r1k1/pp1n1ppp/2p5/4Pb2/2B2P2/B1P5/P5PP/R2R2K1 w - -                ", "e6", "WAC.072"},
            new object[] {"r1q3rk/1ppbb1p1/4Np1p/p3pP2/P3P3/2N4R/1PP1Q1PP/3R2K1 w - -           ", "Qd2", "WAC.073"},
            new object[] {"r3r1k1/pppq1ppp/8/8/1Q4n1/7P/PPP2PP1/RNB1R1K1 b - -                  ", "Qd6", "WAC.075"},
            new object[] {"r1b1qrk1/2p2ppp/pb1pnn2/1p2pNB1/3PP3/1BP5/PP2QPPP/RN1R2K1 w - -      ", "Bxf6", "WAC.076"},
            new object[] {"3r2k1/ppp2ppp/6q1/b4n2/3nQB2/2p5/P4PPP/RN3RK1 b - -                  ", "Ng3", "WAC.077"},
            new object[] {"r2q3r/ppp2k2/4nbp1/5Q1p/2P1NB2/8/PP3P1P/3RR1K1 w - -                 ", "Ng5+", "WAC.078"},
            new object[] {"r4rk1/p1B1bpp1/1p2pn1p/8/2PP4/3B1P2/qP2QP1P/3R1RK1 w - -             ", "Ra1", "WAC.080"},
            new object[] {"r4rk1/1bR1bppp/4pn2/1p2N3/1P6/P3P3/4BPPP/3R2K1 b - -                 ", "Bd6", "WAC.081"},
            new object[] {"3rr1k1/pp3pp1/4b3/8/2P1B2R/6QP/P3q1P1/5R1K w - -                     ", "Bh7+", "WAC.082"},
            new object[] {"r2q1r1k/2p1b1pp/p1n5/1p1Q1bN1/4n3/1BP1B3/PP3PPP/R4RK1 w - -          ", "Qg8+", "WAC.084"},
            new object[] {"8/p7/1ppk1n2/5ppp/P1PP4/2P1K1P1/5N1P/8 b - -                         ", "Ng4+", "WAC.086"},
            new object[] {"8/p3k1p1/4r3/2ppNpp1/PP1P4/2P3KP/5P2/8 b - -                         ", "Rxe5", "WAC.087"},
            new object[] {"r6k/p1Q4p/2p1b1rq/4p3/B3P3/4P3/PPP3P1/4RRK1 b - -                    ", "Rxg2+", "WAC.088"},
            new object[] {"3qrrk1/1pp2pp1/1p2bn1p/5N2/2P5/P1P3B1/1P4PP/2Q1RRK1 w - -            ", "Nxg7", "WAC.090"},
            new object[] {"2qr2k1/4b1p1/2p2p1p/1pP1p3/p2nP3/PbQNB1PP/1P3PK1/4RB2 b - -          ", "Be6", "WAC.091"},
            new object[] {"r4rk1/1p2ppbp/p2pbnp1/q7/3BPPP1/2N2B2/PPP4P/R2Q1RK1 b - -            ", "Bxg4", "WAC.092"},
            new object[] {"r1b1k1nr/pp3pQp/4pq2/3pn3/8/P1P5/2P2PPP/R1B1KBNR w KQkq -            ", "Bh6", "WAC.093"},
            new object[] {"8/k7/p7/3Qp2P/n1P5/3KP3/1q6/8 b - -                                  ", "e4+", "WAC.094"},
            new object[] {"2r5/1r6/4pNpk/3pP1qp/8/2P1QP2/5PK1/R7 w - -                          ", "Ng4+", "WAC.095"},
            new object[] {"6k1/5p2/p5np/4B3/3P4/1PP1q3/P3r1QP/6RK w - -                         ", "Qa8+", "WAC.097"},
            new object[] {"1r3rk1/5pb1/p2p2p1/Q1n1q2p/1NP1P3/3p1P1B/PP1R3P/1K2R3 b - -          ", "Nxe4", "WAC.098"},
            new object[] {"r1bq1r1k/1pp1Np1p/p2p2pQ/4R3/n7/8/PPPP1PPP/R1B3K1 w - -              ", "Rh5", "WAC.099"},
            new object[] {"5rk1/p5pp/8/8/2Pbp3/1P4P1/7P/4RN1K b - -                             ", "Bc3", "WAC.101"},
            new object[] {"6k1/2pb1r1p/3p1PpQ/p1nPp3/1q2P3/2N2P2/PrB5/2K3RR w - -               ", "Qxg6+", "WAC.103"},
            new object[] {"5n2/pRrk2p1/P4p1p/4p3/3N4/5P2/6PP/6K1 w - -                          ", "Nb5", "WAC.107"},
            new object[] {"r5k1/1q4pp/2p5/p1Q5/2P5/5R2/4RKPP/r7 w - -                           ", "Qe5", "WAC.108"},
            new object[] {"rn2k1nr/pbp2ppp/3q4/1p2N3/2p5/QP6/PB1PPPPP/R3KB1R b KQkq -           ", "c3", "WAC.109"},
            new object[] {"2kr4/bp3p2/p2p2b1/P7/2q5/1N4B1/1PPQ2P1/2KR4 b - -                    ", "Be3", "WAC.110"},
            new object[] {"6k1/p5p1/5p2/2P2Q2/3pN2p/3PbK1P/7P/6q1 b - -                         ", "Qf1+", "WAC.111"},
            new object[] {"r4kr1/ppp5/4bq1b/7B/2PR1Q1p/2N3P1/PP3P1P/2K1R3 w - -                 ", "Rxe6", "WAC.112"},
            new object[] {"rnbqkb1r/1p3ppp/5N2/1p2p1B1/2P5/8/PP2PPPP/R2QKB1R b KQkq -           ", "Qxf6", "WAC.113"},
            new object[] {"r1b1rnk1/1p4pp/p1p2p2/3pN2n/3P1PPq/2NBPR1P/PPQ5/2R3K1 w - -          ", "Bxh7+", "WAC.114"},
            new object[] {"4N2k/5rpp/1Q6/p3q3/8/P5P1/1P3P1P/5K2 w - -                           ", "Nd6", "WAC.115"},
            new object[] {"r2r2k1/2p2ppp/p7/1p2P1n1/P6q/5P2/1PB1QP1P/R5RK b - -                 ", "Rd2", "WAC.116"},
            new object[] {"3r1rk1/q4ppp/p1Rnp3/8/1p6/1N3P2/PP3QPP/3R2K1 b - -                   ", "Ne4", "WAC.117"},
            new object[] {"r5k1/pb2rpp1/1p6/2p4q/5R2/2PB2Q1/P1P3PP/5R1K w - -                   ", "Rh4", "WAC.118"},
            new object[] {"r2qr1k1/p1p2ppp/2p5/2b5/4nPQ1/3B4/PPP3PP/R1B2R1K b - -               ", "Qxd3", "WAC.119"},
            new object[] {"6k1/5p1p/2bP2pb/4p3/2P5/1p1pNPPP/1P1Q1BK1/1q6 b - -                  ", "Bxf3+", "WAC.121"},
            new object[] {"1k6/ppp4p/1n2pq2/1N2Rb2/2P2Q2/8/P4KPP/3r1B2 b - -                    ", "Rxf1+", "WAC.122"},
            new object[] {"6k1/3r4/2R5/P5P1/1P4p1/8/4rB2/6K1 b - -                              ", "g3", "WAC.124"},
            new object[] {"r1bqr1k1/pp3ppp/1bp5/3n4/3B4/2N2P1P/PPP1B1P1/R2Q1RK1 b - -           ", "Bxd4+", "WAC.125"},
            new object[] {"r5r1/pQ5p/1qp2R2/2k1p3/4P3/2PP4/P1P3PP/6K1 w - -                     ", "Rxc6+", "WAC.126"},
            new object[] {"2k4r/1pr1n3/p1p1q2p/5pp1/3P1P2/P1P1P3/1R2Q1PP/1RB3K1 w - -           ", "Rxb7", "WAC.127"},
            new object[] {"6rk/1pp2Qrp/3p1B2/1pb1p2R/3n1q2/3P4/PPP3PP/R6K w - -                 ", "Qg6", "WAC.128"},
            new object[] {"3r1r1k/1b2b1p1/1p5p/2p1Pp2/q1B2P2/4P2P/1BR1Q2K/6R1 b - -             ", "Bf3", "WAC.129"},
            new object[] {"6k1/1pp3q1/5r2/1PPp4/3P1pP1/3Qn2P/3B4/4R1K1 b - -                    ", "Qh6", "WAC.130"},
            new object[] {"r1b1k2r/1pp1q2p/p1n3p1/3QPp2/8/1BP3B1/P5PP/3R1RK1 w kq -             ", "Bh4", "WAC.133"},
            new object[] {"3r2k1/p6p/2Q3p1/4q3/2P1p3/P3Pb2/1P3P1P/2K2BR1 b - -                  ", "Rd1+", "WAC.134"},
            new object[] {"3r1r1k/N2qn1pp/1p2np2/2p5/2Q1P2N/3P4/PP4PP/3R1RK1 b - -              ", "Nd4", "WAC.135"},
            new object[] {"3b1rk1/1bq3pp/5pn1/1p2rN2/2p1p3/2P1B2Q/1PB2PPP/R2R2K1 w - -          ", "Rd7", "WAC.137"},
            new object[] {"r1bq3r/ppppR1p1/5n1k/3P4/6pP/3Q4/PP1N1PP1/5K1R w - -                 ", "h5", "WAC.138"},
            new object[] {"rnb3kr/ppp2ppp/1b6/3q4/3pN3/Q4N2/PPP2KPP/R1B1R3 w - -                ", "Nf6+", "WAC.139"},
            new object[] {"r2b1rk1/pq4p1/4ppQP/3pB1p1/3P4/2R5/PP3PP1/5RK1 w - -                 ", "Bc7 Rc7", "WAC.140"},
            new object[] {"4r1k1/p1qr1p2/2pb1Bp1/1p5p/3P1n1R/1B3P2/PP3PK1/2Q4R w - -            ", "Qxf4", "WAC.141"},
            new object[] {"5b2/pp2r1pk/2pp1pRp/4rP1N/2P1P3/1P4QP/P3q1P1/5R1K w - -              ", "Rxh6+", "WAC.143"},
            new object[] {"r2q1rk1/pp3ppp/2p2b2/8/B2pPPb1/7P/PPP1N1P1/R2Q1RK1 b - -             ", "d3", "WAC.144"},
            new object[] {"r2r2k1/ppqbppbp/2n2np1/2pp4/6P1/1P1PPNNP/PBP2PB1/R2QK2R b KQ -       ", "Nxg4", "WAC.147"},
            new object[] {"2r1k3/6pr/p1nBP3/1p3p1p/2q5/2P5/P1R4P/K2Q2R1 w - -                   ", "Rxg7", "WAC.148"},
            new object[] {"6k1/6p1/2p4p/4Pp2/4b1qP/2Br4/1P2RQPK/8 b - -                         ", "Bxg2", "WAC.149"},
            new object[] {"8/3b2kp/4p1p1/pr1n4/N1N4P/1P4P1/1K3P2/3R4 w - -                      ", "Nc3", "WAC.151"},
            new object[] {"1br2rk1/1pqb1ppp/p3pn2/8/1P6/P1N1PN1P/1B3PP1/1QRR2K1 w - -           ", "Ne4", "WAC.152"},
            new object[] {"r1b2rk1/2p2ppp/p7/1p6/3P3q/1BP3bP/PP3QP1/RNB1R1K1 w - -              ", "Qxf7+", "WAC.154"},
            new object[] {"5bk1/1rQ4p/5pp1/2pP4/3n1PP1/7P/1q3BB1/4R1K1 w - -                    ", "d6", "WAC.155"},
            new object[] {"r1b1qN1k/1pp3p1/p2p3n/4p1B1/8/1BP4Q/PP3KPP/8 w - -                   ", "Qxh6+", "WAC.156"},
            new object[] {"5rk1/p4ppp/2p1b3/3Nq3/4P1n1/1p1B2QP/1PPr2P1/1K2R2R w - -             ", "Ne7+", "WAC.157"},
            new object[] {"r1b2r2/5P1p/ppn3pk/2p1p1Nq/1bP1PQ2/3P4/PB4BP/1R3RK1 w - -            ", "Ne6+", "WAC.159"},
            new object[] {"r3kbnr/p4ppp/2p1p3/8/Q1B3b1/2N1B3/PP3PqP/R3K2R w KQkq -              ", "Bd5", "WAC.162"},
            new object[] {"5rk1/2p4p/2p4r/3P4/4p1b1/1Q2NqPp/PP3P1K/R4R2 b - -                   ", "Qg2+", "WAC.163"},
            new object[] {"8/6pp/4p3/1p1n4/1NbkN1P1/P4P1P/1PR3K1/r7 w - -                       ", "Rxc4+", "WAC.164"},
            new object[] {"1r5k/p1p3pp/8/8/4p3/P1P1R3/1P1Q1qr1/2KR4 w - -                       ", "Re2", "WAC.165"},
            new object[] {"r3r1k1/5pp1/p1p4p/2Pp4/8/q1NQP1BP/5PP1/4K2R b K -                    ", "d4", "WAC.166"},
            new object[] {"r3k2r/pb1q1p2/8/2p1pP2/4p1p1/B1P1Q1P1/P1P3K1/R4R2 b kq -             ", "Qd2+", "WAC.168"},
            new object[] {"5rk1/1pp3bp/3p2p1/2PPp3/1P2P3/2Q1B3/4q1PP/R5K1 b - -                 ", "Bh6", "WAC.169"},
            new object[] {"5r1k/6Rp/1p2p3/p2pBp2/1qnP4/4P3/Q4PPP/6K1 w - -                      ", "Qxc4", "WAC.170"},
            new object[] {"2rq4/1b2b1kp/p3p1p1/1p1nNp2/7P/1B2B1Q1/PP3PP1/3R2K1 w - -            ", "Bh6+", "WAC.171"},
            new object[] {"2r1b3/1pp1qrk1/p1n1P1p1/7R/2B1p3/4Q1P1/PP3PP1/3R2K1 w - -            ", "Qh6+", "WAC.173"},
            new object[] {"r5k1/pppb3p/2np1n2/8/3PqNpP/3Q2P1/PPP5/R4RK1 w - -                   ", "Nh5", "WAC.175"},
            new object[] {"3r2k1/p1rn1p1p/1p2pp2/6q1/3PQNP1/5P2/P1P4R/R5K1 w - -                ", "Nxe6", "WAC.178"},
            new object[] {"r1q2rk1/p3bppb/3p1n1p/2nPp3/1p2P1P1/6NP/PP2QPB1/R1BNK2R b KQ -       ", "Nxd5", "WAC.180"},
            new object[] {"r3k2r/2p2p2/p2p1n2/1p2p3/4P2p/1PPPPp1q/1P5P/R1N2QRK b kq -           ", "Ng4", "WAC.181"},
            new object[] {"r1b2rk1/ppqn1p1p/2n1p1p1/2b3N1/2N5/PP1BP3/1B3PPP/R2QK2R w KQ -       ", "Qh5", "WAC.182"},
            new object[] {"6k1/5p2/p3p3/1p3qp1/2p1Qn2/2P1R3/PP1r1PPP/4R1K1 b - -                ", "Nh3+", "WAC.187"},
            new object[] {"3RNbk1/pp3p2/4rQpp/8/1qr5/7P/P4P2/3R2K1 w - -                        ", "Qg7+", "WAC.188"},
            new object[] {"8/p2b2kp/1q1p2p1/1P1Pp3/4P3/3B2P1/P2Q3P/2Nn3K b - -                  ", "Bh3", "WAC.190"},
            new object[] {"r3k3/ppp2Npp/4Bn2/2b5/1n1pp3/N4P2/PPP3qP/R2QKR2 b Qq -               ", "Nd3+", "WAC.192"},
            new object[] {"5rk1/ppq2ppp/2p5/4bN2/4P3/6Q1/PPP2PPP/3R2K1 w - -                    ", "Nh6+", "WAC.194"},
            new object[] {"3r1rk1/1p3p2/p3pnnp/2p3p1/2P2q2/1P5P/PB2QPPN/3RR1K1 w - -            ", "g3", "WAC.195"},
            new object[] {"rr4k1/p1pq2pp/Q1n1pn2/2bpp3/4P3/2PP1NN1/PP3PPP/R1B1K2R b KQ -        ", "Nb4", "WAC.196"},
            new object[] {"7k/1p4p1/7p/3P1n2/4Q3/2P2P2/PP3qRP/7K b - -                          ", "Qf1+", "WAC.197"},
            new object[] {"2br2k1/ppp2p1p/4p1p1/4P2q/2P1Bn2/2Q5/PP3P1P/4R1RK b - -              ", "Rd3", "WAC.198"},
            new object[] {"2rqrn1k/pb4pp/1p2pp2/n2P4/2P3N1/P2B2Q1/1B3PPP/2R1R1K1 w - -          ", "Bxf6", "WAC.200"},
            new object[] {"2b2r1k/4q2p/3p2pQ/2pBp3/8/6P1/1PP2P1P/R5K1 w - -                     ", "Ra7", "WAC.201"},
            new object[] {"QR2rq1k/2p3p1/3p1pPp/8/4P3/8/P1r3PP/1R4K1 b - -                      ", "Rxa2", "WAC.202"},
            new object[] {"r4rk1/5ppp/p3q1n1/2p2NQ1/4n3/P3P3/1B3PPP/1R3RK1 w - -                ", "Qh6", "WAC.203"},
            new object[] {"r1b1qrk1/1p3ppp/p1p5/3Nb3/5N2/P7/1P4PQ/K1R1R3 w - -                  ", "Rxe5", "WAC.204"},
            new object[] {"r3rnk1/1pq2bb1/p4p2/3p1Pp1/3B2P1/1NP4R/P1PQB3/2K4R w - -             ", "Qxg5", "WAC.205"},
            new object[] {"1Qq5/2P1p1kp/3r1pp1/8/8/7P/p4PP1/2R3K1 b - -                         ", "Rc6", "WAC.206"},
            new object[] {"r1bq2kr/p1pp1ppp/1pn1p3/4P3/2Pb2Q1/BR6/P4PPP/3K1BNR w - -            ", "Qxg7+", "WAC.207"},
            new object[] {"3r1bk1/ppq3pp/2p5/2P2Q1B/8/1P4P1/P6P/5RK1 w - -                      ", "Bf7+", "WAC.208"},
            new object[] {"3r1rk1/pp1q1ppp/3pn3/2pN4/5PP1/P5PQ/1PP1B3/1K1R4 w - -               ", "Rh1", "WAC.210"},
            new object[] {"rn1qr2Q/pbppk1p1/1p2pb2/4N3/3P4/2N5/PPP3PP/R4RK1 w - -               ", "Qxg7+", "WAC.212"},
            new object[] {"3r1r1k/1b4pp/ppn1p3/4Pp1R/Pn5P/3P4/4QP2/1qB1NKR1 w - -               ", "Rxh7+", "WAC.213"},
            new object[] {"3r2k1/pb1q1pp1/1p2pb1p/8/3N4/P2QB3/1P3PPP/1Br1R1K1 w - -             ", "Qh7+", "WAC.215"},
            new object[] {"7k/p4q1p/1pb5/2p5/4B2Q/2P1B3/P6P/7K b - -                            ", "Qf1+", "WAC.219"},
            new object[] {"3rr1k1/ppp2ppp/8/5Q2/4n3/1B5R/PPP1qPP1/5RK1 b - -                    ", "Qxf1+", "WAC.220"},
            new object[] {"2r1r2k/1q3ppp/p2Rp3/2p1P3/6QB/p3P3/bP3PPP/3R2K1 w - -                ", "Bf6", "WAC.222"},
            new object[] {"2k1rb1r/ppp3pp/2np1q2/5b2/2B2P2/2P1BQ2/PP1N1P1P/2KR3R b - -          ", "d5", "WAC.227"},
            new object[] {"r4rk1/1bq1bp1p/4p1p1/p2p4/3BnP2/1N1B3R/PPP3PP/R2Q2K1 w - -           ", "Bxe4", "WAC.228"},
            new object[] {"r4rk1/1b1nqp1p/p5p1/1p2PQ2/2p5/5N2/PP3PPP/R1BR2K1 w - -              ", "Bg5", "WAC.231"},
            new object[] {"1R6/p5pk/4p2p/4P3/8/2r3qP/P3R1b1/4Q1K1 b - -                         ", "Rc1", "WAC.236"},
            new object[] {"r5k1/pQp2qpp/8/4pbN1/3P4/6P1/PPr4P/1K1R3R b - -                      ", "Rc1+", "WAC.237"},
            new object[] {"1k1r4/pp1r1pp1/4n1p1/2R5/2Pp1qP1/3P2QP/P4PB1/1R4K1 w - -             ", "Bxb7", "WAC.238"},
            new object[] {"2b4k/p1b2p2/2p2q2/3p1PNp/3P2R1/3B4/P1Q2PKP/4r3 w - -                 ", "Qxc6", "WAC.240"},
            new object[] {"r1b1r1k1/pp1nqp2/2p1p1pp/8/4N3/P1Q1P3/1P3PPP/1BRR2K1 w - -           ", "Rxd7", "WAC.242"},
            new object[] {"1b5k/7P/p1p2np1/2P2p2/PP3P2/4RQ1R/q2r3P/6K1 w - -                    ", "Re8+", "WAC.250"},
            new object[] {"r6k/pp3p1p/2p1bp1q/b3p3/4Pnr1/2PP2NP/PP1Q1PPN/R2B2RK b - -           ", "Nxh3", "WAC.254"},
            new object[] {"3r3r/p4pk1/5Rp1/3q4/1p1P2RQ/5N2/P1P4P/2b4K w - -                     ", "Rfxg6+", "WAC.255"},
            new object[] {"3r1rk1/1pb1qp1p/2p3p1/p7/P2Np2R/1P5P/1BP2PP1/3Q1BK1 w - -            ", "Nf5", "WAC.256"},
            new object[] {"4r1k1/pq3p1p/2p1r1p1/2Q1p3/3nN1P1/1P6/P1P2P1P/3RR1K1 w - -           ", "Rxd4", "WAC.257"},
            new object[] {"r3brkn/1p5p/2p2Ppq/2Pp3B/3Pp2Q/4P1R1/6PP/5R1K w - -                  ", "Bxg6", "WAC.258"},
            new object[] {"2r2b1r/p1Nk2pp/3p1p2/N2Qn3/4P3/q6P/P4PP1/1R3K1R w - -                ", "Qe6+", "WAC.260"},
            new object[] {"6k1/p1B1b2p/2b3r1/2p5/4p3/1PP1N1Pq/P2R1P2/3Q2K1 b - -                ", "Rh6", "WAC.262"},
            new object[] {"rnbqr2k/pppp1Qpp/8/b2NN3/2B1n3/8/PPPP1PPP/R1B1K2R w KQ -             ", "Qg8+", "WAC.263"},
            new object[] {"2r1k2r/2pn1pp1/1p3n1p/p3PP2/4q2B/P1P5/2Q1N1PP/R4RK1 w k -            ", "exf6", "WAC.265"},
            new object[] {"r3q2r/2p1k1p1/p5p1/1p2Nb2/1P2nB2/P7/2PNQbPP/R2R3K b - -              ", "Rxh2+", "WAC.266"},
            new object[] {"2r1kb1r/pp3ppp/2n1b3/1q1N2B1/1P2Q3/8/P4PPP/3RK1NR w Kk -             ", "Nc7+", "WAC.267"},
            new object[] {"2kr2nr/pp1n1ppp/2p1p3/q7/1b1P1B2/P1N2Q1P/1PP1BPP1/R3K2R w KQ -       ", "axb4", "WAC.269"},
            new object[] {"2r1r1k1/pp1q1ppp/3p1b2/3P4/3Q4/5N2/PP2RPPP/4R1K1 w - -               ", "Qg4", "WAC.270"},
            new object[] {"2kr4/ppp3Pp/4RP1B/2r5/5P2/1P6/P2p4/3K4 w - -                         ", "Rd6", "WAC.271"},
            new object[] {"nrq4r/2k1p3/1p1pPnp1/pRpP1p2/P1P2P2/2P1BB2/1R2Q1P1/6K1 w - -         ", "Bxc5", "WAC.272"},
            new object[] {"r2qkb1r/pppb2pp/2np1n2/5pN1/2BQP3/2N5/PPP2PPP/R1B1K2R w KQkq -       ", "Bf7+", "WAC.278"},
            new object[] {"2R5/2R4p/5p1k/6n1/8/1P2QPPq/r7/6K1 w - -                             ", "Rxh7+", "WAC.281"},
            new object[] {"6k1/2p3p1/1p1p1nN1/1B1P4/4PK2/8/2r3b1/7R w - -                       ", "Rh8+", "WAC.282"},
            new object[] {"3q1rk1/4bp1p/1n2P2Q/3p1p2/6r1/Pp2R2N/1B4PP/7K w - -                  ", "Ng5", "WAC.283"},
            new object[] {"3r1k2/1p6/p4P2/2pP2Qb/8/1P1KB3/P6r/8 b - -                           ", "Rxd5+", "WAC.286"},
            new object[] {"r1b2rk1/p4ppp/1p1Qp3/4P2N/1P6/8/P3qPPP/3R1RK1 w - -                  ", "Nf6+", "WAC.288"},
            new object[] {"2r3k1/5p1p/p3q1p1/2n3P1/1p1QP2P/1P4N1/PK6/2R5 b - -                  ", "Qe5", "WAC.289"},
            new object[] {"4r3/1Q1qk2p/p4pp1/3Pb3/P7/6PP/5P2/4R1K1 w - -                        ", "d6+", "WAC.292"},
            new object[] {"1nbq1r1k/3rbp1p/p1p1pp1Q/1p6/P1pPN3/5NP1/1P2PPBP/R4RK1 w - -         ", "Nfg5", "WAC.293"},
            new object[] {"4r3/p4r1p/R1p2pp1/1p1bk3/4pNPP/2P1K3/2P2P2/3R4 w - -                 ", "Rxd5+", "WAC.295"},
            new object[] {"3Q4/p3b1k1/2p2rPp/2q5/4B3/P2P4/7P/6RK w - -                          ", "Qh8+", "WAC.298"},
            new object[] {"b2b1r1k/3R1ppp/4qP2/4p1PQ/4P3/5B2/4N1K1/8 w - -                      ", "g6", "WAC.300"},
            new object[] {"r2q1rk1/2p2ppp/p1n2n2/Pp2p3/1P2P3/1BPPQR2/6PP/RN4K1 b - -            ", "Nd4", "WAC.301" }
        };
    }
}
