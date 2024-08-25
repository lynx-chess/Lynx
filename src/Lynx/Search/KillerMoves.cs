using StructureOfArraysGenerator;

namespace Lynx;

public struct KillerMove
{
    public int Zero;
    public int One;
    public int Two;
}

[MultiArray(typeof(KillerMove))]
public readonly partial struct KillerMoves
{
}