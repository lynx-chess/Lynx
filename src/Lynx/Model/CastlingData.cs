using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct CastlingData
{
    public const int DefaultValues = -1;

    public readonly int WhiteKingsideRook;
    public readonly int WhiteQueensideRook;
    public readonly int BlackKingsideRook;
    public readonly int BlackQueensideRook;

    public CastlingData(
        int whiteKingsideRook,
        int whiteQueensideRook,
        int blackKingsideRook,
        int blackQueensideRook)
    {
        WhiteKingsideRook = whiteKingsideRook;
        WhiteQueensideRook = whiteQueensideRook;
        BlackKingsideRook = blackKingsideRook;
        BlackQueensideRook = blackQueensideRook;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
