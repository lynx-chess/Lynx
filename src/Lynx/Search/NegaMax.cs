using Lynx.Model;

namespace Lynx.Search
{
    public sealed partial class Search
    {
        /// <summary>
        /// NegaMax algorithm implementation using alpha-beta pruning, quiescence search and Iterative Deepeting Depth-First Search (IDDFS)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depthLimit"></param>
        /// <param name="depth"></param>
        /// <param name="alpha">
        /// Best score the Side to move can achieve, assuming best play by the opponent.
        /// Defaults to the worse possible score for Side to move, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
        /// Defaults to the worse possible score for Side to move's opponent, Int.MaxValue
        /// </param>
        /// <returns></returns>
        private (int Evaluation, int MaxDepth) NegaMax(Position position, int depthLimit, int depth, int alpha, int beta)
        {
            AbsoluteCancellationToken.ThrowIfCancellationRequested();
            if (depth > MinDepth)
            {
                CancellationToken.ThrowIfCancellationRequested();
            }

            ++Nodes;
            var pseudoLegalMoves = SortMoves(position.AllPossibleMoves(), position, depth);

            var pvIndex = Model.PVTable.Indexes[depth];
            var nextPvIndex = Model.PVTable.Indexes[depth + 1];
            PVTable[pvIndex] = new Move();  // After getting psuedoLegalMoves

            if (depth >= depthLimit)
            {
                foreach (var candidateMove in pseudoLegalMoves)
                {
                    if (new Position(position, candidateMove).WasProduceByAValidMove())
                    {
                        return QuiescenceSearch(position, depth, alpha, beta);
                    }
                }

                return (position.EvaluateFinalPosition(depth, PositionHistory, MovesWithoutCaptureOrPawnMove), depth);
            }

            Move? bestMove = null;
            bool isAnyMoveValid = false;

            foreach (var move in pseudoLegalMoves)
            {
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }
                isAnyMoveValid = true;

                PrintPreMove(position, depth, move);

                var oldValue = MovesWithoutCaptureOrPawnMove;
                MovesWithoutCaptureOrPawnMove = Update50movesRule(move);
                var repetitions = UpdatePositionHistory(newPosition);
                var (evaluation, bestMoveExistingMoveList) = NegaMax(newPosition, depthLimit, depth + 1, -beta, -alpha);
                MovesWithoutCaptureOrPawnMove = oldValue;
                RevertPositionHistory(newPosition, repetitions);

                evaluation = -evaluation;

                PrintMove(depth, move, evaluation);

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    _logger.Trace($"Pruning: {move} is enough");

                    //if (!move.IsCapture())
                    {
                        KillerMoves[1, depth] = KillerMoves[0, depth];
                        KillerMoves[0, depth] = move.EncodedMove;
                    }

                    return (beta, depth);    // TODO return evaluation?
                }

                if (evaluation > alpha)
                {
                    alpha = evaluation;
                    bestMove = move;

                    PVTable[pvIndex] = move;
                    CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - depth - 1);
                }
            }

            if (bestMove is null)
            {
                return (isAnyMoveValid ? alpha : position.EvaluateFinalPosition(depth, PositionHistory, MovesWithoutCaptureOrPawnMove), depth);
            }

            // Node fails low
            return (alpha, depth);
        }

        /// <summary>
        /// Quiescence search implementation, NegaMax alpha-beta style, fail-hard
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depth"></param>
        /// <param name="alpha">
        /// Best score White can achieve, assuming best play by Black.
        /// Defaults to the worse possible score for white, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Black can achieve, assuming best play by White
        /// Defaults to the works possible score for Black, Int.MaxValue
        /// </param>
        /// <returns></returns>
        public (int Evaluation, int MaxDepth) QuiescenceSearch(Position position, int depth, int alpha, int beta)
        {
            AbsoluteCancellationToken.ThrowIfCancellationRequested();
            //cancellationToken.ThrowIfCancellationRequested();

            ++Nodes;

            var pvIndex = Model.PVTable.Indexes[depth];
            var nextPvIndex = Model.PVTable.Indexes[depth + 1];
            PVTable[pvIndex] = new Move();

            var staticEvaluation = position.StaticEvaluation(PositionHistory, MovesWithoutCaptureOrPawnMove);

            // Fail-hard beta-cutoff (updating alpha after this check)
            if (staticEvaluation >= beta)
            {
                PrintMessage(depth - 1, "Pruning before starting quiescence search");
                return (staticEvaluation, depth);
            }

            // Better move
            if (staticEvaluation > alpha)
            {
                alpha = staticEvaluation;
            }

            if (depth >= Configuration.EngineSettings.QuiescenceSearchDepth)
            {
                return (alpha, depth);   // Alpha?
            }

            var generatedMoves = position.AllCapturesMoves();
            if (generatedMoves.Count == 0)
            {
                return (staticEvaluation, depth);  // TODO check if in check or drawn position
            }

            var movesToEvaluate = SortCaptures(generatedMoves, position, depth);

            Move? bestMove = null;
            bool isAnyMoveValid = false;

            foreach (var move in movesToEvaluate)
            {
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }
                isAnyMoveValid = true;

                PrintPreMove(position, depth, move, isQuiescence: true);

                var oldValue = MovesWithoutCaptureOrPawnMove;
                MovesWithoutCaptureOrPawnMove = Update50movesRule(move);
                var repetitions = UpdatePositionHistory(newPosition);
                var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch(newPosition, depth + 1, -beta, -alpha);
                MovesWithoutCaptureOrPawnMove = oldValue;
                RevertPositionHistory(newPosition, repetitions);

                evaluation = -evaluation;

                PrintMove(depth, move, evaluation);

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    _logger.Trace($"Pruning: {move} is enough to discard this line");
                    return (evaluation, depth); // The refutation doesn't matter, since it'll be pruned
                }

                if (evaluation > alpha)
                {
                    alpha = evaluation;
                    bestMove = move;

                    PVTable[pvIndex] = move;
                    CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - depth - 1);
                }
            }

            if (bestMove is null)
            {
                var eval = isAnyMoveValid || position.AllPossibleMoves().Any(move => new Position(position, move).WasProduceByAValidMove())
                    ? alpha
                    : position.EvaluateFinalPosition(depth, PositionHistory, MovesWithoutCaptureOrPawnMove);

                return (eval, depth);
            }

            // Node fails low
            return (alpha, depth);
        }
    }
}
