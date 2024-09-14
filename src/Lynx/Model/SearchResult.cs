using Lynx.UCI.Commands.Engine;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public struct SearchResult
{
    private readonly Move[] _moves;
    public readonly Move[] Moves
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _moves;
    }

    public (int WDLWin, int WDLDraw, int WDLLoss)? WDL = null;

    public long Nodes;

    public long Time;

    public long NodesPerSecond;

    public int Depth;

    private readonly int _mate;
    public readonly int Mate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _mate;
    }

    public int DepthReached;

    public int HashfullPermill = -1;

    private readonly int _evaluation = int.MinValue;
    public readonly int Evaluation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _evaluation;
    }

    private readonly Move _bestMove;
    public readonly Move BestMove
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _bestMove;
    }

    public SearchResult(Move bestMove, int evaluation, int targetDepth, Move[] moves, int mate = default)
    {
        _bestMove = bestMove;
        _evaluation = evaluation;
        Depth = targetDepth;
        _moves = moves;
        _mate = mate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsDefault() => _bestMove == default;

    public override readonly string ToString()
    {
        var sb = ObjectPools.StringBuilderPool.Get();

        sb.Append(InfoCommand.Id)
          .Append(" depth ").Append(Depth)
          .Append(" seldepth ").Append(DepthReached)
          .Append(" multipv 1")
          .Append(" score ").Append(_mate == default ? "cp " + Lynx.WDL.NormalizeScore(_evaluation) : "mate " + _mate)
          .Append(" nodes ").Append(Nodes)
          .Append(" nps ").Append(NodesPerSecond)
          .Append(" time ").Append(Time);

        if (HashfullPermill != -1)
        {
            sb.Append(" hashfull ").Append(HashfullPermill);
        }

        if (WDL is not null)
        {
            sb.Append(" wdl ")
              .Append(WDL.Value.WDLWin).Append(' ')
              .Append(WDL.Value.WDLDraw).Append(' ')
              .Append(WDL.Value.WDLLoss);
        }

        sb.Append(" pv ");
        foreach (var move in _moves)
        {
            sb.Append(move.UCIStringMemoized()).Append(' ');
        }

        // Remove the trailing space
        if (_moves.Length > 0)
        {
            sb.Length--;
        }

        var result = sb.ToString();

        ObjectPools.StringBuilderPool.Return(sb);

        return result;
    }
}
