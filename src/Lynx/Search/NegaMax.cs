using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Authentication;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// NegaMax algorithm implementation using alpha-beta pruning and quiescence search
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="ply"></param>
    /// <param name="alpha">
    /// Best score the Side to move can achieve, assuming best play by the opponent.
    /// </param>
    /// <param name="beta">
    /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
    /// </param>
    /// <returns></returns>
    private int NegaMax(int depth, int ply, int alpha, int beta, bool parentWasNullMove = false)
    {
        var position = Game.CurrentPosition;

        // Prevents runtime failure in case depth is increased due to check extension, since we're using ply when calculating pvTable index,
        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth {0} reached", Configuration.EngineSettings.MaxDepth);
            return position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove).Score;
        }

        _maxDepthReached[ply] = ply;
        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        bool pvNode = beta - alpha > 1;

        // Before any time-consuming operations
        _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        // 🔍 Improving heuristic: the current position has a better static evaluation than
        // the previous evaluation from the same side (ply - 2).
        // When true, we can:
        // - Prune more aggressively when evaluation is too high: current position is even getter
        // - Prune less aggressively when evaluation is low low: uncertainty on how bad the position really is
        bool improving = false;


        bool isInCheck = position.IsInCheck();
        int staticEval = int.MaxValue;
        int phase = int.MaxValue;

        if (depth <= 0)
        {
            return 0;
        }
        else if (!pvNode)
        {
                (staticEval, phase) = position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove);

            Game.UpdateStaticEvalInStack(ply, staticEval);

            if (ply >= 2)
            {
                var evalDiff = staticEval - Game.ReadStaticEvalFromStack(ply - 2);
                improving = evalDiff >= 0;
            }
        }

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves);

        Span<int> moveScores = stackalloc int[pseudoLegalMoves.Length];

        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            moveScores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: true, 0);
        }

        var nodeType = NodeType.Alpha;
        int bestScore = EvaluationConstants.MinEval;
        Move? bestMove = null;
        bool isAnyMoveValid = false;

        Span<Move> visitedMoves = stackalloc Move[pseudoLegalMoves.Length];
        int visitedMovesCounter = 0;

        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Length; ++moveIndex)
        {
            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            for (int j = moveIndex + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (moveScores[j] > moveScores[moveIndex])
                {
                    (moveScores[moveIndex], moveScores[j], pseudoLegalMoves[moveIndex], pseudoLegalMoves[j]) = (moveScores[j], moveScores[moveIndex], pseudoLegalMoves[j], pseudoLegalMoves[moveIndex]);
                }
            }

            var move = pseudoLegalMoves[moveIndex];

            var gameState = position.MakeMove(move);

            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            visitedMoves[visitedMovesCounter] = move;

            ++_nodes;
            isAnyMoveValid = true;
            var isCapture = move.IsCapture();

            PrintPreMove(position, ply, move);

            // Before making a move
            var oldHalfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;
            var canBeRepetition = Game.Update50movesRule(move, isCapture);
            Game.AddToPositionHashHistory(position.UniqueIdentifier);
            Game.UpdateMoveinStack(ply, move);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void RevertMove()
            {
                Game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
                Game.RemoveFromPositionHashHistory();
                position.UnmakeMove(move, gameState);
            }

            int score;
            if (canBeRepetition && (Game.IsThreefoldRepetition() || Game.Is50MovesRepetition()))
            {
                score = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else if (visitedMovesCounter == 0)
            {
                PrefetchTTEntry();
#pragma warning disable S2234 // Arguments should be passed in the same order as the method parameters
                score = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
#pragma warning restore S2234 // Arguments should be passed in the same order as the method parameters
            }
            else
            {
                PrefetchTTEntry();

                int reduction = 0;

                // 🔍 Late Move Reduction (LMR) - search with reduced depth
                // Impl. based on Ciekce (Stormphrax) and Martin (Motor) advice, and Stormphrax & Akimbo implementations
                if (visitedMovesCounter >= (pvNode ? Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves : Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves - 1)
                    && depth >= Configuration.EngineSettings.LMR_MinDepth
                    && !isCapture)
                {
                    reduction = EvaluationConstants.LMRReductions[depth][visitedMovesCounter];

                    if (!improving)
                    {
                        ++reduction;
                    }

                    // Don't allow LMR to drop into qsearch or increase the depth
                    // depth - 1 - depth +2 = 1, min depth we want
                    reduction = Math.Clamp(reduction, 0, depth - 2);
                }

                // Search with reduced depth
                score = -NegaMax(depth - 1 - reduction, ply + 1, -alpha - 1, -alpha);

                // 🔍 Principal Variation Search (PVS)
                if (score > alpha && reduction > 0)
                {
                    // Optimistic search, validating that the rest of the moves are worse than bestmove.
                    // It should produce more cutoffs and therefore be faster.
                    // https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm

                    // Search with full depth but narrowed score bandwidth
                    score = -NegaMax(depth - 1, ply + 1, -alpha - 1, -alpha);
                }

                if (score > alpha && score < beta)
                {
                    // PVS Hypothesis invalidated -> search with full depth and full score bandwidth
#pragma warning disable S2234 // Arguments should be passed in the same order as the method parameters
                    score = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
#pragma warning restore S2234 // Arguments should be passed in the same order as the method parameters
                }
            }

            // After making a move
            RevertMove();

            PrintMove(position, ply, move, score);

            if (score > bestScore)
            {
                bestScore = score;

                // Improving alpha
                if (score > alpha)
                {
                    alpha = score;
                    bestMove = move;

                    if (pvNode)
                    {
                        _pVTable[pvIndex] = move;
                        CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);
                    }

                    nodeType = NodeType.Exact;
                }

                // Beta-cutoff - refutation found, no need to keep searching this line
                if (score >= beta)
                {
                    PrintMessage($"Pruning: {move} is enough");

                    _tt.RecordHash(position, staticEval, depth, ply, bestScore, NodeType.Beta, bestMove);

                    return bestScore;
                }
            }

            ++visitedMovesCounter;
        }

        if (!isAnyMoveValid)
        {
            Debug.Assert(bestMove is null);

            var finalEval = Position.EvaluateFinalPosition(ply, isInCheck);
            _tt.RecordHash(position, staticEval, depth, ply, finalEval, NodeType.Exact);

            return finalEval;
        }

        _tt.RecordHash(position, staticEval, depth, ply, bestScore, nodeType, bestMove);

        // Node fails low
        return bestScore;
    }
}
