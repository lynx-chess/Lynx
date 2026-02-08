/* Not a big deal, I guess since the struct is small
 *
 *  |                        Method | iterations |          Mean |       Error |      StdDev | Ratio | RatioSD | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |------------------------------ |----------- |--------------:|------------:|------------:|------:|--------:|------:|------:|------:|----------:|
 *  |         MutableBitboardByType |          1 |     0.3573 ns |   0.0240 ns |   0.0200 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  |          MutableBitboardByRef |          1 |     0.3573 ns |   0.0132 ns |   0.0110 ns |  1.00 |    0.06 |     - |     - |     - |         - |
 *  | MutableReadonlyBitboardByType |          1 |     0.3757 ns |   0.0278 ns |   0.0260 ns |  1.05 |    0.09 |     - |     - |     - |         - |
 *  |  MutableReadonlyBitboardByRef |          1 |     0.3685 ns |   0.0193 ns |   0.0161 ns |  1.03 |    0.07 |     - |     - |     - |         - |
 *  |                               |            |               |             |             |       |         |       |       |       |           |
 *  |         MutableBitboardByType |         10 |     7.1571 ns |   0.0959 ns |   0.0897 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  |          MutableBitboardByRef |         10 |     7.1819 ns |   0.1261 ns |   0.0984 ns |  1.01 |    0.02 |     - |     - |     - |         - |
 *  | MutableReadonlyBitboardByType |         10 |     7.1242 ns |   0.1201 ns |   0.1065 ns |  1.00 |    0.02 |     - |     - |     - |         - |
 *  |  MutableReadonlyBitboardByRef |         10 |     7.1762 ns |   0.0747 ns |   0.0624 ns |  1.00 |    0.02 |     - |     - |     - |         - |
 *  |                               |            |               |             |             |       |         |       |       |       |           |
 *  |         MutableBitboardByType |       1000 |   701.9070 ns |  13.2791 ns |  12.4213 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  |          MutableBitboardByRef |       1000 |   703.2412 ns |   4.6742 ns |   3.9032 ns |  1.00 |    0.02 |     - |     - |     - |         - |
 *  | MutableReadonlyBitboardByType |       1000 |   693.5397 ns |   1.6796 ns |   1.3113 ns |  0.99 |    0.02 |     - |     - |     - |         - |
 *  |  MutableReadonlyBitboardByRef |       1000 |   709.5263 ns |  10.8891 ns |   9.6529 ns |  1.01 |    0.02 |     - |     - |     - |         - |
 *  |                               |            |               |             |             |       |         |       |       |       |           |
 *  |         MutableBitboardByType |      10000 | 6,948.3298 ns |  31.7677 ns |  26.5274 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  |          MutableBitboardByRef |      10000 | 7,110.9961 ns | 129.4807 ns | 114.7813 ns |  1.02 |    0.02 |     - |     - |     - |         - |
 *  | MutableReadonlyBitboardByType |      10000 | 6,954.8409 ns |  29.1189 ns |  24.3156 ns |  1.00 |    0.00 |     - |     - |     - |         - |
 *  |  MutableReadonlyBitboardByRef |      10000 | 7,091.4471 ns | 124.1205 ns | 110.0296 ns |  1.02 |    0.02 |     - |     - |     - |         - |
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class MutableReadonlyBitboard_Benchmark : BaseBenchmark
{
    private struct Mutable
    {
        public ulong Board { get; set; }

        public Mutable(ulong n)
        {
            Board = n;
        }
    }

    private struct MutableReadonly
    {
#pragma warning disable RCS1170 // Use read-only auto-implemented property.
        public ulong Board { readonly get; private set; }
#pragma warning restore RCS1170 // Use read-only auto-implemented property.

        public MutableReadonly(ulong n)
        {
            Board = n;
        }
    }

    private Mutable _mutableBoard = new(1234);
    private MutableReadonly _mutableReadonlyBoard = new(1234);

    public static IEnumerable<int> Data => [1, 10, 1_000, 10_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ulong MutableBitboardByType(int iterations)
    {
        var result = 0UL;
        for (int i = 0; i < iterations; ++i) result += AddMutableByType(_mutableBoard, _mutableBoard);
        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong MutableBitboardByRef(int iterations)
    {
        var result = 0UL;
        for (int i = 0; i < iterations; ++i) result += AddMutableByRef(in _mutableBoard, in _mutableBoard);
        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong MutableReadonlyBitboardByType(int iterations)
    {
        var result = 0UL;
        for (int i = 0; i < iterations; ++i) result += AddMutableReadonlyByType(_mutableReadonlyBoard, _mutableReadonlyBoard);
        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong MutableReadonlyBitboardByRef(int iterations)
    {
        var result = 0UL;
        for (int i = 0; i < iterations; ++i) result += AddMutableReadonlyByRef(in _mutableReadonlyBoard, in _mutableReadonlyBoard);
        return result;
    }

#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference. That's the purpose of the test
    private static ulong AddMutableByType(Mutable a, Mutable b) => a.Board + b.Board;
    private static ulong AddMutableByRef(in Mutable a, in Mutable b) => a.Board + b.Board;
    private static ulong AddMutableReadonlyByType(MutableReadonly a, MutableReadonly b) => a.Board + b.Board;
    private static ulong AddMutableReadonlyByRef(in MutableReadonly a, in MutableReadonly b) => a.Board + b.Board;
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference.
}
