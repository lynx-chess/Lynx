/*
 *
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
        var sp = MovePool.AsSpan();
        ref var movePool = ref sp;
        var (moveStart, moveEnd) = MoveGenerator.GenerateAllMovesAsSpan(position, ref movePool, 0, capturesOnly: false);

        if (movePool[moveStart] == 0)
        {
            return -1;
        }

        var generatedMoves = movePool[moveStart..moveEnd];
        Span<int> scores = stackalloc int[generatedMoves.Length];
        for (int i = 0; i < generatedMoves.Length; ++i)
        {
            scores[i] = -SimplifiedScoreMove(generatedMoves[i], position);
        }

        scores.Sort(generatedMoves);

        int counter = 0;

        for (int mvIndex = 0; mvIndex < generatedMoves.Length; ++mvIndex)
        {
            var move = generatedMoves[mvIndex];
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
