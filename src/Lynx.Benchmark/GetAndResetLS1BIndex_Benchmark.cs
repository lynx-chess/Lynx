/*
*
*   BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
*   AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
*   .NET SDK 9.0.200
*     [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
*     DefaultJob : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
*
*   | Method                  | position            | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
*   |------------------------ |-------------------- |---------:|---------:|---------:|------:|----------:|------------:|
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.09 ns | 0.027 ns | 0.021 ns |  1.00 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.12 ns | 0.057 ns | 0.048 ns |  1.00 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.18 ns | 0.115 ns | 0.107 ns |  1.00 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.15 ns | 0.107 ns | 0.095 ns |  1.00 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.08 ns | 0.086 ns | 0.080 ns |  1.00 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 19.88 ns | 0.148 ns | 0.123 ns |  0.99 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.41 ns | 0.158 ns | 0.132 ns |  1.07 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.30 ns | 0.097 ns | 0.086 ns |  1.06 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.28 ns | 0.149 ns | 0.132 ns |  1.06 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.37 ns | 0.112 ns | 0.104 ns |  1.06 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.27 ns | 0.095 ns | 0.089 ns |  1.06 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 20.86 ns | 0.063 ns | 0.056 ns |  1.04 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.06 ns | 0.129 ns | 0.114 ns |  1.15 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.02 ns | 0.103 ns | 0.097 ns |  1.15 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 22.95 ns | 0.054 ns | 0.048 ns |  1.14 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.00 ns | 0.104 ns | 0.097 ns |  1.15 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.03 ns | 0.105 ns | 0.099 ns |  1.15 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 22.40 ns | 0.112 ns | 0.099 ns |  1.11 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 22.13 ns | 0.013 ns | 0.010 ns |  1.10 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 22.19 ns | 0.080 ns | 0.075 ns |  1.10 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 22.21 ns | 0.104 ns | 0.092 ns |  1.11 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 22.18 ns | 0.087 ns | 0.081 ns |  1.10 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 22.18 ns | 0.064 ns | 0.057 ns |  1.10 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 22.35 ns | 0.070 ns | 0.059 ns |  1.11 |         - |          NA |
*
*
*   BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.3207) (Hyper-V)
*   AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
*   .NET SDK 9.0.200
*     [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
*     DefaultJob : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
*
*   | Method                  | position            | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
*   |------------------------ |-------------------- |---------:|---------:|---------:|------:|----------:|------------:|
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 21.48 ns | 0.266 ns | 0.236 ns |  1.00 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.05 ns | 0.020 ns | 0.019 ns |  0.93 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.06 ns | 0.037 ns | 0.033 ns |  0.93 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.05 ns | 0.019 ns | 0.015 ns |  0.93 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 20.05 ns | 0.022 ns | 0.019 ns |  0.93 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 19.73 ns | 0.016 ns | 0.014 ns |  0.92 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 20.06 ns | 0.021 ns | 0.020 ns |  0.93 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.24 ns | 0.212 ns | 0.188 ns |  0.99 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.15 ns | 0.232 ns | 0.194 ns |  0.98 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.20 ns | 0.176 ns | 0.165 ns |  0.99 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 21.33 ns | 0.248 ns | 0.232 ns |  0.99 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 20.13 ns | 0.168 ns | 0.149 ns |  0.94 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.19 ns | 0.024 ns | 0.020 ns |  1.08 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.25 ns | 0.024 ns | 0.022 ns |  1.08 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.24 ns | 0.025 ns | 0.023 ns |  1.08 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.06 ns | 0.033 ns | 0.027 ns |  1.07 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.27 ns | 0.038 ns | 0.032 ns |  1.08 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 22.93 ns | 0.019 ns | 0.018 ns |  1.07 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.01 ns | 0.056 ns | 0.053 ns |  1.12 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 23.94 ns | 0.044 ns | 0.039 ns |  1.11 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 23.19 ns | 0.028 ns | 0.025 ns |  1.08 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 23.77 ns | 0.061 ns | 0.054 ns |  1.11 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 23.92 ns | 0.058 ns | 0.051 ns |  1.11 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 23.34 ns | 0.081 ns | 0.072 ns |  1.09 |         - |          NA |
*
*
*   BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
*   Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
*   .NET SDK 9.0.200
*     [Host]     : .NET 9.0.2 (9.0.225.6610), Arm64 RyuJIT AdvSIMD
*     DefaultJob : .NET 9.0.2 (9.0.225.6610), Arm64 RyuJIT AdvSIMD
*
*   | Method                  | position            | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
*   |------------------------ |-------------------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 27.16 ns | 0.497 ns | 0.441 ns |  1.00 |    0.02 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 25.75 ns | 0.535 ns | 0.965 ns |  0.95 |    0.04 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 25.42 ns | 0.540 ns | 0.682 ns |  0.94 |    0.03 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 24.18 ns | 0.082 ns | 0.064 ns |  0.89 |    0.01 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 24.61 ns | 0.497 ns | 0.464 ns |  0.91 |    0.02 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 24.12 ns | 0.491 ns | 0.525 ns |  0.89 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 25.05 ns | 0.441 ns | 0.507 ns |  0.92 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 24.77 ns | 0.507 ns | 0.449 ns |  0.91 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 25.05 ns | 0.399 ns | 0.373 ns |  0.92 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 25.87 ns | 0.499 ns | 0.467 ns |  0.95 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 24.68 ns | 0.497 ns | 0.488 ns |  0.91 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 23.07 ns | 0.395 ns | 0.513 ns |  0.85 |    0.02 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 24.36 ns | 0.482 ns | 0.574 ns |  0.90 |    0.03 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 24.05 ns | 0.032 ns | 0.030 ns |  0.89 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 23.99 ns | 0.027 ns | 0.021 ns |  0.88 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 24.07 ns | 0.066 ns | 0.059 ns |  0.89 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 24.10 ns | 0.023 ns | 0.021 ns |  0.89 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 22.54 ns | 0.020 ns | 0.016 ns |  0.83 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 23.76 ns | 0.032 ns | 0.025 ns |  0.88 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 23.94 ns | 0.028 ns | 0.025 ns |  0.88 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.08 ns | 0.126 ns | 0.105 ns |  0.89 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.13 ns | 0.029 ns | 0.026 ns |  0.89 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.11 ns | 0.035 ns | 0.029 ns |  0.89 |    0.01 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.59 ns | 0.493 ns | 0.587 ns |  0.91 |    0.03 |         - |          NA |
*
*
*   BenchmarkDotNet v0.14.0, macOS Ventura 13.7.4 (22H420) [Darwin 22.6.0]
*   Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
*   .NET SDK 9.0.200
*     [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
*     DefaultJob : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
*
*   | Method                  | position            | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
*   |------------------------ |-------------------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 18.26 ns | 0.312 ns | 0.437 ns |  1.00 |    0.03 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 18.37 ns | 0.328 ns | 0.322 ns |  1.01 |    0.03 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 18.56 ns | 0.394 ns | 0.387 ns |  1.02 |    0.03 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 18.15 ns | 0.226 ns | 0.211 ns |  0.99 |    0.03 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 18.08 ns | 0.182 ns | 0.161 ns |  0.99 |    0.02 |         - |          NA |
*   | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 16.57 ns | 0.081 ns | 0.072 ns |  0.91 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 14.17 ns | 0.116 ns | 0.103 ns |  0.78 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 14.15 ns | 0.054 ns | 0.048 ns |  0.78 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 14.19 ns | 0.143 ns | 0.119 ns |  0.78 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 14.16 ns | 0.096 ns | 0.090 ns |  0.78 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 14.18 ns | 0.095 ns | 0.074 ns |  0.78 |    0.02 |         - |          NA |
*   | WithoutLS1B_OutIndex    | Lynx.Model.Position | 13.77 ns | 0.107 ns | 0.094 ns |  0.75 |    0.02 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 19.90 ns | 0.175 ns | 0.155 ns |  1.09 |    0.03 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 20.09 ns | 0.176 ns | 0.147 ns |  1.10 |    0.03 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 20.00 ns | 0.164 ns | 0.153 ns |  1.10 |    0.03 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 20.01 ns | 0.133 ns | 0.118 ns |  1.10 |    0.03 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 20.15 ns | 0.280 ns | 0.262 ns |  1.10 |    0.03 |         - |          NA |
*   | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 19.43 ns | 0.109 ns | 0.097 ns |  1.06 |    0.02 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.55 ns | 0.465 ns | 0.388 ns |  1.35 |    0.04 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.75 ns | 0.509 ns | 0.523 ns |  1.36 |    0.04 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.59 ns | 0.517 ns | 0.553 ns |  1.35 |    0.04 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.29 ns | 0.292 ns | 0.259 ns |  1.33 |    0.03 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.79 ns | 0.510 ns | 0.501 ns |  1.36 |    0.04 |         - |          NA |
*   | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 24.38 ns | 0.360 ns | 0.370 ns |  1.34 |    0.04 |         - |          NA |
*
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class GetAndResetLS1BIndex_Benchmark : BaseBenchmark
{
    public static IEnumerable<Position> Data =>
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int GetLS1BIndex_ResetLS1B(Position position)
    {
        int result = 0;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (!bitboard.Empty())
            {
                var square = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                result += square;
            }
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int WithoutLS1B_OutIndex(Position position)
    {
        int result = 0;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (!bitboard.Empty())
            {
                bitboard = bitboard.WithoutLS1B_OutIndex(out var square);

                result += square;
            }
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int GetLS1BIndexAndPopIt(Position position)
    {
        int result = 0;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (!bitboard.Empty())
            {
                var square = bitboard.GetLS1BIndexAndPopIt();

                result += square;
            }
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int GetLS1BIndexAndToggleIt(Position position)
    {
        int result = 0;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (!bitboard.Empty())
            {
                var square = bitboard.GetLS1BIndexAndToggleIt();

                result += square;
            }
        }

        return result;
    }
}

internal static class BitBoardExtensions_GetAndResetLS1BIndex_Benchmark
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndex(this BitBoard board)
    {
        Utils.Assert(board != default);

        return BitOperations.TrailingZeroCount(board);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ResetLS1B(this ref BitBoard board)
    {
        board &= (board - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndexAndPopIt(this ref BitBoard board)
    {
        //var index = GetLS1BIndex(board);
        var index = BitOperations.TrailingZeroCount(board);

        // board.PopBit(index);
        board &= ~(1UL << index);

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndexAndToggleIt(this ref BitBoard board)
    {
        //var index = GetLS1BIndex(board);
        var index = BitOperations.TrailingZeroCount(board);

        //board.ToggleBit(index);
        board ^= 1ul << index;

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard WithoutLS1B_OutIndex(this BitBoard board, out int index)
    {
        index = BitOperations.TrailingZeroCount(board);

        // board.WithoutLSQ1B();
        return board & (board - 1);
    }
}