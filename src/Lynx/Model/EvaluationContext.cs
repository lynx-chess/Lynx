using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public ref struct EvaluationContext
{
    private const int AttacksCount = 12;
    private const int AttacksBySideCount = 2;
    private const int KingRingAttacksCount = 2;

    public const int RequiredBufferSize = AttacksCount + AttacksBySideCount + KingRingAttacksCount;

    public Span<BitBoard> Attacks;
    public Span<BitBoard> AttacksBySide;
    public Span<BitBoard> KingRingAttacks;

    public EvaluationContext(Span<BitBoard> buffer)
    {
        Debug.Assert(buffer.Length == RequiredBufferSize);

        buffer.Clear();

        Attacks = buffer[..AttacksCount];
        AttacksBySide = buffer.Slice(AttacksCount, AttacksBySideCount);
        KingRingAttacks = buffer.Slice(AttacksCount + AttacksBySideCount, KingRingAttacksCount);
    }

    public readonly void Reset()
    {
        Attacks.Clear();
        AttacksBySide.Clear();
        KingRingAttacks.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IncreaseKingRingAttacks(int side, int count)
    {
        KingRingAttacks[side] += (ulong)count;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
