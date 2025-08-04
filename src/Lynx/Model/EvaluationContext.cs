namespace Lynx.Model;

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
}
