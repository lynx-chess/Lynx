/*
 *
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class ReadonlyStruct_vs_Tuple : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int GetReadonlyStruct(int data)
    {
        int sum = 0;
        for (int i = 0; i < data; ++i)
        {
            sum += ReadonlyStructImpl().Evaluation;
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int GetStruct(int data)
    {
        int sum = 0;
        for (int i = 0; i < data; ++i)
        {
            sum += StructImpl().Evaluation;
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int GetClass(int data)
    {
        int sum = 0;
        for (int i = 0; i < data; ++i)
        {
            sum += ClassImpl().Evaluation;
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int GetTuple(int data)
    {
        int sum = 0;
        for (int i = 0; i < data; ++i)
        {
            sum += TupleImpl().Evaluation;
        }

        return sum;
    }

    private static ReadonlyStruct ReadonlyStructImpl() => new ReadonlyStruct(123, 20);
    private static Struct StructImpl() => new Struct(123, 20);
    private static Class ClassImpl() => new Class(123, 20);
    private static (int Evaluation, int Phase) TupleImpl() => (123, 20);

    private readonly struct ReadonlyStruct
    {
        public int Evaluation { get; }

        public int Phase { get; }

        public ReadonlyStruct(int evaluation, int phase)
        {
            Evaluation = evaluation;
            Phase = phase;
        }
    }

    private struct Struct
    {
        public int Evaluation { get; }

        public int Phase { get; }

        public Struct(int evaluation, int phase)
        {
            Evaluation = evaluation;
            Phase = phase;
        }
    }

    private class Class
    {
        public int Evaluation { get; }

        public int Phase { get; }

        public Class(int evaluation, int phase)
        {
            Evaluation = evaluation;
            Phase = phase;
        }
    }
}
