using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Lynx.EvaluationConstants;

namespace Lynx.Model;

/// <summary>
/// Squares attacked by a piece worth less than the one in the square.
/// Attacks aren't refreshed, so some escapes are incorrectly flagged if the target square remains on the slider path
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct LesserAttackers
{
    private readonly Bitboard _byPawns;
    private readonly Bitboard _byPawnsAndMinors;
    private readonly Bitboard _byPawnsMinorsAndRooks;

    public LesserAttackers(ReadOnlySpan<Bitboard> oppositeSideAttacksByPiece)
    {
        Debug.Assert(oppositeSideAttacksByPiece.Length == (int)Piece.K + 1);

        _byPawns = oppositeSideAttacksByPiece[(int)Piece.P];
        _byPawnsAndMinors = _byPawns | oppositeSideAttacksByPiece[(int)Piece.N] | oppositeSideAttacksByPiece[(int)Piece.B];
        _byPawnsMinorsAndRooks = _byPawnsAndMinors | oppositeSideAttacksByPiece[(int)Piece.R];
    }

    /// <summary>
    /// Positive when <paramref name="move"/> escapes from the attack of a lesser piece
    /// Negative when it moves to a square attacked by a lesser piece
    /// 0 otherwise (no attacks)
    /// Known FP: when the piece moves away from the attacker but remains in its slider path
    /// Only quiet moves are supported
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int Score(Move move)
    {
        Bitboard lesserAttackers;
        int scale;

        // Piece type regardless of its color, so that both sides are scored the same way
        switch (move.Piece() % ((int)Piece.K + 1))
        {
            case (int)Piece.N:
            case (int)Piece.B:
                {
                    lesserAttackers = _byPawns;
                    scale = LesserAttackerMinorScore;

                    break;
                }
            case (int)Piece.R:
                {
                    lesserAttackers = _byPawnsAndMinors;
                    scale = LesserAttackerRookScore;

                    break;
                }
            case (int)Piece.Q:
                {
                    lesserAttackers = _byPawnsMinorsAndRooks;
                    scale = LesserAttackerQueenScore;

                    break;
                }
            default:
                {
                    return 0;
                }
        }

        if (lesserAttackers.GetBit(move.TargetSquare()))
        {
            return -scale;
        }

        return lesserAttackers.GetBit(move.SourceSquare())
            ? scale
            : 0;
    }
}
