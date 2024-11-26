/*
 *  BenchmarkDotNet v0.13.7, Windows 11 (10.0.22621.2283/22H2/2022Update/SunValley2)
 *  11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *
 *
 *  |    Method |            position |      Mean |     Error |    StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |-------------------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  |  TwoLoops | Lynx.Model.Position |  8.371 ns | 0.1592 ns | 0.1895 ns |  1.00 |    0.00 |         - |          NA |
 *  |  TwoLoops | Lynx.Model.Position |  8.209 ns | 0.0677 ns | 0.0600 ns |  0.98 |    0.02 |         - |          NA |
 *  |  TwoLoops | Lynx.Model.Position |  8.223 ns | 0.0942 ns | 0.0835 ns |  0.98 |    0.03 |         - |          NA |
 *  |  TwoLoops | Lynx.Model.Position |  8.240 ns | 0.0479 ns | 0.0424 ns |  0.99 |    0.03 |         - |          NA |
 *  |  TwoLoops | Lynx.Model.Position |  8.264 ns | 0.1537 ns | 0.1438 ns |  0.99 |    0.03 |         - |          NA |
 *  |  TwoLoops | Lynx.Model.Position |  8.261 ns | 0.0725 ns | 0.0643 ns |  0.99 |    0.03 |         - |          NA |
 *  |  TwoLoops | Lynx.Model.Position |  8.269 ns | 0.1211 ns | 0.1132 ns |  0.99 |    0.03 |         - |          NA |
 *  |  TwoLoops | Lynx.Model.Position |  8.222 ns | 0.0722 ns | 0.0675 ns |  0.98 |    0.03 |         - |          NA |
 *  |  TwoLoops | Lynx.Model.Position |  8.258 ns | 0.1027 ns | 0.0910 ns |  0.99 |    0.03 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 14.767 ns | 0.0670 ns | 0.0594 ns |  1.77 |    0.05 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 14.839 ns | 0.1934 ns | 0.1809 ns |  1.77 |    0.03 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 14.679 ns | 0.0819 ns | 0.0726 ns |  1.76 |    0.04 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 14.813 ns | 0.1072 ns | 0.0950 ns |  1.77 |    0.05 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 14.816 ns | 0.1629 ns | 0.1524 ns |  1.77 |    0.04 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 14.819 ns | 0.2131 ns | 0.1994 ns |  1.77 |    0.05 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 14.796 ns | 0.1755 ns | 0.1465 ns |  1.78 |    0.04 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 14.741 ns | 0.0890 ns | 0.0789 ns |  1.76 |    0.04 |         - |          NA |
 *  |   OneLoop | Lynx.Model.Position | 15.201 ns | 0.3416 ns | 0.4789 ns |  1.83 |    0.07 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class PopulateOcurriencies_Benchmark : BaseBenchmark
{
    public static IEnumerable<Position> Positions =>
        [
            new(Constants.InitialPositionFEN),
            new(Constants.TrickyTestPositionFEN),
            new(Constants.TrickyTestPositionReversedFEN),
            new(Constants.CmkTestPositionFEN),
            new(Constants.TTPositionFEN),
            new("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10"),
            new("3K4/8/8/8/8/8/4p3/2k2R2 b - - 0 1"),
            new("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1"),
            new("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -"),
        ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Positions))]
    public BitBoard[] TwoLoops(Position position) => PopulateOccupancies_TwoLoops(position.PieceBitBoards, position.OccupancyBitBoards);

    [Benchmark]
    [ArgumentsSource(nameof(Positions))]
    public BitBoard[] OneLoop(Position position) => PopulateOccupancies_OneLoop(position.PieceBitBoards, position.OccupancyBitBoards);

    private static BitBoard[] PopulateOccupancies_TwoLoops(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
    {
        var limit = (int)Piece.K;
        for (int piece = (int)Piece.P; piece <= limit; ++piece)
        {
            occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
        }

        limit = (int)Piece.k;
        for (int piece = (int)Piece.p; piece <= limit; ++piece)
        {
            occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
        }

        occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];

        return occupancyBitBoards;
    }

    private static BitBoard[] PopulateOccupancies_OneLoop(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
    {
        for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
        {
            occupancyBitBoards[1 - (piece / 6)] |= pieceBitBoards[piece];
        }

        occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];

        return occupancyBitBoards;
    }
}
