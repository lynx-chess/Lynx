/*
 * SB consistently faster
 * Init size doesn't matter that much
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.301
 *    [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *
 *  | Method            | result               | Mean       | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------ |--------------------- |-----------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
 *  | StringAddition    | Lynx.(...)esult [23] |   471.1 ns | 1.69 ns | 1.50 ns |  1.00 |    0.00 | 0.0076 |     640 B |        1.00 |
 *  | StringAddition    | Lynx.(...)esult [23] |   847.1 ns | 3.29 ns | 3.08 ns |  1.80 |    0.01 | 0.0143 |    1248 B |        1.95 |
 *  | StringAddition    | Lynx.(...)esult [23] | 1,033.5 ns | 1.95 ns | 1.63 ns |  2.19 |    0.01 | 0.0191 |    1664 B |        2.60 |
 *  | StringAddition    | Lynx.(...)esult [23] | 1,558.6 ns | 3.32 ns | 3.10 ns |  3.31 |    0.01 | 0.0305 |    2648 B |        4.14 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   230.4 ns | 1.14 ns | 1.07 ns |  0.49 |    0.00 | 0.0076 |     640 B |        1.00 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   603.0 ns | 5.86 ns | 5.19 ns |  1.28 |    0.01 | 0.0162 |    1432 B |        2.24 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   815.0 ns | 5.13 ns | 4.54 ns |  1.73 |    0.01 | 0.0210 |    1768 B |        2.76 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] | 1,404.8 ns | 7.00 ns | 6.20 ns |  2.98 |    0.01 | 0.0362 |    3144 B |        4.91 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   249.3 ns | 1.80 ns | 1.68 ns |  0.53 |    0.00 | 0.0105 |     896 B |        1.40 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   556.2 ns | 5.38 ns | 5.03 ns |  1.18 |    0.01 | 0.0162 |    1360 B |        2.12 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   767.1 ns | 4.73 ns | 4.19 ns |  1.63 |    0.01 | 0.0200 |    1696 B |        2.65 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] | 1,369.9 ns | 8.31 ns | 7.77 ns |  2.91 |    0.02 | 0.0362 |    3072 B |        4.80 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2461) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.301
 *    [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *
 *  | Method            | result               | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------ |--------------------- |-----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | StringAddition    | Lynx.(...)esult [23] |   412.1 ns |  5.35 ns |  5.00 ns |  1.00 |    0.00 | 0.0381 |     640 B |        1.00 |
 *  | StringAddition    | Lynx.(...)esult [23] |   713.6 ns | 11.42 ns | 10.69 ns |  1.73 |    0.04 | 0.0744 |    1248 B |        1.95 |
 *  | StringAddition    | Lynx.(...)esult [23] |   904.1 ns |  4.62 ns |  3.85 ns |  2.20 |    0.02 | 0.0992 |    1664 B |        2.60 |
 *  | StringAddition    | Lynx.(...)esult [23] | 1,308.2 ns | 17.17 ns | 16.06 ns |  3.17 |    0.03 | 0.1564 |    2648 B |        4.14 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   183.8 ns |  1.01 ns |  0.85 ns |  0.45 |    0.01 | 0.0381 |     640 B |        1.00 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   513.6 ns |  2.50 ns |  2.22 ns |  1.25 |    0.02 | 0.0849 |    1432 B |        2.24 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   679.7 ns |  4.19 ns |  3.49 ns |  1.65 |    0.01 | 0.1049 |    1768 B |        2.76 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] | 1,190.2 ns | 22.55 ns | 21.09 ns |  2.89 |    0.06 | 0.1869 |    3144 B |        4.91 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   208.6 ns |  1.44 ns |  1.20 ns |  0.51 |    0.01 | 0.0534 |     896 B |        1.40 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   462.9 ns |  3.76 ns |  3.52 ns |  1.12 |    0.01 | 0.0811 |    1360 B |        2.12 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   636.3 ns |  2.32 ns |  2.05 ns |  1.55 |    0.02 | 0.1011 |    1696 B |        2.65 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] | 1,124.3 ns | 16.13 ns | 13.47 ns |  2.73 |    0.05 | 0.1831 |    3072 B |        4.80 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Ventura 13.6.7 (22G720) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.301
 *    [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *
 *  | Method            | result               | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------ |--------------------- |-----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | StringAddition    | Lynx.(...)esult [23] |   561.0 ns | 10.93 ns | 11.22 ns |  1.00 |    0.00 | 0.1011 |     640 B |        1.00 |
 *  | StringAddition    | Lynx.(...)esult [23] | 1,019.5 ns | 12.52 ns | 11.10 ns |  1.82 |    0.05 | 0.1984 |    1248 B |        1.95 |
 *  | StringAddition    | Lynx.(...)esult [23] | 1,232.4 ns | 21.45 ns | 26.34 ns |  2.20 |    0.07 | 0.2651 |    1664 B |        2.60 |
 *  | StringAddition    | Lynx.(...)esult [23] | 1,935.6 ns | 33.04 ns | 30.91 ns |  3.45 |    0.11 | 0.4196 |    2648 B |        4.14 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   269.5 ns |  4.27 ns |  3.78 ns |  0.48 |    0.01 | 0.1016 |     640 B |        1.00 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   742.2 ns | 11.43 ns | 10.69 ns |  1.32 |    0.03 | 0.2279 |    1432 B |        2.24 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] | 1,088.8 ns | 20.35 ns | 20.90 ns |  1.94 |    0.06 | 0.2804 |    1768 B |        2.76 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] | 1,846.4 ns | 36.17 ns | 40.20 ns |  3.30 |    0.10 | 0.4959 |    3144 B |        4.91 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   282.7 ns |  4.11 ns |  3.85 ns |  0.50 |    0.01 | 0.1426 |     896 B |        1.40 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   695.2 ns | 10.64 ns |  9.95 ns |  1.24 |    0.04 | 0.2165 |    1360 B |        2.12 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   966.1 ns | 14.41 ns | 12.77 ns |  1.72 |    0.05 | 0.2689 |    1696 B |        2.65 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] | 1,770.0 ns | 29.46 ns | 26.11 ns |  3.16 |    0.07 | 0.4883 |    3072 B |        4.80 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Sonoma 14.5 (23F79) [Darwin 23.5.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.301
 *    [Host]     : .NET 8.0.6 (8.0.624.26715), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.6 (8.0.624.26715), Arm64 RyuJIT AdvSIMD
 *
 *  | Method            | result               | Mean       | Error    | StdDev   | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |------------------ |--------------------- |-----------:|---------:|---------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | StringAddition    | Lynx.(...)esult [23] |   271.1 ns |  5.26 ns |  6.65 ns |   266.8 ns |  1.00 |    0.00 | 0.1016 |      - |     640 B |        1.00 |
 *  | StringAddition    | Lynx.(...)esult [23] |   478.1 ns |  4.62 ns |  4.33 ns |   477.7 ns |  1.75 |    0.05 | 0.1984 |      - |    1248 B |        1.95 |
 *  | StringAddition    | Lynx.(...)esult [23] |   741.5 ns |  3.61 ns |  3.20 ns |   741.7 ns |  2.71 |    0.07 | 0.2632 |      - |    1664 B |        2.60 |
 *  | StringAddition    | Lynx.(...)esult [23] |   943.3 ns |  4.89 ns |  4.08 ns |   943.0 ns |  3.44 |    0.10 | 0.4196 |      - |    2648 B |        4.14 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   146.0 ns |  0.40 ns |  0.38 ns |   146.0 ns |  0.53 |    0.01 | 0.1018 |      - |     640 B |        1.00 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   387.7 ns |  0.40 ns |  0.38 ns |   387.7 ns |  1.42 |    0.04 | 0.2279 |      - |    1432 B |        2.24 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] |   546.2 ns |  8.14 ns |  7.21 ns |   543.1 ns |  2.00 |    0.05 | 0.2813 |      - |    1768 B |        2.76 |
 *  | StringBuilder_128 | Lynx.(...)esult [23] | 1,088.6 ns | 20.83 ns | 23.99 ns | 1,086.5 ns |  4.01 |    0.15 | 0.4997 |      - |    3144 B |        4.91 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   186.3 ns |  9.25 ns | 27.29 ns |   173.3 ns |  0.84 |    0.07 | 0.1428 |      - |     896 B |        1.40 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   368.0 ns |  5.60 ns |  5.24 ns |   367.9 ns |  1.35 |    0.04 | 0.2165 |      - |    1360 B |        2.12 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   525.8 ns |  9.43 ns | 11.23 ns |   523.4 ns |  1.94 |    0.07 | 0.2699 |      - |    1696 B |        2.65 |
 *  | StringBuilder_256 | Lynx.(...)esult [23] |   901.2 ns |  9.55 ns |  7.98 ns |   897.4 ns |  3.29 |    0.09 | 0.4883 | 0.0019 |    3072 B |        4.80 |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using Lynx.UCI.Commands;
using System.Text;

namespace Lynx.Benchmark;
public class InfoCommand_Benchmark : BaseBenchmark
{
    public static IEnumerable<SearchResult> Data =>
    [
        new SearchResult(33592128, 27, 20, 1, [33592128]),
        new SearchResult(33592128, 27, 20, 12, [33591088, 33975472, 112608, 480352, 109456, 477200, 47968, 495952, 187344, 417008, 24220320, 18256304] ),
        new SearchResult(33592128, 27, 20, 20, [33592128, 33974432, 112608, 477200, 109456, 33976512, 166864, 412848, 44848, 536656, 101055424, 480352, 93856, 18312528, 24276512, 101390400, 43808, 544800, 177056, 19426624] ),
        new SearchResult(33592128, 27, 20, 39, [352512, 684368, 345472, 691648, 344336, 675376, 353536, 699696, 345488, 701104, 344336, 709312, 345344, 706368, 346384, 699152, 345376, 689824, 344336, 688656, 345344, 697856, 344336, 705168, 345344, 714496, 338192, 671632, 345248, 678128, 353552, 714080, 346512, 693136, 345376, 676416, 352528, 717120, 345472]),
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int StringAddition(SearchResult result)
    {
        return InfoCommand_StringAddition.SearchResultInfo(result).Length;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int StringBuilder_128(SearchResult result)
    {
        return InfoCommand_StringBuilder_128.SearchResultInfo(result).Length;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int StringBuilder_256(SearchResult result)
    {
        return InfoCommand_StringBuilder_256.SearchResultInfo(result).Length;
    }

    public sealed class InfoCommand_StringAddition : IEngineBaseCommand
    {
        public const string Id = "info";

        public static string SearchResultInfo(SearchResult searchResult)
        {
#pragma warning disable RCS1214 // Unnecessary interpolated string.
            return Id +
                $" depth {searchResult.Depth}" +
                $" seldepth {searchResult.DepthReached}" +
                $" multipv 1" +
                $" score {(searchResult.Mate == default ? $"cp {WDL.NormalizeScore(searchResult.Score)}" : $"mate {searchResult.Mate}")}" +
                $" nodes {searchResult.Nodes}" +
                $" nps {searchResult.NodesPerSecond}" +
                $" time {searchResult.Time}" +
                (searchResult.HashfullPermill != -1 ? $" hashfull {searchResult.HashfullPermill}" : string.Empty) +
                (searchResult.WDL is not null ? $" wdl {searchResult.WDL.Value.WDLWin} {searchResult.WDL.Value.WDLDraw} {searchResult.WDL.Value.WDLLoss}" : string.Empty) +
                $" pv {string.Join(" ", searchResult.Moves.TakeWhile(m => m != 0).Select(move => move.UCIString()))}";
#pragma warning restore RCS1214 // Unnecessary interpolated string.
        }
    }

    public sealed class InfoCommand_StringBuilder_256 : IEngineBaseCommand
    {
        public const string Id = "info";

        public static string SearchResultInfo(SearchResult searchResult)
        {
            var sb = new StringBuilder(256);

            sb.Append(Id);
            sb.Append(" depth ").Append(searchResult.Depth);
            sb.Append(" seldepth ").Append(searchResult.DepthReached);
            sb.Append(" multipv 1");
            sb.Append(" score ").Append(searchResult.Mate == default ? $"cp {WDL.NormalizeScore(searchResult.Score)}" : $"mate {searchResult.Mate}");
            sb.Append(" nodes ").Append(searchResult.Nodes);
            sb.Append(" nps ").Append(searchResult.NodesPerSecond);
            sb.Append(" time ").Append(searchResult.Time)
                .Append(searchResult.HashfullPermill != -1 ? $" hashfull {searchResult.HashfullPermill}" : string.Empty);

            if (searchResult.WDL is not null)
            {
                sb.Append(" wdl ")
                    .Append(searchResult.WDL.Value.WDLWin).Append(' ')
                    .Append(searchResult.WDL.Value.WDLDraw).Append(' ')
                    .Append(searchResult.WDL.Value.WDLLoss);
            }

            sb.Append(" pv ").AppendJoin(" ", searchResult.Moves.TakeWhile(m => m != 0).Select(move => move.UCIString()));

            return sb.ToString();
        }
    }

    public sealed class InfoCommand_StringBuilder_128 : IEngineBaseCommand
    {
        public const string Id = "info";

        public static string SearchResultInfo(SearchResult searchResult)
        {
            var sb = new StringBuilder(128);

            sb.Append(Id);
            sb.Append(" depth ").Append(searchResult.Depth);
            sb.Append(" seldepth ").Append(searchResult.DepthReached);
            sb.Append(" multipv 1");
            sb.Append(" score ").Append(searchResult.Mate == default ? $"cp {WDL.NormalizeScore(searchResult.Score)}" : $"mate {searchResult.Mate}");
            sb.Append(" nodes ").Append(searchResult.Nodes);
            sb.Append(" nps ").Append(searchResult.NodesPerSecond);
            sb.Append(" time ").Append(searchResult.Time)
                .Append(searchResult.HashfullPermill != -1 ? $" hashfull {searchResult.HashfullPermill}" : string.Empty);

            if (searchResult.WDL is not null)
            {
                sb.Append(" wdl ")
                    .Append(searchResult.WDL.Value.WDLWin).Append(' ')
                    .Append(searchResult.WDL.Value.WDLDraw).Append(' ')
                    .Append(searchResult.WDL.Value.WDLLoss);
            }

            sb.Append(" pv ").AppendJoin(" ", searchResult.Moves.TakeWhile(m => m != 0).Select(move => move.UCIString()));

            return sb.ToString();
        }
    }
}
