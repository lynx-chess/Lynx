using Lynx.Model;
using System.Diagnostics;

namespace Lynx;

public sealed partial class Engine
{
    private static readonly IMovegenStage[] _stages =
    [
        new TTStage(),

       new OtherStage(),
    ];

    private interface IMovegenStage
    {
        Span<Move> GenerateMoves(Engine engine, ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply, Span<Move> movePool);

        int ScoreMove(Engine engine, Position position, Move move, int ply, Bitboard oppositeSideAttacks);
    }

    private sealed class TTStage : IMovegenStage
    {
        public Span<Move> GenerateMoves(Engine engine, ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply, Span<Move> movePool)
        {
            var fullTTMove = MoveGenerator.GenerateFullTTMove(ttMove, position, oppositeSideAttacks);

            if (fullTTMove != 0 && MoveGenerator.IsPseudoLegal(position, fullTTMove, oppositeSideAttacks))
            {
                movePool[0] = fullTTMove;
                return movePool[0..1];
            }

            Debug.Assert(!MoveGenerator.IsPseudoLegal(position, fullTTMove, oppositeSideAttacks));

            return [];
        }

        public int ScoreMove(Engine engine, Position position, Move move, int ply, Bitboard oppositeSideAttacks)
        {
            return EvaluationConstants.TTMoveScoreValue;
        }
    }

    private sealed class OtherStage : IMovegenStage
    {
        public Span<Move> GenerateMoves(Engine engine, ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply, Span<Move> movePool)
        {
            return MoveGenerator.GenerateAllMoves(position, movePool, oppositeSideAttacks);
        }

        public int ScoreMove(Engine engine, Position position, Move move, int ply, Bitboard oppositeSideAttacks)
        {
            return engine.ScoreMove(position, move, ply, oppositeSideAttacks);
        }
    }
}
