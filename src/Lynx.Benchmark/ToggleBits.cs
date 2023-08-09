/*
 * Not exactly conclusive
 *
 *  | Method |                  fen |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
 *  |------- |--------------------- |---------:|----------:|----------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | SetPop | 8/8/8(...)- 0 1 [25] | 5.766 us | 0.2855 us | 0.8417 us | 5.438 us |  1.00 |    0.00 | 1.3580 |   2.78 KB |        1.00 |
 *  | Toggle | 8/8/8(...)- 0 1 [25] | 5.403 us | 0.2476 us | 0.7300 us | 5.018 us |  0.96 |    0.20 | 1.3580 |   2.78 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | r2q1r(...) 0 9  [69] | 7.189 us | 0.2778 us | 0.8190 us | 6.883 us |  1.00 |    0.00 | 1.6098 |    3.3 KB |        1.00 |
 *  | Toggle | r2q1r(...) 0 9  [69] | 6.910 us | 0.2899 us | 0.8549 us | 6.412 us |  0.97 |    0.17 | 1.6098 |    3.3 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | r3k2r(...)- 0 1 [68] | 6.660 us | 0.2652 us | 0.7820 us | 6.261 us |  1.00 |    0.00 | 1.4954 |   3.07 KB |        1.00 |
 *  | Toggle | r3k2r(...)- 0 1 [68] | 6.640 us | 0.2484 us | 0.7284 us | 6.300 us |  1.01 |    0.16 | 1.4954 |   3.07 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | r3k2r(...)- 0 1 [68] | 6.822 us | 0.2753 us | 0.8117 us | 6.405 us |  1.00 |    0.00 | 1.4954 |   3.07 KB |        1.00 |
 *  | Toggle | r3k2r(...)- 0 1 [68] | 6.493 us | 0.2550 us | 0.7519 us | 6.084 us |  0.97 |    0.17 | 1.4954 |   3.07 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | rnbqk(...)6 0 1 [67] | 8.143 us | 0.6984 us | 2.0593 us | 7.684 us |  1.00 |    0.00 | 1.4954 |   3.06 KB |        1.00 |
 *  | Toggle | rnbqk(...)6 0 1 [67] | 9.596 us | 0.5562 us | 1.6399 us | 9.558 us |  1.26 |    0.39 | 1.4954 |   3.06 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | rnbqk(...)- 0 1 [56] | 6.067 us | 0.2854 us | 0.8371 us | 5.867 us |  1.00 |    0.00 | 1.3428 |   2.75 KB |        1.00 |
 *  | Toggle | rnbqk(...)- 0 1 [56] | 5.473 us | 0.2249 us | 0.6631 us | 5.343 us |  0.92 |    0.15 | 1.3428 |   2.75 KB |        1.00 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class ToggleBits : BaseBenchmark
{
    public static IEnumerable<string> Data => new[] {
        Constants.EmptyBoardFEN,
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1",
        "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1",
        "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9 "
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public Position SetPop(string fen)
    {
        var position = new Position(fen);
        for (int b = 0; b < position.PieceBitBoards.Length; ++b)
        {
            var bb = position.PieceBitBoards[b];
            for (int i = 0; i < 32; ++i)
            {
                bb.PopBit(i);
                bb.SetBit(i + 32);

                position.OccupancyBitBoards[i % 2].PopBit(i);
                position.OccupancyBitBoards[i % 2].SetBit(i + 32);
            }
        }

        return position;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Position Toggle(string fen)
    {
        var position = new Position(fen);
        for (int b = 0; b < position.PieceBitBoards.Length; ++b)
        {
            var bb = position.PieceBitBoards[b];
            for (int i = 0; i < 32; ++i)
            {
                bb.ToggleBits(i, i + 32);
                position.OccupancyBitBoards[i % 2].ToggleBits(i, i + 32);
            }
        }

        return position;
    }
}