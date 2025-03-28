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

        int movePiece = move.Piece();
        int moveTargetSquare = move.TargetSquare();

        if (isCapture)
        {
            var baseCaptureScore = (isPromotion || move.IsEnPassant() || SEE.IsGoodCapture(Game.CurrentPosition, move))
                ? GoodCaptureMoveBaseScoreValue
                : BadCaptureMoveBaseScoreValue;

            var capturedPiece = move.CapturedPiece();

            Debug.Assert(capturedPiece != (int)Piece.K && capturedPiece != (int)Piece.k, $"{move.UCIString()} capturing king is generated in position {Game.CurrentPosition.FEN()}");

            return baseCaptureScore
                + MostValueableVictimLeastValuableAttacker[movePiece][capturedPiece]
                //+ EvaluationConstants.MVV_PieceValues[capturedPiece]
                + _captureHistory[CaptureHistoryIndex(movePiece, moveTargetSquare, capturedPiece)];
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

        int counterMoveHistoryScore = 0;
        int followUpHistoryScore = 0;

        if (ply >= 1)
        {
            var previousMove = Game.ReadMoveFromStack(ply - 1);
            Debug.Assert(previousMove != 0);
            var previousMovePiece = previousMove.Piece();
            var previousMoveTargetSquare = previousMove.TargetSquare();

            // Countermove
            if (_counterMoves[CounterMoveIndex(previousMovePiece, previousMoveTargetSquare)] == move)
            {
                return CounterMoveValue;
            }

            // Countermove history
            counterMoveHistoryScore = _continuationHistory[ContinuationHistoryIndex(move.Piece(), move.TargetSquare(), previousMovePiece, previousMoveTargetSquare, CounterMoveHistoryIndex)];

            if (ply >= 2)
            {
                var previousPreviousMove = Game.ReadMoveFromStack(ply - 2);
                Debug.Assert(previousPreviousMove != 0);
                var previousPreviousMovePiece = previousPreviousMove.Piece();
                var previousPreviousMoveTargetSquare = previousPreviousMove.TargetSquare();

                // Follow-up history
                followUpHistoryScore = _continuationHistory[ContinuationHistoryIndex(movePiece, moveTargetSquare, previousPreviousMovePiece, previousPreviousMoveTargetSquare, FollowUpMoveHistoryIndex)];
            }
        }

        return BaseMoveScore
            + _quietHistory[movePiece][moveTargetSquare]
            + counterMoveHistoryScore
            + followUpHistoryScore;
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

            Debug.Assert(capturedPiece != (int)Piece.K && capturedPiece != (int)Piece.k, $"{move.UCIString()} capturing king is generated in position {Game.CurrentPosition.FEN()}");

            return baseCaptureScore
                + MostValueableVictimLeastValuableAttacker[piece][capturedPiece]
                //+ EvaluationConstants.MVV_PieceValues[capturedPiece]
                + _captureHistory[CaptureHistoryIndex(piece, move.TargetSquare(), capturedPiece)];
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
        // 🔍 Quiet history moves
        // Doing this only in beta cutoffs (instead of when eval > alpha) was suggested by Sirius author
        var piece = move.Piece();
        var targetSquare = move.TargetSquare();

        int rawHistoryBonus = HistoryBonus[depth];

        _quietHistory[piece][targetSquare] = ScoreHistoryMove(
            _quietHistory[piece][targetSquare],
            rawHistoryBonus);

        int counterMoveHistoryIndex;
        int followUpHistoryIndex;
        int previousMovePiece = -1;
        int previousMoveTargetSquare = -1;
        int previousPreviousMovePiece = -1;
        int previousPreviousMoveTargetSquare = -1;

        if (!isRoot)
        {
            // 🔍 Continuation history
            // - Countermove history (continuation history, ply - 1)
            var previousMove = Game.ReadMoveFromStack(ply - 1);
            Debug.Assert(previousMove != 0);

            previousMovePiece = previousMove.Piece();
            previousMoveTargetSquare = previousMove.TargetSquare();

            counterMoveHistoryIndex = ContinuationHistoryIndex(piece, targetSquare, previousMovePiece, previousMoveTargetSquare, CounterMoveHistoryIndex);

            _continuationHistory[counterMoveHistoryIndex] = ScoreHistoryMove(
                _continuationHistory[counterMoveHistoryIndex],
                rawHistoryBonus);

            // - Follow-up history (continuation history, ply - 2)
            if (ply > 2)
            {
                var previousPreviousMove = Game.ReadMoveFromStack(ply - 2);
                Debug.Assert(previousPreviousMove != 0);
                previousPreviousMovePiece = previousPreviousMove.Piece();
                previousPreviousMoveTargetSquare = previousPreviousMove.TargetSquare();

                followUpHistoryIndex = ContinuationHistoryIndex(piece, targetSquare, previousPreviousMovePiece, previousPreviousMoveTargetSquare, FollowUpMoveHistoryIndex);

                _continuationHistory[followUpHistoryIndex] = ScoreHistoryMove(
                    _continuationHistory[followUpHistoryIndex],
                    rawHistoryBonus);
            }
        }

        // 🔍 History penalty/malus
        for (int i = 0; i < visitedMovesCounter - 1; ++i)
        {
            var visitedMove = visitedMoves[i];

            if (!visitedMove.IsCapture())
            {
                var visitedMovePiece = visitedMove.Piece();
                var visitedMoveTargetSquare = visitedMove.TargetSquare();

                // Quiet history
                // When a quiet move fails high, penalize previous visited quiet moves
                _quietHistory[visitedMovePiece][visitedMoveTargetSquare] = ScoreHistoryMove(
                    _quietHistory[visitedMovePiece][visitedMoveTargetSquare],
                    -rawHistoryBonus);

                // Continuation history
                if (!isRoot)
                {
                    counterMoveHistoryIndex = ContinuationHistoryIndex(visitedMovePiece, visitedMoveTargetSquare, previousMovePiece, previousMoveTargetSquare, CounterMoveHistoryIndex);

                    _continuationHistory[counterMoveHistoryIndex] = ScoreHistoryMove(
                        _continuationHistory[counterMoveHistoryIndex],
                        -rawHistoryBonus);

                    if (ply >= 2)
                    {
                        followUpHistoryIndex = ContinuationHistoryIndex(visitedMovePiece, visitedMoveTargetSquare, previousPreviousMovePiece, previousPreviousMoveTargetSquare, FollowUpMoveHistoryIndex);

                        _continuationHistory[followUpHistoryIndex] = ScoreHistoryMove(
                            _continuationHistory[followUpHistoryIndex],
                            -rawHistoryBonus);
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
                _counterMoves[CounterMoveIndex(previousMovePiece, previousMoveTargetSquare)] = move;
            }
        }
    }

    /// <summary>
    /// Capture history
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMoveOrderingHeuristicsOnCaptureBetaCutoff(int depth, ReadOnlySpan<int> visitedMoves, int visitedMovesCounter, int move)
    {
        var piece = move.Piece();
        var targetSquare = move.TargetSquare();
        var capturedPiece = move.CapturedPiece();

        var captureHistoryIndex = CaptureHistoryIndex(piece, targetSquare, capturedPiece);
        _captureHistory[captureHistoryIndex] = ScoreHistoryMove(
            _captureHistory[captureHistoryIndex],
            HistoryBonus[depth]);

        // 🔍 Capture history penalty/malus
        // When a capture fails high, penalize previous visited captures
        for (int i = 0; i < visitedMovesCounter; ++i)
        {
            var visitedMove = visitedMoves[i];

            if (visitedMove.IsCapture())
            {
                var visitedMovePiece = visitedMove.Piece();
                var visitedMoveTargetSquare = visitedMove.TargetSquare();
                var visitedMoveCapturedPiece = visitedMove.CapturedPiece();

                captureHistoryIndex = CaptureHistoryIndex(visitedMovePiece, visitedMoveTargetSquare, visitedMoveCapturedPiece);

                _captureHistory[captureHistoryIndex] = ScoreHistoryMove(
                    _captureHistory[captureHistoryIndex],
                    -HistoryBonus[depth]);
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
