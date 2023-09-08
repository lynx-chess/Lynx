/*
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class CheckmateDetectionLimits : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public bool LessAndGreater(int iterations)
    {
        bool result = false;

        for (int i = 0; i < iterations; ++i)
        {
            result ^= (iterations < EvaluationConstants.PositiveCheckmateDetectionLimit && iterations > -EvaluationConstants.PositiveCheckmateDetectionLimit);
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public bool Abs(int iterations)
    {
        bool result = false;

        for (int i = 0; i < iterations; ++i)
        {
            result ^= (Math.Abs(iterations) < EvaluationConstants.PositiveCheckmateDetectionLimit);
        }

        return result;
    }
}
