using Lynx.Model;
using System.Diagnostics;
using System.Linq;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        /// <summary>
        /// NegaMax algorithm implementation using alpha-beta prunning
        /// </summary>
        /// <param name="position"></param>
        /// <param name="plies"></param>
        /// <param name="alpha">
        /// Best score the Side to move can achieve, assuming best play by the opponent.
        /// Defaults to the worse possible score for Side to move, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
        /// Defaults to the worse possible score for Side to move's opponent, Int.MaxValue
        /// </param>
        /// <returns></returns>
        public static (int Evaluation, Result MoveList) NegaMax_AlphaBeta(Position position, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies == Configuration.EngineSettings.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return (position.StaticEvaluation_NegaMax(new(), default), result);
                }
                else
                {
                    return (position.EvaluateFinalPosition_NegaMax(plies, new(), default), result);
                }
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
            {
                var move = pseudoLegalMoves[moveIndex];
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move);

                var (evaluation, bestMoveExistingMoveList) = NegaMax_AlphaBeta(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                PrintMove(plies, move, evaluation, position);

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                alpha = Max(alpha, evaluation);

                if (beta <= alpha)
                {
                    Logger.Trace($"Prunning: {bestMove} is enough");
                    break;
                }
            }

            if (bestMove is not null)
            {
                Debug.Assert(existingMoveList is not null);
                existingMoveList!.Moves.Add(bestMove!.Value);

                return (maxEval, existingMoveList);
            }
            else
            {
                return (position.EvaluateFinalPosition_NegaMax(plies, new(), default), new Result());
            }
        }

        /// <summary>
        /// NegaMax algorithm implementation using alpha-beta prunning and quiescence search
        /// </summary>
        /// <param name="position"></param>
        /// <param name="plies"></param>
        /// <param name="alpha">
        /// Best score the Side to move can achieve, assuming best play by the opponent.
        /// Defaults to the worse possible score for Side to move, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
        /// Defaults to the worse possible score for Side to move's opponent, Int.MaxValue
        /// </param>
        /// <returns></returns>
        public static (int Evaluation, Result MoveList) NegaMax_AlphaBeta_Quiescence_InitialImplementation(Position position, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies >= Configuration.EngineSettings.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return QuiescenceSearch_NegaMax_AlphaBeta_InitialImplementation(position, plies + 1, alpha, beta);
                }
                else
                {
                    return (position.EvaluateFinalPosition_NegaMax(plies, new(), default), result);
                }
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
            {
                var move = pseudoLegalMoves[moveIndex];
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move);

                var (evaluation, bestMoveExistingMoveList) = NegaMax_AlphaBeta_Quiescence_InitialImplementation(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                PrintMove(plies, move, evaluation, position);

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                alpha = Max(alpha, evaluation);

                if (beta <= alpha)
                {
                    Logger.Trace($"Prunning: {bestMove} is enough");
                    break;
                }
            }

            if (bestMove is not null)
            {
                Debug.Assert(existingMoveList is not null);
                existingMoveList!.Moves.Add(bestMove!.Value);

                return (maxEval, existingMoveList);
            }
            else
            {
                return (position.EvaluateFinalPosition_NegaMax(plies, new(), default), new Result());
            }
        }

        /// <summary>
        /// Quiescence search implementation, NegaMax alpha-beta style
        /// </summary>
        /// <param name="position"></param>
        /// <param name="plies"></param>
        /// <param name="alpha">
        /// Best score White can achieve, assuming best play by Black.
        /// Defaults to the worse possible score for white, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Black can achieve, assuming best play by White
        /// Defaults to the works possible score for Black, Int.MaxValue
        /// </param>
        /// <returns></returns>
        public static (int Evaluation, Result MoveList) QuiescenceSearch_NegaMax_AlphaBeta_InitialImplementation(Position position, int plies, int alpha, int beta)
        {
            var staticEvaluation = position.StaticEvaluation_NegaMax(new(), default);

            alpha = Max(alpha, staticEvaluation);

            if (beta <= alpha || plies >= Configuration.EngineSettings.QuiescenceSearchDepth)
            {
                PrintMessage(plies - 1, "Prunning before starting quiescence search");
                return (alpha, new Result());
            }

            var movesToEvaluate = position.AllCapturesMoves();

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < movesToEvaluate.Count; ++moveIndex)
            {
                var move = movesToEvaluate[moveIndex];
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move, isQuiescence: true);

                var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_NegaMax_AlphaBeta_InitialImplementation(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                alpha = Max(alpha, evaluation);

                PrintMove(plies, move, evaluation, position, isQuiescence: true, beta <= alpha);

                if (beta <= alpha)
                {
                    break;
                }
            }

            if (bestMove is not null)
            {
                Debug.Assert(existingMoveList is not null);
                existingMoveList!.Moves.Add(bestMove!.Value);

                return (maxEval, existingMoveList);
            }
            else
            {
                return movesToEvaluate.Count > 0
                    ? (position.EvaluateFinalPosition_NegaMax(plies, new(), default), new Result())
                    : (staticEvaluation, new Result());
            }
        }
    }
}
