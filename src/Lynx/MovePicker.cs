using Lynx.Model;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

internal ref struct MovePicker
{
    private enum Stage
    {
        TTMoveStage,
        GenerateAllMovesStage,
        AllMovesStage
    }

    private readonly Engine _engine;
    private readonly Position _position;
    private readonly Bitboard _oppositeSideAttacks;
    private readonly ShortMove _ttMove;
    private readonly int _ply;
    private readonly bool _isQuiescence;
    private readonly Span<Move> _moves;
    private readonly Span<int> _moveScores;

    private Stage _stage;
    private int _count;
    private int _index;
    private bool _ttMoveGenerated;

    public MovePicker(Engine engine, Position position, ShortMove ttMove, Bitboard oppositeSideAttacks, int ply, Span<Move> moves, Span<int> moveScores, bool isQuiescence = false)
    {
        _engine = engine;
        _position = position;
        _ttMove = ttMove;
        _oppositeSideAttacks = oppositeSideAttacks;
        _ply = ply;
        _isQuiescence = isQuiescence;
        _stage = isQuiescence ? Stage.GenerateAllMovesStage : Stage.TTMoveStage;
        _moves = moves;
        _moveScores = moveScores;
    }

    public bool TryGetNext(out Move move, out int score)
    {
        if (_stage == Stage.TTMoveStage)
        {
            _stage = Stage.GenerateAllMovesStage;

            var fullTTMove = MoveGenerator.GenerateFullTTMove(_ttMove, _position, _oppositeSideAttacks);

            // TT entries can be corrupted (hash collisions), e.g. with promotion bits set for a non-pawn move
            if (fullTTMove != 0
                && (ShortMove)fullTTMove == _ttMove
                && MoveGenerator.IsPseudoLegal(_position, fullTTMove, _oppositeSideAttacks))
            {
                _ttMoveGenerated = true;

                move = fullTTMove;
                score = EvaluationConstants.TTMoveScoreValue;

                return true;
            }
        }

        if (_stage == Stage.GenerateAllMovesStage)
        {
            _stage = Stage.AllMovesStage;

            if (_isQuiescence)
            {
                _count = MoveGenerator.GenerateAllCaptures(_position, _moves, _oppositeSideAttacks).Length;

                for (int i = 0; i < _count; ++i)
                {
                    _moveScores[i] = _engine.ScoreMoveQSearch(_position, _moves[i], _ttMove);
                }
            }
            else
            {
                _count = MoveGenerator.GenerateAllMoves(_position, _moves, _oppositeSideAttacks).Length;

                for (int i = 0; i < _count; ++i)
                {
                    _moveScores[i] = _engine.ScoreMove(_position, _moves[i], _ply, _oppositeSideAttacks, _ttMove);
                }
            }
        }

        ref var movesRef = ref MemoryMarshal.GetReference(_moves);
        ref var scoresRef = ref MemoryMarshal.GetReference(_moveScores);

        while (_index < _count)
        {
            var moveIndex = _index++;

            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            for (int j = moveIndex + 1; j < _count; j++)
            {
                ref var moveI = ref Unsafe.Add(ref movesRef, moveIndex);
                ref var moveJ = ref Unsafe.Add(ref movesRef, j);
                ref var scoreI = ref Unsafe.Add(ref scoresRef, moveIndex);
                ref var scoreJ = ref Unsafe.Add(ref scoresRef, j);

                if (scoreJ > scoreI)
                {
                    (scoreI, scoreJ, moveI, moveJ) = (scoreJ, scoreI, moveJ, moveI);
                }
            }

            // If the TT move was already returned in the TT stage, it'll have the highest score in AllMovesStage. So we skip it
            if (_ttMoveGenerated
                && moveIndex == 0
                && (ShortMove)Unsafe.Add(ref movesRef, 0) == _ttMove)
            {
                continue;
            }

            move = Unsafe.Add(ref movesRef, moveIndex);
            score = Unsafe.Add(ref scoresRef, moveIndex);

            return true;
        }

        move = default;
        score = default;

        return false;
    }
}
