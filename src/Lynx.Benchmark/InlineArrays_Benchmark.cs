/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method             | Size  | Mean            | Error        | StdDev       | Ratio | Gen0     | Allocated  | Alloc Ratio  |
 *  |------------------- |------ |----------------:|-------------:|-------------:|------:|---------:|-----------:|-------------:|
 *  | Span               | 1     |        883.1 ns |      4.33 ns |      3.84 ns |  1.00 |        - |          - |           NA |
 *  | Array              | 1     |      1,082.4 ns |      3.12 ns |      2.61 ns |  1.23 |   0.0134 |     1184 B |           NA |
 *  | InlineArray        | 1     |        919.1 ns |      1.36 ns |      1.14 ns |  1.04 |        - |          - |           NA |
 *  | InlineArray_Reused | 1     |        875.4 ns |      4.57 ns |      4.06 ns |  0.99 |        - |          - |           NA |
 *  |                    |       |                 |              |              |       |          |            |              |
 *  | Span               | 10    |      8,789.4 ns |     22.18 ns |     17.32 ns |  1.00 |        - |          - |           NA |
 *  | Array              | 10    |     10,349.9 ns |     56.43 ns |     50.02 ns |  1.18 |   0.1373 |    11840 B |           NA |
 *  | InlineArray        | 10    |      8,938.9 ns |     54.96 ns |     51.41 ns |  1.02 |        - |          - |           NA |
 *  | InlineArray_Reused | 10    |      8,618.9 ns |     50.39 ns |     47.14 ns |  0.98 |        - |          - |           NA |
 *  |                    |       |                 |              |              |       |          |            |              |
 *  | Span               | 100   |     87,232.9 ns |    146.96 ns |    122.72 ns |  1.00 |        - |          - |           NA |
 *  | Array              | 100   |    103,752.8 ns |    485.79 ns |    454.41 ns |  1.19 |   1.3428 |   118400 B |           NA |
 *  | InlineArray        | 100   |     89,623.4 ns |    335.00 ns |    313.36 ns |  1.03 |        - |          - |           NA |
 *  | InlineArray_Reused | 100   |     85,666.4 ns |    208.26 ns |    184.62 ns |  0.98 |        - |          - |           NA |
 *  |                    |       |                 |              |              |       |          |            |              |
 *  | Span               | 1000  |    872,890.6 ns |  6,438.10 ns |  6,022.20 ns |  1.00 |        - |        1 B |         1.00 |
 *  | Array              | 1000  |  1,031,827.4 ns |  9,332.64 ns |  8,273.14 ns |  1.18 |  13.6719 |  1184002 B | 1,184,002.00 |
 *  | InlineArray        | 1000  |    909,521.5 ns |  5,940.70 ns |  5,266.28 ns |  1.04 |        - |        1 B |         1.00 |
 *  | InlineArray_Reused | 1000  |    846,181.2 ns |    744.44 ns |    659.93 ns |  0.97 |        - |        1 B |         1.00 |
 *  |                    |       |                 |              |              |       |          |            |              |
 *  | Span               | 10000 |  8,752,783.4 ns | 47,153.07 ns | 41,799.97 ns |  1.00 |        - |       17 B |         1.00 |
 *  | Array              | 10000 | 10,461,701.1 ns | 34,890.65 ns | 29,135.27 ns |  1.19 | 140.6250 | 11840017 B |   696,471.59 |
 *  | InlineArray        | 10000 |  8,942,111.9 ns | 41,962.68 ns | 35,040.74 ns |  1.02 |        - |       17 B |         1.00 |
 *  | InlineArray_Reused | 10000 |  8,533,991.9 ns | 53,289.38 ns | 49,846.92 ns |  0.98 |        - |       17 B |         1.00 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method             | Size  | Mean           | Error        | StdDev       | Ratio | Gen0     | Allocated  | Alloc Ratio  |
 *  |------------------- |------ |---------------:|-------------:|-------------:|------:|---------:|-----------:|-------------:|
 *  | Span               | 1     |       860.8 ns |      2.32 ns |      1.81 ns |  1.00 |        - |          - |           NA |
 *  | Array              | 1     |       899.8 ns |      3.94 ns |      3.50 ns |  1.05 |   0.0706 |     1184 B |           NA |
 *  | InlineArray        | 1     |       853.9 ns |      1.29 ns |      1.14 ns |  0.99 |        - |          - |           NA |
 *  | InlineArray_Reused | 1     |       794.5 ns |      1.14 ns |      1.01 ns |  0.92 |        - |          - |           NA |
 *  |                    |       |                |              |              |       |          |            |              |
 *  | Span               | 10    |     8,818.1 ns |     15.26 ns |     13.53 ns |  1.00 |        - |          - |           NA |
 *  | Array              | 10    |     9,062.9 ns |     33.85 ns |     28.27 ns |  1.03 |   0.7019 |    11840 B |           NA |
 *  | InlineArray        | 10    |     8,182.4 ns |      8.03 ns |      7.12 ns |  0.93 |        - |          - |           NA |
 *  | InlineArray_Reused | 10    |     8,052.2 ns |     24.72 ns |     23.12 ns |  0.91 |        - |          - |           NA |
 *  |                    |       |                |              |              |       |          |            |              |
 *  | Span               | 100   |    88,095.0 ns |    231.17 ns |    204.92 ns |  1.00 |        - |          - |           NA |
 *  | Array              | 100   |    89,988.3 ns |    403.75 ns |    377.67 ns |  1.02 |   6.9580 |   118400 B |           NA |
 *  | InlineArray        | 100   |    81,689.1 ns |    138.49 ns |    129.54 ns |  0.93 |        - |          - |           NA |
 *  | InlineArray_Reused | 100   |    78,813.6 ns |    230.98 ns |    192.88 ns |  0.89 |        - |          - |           NA |
 *  |                    |       |                |              |              |       |          |            |              |
 *  | Span               | 1000  |   849,082.5 ns |  1,195.96 ns |  1,060.19 ns |  1.00 |        - |        1 B |         1.00 |
 *  | Array              | 1000  |   907,569.7 ns |  4,929.51 ns |  4,611.07 ns |  1.07 |  70.3125 |  1184001 B | 1,184,001.00 |
 *  | InlineArray        | 1000  |   811,824.9 ns |    811.69 ns |    677.80 ns |  0.96 |        - |        1 B |         1.00 |
 *  | InlineArray_Reused | 1000  |   777,328.7 ns |    929.95 ns |    824.38 ns |  0.92 |        - |        1 B |         1.00 |
 *  |                    |       |                |              |              |       |          |            |              |
 *  | Span               | 10000 | 8,701,565.3 ns | 20,803.81 ns | 17,372.12 ns |  1.00 |        - |       12 B |         1.00 |
 *  | Array              | 10000 | 9,091,302.7 ns | 42,180.68 ns | 37,392.07 ns |  1.04 | 703.1250 | 11840012 B |   986,667.67 |
 *  | InlineArray        | 10000 | 8,145,242.6 ns | 22,198.85 ns | 19,678.70 ns |  0.94 |        - |       12 B |         1.00 |
 *  | InlineArray_Reused | 10000 | 7,793,048.5 ns | 14,878.03 ns | 13,916.92 ns |  0.90 |        - |        6 B |         0.50 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method             | Size  | Mean          | Error       | StdDev      | Ratio | RatioSD | Gen0      | Allocated  | Alloc Ratio |
 *  |------------------- |------ |--------------:|------------:|------------:|------:|--------:|----------:|-----------:|------------:|
 *  | Span               | 1     |      2.011 us |   0.1176 us |   0.3317 us |  1.00 |    0.00 |         - |          - |          NA |
 *  | Array              | 1     |      2.318 us |   0.1594 us |   0.4495 us |  1.18 |    0.28 |    0.1869 |     1184 B |          NA |
 *  | InlineArray        | 1     |      2.070 us |   0.1032 us |   0.3010 us |  1.06 |    0.24 |         - |          - |          NA |
 *  | InlineArray_Reused | 1     |      1.850 us |   0.0615 us |   0.1774 us |  0.95 |    0.19 |         - |          - |          NA |
 *  |                    |       |               |             |             |       |         |           |            |             |
 *  | Span               | 10    |     19.822 us |   0.4571 us |   1.3116 us |  1.00 |    0.00 |         - |          - |          NA |
 *  | Array              | 10    |     16.880 us |   0.7633 us |   2.2022 us |  0.85 |    0.13 |    1.8616 |    11840 B |          NA |
 *  | InlineArray        | 10    |     14.244 us |   0.2703 us |   0.2396 us |  0.72 |    0.04 |         - |          - |          NA |
 *  | InlineArray_Reused | 10    |     11.675 us |   0.2289 us |   0.3355 us |  0.59 |    0.04 |         - |          - |          NA |
 *  |                    |       |               |             |             |       |         |           |            |             |
 *  | Span               | 100   |    137.491 us |   1.2058 us |   1.3403 us |  1.00 |    0.00 |         - |          - |          NA |
 *  | Array              | 100   |    151.970 us |   2.8787 us |   6.8972 us |  1.09 |    0.04 |   18.7988 |   118400 B |          NA |
 *  | InlineArray        | 100   |    133.268 us |   1.5003 us |   1.2528 us |  0.97 |    0.02 |         - |          - |          NA |
 *  | InlineArray_Reused | 100   |    117.669 us |   2.1540 us |   2.0148 us |  0.85 |    0.01 |         - |          - |          NA |
 *  |                    |       |               |             |             |       |         |           |            |             |
 *  | Span               | 1000  |  1,373.589 us |  25.5374 us |  22.6382 us |  1.00 |    0.00 |         - |        2 B |        1.00 |
 *  | Array              | 1000  |  1,406.298 us |   6.4660 us |   6.0483 us |  1.02 |    0.02 |  187.5000 |  1184004 B |  592,002.00 |
 *  | InlineArray        | 1000  |  1,301.676 us |  19.3396 us |  18.0903 us |  0.95 |    0.02 |         - |        2 B |        1.00 |
 *  | InlineArray_Reused | 1000  |  1,183.944 us |   9.0701 us |   8.4842 us |  0.86 |    0.02 |         - |        2 B |        1.00 |
 *  |                    |       |               |             |             |       |         |           |            |             |
 *  | Span               | 10000 | 13,362.301 us |  95.9558 us |  80.1274 us |  1.00 |    0.00 |         - |       17 B |        1.00 |
 *  | Array              | 10000 | 14,564.486 us | 273.5531 us | 325.6455 us |  1.09 |    0.02 | 1875.0000 | 11840017 B |  696,471.59 |
 *  | InlineArray        | 10000 | 13,082.905 us |  82.3161 us |  64.2670 us |  0.98 |    0.01 |         - |       17 B |        1.00 |
 *  | InlineArray_Reused | 10000 | 11,576.168 us | 169.3217 us | 150.0993 us |  0.87 |    0.01 |         - |       17 B |        1.00 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class InlineArrays_Benchmark : BaseBenchmark
{
    [Params(1, 10, 100, 1_000, 10_000)]
    public int Size { get; set; }

    private static readonly Move[] _arrayMovePool = new Move[Constants.MaxNumberOfPossibleMovesInAPosition];

    private static readonly Position[] Positions =
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
        new Position(Constants.TTPositionFEN)
    ];

    [Benchmark(Baseline = true)]
    public long Span()
    {
        long result = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in Positions)
            {
                result += Span(position);
            }
        }

        return result;

        static int Span(Position position)
        {
            Span<Move> moveSpan = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            var allMoves = MoveGenerator.GenerateAllMoves(position, moveSpan);

            return allMoves.Length;
        }
    }

    [Benchmark]
    public long Array()
    {
        long result = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in Positions)
            {
                var allMoves = MoveGenerator.GenerateAllMoves(position, _arrayMovePool);

                result += allMoves.Length;
            }
        }

        return result;
    }

    [Benchmark]
    public long InlineArray()
    {
        long result = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in Positions)
            {
                var moveArray = new MoveArray();
                var allMoves = MoveGenerator.GenerateAllMoves(position, ref moveArray);

                result += allMoves.Length;
            }
        }

        return result;
    }

    [Benchmark]
    public long InlineArray_Reused()
    {
        long result = 0;

        var moveArray = new MoveArray();
        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in Positions)
            {
                var allMoves = MoveGenerator.GenerateAllMoves(position, ref moveArray);
                result += allMoves.Length;
            }
        }

        return result;
    }
}
