using Lynx.Model;
using System.Diagnostics;

namespace Lynx;
public sealed partial class Engine
{
    public async Task<SearchResult?> ProbeOnlineTablebase(Position position, ulong[] positionHashHistory, int halfMovesWithoutCaptureOrPawnMove, CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();

        try
        {
            var tablebaseResult = await OnlineTablebaseProber.RootSearch(position, positionHashHistory, halfMovesWithoutCaptureOrPawnMove, cancellationToken);

            if (tablebaseResult.BestMove != 0)
            {
                var elapsedSeconds = Utils.CalculateElapsedSeconds(stopWatch);

                var searchResult = new SearchResult(
#if MULTITHREAD_DEBUG
                _id,
#endif
                    tablebaseResult.BestMove, score: 0, targetDepth: 0, [tablebaseResult.BestMove], mate: tablebaseResult.MateScore)
                {
                    DepthReached = 0,
                    Depth = 666,                // In case some guis prioritize the info command with biggest depth
                    Time = Utils.CalculateUCITime(elapsedSeconds),
                    NodesPerSecond = 0,
                    HashfullPermill = _tt.HashfullPermillApprox(),
                    WDL = WDL.WDLModel(
                        (int)Math.CopySign(
                            EvaluationConstants.PositiveCheckmateDetectionLimit + tablebaseResult.MateScore,
                            tablebaseResult.MateScore),
                        0)
                };

                await _engineWriter.WriteAsync(searchResult, cancellationToken);
                //await _searchCancellationTokenSource.CancelAsync();   // TODO revisit

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
            _logger.Error(e, "Unexpected error occurred during the online tb probing\n{0}", e.StackTrace);

            return null;
        }
        finally
        {
            _stopWatch.Stop();
        }
    }
}
