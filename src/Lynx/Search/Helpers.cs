using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx;

public class SearchResult
{
    public Move BestMove { get; init; }
    public double Evaluation { get; init; }
    public int Depth { get; set; }
    public List<Move> Moves { get; init; }
    public int Alpha { get; init; }
    public int Beta { get; init; }
    public int Mate { get; init; }

    public int DepthReached { get; set; }

    public int Nodes { get; set; }

    public long Time { get; set; }

    public long NodesPerSecond { get; set; }

    public bool IsCancelled { get; set; }

    public int HashfullPermill { get; set; } = -1;

    public (int WDLWin, int WDLDraw, int WDLLoss)? WDL { get; set; } = null;

    public SearchResult(Move bestMove, double evaluation, int targetDepth, List<Move> moves, int alpha, int beta, int mate = default)
    {
        BestMove = bestMove;
        Evaluation = evaluation;
        Depth = targetDepth;
        Moves = moves;
        Alpha = alpha;
        Beta = beta;
        Mate = mate;
    }

    public SearchResult(SearchResult previousSearchResult)
    {
        BestMove = previousSearchResult.Moves.ElementAtOrDefault(2);
        Evaluation = previousSearchResult.Evaluation;
        Depth = previousSearchResult.Depth - 2;
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
    private List<Move> SortMoves(IEnumerable<Move> moves, int depth, Move bestMoveTTCandidate)
    {
        if (_isFollowingPV)
        {
            _isFollowingPV = false;
            foreach (var move in moves)
            {
                if (move == _pVTable[depth])
                {
                    _isFollowingPV = true;
                    _isScoringPV = true;
                    break;
                }
            }
        }

        var orderedMoves = moves
            .OrderByDescending(move => ScoreMove(move, depth, true, bestMoveTTCandidate))
            .ToList();

        PrintMessage($"For position {Game.CurrentPosition.FEN()}:\n{string.Join(", ", orderedMoves.Select(m => $"{m.ToEPDString()} ({ScoreMove(m, depth, true, bestMoveTTCandidate)})"))})");

        return orderedMoves;
    }

    /// <summary>
    /// Returns the score evaluation of a move taking into account <see cref="_isScoringPV"/>, <paramref name="bestMoveTTCandidate"/>, <see cref="EvaluationConstants.MostValueableVictimLeastValuableAttacker"/>, <see cref="_killerMoves"/> and <see cref="_historyMoves"/>
    /// </summary>
    /// <param name="move"></param>
    /// <param name="depth"></param>
    /// <param name="useKillerAndPositionMoves"></param>
    /// <param name="bestMoveTTCandidate"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int ScoreMove(Move move, int depth, bool useKillerAndPositionMoves, Move bestMoveTTCandidate = default)
    {
        if (_isScoringPV && move == _pVTable[depth])
        {
            _isScoringPV = false;

            return EvaluationConstants.PVMoveScoreValue;
        }

        if (move == bestMoveTTCandidate)
        {
            return EvaluationConstants.TTMoveScoreValue;
        }

        var promotedPiece = move.PromotedPiece();

        // Queen promotion
        if ((promotedPiece + 2) % 6 == 0)
        {
            return EvaluationConstants.CaptureMoveBaseScoreValue + EvaluationConstants.PromotionMoveScoreValue;
        }

        if (move.IsCapture())
        {
            var sourcePiece = move.Piece();
            int targetPiece = (int)Piece.P;    // Important to initialize to P or p, due to en-passant captures

            var targetSquare = move.TargetSquare();
            var offset = Utils.PieceOffset(Game.CurrentPosition.Side);
            var oppositePawnIndex = (int)Piece.p - offset;

            var limit = (int)Piece.k - offset;
            for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
            {
                if (Game.CurrentPosition.PieceBitBoards[pieceIndex].GetBit(targetSquare))
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

        if (useKillerAndPositionMoves)
        {
            // 1st killer move
            if (_killerMoves[0, depth] == move)
            {
                return EvaluationConstants.FirstKillerMoveValue;
            }

            if (_killerMoves[1, depth] == move)
            {
                return EvaluationConstants.SecondKillerMoveValue;
            }

            // History move or 0 if not found
            return _historyMoves[move.Piece(), move.TargetSquare()];
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CopyPVTableMoves(int target, int source, int moveCountToCopy)
    {
        // When asked to copy an incomplete PV one level ahead, clears the rest of the PV Table+
        // PV Table at depth 3
        // Copying 60 moves
        // src: 250, tgt: 190
        //  0   Qxb2   Qxb2   h4     a8     a8     a8     a8     a8
        //  64         b1=Q   exf6   Kxf6   a8     a8     a8     a8
        //  127               a8     b1=Q   Qxb1   Qxb1   a8     a8
        //  189                      Qxb1   Qxb1   Qxb1   a8     a8
        //  250                             a8     Qxb1   a8     a8
        //  310                                    a8     a8     a8
        //
        // PV Table at depth 3
        //  0   Qxb2   Qxb2   h4     a8     a8     a8     a8     a8
        //  64         b1=Q   exf6   Kxf6   a8     a8     a8     a8
        //  127               a8     b1=Q   Qxb1   Qxb1   a8     a8
        //  189                      Qxb1   a8     a8     a8     a8
        //  250                             a8     a8     a8     a8
        //  310                                    a8     a8     a8
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

            if (!MoveExtensions.TryParseFromUCIString(
               move.UCIString(),
               MoveGenerator.GenerateAllMoves(position, Game.MovePool),
               out _))
            {
                var message = $"Unexpected PV move {i}: {move.UCIString()} from position {position.FEN()}";
                _logger.Error(message);
                throw new AssertException(message);
            }

            var newPosition = new Position(position, move);

            if (!newPosition.WasProduceByAValidMove())
            {
                throw new AssertException($"Invalid position after move {move.UCIString()} from position {position.FEN()}");
            }
            position = newPosition;
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
                _logger.Trace($"{Environment.NewLine}{depthStr}{(isQuiescence ? "[Qui] " : "")}{move.ToEPDString()} ({position.Side}, {plies})");
            }
        }
    }

    [Conditional("DEBUG")]
    private static void PrintMove(int plies, Move move, int evaluation, bool isQuiescence = false, bool prune = false)
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

            _logger.Trace($"{depthStr}{(isQuiescence ? "[Qui] " : "")}{move.ToEPDString(),-6} | {evaluation}{(prune ? " | prunning" : "")}");

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
    }

    #endregion
}
