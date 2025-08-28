using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Model;

/// <summary>
/// A pointer to this (TranspositionTableBucket*) can be casted to a TranspositionTableElement* and indexed from 0 to <see cref="Constants.TranspositionTableElementsPerBucket"/> - 1 to access each entry.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 30)]
public struct TranspositionTableBucket
{
#pragma warning disable S1144, RCS1213 // Unused private types or members should be removed
    [FieldOffset(0)]
    private TranspositionTableElement _ttEntry0;

    [FieldOffset(10)]
    private TranspositionTableElement _ttEntry1;

    [FieldOffset(20)]
    private TranspositionTableElement _ttEntry2;
#pragma warning restore S1144 // Unused private types or members should be removed

    // TODO Add byte padding to align with 32 or 64 bytes

    /// <summary>
    /// Struct size in bytes
    /// </summary>
    public static ulong Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ulong)Marshal.SizeOf<TranspositionTableBucket>();
    }
}
