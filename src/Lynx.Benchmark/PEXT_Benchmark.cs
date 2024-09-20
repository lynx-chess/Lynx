/*
 *
 * BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 * Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 * .NET SDK 8.0.100-rc.2.23502.2
 *   [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 * | Method       | Mean     | Error   | StdDev  | Ratio | Allocated | Alloc Ratio |
 * |------------- |---------:|--------:|--------:|------:|----------:|------------:|
 * | MagicNumbers | 378.1 ns | 6.19 ns | 5.79 ns |  1.00 |         - |          NA |
 * | PEXT         | 229.7 ns | 2.79 ns | 2.61 ns |  0.61 |         - |          NA |
 *
 * BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 10 (10.0.20348.2031) (Hyper-V)
 * Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
 * .NET SDK 8.0.100-rc.2.23502.2
 *   [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 * | Method       | Mean     | Error   | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
 * |------------- |---------:|--------:|---------:|------:|--------:|----------:|------------:|
 * | MagicNumbers | 408.9 ns | 8.14 ns | 13.59 ns |  1.00 |    0.00 |         - |          NA |
 * | PEXT         | 326.3 ns | 6.46 ns |  7.93 ns |  0.79 |    0.03 |         - |          NA |
 *
 * BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, macOS Monterey 12.6.9 (21G726) [Darwin 1.6.0]
 * Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 * .NET SDK 8.0.100-rc.2.23502.2
 *   [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 * | Method       | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
 * |------------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
 * | MagicNumbers | 436.3 ns | 28.75 ns | 84.33 ns |  1.00 |    0.00 |         - |          NA |
 * | PEXT         | 274.5 ns | 20.23 ns | 58.69 ns |  0.66 |    0.19 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class PEXTBenchmark_Benchmark : BaseBenchmark
{
    private readonly Position _position = new(Constants.TrickyTestPositionFEN);

    [Benchmark(Baseline = true)]
    public ulong MagicNumbers()
    {
        ulong result = default;

        for (int i = 0; i < 64; ++i)
        {
            result |= MagicNumbersRookAttacks(i, _position.OccupancyBitBoards[0]);
            result |= MagicNumbersBishopAttacks(i, _position.OccupancyBitBoards[0]);
        }

        return result;
    }

    [Benchmark]
    public ulong PEXT()
    {
        ulong result = default;

        for (int i = 0; i < 64; ++i)
        {
            result |= PEXTRookAttacks(i, _position.OccupancyBitBoards[0]);
            result |= PEXTBishopAttacks(i, _position.OccupancyBitBoards[0]);
        }

        return result;
    }

    private static BitBoard MagicNumbersRookAttacks(int squareIndex, BitBoard occupancy) => Attacks.MagicNumbersRookAttacks(squareIndex, occupancy);

    private static BitBoard PEXTRookAttacks(int squareIndex, BitBoard occupancy) => Attacks.RookAttacks(squareIndex, occupancy);

    private static BitBoard MagicNumbersBishopAttacks(int squareIndex, BitBoard occupancy) => Attacks.MagicNumbersBishopAttacks(squareIndex, occupancy);

    private static BitBoard PEXTBishopAttacks(int squareIndex, BitBoard occupancy) => Attacks.BishopAttacks(squareIndex, occupancy);
}
