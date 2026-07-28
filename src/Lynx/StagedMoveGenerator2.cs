using Lynx.Model;

namespace Lynx;

public static class StagedMoveGenerator2
{
    private static readonly MovegenStage[] _stages =
    [
        new TTStage(),

        new OtherStage(),

        new FinalStage(),
    ];

    internal static readonly MovegenStage StartStage = _stages[0];

    internal static readonly MovegenStage EndStage = _stages[^1];

    internal abstract class MovegenStage
    {
        public abstract MovegenStage NextStage();

        public abstract Span<Move> GenerateMoves(short ttMove, Move fullTTMove, Position position, ref EvaluationContext evaluationContext, Span<Move> movePool);
    }

    private sealed class TTStage : MovegenStage
    {
        public override MovegenStage NextStage() => _stages[1];

        public override Span<Move> GenerateMoves(short ttMove, Move fullTTMove, Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
        {
            int localIndex = 0;

            if (fullTTMove != 0)
            {
                movePool[localIndex++] = fullTTMove;
            }

            return movePool[..localIndex];
        }
    }

    private sealed class FinalStage : MovegenStage
    {
        public override MovegenStage NextStage() => throw new NotSupportedException();

        public override Span<Move> GenerateMoves(short ttMove, Move fullTTMove, Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
             => throw new NotSupportedException();
    }

    private sealed class OtherStage : MovegenStage
    {
        public override MovegenStage NextStage() => _stages[2];

        public override Span<Move> GenerateMoves(short ttMove, Move fullTTMove, Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
        {
            var generatedMoves = MoveGenerator.GenerateAllMoves(position, ref evaluationContext, movePool);

            if (fullTTMove == 0)
            {
                return generatedMoves;
            }

            int localIndex = 0;
            for (int i = 0; i < generatedMoves.Length; ++i)
            {
                var move = generatedMoves[i];
                if (move != fullTTMove)
                {
                    movePool[localIndex++] = move;
                }
            }

            return movePool[..localIndex];
        }
    }
}
