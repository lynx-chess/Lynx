/*
 * 
 *  |      Method |     Mean |    Error |   StdDev | Ratio | Allocated |
 *  |------------ |---------:|---------:|---------:|------:|----------:|
 *  |         Sum | 66.41 us | 0.277 us | 0.245 us |  1.00 |         - |
 *  | ArrayAccess | 57.53 us | 0.546 us | 0.456 us |  0.87 |         - |
 *  
 *  |      Method |     Mean |    Error |   StdDev | Ratio | Allocated |
 *  |------------ |---------:|---------:|---------:|------:|----------:|
 *  |         Sum | 66.49 us | 0.235 us | 0.219 us |  1.00 |         - |
 *  | ArrayAccess | 58.05 us | 0.470 us | 0.392 us |  0.87 |         - |
 * 
 * 
 *  C.Sum()
 *      L0000: push ebp
 *      L0001: mov ebp, esp
 *      L0003: xor eax, eax
 *      L0005: xor edx, edx
 *      L0007: mov ecx, edx
 *      L0009: neg ecx
 *      L000b: lea eax, [ecx+eax+0x504]
 *      L0012: inc edx
 *      L0013: cmp edx, 0x32
 *      L0016: jl short L0007
 *      L0018: pop ebp
 *      L0019: ret
 *  
 *  C.ArrayAccess()
 *      L0000: push ebp
 *      L0001: mov ebp, esp
 *      L0003: push edi
 *      L0004: push esi
 *      L0005: xor esi, esi
 *      L0007: xor edi, edi
 *      L0009: mov ecx, 0x13bdc5e8
 *      L000e: xor edx, edx
 *      L0010: call 0x72430560
 *      L0015: mov eax, [eax]
 *      L0017: mov edx, eax
 *      L0019: cmp edi, [edx+4]
 *      L001c: jae short L002e
 *      L001e: add esi, [edx+edi*4+8]
 *      L0022: inc edi
 *      L0023: cmp edi, 0x32
 *      L0026: jl short L0017
 *      L0028: mov eax, esi
 *      L002a: pop esi
 *      L002b: pop edi
 *      L002c: pop ebp
 *      L002d: ret
 *      L002e: call 0x72431100
 *      L0033: int3
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark
{
    public class PVTable_SumVsArrayAccess : BaseBenchmark
    {
        private const int MaxDepth = 64;

        public PVTable_SumVsArrayAccess()
        {
            _ = PVTable.Indexes[0];
        }

        [Benchmark(Baseline = true)]
        public int Sum()
        {
            var total = 0;

            for (int i = 0; i < 1000; ++i)
            {
                for (int depth = 0; depth < MaxDepth; ++depth)
                {
                    total += 1234 + MaxDepth - depth;
                }
            }

            return total;
        }

        [Benchmark]
        public int ArrayAccess()
        {
            var total = 0;

            for (int i = 0; i < 1000; ++i)
            {
                for (int depth = 0; depth < MaxDepth; ++depth)
                {
                    total += PVTable.Indexes[depth];
                }
            }

            return total;
        }

        private static class PVTable
        {
            public static readonly int[] Indexes;

            static PVTable()
            {
                Indexes = new int[MaxDepth];
                int previousPVIndex = 0;
                Indexes[0] = previousPVIndex;

                for (int depth = 0; depth < MaxDepth - 1; ++depth)
                {
                    Indexes[depth + 1] = previousPVIndex + MaxDepth - depth;
                    previousPVIndex = Indexes[depth + 1];
                }
            }
        }
    }
}
