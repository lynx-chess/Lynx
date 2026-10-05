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

        int ScoreMove(Engine engine, Position position, Move move, int ply, Bitboard oppositeSideAttacks, ShortMove bestMoveTTCandidate);
    }

    private sealed class TTStage : IMovegenStage
    {
        public Span<Move> GenerateMoves(Engine engine, ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply, Span<Move> movePool)
        {
            var fullTTMove = MoveGenerator.GenerateFullTTMove(ttMove, position, oppositeSideAttacks);

            // TT entries can be corrupted (hash collisions), e.g. with promotion bits set for a non-pawn move
            if (fullTTMove != 0
                && (ShortMove)fullTTMove == ttMove
                && MoveGenerator.IsPseudoLegal(position, fullTTMove, oppositeSideAttacks))
            {
                movePool[0] = fullTTMove;
                return movePool[0..1];
            }

            return [];
        }

        public int ScoreMove(Engine engine, Position position, Move move, int ply, Bitboard oppositeSideAttacks, ShortMove bestMoveTTCandidate)
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

        public int ScoreMove(Engine engine, Position position, Move move, int ply, Bitboard oppositeSideAttacks, ShortMove bestMoveTTCandidate)
        {
            return engine.ScoreMove(position, move, ply, oppositeSideAttacks, bestMoveTTCandidate);
        }
    }
}
