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
                var orderedMoves = new Dictionary<long, PriorityQueue<Move, int>>(10_000);
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
                    (bestEvaluation, Result? bestResult) = NegaMax(position, positionHistory, movesWithoutCaptureOrPawnMove, orderedMoves, killerMoves, minDepth: minDepth, depthLimit: depth, nodes: ref nodes, plies: 0, alpha: MinValue, beta: MaxValue, cancellationToken, absoluteCancellationToken);

                    if (bestResult is not null)
                    {
                        bestResult.Moves.Reverse();
                        searchResult = new SearchResult(bestResult.Moves.FirstOrDefault(), bestEvaluation, depth, bestResult.MaxDepth ?? depth, nodes, sw.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(nodes / ((0.001 * sw.ElapsedMilliseconds) + 1), 0, Int64.MaxValue)), bestResult.Moves);

                        Task.Run(async () => await engineWriter.WriteAsync(InfoCommand.SearchResultInfo(searchResult)));
                    }
                } while (stopSearchCondition(++depth, maxDepth, bestEvaluation, decisionTime, sw));
            }
            catch (OperationCanceledException)
            {
                isCancelled = true;
                _logger.Info($"Search cancellation requested after {sw.ElapsedMilliseconds}ms, best move will be returned");
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

            static bool stopSearchCondition(int depth, int? maxDepth, int bestEvaluation, int? decisionTime, Stopwatch stopWatch)
            {
                if (Math.Abs(bestEvaluation) > 0.1 * Position.CheckMateEvaluation)
                {
                    _logger.Info($"Stopping at depth {depth - 1}: mate detected");
                    return false;
                }

                if (maxDepth is not null)
                {
                    _logger.Info($"Stopping at depth {depth - 1}: max. depth reached");
                    return depth <= maxDepth;
                }

                var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
                var minTimeToConsiderStopSearching = Configuration.EngineSettings.MinElapsedTimeToConsiderStopSearching;
                var decisionTimePercentageToStopSearching = Configuration.EngineSettings.DecisionTimePercentageToStopSearching;
                if (decisionTime is not null && elapsedMilliseconds > minTimeToConsiderStopSearching && elapsedMilliseconds > decisionTimePercentageToStopSearching * decisionTime)
                {
                    _logger.Info($"Stopping at depth {depth - 1}: {elapsedMilliseconds} > {Configuration.EngineSettings.DecisionTimePercentageToStopSearching * decisionTime} (elapsed time > [{minTimeToConsiderStopSearching}, {decisionTimePercentageToStopSearching} * decision time])");
                    return false;
                }

                return true;
            }
        }
    }
}
