/*
 *
 *  BenchmarkDotNet v0.13.7, Windows 10 (10.0.20348.1850) (Hyper-V)
 *  Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *
 *
 *  | Method |       size |      Mean |     Error |    StdDev | Ratio | Allocated | Alloc Ratio |
 *  |------- |----------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Modulo |    2097152 | 97.429 us | 1.8213 us | 1.7036 us |  1.00 |         - |          NA |
 *  |    And |    2097152 |  7.107 us | 0.1393 us | 0.1710 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |    4194304 | 95.984 us | 0.9829 us | 0.8713 us |  1.00 |         - |          NA |
 *  |    And |    4194304 |  7.024 us | 0.1348 us | 0.1261 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |    8388608 | 98.313 us | 1.9542 us | 1.9192 us |  1.00 |         - |          NA |
 *  |    And |    8388608 |  7.031 us | 0.0797 us | 0.0707 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   16777216 | 96.734 us | 0.8187 us | 0.6392 us |  1.00 |         - |          NA |
 *  |    And |   16777216 |  7.051 us | 0.0885 us | 0.0828 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   33554432 | 97.209 us | 1.0735 us | 1.0041 us |  1.00 |         - |          NA |
 *  |    And |   33554432 |  7.214 us | 0.1191 us | 0.1114 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   67108864 | 97.015 us | 1.4319 us | 1.3394 us |  1.00 |         - |          NA |
 *  |    And |   67108864 |  6.979 us | 0.0889 us | 0.0742 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  134217728 | 96.075 us | 1.2594 us | 1.1164 us |  1.00 |         - |          NA |
 *  |    And |  134217728 |  6.979 us | 0.0808 us | 0.0756 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  268435456 | 96.507 us | 1.6562 us | 1.5492 us |  1.00 |         - |          NA |
 *  |    And |  268435456 |  6.941 us | 0.0405 us | 0.0316 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  536870912 | 95.550 us | 0.7486 us | 0.6636 us |  1.00 |         - |          NA |
 *  |    And |  536870912 |  6.987 us | 0.1078 us | 0.1008 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo | 1073741824 | 95.537 us | 0.9683 us | 0.8584 us |  1.00 |         - |          NA |
 *  |    And | 1073741824 |  7.036 us | 0.0775 us | 0.0687 us |  0.07 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.7, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *
 *
 *  | Method |       size |      Mean |     Error |    StdDev | Ratio | Allocated | Alloc Ratio |
 *  |------- |----------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Modulo |    2097152 | 89.688 us | 1.7218 us | 1.9829 us |  1.00 |         - |          NA |
 *  |    And |    2097152 |  6.625 us | 0.1171 us | 0.1038 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |    4194304 | 91.862 us | 1.7469 us | 2.8209 us |  1.00 |         - |          NA |
 *  |    And |    4194304 |  6.557 us | 0.1248 us | 0.1437 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |    8388608 | 97.875 us | 1.9459 us | 4.5100 us |  1.00 |         - |          NA |
 *  |    And |    8388608 |  7.180 us | 0.1094 us | 0.1023 us |  0.08 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   16777216 | 97.111 us | 1.4592 us | 1.3649 us |  1.00 |         - |          NA |
 *  |    And |   16777216 |  7.029 us | 0.0961 us | 0.0852 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   33554432 | 96.687 us | 1.4462 us | 1.2821 us |  1.00 |         - |          NA |
 *  |    And |   33554432 |  7.116 us | 0.1292 us | 0.1587 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   67108864 | 95.414 us | 1.9038 us | 2.4755 us |  1.00 |         - |          NA |
 *  |    And |   67108864 |  6.706 us | 0.1328 us | 0.1727 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  134217728 | 90.297 us | 1.7617 us | 2.7943 us |  1.00 |         - |          NA |
 *  |    And |  134217728 |  6.429 us | 0.1149 us | 0.1019 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  268435456 | 90.804 us | 1.7107 us | 1.7568 us |  1.00 |         - |          NA |
 *  |    And |  268435456 |  7.288 us | 0.1273 us | 0.1191 us |  0.08 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  536870912 | 97.162 us | 1.4213 us | 1.3294 us |  1.00 |         - |          NA |
 *  |    And |  536870912 |  7.063 us | 0.1331 us | 0.1367 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo | 1073741824 | 96.803 us | 0.6576 us | 0.5492 us |  1.00 |         - |          NA |
 *  |    And | 1073741824 |  7.006 us | 0.0816 us | 0.0724 us |  0.07 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.7, macOS Monterey 12.6.7 (21G651) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *
 *
 *  | Method |       size |      Mean |     Error |    StdDev | Ratio | Allocated | Alloc Ratio |
 *  |------- |----------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Modulo |    2097152 | 88.642 us | 0.3838 us | 0.3590 us |  1.00 |         - |          NA |
 *  |    And |    2097152 |  5.739 us | 0.0490 us | 0.0458 us |  0.06 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |    4194304 | 90.434 us | 1.7834 us | 1.7516 us |  1.00 |         - |          NA |
 *  |    And |    4194304 |  5.769 us | 0.1140 us | 0.1401 us |  0.06 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |    8388608 | 89.637 us | 1.7229 us | 1.9841 us |  1.00 |         - |          NA |
 *  |    And |    8388608 |  5.846 us | 0.1161 us | 0.1192 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   16777216 | 90.198 us | 1.7830 us | 1.6678 us |  1.00 |         - |          NA |
 *  |    And |   16777216 |  5.814 us | 0.0910 us | 0.0806 us |  0.06 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   33554432 | 90.161 us | 1.5999 us | 1.3360 us |  1.00 |         - |          NA |
 *  |    And |   33554432 |  5.830 us | 0.0894 us | 0.0698 us |  0.06 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |   67108864 | 90.224 us | 1.3689 us | 1.2135 us |  1.00 |         - |          NA |
 *  |    And |   67108864 |  5.900 us | 0.1072 us | 0.1700 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  134217728 | 89.650 us | 1.4173 us | 1.3258 us |  1.00 |         - |          NA |
 *  |    And |  134217728 |  5.863 us | 0.1158 us | 0.1334 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  268435456 | 90.653 us | 1.5062 us | 1.9585 us |  1.00 |         - |          NA |
 *  |    And |  268435456 |  5.946 us | 0.1170 us | 0.1890 us |  0.07 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo |  536870912 | 91.520 us | 1.8254 us | 2.3735 us |  1.00 |         - |          NA |
 *  |    And |  536870912 |  5.783 us | 0.1068 us | 0.0999 us |  0.06 |         - |          NA |
 *  |        |            |           |           |           |       |           |             |
 *  | Modulo | 1073741824 | 88.625 us | 1.6487 us | 1.3767 us |  1.00 |         - |          NA |
 *  |    And | 1073741824 |  5.740 us | 0.0438 us | 0.0389 us |  0.06 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class TranspositionTableIndex_Benchmark : BaseBenchmark
{
    public static IEnumerable<int> Data =>
    [
        2 * 1024 * 1024,
        4 * 1024 * 1024,
        8 * 1024 * 1024,
        16 * 1024 * 1024,
        32 * 1024 * 1024,
        64 * 1024 * 1024,
        128 * 1024 * 1024,
        256 * 1024 * 1024,
        512 * 1024 * 1024,
        1024 * 1024 * 1024
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public long Modulo(int size)
    {
        var mask = size - 1;

        long total = 0;
        for (long i = 0; i < 10_000L; ++i)
        {
            total += i % size;
        }

        return total + mask;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long And(int size)
    {
        var mask = size - 1;

        long total = 0;
        for (long i = 0; i < 10_000L; ++i)
        {
            total += i & mask;
        }

        return total + mask;
    }
}
