/*
 *
 *  |                 Method |            position |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD | Allocated |
 *  |----------------------- |-------------------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|
 *  |   Check_LS1B_And_Reset | Lynx.Model.Position | 48.387 ns | 0.8280 ns | 0.7340 ns | 48.232 ns |  1.00 |    0.00 |         - |
 *  |   Check_LS1B_And_Reset | Lynx.Model.Position | 48.213 ns | 0.7675 ns | 0.8212 ns | 48.108 ns |  1.00 |    0.02 |         - |
 *  |   Check_LS1B_And_Reset | Lynx.Model.Position | 48.163 ns | 0.8881 ns | 0.8307 ns | 47.859 ns |  1.00 |    0.02 |         - |
 *  |   Check_LS1B_And_Reset | Lynx.Model.Position | 49.034 ns | 1.0028 ns | 1.4700 ns | 48.437 ns |  1.01 |    0.04 |         - |
 *  |   Check_LS1B_And_Reset | Lynx.Model.Position | 47.857 ns | 0.8548 ns | 0.7577 ns | 47.621 ns |  0.99 |    0.02 |         - |
 *  |   Check_LS1B_And_Reset | Lynx.Model.Position | 49.610 ns | 0.7741 ns | 0.7603 ns | 49.503 ns |  1.03 |    0.03 |         - |
 *  | BitOperations_PopCount | Lynx.Model.Position |  9.556 ns | 0.2291 ns | 0.5577 ns |  9.352 ns |  0.21 |    0.02 |         - |
 *  | BitOperations_PopCount | Lynx.Model.Position | 11.242 ns | 0.8306 ns | 2.4099 ns | 10.128 ns |  0.20 |    0.01 |         - |
 *  | BitOperations_PopCount | Lynx.Model.Position |  9.283 ns | 0.2287 ns | 0.2633 ns |  9.249 ns |  0.19 |    0.01 |         - |
 *  | BitOperations_PopCount | Lynx.Model.Position |  9.184 ns | 0.1203 ns | 0.1004 ns |  9.181 ns |  0.19 |    0.00 |         - |
 *  | BitOperations_PopCount | Lynx.Model.Position |  8.924 ns | 0.2153 ns | 0.1909 ns |  8.926 ns |  0.18 |    0.01 |         - |
 *  | BitOperations_PopCount | Lynx.Model.Position |  9.198 ns | 0.2225 ns | 0.1973 ns |  9.128 ns |  0.19 |    0.00 |         - |
 *
 */


using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class CountBits : BaseBenchmark
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
    public int Check_LS1B_And_Reset(Position position)
    {
        int counter = 0;

        foreach (var bitboard in position.PieceBitBoards)
        {
            counter += CountBits_Naive(bitboard.Board);
        }

        foreach (var bitboard in position.OccupancyBitBoards)
        {
            counter += CountBits_Naive(bitboard.Board);
        }

        return counter;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int BitOperations_PopCount(Position position)
    {
        int counter = 0;

        foreach (var bitboard in position.PieceBitBoards)
        {
            counter += CountBits_PopCount(bitboard.Board);
        }

        foreach (var bitboard in position.OccupancyBitBoards)
        {
            counter += CountBits_PopCount(bitboard.Board);
        }

        return counter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CountBits_Naive(ulong bitboard)
    {
        int counter = 0;

        // Consecutively reset LSB
        while (bitboard != default)
        {
            ++counter;
            bitboard = ResetLS1B(bitboard);
        }

        return counter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CountBits_PopCount(ulong bitboard) => BitOperations.PopCount(bitboard);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ResetLS1B(ulong bitboard)
    {
        return bitboard & (bitboard - 1);
    }
}
