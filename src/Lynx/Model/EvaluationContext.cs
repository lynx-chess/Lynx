namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public ref struct EvaluationContext
{
    public Span<BitBoard> Attacks;
    public Span<BitBoard> AttacksBySide;

    public int WhiteKingRingAttacks;
    public int BlackKingRingAttacks;

    public EvaluationContext(Span<BitBoard> attacks, Span<BitBoard> attacksBySide)
    {
        Attacks = attacks;
        AttacksBySide = attacksBySide;

        Attacks.Clear();
        AttacksBySide.Clear();
    }

    public void IncreaseKingRingAttacks(int side, int count)
    {
        if (side == (int)Side.White)
        {
            WhiteKingRingAttacks += count;
        }
        else
        {
            BlackKingRingAttacks += count;
        }
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
