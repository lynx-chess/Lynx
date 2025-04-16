using Lynx.UCI.Commands.Engine;

namespace Lynx.Model;

public sealed class SearchResult
{
#if MULTITHREAD_DEBUG
    public int EngineId { get; init; }
#endif

    public Move[] Moves { get; init; }

    public (int WDLWin, int WDLDraw, int WDLLoss)? WDL { get; set; }

    public ulong Nodes { get; set; }

    public ulong Time { get; set; }

    public ulong NodesPerSecond { get; set; }

    public int Score { get; init; }

    public int Depth { get; set; }

    public int Mate { get; init; }

    public int DepthReached { get; set; }

    public int HashfullPermill { get; set; } = -1;

    public Move BestMove { get; init; }

#if MULTITHREAD_DEBUG
    public SearchResult(Move bestMove, int score, int targetDepth, Move[] moves, int mate = default)
        : this(-2, bestMove, score, targetDepth, moves, mate)
    {
    }
#endif

    public SearchResult(
#if MULTITHREAD_DEBUG
        int engineId,
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
        sb.EnsureCapacity(128 + (Moves.Length * 5));

#if MULTITHREAD_DEBUG
        sb.Append("[#").Append(EngineId).Append("] ");
#endif

        var nps = NodesPerSecond;

        if (HashfullPermill != -1   // Not last info command
            && Configuration.EngineSettings.EstimateMultithreadedSearchNPS)
        {
            nps *= (ulong)Configuration.EngineSettings.Threads;
        }

        sb.Append(InfoCommand.Id)
          .Append(" depth ").Append(Depth)
          .Append(" seldepth ").Append(DepthReached)
          .Append(" multipv 1")
          .Append(" score ").Append(Mate == default ? "cp " + Lynx.WDL.NormalizeScore(Score) : "mate " + Mate)
          .Append(" nodes ").Append(Nodes)
          .Append(" nps ").Append(nps)
          .Append(" time ").Append(Time);

        if (HashfullPermill != -1)
        {
            sb.Append(" hashfull ").Append(HashfullPermill);
        }

        if (WDL is not null)
        {
            var (wdlWin, wdlDraw, wdlLoss) = WDL.Value;

            sb.Append(" wdl ")
              .Append(wdlWin).Append(' ')
              .Append(wdlDraw).Append(' ')
              .Append(wdlLoss);
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
