/*
 * Not clear results in terms of what's better time-wise, it depends on os and position
 *
 *  Windows: 10m 26s
 *
 *  BenchmarkDotNet v0.13.7, Windows 10 (10.0.20348.1850) (Hyper-V)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.6.23330.14
 *    [Host]     : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
 *
 *  |                   Method |                 data |       Mean |     Error |    StdDev | Ratio | RatioSD |       Gen0 |      Gen1 |  Allocated | Alloc Ratio |
 *  |------------------------- |--------------------- |-----------:|----------:|----------:|------:|--------:|-----------:|----------:|-----------:|------------:|
 *  |              NewPosition | (2K2r(...)1, 6) [38] | 505.081 ms | 7.1977 ms | 6.7328 ms |  1.00 |    0.00 | 58000.0000 | 1000.0000 | 1042.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (2K2r(...)1, 6) [38] | 527.837 ms | 1.6840 ms | 1.5752 ms |  1.05 |    0.02 | 16000.0000 |         - |  293.38 MB |        0.28 |
 *  | MakeUnmakeMove_AllocBase | (2K2r(...)1, 6) [38] | ~~~~~~~ ms | 2.8088 ms | 2.6273 ms |  0.54 |    0.01 | 23000.0000 |         - |  411.23 MB |        0.39 |
 *  |   MakeUnmakeMove_PassOut | (2K2r(...)1, 6) [38] | 498.417 ms | 1.2868 ms | 1.1407 ms |  0.99 |    0.01 | 16000.0000 |         - |  293.38 MB |        0.28 |
 *  |   MakeUnmakeMove_PassRef | (2K2r(...)1, 6) [38] | 499.637 ms | 1.2777 ms | 1.1951 ms |  0.99 |    0.01 | 16000.0000 |         - |  293.38 MB |        0.28 |
 *  |                          |                      |            |           |           |       |         |            |           |            |             |
 *  |              NewPosition | (3K4/(...)1, 6) [38] | 511.377 ms | 9.6738 ms | 9.9343 ms |  1.00 |    0.00 | 58000.0000 | 1000.0000 | 1042.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (3K4/(...)1, 6) [38] | 518.529 ms | 2.1296 ms | 1.9920 ms |  1.01 |    0.02 | 16000.0000 |         - |  293.38 MB |        0.28 |
 *  | MakeUnmakeMove_AllocBase | (3K4/(...)1, 6) [38] | ~~~~~~~ ms | 2.7038 ms | 2.5291 ms |  0.51 |    0.01 | 23000.0000 |         - |  411.23 MB |        0.39 |
 *  |   MakeUnmakeMove_PassOut | (3K4/(...)1, 6) [38] | 498.936 ms | 1.2613 ms | 1.1181 ms |  0.97 |    0.02 | 16000.0000 |         - |  293.38 MB |        0.28 |
 *  |   MakeUnmakeMove_PassRef | (3K4/(...)1, 6) [38] | 489.408 ms | 1.7012 ms | 1.5913 ms |  0.96 |    0.02 | 16000.0000 |         - |  293.38 MB |        0.28 |
 *  |                          |                      |            |           |           |       |         |            |           |            |             |
 *  |              NewPosition | (8/p7(...)-, 6) [37] |  18.426 ms | 0.2205 ms | 0.2063 ms |  1.00 |    0.00 |  2343.7500 |   31.2500 |   41.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (8/p7(...)-, 6) [37] |  17.618 ms | 0.0497 ms | 0.0388 ms |  0.96 |    0.01 |   937.5000 |         - |   16.72 MB |        0.40 |
 *  | MakeUnmakeMove_AllocBase | (8/p7(...)-, 6) [37] |  ~~~~~~ ms | 0.1590 ms | 0.1561 ms |  0.45 |    0.01 |  1125.0000 |   15.6250 |   20.05 MB |        0.48 |
 *  |   MakeUnmakeMove_PassOut | (8/p7(...)-, 6) [37] |  18.425 ms | 0.1074 ms | 0.1004 ms |  1.00 |    0.01 |   937.5000 |         - |   16.72 MB |        0.40 |
 *  |   MakeUnmakeMove_PassRef | (8/p7(...)-, 6) [37] |  17.971 ms | 0.1246 ms | 0.1166 ms |  0.98 |    0.01 |   937.5000 |         - |   16.72 MB |        0.40 |
 *  |                          |                      |            |           |           |       |         |            |           |            |             |
 *  |              NewPosition | (r3k2(...)1, 4) [73] | 523.953 ms | 7.3117 ms | 6.8394 ms |  1.00 |    0.00 | 43000.0000 |         - |  782.46 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (r3k2(...)1, 4) [73] | 490.636 ms | 1.3682 ms | 1.2798 ms |  0.94 |    0.01 |  5000.0000 |         - |  102.19 MB |        0.13 |
 *  | MakeUnmakeMove_AllocBase | (r3k2(...)1, 4) [73] | ~~~~~~~ ms | 1.0410 ms | 0.9738 ms |  0.45 |    0.01 |  6333.3333 |         - |  115.47 MB |        0.15 |
 *  |   MakeUnmakeMove_PassOut | (r3k2(...)1, 4) [73] | 527.345 ms | 1.2952 ms | 1.0815 ms |  1.01 |    0.01 |  5000.0000 |         - |  102.19 MB |        0.13 |
 *  |   MakeUnmakeMove_PassRef | (r3k2(...)1, 4) [73] | 495.030 ms | 0.7315 ms | 0.6843 ms |  0.94 |    0.01 |  5000.0000 |         - |  102.19 MB |        0.13 |
 *  |                          |                      |            |           |           |       |         |            |           |            |             |
 *  |              NewPosition | (r4rk(...)0, 4) [77] | 440.616 ms | 4.0982 ms | 3.8334 ms |  1.00 |    0.00 | 41000.0000 |         - |  747.62 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (r4rk(...)0, 4) [77] | 427.875 ms | 0.6081 ms | 0.5688 ms |  0.97 |    0.01 |  5000.0000 |         - |   94.08 MB |        0.13 |
 *  | MakeUnmakeMove_AllocBase | (r4rk(...)0, 4) [77] | ~~~~~~~ ms | 0.3270 ms | 0.3059 ms |  0.41 |    0.00 |  5666.6667 |         - |  101.73 MB |        0.14 |
 *  |   MakeUnmakeMove_PassOut | (r4rk(...)0, 4) [77] | 434.955 ms | 1.0937 ms | 1.0230 ms |  0.99 |    0.01 |  5000.0000 |         - |   94.08 MB |        0.13 |
 *  |   MakeUnmakeMove_PassRef | (r4rk(...)0, 4) [77] | 406.970 ms | 0.6673 ms | 0.5210 ms |  0.92 |    0.01 |  5000.0000 |         - |   94.08 MB |        0.13 |
 *  |                          |                      |            |           |           |       |         |            |           |            |             |
 *  |              NewPosition | (rnbq(...)1, 4) [61] |  25.436 ms | 0.1450 ms | 0.1356 ms |  1.00 |    0.00 |  2375.0000 |   31.2500 |   42.71 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (rnbq(...)1, 4) [61] |  25.403 ms | 0.0621 ms | 0.0581 ms |  1.00 |    0.01 |   531.2500 |         - |    9.54 MB |        0.22 |
 *  | MakeUnmakeMove_AllocBase | (rnbq(...)1, 4) [61] | ~~~~~~~ ms | 0.0362 ms | 0.0302 ms |  0.33 |    0.00 |   468.7500 |         - |    8.61 MB |        0.20 |
 *  |   MakeUnmakeMove_PassOut | (rnbq(...)1, 4) [61] |  28.147 ms | 0.0858 ms | 0.0760 ms |  1.11 |    0.01 |   531.2500 |         - |    9.54 MB |        0.22 |
 *  |   MakeUnmakeMove_PassRef | (rnbq(...)1, 4) [61] |  28.532 ms | 0.0633 ms | 0.0562 ms |  1.12 |    0.01 |   531.2500 |         - |    9.54 MB |        0.22 |
 *
 *  Macos: 15m 52s
 *
 *  BenchmarkDotNet v0.13.7, macOS Monterey 12.6.7 (21G651) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-preview.6.23330.14
 *    [Host]     : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX
 *
 *  |                   Method |                 data |       Mean |      Error |     StdDev |     Median | Ratio | RatioSD |        Gen0 |      Gen1 |  Allocated | Alloc Ratio |
 *  |------------------------- |--------------------- |-----------:|-----------:|-----------:|-----------:|------:|--------:|------------:|----------:|-----------:|------------:|
 *  |              NewPosition | (2K2r(...)1, 6) [38] | 554.729 ms | 16.2321 ms | 46.8332 ms | 542.447 ms |  1.00 |    0.00 | 174000.0000 | 2000.0000 | 1042.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (2K2r(...)1, 6) [38] | 672.669 ms | 32.2980 ms | 94.7243 ms | 648.478 ms |  1.22 |    0.21 |  49000.0000 |         - |  293.38 MB |        0.28 |  MakeUnmake worse!!
 *  | MakeUnmakeMove_AllocBase | (2K2r(...)1, 6) [38] | ~~~~~~~ ms |  2.4888 ms |  2.0783 ms | 228.523 ms |  0.40 |    0.04 |  68500.0000 |  500.0000 |  411.23 MB |        0.39 |
 *  |   MakeUnmakeMove_PassOut | (2K2r(...)1, 6) [38] | 640.642 ms |  6.8037 ms |  6.3642 ms | 639.964 ms |  1.13 |    0.11 |  49000.0000 |         - |  293.38 MB |        0.28 |
 *  |   MakeUnmakeMove_PassRef | (2K2r(...)1, 6) [38] | 603.487 ms |  7.5206 ms |  6.6668 ms | 601.619 ms |  1.06 |    0.11 |  49000.0000 |         - |  293.38 MB |        0.28 |
 *  |                          |                      |            |            |            |            |       |         |             |           |            |             |
 *  |              NewPosition | (3K4/(...)1, 6) [38] | 499.420 ms |  9.8181 ms | 14.3913 ms | 495.514 ms |  1.00 |    0.00 | 174000.0000 | 2000.0000 | 1042.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (3K4/(...)1, 6) [38] | 591.564 ms |  2.6034 ms |  2.1740 ms | 591.801 ms |  1.18 |    0.04 |  49000.0000 |         - |  293.38 MB |        0.28 |  MakeUnmake worse!!
 *  | MakeUnmakeMove_AllocBase | (3K4/(...)1, 6) [38] | ~~~~~~~ ms |  3.8545 ms |  5.2760 ms | 222.555 ms |  0.45 |    0.01 |  68666.6667 |  666.6667 |  411.23 MB |        0.39 |
 *  |   MakeUnmakeMove_PassOut | (3K4/(...)1, 6) [38] | 626.900 ms | 11.2961 ms | 10.5663 ms | 623.760 ms |  1.25 |    0.05 |  49000.0000 |         - |  293.38 MB |        0.28 |
 *  |   MakeUnmakeMove_PassRef | (3K4/(...)1, 6) [38] | 611.374 ms |  5.3026 ms |  4.4279 ms | 611.434 ms |  1.22 |    0.04 |  49000.0000 |         - |  293.38 MB |        0.28 |
 *  |                          |                      |            |            |            |            |       |         |             |           |            |             |
 *  |              NewPosition | (8/p7(...)-, 6) [37] |  21.585 ms |  0.4289 ms |  1.0358 ms |  21.254 ms |  1.00 |    0.00 |   7000.0000 |  125.0000 |   41.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (8/p7(...)-, 6) [37] |  20.890 ms |  0.4154 ms |  0.5402 ms |  20.652 ms |  0.98 |    0.04 |   2781.2500 |   31.2500 |   16.72 MB |        0.40 |
 *  | MakeUnmakeMove_AllocBase | (8/p7(...)-, 6) [37] |  ~~~~~~ ms |  0.1509 ms |  0.1614 ms |   7.765 ms |  0.36 |    0.02 |   3343.7500 |   46.8750 |   20.05 MB |        0.48 |
 *  |   MakeUnmakeMove_PassOut | (8/p7(...)-, 6) [37] |  22.490 ms |  0.4300 ms |  0.3590 ms |  22.381 ms |  1.04 |    0.05 |   2781.2500 |   31.2500 |   16.72 MB |        0.40 |
 *  |   MakeUnmakeMove_PassRef | (8/p7(...)-, 6) [37] |  23.021 ms |  0.1899 ms |  0.1586 ms |  22.994 ms |  1.06 |    0.05 |   2781.2500 |   31.2500 |   16.72 MB |        0.40 |
 *  |                          |                      |            |            |            |            |       |         |             |           |            |             |
 *  |              NewPosition | (r3k2(...)1, 4) [73] | 513.828 ms |  3.5809 ms |  3.1743 ms | 513.608 ms |  1.00 |    0.00 | 130000.0000 | 1000.0000 |  782.46 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (r3k2(...)1, 4) [73] | 504.889 ms |  2.6640 ms |  2.0799 ms | 505.221 ms |  0.98 |    0.01 |  17000.0000 |         - |  102.19 MB |        0.13 |
 *  | MakeUnmakeMove_AllocBase | (r3k2(...)1, 4) [73] | ~~~~~~~ ms |  4.2723 ms |  3.9963 ms | 216.646 ms |  0.43 |    0.01 |  19000.0000 |         - |  115.47 MB |        0.15 |
 *  |   MakeUnmakeMove_PassOut | (r3k2(...)1, 4) [73] | 645.425 ms |  7.3456 ms |  6.1339 ms | 643.985 ms |  1.26 |    0.02 |  17000.0000 |         - |  102.19 MB |        0.13 |  Ref and out way worse
 *  |   MakeUnmakeMove_PassRef | (r3k2(...)1, 4) [73] | 620.624 ms |  7.6333 ms |  6.3741 ms | 619.387 ms |  1.21 |    0.01 |  17000.0000 |         - |  102.19 MB |        0.13 |
 *  |                          |                      |            |            |            |            |       |         |             |           |            |             |
 *  |              NewPosition | (r4rk(...)0, 4) [77] | 424.438 ms |  6.7093 ms |  6.2759 ms | 423.493 ms |  1.00 |    0.00 | 124000.0000 | 1000.0000 |  747.62 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (r4rk(...)0, 4) [77] | 498.532 ms |  4.9346 ms |  4.1206 ms | 497.480 ms |  1.17 |    0.02 |  15000.0000 |         - |   94.08 MB |        0.13 |  MakeUnmake worse!!
 *  | MakeUnmakeMove_AllocBase | (r4rk(...)0, 4) [77] | ~~~~~~~ ms |  2.4405 ms |  2.1635 ms | 173.112 ms |  0.41 |    0.01 |  17000.0000 |         - |  101.73 MB |        0.14 |
 *  |   MakeUnmakeMove_PassOut | (r4rk(...)0, 4) [77] | 494.288 ms |  3.0732 ms |  2.3993 ms | 493.571 ms |  1.16 |    0.02 |  15000.0000 |         - |   94.08 MB |        0.13 |
 *  |   MakeUnmakeMove_PassRef | (r4rk(...)0, 4) [77] | 523.703 ms |  3.5379 ms |  2.9543 ms | 523.081 ms |  1.23 |    0.02 |  15000.0000 |         - |   94.08 MB |        0.13 |  ref worse
 *  |                          |                      |            |            |            |            |       |         |             |           |            |             |
 *  |              NewPosition | (rnbq(...)1, 4) [61] |  27.434 ms |  0.4791 ms |  0.4247 ms |  27.402 ms |  1.00 |    0.00 |   7125.0000 |   93.7500 |   42.71 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (rnbq(...)1, 4) [61] |  31.436 ms |  0.4706 ms |  0.3930 ms |  31.297 ms |  1.15 |    0.02 |   1562.5000 |         - |    9.54 MB |        0.22 |  MakeUnmake worse!!
 *  | MakeUnmakeMove_AllocBase | (rnbq(...)1, 4) [61] | ~~~~~~~ ms |  0.0572 ms |  0.0507 ms |   7.837 ms |  0.29 |    0.00 |   1437.5000 |   15.6250 |    8.61 MB |        0.20 |
 *  |   MakeUnmakeMove_PassOut | (rnbq(...)1, 4) [61] |  29.899 ms |  0.1640 ms |  0.1369 ms |  29.849 ms |  1.09 |    0.02 |   1593.7500 |         - |    9.54 MB |        0.22 |
 *  |   MakeUnmakeMove_PassRef | (rnbq(...)1, 4) [61] |  34.043 ms |  0.5202 ms |  0.4866 ms |  33.834 ms |  1.24 |    0.02 |   1533.3333 |         - |    9.54 MB |        0.22 |  ref worse
 *
 *
 * Ubuntu: 24min 36s
 *
 *  BenchmarkDotNet v0.13.7, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
 *  Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.6.23330.14
 *    [Host]     : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
 *
 *
 *  |                   Method |                 data |       Mean |      Error |     StdDev |     Median | Ratio | RatioSD |       Gen0 |    Gen1 |  Allocated | Alloc Ratio |
 *  |------------------------- |--------------------- |-----------:|-----------:|-----------:|-----------:|------:|--------:|-----------:|--------:|-----------:|------------:|
 *  |              NewPosition | (2K2r(...)1, 6) [38] | 555.085 ms | 15.8110 ms | 46.6192 ms | 553.541 ms |  1.00 |    0.00 | 41000.0000 |       - | 1042.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (2K2r(...)1, 6) [38] | 473.748 ms |  9.3788 ms | 21.3603 ms | 468.637 ms |  0.83 |    0.06 | 11000.0000 |       - |  293.38 MB |        0.28 | Original better
 *  | MakeUnmakeMove_AllocBase | (2K2r(...)1, 6) [38] | ~~~~~~~ ms |  4.6135 ms |  8.9983 ms | 228.217 ms |  0.41 |    0.03 | 16333.3333 |       - |  411.23 MB |        0.39 |
 *  |   MakeUnmakeMove_PassOut | (2K2r(...)1, 6) [38] | 604.758 ms | 21.3572 ms | 62.9722 ms | 626.625 ms |  1.10 |    0.14 | 11000.0000 |       - |  293.38 MB |        0.28 |
 *  |   MakeUnmakeMove_PassRef | (2K2r(...)1, 6) [38] | 512.277 ms | 16.7119 ms | 49.2754 ms | 495.644 ms |  0.93 |    0.15 | 11000.0000 |       - |  293.38 MB |        0.28 |
 *  |                          |                      |            |            |            |            |       |         |            |         |            |             |
 *  |              NewPosition | (3K4/(...)1, 6) [38] | 543.778 ms | 15.0299 ms | 43.8429 ms | 538.750 ms |  1.00 |    0.00 | 41000.0000 |       - | 1042.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (3K4/(...)1, 6) [38] | 487.426 ms |  9.7106 ms | 16.7503 ms | 486.213 ms |  0.87 |    0.09 | 11000.0000 |       - |  293.38 MB |        0.28 |
 *  | MakeUnmakeMove_AllocBase | (3K4/(...)1, 6) [38] | ~~~~~~~ ms |  4.9895 ms | 12.4256 ms | 249.633 ms |  0.46 |    0.05 | 16333.3333 |       - |  411.23 MB |        0.39 |
 *  |   MakeUnmakeMove_PassOut | (3K4/(...)1, 6) [38] | 486.563 ms |  9.0119 ms | 18.4090 ms | 486.559 ms |  0.90 |    0.08 | 11000.0000 |       - |  293.38 MB |        0.28 |
 *  |   MakeUnmakeMove_PassRef | (3K4/(...)1, 6) [38] | 481.843 ms |  8.7130 ms | 13.5651 ms | 482.082 ms |  0.85 |    0.07 | 11000.0000 |       - |  293.38 MB |        0.28 |
 *  |                          |                      |            |            |            |            |       |         |            |         |            |             |
 *  |              NewPosition | (8/p7(...)-, 6) [37] |  18.649 ms |  0.3700 ms |  1.0190 ms |  18.488 ms |  1.00 |    0.00 |  1656.2500 | 31.2500 |   41.91 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (8/p7(...)-, 6) [37] |  17.304 ms |  0.3350 ms |  0.5410 ms |  17.306 ms |  0.92 |    0.06 |   656.2500 |       - |   16.72 MB |        0.40 |
 *  | MakeUnmakeMove_AllocBase | (8/p7(...)-, 6) [37] | ~~~~~~~ ms |  0.1654 ms |  0.4694 ms |   7.850 ms |  0.43 |    0.03 |   796.8750 | 15.6250 |   20.05 MB |        0.48 |
 *  |   MakeUnmakeMove_PassOut | (8/p7(...)-, 6) [37] |  19.250 ms |  0.4507 ms |  1.3289 ms |  19.201 ms |  1.03 |    0.09 |   656.2500 |       - |   16.72 MB |        0.40 |
 *  |   MakeUnmakeMove_PassRef | (8/p7(...)-, 6) [37] |  18.702 ms |  0.3731 ms |  0.6234 ms |  18.722 ms |  0.99 |    0.05 |   656.2500 |       - |   16.72 MB |        0.40 |
 *  |                          |                      |            |            |            |            |       |         |            |         |            |             |
 *  |              NewPosition | (r3k2(...)1, 4) [73] | 537.770 ms | 11.6499 ms | 34.1672 ms | 528.499 ms |  1.00 |    0.00 | 31000.0000 |       - |  782.46 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (r3k2(...)1, 4) [73] | 522.154 ms | 10.4351 ms | 12.4222 ms | 519.825 ms |  0.92 |    0.07 |  4000.0000 |       - |  102.19 MB |        0.13 |
 *  | MakeUnmakeMove_AllocBase | (r3k2(...)1, 4) [73] | ~~~~~~~ ms |  4.5536 ms | 11.5904 ms | 231.478 ms |  0.43 |    0.04 |  4333.3333 |       - |  115.47 MB |        0.15 |
 *  |   MakeUnmakeMove_PassOut | (r3k2(...)1, 4) [73] | 500.713 ms | 10.7942 ms | 31.3159 ms | 493.367 ms |  0.93 |    0.07 |  4000.0000 |       - |  102.19 MB |        0.13 |
 *  |   MakeUnmakeMove_PassRef | (r3k2(...)1, 4) [73] | 635.080 ms | 12.6013 ms | 11.7873 ms | 632.577 ms |  1.11 |    0.07 |  4000.0000 |       - |  102.19 MB |        0.13 | ref worse
 *  |                          |                      |            |            |            |            |       |         |            |         |            |             |
 *  |              NewPosition | (r4rk(...)0, 4) [77] | 411.582 ms |  8.1631 ms | 19.2413 ms | 408.568 ms |  1.00 |    0.00 | 29000.0000 |       - |  747.62 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (r4rk(...)0, 4) [77] | 410.471 ms |  8.2078 ms | 12.7785 ms | 408.010 ms |  1.00 |    0.05 |  3000.0000 |       - |   94.08 MB |        0.13 |
 *  | MakeUnmakeMove_AllocBase | (r4rk(...)0, 4) [77] | ~~~~~~~ ms |  3.5925 ms |  4.6713 ms | 185.756 ms |  0.45 |    0.03 |  4000.0000 |       - |  101.73 MB |        0.14 |
 *  |   MakeUnmakeMove_PassOut | (r4rk(...)0, 4) [77] | 414.260 ms |  6.4043 ms |  6.8525 ms | 411.839 ms |  1.01 |    0.05 |  3000.0000 |       - |   94.08 MB |        0.13 |
 *  |   MakeUnmakeMove_PassRef | (r4rk(...)0, 4) [77] | 367.889 ms |  7.3063 ms |  8.6977 ms | 366.182 ms |  0.89 |    0.04 |  3000.0000 |       - |   94.08 MB |        0.13 | ref better
 *  |                          |                      |            |            |            |            |       |         |            |         |            |             |
 *  |              NewPosition | (rnbq(...)1, 4) [61] |  27.921 ms |  0.5829 ms |  1.6630 ms |  27.772 ms |  1.00 |    0.00 |  1687.5000 |       - |   42.71 MB |        1.00 |
 *  |  MakeUnmakeMove_Original | (rnbq(...)1, 4) [61] |  26.648 ms |  0.5312 ms |  0.8728 ms |  26.380 ms |  0.96 |    0.06 |   375.0000 |       - |    9.54 MB |        0.22 |
 *  | MakeUnmakeMove_AllocBase | (rnbq(...)1, 4) [61] | ~~~~~~~ ms |  0.1581 ms |  0.4052 ms |   7.838 ms |  0.28 |    0.02 |   343.7500 |       - |    8.61 MB |        0.20 |
 *  |   MakeUnmakeMove_PassOut | (rnbq(...)1, 4) [61] |  28.143 ms |  0.5510 ms |  0.6969 ms |  28.054 ms |  1.00 |    0.05 |   375.0000 |       - |    9.54 MB |        0.22 |
 *  |   MakeUnmakeMove_PassRef | (rnbq(...)1, 4) [61] |  27.016 ms |  0.5351 ms |  0.8940 ms |  26.693 ms |  0.97 |    0.05 |   375.0000 |       - |    9.54 MB |        0.22 |
 */

#pragma warning disable S101, S1854 // Types should be named in PascalCase
using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class MakeUnmakeMove_integration : BaseBenchmark
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
    public long MakeUnmakeMove_AllocBase((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_AllocBase(new(data.Fen), data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove_PassOut((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_MakeUnmakeMove_PassOut(new(data.Fen), data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove_PassRef((string Fen, int Depth) data) => MakeMovePerft.ResultsImpl_MakeUnmakeMove_PassRef(new(data.Fen), data.Depth, default);

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

        public static long ResultsImpl_AllocBase(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_AllocBase(position, depth - 1, nodes);
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

        public static long ResultsImpl_MakeUnmakeMove_PassOut(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    //_gameStates.Push(position.MakeMove(move));
                    position.MakeMove_PassOut(move, out var state);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_MakeUnmakeMove_PassOut(position, depth - 1, nodes);
                    }
                    //position.UnmakeMove(move, _gameStates.Pop());
                    position.UnmakeMove_PassOut(move, state);
                }

                return nodes;
            }

            return ++nodes;
        }

        public static long ResultsImpl_MakeUnmakeMove_PassRef(MakeMovePosition position, int depth, long nodes)
        {
            if (depth != 0)
            {
                MakeMoveGameState_PassRef state = new();
                foreach (var move in MakeMoveMoveGenerator.GenerateAllMoves(position))
                {
                    //_gameStates.Push(position.MakeMove(move));
                    position.MakeMove_PassRef(move, ref state);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl_MakeUnmakeMove_PassRef(position, depth - 1, nodes);
                    }
                    //position.UnmakeMove(move, _gameStates.Pop());
                    position.UnmakeMove_PassRef(move, state);
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

        public int Castle { get; private set; }

        public MakeMovePosition(string fen) : this(FENParser.ParseFEN(fen))
        {
        }

        public MakeMovePosition((bool Success, BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, int Castle, BoardSquare EnPassant,
            int HalfMoveClock, int FullMoveCounter) parsedFEN)
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
                Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

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
            int castleCopy = Castle;
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
                Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

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
        public void MakeMove_PassOut(Move move, out MakeMoveGameState_PassOut gameState)
        {
            int capturedPiece = -1;
            int castleCopy = Castle;
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
                Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

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

            gameState = new MakeMoveGameState_PassOut(capturedPiece, castleCopy, enpassantCopy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MakeMove_PassRef(Move move, ref MakeMoveGameState_PassRef gameState)
        {
            int capturedPiece = -1;
            int castleCopy = Castle;
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
                Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

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

            gameState.CapturedPiece = capturedPiece;
            gameState.Castle = castleCopy;
            gameState.EnPassant = enpassantCopy;
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
        public void UnmakeMove_PassOut(Move move, MakeMoveGameState_PassOut gameState)
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
        public void UnmakeMove_PassRef(Move move, MakeMoveGameState_PassRef gameState)
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

    public readonly struct MakeMoveGameState(int capturedPiece, int castle, BoardSquare enpassant)
    {
        public readonly int CapturedPiece = capturedPiece;

        public readonly int Castle = castle;

        public readonly BoardSquare EnPassant = enpassant;
    }

    public struct MakeMoveGameState_PassOut(int capturedPiece, int castle, BoardSquare enpassant)
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        public int CapturedPiece = capturedPiece;

        public int Castle = castle;

        public BoardSquare EnPassant = enpassant;
#pragma warning restore S1104 // Fields should not have public accessibility
    }

    public struct MakeMoveGameState_PassRef
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        public int CapturedPiece;

        public int Castle;

        public BoardSquare EnPassant;
#pragma warning restore S1104 // Fields should not have public accessibility
    }

    #region ;(

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

#if DEBUG
            if (!Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare))
            {
                throw new ArgumentException($"{Constants.Coordinates[enPassantSquare]} is not a valid en-passant square");
            }
#endif

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
        public static long CastleHash(int castle)
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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int TRUE = 1;

        /// <summary>
        /// Indexed by <see cref="Piece"/>.
        /// Checks are not considered
        /// </summary>
        private static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks = new Func<int, BitBoard, BitBoard>[]
        {
            (int origin, BitBoard _) => MakeMoveAttacks.PawnAttacks[(int)Side.White, origin],
            (int origin, BitBoard _) => MakeMoveAttacks.KnightAttacks[origin],
            (int origin, BitBoard occupancy) => MakeMoveAttacks.BishopAttacks(origin, occupancy),
            (int origin, BitBoard occupancy) => MakeMoveAttacks.RookAttacks(origin, occupancy),
            (int origin, BitBoard occupancy) => MakeMoveAttacks.QueenAttacks(origin, occupancy),
            (int origin, BitBoard _) => MakeMoveAttacks.KingAttacks[origin],

            (int origin, BitBoard _) => MakeMoveAttacks.PawnAttacks[(int)Side.Black, origin],
            (int origin, BitBoard _) => MakeMoveAttacks.KnightAttacks[origin],
            (int origin, BitBoard occupancy) => MakeMoveAttacks.BishopAttacks(origin, occupancy),
            (int origin, BitBoard occupancy) => MakeMoveAttacks.RookAttacks(origin, occupancy),
            (int origin, BitBoard occupancy) => MakeMoveAttacks.QueenAttacks(origin, occupancy),
            (int origin, BitBoard _) => MakeMoveAttacks.KingAttacks[origin],
        };

        /// <summary>
        /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="capturesOnly">Filters out all moves but captures</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Move> GenerateAllMoves(MakeMovePosition position, Move[]? movePool = null, bool capturesOnly = false)
        {
#if DEBUG
            if (position.Side == Side.Both)
            {
                return new List<Move>();
            }
#endif

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

#if DEBUG
                if (sourceRank == 1 || sourceRank == 8)
                {
                    _logger.Warn("There's a non-promoted {0} pawn in rank {1}", position.Side, sourceRank);
                    continue;
                }
#endif

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var targetRank = (singlePushSquare / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
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
                            movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE);
                        }
                    }
                }

                var attacks = MakeMoveAttacks.PawnAttacks[(int)position.Side, sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
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
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                    }
                    else
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
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
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                    }

                    if (((position.Castle & (int)CastlingRights.WQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                        && !ise1Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
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
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                    }

                    if (((position.Castle & (int)CastlingRights.BQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                        && !ise8Attacked
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                        && !MakeMoveAttacks.IsSquaredAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
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
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
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
        private static readonly BitBoard[,] _bishopAttacks;

        /// <summary>
        /// [64 (Squares), 4096 (Occupancies)]
        /// Use <see cref="RookAttacks(int, BitBoard)"/>
        /// </summary>
        private static readonly BitBoard[,] _rookAttacks;

        /// <summary>
        /// [2 (B|W), 64 (Squares)]
        /// </summary>
        public static BitBoard[,] PawnAttacks { get; }
        public static BitBoard[] KnightAttacks { get; }
        public static BitBoard[] KingAttacks { get; }

        static MakeMoveAttacks()
        {
            KingAttacks = AttackGenerator.InitializeKingAttacks();
            PawnAttacks = AttackGenerator.InitializePawnAttacks();
            KnightAttacks = AttackGenerator.InitializeKnightAttacks();

            (_bishopOccupancyMasks, _bishopAttacks) = AttackGenerator.InitializeBishopAttacks();
            (_rookOccupancyMasks, _rookAttacks) = AttackGenerator.InitializeRookAttacks();
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

            return _bishopAttacks[squareIndex, occ];
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

            return _rookAttacks[squareIndex, occ];
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

            return (PawnAttacks[oppositeColorIndex, squareIndex] & pieces[offset]) != default;
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
