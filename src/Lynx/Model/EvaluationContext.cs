using Singulink.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[NonCopyable]
[StructLayout(LayoutKind.Sequential)]
public unsafe ref struct EvaluationContext
{
    private const int AttacksCount = 12;
    private const int AttacksBySideCount = 2;

    public const int RequiredBufferSize = AttacksCount + AttacksBySideCount;

    public readonly Bitboard* Attacks;
    public readonly Bitboard* AttacksBySide;

    public int WhiteKingRingAttacks;
    public int BlackKingRingAttacks;

    public EvaluationContext(Span<Bitboard> buffer)
    {
        Debug.Assert(buffer.Length == RequiredBufferSize);

        buffer.Clear();

        // buffer comes from the stack, so it's guaranteed not to be moved
        Attacks = (Bitboard*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(buffer));
        AttacksBySide = Attacks + AttacksCount;
    }

    public void Reset()
    {
        new Span<Bitboard>(Attacks, RequiredBufferSize).Clear();

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
