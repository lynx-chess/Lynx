/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method     | square | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |----------- |------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Dictionary | b6     | 2.5037 ns | 0.0047 ns | 0.0042 ns |  1.00 |         - |          NA |
 *  | Array      | b6     | 0.3192 ns | 0.0244 ns | 0.0228 ns |  0.13 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | d6     | 2.1117 ns | 0.0079 ns | 0.0062 ns |  1.00 |         - |          NA |
 *  | Array      | d6     | 0.3081 ns | 0.0012 ns | 0.0011 ns |  0.15 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | f6     | 2.1361 ns | 0.0075 ns | 0.0063 ns |  1.00 |         - |          NA |
 *  | Array      | f6     | 0.3251 ns | 0.0244 ns | 0.0228 ns |  0.15 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | h6     | 2.1471 ns | 0.0407 ns | 0.0381 ns |  1.00 |         - |          NA |
 *  | Array      | h6     | 0.3160 ns | 0.0188 ns | 0.0176 ns |  0.15 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | a3     | 2.1403 ns | 0.0458 ns | 0.0428 ns |  1.00 |         - |          NA |
 *  | Array      | a3     | 0.3102 ns | 0.0029 ns | 0.0025 ns |  0.14 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | c3     | 2.4238 ns | 0.0417 ns | 0.0390 ns |  1.00 |         - |          NA |
 *  | Array      | c3     | 0.3191 ns | 0.0233 ns | 0.0194 ns |  0.13 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | e3     | 2.1081 ns | 0.0042 ns | 0.0035 ns |  1.00 |         - |          NA |
 *  | Array      | e3     | 0.3221 ns | 0.0231 ns | 0.0216 ns |  0.15 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | g3     | 2.1282 ns | 0.0385 ns | 0.0341 ns |  1.00 |         - |          NA |
 *  | Array      | g3     | 0.3095 ns | 0.0013 ns | 0.0011 ns |  0.15 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method     | square | Mean      | Error     | StdDev    | Median    | Ratio | Allocated | Alloc Ratio |
 *  |----------- |------- |----------:|----------:|----------:|----------:|------:|----------:|------------:|
 *  | Dictionary | b6     | 2.2527 ns | 0.0111 ns | 0.0092 ns | 2.2528 ns |  1.00 |         - |          NA |
 *  | Array      | b6     | 0.2910 ns | 0.0036 ns | 0.0033 ns | 0.2894 ns |  0.13 |         - |          NA |
 *  |            |        |           |           |           |           |       |           |             |
 *  | Dictionary | d6     | 2.2405 ns | 0.0057 ns | 0.0047 ns | 2.2393 ns | 1.000 |         - |          NA |
 *  | Array      | d6     | 0.0011 ns | 0.0015 ns | 0.0014 ns | 0.0000 ns | 0.000 |         - |          NA |
 *  |            |        |           |           |           |           |       |           |             |
 *  | Dictionary | f6     | 2.2561 ns | 0.0089 ns | 0.0083 ns | 2.2539 ns | 1.000 |         - |          NA |
 *  | Array      | f6     | 0.0032 ns | 0.0031 ns | 0.0028 ns | 0.0027 ns | 0.001 |         - |          NA |
 *  |            |        |           |           |           |           |       |           |             |
 *  | Dictionary | h6     | 2.2558 ns | 0.0067 ns | 0.0060 ns | 2.2551 ns | 1.000 |         - |          NA |
 *  | Array      | h6     | 0.0002 ns | 0.0004 ns | 0.0003 ns | 0.0000 ns | 0.000 |         - |          NA |
 *  |            |        |           |           |           |           |       |           |             |
 *  | Dictionary | a3     | 1.9623 ns | 0.0046 ns | 0.0043 ns | 1.9635 ns |  1.00 |         - |          NA |
 *  | Array      | a3     | 0.2893 ns | 0.0011 ns | 0.0009 ns | 0.2893 ns |  0.15 |         - |          NA |
 *  |            |        |           |           |           |           |       |           |             |
 *  | Dictionary | c3     | 1.9713 ns | 0.0074 ns | 0.0062 ns | 1.9743 ns | 1.000 |         - |          NA |
 *  | Array      | c3     | 0.0014 ns | 0.0017 ns | 0.0014 ns | 0.0009 ns | 0.001 |         - |          NA |
 *  |            |        |           |           |           |           |       |           |             |
 *  | Dictionary | e3     | 1.9725 ns | 0.0064 ns | 0.0056 ns | 1.9719 ns | 1.000 |         - |          NA |
 *  | Array      | e3     | 0.0009 ns | 0.0012 ns | 0.0010 ns | 0.0006 ns | 0.000 |         - |          NA |
 *  |            |        |           |           |           |           |       |           |             |
 *  | Dictionary | g3     | 2.2266 ns | 0.0242 ns | 0.0226 ns | 2.2245 ns | 1.000 |         - |          NA |
 *  | Array      | g3     | 0.0008 ns | 0.0011 ns | 0.0009 ns | 0.0005 ns | 0.000 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *
 *  | Method     | square | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |----------- |------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Dictionary | b6     | 3.7197 ns | 0.1121 ns | 0.1903 ns |  1.00 |         - |          NA |
 *  | Array      | b6     | 0.3118 ns | 0.0068 ns | 0.0057 ns |  0.08 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | d6     | 3.6341 ns | 0.0158 ns | 0.0132 ns |  1.00 |         - |          NA |
 *  | Array      | d6     | 0.4346 ns | 0.0474 ns | 0.0582 ns |  0.11 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | f6     | 3.6134 ns | 0.0479 ns | 0.0400 ns |  1.00 |         - |          NA |
 *  | Array      | f6     | 0.3405 ns | 0.0089 ns | 0.0083 ns |  0.09 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | h6     | 3.8879 ns | 0.1147 ns | 0.1127 ns |  1.00 |         - |          NA |
 *  | Array      | h6     | 0.3174 ns | 0.0467 ns | 0.0459 ns |  0.08 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | a3     | 3.9865 ns | 0.0769 ns | 0.0681 ns |  1.00 |         - |          NA |
 *  | Array      | a3     | 0.2815 ns | 0.0325 ns | 0.0288 ns |  0.07 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | c3     | 3.5451 ns | 0.0580 ns | 0.0542 ns |  1.00 |         - |          NA |
 *  | Array      | c3     | 0.3042 ns | 0.0303 ns | 0.0268 ns |  0.09 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | e3     | 3.4793 ns | 0.1037 ns | 0.0970 ns |  1.00 |         - |          NA |
 *  | Array      | e3     | 0.3493 ns | 0.0305 ns | 0.0255 ns |  0.10 |         - |          NA |
 *  |            |        |           |           |           |       |           |             |
 *  | Dictionary | g3     | 3.5165 ns | 0.0173 ns | 0.0162 ns |  1.00 |         - |          NA |
 *  | Array      | g3     | 0.3083 ns | 0.0085 ns | 0.0079 ns |  0.09 |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Collections.Frozen;

namespace Lynx.Benchmark;
public class EnPassantCaptureSquares : BaseBenchmark
{
    public static IEnumerable<BoardSquare> Data => new[] {
        BoardSquare.a3, BoardSquare.c3, BoardSquare.e3, BoardSquare.g3,
        BoardSquare.b6, BoardSquare.d6, BoardSquare.f6, BoardSquare.h6
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Dictionary(BoardSquare square) => EnPassantCaptureSquaresDictionary[(int)square];

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int Array(BoardSquare square) => Constants.EnPassantCaptureSquares[(int)square];

    private static readonly FrozenDictionary<int, int> EnPassantCaptureSquaresDictionary = new Dictionary<int, int>(16)
    {
        [(int)BoardSquare.a6] = (int)BoardSquare.a6 + 8,
        [(int)BoardSquare.b6] = (int)BoardSquare.b6 + 8,
        [(int)BoardSquare.c6] = (int)BoardSquare.c6 + 8,
        [(int)BoardSquare.d6] = (int)BoardSquare.d6 + 8,
        [(int)BoardSquare.e6] = (int)BoardSquare.e6 + 8,
        [(int)BoardSquare.f6] = (int)BoardSquare.f6 + 8,
        [(int)BoardSquare.g6] = (int)BoardSquare.g6 + 8,
        [(int)BoardSquare.h6] = (int)BoardSquare.h6 + 8,

        [(int)BoardSquare.a3] = (int)BoardSquare.a3 - 8,
        [(int)BoardSquare.b3] = (int)BoardSquare.b3 - 8,
        [(int)BoardSquare.c3] = (int)BoardSquare.c3 - 8,
        [(int)BoardSquare.d3] = (int)BoardSquare.d3 - 8,
        [(int)BoardSquare.e3] = (int)BoardSquare.e3 - 8,
        [(int)BoardSquare.f3] = (int)BoardSquare.f3 - 8,
        [(int)BoardSquare.g3] = (int)BoardSquare.g3 - 8,
        [(int)BoardSquare.h3] = (int)BoardSquare.h3 - 8,
    }.ToFrozenDictionary();
}
