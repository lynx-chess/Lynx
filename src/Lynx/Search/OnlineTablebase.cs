using Lynx.Model;
using System.Diagnostics;

using static Lynx.Model.TranspositionTable;

namespace Lynx;
public sealed partial class Engine
{
    public async Task<SearchResult?> ProbeOnlineTablebase(Position position, ulong[] positionHashHistory, int halfMovesWithoutCaptureOrPawnMove)
    {
        var stopWatch = Stopwatch.StartNew();

        try
        {
            var tablebaseResult = await OnlineTablebaseProber.RootSearch(position, positionHashHistory, halfMovesWithoutCaptureOrPawnMove, _searchCancellationTokenSource.Token);

            if (tablebaseResult.BestMove != 0)
            {
                var elapsedSeconds = Utils.CalculateElapsedSeconds(stopWatch);

                var searchResult = new SearchResult(tablebaseResult.BestMove, score: 0, targetDepth: 0, [tablebaseResult.BestMove], mate: tablebaseResult.MateScore)
                {
                    DepthReached = 0,
                    Nodes = 666,                // In case some guis proritize the info command with biggest depth
                    Time = Utils.CalculateUCITime(elapsedSeconds),
                    NodesPerSecond = 0,
                    HashfullPermill = HashfullPermillApprox(),
                    WDL = WDL.WDLModel(
                        (int)Math.CopySign(
                            EvaluationConstants.PositiveCheckmateDetectionLimit + (EvaluationConstants.CheckmateDepthFactor * tablebaseResult.MateScore),
                            tablebaseResult.MateScore),
                        0)
                };

                await _engineWriter.WriteAsync(searchResult);
                await _searchCancellationTokenSource.CancelAsync();

                return searchResult;
            }

            return null;
        }
        catch (OperationCanceledException) // Also catches TaskCanceledException
        {
#pragma warning disable S6667 // Logging in a catch clause should pass the caught exception as a parameter. - expected
            _logger.Info("Online tb probing cancellation requested after {0}ms", _stopWatch.ElapsedMilliseconds);
#pragma warning restore S6667 // Logging in a catch clause should pass the caught exception as a parameter.

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
