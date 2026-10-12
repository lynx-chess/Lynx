using System.Runtime.InteropServices;

namespace Lynx.Model;

[StructLayout(LayoutKind.Sequential)]
public struct PlyStackEntry
{
    public const int StaticEvalDefaultValue = int.MaxValue;

    public int StaticEval { get; set; }

    public int DoubleExtensions { get; set; }

    public Move Move { get; set; }

    public PlyStackEntry()
    {
        Reset();
    }

    public void Reset()
    {
        StaticEval = StaticEvalDefaultValue;
        DoubleExtensions = 0;
        Move = 0;
    }
}
