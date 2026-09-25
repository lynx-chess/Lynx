using System.Runtime.InteropServices;

namespace Lynx.Model;

[StructLayout(LayoutKind.Sequential)]
public struct PlyStackEntry
{
    public int StaticEval { get; set; }

    public int DoubleExtensions { get; set; }

    public int Piece { get; set; }

    public Move Move { get; set; }

    public PlyStackEntry()
    {
        Reset();
    }

    public void Reset()
    {
        StaticEval = int.MaxValue;
        DoubleExtensions = 0;
        Piece = 0;
        Move = 0;
    }
}
