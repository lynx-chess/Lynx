/*
 * 
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *  
 *  | Method             | Iterations | Mean         | Error       | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |----------- |-------------:|------------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | StackallocSpan     | 1          |     306.2 ns |     4.27 ns |   4.00 ns |  1.00 |    0.02 |  0.0300 |      - |     760 B |        1.00 |
 *  | DictionaryMemoized | 1          |     163.4 ns |     0.52 ns |   0.46 ns |  0.53 |    0.01 |  0.0136 |      - |     344 B |        0.45 |
 *  | LookupTable        | 1          |     138.1 ns |     0.77 ns |   0.72 ns |  0.45 |    0.01 |  0.0136 |      - |     344 B |        0.45 |
 *  |                    |            |              |             |           |       |         |         |        |           |             |
 *  | StackallocSpan     | 10         |   2,616.4 ns |    36.86 ns |  34.48 ns |  1.00 |    0.02 |  0.2670 |      - |    6712 B |        1.00 |
 *  | DictionaryMemoized | 10         |   1,243.4 ns |     5.40 ns |   4.79 ns |  0.48 |    0.01 |  0.1011 |      - |    2552 B |        0.38 |
 *  | LookupTable        | 10         |     921.4 ns |    14.73 ns |  13.78 ns |  0.35 |    0.01 |  0.1011 | 0.0010 |    2552 B |        0.38 |
 *  |                    |            |              |             |           |       |         |         |        |           |             |
 *  | StackallocSpan     | 100        |  28,467.5 ns |   212.08 ns | 188.00 ns |  1.00 |    0.01 |  2.3193 | 0.1221 |   58704 B |        1.00 |
 *  | DictionaryMemoized | 100        |  10,974.0 ns |    56.00 ns |  49.64 ns |  0.39 |    0.00 |  0.6714 | 0.0305 |   17104 B |        0.29 |
 *  | LookupTable        | 100        |   7,348.7 ns |    55.88 ns |  52.27 ns |  0.26 |    0.00 |  0.6790 | 0.0381 |   17104 B |        0.29 |
 *  |                    |            |              |             |           |       |         |         |        |           |             |
 *  | StackallocSpan     | 1000       | 238,187.9 ns | 1,017.21 ns | 901.73 ns |  1.00 |    0.01 | 21.7285 | 7.0801 |  545608 B |        1.00 |
 *  | DictionaryMemoized | 1000       | 100,827.9 ns |   359.33 ns | 300.06 ns |  0.42 |    0.00 |  5.1270 | 1.2207 |  129608 B |        0.24 |
 *  | LookupTable        | 1000       |  65,389.8 ns |   423.74 ns | 375.63 ns |  0.27 |    0.00 |  5.1270 | 1.2207 |  129608 B |        0.24 |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32370/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *  
 *  | Method             | Iterations | Mean          | Error        | StdDev       | Ratio | RatioSD | Gen0    | Gen1    | Allocated | Alloc Ratio |
 *  |------------------- |----------- |--------------:|-------------:|-------------:|------:|--------:|--------:|--------:|----------:|------------:|
 *  | StackallocSpan     | 1          |     243.67 ns |     2.010 ns |     1.880 ns |  1.00 |    0.01 |  0.0453 |       - |     760 B |        1.00 |
 *  | DictionaryMemoized | 1          |     137.72 ns |     1.149 ns |     1.075 ns |  0.57 |    0.01 |  0.0205 |       - |     344 B |        0.45 |
 *  | LookupTable        | 1          |      91.48 ns |     0.601 ns |     0.533 ns |  0.38 |    0.00 |  0.0205 |       - |     344 B |        0.45 |
 *  |                    |            |               |              |              |       |         |         |         |           |             |
 *  | StackallocSpan     | 10         |   2,207.47 ns |    13.105 ns |    12.258 ns |  1.00 |    0.01 |  0.4005 |  0.0038 |    6712 B |        1.00 |
 *  | DictionaryMemoized | 10         |   1,117.80 ns |     5.681 ns |     5.314 ns |  0.51 |    0.00 |  0.1507 |       - |    2552 B |        0.38 |
 *  | LookupTable        | 10         |     639.13 ns |     3.558 ns |     3.328 ns |  0.29 |    0.00 |  0.1516 |  0.0010 |    2552 B |        0.38 |
 *  |                    |            |               |              |              |       |         |         |         |           |             |
 *  | StackallocSpan     | 100        |  20,847.12 ns |   102.009 ns |    79.642 ns |  1.00 |    0.01 |  3.4790 |  0.1831 |   58704 B |        1.00 |
 *  | DictionaryMemoized | 100        |  10,169.64 ns |    30.387 ns |    28.424 ns |  0.49 |    0.00 |  1.0223 |  0.0458 |   17104 B |        0.29 |
 *  | LookupTable        | 100        |   5,719.49 ns |    19.252 ns |    17.066 ns |  0.27 |    0.00 |  1.0223 |  0.0534 |   17104 B |        0.29 |
 *  |                    |            |               |              |              |       |         |         |         |           |             |
 *  | StackallocSpan     | 1000       | 204,596.21 ns | 2,718.205 ns | 2,409.618 ns |  1.00 |    0.02 | 32.4707 | 10.7422 |  545608 B |        1.00 |
 *  | DictionaryMemoized | 1000       |  95,835.02 ns |   507.290 ns |   474.520 ns |  0.47 |    0.01 |  7.6904 |  1.8311 |  129608 B |        0.24 |
 *  | LookupTable        | 1000       |  52,714.53 ns |   159.151 ns |   148.870 ns |  0.26 |    0.00 |  7.7515 |  1.8921 |  129608 B |        0.24 |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *  
 *  | Method             | Iterations | Mean         | Error       | StdDev      | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |----------- |-------------:|------------:|------------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | StackallocSpan     | 1          |     449.8 ns |     8.19 ns |     7.66 ns |  1.00 |    0.02 |  0.1211 |      - |     760 B |        1.00 |
 *  | DictionaryMemoized | 1          |     211.0 ns |     4.17 ns |     6.84 ns |  0.47 |    0.02 |  0.0534 |      - |     344 B |        0.45 |
 *  | LookupTable        | 1          |     180.4 ns |     3.02 ns |     2.83 ns |  0.40 |    0.01 |  0.0548 |      - |     344 B |        0.45 |
 *  |                    |            |              |             |             |       |         |         |        |           |             |
 *  | StackallocSpan     | 10         |   3,841.0 ns |    72.30 ns |    71.01 ns |  1.00 |    0.03 |  1.0681 | 0.0076 |    6712 B |        1.00 |
 *  | DictionaryMemoized | 10         |   1,449.0 ns |    19.06 ns |    17.83 ns |  0.38 |    0.01 |  0.4063 | 0.0038 |    2552 B |        0.38 |
 *  | LookupTable        | 10         |     997.1 ns |    20.00 ns |    23.81 ns |  0.26 |    0.01 |  0.4063 | 0.0038 |    2552 B |        0.38 |
 *  |                    |            |              |             |             |       |         |         |        |           |             |
 *  | StackallocSpan     | 100        |  33,355.0 ns |   274.13 ns |   228.91 ns |  1.00 |    0.01 |  9.3384 | 0.5493 |   58704 B |        1.00 |
 *  | DictionaryMemoized | 100        |  12,191.7 ns |   146.16 ns |   136.72 ns |  0.37 |    0.00 |  2.7161 | 0.1526 |   17104 B |        0.29 |
 *  | LookupTable        | 100        |   9,268.8 ns |   111.18 ns |    98.55 ns |  0.28 |    0.00 |  2.7161 | 0.1526 |   17104 B |        0.29 |
 *  |                    |            |              |             |             |       |         |         |        |           |             |
 *  | StackallocSpan     | 1000       | 353,692.6 ns | 4,065.58 ns | 3,604.03 ns |  1.00 |    0.01 | 86.9141 | 0.4883 |  545608 B |        1.00 |
 *  | DictionaryMemoized | 1000       | 110,553.6 ns |   966.46 ns |   904.03 ns |  0.31 |    0.00 | 20.5078 | 4.8828 |  129608 B |        0.24 |
 *  | LookupTable        | 1000       |  66,885.3 ns |   932.78 ns |   778.91 ns |  0.19 |    0.00 | 20.5078 | 5.1270 |  129608 B |        0.24 |
 *  
 *  
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *  
 *  | Method             | Iterations | Mean         | Error       | StdDev       | Median       | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |----------- |-------------:|------------:|-------------:|-------------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | StackallocSpan     | 1          |     252.1 ns |     6.88 ns |     19.74 ns |     253.7 ns |  1.01 |    0.11 |  0.1211 |      - |     760 B |        1.00 |
 *  | DictionaryMemoized | 1          |     148.9 ns |     4.29 ns |     12.64 ns |     148.2 ns |  0.59 |    0.07 |  0.0548 |      - |     344 B |        0.45 |
 *  | LookupTable        | 1          |     115.8 ns |     2.34 ns |      6.17 ns |     115.2 ns |  0.46 |    0.04 |  0.0548 |      - |     344 B |        0.45 |
 *  |                    |            |              |             |              |              |       |         |         |        |           |             |
 *  | StackallocSpan     | 10         |   2,393.3 ns |    47.65 ns |    113.25 ns |   2,384.5 ns |  1.00 |    0.07 |  1.0681 | 0.0076 |    6712 B |        1.00 |
 *  | DictionaryMemoized | 10         |   1,093.0 ns |    38.84 ns |    112.69 ns |   1,054.4 ns |  0.46 |    0.05 |  0.4063 | 0.0038 |    2552 B |        0.38 |
 *  | LookupTable        | 10         |     701.3 ns |    33.58 ns |     98.48 ns |     693.6 ns |  0.29 |    0.04 |  0.4063 | 0.0038 |    2552 B |        0.38 |
 *  |                    |            |              |             |              |              |       |         |         |        |           |             |
 *  | StackallocSpan     | 100        |  17,386.9 ns |   528.81 ns |  1,559.21 ns |  16,764.7 ns |  1.01 |    0.13 |  9.3384 | 0.5493 |   58704 B |        1.00 |
 *  | DictionaryMemoized | 100        |   8,255.7 ns |   148.31 ns |    138.73 ns |   8,219.0 ns |  0.48 |    0.04 |  2.7161 | 0.1526 |   17104 B |        0.29 |
 *  | LookupTable        | 100        |   5,099.4 ns |   176.40 ns |    517.36 ns |   4,852.8 ns |  0.30 |    0.04 |  2.7237 | 0.1602 |   17104 B |        0.29 |
 *  |                    |            |              |             |              |              |       |         |         |        |           |             |
 *  | StackallocSpan     | 1000       | 173,924.5 ns | 5,060.00 ns | 14,919.52 ns | 167,184.0 ns |  1.01 |    0.12 | 86.9141 | 0.2441 |  545608 B |        1.00 |
 *  | DictionaryMemoized | 1000       |  83,997.5 ns | 2,105.74 ns |  6,175.77 ns |  82,364.6 ns |  0.49 |    0.05 | 20.5078 | 5.1270 |  129608 B |        0.24 |
 *  | LookupTable        | 1000       |  46,715.1 ns | 1,266.40 ns |  3,734.01 ns |  45,110.2 ns |  0.27 |    0.03 | 20.5688 | 5.1270 |  129608 B |        0.24 |
 * 
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx.Benchmark;

public class Move_UCIString_LookupTable_Benchmark : BaseBenchmark
{
    private static readonly Move[] _moves =
    [
        MoveExtensions.EncodeShortCastle(Constants.InitialWhiteKingSquare, Constants.WhiteKingShortCastleSquare, (int)Piece.K),
        MoveExtensions.EncodeLongCastle(Constants.InitialBlackKingSquare, Constants.BlackKingLongCastleSquare, (int)Piece.k),
        MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P),
        MoveExtensions.EncodePromotion((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.p, promotedPiece: (int)Piece.q),
        MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.b8, (int)Piece.p, promotedPiece: (int)Piece.n, capturedPiece: (int)Piece.B),
        MoveExtensions.EncodeCapture((int)BoardSquare.a8, (int)BoardSquare.h1, (int)Piece.B, capturedPiece: (int)Piece.b),
        MoveExtensions.EncodeEnPassant((int)BoardSquare.e5, (int)BoardSquare.d6, (int)Piece.P),
        MoveExtensions.Encode((int)BoardSquare.g1, (int)BoardSquare.f3, (int)Piece.N),
        MoveExtensions.EncodeDoublePawnPush((int)BoardSquare.d2, (int)BoardSquare.d4, (int)Piece.P),
        MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.Q),
        MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.R),
        MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.B),
        MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.N),
    ];

    [Params(1, 10, 100, 1000)]
    public int Iterations { get; set; }

    [Benchmark(Baseline = true)]
    public StringBuilder StackallocSpan()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Iterations; i++)
        {
            foreach (var move in _moves)
            {
                sb.Append(move.StackallocUCIString());
            }
        }

        return sb;
    }

    [Benchmark]
    public StringBuilder DictionaryMemoized()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Iterations; i++)
        {
            foreach (var move in _moves)
            {
                sb.Append(move.DictionaryMemoizedUCIString());
            }
        }

        return sb;
    }

    [Benchmark]
    public StringBuilder LookupTable()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Iterations; i++)
        {
            foreach (var move in _moves)
            {
                sb.Append(move.LookupTableUCIString());
            }
        }

        return sb;
    }
}

file static class LookupBenchmarkHelpers
{
    private const int SourceSquareOffset = 4;
    private const int TargetSquareOffset = 10;

    [SkipLocalsInit]
    public static string StackallocUCIString(this Move move)
    {
        Span<char> span = stackalloc char[5];

        var source = Constants.CoordinatesCharArray[move.SourceSquare()];
        var target = Constants.CoordinatesCharArray[move.TargetSquare()];

        span[0] = source[0];
        span[1] = source[1];
        span[2] = target[0];
        span[3] = target[1];

        var promotedPiece = move.PromotedPiece();
        if (promotedPiece != default)
        {
            span[4] = Constants.AsciiPiecesLowercase[promotedPiece];

            return span.ToString();
        }

        return span[..^1].ToString();
    }

    private static readonly Dictionary<int, string> _cache = new(4096);

    public static string DictionaryMemoizedUCIString(this Move move)
    {
        if (_cache.TryGetValue(move, out var uciString))
        {
            return uciString;
        }

        var str = move.StackallocUCIString();
        _cache[move] = str;

        return str;
    }

    private static readonly string[] _uciStrings = InitUCIStrings();

    private static string[] InitUCIStrings()
    {
        var result = new string[ushort.MaxValue + 1];

        for (int source = 0; source < 64; source++)
        {
            for (int target = 0; target < 64; target++)
            {
                int baseIndex = (source << SourceSquareOffset) | (target << TargetSquareOffset);
                var baseStr = string.Concat(Constants.Coordinates[source], Constants.Coordinates[target]);
                result[baseIndex] = baseStr;

                for (int promotedPiece = 1; promotedPiece < 12; promotedPiece++)
                {
                    result[baseIndex | promotedPiece] = $"{baseStr}{Constants.AsciiPiecesLowercase[promotedPiece]}";
                }
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string LookupTableUCIString(this Move move) => _uciStrings[move & 0xFFFF];
}
