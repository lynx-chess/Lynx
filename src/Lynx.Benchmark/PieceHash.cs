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
 *  | Module   | 0.3194 ns | 0.0039 ns | 0.0032 ns |  1.00 |    0.00 |         - |          NA |
 *  | AndTrick | 0.3471 ns | 0.0159 ns | 0.0141 ns |  1.09 |    0.05 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method   | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |--------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Module   | 0.5044 ns | 0.0036 ns | 0.0030 ns |  1.00 |         - |          NA |
 *  | AndTrick | 0.5051 ns | 0.0034 ns | 0.0027 ns |  1.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method   | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Module   | 0.4056 ns | 0.0725 ns | 0.2033 ns | 0.3153 ns |  1.00 |    0.00 |         - |          NA |
 *  | AndTrick | 0.1020 ns | 0.0627 ns | 0.1849 ns | 0.0000 ns |  0.34 |    0.66 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class PieceHash : BaseBenchmark
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

        var file = enPassantSquare & 0x03;

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
