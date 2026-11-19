using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

internal sealed unsafe class PinnedArray<T>
    where T : unmanaged
{
    public T[] Array { get; }

    public T* Pointer { get; }

    public PinnedArray(int length)
    {
        Array = GC.AllocateArray<T>(length, pinned: true);
        Pointer = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(Array));
    }
}
