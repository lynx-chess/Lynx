/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method   | position            | Mean      | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |--------- |-------------------- |----------:|---------:|---------:|------:|----------:|------------:|
 *  | Original | Lynx.Model.Position | 557.37 ns | 4.251 ns | 3.977 ns |  1.00 |         - |          NA |
 *  | Original | Lynx.Model.Position | 550.96 ns | 4.694 ns | 4.391 ns |  0.99 |         - |          NA |
 *  | Original | Lynx.Model.Position | 548.83 ns | 2.298 ns | 1.919 ns |  0.98 |         - |          NA |
 *  | Original | Lynx.Model.Position | 580.34 ns | 4.985 ns | 4.419 ns |  1.04 |         - |          NA |
 *  | Original | Lynx.Model.Position | 542.14 ns | 3.881 ns | 3.630 ns |  0.97 |         - |          NA |
 *  | Original | Lynx.Model.Position | 589.89 ns | 3.217 ns | 3.009 ns |  1.06 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  45.48 ns | 0.265 ns | 0.235 ns |  0.08 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  46.10 ns | 0.426 ns | 0.398 ns |  0.08 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  45.53 ns | 0.210 ns | 0.186 ns |  0.08 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  44.13 ns | 0.253 ns | 0.237 ns |  0.08 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  45.63 ns | 0.222 ns | 0.208 ns |  0.08 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  47.06 ns | 0.169 ns | 0.150 ns |  0.08 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method   | position            | Mean      | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
 *  |--------- |-------------------- |----------:|---------:|---------:|------:|----------:|------------:|
 *  | Original | Lynx.Model.Position | 539.63 ns | 2.678 ns | 2.090 ns |  1.00 |         - |          NA |
 *  | Original | Lynx.Model.Position | 544.14 ns | 1.561 ns | 1.384 ns |  1.01 |         - |          NA |
 *  | Original | Lynx.Model.Position | 550.08 ns | 3.062 ns | 2.714 ns |  1.02 |         - |          NA |
 *  | Original | Lynx.Model.Position | 577.32 ns | 3.193 ns | 2.830 ns |  1.07 |         - |          NA |
 *  | Original | Lynx.Model.Position | 551.03 ns | 1.454 ns | 1.289 ns |  1.02 |         - |          NA |
 *  | Original | Lynx.Model.Position | 593.00 ns | 2.658 ns | 2.487 ns |  1.10 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  46.42 ns | 0.135 ns | 0.120 ns |  0.09 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  46.46 ns | 0.138 ns | 0.129 ns |  0.09 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  46.65 ns | 0.227 ns | 0.189 ns |  0.09 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  44.71 ns | 0.817 ns | 0.682 ns |  0.08 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  46.56 ns | 0.176 ns | 0.164 ns |  0.09 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  46.13 ns | 0.142 ns | 0.132 ns |  0.09 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method   | position            | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |-------------------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Original | Lynx.Model.Position | 441.53 ns |  7.102 ns |  6.643 ns | 442.30 ns |  1.00 |    0.00 |         - |          NA |
 *  | Original | Lynx.Model.Position | 423.40 ns |  8.091 ns |  9.317 ns | 422.60 ns |  0.96 |    0.03 |         - |          NA |
 *  | Original | Lynx.Model.Position | 484.47 ns | 11.096 ns | 32.542 ns | 491.36 ns |  0.96 |    0.03 |         - |          NA |
 *  | Original | Lynx.Model.Position | 503.78 ns | 10.035 ns | 16.766 ns | 505.44 ns |  1.13 |    0.05 |         - |          NA |
 *  | Original | Lynx.Model.Position | 510.14 ns |  9.727 ns |  9.553 ns | 510.72 ns |  1.16 |    0.03 |         - |          NA |
 *  | Original | Lynx.Model.Position | 513.48 ns | 14.230 ns | 41.956 ns | 522.35 ns |  1.19 |    0.05 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  58.91 ns |  1.811 ns |  5.284 ns |  60.56 ns |  0.12 |    0.01 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  58.95 ns |  1.663 ns |  4.902 ns |  60.06 ns |  0.13 |    0.01 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  56.61 ns |  1.997 ns |  5.731 ns |  56.60 ns |  0.12 |    0.01 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  52.01 ns |  1.107 ns |  3.140 ns |  52.15 ns |  0.12 |    0.00 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  55.71 ns |  1.620 ns |  4.752 ns |  54.88 ns |  0.12 |    0.01 |         - |          NA |
 *  | Improved | Lynx.Model.Position |  59.73 ns |  1.821 ns |  5.194 ns |  59.65 ns |  0.14 |    0.01 |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class ZobristPositionHash : BaseBenchmark
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
    public long Original(Position position) => PositionHash_Original_DoubleLoop(position);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long Improved(Position position) => PositionHash_Improved(position);

    private static long PositionHash_Original_DoubleLoop(Position position)
    {
        long positionHash = 0;

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                if (position.PieceBitBoards[pieceIndex].GetBit(squareIndex))
                {
                    positionHash ^= ZobristTable.PieceHash(squareIndex, pieceIndex);
                }
            }
        }

        positionHash ^= ZobristTable.EnPassantHash((int)position.EnPassant)
            ^ ZobristTable.SideHash()
            ^ ZobristTable.CastleHash(position.Castle);

        return positionHash;
    }

    private static long PositionHash_Improved(Position position)
    {
        long positionHash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                var pieceSquareIndex = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                positionHash ^= ZobristTable.PieceHash(pieceSquareIndex, pieceIndex);
            }
        }

        positionHash ^= ZobristTable.EnPassantHash((int)position.EnPassant)
            ^ ZobristTable.SideHash()
            ^ ZobristTable.CastleHash(position.Castle);

        return positionHash;
    }
}
