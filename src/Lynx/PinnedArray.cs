using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

internal readonly unsafe struct PinnedArray<T>
    where T : unmanaged
{

#pragma warning disable CA1051 // Do not declare visible instance fields

    public readonly T[] Array;

    public readonly T* Pointer;

    public readonly int Length;

#pragma warning restore CA1051 // Do not declare visible instance fields

    public PinnedArray(int length)
    {
        Array = GC.AllocateArray<T>(length, pinned: true);
        Pointer = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(Array));
        Length = length;
    }

    public void Clear() => System.Array.Clear(Array);
}
