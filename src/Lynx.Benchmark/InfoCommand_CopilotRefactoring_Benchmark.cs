/*
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method   | result               | Mean       | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |--------- |--------------------- |-----------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
 *  | Original | Lynx.(...)esult [23] |   256.2 ns | 2.53 ns | 2.11 ns |  1.00 |    0.01 | 0.0105 |     896 B |        1.00 |
 *  | Original | Lynx.(...)esult [23] |   601.6 ns | 5.29 ns | 4.94 ns |  2.35 |    0.03 | 0.0162 |    1360 B |        1.52 |
 *  | Original | Lynx.(...)esult [23] |   800.4 ns | 4.72 ns | 4.42 ns |  3.12 |    0.03 | 0.0200 |    1696 B |        1.89 |
 *  | Original | Lynx.(...)esult [23] | 1,378.7 ns | 7.41 ns | 6.93 ns |  5.38 |    0.05 | 0.0362 |    3072 B |        3.43 |
 *  | Copilot  | Lynx.(...)esult [23] |   174.3 ns | 0.76 ns | 0.71 ns |  0.68 |    0.01 | 0.0098 |     824 B |        0.92 |
 *  | Copilot  | Lynx.(...)esult [23] |   414.0 ns | 2.97 ns | 2.63 ns |  1.62 |    0.02 | 0.0153 |    1288 B |        1.44 |
 *  | Copilot  | Lynx.(...)esult [23] |   576.3 ns | 4.31 ns | 3.82 ns |  2.25 |    0.02 | 0.0191 |    1624 B |        1.81 |
 *  | Copilot  | Lynx.(...)esult [23] | 1,041.4 ns | 7.90 ns | 7.39 ns |  4.06 |    0.04 | 0.0343 |    3000 B |        3.35 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2655) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method   | result               | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |--------- |--------------------- |-----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | Original | Lynx.(...)esult [23] |   232.9 ns |  3.95 ns |  3.50 ns |  1.00 |    0.02 | 0.0534 |     896 B |        1.00 |
 *  | Original | Lynx.(...)esult [23] |   507.6 ns |  6.64 ns |  6.21 ns |  2.18 |    0.04 | 0.0811 |    1360 B |        1.52 |
 *  | Original | Lynx.(...)esult [23] |   694.1 ns |  8.75 ns |  8.19 ns |  2.98 |    0.06 | 0.1011 |    1696 B |        1.89 |
 *  | Original | Lynx.(...)esult [23] | 1,214.0 ns | 23.94 ns | 23.51 ns |  5.21 |    0.12 | 0.1831 |    3072 B |        3.43 |
 *  | Copilot  | Lynx.(...)esult [23] |   132.6 ns |  2.63 ns |  3.60 ns |  0.57 |    0.02 | 0.0491 |     824 B |        0.92 |
 *  | Copilot  | Lynx.(...)esult [23] |   331.7 ns |  6.32 ns |  5.91 ns |  1.42 |    0.03 | 0.0768 |    1288 B |        1.44 |
 *  | Copilot  | Lynx.(...)esult [23] |   468.3 ns |  4.41 ns |  4.12 ns |  2.01 |    0.03 | 0.0968 |    1624 B |        1.81 |
 *  | Copilot  | Lynx.(...)esult [23] |   855.3 ns | 16.99 ns | 16.69 ns |  3.67 |    0.09 | 0.1793 |    3000 B |        3.35 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *
 *  | Method   | result               | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |--------- |--------------------- |----------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Original | Lynx.(...)esult [23] | 172.81 ns |  3.230 ns |  4.527 ns |  1.00 |    0.04 | 0.1428 |      - |     896 B |        1.00 |
 *  | Original | Lynx.(...)esult [23] | 403.50 ns |  6.900 ns |  6.776 ns |  2.34 |    0.07 | 0.2165 |      - |    1360 B |        1.52 |
 *  | Original | Lynx.(...)esult [23] | 573.27 ns | 11.103 ns | 10.386 ns |  3.32 |    0.10 | 0.2699 |      - |    1696 B |        1.89 |
 *  | Original | Lynx.(...)esult [23] | 992.51 ns | 19.325 ns | 19.846 ns |  5.75 |    0.18 | 0.4883 | 0.0019 |    3072 B |        3.43 |
 *  | Copilot  | Lynx.(...)esult [23] |  91.97 ns |  1.506 ns |  1.176 ns |  0.53 |    0.01 | 0.1312 |      - |     824 B |        0.92 |
 *  | Copilot  | Lynx.(...)esult [23] | 246.41 ns |  3.417 ns |  2.668 ns |  1.43 |    0.04 | 0.2050 |      - |    1288 B |        1.44 |
 *  | Copilot  | Lynx.(...)esult [23] | 353.94 ns |  7.065 ns |  8.935 ns |  2.05 |    0.07 | 0.2584 | 0.0005 |    1624 B |        1.81 |
 *  | Copilot  | Lynx.(...)esult [23] | 660.87 ns | 10.762 ns | 11.515 ns |  3.83 |    0.12 | 0.4778 | 0.0010 |    3000 B |        3.35 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.6.9 (22G830) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method   | result               | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio   *|
 *  |--------- |---------------------     *|-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
 *  | Original | Lynx.(...)esult [23] |   349.5 ns |  11.36 ns |  32.77 ns |   349.5 ns |  1.01 |    0.13 | 0.1426 |     896 B |        1.00   *|
 *  | Original | Lynx.(...)esult [23] |   842.6 ns |  16.83 ns |  43.14 ns |   833.6 ns |  2.43 |    0.25 | 0.2165 |    1360 B |        1.52   *|
 *  | Original | Lynx.(...)esult [23] | 1,150.8 ns |  21.33 ns |  19.95 ns | 1,151.0 ns |  3.32 |    0.31 | 0.2689 |    1696 B |        1.89   *|
 *  | Original | Lynx.(...)esult [23] | 1,989.1 ns | 103.03 ns | 295.62 ns | 1,855.6 ns |  5.74 |    1.00 | 0.4883 |    3072 B |        3.43   *|
 *  | Copilot  | Lynx.(...)esult [23] |   170.4 ns |   2.08 ns |   1.95 ns |   169.7 ns |  0.49 |    0.05 | 0.1311 |     824 B |        0.92   *|
 *  | Copilot  | Lynx.(...)esult [23] |   493.6 ns |   7.44 ns |   6.96 ns |   490.7 ns |  1.42 |    0.13 | 0.2050 |    1288 B |        1.44   *|
 *  | Copilot  | Lynx.(...)esult [23] |   694.8 ns |   7.41 ns |   6.19 ns |   697.0 ns |  2.00 |    0.18 | 0.2584 |    1624 B |        1.81   *|
 *  | Copilot  | Lynx.(...)esult [23] | 1,315.1 ns |  22.88 ns |  21.40 ns | 1,314.8 ns |  3.79 |    0.35 | 0.4768 |    3000 B |        3.35 |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using Lynx.UCI.Commands;
using System.Text;

namespace Lynx.Benchmark;
public class InfoCommand_CopilotRefactoring_Benchmark : BaseBenchmark
{
    public static IEnumerable<SearchResult> Data =>
    [
        new SearchResult(33592128, 27, 20, [33592128]),
        new SearchResult(33592128, 27, 20, [33591088, 33975472, 112608, 480352, 109456, 477200, 47968, 495952, 187344, 417008, 24220320, 18256304]),
        new SearchResult(33592128, 27, 20, [33592128, 33974432, 112608, 477200, 109456, 33976512, 166864, 412848, 44848, 536656, 101055424, 480352, 93856, 18312528, 24276512, 101390400, 43808, 544800, 177056, 19426624]),
        new SearchResult(33592128, 27, 20, [352512, 684368, 345472, 691648, 344336, 675376, 353536, 699696, 345488, 701104, 344336, 709312, 345344, 706368, 346384, 699152, 345376, 689824, 344336, 688656, 345344, 697856, 344336, 705168, 345344, 714496, 338192, 671632, 345248, 678128, 353552, 714080, 346512, 693136, 345376, 676416, 352528, 717120, 345472]),
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Original(SearchResult result)
    {
        return InfoCommand_Benchmark.SearchResultInfo_Original(result).Length;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int Copilot(SearchResult result)
    {
        return InfoCommand_Benchmark.SearchResultInfo_Copilot(result).Length;
    }

    public sealed class InfoCommand_Benchmark : IEngineBaseCommand
    {
        public const string Id = "info";

        public static string SearchResultInfo_Original(SearchResult searchResult)
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

            sb.Append(" pv ").AppendJoin(" ", searchResult.Moves.Select(move => move.UCIString()));

            return sb.ToString();
        }

        public static string SearchResultInfo_Copilot(SearchResult searchResult)
        {
            var sb = new StringBuilder(256);

            sb.Append(Id)
              .Append(" depth ").Append(searchResult.Depth)
              .Append(" seldepth ").Append(searchResult.DepthReached)
              .Append(" multipv 1")
              .Append(" score ").Append(searchResult.Mate == default ? "cp " + WDL.NormalizeScore(searchResult.Score) : "mate " + searchResult.Mate)
              .Append(" nodes ").Append(searchResult.Nodes)
              .Append(" nps ").Append(searchResult.NodesPerSecond)
              .Append(" time ").Append(searchResult.Time);

            if (searchResult.HashfullPermill != -1)
            {
                sb.Append(" hashfull ").Append(searchResult.HashfullPermill);
            }

            if (searchResult.WDL is not null)
            {
                sb.Append(" wdl ")
                  .Append(searchResult.WDL.Value.WDLWin).Append(' ')
                  .Append(searchResult.WDL.Value.WDLDraw).Append(' ')
                  .Append(searchResult.WDL.Value.WDLLoss);
            }

            sb.Append(" pv ");
            foreach (var move in searchResult.Moves)
            {
                sb.Append(move.UCIString()).Append(' ');
            }

            // Remove the trailing space
            if (searchResult.Moves.Length > 0)
            {
                sb.Length--;
            }

            return sb.ToString();
        }
    }
}
