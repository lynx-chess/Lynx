/*
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method | nextPiece | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------- |---------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Naive  | P         | 0.1449 ns | 0.0043 ns | 0.0036 ns | 0.1439 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | P         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | N         | 0.1604 ns | 0.0044 ns | 0.0035 ns | 0.1600 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | N         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | B         | 0.1655 ns | 0.0159 ns | 0.0148 ns | 0.1601 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | B         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | R         | 0.1725 ns | 0.0107 ns | 0.0100 ns | 0.1664 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | R         | 0.0001 ns | 0.0002 ns | 0.0002 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | Q         | 0.1765 ns | 0.0196 ns | 0.0153 ns | 0.1698 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | Q         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | K         | 0.0257 ns | 0.0180 ns | 0.0168 ns | 0.0203 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | K         | 0.0005 ns | 0.0022 ns | 0.0020 ns | 0.0000 ns |  0.05 |    0.18 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | p         | 0.1455 ns | 0.0042 ns | 0.0037 ns | 0.1446 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | p         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | n         | 0.1580 ns | 0.0058 ns | 0.0045 ns | 0.1587 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | n         | 0.0009 ns | 0.0028 ns | 0.0024 ns | 0.0000 ns | 0.007 |    0.02 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | b         | 0.1481 ns | 0.0047 ns | 0.0041 ns | 0.1491 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | b         | 0.1427 ns | 0.0047 ns | 0.0044 ns | 0.1439 ns |  0.96 |    0.05 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | r         | 0.1626 ns | 0.0067 ns | 0.0063 ns | 0.1632 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | r         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | q         | 0.1642 ns | 0.0073 ns | 0.0068 ns | 0.1626 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | q         | 0.0003 ns | 0.0011 ns | 0.0011 ns | 0.0000 ns | 0.002 |    0.01 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | k         | 0.1522 ns | 0.0050 ns | 0.0045 ns | 0.1514 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | k         | 0.0078 ns | 0.0083 ns | 0.0070 ns | 0.0065 ns |  0.05 |    0.05 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method | nextPiece | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------- |---------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Naive  | P         | 0.0407 ns | 0.0351 ns | 0.0345 ns | 0.0377 ns |     ? |       ? |         - |           ? |
 *  | Smart  | P         | 0.0000 ns | 0.0001 ns | 0.0001 ns | 0.0000 ns |     ? |       ? |         - |           ? |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | N         | 0.1627 ns | 0.0168 ns | 0.0157 ns | 0.1610 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | N         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | B         | 0.1334 ns | 0.0049 ns | 0.0043 ns | 0.1320 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | B         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | R         | 0.1568 ns | 0.0093 ns | 0.0087 ns | 0.1595 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | R         | 0.0001 ns | 0.0005 ns | 0.0005 ns | 0.0000 ns | 0.001 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | Q         | 0.0617 ns | 0.0183 ns | 0.0162 ns | 0.0575 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | Q         | 0.0073 ns | 0.0073 ns | 0.0068 ns | 0.0057 ns |  0.13 |    0.11 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | K         | 0.1557 ns | 0.0086 ns | 0.0076 ns | 0.1571 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | K         | 0.0051 ns | 0.0071 ns | 0.0067 ns | 0.0014 ns |  0.03 |    0.04 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | p         | 0.1423 ns | 0.0039 ns | 0.0033 ns | 0.1413 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | p         | 0.0005 ns | 0.0016 ns | 0.0014 ns | 0.0000 ns | 0.004 |    0.01 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | n         | 0.1559 ns | 0.0107 ns | 0.0101 ns | 0.1550 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | n         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | b         | 0.1400 ns | 0.0051 ns | 0.0048 ns | 0.1410 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | b         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | r         | 0.2127 ns | 0.0410 ns | 0.0729 ns | 0.1901 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | r         | 0.0086 ns | 0.0051 ns | 0.0048 ns | 0.0102 ns |  0.05 |    0.03 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | q         | 0.1682 ns | 0.0078 ns | 0.0069 ns | 0.1671 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | q         | 0.0001 ns | 0.0003 ns | 0.0003 ns | 0.0000 ns | 0.001 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | k         | 0.1583 ns | 0.0104 ns | 0.0098 ns | 0.1558 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | k         | 0.0010 ns | 0.0027 ns | 0.0025 ns | 0.0000 ns | 0.006 |    0.02 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method | nextPiece | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------- |---------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Naive  | P         | 0.2116 ns | 0.0330 ns | 0.0324 ns | 0.2033 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | P         | 0.1075 ns | 0.0467 ns | 0.1341 ns | 0.0523 ns |  1.23 |    0.81 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | N         | 1.3323 ns | 0.0760 ns | 0.2168 ns | 1.2382 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | N         | 0.0177 ns | 0.0163 ns | 0.0358 ns | 0.0000 ns |  0.01 |    0.03 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | B         | 0.4286 ns | 0.0366 ns | 0.0305 ns | 0.4282 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | B         | 0.0022 ns | 0.0050 ns | 0.0041 ns | 0.0000 ns | 0.005 |    0.01 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | R         | 1.3235 ns | 0.0565 ns | 0.1076 ns | 1.2989 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | R         | 0.0001 ns | 0.0005 ns | 0.0004 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | Q         | 1.0160 ns | 0.0485 ns | 0.0679 ns | 1.0027 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | Q         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | K         | 1.2449 ns | 0.0547 ns | 0.1202 ns | 1.2179 ns |  1.00 |    0.00 |         - |          NA |
 *  | Smart  | K         | 0.0254 ns | 0.0240 ns | 0.0533 ns | 0.0000 ns |  0.02 |    0.04 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | p         | 0.1971 ns | 0.0177 ns | 0.0165 ns | 0.2013 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | p         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | n         | 1.1230 ns | 0.0279 ns | 0.0247 ns | 1.1218 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | n         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | b         | 0.7417 ns | 0.0150 ns | 0.0125 ns | 0.7424 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | b         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | r         | 1.2264 ns | 0.0346 ns | 0.0306 ns | 1.2200 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | r         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | q         | 1.1813 ns | 0.0118 ns | 0.0099 ns | 1.1839 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | q         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *  |        |           |           |           |           |           |       |         |           |             |
 *  | Naive  | k         | 1.1846 ns | 0.0081 ns | 0.0068 ns | 1.1829 ns | 1.000 |    0.00 |         - |          NA |
 *  | Smart  | k         | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class Piece_IsPawnBishopOrQueen_Benchmark : BaseBenchmark
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
    [ArgumentsSource(nameof(Data))]
    public bool Naive(Piece nextPiece)
    {
        return nextPiece == Piece.P || nextPiece == Piece.p
            || nextPiece == Piece.B || nextPiece == Piece.b
            || nextPiece == Piece.Q || nextPiece == Piece.q;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool Smart(Piece nextPiece)
    {
        return (int)nextPiece % 2 == 0;
    }
}
