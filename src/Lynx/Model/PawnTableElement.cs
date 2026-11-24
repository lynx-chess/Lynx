using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[StructLayout(LayoutKind.Sequential)]
public struct PawnTableElement
{
    public uint Key;

    public int PackedScore;

    /// <summary>
    /// Upper 32 bits of the king-pawn hash.
    /// Table index uses 18 lower bits, as per <see cref="Constants.KingPawnHashMask"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint CalculateKey(ulong kingPawnHash) => (uint)(kingPawnHash >> 32);

    public void Update(uint key, int packedScore)
    {
        Key = key;
        PackedScore = packedScore;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
