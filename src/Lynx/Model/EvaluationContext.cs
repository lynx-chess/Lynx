using Microsoft.Extensions.ObjectPool;

namespace Lynx.Model;

public class EvaluationContext : IResettable
{
    public BitBoard[] Attacks { get; }
    public BitBoard[] AttacksBySide { get; }

    public int WhiteKingRingAttacks { get; private set; }
    public int BlackKingRingAttacks { get; private set; }

    public EvaluationContext()
    {
        Attacks = new BitBoard[12];
        AttacksBySide = new BitBoard[2];
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

    public bool TryReset()
    {
        WhiteKingRingAttacks = 0;
        BlackKingRingAttacks = 0;

        Array.Clear(Attacks);
        Array.Clear(AttacksBySide);

        return true;
    }
}
