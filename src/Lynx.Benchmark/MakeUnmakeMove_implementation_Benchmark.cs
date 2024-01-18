/*
 * Consistent local reults
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                                          | data                 | Mean         | Error       | StdDev      | Ratio | RatioSD | Gen0       | Allocated     | Alloc Ratio |
 *  |------------------------------------------------ |--------------------- |-------------:|------------:|------------:|------:|--------:|-----------:|--------------:|------------:|
 *  | NewPosition                                     | (2K2r(...)1, 6) [38] | 397,594.7 us | 4,212.70 us | 3,734.45 us |  1.00 |    0.00 | 13000.0000 | 1069286.11 KB |        1.00 |
 *  | MakeUnmakeMove_Original                         | (2K2r(...)1, 6) [38] | 434,075.4 us | 1,349.84 us | 1,127.17 us |  1.09 |    0.01 |  3000.0000 |  278498.13 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                   | (2K2r(...)1, 6) [38] | 275,547.0 us | 2,201.86 us | 2,059.62 us |  0.69 |    0.01 |  3000.0000 |  278497.61 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (2K2r(...)1, 6) [38] |  21,979.3 us |   169.82 us |   158.85 us |  0.06 |    0.00 |   406.2500 |   35056.14 KB |        0.03 |
 *  |                                                 |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                     | (3K4/(...)1, 6) [38] | 394,190.6 us | 2,760.12 us | 2,446.77 us |  1.00 |    0.00 | 13000.0000 | 1069286.11 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (3K4/(...)1, 6) [38] | 451,305.4 us | 1,804.21 us | 1,599.39 us |  1.14 |    0.01 |  3000.0000 |  278498.13 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                   | (3K4/(...)1, 6) [38] | 275,680.2 us | 2,376.10 us | 2,222.60 us |  0.70 |    0.01 |  3000.0000 |  278497.61 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (3K4/(...)1, 6) [38] |  21,480.1 us |    51.64 us |    45.78 us |  0.05 |    0.00 |    62.5000 |    5446.33 KB |       0.005 |
 *  |                                                 |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                     | (8/p7(...)-, 6) [37] |  14,014.0 us |    74.89 us |    70.05 us |  1.00 |    0.00 |   515.6250 |    42915.8 KB |        1.00 |
 *  | MakeUnmakeMove_Original                         | (8/p7(...)-, 6) [37] |  15,909.3 us |    47.47 us |    42.08 us |  1.13 |    0.01 |   187.5000 |   17113.08 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                   | (8/p7(...)-, 6) [37] |  10,438.8 us |    93.73 us |    83.09 us |  0.74 |    0.01 |   203.1250 |   17113.07 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (8/p7(...)-, 6) [37] |   4,719.8 us |    36.61 us |    32.45 us |  0.34 |    0.00 |   101.5625 |    8398.54 KB |        0.20 |
 *  |                                                 |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                     | (r3k2(...)1, 4) [73] | 360,128.3 us | 4,954.12 us | 4,391.70 us | 1.000 |    0.00 |  9000.0000 |  801233.53 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (r3k2(...)1, 4) [73] | 347,125.5 us | 1,845.83 us | 1,441.10 us | 0.963 |    0.01 |  1000.0000 |  104639.41 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                   | (r3k2(...)1, 4) [73] | 240,126.7 us | 1,455.36 us | 1,361.34 us | 0.666 |    0.01 |  1000.0000 |  104638.72 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (r3k2(...)1, 4) [73] |     165.8 us |     1.48 us |     1.39 us | 0.000 |    0.00 |     1.2207 |     102.76 KB |       0.000 |
 *  |                                                 |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                     | (r4rk(...)0, 4) [77] | 312,452.4 us | 2,062.78 us | 1,929.52 us | 1.000 |    0.00 |  9000.0000 |  765557.21 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (r4rk(...)0, 4) [77] | 367,667.9 us |   725.59 us |   605.90 us | 1.176 |    0.01 |  1000.0000 |   96333.51 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                   | (r4rk(...)0, 4) [77] | 214,976.8 us | 1,413.40 us | 1,322.10 us | 0.688 |    0.01 |  1000.0000 |   96332.81 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (r4rk(...)0, 4) [77] |     351.6 us |     1.32 us |     1.17 us | 0.001 |    0.00 |     2.4414 |      208.5 KB |       0.000 |
 *  |                                                 |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                     | (rnbq(...)1, 4) [61] |  17,522.2 us |   312.79 us |   292.58 us |  1.00 |    0.00 |   531.2500 |   43733.27 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (rnbq(...)1, 4) [61] |  15,792.3 us |    34.00 us |    28.39 us |  0.90 |    0.02 |    93.7500 |    9760.59 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                   | (rnbq(...)1, 4) [61] |  11,878.4 us |    85.36 us |    79.85 us |  0.68 |    0.01 |   109.3750 |    9760.58 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (rnbq(...)1, 4) [61] |     251.4 us |     0.63 us |     0.49 us |  0.01 |    0.00 |     2.4414 |     220.02 KB |       0.005 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                                          | data                 | Mean         | Error       | StdDev      | Ratio | Gen0       | Gen1      | Allocated     | Alloc Ratio |
 *  |------------------------------------------------ |--------------------- |-------------:|------------:|------------:|------:|-----------:|----------:|--------------:|------------:|
 *  | NewPosition                                     | (2K2r(...)1, 6) [38] | 366,053.0 us | 2,768.52 us | 2,589.68 us |  1.00 | 65000.0000 | 1000.0000 | 1069285.78 KB |        1.00 |
 *  | MakeUnmakeMove_Original                         | (2K2r(...)1, 6) [38] | 447,409.1 us | 4,180.66 us | 3,706.04 us |  1.22 | 17000.0000 |         - |   278497.8 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                   | (2K2r(...)1, 6) [38] | 274,326.0 us |   704.09 us |   624.15 us |  0.75 | 17000.0000 |         - |  278497.45 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (2K2r(...)1, 6) [38] |  22,248.0 us |   115.31 us |    96.29 us |  0.06 |  2125.0000 |   31.2500 |   35057.84 KB |        0.03 |
 *  |                                                 |                      |              |             |             |       |            |           |               |             |
 *  | NewPosition                                     | (3K4/(...)1, 6) [38] | 348,384.8 us | 2,910.09 us | 2,579.72 us |  1.00 | 65000.0000 | 1000.0000 | 1069285.78 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (3K4/(...)1, 6) [38] | 445,346.5 us |   824.01 us |   688.08 us |  1.28 | 17000.0000 |         - |   278497.8 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                   | (3K4/(...)1, 6) [38] | 270,130.7 us |   599.63 us |   500.72 us |  0.77 | 17000.0000 |         - |  278497.45 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (3K4/(...)1, 6) [38] |  21,160.2 us |   136.92 us |   128.07 us |  0.06 |   312.5000 |         - |    5447.08 KB |       0.005 |
 *  |                                                 |                      |              |             |             |       |            |           |               |             |
 *  | NewPosition                                     | (8/p7(...)-, 6) [37] |  12,731.9 us |   118.51 us |   110.86 us |  1.00 |  2625.0000 |   62.5000 |   42917.22 KB |        1.00 |
 *  | MakeUnmakeMove_Original                         | (8/p7(...)-, 6) [37] |  16,804.4 us |   112.51 us |   105.24 us |  1.32 |  1031.2500 |         - |   17115.45 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                   | (8/p7(...)-, 6) [37] |  10,124.0 us |    60.33 us |    53.48 us |  0.80 |  1046.8750 |   15.6250 |   17115.44 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (8/p7(...)-, 6) [37] |   4,634.1 us |    22.34 us |    20.90 us |  0.36 |   507.8125 |    7.8125 |    8399.77 KB |        0.20 |
 *  |                                                 |                      |              |             |             |       |            |           |               |             |
 *  | NewPosition                                     | (r3k2(...)1, 4) [73] | 290,074.0 us | 2,156.13 us | 1,911.36 us | 1.000 | 49000.0000 |  500.0000 |  801232.84 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (r3k2(...)1, 4) [73] | 335,338.4 us |   709.24 us |   628.72 us | 1.156 |  6000.0000 |         - |  104639.09 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                   | (r3k2(...)1, 4) [73] | 263,131.3 us |   472.10 us |   394.22 us | 0.907 |  6000.0000 |         - |  104638.73 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (r3k2(...)1, 4) [73] |     166.1 us |     0.32 us |     0.30 us | 0.001 |     6.1035 |         - |     102.78 KB |       0.000 |
 *  |                                                 |                      |              |             |             |       |            |           |               |             |
 *  | NewPosition                                     | (r4rk(...)0, 4) [77] | 255,742.8 us | 2,229.76 us | 2,085.72 us | 1.000 | 46500.0000 |  500.0000 |  765557.05 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (r4rk(...)0, 4) [77] | 279,471.5 us |   511.51 us |   453.44 us | 1.092 |  5500.0000 |         - |   96332.82 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                   | (r4rk(...)0, 4) [77] | 208,362.8 us |   869.15 us |   813.01 us | 0.815 |  5666.6667 |         - |    96332.7 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (r4rk(...)0, 4) [77] |     357.0 us |     0.96 us |     0.90 us | 0.001 |    12.6953 |         - |     208.53 KB |       0.000 |
 *  |                                                 |                      |              |             |             |       |            |           |               |             |
 *  | NewPosition                                     | (rnbq(...)1, 4) [61] |  14,302.6 us |   142.12 us |   125.99 us |  1.00 |  2671.8750 |   31.2500 |   43734.68 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (rnbq(...)1, 4) [61] |  20,109.9 us |    44.69 us |    39.61 us |  1.41 |   593.7500 |         - |    9762.01 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                   | (rnbq(...)1, 4) [61] |  11,840.0 us |    44.82 us |    41.93 us |  0.83 |   593.7500 |         - |       9762 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (rnbq(...)1, 4) [61] |     250.3 us |     1.11 us |     1.04 us |  0.02 |    13.1836 |         - |     220.05 KB |       0.005 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                                          | data                 | Mean         | Error       | StdDev       | Ratio | RatioSD | Gen0        | Gen1      | Allocated     | Alloc Ratio |
 *  |------------------------------------------------ |--------------------- |-------------:|------------:|-------------:|------:|--------:|------------:|----------:|--------------:|------------:|
 *  | NewPosition                                     | (2K2r(...)1, 6) [38] | 497,953.8 us | 9,759.34 us | 12,689.90 us |  1.00 |    0.00 | 174000.0000 | 2000.0000 | 1069286.11 KB |        1.00 |
 *  | MakeUnmakeMove_Original                         | (2K2r(...)1, 6) [38] | 468,175.4 us | 9,099.16 us | 11,507.53 us |  0.94 |    0.03 |  45000.0000 |         - |  278498.13 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                   | (2K2r(...)1, 6) [38] | 308,002.9 us | 6,088.07 us |  5,083.82 us |  0.61 |    0.01 |  45000.0000 |  500.0000 |  278497.61 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (2K2r(...)1, 6) [38] |  25,735.8 us |   430.37 us |    719.06 us |  0.05 |    0.00 |   5718.7500 |   62.5000 |   35057.85 KB |        0.03 |
 *  |                                                 |                      |              |             |              |       |         |             |           |               |             |
 *  | NewPosition                                     | (3K4/(...)1, 6) [38] | 485,879.0 us | 9,441.38 us | 11,940.33 us |  1.00 |    0.00 | 174000.0000 | 2000.0000 | 1069286.11 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (3K4/(...)1, 6) [38] | 461,012.4 us | 8,901.81 us | 11,574.87 us |  0.95 |    0.03 |  45000.0000 |         - |  278498.13 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                   | (3K4/(...)1, 6) [38] | 311,701.0 us | 5,907.80 us |  6,566.51 us |  0.64 |    0.02 |  45000.0000 |  500.0000 |  278497.61 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (3K4/(...)1, 6) [38] |  23,776.9 us |   438.04 us |    789.88 us |  0.05 |    0.00 |    857.1429 |         - |    5448.69 KB |       0.005 |
 *  |                                                 |                      |              |             |              |       |         |             |           |               |             |
 *  | NewPosition                                     | (8/p7(...)-, 6) [37] |  20,686.2 us |   765.70 us |  2,108.97 us |  1.00 |    0.00 |   7000.0000 |  125.0000 |   42917.24 KB |        1.00 |
 *  | MakeUnmakeMove_Original                         | (8/p7(...)-, 6) [37] |  18,236.6 us |   432.24 us |  1,247.10 us |  0.89 |    0.11 |   2733.3333 |         - |    17115.5 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                   | (8/p7(...)-, 6) [37] |  11,901.3 us |   168.57 us |    140.76 us |  0.63 |    0.06 |   2781.2500 |   31.2500 |   17115.45 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (8/p7(...)-, 6) [37] |   5,332.6 us |   104.37 us |    257.97 us |  0.26 |    0.03 |   1367.1875 |   23.4375 |    8401.25 KB |        0.20 |
 *  |                                                 |                      |              |             |              |       |         |             |           |               |             |
 *  | NewPosition                                     | (r3k2(...)1, 4) [73] | 437,978.9 us | 9,011.62 us | 26,570.98 us | 1.000 |    0.00 | 130000.0000 | 1000.0000 |  801233.53 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (r3k2(...)1, 4) [73] | 363,920.4 us | 4,364.21 us |  3,407.29 us | 0.804 |    0.03 |  17000.0000 |         - |  104639.41 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                   | (r3k2(...)1, 4) [73] | 278,624.2 us | 4,591.36 us |  4,294.76 us | 0.610 |    0.03 |  17000.0000 |         - |  104638.89 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (r3k2(...)1, 4) [73] |     163.9 us |     2.42 us |      2.26 us | 0.000 |    0.00 |     16.6016 |         - |     102.81 KB |       0.000 |
 *  |                                                 |                      |              |             |              |       |         |             |           |               |             |
 *  | NewPosition                                     | (r4rk(...)0, 4) [77] | 383,821.2 us | 7,672.70 us | 10,242.84 us | 1.000 |    0.00 | 124000.0000 | 1000.0000 |  765557.73 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (r4rk(...)0, 4) [77] | 373,585.2 us | 5,621.68 us |  4,983.47 us | 0.963 |    0.02 |  15000.0000 |         - |   96333.51 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                   | (r4rk(...)0, 4) [77] | 238,751.5 us | 1,165.60 us |    973.33 us | 0.616 |    0.02 |  15000.0000 |         - |   96333.51 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (r4rk(...)0, 4) [77] |     382.2 us |     5.68 us |      5.03 us | 0.001 |    0.00 |     33.6914 |         - |      208.6 KB |       0.000 |
 *  |                                                 |                      |              |             |              |       |         |             |           |               |             |
 *  | NewPosition                                     | (rnbq(...)1, 4) [61] |  21,280.7 us |   348.64 us |    326.12 us |  1.00 |    0.00 |   7125.0000 |   93.7500 |    43734.7 KB |       1.000 |
 *  | MakeUnmakeMove_Original                         | (rnbq(...)1, 4) [61] |  18,844.6 us |   418.23 us |  1,233.15 us |  0.85 |    0.04 |   1593.7500 |         - |    9763.26 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                   | (rnbq(...)1, 4) [61] |  14,520.0 us |   286.52 us |    552.03 us |  0.68 |    0.03 |   1593.7500 |   15.6250 |    9763.24 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove | (rnbq(...)1, 4) [61] |     306.5 us |     5.95 us |     15.58 us |  0.01 |    0.00 |     35.6445 |         - |     220.12 KB |       0.005 |
 */

#pragma warning disable S101, S1854 // Types should be named in PascalCase

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class MakeUnmakeMove_implementation_Benchmark : BaseBenchmark
{
    public static IEnumerable<(string, int)> Data => new[] {
            (Constants.InitialPositionFEN, 4),
            (Constants.TrickyTestPositionFEN, 4),
            ("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 4),
            ("3K4/8/8/8/8/8/4p3/2k2R2 b - - 0 1", 6),
            ("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1", 6),
            ("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -", 6),
        };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public long NewPosition((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_Original(new(data.Fen), data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove_Original((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_MakeUnmakeMove(new(data.Fen), data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove_WithZobristKey((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_MakeUnmakeMove_WithZobritsKey(new(data.Fen), data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_MakeUnmakeMove_WithZobritsKey_PreSwitchSpecialMove(new(data.Fen), data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove_WithZobristKey_SwitchSpecialMove((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_MakeUnmakeMove_WithZobritsKey_SwitchSpecialMove(new(data.Fen), data.Depth, default);

    public static class MakeMovePerft
    {
        public static long ResultsImpl_Original(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    var newPosition = new MakeMovePosition(position, move);

                    if (newPosition.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_Original(newPosition, depth - 1, nodes);
                    }
                }

                return nodes;
            }

            return ++nodes;
        }

        public static long ResultsImpl_MakeUnmakeMove(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    //_gameStates.Push(position.MakeMove(move));
                    var state = position.MakeMove_Original(move);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_MakeUnmakeMove(position, depth - 1, nodes);
                    }
                    //position.UnmakeMove(move, _gameStates.Pop());
                    position.UnmakeMove_Original(move, state);
                }

                return nodes;
            }

            return ++nodes;
        }

        public static long ResultsImpl_MakeUnmakeMove_WithZobritsKey(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    //_gameStates.Push(position.MakeMove(move));
                    var state = position.MakeMove_WithZobristKey(move);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_MakeUnmakeMove_WithZobritsKey(position, depth - 1, nodes);
                    }
                    //position.UnmakeMove(move, _gameStates.Pop());
                    position.UnmakeMove_WithZobristKey(move, state);
                }

                return nodes;
            }

            return ++nodes;
        }

        public static long ResultsImpl_MakeUnmakeMove_WithZobritsKey_PreSwitchSpecialMove(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    //_gameStates.Push(position.MakeMove(move));
                    var state = position.MakeMove_WithZobristKey_PreSwitchSpecialMove(move);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_MakeUnmakeMove_WithZobritsKey_PreSwitchSpecialMove(position, depth - 1, nodes);
                    }
                    //position.UnmakeMove(move, _gameStates.Pop());
                    position.UnmakeMove_WithZobristKey_PreSwitchSpecialMove(move, state);
                }

                return nodes;
            }

            return ++nodes;
        }

        public static long ResultsImpl_MakeUnmakeMove_WithZobritsKey_SwitchSpecialMove(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    //_gameStates.Push(position.MakeMove(move));
                    var state = position.MakeMove_WithZobristKey_SwitchSpecialMove(move);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_MakeUnmakeMove_WithZobritsKey_SwitchSpecialMove(position, depth - 1, nodes);
                    }
                    //position.UnmakeMove(move, _gameStates.Pop());
                    position.UnmakeMove_WithZobristKey_SwitchSpecialMove(move, state);
                }

                return nodes;
            }

            return ++nodes;
        }
    }

    public struct MakeMovePosition
    {
        public long UniqueIdentifier { get; private set; }

        public BitBoard[] PieceBitBoards { get; }

        public BitBoard[] OccupancyBitBoards { get; }

        public Side Side { get; private set; }

        public BoardSquare EnPassant { get; private set; }

        public byte Castle { get; private set; }

        public MakeMovePosition(string fen) : this(FENParser.ParseFEN(fen))
        {
        }

        public MakeMovePosition((BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, byte Castle, BoardSquare EnPassant,
            int HalfMoveClock/*, int FullMoveCounter*/) parsedFEN)
        {
            PieceBitBoards = parsedFEN.PieceBitBoards;
            OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
            Side = parsedFEN.Side;
            Castle = parsedFEN.Castle;
            EnPassant = parsedFEN.EnPassant;

            UniqueIdentifier = MakeMoveZobristTable.PositionHash(this);
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="position"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMovePosition(MakeMovePosition position)
        {
            UniqueIdentifier = position.UniqueIdentifier;
            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = position.Side;
            Castle = position.Castle;
            EnPassant = position.EnPassant;
        }

        /// <summary>
        /// Null moves constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="nullMove"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable RCS1163, IDE0060 // Unused parameter.
        public MakeMovePosition(MakeMovePosition position, bool nullMove)
        {
            UniqueIdentifier = position.UniqueIdentifier;
            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = (Side)Utils.OppositeSide(position.Side);
            Castle = position.Castle;
            EnPassant = BoardSquare.noSquare;

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.EnPassantHash((int)position.EnPassant);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMovePosition(MakeMovePosition position, Move move) : this(position)
        {
            var oldSide = Side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            EnPassant = BoardSquare.noSquare;

            PieceBitBoards[piece].PopBit(sourceSquare);
            OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

            PieceBitBoards[newPiece].SetBit(targetSquare);
            OccupancyBitBoards[(int)Side].SetBit(targetSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)position.EnPassant)
                ^ ZobristTable.CastleHash(position.Castle);

            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                }
                else
                {
                    var limit = (int)Piece.K + oppositeSideOffset;
                    for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                    {
                        if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                        {
                            PieceBitBoards[pieceIndex].PopBit(targetSquare);
                            UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                            break;
                        }
                    }

                    OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
                }
            }
            else if (move.IsDoublePawnPush())
            {
                var pawnPush = +8 - ((int)oldSide * 16);
                var enPassantSquare = sourceSquare + pawnPush;

                EnPassant = (BoardSquare)enPassantSquare;
                UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMoveGameState MakeMove_Original(Move move)
        {
            int capturedPiece = -1;
            var castleCopy = Castle;
            BoardSquare enpassantCopy = EnPassant;

            var oldSide = Side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[piece].PopBit(sourceSquare);
            OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

            PieceBitBoards[newPiece].SetBit(targetSquare);
            OccupancyBitBoards[(int)Side].SetBit(targetSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)EnPassant)            // We clear the existing enpassant square, if any
                ^ ZobristTable.CastleHash(Castle);                      // We clear the existing castle rights

            EnPassant = BoardSquare.noSquare;
            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                    capturedPiece = oppositePawnIndex;
                }
                else
                {
                    var limit = (int)Piece.K + oppositeSideOffset;
                    for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                    {
                        if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                        {
                            PieceBitBoards[pieceIndex].PopBit(targetSquare);
                            UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                            capturedPiece = pieceIndex;
                            break;
                        }
                    }

                    OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
                }
            }
            else if (move.IsDoublePawnPush())
            {
                var pawnPush = +8 - ((int)oldSide * 16);
                var enPassantSquare = sourceSquare + pawnPush;

                EnPassant = (BoardSquare)enPassantSquare;
                UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

            return new MakeMoveGameState(capturedPiece, castleCopy, enpassantCopy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnmakeMove_Original(Move move, MakeMoveGameState gameState)
        {
            var oppositeSide = (int)Side;
            Side = (Side)Utils.OppositeSide(Side);
            var offset = Utils.PieceOffset(Side);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[newPiece].PopBit(targetSquare);
            OccupancyBitBoards[(int)Side].PopBit(targetSquare);

            PieceBitBoards[piece].SetBit(sourceSquare);
            OccupancyBitBoards[(int)Side].SetBit(sourceSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)EnPassant)        // We clear the existing enpassant square, if any
                ^ ZobristTable.CastleHash(Castle);                  // We clear the existing castling rights

            EnPassant = BoardSquare.noSquare;
            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(OccupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                        $"Expected empty {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                }
                else
                {
                    PieceBitBoards[gameState.CapturedPiece].SetBit(targetSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, gameState.CapturedPiece);

                    OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
                }
            }
            else if (move.IsDoublePawnPush())
            {
                EnPassant = gameState.EnPassant;
                UniqueIdentifier ^= ZobristTable.EnPassantHash((int)EnPassant);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(Side);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(Side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(Side);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(Side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating castling rights
            Castle = gameState.Castle;

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMoveGameStateWithZobristKey MakeMove_WithZobristKey(Move move)
        {
            int capturedPiece = -1;
            var castleCopy = Castle;
            BoardSquare enpassantCopy = EnPassant;
            long uniqueIdentifierCopy = UniqueIdentifier;

            var oldSide = Side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[piece].PopBit(sourceSquare);
            OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

            PieceBitBoards[newPiece].SetBit(targetSquare);
            OccupancyBitBoards[(int)Side].SetBit(targetSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)EnPassant)            // We clear the existing enpassant square, if any
                ^ ZobristTable.CastleHash(Castle);                      // We clear the existing castle rights

            EnPassant = BoardSquare.noSquare;
            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                    capturedPiece = oppositePawnIndex;
                }
                else
                {
                    var limit = (int)Piece.K + oppositeSideOffset;
                    for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                    {
                        if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                        {
                            PieceBitBoards[pieceIndex].PopBit(targetSquare);
                            UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                            capturedPiece = pieceIndex;
                            break;
                        }
                    }

                    OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
                }
            }
            else if (move.IsDoublePawnPush())
            {
                var pawnPush = +8 - ((int)oldSide * 16);
                var enPassantSquare = sourceSquare + pawnPush;

                EnPassant = (BoardSquare)enPassantSquare;
                UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

            return new MakeMoveGameStateWithZobristKey(capturedPiece, castleCopy, enpassantCopy, uniqueIdentifierCopy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMoveGameStateWithZobristKey MakeMove_WithZobristKey_PreSwitchSpecialMove(Move move)
        {
            int capturedPiece = -1;
            byte castleCopy = Castle;
            BoardSquare enpassantCopy = EnPassant;
            long uniqueIdentifierCopy = UniqueIdentifier;

            var oldSide = (int)Side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[piece].PopBit(sourceSquare);
            OccupancyBitBoards[oldSide].PopBit(sourceSquare);

            PieceBitBoards[newPiece].SetBit(targetSquare);
            OccupancyBitBoards[oldSide].SetBit(targetSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)EnPassant)            // We clear the existing enpassant square, if any
                ^ ZobristTable.CastleHash(Castle);                      // We clear the existing castle rights

            EnPassant = BoardSquare.noSquare;
            if (move.IsCapture())
            {
                var oppositePawnIndex = (int)Piece.p - offset;

                var capturedSquare = targetSquare;
                capturedPiece = oppositePawnIndex;
                if (move.IsEnPassant())
                {
                    capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");
                }
                else
                {
                    capturedPiece = move.CapturedPiece();
                }

                PieceBitBoards[capturedPiece].PopBit(capturedSquare);
                OccupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                UniqueIdentifier ^= ZobristTable.PieceHash(capturedSquare, capturedPiece);
            }
            else if (move.IsDoublePawnPush())
            {
                var pawnPush = +8 - (oldSide * 16);
                var enPassantSquare = sourceSquare + pawnPush;
                Utils.Assert(Constants.EnPassantCaptureSquares.Length > enPassantSquare && Constants.EnPassantCaptureSquares[enPassantSquare] != 0, $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

                EnPassant = (BoardSquare)enPassantSquare;
                UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[oldSide].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[oldSide].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                OccupancyBitBoards[oldSide].PopBit(rookSourceSquare);

                PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                OccupancyBitBoards[oldSide].SetBit(rookTargetSquare);

                UniqueIdentifier ^=
                    ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                    ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[2] = OccupancyBitBoards[1] | OccupancyBitBoards[0];

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

            return new MakeMoveGameStateWithZobristKey(capturedPiece, castleCopy, enpassantCopy, uniqueIdentifierCopy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMoveGameStateWithZobristKey MakeMove_WithZobristKey_SwitchSpecialMove(Move move)
        {
            int capturedPiece = -1;

            byte castleCopy = Castle;
            BoardSquare enpassantCopy = EnPassant;
            long uniqueIdentifierCopy = UniqueIdentifier;

            var oldSide = (int)Side;
            var offset = Utils.PieceOffset(oldSide);
            var oppositeSide = Utils.OppositeSide(oldSide);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[piece].PopBit(sourceSquare);
            OccupancyBitBoards[oldSide].PopBit(sourceSquare);

            PieceBitBoards[newPiece].SetBit(targetSquare);
            OccupancyBitBoards[oldSide].SetBit(targetSquare);

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.PieceHash(sourceSquare, piece)
                ^ ZobristTable.PieceHash(targetSquare, newPiece)
                ^ ZobristTable.EnPassantHash((int)EnPassant)            // We clear the existing enpassant square, if any
                ^ ZobristTable.CastleHash(Castle);                      // We clear the existing castle rights

            EnPassant = BoardSquare.noSquare;

            switch (move.SpecialMoveFlag())
            {
                case SpecialMoveType.DoublePawnPush:
                    {
                        var pawnPush = +8 - (oldSide * 16);
                        var enPassantSquare = sourceSquare + pawnPush;
                        Utils.Assert(Constants.EnPassantCaptureSquares.Length > enPassantSquare && Constants.EnPassantCaptureSquares[enPassantSquare] != 0, $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

                        EnPassant = (BoardSquare)enPassantSquare;
                        UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                        break;
                    }
                case SpecialMoveType.ShortCastle:
                    {
                        var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                        OccupancyBitBoards[oldSide].PopBit(rookSourceSquare);

                        PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                        OccupancyBitBoards[oldSide].SetBit(rookTargetSquare);

                        UniqueIdentifier ^=
                            ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        break;
                    }
                case SpecialMoveType.LongCastle:
                    {
                        var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                        OccupancyBitBoards[oldSide].PopBit(rookSourceSquare);

                        PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                        OccupancyBitBoards[oldSide].SetBit(rookTargetSquare);

                        UniqueIdentifier ^=
                            ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        break;
                    }
                case SpecialMoveType.EnPassant:
                    {
                        var oppositePawnIndex = (int)Piece.p - offset;

                        var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                        capturedPiece = oppositePawnIndex;
                        Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");

                        PieceBitBoards[capturedPiece].PopBit(capturedSquare);
                        OccupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                        UniqueIdentifier ^= ZobristTable.PieceHash(capturedSquare, capturedPiece);

                        break;
                    }
                default:
                    {
                        if (move.IsCapture())
                        {
                            var capturedSquare = targetSquare;
                            capturedPiece = move.CapturedPiece();

                            PieceBitBoards[capturedPiece].PopBit(capturedSquare);
                            OccupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                            UniqueIdentifier ^= ZobristTable.PieceHash(capturedSquare, capturedPiece);
                        }

                        break;
                    }
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[2] = OccupancyBitBoards[1] | OccupancyBitBoards[0];

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

            return new MakeMoveGameStateWithZobristKey(capturedPiece, castleCopy, enpassantCopy, uniqueIdentifierCopy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnmakeMove_WithZobristKey(Move move, MakeMoveGameStateWithZobristKey gameState)
        {
            var oppositeSide = (int)Side;
            Side = (Side)Utils.OppositeSide(Side);
            var offset = Utils.PieceOffset(Side);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[newPiece].PopBit(targetSquare);
            OccupancyBitBoards[(int)Side].PopBit(targetSquare);

            PieceBitBoards[piece].SetBit(sourceSquare);
            OccupancyBitBoards[(int)Side].SetBit(sourceSquare);

            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(OccupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                        $"Expected empty {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);
                }
                else
                {
                    PieceBitBoards[gameState.CapturedPiece].SetBit(targetSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
                }
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(Side);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(Side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookTargetSquare);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(Side);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(Side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[(int)Side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[(int)Side].PopBit(rookTargetSquare);
            }

            OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

            // Updating saved values
            Castle = gameState.Castle;
            EnPassant = gameState.EnPassant;
            UniqueIdentifier = gameState.ZobristKey;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnmakeMove_WithZobristKey_PreSwitchSpecialMove(Move move, MakeMoveGameStateWithZobristKey gameState)
        {
            var oppositeSide = (int)Side;
            var side = Utils.OppositeSide(oppositeSide);
            Side = (Side)side;
            var offset = Utils.PieceOffset(side);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[newPiece].PopBit(targetSquare);
            OccupancyBitBoards[side].PopBit(targetSquare);

            PieceBitBoards[piece].SetBit(sourceSquare);
            OccupancyBitBoards[side].SetBit(sourceSquare);

            if (move.IsCapture())
            {
                var oppositePawnIndex = (int)Piece.p - offset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(OccupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                        $"Expected empty {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);
                }
                else
                {
                    PieceBitBoards[move.CapturedPiece()].SetBit(targetSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
                }
            }
            else if (move.IsShortCastle())
            {
                var rookSourceSquare = Utils.ShortCastleRookSourceSquare(side);
                var rookTargetSquare = Utils.ShortCastleRookTargetSquare(side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[side].PopBit(rookTargetSquare);
            }
            else if (move.IsLongCastle())
            {
                var rookSourceSquare = Utils.LongCastleRookSourceSquare(side);
                var rookTargetSquare = Utils.LongCastleRookTargetSquare(side);
                var rookIndex = (int)Piece.R + offset;

                PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                OccupancyBitBoards[side].SetBit(rookSourceSquare);

                PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                OccupancyBitBoards[side].PopBit(rookTargetSquare);
            }

            OccupancyBitBoards[2] = OccupancyBitBoards[1] | OccupancyBitBoards[0];

            // Updating saved values
            Castle = gameState.Castle;
            EnPassant = gameState.EnPassant;
            UniqueIdentifier = gameState.ZobristKey;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnmakeMove_WithZobristKey_SwitchSpecialMove(Move move, MakeMoveGameStateWithZobristKey gameState)
        {
            var oppositeSide = (int)Side;
            var side = Utils.OppositeSide(oppositeSide);
            Side = (Side)side;
            var offset = Utils.PieceOffset(side);

            int sourceSquare = move.SourceSquare();
            int targetSquare = move.TargetSquare();
            int piece = move.Piece();
            int promotedPiece = move.PromotedPiece();

            var newPiece = piece;
            if (promotedPiece != default)
            {
                newPiece = promotedPiece;
            }

            PieceBitBoards[newPiece].PopBit(targetSquare);
            OccupancyBitBoards[side].PopBit(targetSquare);

            PieceBitBoards[piece].SetBit(sourceSquare);
            OccupancyBitBoards[side].SetBit(sourceSquare);

            switch (move.SpecialMoveFlag())
            {
                case SpecialMoveType.ShortCastle:
                    {
                        var rookSourceSquare = Utils.ShortCastleRookSourceSquare(side);
                        var rookTargetSquare = Utils.ShortCastleRookTargetSquare(side);
                        var rookIndex = (int)Piece.R + offset;

                        PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                        OccupancyBitBoards[side].SetBit(rookSourceSquare);

                        PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                        OccupancyBitBoards[side].PopBit(rookTargetSquare);

                        break;
                    }
                case SpecialMoveType.LongCastle:
                    {
                        var rookSourceSquare = Utils.LongCastleRookSourceSquare(side);
                        var rookTargetSquare = Utils.LongCastleRookTargetSquare(side);
                        var rookIndex = (int)Piece.R + offset;

                        PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                        OccupancyBitBoards[side].SetBit(rookSourceSquare);

                        PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                        OccupancyBitBoards[side].PopBit(rookTargetSquare);

                        break;
                    }
                case SpecialMoveType.EnPassant:
                    {
                        var oppositePawnIndex = (int)Piece.p - offset;

                        if (move.IsEnPassant())
                        {
                            var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                            Utils.Assert(OccupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                                $"Expected empty {capturedPawnSquare}");

                            PieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                            OccupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);
                        }

                        break;
                    }
                default:
                    {
                        if (move.IsCapture())
                        {
                            PieceBitBoards[move.CapturedPiece()].SetBit(targetSquare);
                            OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
                        }

                        break;
                    }
            }

            OccupancyBitBoards[2] = OccupancyBitBoards[1] | OccupancyBitBoards[0];

            // Updating saved values
            Castle = gameState.Castle;
            EnPassant = gameState.EnPassant;
            UniqueIdentifier = gameState.ZobristKey;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MakeNullMove()
        {
            Side = (Side)Utils.OppositeSide(Side);
            var oldEnPassant = EnPassant;
            EnPassant = BoardSquare.noSquare;

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.EnPassantHash((int)oldEnPassant);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnMakeNullMove(MakeMoveGameState gameState)
        {
            Side = (Side)Utils.OppositeSide(Side);
            EnPassant = gameState.EnPassant;

            UniqueIdentifier ^=
                ZobristTable.SideHash()
                ^ ZobristTable.EnPassantHash((int)EnPassant);
        }

        /// <summary>
        /// False if any of the kings has been captured, or if the opponent king is in check.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal readonly bool IsValid()
        {
            var kingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset(Side)];
            var kingSquare = kingBitBoard == default ? -1 : kingBitBoard.GetLS1BIndex();

            var oppositeKingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset((Side)Utils.OppositeSide(Side))];
            var oppositeKingSquare = oppositeKingBitBoard == default ? -1 : oppositeKingBitBoard.GetLS1BIndex();

            return kingSquare >= 0 && oppositeKingSquare >= 0
                && !MakeMoveAttacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
        }

        /// <summary>
        /// Lightweight version of <see cref="IsValid"/>
        /// False if the opponent king is in check.
        /// This method is meant to be invoked only after <see cref="Position(Position, Move)"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool WasProduceByAValidMove()
        {
            var oppositeKingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset((Side)Utils.OppositeSide(Side))];
            var oppositeKingSquare = oppositeKingBitBoard == default ? -1 : oppositeKingBitBoard.GetLS1BIndex();

            return oppositeKingSquare >= 0 && !MakeMoveAttacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<Move> AllPossibleMoves(Move[]? movePool = null) => MakeMoveMoveGenerator.GenerateAllMoves(this, movePool);
    }

    public readonly struct MakeMoveGameState
    {
        public readonly int CapturedPiece;

        public readonly byte Castle;

        public readonly BoardSquare EnPassant;

        public MakeMoveGameState(int capturedPiece, byte castle, BoardSquare enpassant)
        {
            CapturedPiece = capturedPiece;
            Castle = castle;
            EnPassant = enpassant;
        }
    }

    public readonly struct MakeMoveGameStateWithZobristKey
    {
        public readonly long ZobristKey;

        public readonly int CapturedPiece;

        public readonly BoardSquare EnPassant;

        public readonly byte Castle;

        public MakeMoveGameStateWithZobristKey(int capturedPiece, byte castle, BoardSquare enpassant, long zobristKey)
        {
            CapturedPiece = capturedPiece;
            Castle = castle;
            EnPassant = enpassant;
            ZobristKey = zobristKey;
        }
    }

    #region

    public static class MakeMoveZobristTable
    {
        private static readonly long[,] _table = Initialize();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PieceHash(int boardSquare, int piece) => _table[boardSquare, piece];

        /// <summary>
        /// Uses <see cref="Piece.P"/> and squares <see cref="BoardSquare.a1"/>-<see cref="BoardSquare.h1"/>
        /// </summary>
        /// <param name="enPassantSquare"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long EnPassantHash(int enPassantSquare)
        {
            if (enPassantSquare == (int)BoardSquare.noSquare)
            {
                return default;
            }

            var file = enPassantSquare % 8;

            return _table[file, (int)Piece.P];
        }

        /// <summary>
        /// Uses <see cref="Piece.p"/> and <see cref="BoardSquare.h8"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SideHash()
        {
            return _table[(int)BoardSquare.h8, (int)Piece.p];
        }

        /// <summary>
        /// Uses <see cref="Piece.p"/> and
        /// <see cref="BoardSquare.a8"/> for <see cref="CastlingRights.WK"/>, <see cref="BoardSquare.b8"/> for <see cref="CastlingRights.WQ"/>
        /// <see cref="BoardSquare.c8"/> for <see cref="CastlingRights.BK"/>, <see cref="BoardSquare.d8"/> for <see cref="CastlingRights.BQ"/>
        /// </summary>
        /// <param name="castle"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CastleHash(byte castle)
        {
            long combinedHash = 0;

            if ((castle & (int)CastlingRights.WK) != default)
            {
                combinedHash ^= _table[(int)BoardSquare.a8, (int)Piece.p];        // a8
            }

            if ((castle & (int)CastlingRights.WQ) != default)
            {
                combinedHash ^= _table[(int)BoardSquare.b8, (int)Piece.p];        // b8
            }

            if ((castle & (int)CastlingRights.BK) != default)
            {
                combinedHash ^= _table[(int)BoardSquare.c8, (int)Piece.p];        // c8
            }

            if ((castle & (int)CastlingRights.BQ) != default)
            {
                combinedHash ^= _table[(int)BoardSquare.d8, (int)Piece.p];        // d8
            }

            return combinedHash;
        }

        /// <summary>
        /// Calculates from scratch the hash of a position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PositionHash(MakeMovePosition position)
        {
            long positionHash = 0;

            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
                {
                    if (position.PieceBitBoards[pieceIndex].GetBit(squareIndex))
                    {
                        positionHash ^= PieceHash(squareIndex, pieceIndex);
                    }
                }
            }

            positionHash ^= EnPassantHash((int)position.EnPassant)
                ^ SideHash()
                ^ CastleHash(position.Castle);

            return positionHash;
        }

        /// <summary>
        /// Initializes Zobrist table (long[64, 12])
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long[,] Initialize()
        {
            var zobristTable = new long[64, 12];
            var randomInstance = new Random(int.MaxValue);

            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
                {
                    zobristTable[squareIndex, pieceIndex] = randomInstance.NextInt64();
                }
            }

            return zobristTable;
        }
    }

    public static class MakeMoveMoveGenerator
    {
        /// <summary>
        /// Indexed by <see cref="Piece"/>.
        /// Checks are not considered
        /// </summary>
        private static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks =
        [
            (int origin, BitBoard _) => MakeMoveAttacks.PawnAttacks[(int)Side.White][origin],
            (int origin, BitBoard _) => MakeMoveAttacks.KnightAttacks[origin],
            MakeMoveAttacks.BishopAttacks,
            MakeMoveAttacks.RookAttacks,
            MakeMoveAttacks.QueenAttacks,
            (int origin, BitBoard _) => MakeMoveAttacks.KingAttacks[origin],

            (int origin, BitBoard _) => MakeMoveAttacks.PawnAttacks[(int)Side.Black][origin],
            (int origin, BitBoard _) => MakeMoveAttacks.KnightAttacks[origin],
            MakeMoveAttacks.BishopAttacks,
            MakeMoveAttacks.RookAttacks,
            MakeMoveAttacks.QueenAttacks,
            (int origin, BitBoard _) => MakeMoveAttacks.KingAttacks[origin],
        ];

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="capturesOnly">Filters out all moves but captures</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Move> GenerateAllMoves(MakeMovePosition position, Move[]? movePool = null, bool capturesOnly = false)
        {
            movePool ??= new Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            int localIndex = 0;

            var offset = Utils.PieceOffset(position.Side);

            GeneratePawnMoves(ref localIndex, movePool, position, offset, capturesOnly);
            GenerateCastlingMoves(ref localIndex, movePool, position, offset);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.K + offset, position, capturesOnly);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position, capturesOnly);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position, capturesOnly);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position, capturesOnly);
            GeneratePieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position, capturesOnly);

            return movePool.Take(localIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GeneratePawnMoves(ref int localIndex, Move[] movePool, MakeMovePosition position, int offset, bool capturesOnly = false)
        {
            int sourceSquare, targetSquare;

            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece];

            while (bitboard != default)
            {
                sourceSquare = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                var sourceRank = (sourceSquare / 8) + 1;

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var targetRank = (singlePushSquare / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                        movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                        movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                        movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                    }
                    else if (!capturesOnly)
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
                    }

                    // Double pawn push
                    // Inside of the if because singlePush square cannot be occupied either
                    if (!capturesOnly)
                    {
                        var doublePushSquare = sourceSquare + (2 * pawnPush);
                        if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                            && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White)))
                        {
                            movePool[localIndex++] = MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece);
                        }
                    }
                }

                var attacks = MakeMoveAttacks.PawnAttacks[(int)position.Side][sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                {
                    movePool[localIndex++] = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece);
                }

                // Captures
                var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
                while (attackedSquares != default)
                {
                    targetSquare = attackedSquares.GetLS1BIndex();
                    attackedSquares.ResetLS1B();

                    var targetRank = (targetSquare / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                    {
                        movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset);
                        movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset);
                        movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset);
                        movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset);
                    }
                    else
                    {
                        movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece);
                    }
                }
            }
        }

        /// <summary>
        /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
        /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
        /// </summary>
        /// <param name="position"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GenerateCastlingMoves(ref int localIndex, Move[] movePool, MakeMovePosition position, int offset)
        {
            var piece = (int)Piece.K + offset;
            var oppositeSide = (Side)Utils.OppositeSide(position.Side);

            int sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

            // Castles
            if (position.Castle != default)
            {
                if (position.Side == Side.White)
                {
                    bool ise1Attacked = MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.e1, position, oppositeSide);
                    if (((position.Castle & (int)CastlingRights.WK) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                        && !ise1Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.EncodeShortCastle(sourceSquare, Constants.WhiteShortCastleKingSquare, piece);
                    }

                    if (((position.Castle & (int)CastlingRights.WQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                        && !ise1Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.EncodeLongCastle(sourceSquare, Constants.WhiteLongCastleKingSquare, piece);
                    }
                }
                else
                {
                    bool ise8Attacked = MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.e8, position, oppositeSide);
                    if (((position.Castle & (int)CastlingRights.BK) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                        && !ise8Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.EncodeShortCastle(sourceSquare, Constants.BlackShortCastleKingSquare, piece);
                    }

                    if (((position.Castle & (int)CastlingRights.BQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                        && !ise8Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.EncodeLongCastle(sourceSquare, Constants.BlackLongCastleKingSquare, piece);
                    }
                }
            }
        }

        /// <summary>
        /// Generate Knight, Bishop, Rook and Queen moves
        /// </summary>
        /// <param name="piece"><see cref="Piece"/></param>
        /// <param name="position"></param>
        /// <param name="capturesOnly"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GeneratePieceMoves(ref int localIndex, Move[] movePool, int piece, MakeMovePosition position, bool capturesOnly = false)
        {
            var bitboard = position.PieceBitBoards[piece];
            int sourceSquare, targetSquare;

            while (bitboard != default)
            {
                sourceSquare = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                    & ~position.OccupancyBitBoards[(int)position.Side];

                while (attacks != default)
                {
                    targetSquare = attacks.GetLS1BIndex();
                    attacks.ResetLS1B();

                    if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                    {
                        movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece: 1);
                    }
                    else if (!capturesOnly)
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece);
                    }
                }
            }
        }
    }

    public static class MakeMoveAttacks
    {
        private static readonly BitBoard[] _bishopOccupancyMasks;
        private static readonly BitBoard[] _rookOccupancyMasks;

        /// <summary>
        /// [64 (Squares), 512 (Occupancies)]
        /// Use <see cref="BishopAttacks(int, BitBoard)"/>
        /// </summary>
        private static readonly BitBoard[][] _bishopAttacks;

        /// <summary>
        /// [64 (Squares), 4096 (Occupancies)]
        /// Use <see cref="RookAttacks(int, BitBoard)"/>
        /// </summary>
        private static readonly BitBoard[][] _rookAttacks;

        /// <summary>
        /// [2 (B|W), 64 (Squares)]
        /// </summary>
        public static BitBoard[][] PawnAttacks { get; }
        public static BitBoard[] KnightAttacks { get; }
        public static BitBoard[] KingAttacks { get; }

        static MakeMoveAttacks()
        {
            KingAttacks = AttackGenerator.InitializeKingAttacks();
            PawnAttacks = AttackGenerator.InitializePawnAttacks();
            KnightAttacks = AttackGenerator.InitializeKnightAttacks();

            (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopMagicAttacks();
            (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookMagicAttacks();
        }

        /// <summary>
        /// Get Bishop attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard BishopAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy & _bishopOccupancyMasks[squareIndex];
            occ *= Constants.BishopMagicNumbers[squareIndex];
            occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

            return _bishopAttacks[squareIndex][occ];
        }

        /// <summary>
        /// Get Rook attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard RookAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy & _rookOccupancyMasks[squareIndex];
            occ *= Constants.RookMagicNumbers[squareIndex];
            occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

            return _rookAttacks[squareIndex][occ];
        }

        /// <summary>
        /// Get Queen attacks assuming current board occupancy
        /// Use <see cref="QueenAttacks(BitBoard, BitBoard)"/> if rook and bishop attacks are already calculated
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard QueenAttacks(int squareIndex, BitBoard occupancy)
        {
            return QueenAttacks(
                RookAttacks(squareIndex, occupancy),
                BishopAttacks(squareIndex, occupancy));
        }

        /// <summary>
        /// Get Queen attacks having rook and bishop attacks pre-calculated
        /// </summary>
        /// <param name="rookAttacks"></param>
        /// <param name="bishopAttacks"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard QueenAttacks(BitBoard rookAttacks, BitBoard bishopAttacks)
        {
            return rookAttacks | bishopAttacks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSquaredAttackedBySide(int squaredIndex, MakeMovePosition position, Side sideToMove) =>
            IsSquaredAttacked(squaredIndex, sideToMove, position.PieceBitBoards, position.OccupancyBitBoards);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSquaredAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            Utils.Assert(sideToMove != Side.Both);

            var offset = Utils.PieceOffset(sideToMove);

            // I tried to order them from most to least likely
            return
                IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition)
                || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
                || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
                || IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
                || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
                || IsSquareAttackedByKing(squareIndex, offset, piecePosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSquareInCheck(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            Utils.Assert(sideToMove != Side.Both);

            var offset = Utils.PieceOffset(sideToMove);

            // I tried to order them from most to least likely
            return
                IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
                || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
                || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
                || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
                || IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByPawns(int squareIndex, Side sideToMove, int offset, BitBoard[] pieces)
        {
            var oppositeColorIndex = ((int)sideToMove + 1) % 2;

            return (PawnAttacks[oppositeColorIndex][squareIndex] & pieces[offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard bishopAttacks)
        {
            bishopAttacks = BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard rookAttacks)
        {
            rookAttacks = RookAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard[] piecePosition)
        {
            var queenAttacks = QueenAttacks(rookAttacks, bishopAttacks);
            return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
        }
    }

    #endregion
}

#pragma warning restore S101, S1854 // Types should be named in PascalCase
