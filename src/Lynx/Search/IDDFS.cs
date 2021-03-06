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
    private readonly int[] _maxDepthReached = new int[Configuration.EngineSettings.MaxDepth];

    private int _nodes;
    private bool _isFollowingPV;
    private bool _isScoringPV;

    private SearchResult? _previousSearchResult;
    private readonly int[,] _previousKillerMoves = new int[2, Configuration.EngineSettings.MaxDepth];

    /// <summary>
    /// Copy of <see cref="Game.HalfMovesWithoutCaptureOrPawnMove"/>
    /// </summary>
    private int _halfMovesWithoutCaptureOrPawnMove;

    private readonly Move _defaultMove = new();

    public SearchResult IDDFS(int minDepth, int? maxDepth, int? decisionTime)
    {
        // Cleanup
        _nodes = 0;
        _isFollowingPV = false;
        _isScoringPV = false;
        _stopWatch.Reset();

        Array.Clear(_pVTable);
        Array.Clear(_maxDepthReached);

        _halfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;

        int bestEvaluation = 0;
        int alpha = MinValue;
        int beta = MaxValue;
        SearchResult? lastSearchResult = null;
        int depth = 1;
        bool isCancelled = false;
        bool isMateDetected = false;

        if (Game.MoveHistory.Count >= 2
            && _previousSearchResult?.Moves.Count >= 2
            && _previousSearchResult.BestMove.EncodedMove != default
            && Game.MoveHistory[^2].EncodedMove == _previousSearchResult.Moves[0].EncodedMove
            && Game.MoveHistory[^1].EncodedMove == _previousSearchResult.Moves[1].EncodedMove)
        {
            _logger.Debug("Ponder hit");

            lastSearchResult = new SearchResult(_previousSearchResult);
            Array.Copy(_previousSearchResult.Moves.ToArray(), 2, _pVTable, 0, _previousSearchResult.Moves.Count - 2);

            Task.Run(async () => await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(lastSearchResult)));

            for (int d = 1; d < Configuration.EngineSettings.MaxDepth - 2; ++d)
            {
                _killerMoves[0, d] = _previousKillerMoves[0, d + 2];
                _killerMoves[1, d] = _previousKillerMoves[1, d + 2];
            }

            depth = lastSearchResult.TargetDepth + 1;
            alpha = lastSearchResult.Alpha;
            beta = lastSearchResult.Beta;
        }
        else
        {
            Array.Clear(_killerMoves);
            Array.Clear(_historyMoves);
        }

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

                AspirationWindows_SearchAgain:

                _isFollowingPV = true;
                bestEvaluation = NegaMax(Game.CurrentPosition, minDepth, maxDepth: depth, depth: 0, alpha, beta, isVerifyingNullMoveCutOff: true);

                var bestEvaluationAbs = Math.Abs(bestEvaluation);
                isMateDetected = bestEvaluationAbs > 0.1 * EvaluationConstants.CheckMateEvaluation;

                // 🔍 Aspiration Windows
                if (!isMateDetected && ((bestEvaluation <= alpha) || (bestEvaluation >= beta)))
                {
                    alpha = MinValue;   // We fell outside the window, so try again with a
                    beta = MaxValue;    // full-width window (and the same depth).

                    _logger.Debug($"Outside of aspiration window (depth {depth}, nodes {_nodes}): eval {bestEvaluation}, alpha {alpha}, beta {beta}");
                    goto AspirationWindows_SearchAgain;
                }

                alpha = bestEvaluation - Configuration.EngineSettings.AspirationWindowAlpha;
                beta = bestEvaluation + Configuration.EngineSettings.AspirationWindowBeta;

                //PrintPvTable();
                ValidatePVTable();

                var pvMoves = _pVTable.TakeWhile(m => m.EncodedMove != default).ToList();
                var maxDepthReached = _maxDepthReached.LastOrDefault(item => item != default);

                int mate = default;
                if (isMateDetected)
                {
                    mate = (int)Math.Ceiling(0.5 * ((EvaluationConstants.CheckMateEvaluation - bestEvaluationAbs) / Position.DepthFactor));
                    mate = (int)Math.CopySign(mate, bestEvaluation);
                }

                var elapsedTime = _stopWatch.ElapsedMilliseconds;

                _previousSearchResult = lastSearchResult;
                lastSearchResult = new SearchResult(pvMoves.FirstOrDefault(), bestEvaluation, depth, maxDepthReached, _nodes, elapsedTime, Convert.ToInt64(Math.Clamp(_nodes / ((0.001 * elapsedTime) + 1), 0, Int64.MaxValue)), pvMoves, alpha, beta, mate);

                Task.Run(async () => await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(lastSearchResult)));

                Array.Copy(_killerMoves, _previousKillerMoves, _killerMoves.Length);

            } while (stopSearchCondition(++depth, maxDepth, isMateDetected, _nodes, decisionTime, _stopWatch, _logger));
        }
        catch (OperationCanceledException)
        {
            isCancelled = true;
            _logger.Info($"Search cancellation requested after {_stopWatch.ElapsedMilliseconds}ms (depth {depth}, nodes {_nodes}), best move will be returned");

            for (int i = 0; i < lastSearchResult?.Moves.Count; ++i)
            {
                _pVTable[i] = lastSearchResult.Moves[i];
            }
        }
        catch (Exception e) when (e is not AssertException)
        {
            _logger.Error(e, $"Unexpected error ocurred during the search at depth {depth}, best move will be returned");
        }
        finally
        {
            _stopWatch.Stop();
        }

        if (lastSearchResult is not null)
        {
            lastSearchResult.IsCancelled = isCancelled;
            return lastSearchResult;
        }
        else
        {
            return new(default, bestEvaluation, depth, depth, _nodes, _stopWatch.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(_nodes / ((0.001 * _stopWatch.ElapsedMilliseconds) + 1), 0, Int64.MaxValue)), new List<Move>(), alpha, beta);
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
