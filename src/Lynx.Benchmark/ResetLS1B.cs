using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Lynx.Benchmark
{
    public static class ResetLS1BImplementations
    {
        public static int GetLS1BIndex(ulong bitboard)
        {
            if (bitboard == default)
            {
                return -1;
            }

            return CountBits(bitboard ^ (bitboard - 1)) - 1;
        }

        public static int CountBits(ulong bitboard)
        {
            int counter = 0;

            // Consecutively reset LSB
            while (bitboard != default)
            {
                ++counter;
                bitboard = ResetLS1B(bitboard);
            }

            return counter;
        }

        public static ulong ResetLS1B(ulong bitboard)
        {
            return bitboard & (bitboard - 1);
        }

        public static ulong PopBit(ulong bitboard, int square)
        {
            return bitboard & ~(1UL << square);
        }
    }

    public class ResetLS1B : BaseBenchmark
    {
        public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

        /// <summary>
        /// Same perf.
        /// </summary>
        /// <param name="iterations"></param>
        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public void GetAndReset(int iterations)
        {
            ulong bitboard = 6060551578861568;
            ulong bitboard2 = 335588096;

            for (int i = 0; i < iterations; ++i)
            {
                while (bitboard != default)
                {
                    var ls1b = ResetLS1BImplementations.GetLS1BIndex(bitboard);
                    bitboard = ResetLS1BImplementations.ResetLS1B(bitboard);
                }

                while (bitboard2 != default)
                {
                    var ls1b = ResetLS1BImplementations.GetLS1BIndex(bitboard2);
                    bitboard2 = ResetLS1BImplementations.ResetLS1B(bitboard2);
                }
            }
        }

        /// <summary>
        /// Same perf.
        /// </summary>
        /// <param name="iterations"></param>
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void GetAndPop(int iterations)
        {
            ulong bitboard = 6060551578861568;
            ulong bitboard2 = 335588096;

            for (int i = 0; i < iterations; ++i)
            {
                while (bitboard != default)
                {
                    var ls1b = ResetLS1BImplementations.GetLS1BIndex(bitboard);
                    bitboard = ResetLS1BImplementations.PopBit(bitboard, ls1b);
                }

                while (bitboard2 != default)
                {
                    var ls1b = ResetLS1BImplementations.GetLS1BIndex(bitboard2);
                    bitboard2 = ResetLS1BImplementations.PopBit(bitboard2, ls1b);
                }
            }
        }
    }
}
