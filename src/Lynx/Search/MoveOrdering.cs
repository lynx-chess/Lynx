using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using static Lynx.EvaluationConstants;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// Returns the score evaluation of a move taking into account <paramref name="bestMoveTTCandidate"/>, <see cref="MostValueableVictimLeastValuableAttacker"/>, <see cref="_killerMoves"/> and <see cref="_quietHistory"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int ScoreMove(Move move, int ply, ShortMove bestMoveTTCandidate = default)
    {
        if ((ShortMove)move == bestMoveTTCandidate)
        {
            return TTMoveScoreValue;
        }

        var promotedPiece = move.PromotedPiece();
        var isPromotion = promotedPiece != default;
        var isCapture = move.IsCapture();

        // Queen promotion
        if ((promotedPiece + 2) % 6 == 0)
        {
            if (isCapture)
            {
                return QueenPromotionWithCaptureBaseValue + move.CapturedPiece();
            }

            return PromotionMoveScoreValue
                + (SEE.HasPositiveScore(Game.CurrentPosition, move)
                    ? GoodCaptureMoveBaseScoreValue
                    : BadCaptureMoveBaseScoreValue);
        }

        if (isCapture)
        {
            var baseCaptureScore = (isPromotion || move.IsEnPassant() || SEE.IsGoodCapture(Game.CurrentPosition, move))
                ? GoodCaptureMoveBaseScoreValue
                : BadCaptureMoveBaseScoreValue;

            var capturedPiece = move.CapturedPiece();

            Debug.Assert(capturedPiece != (int)Piece.K && capturedPiece != (int)Piece.k,
                $"{move.UCIString()} capturing king is generated in position {Game.CurrentPosition.FEN(Game.HalfMovesWithoutCaptureOrPawnMove)}");

            return baseCaptureScore
                + MostValueableVictimLeastValuableAttacker[move.Piece()][capturedPiece]
                //+ EvaluationConstants.MVV_PieceValues[capturedPiece]
                + CaptureHistoryEntry(move);
        }

        if (isPromotion)
        {
            return PromotionMoveScoreValue;
        }

        var thisPlyKillerMovesBaseIndex = ply * 2;

        // 1st killer move
        if (_killerMoves[thisPlyKillerMovesBaseIndex] == move)
        {
            return FirstKillerMoveValue;
        }

        // 2nd killer move
        if (_killerMoves[thisPlyKillerMovesBaseIndex + 1] == move)
        {
            return SecondKillerMoveValue;
        }

        var piece = move.Piece();
        var targetSquare = move.TargetSquare();

        if (ply >= 2)
        {
            // Countermove
            if (CounterMove(ply - 1) == move)
            {
                return CounterMoveValue;
            }

            // Countermove + follow-up history
            return BaseMoveScore
                + _quietHistory[piece][targetSquare]
                + CounterMoveHistoryEntry(piece, targetSquare, ply)
                + (FollowUpHistoryEntry(piece, targetSquare, ply) / 2);
        }

        if (ply >= 1)
        {
            // Countermove
            if (CounterMove(ply - 1) == move)
            {
                return CounterMoveValue;
            }

            // Countermove history
            return BaseMoveScore
                + _quietHistory[piece][targetSquare]
                + CounterMoveHistoryEntry(piece, targetSquare, ply);
        }

        // History move or 0 if not found
        return BaseMoveScore
            + _quietHistory[piece][targetSquare];
    }

    /// <summary>
    /// Returns the score evaluation of a move taking into account <paramref name="bestMoveTTCandidate"/>, <see cref="MostValueableVictimLeastValuableAttacker"/>, <see cref="_killerMoves"/> and <see cref="_quietHistory"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int ScoreMoveQSearch(Move move, ShortMove bestMoveTTCandidate = default)
    {
        if ((ShortMove)move == bestMoveTTCandidate)
        {
            return TTMoveScoreValue;
        }

        var promotedPiece = move.PromotedPiece();
        var isPromotion = promotedPiece != default;
        var isCapture = move.IsCapture();

        // Queen promotion
        if ((promotedPiece + 2) % 6 == 0)
        {
            if (isCapture)
            {
                return QueenPromotionWithCaptureBaseValue + move.CapturedPiece();
            }

            return PromotionMoveScoreValue
                + (SEE.HasPositiveScore(Game.CurrentPosition, move)
                    ? GoodCaptureMoveBaseScoreValue
                    : BadCaptureMoveBaseScoreValue);
        }

        if (isCapture)
        {
            var baseCaptureScore = (isPromotion || move.IsEnPassant() || SEE.IsGoodCapture(Game.CurrentPosition, move))
                ? GoodCaptureMoveBaseScoreValue
                : BadCaptureMoveBaseScoreValue;

            var piece = move.Piece();
            var capturedPiece = move.CapturedPiece();

            Debug.Assert(capturedPiece != (int)Piece.K && capturedPiece != (int)Piece.k,
                $"{move.UCIString()} capturing king is generated in position {Game.CurrentPosition.FEN(Game.HalfMovesWithoutCaptureOrPawnMove)}");

            return baseCaptureScore
                + MostValueableVictimLeastValuableAttacker[piece][capturedPiece]
                //+ EvaluationConstants.MVV_PieceValues[capturedPiece]
                + CaptureHistoryEntry(move);
        }

        if (isPromotion)
        {
            return PromotionMoveScoreValue;
        }

        return BaseMoveScore;
    }

    /// <summary>
    /// Quiet history, continuation history, killers and counter moves
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMoveOrderingHeuristicsOnQuietBetaCutoff(int depth, int ply, ReadOnlySpan<int> visitedMoves, int visitedMovesCounter, int move, bool isRoot, bool pvNode)
    {
        var piece = move.Piece();
        var targetSquare = move.TargetSquare();

        // 🔍 Quiet history moves
        // Doing this only in beta cutoffs (instead of when eval > alpha) was suggested by Sirius author
        int rawHistoryBonus = HistoryBonus[depth];

        ref var quietHistoryEntry = ref _quietHistory[piece][targetSquare];
        quietHistoryEntry = ScoreHistoryMove(quietHistoryEntry, rawHistoryBonus);

        if (ply >= 2)
        {
            // 🔍 Continuation history
            // - Countermove history (continuation history, ply - 1)
            ref var counterMoveHistoryEntry = ref CounterMoveHistoryEntry(piece, targetSquare, ply);
            counterMoveHistoryEntry = ScoreHistoryMove(counterMoveHistoryEntry, rawHistoryBonus);

            // - Follow-up history (continuation history, ply - 1)
            ref var followUpHistoryEntry = ref FollowUpHistoryEntry(piece, targetSquare, ply);
            followUpHistoryEntry = ScoreHistoryMove(followUpHistoryEntry, rawHistoryBonus);
        }
        else if (!isRoot)
        {
            // 🔍 Continuation history
            // - Counter move history (continuation history, ply - 1)
            ref var counterMoveHistoryEntry = ref CounterMoveHistoryEntry(piece, targetSquare, ply);
            counterMoveHistoryEntry = ScoreHistoryMove(counterMoveHistoryEntry, rawHistoryBonus);
        }

        for (int i = 0; i < visitedMovesCounter; ++i)
        {
            var visitedMove = visitedMoves[i];

            if (!visitedMove.IsCapture())
            {
                var visitedMovePiece = visitedMove.Piece();
                var visitedMoveTargetSquare = visitedMove.TargetSquare();

                // Quiet history
                // When a quiet move fails high, penalize previous visited quiet moves
                quietHistoryEntry = ref _quietHistory[visitedMovePiece][visitedMoveTargetSquare];
                quietHistoryEntry = ScoreHistoryMove(quietHistoryEntry, -rawHistoryBonus);

                // Continuation history
                if (ply >= 2)
                {
                    ref var counterMoveHistoryEntry = ref CounterMoveHistoryEntry(visitedMovePiece, visitedMoveTargetSquare, ply);
                    counterMoveHistoryEntry = ScoreHistoryMove(counterMoveHistoryEntry, -rawHistoryBonus);

                    ref var followUpHistoryEntry = ref FollowUpHistoryEntry(visitedMovePiece, visitedMoveTargetSquare, ply);
                    followUpHistoryEntry = ScoreHistoryMove(followUpHistoryEntry, -rawHistoryBonus);
                }
                else if (!isRoot)
                {
                    ref var counterMoveHistoryEntry = ref CounterMoveHistoryEntry(visitedMovePiece, visitedMoveTargetSquare, ply);
                    counterMoveHistoryEntry = ScoreHistoryMove(counterMoveHistoryEntry, -rawHistoryBonus);
                }
            }
        }

        var thisPlyKillerMovesBaseIndex = ply * 2;
        var firstKillerMove = _killerMoves[thisPlyKillerMovesBaseIndex];

        if (move.PromotedPiece() == default && move != firstKillerMove)
        {
            // 🔍 Killer moves
            if (move != _killerMoves[thisPlyKillerMovesBaseIndex + 1])
            {
                _killerMoves[thisPlyKillerMovesBaseIndex + 1] = firstKillerMove;
            }

            _killerMoves[thisPlyKillerMovesBaseIndex] = move;

            if (!isRoot && (depth >= Configuration.EngineSettings.CounterMoves_MinDepth || pvNode))
            {
                // 🔍 Countermoves - fails to fix the bug and remove killer moves condition, see  https://github.com/lynx-chess/Lynx/pull/944
                ref var counterMove = ref CounterMove(ply - 1);
                counterMove = move;
            }
        }
    }

    /// <summary>
    /// Capture history
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMoveOrderingHeuristicsOnCaptureBetaCutoff(int depth, ReadOnlySpan<int> visitedMoves, int visitedMovesCounter, int move)
    {
        var rawHistoryBonus = HistoryBonus[depth];

        ref var captureHistoryEntry = ref CaptureHistoryEntry(move);
        captureHistoryEntry = ScoreHistoryMove(captureHistoryEntry, rawHistoryBonus);

        // 🔍 Capture history penalty/malus
        // When a capture fails high, penalize previous visited captures
        for (int i = 0; i < visitedMovesCounter; ++i)
        {
            var visitedMove = visitedMoves[i];

            if (visitedMove.IsCapture())
            {
                ref var captureHistoryVisitedMove = ref CaptureHistoryEntry(visitedMove);
                captureHistoryVisitedMove = ScoreHistoryMove(captureHistoryVisitedMove, -rawHistoryBonus);
            }
        }
    }

    /// <summary>
    /// Soft caps history score
    /// Formula taken from EP discord, https://discord.com/channels/1132289356011405342/1132289356447625298/1141102105847922839
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ScoreHistoryMove(int score, int rawHistoryBonus)
    {
        return score + rawHistoryBonus - (score * Math.Abs(rawHistoryBonus) / Configuration.EngineSettings.History_MaxMoveValue);
    }
}
