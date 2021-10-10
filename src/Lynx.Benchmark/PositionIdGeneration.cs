/*
 * 
 *  | Method |         newPosition |       Mean |    Error |    StdDev |     Median | Ratio | RatioSD |  Gen 0 | Allocated |
 *  |------- |-------------------- |-----------:|---------:|----------:|-----------:|------:|--------:|-------:|----------:|
 *  |    FEN | Lynx.Model.Position |   955.9 ns | 18.90 ns |  25.87 ns |   954.0 ns |  1.00 |    0.00 | 0.1945 |     408 B |
 *  |    FEN | Lynx.Model.Position | 1,226.2 ns | 98.16 ns | 289.43 ns | 1,086.5 ns |  1.05 |    0.07 | 0.2060 |     432 B |
 *  |    FEN | Lynx.Model.Position |   993.1 ns | 19.72 ns |  41.15 ns |   997.3 ns |  1.05 |    0.06 | 0.2060 |     432 B |
 *  |    FEN | Lynx.Model.Position | 1,054.8 ns | 21.02 ns |  46.58 ns | 1,048.6 ns |  1.10 |    0.06 | 0.2022 |     424 B |
 *  |     Id | Lynx.Model.Position |   479.8 ns |  9.10 ns |  18.80 ns |   478.2 ns |  0.50 |    0.03 | 0.6580 |   1,376 B |
 *  |     Id | Lynx.Model.Position |   501.7 ns | 10.07 ns |  15.08 ns |   500.6 ns |  0.52 |    0.02 | 0.6580 |   1,376 B |
 *  |     Id | Lynx.Model.Position |   521.2 ns | 10.33 ns |  16.39 ns |   518.1 ns |  0.55 |    0.03 | 0.6580 |   1,376 B |
 *  |     Id | Lynx.Model.Position |   455.6 ns |  8.37 ns |  12.52 ns |   455.7 ns |  0.48 |    0.02 | 0.6580 |   1,376 B | 
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Collections.Generic;

namespace Lynx.Benchmark
{
    [SimpleJob]
    public class PositionIdGeneration : BaseBenchmark
    {
        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public string FEN(Position newPosition)
        {
            return newPosition.CalculateFEN();
        }

        /// <summary>
        /// Twice as faster, but allocates three times more
        /// </summary>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string Id(Position newPosition)
        {
            return newPosition.CalculateId();
        }

        public static Position[] Positions => new Position[] {
            new (Constants.InitialPositionFEN),
            new (Constants.TrickyTestPositionFEN),
            new ("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1"),
            new ("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1"),
            new ("r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 1)")
        };

        public static IEnumerable<Position> Data => new[] {
            //Constants.EmptyBoardFEN,
            new Position(Positions[0], Positions[0].AllPossibleMoves()[0]),
            new Position(Positions[1], Positions[1].AllPossibleMoves()[0]),
            new Position(Positions[2], Positions[2].AllPossibleMoves()[0]),
            new Position(Positions[3], Positions[3].AllPossibleMoves()[0])
        };
    }
}
