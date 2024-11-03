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
    internal int ScoreMove(Move move, int ply, bool isNotQSearch, ShortMove bestMoveTTCandidate = default)
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

        if (isNotQSearch)
        {
            var thisPlyKillerMoves = _killerMoves[ply];

            // 1st killer move
            if (thisPlyKillerMoves[0] == move)
            {
                return FirstKillerMoveValue;
            }

            // 2nd killer move
            if (thisPlyKillerMoves[1] == move)
            {
                return SecondKillerMoveValue;
            }

            // 3rd killer move
            if (thisPlyKillerMoves[2] == move)
            {
                return ThirdKillerMoveValue;
            }

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

                if (ply >= 2)
                {
                    var previousPreviousMove = Game.ReadMoveFromStack(ply - 2);
                    Debug.Assert(previousPreviousMove != 0);
                    var previousPreviousMovePiece = previousPreviousMove.Piece();
                    var previousPreviousMoveTargetSquare = previousPreviousMove.TargetSquare();

                    var piece = move.Piece();
                    var targetSquare = move.TargetSquare();

                    // Includes counter-move and follow-up move history
                    return BaseMoveScore
                        + _quietHistory[move.Piece()][move.TargetSquare()]
                        + _continuationHistory[ContinuationHistoryIndex(piece, targetSquare, previousMovePiece, previousMoveTargetSquare, 0)]
                        + _continuationHistory[ContinuationHistoryIndex(piece, targetSquare, previousPreviousMovePiece, previousPreviousMoveTargetSquare, 1)];
                }
                else
                {
                    // Includes counter-move history
                    return BaseMoveScore
                        + _quietHistory[move.Piece()][move.TargetSquare()]
                        + _continuationHistory[ContinuationHistoryIndex(move.Piece(), move.TargetSquare(), previousMovePiece, previousMoveTargetSquare, 0)];
                }
            }

            // History move or 0 if not found
            return BaseMoveScore
                + _quietHistory[move.Piece()][move.TargetSquare()];
        }

        return BaseMoveScore;
    }

    /// <summary>
    /// Quiet history, contination history, killers and counter moves
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMoveOrderingHeuristicsOnQuietBetaCutoff(int depth, int ply, ReadOnlySpan<int> visitedMoves, int visitedMovesCounter, int move)
    {
        // 🔍 Quiet history moves
        // Doing this only in beta cutoffs (instead of when eval > alpha) was suggested by Sirius author
        var piece = move.Piece();
        var targetSquare = move.TargetSquare();

        _quietHistory[piece][targetSquare] = ScoreHistoryMove(
            _quietHistory[piece][targetSquare],
            HistoryBonus[depth]);

        int continuationHistoryIndex;
        int followUpHistoryIndex;
        int previousMovePiece = -1;
        int previousTargetSquare = -1;
        int previousPreviousMovePiece = -1;
        int previousPreviousMoveTargetSquare = -1;

        // 🔍 Continuation history
        // - Counter move history (continuation history, ply - 1)
        if (ply >= 1)
        {
            var previousMove = Game.ReadMoveFromStack(ply - 1);
            Debug.Assert(previousMove != 0);

            previousMovePiece = previousMove.Piece();
            previousTargetSquare = previousMove.TargetSquare();

            continuationHistoryIndex = ContinuationHistoryIndex(piece, targetSquare, previousMovePiece, previousTargetSquare, 0);
            _continuationHistory[continuationHistoryIndex] = ScoreHistoryMove(
                _continuationHistory[continuationHistoryIndex],
                HistoryBonus[depth]);

            // - Follow-up history (continuation history, ply - 2)
            if (ply >= 2)
            {
                var previousPreviousMove = Game.ReadMoveFromStack(ply - 2);
                previousPreviousMovePiece = previousPreviousMove.Piece();
                previousPreviousMoveTargetSquare = previousPreviousMove.TargetSquare();

                followUpHistoryIndex = ContinuationHistoryIndex(piece, targetSquare, previousPreviousMovePiece, previousPreviousMoveTargetSquare, 1);
                _continuationHistory[followUpHistoryIndex] = ScoreHistoryMove(
                    _continuationHistory[followUpHistoryIndex],
                    HistoryBonus[depth]);
            }
        }

        for (int i = 0; i < visitedMovesCounter - 1; ++i)
        {
            var visitedMove = visitedMoves[i];

            if (!visitedMove.IsCapture())
            {
                var visitedMovePiece = visitedMove.Piece();
                var visitedMoveTargetSquare = visitedMove.TargetSquare();

                // 🔍 Quiet history penalty / malus
                // When a quiet move fails high, penalize previous visited quiet moves
                _quietHistory[visitedMovePiece][visitedMoveTargetSquare] = ScoreHistoryMove(
                    _quietHistory[visitedMovePiece][visitedMoveTargetSquare],
                    -HistoryBonus[depth]);

                // 🔍 Continuation history penalty / malus
                if (ply >= 1)
                {
                    continuationHistoryIndex = ContinuationHistoryIndex(visitedMovePiece, visitedMoveTargetSquare, previousMovePiece, previousTargetSquare, 0);

                    _continuationHistory[continuationHistoryIndex] = ScoreHistoryMove(
                        _continuationHistory[continuationHistoryIndex],
                        -HistoryBonus[depth]);

                    // 🔍 Follow-up history penalty / malus
                    if (ply >= 2)
                    {
                        followUpHistoryIndex = ContinuationHistoryIndex(visitedMovePiece, visitedMoveTargetSquare, previousPreviousMovePiece, previousPreviousMoveTargetSquare, 1);

                        _continuationHistory[followUpHistoryIndex] = ScoreHistoryMove(
                            _continuationHistory[followUpHistoryIndex],
                            -HistoryBonus[depth]);
                    }
                }
            }
        }

        var thisPlyKillerMoves = _killerMoves[ply];
        if (move.PromotedPiece() == default && move != thisPlyKillerMoves[0])
        {
            // 🔍 Killer moves
            if (move != thisPlyKillerMoves[1])
            {
                thisPlyKillerMoves[2] = thisPlyKillerMoves[1];
            }

            thisPlyKillerMoves[1] = thisPlyKillerMoves[0];
            thisPlyKillerMoves[0] = move;

            if (ply >= 1)
            {
                // 🔍 Countermoves - fails to fix the bug and remove killer moves condition, see  https://github.com/lynx-chess/Lynx/pull/944
                _counterMoves[CounterMoveIndex(previousMovePiece, previousTargetSquare)] = move;
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
