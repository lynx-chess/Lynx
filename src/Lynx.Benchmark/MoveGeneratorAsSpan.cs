/*
 *
 *  BenchmarkDotNet v0.13.8, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23463.5
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method  | fen                  | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |-------- |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | Array   | 8/k7/(...)- 0 1 [39] | 2.886 us | 0.0571 us | 0.0634 us |  1.00 |    0.00 | 0.0381 |    1024 B |        1.00 |
 *  | AsSpan  | 8/k7/(...)- 0 1 [39] | 2.948 us | 0.0502 us | 0.0470 us |  1.02 |    0.03 | 0.0191 |     528 B |        0.52 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | r2q1r(...)- 0 9 [68] | 5.748 us | 0.1122 us | 0.1201 us |  1.00 |    0.00 | 0.0610 |    1648 B |        1.00 |
 *  | AsSpan  | r2q1r(...)- 0 9 [68] | 4.756 us | 0.0887 us | 0.0830 us |  0.83 |    0.03 | 0.0229 |     672 B |        0.41 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | r3k2r(...)- 0 1 [68] | 5.504 us | 0.1095 us | 0.1124 us |  1.00 |    0.00 | 0.0534 |    1576 B |        1.00 |
 *  | AsSpan  | r3k2r(...)- 0 1 [68] | 4.512 us | 0.0832 us | 0.0778 us |  0.82 |    0.02 | 0.0229 |     600 B |        0.38 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | r3k2r(...)- 0 1 [68] | 5.755 us | 0.1144 us | 0.1070 us |  1.00 |    0.00 | 0.0610 |    1624 B |        1.00 |
 *  | AsSpan  | r3k2r(...)- 0 1 [68] | 4.591 us | 0.0892 us | 0.0876 us |  0.80 |    0.02 | 0.0229 |     600 B |        0.37 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | rnbqk(...)6 0 1 [67] | 5.304 us | 0.0997 us | 0.0884 us |  1.00 |    0.00 | 0.0534 |    1528 B |        1.00 |
 *  | AsSpan  | rnbqk(...)6 0 1 [67] | 4.417 us | 0.0862 us | 0.0807 us |  0.83 |    0.02 | 0.0153 |     576 B |        0.38 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | rnbqk(...)- 0 1 [56] | 3.155 us | 0.0586 us | 0.0575 us |  1.00 |    0.00 | 0.0381 |    1000 B |        1.00 |
 *  | AsSpan  | rnbqk(...)- 0 1 [56] | 2.912 us | 0.0577 us | 0.0665 us |  0.92 |    0.02 | 0.0114 |     312 B |        0.31 |
 *
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.20348.1970) (Hyper-V)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23463.5
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method  | fen                  | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |-------- |--------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 *  | Array   | 8/k7/(...)- 0 1 [39] | 2.278 us | 0.0095 us | 0.0084 us |  1.00 | 0.0534 |    1024 B |        1.00 |
 *  | AsSpan  | 8/k7/(...)- 0 1 [39] | 2.622 us | 0.0210 us | 0.0197 us |  1.15 | 0.0267 |     528 B |        0.52 |
 *  |         |                      |          |           |           |       |        |           |             |
 *  | Array   | r2q1r(...)- 0 9 [68] | 4.696 us | 0.0200 us | 0.0177 us |  1.00 | 0.0839 |    1648 B |        1.00 |
 *  | AsSpan  | r2q1r(...)- 0 9 [68] | 3.906 us | 0.0149 us | 0.0124 us |  0.83 | 0.0305 |     672 B |        0.41 |
 *  |         |                      |          |           |           |       |        |           |             |
 *  | Array   | r3k2r(...)- 0 1 [68] | 4.437 us | 0.0146 us | 0.0122 us |  1.00 | 0.0839 |    1576 B |        1.00 |
 *  | AsSpan  | r3k2r(...)- 0 1 [68] | 3.797 us | 0.0144 us | 0.0128 us |  0.86 | 0.0305 |     600 B |        0.38 |
 *  |         |                      |          |           |           |       |        |           |             |
 *  | Array   | r3k2r(...)- 0 1 [68] | 4.866 us | 0.0175 us | 0.0164 us |  1.00 | 0.0839 |    1624 B |        1.00 |
 *  | AsSpan  | r3k2r(...)- 0 1 [68] | 3.912 us | 0.0146 us | 0.0137 us |  0.80 | 0.0305 |     600 B |        0.37 |
 *  |         |                      |          |           |           |       |        |           |             |
 *  | Array   | rnbqk(...)6 0 1 [67] | 4.595 us | 0.0182 us | 0.0162 us |  1.00 | 0.0763 |    1528 B |        1.00 |
 *  | AsSpan  | rnbqk(...)6 0 1 [67] | 3.779 us | 0.0087 us | 0.0073 us |  0.82 | 0.0305 |     576 B |        0.38 |
 *  |         |                      |          |           |           |       |        |           |             |
 *  | Array   | rnbqk(...)- 0 1 [56] | 2.444 us | 0.0079 us | 0.0062 us |  1.00 | 0.0534 |    1000 B |        1.00 |
 *  | AsSpan  | rnbqk(...)- 0 1 [56] | 2.584 us | 0.0146 us | 0.0137 us |  1.06 | 0.0153 |     312 B |        0.31 |
 *
 *
 *  BenchmarkDotNet v0.13.8, macOS Monterey 12.7 (21G816) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-rc.1.23463.5
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *
 *
 *  | Method  | fen                  | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |-------- |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | Array   | 8/k7/(...)- 0 1 [39] | 2.393 us | 0.0234 us | 0.0196 us |  1.00 |    0.00 | 0.1602 |    1024 B |        1.00 |
 *  | AsSpan  | 8/k7/(...)- 0 1 [39] | 2.646 us | 0.0442 us | 0.0392 us |  1.11 |    0.02 | 0.0839 |     528 B |        0.52 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | r2q1r(...)- 0 9 [68] | 4.675 us | 0.0721 us | 0.0674 us |  1.00 |    0.00 | 0.2594 |    1649 B |        1.00 |
 *  | AsSpan  | r2q1r(...)- 0 9 [68] | 3.800 us | 0.0322 us | 0.0286 us |  0.81 |    0.01 | 0.1068 |     672 B |        0.41 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | r3k2r(...)- 0 1 [68] | 4.369 us | 0.0302 us | 0.0252 us |  1.00 |    0.00 | 0.2441 |    1577 B |        1.00 |
 *  | AsSpan  | r3k2r(...)- 0 1 [68] | 3.878 us | 0.0764 us | 0.1071 us |  0.90 |    0.03 | 0.0954 |     600 B |        0.38 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | r3k2r(...)- 0 1 [68] | 5.057 us | 0.1002 us | 0.1702 us |  1.00 |    0.00 | 0.2518 |    1625 B |        1.00 |
 *  | AsSpan  | r3k2r(...)- 0 1 [68] | 4.092 us | 0.0810 us | 0.0994 us |  0.81 |    0.03 | 0.0916 |     600 B |        0.37 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | rnbqk(...)6 0 1 [67] | 4.691 us | 0.0678 us | 0.0634 us |  1.00 |    0.00 | 0.2365 |    1529 B |        1.00 |
 *  | AsSpan  | rnbqk(...)6 0 1 [67] | 3.750 us | 0.0744 us | 0.1303 us |  0.82 |    0.03 | 0.0916 |     576 B |        0.38 |
 *  |         |                      |          |           |           |       |         |        |           |             |
 *  | Array   | rnbqk(...)- 0 1 [56] | 2.769 us | 0.0526 us | 0.0492 us |  1.00 |    0.00 | 0.1564 |    1000 B |        1.00 |
 *  | AsSpan  | rnbqk(...)- 0 1 [56] | 2.652 us | 0.0476 us | 0.0445 us |  0.96 |    0.02 | 0.0496 |     312 B |        0.31 |
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
    public int AsSpan (string fen)
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
