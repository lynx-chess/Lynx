using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx;

/// <summary>
/// Implementation based on Stormprhax, some comments and clarifications from Altair
/// </summary>
public static class SEE
{
    #pragma warning disable IDE0055 // Discard formatting in this region

    private static readonly int[] _pieceValues =
    [
        100, 450, 450, 650, 1250, 0,
        100, 450, 450, 650, 1250, 0
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

        var score = Gain(position, move) - threshold;

        // If taking the opponent's piece without any risk is still negative
        if (score < 0)
        {
            return false;
        }

        var next = move.Piece();
        score -= _pieceValues[next];

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

        var attackers = position.AllAttackersTo(targetSquare, occupancy);

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
            if (nextPiece == Piece.P || nextPiece == Piece.p
                || nextPiece == Piece.B || nextPiece == Piece.b
                || nextPiece == Piece.Q || nextPiece == Piece.q)
            {
                attackers |= Attacks.BishopAttacks(targetSquare, occupancy) & bishops;
            }

            if (nextPiece == Piece.R || nextPiece == Piece.r
                || nextPiece == Piece.Q || nextPiece == Piece.q)
            {
                attackers |= Attacks.RookAttacks(targetSquare, occupancy) & rooks;
            }

            // Removing used pieces from attackers
            attackers &= occupancy;

            score = -score - 1 - _pieceValues[(int)nextPiece];
            us = Utils.OppositeSide(us);

            if (score >= 0)
            {
                // Our only attacker is our king, but the opponent still has defenders
                if ((nextPiece == Piece.K || nextPiece == Piece.k)
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
    /// Doesn't handle non-captures, promotions and en-passants
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Gain(Position position, Move move) => _pieceValues[position.PieceAt(move.TargetSquare())];

    /// <summary>
    /// Doesn't handle non-captures, promotions and en-passants
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <returns></returns>
    [Obsolete("Since we're not handling non-captures, promotiosn and en-passants, we don't really need this")]
    private static int CompleteGain(Position position, Move move)
    {
        if (move.IsCastle())
        {
            return 0;
        }
        else if (move.IsEnPassant())
        {
            return _pieceValues[(int)Piece.P];
        }

        var promotedPiece = move.PromotedPiece();

        return promotedPiece == default
            ? _pieceValues[position.PieceAt(move.TargetSquare())]
            : _pieceValues[promotedPiece] - _pieceValues[(int)Piece.P];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Piece PopLeastValuableAttacker(Position position, ref BitBoard occupancy, BitBoard attackers, int color)
    {
        var start = Utils.PieceOffset(color);

        for (int i = start; i < start + 6; ++i)
        {
            var piece = (Piece)i;
            var board = attackers & position.PieceBitBoards[i];

            if (!board.Empty())
            {
                occupancy ^= board.LSB();

                return piece;
            }
        }

        return Piece.Unknown;
    }
}
