/*
 *
 *  |     Method |                  fen |     Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
 *  |----------- |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|
 *  | Dictionary | r2q1r(...)- 0 9 [68] | 8.000 us | 0.1077 us | 0.1007 us |  1.00 |    0.00 | 1.6632 |      3 KB |
 *  |      Array | r2q1r(...)- 0 9 [68] | 7.606 us | 0.0991 us | 0.0927 us |  0.95 |    0.02 | 1.6632 |      3 KB |
 *  |            |                      |          |           |           |       |         |        |           |
 *  | Dictionary | r3k2r(...)- 0 1 [68] | 7.858 us | 0.0937 us | 0.0831 us |  1.00 |    0.00 | 1.6327 |      3 KB |
 *  |      Array | r3k2r(...)- 0 1 [68] | 7.529 us | 0.0777 us | 0.0727 us |  0.96 |    0.01 | 1.6251 |      3 KB |
 *  |            |                      |          |           |           |       |         |        |           |
 *  | Dictionary | r3k2r(...)- 0 1 [68] | 7.906 us | 0.1525 us | 0.1816 us |  1.00 |    0.00 | 1.6174 |      3 KB |
 *  |      Array | r3k2r(...)- 0 1 [68] | 7.569 us | 0.0694 us | 0.0615 us |  0.96 |    0.03 | 1.6251 |      3 KB |
 *  |            |                      |          |           |           |       |         |        |           |
 *  | Dictionary | rnbqk(...)6 0 1 [67] | 7.731 us | 0.0771 us | 0.0722 us |  1.00 |    0.00 | 1.6174 |      3 KB |
 *  |      Array | rnbqk(...)6 0 1 [67] | 7.341 us | 0.0895 us | 0.0793 us |  0.95 |    0.01 | 1.6251 |      3 KB |
 *  |            |                      |          |           |           |       |         |        |           |
 *  | Dictionary | rnbqk(...)- 0 1 [56] | 6.523 us | 0.0918 us | 0.0859 us |  1.00 |    0.00 | 1.4725 |      3 KB |
 *  |      Array | rnbqk(...)- 0 1 [56] | 6.155 us | 0.0812 us | 0.0720 us |  0.94 |    0.01 | 1.4725 |      3 KB |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class MoveGeneratorAsSpan : BaseBenchmark
{
    private static Move[] MovePool { get; } = new Move[Constants.MaxNumberOfPossibleMovesInAPosition];

    public static IEnumerable<string> Data => new[]
    {
            Constants.InitialPositionFEN,
            Constants.TrickyTestPositionFEN,
            Constants.TrickyTestPositionReversedFEN,
            Constants.CmkTestPositionFEN,
            Constants.KillerTestPositionFEN,
            Constants.TTPositionFEN
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Array(string fen)
    {
        var position = new Position(fen);

        var generatedMoves = MoveGenerator.GenerateAllMoves(position, MovePool, capturesOnly: false);

        if (!generatedMoves.Any())
        {
            return -1;
        }

        var movesToEvaluate = generatedMoves.OrderByDescending(move => SimplifiedScoreMove(move, position));

        int counter = 0;

        foreach (var move in movesToEvaluate)
        {
            counter += move;
        }

        return counter;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int AsArray(string fen)
    {
        var position = new Position(fen);

        var generatedMoves = MoveGenerator.GenerateAllMovesAsSpan(position, MovePool, capturesOnly: false);

        if (generatedMoves.Length == 0)
        {
            return -1;
        }

        var scores = new int[generatedMoves.Length];
        for (int i = 0; i < generatedMoves.Length; ++i)
        {
            scores[i] = -SimplifiedScoreMove(generatedMoves[i], position);
        }

        MemoryExtensions.Sort(scores.AsSpan(), generatedMoves);

        int counter = 0;

        foreach (var move in generatedMoves)
        {
            counter += move;
        }

        return counter;
    }

    private static int SimplifiedScoreMove(Move move, Position position)
    {
        var promotedPiece = move.PromotedPiece();
        if ((promotedPiece + 2) % 6 == 0)
        {
            return EvaluationConstants.CaptureMoveBaseScoreValue + EvaluationConstants.PromotionMoveScoreValue;
        }

        if (move.IsCapture())
        {
            var sourcePiece = move.Piece();
            int targetPiece = (int)Piece.P;    // Important to initialize to P or p, due to en-passant captures

            var targetSquare = move.TargetSquare();
            var offset = Utils.PieceOffset(position.Side);
            var oppositePawnIndex = (int)Piece.p - offset;

            var limit = (int)Piece.k - offset;
            for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
            {
                if (position.PieceBitBoards[pieceIndex].GetBit(targetSquare))
                {
                    targetPiece = pieceIndex;
                    break;
                }
            }

            return EvaluationConstants.CaptureMoveBaseScoreValue + EvaluationConstants.MostValueableVictimLeastValuableAttacker[sourcePiece, targetPiece];
        }

        if (promotedPiece != default)
        {
            return EvaluationConstants.PromotionMoveScoreValue;
        }

        return default;
    }
}
