/*
 * Span Split()
 *
 *  BenchmarkDotNet v0.13.8, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method             | positionCommand      | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |--------------------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | ParseGame_Original | position startpos    |   5.993 us | 0.0232 us | 0.0181 us |  1.00 |    0.00 |  0.4349 | 0.0076 |   8.06 KB |        1.00 |
 *  | ParseGame_Current  | position startpos    |   6.416 us | 0.1248 us | 0.1867 us |  1.07 |    0.04 |  0.4272 | 0.0076 |   7.91 KB |        0.98 |
 *  |                    |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original | posi(...)b7b6 [193]  |  35.543 us | 0.6989 us | 1.0024 us |  1.00 |    0.00 |  1.0376 |      - |   19.1 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)b7b6 [193]  |  32.770 us | 0.0735 us | 0.0574 us |  0.92 |    0.03 |  0.8545 |      - |  16.35 KB |        0.86 |
 *  |                    |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original | posi(...)f3g3 [353]  |  57.090 us | 0.2043 us | 0.1706 us |  1.00 |    0.00 |  1.5869 |      - |  29.16 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)f3g3 [353]  |  54.818 us | 0.1064 us | 0.0888 us |  0.96 |    0.00 |  1.2817 |      - |   24.1 KB |        0.83 |
 *  |                    |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original | posi(...)g4g8 [979]  | 138.864 us | 1.0745 us | 0.8972 us |  1.00 |    0.00 |  4.1504 | 0.2441 |  76.73 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)g4g8 [979]  | 134.049 us | 0.2420 us | 0.2145 us |  0.97 |    0.01 |  3.4180 |      - |  62.63 KB |        0.82 |
 *  |                    |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original | posi(...)h3f1 [2984] | 377.699 us | 1.1069 us | 1.0354 us |  1.00 |    0.00 | 11.7188 | 1.4648 | 220.13 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)h3f1 [2984] | 201.096 us | 0.3613 us | 0.3380 us |  0.53 |    0.00 |  4.6387 |      - |  88.65 KB |        0.40 |
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method             | positionCommand      | Mean       | Error     | StdDev    | Ratio | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |--------------------- |-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | ParseGame_Original | position startpos    |   4.291 us | 0.0413 us | 0.0386 us |  1.00 |  0.4349 | 0.0076 |   8.06 KB |        1.00 |
 *  | ParseGame_Current  | position startpos    |   4.099 us | 0.0311 us | 0.0276 us |  0.95 |  0.4272 | 0.0076 |   7.91 KB |        0.98 |
 *  |                    |                      |            |           |           |       |         |        |           |             |
 *  | ParseGame_Original | posi(...)b7b6 [193]  |  25.962 us | 0.0811 us | 0.0758 us |  1.00 |  1.0376 |      - |   19.1 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)b7b6 [193]  |  25.368 us | 0.0662 us | 0.0619 us |  0.98 |  0.8850 |      - |  16.35 KB |        0.86 |
 *  |                    |                      |            |           |           |       |         |        |           |             |
 *  | ParseGame_Original | posi(...)f3g3 [353]  |  42.801 us | 0.0650 us | 0.0543 us |  1.00 |  1.5869 |      - |  29.16 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)f3g3 [353]  |  42.378 us | 0.0980 us | 0.0868 us |  0.99 |  1.2817 |      - |   24.1 KB |        0.83 |
 *  |                    |                      |            |           |           |       |         |        |           |             |
 *  | ParseGame_Original | posi(...)g4g8 [979]  | 100.014 us | 0.3599 us | 0.3190 us |  1.00 |  4.1504 | 0.2441 |  76.73 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)g4g8 [979]  | 102.567 us | 0.4239 us | 0.3966 us |  1.03 |  3.4180 | 0.1221 |  62.63 KB |        0.82 |
 *  |                    |                      |            |           |           |       |         |        |           |             |
 *  | ParseGame_Original | posi(...)h3f1 [2984] | 285.070 us | 1.2580 us | 1.1768 us |  1.00 | 11.7188 | 1.4648 | 220.13 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)h3f1 [2984] | 158.920 us | 0.5421 us | 0.4806 us |  0.56 |  4.6387 |      - |  88.65 KB |        0.40 |
 *
 *  BenchmarkDotNet v0.13.8, macOS Monterey 12.6.8 (21G725) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *
 *
 *  | Method             | positionCommand      | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |--------------------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | ParseGame_Original | position startpos    |   4.627 us | 0.0838 us | 0.0784 us |  1.00 |    0.00 |  1.3123 | 0.0153 |   8.06 KB |        1.00 |
 *  | ParseGame_Current  | position startpos    |   4.629 us | 0.0514 us | 0.0480 us |  1.00 |    0.02 |  1.2894 | 0.0076 |   7.91 KB |        0.98 |
 *  |                    |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original | posi(...)b7b6 [193]  |  28.262 us | 0.1340 us | 0.1046 us |  1.00 |    0.00 |  3.1128 | 0.0305 |   19.1 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)b7b6 [193]  |  27.747 us | 0.1485 us | 0.1316 us |  0.98 |    0.01 |  2.6550 | 0.0305 |  16.35 KB |        0.86 |
 *  |                    |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original | posi(...)f3g3 [353]  |  47.539 us | 0.7709 us | 0.7211 us |  1.00 |    0.00 |  4.7607 | 0.1221 |  29.17 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)f3g3 [353]  |  45.521 us | 0.1903 us | 0.1687 us |  0.96 |    0.02 |  3.9063 | 0.0610 |  24.11 KB |        0.83 |
 *  |                    |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original | posi(...)g4g8 [979]  | 115.372 us | 1.1374 us | 1.0639 us |  1.00 |    0.00 | 12.4512 | 0.7324 |  76.76 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)g4g8 [979]  | 106.751 us | 1.4607 us | 1.3663 us |  0.93 |    0.02 | 10.1318 | 0.3662 |  62.65 KB |        0.82 |
 *  |                    |                      |            |           |           |       |         |         |        |           |             |
 *  | ParseGame_Original | posi(...)h3f1 [2984] | 316.310 us | 2.6989 us | 2.5246 us |  1.00 |    0.00 | 35.6445 | 4.3945 | 220.21 KB |        1.00 |
 *  | ParseGame_Current  | posi(...)h3f1 [2984] | 200.254 us | 1.8876 us | 1.7656 us |  0.63 |    0.01 | 14.4043 | 0.4883 |  88.67 KB |        0.40 |
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
    public Game ParseGame_Current(string positionCommand) => ParseGame_ImprovedClass.ParseGame(positionCommand);

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

                var moves = _movesRegex.Match(positionCommand).Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

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

        public static partial class ParseGame_ImprovedClass
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

                    Span<Range> moves = stackalloc Range[250];
                    var movesRegexResult = _movesRegex.Match(positionCommand).ValueSpan;
                    movesRegexResult.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries);

                    return new Game(initialPosition, movesRegexResult, moves);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                    return new Game();
                }
            }
        }
    }
