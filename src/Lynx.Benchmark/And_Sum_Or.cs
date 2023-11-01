/*
 *
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class And_Sum_Or : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public bool And(int data)
    {
        bool sum = false;
        for (int i = 0; i < data; ++i)
        {
            int a = i % 2;
            int b = i % 3;
            int c = i % 6;

            sum ^= (a == 0 && b == 0 && c == 0);
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool Sum(int data)
    {
        bool sum = false;
        for (int i = 0; i < data; ++i)
        {
            int a = i % 2;
            int b = i % 3;
            int c = i % 6;

            sum ^= (a + b + c == 0);
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool Or(int data)
    {
        bool sum = false;
        for (int i = 0; i < data; ++i)
        {
            int a = i % 2;
            int b = i % 3;
            int c = i % 6;

            sum ^= ((a | b | c) == 0);
        }

        return sum;
    }
}
