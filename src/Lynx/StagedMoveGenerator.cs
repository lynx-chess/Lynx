using Lynx.Model;

namespace Lynx;

public sealed partial class Engine
{
    private static readonly IMovegenStage[] _stages =
    [
        new TTStage(),

       new OtherStage(),
   ];

    public IEnumerable<(Move move, int score)> GenerateStagedMoves(ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply)
    {
        foreach (var stage in _stages)
        {
            foreach (var pair in stage.GenerateMoves(this, ttMove, position, oppositeSideAttacks, ply))
            {
                yield return pair;
            }
        }
    }

    private interface IMovegenStage
    {
        IEnumerable<(Move Move, int Score)> GenerateMoves(Engine engine, ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply);
    }

    private sealed class TTStage : IMovegenStage
    {
        public IEnumerable<(Move Move, int Score)> GenerateMoves(Engine engine, ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply)
        {
            var fullTTMove = MoveGenerator.GenerateFullTTMove(ttMove, position, oppositeSideAttacks);

            if (fullTTMove != 0)
            {
                return [(fullTTMove, EvaluationConstants.TTMoveScoreValue)];
            }

            return [];
        }
    }

    private sealed class OtherStage : IMovegenStage
    {
        public IEnumerable<(Move Move, int Score)> GenerateMoves(Engine engine, ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply)
        {
            Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
            var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves, oppositeSideAttacks);

            Span<int> moveScores = stackalloc int[pseudoLegalMoves.Length];
            for (int i = 0; i < pseudoLegalMoves.Length; ++i)
            {
                moveScores[i] = engine.ScoreMove(position, pseudoLegalMoves[i], ply, oppositeSideAttacks, ttMove);
            }

            var arr1 = pseudoLegalMoves.ToArray();
            var arr2 = moveScores.ToArray();

            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            foreach (var pair in Sort(arr1, arr2))
            {
                yield return pair;
            }


            static IEnumerable<(Move Move, int Score)> Sort(int[] pseudoLegalMoves, int[] moveScores)
            {
                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Length; ++moveIndex)
                {
                    for (int j = moveIndex + 1; j < pseudoLegalMoves.Length; j++)
                    {
                        ref var moveI = ref pseudoLegalMoves[moveIndex];
                        ref var moveJ = ref pseudoLegalMoves[j];
                        ref var scoreI = ref moveScores[moveIndex];
                        ref var scoreJ = ref moveScores[j];

                        if (scoreJ > scoreI)
                        {
                            (scoreI, scoreJ, moveI, moveJ) = (scoreJ, scoreI, moveJ, moveI);
                        }
                    }

                    // Value copy
                    var move = pseudoLegalMoves[moveIndex];
                    var moveScore = moveScores[moveIndex];

                    yield return (move, moveScore);
                }
            }

            //    return pseudoLegalMoves.ToArray();
        }
    }
}
