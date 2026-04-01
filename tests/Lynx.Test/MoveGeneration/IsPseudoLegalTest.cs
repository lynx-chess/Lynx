using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class IsPseudoLegalTest
{
    private static readonly string[] _fens =
    [
        Constants.InitialPositionFEN,

        // Castling: rights available vs unavailable
        "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1",
        "r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1",

        // Castling: same rights, but path blocked/attacked
        "r3k2r/8/8/8/8/8/8/R2QK2R w KQkq - 0 1",
        "r3k2r/8/8/8/2b5/8/8/R3K2R w KQkq - 0 1",

        // En-passant: available vs unavailable (white to move)
        "8/8/8/3pP3/8/8/8/K1k5 w - d6 0 1",
        "8/8/8/3pP3/8/8/8/K1k5 w - - 0 1",

        // En-passant: available vs unavailable (black to move)
        "8/8/8/8/3Pp3/8/8/K1k5 b - d3 0 1",
        "8/8/8/8/3Pp3/8/8/K1k5 b - - 0 1",

        // Similar tactical structures: capture available vs not available
        "4k3/8/8/3n4/4B3/8/8/4K3 w - - 0 1",
        "4k3/8/8/8/4B3/8/8/4K3 w - - 0 1",

        // Similar king/rook pressure structures
        "4k3/8/8/8/8/8/4r3/4K3 w - - 0 1",
        "4k3/8/8/7b/8/8/4r3/4K3 w - - 0 1",

        // Mixed middlegame
        "4k3/P6P/8/8/8/8/p6p/4K3 w - - 0 1",
        "r2q1rk1/pp1n1ppp/2pbpn2/3p4/3P4/2N1PN2/PPQ1BPPP/R1B2RK1 b - - 0 1"
    ];

    [Test]
    public void IsPseudoLegal_ReturnsTrueForAllGeneratedPseudoLegalMoves()
    {
        foreach (var fen in _fens)
        {
            var position = new Position(fen);
            var moves = GeneratePseudoLegalMoves(position);

            Span<Bitboard> buffer = new Bitboard[EvaluationContext.RequiredBufferSize];
            var evaluationContext = new EvaluationContext(buffer);

            foreach (var move in moves)
            {
                Assert.That(MoveGenerator.IsPseudoLegal(position, move, ref evaluationContext), Is.True,
                    $"Expected move {move.UCIString()} to be pseudo-legal in {fen}");
            }
        }
    }

    [Test]
    public void IsPseudoLegal_ReturnsFalseForMovesGeneratedFromOtherPositions()
    {
        var positions = _fens.Select(f => new Position(f)).ToArray();
        var moveLists = positions.Select(GeneratePseudoLegalMoves).ToArray();

        for (int i = 0; i < positions.Length; ++i)
        {
            var currentPosition = positions[i];
            var currentMoves = moveLists[i];
            var currentMoveSet = currentMoves.ToHashSet();

            for (int j = 0; j < positions.Length; ++j)
            {
                if (i == j)
                {
                    continue;
                }

                Span<Bitboard> buffer = new Bitboard[EvaluationContext.RequiredBufferSize];
                var evaluationContext = new EvaluationContext(buffer);

                foreach (var foreignMove in moveLists[j])
                {
                    if (currentMoveSet.Contains(foreignMove))
                    {
                        continue;
                    }

                    Assert.That(MoveGenerator.IsPseudoLegal(currentPosition, foreignMove, ref evaluationContext), Is.False,
                        $"Foreign move {foreignMove.UCIString()} should not be pseudo-legal in {_fens[i]} (from {_fens[j]})");
                }
            }
        }
    }

    private static Move[] GeneratePseudoLegalMoves(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        return MoveGenerator.GenerateAllMoves(position, ref evaluationContext, moves).ToArray();
    }
}
