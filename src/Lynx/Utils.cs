using Lynx.Model;
using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

public static class Utils
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Side.White -> 0
    /// Side.Black -> 6
    /// </summary>
    /// <param name="side"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PieceOffset(Side side) => PieceOffset((int)side);

    /// <summary>
    /// Side.White -> 0
    /// Side.Black -> 6
    /// </summary>
    /// <param name="side"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PieceOffset(int side)
    {
        GuardAgainstSideBoth(side);

        return 6 - (6 * side);
    }

    /// <summary>
    /// Side.White -> 0
    /// Side.Black -> 6
    /// </summary>
    /// <param name="isWhite"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PieceOffset(bool isWhite)
    {
        return 6 - (6 * Unsafe.As<bool, byte>(ref isWhite));
    }

    /// <summary>
    /// Side.Black -> Side.White
    /// Side.White -> Side.Black
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int OppositeSide(Side side) => OppositeSide((int)side);

    /// <summary>
    /// Side.Black -> Side.White
    /// Side.White -> Side.Black
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int OppositeSide(int side)
    {
        GuardAgainstSideBoth(side);

        return side ^ 1;     // or  (int)Side.White - side
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ShortCastleRookTargetSquare(Side side) => ShortCastleRookTargetSquare((int)side);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ShortCastleRookTargetSquare(int side)
    {
        GuardAgainstSideBoth(side);

        return Constants.BlackShortCastleRookSquare + (7 * 8 * side);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LongCastleRookTargetSquare(Side side) => LongCastleRookTargetSquare((int)side);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LongCastleRookTargetSquare(int side)
    {
        GuardAgainstSideBoth(side);

        return Constants.BlackLongCastleRookSquare + (7 * 8 * side);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ShortCastleRookSourceSquare(Side side) => ShortCastleRookSourceSquare((int)side);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ShortCastleRookSourceSquare(int side)
    {
        GuardAgainstSideBoth(side);

        return (int)BoardSquare.h8 + (7 * 8 * side);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LongCastleRookSourceSquare(Side side) => LongCastleRookSourceSquare((int)side);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LongCastleRookSourceSquare(int side)
    {
        GuardAgainstSideBoth(side);

        return (int)BoardSquare.a8 + (7 * 8 * side);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Source, int Target) ShortCastleRookSourceAndTargetSquare(Side side) => ShortCastleRookSourceAndTargetSquare((int)side);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Source, int Target) ShortCastleRookSourceAndTargetSquare(int side)
    {
        GuardAgainstSideBoth(side);

        return (
            (int)BoardSquare.h8 + (7 * 8 * side),
            Constants.BlackShortCastleRookSquare + (7 * 8 * side));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Source, int Target) LongCastleRookSourceAndTargetSquare(Side side) => LongCastleRookSourceAndTargetSquare((int)side);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Source, int Target) LongCastleRookSourceAndTargetSquare(int side)
    {
        GuardAgainstSideBoth(side);

        return (
            (int)BoardSquare.a8 + (7 * 8 * side),
            Constants.BlackLongCastleRookSquare + (7 * 8 * side));
    }

    /// <summary>
    /// Updates <paramref name="halfMovesWithoutCaptureOrPawnMove"/>.
    /// See also <see cref="Game.Update50movesRule(int, bool)"/>
    /// </summary>
    /// <param name="moveToPlay"></param>
    /// <param name="halfMovesWithoutCaptureOrPawnMove"></param>
    /// <remarks>
    /// Checking halfMovesWithoutCaptureOrPawnMove >= 100 since a capture/pawn move doesn't necessarily 'clear' the variable.
    /// i.e. while the engine is searching:
    ///     At depth 2, 50 rules move applied and eval is 0
    ///     At depth 3, there's a capture, but the eval should still be 0
    ///     At depth 4 there's no capture, but the eval should still be 0
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Update50movesRule(Move moveToPlay, int halfMovesWithoutCaptureOrPawnMove)
    {
        if (moveToPlay.IsCapture())
        {
            return halfMovesWithoutCaptureOrPawnMove >= 100
                ? halfMovesWithoutCaptureOrPawnMove
                : 0;
        }
        else
        {
            var pieceToMove = moveToPlay.Piece();

            return (pieceToMove == (int)Piece.P || pieceToMove == (int)Piece.p) && halfMovesWithoutCaptureOrPawnMove < 100
                ? 0
                : halfMovesWithoutCaptureOrPawnMove + 1;
        }
    }

    /// <summary>
    /// Providing there's a checkmate detected in <paramref name="bestEvaluation"/>, returns in how many moves
    /// </summary>
    /// <param name="bestEvaluation"></param>
    /// <param name="bestEvaluationAbs"></param>
    /// <returns>Positive value if white is checkmating, negative value if black is</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CalculateMateInX(int bestEvaluation, int bestEvaluationAbs)
    {
        int mate = (int)Math.Ceiling(0.5 * ((EvaluationConstants.CheckMateBaseEvaluation - bestEvaluationAbs) / EvaluationConstants.CheckmateDepthFactor));

        return (int)Math.CopySign(mate, bestEvaluation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long CalculateNps(int nodes, long elapsedMilliseconds)
    {
        return Convert.ToInt64(Math.Clamp(nodes / ((0.001 * elapsedMilliseconds) + 1), 0, long.MaxValue));
    }

    [Conditional("DEBUG")]
    private static void GuardAgainstSideBoth(int side)
    {
        if (side == (int)Side.Both)
        {
            throw new ArgumentException($"{Side.Both} wasn't expected");
        }
    }

    [Conditional("DEBUG")]
    public static void Assert(bool value, string errorMessage = "Assertion failed")
    {
        if (!value)
        {
            _logger.Error(errorMessage);
            throw new AssertException(errorMessage);
        }
    }
}
