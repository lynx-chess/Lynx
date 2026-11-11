using Lynx.Model;

namespace Lynx.Test;

/// <summary>
/// Perft implementation that uses <see cref="MovePicker"/> (staged move generation) instead of generating all moves at once.
/// To make the TT, killer and counter move stages kick in, the picker is fed with stale moves from previously visited nodes of the same ply.
/// Not thread safe, create one per test.
/// </summary>
internal sealed class PerftMovePickerWalker
{
    private const int MaxPly = 16;

    private readonly Move[] _lastMoves = new Move[MaxPly];
    private readonly Move[] _lastQuietMoves = new Move[MaxPly];
    private readonly Move[] _beforeLastQuietMoves = new Move[MaxPly];

    private readonly Engine _engine;

    public PerftMovePickerWalker(Engine engine)
    {
        _engine = engine;
    }

    public long Perft(Position position, int depth, int ply)
    {
        if (depth == 0)
        {
            return 1;
        }

        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);
        position.CalculateThreats(ref evaluationContext);
        var oppositeSideAttacks = evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<int> moveScores = stackalloc int[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        // Stale moves from other nodes: they're valid here sometimes, so all stages (and their deduplication) get exercised
        _engine._killerMoves[ply * 2] = _lastQuietMoves[ply];
        _engine._killerMoves[(ply * 2) + 1] = _beforeLastQuietMoves[ply];

        if (ply >= 1)
        {
            var previousMove = _engine.Game.Stack(ply - 1).Move;
            _engine._counterMoves[(previousMove.Piece() * 64) + previousMove.TargetSquare()] = _lastQuietMoves[ply];
        }

        var movePicker = new MovePicker(_engine, position, (ShortMove)_lastMoves[ply], oppositeSideAttacks, ply, moves, moveScores, isInCheck: position.IsInCheck(), capturesOnly: false);

        long nodes = 0;

        while (movePicker.TryGetNext(out var move, out _))
        {
            var gameState = position.MakeMove(move);

            if (position.WasProduceByAValidMove())
            {
                _engine.Game.Stack(ply).Move = move;

                nodes += Perft(position, depth - 1, ply + 1);
            }

            position.UnmakeMove(move, gameState);

            _lastMoves[ply] = move;

            if (move.CapturedPiece() == (int)Piece.None && !move.IsPromotion())
            {
                _beforeLastQuietMoves[ply] = _lastQuietMoves[ply];
                _lastQuietMoves[ply] = move;
            }
        }

        return nodes;
    }
}
