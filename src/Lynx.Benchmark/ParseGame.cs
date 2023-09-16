/*
 * Using Span.Split()
 *
 *  BenchmarkDotNet v0.13.8, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method              | positionCommand      | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------- |--------------------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | ParseGame_Original  | position startpos    |   6.871 us | 0.1247 us | 0.1167 us |  1.00 |    0.00 |  1.6251 | 0.1450 |   29.8 KB |        1.00 |
 *  | ParseGame_Improved1 | position startpos    |   7.054 us | 0.1380 us | 0.1534 us |  1.03 |    0.03 |  1.6251 | 0.1602 |  29.68 KB |        1.00 |
 *  | ParseGame_Improved2 | position startpos    |   7.146 us | 0.1382 us | 0.1845 us |  1.03 |    0.03 |  1.6251 | 0.1602 |  29.68 KB |        1.00 |
 *  | ParseGame_Improved3 | position startpos    |   6.894 us | 0.1260 us | 0.2305 us |  1.04 |    0.04 |  1.6251 | 0.1450 |   29.8 KB |        1.00 |
 *  | ParseGame_Improved4 | position startpos    |   7.564 us | 0.1501 us | 0.1668 us |  1.10 |    0.03 |  1.6174 | 0.1526 |  29.77 KB |        1.00 |
 *  |                     |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)b7b6 [193]  |  34.873 us | 0.5100 us | 0.4770 us |  1.00 |    0.00 |  2.1973 | 0.1831 |  40.55 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)b7b6 [193]  |  34.928 us | 0.4407 us | 0.4122 us |  1.00 |    0.02 |  2.1362 | 0.1831 |  39.07 KB |        0.96 |
 *  | ParseGame_Improved2 | posi(...)b7b6 [193]  |  33.461 us | 0.5829 us | 0.5453 us |  0.96 |    0.02 |  2.0142 | 0.1831 |  37.06 KB |        0.91 |
 *  | ParseGame_Improved3 | posi(...)b7b6 [193]  |  34.852 us | 0.4138 us | 0.3668 us |  1.00 |    0.02 |  2.0752 | 0.1831 |  38.54 KB |        0.95 |
 *  | ParseGame_Improved4 | posi(...)b7b6 [193]  |  35.040 us | 0.5429 us | 0.5078 us |  1.00 |    0.02 |  2.0142 | 0.1831 |  36.94 KB |        0.91 |
 *  |                     |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)f3g3 [353]  |  55.621 us | 0.7694 us | 0.7197 us |  1.00 |    0.00 |  2.7466 | 0.2441 |  50.37 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)f3g3 [353]  |  53.732 us | 0.4313 us | 0.4035 us |  0.97 |    0.02 |  2.5635 | 0.2441 |  47.63 KB |        0.95 |
 *  | ParseGame_Improved2 | posi(...)f3g3 [353]  |  55.343 us | 0.6590 us | 0.5842 us |  0.99 |    0.02 |  2.3804 | 0.1831 |  43.81 KB |        0.87 |
 *  | ParseGame_Improved3 | posi(...)f3g3 [353]  |  55.508 us | 0.5819 us | 0.5443 us |  1.00 |    0.01 |  2.5024 | 0.2441 |  46.55 KB |        0.92 |
 *  | ParseGame_Improved4 | posi(...)f3g3 [353]  |  54.922 us | 0.5243 us | 0.4905 us |  0.99 |    0.01 |  2.3804 | 0.1831 |  43.69 KB |        0.87 |
 *  |                     |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)g4g8 [979]  | 131.549 us | 2.3017 us | 2.1530 us |  1.00 |    0.00 |  4.6387 | 0.2441 |  88.81 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)g4g8 [979]  | 127.214 us | 1.5846 us | 1.4822 us |  0.97 |    0.02 |  4.3945 | 0.4883 |   81.2 KB |        0.91 |
 *  | ParseGame_Improved2 | posi(...)g4g8 [979]  | 125.834 us | 1.0297 us | 0.9128 us |  0.96 |    0.02 |  3.6621 | 0.2441 |  70.29 KB |        0.79 |
 *  | ParseGame_Improved3 | posi(...)g4g8 [979]  | 129.836 us | 1.3107 us | 1.2260 us |  0.99 |    0.02 |  4.1504 | 0.2441 |  77.91 KB |        0.88 |
 *  | ParseGame_Improved4 | posi(...)g4g8 [979]  | 124.132 us | 0.4781 us | 0.4238 us |  0.94 |    0.01 |  3.6621 | 0.2441 |  70.18 KB |        0.79 |
 *  |                     |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)h3f1 [2984] | 359.558 us | 5.2840 us | 4.9426 us |  1.00 |    0.00 | 11.2305 | 1.4648 |  211.8 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)h3f1 [2984] | 347.574 us | 3.6558 us | 3.4196 us |  0.97 |    0.02 | 10.2539 | 1.4648 | 188.51 KB |        0.89 |
 *  | ParseGame_Improved2 | posi(...)h3f1 [2984] | 331.351 us | 3.1412 us | 2.9382 us |  0.92 |    0.02 |  8.3008 | 0.4883 |  154.9 KB |        0.73 |
 *  | ParseGame_Improved3 | posi(...)h3f1 [2984] | 345.972 us | 2.1860 us | 1.8254 us |  0.96 |    0.01 |  9.2773 | 0.4883 | 178.18 KB |        0.84 |
 *  | ParseGame_Improved4 | posi(...)h3f1 [2984] | 316.134 us | 1.9524 us | 1.7308 us |  0.88 |    0.01 |  8.3008 | 0.4883 | 154.78 KB |        0.73 |
 *
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method              | positionCommand      | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------- |--------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | ParseGame_Original  | position startpos    |   7.307 us | 0.1456 us | 0.4224 us |   7.187 us |  1.00 |    0.00 | 1.1597 | 0.0992 |   29.8 KB |        1.00 |
 *  | ParseGame_Improved1 | position startpos    |   8.015 us | 0.1632 us | 0.4812 us |   8.101 us |  1.10 |    0.09 | 1.1597 | 0.0992 |  29.68 KB |        1.00 |
 *  | ParseGame_Improved2 | position startpos    |   7.846 us | 0.1505 us | 0.1546 us |   7.855 us |  1.10 |    0.06 | 1.1597 | 0.0992 |  29.68 KB |        1.00 |
 *  | ParseGame_Improved3 | position startpos    |   7.621 us | 0.1422 us | 0.1397 us |   7.603 us |  1.07 |    0.04 | 1.1597 | 0.0992 |   29.8 KB |        1.00 |
 *  | ParseGame_Improved4 | position startpos    |   8.560 us | 0.1703 us | 0.3947 us |   8.543 us |  1.16 |    0.10 | 1.1597 | 0.1068 |  29.77 KB |        1.00 |
 *  |                     |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseGame_Original  | posi(...)b7b6 [193]  |  38.037 us | 0.7471 us | 1.2275 us |  38.141 us |  1.00 |    0.00 | 1.5869 | 0.1221 |  40.55 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)b7b6 [193]  |  37.127 us | 0.6398 us | 0.5985 us |  37.047 us |  1.00 |    0.03 | 1.5259 | 0.1221 |  39.07 KB |        0.96 |
 *  | ParseGame_Improved2 | posi(...)b7b6 [193]  |  35.052 us | 0.6982 us | 1.0014 us |  34.867 us |  0.93 |    0.05 | 1.4038 | 0.1221 |  37.06 KB |        0.91 |
 *  | ParseGame_Improved3 | posi(...)b7b6 [193]  |  34.950 us | 0.6290 us | 0.6459 us |  34.797 us |  0.94 |    0.03 | 1.4648 | 0.1221 |  38.54 KB |        0.95 |
 *  | ParseGame_Improved4 | posi(...)b7b6 [193]  |  35.013 us | 0.6706 us | 1.0637 us |  35.186 us |  0.92 |    0.05 | 1.4038 | 0.1221 |  36.94 KB |        0.91 |
 *  |                     |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseGame_Original  | posi(...)f3g3 [353]  |  57.933 us | 1.1546 us | 2.0222 us |  58.354 us |  1.00 |    0.00 | 1.9531 | 0.1831 |  50.37 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)f3g3 [353]  |  54.611 us | 0.9854 us | 0.8736 us |  54.500 us |  0.97 |    0.03 | 1.8311 | 0.1831 |  47.63 KB |        0.95 |
 *  | ParseGame_Improved2 | posi(...)f3g3 [353]  |  53.850 us | 1.0766 us | 1.6441 us |  54.208 us |  0.93 |    0.04 | 1.7090 | 0.1221 |  43.81 KB |        0.87 |
 *  | ParseGame_Improved3 | posi(...)f3g3 [353]  |  54.731 us | 1.0093 us | 1.2015 us |  54.825 us |  0.95 |    0.05 | 1.7700 | 0.1221 |  46.54 KB |        0.92 |
 *  | ParseGame_Improved4 | posi(...)f3g3 [353]  |  57.566 us | 1.1010 us | 1.1780 us |  57.754 us |  1.01 |    0.03 | 1.7090 | 0.1221 |  43.69 KB |        0.87 |
 *  |                     |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseGame_Original  | posi(...)g4g8 [979]  | 128.079 us | 2.4266 us | 2.1511 us | 128.147 us |  1.00 |    0.00 | 3.4180 | 0.2441 |  88.81 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)g4g8 [979]  | 123.052 us | 2.4402 us | 3.3402 us | 122.040 us |  0.95 |    0.02 | 3.1738 | 0.3662 |  81.19 KB |        0.91 |
 *  | ParseGame_Improved2 | posi(...)g4g8 [979]  | 118.292 us | 2.3150 us | 3.8036 us | 118.165 us |  0.93 |    0.04 | 2.6855 | 0.2441 |  70.29 KB |        0.79 |
 *  | ParseGame_Improved3 | posi(...)g4g8 [979]  | 123.003 us | 1.9221 us | 1.7039 us | 123.349 us |  0.96 |    0.02 | 2.9297 | 0.2441 |  77.91 KB |        0.88 |
 *  | ParseGame_Improved4 | posi(...)g4g8 [979]  | 119.522 us | 1.2756 us | 1.1307 us | 119.621 us |  0.93 |    0.02 | 2.6855 | 0.2441 |  70.17 KB |        0.79 |
 *  |                     |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseGame_Original  | posi(...)h3f1 [2984] | 371.790 us | 7.1718 us | 8.2591 us | 372.192 us |  1.00 |    0.00 | 7.8125 | 0.9766 | 211.79 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)h3f1 [2984] | 352.763 us | 6.8505 us | 7.3300 us | 352.608 us |  0.95 |    0.03 | 7.3242 | 0.9766 |  188.5 KB |        0.89 |
 *  | ParseGame_Improved2 | posi(...)h3f1 [2984] | 319.417 us | 4.5244 us | 3.7781 us | 318.915 us |  0.86 |    0.02 | 5.8594 | 0.4883 | 154.89 KB |        0.73 |
 *  | ParseGame_Improved3 | posi(...)h3f1 [2984] | 333.104 us | 5.6645 us | 8.6503 us | 331.891 us |  0.90 |    0.03 | 6.8359 | 0.4883 | 178.17 KB |        0.84 |
 *  | ParseGame_Improved4 | posi(...)h3f1 [2984] | 324.352 us | 6.1629 us | 5.1463 us | 325.998 us |  0.87 |    0.03 | 5.8594 | 0.4883 | 154.77 KB |        0.73 |
 *
 *
 *  BenchmarkDotNet v0.13.8, macOS Monterey 12.6.8 (21G725) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *
 *
 *  | Method              | positionCommand      | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------- |--------------------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | ParseGame_Original  | position startpos    |   5.854 us | 0.1140 us | 0.1170 us |  1.00 |    0.00 |  4.8523 | 0.1755 |  29.81 KB |        1.00 |
 *  | ParseGame_Improved1 | position startpos    |   6.305 us | 0.1791 us | 0.5253 us |  1.15 |    0.07 |  4.8370 | 0.2823 |  29.69 KB |        1.00 |
 *  | ParseGame_Improved2 | position startpos    |   5.935 us | 0.1101 us | 0.1543 us |  1.01 |    0.04 |  4.8370 | 0.2823 |  29.69 KB |        1.00 |
 *  | ParseGame_Improved3 | position startpos    |   6.076 us | 0.1208 us | 0.1240 us |  1.04 |    0.03 |  4.8523 | 0.1755 |  29.81 KB |        1.00 |
 *  | ParseGame_Improved4 | position startpos    |   6.337 us | 0.0460 us | 0.0384 us |  1.08 |    0.02 |  4.8523 | 0.3662 |  29.78 KB |        1.00 |
 *  |                     |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)b7b6 [193]  |  32.301 us | 0.6213 us | 0.7630 us |  1.00 |    0.00 |  6.5918 | 0.3357 |  40.57 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)b7b6 [193]  |  31.763 us | 0.4178 us | 0.3908 us |  0.98 |    0.02 |  6.3477 | 0.4883 |  39.08 KB |        0.96 |
 *  | ParseGame_Improved2 | posi(...)b7b6 [193]  |  30.856 us | 0.6057 us | 0.7211 us |  0.96 |    0.03 |  5.9814 | 0.3052 |  37.07 KB |        0.91 |
 *  | ParseGame_Improved3 | posi(...)b7b6 [193]  |  30.243 us | 0.2401 us | 0.2128 us |  0.94 |    0.02 |  6.2866 | 0.3052 |  38.56 KB |        0.95 |
 *  | ParseGame_Improved4 | posi(...)b7b6 [193]  |  28.548 us | 0.2012 us | 0.1784 us |  0.88 |    0.02 |  6.0120 | 0.2747 |  36.96 KB |        0.91 |
 *  |                     |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)f3g3 [353]  |  48.956 us | 0.3678 us | 0.3261 us |  1.00 |    0.00 |  8.1787 | 0.6714 |  50.38 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)f3g3 [353]  |  46.983 us | 0.3641 us | 0.3040 us |  0.96 |    0.01 |  7.7515 | 0.6714 |  47.65 KB |        0.95 |
 *  | ParseGame_Improved2 | posi(...)f3g3 [353]  |  45.676 us | 0.3849 us | 0.3412 us |  0.93 |    0.01 |  7.1411 | 0.6104 |  43.83 KB |        0.87 |
 *  | ParseGame_Improved3 | posi(...)f3g3 [353]  |  48.842 us | 0.9442 us | 1.0874 us |  0.99 |    0.03 |  7.5684 | 0.5493 |  46.56 KB |        0.92 |
 *  | ParseGame_Improved4 | posi(...)f3g3 [353]  |  49.210 us | 0.8985 us | 0.9986 us |  1.00 |    0.02 |  7.0801 | 0.4272 |  43.71 KB |        0.87 |
 *  |                     |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)g4g8 [979]  | 120.794 us | 1.0616 us | 0.9930 us |  1.00 |    0.00 | 14.4043 | 1.4648 |  88.85 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)g4g8 [979]  | 119.473 us | 2.2270 us | 2.1872 us |  0.99 |    0.02 | 13.1836 | 1.0986 |  81.23 KB |        0.91 |
 *  | ParseGame_Improved2 | posi(...)g4g8 [979]  | 107.950 us | 1.0335 us | 0.9667 us |  0.89 |    0.01 | 11.3525 | 0.8545 |  70.32 KB |        0.79 |
 *  | ParseGame_Improved3 | posi(...)g4g8 [979]  | 117.105 us | 1.4753 us | 1.2320 us |  0.97 |    0.02 | 12.6953 | 0.9766 |  77.94 KB |        0.88 |
 *  | ParseGame_Improved4 | posi(...)g4g8 [979]  | 107.548 us | 1.3435 us | 1.1910 us |  0.89 |    0.01 | 11.3525 | 0.8545 |  70.21 KB |        0.79 |
 *  |                     |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)h3f1 [2984] | 308.550 us | 5.7367 us | 5.3661 us |  1.00 |    0.00 | 34.1797 | 3.9063 | 211.88 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)h3f1 [2984] | 294.975 us | 4.6836 us | 4.3811 us |  0.96 |    0.02 | 30.7617 | 4.3945 | 188.59 KB |        0.89 |
 *  | ParseGame_Improved2 | posi(...)h3f1 [2984] | 290.182 us | 3.2207 us | 3.0126 us |  0.94 |    0.02 | 24.9023 | 1.9531 | 154.96 KB |        0.73 |
 *  | ParseGame_Improved3 | posi(...)h3f1 [2984] | 303.700 us | 5.9219 us | 4.6234 us |  0.98 |    0.03 | 28.8086 | 1.9531 | 178.25 KB |        0.84 |
 *  | ParseGame_Improved4 | posi(...)h3f1 [2984] | 275.244 us | 2.4260 us | 2.2693 us |  0.89 |    0.02 | 24.9023 | 1.9531 | 154.84 KB |        0.73 |
 *
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark;
public partial class ParseGameBenchmark : BaseBenchmark
{
    public static IEnumerable<string> Data => new[] {
        "position startpos",    // No moves
        "position startpos moves d2d4 d7d5 b1c3 g8f6 f2f3 c7c5 e2e3 b8c6 a2a3 e7e6 f1e2 c8d7 d4c5 f8c5 h2h3 f6h5 h1h2 d8h4 e1d2 h4f2 c3d5 e6d5 d2c3 c5e3 c1d2 c6a5 a3a4 e3d4 c3b4 d4c5 b4a5 a7a6 a1a2 b7b6",    // 17 moves
        "position startpos moves d2d4 d7d5 g1f3 g8f6 e2e3 b8c6 f1e2 c8f5 b1c3 f6e4 e1g1 e4c3 b2c3 e7e6 a1b1 a8b8 c1b2 b7b5 f3e5 c6e5 d4e5 f8c5 e2f3 e8g8 f1e1 d8h4 g2g4 f5g6 b1c1 f7f6 e5f6 h4f6 g1g2 g6e4 f3e4 f6f2 g2h1 d5e4 d1e2 f2f3 e2f3 e4f3 e3e4 f3f2 e1f1 f8f4 c1d1 f4e4 g4g5 e4g4 b2c1 b8f8 a2a3 e6e5 h2h3 g4g3 c1b2 f8f3 d1d8 g8f7 d8d7 f7e6 d7c7 g3h3 h1g2 f3g3",    // 36 moves
        "position startpos moves g1f3 e7e6 e2e4 b7b6 d2d4 g8f6 e4e5 f6d5 c2c4 f8b4 b1d2 d5e7 a2a3 b4d2 c1d2 d7d6 d2c3 d6e5 f3e5 c8b7 f1e2 e8g8 e2f3 b7f3 d1f3 b8d7 a1d1 f7f6 e5c6 e7c6 f3c6 f8e8 e1g1 d7f8 f1e1 d8d7 c6f3 f8g6 f3b7 a7a5 c4c5 a8b8 b7e4 d7b5 d4d5 e6e5 d5d6 b5c5 d6d7 e8d8 e1e3 g6f4 g2g3 f4h3 g1g2 h3g5 e4a4 g5f7 e3d3 f7d6 d3d5 c5c4 a4c4 d6c4 b2b4 a5a4 d1c1 c4d6 c3e5 d8d7 e5d6 d7d6 d5d6 c7d6 c1c6 d6d5 c6d6 b8c8 d6d5 c8c3 b4b5 c3b3 d5d4 b3a3 d4d6 a3a1 d6b6 a4a3 b6b8 g8f7 b5b6 a1b1 b8a8 b1b6 a8a3 b6b8 a3a7 f7g8 h2h4 b8e8 g3g4 e8d8 g2f3 d8d3 f3e4 d3d8 e4f4 d8d4 f4g3 d4d8 h4h5 d8f8 g3f4 f8b8 f4f3 b8b3 f3g2 b3b8 a7c7 b8a8 g2f3 a8a3 f3f4 a3a4 f4g3 a4a8 c7b7 a8f8 g3f4 f8c8 f4e4 c8c4 e4f3 c4c3 f3g2 c3c8 f2f4 c8c2 g2f3 c2c3 f3e4 c3c4 e4f5 c4c5 f5e6 c5c6 e6d5 c6c8 h5h6 g7h6 d5e6 c8c4 e6f5 c4c6 b7a7 c6d6 a7c7 d6d4 c7b7 d4a4 b7e7 a4a6 e7e6 a6e6 f5e6 g8g7 f4f5 h6h5 g4h5 h7h6 e6e7 g7g8 e7f6 g8f8 f6e6 f8g7 e6e7 g7h8 f5f6 h8g8 f6f7 g8h8 f7f8Q h8h7 f8f4 h7g8 f4g4 g8h8 e7f8 h8h7 g4g8",  // 96 movws
        "position startpos moves d2d4 g8f6 g1f3 d7d5 b1c3 e7e6 g2g3 c7c5 e2e3 c5c4 f1g2 e6e5 d4e5 f6e4 d1d5 e4c3 d5d8 e8d8 b2c3 b8c6 c1b2 a7a6 e1c1 d8c7 d1e1 a6a5 e3e4 a5a4 a2a3 b7b6 f3d2 b6b5 f2f4 h8g8 d2f3 c8g4 f3g5 f8c5 g5h7 g8h8 h7g5 f7f6 e5f6 g7f6 g5f3 a8b8 f3d4 g4d7 h2h4 h8g8 e1e3 b5b4 a3b4 a4a3 b2a3 c6d4 b4c5 g8g3 e3g3 d4e2 c1d2 e2g3 h1e1 d7c6 a3b4 b8h8 b4a5 c7d7 e1e3 h8g8 d2e1 f6f5 g2h3 d7e7 e4f5 g3e4 e1f1 e7f7 a5b6 g8h8 h3g2 h8h4 b6c7 e4d2 f1g1 h4g4 e3e2 d2f3 g1f1 f3h2 f1g1 h2f3 g1f2 g4g2 f2g2 f3d4 g2f2 d4e2 f2e2 f7f6 e2e3 f6f5 c7e5 f5e6 e3d4 c6f3 d4c4 f3e2 c4d4 e2b5 c3c4 b5e8 d4d3 e8g6 d3c3 e6d7 c3b3 g6e4 c2c3 d7c6 e5d6 c6d7 d6e5 d7c6 b3b4 e4b1 e5d4 b1c2 b4a5 c2f5 a5b4 f5d3 d4e3 d3b1 e3d4 b1e4 d4h8 e4g6 h8g7 g6h7 g7f8 h7e4 f8g7 e4c2 g7f8 c2h7 f8d6 h7d3 d6b8 d3h7 b8e5 h7b1 e5h8 b1e4 h8d4 e4c2 d4e3 c2f5 b4b3 f5g6 b3b4 g6c2 e3c1 c2e4 c1a3 e4g6 a3c1 g6b1 c1e3 b1d3 e3c1 d3b1 c1a3 b1c2 a3b2 c2f5 b2a3 f5d3 b4b3 d3f5 b3a4 f5c2 a4a5 c2f5 a5a4 f5c2 a4a5 c2g6 a5b4 g6c2 a3b2 c2f5 b4b3 c6c5 b2a3 c5c6 b3a4 f5c2 a4a5 c2d3 a5b4 d3f5 a3b2 f5c2 c4c5 c2d3 c3c4 d3f5 b2a3 f5h3 b4c3 h3e6 c3d4 c6c7 a3b2 c7d7 d4c3 e6g4 c3d4 d7e6 d4e3 g4h3 e3f2 h3f5 f2e3 f5h3 b2e5 e6d7 e5d6 h3f5 d6b8 d7c6 b8d6 c6d7 d6b8 d7c6 e3d4 f5e6 b8e5 e6f5 e5d6 f5e6 d6f8 c6c7 f8e7 c7c6 e7d6 c6d7 d6e5 e6h3 d4c3 h3g2 f4f5 g2e4 f5f6 e4g6 c3d4 g6f7 e5d6 d7e6 d6e7 e6d7 d4c3 f7g8 e7d6 g8f7 c3d4 d7e6 d6e5 e6d7 d4d3 d7c6 e5d6 c6d7 d6e5 f7g6 d3e3 g6f7 e3d4 f7g8 d4d3 d7c6 e5d6 c6d7 d3d4 g8f7 d6e7 f7e6 d4c3 e6g8 c3b4 d7c6 e7d6 g8f7 d6e5 f7e6 e5b2 e6f7 b2d4 f7e8 d4c3 c6b7 c3b2 b7c6 b2d4 c6c7 d4e5 c7c6 e5d4 e8f7 d4e5 f7g6 e5b2 g6f7 b2c3 f7h5 c3d4 h5e8 b4b3 e8h5 b3c3 h5e8 c3b2 e8h5 b2c3 h5g6 c3d2 g6f7 d2c3 f7g8 c3b3 g8f7 d4f2 c6d7 b3b4 d7c6 b4c3 f7e6 c3d4 c6d7 f2g3 e6f7 g3e5 d7c6 e5d6 f7e6 d6e7 e6f7 e7d6 f7g8 d6e5 g8e6 f6f7 e6f7 e5f4 f7h5 f4d6 h5g4 d6e5 g4h3 e5d6 c6b7 d6f4 h3f1 f4e5 f1e2 d4d5 e2d1 c5c6 b7c8 c4c5 d1e2 d5d6 e2f3 e5h8 f3d1 h8b2 d1f3 b2e5 f3e4 e5b2 e4h1 b2h8 h1f3 h8e5 f3g4 e5b2 g4h5 b2d4 h5g6 d4b2 g6h5 c6c7 h5f3 b2e5 f3g2 e5b2 g2f3 b2g7 f3h1 g7e5 h1g2 e5d4 g2c6 d4h8 c6f3 h8b2 f3d1 b2e5 d1e2 d6e6 e2b5 e6d6 b5e2 d6e6 e2f3 e6f5 f3b7 f5f6 b7f3 f6e6 f3c6 e6f5 c6g2 f5e6 g2c6 e6f6 c6d7 f6g5 d7h3 g5f6 h3g2 f6f5 g2b7 f5f4 c8d7 f4e3 d7c6 e3d4 b7c8 e5d6 c8b7 d4c4 b7a6 c4d4 a6b7 d6e5 c6d7 d4c4 b7a6 c4d4 a6b7 d4c4 d7c6 e5f4 b7a6 c4d4 a6c8 f4e5 c8b7 e5d6 c6d7 d6e5 d7c6 e5d6 c6d7 d6g3 d7c6 g3f4 b7c8 f4h2 c8b7 h2g3 c6d7 d4c4 d7c6 c4d4 c6d7 g3f4 d7c6 f4h2 c6d7 h2f4 d7c6 d4c4 b7a6 c4b4 a6c8 b4c3 c8a6 c7c8Q a6c8 c3d4 c8b7 d4c4 b7c8 c4d4 c8h3 f4c1 h3f1 c1a3 f1a6 a3b2 a6c8 b2a3 c8h3 d4e5 h3d7 e5d4 d7c8 a3b2 c8d7 d4c4 d7e6 c4d4 e6g4 b2a3 g4c8 a3b2 c8e6 b2c3 e6c8 d4c4 c8a6 c4d4 a6b7 d4c4 b7c8 c3b2 c8h3 b2a1 h3e6 c4d4 e6d5 a1c3 d5g2 d4c4 g2f1 c4d4 f1a6 c3d2 a6f1 d2f4 f1h3 f4h6 h3f1 h6g5 f1h3 d4c4 h3f1 c4d4 f1h3 g5f4 h3f1 f4e5 f1e2 e5d6 e2b5 d6e5 b5f1 e5b8 f1b5 b8d6 b5f1 d6e5 f1a6 e5f4 a6e2 f4e5 e2g4 e5d6 g4e6 d6e5 e6h3 e5f4 h3g2 f4d2 g2d5 d2f4 d5f7 f4e5 f7a2 e5d6 a2g8 d6e7 g8e6 e7d6 e6d7 d4e5 d7h3 e5f6 h3f1", // 296 moves
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public Game ParseGame_Original(string positionCommand) => ParseGame_OriginalClass.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Game ParseGame_Improved1(string positionCommand) => ParseGame_ImprovedClass1.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Game ParseGame_Improved2(string positionCommand) => ParseGame_ImprovedClass2.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Game ParseGame_Improved3(string positionCommand) => ParseGame_ImprovedClass3.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Game ParseGame_Improved4(string positionCommand) => ParseGame_ImprovedClass4.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Game ParseGame_Improved5(string positionCommand) => ParseGame_ImprovedClass5.ParseGame(positionCommand);

    public static partial class ParseGame_OriginalClass
    {
        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        [GeneratedRegex("(?<=fen).+?(?=moves|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex FenRegex();

        [GeneratedRegex("(?<=moves).+?(?=$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex MovesRegex();

        private static readonly Regex _fenRegex = FenRegex();
        private static readonly Regex _movesRegex = MovesRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Game ParseGame(string positionCommand)
        {
            try
            {
                var items = positionCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                bool isInitialPosition = string.Equals(items.ElementAtOrDefault(1), StartPositionString, StringComparison.OrdinalIgnoreCase);

                var initialPosition = isInitialPosition
                        ? Constants.InitialPositionFEN
                        : _fenRegex.Match(positionCommand).Value.Trim();

                if (string.IsNullOrEmpty(initialPosition))
                {
                    _logger.Error("Error parsing position command '{0}': no initial position found", positionCommand);
                }

                var moves = _movesRegex.Match(positionCommand).Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable CS0618 // Type or member is obsolete
                return new Game(initialPosition, moves);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                return new Game();
            }
        }
    }

    public static partial class ParseGame_ImprovedClass1
    {
        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        [GeneratedRegex("(?<=fen).+?(?=moves|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex FenRegex();

        [GeneratedRegex("(?<=moves).+?(?=$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex MovesRegex();

        private static readonly Regex _fenRegex = FenRegex();
        private static readonly Regex _movesRegex = MovesRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Game ParseGame(string positionCommand)
        {
            try
            {
                var positionCommandSpan = positionCommand.AsSpan();
                Span<Range> items = stackalloc Range[3];    // Leaving 'everything else' in the third one
                positionCommandSpan.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries);
                bool isInitialPosition = positionCommandSpan[items[1]].Equals(StartPositionString, StringComparison.OrdinalIgnoreCase);

                var initialPosition = isInitialPosition
                        ? Constants.InitialPositionFEN
                        : _fenRegex.Match(positionCommand).Value.Trim();

                if (string.IsNullOrEmpty(initialPosition))
                {
                    _logger.Error("Error parsing position command '{0}': no initial position found", positionCommand);
                }

                var moves = _movesRegex.Match(positionCommand).Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                return new Game(initialPosition, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                return new Game();
            }
        }
    }

    public static partial class ParseGame_ImprovedClass2
    {
        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        [GeneratedRegex("(?<=fen).+?(?=moves|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex FenRegex();

        [GeneratedRegex("(?<=moves).+?(?=$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex MovesRegex();

        private static readonly Regex _fenRegex = FenRegex();
        private static readonly Regex _movesRegex = MovesRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Game ParseGame(string positionCommand)
        {
            try
            {
                var positionCommandSpan = positionCommand.AsSpan();
                Span<Range> items = stackalloc Range[3];    // Leaving 'everything else' in the third one
                positionCommandSpan.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries);
                bool isInitialPosition = positionCommandSpan[items[1]].Equals(StartPositionString, StringComparison.OrdinalIgnoreCase);

                var initialPosition = isInitialPosition
                        ? Constants.InitialPositionFEN
                        : _fenRegex.Match(positionCommand).Value.Trim();

                if (string.IsNullOrEmpty(initialPosition))
                {
                    _logger.Error("Error parsing position command '{0}': no initial position found", positionCommand);
                }

                var movesRegexResultAsSpan = _movesRegex.Match(positionCommand).ValueSpan;
                Span<Range> moves = stackalloc Range[(movesRegexResultAsSpan.Length / 5) + 1];
                movesRegexResultAsSpan.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries);

                return new Game(initialPosition, movesRegexResultAsSpan, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                return new Game();
            }
        }
    }

    public static partial class ParseGame_ImprovedClass3
    {
        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        [GeneratedRegex("(?<=fen).+?(?=moves|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex FenRegex();

        [GeneratedRegex("(?<=moves).+?(?=$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex MovesRegex();

        private static readonly Regex _fenRegex = FenRegex();
        private static readonly Regex _movesRegex = MovesRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Game ParseGame(string positionCommand)
        {
            try
            {
                var items = positionCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                bool isInitialPosition = string.Equals(items.ElementAtOrDefault(1), StartPositionString, StringComparison.OrdinalIgnoreCase);

                var initialPosition = isInitialPosition
                        ? Constants.InitialPositionFEN
                        : _fenRegex.Match(positionCommand).Value.Trim();

                if (string.IsNullOrEmpty(initialPosition))
                {
                    _logger.Error("Error parsing position command '{0}': no initial position found", positionCommand);
                }

                var movesRegexResultAsSpan = _movesRegex.Match(positionCommand).ValueSpan;
                Span<Range> moves = stackalloc Range[(movesRegexResultAsSpan.Length / 5) + 1];
                movesRegexResultAsSpan.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries);

                return new Game(initialPosition, movesRegexResultAsSpan, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                return new Game();
            }
        }
    }

    public static class ParseGame_ImprovedClass4
    {
        public const string Id = "position";

        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Game ParseGame(ReadOnlySpan<char> positionCommand)
        {
            try
            {
                // We divide the position command in these two sections:
                // "position startpos                       ||"
                // "position startpos                       || moves e2e4 e7e5"
                // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  ||"
                // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  || moves e2e4 e7e5"
                Span<Range> items = stackalloc Range[2];
                positionCommand.Split(items, "moves", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var initialPositionSection = positionCommand[items[0]];

                // We divide in these two parts
                // "position startpos ||"       <-- If "fen" doesn't exist in the section
                // "position || (fen) 8/8/8/8/8/8/8/8 w - - 0 1"  <-- If "fen" does exist
                Span<Range> initialPositionParts = stackalloc Range[2];
                initialPositionSection.Split(initialPositionParts, "fen", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                ReadOnlySpan<char> fen = initialPositionSection[initialPositionParts[0]].Length == Id.Length   // "position" o "position startpos"
                    ? initialPositionSection[initialPositionParts[1]]
                    : Constants.InitialPositionFEN.AsSpan();

                var movesSection = positionCommand[items[1]];

                Span<Range> moves = stackalloc Range[2048]; // Number of potential half-moves provided in the string
                movesSection.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new Game(fen, movesSection, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand.ToString());
                return new Game();
            }
        }
    }

    public static class ParseGame_ImprovedClass5
    {
        public const string Id = "position";

        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Game ParseGame(ReadOnlySpan<char> positionCommand)
        {
            try
            {
                // We divide the position command in these two sections:
                // "position startpos                       ||"
                // "position startpos                       || moves e2e4 e7e5"
                // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  ||"
                // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  || moves e2e4 e7e5"
                Span<Range> items = stackalloc Range[2];
                positionCommand.Split(items, "moves", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var initialPositionSection = positionCommand[items[0]];

                // We divide in these two parts
                // "position startpos ||"       <-- If "fen" doesn't exist in the section
                // "position || (fen) 8/8/8/8/8/8/8/8 w - - 0 1"  <-- If "fen" does exist
                Span<Range> initialPositionParts = stackalloc Range[2];
                initialPositionSection.Split(initialPositionParts, "fen", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                ReadOnlySpan<char> fen = initialPositionSection[initialPositionParts[0]].Length == Id.Length   // "position" o "position startpos"
                    ? initialPositionSection[initialPositionParts[1]]
                    : Constants.InitialPositionFEN.AsSpan();

                var movesSection = positionCommand[items[1]];

                Span<Range> moves = stackalloc Range[(movesSection.Length / 5) + 1]; // Number of potential half-moves provided in the string
                movesSection.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new Game(fen, movesSection, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand.ToString());
                return new Game();
            }
        }
    }
}
