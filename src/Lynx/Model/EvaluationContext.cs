using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct EvaluationContext
{
    private const int AttacksCount = 12;
    private const int AttacksBySideCount = 2;
    private const int KingRingAttacksCount = 2;

    private const int KingRingAttacksOffset = AttacksCount + KingRingAttacksCount;

    public const int RequiredBufferSize = AttacksCount + AttacksBySideCount + KingRingAttacksCount;

    public readonly Span<Bitboard> Attacks;
    public readonly Span<Bitboard> AttacksBySide;
    public readonly Span<ulong> KingRingAttacks;

    public EvaluationContext(Span<Bitboard> buffer)
    {
        Debug.Assert(buffer.Length == RequiredBufferSize);

        buffer.Clear();

        Attacks = buffer[..AttacksCount];
        AttacksBySide = buffer.Slice(AttacksCount, AttacksBySideCount);
        KingRingAttacks = buffer.Slice(KingRingAttacksOffset, KingRingAttacksCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void IncreaseKingRingAttacks(int side, int count) => KingRingAttacks[side] += (ulong)count;
}

#pragma warning restore CA1051 // Do not declare visible instance fields
