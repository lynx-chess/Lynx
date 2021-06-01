using Lynx.Model;

namespace Lynx.Search
{
	public static partial class SearchAlgorithms
	{
		public static int Theoretical_MiniMax(Position position, int depth, bool isWhite)
		{
			static bool IsGameFinished(Position position) => throw new();

			if (depth == 0 || IsGameFinished(position))
			{
				return position.StaticEvaluation();
			}

			if (isWhite)
			{
				var maxEval = MinValue;

				var pseudoLegalMoves = position.AllPossibleMoves();
				for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
				{
					var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
					if (!newPosition.IsValid())
					{
						continue;
					}

					var eval = Theoretical_MiniMax(newPosition, depth - 1, isWhite: false);
					maxEval = Max(maxEval, eval);
				}

				return maxEval;
			}
			else
			{
				var minEval = MaxValue;

				var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
				for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
				{
					var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
					if (!newPosition.IsValid())
					{
						continue;
					}

					var eval = Theoretical_MiniMax(newPosition, depth - 1, isWhite: true);
					minEval = Min(minEval, eval);
				}

				return minEval;
			}
		}

		public static int Theoretical_MiniMax_AlphaBeta(Position position, int depth, bool isWhite, int alpha = MinValue, int beta = MaxValue)
		{
			static bool IsGameFinished(Position position) => throw new();

			if (depth == 0 || IsGameFinished(position))
			{
				return position.StaticEvaluation();
			}

			if (isWhite)
			{
				var maxEval = MinValue;

				var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
				for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
				{
					var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
					if (!newPosition.IsValid())
					{
						continue;
					}

					var eval = Theoretical_MiniMax_AlphaBeta(newPosition, depth - 1, isWhite: true, alpha, beta);
					maxEval = Max(maxEval, eval);
					alpha = Max(alpha, eval);

					if (beta <= alpha)
					{
						break;
					}
				}

				return maxEval;
			}
			else
			{
				var minEval = MaxValue;

				var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
				for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
				{
					var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
					if (!newPosition.IsValid())
					{
						continue;
					}

					var eval = Theoretical_MiniMax_AlphaBeta(newPosition, depth - 1, isWhite: false, alpha, beta);
					minEval = Min(minEval, eval);
					beta = Min(beta, eval);

					if (beta <= alpha)
					{
						break;
					}
				}

				return minEval;
			}
		}

		public static int Theoretical_NegaMax_AlphaBeta(Position position, int depth, bool isWhite, int alpha = int.MinValue, int beta = int.MaxValue)
		{
			static bool IsGameFinished(Position position) => throw new();

			if (depth == 0 || IsGameFinished(position))
			{
				return position.StaticEvaluation_NegaMax();
			}

			var maxEval = int.MinValue;
			var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
			for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
			{
				var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
				if (!newPosition.IsValid())
				{
					continue;
				}

				var eval = -Theoretical_NegaMax_AlphaBeta(newPosition, depth - 1, isWhite: !isWhite, -beta, -alpha);

				maxEval = Max(maxEval, eval);   // Branch prediction optimized - should have started with most likely positions
				alpha = Max(alpha, eval);       // maxTODO optimize branch prediction -> Should alpha be generally greater than eval?

				if (beta <= alpha)
				{
					break;
				}
			}

			return maxEval;
		}
	}
}