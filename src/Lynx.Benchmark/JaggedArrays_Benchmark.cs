/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                     | Size  | Mean               | Error           | StdDev          | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------------------- |------ |-------------------:|----------------:|----------------:|------:|--------:|----------:|------------:|
 *  | MultiDimensional           | 1     |          1.1818 ns |       0.0189 ns |       0.0176 ns |  1.00 |    0.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 1     |          0.8326 ns |       0.0089 ns |       0.0079 ns |  0.70 |    0.01 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 1     |          0.6801 ns |       0.0135 ns |       0.0127 ns |  0.58 |    0.02 |         - |          NA |
 *  |                            |       |                    |                 |                 |       |         |           |             |
 *  | MultiDimensional           | 10    |         94.8291 ns |       0.4989 ns |       0.4423 ns |  1.00 |    0.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 10    |         69.7207 ns |       0.4322 ns |       0.3832 ns |  0.74 |    0.01 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 10    |         63.8542 ns |       1.2767 ns |       1.5198 ns |  0.68 |    0.02 |         - |          NA |
 *  |                            |       |                    |                 |                 |       |         |           |             |
 *  | MultiDimensional           | 100   |     10,143.8262 ns |      71.5594 ns |      66.9368 ns |  1.00 |    0.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 100   |      6,971.9057 ns |       5.5335 ns |       4.9053 ns |  0.69 |    0.00 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 100   |      4,927.4731 ns |      29.4516 ns |      27.5490 ns |  0.49 |    0.00 |         - |          NA |
 *  |                            |       |                    |                 |                 |       |         |           |             |
 *  | MultiDimensional           | 1000  |    937,708.5576 ns |   4,858.6690 ns |   4,544.8020 ns |  1.00 |    0.00 |       1 B |        1.00 |
 *  | Jagged_NoTemporaryVariable | 1000  |    640,651.4827 ns |   4,535.9684 ns |   4,242.9476 ns |  0.68 |    0.01 |       1 B |        1.00 |
 *  | Jagged_TemporaryVariable   | 1000  |    423,936.5220 ns |   1,204.1832 ns |   1,005.5475 ns |  0.45 |    0.00 |         - |        0.00 |
 *  |                            |       |                    |                 |                 |       |         |           |             |
 *  | MultiDimensional           | 10000 | 92,738,288.3846 ns |  64,192.0888 ns |  53,603.3014 ns |  1.00 |    0.00 |     123 B |        1.00 |
 *  | Jagged_NoTemporaryVariable | 10000 | 63,704,982.0521 ns | 125,913.5244 ns |  98,304.9976 ns |  0.69 |    0.00 |      92 B |        0.75 |
 *  | Jagged_TemporaryVariable   | 10000 | 63,516,681.9333 ns | 424,828.0849 ns | 397,384.4542 ns |  0.68 |    0.00 |      61 B |        0.50 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                     | Size  | Mean               | Error           | StdDev          | Ratio | Allocated | Alloc Ratio |
 *  |--------------------------- |------ |-------------------:|----------------:|----------------:|------:|----------:|------------:|
 *  | MultiDimensional           | 1     |          1.4346 ns |       0.0058 ns |       0.0054 ns |  1.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 1     |          1.1643 ns |       0.0041 ns |       0.0037 ns |  0.81 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 1     |          0.8552 ns |       0.0077 ns |       0.0069 ns |  0.60 |         - |          NA |
 *  |                            |       |                    |                 |                 |       |           |             |
 *  | MultiDimensional           | 10    |         95.1626 ns |       0.3724 ns |       0.3109 ns |  1.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 10    |         68.5535 ns |       0.2985 ns |       0.2646 ns |  0.72 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 10    |         47.1674 ns |       0.3009 ns |       0.2815 ns |  0.50 |         - |          NA |
 *  |                            |       |                    |                 |                 |       |           |             |
 *  | MultiDimensional           | 100   |     10,082.2974 ns |      11.2433 ns |       8.7780 ns |  1.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 100   |      6,912.2300 ns |      13.7895 ns |      12.2240 ns |  0.69 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 100   |      6,802.6880 ns |      23.8914 ns |      19.9504 ns |  0.67 |         - |          NA |
 *  |                            |       |                    |                 |                 |       |           |             |
 *  | MultiDimensional           | 1000  |    933,573.3699 ns |     988.3749 ns |     825.3378 ns |  1.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 1000  |    630,488.0022 ns |   1,275.0646 ns |   1,130.3115 ns |  0.68 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 1000  |    628,718.4291 ns |   1,183.5234 ns |   1,049.1626 ns |  0.67 |         - |          NA |
 *  |                            |       |                    |                 |                 |       |           |             |
 *  | MultiDimensional           | 10000 | 93,283,711.5385 ns | 288,873.2842 ns | 241,222.2755 ns |  1.00 |      67 B |        1.00 |
 *  | Jagged_NoTemporaryVariable | 10000 | 63,569,183.6538 ns |  89,192.8262 ns |  74,480.0495 ns |  0.68 |      50 B |        0.75 |
 *  | Jagged_TemporaryVariable   | 10000 | 43,147,311.6667 ns |  74,994.2108 ns |  70,149.6313 ns |  0.46 |      33 B |        0.49 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                     | Size  | Mean               | Error             | StdDev            | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------------------------- |------ |-------------------:|------------------:|------------------:|------:|--------:|----------:|------------:|
 *  | MultiDimensional           | 1     |           1.014 ns |         0.0227 ns |         0.0213 ns |  1.00 |    0.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 1     |           1.585 ns |         0.0305 ns |         0.0270 ns |  1.56 |    0.04 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 1     |           1.043 ns |         0.0266 ns |         0.0249 ns |  1.03 |    0.03 |         - |          NA |
 *  |                            |       |                    |                   |                   |       |         |           |             |
 *  | MultiDimensional           | 10    |         121.689 ns |         1.5593 ns |         1.4586 ns |  1.00 |    0.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 10    |          96.848 ns |         0.9103 ns |         0.8514 ns |  0.80 |    0.01 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 10    |          48.190 ns |         0.4028 ns |         0.3768 ns |  0.40 |    0.00 |         - |          NA |
 *  |                            |       |                    |                   |                   |       |         |           |             |
 *  | MultiDimensional           | 100   |      12,202.225 ns |       114.8129 ns |       107.3960 ns |  1.00 |    0.00 |         - |          NA |
 *  | Jagged_NoTemporaryVariable | 100   |       9,903.200 ns |       100.5410 ns |        89.1269 ns |  0.81 |    0.01 |         - |          NA |
 *  | Jagged_TemporaryVariable   | 100   |       5,113.809 ns |        57.8120 ns |        54.0774 ns |  0.42 |    0.01 |         - |          NA |
 *  |                            |       |                    |                   |                   |       |         |           |             |
 *  | MultiDimensional           | 1000  |   1,169,149.169 ns |    15,226.2827 ns |    14,242.6743 ns |  1.00 |    0.00 |       1 B |        1.00 |
 *  | Jagged_NoTemporaryVariable | 1000  |     932,439.466 ns |     9,011.6555 ns |     7,988.5978 ns |  0.80 |    0.01 |       1 B |        1.00 |
 *  | Jagged_TemporaryVariable   | 1000  |     361,408.242 ns |     4,059.9730 ns |     3,797.7013 ns |  0.31 |    0.01 |         - |        0.00 |
 *  |                            |       |                    |                   |                   |       |         |           |             |
 *  | MultiDimensional           | 10000 | 125,272,657.464 ns | 1,140,098.6791 ns | 1,010,667.7830 ns |  1.00 |    0.00 |     184 B |        1.00 |
 *  | Jagged_NoTemporaryVariable | 10000 | 101,482,805.476 ns | 1,315,083.9728 ns | 1,165,787.6880 ns |  0.81 |    0.01 |      82 B |        0.45 |
 *  | Jagged_TemporaryVariable   | 10000 |  55,565,388.494 ns |   819,545.1880 ns |   766,603.0773 ns |  0.44 |    0.01 |      61 B |        0.33 |
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;
public class JaggedArrays_Benchmark : BaseBenchmark
{
    [Params(1, 10, 100, 1_000, 10_000)]
    public int Size { get; set; }

    private int[,] _multiDimensionalArray = null!;
    private int[][] _jaggedArray = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _multiDimensionalArray = new int[Size, Size];

        _jaggedArray = new int[Size][];
        for (int i = 0; i < Size; i++)
        {
            _jaggedArray[i] = new int[Size];
        }
    }

    [Benchmark(Baseline = true)]
    public long MultiDimensional()
    {
        long result = 0;

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                result += _multiDimensionalArray[i, j];
            }
        }

        return result;
    }

    [Benchmark]
    public long Jagged_NoTemporaryVariable()
    {
        long result = 0;

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                result += _jaggedArray[i][j];
            }
        }

        return result;
    }

    [Benchmark]
    public long Jagged_TemporaryVariable()
    {
        long result = 0;

        for (int i = 0; i < Size; i++)
        {
            var temp = _jaggedArray[i];

            for (int j = 0; j < Size; j++)
            {
                result += temp[j];
            }
        }

        return result;
    }
}
