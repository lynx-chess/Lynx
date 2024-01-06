/*
 *
 *  |            Method | iterations |            Mean |          Error |         StdDev |          Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |------------------ |----------- |----------------:|---------------:|---------------:|----------------:|------:|------:|------:|----------:|
 *  |          IfReturn |          1 |        59.74 ns |       0.523 ns |       0.437 ns |        59.59 ns |     - |     - |     - |         - |
 *  |            IfElse |          1 |        56.38 ns |       0.435 ns |       0.363 ns |        56.23 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |          1 |        55.60 ns |       1.076 ns |       0.898 ns |        55.68 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |          1 |        61.04 ns |       0.419 ns |       0.372 ns |        61.00 ns |     - |     - |     - |         - |
 *  |          IfReturn |         10 |       527.44 ns |      10.406 ns |       9.734 ns |       523.41 ns |     - |     - |     - |         - |
 *  |            IfElse |         10 |       489.20 ns |       4.418 ns |       3.689 ns |       488.88 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |         10 |       507.73 ns |       8.027 ns |       6.267 ns |       507.66 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |         10 |       608.47 ns |       3.330 ns |       2.781 ns |       608.29 ns |     - |     - |     - |         - |
 *  |          IfReturn |       1000 |    50,581.23 ns |     731.498 ns |     648.454 ns |    50,517.42 ns |     - |     - |     - |         - |
 *  |            IfElse |       1000 |    56,827.54 ns |     384.558 ns |     300.238 ns |    56,863.34 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |       1000 |    46,857.21 ns |     922.390 ns |   1,262.578 ns |    46,150.58 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |       1000 |    59,021.93 ns |   1,161.945 ns |   1,086.884 ns |    58,640.30 ns |     - |     - |     - |         - |
 *  |          IfReturn |      10000 |   527,445.45 ns |   9,618.760 ns |   8,526.780 ns |   523,854.15 ns |     - |     - |     - |         - |
 *  |            IfElse |      10000 |   568,891.62 ns |   6,937.813 ns |   5,793.388 ns |   566,935.45 ns |     - |     - |     - |       1 B |
 *  | IfReturnBranchOpt |      10000 |   489,929.22 ns |   9,674.642 ns |   9,501.792 ns |   486,080.81 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |      10000 |   611,632.16 ns |   8,852.579 ns |   7,847.581 ns |   613,367.48 ns |     - |     - |     - |         - |
 *  |          IfReturn |     100000 | 5,208,419.40 ns |  40,817.542 ns |  31,867.652 ns | 5,212,677.34 ns |     - |     - |     - |         - |
 *  |            IfElse |     100000 | 5,760,253.75 ns | 108,602.540 ns | 101,586.883 ns | 5,701,725.78 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |     100000 | 4,883,961.18 ns |  91,472.543 ns |  76,383.716 ns | 4,860,324.22 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |     100000 | 6,160,576.17 ns | 117,913.502 ns | 197,006.973 ns | 6,098,886.33 ns |     - |     - |     - |         - |
 *
 *
 *  |            Method | iterations |            Mean |          Error |         StdDev |          Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |------------------ |----------- |----------------:|---------------:|---------------:|----------------:|------:|------:|------:|----------:|
 *  |          IfReturn |          1 |        58.20 ns |       0.405 ns |       0.379 ns |        58.16 ns |     - |     - |     - |         - |
 *  |            IfElse |          1 |        65.75 ns |       0.621 ns |       0.690 ns |        65.60 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |          1 |        55.31 ns |       0.650 ns |       0.576 ns |        55.16 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |          1 |        65.37 ns |       0.485 ns |       0.454 ns |        65.35 ns |     - |     - |     - |         - |
 *  |          IfReturn |         10 |       521.66 ns |       4.328 ns |       4.049 ns |       521.03 ns |     - |     - |     - |         - |
 *  |            IfElse |         10 |       578.08 ns |       2.710 ns |       2.402 ns |       577.46 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |         10 |       502.52 ns |       2.686 ns |       2.381 ns |       503.26 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |         10 |       603.82 ns |       4.242 ns |       3.968 ns |       603.46 ns |     - |     - |     - |         - |
 *  |          IfReturn |       1000 |    49,802.15 ns |     240.623 ns |     225.079 ns |    49,872.05 ns |     - |     - |     - |         - |
 *  |            IfElse |       1000 |    46,495.47 ns |     252.982 ns |     211.252 ns |    46,427.63 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |       1000 |    48,328.54 ns |     232.710 ns |     217.677 ns |    48,317.12 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |       1000 |    59,890.03 ns |     416.728 ns |     369.418 ns |    59,851.78 ns |     - |     - |     - |         - |
 *  |          IfReturn |      10000 |   522,730.30 ns |  13,014.809 ns |  36,280.084 ns |   504,114.31 ns |     - |     - |     - |         - |
 *  |            IfElse |      10000 |   626,532.83 ns |  45,507.425 ns | 132,025.290 ns |   586,666.99 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |      10000 |   626,863.99 ns |  45,309.606 ns | 133,596.397 ns |   602,557.47 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |      10000 |   594,692.20 ns |  18,088.197 ns |  51,018.081 ns |   577,805.22 ns |     - |     - |     - |         - |
 *  |          IfReturn |     100000 | 5,823,341.45 ns | 177,344.010 ns | 500,201.927 ns | 5,726,350.00 ns |     - |     - |     - |         - |
 *  |            IfElse |     100000 | 5,943,093.27 ns | 117,265.523 ns | 267,072.936 ns | 5,883,730.47 ns |     - |     - |     - |         - |
 *  | IfReturnBranchOpt |     100000 | 5,206,963.60 ns | 220,073.559 ns | 638,473.299 ns | 5,015,431.25 ns |     - |     - |     - |         - |
 *  |   IfElseBranchOpt |     100000 | 6,179,092.76 ns | 116,114.417 ns | 129,060.867 ns | 6,147,067.97 ns |     - |     - |     - |         - |
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

[SimpleJob]
public class Branching_Benchmark : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void IfReturn(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            IfReturnImpl(20);
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void IfElse(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            IfElseImpl(20);
        }
    }

    /// <summary>
    /// Seems the best alternative
    /// </summary>
    /// <param name="iterations"></param>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void IfReturnBranchOpt(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            IfReturnImplBranchOpt(20);
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void IfElseBranchOpt(int iterations)
    {
        for (int i = 0; i < iterations; ++i)
        {
            IfElseImplBranchOpt(20);
        }
    }

    private static long IfReturnImpl(int depth, long nodes = 0)
    {
        if (depth == 0)
        {
            ++nodes;
            return nodes;
        }

        nodes += IfReturnImpl(depth - 1, nodes);

        return nodes;
    }

    private static long IfReturnImplBranchOpt(int depth, long nodes = 0)
    {
        if (depth != 0)
        {
            return nodes + IfReturnImplBranchOpt(depth - 1, nodes);
        }

        return ++nodes;
    }

    private static long IfElseImpl(int depth, long nodes = 0)
    {
        if (depth == 0)
        {
            ++nodes;
        }
        else
        {
            nodes += IfElseImpl(depth - 1, nodes);
        }

        return nodes;
    }

    private static long IfElseImplBranchOpt(int depth, long nodes = 0)
    {
        if (depth != 0)
        {
            nodes += IfElseImplBranchOpt(depth - 1, nodes);
        }
        else
        {
            ++nodes;
        }

        return nodes;
    }
}
