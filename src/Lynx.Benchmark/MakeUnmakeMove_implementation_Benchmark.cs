/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                                             | data                 | Mean         | Error       | StdDev      | Ratio | Gen0       | Allocated     | Alloc Ratio |
 *  |--------------------------------------------------- |--------------------- |-------------:|------------:|------------:|------:|-----------:|--------------:|------------:|
 *  | NewPosition                                        | (2K2r(...)1, 6) [38] | 397,636.8 us | 1,175.57 us |   917.81 us |  1.00 | 13000.0000 | 1069286.11 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (2K2r(...)1, 6) [38] | 440,829.8 us | 2,429.95 us | 2,272.97 us |  1.11 |  3000.0000 |  278498.13 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                      | (2K2r(...)1, 6) [38] | 280,883.6 us | 3,071.18 us | 2,722.52 us |  0.71 |  3000.0000 |  278497.61 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (2K2r(...)1, 6) [38] |  22,239.5 us |    93.62 us |    82.99 us |  0.06 |   406.2500 |   35056.14 KB |        0.03 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (2K2r(...)1, 6) [38] |  21,462.0 us |   131.14 us |   122.67 us |  0.05 |   406.2500 |   35056.14 KB |        0.03 |
 *  |                                                    |                      |              |             |             |       |            |               |             |
 *  | NewPosition                                        | (3K4/(...)1, 6) [38] | 392,544.0 us | 1,401.80 us | 1,094.43 us |  1.00 | 13000.0000 | 1069286.11 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (3K4/(...)1, 6) [38] | 442,075.5 us | 3,344.91 us | 2,965.17 us |  1.13 |  3000.0000 |  278498.13 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                      | (3K4/(...)1, 6) [38] | 284,606.2 us | 1,503.41 us | 1,332.74 us |  0.73 |  3000.0000 |  278497.61 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (3K4/(...)1, 6) [38] |  21,033.2 us |    32.48 us |    25.36 us |  0.05 |    62.5000 |    5446.33 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (3K4/(...)1, 6) [38] |  20,827.3 us |    99.09 us |    92.69 us |  0.05 |    62.5000 |    5446.33 KB |       0.005 |
 *  |                                                    |                      |              |             |             |       |            |               |             |
 *  | NewPosition                                        | (8/p7(...)-, 6) [37] |  14,260.2 us |    46.97 us |    43.94 us |  1.00 |   515.6250 |    42915.8 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (8/p7(...)-, 6) [37] |  16,042.8 us |    43.61 us |    34.05 us |  1.12 |   187.5000 |   17113.08 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                      | (8/p7(...)-, 6) [37] |  10,309.4 us |    28.15 us |    24.95 us |  0.72 |   203.1250 |   17113.07 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (8/p7(...)-, 6) [37] |   4,650.9 us |    29.48 us |    27.58 us |  0.33 |   101.5625 |    8398.54 KB |        0.20 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (8/p7(...)-, 6) [37] |   4,650.4 us |    30.97 us |    27.46 us |  0.33 |   101.5625 |    8398.54 KB |        0.20 |
 *  |                                                    |                      |              |             |             |       |            |               |             |
 *  | NewPosition                                        | (r3k2(...)1, 4) [73] | 329,451.7 us | 1,566.86 us | 1,465.64 us | 1.000 |  9500.0000 |  801233.01 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r3k2(...)1, 4) [73] | 331,610.6 us |   419.38 us |   327.42 us | 1.006 |  1000.0000 |  104639.41 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r3k2(...)1, 4) [73] | 249,777.8 us | 2,035.13 us | 1,903.66 us | 0.758 |  1000.0000 |  104638.89 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r3k2(...)1, 4) [73] |     163.1 us |     0.33 us |     0.27 us | 0.000 |     1.2207 |     102.76 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r3k2(...)1, 4) [73] |     161.1 us |     2.51 us |     2.35 us | 0.000 |     1.2207 |     102.76 KB |       0.000 |
 *  |                                                    |                      |              |             |             |       |            |               |             |
 *  | NewPosition                                        | (r4rk(...)0, 4) [77] | 310,900.6 us | 1,324.89 us | 1,034.39 us | 1.000 |  9000.0000 |  765557.21 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r4rk(...)0, 4) [77] | 282,021.9 us | 1,572.90 us | 1,471.30 us | 0.907 |  1000.0000 |   96332.98 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r4rk(...)0, 4) [77] | 213,212.4 us | 1,601.03 us | 1,497.60 us | 0.685 |  1000.0000 |   96332.81 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r4rk(...)0, 4) [77] |     348.3 us |     2.25 us |     2.11 us | 0.001 |     2.4414 |      208.5 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r4rk(...)0, 4) [77] |     340.1 us |     1.96 us |     1.74 us | 0.001 |     2.4414 |      208.5 KB |       0.000 |
 *  |                                                    |                      |              |             |             |       |            |               |             |
 *  | NewPosition                                        | (rnbq(...)1, 4) [61] |  17,400.8 us |   200.22 us |   187.28 us |  1.00 |   531.2500 |   43733.27 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (rnbq(...)1, 4) [61] |  15,707.5 us |    22.23 us |    19.71 us |  0.90 |    93.7500 |    9760.59 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                      | (rnbq(...)1, 4) [61] |  11,660.6 us |    53.70 us |    50.23 us |  0.67 |   109.3750 |    9760.58 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (rnbq(...)1, 4) [61] |     228.4 us |     0.39 us |     0.36 us |  0.01 |     2.6855 |     220.02 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (rnbq(...)1, 4) [61] |     247.4 us |     1.68 us |     1.57 us |  0.01 |     2.4414 |     220.02 KB |       0.005 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                                             | data                 | Mean         | Error       | StdDev      | Ratio | RatioSD | Gen0       | Gen1      | Allocated     | Alloc Ratio |
 *  |--------------------------------------------------- |--------------------- |-------------:|------------:|------------:|------:|--------:|-----------:|----------:|--------------:|------------:|
 *  | NewPosition                                        | (2K2r(...)1, 6) [38] | 362,623.0 us | 7,118.07 us | 8,197.18 us |  1.00 |    0.00 | 65000.0000 | 1000.0000 | 1069285.13 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (2K2r(...)1, 6) [38] | 469,134.5 us | 7,765.80 us | 7,264.13 us |  1.29 |    0.04 | 17000.0000 |         - |   278497.8 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                      | (2K2r(...)1, 6) [38] | 262,467.8 us | 5,146.64 us | 7,703.25 us |  0.73 |    0.02 | 17000.0000 |         - |  278497.45 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (2K2r(...)1, 6) [38] |  21,355.4 us |   155.29 us |   145.26 us |  0.06 |    0.00 |  2125.0000 |   31.2500 |   35057.84 KB |        0.03 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (2K2r(...)1, 6) [38] |  21,132.3 us |   205.52 us |   192.24 us |  0.06 |    0.00 |  2125.0000 |   31.2500 |   35057.84 KB |        0.03 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (3K4/(...)1, 6) [38] | 317,801.2 us | 3,109.67 us | 2,908.79 us |  1.00 |    0.00 | 65000.0000 | 1000.0000 | 1069285.42 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (3K4/(...)1, 6) [38] | 447,091.0 us | 2,146.22 us | 2,007.57 us |  1.41 |    0.01 | 17000.0000 |         - |   278497.8 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                      | (3K4/(...)1, 6) [38] | 270,516.0 us | 1,031.18 us |   914.11 us |  0.85 |    0.01 | 17000.0000 |         - |  278497.45 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (3K4/(...)1, 6) [38] |  21,649.2 us |    58.64 us |    54.85 us |  0.07 |    0.00 |   312.5000 |         - |    5447.08 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (3K4/(...)1, 6) [38] |  21,053.5 us |   120.87 us |    94.36 us |  0.07 |    0.00 |   312.5000 |         - |    5447.08 KB |       0.005 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (8/p7(...)-, 6) [37] |  13,088.8 us |   259.00 us |   242.27 us |  1.00 |    0.00 |  2625.0000 |   62.5000 |   42917.22 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (8/p7(...)-, 6) [37] |  16,455.7 us |   224.85 us |   210.32 us |  1.26 |    0.02 |  1031.2500 |         - |   17115.45 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                      | (8/p7(...)-, 6) [37] |  10,077.0 us |    67.46 us |    63.10 us |  0.77 |    0.02 |  1046.8750 |   15.6250 |   17115.44 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (8/p7(...)-, 6) [37] |   4,534.2 us |    49.09 us |    45.92 us |  0.35 |    0.01 |   507.8125 |    7.8125 |    8399.77 KB |        0.20 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (8/p7(...)-, 6) [37] |   4,543.6 us |    46.58 us |    41.29 us |  0.35 |    0.01 |   507.8125 |    7.8125 |    8399.77 KB |        0.20 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (r3k2(...)1, 4) [73] | 287,097.0 us | 5,167.07 us | 4,580.48 us | 1.000 |    0.00 | 49000.0000 |  500.0000 |  801232.84 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r3k2(...)1, 4) [73] | 434,386.8 us | 1,566.22 us | 1,388.41 us | 1.513 |    0.02 |  6000.0000 |         - |  104639.09 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r3k2(...)1, 4) [73] | 257,067.5 us |   987.63 us |   923.83 us | 0.895 |    0.01 |  6000.0000 |         - |  104638.73 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r3k2(...)1, 4) [73] |     164.6 us |     0.64 us |     0.60 us | 0.001 |    0.00 |     6.1035 |         - |     102.78 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r3k2(...)1, 4) [73] |     166.6 us |     2.61 us |     2.44 us | 0.001 |    0.00 |     6.1035 |         - |     102.78 KB |       0.000 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (r4rk(...)0, 4) [77] | 265,197.6 us | 3,409.14 us | 3,188.91 us | 1.000 |    0.00 | 46500.0000 |  500.0000 |  765557.05 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r4rk(...)0, 4) [77] | 275,395.6 us |   628.77 us |   557.39 us | 1.038 |    0.01 |  5500.0000 |         - |   96332.82 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r4rk(...)0, 4) [77] | 206,780.7 us |   743.40 us |   580.39 us | 0.779 |    0.01 |  5666.6667 |         - |    96332.7 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r4rk(...)0, 4) [77] |     347.7 us |     0.50 us |     0.39 us | 0.001 |    0.00 |    12.6953 |         - |     208.53 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r4rk(...)0, 4) [77] |     341.5 us |     1.03 us |     0.97 us | 0.001 |    0.00 |    12.6953 |         - |     208.53 KB |       0.000 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (rnbq(...)1, 4) [61] |  14,317.8 us |   137.40 us |   128.52 us |  1.00 |    0.00 |  2671.8750 |   31.2500 |   43734.68 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (rnbq(...)1, 4) [61] |  15,662.9 us |    37.47 us |    33.22 us |  1.09 |    0.01 |   593.7500 |         - |    9762.01 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                      | (rnbq(...)1, 4) [61] |  11,864.3 us |    40.01 us |    35.47 us |  0.83 |    0.01 |   593.7500 |         - |       9762 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (rnbq(...)1, 4) [61] |     226.2 us |     1.48 us |     1.31 us |  0.02 |    0.00 |    13.4277 |         - |     220.05 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (rnbq(...)1, 4) [61] |     247.4 us |     1.40 us |     1.17 us |  0.02 |    0.00 |    13.1836 |         - |     220.05 KB |       0.005 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                                             | data                 | Mean         | Error        | StdDev       | Median       | Ratio | RatioSD | Gen0        | Gen1      | Allocated     | Alloc Ratio |
 *  |--------------------------------------------------- |--------------------- |-------------:|-------------:|-------------:|-------------:|------:|--------:|------------:|----------:|--------------:|------------:|
 *  | NewPosition                                        | (2K2r(...)1, 6) [38] | 596,556.4 us | 23,284.29 us | 67,921.36 us | 572,548.9 us |  1.00 |    0.00 | 174000.0000 | 2000.0000 | 1069286.11 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (2K2r(...)1, 6) [38] | 581,562.8 us | 32,111.07 us | 92,647.79 us | 569,710.1 us |  0.99 |    0.18 |  45000.0000 |         - |  278498.13 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                      | (2K2r(...)1, 6) [38] | 385,904.5 us | 20,523.47 us | 60,513.91 us | 367,557.5 us |  0.66 |    0.12 |  45000.0000 |  500.0000 |  278497.61 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (2K2r(...)1, 6) [38] |  26,102.9 us |    553.67 us |  1,552.56 us |  25,638.0 us |  0.04 |    0.01 |   5718.7500 |   62.5000 |   35057.85 KB |        0.03 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (2K2r(...)1, 6) [38] |  23,625.7 us |    431.98 us |    697.56 us |  23,592.0 us |  0.04 |    0.00 |   5718.7500 |   62.5000 |   35057.85 KB |        0.03 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (3K4/(...)1, 6) [38] | 520,639.0 us | 11,233.39 us | 31,499.62 us | 515,944.3 us |  1.00 |    0.00 | 174000.0000 | 2000.0000 | 1069286.11 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (3K4/(...)1, 6) [38] | 487,835.4 us |  9,495.67 us | 21,433.32 us | 481,627.1 us |  0.94 |    0.07 |  45000.0000 |         - |  278498.13 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                      | (3K4/(...)1, 6) [38] | 369,615.4 us | 13,953.87 us | 39,584.82 us | 365,639.3 us |  0.71 |    0.08 |  45000.0000 |  500.0000 |  278497.61 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (3K4/(...)1, 6) [38] |  27,328.3 us |  1,008.16 us |  2,908.77 us |  26,738.7 us |  0.05 |    0.01 |    875.0000 |         - |    5448.71 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (3K4/(...)1, 6) [38] |  23,015.6 us |    553.61 us |  1,606.12 us |  22,453.1 us |  0.04 |    0.00 |    875.0000 |         - |    5448.71 KB |       0.005 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (8/p7(...)-, 6) [37] |  18,770.8 us |    371.77 us |    890.73 us |  18,935.4 us |  1.00 |    0.00 |   7000.0000 |  125.0000 |   42917.24 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (8/p7(...)-, 6) [37] |  17,203.1 us |    340.58 us |    782.54 us |  17,155.4 us |  0.92 |    0.06 |   2781.2500 |   31.2500 |   17115.46 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                      | (8/p7(...)-, 6) [37] |  12,017.8 us |    239.95 us |    344.13 us |  12,025.8 us |  0.65 |    0.03 |   2781.2500 |   46.8750 |   17115.45 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (8/p7(...)-, 6) [37] |   5,453.7 us |    106.75 us |    109.62 us |   5,443.5 us |  0.30 |    0.02 |   1367.1875 |   23.4375 |    8401.25 KB |        0.20 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (8/p7(...)-, 6) [37] |   5,295.0 us |    105.24 us |    195.07 us |   5,260.0 us |  0.29 |    0.02 |   1367.1875 |   23.4375 |    8401.25 KB |        0.20 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (r3k2(...)1, 4) [73] | 521,874.5 us | 27,099.15 us | 78,187.23 us | 481,362.9 us | 1.000 |    0.00 | 130000.0000 | 1000.0000 |  801233.53 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r3k2(...)1, 4) [73] | 367,470.0 us |  4,942.33 us |  6,426.42 us | 366,073.7 us | 0.686 |    0.10 |  17000.0000 |         - |  104639.41 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r3k2(...)1, 4) [73] | 301,125.9 us |  3,070.02 us |  2,396.87 us | 300,703.8 us | 0.594 |    0.06 |  17000.0000 |         - |  104638.89 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r3k2(...)1, 4) [73] |     163.3 us |      2.86 us |      2.39 us |     164.2 us | 0.000 |    0.00 |     16.6016 |         - |     102.81 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r3k2(...)1, 4) [73] |     168.6 us |      2.50 us |      2.22 us |     168.4 us | 0.000 |    0.00 |     16.6016 |         - |     102.81 KB |       0.000 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (r4rk(...)0, 4) [77] | 386,108.7 us |  7,594.60 us |  8,126.14 us | 387,721.3 us | 1.000 |    0.00 | 124000.0000 | 1000.0000 |  765557.73 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r4rk(...)0, 4) [77] | 308,136.0 us |  5,898.38 us |  6,792.58 us | 307,536.4 us | 0.798 |    0.03 |  15500.0000 |         - |   96332.98 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r4rk(...)0, 4) [77] | 236,499.4 us |  3,099.38 us |  2,899.16 us | 237,067.0 us | 0.610 |    0.01 |  15666.6667 |         - |   96332.81 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r4rk(...)0, 4) [77] |     312.4 us |      6.07 us |      5.67 us |     312.1 us | 0.001 |    0.00 |     33.6914 |         - |      208.6 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r4rk(...)0, 4) [77] |     363.2 us |      6.15 us |      5.75 us |     362.3 us | 0.001 |    0.00 |     33.6914 |         - |      208.6 KB |       0.000 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (rnbq(...)1, 4) [61] |  20,415.4 us |    380.05 us |    355.50 us |  20,395.0 us |  1.00 |    0.00 |   7125.0000 |   93.7500 |    43734.7 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (rnbq(...)1, 4) [61] |  16,943.5 us |    144.12 us |    120.35 us |  16,999.3 us |  0.83 |    0.02 |   1593.7500 |         - |    9763.26 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                      | (rnbq(...)1, 4) [61] |  13,816.4 us |    273.34 us |    355.42 us |  13,743.5 us |  0.69 |    0.02 |   1593.7500 |   15.6250 |    9763.24 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (rnbq(...)1, 4) [61] |     251.5 us |      3.29 us |      2.91 us |     251.1 us |  0.01 |    0.00 |     35.6445 |         - |     220.12 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (rnbq(...)1, 4) [61] |     316.3 us |      2.19 us |      2.05 us |     315.9 us |  0.02 |    0.00 |     35.6445 |         - |     220.12 KB |       0.005 |
 *
 */

#pragma warning disable S101, S1854 // Types should be named in PascalCase

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class MakeUnmakeMove_implementation_Benchmark : BaseBenchmark
{
    public static IEnumerable<(string, int)> Data =>
    [
        (Constants.InitialPositionFEN, 4),
        (Constants.TrickyTestPositionFEN, 4),
        ("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10", 4),
        ("3K4/8/8/8/8/8/4p3/2k2R2 b - - 0 1", 6),
        ("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1", 6),
        ("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -", 6),
    ];

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
        public ulong UniqueIdentifier { get; private set; }

        public BitBoard[] PieceBitBoards { get; }

        public BitBoard[] OccupancyBitBoards { get; }

        public int[] Board { get; }

        public Side Side { get; private set; }

        public BoardSquare EnPassant { get; private set; }

        public byte Castle { get; private set; }

        public MakeMovePosition(string fen) : this(FENParser.ParseFEN(fen))
        {
        }

        public MakeMovePosition((BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, int[] board, Side Side, byte Castle, BoardSquare EnPassant,
            int HalfMoveClock/*, int FullMoveCounter*/) parsedFEN)
        {
            PieceBitBoards = parsedFEN.PieceBitBoards;
            OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
            Board = parsedFEN.board;
            Side = parsedFEN.Side;
            Castle = parsedFEN.Castle;
            EnPassant = parsedFEN.EnPassant;

            UniqueIdentifier = MakeMoveZobristTable.PositionHash(this);
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MakeMovePosition(MakeMovePosition position)
        {
            UniqueIdentifier = position.UniqueIdentifier;
            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Board = new int[64];
            Array.Copy(position.Board, Board, position.Board.Length);

            Side = position.Side;
            Castle = position.Castle;
            EnPassant = position.EnPassant;
        }

        /// <summary>
        /// Null moves constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable RCS1163, IDE0060 // Unused parameter.
        public MakeMovePosition(MakeMovePosition position, bool nullMove)
        {
            UniqueIdentifier = position.UniqueIdentifier;
            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Board = new int[64];
            Array.Copy(position.Board, Board, position.Board.Length);

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
            var uniqueIdentifierCopy = UniqueIdentifier;

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
            var uniqueIdentifierCopy = UniqueIdentifier;

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
            var uniqueIdentifierCopy = UniqueIdentifier;

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
                case SpecialMoveType.None:
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
                case SpecialMoveType.None:
                    {
                        if (move.IsCapture())
                        {
                            PieceBitBoards[move.CapturedPiece()].SetBit(targetSquare);
                            OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
                        }

                        break;
                    }
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
        /// This method is meant to be invoked only after <see cref="Position.MakeMove(int)"/>
        /// </summary>
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
        public readonly ulong ZobristKey;

        public readonly int CapturedPiece;

        public readonly BoardSquare EnPassant;

        public readonly byte Castle;

        public MakeMoveGameStateWithZobristKey(int capturedPiece, byte castle, BoardSquare enpassant, ulong zobristKey)
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
        private static readonly ulong[,] _table = Initialize();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PieceHash(int boardSquare, int piece) => _table[boardSquare, piece];

        /// <summary>
        /// Uses <see cref="Piece.P"/> and squares <see cref="BoardSquare.a1"/>-<see cref="BoardSquare.h1"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EnPassantHash(int enPassantSquare)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SideHash()
        {
            return _table[(int)BoardSquare.h8, (int)Piece.p];
        }

        /// <summary>
        /// Uses <see cref="Piece.p"/> and
        /// <see cref="BoardSquare.a8"/> for <see cref="CastlingRights.WK"/>, <see cref="BoardSquare.b8"/> for <see cref="CastlingRights.WQ"/>
        /// <see cref="BoardSquare.c8"/> for <see cref="CastlingRights.BK"/>, <see cref="BoardSquare.d8"/> for <see cref="CastlingRights.BQ"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong CastleHash(byte castle)
        {
            ulong combinedHash = 0;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PositionHash(MakeMovePosition position)
        {
            ulong positionHash = 0;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong[,] Initialize()
        {
            var zobristTable = new ulong[64, 12];
            var randomInstance = new LynxRandom(int.MaxValue);

            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
                {
                    zobristTable[squareIndex, pieceIndex] = randomInstance.NextUInt64();
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
        /// <param name="capturesOnly">Filters out all moves but captures</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Move> GenerateAllMoves(MakeMovePosition position, Move[]? movePool = null, bool capturesOnly = false)
        {
            movePool ??= new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
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
                        movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, position.Board[targetSquare]);
                    }
                }
            }
        }

        /// <summary>
        /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
        /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
        /// </summary>
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
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
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
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
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
        /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
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
