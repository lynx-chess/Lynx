using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using System.Diagnostics;
using System.Threading.Channels;

namespace Lynx.Search
{
    public sealed partial class Search
    {
        private readonly ChannelWriter<string> _engineWriter;
        private readonly Dictionary<long, int> _positionHistory;
        private readonly int _minDepth;
        private readonly Move[] _pVTable;
        private readonly int[,] _killerMoves;

        private int _movesWithoutCaptureOrPawnMove;
        private int _nodes;
        private bool _isFollowingPV;
        private bool _isScoringPV;

        private readonly CancellationToken _cancellationToken;
        private readonly CancellationToken _absoluteCancellationToken;

        public Search(ChannelWriter<string> engineWriter, Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove, int minDepth, CancellationToken cancellationToken, CancellationToken absoluteCancellationToken)
        {
            _engineWriter = engineWriter;
            _positionHistory = positionHistory;
            _movesWithoutCaptureOrPawnMove = movesWithoutCaptureOrPawnMove;
            _minDepth = minDepth;
            _cancellationToken = cancellationToken;
            _absoluteCancellationToken = absoluteCancellationToken;

            _nodes = 0;
            _killerMoves = new int[2, EvaluationConstants.MaxPlies];

            var maxPossibleDepth = Configuration.EngineSettings.MaxDepth;
            _pVTable = new Move[((maxPossibleDepth * maxPossibleDepth) + maxPossibleDepth) / 2];
        }

        public SearchResult IDDFS(Position position, int? maxDepth, int? decisionTime)
        {
            int bestEvaluation = 0;
            SearchResult? searchResult = null;
            int depth = 1;
            var sw = new Stopwatch();
            bool isCancelled = false;

            try
            {
                sw.Start();

                do
                {
                    _absoluteCancellationToken.ThrowIfCancellationRequested();
                    if (depth - 1 > _minDepth)
                    {
                        _cancellationToken.ThrowIfCancellationRequested();
                    }
                    _nodes = 0;
                    _isFollowingPV = true;

                    (bestEvaluation, int maxDepthReached) = NegaMax(position, depthLimit: depth, depth: 0, alpha: MinValue, beta: MaxValue);

                    PrintPvTable();

                    var pvMoves = _pVTable.TakeWhile(m => m.EncodedMove != default).ToList();
                    searchResult = new SearchResult(pvMoves.FirstOrDefault(), bestEvaluation, depth, maxDepthReached, _nodes, sw.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(_nodes / ((0.001 * sw.ElapsedMilliseconds) + 1), 0, Int64.MaxValue)), pvMoves);

                    Task.Run(async () => await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(searchResult)));

                } while (stopSearchCondition(++depth, maxDepth, bestEvaluation, _nodes, decisionTime, sw));
            }
            catch (OperationCanceledException)
            {
                isCancelled = true;
                _logger.Info($"Search cancellation requested after {sw.ElapsedMilliseconds}ms (depth {depth}, nodes {_nodes}), best move will be returned");
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
                return new(default, bestEvaluation, depth, depth, _nodes, sw.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(_nodes / ((0.001 * sw.ElapsedMilliseconds) + 1), 0, Int64.MaxValue)), new List<Move>());
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
