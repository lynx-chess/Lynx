/*
 *
 *  |             Method |   data |            Mean |         Error |        StdDev |          Median |  Ratio | RatioSD | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |------------------- |------- |----------------:|--------------:|--------------:|----------------:|-------:|--------:|------:|------:|------:|----------:|
 *  |           Dividing |      1 |       0.0490 ns |     0.0396 ns |     0.0351 ns |       0.0426 ns |   1.00 |    0.00 |     - |     - |     - |         - |
 *  |        Multiplying |      1 |       5.0197 ns |     0.2762 ns |     0.8143 ns |       4.4857 ns | 287.23 |  544.39 |     - |     - |     - |         - |
 *  | RightShiftOperator |      1 |       0.0467 ns |     0.0200 ns |     0.0187 ns |       0.0417 ns |   2.62 |    3.97 |     - |     - |     - |         - |
 *  |                    |        |                 |               |               |                 |        |         |       |       |       |           |
 *  |           Dividing |     10 |       6.6264 ns |     0.1053 ns |     0.0985 ns |       6.6342 ns |   1.00 |    0.00 |     - |     - |     - |         - |
 *  |        Multiplying |     10 |      41.5049 ns |     0.4451 ns |     0.3475 ns |      41.3493 ns |   6.27 |    0.10 |     - |     - |     - |         - |
 *  | RightShiftOperator |     10 |       6.0727 ns |     0.0586 ns |     0.0520 ns |       6.0675 ns |   0.92 |    0.01 |     - |     - |     - |         - |
 *  |                    |        |                 |               |               |                 |        |         |       |       |       |           |
 *  |           Dividing |   1000 |     664.4654 ns |     3.9397 ns |     3.4925 ns |     664.1221 ns |   1.00 |    0.00 |     - |     - |     - |         - |
 *  |        Multiplying |   1000 |   4,075.5717 ns |    53.9482 ns |    47.8237 ns |   4,072.6608 ns |   6.13 |    0.07 |     - |     - |     - |         - |
 *  | RightShiftOperator |   1000 |     501.3558 ns |     4.0566 ns |     3.7946 ns |     501.1838 ns |   0.75 |    0.01 |     - |     - |     - |         - |
 *  |                    |        |                 |               |               |                 |        |         |       |       |       |           |
 *  |           Dividing |  10000 |   6,526.4149 ns |    42.4275 ns |    37.6109 ns |   6,513.2679 ns |   1.00 |    0.00 |     - |     - |     - |         - |
 *  |        Multiplying |  10000 |  40,794.3372 ns |   583.7972 ns |   517.5210 ns |  40,730.7404 ns |   6.25 |    0.09 |     - |     - |     - |         - |
 *  | RightShiftOperator |  10000 |   4,951.7039 ns |    38.9041 ns |    32.4867 ns |   4,958.1413 ns |   0.76 |    0.01 |     - |     - |     - |         - |
 *  |                    |        |                 |               |               |                 |        |         |       |       |       |           |
 *  |           Dividing | 100000 |  65,583.7874 ns |   693.6764 ns |   648.8653 ns |  65,513.7207 ns |   1.00 |    0.00 |     - |     - |     - |         - |
 *  |        Multiplying | 100000 | 372,849.5736 ns | 2,621.9462 ns | 2,452.5701 ns | 372,794.5801 ns |   5.69 |    0.06 |     - |     - |     - |         - |
 *  | RightShiftOperator | 100000 |  48,995.2628 ns |   288.9946 ns |   256.1861 ns |  49,024.7467 ns |   0.75 |    0.01 |     - |     - |     - |         - |
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class DivideByHalf_Benchmark : BaseBenchmark
{
    public static IEnumerable<int> Data => [1, 10, 1_000, 10_000, 100_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Dividing(int data)
    {
        int sum = 0;
        for (int i = 0; i < data; ++i)
        {
            sum += i / 2;
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int Multiplying(int data)
    {
        int sum = 0;
        for (int i = 0; i < data; ++i)
        {
            sum += Convert.ToInt32(i * 0.5);
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int RightShiftOperator(int data)
    {
        int sum = 0;
        for (int i = 0; i < data; ++i)
        {
            sum += i >> 1;
        }

        return sum;
    }
}
