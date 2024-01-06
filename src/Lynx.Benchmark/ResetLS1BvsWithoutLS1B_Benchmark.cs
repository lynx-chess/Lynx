/*
 * Not much noticeable difference with this benchmark
 *
 *  |      Method |            position |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD | Allocated |
 *  |------------ |-------------------- |---------:|---------:|---------:|---------:|------:|--------:|----------:|
 *  |   ResetLS1B | Lynx.Model.Position | 14.58 ns | 0.503 ns | 1.403 ns | 14.02 ns |  1.00 |    0.00 |         - |
 *  |   ResetLS1B | Lynx.Model.Position | 14.79 ns | 0.593 ns | 1.683 ns | 14.02 ns |  1.02 |    0.13 |         - |
 *  |   ResetLS1B | Lynx.Model.Position | 13.92 ns | 0.316 ns | 0.351 ns | 13.87 ns |  0.96 |    0.08 |         - |
 *  |   ResetLS1B | Lynx.Model.Position | 13.58 ns | 0.310 ns | 0.369 ns | 13.52 ns |  0.94 |    0.08 |         - |
 *  |   ResetLS1B | Lynx.Model.Position | 15.44 ns | 0.808 ns | 2.332 ns | 14.21 ns |  1.07 |    0.19 |         - |
 *  |   ResetLS1B | Lynx.Model.Position | 13.87 ns | 0.317 ns | 0.445 ns | 13.84 ns |  0.95 |    0.10 |         - |
 *  | WithoutLS1B | Lynx.Model.Position | 13.64 ns | 0.315 ns | 0.518 ns | 13.49 ns |  0.94 |    0.09 |         - |
 *  | WithoutLS1B | Lynx.Model.Position | 14.64 ns | 0.647 ns | 1.856 ns | 13.75 ns |  1.01 |    0.16 |         - |
 *  | WithoutLS1B | Lynx.Model.Position | 14.12 ns | 0.275 ns | 0.679 ns | 13.97 ns |  0.97 |    0.10 |         - |
 *  | WithoutLS1B | Lynx.Model.Position | 14.11 ns | 0.261 ns | 0.310 ns | 14.06 ns |  0.98 |    0.07 |         - |
 *  | WithoutLS1B | Lynx.Model.Position | 14.05 ns | 0.315 ns | 0.294 ns | 14.00 ns |  0.98 |    0.07 |         - |
 *  | WithoutLS1B | Lynx.Model.Position | 14.08 ns | 0.303 ns | 0.284 ns | 14.10 ns |  0.98 |    0.06 |         - |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public static class BenchmarkExtensions
{
    public static void ResetLS1BBenchmark(this ref BitBoard board) => board &= (board - 1);
    public static BitBoard WithoutLS1BBenchmark(this BitBoard board) => board & (board - 1);
}

public class ResetLS1BvsWithoutLS1B_Benchmark : BaseBenchmark
{
    public static IEnumerable<Position> Data => new[] {
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ulong ResetLS1B(Position position)
    {
        ulong counter = 0;

        for (int i = 0; i < position.PieceBitBoards.Length; ++i)
        {
            var bitboard = position.PieceBitBoards[i];
            bitboard.ResetLS1BBenchmark();
            counter += bitboard;
        }

        for (int i = 0; i < position.OccupancyBitBoards.Length; ++i)
        {
            var bitboard = position.OccupancyBitBoards[i];
            bitboard.ResetLS1BBenchmark();
            counter += bitboard;
        }

        return counter;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong WithoutLS1B(Position position)
    {
        ulong counter = 0;

        for (int i = 0; i < position.PieceBitBoards.Length; ++i)
        {
            var bitboard = position.PieceBitBoards[i];
            counter += bitboard.WithoutLS1BBenchmark();
        }

        for (int i = 0; i < position.OccupancyBitBoards.Length; ++i)
        {
            var bitboard = position.OccupancyBitBoards[i];
            counter += bitboard.WithoutLS1BBenchmark();
        }

        return counter;
    }
}
