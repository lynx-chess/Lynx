using Lynx.UCI.Commands.GUI;
using System.Diagnostics;

namespace Lynx;

public partial class Engine
{
    internal static readonly string[] _benchmarkFens =
    [
        "r3k2r/2pb1ppp/2pp1q2/p7/1nP1B3/1P2P3/P2N1PPP/R2QK2R w KQkq a6 0 14",
        "4rrk1/2p1b1p1/p1p3q1/4p3/2P2n1p/1P1NR2P/PB3PP1/3R1QK1 b - - 2 24",
        "r3qbrk/6p1/2b2pPp/p3pP1Q/PpPpP2P/3P1B2/2PB3K/R5R1 w - - 16 42",
        "6k1/1R3p2/6p1/2Bp3p/3P2q1/P7/1P2rQ1K/5R2 b - - 4 44",
        "8/8/1p2k1p1/3p3p/1p1P1P1P/1P2PK2/8/8 w - - 3 54",
        "7r/2p3k1/1p1p1qp1/1P1Bp3/p1P2r1P/P7/4R3/Q4RK1 w - - 0 36",
        "r1bq1rk1/pp2b1pp/n1pp1n2/3P1p2/2P1p3/2N1P2N/PP2BPPP/R1BQ1RK1 b - - 2 10",
        "3r3k/2r4p/1p1b3q/p4P2/P2Pp3/1B2P3/3BQ1RP/6K1 w - - 3 87",
        "2r4r/1p4k1/1Pnp4/3Qb1pq/8/4BpPp/5P2/2RR1BK1 w - - 0 42",
        "4q1bk/6b1/7p/p1p4p/PNPpP2P/KN4P1/3Q4/4R3 b - - 0 37",
        "2q3r1/1r2pk2/pp3pp1/2pP3p/P1Pb1BbP/1P4Q1/R3NPP1/4R1K1 w - - 2 34",
        "1r2r2k/1b4q1/pp5p/2pPp1p1/P3Pn2/1P1B1Q1P/2R3P1/4BR1K b - - 1 37",
        "r3kbbr/pp1n1p1P/3ppnp1/q5N1/1P1pP3/P1N1B3/2P1QP2/R3KB1R b KQkq b3 0 17",
        "8/6pk/2b1Rp2/3r4/1R1B2PP/P5K1/8/2r5 b - - 16 42",
        "1r4k1/4ppb1/2n1b1qp/pB4p1/1n1BP1P1/7P/2PNQPK1/3RN3 w - - 8 29",
        "8/p2B4/PkP5/4p1pK/4Pb1p/5P2/8/8 w - - 29 68",
        "3r4/ppq1ppkp/4bnp1/2pN4/2P1P3/1P4P1/PQ3PBP/R4K2 b - - 2 20",
        "5rr1/4n2k/4q2P/P1P2n2/3B1p2/4pP2/2N1P3/1RR1K2Q w - - 1 49",
        "1r5k/2pq2p1/3p3p/p1pP4/4QP2/PP1R3P/6PK/8 w - - 1 51",
        "q5k1/5ppp/1r3bn1/1B6/P1N2P2/BQ2P1P1/5K1P/8 b - - 2 34",
        "r1b2k1r/5n2/p4q2/1ppn1Pp1/3pp1p1/NP2P3/P1PPBK2/1RQN2R1 w - - 0 22",
        "r1bqk2r/pppp1ppp/5n2/4b3/4P3/P1N5/1PP2PPP/R1BQKB1R w KQkq - 0 5",
        "r1bqr1k1/pp1p1ppp/2p5/8/3N1Q2/P2BB3/1PP2PPP/R3K2n b Q - 1 12",
        "r1bq2k1/p4r1p/1pp2pp1/3p4/1P1B3Q/P2B1N2/2P3PP/4R1K1 b - - 2 19",
        "r4qk1/6r1/1p4p1/2ppBbN1/1p5Q/P7/2P3PP/5RK1 w - - 2 25",
        "r7/6k1/1p6/2pp1p2/7Q/8/p1P2K1P/8 w - - 0 32",
        "r3k2r/ppp1pp1p/2nqb1pn/3p4/4P3/2PP4/PP1NBPPP/R2QK1NR w KQkq - 1 5",
        "3r1rk1/1pp1pn1p/p1n1q1p1/3p4/Q3P3/2P5/PP1NBPPP/4RRK1 w - - 0 12",
        "5rk1/1pp1pn1p/p3Brp1/8/1n6/5N2/PP3PPP/2R2RK1 w - - 2 20",
        "8/1p2pk1p/p1p1r1p1/3n4/8/5R2/PP3PPP/4R1K1 b - - 3 27",
        "8/4pk2/1p1r2p1/p1p4p/Pn5P/3R4/1P3PP1/4RK2 w - - 1 33",
        "8/5k2/1pnrp1p1/p1p4p/P6P/4R1PK/1P3P2/4R3 b - - 1 38",
        "8/8/1p1kp1p1/p1pr1n1p/P6P/1R4P1/1P3PK1/1R6 b - - 15 45",
        "8/8/1p1k2p1/p1prp2p/P2n3P/6P1/1P1R1PK1/4R3 b - - 5 49",
        "8/8/1p4p1/p1p2k1p/P2npP1P/4K1P1/1P6/3R4 w - - 6 54",
        "8/8/1p4p1/p1p2k1p/P2n1P1P/4K1P1/1P6/6R1 b - - 6 59",
        "8/5k2/1p4p1/p1pK3p/P2n1P1P/6P1/1P6/4R3 b - - 14 63",
        "8/1R6/1p1K1kp1/p6p/P1p2P1P/6P1/1Pn5/8 w - - 0 67",
        "1rb1rn1k/p3q1bp/2p3p1/2p1p3/2P1P2N/PP1RQNP1/1B3P2/4R1K1 b - - 4 23",
        "4rrk1/pp1n1pp1/q5p1/P1pP4/2n3P1/7P/1P3PB1/R1BQ1RK1 w - - 3 22",
        "r2qr1k1/pb1nbppp/1pn1p3/2ppP3/3P4/2PB1NN1/PP3PPP/R1BQR1K1 w - - 4 12",
        "2r2k2/8/4P1R1/1p6/8/P4K1N/7b/2B5 b - - 0 55",
        "6k1/5pp1/8/2bKP2P/2P5/p4PNb/B7/8 b - - 1 44",
        "2rqr1k1/1p3p1p/p2p2p1/P1nPb3/2B1P3/5P2/1PQ2NPP/R1R4K w - - 3 25",
        "r1b2rk1/p1q1ppbp/6p1/2Q5/8/4BP2/PPP3PP/2KR1B1R b - - 2 14",
        "6r1/5k2/p1b1r2p/1pB1p1p1/1Pp3PP/2P1R1K1/2P2P2/3R4 w - - 1 36",
        "rnbqkb1r/pppppppp/5n2/8/2PP4/8/PP2PPPP/RNBQKBNR b KQkq c3 0 2",
        "2rr2k1/1p4bp/p1q1p1p1/4Pp1n/2PB4/1PN3P1/P3Q2P/2RR2K1 w - f6 0 20",
        "3br1k1/p1pn3p/1p3n2/5pNq/2P1p3/1PN3PP/P2Q1PB1/4R1K1 w - - 0 23",
        "2r2b2/5p2/5k2/p1r1pP2/P2pB3/1P3P2/K1P3R1/7R w - - 23 93",
        "5k2/4q1p1/3P1pQb/1p1B4/pP5p/P1PR4/5PP1/1K6 b - - 0 38",
        "6k1/6p1/8/6KQ/1r6/q2b4/8/8 w - - 0 32",
        "5rk1/1rP3pp/p4n2/3Pp3/1P2Pq2/2Q4P/P5P1/R3R1K1 b - - 0 32",
        "4r1k1/4r1p1/8/p2R1P1K/5P1P/1QP3q1/1P6/3R4 b - - 0 1",
        "R4r2/4q1k1/2p1bb1p/2n2B1Q/1N2pP2/1r2P3/1P5P/2B2KNR w - - 3 31",
        "r6k/pbR5/1p2qn1p/P2pPr2/4n2Q/1P2RN1P/5PBK/8 w - - 2 31",
        "rn2k3/4r1b1/pp1p1n2/1P1q1p1p/3P4/P3P1RP/1BQN1PR1/1K6 w - - 6 28",
        "3q1k2/3P1rb1/p6r/1p2Rp2/1P5p/P1N2pP1/5B1P/3QRK2 w - - 1 42",
        "4r2k/1p3rbp/2p1N1p1/p3n3/P2NB1nq/1P6/4R1P1/B1Q2RK1 b - - 4 32",
        "4r1k1/1q1r3p/2bPNb2/1p1R3Q/pB3p2/n5P1/6B1/4R1K1 w - - 2 36",
        "3qr2k/1p3rbp/2p3p1/p7/P2pBNn1/1P3n2/6P1/B1Q1RR1K b - - 1 30",
        "3qk1b1/1p4r1/1n4r1/2P1b2B/p3N2p/P2Q3P/8/1R3R1K w - - 2 39",

        "6RR/4bP2/8/8/5r2/3K4/5p2/4k3 w - - 0 1",                       // SEE test suite - regular promotion
        "1n2kb1r/p1P4p/2qb4/5pP1/4n2Q/8/PP1PPP1P/RNB1KBNR w KQk - 0 1", // SEE test suite - promotion with capture
        "6Q1/8/1kp4P/2q1p3/2PpP3/2nP2P1/p7/5BK1 b - - 1 35",            // Fischer vs Petrosian - double promotion
        "5R2/2k3PK/8/5N2/7P/5q2/8/q7 w - - 0 69",                       // McShane - Aronian 2012 - knight promotion
        "rnbqk1nr/ppp2ppp/8/4P3/1BP5/8/PP2KpPP/RN1Q1BNR b kq - 1 7",    // Albin Countergambit, Lasker Trap - knight promotion

        "8/nRp5/8/8/8/7k/8/7K w - - 0 1",                               // R vs NP
        "8/bRp5/8/8/8/7k/8/7K w - - 0 1",                               // R vs BP
        "8/8/4k3/3n1n2/5P2/8/3K4/8 b - - 0 12",                         // NN vs P endgame
        "8/bQr5/8/8/8/7k/8/7K w - - 0 1",                               // Q vs RB, leading to Q vs R or Q vs B
        "8/nQr5/8/8/8/7k/8/7K w - - 0 1",                               // Q vs RN, leading to Q vs R or Q vs N
        "4kq2/8/n7/8/8/3Q3b/8/3K4 w - - 0 1",                           // Q vs QBN, leading to Q vs QB or Q vs QN
        "8/5R2/1n2RK2/8/8/7k/4r3/8 b - - 0 1",                          // RR vs RN endgame, where if black takes, they actually loses
        "8/n3p3/8/2B5/2b5/7k/P7/7K w - - 0 1",                          // BP vs BNP endgame, leading to B vs BN
        "8/n3p3/8/2B5/1n6/7k/P7/7K w - - 0 1",                          // BP vs NNP endgame, leading to B vs NN
        "8/bRn5/8/7b/8/7k/8/7K w - - 0 1",                              // R vs BBN, leading to R vs BN or R vs BB
        "1b6/1R1r4/8/1n6/7k/8/8/7K w - - 0 1",                          // R vs RBN, leading to R vs BN or R vs RB or R vs RN
        "8/q5rk/8/8/8/8/Q5RK/7N w - - 0 1",                             // Endgame that can lead to QN vs Q or RN vs R positions
        "1kr5/2bp3q/Q7/1K6/6q1/6B1/8/8 w - - 0 1",                      // Endgame where triple repetition can and needs to be forced by white
        "1kr5/2bp3q/R7/1K6/6q1/6B1/8/8 w - - 96 200",                   // Endgame where 50 moves draw can and needs to be forced by white
        "k7/8/8/K7/8/3B4/P7/8 w - - 0 48",                              // BP won endgame
        "k7/8/8/K7/8/2B5/P7/8 w - - 0 48",                              // BP drawn endgame
        "8/2bk4/8/3p4/P1K5/4B3/3P4/8 w - - 0 47",                       // Endgame leading to BP drawn endgame - Sirius vs Stormphrax FRD4, TCEC
        "8/3k4/8/b1K5/P7/4B3/3P4/8 b - - 2 48",                         // Endgame leading to BP drawn endgame II - Sirius vs Stormphrax FRD4, TCEC
        "8/2bk4/8/3p4/P1K5/8/3PB3/8 w - - 0 47",                        // Endgame leading to BP drawn endgame - but not because of the king on the corner
        "4K3/8/8/3k4/8/2B5/P7/8 b - - 0 1",                             // Potential false negative of BP drawn endgame
        "2K5/8/1k6/8/8/2B5/P7/8 b - - 0 1",                             // Relatively tricky BP drawn endgame
        "3K4/8/2k5/8/8/2B5/P7/8 b - - 0 1",                             // Relatively tricky BP drawn endgame II
        "k7/8/K7/8/8/8/P7/8 w - - 0 1",                                 // KP drawn endgame
        "1k6/8/P1K5/P7/P7/P7/P7/8 w - - 0 1",                           // KP drawn endgame II
        "rnbqk2r/pppp1ppp/5n2/8/Bb2N3/8/PPPPQPPP/RNB1K2R w KQkq - 2 1", // Petroff defense alike position with double check Q and N
        "rnb1kb1r/pppp1ppp/5n2/8/4N3/8/PPPP1PPP/RNB1R1K1 w kq - 2 5",   // Double check R and N
        "rnbqk2r/ppp2ppp/3p4/8/1b2Bn2/8/PPPPQPPP/RNB1K2R w KQkq - 2 5", // Double check Q and B
        "rnbqk2r/ppp2ppp/3p4/8/1b2B3/3n4/PPPP1PPP/RNBQR1K1 w kq - 2 5", // Double check R and B
        "r3k2r/ppp2ppp/n7/1N1p4/Bb6/8/PPPP1PPP/RNBQ1RK1 w kq - 2 1",    // Double check B and N, castling rights
        "r3k2r/ppp2ppp/n7/1N1p4/Bb6/8/PPPP1PPP/RNBQ1RK1 w - - 2 1",     // Double check B and N, no castling rights

        "r7/8/8/4k3/8/4B3/4K2P/7R w - - 95 1",                          // High 50mr counter in advantage position (needs to move pawn)
        "7K/r7/8/8/7P/6PR/6PR/1k6 b - - 95 1",                          // High 50mr counter in disadvantage position (needs to keep checking)

        "nqbnrkrb/pppppppp/8/8/8/8/PPPPPPPP/NQBNRKRB w - - 0 1",     // Cornered/trapped bishop and knight
        "rqbbnknr/pppppppp/8/8/8/8/PPPPPPPP/NQBNRKRB w - - 0 1",     // Cornered/trapped bishop and knight - only one side
    ];

    /// <summary>
    /// Positions taken from Akimbo
    /// (https://github.com/JacquesRW/akimbo/blob/main/resources/fens.txt)
    /// plus random some endgame positions to ensure promotions with/without captures are well covered
    /// </summary>
    public (ulong TotalNodes, ulong Nps) Bench(int depth)
    {
        var stopwatch = new Stopwatch();

        ulong totalNodes = 0;
        double totalSeconds = 0;

        foreach (var fen in _benchmarkFens)
        {
            _engineWriter.TryWrite($"Benchmarking {fen} at depth {depth}");

            AdjustPosition($"position fen {fen}");
            stopwatch.Restart();

            var goCommand = new GoCommand($"go depth {depth}");
            var result = BestMove(goCommand);

            var elapsedSeconds = Utils.CalculateElapsedSeconds(_stopWatch);
            totalSeconds += elapsedSeconds;
            totalNodes += result.Nodes;
        }

        _engineWriter.TryWrite($"Total time: {Utils.CalculateUCITime(totalSeconds)}");

        return (totalNodes, Utils.CalculateNps(totalNodes, totalSeconds));
    }

    public async ValueTask PrintBenchResults((ulong TotalNodes, ulong Nps) benchResult) => await _engineWriter.WriteAsync($"{benchResult.TotalNodes} nodes {benchResult.Nps} nps");
    public static void PrintBenchResults((ulong TotalNodes, ulong Nps) benchResult, Action<string> write) => write($"{benchResult.TotalNodes} nodes {benchResult.Nps} nps");
    public static async ValueTask PrintBenchResults((ulong TotalNodes, ulong Nps) benchResult, Func<string, ValueTask> write) => await write($"{benchResult.TotalNodes} nodes {benchResult.Nps} nps");
}
