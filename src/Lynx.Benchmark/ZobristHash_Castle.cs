/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method     | position            | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |----------- |-------------------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Naive      | Lynx.Model.Position | 3.5197 ns | 0.0081 ns | 0.0063 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6657 ns | 0.0367 ns | 0.0343 ns |  1.04 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6447 ns | 0.0145 ns | 0.0129 ns |  1.04 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 0.5235 ns | 0.0033 ns | 0.0026 ns |  0.15 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6404 ns | 0.0116 ns | 0.0108 ns |  1.03 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6410 ns | 0.0135 ns | 0.0106 ns |  1.03 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2018 ns | 0.0184 ns | 0.0144 ns |  1.19 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2320 ns | 0.0407 ns | 0.0381 ns |  1.20 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2310 ns | 0.0413 ns | 0.0386 ns |  1.20 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.1984 ns | 0.0056 ns | 0.0046 ns |  1.19 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.3165 ns | 0.0099 ns | 0.0077 ns |  1.23 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2421 ns | 0.0394 ns | 0.0369 ns |  1.21 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6337 ns | 0.0260 ns | 0.0217 ns |  0.18 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6191 ns | 0.0020 ns | 0.0018 ns |  0.18 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6208 ns | 0.0023 ns | 0.0019 ns |  0.18 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6316 ns | 0.0201 ns | 0.0188 ns |  0.18 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6148 ns | 0.0024 ns | 0.0019 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6134 ns | 0.0019 ns | 0.0016 ns |  0.17 |         - |          NA |
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
 *  | Naive      | Lynx.Model.Position | 3.6603 ns | 0.0344 ns | 0.0269 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6447 ns | 0.0129 ns | 0.0121 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6488 ns | 0.0225 ns | 0.0188 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 0.5395 ns | 0.0017 ns | 0.0016 ns |  0.15 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6643 ns | 0.0123 ns | 0.0109 ns |  1.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 3.6399 ns | 0.0144 ns | 0.0135 ns |  0.99 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9896 ns | 0.0056 ns | 0.0047 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9932 ns | 0.0102 ns | 0.0095 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9969 ns | 0.0050 ns | 0.0042 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9782 ns | 0.0046 ns | 0.0043 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.0010 ns | 0.0097 ns | 0.0081 ns |  1.09 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.9937 ns | 0.0059 ns | 0.0052 ns |  1.09 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6184 ns | 0.0013 ns | 0.0011 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6187 ns | 0.0013 ns | 0.0011 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6209 ns | 0.0038 ns | 0.0033 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6191 ns | 0.0010 ns | 0.0009 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6195 ns | 0.0015 ns | 0.0014 ns |  0.17 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.6187 ns | 0.0011 ns | 0.0009 ns |  0.17 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method     | position            | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------- |-------------------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Naive      | Lynx.Model.Position | 4.5496 ns | 0.1028 ns | 0.1056 ns | 4.5106 ns |  1.00 |    0.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 4.6807 ns | 0.1243 ns | 0.2273 ns | 4.6042 ns |  1.04 |    0.07 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 4.5719 ns | 0.0483 ns | 0.0428 ns | 4.5674 ns |  1.00 |    0.03 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 0.2684 ns | 0.0212 ns | 0.0188 ns | 0.2638 ns |  0.06 |    0.00 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 5.0092 ns | 0.1984 ns | 0.5530 ns | 4.7739 ns |  1.12 |    0.10 |         - |          NA |
 *  | Naive      | Lynx.Model.Position | 4.5935 ns | 0.0967 ns | 0.1113 ns | 4.5459 ns |  1.01 |    0.03 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.5921 ns | 0.0316 ns | 0.0295 ns | 3.6023 ns |  0.79 |    0.02 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.6110 ns | 0.0319 ns | 0.0298 ns | 3.6100 ns |  0.79 |    0.02 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.6556 ns | 0.0760 ns | 0.0673 ns | 3.6318 ns |  0.80 |    0.03 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 4.2231 ns | 0.0405 ns | 0.0339 ns | 4.2144 ns |  0.93 |    0.02 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.6316 ns | 0.0310 ns | 0.0259 ns | 3.6253 ns |  0.80 |    0.02 |         - |          NA |
 *  | Dictionary | Lynx.Model.Position | 3.6691 ns | 0.0879 ns | 0.1203 ns | 3.6119 ns |  0.81 |    0.03 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.7558 ns | 0.0336 ns | 0.0280 ns | 0.7557 ns |  0.17 |    0.01 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.7249 ns | 0.0213 ns | 0.0178 ns | 0.7236 ns |  0.16 |    0.01 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.7213 ns | 0.0314 ns | 0.0419 ns | 0.7132 ns |  0.16 |    0.01 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.7232 ns | 0.0171 ns | 0.0152 ns | 0.7241 ns |  0.16 |    0.01 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.7799 ns | 0.0466 ns | 0.0853 ns | 0.7376 ns |  0.19 |    0.02 |         - |          NA |
 *  | Switch     | Lynx.Model.Position | 0.7437 ns | 0.0120 ns | 0.0093 ns | 0.7464 ns |  0.16 |    0.00 |         - |          NA |
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
