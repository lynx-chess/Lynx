namespace Lynx.Model;

public readonly struct EvaluationResult
{
    public int Evaluation { get; }

    public int Phase { get; }

    public EvaluationResult(int evaluation, int phase)
    {
        Evaluation = evaluation;
        Phase = phase;
    }
}
