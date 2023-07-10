﻿using BenchmarkDotNet.Attributes;
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
            moves.AddRange(Sort_LocalVariableAndIn(position.AllPossibleMoves(), position));

        return Sort_LocalVariableAndIn(moves, position)[0];
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int NoIn(string fen)
    {
        var moves = new List<Move>(50_000);
        var position = new Position(fen);

        for (int i = 0; i < 1000; ++i)
            moves.AddRange(Sort_NoIn(position.AllPossibleMoves(), position));

        return Sort_NoIn(moves, position)[0];
    }

    private List<Move> Sort_LocalVariableAndIn(IEnumerable<Move> moves, in Position currentPosition)
    {
        var localPosition = currentPosition;
        return moves.OrderByDescending(move => Score(move, in localPosition)).ToList();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private List<Move> Sort_NoIn(IEnumerable<Move> moves, Position currentPosition)
    {
        return moves.OrderByDescending(move => Score(move, currentPosition)).ToList();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Score(Move move, in Position position) => move.Score(in position);
}
