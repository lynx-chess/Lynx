using Singulink.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Lynx.Model;

[NonCopyable]
[StructLayout(LayoutKind.Sequential)]
public struct PlyStackEntry
{
    public int StaticEval { get; set; }

    public int DoubleExtensions { get; set; }

    public Move Move { get; set; }

    public PlyStackEntry()
    {
        Reset();
    }

    public void Reset()
    {
        StaticEval = int.MaxValue;
        DoubleExtensions = 0;
        Move = 0;
    }
}
