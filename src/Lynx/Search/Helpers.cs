using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Lynx;

public sealed partial class Engine
{
    private const int MinValue = short.MinValue;
    private const int MaxValue = short.MaxValue;

    /// <summary>
    /// Returns the score evaluation of a move taking into account <see cref="_isScoringPV"/>, <paramref name="bestMoveTTCandidate"/>, <see cref="EvaluationConstants.MostValueableVictimLeastValuableAttacker"/>, <see cref="_killerMoves"/> and <see cref="_quietHistory"/>
    /// </summary>
    /// <param name="move"></param>
    /// <param name="ply"></param>
    /// <param name="isNotQSearch"></param>
    /// <param name="bestMoveTTCandidate"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int ScoreMove(Move move, int ply, bool isNotQSearch, ShortMove bestMoveTTCandidate = default)
    {
        if (_isScoringPV && move == _pVTable[ply])
        {
            _isScoringPV = false;

            return EvaluationConstants.PVMoveScoreValue;
        }

        if ((ShortMove)move == bestMoveTTCandidate)
        {
            return EvaluationConstants.TTMoveScoreValue;
        }

        var promotedPiece = move.PromotedPiece();
        var isPromotion = promotedPiece != default;
        var isCapture = move.IsCapture();

        // Queen promotion
        if ((promotedPiece + 2) % 6 == 0)
        {
            var baseScore = SEE.HasPositiveScore(Game.CurrentPosition, move)
                ? EvaluationConstants.GoodCaptureMoveBaseScoreValue
                : EvaluationConstants.BadCaptureMoveBaseScoreValue;

            var captureBonus = isCapture ? 1 : 0;

            return baseScore + EvaluationConstants.PromotionMoveScoreValue + captureBonus;
        }

        if (isCapture)
        {
            var baseCaptureScore = (isPromotion || move.IsEnPassant() || SEE.IsGoodCapture(Game.CurrentPosition, move))
                ? EvaluationConstants.GoodCaptureMoveBaseScoreValue
                : EvaluationConstants.BadCaptureMoveBaseScoreValue;

            var piece = move.Piece();
            var capturedPiece = move.CapturedPiece();

            Debug.Assert(capturedPiece != (int)Piece.K && capturedPiece != (int)Piece.k, $"{move.UCIString()} capturing king is generated in position {Game.CurrentPosition.FEN()}");

            return baseCaptureScore
                + EvaluationConstants.MostValueableVictimLeastValuableAttacker[piece][capturedPiece]
                //+ EvaluationConstants.MVV_PieceValues[capturedPiece]
                + _captureHistory[piece][move.TargetSquare()][capturedPiece];
        }

        if (isPromotion)
        {
            return EvaluationConstants.PromotionMoveScoreValue;
        }

        if (isNotQSearch)
        {
            // 1st killer move
            if (_killerMoves[0][ply] == move)
            {
                return EvaluationConstants.FirstKillerMoveValue;
            }

            // 2nd killer move
            if (_killerMoves[1][ply] == move)
            {
                return EvaluationConstants.SecondKillerMoveValue;
            }

            // 3rd killer move
            if (_killerMoves[2][ply] == move)
            {
                return EvaluationConstants.ThirdKillerMoveValue;
            }

            // Counter move history
            if (ply >= 1)
            {
                var previousMove = Game.PopFromMoveStack(ply - 1);
                Debug.Assert(previousMove != 0);

                // Counter move and follow up history
                //if (ply >= 2)
                //{
                //    var previousPreviousMove = Game.MoveStack[ply - 2];

                //    return EvaluationConstants.BaseMoveScore
                //        + _quietHistory[move.Piece()][move.TargetSquare()]
                //        + _continuationHistory[move.Piece()][move.TargetSquare()][0][previousMove.Piece()][previousMove.TargetSquare()]
                //        + _continuationHistory[move.Piece()][move.TargetSquare()][1][previousPreviousMove.Piece()][previousPreviousMove.TargetSquare()];
                //}
                return EvaluationConstants.BaseMoveScore
                    + _quietHistory[move.Piece()][move.TargetSquare()]
                    + _continuationHistory[move.Piece()][move.TargetSquare()][0][previousMove.Piece()][previousMove.TargetSquare()];
            }

            // History move or 0 if not found
            return EvaluationConstants.BaseMoveScore
                + _quietHistory[move.Piece()][move.TargetSquare()];
        }

        return EvaluationConstants.BaseMoveScore;
    }

    /// <summary>
    /// Soft caps history score
    /// Formula taken from EP discord, https://discord.com/channels/1132289356011405342/1132289356447625298/1141102105847922839
    /// </summary>
    /// <param name="score"></param>
    /// <param name="rawHistoryBonus"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ScoreHistoryMove(int score, int rawHistoryBonus)
    {
        return score + rawHistoryBonus - (score * Math.Abs(rawHistoryBonus) / Configuration.EngineSettings.History_MaxMoveValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrefetchTTEntry()
    {
        if (Sse.IsSupported)
        {
            var index = Game.CurrentPosition.UniqueIdentifier & _ttMask;

            unsafe
            {
                // Since _tt is a pinned array
                // This is no-op pinning as it does not influence the GC compaction
                // https://tooslowexception.com/pinned-object-heap-in-net-5/
                fixed (TranspositionTableElement* ttPtr = _tt)
                {
                    Sse.Prefetch0(ttPtr + index);
                }
            }
        }
    }

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
    /// <param name="target"></param>
    /// <param name="source"></param>
    /// <param name="moveCountToCopy"></param>
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

            var newPosition = new Position(position, move);
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
            _logger.Trace($"{depthStr}{(isQuiescence ? "[Qui] " : "")}{move.ToEPDString(position),-6} | {evaluation}{(prune ? " | prnning" : "")}");
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
    /// <param name="target"></param>
    /// <param name="source"></param>
    /// <param name="movesToCopy"></param>
    /// <param name="depth"></param>
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
        int max = int.MinValue;

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

        _logger.Debug($"Max history: {max}");
    }

#pragma warning restore S125 // Sections of code should not be commented out
#pragma warning restore S1199 // Nested code blocks should not be used

    #endregion
}
