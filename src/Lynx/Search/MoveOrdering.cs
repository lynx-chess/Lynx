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
            var previousMove = Game.ReadMoveFromStack(ply - 1);
            Debug.Assert(previousMove != 0);
            var previousMovePiece = previousMove.Piece();
            var previousMoveTargetSquare = previousMove.TargetSquare();

            // Counter move history
            return BaseMoveScore
                + _quietHistory[move.Piece()][move.TargetSquare()]
                + _continuationHistory[ContinuationHistoryIndex(move.Piece(), move.TargetSquare(), previousMovePiece, previousMoveTargetSquare, 0)];
        }

        // History move or 0 if not found
        return BaseMoveScore
            + _quietHistory[move.Piece()][move.TargetSquare()];
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
    /// Quiet history, contination history, killers and counter moves
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMoveOrderingHeuristicsOnQuietBetaCutoff(int depth, int ply, ReadOnlySpan<int> visitedMoves, int visitedMovesCounter, int move, bool isRoot)
    {
        // 🔍 Quiet history moves
        // Doing this only in beta cutoffs (instead of when eval > alpha) was suggested by Sirius author
        var piece = move.Piece();
        var targetSquare = move.TargetSquare();

        _quietHistory[piece][targetSquare] = ScoreHistoryMove(
            _quietHistory[piece][targetSquare],
            HistoryBonus[depth]);

        int continuationHistoryIndex;
        int previousMovePiece = -1;
        int previousTargetSquare = -1;

        if (!isRoot)
        {
            // 🔍 Continuation history
            // - Counter move history (continuation history, ply - 1)
            var previousMove = Game.ReadMoveFromStack(ply - 1);
            Debug.Assert(previousMove != 0);

            previousMovePiece = previousMove.Piece();
            previousTargetSquare = previousMove.TargetSquare();

            continuationHistoryIndex = ContinuationHistoryIndex(piece, targetSquare, previousMovePiece, previousTargetSquare, 0);

            _continuationHistory[continuationHistoryIndex] = ScoreHistoryMove(
                _continuationHistory[continuationHistoryIndex],
                HistoryBonus[depth]);

            //    var previousPreviousMove = Game.MoveStack[ply - 2];
            //    var previousPreviousMovePiece = previousPreviousMove.Piece();
            //    var previousPreviousMoveTargetSquare = previousPreviousMove.TargetSquare();

            //    _continuationHistory[piece][targetSquare][1][previousPreviousMovePiece][previousPreviousMoveTargetSquare] = ScoreHistoryMove(
            //        _continuationHistory[piece][targetSquare][1][previousPreviousMovePiece][previousPreviousMoveTargetSquare],
            //        EvaluationConstants.HistoryBonus[depth]);
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

                if (!isRoot)
                {
                    // 🔍 Continuation history penalty / malus
                    continuationHistoryIndex = ContinuationHistoryIndex(visitedMovePiece, visitedMoveTargetSquare, previousMovePiece, previousTargetSquare, 0);

                    _continuationHistory[continuationHistoryIndex] = ScoreHistoryMove(
                        _continuationHistory[continuationHistoryIndex],
                        -HistoryBonus[depth]);
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
