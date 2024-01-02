using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class LocalVariableIn_vs_NoIn : BaseBenchmark
{
    public static IEnumerable<string> Data => new[] {
            Constants.EmptyBoardFEN,
            Constants.InitialPositionFEN,
            Constants.TrickyTestPositionFEN,
            "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1",
            "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1",
            "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9 "
        };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int LocalVariableAndIn(string fen)
    {
        var position = new Position(fen);
        var moves = new List<Move>(50_000);

        for (int i = 0; i < 1000; ++i)
            moves.AddRange(Sort_LocalVariableAndIn(MoveGenerator.GenerateAllMoves(position), position));

        return Sort_LocalVariableAndIn(moves, position)[0];
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int NoIn(string fen)
    {
        var moves = new List<Move>(50_000);
        var position = new Position(fen);

        for (int i = 0; i < 1000; ++i)
            moves.AddRange(Sort_NoIn(MoveGenerator.GenerateAllMoves(position), position));

        return Sort_NoIn(moves, position)[0];
    }

    private List<Move> Sort_LocalVariableAndIn(IEnumerable<Move> moves, Position currentPosition)
    {
        var localPosition = currentPosition;
        return [.. moves.OrderByDescending(move => Score(move, localPosition))];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private List<Move> Sort_NoIn(IEnumerable<Move> moves, Position currentPosition)
    {
        return [.. moves.OrderByDescending(move => Score(move, currentPosition))];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Score(Move move, in Position position) => move.OldScore(in position);
}

public static class BenchmarkLegacyMoveExtensions
{
    /// <summary>
    /// Returns the score evaluation of a move taking into account <see cref="EvaluationConstants.MostValueableVictimLeastValuableAttacker"/>
    /// </summary>
    /// <param name="position">The position that precedes a move</param>
    /// <param name="killerMoves"></param>
    /// <param name="plies"></param>
    /// <param name="historyMoves"></param>
    /// <returns>The higher the score is, the more valuable is the captured piece and the less valuable is the piece that makes the such capture</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int OldScore(this Move move, in Position position, int[,]? killerMoves = null, int? plies = null, int[,]? historyMoves = null)
    {
        if (move.IsCapture())
        {
            var sourcePiece = move.Piece();
            int targetPiece = (int)Model.Piece.P;    // Important to initialize to P or p, due to en-passant captures

            var targetSquare = move.TargetSquare();
            var oppositeSide = Utils.OppositeSide(position.Side);
            var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
            var oppositePawnIndex = (int)Model.Piece.P + oppositeSideOffset;

            var limit = (int)Model.Piece.K + oppositeSideOffset;
            for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
            {
                if (position.PieceBitBoards[pieceIndex].GetBit(targetSquare))
                {
                    targetPiece = pieceIndex;
                    break;
                }
            }

            return EvaluationConstants.BadCaptureMoveBaseScoreValue + EvaluationConstants.MostValueableVictimLeastValuableAttacker[sourcePiece, targetPiece];
        }
        else if (killerMoves is not null && plies is not null)
        {
            // 1st killer move
            if (killerMoves[0, plies.Value] == move)
            {
                return EvaluationConstants.FirstKillerMoveValue;
            }

            // 2nd killer move
            else if (killerMoves[1, plies.Value] == move)
            {
                return EvaluationConstants.SecondKillerMoveValue;
            }

            // History move
            else if (historyMoves is not null)
            {
                return historyMoves[move.Piece(), move.TargetSquare()];
            }
        }

        return default;
    }
}
