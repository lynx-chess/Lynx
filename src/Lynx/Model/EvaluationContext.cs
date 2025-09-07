namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public ref struct EvaluationContext
{
    public Span<BitBoard> Attacks;
    public Span<BitBoard> AttacksBySide;

    public EvaluationContext(Span<BitBoard> attacks, Span<BitBoard> attacksBySide)
    {
        Attacks = attacks;
        AttacksBySide = attacksBySide;

        Attacks.Clear();
        AttacksBySide.Clear();
    }

    public void EnsureThreatsAreCalculated(Position position)
    {
        if(AttacksBySide[(int)Side.White] == 0)
        {
            position.CalculateThreats(ref this);
        }
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
