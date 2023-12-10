/*
 *
 *  | Method      | pieceIndex | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------ |----------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | If          | 5          | 0.0249 ns | 0.0258 ns | 0.0216 ns | 0.0246 ns |     ? |       ? |         - |           ? |
 *  | BitTrickery | 5          | 0.0084 ns | 0.0132 ns | 0.0123 ns | 0.0000 ns |     ? |       ? |         - |           ? |
 *  |             |            |           |           |           |           |       |         |           |             |
 *  | If          | 11         | 0.0273 ns | 0.0376 ns | 0.0352 ns | 0.0122 ns |     ? |       ? |         - |           ? |
 *  | BitTrickery | 11         | 0.0054 ns | 0.0103 ns | 0.0096 ns | 0.0000 ns |     ? |       ? |         - |           ? |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class KingPieceIndexToSide : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { (int)Piece.K, (int)Piece.k };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int If(int pieceIndex)
    {
        return pieceIndex == (int)Piece.K
            ? 1
            : 0;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int BitTrickery(int pieceIndex)
    {
        return (pieceIndex & 0b111) >> 2;
    }
}
