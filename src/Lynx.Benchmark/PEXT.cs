using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class PEXTBenchmark : BaseBenchmark
{
    private readonly Position _position = new Position(Constants.TrickyTestPositionFEN);

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
