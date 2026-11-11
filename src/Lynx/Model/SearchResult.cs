using Lynx.UCI.Commands.Engine;
using System.Diagnostics;
using System.Text;

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

    /// <summary>
    /// 1-based index of the PV line
    /// </summary>
    public int MultiPVIndex { get; init; } = 1;

    /// <summary>
    /// Empty unless in a root instance of a MultiPV search
    /// (<see cref="Configuration.EngineSettings.MultiPV"/> &gt; 1 and <see cref="MultiPVIndex"/> == 1)
    /// </summary>
    public SearchResult[] ExtraLines { get; init; } = [];

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
        Debug.Assert(Depth >= 1, "Depth cannot be less than 1");
        Debug.Assert(DepthReached >= 1, "Seldepth cannot be less than 1");

        var sb = ObjectPools.StringBuilderPool.Get();
        sb.EnsureCapacity(128 + (Moves.Length * 5));

        AppendInfoLine(sb, this);

        // MultiPV: the rest of the lines are written in the SB together with the first one, so that they are all sent to UCI at the same time
        // They all share nodes, time, nps and hashfull
        foreach (var line in ExtraLines)
        {
            sb.AppendLine();
            line.AppendInfoLine(sb, this);
        }

        var result = sb.ToString();

        ObjectPools.StringBuilderPool.Return(sb);

        return result;
    }

    private void AppendInfoLine(StringBuilder sb, SearchResult shared)
    {
#if MULTITHREAD_DEBUG
        sb.Append("[#").Append(EngineId).Append("] ");
#endif

        var nps = shared.NodesPerSecond;

        if (Configuration.EngineSettings.Threads > 1
            && Configuration.EngineSettings.EstimateMultithreadedSearchNPS
            && shared.HashfullPermill == -1)   // Not last info command
        {
            // Estimate total nps
            nps *= (ulong)Configuration.EngineSettings.Threads;

            // Remove the 5 less significative digits to hint that this is an estimate
            const int k = 100_000;
            nps = nps / k * k;
        }

#pragma warning disable MA0075 // Do not use implicit culture-sensitive ToString
        sb.Append(InfoCommand.Id)
          .Append(" depth ").Append(Depth)
          .Append(" seldepth ").Append(DepthReached)
          .Append(" multipv ").Append(MultiPVIndex)
          .Append(" score ").Append(Mate == default ? "cp " + Lynx.WDL.NormalizeScore(Score) : "mate " + Mate)
          .Append(" nodes ").Append(shared.Nodes)
          .Append(" nps ").Append(nps)
          .Append(" time ").Append(shared.Time);
#pragma warning restore MA0075 // Do not use implicit culture-sensitive ToString

        if (shared.HashfullPermill != -1)
        {
            sb.Append(" hashfull ").Append(shared.HashfullPermill);
        }

        // Extra lines don't have their own WDL, since it depends on their score
        var wdl = ReferenceEquals(this, shared) || shared.WDL is null
            ? WDL
            : Lynx.WDL.WDLModel(Score, Depth);

        if (wdl is not null)
        {
            var (wdlWin, wdlDraw, wdlLoss) = wdl.Value;

            sb.Append(" wdl ")
              .Append(wdlWin).Append(' ')
              .Append(wdlDraw).Append(' ')
              .Append(wdlLoss);
        }

        sb.Append(" pv ");
        foreach (var move in Moves)
        {
            sb.Append(move.UCIString()).Append(' ');
        }

        // Remove the trailing space
        if (Moves.Length > 0)
        {
            sb.Length--;
        }
    }
}
