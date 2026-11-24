using Lynx.Model;
using NUnit.Framework;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Test;

public class AlignedPinnedArrayTest
{
    private struct Element24
    {
#pragma warning disable CS0649, S1144, RCS1213 // Unused fields - only their size matters
        public long A, B, C;
#pragma warning restore CS0649, S1144, RCS1213
    }

    [TestCase(1)]
    [TestCase(7)]
    [TestCase(1_000)]
    [TestCase(Constants.KingPawnHashSize)]
    public void PointerIsCacheLineAlignedAndWithinTheBackingArray(int length)
    {
        // Pinned allocations of varying sizes in between, so that the backing arrays start at different offsets
        var spacers = new List<byte[]>();

        for (int i = 0; i < 16; ++i)
        {
            spacers.Add(GC.AllocateArray<byte>(i, pinned: true));

            AssertAlignedAndWithinBackingArray(new AlignedPinnedArray<byte>(length));
            AssertAlignedAndWithinBackingArray(new AlignedPinnedArray<PawnTableElement>(length));
            AssertAlignedAndWithinBackingArray(new AlignedPinnedArray<Element24>(length));
        }

        GC.KeepAlive(spacers);
    }

    [Test]
    public unsafe void Clear()
    {
        var array = new AlignedPinnedArray<PawnTableElement>(100);
        for (int i = 0; i < array.Length; ++i)
        {
            array.Pointer[i].Update((ulong)i + 1, i + 1);
        }

        array.Clear();

        for (int i = 0; i < array.Length; ++i)
        {
            Assert.AreEqual(0UL, array.Pointer[i].Key);
            Assert.AreEqual(0, array.Pointer[i].PackedScore);
        }
    }

    private static unsafe void AssertAlignedAndWithinBackingArray<T>(AlignedPinnedArray<T> array)
        where T : unmanaged
    {
        Assert.AreEqual(0UL, (ulong)array.Pointer % 64);

        var backingArray = (T[])typeof(AlignedPinnedArray<T>)
            .GetField("_array", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(array)!;

        var backingStart = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(backingArray));

        Assert.True(array.Pointer >= backingStart);
        Assert.True(array.Pointer + array.Length <= backingStart + backingArray.Length);
    }
}
