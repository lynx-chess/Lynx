/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method   | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Module   | 0.3307 ns | 0.0075 ns | 0.0066 ns |  1.00 |    0.00 |         - |          NA |
 *  | AndTrick | 0.3286 ns | 0.0071 ns | 0.0056 ns |  0.99 |    0.03 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *  | Method   | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |--------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Module   | 0.5209 ns | 0.0014 ns | 0.0013 ns |  1.00 |         - |          NA |
 *  | AndTrick | 0.4598 ns | 0.0018 ns | 0.0014 ns |  0.88 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *
 *  | Method   | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Module   | 0.5491 ns | 0.0484 ns | 0.1092 ns | 0.5070 ns |  1.00 |    0.00 |         - |          NA |
 *  | AndTrick | 0.7057 ns | 0.0292 ns | 0.0287 ns | 0.6935 ns |  1.18 |    0.27 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class ZobristHash_EnPassant : BaseBenchmark
{
    private static readonly long[,] _table = Initialize();

    [Benchmark(Baseline = true)]
    public long Module() => Module((int)BoardSquare.c6);

    [Benchmark]
    public long AndTrick() => AndTrick((int)BoardSquare.c6);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Module(int enPassantSquare)
    {
        if (enPassantSquare == (int)BoardSquare.noSquare)
        {
            return default;
        }

        var file = enPassantSquare % 8;

        return _table[file, (int)Piece.P];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long AndTrick(int enPassantSquare)
    {
        if (enPassantSquare == (int)BoardSquare.noSquare)
        {
            return default;
        }

        var file = enPassantSquare & 0x07;

        return _table[file, (int)Piece.P];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
