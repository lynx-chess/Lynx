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
        public static SearchResult IDDFS(Position position, int minDepth, int? maxDepth, ChannelWriter<string> engineWriter, CancellationToken cancellationToken, CancellationToken absoluteCancellationToken)
        {
            int bestEvaluation = 0;
            SearchResult? searchResult = null;
            int depth = 1;
            int nodes = 0;
            var sw = new Stopwatch();
            bool isCancelled = false;

            try
            {
                var orderedMoves = new Dictionary<string, PriorityQueue<Move, int>>(10_000);
                var killerMoves = new int[2, 64];

                sw.Start();

                do
                {
                    absoluteCancellationToken.ThrowIfCancellationRequested();
                    if (depth > minDepth)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    nodes = 0;
                    (bestEvaluation, Result? bestResult) = NegaMax_AlphaBeta_Quiescence_IDDFS(position, orderedMoves, killerMoves, minDepth: minDepth, depthLimit: depth, nodes: ref nodes, plies: 0, alpha: MinValue, beta: MaxValue, cancellationToken, absoluteCancellationToken);

                    if (bestResult is not null)
                    {
                        bestResult.Moves.Reverse();
                        searchResult = new SearchResult(bestResult.Moves.FirstOrDefault(), bestEvaluation, depth, bestResult.MaxDepth ?? depth, nodes, sw.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(nodes / ((0.001 * sw.ElapsedMilliseconds) + 1), 0, Int64.MaxValue)), bestResult.Moves);

                        Task.Run(async () => await engineWriter.WriteAsync(InfoCommand.SearchResultInfo(searchResult)));
                    }
                } while (stopSearchCondition(++depth, maxDepth, bestEvaluation));
            }
            catch (OperationCanceledException)
            {
                isCancelled = true;
                Logger.Info("Search cancellation requested, best move will be returned");
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected error ocurred during the search, best move will be returned" +
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

            static bool stopSearchCondition(int depth, int? depthLimit, int bestEvaluation)
            {
                if (Math.Abs(bestEvaluation) > 0.1 * Position.CheckMateEvaluation)   // Mate detected
                {
                    return false;
                }

                if (depthLimit is not null)
                {
                    return depth <= depthLimit;
                }

                return true;
            }
        }
    }
}
