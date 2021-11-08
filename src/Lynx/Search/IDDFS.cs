using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        public static SearchResult IDDFS(Position position, Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove, int minDepth, int? maxDepth, int? decisionTime, ChannelWriter<string> engineWriter, CancellationToken cancellationToken, CancellationToken absoluteCancellationToken)
        {
            int bestEvaluation = 0;
            SearchResult? searchResult = null;
            int depth = 1;
            int nodes = 0;
            var sw = new Stopwatch();
            bool isCancelled = false;

            try
            {
                var maxPossibleDepth = Configuration.EngineSettings.MaxDepth;
                var pvTable = new Move[((maxPossibleDepth * maxPossibleDepth) + maxPossibleDepth) / 2];
                var killerMoves = new int[2, EvaluationConstants.MaxPlies];

                sw.Start();

                do
                {
                    absoluteCancellationToken.ThrowIfCancellationRequested();
                    if (depth - 1 > minDepth)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    nodes = 0;

                    (bestEvaluation, int maxDepthReached) = NegaMax(position, positionHistory, movesWithoutCaptureOrPawnMove, pvTable, pvIndex: PVTable.Indexes[0], killerMoves, minDepth: minDepth, depthLimit: depth, nodes: ref nodes, plies: 0, alpha: MinValue, beta: MaxValue, cancellationToken, absoluteCancellationToken);

                    var pvMoves = pvTable.TakeWhile(m => m.EncodedMove != default).ToList();
                    searchResult = new SearchResult(pvMoves.FirstOrDefault(), bestEvaluation, depth, maxDepthReached, nodes, sw.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(nodes / ((0.001 * sw.ElapsedMilliseconds) + 1), 0, Int64.MaxValue)), pvMoves);

                    Task.Run(async () => await engineWriter.WriteAsync(InfoCommand.SearchResultInfo(searchResult)));

                } while (stopSearchCondition(++depth, maxDepth, bestEvaluation, nodes, decisionTime, sw));
            }
            catch (OperationCanceledException)
            {
                isCancelled = true;
                _logger.Info($"Search cancellation requested after {sw.ElapsedMilliseconds}ms (depth {depth}, nodes {nodes}), best move will be returned");
            }
            catch (Exception e)
            {
                _logger.Error($"Unexpected error ocurred during the search at depth {depth}, best move will be returned" +
                    Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
            }
            finally
            {
                sw.Stop();
            }

            if (searchResult is not null)
            {
                searchResult.IsCancelled = isCancelled;
                return searchResult;
            }
            else
            {
                return new(default, bestEvaluation, depth, depth, nodes, sw.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(nodes / ((0.001 * sw.ElapsedMilliseconds) + 1), 0, Int64.MaxValue)), new List<Move>());
            }

            static bool stopSearchCondition(int depth, int? maxDepth, int bestEvaluation, int nodes, int? decisionTime, Stopwatch stopWatch)
            {
                if (Math.Abs(bestEvaluation) > 0.1 * Position.CheckMateEvaluation)
                {
                    _logger.Info($"Stopping at depth {depth - 1}: mate detected");
                    return false;
                }

                if (maxDepth is not null)
                {
                    bool shouldContinue = depth <= maxDepth;
                    if (!shouldContinue)
                    {
                        _logger.Info($"Stopping at depth {depth - 1}: max. depth reached");
                    }
                    return shouldContinue;
                }

                var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
                var minTimeToConsiderStopSearching = Configuration.EngineSettings.MinElapsedTimeToConsiderStopSearching;
                var decisionTimePercentageToStopSearching = Configuration.EngineSettings.DecisionTimePercentageToStopSearching;
                if (decisionTime is not null && elapsedMilliseconds > minTimeToConsiderStopSearching && elapsedMilliseconds > decisionTimePercentageToStopSearching * decisionTime)
                {
                    _logger.Info($"Stopping at depth {depth - 1} (nodes {nodes}): {elapsedMilliseconds} > {Configuration.EngineSettings.DecisionTimePercentageToStopSearching * decisionTime} (elapsed time > [{minTimeToConsiderStopSearching}, {decisionTimePercentageToStopSearching} * decision time])");
                    return false;
                }

                return true;
            }
        }
    }
}
