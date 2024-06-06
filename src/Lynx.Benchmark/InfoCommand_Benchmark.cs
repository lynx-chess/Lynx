/*
 * Consistently faster using SB
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.301
 *    [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *
 *  | Method         | result               | Mean       | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |--------------- |--------------------- |-----------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
 *  | StringAddition | Lynx.(...)esult [23] |   463.0 ns | 1.15 ns | 1.02 ns |  1.00 |    0.00 | 0.0076 |     640 B |        1.00 |
 *  | StringAddition | Lynx.(...)esult [23] |   957.6 ns | 2.76 ns | 2.58 ns |  2.07 |    0.01 | 0.0143 |    1248 B |        1.95 |
 *  | StringAddition | Lynx.(...)esult [23] | 1,035.2 ns | 2.43 ns | 1.90 ns |  2.24 |    0.01 | 0.0191 |    1664 B |        2.60 |
 *  | StringAddition | Lynx.(...)esult [23] | 1,550.9 ns | 4.63 ns | 4.33 ns |  3.35 |    0.01 | 0.0305 |    2648 B |        4.14 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   245.0 ns | 1.22 ns | 1.08 ns |  0.53 |    0.00 | 0.0100 |     864 B |        1.35 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   652.2 ns | 2.46 ns | 2.30 ns |  1.41 |    0.01 | 0.0153 |    1328 B |        2.08 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   768.5 ns | 5.62 ns | 5.25 ns |  1.66 |    0.01 | 0.0191 |    1664 B |        2.60 |
 *  | StringBuilder  | Lynx.(...)esult [23] | 1,361.2 ns | 7.49 ns | 7.01 ns |  2.94 |    0.02 | 0.0362 |    3032 B |        4.74 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2461) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.301
 *    [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
 *
 *  | Method         | result               | Mean       | Error    | StdDev   | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |--------------- |--------------------- |-----------:|---------:|---------:|-----------:|------:|--------:|-------:|----------:|------------:|
 *  | StringAddition | Lynx.(...)esult [23] |   411.8 ns |  5.85 ns |  5.47 ns |   413.0 ns |  1.00 |    0.00 | 0.0381 |     640 B |        1.00 |
 *  | StringAddition | Lynx.(...)esult [23] |   737.8 ns |  6.32 ns |  5.91 ns |   739.5 ns |  1.79 |    0.03 | 0.0744 |    1248 B |        1.95 |
 *  | StringAddition | Lynx.(...)esult [23] |   869.9 ns |  6.30 ns |  5.58 ns |   870.8 ns |  2.11 |    0.03 | 0.0992 |    1664 B |        2.60 |
 *  | StringAddition | Lynx.(...)esult [23] | 1,293.9 ns | 23.91 ns | 22.37 ns | 1,298.9 ns |  3.14 |    0.09 | 0.1564 |    2648 B |        4.14 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   194.6 ns |  3.60 ns |  7.36 ns |   191.7 ns |  0.47 |    0.02 | 0.0515 |     864 B |        1.35 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   477.9 ns |  9.52 ns |  9.35 ns |   476.3 ns |  1.16 |    0.02 | 0.0792 |    1328 B |        2.08 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   637.1 ns |  2.90 ns |  2.42 ns |   636.8 ns |  1.55 |    0.02 | 0.0992 |    1664 B |        2.60 |
 *  | StringBuilder  | Lynx.(...)esult [23] | 1,092.3 ns |  2.82 ns |  2.35 ns | 1,091.5 ns |  2.65 |    0.04 | 0.1812 |    3032 B |        4.74 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Sonoma 14.5 (23F79) [Darwin 23.5.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.301
 *    [Host]     : .NET 8.0.6 (8.0.624.26715), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.6 (8.0.624.26715), Arm64 RyuJIT AdvSIMD
 *
 *  | Method         | result               | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |--------------- |--------------------- |-----------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | StringAddition | Lynx.(...)esult [23] |   299.1 ns |  6.00 ns | 11.71 ns |  1.00 |    0.00 | 0.1016 |      - |     640 B |        1.00 |
 *  | StringAddition | Lynx.(...)esult [23] |   531.0 ns | 10.27 ns | 14.73 ns |  1.78 |    0.11 | 0.1984 |      - |    1248 B |        1.95 |
 *  | StringAddition | Lynx.(...)esult [23] |   709.8 ns | 13.86 ns | 13.62 ns |  2.41 |    0.15 | 0.2651 |      - |    1664 B |        2.60 |
 *  | StringAddition | Lynx.(...)esult [23] | 1,072.6 ns | 21.41 ns | 26.29 ns |  3.62 |    0.17 | 0.4196 |      - |    2648 B |        4.14 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   168.9 ns |  3.39 ns |  4.64 ns |  0.57 |    0.02 | 0.1376 | 0.0002 |     864 B |        1.35 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   397.1 ns |  7.81 ns | 11.45 ns |  1.33 |    0.07 | 0.2112 |      - |    1328 B |        2.08 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   543.3 ns | 10.66 ns | 14.59 ns |  1.83 |    0.09 | 0.2651 |      - |    1664 B |        2.60 |
 *  | StringBuilder  | Lynx.(...)esult [23] |   885.8 ns |  6.08 ns |  5.68 ns |  3.01 |    0.17 | 0.4826 |      - |    3032 B |        4.74 |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using Lynx.UCI.Commands;
using System.Text;

namespace Lynx.Benchmark;
public class InfoCommand_Benchmark : BaseBenchmark
{
    public static IEnumerable<SearchResult> Data => new[] {
        new SearchResult(33592128, 27, 20, [33592128], -500, +500 ),
        new SearchResult(33592128, 27, 20, [33591088, 33975472, 112608, 480352, 109456, 477200, 47968, 495952, 187344, 417008, 24220320, 18256304], -500, +500 ),
        new SearchResult(33592128, 27, 20, [33592128, 33974432, 112608, 477200, 109456, 33976512, 166864, 412848, 44848, 536656, 101055424, 480352, 93856, 18312528, 24276512, 101390400, 43808, 544800, 177056, 19426624], -500, +500 ),
        new SearchResult(33592128, 27, 20, [352512, 684368, 345472, 691648, 344336, 675376, 353536, 699696, 345488, 701104, 344336, 709312, 345344, 706368, 346384, 699152, 345376, 689824, 344336, 688656, 345344, 697856, 344336, 705168, 345344, 714496, 338192, 671632, 345248, 678128, 353552, 714080, 346512, 693136, 345376, 676416, 352528, 717120, 345472], -500, +500),
        };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int StringAddition(SearchResult result)
    {
        return InfoCommand_StringAddition.SearchResultInfo(result).Length;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int StringBuilder(SearchResult result)
    {
        return InfoCommand_StringBuilder.SearchResultInfo(result).Length;
    }

    public sealed class InfoCommand_StringAddition : EngineBaseCommand
    {
        public const string Id = "info";

        public static string SearchResultInfo(SearchResult searchResult)
        {
#pragma warning disable RCS1214 // Unnecessary interpolated string.
            return Id +
                $" depth {searchResult.Depth}" +
                $" seldepth {searchResult.DepthReached}" +
                $" multipv 1" +
                $" score {(searchResult.Mate == default ? $"cp {WDL.NormalizeScore(searchResult.Evaluation)}" : $"mate {searchResult.Mate}")}" +
                $" nodes {searchResult.Nodes}" +
                $" nps {searchResult.NodesPerSecond}" +
                $" time {searchResult.Time}" +
                (searchResult.HashfullPermill != -1 ? $" hashfull {searchResult.HashfullPermill}" : string.Empty) +
                (searchResult.WDL is not null ? $" wdl {searchResult.WDL.Value.WDLWin} {searchResult.WDL.Value.WDLDraw} {searchResult.WDL.Value.WDLLoss}" : string.Empty) +
                $" pv {string.Join(" ", searchResult.Moves.Select(move => move.UCIString()))}";
#pragma warning restore RCS1214 // Unnecessary interpolated string.
        }
    }

    public sealed class InfoCommand_StringBuilder_256 : EngineBaseCommand
    {
        public const string Id = "info";

        public static string SearchResultInfo(SearchResult searchResult)
        {
            var sb = new StringBuilder(256);

            sb.Append(Id);
            sb.Append(" depth ").Append(searchResult.Depth);
            sb.Append(" seldepth ").Append(searchResult.DepthReached);
            sb.Append(" multipv 1");
            sb.Append(" score ").Append(searchResult.Mate == default ? $"cp {WDL.NormalizeScore(searchResult.Evaluation)}" : $"mate {searchResult.Mate}");
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

            sb.Append(" pv ").AppendJoin(" ", searchResult.Moves.Select(move => move.UCIString()));

            return sb.ToString();
        }
    }

    public sealed class InfoCommand_StringBuilder_128 : EngineBaseCommand
    {
        public const string Id = "info";

        public static string SearchResultInfo(SearchResult searchResult)
        {
            var sb = new StringBuilder(128);

            sb.Append(Id);
            sb.Append(" depth ").Append(searchResult.Depth);
            sb.Append(" seldepth ").Append(searchResult.DepthReached);
            sb.Append(" multipv 1");
            sb.Append(" score ").Append(searchResult.Mate == default ? $"cp {WDL.NormalizeScore(searchResult.Evaluation)}" : $"mate {searchResult.Mate}");
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

            sb.Append(" pv ").AppendJoin(" ", searchResult.Moves.Select(move => move.UCIString()));

            return sb.ToString();
        }
    }
}
