﻿using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx;

public sealed partial class Engine
{
#pragma warning disable RCS1226 // Add paragraph to documentation comment
#pragma warning disable RCS1243 // Duplicate word in a comment
    /// <summary>
    /// When asked to copy an incomplete PV one level ahead, clears the rest of the PV Table+
    /// PV Table at depth 3
    /// Copying 60 moves
    /// src: 250, tgt: 190
    ///  0   Qxb2   Qxb2   h4     a8     a8     a8     a8     a8
    ///  64         b1=Q   exf6   Kxf6   a8     a8     a8     a8
    ///  127               a8     b1=Q   Qxb1   Qxb1   a8     a8
    ///  189                      Qxb1   Qxb1   Qxb1   a8     a8
    ///  250                             a8     Qxb1   a8     a8
    ///  310                                    a8     a8     a8
    ///
    /// PV Table at depth 3
    ///  0   Qxb2   Qxb2   h4     a8     a8     a8     a8     a8
    ///  64         b1=Q   exf6   Kxf6   a8     a8     a8     a8
    ///  127               a8     b1=Q   Qxb1   Qxb1   a8     a8
    ///  189                      Qxb1   a8     a8     a8     a8
    ///  250                             a8     a8     a8     a8
    ///  310                                    a8     a8     a8
    /// </summary>
#pragma warning restore RCS1243 // Duplicate word in a comment
#pragma warning restore RCS1226 // Add paragraph to documentation comment
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CopyPVTableMoves(int target, int source, int moveCountToCopy)
    {
        if (_pVTable[source] == default)
        {
            Array.Clear(_pVTable, target, _pVTable.Length - target);
            return;
        }

        //PrintPvTable(target: target, source: source, movesToCopy: moveCountToCopy);
        Array.Copy(_pVTable, source, _pVTable, target, moveCountToCopy);
        //PrintPvTable();
    }

    /// <summary>
    /// [12][64][12]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CaptureHistoryIndex(int piece, int targetSquare, int capturedPiece)
    {
        const int pieceOffset = 64 * 12;
        const int targetSquareOffset = 12;

        return (piece * pieceOffset)
            + (targetSquare * targetSquareOffset)
            + capturedPiece;
    }

    /// <summary>
    /// [12][64][12][64][ContinuationHistoryPlyCount]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ContinuationHistoryIndex(int piece, int targetSquare, int previousMovePiece, int previousMoveTargetSquare, int ply)
    {
        const int pieceOffset = 64 * 12 * 64 * EvaluationConstants.ContinuationHistoryPlyCount;
        const int targetSquareOffset = 12 * 64 * EvaluationConstants.ContinuationHistoryPlyCount;
        const int previousMovePieceOffset = 64 * EvaluationConstants.ContinuationHistoryPlyCount;
        const int previousMoveTargetSquareOffset = EvaluationConstants.ContinuationHistoryPlyCount;

        return (piece * pieceOffset)
            + (targetSquare * targetSquareOffset)
            + (previousMovePiece * previousMovePieceOffset)
            + (previousMoveTargetSquare * previousMoveTargetSquareOffset)
            + ply;
    }

    /// <summary>
    /// [64][64]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CounterMoveIndex(int previousMoveSourceSquare, int previousMoveTargetSquare)
    {
        const int sourceSquareOffset = 64;

        return (previousMoveSourceSquare * sourceSquareOffset)
            + previousMoveTargetSquare;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMoveNodeCount(Move move, ulong nodesToAdd)
    {
        _moveNodeCount[move.Piece()][move.TargetSquare()] += nodesToAdd;
    }

    #region Debugging

#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable S1199 // Nested code blocks should not be used

    [Conditional("DEBUG")]
    private void ValidatePVTable()
    {
        var position = Game.CurrentPosition;
        for (int i = 0; i < PVTable.Indexes[1]; ++i)
        {
            if (_pVTable[i] == default)
            {
                for (int j = i + 1; j < PVTable.Indexes[1]; ++j)
                {
                    Utils.Assert(_pVTable[j] == default, $"Not expecting a move in _pvTable[{j}]");
                }
                break;
            }

            var move = _pVTable[i];
            TryParseMove(position, i, move);

            var newPosition = new Position(position);
            newPosition.MakeMove(move);
            if (!newPosition.WasProduceByAValidMove())
            {
                throw new AssertException($"Invalid position after move {move.UCIString()} from position {position.FEN()}");
            }
            position = newPosition;
        }

        static void TryParseMove(Position position, int i, int move)
        {
            Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];

            if (!MoveExtensions.TryParseFromUCIString(
               move.UCIString(),
               MoveGenerator.GenerateAllMoves(position, movePool),
               out _))
            {
                var message = $"Unexpected PV move {i}: {move.UCIString()} from position {position.FEN()}";
                _logger.Error(message);
                throw new AssertException(message);
            }
        }
    }

    [Conditional("DEBUG")]
    private static void PrintPreMove(Position position, int plies, Move move, bool isQuiescence = false)
    {
        if (_logger.IsTraceEnabled)
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
#pragma warning disable CS0618 // Type or member is obsolete
                _logger.Trace($"{Environment.NewLine}{depthStr}{(isQuiescence ? "[Qui] " : "")}{move.ToEPDString(position)} ({position.Side}, {plies})");
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }

    [Conditional("DEBUG")]
    private static void PrintMove(Position position, int plies, Move move, int evaluation, bool isQuiescence = false, bool prune = false)
    {
        if (_logger.IsTraceEnabled)
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

#pragma warning disable CS0618 // Type or member is obsolete
            _logger.Trace($"{depthStr}{(isQuiescence ? "[Qui] " : "")}{move.ToEPDString(position),-6} | {evaluation}{(prune ? " | pruning" : "")}");
#pragma warning restore CS0618 // Type or member is obsolete

            //Console.ResetColor();
        }
    }

    [Conditional("DEBUG")]
    private static void PrintMessage(int plies, string message)
    {
        if (_logger.IsTraceEnabled)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= plies; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            _logger.Trace(depthStr + message);
        }
    }

    [Conditional("DEBUG")]
    private static void PrintMessage(string message)
    {
        if (_logger.IsTraceEnabled)
        {
            _logger.Trace(message);
        }
    }

    /// <summary>
    /// Assumes Configuration.EngineSettings.MaxDepth = 64
    /// </summary>
    [Conditional("DEBUG")]
    private void PrintPvTable(int target = -1, int source = -1, int movesToCopy = 0, int depth = 0)
    {
        if (depth != default)
        {
            Console.WriteLine($"PV Table at depth {depth}");
        }
        if (movesToCopy != default)
        {
            Console.WriteLine($"Copying {movesToCopy} moves");
        }

#pragma warning disable CS0618 // Type or member is obsolete
        Console.WriteLine(
(target != -1 ? $"src: {source}, tgt: {target}" + Environment.NewLine : "") +
$" {0,-3} {_pVTable[0].ToEPDString(),-6} {_pVTable[1].ToEPDString(),-6} {_pVTable[2].ToEPDString(),-6} {_pVTable[3].ToEPDString(),-6} {_pVTable[4].ToEPDString(),-6} {_pVTable[5].ToEPDString(),-6} {_pVTable[6].ToEPDString(),-6} {_pVTable[7].ToEPDString(),-6} {_pVTable[8].ToEPDString(),-6} {_pVTable[9].ToEPDString(),-6} {_pVTable[10].ToEPDString(),-6}" + Environment.NewLine +
$" {64,-3}        {_pVTable[64].ToEPDString(),-6} {_pVTable[65].ToEPDString(),-6} {_pVTable[66].ToEPDString(),-6} {_pVTable[67].ToEPDString(),-6} {_pVTable[68].ToEPDString(),-6} {_pVTable[69].ToEPDString(),-6} {_pVTable[70].ToEPDString(),-6} {_pVTable[71].ToEPDString(),-6} {_pVTable[72].ToEPDString(),-6} {_pVTable[73].ToEPDString(),-6}" + Environment.NewLine +
$" {127,-3}               {_pVTable[127].ToEPDString(),-6} {_pVTable[128].ToEPDString(),-6} {_pVTable[129].ToEPDString(),-6} {_pVTable[130].ToEPDString(),-6} {_pVTable[131].ToEPDString(),-6} {_pVTable[132].ToEPDString(),-6} {_pVTable[133].ToEPDString(),-6} {_pVTable[134].ToEPDString(),-6} {_pVTable[135].ToEPDString(),-6}" + Environment.NewLine +
$" {189,-3}                      {_pVTable[189].ToEPDString(),-6} {_pVTable[190].ToEPDString(),-6} {_pVTable[191].ToEPDString(),-6} {_pVTable[192].ToEPDString(),-6} {_pVTable[193].ToEPDString(),-6} {_pVTable[194].ToEPDString(),-6} {_pVTable[195].ToEPDString(),-6} {_pVTable[196].ToEPDString(),-6}" + Environment.NewLine +
$" {250,-3}                             {_pVTable[250].ToEPDString(),-6} {_pVTable[251].ToEPDString(),-6} {_pVTable[252].ToEPDString(),-6} {_pVTable[253].ToEPDString(),-6} {_pVTable[254].ToEPDString(),-6} {_pVTable[255].ToEPDString(),-6} {_pVTable[256].ToEPDString(),-6}" + Environment.NewLine +
$" {310,-3}                                    {_pVTable[310].ToEPDString(),-6} {_pVTable[311].ToEPDString(),-6} {_pVTable[312].ToEPDString(),-6} {_pVTable[313].ToEPDString(),-6} {_pVTable[314].ToEPDString(),-6} {_pVTable[315].ToEPDString(),-6}" + Environment.NewLine +
$" {369,-3}                                           {_pVTable[369].ToEPDString(),-6} {_pVTable[370].ToEPDString(),-6} {_pVTable[371].ToEPDString(),-6} {_pVTable[372].ToEPDString(),-6} {_pVTable[373].ToEPDString(),-6}" + Environment.NewLine +
$" {427,-3}                                                  {_pVTable[427].ToEPDString(),-6} {_pVTable[428].ToEPDString(),-6} {_pVTable[429].ToEPDString(),-6} {_pVTable[430].ToEPDString(),-6}" + Environment.NewLine +
$" {484,-3}                                                         {_pVTable[484].ToEPDString(),-6} {_pVTable[485].ToEPDString(),-6} {_pVTable[486].ToEPDString(),-6}" + Environment.NewLine +
(target == -1 ? "------------------------------------------------------------------------------------" + Environment.NewLine : ""));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Conditional("DEBUG")]
    internal void PrintHistoryMoves()
    {
        int max = EvaluationConstants.MinEval;

        for (int i = 0; i < 12; ++i)
        {
            var tmp = _quietHistory[i];
            for (int j = 0; j < 64; ++j)
            {
                var item = tmp[j];

                if (item > max)
                {
                    max = item;
                }
            }
        }

        _logger.ConditionalDebug($"Max history: {max}");
    }

#pragma warning restore S125 // Sections of code should not be commented out
#pragma warning restore S1199 // Nested code blocks should not be used

    #endregion
}
