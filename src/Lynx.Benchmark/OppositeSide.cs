/*
 *  |              Method | iterations |           Mean |       Error |      StdDev |         Median | Ratio | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |-------------------- |----------- |---------------:|------------:|------------:|---------------:|------:|------:|------:|------:|----------:|
 *  |               Array |          1 |      2.0454 ns |   0.1527 ns |   0.4429 ns |      1.7810 ns | 1.000 |     - |     - |     - |         - |
 *  | Method_Substraction |          1 |      0.0556 ns |   0.0175 ns |   0.0155 ns |      0.0558 ns | 0.030 |     - |     - |     - |         - |
 *  |    Method_BitwiseOr |          1 |      0.0033 ns |   0.0052 ns |   0.0046 ns |      0.0011 ns | 0.002 |     - |     - |     - |         - |
 *  |                     |            |                |             |             |                |       |       |       |       |           |
 *  |               Array |         10 |      8.4530 ns |   0.0811 ns |   0.0677 ns |      8.4539 ns |  1.00 |     - |     - |     - |         - |
 *  | Method_Substraction |         10 |      3.6529 ns |   0.0700 ns |   0.0621 ns |      3.6361 ns |  0.43 |     - |     - |     - |         - |
 *  |    Method_BitwiseOr |         10 |      3.5966 ns |   0.0498 ns |   0.0416 ns |      3.6105 ns |  0.43 |     - |     - |     - |         - |
 *  |                     |            |                |             |             |                |       |       |       |       |           |
 *  |               Array |       1000 |    625.6390 ns |   6.0374 ns |   5.3520 ns |    626.5738 ns |  1.00 |     - |     - |     - |         - |
 *  | Method_Substraction |       1000 |    354.0243 ns |   1.8912 ns |   1.6765 ns |    353.9815 ns |  0.57 |     - |     - |     - |         - |
 *  |    Method_BitwiseOr |       1000 |    356.8414 ns |   5.5939 ns |   5.7445 ns |    354.6032 ns |  0.57 |     - |     - |     - |         - |
 *  |                     |            |                |             |             |                |       |       |       |       |           |
 *  |               Array |      10000 |  6,148.2383 ns |  52.6677 ns |  43.9799 ns |  6,138.4144 ns |  1.00 |     - |     - |     - |         - |
 *  | Method_Substraction |      10000 |  3,498.4239 ns |  25.5074 ns |  21.2998 ns |  3,493.5265 ns |  0.57 |     - |     - |     - |         - |
 *  |    Method_BitwiseOr |      10000 |  3,479.8918 ns |  30.6982 ns |  28.7151 ns |  3,486.3743 ns |  0.57 |     - |     - |     - |         - |
 *  |                     |            |                |             |             |                |       |       |       |       |           |
 *  |               Array |     100000 | 61,290.0057 ns | 699.5336 ns | 584.1423 ns | 61,412.5793 ns |  1.00 |     - |     - |     - |         - |
 *  | Method_Substraction |     100000 | 34,730.1760 ns | 241.3976 ns | 225.8035 ns | 34,690.5182 ns |  0.57 |     - |     - |     - |         - |
 *  |    Method_BitwiseOr |     100000 | 34,666.9640 ns | 179.2668 ns | 149.6959 ns | 34,676.4832 ns |  0.57 |     - |     - |     - |         - |
 * */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark
{
    /// <summary>
    /// <see cref="Utils.OppositeSide(Side)"/>
    /// </summary>
    public static class OppositeSiteImplementations
    {
        public static int Method_Substraction(Side side) => (int)Side.White - (int)side;

        public static int Method_BitwiseOr(Side side) => (int)side ^ 1;

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
        /// Faster than <see cref="Array(int)"/>
        /// </summary>
        /// <param name="iterations"></param>
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void Method_Substraction(int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                _ = OppositeSiteImplementations.Method_Substraction(Side.Black);
                _ = OppositeSiteImplementations.Method_Substraction(Side.White);
            }
        }

        /// <summary>
        /// Very similar to <see cref="Method_Substraction(int)"/>
        /// </summary>
        /// <param name="iterations"></param>
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void Method_BitwiseOr(int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                _ = OppositeSiteImplementations.Method_BitwiseOr(Side.Black);
                _ = OppositeSiteImplementations.Method_BitwiseOr(Side.White);
            }
        }
    }
}
