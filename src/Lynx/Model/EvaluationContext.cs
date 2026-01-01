using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public ref struct EvaluationContext
{
    private const int AttacksCount = 12;
    private const int AttacksBySideCount = 2;

    public const int RequiredBufferSize = AttacksCount + AttacksBySideCount;

    public Span<BitBoard> Attacks;
    public Span<BitBoard> AttacksBySide;

    public int WhiteKingRingAttacks;
    public int BlackKingRingAttacks;

    public int WhiteKingRingWeigthedAttacks;
    public int BlackKingRingWeigthedAttacks;

    public EvaluationContext(Span<BitBoard> buffer)
    {
        Debug.Assert(buffer.Length == RequiredBufferSize);

        buffer.Clear();

        Attacks = buffer[..AttacksCount];
        AttacksBySide = buffer.Slice(AttacksCount, AttacksBySideCount);
    }

    public void Reset()
    {
        Attacks.Clear();
        AttacksBySide.Clear();

        WhiteKingRingAttacks = 0;
        BlackKingRingAttacks = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IncreaseKingRingAttacks(int side, int attacks, int weightedAttacks)
    {
        if (side == (int)Side.White)
        {
            WhiteKingRingAttacks += attacks;
            WhiteKingRingWeigthedAttacks += weightedAttacks;
        }
        else
        {
            BlackKingRingAttacks += attacks;
            BlackKingRingWeigthedAttacks += weightedAttacks;
        }
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
