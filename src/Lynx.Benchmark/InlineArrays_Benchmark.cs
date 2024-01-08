/*
 *
 *      BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *      AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *      .NET SDK 8.0.100
 *        [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *        DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *      | Method      | Size  | Mean            | Error        | StdDev       | Median          | Ratio | RatioSD | Gen0     | Allocated  | Alloc Ratio       |
 *      |------------ |------     | ----------------:|-------------:|-------------:|----------------:|------:|--------:|---------:|-----------:|-------------:|
 *      | Span        | 1     |        865.0 ns |      2.46 ns |      2.18 ns |        864.3 ns |  1.00 |    0.00 |        - |          - |           N A  |
 *      | Array       | 1     |      1,252.5 ns |      7.91 ns |      7.40 ns |      1,251.9 ns |  1.45 |    0.01 |   0.0134 |     1184 B |           N A  |
 *      | InlineArray | 1     |      1,097.7 ns |      1.58 ns |      1.40 ns |      1,097.9 ns |  1.27 |    0.00 |        - |          - |           N A  |
 *      |             |       |                 |              |              |                 |       |         |          |            |                   |
 *      | Span        | 10    |      8,760.9 ns |     34.07 ns |     28.45 ns |      8,748.4 ns |  1.00 |    0.00 |        - |          - |           N A  |
 *      | Array       | 10    |     11,986.2 ns |     59.23 ns |     55.40 ns |     11,998.5 ns |  1.37 |    0.01 |   0.1373 |    11840 B |           N A  |
 *      | InlineArray | 10    |     10,710.6 ns |      9.52 ns |      7.95 ns |     10,710.0 ns |  1.22 |    0.00 |        - |          - |           N A  |
 *      |             |       |                 |              |              |                 |       |         |          |            |                   |
 *      | Span        | 100   |     88,190.3 ns |    547.56 ns |    512.19 ns |     88,108.9 ns |  1.00 |    0.00 |        - |          - |           N A  |
 *      | Array       | 100   |    119,838.8 ns |    599.71 ns |    560.96 ns |    119,762.5 ns |  1.36 |    0.01 |   1.3428 |   118400 B |           N A  |
 *      | InlineArray | 100   |    107,655.0 ns |    428.70 ns |    380.03 ns |    107,767.0 ns |  1.22 |    0.01 |        - |          - |           N A  |
 *      |             |       |                 |              |              |                 |       |         |          |            |                   |
 *      | Span        | 1000  |    879,708.9 ns |  6,501.91 ns |  5,429.39 ns |    876,643.0 ns |  1.00 |    0.00 |        - |        1 B |         1   .00  |
 *      | Array       | 1000  |  1,230,240.4 ns |  6,643.96 ns |  5,889.69 ns |  1,227,653.0 ns |  1.40 |    0.01 |  13.6719 |  1184002 B | 1   ,184,002.00  |
 *      | InlineArray | 1000  |  1,099,007.6 ns | 21,964.93 ns | 48,213.56 ns |  1,139,939.6 ns |  1.22 |    0.05 |        - |        2 B |         2   .00  |
 *      |             |       |                 |              |              |                 |       |         |          |            |                   |
 *      | Span        | 10000 |  8,794,110.2 ns | 65,234.36 ns | 61,020.26 ns |  8,796,836.6 ns |  1.00 |    0.00 |        - |       17 B |         1   .00  |
 *      | Array       | 10000 | 12,268,578.7 ns | 53,126.63 ns | 41,477.78 ns | 12,282,356.0 ns |  1.40 |    0.01 | 140.6250 | 11840017 B |   6 96,471.59  |
 *      | InlineArray | 10000 | 10,863,634.7 ns | 57,803.32 ns | 54,069.27 ns | 10,865,568.9 ns |  1.24 |    0.01 |        - |       17 B |         1   .00  |
 *
 *
 *      BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *      AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *      .NET SDK 8.0.100
 *        [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *        DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *      | Method      | Size  | Mean            | Error         | StdDev        | Ratio | Gen0     | Allocated  | Alloc Ratio  |
 *      |------------ |------ |----------------:|--------------:|--------------:|------:|---------:|-----------:|-------------:|
 *      | Span        | 1     |        885.8 ns |       6.96 ns |       5.43 ns |  1.00 |        - |          - |           NA |
 *      | Array       | 1     |      1,144.1 ns |       6.15 ns |       5.75 ns |  1.29 |   0.0706 |     1184 B |           NA |
 *      | InlineArray | 1     |      1,029.4 ns |       1.23 ns |       1.03 ns |  1.16 |        - |          - |           NA |
 *      |             |       |                 |               |               |       |          |            |              |
 *      | Span        | 10    |      8,517.7 ns |      12.26 ns |      10.86 ns |  1.00 |        - |          - |           NA |
 *      | Array       | 10    |     11,135.2 ns |      37.68 ns |      33.40 ns |  1.31 |   0.7019 |    11840 B |           NA |
 *      | InlineArray | 10    |     10,289.7 ns |      12.62 ns |      11.19 ns |  1.21 |        - |          - |           NA |
 *      |             |       |                 |               |               |       |          |            |              |
 *      | Span        | 100   |     87,016.3 ns |     167.74 ns |     140.07 ns |  1.00 |        - |          - |           NA |
 *      | Array       | 100   |    109,855.2 ns |     405.28 ns |     359.27 ns |  1.26 |   6.9580 |   118400 B |           NA |
 *      | InlineArray | 100   |    100,458.3 ns |     127.24 ns |     112.80 ns |  1.15 |        - |          - |           NA |
 *      |             |       |                 |               |               |       |          |            |              |
 *      | Span        | 1000  |    848,901.3 ns |   3,623.56 ns |   3,212.19 ns |  1.00 |        - |        1 B |         1.00 |
 *      | Array       | 1000  |  1,100,846.0 ns |   7,050.30 ns |   6,249.90 ns |  1.30 |  70.3125 |  1184001 B | 1,184,001.00 |
 *      | InlineArray | 1000  |  1,011,266.8 ns |   1,444.43 ns |   1,127.72 ns |  1.19 |        - |        1 B |         1.00 |
 *      |             |       |                 |               |               |       |          |            |              |
 *      | Span        | 10000 |  8,754,527.3 ns |  25,603.41 ns |  23,949.44 ns |  1.00 |        - |       12 B |         1.00 |
 *      | Array       | 10000 | 11,037,788.5 ns | 128,035.75 ns | 113,500.36 ns |  1.26 | 703.1250 | 11840006 B |   986,667.17 |
 *      | InlineArray | 10000 | 10,138,412.1 ns |  54,830.85 ns |  51,288.81 ns |  1.16 |        - |       12 B |         1.00 |
 *
 *
 *      BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *      Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *      .NET SDK 8.0.100
 *        [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *        DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *      | Method      | Size  | Mean          | Error       | StdDev        | Median        | Ratio | RatioSD | Gen0      | Allocated  | Alloc Ratio |
 *      |------------ |------ |--------------:|------------:|--------------:|--------------:|------:|--------:|----------:|-----------:|------------:|
 *      | Span        | 1     |      1.701 us |   0.0335 us |     0.0595 us |      1.721 us |  1.00 |    0.00 |         - |          - |          NA |
 *      | Array       | 1     |      1.899 us |   0.0376 us |     0.0502 us |      1.909 us |  1.11 |    0.04 |    0.1869 |     1184 B |          NA |
 *      | InlineArray | 1     |      2.004 us |   0.0392 us |     0.0550 us |      2.027 us |  1.17 |    0.05 |         - |          - |          NA |
 *      |             |       |               |             |               |               |       |         |           |            |             |
 *      | Span        | 10    |     17.044 us |   0.3350 us |     0.3585 us |     17.018 us |  1.00 |    0.00 |         - |          - |          NA |
 *      | Array       | 10    |     19.391 us |   0.2286 us |     0.2138 us |     19.385 us |  1.14 |    0.03 |    1.8616 |    11840 B |          NA |
 *      | InlineArray | 10    |     20.511 us |   0.3951 us |     0.4057 us |     20.403 us |  1.21 |    0.03 |         - |          - |          NA |
 *      |             |       |               |             |               |               |       |         |           |            |             |
 *      | Span        | 100   |    171.641 us |   2.4507 us |     2.2924 us |    171.424 us |  1.00 |    0.00 |         - |          - |          NA |
 *      | Array       | 100   |    186.504 us |   3.1989 us |     2.9922 us |    186.839 us |  1.09 |    0.02 |   18.7988 |   118400 B |          NA |
 *      | InlineArray | 100   |    203.386 us |   4.0060 us |     7.0161 us |    201.587 us |  1.22 |    0.03 |         - |          - |          NA |
 *      |             |       |               |             |               |               |       |         |           |            |             |
 *      | Span        | 1000  |  1,382.093 us |  28.3285 us |    78.9686 us |  1,358.361 us |  1.00 |    0.00 |         - |        2 B |        1.00 |
 *      | Array       | 1000  |  1,483.328 us |  29.3896 us |    71.5384 us |  1,465.778 us |  1.07 |    0.07 |  187.5000 |  1184002 B |  592,001.00 |
 *      | InlineArray | 1000  |  1,537.432 us |  24.4554 us |    20.4213 us |  1,533.203 us |  1.05 |    0.05 |         - |        2 B |        1.00 |
 *      |             |       |               |             |               |               |       |         |           |            |             |
 *      | Span        | 10000 | 13,118.931 us | 241.1647 us |   236.8560 us | 13,088.418 us |  1.00 |    0.00 |         - |       17 B |        1.00 |
 *      | Array       | 10000 | 14,841.933 us | 296.6092 us |   721.9879 us | 14,674.459 us |  1.12 |    0.06 | 1875.0000 | 11840017 B |  696,471.59 |
 *      | InlineArray | 10000 | 19,648.227 us | 757.0622 us | 2,122.8835 us | 19,642.494 us |  1.42 |    0.13 |         - |       34 B |        2.00 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class InlineArrays_Benchmark : BaseBenchmark
{
    private static readonly Move[] _arrayMovePool = new Move[Constants.MaxNumberOfPossibleMovesInAPosition];

    [Params(1, 10, 100, 1_000, 10_000)]
    public int Size { get; set; }

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

        int Array(Position position)
        {
            var allMoves = MoveGenerator.GenerateAllMoves(position, _arrayMovePool);

            return allMoves.Length;
        }
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

        static int InlineArray(Position position)
        {
            var moveArray = new MoveArray();
            var allMoves = MoveGenerator.GenerateAllMoves(position, ref moveArray);

            return allMoves.Length;
        }
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
