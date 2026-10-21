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
    // Killer and counter moves also keep the piece that moved, see Engine.PieceAwareMove
    private readonly int[] _lastQuietMoves = new int[MaxPly];
    private readonly int[] _beforeLastQuietMoves = new int[MaxPly];

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
            var (previousMove, previousPiece) = _engine.Game.ReadMoveAndPieceFromStack(ply - 1);
            _engine._counterMoves[(previousPiece * 64) + previousMove.TargetSquare()] = _lastQuietMoves[ply];
        }

        var movePicker = new MovePicker(_engine, position, _lastMoves[ply], oppositeSideAttacks, ply, moves, moveScores, isInCheck: position.IsInCheck());

        long nodes = 0;

        while (movePicker.TryGetNext(out var move, out _))
        {
            var piece = position.Board[move.SourceSquare()];
            var isQuiet = move.CapturedPiece(position.Board, (int)position.Side) == (int)Piece.None && !move.IsPromotion();

            var gameState = position.MakeMove(move);

            if (position.WasProduceByAValidMove())
            {
                _engine.Game.Stack(ply).Move = move;
                _engine.Game.Stack(ply).Piece = piece;

                nodes += Perft(position, depth - 1, ply + 1);
            }

            position.UnmakeMove(move, gameState);

            _lastMoves[ply] = move;

            if (isQuiet)
            {
                _beforeLastQuietMoves[ply] = _lastQuietMoves[ply];
                _lastQuietMoves[ply] = Engine.PieceAwareMove(move, piece);
            }
        }

        return nodes;
    }
}
