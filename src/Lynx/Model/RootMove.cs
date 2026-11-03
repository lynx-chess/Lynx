namespace Lynx.Model;

public struct RootMove
{
    public const int DefaultScore = EvaluationConstants.MinEval;

    public Move Move { get; set; }

    /// <summary>
    /// Score of the last aspiration window search where this move was the best one of its PV line
    /// Used to center the window of the next depth and when sorting the PV lines
    /// </summary>
    public int Score { get; set; }

    public Move[] PV { get; set; }

    public RootMove(Move move)
    {
        Move = move;
        Score = DefaultScore;
        PV = [];
    }
}
