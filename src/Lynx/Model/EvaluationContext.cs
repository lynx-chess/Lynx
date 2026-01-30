using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[StructLayout(LayoutKind.Sequential)]
public ref struct EvaluationContext
{
    private const int AttacksCount = 12;
    private const int AttacksBySideCount = 2;

    public const int RequiredBufferSize = AttacksCount + AttacksBySideCount;

    public Span<BitBoard> Attacks;
    public Span<BitBoard> AttacksBySide;

    public int WhiteKingRingAttacks;
    public int BlackKingRingAttacks;

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
