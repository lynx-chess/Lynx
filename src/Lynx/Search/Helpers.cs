using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx;

public record SearchResult(Move BestMove, double Evaluation, int TargetDepth, int DepthReached, int Nodes, long Time, long NodesPerSecond, List<Move> Moves, int Alpha, int Beta, int Mate = default)
{
    public bool IsCancelled { get; set; }

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
    private const int MinValue = -2 * EvaluationConstants.CheckMateEvaluation;
    private const int MaxValue = +2 * EvaluationConstants.CheckMateEvaluation;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private List<Move> SortMoves(List<Move> moves, Position currentPosition, int depth)
    {
        if (_isFollowingPV)
        {
            _isFollowingPV = false;
            for (int moveIndex = 0; moveIndex < moves.Count; ++moveIndex)
            {
                if (moves[moveIndex].EncodedMove == _pVTable[depth].EncodedMove)
                {
                    _isFollowingPV = true;
                    _isScoringPV = true;
                }
            }
        }

        return moves.OrderByDescending(move => Score(move, currentPosition, depth)).ToList();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Score(Move move, Position position, int depth)
    {
        if (_isScoringPV && move.EncodedMove == _pVTable[depth].EncodedMove)
        {
            _isScoringPV = false;

            return EvaluationConstants.PVMoveValue;
        }

        return move.Score(position, _killerMoves, depth, _historyMoves);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IOrderedEnumerable<Move> SortCaptures(List<Move> moves, Position currentPosition, int depth) => moves.OrderByDescending(move => Score(move, currentPosition, depth));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CopyPVTableMoves(int target, int source, int moveCountToCopy)
    {
        if (_pVTable[source].EncodedMove == default)
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
        for (int i = 0; i < PVTable.Indexes[1]; ++i)
        {
            if (_pVTable[i].EncodedMove == default)
            {
                for (int j = i + 1; j < PVTable.Indexes[1]; ++j)
                {
                    Utils.Assert(_pVTable[j].EncodedMove == default, $"Not expecting a move in _pvTable[{j}]");
                }
                break;
            }
            var move = _pVTable[i];

            if (!Move.TryParseFromUCIString(
               move.UCIString(),
               position.AllPossibleMoves(),
               out _))
            {
                var message = $"Unexpected PV move {move.UCIString()} from position {position.FEN}";
                _logger.Error(message);
                throw new AssertException(message);
            }

            var newPosition = new Position(position, move);

            if (!newPosition.WasProduceByAValidMove())
            {
                throw new AssertException($"Invalid position after move {move.UCIString()} from position {position.FEN}");
            }
            position = newPosition;
        }
    }

    [Conditional("DEBUG")]
    private static void PrintPreMove(Position position, int plies, Move move, bool isQuiescence = false)
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
        Console.WriteLine(
(target != -1 ? $"src: {source}, tgt: {target}" + Environment.NewLine : "") +
$" {0,-3} {_pVTable[0],-6} {_pVTable[1],-6} {_pVTable[2],-6} {_pVTable[3],-6} {_pVTable[4],-6} {_pVTable[5],-6} {_pVTable[6],-6} {_pVTable[7],-6}" + Environment.NewLine +
$" {64,-3}        {_pVTable[64],-6} {_pVTable[65],-6} {_pVTable[66],-6} {_pVTable[67],-6} {_pVTable[68],-6} {_pVTable[69],-6} {_pVTable[70],-6}" + Environment.NewLine +
$" {127,-3}               {_pVTable[127],-6} {_pVTable[128],-6} {_pVTable[129],-6} {_pVTable[130],-6} {_pVTable[131],-6} {_pVTable[132],-6}" + Environment.NewLine +
$" {189,-3}                      {_pVTable[189],-6} {_pVTable[190],-6} {_pVTable[191],-6} {_pVTable[192],-6} {_pVTable[193],-6}" + Environment.NewLine +
$" {250,-3}                             {_pVTable[250],-6} {_pVTable[251],-6} {_pVTable[252],-6} {_pVTable[253],-6}" + Environment.NewLine +
$" {310,-3}                                    {_pVTable[310],-6} {_pVTable[311],-6} {_pVTable[312],-6}" + Environment.NewLine +
(target == -1 ? "------------------------------------------------------------------------------------" + Environment.NewLine : ""));
    }

    #endregion
}
