using BenchmarkDotNet.Attributes;
using SharpFish.Model;
using System.Collections.Generic;

namespace SharpFish.Benchmark
{
    public static class PieceOffsetImplementations
    {
        public static int Method(Side side) => 6 - (6 * (int)side);

        public static readonly int[] Array = new[] { 6, 0 };
    }

    public class PieceOffset : BaseBenchmark
    {
        public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public void Array(int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                _ = PieceOffsetImplementations.Array[(int)Side.Black];
                _ = PieceOffsetImplementations.Array[(int)Side.White];
            }
        }

        /// <summary>
        /// Faster
        /// </summary>
        /// <param name="iterations"></param>
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void Method(int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                _ = PieceOffsetImplementations.Method(Side.Black);
                _ = PieceOffsetImplementations.Method(Side.White);
            }
        }
    }
}
