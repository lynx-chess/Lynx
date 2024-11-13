using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx;

/// <summary>
/// Implementation based on Stormprhax, some comments and clarifications from Altair
/// </summary>
public static class SEE
{
    #pragma warning disable IDE0055 // Discard formatting in this region

    public static ReadOnlySpan<int> PieceValues =>
    [
        100, 450, 450, 650, 1250, 0,
        100, 450, 450, 650, 1250, 0,
        0
    ];

    #pragma warning restore IDE0055

    /// <summary>
    /// Doesn't handle non-captures, promotions and en-passants
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGoodCapture(Position position, Move move, int threshold = 0)
    {
        System.Diagnostics.Debug.Assert(move.IsCapture(), $"{nameof(IsGoodCapture)} doesn't handle non-capture moves");
        System.Diagnostics.Debug.Assert(move.PromotedPiece() == default, $"{nameof(IsGoodCapture)} doesn't handle promotion moves");
        System.Diagnostics.Debug.Assert(!move.IsEnPassant(), $"{nameof(IsGoodCapture)} potentially doesn't handle en-passant moves");

        var sideToMove = position.Side;

        var score = PieceValues[move.CapturedPiece()] - threshold;    // Gain() - threshold

        // If taking the opponent's piece without any risk is still negative
        if (score < 0)
        {
            return false;
        }

        var next = move.Piece();
        score -= PieceValues[next];

        // If risking our piece being fully lost and the exchange value is still >= 0
        if (score >= 0)
        {
            return true;
        }

        var targetSquare = move.TargetSquare();

        var occupancy = position.OccupancyBitBoards[(int)Side.Both]
            ^ BitBoardExtensions.SquareBit(move.SourceSquare())
            ^ BitBoardExtensions.SquareBit(targetSquare);

        var queens = position.Queens;
        var bishops = queens | position.Bishops;
        var rooks = queens | position.Rooks;

        var attackers = position.AllAttackersTo(targetSquare, occupancy, rooks, bishops);

        var us = Utils.OppositeSide(sideToMove);

        while (true)
        {
            var ourAttackers = attackers & position.OccupancyBitBoards[us];

            if (ourAttackers.Empty())
            {
                break;
            }

            var nextPiece = PopLeastValuableAttacker(position, ref occupancy, ourAttackers, us);

            // After removing an attacker, there could be a sliding piece attack
            if ((nextPiece & 0x01) == 0)    // Equivalent to nextPiece % 2 == 0): true for P, B, Q (and p, b, q, should PopLeastValuableAttacker also return black pieces ever gain)
            {
                attackers |= Attacks.BishopAttacks(targetSquare, occupancy) & bishops;
            }

            if (nextPiece == (int)Piece.R || nextPiece == (int)Piece.Q)
            {
                attackers |= Attacks.RookAttacks(targetSquare, occupancy) & rooks;
            }

            // Removing used pieces from attackers
            attackers &= occupancy;

            score = -score - 1 - PieceValues[nextPiece];
            us = Utils.OppositeSide(us);

            if (score >= 0)
            {
                // Our only attacker is our king, but the opponent still has defenders
                if ((nextPiece == (int)Piece.K)
                    && (attackers & position.OccupancyBitBoards[us]).NotEmpty())
                {
                    us = Utils.OppositeSide(us);
                }

                break;
            }
        }

        return (int)sideToMove != us;
    }

    /// <summary>
    /// Handles all kinds of moves
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasPositiveScore(Position position, Move move, int threshold = 0)
    {
        var sideToMove = position.Side;

        var score = Gain(move) - threshold;

        // If taking the opponent's piece without any risk is still negative
        if (score < 0)
        {
            return false;
        }

        var next = move.PromotedPiece() != default
            ? move.PromotedPiece()
            : move.Piece();

        score -= PieceValues[next];

        // If risking our piece being fully lost and the exchange value is still >= 0
        if (score >= 0)
        {
            return true;
        }

        var targetSquare = move.TargetSquare();

        var occupancy = position.OccupancyBitBoards[(int)Side.Both]
            ^ BitBoardExtensions.SquareBit(move.SourceSquare())
            ^ BitBoardExtensions.SquareBit(targetSquare);

        var queens = position.Queens;
        var bishops = queens | position.Bishops;
        var rooks = queens | position.Rooks;

        var attackers = position.AllAttackersTo(targetSquare, occupancy, rooks, bishops);

        var us = Utils.OppositeSide(sideToMove);

        while (true)
        {
            var ourAttackers = attackers & position.OccupancyBitBoards[us];

            if (ourAttackers.Empty())
            {
                break;
            }

            var nextPiece = PopLeastValuableAttacker(position, ref occupancy, ourAttackers, us);

            // After removing an attacker, there could be a sliding piece attack
            if ((nextPiece & 0x01) == 0)    // Equivalent to nextPiece % 2 == 0): true for P, B, Q, p, b and q
            {
                attackers |= Attacks.BishopAttacks(targetSquare, occupancy) & bishops;
            }

            if (nextPiece == (int)Piece.R || nextPiece == (int)Piece.Q)
            {
                attackers |= Attacks.RookAttacks(targetSquare, occupancy) & rooks;
            }

            // Removing used pieces from attackers
            attackers &= occupancy;

            score = -score - 1 - PieceValues[nextPiece];
            us = Utils.OppositeSide(us);

            if (score >= 0)
            {
                // Our only attacker is our king, but the opponent still has defenders
                if ((nextPiece == (int)Piece.K)
                    && (attackers & position.OccupancyBitBoards[us]).NotEmpty())
                {
                    us = Utils.OppositeSide(us);
                }

                break;
            }
        }

        return (int)sideToMove != us;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Gain(Move move)
    {
        if (move.IsCastle())
        {
            return 0;
        }
        else if (move.IsEnPassant())
        {
            return PieceValues[(int)Piece.P];
        }

        var promotedPiece = move.PromotedPiece();

#pragma warning disable S3358 // Ternary operators should not be nested
        return promotedPiece == default
            ? PieceValues[move.CapturedPiece()]
            : PieceValues[promotedPiece] - PieceValues[(int)Piece.P] + (move.IsCapture() ? PieceValues[move.CapturedPiece()] : 0);
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    /// <summary>
    /// Returns only <see cref="Side.White"/> pieces
    /// </summary>
    /// <param name="position"></param>
    /// <param name="occupancy"></param>
    /// <param name="attackers"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PopLeastValuableAttacker(Position position, ref BitBoard occupancy, BitBoard attackers, int color)
    {
        var offset = Utils.PieceOffset(color);

        for (int i = 0; i < 6; ++i)
        {
            var board = attackers & position.PieceBitBoards[i + offset];

            if (board.NotEmpty())
            {
                occupancy ^= board.LSB();

                return i;
            }
        }

        System.Diagnostics.Debug.Fail($"Unexpected outcome of {nameof(PopLeastValuableAttacker)}: no attacker returned");

        return (int)Piece.Unknown;
    }
}
