using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

[InlineArray(Constants.TranspositionTableElementsPerBucket)]
public struct TranspositionTableBucket
{
#pragma warning disable S1144, RCS1213 // Unused private types or members should be removed
    private TranspositionTableElement _ttEntry;
#pragma warning restore S1144 // Unused private types or members should be removed

    /// <summary>
    /// Struct size in bytes
    /// </summary>
    public static ulong Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ulong)Marshal.SizeOf<TranspositionTableBucket>();
    }
}
