/*
 *  |                     Method |    Size |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |--------------------------- |-------- |---------:|----------:|----------:|---------:|------:|--------:|------:|------:|------:|----------:|
 *  |     DequeueReusingVariable |      10 | 3.677 ns | 0.2253 ns | 0.6607 ns | 3.240 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  | DequeueRedeclaringVariable |      10 | 3.862 ns | 0.3745 ns | 1.0684 ns | 3.745 ns |  1.09 |    0.36 |     - |     - |     - |         - |
 *  |                            |         |          |           |           |          |       |         |       |       |       |           |
 *  |     DequeueReusingVariable |     100 | 3.747 ns | 0.2404 ns | 0.7088 ns | 3.302 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  | DequeueRedeclaringVariable |     100 | 3.760 ns | 0.3419 ns | 0.9587 ns | 3.634 ns |  1.03 |    0.31 |     - |     - |     - |         - |
 *  |                            |         |          |           |           |          |       |         |       |       |       |           |
 *  |     DequeueReusingVariable |    1000 | 3.830 ns | 0.2452 ns | 0.7230 ns | 3.460 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  | DequeueRedeclaringVariable |    1000 | 3.480 ns | 0.2797 ns | 0.8158 ns | 3.431 ns |  0.95 |    0.30 |     - |     - |     - |         - |
 *  |                            |         |          |           |           |          |       |         |       |       |       |           |
 *  |     DequeueReusingVariable | 1000000 | 6.738 ns | 0.3485 ns | 1.0222 ns | 6.600 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  | DequeueRedeclaringVariable | 1000000 | 6.148 ns | 0.3121 ns | 0.9154 ns | 6.107 ns |  0.93 |    0.20 |     - |     - |     - |         - |
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark
{
    public class PriorityQueue_Dequeue : BaseBenchmark
    {
        [Params(10, 100, 1_000, 1_000_000)]
        public int Size { get; set; }

        private PriorityQueue<int, int> _queue = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _queue = new(Size);
            for (int i = 0; i < Size; i++)
            {
                _queue.Enqueue(i, i);
            }
        }

        [Benchmark(Baseline = true)]
        public int DequeueReusingVariable()
        {
            int total = 0;
            while (_queue.TryDequeue(out int n, out _))
            {
                total += n;
            }
            return total;
        }

        [Benchmark]
        public int DequeueRedeclaringVariable()
        {
            int total = 0;
            while (_queue.TryDequeue(out int n, out _))
            {
                total += n;
            }
            return total;
        }
    }
}
