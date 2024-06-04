using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Lynx;

public sealed partial class Engine
{
    private readonly Stopwatch _stopWatch = new();
    private readonly Move[] _pVTable = new Move[Configuration.EngineSettings.MaxDepth * (Configuration.EngineSettings.MaxDepth + 1) / 2];

    /// <summary>
    /// 3x<see cref="Configuration.EngineSettings.MaxDepth"/>
    /// </summary>
    private readonly int[][] _killerMoves =
    [
        new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin],
        new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin],
        new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin]
    ];

    /// <summary>
    /// 12x64
    /// </summary>
    private readonly int[][] _quietHistory;

    /// <summary>
    /// 12x64x12
    /// </summary>
    private readonly int[][][] _captureHistory;

    private readonly int[] _maxDepthReached = new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin];
    private TranspositionTable _tt = [];
    private int _ttMask;

    private int _nodes;
    private bool _isFollowingPV;
    private bool _isScoringPV;

    private SearchResult? _previousSearchResult;

    private readonly Move _defaultMove = default;

    /// <summary>
    /// IDDFs search
    /// </summary>
    /// <param name="maxDepth"></param>
    /// <param name="softLimitTimeBound"></param>
    /// <returns>Not null <see cref="SearchResult"/>, although made nullable in order to match online tb probing signature</returns>
    public SearchResult IDDFS(int maxDepth, int softLimitTimeBound)
    {
        // Cleanup
        _nodes = 0;
        _isFollowingPV = false;
        _isScoringPV = false;
        _stopWatch.Reset();

        Array.Clear(_pVTable);
        Array.Clear(_maxDepthReached);

        int bestEvaluation = 0;
        int alpha = MinValue;
        int beta = MaxValue;
        SearchResult? lastSearchResult = null;
        int depth = 1;
        bool isCancelled = false;
        bool isMateDetected = false;
        Move firstLegalMove = default;

        try
        {
            _stopWatch.Start();

            if (OnlyOneLegalMove(ref firstLegalMove, out var onlyOneLegalMoveSearchResult))
            {
                _engineWriter.TryWrite(InfoCommand.SearchResultInfo(onlyOneLegalMoveSearchResult));

                return onlyOneLegalMoveSearchResult;
            }

            Debug.Assert(_killerMoves.Length == 3);
            Array.Clear(_killerMoves[0]);
            Array.Clear(_killerMoves[1]);
            Array.Clear(_killerMoves[2]);
            // Not clearing _quietHistory on purpose
            // Not clearing _captureHistory on purpose

            if (lastSearchResult is not null)
            {
                _engineWriter.TryWrite(InfoCommand.SearchResultInfo(lastSearchResult));
            }

            do
            {
                _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _nodes = 0;

                if (depth < Configuration.EngineSettings.AspirationWindow_MinDepth || lastSearchResult?.Evaluation is null)
                {
                    bestEvaluation = NegaMax(depth: depth, ply: 0, alpha, beta);
                }
                else
                {
                    // 🔍 Aspiration Windows
                    // Reduction implementation taken from Simbelmyne

                    var window = Configuration.EngineSettings.AspirationWindow_Delta;
                    int reduction = 0;

                    alpha = Math.Max(MinValue, lastSearchResult.Evaluation - window);
                    beta = Math.Min(MaxValue, lastSearchResult.Evaluation + window);

                    while (true)
                    {
                        _isFollowingPV = true;
                        bestEvaluation = NegaMax(depth: depth - reduction, ply: 0, alpha, beta);

                        if (alpha < bestEvaluation && beta > bestEvaluation)
                        {
                            break;
                        }

                        _logger.Debug("Eval ({0}) outside of aspiration window [{1}, {2}] (depth {3}, nodes {4})", bestEvaluation, alpha, beta, depth, _nodes);

                        window += window >> 1;   // window / 2

                        // Depth change: https://github.com/lynx-chess/Lynx/pull/440
                        if (alpha >= bestEvaluation)     // Fail low
                        {
                            alpha = Math.Max(bestEvaluation - window, MinValue);
                            beta = (alpha + beta) >> 1;  // (alpha + beta) / 2
                            reduction = 0;
                        }
                        else if (beta <= bestEvaluation)     // Fail high
                        {
                            beta = Math.Min(bestEvaluation + window, MaxValue);
                            ++reduction;
                        }
                    }
                }

                //PrintPvTable(depth: depth);
                ValidatePVTable();

                var bestEvaluationAbs = Math.Abs(bestEvaluation);
                isMateDetected = bestEvaluationAbs > EvaluationConstants.PositiveCheckmateDetectionLimit;

                lastSearchResult = UpdateLastSearchResult(lastSearchResult, bestEvaluation, alpha, beta, depth, isMateDetected, bestEvaluationAbs);

                _engineWriter.TryWrite(InfoCommand.SearchResultInfo(lastSearchResult));
            } while (StopSearchCondition(++depth, maxDepth, isMateDetected, softLimitTimeBound));
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
            _logger.Error(e, "Unexpected error ocurred during the search of position {0} at depth {1}, best move will be returned\n{2}", Game.PositionBeforeLastSearch.FEN(), depth, e.StackTrace);
        }
        finally
        {
            _stopWatch.Stop();
        }

        var finalSearchResult = GenerateFinalSearchResult(lastSearchResult, bestEvaluation, alpha, beta, depth, firstLegalMove, isCancelled);

        if (isMateDetected && finalSearchResult.Mate + Game.HalfMovesWithoutCaptureOrPawnMove < 96)
        {
            _logger.Info("Engine search found a short enough mate, cancelling online tb probing if still active");
            _searchCancellationTokenSource.Cancel();
        }

        _engineWriter.TryWrite(InfoCommand.SearchResultInfo(finalSearchResult));

        return finalSearchResult;
    }

    private bool StopSearchCondition(int depth, int maxDepth, bool isMateDetected, int softLimitTimeBound)
    {
        if (isMateDetected)
        {
            _logger.Info("Stopping at depth {0}: mate detected", depth - 1);
            return false;
        }

        if (depth >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth reached: {0}", Configuration.EngineSettings.MaxDepth);
            return false;
        }

        if (maxDepth > 0)
        {
            var shouldContinue = depth <= maxDepth;

            if (!shouldContinue)
            {
                _logger.Info("Stopping at depth {0}: max. depth reached", depth - 1);
            }

            return shouldContinue;
        }

        var elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;

        if (elapsedMilliseconds > softLimitTimeBound)
        {
            _logger.Info("Stopping at depth {0} (nodes {1}): {2}ms > {3}ms", depth - 1, _nodes, elapsedMilliseconds, softLimitTimeBound);
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool OnlyOneLegalMove(ref Move firstLegalMove, [NotNullWhen(true)] out SearchResult? result)
    {
        bool onlyOneLegalMove = false;

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        foreach (var move in MoveGenerator.GenerateAllMoves(Game.CurrentPosition, moves))
        {
            var gameState = Game.CurrentPosition.MakeMove(move);
            bool isPositionValid = Game.CurrentPosition.WasProduceByAValidMove();
            Game.CurrentPosition.UnmakeMove(move, gameState);

            if (isPositionValid)
            {
                // We save the first legal move and check if there's at least another one
                if (firstLegalMove == default)
                {
                    firstLegalMove = move;
                    onlyOneLegalMove = true;
                }
                // If there's a second legal move, we exit and let the search continue
                else
                {
                    onlyOneLegalMove = false;
                    break;
                }
            }
        }

        // Detect if there was only one legal move
        if (onlyOneLegalMove)
        {
            _logger.Debug("One single move found");
            var elapsedTime = _stopWatch.ElapsedMilliseconds;

            // We don't have or need any eval, and we don't want to return 0 or a negative eval that
            // could make the GUI resign or take a draw from this position.
            // Since this only happens in root, we don't really care about being more precise for raising
            // alphas or betas of parent moves, so let's just return +-2 pawns depending on the side to move
            var eval = Game.CurrentPosition.Side == Side.White
                ? +EvaluationConstants.SingleMoveEvaluation
                : -EvaluationConstants.SingleMoveEvaluation;

            result = new SearchResult(firstLegalMove, eval, 0, [firstLegalMove], MinValue, MaxValue)
            {
                DepthReached = 0,
                Nodes = 0,
                Time = elapsedTime,
                NodesPerSecond = 0
            };

            return true;
        }

        result = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SearchResult UpdateLastSearchResult(SearchResult? lastSearchResult,
        int bestEvaluation, int alpha, int beta, int depth, bool isMateDetected, int bestEvaluationAbs)
    {
        var pvMoves = _pVTable.TakeWhile(m => m != default).ToList();
        var maxDepthReached = _maxDepthReached.LastOrDefault(item => item != default);

        int mate = default;
        if (isMateDetected)
        {
            mate = Utils.CalculateMateInX(bestEvaluation, bestEvaluationAbs);
        }

        var elapsedTime = _stopWatch.ElapsedMilliseconds;

        _previousSearchResult = lastSearchResult;
        return new SearchResult(pvMoves.FirstOrDefault(), bestEvaluation, depth, pvMoves, alpha, beta, mate)
        {
            DepthReached = maxDepthReached,
            Nodes = _nodes,
            Time = elapsedTime,
            NodesPerSecond = Utils.CalculateNps(_nodes, elapsedTime)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SearchResult GenerateFinalSearchResult(SearchResult? lastSearchResult,
        int bestEvaluation, int alpha, int beta, int depth, Move firstLegalMove, bool isCancelled)
    {
        SearchResult finalSearchResult;
        if (lastSearchResult is null)
        {
            // In the event of a quick ponderhit/stop while pondering because the opponent moved quickly, we don't want no warning triggered here
            // when cancelling the pondering search
            if (!_isPondering)
            {
                _logger.Warn("Search cancelled at depth 1, choosing first found legal move as best one");
            }
            finalSearchResult = new(firstLegalMove, 0, 0, [firstLegalMove], alpha, beta);
        }
        else
        {
            finalSearchResult = _previousSearchResult = lastSearchResult;
        }

        finalSearchResult.IsCancelled = isCancelled;
        finalSearchResult.DepthReached = Math.Max(finalSearchResult.DepthReached, _maxDepthReached.LastOrDefault(item => item != default));
        finalSearchResult.Nodes = _nodes;
        finalSearchResult.Time = _stopWatch.ElapsedMilliseconds;
        finalSearchResult.NodesPerSecond = Utils.CalculateNps(_nodes, _stopWatch.ElapsedMilliseconds);
        finalSearchResult.HashfullPermill = _tt.HashfullPermillApprox();
        if (Configuration.EngineSettings.ShowWDL)
        {
            finalSearchResult.WDL = WDL.WDLModel(bestEvaluation, depth);
        }

        return finalSearchResult;
    }
}
