using System.Runtime.InteropServices;

namespace Lynx.Model;

[StructLayout(LayoutKind.Sequential)]
public struct PlyStackEntry
{
    public int StaticEval { get; set; }

    public int DoubleExtensions { get; set; }

    public Move Move { get; set; }

    public Bitboard[] AttacksBySide { get; set; }

    public PlyStackEntry()
    {
        AttacksBySide = new Bitboard[2];
        Reset();
    }

    public void Reset()
    {
        StaticEval = int.MaxValue;
        DoubleExtensions = 0;
        Move = 0;

        AttacksBySide ??= new Bitboard[2];

        Array.Clear(AttacksBySide, 0, 2);
    }
}
