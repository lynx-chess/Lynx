/*
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.19045.3803/22H2/2022Update)
 *  Intel Core i7-5500U CPU 2.40GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------- |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | Naive  | 930.5 ns | 44.77 ns | 132.0 ns | 885.7 ns |  1.00 |    0.00 | 0.1450 |     304 B |        1.00 |
 *  | Smart  | 867.8 ns | 36.15 ns | 106.0 ns | 820.3 ns |  0.95 |    0.18 | 0.1450 |     304 B |        1.00 |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class Piece_IsPawnBishopOrQueen : BaseBenchmark
{
    public static IEnumerable<Piece> Data => new[] {
        Piece.P, Piece.p,
        Piece.N, Piece.n,
        Piece.B, Piece.b,
        Piece.R, Piece.r,
        Piece.Q, Piece.q,
        Piece.K, Piece.k
    };

    [Benchmark(Baseline = true)]
    public bool Naive(Piece nextPiece)
    {
        return nextPiece == Piece.P || nextPiece == Piece.p
            || nextPiece == Piece.B || nextPiece == Piece.b
            || nextPiece == Piece.Q || nextPiece == Piece.q;
    }

    [Benchmark]
    public bool Smart(Piece nextPiece)
    {
        return (int)nextPiece % 2 == 0;
    }
}
