using System.Runtime.CompilerServices;

namespace Hezium.Memory;

public sealed partial class BigArray<T>
{
    private const int MaxChunkByteLength = 65535;

    // Exhaustive for every value produced by 65535 / sizeof(T). Keep each
    // allocation behind a static lambda so unselected chunk-array types stay lazy.
    // Partition into groups to allow inlining and constant folding of the length checks.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Func<int, bool, bool, Array> CreateBigArrayAllocator(int chunkLength)
    {
        if (chunkLength <= 257)
        {
            return CreateBigArrayAllocator1To257<T>(chunkLength);
        }
        else
        {
            return CreateBigArrayAllocator258To65535<T>(chunkLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator1To257<TElement>(int chunkLength)
        {
            if (chunkLength <= 16)
            {
                return CreateBigArrayAllocator1To16<TElement>(chunkLength);
            }
            else if (chunkLength <= 32)
            {
                return CreateBigArrayAllocator17To32<TElement>(chunkLength);
            }
            else if (chunkLength <= 48)
            {
                return CreateBigArrayAllocator33To48<TElement>(chunkLength);
            }
            else if (chunkLength <= 64)
            {
                return CreateBigArrayAllocator49To64<TElement>(chunkLength);
            }
            else if (chunkLength <= 80)
            {
                return CreateBigArrayAllocator65To80<TElement>(chunkLength);
            }
            else if (chunkLength <= 96)
            {
                return CreateBigArrayAllocator81To96<TElement>(chunkLength);
            }
            else if (chunkLength <= 112)
            {
                return CreateBigArrayAllocator97To112<TElement>(chunkLength);
            }
            else if (chunkLength <= 128)
            {
                return CreateBigArrayAllocator113To128<TElement>(chunkLength);
            }
            else if (chunkLength <= 144)
            {
                return CreateBigArrayAllocator129To144<TElement>(chunkLength);
            }
            else if (chunkLength <= 160)
            {
                return CreateBigArrayAllocator145To160<TElement>(chunkLength);
            }
            else if (chunkLength <= 176)
            {
                return CreateBigArrayAllocator161To176<TElement>(chunkLength);
            }
            else if (chunkLength <= 192)
            {
                return CreateBigArrayAllocator177To192<TElement>(chunkLength);
            }
            else if (chunkLength <= 208)
            {
                return CreateBigArrayAllocator193To208<TElement>(chunkLength);
            }
            else if (chunkLength <= 224)
            {
                return CreateBigArrayAllocator209To224<TElement>(chunkLength);
            }
            else if (chunkLength <= 240)
            {
                return CreateBigArrayAllocator225To240<TElement>(chunkLength);
            }
            else
            {
                return CreateBigArrayAllocator241To257<TElement>(chunkLength);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator258To65535<TElement>(int chunkLength)
        {
            if (chunkLength <= 274)
            {
                return CreateBigArrayAllocator258To274<TElement>(chunkLength);
            }
            else if (chunkLength <= 293)
            {
                return CreateBigArrayAllocator275To293<TElement>(chunkLength);
            }
            else if (chunkLength <= 316)
            {
                return CreateBigArrayAllocator295To316<TElement>(chunkLength);
            }
            else if (chunkLength <= 343)
            {
                return CreateBigArrayAllocator318To343<TElement>(chunkLength);
            }
            else if (chunkLength <= 374)
            {
                return CreateBigArrayAllocator344To374<TElement>(chunkLength);
            }
            else if (chunkLength <= 412)
            {
                return CreateBigArrayAllocator376To412<TElement>(chunkLength);
            }
            else if (chunkLength <= 458)
            {
                return CreateBigArrayAllocator414To458<TElement>(chunkLength);
            }
            else if (chunkLength <= 516)
            {
                return CreateBigArrayAllocator461To516<TElement>(chunkLength);
            }
            else if (chunkLength <= 590)
            {
                return CreateBigArrayAllocator520To590<TElement>(chunkLength);
            }
            else if (chunkLength <= 689)
            {
                return CreateBigArrayAllocator595To689<TElement>(chunkLength);
            }
            else if (chunkLength <= 829)
            {
                return CreateBigArrayAllocator697To829<TElement>(chunkLength);
            }
            else if (chunkLength <= 1040)
            {
                return CreateBigArrayAllocator840To1040<TElement>(chunkLength);
            }
            else if (chunkLength <= 1394)
            {
                return CreateBigArrayAllocator1057To1394<TElement>(chunkLength);
            }
            else if (chunkLength <= 2114)
            {
                return CreateBigArrayAllocator1424To2114<TElement>(chunkLength);
            }
            else if (chunkLength <= 4369)
            {
                return CreateBigArrayAllocator2184To4369<TElement>(chunkLength);
            }
            else
            {
                return CreateBigArrayAllocator4681To65535<TElement>(chunkLength);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator1To16<TElement>(int chunkLength)
        {
            if (chunkLength == 1)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk1<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 3)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 4)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 5)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 6)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 7)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 8)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 9)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 10)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 11)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 12)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 13)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 14)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 15)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<TElement>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<TElement>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator17To32<TElement>(int chunkLength)
        {
            if (chunkLength == 17)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk17<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 18)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 19)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk19<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 20)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk5<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 21)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk7<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 22)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk11<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 23)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk23<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 24)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 25)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk5<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 26)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk13<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 27)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 28)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 29)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk29<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 30)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk5<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 31)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk31<TElement>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<TElement>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator33To48<TElement>(int chunkLength)
        {
            if (chunkLength == 33)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk11<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 34)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk17<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 35)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk7<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 36)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 37)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk37<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 38)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk19<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 39)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk13<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 40)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 41)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk41<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 42)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 43)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk43<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 44)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 45)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk5<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 46)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk23<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 47)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk47<TElement>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<TElement>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator49To64<TElement>(int chunkLength)
        {
            if (chunkLength == 49)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk7<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 50)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk5<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 51)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk17<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 52)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 53)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk53<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 54)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 55)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk11<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 56)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 57)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk19<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 58)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk29<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 59)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk59<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 60)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 61)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk61<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 62)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk31<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 63)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator65To80<TElement>(int chunkLength)
        {
            if (chunkLength == 65)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk13<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 66)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 67)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk67<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 68)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 69)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk23<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 70)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 71)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk71<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 72)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 73)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk73<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 74)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk37<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 75)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk5<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 76)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk19<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 77)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk11<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 78)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 79)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk79<TElement>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<TElement>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator81To96<TElement>(int chunkLength)
        {
            if (chunkLength == 81)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 82)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk41<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 83)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk83<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 84)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 85)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk17<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 86)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk43<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 87)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk29<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 88)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk11<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 89)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk89<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 90)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 91)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk13<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 92)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk23<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 93)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk31<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 94)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk47<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 95)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk19<TElement>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator97To112<TElement>(int chunkLength)
        {
            if (chunkLength == 97)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk97<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 98)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 99)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 100)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk5<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 101)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk101<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 102)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 103)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk103<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 104)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 105)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 106)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk53<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 107)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk107<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 108)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 109)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk109<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 110)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 111)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk37<TElement>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk7<TElement>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator113To128<TElement>(int chunkLength)
        {
            if (chunkLength == 113)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk113<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 114)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk19<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 115)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk23<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 116)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk29<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 117)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 118)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk59<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 119)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk17<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 120)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 121)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk11<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 122)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk61<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 123)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk41<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 124)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk31<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 125)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk5<ElementChunk5<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 126)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 127)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk127<TElement>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator129To144<TElement>(int chunkLength)
        {
            if (chunkLength == 129)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk43<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 130)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 131)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk131<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 132)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk11<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 133)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk19<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 134)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk67<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 135)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk5<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 136)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk17<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 137)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk137<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 138)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk23<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 139)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk139<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 140)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 141)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk47<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 142)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk71<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 143)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk13<TElement>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator145To160<TElement>(int chunkLength)
        {
            if (chunkLength == 145)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk29<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 146)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk73<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 147)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk7<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 148)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk37<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 149)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk149<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 150)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk5<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 151)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk151<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 152)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk19<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 153)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 154)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 155)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk31<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 156)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 157)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk157<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 158)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk79<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 159)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk53<TElement>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator161To176<TElement>(int chunkLength)
        {
            if (chunkLength == 161)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk23<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 162)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 163)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk163<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 164)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk41<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 165)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 166)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk83<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 167)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk167<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 168)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk7<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 169)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<ElementChunk13<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 170)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 171)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk19<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 172)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk43<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 173)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk173<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 174)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk29<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 175)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk5<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk11<TElement>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator177To192<TElement>(int chunkLength)
        {
            if (chunkLength == 177)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk59<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 178)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk89<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 179)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk179<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 180)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 181)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk181<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 182)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 183)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk61<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 184)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk23<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 185)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk37<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 186)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk31<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 187)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk17<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 188)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk47<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 189)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 190)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk19<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 191)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk191<TElement>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator193To208<TElement>(int chunkLength)
        {
            if (chunkLength == 193)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk193<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 194)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk97<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 195)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 196)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk7<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 197)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk197<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 198)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk11<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 199)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk199<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 200)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk5<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 201)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk67<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 202)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk101<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 203)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk29<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 204)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk17<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 205)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk41<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 206)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk103<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 207)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk23<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator209To224<TElement>(int chunkLength)
        {
            if (chunkLength == 209)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk19<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 210)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 211)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk211<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 212)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk53<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 213)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk71<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 214)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk107<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 215)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk43<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 216)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 217)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk31<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 218)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk109<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 219)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk73<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 220)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk11<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 221)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<ElementChunk17<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 222)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk37<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 223)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk223<TElement>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk7<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator225To240<TElement>(int chunkLength)
        {
            if (chunkLength == 225)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk5<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 226)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk113<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 227)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk227<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 228)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk19<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 229)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk229<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 230)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk23<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 231)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk7<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 232)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk29<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 233)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk233<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 234)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 235)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk47<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 236)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk59<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 237)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk79<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 238)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 239)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk239<TElement>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator241To257<TElement>(int chunkLength)
        {
            if (chunkLength == 241)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk241<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 242)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk11<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 243)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 244)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk61<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 245)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk7<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 246)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk41<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 247)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<ElementChunk19<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 248)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk31<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 249)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk83<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 250)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk5<ElementChunk5<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 251)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk251<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 252)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk7<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 253)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk23<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 254)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk127<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 255)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk257<TElement>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator258To274<TElement>(int chunkLength)
        {
            if (chunkLength == 258)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk43<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 259)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk37<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 260)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 261)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk29<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 262)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk131<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 263)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk263<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 264)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk11<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 265)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk53<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 266)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk19<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 267)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk89<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 268)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk67<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 269)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk269<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 270)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk5<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 271)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk271<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 273)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk7<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk137<TElement>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator275To293<TElement>(int chunkLength)
        {
            if (chunkLength == 275)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk5<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 276)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk23<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 277)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk277<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 278)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk139<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 280)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk7<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 281)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk281<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 282)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk47<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 283)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk283<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 284)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk71<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 286)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk11<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 287)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk41<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 288)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 289)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk17<ElementChunk17<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 291)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk97<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 292)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk73<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk293<TElement>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator295To316<TElement>(int chunkLength)
        {
            if (chunkLength == 295)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk59<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 296)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk37<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 297)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk11<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 299)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<ElementChunk23<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 300)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk5<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 302)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk151<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 303)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk101<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 304)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk19<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 306)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk17<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 307)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk307<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 309)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk103<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 310)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk31<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 312)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 313)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk313<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 315)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk79<TElement>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator318To343<TElement>(int chunkLength)
        {
            if (chunkLength == 318)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk53<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 319)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk29<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 321)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk107<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 322)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk23<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 324)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 326)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk163<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 327)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk109<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 329)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk47<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 330)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk11<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 332)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk83<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 334)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk167<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 336)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk7<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 337)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk337<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 339)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk113<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 341)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk31<TElement>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk7<ElementChunk7<TElement>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator344To374<TElement>(int chunkLength)
        {
            if (chunkLength == 344)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk43<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 346)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk173<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 348)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk29<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 350)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk5<ElementChunk7<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 352)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk11<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 354)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk59<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 356)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk89<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 358)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk179<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 360)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 362)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk181<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 364)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk7<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 366)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk61<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 368)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk23<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 370)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk37<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 372)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk31<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk11<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator376To412<TElement>(int chunkLength)
        {
            if (chunkLength == 376)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk47<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 378)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk7<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 381)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk127<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 383)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk383<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 385)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk7<ElementChunk11<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 387)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk43<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 390)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 392)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk7<ElementChunk7<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 394)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk197<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 397)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk397<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 399)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk7<ElementChunk19<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 402)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk67<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 404)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk101<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 407)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk37<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 409)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk409<TElement>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk103<TElement>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator414To458<TElement>(int chunkLength)
        {
            if (chunkLength == 414)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk23<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 417)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk139<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 420)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 422)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk211<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 425)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk5<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 428)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk107<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 431)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk431<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 434)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk31<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 436)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk109<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 439)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk439<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 442)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk13<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 445)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk89<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 448)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk7<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 451)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk11<ElementChunk41<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 455)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk7<ElementChunk13<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk229<TElement>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator461To516<TElement>(int chunkLength)
        {
            if (chunkLength == 461)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk461<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 464)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk29<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 468)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 471)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk157<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 474)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk79<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 478)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk239<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 481)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<ElementChunk37<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 485)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk97<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 489)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk163<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 492)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk41<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 496)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk31<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 500)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk5<ElementChunk5<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 504)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk7<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 508)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk127<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 511)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk73<TElement>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk43<TElement>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator520To590<TElement>(int chunkLength)
        {
            if (chunkLength == 520)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 524)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk131<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 528)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk11<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 532)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk7<ElementChunk19<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 537)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk179<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 541)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk541<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 546)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk7<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 550)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk5<ElementChunk11<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 555)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk37<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 560)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk7<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 564)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk47<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 569)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk569<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 574)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk41<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 579)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk193<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 585)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk59<TElement>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator595To689<TElement>(int chunkLength)
        {
            if (chunkLength == 595)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk7<ElementChunk17<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 601)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk601<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 606)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk101<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 612)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk17<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 618)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk103<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 624)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 630)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 636)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk53<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 642)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk107<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 648)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk3<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 655)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk131<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 661)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk661<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 668)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk167<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 675)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk5<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 682)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk11<ElementChunk31<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<ElementChunk53<TElement>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator697To829<TElement>(int chunkLength)
        {
            if (chunkLength == 697)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk17<ElementChunk41<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 704)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk11<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 712)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk89<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 720)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 728)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk7<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 736)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk23<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 744)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk31<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 753)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk251<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 762)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk127<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 771)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk257<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 780)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 789)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk263<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 799)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk17<ElementChunk47<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 809)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk809<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 819)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk7<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk829<TElement>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator840To1040<TElement>(int chunkLength)
        {
            if (chunkLength == 840)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 851)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk23<ElementChunk37<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 862)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk431<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 873)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk97<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 885)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk59<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 897)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk13<ElementChunk23<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 910)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk7<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 923)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<ElementChunk71<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 936)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 949)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk13<ElementChunk73<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 963)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk107<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 978)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk163<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 992)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk31<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1008)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk7<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1023)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk11<ElementChunk31<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator1057To1394<TElement>(int chunkLength)
        {
            if (chunkLength == 1057)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk151<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1074)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk179<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1092)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk7<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1110)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk37<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1129)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk1129<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1149)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk383<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1170)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1191)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk397<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1213)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk1213<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1236)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk103<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1260)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1285)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk257<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1310)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk5<ElementChunk131<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1337)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk191<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1365)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk7<ElementChunk13<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk17<ElementChunk41<TElement>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator1424To2114<TElement>(int chunkLength)
        {
            if (chunkLength == 1424)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk89<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1456)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk7<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1489)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk1489<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1524)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk127<TElement>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1560)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1598)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk17<ElementChunk47<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1638)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk7<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1680)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1724)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk431<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1771)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk11<ElementChunk23<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1820)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk7<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1872)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk13<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1927)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk41<ElementChunk47<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 1985)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk397<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2047)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk23<ElementChunk89<TElement>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk7<ElementChunk151<TElement>>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator2184To4369<TElement>(int chunkLength)
        {
            if (chunkLength == 2184)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk7<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2259)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk251<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2340)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2427)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk809<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2520)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk7<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2621)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2621<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2730)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk7<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2849)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk11<ElementChunk37<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 2978)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk1489<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 3120)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk5<ElementChunk13<TElement>>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 3276)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk3<ElementChunk3<ElementChunk7<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 3449)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3449<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 3640)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk2<ElementChunk2<ElementChunk5<ElementChunk7<ElementChunk13<TElement>>>>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 3855)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk257<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 4095)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk5<ElementChunk7<ElementChunk13<TElement>>>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk17<ElementChunk257<TElement>>>(chunks, pinned, uninitialized);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<int, bool, bool, Array> CreateBigArrayAllocator4681To65535<TElement>(int chunkLength)
        {
            if (chunkLength == 4681)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk31<ElementChunk151<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 5041)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk71<ElementChunk71<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 5461)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk43<ElementChunk127<TElement>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 5957)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk23<ElementChunk37<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 6553)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk6553<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 7281)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk3<ElementChunk809<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 8191)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk8191<TElement>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 9362)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk31<ElementChunk151<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 10922)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk2<ElementChunk43<ElementChunk127<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 13107)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk17<ElementChunk257<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 16383)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk43<ElementChunk127<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 21845)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk5<ElementChunk17<ElementChunk257<TElement>>>>(chunks, pinned, uninitialized);
            }
            else if (chunkLength == 32767)
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk7<ElementChunk31<ElementChunk151<TElement>>>>(chunks, pinned, uninitialized);
            }
            else
            {
                return static (chunks, pinned, uninitialized) => AllocateArray<ElementChunk3<ElementChunk5<ElementChunk17<ElementChunk257<TElement>>>>>(chunks, pinned, uninitialized);
            }
        }
    }

    // Must not be inlined to avoid invalid type combinations that would cause a TypeLoadException when the method is JITted.
    // Invalid type combinations won't be used because the method is only called with valid type combinations.
    // The return type should not be changed to TElement[] for the same reason.
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static Array AllocateArray<TElement>(int chunks, bool pinned, bool uninitialized)
    {
        return uninitialized
            ? GC.AllocateUninitializedArray<TElement>(chunks, pinned)
            : GC.AllocateArray<TElement>(chunks, pinned);
    }

    private static int GetChunkLength()
    {
        return MaxChunkByteLength / Unsafe.SizeOf<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Array CreateBigArraySlow(nint length, bool pinned = false, bool uninitialized = false)
    {
        int chunkLength = GetChunkLength();
        int chunks = (int)((length / chunkLength) + (length % chunkLength == 0 ? 0 : 1));

        return CreateBigArrayAllocator(chunkLength)(chunks, pinned, uninitialized);
    }
}
