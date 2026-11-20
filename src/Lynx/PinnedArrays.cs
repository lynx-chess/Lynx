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
        var pinned = new PinnedArray<T>(source.Length);
        source.CopyTo(pinned.Array, 0);

        return KeepAlive(pinned);
    }

    /// <summary>
    /// Flattens a jagged array into a pinned array [<paramref name="source"/>.Length x <paramref name="stride"/>], with index: (i x stride) + j
    /// </summary>
    public static T* FlattenToPinnedArray<T>(T[][] source, int stride)
        where T : unmanaged
    {
        var pinned = new PinnedArray<T>(source.Length * stride);

        for (int i = 0; i < source.Length; ++i)
        {
            if (source[i].Length > stride)
            {
                throw new LynxException($"A larger {nameof(stride)} is needed, since inner array length ({source[i].Length}) is greater than the specified one ({stride})");
            }

            source[i].CopyTo(pinned.Array, i * stride);
        }

        return KeepAlive(pinned);
    }

    /// <summary>
    /// Allocates a zeroed pinned array, to be filled using <paramref name="array"/> so that the writes are bounds-checked
    /// </summary>
    public static T* AllocatePinnedArray<T>(int length, out T[] array)
        where T : unmanaged
    {
        var pinned = new PinnedArray<T>(length);
        array = pinned.Array;

        return KeepAlive(pinned);
    }

    /// <summary>
    /// Keeps alive an already allocated <see cref="PinnedArray{T}"/> without copying it.
    /// To be used instead of <see cref="CopyToPinnedArray{T}(T[])"/> when pointers into it already exist
    /// </summary>
    public static T* KeepAlive<T>(PinnedArray<T> pinned)
        where T : unmanaged
    {
        if (pinned.Array is null)
        {
            throw new ArgumentException("Uninitialized (default) PinnedArray", nameof(pinned));
        }

        lock (_keepAliveLock)
        {
            _keepAlive.Add(pinned.Array);
        }

        return pinned.Pointer;
    }
}
