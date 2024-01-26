/*

 *  | ExternalMethods | 6.900 us | 0.0547 us | 0.0512 us |  1.00 |     - |     - |     - |         - |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class IsSquareAttackedByPawns_Benchmark : BaseBenchmark
{
    private readonly Position[] _positions =
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    [Benchmark(Baseline = true)]
    public bool IsSquaredAttackedByPawns_PassBitBoards()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                var sideToMoveInt = (int)position.Side;
                var offset = Utils.PieceOffset(sideToMoveInt);

                if (IsSquaredAttackedByPawns_PassBitBoards(squareIndex, sideToMoveInt, offset, position.PieceBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public bool IsSquaredAttackedByPawns_PassPawnBitBoard()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                var sideToMoveInt = (int)position.Side;
                var offset = Utils.PieceOffset(sideToMoveInt);

                if (IsSquaredAttackedByPawns_PassPawnBitBoard(squareIndex, sideToMoveInt, position.PieceBitBoards[offset]))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquaredAttackedByPawns_PassBitBoards(int squareIndex, int sideToMove, int offset, BitBoard[] pieces)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & pieces[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquaredAttackedByPawns_PassPawnBitBoard(int squareIndex, int sideToMove, BitBoard pawnBitBoard)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & pawnBitBoard) != default;
    }
}
