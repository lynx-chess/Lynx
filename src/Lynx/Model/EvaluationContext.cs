namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public ref struct EvaluationContext
{
    public Span<BitBoard> Attacks;
    public Span<BitBoard> AttacksBySide;

    public int KingAttacks;

    public EvaluationContext(Span<BitBoard> attacks, Span<BitBoard> attacksBySide)
    {
        Attacks = attacks;
        AttacksBySide = attacksBySide;

        Attacks.Clear();
        AttacksBySide.Clear();

        KingAttacks = 0;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
