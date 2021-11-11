﻿using Lynx.Model;
using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx.Search
{
    public record SearchResult(Move BestMove, double Evaluation, int TargetDepth, int DepthReached, int Nodes, long Time, long NodesPerSecond, List<Move> Moves)
    {
        public bool IsCancelled { get; set; }
    }

    public sealed partial class Search
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int MinValue = -2 * Position.CheckMateEvaluation;
        private const int MaxValue = +2 * Position.CheckMateEvaluation;

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

            return move.Score(position, _killerMoves, depth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IOrderedEnumerable<Move> SortCaptures(List<Move> moves, Position currentPosition, int depth) => moves.OrderByDescending(move => Score(move, currentPosition, depth));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int UpdatePositionHistory(Position newPosition) => Utils.UpdatePositionHistory(newPosition, _positionHistory);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RevertPositionHistory(Position newPosition, int repetitions) => Utils.RevertPositionHistory(newPosition, _positionHistory, repetitions);

        /// <summary>
        /// Updates <paramref name="MovesWithoutCaptureOrPawnMove"/>
        /// </summary>
        /// <param name="moveToPlay"></param>
        /// <remarks>
        /// Checking movesWithoutCaptureOrPawnMove >= 50 since a capture/pawn move doesn't necessarily 'clear' the variable.
        /// i.e. while the engine is searching:
        ///     At depth 2, 50 rules move applied and eval is 0
        ///     At depth 3, there's a capture, but the eval should still be 0
        ///     At depth 4 there's no capture, but the eval should still be 0
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Update50movesRule(Move moveToPlay) => Utils.Update50movesRule(moveToPlay, _movesWithoutCaptureOrPawnMove);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CopyPVTableMoves(int target, int source, int moveCountToCopy)
        {
            //PrintPvTable(target, source);
            Array.Copy(_pVTable, source, _pVTable, target, moveCountToCopy);
            //PrintPvTable(pvTable);
        }

        #region Debugging

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
    $" {32,-3}        {_pVTable[32],-6} {_pVTable[33],-6} {_pVTable[34],-6} {_pVTable[35],-6} {_pVTable[36],-6} {_pVTable[37],-6} {_pVTable[38],-6}" + Environment.NewLine +
    $" {63,-3}               {_pVTable[63],-6} {_pVTable[64],-6} {_pVTable[65],-6} {_pVTable[66],-6} {_pVTable[67],-6} {_pVTable[68],-6}" + Environment.NewLine +
    $" {93,-3}                      {_pVTable[93],-6} {_pVTable[94],-6} {_pVTable[95],-6} {_pVTable[96],-6} {_pVTable[97],-6}" + Environment.NewLine +
    $" {122,-3}                             {_pVTable[122],-6} {_pVTable[123],-6} {_pVTable[124],-6} {_pVTable[125],-6}" + Environment.NewLine +
    $" {150,-3}                                    {_pVTable[150],-6} {_pVTable[151],-6} {_pVTable[152],-6}" + Environment.NewLine +
    (target == -1 ? "------------------------------------------------------------------------------------" + Environment.NewLine : ""));
        }

        #endregion
    }
}
