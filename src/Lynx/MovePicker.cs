using Lynx.Model;
using System.Diagnostics;
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
    private readonly Move _ttMove;
    private readonly int _ply;
    private readonly bool _isQuiescence;
    private readonly bool _isInCheck;
    private readonly bool _isDoubleCheck;
    private readonly int _oppositeSideKing;
    private readonly int _checkerSquare;
    private readonly Bitboard _evasionTargets;
    private readonly Span<Move> _moves;
    private readonly Span<int> _moveScores;

    private Stage _stage;
    private int _count;
    private int _index;
    private bool _ttMoveGenerated;
    private Move _firstKiller;
    private Move _secondKiller;
    private Move _counterMove;

    public MovePicker(Engine engine, Position position, Move ttMove, Bitboard oppositeSideAttacks, int ply, Span<Move> moves, Span<int> moveScores, bool isQuiescence = false, bool isInCheck = false)
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

        // If enabled, we prepare to filter out most moves that can't get the king out of check
        if (isInCheck)
        {
            _isInCheck = true;

            var side = (int)position.Side;
            _oppositeSideKing = (int)Piece.K + Utils.PieceOffset(side);

            var _oppositeSideKingSquare = position.PieceBitboards[_oppositeSideKing].GetLS1BIndex();
            var checkers = position.AllSideAttackersTo(_oppositeSideKingSquare, side);

            Debug.Assert(checkers.NotEmpty(), "Not in check");

            _isDoubleCheck = !checkers.IsSinglePopulated();

            if (!_isDoubleCheck)
            {
                _checkerSquare = checkers.GetLS1BIndex();

                // Capturing the attacker or blocking its ray (only B, R and Q)
                _evasionTargets = checkers | Attacks.RaysBetween[_oppositeSideKingSquare][_checkerSquare];
            }
        }
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

                        // The TT move can come from a different position (hash collision), so it needs to be validated
                        if (_ttMove != 0
                            && MoveGenerator.IsPseudoLegal(_position, _ttMove, _oppositeSideAttacks)
                            && (!_isInCheck || IsEvasion(_ttMove)))
                        {
                            _ttMoveGenerated = true;

                            move = _ttMove;
                            score = EvaluationConstants.TTMoveScoreValue;

                            return true;
                        }

                        break;
                    }
                case Stage.GenerateNoisyMovesStage:
                    {
                        _stage = Stage.GoodNoisyMovesStage;

                        _count = MoveGenerator.GenerateAllCaptures(_position, _moves, _oppositeSideAttacks).Length;

                        if (_isInCheck)
                        {
                            _count = RemoveNonEvasions(_count);
                        }

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

                        // In qsearch this is the last stage and all generated moves (captures and promotions) are returned
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

                        if (TryGetValidQuietCandidate(_engine.GetKillerMove(_ply, 0), out var killer))
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

                        if (TryGetValidQuietCandidate(_engine.GetKillerMove(_ply, 1), out var killer)
                            && killer != _firstKiller)
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
                            if (TryGetValidQuietCandidate(_engine.GetCounterMove(_ply), out var counterMove)
                                && counterMove != _firstKiller && counterMove != _secondKiller)
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

                        // No more noisy moves, so their buffers can be reused
                        var quietMoves = MoveGenerator.GenerateAllQuiets(_position, _moves, _oppositeSideAttacks);

                        // Removing moves already returned in previous stages, preserving the order of the rest
                        _count = 0;
                        _index = 0;

                        foreach (var quietMove in quietMoves)
                        {
                            if (quietMove == _firstKiller
                                || quietMove == _secondKiller
                                || quietMove == _counterMove
                                || (_ttMoveGenerated && quietMove == _ttMove)
                                || (_isInCheck && !IsEvasion(quietMove)))
                            {
                                continue;
                            }

                            _moves[_count++] = quietMove;
                        }

                        for (int i = 0; i < _count; ++i)
                        {
                            _moveScores[i] = _engine.ScoreQuietMove(_position, _moves[i], _ply, _oppositeSideAttacks);
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

    /// <summary>
    /// Killer and counter moves keep the piece that moved (see <see cref="Engine.PieceAwareMove(Move, int)"/>), since they come from other positions.
    /// They're only valid if that piece is the one in the source square, if they're still quiet and if they're pseudo-legal
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool TryGetValidQuietCandidate(int pieceAwareMove, out Move move)
    {
        move = (Move)pieceAwareMove;
        var piece = pieceAwareMove >> 16;

        var board = _position.Board;

        if (move != 0
            && board[move.SourceSquare()] == piece
            && (board[move.TargetSquare()] == (int)Piece.None || move.IsCastle())   // DFRC castling is encoded as KxR
            && !move.IsEnPassant()
            && !move.IsPromotion()
            && !(_ttMoveGenerated && move == _ttMove)
            && MoveGenerator.IsPseudoLegal(_position, move, _oppositeSideAttacks)
            && (!_isInCheck || IsEvasion(move)))
        {
            return true;
        }

        move = default;

        return false;
    }

    /// <summary>
    /// Whether <paramref name="move"/> can get the king out of check: king moves, captures of the attacker (including en passant) and blocks.
    /// This doesn't check for pins nor discards moves where the king moves to another attacked square (i.e. not getting out of the ray)
    /// Only meant to be used when <see cref="_isInCheck"/> is true
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool IsEvasion(Move move)
    {
        if (_position.Board[move.SourceSquare()] == _oppositeSideKing)
        {
            return true;
        }

        if (_isDoubleCheck)
        {
            return false;
        }

        var targetSquare = move.TargetSquare();

        return _evasionTargets.GetBit(targetSquare)
            || (move.IsEnPassant() && targetSquare - Utils.PawhPushSquare((int)_position.Side) == _checkerSquare);   // Captured pawn is the checker, which isn't in the target square
    }

    /// <summary>
    /// Removes non-evasions (<see cref="IsEvasion(Move)"/>) from the first <paramref name="count"/> moves in _moves, keeping the order.
    /// </summary>
    /// <returns>New effective size of _moves</returns>
    private readonly int RemoveNonEvasions(int count)
    {
        var kept = 0;

        for (int i = 0; i < count; ++i)
        {
            var move = _moves[i];

            if (IsEvasion(move))
            {
                _moves[kept++] = move;
            }
        }

        return kept;
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

            // We save this move for the next stage where .TrySelectNext() is used (bad captures), which will continue from this _index
            // Sorting will produce no changes for the first bad capture
            if (Unsafe.Add(ref scoresRef, moveIndex) <= minScore)
            {
                break;
            }

            ++_index;

            // If the TT move was already returned in the TT stage, it'll have the highest score in the noisy moves list. So we skip it
            if (_ttMoveGenerated
                && moveIndex == 0
                && movesRef == _ttMove)
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
