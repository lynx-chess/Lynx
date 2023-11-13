/*
 *
 *  BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.2.23502.2
 *    [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 *  | Method            | data   | Mean            | Error         | StdDev        | Ratio   | RatioSD | Gen0     | Allocated | Alloc Ratio |
 *  |------------------ |------- |----------------:|--------------:|--------------:|--------:|--------:|---------:|----------:|------------:|
 *  | GetReadonlyStruct | 1      |       0.0433 ns |     0.0020 ns |     0.0017 ns |   1.000 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 1      |       0.0411 ns |     0.0003 ns |     0.0002 ns |   0.949 |    0.04 |        - |         - |          NA |
 *  | GetClass          | 1      |       7.0160 ns |     0.0913 ns |     0.0854 ns | 162.212 |    6.10 |   0.0013 |      24 B |          NA |
 *  | GetTuple          | 1      |       0.0000 ns |     0.0000 ns |     0.0000 ns |   0.000 |    0.00 |        - |         - |          NA |
 *  |                   |        |                 |               |               |         |         |          |           |             |
 *  | GetReadonlyStruct | 10     |       4.1764 ns |     0.0101 ns |     0.0089 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 10     |       4.1206 ns |     0.0093 ns |     0.0082 ns |    0.99 |    0.00 |        - |         - |          NA |
 *  | GetClass          | 10     |      68.1948 ns |     0.9756 ns |     0.9126 ns |   16.35 |    0.23 |   0.0128 |     240 B |          NA |
 *  | GetTuple          | 10     |       4.0593 ns |     0.0122 ns |     0.0102 ns |    0.97 |    0.00 |        - |         - |          NA |
 *  |                   |        |                 |               |               |         |         |          |           |             |
 *  | GetReadonlyStruct | 1000   |     342.0937 ns |     0.3223 ns |     0.2691 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 1000   |     341.9525 ns |     0.1589 ns |     0.1486 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  | GetClass          | 1000   |   6,594.3512 ns |   126.1058 ns |   150.1200 ns |   19.48 |    0.43 |   1.2817 |   24000 B |          NA |
 *  | GetTuple          | 1000   |     341.1013 ns |     0.0792 ns |     0.0662 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  |                   |        |                 |               |               |         |         |          |           |             |
 *  | GetReadonlyStruct | 10000  |   3,361.0422 ns |     1.4774 ns |     1.3097 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 10000  |   3,360.5295 ns |     0.9026 ns |     0.7537 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  | GetClass          | 10000  |  65,309.0613 ns | 1,027.4814 ns |   910.8355 ns |   19.43 |    0.27 |  12.8174 |  240000 B |          NA |
 *  | GetTuple          | 10000  |   3,360.8279 ns |     1.4204 ns |     1.1861 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  |                   |        |                 |               |               |         |         |          |           |             |
 *  | GetReadonlyStruct | 100000 |  33,480.1101 ns |    10.3403 ns |     9.1664 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 100000 |  33,482.5471 ns |    10.1962 ns |     9.0387 ns |    1.00 |    0.00 |        - |         - |          NA |
 *  | GetClass          | 100000 | 670,398.0542 ns | 4,806.6014 ns | 4,013.7298 ns |   20.02 |    0.12 | 127.9297 | 2400001 B |          NA |
 *  | GetTuple          | 100000 |  33,479.7699 ns |     4.8486 ns |     3.7854 ns |    1.00 |    0.00 |        - |         - |          NA |
 *
 *  BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 10 (10.0.20348.2031) (Hyper-V)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.2.23502.2
 *    [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 *  | Method            | data   | Mean            | Error         | StdDev        | Median          | Ratio | RatioSD | Gen0     | Allocated | Alloc Ratio |
 *  |------------------ |------- |----------------:|--------------:|--------------:|----------------:|------:|--------:|---------:|----------:|------------:|
 *  | GetReadonlyStruct | 1      |       0.0065 ns |     0.0065 ns |     0.0060 ns |       0.0065 ns |     ? |       ? |        - |         - |           ? |
 *  | GetStruct         | 1      |       0.0105 ns |     0.0054 ns |     0.0045 ns |       0.0107 ns |     ? |       ? |        - |         - |           ? |
 *  | GetClass          | 1      |       5.6833 ns |     0.0900 ns |     0.0841 ns |       5.7046 ns |     ? |       ? |   0.0013 |      24 B |           ? |
 *  | GetTuple          | 1      |       0.0000 ns |     0.0000 ns |     0.0000 ns |       0.0000 ns |     ? |       ? |        - |         - |           ? |
 *  |                   |        |                 |               |               |                 |       |         |          |           |             |
 *  | GetReadonlyStruct | 10     |       3.4586 ns |     0.0111 ns |     0.0103 ns |       3.4556 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 10     |       3.8685 ns |     0.1197 ns |     0.2702 ns |       3.7757 ns |  1.16 |    0.07 |        - |         - |          NA |
 *  | GetClass          | 10     |      53.5078 ns |     0.3294 ns |     0.2920 ns |      53.5022 ns | 15.47 |    0.08 |   0.0128 |     240 B |          NA |
 *  | GetTuple          | 10     |       3.5800 ns |     0.0196 ns |     0.0184 ns |       3.5786 ns |  1.04 |    0.00 |        - |         - |          NA |
 *  |                   |        |                 |               |               |                 |       |         |          |           |             |
 *  | GetReadonlyStruct | 1000   |     344.6542 ns |     0.7263 ns |     0.6794 ns |     344.8699 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 1000   |     344.7361 ns |     1.1600 ns |     1.0850 ns |     344.5212 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetClass          | 1000   |   5,239.0531 ns |    51.2842 ns |    42.8246 ns |   5,237.4138 ns | 15.20 |    0.13 |   1.2817 |   24000 B |          NA |
 *  | GetTuple          | 1000   |     344.2790 ns |     0.5455 ns |     0.4836 ns |     344.2186 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  |                   |        |                 |               |               |                 |       |         |          |           |             |
 *  | GetReadonlyStruct | 10000  |   3,377.8829 ns |     6.5812 ns |     6.1561 ns |   3,377.9812 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 10000  |   3,379.4917 ns |    12.6944 ns |    11.2532 ns |   3,374.7431 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetClass          | 10000  |  51,723.2394 ns |   295.1227 ns |   246.4408 ns |  51,721.0327 ns | 15.31 |    0.08 |  12.8174 |  240000 B |          NA |
 *  | GetTuple          | 10000  |   3,381.6790 ns |     7.5980 ns |     7.1072 ns |   3,384.0626 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  |                   |        |                 |               |               |                 |       |         |          |           |             |
 *  | GetReadonlyStruct | 100000 |  33,644.7469 ns |    64.1477 ns |    60.0038 ns |  33,635.8459 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 100000 |  33,679.7611 ns |   129.4815 ns |   121.1171 ns |  33,674.6948 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetClass          | 100000 | 523,249.7907 ns | 7,357.1004 ns | 6,521.8779 ns | 523,575.9277 ns | 15.56 |    0.19 | 127.9297 | 2400000 B |          NA |
 *  | GetTuple          | 100000 |  33,692.9247 ns |    98.8249 ns |    87.6057 ns |  33,691.0919 ns |  1.00 |    0.00 |        - |         - |          NA |
 *
 *  BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, macOS Monterey 12.6.9 (21G726) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100-rc.2.23502.2
 *    [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
 *
 *
 *  | Method            | data   | Mean              | Error          | StdDev         | Median            | Ratio | RatioSD | Gen0     | Allocated | Alloc Ratio |
 *  |------------------ |------- |------------------:|---------------:|---------------:|------------------:|------:|--------:|---------:|----------:|------------:|
 *  | GetReadonlyStruct | 1      |         0.0078 ns |      0.0138 ns |      0.0129 ns |         0.0000 ns |     ? |       ? |        - |         - |           ? |
 *  | GetStruct         | 1      |         0.0009 ns |      0.0028 ns |      0.0023 ns |         0.0000 ns |     ? |       ? |        - |         - |           ? |
 *  | GetClass          | 1      |        10.5401 ns |      0.1562 ns |      0.1304 ns |        10.5264 ns |     ? |       ? |   0.0038 |      24 B |           ? |
 *  | GetTuple          | 1      |         0.0341 ns |      0.0192 ns |      0.0179 ns |         0.0324 ns |     ? |       ? |        - |         - |           ? |
 *  |                   |        |                   |                |                |                   |       |         |          |           |             |
 *  | GetReadonlyStruct | 10     |         2.7998 ns |      0.0371 ns |      0.0329 ns |         2.7961 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 10     |         2.7880 ns |      0.0420 ns |      0.0392 ns |         2.7797 ns |  1.00 |    0.02 |        - |         - |          NA |
 *  | GetClass          | 10     |        95.1083 ns |      1.8462 ns |      1.8132 ns |        95.0949 ns | 33.89 |    0.69 |   0.0381 |     240 B |          NA |
 *  | GetTuple          | 10     |         2.8511 ns |      0.0401 ns |      0.0375 ns |         2.8514 ns |  1.02 |    0.02 |        - |         - |          NA |
 *  |                   |        |                   |                |                |                   |       |         |          |           |             |
 *  | GetReadonlyStruct | 1000   |       234.7936 ns |      3.2107 ns |      3.0033 ns |       233.2950 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 1000   |       235.4478 ns |      2.6695 ns |      2.4971 ns |       234.5212 ns |  1.00 |    0.02 |        - |         - |          NA |
 *  | GetClass          | 1000   |     9,606.5149 ns |    179.1135 ns |    167.5429 ns |     9,585.4876 ns | 40.92 |    0.88 |   3.8147 |   24000 B |          NA |
 *  | GetTuple          | 1000   |       236.4167 ns |      3.4962 ns |      3.0993 ns |       235.8719 ns |  1.01 |    0.02 |        - |         - |          NA |
 *  |                   |        |                   |                |                |                   |       |         |          |           |             |
 *  | GetReadonlyStruct | 10000  |     2,310.6428 ns |     27.1230 ns |     25.3708 ns |     2,299.6612 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 10000  |     2,320.7464 ns |     23.0914 ns |     21.5997 ns |     2,323.3818 ns |  1.00 |    0.01 |        - |         - |          NA |
 *  | GetClass          | 10000  |   106,528.3055 ns |  2,195.7536 ns |  6,228.9875 ns |   105,840.7091 ns | 45.10 |    1.53 |  38.2080 |  240000 B |          NA |
 *  | GetTuple          | 10000  |     2,352.1191 ns |      4.8002 ns |      4.4901 ns |     2,350.7293 ns |  1.02 |    0.01 |        - |         - |          NA |
 *  |                   |        |                   |                |                |                   |       |         |          |           |             |
 *  | GetReadonlyStruct | 100000 |    23,604.4814 ns |    244.7303 ns |    216.9470 ns |    23,549.3223 ns |  1.00 |    0.00 |        - |         - |          NA |
 *  | GetStruct         | 100000 |    24,493.6783 ns |    436.6484 ns |    485.3335 ns |    24,537.2996 ns |  1.04 |    0.02 |        - |         - |          NA |
 *  | GetClass          | 100000 | 1,139,894.3549 ns | 22,402.8241 ns | 20,955.6155 ns | 1,146,713.2461 ns | 48.27 |    1.19 | 380.8594 | 2400001 B |          NA |
 *  | GetTuple          | 100000 |    25,205.2431 ns |    850.2591 ns |  2,398.1708 ns |    24,261.8659 ns |  1.13 |    0.11 |        - |         - |          NA |
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