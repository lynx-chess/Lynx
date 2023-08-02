using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using NLog;
using System.Diagnostics;

namespace Lynx;

public sealed partial class Engine
{
    private readonly Stopwatch _stopWatch = new();
    private readonly Move[] _pVTable = new Move[Configuration.EngineSettings.MaxDepth * (Configuration.EngineSettings.MaxDepth + 1) / 2];
    private readonly int[,] _killerMoves = new int[2, Configuration.EngineSettings.MaxDepth];
    private readonly int[,] _historyMoves = new int[12, 64];
    private readonly int[] _maxDepthReached = new int[Constants.AbsoluteMaxDepth];
    private TranspositionTable _transpositionTable = Configuration.EngineSettings.TranspositionTableEnabled
        ? new TranspositionTableElement[TranspositionTableExtensions.TranspositionTableArrayLength]
        : Array.Empty<TranspositionTableElement>();

    private int _nodes;
    private bool _isFollowingPV;
    private bool _isScoringPV;

    private SearchResult? _previousSearchResult;
    private readonly int[,] _previousKillerMoves = new int[2, Configuration.EngineSettings.MaxDepth];

    /// <summary>
    /// Copy of <see cref="Game.HalfMovesWithoutCaptureOrPawnMove"/>
    /// </summary>
    private int _halfMovesWithoutCaptureOrPawnMove;

    private readonly Move _defaultMove = default;

    public async Task<SearchResult?> IDDFS(int minDepth, int? maxDepth, int? decisionTime)
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
            && _previousSearchResult?.Moves.Count > 2
            && _previousSearchResult.BestMove != default
            && Game.MoveHistory[^2] == _previousSearchResult.Moves[0]
            && Game.MoveHistory[^1] == _previousSearchResult.Moves[1])
        {
            _logger.Debug("Ponder hit");

            lastSearchResult = new SearchResult(_previousSearchResult)
            {
                HashfullPermill = _transpositionTable.HashfullPermill()
            };

            Array.Copy(_previousSearchResult.Moves.ToArray(), 2, _pVTable, 0, _previousSearchResult.Moves.Count - 2);

            await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(lastSearchResult));

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
                if (minDepth == maxDepth    // go depth n commands
                    || depth - 1 > minDepth)
                {
                    _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                _nodes = 0;

                AspirationWindows_SearchAgain:

                _isFollowingPV = true;
                bestEvaluation = NegaMax(Game.CurrentPosition, minDepth, targetDepth: depth, ply: 0, alpha, beta, isVerifyingNullMoveCutOff: true);

                var bestEvaluationAbs = Math.Abs(bestEvaluation);
                isMateDetected = bestEvaluationAbs > EvaluationConstants.PositiveCheckmateDetectionLimit;

                // 🔍 Aspiration Windows
                if (!isMateDetected && ((bestEvaluation <= alpha) || (bestEvaluation >= beta)))
                {
                    alpha = MinValue;   // We fell outside the window, so try again with a
                    beta = MaxValue;    // full-width window (and the same depth).

                    _logger.Debug("Outside of aspiration window (depth {0}, nodes {1}): eval {2}, alpha {3}, beta {4}", depth, _nodes, bestEvaluation, alpha, beta);
                    goto AspirationWindows_SearchAgain;
                }

                alpha = bestEvaluation - Configuration.EngineSettings.AspirationWindowAlpha;
                beta = bestEvaluation + Configuration.EngineSettings.AspirationWindowBeta;

                //PrintPvTable();
                ValidatePVTable();

                var pvMoves = _pVTable.TakeWhile(m => m != default).ToList();
                var maxDepthReached = _maxDepthReached.LastOrDefault(item => item != default);

                int mate = default;
                if (isMateDetected)
                {
                    mate = Utils.CalculateMateInX(bestEvaluation, bestEvaluationAbs);
                }

                var elapsedTime = _stopWatch.ElapsedMilliseconds;

                _previousSearchResult = lastSearchResult;
                lastSearchResult = new SearchResult(pvMoves.FirstOrDefault(), bestEvaluation, depth, pvMoves, alpha, beta, mate)
                {
                    DepthReached = maxDepthReached,
                    Nodes = _nodes,
                    Time = elapsedTime,
                    NodesPerSecond = Convert.ToInt64(Math.Clamp(_nodes / ((0.001 * elapsedTime) + 1), 0, long.MaxValue)),
                    HashfullPermill = _transpositionTable.HashfullPermill()
                };

                await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(lastSearchResult));

                Array.Copy(_killerMoves, _previousKillerMoves, _killerMoves.Length);

            } while (stopSearchCondition(++depth, maxDepth, isMateDetected, _nodes, decisionTime, _stopWatch, _logger));
        }
        catch (OperationCanceledException)
        {
            isCancelled = true;
            _logger.Info("Search cancellation requested after {0}ms (depth {1}, nodes {2}), best move will be returned", _stopWatch.ElapsedMilliseconds, depth, _nodes);

            for (int i = 0; i < lastSearchResult?.Moves.Count; ++i)
            {
                _pVTable[i] = lastSearchResult.Moves[i];
            }
        }
        catch (Exception e) when (e is not AssertException)
        {
            _logger.Error(e, "Unexpected error ocurred during the search at depth {0}, best move will be returned\n{1}", depth, e.StackTrace);
        }
        finally
        {
            _stopWatch.Stop();
        }

        var finalSearchResult = lastSearchResult ??= new(default, bestEvaluation, depth, new List<Move>(), alpha, beta);

        finalSearchResult.IsCancelled = isCancelled;
        finalSearchResult.DepthReached = Math.Max(finalSearchResult.DepthReached, _maxDepthReached.LastOrDefault(item => item != default));
        finalSearchResult.Nodes = _nodes;
        finalSearchResult.Time = _stopWatch.ElapsedMilliseconds;
        finalSearchResult.NodesPerSecond = Convert.ToInt64(Math.Clamp(_nodes / ((0.001 * _stopWatch.ElapsedMilliseconds) + 1), 0, long.MaxValue));
        finalSearchResult.HashfullPermill = _transpositionTable.HashfullPermill();

        if (isMateDetected && finalSearchResult.Mate + _halfMovesWithoutCaptureOrPawnMove < 96)
        {
            _logger.Info("Engine search found a short enough mate, cancelling online tb probing if still active");
            _searchCancellationTokenSource.Cancel();
        }

        await _engineWriter.WriteAsync(InfoCommand.SearchResultInfo(finalSearchResult));

        return finalSearchResult;

        static bool stopSearchCondition(int depth, int? maxDepth, bool isMateDetected, int nodes, int? decisionTime, Stopwatch stopWatch, Logger logger)
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
                    logger.Info("Stopping at depth {0}: max. depth reached", depth - 1);
                }
                return shouldContinue;
            }

            var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            var minTimeToConsiderStopSearching = Configuration.EngineSettings.MinElapsedTimeToConsiderStopSearching;
            var decisionTimePercentageToStopSearching = Configuration.EngineSettings.DecisionTimePercentageToStopSearching;
            if (decisionTime is not null && elapsedMilliseconds > minTimeToConsiderStopSearching && elapsedMilliseconds > decisionTimePercentageToStopSearching * decisionTime)
            {
                logger.Info("Stopping at depth {0} (nodes {1}): {2} > {3} (elapsed time > [{4}, {5} * decision time])",
                    depth - 1, nodes, elapsedMilliseconds, decisionTimePercentageToStopSearching * decisionTime, minTimeToConsiderStopSearching, decisionTimePercentageToStopSearching);
                return false;
            }

            return true;
        }
    }
}
