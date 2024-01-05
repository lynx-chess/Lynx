/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *
BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


| Method               | position            | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|--------------------- |-------------------- |----------:|----------:|----------:|------:|----------:|------------:|
| Naive                | Lynx.Model.Position | 3.6647 ns | 0.0090 ns | 0.0070 ns |  1.00 |         - |          NA |
| Naive                | Lynx.Model.Position | 3.6659 ns | 0.0061 ns | 0.0047 ns |  1.00 |         - |          NA |
| Naive                | Lynx.Model.Position | 3.6757 ns | 0.0245 ns | 0.0229 ns |  1.00 |         - |          NA |
| Naive                | Lynx.Model.Position | 0.5081 ns | 0.0159 ns | 0.0148 ns |  0.14 |         - |          NA |
| Naive                | Lynx.Model.Position | 3.6627 ns | 0.0065 ns | 0.0051 ns |  1.00 |         - |          NA |
| Naive                | Lynx.Model.Position | 3.6720 ns | 0.0089 ns | 0.0074 ns |  1.00 |         - |          NA |
| Dictionary           | Lynx.Model.Position | 4.2204 ns | 0.0277 ns | 0.0259 ns |  1.15 |         - |          NA |
| Dictionary           | Lynx.Model.Position | 4.3162 ns | 0.0289 ns | 0.0241 ns |  1.18 |         - |          NA |
| Dictionary           | Lynx.Model.Position | 4.2144 ns | 0.0260 ns | 0.0243 ns |  1.15 |         - |          NA |
| Dictionary           | Lynx.Model.Position | 4.3022 ns | 0.0205 ns | 0.0171 ns |  1.17 |         - |          NA |
| Dictionary           | Lynx.Model.Position | 4.3205 ns | 0.0279 ns | 0.0261 ns |  1.18 |         - |          NA |
| Dictionary           | Lynx.Model.Position | 4.3908 ns | 0.0279 ns | 0.0261 ns |  1.20 |         - |          NA |
| Switch               | Lynx.Model.Position | 0.6192 ns | 0.0015 ns | 0.0012 ns |  0.17 |         - |          NA |
| Switch               | Lynx.Model.Position | 0.6174 ns | 0.0025 ns | 0.0021 ns |  0.17 |         - |          NA |
| Switch               | Lynx.Model.Position | 0.6277 ns | 0.0132 ns | 0.0124 ns |  0.17 |         - |          NA |
| Switch               | Lynx.Model.Position | 0.4964 ns | 0.0159 ns | 0.0141 ns |  0.14 |         - |          NA |
| Switch               | Lynx.Model.Position | 0.6192 ns | 0.0025 ns | 0.0020 ns |  0.17 |         - |          NA |
| Switch               | Lynx.Model.Position | 0.6189 ns | 0.0017 ns | 0.0014 ns |  0.17 |         - |          NA |
| Switch_Precalculated | Lynx.Model.Position | 0.5519 ns | 0.0102 ns | 0.0096 ns |  0.15 |         - |          NA |
| Switch_Precalculated | Lynx.Model.Position | 0.5597 ns | 0.0119 ns | 0.0112 ns |  0.15 |         - |          NA |
| Switch_Precalculated | Lynx.Model.Position | 0.5493 ns | 0.0049 ns | 0.0043 ns |  0.15 |         - |          NA |
| Switch_Precalculated | Lynx.Model.Position | 0.5647 ns | 0.0108 ns | 0.0101 ns |  0.15 |         - |          NA |
| Switch_Precalculated | Lynx.Model.Position | 0.5602 ns | 0.0121 ns | 0.0107 ns |  0.15 |         - |          NA |
| Switch_Precalculated | Lynx.Model.Position | 0.5573 ns | 0.0055 ns | 0.0049 ns |  0.15 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class ZobristHash_Castle_Benchmark : BaseBenchmark
{
    [Params(1, 10, 100, 1_000, 10_000)]
    public int Size { get; set; }

    private static readonly Position[] _positions =
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    [Benchmark(Baseline = true)]
    public long Naive()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += CalculateMethod(position.Castle);
            }
        }

        return count;
    }

    [Benchmark]
    public long Dictionary()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += DictionaryMethod(position.Castle);
            }
        }

        return count;
    }

    [Benchmark]
    public long Switch()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += SwitchMethod(position.Castle);
            }
        }

        return count;
    }

    [Benchmark]
    public long Switch_Precalculated()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += ZobristTable.EnPassantHash(position.Castle);
            }
        }

        return count;
    }

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
