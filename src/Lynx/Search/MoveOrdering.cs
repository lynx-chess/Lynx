﻿using Lynx.Model;
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
    internal int ScoreMove(Position position, Move move, int ply, ref EvaluationContext evaluationContext, ShortMove bestMoveTTCandidate = default)
    {
        if ((ShortMove)move == bestMoveTTCandidate)
        {
            return TTMoveScoreValue;
        }

        var promotedPiece = move.PromotedPiece();
        var isPromotion = promotedPiece != default;
        var capturedPiece = move.CapturedPiece();
        var isCapture = capturedPiece != (int)Piece.None;

        if (!isCapture && !isPromotion)
        {
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

            if (ply >= 1)
            {
                // Countermove
                if (CounterMove(ply - 1) == move)
                {
                    return CounterMoveValue;
                }

                // Counter move history
                return BaseMoveScore
                    + QuietHistoryEntry(position, move, ref evaluationContext)
                    + ContinuationHistoryEntry(move.Piece(), move.TargetSquare(), ply - 1);
            }

            // History move or 0 if not found
            return BaseMoveScore
                + QuietHistoryEntry(position, move, ref evaluationContext);
        }

        // Queen promotion
        if ((promotedPiece + 2) % 6 == 0)
        {
            if (isCapture)
            {
                return QueenPromotionWithCaptureBaseValue + capturedPiece;
            }

            return PromotionMoveScoreValue
                + (SEE.HasPositiveScore(Game.CurrentPosition, move)
                    ? GoodCaptureMoveBaseScoreValue
                    : BadCaptureMoveBaseScoreValue);
        }

        if (isCapture)
        {
            var piece = move.Piece();
            Debug.Assert(capturedPiece != (int)Piece.K && capturedPiece != (int)Piece.k,
                $"{move.UCIString()} capturing king is generated in position {Game.CurrentPosition.FEN(Game.HalfMovesWithoutCaptureOrPawnMove)}");

            var baseCaptureScore = (isPromotion || move.IsEnPassant() || SEE.IsGoodCapture(Game.CurrentPosition, move))
                ? GoodCaptureMoveBaseScoreValue
                : BadCaptureMoveBaseScoreValue;

            return baseCaptureScore
                + MostValueableVictimLeastValuableAttacker[piece][capturedPiece]
                //+ EvaluationConstants.MVV_PieceValues[capturedPiece]
                + CaptureHistoryEntry(move);
        }

        if (isPromotion)
        {
            return PromotionMoveScoreValue;
        }

        _logger.Warn("Unexpected move while scoring: {Move}", move.UCIString());

        return BaseMoveScore;
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
        var capturedPiece = move.CapturedPiece();
        var isCapture = capturedPiece != (int)Piece.None;

        // Queen promotion
        if ((promotedPiece + 2) % 6 == 0)
        {
            if (isCapture)
            {
                return QueenPromotionWithCaptureBaseValue + capturedPiece;
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
    /// Quiet history, contination history, killers and counter moves
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMoveOrderingHeuristicsOnQuietBetaCutoff(Position position, int depth, int ply, ReadOnlySpan<int> visitedMoves, int visitedMovesCounter, int move, bool isRoot, bool pvNode, ref EvaluationContext evaluationContext)
    {
        var piece = move.Piece();
        var targetSquare = move.TargetSquare();

        // Idea by Alayan in Ethereal: don't update history on low depths
        if (depth >= Configuration.EngineSettings.History_MinDepth || visitedMovesCounter >= Configuration.EngineSettings.History_MinVisitedMoves)
        {
            // 🔍 Quiet history moves
            // Doing this only in beta cutoffs (instead of when eval > alpha) was suggested by Sirius author
            int rawHistoryBonus = HistoryBonus[depth];
            int rawHistoryMalus = HistoryMalus[depth];

            ref var quietHistoryEntry = ref QuietHistoryEntry(position, move, ref evaluationContext);
            quietHistoryEntry = ScoreHistoryMove(quietHistoryEntry, rawHistoryBonus);

            if (!isRoot)
            {
                // 🔍 Continuation history
                // - Counter move history (continuation history, ply - 1)
                ref var continuationHistoryEntry = ref ContinuationHistoryEntry(piece, targetSquare, ply - 1);
                continuationHistoryEntry = ScoreHistoryMove(continuationHistoryEntry, rawHistoryBonus);
            }

            for (int i = 0; i < visitedMovesCounter; ++i)
            {
                var visitedMove = visitedMoves[i];
                var capturedPiece = visitedMove.CapturedPiece();

                if (capturedPiece == (int)Piece.None)
                {
                    var visitedMovePiece = visitedMove.Piece();
                    var visitedMoveTargetSquare = visitedMove.TargetSquare();

                    // 🔍 Quiet history penalty / malus
                    // When a quiet move fails high, penalize previous visited quiet moves
                    quietHistoryEntry = ref QuietHistoryEntry(position, visitedMove, ref evaluationContext);
                    quietHistoryEntry = ScoreHistoryMove(quietHistoryEntry, -rawHistoryMalus);

                    if (!isRoot)
                    {
                        // 🔍 Continuation history penalty / malus
                        ref var continuationHistoryEntry = ref ContinuationHistoryEntry(visitedMovePiece, visitedMoveTargetSquare, ply - 1);
                        continuationHistoryEntry = ScoreHistoryMove(continuationHistoryEntry, -rawHistoryMalus);
                    }
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
        var rawHistoryMalus = HistoryMalus[depth];

        ref var captureHistoryEntry = ref CaptureHistoryEntry(move);
        captureHistoryEntry = ScoreHistoryMove(captureHistoryEntry, rawHistoryBonus);

        // 🔍 Capture history penalty/malus
        // When a capture fails high, penalize previous visited captures
        for (int i = 0; i < visitedMovesCounter; ++i)
        {
            var visitedMove = visitedMoves[i];
            var capturedPiece = visitedMove.CapturedPiece();

            if (capturedPiece != (int)Piece.None)
            {
                ref var captureHistoryVisitedMove = ref CaptureHistoryEntry(visitedMove);
                captureHistoryVisitedMove = ScoreHistoryMove(captureHistoryVisitedMove, -rawHistoryMalus);
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
