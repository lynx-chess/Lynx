/*
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method                  | position            | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------------------ |-------------------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.184 ns | 0.0806 ns | 0.0673 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.185 ns | 0.1310 ns | 0.1226 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.192 ns | 0.0601 ns | 0.0533 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.165 ns | 0.0120 ns | 0.0100 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.103 ns | 0.1859 ns | 0.1451 ns |  0.99 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.156 ns | 0.0328 ns | 0.0291 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  7.673 ns | 0.1576 ns | 0.1475 ns |  0.69 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  7.666 ns | 0.1749 ns | 0.1636 ns |  0.69 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  6.922 ns | 0.0195 ns | 0.0163 ns |  0.62 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  6.964 ns | 0.0591 ns | 0.0553 ns |  0.62 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  6.944 ns | 0.0608 ns | 0.0539 ns |  0.62 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  6.922 ns | 0.0620 ns | 0.0518 ns |  0.62 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.137 ns | 0.1436 ns | 0.1343 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.168 ns | 0.0694 ns | 0.0649 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.146 ns | 0.0610 ns | 0.0571 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.124 ns | 0.0118 ns | 0.0099 ns |  0.99 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.140 ns | 0.1277 ns | 0.1132 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.141 ns | 0.1154 ns | 0.1079 ns |  1.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2655) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method                  | position            | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------------------ |-------------------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.157 ns | 0.0323 ns | 0.0252 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.129 ns | 0.0350 ns | 0.0292 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.111 ns | 0.0974 ns | 0.0863 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.134 ns | 0.0261 ns | 0.0231 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.155 ns | 0.0308 ns | 0.0273 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 11.122 ns | 0.0658 ns | 0.0549 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  7.286 ns | 0.0387 ns | 0.0343 ns |  0.65 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  7.291 ns | 0.0307 ns | 0.0287 ns |  0.65 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  7.269 ns | 0.0352 ns | 0.0294 ns |  0.65 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  7.281 ns | 0.0363 ns | 0.0321 ns |  0.65 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  7.260 ns | 0.0351 ns | 0.0328 ns |  0.65 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position |  7.268 ns | 0.0346 ns | 0.0324 ns |  0.65 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.163 ns | 0.0685 ns | 0.0607 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.184 ns | 0.0287 ns | 0.0255 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.160 ns | 0.0820 ns | 0.0767 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.168 ns | 0.0552 ns | 0.0461 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.151 ns | 0.0727 ns | 0.0644 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 11.172 ns | 0.0460 ns | 0.0384 ns |  1.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *
 *  | Method                  | position            | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------------------ |-------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.356 ns | 0.0117 ns | 0.0097 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.353 ns | 0.0116 ns | 0.0096 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.358 ns | 0.0132 ns | 0.0103 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.359 ns | 0.0096 ns | 0.0085 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.360 ns | 0.0121 ns | 0.0101 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.360 ns | 0.0146 ns | 0.0122 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.347 ns | 0.0060 ns | 0.0051 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.382 ns | 0.0295 ns | 0.0261 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.356 ns | 0.0070 ns | 0.0058 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.372 ns | 0.0256 ns | 0.0227 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.357 ns | 0.0079 ns | 0.0070 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.351 ns | 0.0057 ns | 0.0048 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.355 ns | 0.0081 ns | 0.0076 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.368 ns | 0.0359 ns | 0.0300 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.353 ns | 0.0080 ns | 0.0067 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.357 ns | 0.0080 ns | 0.0063 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.357 ns | 0.0028 ns | 0.0023 ns |  1.00 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.353 ns | 0.0111 ns | 0.0087 ns |  1.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.6.9 (22G830) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method                  | position            | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------------------ |-------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.258 ns | 0.0381 ns | 0.0338 ns |  1.00 |    0.01 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.254 ns | 0.0173 ns | 0.0153 ns |  1.00 |    0.01 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.301 ns | 0.0546 ns | 0.0456 ns |  1.01 |    0.01 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.274 ns | 0.0475 ns | 0.0444 ns |  1.00 |    0.01 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.280 ns | 0.0532 ns | 0.0444 ns |  1.00 |    0.01 |         - |          NA |
 *  | GetLS1BIndex_ResetLS1B  | Lynx.Model.Position | 5.282 ns | 0.0443 ns | 0.0414 ns |  1.00 |    0.01 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.993 ns | 0.1501 ns | 0.1787 ns |  1.14 |    0.03 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.991 ns | 0.1493 ns | 0.1778 ns |  1.14 |    0.03 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.827 ns | 0.1073 ns | 0.1004 ns |  1.11 |    0.02 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.790 ns | 0.0721 ns | 0.0639 ns |  1.10 |    0.01 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.766 ns | 0.0737 ns | 0.0653 ns |  1.10 |    0.01 |         - |          NA |
 *  | GetLS1BIndexAndPopIt    | Lynx.Model.Position | 5.850 ns | 0.1006 ns | 0.0840 ns |  1.11 |    0.02 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.789 ns | 0.1451 ns | 0.1670 ns |  1.10 |    0.03 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.763 ns | 0.0807 ns | 0.0630 ns |  1.10 |    0.01 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 6.019 ns | 0.1452 ns | 0.2797 ns |  1.14 |    0.05 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.926 ns | 0.1450 ns | 0.2126 ns |  1.13 |    0.04 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.935 ns | 0.1145 ns | 0.1881 ns |  1.13 |    0.04 |         - |          NA |
 *  | GetLS1BIndexAndToggleIt | Lynx.Model.Position | 5.909 ns | 0.1409 ns | 0.1623 ns |  1.12 |    0.03 |         - |          NA |
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

            var square = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            result += square;
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

            var square = bitboard.GetLS1BIndexAndPopIt();

            result += square;
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

            var square = bitboard.GetLS1BIndexAndToggleIt();

            result += square;
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
        var index = GetLS1BIndex(board);
        board.PopBit(index);

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndexAndToggleIt(this ref BitBoard board)
    {
        var index = GetLS1BIndex(board);
        board.ToggleBit(index);

        return index;
    }
}
