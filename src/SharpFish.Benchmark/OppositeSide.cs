using BenchmarkDotNet.Attributes;
using SharpFish.Model;
using System.Collections.Generic;

namespace SharpFish.Benchmark
{
    public static class OppositeSiteImplementations
    {
        public static int Method(Side side) => (int)Side.White - (int)side;

        public static readonly int[] Array = new[] { 1, 0 };
    }

    public class OppositeSide : BaseBenchmark
    {
        public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public void Array(int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                _ = OppositeSiteImplementations.Array[(int)Side.Black];
                _ = OppositeSiteImplementations.Array[(int)Side.White];
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
                _ = OppositeSiteImplementations.Method(Side.Black);
                _ = OppositeSiteImplementations.Method(Side.White);
            }
        }
    }
}
