/*
 *  |          Method | itemsCount |            Mean |         Error |        StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
 *  |---------------- |----------- |----------------:|--------------:|--------------:|------:|--------:|---------:|---------:|---------:|-----------:|
 *  | EnqueueOneByOne |         10 |        147.2 ns |       0.65 ns |       0.58 ns |  1.00 |    0.00 |   0.0725 |        - |        - |      152 B |
 *  |    EnqueueRange |         10 |        228.2 ns |       2.42 ns |       1.89 ns |  1.55 |    0.01 |   0.1376 |        - |        - |      288 B |
 *  |                 |            |                 |               |               |       |         |          |          |          |            |
 *  | EnqueueOneByOne |        100 |      1,355.6 ns |       5.01 ns |       4.18 ns |  1.00 |    0.00 |   0.4158 |        - |        - |      872 B |
 *  |    EnqueueRange |        100 |      1,655.2 ns |       6.09 ns |       4.76 ns |  1.22 |    0.01 |   0.6523 |        - |        - |     1368 B |
 *  |                 |            |                 |               |               |       |         |          |          |          |            |
 *  | EnqueueOneByOne |       1000 |     13,730.4 ns |      94.51 ns |      78.92 ns |  1.00 |    0.00 |   3.8452 |        - |        - |     8072 B |
 *  |    EnqueueRange |       1000 |     15,925.2 ns |      61.39 ns |      51.26 ns |  1.16 |    0.01 |   5.7983 |        - |        - |    12168 B |
 *  |                 |            |                 |               |               |       |         |          |          |          |            |
 *  | EnqueueOneByOne |    1000000 | 15,554,643.8 ns | 229,715.58 ns | 203,636.88 ns |  1.00 |    0.00 | 468.7500 | 468.7500 | 468.7500 |  8000064 B |
 *  |    EnqueueRange |    1000000 | 19,020,092.0 ns | 378,978.47 ns | 354,496.70 ns |  1.22 |    0.03 | 625.0000 | 625.0000 | 625.0000 | 12001959 B |
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class PriorityQueue_EnqueueRange_Benchmark : BaseBenchmark
{
    private const int Priority = 1_1111_111;

    public static IEnumerable<int> Data => new[] { 10, 100, 1_000, 1_000_000 };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public void EnqueueOneByOne(int itemsCount)
    {
        var queue = new PriorityQueue<int, int>(itemsCount);

        for (int i = 0; i < itemsCount; ++i)
        {
            queue.Enqueue(i, Priority);
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void EnqueueRange(int itemsCount)
    {
        var queue = new PriorityQueue<int, int>(itemsCount);
        var items = new List<int>(itemsCount);

        for (int i = 0; i < itemsCount; ++i)
        {
            items.Add(i);
        }

        queue.EnqueueRange(items, Priority);
    }
}
