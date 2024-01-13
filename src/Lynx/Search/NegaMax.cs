using Lynx.Model;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// NegaMax algorithm implementation using alpha-beta pruning, quiescence search and Iterative Deepeting Depth-First Search (IDDFS)
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="ply"></param>
    /// <param name="alpha">
    /// Best score the Side to move can achieve, assuming best play by the opponent.
    /// Defaults to the worse possible score for Side to move, Int.MinValue.
    /// </param>
    /// <param name="beta">
    /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
    /// Defaults to the worse possible score for Side to move's opponent, Int.MaxValue
    /// </param>
    /// <returns></returns>
    private int NegaMax(int depth, int ply, int alpha, int beta, bool parentWasNullMove = false)
    {
        var position = Game.CurrentPosition;

        // Prevents runtime failure in case depth is increased due to check extension, since we're using ply when calculating pvTable index,
        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth {0} reached", Configuration.EngineSettings.MaxDepth);
            return position.StaticEvaluation().Score;
        }

        _maxDepthReached[ply] = ply;
        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        bool isRoot = ply == 0;
        bool pvNode = beta - alpha > 1;
        ShortMove ttBestMove = default;
        NodeType ttElementType = default;
        int ttEvaluation = default;

        if (!isRoot)
        {
            (ttEvaluation, ttBestMove, ttElementType) = _tt.ProbeHash(_ttMask, position, depth, ply, alpha, beta);
            if (ttEvaluation != EvaluationConstants.NoHashEntry)
            {
                return ttEvaluation;
            }

            // Internal iterative reduction (IIR)
            // If this position isn't found in TT, it has never been searched before,
            // so the search will be potentially expensive.
            // Therefore, we search with reduced depth for now, expecting to record a TT move
            // which we'll be able to use later for the full depth search
            if (ttElementType == default && depth >= Configuration.EngineSettings.IIR_MinDepth)
            {
                --depth;
            }
        }

        // Before any time-consuming operations
        _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        bool isInCheck = position.IsInCheck();

        if (isInCheck)
        {
            ++depth;
        }
        if (depth <= 0)
        {
            if (MoveGenerator.CanGenerateAtLeastAValidMove(position))
            {
                return QuiescenceSearch(ply, alpha, beta);
            }

            var finalPositionEvaluation = Position.EvaluateFinalPosition(ply, isInCheck);
            _tt.RecordHash(_ttMask, position, depth, ply, finalPositionEvaluation, NodeType.Exact);
            return finalPositionEvaluation;
        }

        if (!pvNode && !isInCheck)
        {
            var (staticEval, phase) = position.StaticEvaluation();

            // 🔍 Null Move Pruning (NMP) - our position is so good that we can potentially afford giving our opponent a double move and still remain ahead of beta
            if (depth >= Configuration.EngineSettings.NMP_MinDepth
                && staticEval >= beta
                && !parentWasNullMove
                && phase > 2   // Zugzwang risk reduction: pieces other than pawn presents
                && (ttElementType != NodeType.Alpha || ttEvaluation >= beta))   // TT suggests NMP will fail: entry must not be a fail-low entry with a score below beta - Stormphrax and Ethereal
            {
                var nmpReduction = Configuration.EngineSettings.NMP_BaseDepthReduction + ((depth + 1) / 3);   // Clarity

                // TODO more advanced adaptative reduction, similar to what Ethereal and Stormphrax are doing
                //var nmpReduction = Math.Min(
                //    depth,
                //    3 + (depth / 3) + Math.Min((staticEval - beta) / 200, 3));

                var gameState = position.MakeNullMove();
                var evaluation = -NegaMax(depth - 1 - nmpReduction, ply + 1, -beta, -beta + 1, parentWasNullMove: true);
                position.UnMakeNullMove(gameState);

                if (evaluation >= beta)
                {
                    return evaluation;
                }
            }

            if (depth <= Configuration.EngineSettings.RFP_MaxDepth)
            {
                // 🔍 Reverse Futility Pruning (RFP) - https://www.chessprogramming.org/Reverse_Futility_Pruning
                if (staticEval - (Configuration.EngineSettings.RFP_DepthScalingFactor * depth) >= beta)
                {
                    return staticEval;
                }

                // 🔍 Razoring - Strelka impl (CPW) - https://www.chessprogramming.org/Razoring#Strelka
                if (depth <= Configuration.EngineSettings.Razoring_MaxDepth)
                {
                    var score = staticEval + Configuration.EngineSettings.Razoring_Depth1Bonus;

                    if (score < beta)               // Static evaluation + bonus indicates fail-low node
                    {
                        if (depth == 1)
                        {
                            var qSearchScore = QuiescenceSearch(ply, alpha, beta);

                            return qSearchScore > score
                                ? qSearchScore
                                : score;
                        }

                        score += Configuration.EngineSettings.Razoring_NotDepth1Bonus;

                        if (score < beta)               // Static evaluation indicates fail-low node
                        {
                            var qSearchScore = QuiescenceSearch(ply, alpha, beta);
                            if (qSearchScore < beta)    // Quiescence score also indicates fail-low node
                            {
                                return qSearchScore > score
                                    ? qSearchScore
                                    : score;
                            }
                        }
                    }
                }
            }
        }

        var nodeType = NodeType.Alpha;
        int movesSearched = 0;
        Move? bestMove = null;
        bool isAnyMoveValid = false;

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves);

        Span<int> scores = stackalloc int[pseudoLegalMoves.Length];
        if (_isFollowingPV)
        {
            _isFollowingPV = false;
            for (int i = 0; i < pseudoLegalMoves.Length; ++i)
            {
                scores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: true, ttBestMove);

                if (pseudoLegalMoves[i] == _pVTable[depth])
                {
                    _isFollowingPV = true;
                    _isScoringPV = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < pseudoLegalMoves.Length; ++i)
            {
                scores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: true, ttBestMove);
            }
        }

        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Length; ++moveIndex)
        {
            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            for (int j = moveIndex + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (scores[j] > scores[moveIndex])
                {
                    (scores[moveIndex], scores[j], pseudoLegalMoves[moveIndex], pseudoLegalMoves[j]) = (scores[j], scores[moveIndex], pseudoLegalMoves[j], pseudoLegalMoves[moveIndex]);
                }
            }

            var move = pseudoLegalMoves[moveIndex];

            var gameState = position.MakeMove(move);

            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            ++_nodes;
            isAnyMoveValid = true;

            PrintPreMove(position, ply, move);

            // Before making a move
            var oldValue = Game.HalfMovesWithoutCaptureOrPawnMove;
            Game.HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, Game.HalfMovesWithoutCaptureOrPawnMove);
            var isThreeFoldRepetition = !Game.PositionHashHistory.Add(position.UniqueIdentifier);

            int evaluation;
            if (isThreeFoldRepetition || Game.Is50MovesRepetition())
            {
                evaluation = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else if (pvNode && movesSearched == 0)
            {
                evaluation = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
            }
            else
            {
                // Late Move Pruning (LMP) - all quiet moves can be pruned
                // after searching the first few given by the move ordering algorithm
                if (!pvNode
                    && !isInCheck
                    && depth <= Configuration.EngineSettings.LMP_MaxDepth
                    && scores[moveIndex] < EvaluationConstants.PromotionMoveScoreValue  // Quiet moves
                    && moveIndex >= Configuration.EngineSettings.LMP_BaseMovesToTry + (Configuration.EngineSettings.LMP_MovesDepthMultiplier * depth)) // Based on formula suggested by Antares
                {
                    // After making a move
                    Game.HalfMovesWithoutCaptureOrPawnMove = oldValue;
                    if (!isThreeFoldRepetition)
                    {
                        Game.PositionHashHistory.Remove(position.UniqueIdentifier);
                    }
                    position.UnmakeMove(move, gameState);

                    break;
                }

                int reduction = 0;

                // 🔍 Late Move Reduction (LMR) - search with reduced depth
                // Impl. based on Ciekce advice (Stormphrax) and Stormphrax & Akimbo implementations
                if (movesSearched >= (pvNode ? Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves : Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves - 1)
                    && depth >= Configuration.EngineSettings.LMR_MinDepth
                    && !isInCheck
                    && !move.IsCapture())
                {
                    reduction = EvaluationConstants.LMRReductions[depth][movesSearched];

                    if (pvNode)
                    {
                        --reduction;
                    }
                    if (position.IsInCheck())   // i.e. move gives check
                    {
                        --reduction;
                    }

                    // Don't allow LMR to drop into qsearch or increase the depth
                    // depth - 1 - depth +2 = 1, min depth we want
                    reduction = Math.Clamp(reduction, 0, depth - 2);
                }

                // 🔍 Static Exchange Evaluation (SEE) reduction
                // Bad captures are reduced more
                if (!isInCheck
                    && scores[moveIndex] < EvaluationConstants.PromotionMoveScoreValue
                    && scores[moveIndex] >= EvaluationConstants.BadCaptureMoveBaseScoreValue)
                {
                    reduction += Configuration.EngineSettings.SEE_BadCaptureReduction;
                }

                // Search with reduced depth
                evaluation = -NegaMax(depth - 1 - reduction, ply + 1, -alpha - 1, -alpha);

                // 🔍 Principal Variation Search (PVS)
                if (evaluation > alpha && reduction > 0)
                {
                    // Optimistic search, validating that the rest of the moves are worse than bestmove.
                    // It should produce more cutoffs and therefore be faster.
                    // https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm

                    // Search with full depth but narrowed score bandwidth
                    evaluation = -NegaMax(depth - 1, ply + 1, -alpha - 1, -alpha);
                }

                if (evaluation > alpha && evaluation < beta)
                {
                    // PVS Hipothesis invalidated -> search with full depth and full score bandwidth
                    evaluation = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
                }
            }

            // After making a move
            Game.HalfMovesWithoutCaptureOrPawnMove = oldValue;
            if (!isThreeFoldRepetition)
            {
                Game.PositionHashHistory.Remove(position.UniqueIdentifier);
            }
            position.UnmakeMove(move, gameState);

            PrintMove(ply, move, evaluation);

            // Fail-hard beta-cutoff - refutation found, no need to keep searching this line
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough");

                if (!move.IsCapture())
                {
                    // 🔍 Quiet history moves
                    // Doing this only in beta cutoffs (instead of when eval > alpha) was suggested by Sirius author
                    var piece = move.Piece();
                    var targetSquare = move.TargetSquare();

                    _historyMoves[piece][targetSquare] = ScoreHistoryMove(
                        _historyMoves[piece][targetSquare],
                        EvaluationConstants.HistoryBonus[depth]);

                    // 🔍 History penalty/malus
                    // When a quiet move fails high, penalize previous visited ones
                    for (int i = 0; i < moveIndex; ++i)
                    {
                        var visitedMove = pseudoLegalMoves[moveIndex];
                        if (!visitedMove.IsCapture())                           // TODO: Penalize only quiets?
                        {
                            var visitedMovePiece = visitedMove.Piece();
                            var visitedMoveTargetSquare = visitedMove.TargetSquare();

                            _historyMoves[visitedMovePiece][visitedMoveTargetSquare] = ScoreHistoryMove(
                                _historyMoves[visitedMovePiece][visitedMoveTargetSquare],
                                -EvaluationConstants.HistoryBonus[depth]);
                        }
                    }

                    // 🔍 Killer moves
                    if (move.PromotedPiece() == default && move != _killerMoves[0][ply])
                    {
                        if (move != _killerMoves[1][ply])
                        {
                            _killerMoves[2][ply] = _killerMoves[1][ply];
                        }

                        _killerMoves[1][ply] = _killerMoves[0][ply];
                        _killerMoves[0][ply] = move;
                    }
                }

                _tt.RecordHash(_ttMask, position, depth, ply, beta, NodeType.Beta, bestMove);

                return beta;    // TODO return evaluation?
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
                bestMove = move;

                _pVTable[pvIndex] = move;
                CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);

                nodeType = NodeType.Exact;
            }

            ++movesSearched;
        }

        if (bestMove is null && !isAnyMoveValid)
        {
            var eval = Position.EvaluateFinalPosition(ply, isInCheck);

            _tt.RecordHash(_ttMask, position, depth, ply, eval, NodeType.Exact);
            return eval;
        }

        _tt.RecordHash(_ttMask, position, depth, ply, alpha, nodeType, bestMove);

        // Node fails low
        return alpha;
    }

    /// <summary>
    /// Quiescence search implementation, NegaMax alpha-beta style, fail-hard
    /// </summary>
    /// <param name="ply"></param>
    /// <param name="alpha">
    /// Best score White can achieve, assuming best play by Black.
    /// Defaults to the worse possible score for white, Int.MinValue.
    /// </param>
    /// <param name="beta">
    /// Best score Black can achieve, assuming best play by White
    /// Defaults to the works possible score for Black, Int.MaxValue
    /// </param>
    /// <returns></returns>
    public int QuiescenceSearch(int ply, int alpha, int beta)
    {
        var position = Game.CurrentPosition;

        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
        _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth {0} reached", Configuration.EngineSettings.MaxDepth);
            return position.StaticEvaluation().Score;
        }

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        var ttProbeResult = _tt.ProbeHash(_ttMask, position, 0, ply, alpha, beta);
        if (ttProbeResult.Evaluation != EvaluationConstants.NoHashEntry)
        {
            return ttProbeResult.Evaluation;
        }
        ShortMove ttBestMove = ttProbeResult.BestMove;

        _maxDepthReached[ply] = ply;

        var staticEvaluation = position.StaticEvaluation().Score;

        // Fail-hard beta-cutoff (updating alpha after this check)
        if (staticEvaluation >= beta)
        {
            PrintMessage(ply - 1, "Pruning before starting quiescence search");
            return staticEvaluation;
        }

        // Better move
        if (staticEvaluation > alpha)
        {
            alpha = staticEvaluation;
        }

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var pseudoLegalMoves = MoveGenerator.GenerateAllCaptures(position, moves);
        if (pseudoLegalMoves.Length == 0)
        {
            // Checking if final position first: https://github.com/lynx-chess/Lynx/pull/358
            return staticEvaluation;
        }

        var nodeType = NodeType.Alpha;
        Move? bestMove = null;
        bool isThereAnyValidCapture = false;

        Span<int> scores = stackalloc int[pseudoLegalMoves.Length];
        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            scores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: false, ttBestMove);
        }

        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            for (int j = i + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (scores[j] > scores[i])
                {
                    (scores[i], scores[j], pseudoLegalMoves[i], pseudoLegalMoves[j]) = (scores[j], scores[i], pseudoLegalMoves[j], pseudoLegalMoves[i]);
                }
            }

            var move = pseudoLegalMoves[i];

            // Prune bad captures
            if (scores[i] < EvaluationConstants.PromotionMoveScoreValue && scores[i] >= EvaluationConstants.BadCaptureMoveBaseScoreValue)
            {
                continue;
            }

            var gameState = position.MakeMove(move);
            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            ++_nodes;
            isThereAnyValidCapture = true;

            PrintPreMove(position, ply, move, isQuiescence: true);

            // Before making a move
            var oldValue = Game.HalfMovesWithoutCaptureOrPawnMove;
            Game.HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, Game.HalfMovesWithoutCaptureOrPawnMove);
            var isThreeFoldRepetition = !Game.PositionHashHistory.Add(position.UniqueIdentifier);

            int evaluation;
            if (isThreeFoldRepetition || Game.Is50MovesRepetition())
            {
                evaluation = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else
            {
                evaluation = -QuiescenceSearch(ply + 1, -beta, -alpha);
            }

            // After making a move
            Game.HalfMovesWithoutCaptureOrPawnMove = oldValue;
            if (!isThreeFoldRepetition)
            {
                Game.PositionHashHistory.Remove(position.UniqueIdentifier);
            }
            position.UnmakeMove(move, gameState);

            PrintMove(ply, move, evaluation);

            // Fail-hard beta-cutoff
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough to discard this line");

                _tt.RecordHash(_ttMask, position, 0, ply, beta, NodeType.Beta, bestMove);

                return evaluation; // The refutation doesn't matter, since it'll be pruned
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
                bestMove = move;

                _pVTable[pvIndex] = move;
                CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);

                nodeType = NodeType.Exact;
            }
        }

        if (bestMove is null
            && !isThereAnyValidCapture
            && !MoveGenerator.CanGenerateAtLeastAValidMove(position))
        {
            var finalEval = Position.EvaluateFinalPosition(ply, position.IsInCheck());
            _tt.RecordHash(_ttMask, position, 0, ply, finalEval, NodeType.Exact);

            return finalEval;
        }

        _tt.RecordHash(_ttMask, position, 0, ply, alpha, nodeType, bestMove);

        return alpha;
    }
}
