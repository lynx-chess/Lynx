/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *
 *  | Method     | position            | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |----------- |-------------------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Naive      | Lynx.Model.Position | 3.6573 ns | 0.0236 ns | 0.0221 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6536 ns | 0.0270 ns | 0.0253 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6658 ns | 0.0390 ns | 0.0345 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 0.5306 ns | 0.0131 ns | 0.0116 ns |  0.15 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6498 ns | 0.0266 ns | 0.0236 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6450 ns | 0.0182 ns | 0.0152 ns |  1.00 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2155 ns | 0.0417 ns | 0.0348 ns |  1.15 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2197 ns | 0.0365 ns | 0.0342 ns |  1.15 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2167 ns | 0.0281 ns | 0.0263 ns |  1.15 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2664 ns | 0.0254 ns | 0.0238 ns |  1.17 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2085 ns | 0.0254 ns | 0.0237 ns |  1.15 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2174 ns | 0.0287 ns | 0.0254 ns |  1.15 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6271 ns | 0.0158 ns | 0.0147 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6266 ns | 0.0135 ns | 0.0127 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6262 ns | 0.0151 ns | 0.0141 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.4921 ns | 0.0133 ns | 0.0124 ns |  0.13 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6203 ns | 0.0014 ns | 0.0013 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6133 ns | 0.0126 ns | 0.0118 ns |  0.17 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method     | position            | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |----------- |-------------------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Naive      | Lynx.Model.Position | 3.6573 ns | 0.0171 ns | 0.0133 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6489 ns | 0.0170 ns | 0.0150 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6426 ns | 0.0147 ns | 0.0138 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 0.5414 ns | 0.0018 ns | 0.0017 ns |  0.15 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6448 ns | 0.0237 ns | 0.0210 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6530 ns | 0.0153 ns | 0.0127 ns |  1.00 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9936 ns | 0.0076 ns | 0.0068 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9949 ns | 0.0064 ns | 0.0053 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9996 ns | 0.0084 ns | 0.0079 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9819 ns | 0.0039 ns | 0.0036 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9955 ns | 0.0061 ns | 0.0051 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9970 ns | 0.0041 ns | 0.0036 ns |  1.09 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.3095 ns | 0.0015 ns | 0.0012 ns |  0.08 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.3101 ns | 0.0021 ns | 0.0018 ns |  0.08 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.3082 ns | 0.0010 ns | 0.0009 ns |  0.08 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.3116 ns | 0.0027 ns | 0.0025 ns |  0.09 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.3088 ns | 0.0016 ns | 0.0014 ns |  0.08 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.3107 ns | 0.0017 ns | 0.0015 ns |  0.08 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *
 *  | Method     | position            | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------- |-------------------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Naive      | Lynx.Model.Position | 5.4659 ns | 0.1821 ns | 0.4924 ns | 5.2647 ns |  1.00 |    0.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 5.1548 ns | 0.1188 ns | 0.1271 ns | 5.1218 ns |  0.94 |    0.10 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 5.0598 ns | 0.0379 ns | 0.0354 ns | 5.0433 ns |  0.92 |    0.09 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 0.6435 ns | 0.0152 ns | 0.0127 ns | 0.6474 ns |  0.12 |    0.01 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 5.0734 ns | 0.0417 ns | 0.0370 ns | 5.0662 ns |  0.93 |    0.09 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 5.0377 ns | 0.0530 ns | 0.0414 ns | 5.0147 ns |  0.92 |    0.10 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 5.5652 ns | 0.0642 ns | 0.0569 ns | 5.5691 ns |  1.02 |    0.10 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 5.5223 ns | 0.1023 ns | 0.0957 ns | 5.5145 ns |  1.01 |    0.10 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 6.5139 ns | 0.0813 ns | 0.0721 ns | 6.4870 ns |  1.20 |    0.12 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 6.7231 ns | 0.1553 ns | 0.1453 ns | 6.7304 ns |  1.23 |    0.10 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 5.7433 ns | 0.1528 ns | 0.1429 ns | 5.7473 ns |  1.05 |    0.11 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 5.5860 ns | 0.0609 ns | 0.0508 ns | 5.5894 ns |  1.02 |    0.10 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 1.2280 ns | 0.0627 ns | 0.0587 ns | 1.2163 ns |  0.22 |    0.02 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 1.1605 ns | 0.0602 ns | 0.0564 ns | 1.1582 ns |  0.21 |    0.02 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 1.1891 ns | 0.0371 ns | 0.0347 ns | 1.1862 ns |  0.22 |    0.02 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.8324 ns | 0.0356 ns | 0.0333 ns | 0.8413 ns |  0.15 |    0.02 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 1.1439 ns | 0.0257 ns | 0.0228 ns | 1.1468 ns |  0.21 |    0.02 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 1.1235 ns | 0.0397 ns | 0.0332 ns | 1.1063 ns |  0.21 |    0.02 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class ZobristHash_Castle : BaseBenchmark
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
    public long Naive(Position position) => CalculateMethod(position.Castle);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long Dictionary(Position position) => DictionaryMethod(position.Castle);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long Switch(Position position) => SwitchMethod(position.Castle);

    private static readonly long[,] _table = Initialize();

    private static readonly long WK_Hash = _table[(int)BoardSquare.a8, (int)Piece.p];
    private static readonly long WQ_Hash = _table[(int)BoardSquare.b8, (int)Piece.p];
    private static readonly long BK_Hash = _table[(int)BoardSquare.c8, (int)Piece.p];
    private static readonly long BQ_Hash = _table[(int)BoardSquare.d8, (int)Piece.p];

    private static readonly Dictionary<byte, long> _castleHashDictionary = new()
    {
        [0] = 0,                                // -    | -
        [(byte)CastlingRights.WK] = WK_Hash,    // K    | -
        [(byte)CastlingRights.WQ] = WQ_Hash,    // Q    | -
        [(byte)CastlingRights.BK] = BK_Hash,    // -    | k
        [(byte)CastlingRights.BQ] = BQ_Hash,    // -    | q

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ] = WK_Hash ^ WQ_Hash,    // KQ   | -
        [(byte)CastlingRights.WK | (byte)CastlingRights.BK] = WK_Hash ^ BK_Hash,    // K    | k
        [(byte)CastlingRights.WK | (byte)CastlingRights.BQ] = WK_Hash ^ BQ_Hash,    // K    | q
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BK] = WQ_Hash ^ BK_Hash,    // Q    | k
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BQ] = WQ_Hash ^ BQ_Hash,    // Q    | q
        [(byte)CastlingRights.BK | (byte)CastlingRights.BQ] = BK_Hash ^ BQ_Hash,    // -    | kq

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK] = WK_Hash ^ WQ_Hash ^ BK_Hash,    // KQ   | k
        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ] = WK_Hash ^ WQ_Hash ^ BQ_Hash,    // KQ   | q
        [(byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] = WK_Hash ^ BK_Hash ^ BQ_Hash,    // K    | kq
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] = WQ_Hash ^ BK_Hash ^ BQ_Hash,    // Q    | kq

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] =       // KQ   | kq
            WK_Hash ^ WQ_Hash ^ BK_Hash ^ BQ_Hash
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long CalculateMethod(byte castle)
    {
        long combinedHash = 0;

        if ((castle & (int)CastlingRights.WK) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.a8, (int)Piece.p];        // a8
        }

        if ((castle & (int)CastlingRights.WQ) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.b8, (int)Piece.p];        // b8
        }

        if ((castle & (int)CastlingRights.BK) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.c8, (int)Piece.p];        // c8
        }

        if ((castle & (int)CastlingRights.BQ) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.d8, (int)Piece.p];        // d8
        }

        return combinedHash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long DictionaryMethod(byte castle) => _castleHashDictionary[castle];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SwitchMethod(byte castle)
    {
        return castle switch
        {
            0 => 0,                                // -    | -

            (byte)CastlingRights.WK => WK_Hash,    // K    | -
            (byte)CastlingRights.WQ => WQ_Hash,    // Q    | -
            (byte)CastlingRights.BK => BK_Hash,    // -    | k
            (byte)CastlingRights.BQ => BQ_Hash,    // -    | q

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ => WK_Hash ^ WQ_Hash,    // KQ   | -
            (byte)CastlingRights.WK | (byte)CastlingRights.BK => WK_Hash ^ BK_Hash,    // K    | k
            (byte)CastlingRights.WK | (byte)CastlingRights.BQ => WK_Hash ^ BQ_Hash,    // K    | q
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WQ_Hash ^ BK_Hash,    // Q    | k
            (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WQ_Hash ^ BQ_Hash,    // Q    | q
            (byte)CastlingRights.BK | (byte)CastlingRights.BQ => BK_Hash ^ BQ_Hash,    // -    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WK_Hash ^ WQ_Hash ^ BK_Hash,    // KQ   | k
            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WK_Hash ^ WQ_Hash ^ BQ_Hash,    // KQ   | q
            (byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WK_Hash ^ BK_Hash ^ BQ_Hash,    // K    | kq
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WQ_Hash ^ BK_Hash ^ BQ_Hash,    // Q    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ =>       // KQ   | kq
                WK_Hash ^ WQ_Hash ^ BK_Hash ^ BQ_Hash,

            _ => new()
        };
    }

    /// <summary>
    /// Initializes Zobrist table (long[64, 12])
    /// </summary>
    /// <returns></returns>
    private static long[,] Initialize()
    {
        var zobristTable = new long[64, 12];
        var randomInstance = new Random(int.MaxValue);

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                zobristTable[squareIndex, pieceIndex] = randomInstance.NextInt64();
            }
        }

        return zobristTable;
    }
}
