using Lynx.Model;
using NLog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Lynx;

public sealed partial class Engine
{
    private readonly Stopwatch _stopWatch = new();
    private readonly Move[] _pVTable = GC.AllocateArray<Move>(Configuration.EngineSettings.MaxDepth * (Configuration.EngineSettings.MaxDepth + 1 + Constants.ArrayDepthMargin) / 2, pinned: true);

    /// <summary>
    /// 2 x (<see cref="Configuration.EngineSettings.MaxDepth"/> + <see cref="Constants.ArrayDepthMargin"/>)
    /// </summary>
    internal readonly int[] _killerMoves = GC.AllocateArray<int>(2 * (Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin), pinned: true);

    /// <summary>
    /// 12 x 64
    /// </summary>
    internal readonly int[] _counterMoves = GC.AllocateArray<int>(12 * 64, pinned: true);

    private const int PieceToQuietHistoryLength = 12 * 64 * 2 * 2;

    private const int ButterflyHistoryLength = 64 * 64 * 2 * 2;

    /// <summary>
    /// 12 x 64 x 2 x 2
    /// piece x target square x source is attacked x target is attacked
    /// </summary>
    private readonly short[] _pieceToQuietHistory = GC.AllocateArray<short>(PieceToQuietHistoryLength, pinned: true);

    /// <summary>
    /// 64 x 64 x 2 x 2
    /// source square x target square x source is attacked x target is attacked
    /// </summary>
    private readonly short[] _butterflyQuietHistory = GC.AllocateArray<short>(ButterflyHistoryLength, pinned: true);

    /// <summary>
    /// 12 x 64 x 12,
    /// piece x target square x captured piece
    /// </summary>
    private readonly short[] _captureHistory = GC.AllocateArray<short>(12 * 64 * 12, pinned: true);

    /// <summary>
    /// 12 x 64 x 12 x 64
    /// piece x target square x previous piece x previous target square
    /// ply - 1 -> Counter-move history
    /// ply - 2 -> Follow-up move history
    /// </summary>
    private readonly short[] _continuationHistory = GC.AllocateArray<short>(12 * 64 * 12 * 64, pinned: true);

    /// <summary>
    /// <see cref="Constants.PawnCorrHistoryHashSize"/> x 2
    /// Pawn hash x side to move
    /// </summary>
    private readonly short[] _pawnCorrHistory = GC.AllocateArray<short>(Constants.PawnCorrHistoryHashSize * 2, pinned: true);

    /// <summary>
    /// <see cref="Constants.NonPawnCorrHistoryHashMask"/> x 2 x 2
    /// Non-pawn side hash x side to move x piece hash side
    /// </summary>
    private readonly short[] _nonPawnCorrHistory = GC.AllocateArray<short>(Constants.NonPawnCorrHistoryHashSize * 2 * 2, pinned: true);

    /// <summary>
    /// <see cref="Constants.MinorCorrHistoryHashSize"/> x 2
    /// Minor hash x side to move
    /// </summary>
    private readonly short[] _minorCorrHistory = GC.AllocateArray<short>(Constants.MinorCorrHistoryHashSize * 2, pinned: true);

    /// <summary>
    /// <see cref="Constants.MajorCorrHistoryHashSize"/> x 2
    /// Major hash x side to move
    /// </summary>
    private readonly short[] _majorCorrHistory = GC.AllocateArray<short>(Constants.MajorCorrHistoryHashSize * 2, pinned: true);

    /// <summary>
    /// <see cref="Constants.MaterialCorrHistoryHashSize"/> x 2
    /// Material hash x side to move
    /// </summary>
    private readonly short[] _materialCorrHistory = GC.AllocateArray<short>(Constants.MaterialCorrHistoryHashSize * 2, pinned: true);

    /// <summary>
    /// <see cref="Constants.ContinuationCorrHistoryHashSize"/>
    /// </summary>
    private readonly short[] _continuationCorrHistory = GC.AllocateArray<short>(Constants.ContinuationCorrHistoryHashSize, pinned: true);

    /// <summary>
    /// 12 x 64
    /// piece x target square
    /// </summary>
    private readonly ulong[][] _moveNodeCount;

    private readonly int[] _maxDepthReached = GC.AllocateArray<int>(Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin, pinned: true);

    /// <summary>
    /// <see cref="Constants.KingPawnHashSize"/>
    /// </summary>
    private readonly PawnTableElement[] _pawnEvalTable = GC.AllocateArray<PawnTableElement>(Constants.KingPawnHashSize, pinned: true);

    private ulong _nodes;

    private SearchResult? _previousSearchResult;

#pragma warning disable CA1805 // Do not initialize unnecessarily - Interferes with S3459
    private readonly Move _defaultMove = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily

    private int _bestMoveStability;
    private int _scoreDelta;

    #region MultiPV

    /// <summary>
    /// Number of PV lines searched per depth. This is equivalent to <see cref="Configuration.EngineSettings.MultiPV"/>, limited by the number of legal moves
    /// </summary>
    private int _multiPV = 1;

    /// <summary>
    /// Index (0-based) of the PV line being searched.
    /// It divides <see cref="_rootMoves"/> in finished/searched PV lines and unfinished ones
    /// </summary>
    private int _pvIndex;

    /// <summary>
    /// First <see cref="_pvIndex"/> moves are the already searched PV lines of the current depth, sorted by score
    /// </summary>
    private readonly RootMove[] _rootMoves = new RootMove[Constants.MaxNumberOfLegalMovesInAPosition];

    /// <summary>
    /// Number of legal moves at root, aka used length of <see cref="_rootMoves"/> buffer
    /// </summary>
    private int _rootMovesCount;

    #endregion

    /// <summary>
    /// Iterative Deepening Depth-First Search (IDDFS) using alpha-beta pruning.
    /// Requires <see cref="_searchConstraints"/> to be populated before invoking it
    /// </summary>
    /// <returns>Not null <see cref="SearchResult"/>, although made nullable in order to match online tb probing signature</returns>
    [SkipLocalsInit]
    private SearchResult IDDFS(bool isPondering, CancellationToken cancellationToken)
    {
        // Cleanup
        _nodes = 0;

        Array.Clear(_pVTable);
        Array.Clear(_maxDepthReached);
        for (int i = 0; i < 12; ++i)
        {
            Array.Clear(_moveNodeCount[i]);
        }

        AgeQuietHistory();

        int bestScore = 0;
        SearchResult? lastSearchResult = null;
        int depth = 1;
        Move firstLegalMove = default;

        _stopWatch.Restart();

        var logLevel = IsMainEngine
            ? LogLevel.Debug
#if MULTITHREAD_DEBUG
            : LogLevel.Trace;
#else
            : LogLevel.Off;
#endif

        try
        {
            if (!isPondering && _searchConstraints.MaxDepth == SearchConstraints.DefaultMaxDepth && _searchConstraints.MaxNodes == SearchConstraints.DefaultMaxNodes
                && OnlyOneLegalMove(ref firstLegalMove, out var onlyOneLegalMoveSearchResult))
            {
                if (!Configuration.EngineSettings.UCI_Minimal)
                {
                    _engineWriter.TryWrite(onlyOneLegalMoveSearchResult);
                }

                return onlyOneLegalMoveSearchResult;
            }

            Array.Clear(_killerMoves);
            // Not clearing quiet and capture histories here on purpose

            int mate = 0;

            _multiPV = 1;
            _pvIndex = 0;

            if (Configuration.EngineSettings.MultiPV > 1)
            {
                InitializeRootMoves();
            }

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                Debug.Assert(
                    lastSearchResult is null
                    || (lastSearchResult.Mate == 0
                        ? lastSearchResult.Score < EvaluationConstants.PositiveCheckmateDetectionLimit && lastSearchResult.Score > EvaluationConstants.NegativeCheckmateDetectionLimit
                        : Math.Abs(lastSearchResult.Score) < EvaluationConstants.CheckMateBaseEvaluation && (lastSearchResult.Score > EvaluationConstants.PositiveCheckmateDetectionLimit || lastSearchResult.Score < EvaluationConstants.NegativeCheckmateDetectionLimit)));

                SearchResult[] extraLines = [];

                if (_multiPV == 1)
                {
                    bestScore = SearchRoot(depth, lastSearchResult?.Score, logLevel, cancellationToken);

                    //PrintPvTable(depth: depth);
                    ValidatePVTable();
                }
                else
                {
                    var validLines = SearchRootLines(depth, logLevel, cancellationToken);
                    // We need to re-populate it with the best PV

                    Array.Clear(_pVTable);

                    if (validLines > 0)
                    {
                        bestScore = _rootMoves[0].Score;

                        // Before clearing it above, The PV table held the PV of the last line searched.
                        // However, it's the strongest line's PV the one we need for the rest of the logic
                        _rootMoves[0].PV.CopyTo(_pVTable, 0);

                        extraLines = BuildExtraLines(validLines, depth);
                    }
                    else
                    {
                        _logger.Warn("No best move found while doing MultiPV {MultiPV} of {Position}",
                        _multiPV, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));
                    }
                }

                Debug.Assert(bestScore != EvaluationConstants.MinEval);

                mate = CalculateMate(bestScore);

                var oldBestMove = lastSearchResult?.BestMove;
                var oldScore = lastSearchResult?.Score ?? 0;
                var lastSearchResultCandidate = UpdateLastSearchResult(lastSearchResult, bestScore, depth, mate, extraLines);

                if (lastSearchResultCandidate.BestMove == default)
                {
                    _logger.Warn(
                        "[#{EngineId}] Depth {Depth}: search didn't produce a best move for position {Position} after {Time}ms. Score {Score} (mate in {Mate}?) detected",
                        _id, depth, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove), _stopWatch.ElapsedMilliseconds, bestScore, mate);

                    _bestMoveStability = 0;
                    _scoreDelta = 0;

                    continue;
                }

                lastSearchResult = lastSearchResultCandidate;

                if (oldBestMove == lastSearchResult.BestMove)
                {
                    ++_bestMoveStability;
                }
                else
                {
                    _bestMoveStability = 0;
                }

                _scoreDelta = oldScore - lastSearchResult.Score;

                if (!Configuration.EngineSettings.UCI_Minimal)
                {
                    _engineWriter.TryWrite(lastSearchResult);
                }

            } while (StopSearchCondition(lastSearchResult?.BestMove, depth++, mate, bestScore, isPondering));
        }
        catch (OperationCanceledException)
        {
            // One degree higher than log level, but can't add 1 to Off
            var higherLogLevel = IsMainEngine
                ? LogLevel.Info
#if MULTITHREAD_DEBUG
                : LogLevel.Debug;
#else
                : LogLevel.Off;
#endif

#pragma warning disable S6667 // Logging in a catch clause should pass the caught exception as a parameter - expected exception we want to ignore
            _logger.Log(higherLogLevel,
                "[#{EngineId}] Depth {Depth}: main search cancellation requested after {Time}ms (>= {HardLimitTime}ms). Nodes {Nodes}, best move will be returned",
                _id, depth, _stopWatch.ElapsedMilliseconds, _searchConstraints.HardLimitTimeBound, _nodes);
#pragma warning restore S6667 // Logging in a catch clause should pass the caught exception as a parameter.

            Array.Clear(_pVTable);
            for (int i = 0; i < lastSearchResult?.Moves.Length; ++i)
            {
                _pVTable[i] = lastSearchResult.Moves[i];
            }
        }
        catch (Exception e) when (e is not LynxException)
        {
            _logger.Error(e,
                "[#{EngineId}] Depth {Depth}: unexpected error occurred during the search of position {Position}, best move will be returned\n",
                _id, depth, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));
        }
        finally
        {
            _stopWatch.Stop();
        }

        //TODO revisit
        //if (Configuration.EngineSettings.UseOnlineTablebaseInRootPositions
        //    && isMateDetected
        //    && (finalSearchResult.Mate * 2) + Game.HalfMovesWithoutCaptureOrPawnMove < Constants.MaxMateDistanceToStopSearching)
        //{
        //    _searchCancellationToken.Cancel();
        //    _logger.Info("Engine search found a short enough mate, cancelling online tb probing if still active");
        //}

        return GenerateFinalSearchResult(lastSearchResult, bestScore, depth, firstLegalMove, isPondering);
    }

    /// <summary>
    /// Searches the root position at the given depth.
    /// Uses aspiration windows centered in <paramref name="expectedScore"/> when the depth is high enough and there is one,
    /// and a full window search otherwise
    /// </summary>
    /// <param name="expectedScore">lastSearchResult.Score</param>
    /// <returns>Score of the search, whose PV is left in <see cref="_pVTable"/></returns>
    private int SearchRoot(int depth, int? expectedScore, LogLevel logLevel, CancellationToken cancellationToken)
    {
        int bestScore = EvaluationConstants.MinEval;

        if (depth < Configuration.EngineSettings.AspirationWindow_MinDepth
            || expectedScore is null)
        {
            return NegaMax(depth: depth, ply: 0, EvaluationConstants.MinEval, EvaluationConstants.MaxEval, cutnode: false, cancellationToken);
        }

        // 🔍 Aspiration windows - search using a window around an expected score (the previous search one)
        // If the resulting score doesn't fall inside of the window, it is widened it until it does
        var window = Configuration.EngineSettings.AspirationWindow_Base;

        // A temporary reduction is used for fail highs, because the verification for those 'too good' lines
        // are expected to happen at lower depths
        int failHighReduction = 0;

        var alpha = Math.Clamp(expectedScore.Value - window, EvaluationConstants.MinEval, EvaluationConstants.MaxEval);
        var beta = Math.Clamp(expectedScore.Value + window, EvaluationConstants.MinEval, EvaluationConstants.MaxEval);

        if (_logger.IsEnabled(logLevel))
        {
            _logger.Log(logLevel,
                "[#{EngineId}] Depth {Depth}: asp-win [{Alpha}, {Beta}] for previous search score {Score}, nodes {Nodes}",
                _id, depth, alpha, beta, expectedScore.Value, _nodes);
        }

        Debug.Assert(Math.Abs(expectedScore.Value) < EvaluationConstants.CheckMateBaseEvaluation);

        while (true)
        {
            var depthToSearch = depth - failHighReduction;
            Debug.Assert(depthToSearch > 0);

            // Alpha and beta values close to checkmate scores due to window widening can cause false mate reporting (very high/low mate values) after being saved in TT
            if (beta >= EvaluationConstants.PositiveCheckmateDetectionLimit)
            {
                beta = EvaluationConstants.MaxEval;
            }

            if (alpha <= EvaluationConstants.NegativeCheckmateDetectionLimit)
            {
                alpha = EvaluationConstants.MinEval;
            }

            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] Asp-win depth {Depth} ({DepthWithoutReduction} - {Reduction}), window {Window}: [{Alpha}, {Beta}] for score {Score}, time {Time}, nodes {Nodes}",
                    _id, depthToSearch, depth, failHighReduction, window, alpha, beta, bestScore, _stopWatch.ElapsedMilliseconds, _nodes);
            }

            bestScore = NegaMax(depth: depthToSearch, ply: 0, alpha, beta, cutnode: false, cancellationToken);
            Debug.Assert(bestScore > EvaluationConstants.MinEval && bestScore < EvaluationConstants.MaxEval);

            // 13, 19, 28, 42, 63, 94, 141, 211, 316, 474, 711, 1066, 1599, 2398, 3597, 5395, 8092, 12138, 18207, 27310, EvaluationConstants.MaxEval
            window = Math.Min(EvaluationConstants.MaxEval, (int)(window * Configuration.EngineSettings.AspirationWindow_Multiplier));

            // Depth change: https://github.com/lynx-chess/Lynx/pull/440
            if (alpha >= bestScore)     // Fail low
            {
                alpha = Math.Clamp(bestScore - window, EvaluationConstants.MinEval, EvaluationConstants.MaxEval);
                beta = (alpha + beta) >> 1;  // (alpha + beta) / 2
                failHighReduction = 0;
            }
            else if (beta <= bestScore)     // Fail high
            {
                beta = Math.Clamp(bestScore + window, EvaluationConstants.MinEval, EvaluationConstants.MaxEval);
                if (failHighReduction < 3)
                {
                    ++failHighReduction;
                }
            }
            else
            {
                break;
            }

            if (bestScore > EvaluationConstants.CheckMateBaseEvaluation)
            {
                _logger.Warn(
                    "[#{EngineId}] Depth {Depth}: potential +X checkmate detected in position {Position}, but score {BestScore} outside of the limits",
                    _id, depth, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove), bestScore);

                bestScore = EvaluationConstants.PositiveCheckmateDetectionLimit + 1;

                break;
            }

            if (bestScore < -EvaluationConstants.CheckMateBaseEvaluation)
            {
                _logger.Warn(
                    "[#{EngineId}] Depth {Depth}: potential -X checkmate detected in position {Position}, but score {BestScore} outside of the limits",
                    _id, depth, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove), bestScore);

                bestScore = EvaluationConstants.NegativeCheckmateDetectionLimit - 1;

                break;
            }
        }

        return bestScore;
    }

    [SkipLocalsInit]
    private unsafe void InitializeRootMoves()
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        _rootMovesCount = 0;

        foreach (var move in MoveGenerator.GenerateAllMoves(Game.CurrentPosition, moves))
        {
            var gameState = Game.CurrentPosition.MakeMove(move);
            bool isPositionValid = Game.CurrentPosition.WasProduceByAValidMove();
            Game.CurrentPosition.UnmakeMove(move, gameState);

            if (isPositionValid)
            {
                _rootMoves[_rootMovesCount++] = new RootMove(move);
            }
        }

        _multiPV = Math.Clamp(_rootMovesCount, 1, Configuration.EngineSettings.MultiPV);
    }

    /// <summary>
    /// Searches the first <see cref="_multiPV"/> PV lines of the root position at the given depth, one after the other.
    /// Moves chosen as the best ones of the previous lines are excluded from the search of the following ones.
    /// After this method, the first moves of <see cref="_rootMoves"/> are the PV lines (as many as the returned value), sorted by score
    /// </summary>
    /// <returns>Number of PV lines successfully searched</returns>
    private int SearchRootLines(int depth, LogLevel logLevel, CancellationToken cancellationToken)
    {
        for (_pvIndex = 0; _pvIndex < _multiPV; ++_pvIndex)
        {
            var previousScore = _rootMoves[_pvIndex].Score;

            var score = SearchRoot(
                depth,
                previousScore != RootMove.DefaultScore ? previousScore : null,
                logLevel,
                cancellationToken);

            ValidatePVTable();

            var pvTableSpan = _pVTable.AsSpan();
            var pvLength = pvTableSpan.IndexOf(0);
            if (pvLength <= 0)
            {
                _logger.Warn(
                    "[#{EngineId}] Depth {Depth}: PV line {PVIndex} didn't produce a best move for position {Position}. Score {Score} detected",
                    _id, depth, _pvIndex + 1, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove), score);

                return _pvIndex;
            }

            var pv = pvTableSpan[..pvLength].ToArray();
            var firstPVMove = pv[0];

            // Search the RootMove equivalent of firstPVMove in the unfinished part of _rootMoves
            var rootMoveIndex = -1;
            for (int i = _pvIndex; i < _rootMovesCount; ++i)
            {
                if (_rootMoves[i].Move == firstPVMove)
                {
                    rootMoveIndex = i;
                    break;
                }
            }

            if (rootMoveIndex == -1)
            {
                _logger.Warn(
                    "[#{EngineId}] Depth {Depth}: PV line {PVIndex} best move {Move} not found among the remaining root moves of position {Position}",
                    _id, depth, _pvIndex + 1, firstPVMove.UCIString(), Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));

                return _pvIndex;
            }

            // Move that RootMove to _pvIndex, right after the previously finished PV lines, keeping the relative order of the rest (stable)
            var rootMove = _rootMoves[rootMoveIndex];
            Array.Copy(_rootMoves, _pvIndex, _rootMoves, _pvIndex + 1, rootMoveIndex - _pvIndex);
            rootMove.Score = score;
            rootMove.PV = pv;
            _rootMoves[_pvIndex] = rootMove;

            // Sort by score the lines searched so far. Lines before this one were already sorted, so only this one can be misplaced
            for (int i = _pvIndex; i > 0 && _rootMoves[i].Score > _rootMoves[i - 1].Score; --i)
            {
                (_rootMoves[i], _rootMoves[i - 1]) = (_rootMoves[i - 1], _rootMoves[i]);
            }
        }

        return _multiPV;
    }

    /// <summary>
    /// Whether <paramref name="move"/> has already been chosen as the best move of a previous PV line of the current depth
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool WasSearchedAsRootMove(Move move)
    {
        for (int i = 0; i < _pvIndex; ++i)
        {
            if (_rootMoves[i].Move == move)
            {
                return true;
            }
        }

        return false;
    }

    private SearchResult[] BuildExtraLines(int validLines, int depth)
    {
        var extraLinesCount = validLines - 1;
        var extraLines = new SearchResult[extraLinesCount];

        for (int i = 0; i < extraLinesCount; ++i)
        {
            ref var rootMove = ref _rootMoves[i + 1];

            extraLines[i] = new SearchResult(
#if MULTITHREAD_DEBUG
                _id,
#endif
                rootMove.PV[0], rootMove.Score, depth, rootMove.PV, CalculateMate(rootMove.Score))
            {
                MultiPVIndex = i + 2,
                DepthReached = _maxDepthReached.Max(),
            };
        }

        return extraLines;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateMate(int score)
    {
        var scoreAbs = Math.Abs(score);
        bool isMateDetected = scoreAbs > EvaluationConstants.PositiveCheckmateDetectionLimit && scoreAbs < EvaluationConstants.CheckMateBaseEvaluation;

        return isMateDetected
            ? Utils.CalculateMateInX(score, scoreAbs)
            : 0;
    }

    private bool StopSearchCondition(Move? bestMove, int depth, int mate, int bestScore, bool isPondering)
    {
        var logLevel = IsMainEngine
            ? LogLevel.Info
#if MULTITHREAD_DEBUG
            : LogLevel.Debug;
#else
                : LogLevel.Off;
#endif

        if (depth + 1 >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Log(logLevel,
                "[#{EngineId}] Max depth reached: {MaxDepth}",
                _id, Configuration.EngineSettings.MaxDepth);

            return false;
        }

        if (bestMove is null || bestMove == 0)
        {
            _logger.Warn(
                "[#{EngineId}] Depth {Depth}: search continues, due to lack of best move", _id, depth);

            return true;
        }

        if (mate != 0)
        {
            if (mate == EvaluationConstants.MaxMate || mate == EvaluationConstants.MinMate)
            {
                //_logger.Warn( // TODO bug
                _logger.Info(
                    "[#{EngineId}] Depth {Depth}: mate {Mate} outside of range detected, stopping search and playing best move so far: {BestMove}",
                    _id, depth, mate, bestMove.Value.UCIString());

                return false;
            }

            var winningMateThreshold = (100 - Game.HalfMovesWithoutCaptureOrPawnMove) / 2;

            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] Depth {Depth}: mate in {Mate} detected (score {Score}, {MateThreshold} moves until draw by repetition)",
                    _id, depth, mate, bestScore, winningMateThreshold);
            }

            if (!isPondering && (mate < 0 || mate + Constants.MateDistanceMarginToStopSearching < winningMateThreshold))
            {
                if (_searchConstraints.SoftLimitTimeBound < Configuration.EngineSettings.StopSearchOnMate_MaxSoftTimeBoundLimit
                    && (depth >= Configuration.EngineSettings.StopSearchOnMate_MinDepth
                        || depth >= mate * 2))
                {
                    _logger.Log(logLevel,
                        "[#{EngineId}] Stopping, since mate is short enough and we're short on time: soft limit {SoftLimit}ms",
                        _id, _searchConstraints.SoftLimitTimeBound);

                    return false;
                }

                _logger.Log(logLevel,
                    "[#{EngineId}] Could stop search, since mate is short enough",
                    _id, _searchConstraints.SoftLimitTimeBound);
            }

            _logger.Log(logLevel, "[#{EngineId}] Search continues, hoping to find a faster mate", _id);
        }

        var maxDepth = _searchConstraints.MaxDepth;
        if (maxDepth != SearchConstraints.DefaultMaxDepth)
        {
            var shouldContinue = depth + 1 <= maxDepth;

            if (!shouldContinue)
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] Depth {Depth}: stopping, max. go command depth reached",
                    _id, depth);
            }

            return shouldContinue;
        }

        var maxNodes = _searchConstraints.MaxNodes;
        if (maxNodes != SearchConstraints.DefaultMaxNodes)
        {
            var shouldContinue = _nodes <= maxNodes;

            if (!shouldContinue)
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] Nodes {Nodes}: stopping, go command nodes reached",
                    _id, _nodes);
            }

            return shouldContinue;
        }

        if (!isPondering)
        {
            var elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;

            var bestMoveNodeCount = _moveNodeCount[bestMove.Value.Piece()][bestMove.Value.TargetSquare()];
            var scaledSoftLimitTimeBound = TimeManager.SoftLimit(_searchConstraints, depth, bestMoveNodeCount, _nodes, _bestMoveStability, _scoreDelta);

            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] [TM] {ElapsedMilliseconds}ms | Depth {Depth}: hard limit {HardLimit}, base soft limit {BaseSoftLimit}ms, scaled soft limit {ScaledSoftLimit}ms",
                    _id, elapsedMilliseconds, depth, _searchConstraints.HardLimitTimeBound, _searchConstraints.SoftLimitTimeBound, scaledSoftLimitTimeBound);
            }

            if (elapsedMilliseconds > scaledSoftLimitTimeBound)
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] [TM] Stopping at depth {0} (nodes {1}): {2}ms > {3}ms",
                    _id, depth, _nodes, elapsedMilliseconds, scaledSoftLimitTimeBound);

                return false;
            }
        }

        return true;
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe bool OnlyOneLegalMove(ref Move firstLegalMove, [NotNullWhen(true)] out SearchResult? result)
    {
        bool onlyOneLegalMove = false;

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

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

            // We don't have or need any eval, and we return a fake but recognizable one
            // See constant XML for details
            var score = Game.CurrentPosition.Side == Side.White
                ? +EvaluationConstants.SingleMoveScore
                : -EvaluationConstants.SingleMoveScore;

            result = new SearchResult(
#if MULTITHREAD_DEBUG
                _id,
#endif
                firstLegalMove, score, targetDepth: 1, [firstLegalMove])
            {
                DepthReached = 1,
                Nodes = 0,
                Time = 1,
                NodesPerSecond = 1,
            };

            return true;
        }

        result = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SearchResult UpdateLastSearchResult(SearchResult? lastSearchResult,
        int bestScore, int depth, int mate, SearchResult[] extraLines)
    {
        var pvTableSpan = _pVTable.AsSpan();
        var pvMoves = pvTableSpan[..pvTableSpan.IndexOf(0)].ToArray();

        var elapsedSeconds = Utils.CalculateElapsedSeconds(_stopWatch);

        _previousSearchResult = lastSearchResult;
        return new SearchResult(
#if MULTITHREAD_DEBUG
                _id,
#endif
            pvMoves.FirstOrDefault(), bestScore, depth, pvMoves, mate)
        {
            DepthReached = _maxDepthReached.Max(),
            Nodes = _nodes,
            Time = Utils.CalculateUCITime(elapsedSeconds),
            NodesPerSecond = Utils.CalculateNps(_nodes, elapsedSeconds),
            ExtraLines = extraLines,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SearchResult GenerateFinalSearchResult(SearchResult? lastSearchResult,
        int bestScore, int depth, Move firstLegalMove, bool isPondering)
    {
        SearchResult finalSearchResult;
        if (lastSearchResult is null)
        {
            var noDepth1Message =
                $"[#{_id}] Depth {depth}: search cancelled with no result for position {Game.PositionBeforeLastSearch.FEN()} (hard limit {_searchConstraints.HardLimitTimeBound}ms, soft limit {_searchConstraints.SoftLimitTimeBound}ms). Choosing an emergency move";

            // In the event of a quick ponderhit/stop while pondering because the opponent moved quickly, we don't want no warning triggered here
            //  when cancelling the pondering search
            // The other condition reflects what happens in helper engines when a mate is quickly detected in the main:
            //  search in helper engines sometimes get cancelled before any meaningful result is found, so we don't want a warning either
            if (isPondering || !IsMainEngine)
            {
                _logger.Debug(noDepth1Message);
            }
            else
            {
                _logger.Warn(noDepth1Message);
            }

            finalSearchResult = BestMoveRoot(firstLegalMove);
        }
        else
        {
            finalSearchResult = _previousSearchResult = lastSearchResult;
        }

        var elapsedSeconds = Utils.CalculateElapsedSeconds(_stopWatch);

        finalSearchResult.DepthReached = Math.Max(finalSearchResult.DepthReached, _maxDepthReached.LastOrDefault(item => item != default));
        finalSearchResult.Nodes = _nodes;
        finalSearchResult.Time = Utils.CalculateUCITime(elapsedSeconds);
        finalSearchResult.NodesPerSecond = Utils.CalculateNps(_nodes, elapsedSeconds);
        finalSearchResult.HashfullPermill = _tt.HashfullPermillApprox();
        if (Configuration.EngineSettings.ShowWDL)
        {
            finalSearchResult.WDL = WDL.WDLModel(bestScore, depth);
        }

        return finalSearchResult;
    }

    /// <summary>
    /// Find the best move without searching, based on TT and <see cref="ScoreMove(int, int, short)"/>
    /// </summary>
    private SearchResult BestMoveRoot(Move firstLegalMove)
    {
        var score = 0;
        ShortMove ttBestMove = default;

        using var position = new Position(Game.PositionBeforeLastSearch);
        var ttHit = _tt.ProbeHash(position, Game.HalfMovesWithoutCaptureOrPawnMove, ply: 0, out var ttEntry);

        if (ttHit)
        {
            ttBestMove = ttEntry.BestMove;
            score = ttEntry.Score;

            if (ttEntry.Score == EvaluationConstants.NoScore)
            {
                score = ttEntry.StaticEval;
            }
        }

        // TODO calculate manually
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);
        position.CalculateThreats(ref evaluationContext);
        var oppositeSideAttacks = evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];

        Span<Move> pseudoLegalMoves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, pseudoLegalMoves, oppositeSideAttacks);

        Span<int> moveScores = stackalloc int[pseudoLegalMoves.Length];
        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            moveScores[i] = ScoreMove(position, pseudoLegalMoves[i], 0, oppositeSideAttacks, ttBestMove);
        }

        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            // Incremental move sorting
            for (int j = i + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (moveScores[j] > moveScores[i])
                {
                    (moveScores[i], moveScores[j], pseudoLegalMoves[i], pseudoLegalMoves[j]) = (moveScores[j], moveScores[i], pseudoLegalMoves[j], pseudoLegalMoves[i]);
                }
            }

            var move = pseudoLegalMoves[i];

            var gameState = position.MakeMove(move);
            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            // We don't have or need any eval, and we don't want to return 0 or a negative eval that
            // could make the GUI resign or take a draw from this position.
            // Since this only happens in root, we don't really care about being more precise for raising
            // alphas or betas of parent moves, so let's just return +-2 pawns depending on the side to move
            var singleMoveEval = Game.CurrentPosition.Side == Side.White
                ? EvaluationConstants.EmergencyMoveScore        // -0.66
                : -EvaluationConstants.EmergencyMoveScore;      // +0.66

            return new SearchResult(
#if MULTITHREAD_DEBUG
                _id,
#endif
                move, singleMoveEval, targetDepth: 1, [move])
            {
                DepthReached = 1,
            };
        }

        _logger.Error("No valid move found while looking for an emergency move for position {Fen}", position.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));

        return new(
#if MULTITHREAD_DEBUG
                _id,
#endif
            firstLegalMove, 0, 0, [firstLegalMove]);
    }
}
