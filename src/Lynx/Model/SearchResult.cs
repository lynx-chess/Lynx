﻿namespace Lynx.Model;

public class SearchResult
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
}
