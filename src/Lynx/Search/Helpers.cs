﻿using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx;

public sealed record SearchResult(Move BestMove, double Evaluation, int TargetDepth, List<Move> Moves, int Alpha, int Beta, int Mate = default)
{
    public int DepthReached { get; set; }

    public int Nodes { get; set; }

    public long Time { get; set; }

    public long NodesPerSecond { get; set; }

    public bool IsCancelled { get; set; }

    public int HashfullPermill { get; set; }

    public SearchResult(SearchResult previousSearchResult)
    {
        BestMove = previousSearchResult.Moves.ElementAtOrDefault(2);
        Evaluation = previousSearchResult.Evaluation;
        TargetDepth = previousSearchResult.TargetDepth - 2;
        DepthReached = previousSearchResult.DepthReached - 2;
        Moves = previousSearchResult.Moves.Skip(2).ToList();
        Alpha = previousSearchResult.Alpha;
        Beta = previousSearchResult.Beta;
        Mate = previousSearchResult.Mate == 0 ? 0 : (int)Math.CopySign(Math.Abs(previousSearchResult.Mate) - 1, previousSearchResult.Mate);
    }
}

public sealed partial class Engine
{
    private const int MinValue = short.MinValue;
    private const int MaxValue = short.MaxValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private List<Move> SortMoves(IEnumerable<Move> moves, in Position currentPosition, int depth)
    {
        if (_isFollowingPV)
        {
            _isFollowingPV = false;
            foreach (var move in moves)
            {
                if (move == _pVTable[0, depth])
                {
                    _isFollowingPV = true;
                    _isScoringPV = true;
                }
            }
        }

        var localPosition = currentPosition;
        return moves.OrderByDescending(move => Score(move, in localPosition, depth)).ToList();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Score(Move move, in Position position, int depth)
    {
        if (_isScoringPV && move == _pVTable[0, depth])
        {
            _isScoringPV = false;

            return EvaluationConstants.PVMoveValue;
        }

        return move.Score(in position, _killerMoves, depth, _historyMoves);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IOrderedEnumerable<Move> SortCaptures(IEnumerable<Move> moves, in Position currentPosition, int depth)
    {
        var localPosition = currentPosition;
        return moves.OrderByDescending(move => Score(move, in localPosition, depth));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CopyPVTableMoves(int target, int source, int moveCountToCopy)
    {
        if (_pVTable[source] == default)
        {
            Array.Clear(_pVTable, target, _pVTable.Length - target);
            return;
        }

        //PrintPvTable(target, source);
        Array.Copy(_pVTable, source, _pVTable, target, moveCountToCopy);
        //PrintPvTable();
    }

    #region Debugging

    [Conditional("DEBUG")]
    private void ValidatePVTable()
    {
        var position = Game.CurrentPosition;
        for (int index = 0; index < _pVTable.Length; ++index)
        {
            for (int i = 0; i < PVTable.Indexes[1]; ++i)
            {
                if (_pVTable[index, i] == default)
                {
                    for (int j = i + 1; j < PVTable.Indexes[1]; ++j)
                    {
                        Utils.Assert(_pVTable[index, j] == default, $"Not expecting a move in _pvTable[{j}]");
                    }
                    break;
                }
                var move = _pVTable[index, i];

                if (!MoveExtensions.TryParseFromUCIString(
                   move.UCIString(),
                   position.AllPossibleMoves(Game.MovePool),
                   out _))
                {
                    var message = $"Unexpected PV move {move.UCIString()} from position {position.FEN()}";
                    _logger.Error(message);
                    throw new AssertException(message);
                }

                var newPosition = new Position(in position, move);

                if (!newPosition.WasProduceByAValidMove())
                {
                    throw new AssertException($"Invalid position after move {move.UCIString()} from position {position.FEN()}");
                }
                position = newPosition;
            }
        }
    }

    [Conditional("DEBUG")]
    private static void PrintPreMove(in Position position, int plies, Move move, bool isQuiescence = false)
    {
        var sb = new StringBuilder();
        for (int i = 0; i <= plies; ++i)
        {
            sb.Append("\t\t");
        }
        string depthStr = sb.ToString();

        //if (plies < Configuration.Parameters.Depth - 1)
        {
            //Console.WriteLine($"{Environment.NewLine}{depthStr}{move} ({position.Side}, {depth})");
            _logger.Trace($"{Environment.NewLine}{depthStr}{(isQuiescence ? "[Qui] " : "")}{move} ({position.Side}, {plies})");
        }
    }

    [Conditional("DEBUG")]
    private static void PrintMove(int plies, Move move, int evaluation, bool isQuiescence = false, bool prune = false)
    {
        var sb = new StringBuilder();
        for (int i = 0; i <= plies; ++i)
        {
            sb.Append("\t\t");
        }
        string depthStr = sb.ToString();

        //Console.ForegroundColor = depth switch
        //{
        //    0 => ConsoleColor.Red,
        //    1 => ConsoleColor.Blue,
        //    2 => ConsoleColor.Green,
        //    3 => ConsoleColor.White,
        //    _ => ConsoleColor.White
        //};
        //Console.WriteLine($"{depthStr}{move} ({position.Side}, {depthLeft}) | {evaluation}");
        //Console.WriteLine($"{depthStr}{move} | {evaluation}");

        _logger.Trace($"{depthStr}{(isQuiescence ? "[Qui] " : "")}{move,-6} | {evaluation}{(prune ? " | prunning" : "")}");

        //Console.ResetColor();
    }

    [Conditional("DEBUG")]
    private static void PrintMessage(int plies, string message)
    {
        var sb = new StringBuilder();
        for (int i = 0; i <= plies; ++i)
        {
            sb.Append("\t\t");
        }
        string depthStr = sb.ToString();

        _logger.Trace(depthStr + message);
    }

    [Conditional("DEBUG")]
    private static void PrintMessage(string message)
    {
        _logger.Trace(message);
    }

    [Conditional("DEBUG")]
    private void PrintPvTable(int target = -1, int source = -1)
    {
        for (int index = 0; index < PVTable.PVLength; ++index)
        {
            Console.WriteLine(
    (target != -1 ? $"src: {source}, tgt: {target}" + Environment.NewLine : "") +
    $" {0,-3} {_pVTable[index, 0],-6} {_pVTable[index, 1],-6} {_pVTable[index, 2],-6} {_pVTable[index, 3],-6} {_pVTable[index, 4],-6} {_pVTable[index, 5],-6} {_pVTable[index, 6],-6} {_pVTable[index, 7],-6}" + Environment.NewLine +
    $" {64,-3}        {_pVTable[index, 64],-6} {_pVTable[index, 65],-6} {_pVTable[index, 66],-6} {_pVTable[index, 67],-6} {_pVTable[index, 68],-6} {_pVTable[index, 69],-6} {_pVTable[index, 70],-6}" + Environment.NewLine +
    $" {127,-3}               {_pVTable[index, 127],-6} {_pVTable[index, 128],-6} {_pVTable[index, 129],-6} {_pVTable[index, 130],-6} {_pVTable[index, 131],-6} {_pVTable[index, 132],-6}" + Environment.NewLine +
    $" {189,-3}                      {_pVTable[index, 189],-6} {_pVTable[index, 190],-6} {_pVTable[index, 191],-6} {_pVTable[index, 192],-6} {_pVTable[index, 193],-6}" + Environment.NewLine +
    $" {250,-3}                             {_pVTable[index, 250],-6} {_pVTable[index, 251],-6} {_pVTable[index, 252],-6} {_pVTable[index, 253],-6}" + Environment.NewLine +
    $" {310,-3}                                    {_pVTable[index, 310],-6} {_pVTable[index, 311],-6} {_pVTable[index, 312],-6}" + Environment.NewLine +
    (target == -1 ? "------------------------------------------------------------------------------------" + Environment.NewLine : ""));
        }
    }

    #endregion
}
