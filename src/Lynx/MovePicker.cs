using Lynx.Model;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

internal ref struct MovePicker
{
    private enum Stage
    {
        TTMoveStage,

        GenerateNoisyMovesStage,

        GoodNoisyMovesStage,
        FirstKillerMoveStage,
        SecondKillerMoveStage,
        CounterMoveStage,
        BadNoisyMovesStage,

        GenerateQuietMovesStage,

        QuietMovesStage,

        EndStage,
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
    private Move _firstKiller;
    private Move _secondKiller;
    private Move _counterMove;

    public MovePicker(Engine engine, Position position, ShortMove ttMove, Bitboard oppositeSideAttacks, int ply, Span<Move> moves, Span<int> moveScores, bool isQuiescence = false)
    {
        _engine = engine;
        _position = position;
        _ttMove = ttMove;
        _oppositeSideAttacks = oppositeSideAttacks;
        _ply = ply;
        _isQuiescence = isQuiescence;
        _stage = isQuiescence ? Stage.GenerateNoisyMovesStage : Stage.TTMoveStage;
        _moves = moves;
        _moveScores = moveScores;
    }

    [SkipLocalsInit]
    public bool TryGetNext(out Move move, out int score)
    {
        while (true)
        {
            switch (_stage)
            {
                case Stage.TTMoveStage:
                    {
                        _stage = Stage.GenerateNoisyMovesStage;

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

                        break;
                    }
                case Stage.GenerateNoisyMovesStage:
                    {
                        _stage = Stage.GoodNoisyMovesStage;

                        // Scored lazily, after the TT move has been searched
                        _count = MoveGenerator.GenerateAllCaptures(_position, _moves, _oppositeSideAttacks).Length;

                        for (int i = 0; i < _count; ++i)
                        {
                            _moveScores[i] = _engine.ScoreMoveQSearch(_position, _moves[i], _ttMove);
                        }

                        break;
                    }
                case Stage.GoodNoisyMovesStage:
                    {
                        int nextThreshold;
                        Stage nextStage;

                        // In quiescence this is the last stage and all generated moves (captures) are returned
                        if (_isQuiescence)
                        {
                            nextStage = Stage.EndStage;
                            nextThreshold = int.MinValue;
                        }
                        else
                        {
                            nextStage = Stage.FirstKillerMoveStage;
                            nextThreshold = EvaluationConstants.GoodCaptureMoveCutoff;
                        }

                        if (TrySelectNext(nextThreshold, out move, out score))
                        {
                            return true;
                        }

                        _stage = nextStage;

                        break;
                    }
                case Stage.FirstKillerMoveStage:
                    {
                        _stage = Stage.SecondKillerMoveStage;

                        var killer = _engine.GetKillerMove(_ply, 0);
                        if (IsValidQuietCandidate(killer))
                        {
                            _firstKiller = killer;

                            move = killer;
                            score = EvaluationConstants.FirstKillerMoveValue;

                            return true;
                        }

                        break;
                    }
                case Stage.SecondKillerMoveStage:
                    {
                        _stage = Stage.CounterMoveStage;

                        var killer = _engine.GetKillerMove(_ply, 1);
                        if (killer != _firstKiller && IsValidQuietCandidate(killer))
                        {
                            _secondKiller = killer;

                            move = killer;
                            score = EvaluationConstants.SecondKillerMoveValue;

                            return true;
                        }

                        break;
                    }
                case Stage.CounterMoveStage:
                    {
                        _stage = Stage.BadNoisyMovesStage;

                        if (_ply >= 1)
                        {
                            var counterMove = _engine.GetCounterMove(_ply);
                            if (counterMove != _firstKiller && counterMove != _secondKiller && IsValidQuietCandidate(counterMove))
                            {
                                _counterMove = counterMove;

                                move = counterMove;
                                score = EvaluationConstants.CounterMoveValue;

                                return true;
                            }
                        }

                        break;
                    }
                case Stage.BadNoisyMovesStage:
                    {
                        if (TrySelectNext(int.MinValue, out move, out score))
                        {
                            return true;
                        }

                        _stage = Stage.GenerateQuietMovesStage;

                        break;
                    }
                case Stage.GenerateQuietMovesStage:
                    {
                        _stage = Stage.QuietMovesStage;

                        // Noisy moves are exhausted at this point, so their buffers can be reused
                        var quietMoves = MoveGenerator.GenerateAllQuiets(_position, _moves, _oppositeSideAttacks);

                        // Removing moves already returned in previous stages, preserving the order of the rest
                        _count = 0;
                        _index = 0;

                        foreach (var quietMove in quietMoves)
                        {
                            if (quietMove == _firstKiller
                                || quietMove == _secondKiller
                                || quietMove == _counterMove
                                || (_ttMoveGenerated && (ShortMove)quietMove == _ttMove))
                            {
                                continue;
                            }

                            _moves[_count++] = quietMove;
                        }

                        for (int i = 0; i < _count; ++i)
                        {
                            _moveScores[i] = _engine.ScoreQuietMove(_moves[i], _ply, _oppositeSideAttacks);
                        }

                        break;
                    }
                case Stage.QuietMovesStage:
                    {
                        if (TrySelectNext(int.MinValue, out move, out score))
                        {
                            return true;
                        }

                        _stage = Stage.EndStage;

                        break;
                    }
                default:
                    {
                        move = default;
                        score = default;

                        return false;
                    }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool IsValidQuietCandidate(Move move)
    {
        return move != 0
            && !(_ttMoveGenerated && (ShortMove)move == _ttMove)
            && MoveGenerator.IsPseudoLegal(_position, move, _oppositeSideAttacks);
    }

    private bool TrySelectNext(int minScore, out Move move, out int score)
    {
        ref var movesRef = ref MemoryMarshal.GetReference(_moves);
        ref var scoresRef = ref MemoryMarshal.GetReference(_moveScores);

        while (_index < _count)
        {
            var moveIndex = _index;

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

            // Not consumed: the next stage will continue from this index (repeating the sorting above is a no-op)
            if (Unsafe.Add(ref scoresRef, moveIndex) <= minScore)
            {
                break;
            }

            ++_index;

            // If the TT move was already returned in the TT stage, it'll have the highest score in the noisy moves list. So we skip it
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
