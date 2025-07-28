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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PieceOffset(Side side) => PieceOffset((int)side);

    /// <summary>
    /// Side.White -> 0
    /// Side.Black -> 6
    /// </summary>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PieceOffset(bool isWhite) => isWhite ? 0 : 6;

    /// <summary>
    /// Side.Black -> Side.White
    /// Side.White -> Side.Black
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int OppositeSide(Side side) => OppositeSide((int)side);

    /// <summary>
    /// Side.Black -> Side.White
    /// Side.White -> Side.Black
    /// </summary>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CalculateMateInX(int score) => CalculateMateInX(score, Math.Abs(score));

    /// <summary>
    /// Providing there's a checkmate detected in <paramref name="score"/>, returns in how many moves
    /// </summary>
    /// <returns>Positive value if white is checkmating, negative value if black is</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CalculateMateInX(int score, int bestScoreAbs)
    {
        int mate = (int)Math.Ceiling(0.5 * (EvaluationConstants.CheckMateBaseEvaluation - bestScoreAbs));

        return (int)Math.CopySign(mate, score);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong CalculateNps(ulong nodes, double elapsedSeconds)
    {
        // Adding double.Epsilon to avoid potential System.OverflowException
        // i.e. when calculating multithreading aggregated stats in a single-move position:0
        // the elapsed seconds are taken from existing SearchResult.Time, already rounded and therefore 0
        return Convert.ToUInt64(Math.Clamp(nodes / (elapsedSeconds + double.Epsilon), 1, ulong.MaxValue));
    }

    /// <summary>
    /// Calculates elapsed time with sub-ms precision.
    /// We care when reporting nps for low depths, but more importantly to avoid the risk of dividing by zero.
    /// http://geekswithblogs.net/BlackRabbitCoder/archive/2012/01/12/c.net-little-pitfalls-stopwatch-ticks-are-not-timespan-ticks.aspx
    /// </summary>
    /// <returns>Elapsed time in seconds</returns>
    public static double CalculateElapsedSeconds(Stopwatch stopwatch)
    {
        return stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
    }

    /// <summary>
    /// Transforms the high precision elapsed time in seconds into the time format UCI expects: ms
    /// </summary>
    public static ulong CalculateUCITime(double elapsedSeconds)
    {
        return Math.Clamp(Convert.ToUInt64(elapsedSeconds * 1_000), 1, ulong.MaxValue);
    }

    /// <summary>
    /// https://minuskelvin.net/chesswiki/content/packed-eval.html
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Pack(short mg, short eg)
    {
        return (eg << 16) + mg;
    }

    /// <summary>
    /// https://minuskelvin.net/chesswiki/content/packed-eval.html
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short UnpackMG(int packed)
    {
        return (short)packed;
    }

    /// <summary>
    /// https://minuskelvin.net/chesswiki/content/packed-eval.html
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short UnpackEG(int packed)
    {
        return (short)((packed + 0x8000) >> 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMinorPiece(int piece)
    {
        const int MinorPieceMask = (1 << (int)Piece.N) | (1 << (int)Piece.n) | (1 << (int)Piece.B) | (1 << (int)Piece.b);

        return ((1 << piece) & MinorPieceMask) != 0;
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
            throw new LynxException(errorMessage);
        }
    }
}
