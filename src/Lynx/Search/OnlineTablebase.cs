using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using System.Diagnostics;

namespace Lynx;
public sealed partial class Engine
{
    public async Task<SearchResult?> ProbeOnlineTablebase(Position position, long[] positionHashHistory, int halfMovesWithoutCaptureOrPawnMove)
    {
        var stopWatch = Stopwatch.StartNew();

        try
        {
            var tablebaseResult = await OnlineTablebaseProber.RootSearch(position, positionHashHistory, halfMovesWithoutCaptureOrPawnMove, _searchCancellationTokenSource.Token);

            if (tablebaseResult.BestMove != 0)
            {
                var searchResult = new SearchResult(tablebaseResult.BestMove, evaluation: 0, targetDepth: 0, [tablebaseResult.BestMove], MinValue, MaxValue, mate: tablebaseResult.MateScore)
                {
                    DepthReached = 0,
                    Nodes = 666,                // In case some guis proritize the info command with biggest depth
                    Time = stopWatch.ElapsedMilliseconds,
                    NodesPerSecond = 0,
                    HashfullPermill = _tt.HashfullPermillApprox(),
                    WDL = WDL.WDLModel(
                        (int)Math.CopySign(
                            EvaluationConstants.PositiveCheckmateDetectionLimit + (EvaluationConstants.CheckmateDepthFactor * tablebaseResult.MateScore),
                            tablebaseResult.MateScore),
                        0)
                };

                await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(searchResult));
                _searchCancellationTokenSource.Cancel();

                return searchResult;
            }

            return null;
        }
        catch (OperationCanceledException) // Also catches TaskCanceledException
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
