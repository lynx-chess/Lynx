namespace Lynx.Model;

public class SearchResult
{
    public Move BestMove { get; init; }
    public int Evaluation { get; init; }
    public int Depth { get; set; }
    public List<Move> Moves { get; init; }
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

    public SearchResult(Move bestMove, int evaluation, int targetDepth, List<Move> moves, int alpha, int beta, int mate = default)
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
