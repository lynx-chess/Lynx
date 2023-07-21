using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using System.Diagnostics;

namespace Lynx;
public sealed partial class Engine
{
    public async Task<SearchResult?> ProbeOnlineTablebase(Position position, Dictionary<long, int> positionHashHistory, int halfMovesWithoutCaptureOrPawnMove)
    {
        var stopWatch = Stopwatch.StartNew();

        try
        {
            var tablebaseResult = await OnlineTablebaseProber.RootSearch(position, positionHashHistory, halfMovesWithoutCaptureOrPawnMove, _searchCancellationTokenSource.Token);

            if (tablebaseResult.BestMove != 0)
            {
                var searchResult = new SearchResult(tablebaseResult.BestMove, Evaluation: 0, TargetDepth: 0, new List<Move>(), MinValue, MaxValue, Mate: tablebaseResult.MateScore)
                {
                    DepthReached = 0,
                    Nodes = 0,
                    Time = stopWatch.ElapsedMilliseconds,
                    NodesPerSecond = 0,
                    HashfullPermill = _transpositionTable.HashfullPermill()
                };

                Task.Run(async () => await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(searchResult))).Wait();

                return searchResult;
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            _logger.Info("Online tb probing cancellation requested after {0}ms", _stopWatch.ElapsedMilliseconds);

            return null;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Unexpected error ocurred during the online tb probing\n{0}", e.StackTrace);

            return null;
        }
        finally
        {
            _stopWatch.Stop();
        }
    }
}
