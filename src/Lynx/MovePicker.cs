using Lynx.Model;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Lynx;

internal enum Stage
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

/// <summary>
/// Staged move generation.
/// This struct can't hold managed references or byrefs (<see cref="Span{T}"/>) to prevent JIT from zeroing the whole struct
/// in <see cref="Engine.NegaMax"/> and <see cref="Engine.QuiescenceSearch"/> prologue on every call.
/// </summary>
internal unsafe ref struct MovePicker
{
    private readonly Bitboard _oppositeSideAttacks;
    private readonly ShortMove _ttMove;
    private readonly int _ply;
    private readonly bool _capturesOnly;
    private readonly bool _isInCheck;
    private readonly bool _isDoubleCheck;
    private readonly int _oppositeSideKing;
    private readonly int _checkerSquare;
    private readonly Bitboard _evasionTargets;
    private readonly Move* _moves;
    private readonly int* _moveScores;
    private readonly int _bufferLength;

    private Stage _stage;
    private int _count;
    private int _index;
    private bool _ttMoveGenerated;
    private Move _firstKiller;
    private Move _secondKiller;
    private Move _counterMove;

    internal readonly Stage Stage => _stage;

    public MovePicker(Position position, ShortMove ttMove, Bitboard oppositeSideAttacks, int ply, Move* moves, int* moveScores, int bufferLength, bool isInCheck, bool capturesOnly)
    {
        Debug.Assert(!(capturesOnly && isInCheck));

        _ttMove = ttMove;
        _oppositeSideAttacks = oppositeSideAttacks;
        _ply = ply;
        _capturesOnly = capturesOnly;
        _stage = _capturesOnly ? Stage.GenerateNoisyMovesStage : Stage.TTMoveStage;
        _moves = moves;
        _moveScores = moveScores;
        _bufferLength = bufferLength;

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
                _evasionTargets = checkers | Attacks.RaysBetween(_oppositeSideKingSquare, _checkerSquare);
            }
        }
    }

    public bool TryGetNext(Engine engine, Position position, out Move move, out int score)
    {
        while (true)
        {
            switch (_stage)
            {
                case Stage.TTMoveStage:
                    {
                        _stage = Stage.GenerateNoisyMovesStage;

                        var fullTTMove = MoveGenerator.GenerateFullTTMove(_ttMove, position, _oppositeSideAttacks);

                        if (fullTTMove != 0
                            && (ShortMove)fullTTMove == _ttMove
                            && MoveGenerator.IsPseudoLegal(position, fullTTMove, _oppositeSideAttacks)
                            && (!_isInCheck || IsEvasion(fullTTMove, position)))
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

                        _count = MoveGenerator.GenerateAllCaptures(position, _moves, _bufferLength, _oppositeSideAttacks);

                        if (_isInCheck)
                        {
                            _count = RemoveNonEvasions(_count, position);
                        }

                        for (int i = 0; i < _count; ++i)
                        {
                            _moveScores[i] = engine.ScoreMoveQSearch(position, _moves[i], _ttMove);
                        }

                        PadScores();

                        break;
                    }
                case Stage.GoodNoisyMovesStage:
                    {
                        int nextThreshold;
                        Stage nextStage;

                        // When generating only captures, this is the last stage and all generated moves (captures and promotions) are returned
                        if (_capturesOnly)
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

                        var killer = engine.GetKillerMove(_ply, 0);
                        if (IsValidQuietCandidate(killer, position))
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

                        var killer = engine.GetKillerMove(_ply, 1);
                        if (killer != _firstKiller && IsValidQuietCandidate(killer, position))
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
                            var counterMove = engine.GetCounterMove(_ply);
                            if (counterMove != _firstKiller && counterMove != _secondKiller && IsValidQuietCandidate(counterMove, position))
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
                        var quietMovesCount = MoveGenerator.GenerateAllQuiets(position, _moves, _bufferLength, _oppositeSideAttacks);

                        // Removing moves already returned in previous stages, preserving the order of the rest
                        _count = 0;
                        _index = 0;

                        for (int i = 0; i < quietMovesCount; ++i)
                        {
                            var quietMove = _moves[i];

                            if (quietMove == _firstKiller
                                || quietMove == _secondKiller
                                || quietMove == _counterMove
                                || (_ttMoveGenerated && (ShortMove)quietMove == _ttMove)
                                || (_isInCheck && !IsEvasion(quietMove, position)))
                            {
                                continue;
                            }

                            _moves[_count++] = quietMove;
                        }

                        for (int i = 0; i < _count; ++i)
                        {
                            _moveScores[i] = engine.ScoreQuietMove(_moves[i], _ply, _oppositeSideAttacks);
                        }

                        PadScores();

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
    private readonly bool IsValidQuietCandidate(Move move, Position position)
    {
        return move != 0
            && !(_ttMoveGenerated && (ShortMove)move == _ttMove)
            && MoveGenerator.IsPseudoLegal(position, move, _oppositeSideAttacks)
            && (!_isInCheck || IsEvasion(move, position));
    }

    /// <summary>
    /// Whether <paramref name="move"/> can get the king out of check: king moves, captures of the attacker (including en passant) and blocks.
    /// This doesn't check for pins nor discards moves where the king moves to another attacked square (i.e. not getting out of the ray)
    /// Only meant to be used when <see cref="_isInCheck"/> is true
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool IsEvasion(Move move, Position position)
    {
        if (move.Piece() == _oppositeSideKing)
        {
            return true;
        }

        if (_isDoubleCheck)
        {
            return false;
        }

        var targetSquare = move.TargetSquare();

        return _evasionTargets.GetBit(targetSquare)
            || (move.IsEnPassant() && targetSquare - (+8 - ((int)position.Side * 16)) == _checkerSquare);   // Captured pawn is the checker, which isn't in the target square
    }

    /// <summary>
    /// Removes non-evasions (<see cref="IsEvasion(Move, Position)"/>) from the first <paramref name="count"/> moves in _moves, keeping the order.
    /// </summary>
    /// <returns>New effective size of _moves</returns>
    private readonly int RemoveNonEvasions(int count, Position position)
    {
        var kept = 0;

        for (int i = 0; i < count; ++i)
        {
            var move = _moves[i];

            if (IsEvasion(move, position))
            {
                _moves[kept++] = move;
            }
        }

        return kept;
    }

    private bool TrySelectNext(int minScore, out Move move, out int score)
    {
        ref var movesRef = ref *_moves;
        ref var scoresRef = ref *_moveScores;

        var count = _count;

        while (_index < count)
        {
            var moveIndex = _index;

            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            var bestIndex = IndexOfFirstMax(new ReadOnlySpan<int>(_moveScores, _bufferLength), moveIndex, count);

            // Swap only once, at the end: perf improvement over original implementation
            if (bestIndex != moveIndex)
            {
                ref var moveI = ref Unsafe.Add(ref movesRef, moveIndex);
                ref var moveJ = ref Unsafe.Add(ref movesRef, bestIndex);
                ref var scoreI = ref Unsafe.Add(ref scoresRef, moveIndex);
                ref var scoreB = ref Unsafe.Add(ref scoresRef, bestIndex);

                (scoreI, scoreB, moveI, moveJ) = (scoreB, scoreI, moveJ, moveI);
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
                && (ShortMove)movesRef == _ttMove)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void PadScores()
    {
        if (Vector128.IsHardwareAccelerated)
        {
            Debug.Assert(_count + Vector128<int>.Count <= _bufferLength);

            Vector128.Create(int.MinValue).Store(_moveScores + _count);
        }
    }

    /// <summary>
    /// Index of the first highest score in [<paramref name="start"/>, <paramref name="count"/>), using Vector128 when it's hardware accelerated.
    /// The vectorized path reads up to (<see cref="Vector128{T}.Count"/> - 1) scores after <paramref name="count"/>,
    /// which are expected to be <see cref="int.MinValue"/> to never be chosen a highest score (see <see cref="PadScores"/>)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfFirstMax(ReadOnlySpan<int> scores, int start, int count)
    {
        if (Vector128.IsHardwareAccelerated)
        {
            return IndexOfFirstMaxVector128(scores, start, count);
        }

        return IndexOfFirstMaxScalar(scores, start, count);
    }

    /// <summary>
    /// <see cref="IndexOfFirstMax"/> using <see cref="Vector128{T}"/>: reads up to (<see cref="Vector128{T}.Count"/> - 1) scores after <paramref name="count"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfFirstMaxVector128(ReadOnlySpan<int> scores, int start, int count)
    {
        Debug.Assert(start >= 0 && start < count);
        Debug.Assert(count + Vector128<int>.Count - 1 <= scores.Length);
        Debug.Assert(!scores.Slice(count, Vector128<int>.Count - 1).ContainsAnyExcept(int.MinValue), "Missing padding");

        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        // Find the highest score in each lane
        var max = Vector128.LoadUnsafe(ref scoresRef, (nuint)start);
        for (int i = start + Vector128<int>.Count; i < count; i += Vector128<int>.Count)
        {
            max = Vector128.Max(max, Vector128.LoadUnsafe(ref scoresRef, (nuint)i));
        }

        // Copy the single highest score among all 4 lanes to all of them
        max = Vector128.Max(max, Vector128.Shuffle(max, Vector128.Create(2, 3, 0, 1)));
        max = Vector128.Max(max, Vector128.Shuffle(max, Vector128.Create(1, 0, 3, 2)));

        // Find the (first) index of the highest score
        var index = start;
        while (true)
        {
            var matches = Vector128.Equals(Vector128.LoadUnsafe(ref scoresRef, (nuint)index), max).ExtractMostSignificantBits();

            if (matches != 0)
            {
                return index
                    + BitOperations.TrailingZeroCount(matches); // Lane that contained the highest score
            }

            index += Vector128<int>.Count;
        }
    }

    /// <summary>
    /// Scalar implementation for <see cref="IndexOfFirstMax"/>: doesn't read anything after <paramref name="count"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfFirstMaxScalar(ReadOnlySpan<int> scores, int start, int count)
    {
        Debug.Assert(start >= 0 && start < count);

        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        var bestIndex = start;
        var bestScore = Unsafe.Add(ref scoresRef, start);

        for (int i = start + 1; i < count; ++i)
        {
            var score = Unsafe.Add(ref scoresRef, i);

            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
}
