using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class PawnAttacks : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { 64, 1_000, 10_000, 100_000 };

    /// <summary>
    /// Best for data <= 1000 (64 in real life)
    /// </summary>
    /// <param name="data"></param>
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public void InitializePawnAttacks_Sequential(int data)
    {
        BitBoard[,] pawnAttacks = new BitBoard[2, data];

        for (int square = 0; square < data; ++square)
        {
            pawnAttacks[0, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false);
            pawnAttacks[1, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: true);
        }
    }

    /// <summary>
    /// Only starts to makes sense somewhere between 1_000 < data <= 10_000
    /// </summary>
    /// <param name="data"></param>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void InitializePawnAttacks_Parallel(int data)
    {
        BitBoard[,] pawnAttacks = new BitBoard[2, data];

        Parallel.For(0, data, (square) =>
        {
            pawnAttacks[0, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false);
            pawnAttacks[1, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: true);
        });
    }

    /// <summary>
    /// ~2x slower than <see cref="InitializePawnAttacks_Parallel"/>
    /// </summary>
    /// <param name="data"></param>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void InitializePawnAttacks_Parallel_2(int data)
    {
        BitBoard[,] pawnAttacks = new BitBoard[2, data];

        Parallel.For(0, 2, (n) =>
            Parallel.For(0, data, (square) =>
                pawnAttacks[n, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false)));
    }

    /// <summary>
    /// Completely useless (> 50 times slower than anything else)
    /// </summary>
    /// <param name="data"></param>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void InitializePawnAttacks_Parallel_3(int data)
    {
        BitBoard[,] pawnAttacks = new BitBoard[2, data];

        Parallel.For(0, data, (square) =>
            Parallel.For(0, 2, (n) =>
                pawnAttacks[n, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false)));
    }
}
