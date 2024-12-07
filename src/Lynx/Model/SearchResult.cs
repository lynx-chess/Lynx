using Lynx.UCI.Commands.Engine;

namespace Lynx.Model;

public sealed class SearchResult
{
#if MULTITHREAD_DEBUG
    public string EngineId { get; init; }
#endif
    public Move[] Moves { get; init; }

    public (int WDLWin, int WDLDraw, int WDLLoss)? WDL { get; set; } = null;

    public ulong Nodes { get; set; }

    public ulong Time { get; set; }

    public ulong NodesPerSecond { get; set; }

    public int Score { get; init; }

    public int Depth { get; set; }

    public int Mate { get; init; }

    public int DepthReached { get; set; }

    public int HashfullPermill { get; set; } = -1;

    public Move BestMove { get; init; }

    public SearchResult(
#if MULTITHREAD_DEBUG
        string engineId,
#endif
        Move bestMove, int score, int targetDepth, Move[] moves, int mate = default)
    {
#if MULTITHREAD_DEBUG
        EngineId = engineId;
#endif
        BestMove = bestMove;
        Score = score;
        Depth = targetDepth;
        Moves = moves;
        Mate = mate;
    }

    public override string ToString()
    {
        var sb = ObjectPools.StringBuilderPool.Get();

#if MULTITHREAD_DEBUG
        sb.Append("[#" + EngineId + "] ");
#endif

        sb.Append(InfoCommand.Id)
          .Append(" depth ").Append(Depth)
          .Append(" seldepth ").Append(DepthReached)
          .Append(" multipv 1")
          .Append(" score ").Append(Mate == default ? "cp " + Lynx.WDL.NormalizeScore(Score) : "mate " + Mate)
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
