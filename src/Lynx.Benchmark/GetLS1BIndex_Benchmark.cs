/*
 *
 *  |                     Method |            position |     Mean |    Error |   StdDev | Ratio | RatioSD | Allocated |
 *  |--------------------------- |-------------------- |---------:|---------:|---------:|------:|--------:|----------:|
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 23.23 ns | 0.380 ns | 0.355 ns |  1.00 |    0.00 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 24.79 ns | 0.816 ns | 2.394 ns |  1.14 |    0.08 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 22.82 ns | 0.346 ns | 0.289 ns |  0.98 |    0.02 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 21.66 ns | 0.437 ns | 0.365 ns |  0.93 |    0.02 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 21.66 ns | 0.287 ns | 0.269 ns |  0.93 |    0.01 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 23.03 ns | 0.292 ns | 0.259 ns |  0.99 |    0.02 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.65 ns | 0.143 ns | 0.119 ns |  0.67 |    0.01 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.19 ns | 0.170 ns | 0.151 ns |  0.65 |    0.01 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.21 ns | 0.223 ns | 0.198 ns |  0.65 |    0.01 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.03 ns | 0.290 ns | 0.226 ns |  0.65 |    0.01 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.35 ns | 0.155 ns | 0.129 ns |  0.66 |    0.01 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.59 ns | 0.238 ns | 0.222 ns |  0.67 |    0.02 |         - |
 *
 *  |                     Method |            position |     Mean |    Error |   StdDev | Ratio | RatioSD | Allocated |
 *  |--------------------------- |-------------------- |---------:|---------:|---------:|------:|--------:|----------:|
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 23.05 ns | 0.292 ns | 0.273 ns |  1.00 |    0.00 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 21.45 ns | 0.280 ns | 0.262 ns |  0.93 |    0.02 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 23.01 ns | 0.208 ns | 0.194 ns |  1.00 |    0.01 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 22.80 ns | 0.116 ns | 0.091 ns |  0.99 |    0.01 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 21.56 ns | 0.306 ns | 0.286 ns |  0.94 |    0.02 |         - |
 *  |      Original_GetLS1BIndex | Lynx.Model.Position | 21.38 ns | 0.160 ns | 0.141 ns |  0.93 |    0.01 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.72 ns | 0.336 ns | 0.315 ns |  0.68 |    0.02 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.21 ns | 0.348 ns | 0.610 ns |  0.66 |    0.03 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.40 ns | 0.123 ns | 0.109 ns |  0.67 |    0.01 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.79 ns | 0.344 ns | 0.338 ns |  0.69 |    0.02 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.30 ns | 0.171 ns | 0.133 ns |  0.66 |    0.01 |         - |
 *  | BitOperations_GetLS1BIndex | Lynx.Model.Position | 15.47 ns | 0.156 ns | 0.138 ns |  0.67 |    0.01 |         - |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class GetLS1BIndex_Benchmark : BaseBenchmark
{
    public static IEnumerable<Position> Data =>
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Original_GetLS1BIndex(Position position)
    {
        int counter = 0;

        foreach (var bitboard in position.PieceBitboards)
        {
            counter += Original_GetLS1BIndex_Impl(bitboard);
        }

        foreach (var bitboard in position.OccupancyBitboards)
        {
            counter += Original_GetLS1BIndex_Impl(bitboard);
        }

        return counter;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int BitOperations_GetLS1BIndex(Position position)
    {
        int counter = 0;

        foreach (var bitboard in position.PieceBitboards)
        {
            counter += BitOperations_GetLS1BIndex_Impl(bitboard);
        }

        foreach (var bitboard in position.OccupancyBitboards)
        {
            counter += BitOperations_GetLS1BIndex_Impl(bitboard);
        }

        return counter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Original_GetLS1BIndex_Impl(ulong bitboard)
    {
        if (bitboard == default)
        {
            return (int)BoardSquare.noSquare;
        }

        return CountBits(bitboard ^ (bitboard - 1)) - 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BitOperations_GetLS1BIndex_Impl(ulong bitboard)
    {
        if (bitboard == default)
        {
            return (int)BoardSquare.noSquare;
        }

        return BitOperations.TrailingZeroCount(bitboard);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CountBits(ulong bitboard)
    {
        return BitOperations.PopCount(bitboard);
    }
}