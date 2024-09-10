using BenchmarkDotNet.Attributes;
using Lynx.Model;
using Lynx.UCI.Commands;
using System.Text;

namespace Lynx.Benchmark;
public class InfoCommand_CopilotRefactoring_Benchmark : BaseBenchmark
{
    public static IEnumerable<SearchResult> Data => new[] {
        new SearchResult(33592128, 27, 20, [33592128], -500, +500 ),
        new SearchResult(33592128, 27, 20, [33591088, 33975472, 112608, 480352, 109456, 477200, 47968, 495952, 187344, 417008, 24220320, 18256304], -500, +500 ),
        new SearchResult(33592128, 27, 20, [33592128, 33974432, 112608, 477200, 109456, 33976512, 166864, 412848, 44848, 536656, 101055424, 480352, 93856, 18312528, 24276512, 101390400, 43808, 544800, 177056, 19426624], -500, +500 ),
        new SearchResult(33592128, 27, 20, [352512, 684368, 345472, 691648, 344336, 675376, 353536, 699696, 345488, 701104, 344336, 709312, 345344, 706368, 346384, 699152, 345376, 689824, 344336, 688656, 345344, 697856, 344336, 705168, 345344, 714496, 338192, 671632, 345248, 678128, 353552, 714080, 346512, 693136, 345376, 676416, 352528, 717120, 345472], -500, +500),
        };

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

    public sealed class InfoCommand_Benchmark : EngineBaseCommand
    {
        public const string Id = "info";

        public static string SearchResultInfo_Original(SearchResult searchResult)
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

        public static string SearchResultInfo_Copilot(SearchResult searchResult)
        {
            var sb = new StringBuilder(256);

            sb.Append(Id)
              .Append(" depth ").Append(searchResult.Depth)
              .Append(" seldepth ").Append(searchResult.DepthReached)
              .Append(" multipv 1")
              .Append(" score ").Append(searchResult.Mate == default ? "cp " + WDL.NormalizeScore(searchResult.Evaluation) : "mate " + searchResult.Mate)
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
            if (searchResult.Moves.Count > 0)
            {
                sb.Length--;
            }

            return sb.ToString();
        }
    }
}
