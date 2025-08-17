namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public ref struct TTResult
{
    public int Score;

    public short BestMove;

    public NodeType NodeType;

    public int StaticEval;

    public int Depth;

    public bool WasPv;

    public TTResult()
    {
        Score = EvaluationConstants.NoScore;
        StaticEval = EvaluationConstants.NoScore;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
