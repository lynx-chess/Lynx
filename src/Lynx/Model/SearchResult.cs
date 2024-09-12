using Lynx.UCI.Commands.Engine;

namespace Lynx.Model;

public sealed class SearchResult
{
    public Move BestMove { get; init; }

    public int Evaluation { get; init; }

    public int Depth { get; set; }

    public Move[] Moves { get; init; }

    public int Alpha { get; init; }

    public int Beta { get; init; }

    public int Mate { get; init; }

    public int DepthReached { get; set; }

    public long Nodes { get; set; }

    public long Time { get; set; }

    public long NodesPerSecond { get; set; }

    public bool IsCancelled { get; set; }

    public int HashfullPermill { get; set; } = -1;

    public (int WDLWin, int WDLDraw, int WDLLoss)? WDL { get; set; } = null;

    public SearchResult(Move bestMove, int evaluation, int targetDepth, Move[] moves, int alpha, int beta, int mate = default)
    {
        BestMove = bestMove;
        Evaluation = evaluation;
        Depth = targetDepth;
        Moves = moves;
        Alpha = alpha;
        Beta = beta;
        Mate = mate;
    }

    public override string ToString()
    {
        var sb = ObjectPools.StringBuilderPool.Get();

        sb.Append(InfoCommand.Id)
          .Append(" depth ").Append(Depth)
          .Append(" seldepth ").Append(DepthReached)
          .Append(" multipv 1")
          .Append(" score ").Append(Mate == default ? "cp " + Lynx.WDL.NormalizeScore(Evaluation) : "mate " + Mate)
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
        foreach (var move in Moves)
        {
            sb.Append(move.UCIStringMemoized()).Append(' ');
        }

        // Remove the trailing space
        if (Moves.Length > 0)
        {
            sb.Length--;
        }

        var result = sb.ToString();

        ObjectPools.StringBuilderPool.Return(sb);

        return result;
    }
}
