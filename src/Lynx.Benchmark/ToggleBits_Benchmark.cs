/*
 * Not exactly conclusive, but apparently ToggleBits can be slightly better in general
 * Whatever sprt says
 *
 *  Local:
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
 *  BenchmarkDotNet v0.13.7, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.6.23330.14
 *    [Host]     : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
 *
 *
 *  | Method |                  fen |     Mean |     Error |    StdDev | Ratio |   Gen0 | Allocated | Alloc Ratio |
 *  |------- |--------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 *  | SetPop | 8/8/8(...)- 0 1 [25] | 4.229 us | 0.0213 us | 0.0178 us |  1.00 | 0.1068 |   2.77 KB |        1.00 |
 *  | Toggle | 8/8/8(...)- 0 1 [25] | 4.224 us | 0.0111 us | 0.0104 us |  1.00 | 0.1068 |   2.77 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | r2q1r(...) 0 9  [69] | 5.427 us | 0.0168 us | 0.0149 us |  1.00 | 0.1297 |    3.3 KB |        1.00 |
 *  | Toggle | r2q1r(...) 0 9  [69] | 5.282 us | 0.0129 us | 0.0120 us |  0.97 | 0.1297 |    3.3 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | r3k2r(...)- 0 1 [68] | 5.188 us | 0.0121 us | 0.0113 us |  1.00 | 0.1221 |   3.06 KB |        1.00 |
 *  | Toggle | r3k2r(...)- 0 1 [68] | 5.117 us | 0.0097 us | 0.0081 us |  0.99 | 0.1221 |   3.06 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | r3k2r(...)- 0 1 [68] | 5.210 us | 0.0089 us | 0.0075 us |  1.00 | 0.1221 |   3.06 KB |        1.00 |
 *  | Toggle | r3k2r(...)- 0 1 [68] | 5.082 us | 0.0143 us | 0.0127 us |  0.98 | 0.1221 |   3.06 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | rnbqk(...)6 0 1 [67] | 5.093 us | 0.0148 us | 0.0138 us |  1.00 | 0.1221 |   3.05 KB |        1.00 |
 *  | Toggle | rnbqk(...)6 0 1 [67] | 5.070 us | 0.0154 us | 0.0144 us |  1.00 | 0.1221 |   3.05 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | rnbqk(...)- 0 1 [56] | 4.274 us | 0.0143 us | 0.0127 us |  1.00 | 0.1068 |   2.75 KB |        1.00 |
 *  | Toggle | rnbqk(...)- 0 1 [56] | 4.350 us | 0.0094 us | 0.0083 us |  1.02 | 0.1068 |   2.75 KB |        1.00 |
 *
 *  BenchmarkDotNet v0.13.7, Windows 10 (10.0.20348.1850) (Hyper-V)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.6.23330.14
 *    [Host]     : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
 *
 *
 *  | Method |                  fen |     Mean |     Error |    StdDev | Ratio |   Gen0 | Allocated | Alloc Ratio |
 *  |------- |--------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 *  | SetPop | 8/8/8(...)- 0 1 [25] | 3.913 us | 0.0097 us | 0.0086 us |  1.00 | 0.1068 |   2.77 KB |        1.00 |
 *  | Toggle | 8/8/8(...)- 0 1 [25] | 3.725 us | 0.0157 us | 0.0131 us |  0.95 | 0.1106 |   2.77 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | r2q1r(...) 0 9  [69] | 4.983 us | 0.0313 us | 0.0261 us |  1.00 | 0.1297 |    3.3 KB |        1.00 |
 *  | Toggle | r2q1r(...) 0 9  [69] | 5.061 us | 0.0529 us | 0.0495 us |  1.02 | 0.1297 |    3.3 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | r3k2r(...)- 0 1 [68] | 4.891 us | 0.0634 us | 0.0593 us |  1.00 | 0.1221 |   3.06 KB |        1.00 |
 *  | Toggle | r3k2r(...)- 0 1 [68] | 4.804 us | 0.0184 us | 0.0153 us |  0.98 | 0.1221 |   3.06 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | r3k2r(...)- 0 1 [68] | 4.861 us | 0.0089 us | 0.0074 us |  1.00 | 0.1221 |   3.06 KB |        1.00 |
 *  | Toggle | r3k2r(...)- 0 1 [68] | 4.665 us | 0.0271 us | 0.0240 us |  0.96 | 0.1221 |   3.06 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | rnbqk(...)6 0 1 [67] | 4.862 us | 0.0228 us | 0.0202 us |  1.00 | 0.1221 |   3.05 KB |        1.00 |
 *  | Toggle | rnbqk(...)6 0 1 [67] | 4.694 us | 0.0219 us | 0.0183 us |  0.97 | 0.1221 |   3.05 KB |        1.00 |
 *  |        |                      |          |           |           |       |        |           |             |
 *  | SetPop | rnbqk(...)- 0 1 [56] | 4.229 us | 0.0392 us | 0.0348 us |  1.00 | 0.1068 |   2.75 KB |        1.00 |
 *  | Toggle | rnbqk(...)- 0 1 [56] | 4.178 us | 0.0068 us | 0.0053 us |  0.99 | 0.1068 |   2.75 KB |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.13.7, macOS Monterey 12.6.7 (21G651) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *
 *
 *  | Method |                  fen |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
 *  |------- |--------------------- |---------:|----------:|----------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | SetPop | 8/8/8(...)- 0 1 [25] | 4.150 us | 0.0774 us | 0.0724 us | 4.141 us |  1.00 |    0.00 | 0.4501 |   2.77 KB |        1.00 |
 *  | Toggle | 8/8/8(...)- 0 1 [25] | 4.044 us | 0.0300 us | 0.0266 us | 4.049 us |  0.97 |    0.02 | 0.4501 |   2.77 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | r2q1r(...) 0 9  [69] | 5.486 us | 0.0626 us | 0.0586 us | 5.474 us |  1.00 |    0.00 | 0.5341 |    3.3 KB |        1.00 |
 *  | Toggle | r2q1r(...) 0 9  [69] | 5.672 us | 0.1520 us | 0.4337 us | 5.537 us |  1.12 |    0.10 | 0.5341 |    3.3 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | r3k2r(...)- 0 1 [68] | 5.130 us | 0.0873 us | 0.0729 us | 5.136 us |  1.00 |    0.00 | 0.4959 |   3.06 KB |        1.00 |
 *  | Toggle | r3k2r(...)- 0 1 [68] | 5.220 us | 0.0801 us | 0.0750 us | 5.186 us |  1.02 |    0.02 | 0.4959 |   3.06 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | r3k2r(...)- 0 1 [68] | 5.278 us | 0.0491 us | 0.0460 us | 5.269 us |  1.00 |    0.00 | 0.4959 |   3.06 KB |        1.00 |
 *  | Toggle | r3k2r(...)- 0 1 [68] | 5.076 us | 0.0556 us | 0.0493 us | 5.076 us |  0.96 |    0.01 | 0.4959 |   3.06 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | rnbqk(...)6 0 1 [67] | 5.361 us | 0.0999 us | 0.0935 us | 5.341 us |  1.00 |    0.00 | 0.4959 |   3.06 KB |        1.00 |
 *  | Toggle | rnbqk(...)6 0 1 [67] | 5.071 us | 0.0641 us | 0.0569 us | 5.090 us |  0.95 |    0.02 | 0.4959 |   3.06 KB |        1.00 |
 *  |        |                      |          |           |           |          |       |         |        |           |             |
 *  | SetPop | rnbqk(...)- 0 1 [56] | 4.373 us | 0.0782 us | 0.1700 us | 4.303 us |  1.00 |    0.00 | 0.4425 |   2.75 KB |        1.00 |
 *  | Toggle | rnbqk(...)- 0 1 [56] | 4.152 us | 0.0480 us | 0.0449 us | 4.157 us |  0.91 |    0.04 | 0.4425 |   2.75 KB |        1.00 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class ToggleBits_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
    [
        Constants.EmptyBoardFEN,
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1",
        "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1",
        "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9 "
    ];

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