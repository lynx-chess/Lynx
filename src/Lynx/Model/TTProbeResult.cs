namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public ref struct TTProbeResult
{
    public int Score;

    public int StaticEval;

    public int Depth;

    public short BestMove;

    public NodeType NodeType;

    public bool WasPv;

    public TTProbeResult()
    {
        Score = EvaluationConstants.NoScore;
        StaticEval = EvaluationConstants.NoScore;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
