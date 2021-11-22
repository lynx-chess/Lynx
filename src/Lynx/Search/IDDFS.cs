using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using NLog;
using System.Diagnostics;

namespace Lynx;

public sealed partial class Engine
{
    private readonly Stopwatch _stopWatch = new();
    private readonly Move[] _pVTable = new Move[((Configuration.EngineSettings.MaxDepth * Configuration.EngineSettings.MaxDepth) + Configuration.EngineSettings.MaxDepth) / 2];
    private readonly int[,] _killerMoves = new int[2, Configuration.EngineSettings.MaxDepth];
    private readonly int[,] _historyMoves = new int[12, 64];

    private int _nodes;
    private bool _isFollowingPV;
    private bool _isScoringPV;

    /// <summary>
    /// Copy of <see cref="Game.HalfMovesWithoutCaptureOrPawnMove"/>
    /// </summary>
    private int _halfMovesWithoutCaptureOrPawnMove;

    private readonly int[] _maxDepthReached = new int[Configuration.EngineSettings.MaxDepth];

    private readonly Move _defaultMove = new();

    public SearchResult IDDFS(int minDepth, int? maxDepth, int? decisionTime)
    {
        // Cleanup
        _nodes = 0;
        _isFollowingPV = false;
        _isScoringPV = false;
        _stopWatch.Reset();

        Array.Clear(_killerMoves);
        Array.Clear(_historyMoves);
        Array.Clear(_pVTable);
        Array.Clear(_maxDepthReached);

        _halfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;

        int bestEvaluation = 0;
        SearchResult? searchResult = null;
        int depth = 1;
        bool isCancelled = false;
        bool isMateDetected = false;

        try
        {
            _stopWatch.Start();

            do
            {
                _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (depth - 1 > minDepth)
                {
                    _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                _nodes = 0;
                _isFollowingPV = true;

                bestEvaluation = NegaMax(Game.CurrentPosition, minDepth, maxDepth: depth, depth: 0, alpha: MinValue, beta: MaxValue, isVerifyingNullMoveCutOff: true);

                ValidatePVTable();
                //PrintPvTable();

                var pvMoves = _pVTable.TakeWhile(m => m.EncodedMove != default).ToList();
                var maxDepthReached = _maxDepthReached.Last(item => item != default);

                int mate = default;
                var bestEvaluationAbs = Math.Abs(bestEvaluation);
                isMateDetected = bestEvaluationAbs > 0.1 * EvaluationConstants.CheckMateEvaluation;
                if (isMateDetected)
                {
                    mate = (int)Math.Ceiling(0.5 * ((EvaluationConstants.CheckMateEvaluation - bestEvaluationAbs) / Position.DepthFactor));
                    Math.CopySign(bestEvaluation, mate);
                }

                var elapsedTime = _stopWatch.ElapsedMilliseconds;
                searchResult = new SearchResult(pvMoves.FirstOrDefault(), bestEvaluation, depth, maxDepthReached, _nodes, elapsedTime, Convert.ToInt64(Math.Clamp(_nodes / ((0.001 * elapsedTime) + 1), 0, Int64.MaxValue)), pvMoves, mate);

                Task.Run(async () => await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(searchResult)));

            } while (stopSearchCondition(++depth, maxDepth, isMateDetected, _nodes, decisionTime, _stopWatch, _logger));
        }
        catch (OperationCanceledException)
        {
            isCancelled = true;
            _logger.Info($"Search cancellation requested after {_stopWatch.ElapsedMilliseconds}ms (depth {depth}, nodes {_nodes}), best move will be returned");
        }
        catch (Exception e) when (e is not AssertException)
        {
            _logger.Error($"Unexpected error ocurred during the search at depth {depth}, best move will be returned" +
                Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
        }
        finally
        {
            _stopWatch.Stop();
        }

        if (searchResult is not null)
        {
            searchResult.IsCancelled = isCancelled;
            return searchResult;
        }
        else
        {
            return new(default, bestEvaluation, depth, depth, _nodes, _stopWatch.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(_nodes / ((0.001 * _stopWatch.ElapsedMilliseconds) + 1), 0, Int64.MaxValue)), new List<Move>());
        }

        static bool stopSearchCondition(int depth, int? maxDepth, bool isMateDetected, int nodes, int? decisionTime, Stopwatch stopWatch, ILogger logger)
        {
            if (isMateDetected)
            {
                logger.Info($"Stopping at depth {depth - 1}: mate detected");
                return false;
            }

            if (maxDepth is not null)
            {
                bool shouldContinue = depth <= maxDepth;
                if (!shouldContinue)
                {
                    logger.Info($"Stopping at depth {depth - 1}: max. depth reached");
                }
                return shouldContinue;
            }

            var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            var minTimeToConsiderStopSearching = Configuration.EngineSettings.MinElapsedTimeToConsiderStopSearching;
            var decisionTimePercentageToStopSearching = Configuration.EngineSettings.DecisionTimePercentageToStopSearching;
            if (decisionTime is not null && elapsedMilliseconds > minTimeToConsiderStopSearching && elapsedMilliseconds > decisionTimePercentageToStopSearching * decisionTime)
            {
                logger.Info($"Stopping at depth {depth - 1} (nodes {nodes}): {elapsedMilliseconds} > {Configuration.EngineSettings.DecisionTimePercentageToStopSearching * decisionTime} (elapsed time > [{minTimeToConsiderStopSearching}, {decisionTimePercentageToStopSearching} * decision time])");
                return false;
            }

            return true;
        }
    }
}
