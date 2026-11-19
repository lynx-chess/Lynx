using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

/// <summary>
/// Pinned arrays exposed as pointers, for tables that live throughout the whole process
/// </summary>
internal static unsafe class PinnedArrays
{
    private static readonly List<Array> _keepAlive = [];

    private static readonly Lock _keepAliveLock = new();

    public static T* CopyToPinnedArray<T>(T[] source)
        where T : unmanaged
    {
        var pinned = GC.AllocateUninitializedArray<T>(source.Length, pinned: true);
        source.CopyTo(pinned, 0);

        return KeepAlive(pinned);
    }

    /// <summary>
    /// Flattens a jagged array into a pinned array [<paramref name="source"/>.Length x <paramref name="stride"/>], with index: (i x stride) + j
    /// </summary>
    public static T* FlattenToPinnedArray<T>(T[][] source, int stride)
        where T : unmanaged
    {
        var pinned = GC.AllocateArray<T>(source.Length * stride, pinned: true);

        for (int i = 0; i < source.Length; ++i)
        {
            if (source[i].Length > stride)
            {
                throw new LynxException($"A larger {nameof(stride)} is needed, since inner array length ({source[i].Length}) is greater than the specified one ({stride})");
            }

            source[i].CopyTo(pinned, i * stride);
        }

        return KeepAlive(pinned);
    }

    /// <summary>
    /// Allocates a zeroed pinned array, to be filled using <paramref name="array"/> so that the writes are bounds-checked
    /// </summary>
    public static T* AllocatePinnedArray<T>(int length, out T[] array)
        where T : unmanaged
    {
        array = GC.AllocateArray<T>(length, pinned: true);

        return KeepAlive(array);
    }

    /// <summary>
    /// Keeps alive a pre-allocated array.
    /// Prefer <see cref="CopyToPinnedArray{T}(T[])"/> unless pointers into <paramref name="pinned"/> already exist
    /// </summary>
    public static T* KeepAlive<T>(T[] pinned)
        where T : unmanaged
    {
        lock (_keepAliveLock)
        {
            _keepAlive.Add(pinned);
        }

        return (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(pinned));
    }
}
