namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly ref struct TTResult
{
    public readonly int Score = EvaluationConstants.NoScore;

    public readonly short BestMove;

    public readonly NodeType NodeType;

    public readonly int StaticEval = EvaluationConstants.NoScore;

    public readonly int Depth;

    public readonly bool WasPv;

    public TTResult(int score, short bestMove, NodeType nodeType, int staticEval, int depth, bool wasPv)
    {
        Score = score;
        BestMove = bestMove;
        NodeType = nodeType;
        StaticEval = staticEval;
        Depth = depth;
        WasPv = wasPv;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
