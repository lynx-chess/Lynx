/*
 * Consistent local reults
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *
 *  | Method                                             | data                 | Mean         | Error       | StdDev      | Ratio | RatioSD | Gen0       | Allocated     | Alloc Ratio |
 *  |--------------------------------------------------- |--------------------- |-------------:|------------:|------------:|------:|--------:|-----------:|--------------:|------------:|
 *  | NewPosition                                        | (2K2r(...)1, 6) [38] | 371,916.2 us | 3,357.88 us | 2,976.67 us |  1.00 |    0.00 | 13000.0000 | 1069286.11 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (2K2r(...)1, 6) [38] | 438,803.6 us | 3,102.83 us | 2,902.39 us |  1.18 |    0.01 |  3000.0000 |  278498.13 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                      | (2K2r(...)1, 6) [38] | 276,965.8 us | 2,425.10 us | 2,268.44 us |  0.75 |    0.01 |  3000.0000 |  278497.61 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (2K2r(...)1, 6) [38] |  21,992.5 us |   181.89 us |   161.24 us |  0.06 |    0.00 |   406.2500 |   35056.14 KB |        0.03 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (2K2r(...)1, 6) [38] |  22,462.7 us |   394.02 us |   349.29 us |  0.06 |    0.00 |   406.2500 |   35056.14 KB |        0.03 |
 *  |                                                    |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                        | (3K4/(...)1, 6) [38] | 388,579.9 us | 3,590.55 us | 3,358.60 us |  1.00 |    0.00 | 13000.0000 | 1069286.11 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (3K4/(...)1, 6) [38] | 441,682.8 us | 3,460.67 us | 3,067.79 us |  1.14 |    0.02 |  3000.0000 |  278498.13 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                      | (3K4/(...)1, 6) [38] | 260,931.1 us | 2,110.65 us | 1,974.30 us |  0.67 |    0.01 |  3000.0000 |  278497.61 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (3K4/(...)1, 6) [38] |  21,348.5 us |   129.00 us |   120.66 us |  0.05 |    0.00 |    62.5000 |    5446.33 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (3K4/(...)1, 6) [38] |  21,358.5 us |   151.00 us |   141.25 us |  0.05 |    0.00 |    62.5000 |    5446.33 KB |       0.005 |
 *  |                                                    |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                        | (8/p7(...)-, 6) [37] |  13,754.5 us |    54.57 us |    51.05 us |  1.00 |    0.00 |   515.6250 |    42915.8 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (8/p7(...)-, 6) [37] |  15,711.2 us |    26.07 us |    24.39 us |  1.14 |    0.00 |   187.5000 |   17113.08 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                      | (8/p7(...)-, 6) [37] |  10,406.1 us |    55.41 us |    49.12 us |  0.76 |    0.01 |   203.1250 |   17113.07 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (8/p7(...)-, 6) [37] |   4,618.6 us |    25.61 us |    23.96 us |  0.34 |    0.00 |   101.5625 |    8398.54 KB |        0.20 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (8/p7(...)-, 6) [37] |   4,712.0 us |    52.73 us |    49.32 us |  0.34 |    0.00 |   101.5625 |    8398.54 KB |        0.20 |
 *  |                                                    |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                        | (r3k2(...)1, 4) [73] | 319,595.5 us | 1,843.29 us | 1,724.21 us | 1.000 |    0.00 |  9500.0000 |  801233.01 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r3k2(...)1, 4) [73] | 347,561.9 us | 1,235.28 us |   964.43 us | 1.087 |    0.01 |  1000.0000 |  104639.41 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r3k2(...)1, 4) [73] | 244,314.2 us |   924.89 us |   819.89 us | 0.764 |    0.00 |  1000.0000 |  104638.72 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r3k2(...)1, 4) [73] |     161.3 us |     0.43 us |     0.33 us | 0.001 |    0.00 |     1.2207 |     102.76 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r3k2(...)1, 4) [73] |     168.9 us |     0.87 us |     0.77 us | 0.001 |    0.00 |     1.2207 |     102.76 KB |       0.000 |
 *  |                                                    |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                        | (r4rk(...)0, 4) [77] | 307,631.2 us | 2,571.62 us | 2,405.49 us | 1.000 |    0.00 |  9000.0000 |  765557.21 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r4rk(...)0, 4) [77] | 279,892.9 us | 1,750.95 us | 1,637.84 us | 0.910 |    0.01 |  1000.0000 |   96332.98 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r4rk(...)0, 4) [77] | 215,268.1 us | 1,380.54 us | 1,291.35 us | 0.700 |    0.01 |  1000.0000 |   96332.81 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r4rk(...)0, 4) [77] |     347.8 us |     1.24 us |     1.10 us | 0.001 |    0.00 |     2.4414 |      208.5 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r4rk(...)0, 4) [77] |     358.4 us |     1.61 us |     1.26 us | 0.001 |    0.00 |     2.4414 |      208.5 KB |       0.000 |
 *  |                                                    |                      |              |             |             |       |         |            |               |             |
 *  | NewPosition                                        | (rnbq(...)1, 4) [61] |  16,786.2 us |    34.14 us |    28.51 us |  1.00 |    0.00 |   531.2500 |   43733.27 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (rnbq(...)1, 4) [61] |  15,329.7 us |    24.72 us |    20.64 us |  0.91 |    0.00 |   109.3750 |    9760.58 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                      | (rnbq(...)1, 4) [61] |  12,382.0 us |    41.57 us |    36.85 us |  0.74 |    0.00 |   109.3750 |    9760.58 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (rnbq(...)1, 4) [61] |     250.9 us |     1.71 us |     1.51 us |  0.01 |    0.00 |     2.4414 |     220.02 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (rnbq(...)1, 4) [61] |     249.2 us |     1.73 us |     1.62 us |  0.01 |    0.00 |     2.4414 |     220.02 KB |       0.005 |
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
 *  | NewPosition                                        | (2K2r(...)1, 6) [38] | 340,274.7 us | 5,376.88 us | 5,029.54 us |  1.00 |    0.00 | 65000.0000 | 1000.0000 | 1069285.78 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (2K2r(...)1, 6) [38] | 442,876.5 us | 4,237.68 us | 3,538.65 us |  1.30 |    0.01 | 17000.0000 |         - |   278497.8 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                      | (2K2r(...)1, 6) [38] | 269,098.4 us |   554.73 us |   491.76 us |  0.79 |    0.01 | 17000.0000 |         - |  278497.45 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (2K2r(...)1, 6) [38] |  21,627.3 us |   416.92 us |   480.13 us |  0.06 |    0.00 |  2125.0000 |   31.2500 |   35057.84 KB |        0.03 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (2K2r(...)1, 6) [38] |  22,258.5 us |   383.98 us |   359.18 us |  0.07 |    0.00 |  2125.0000 |   31.2500 |   35057.84 KB |        0.03 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (3K4/(...)1, 6) [38] | 345,690.7 us | 6,394.45 us | 5,981.37 us |  1.00 |    0.00 | 65000.0000 | 1000.0000 | 1069285.78 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (3K4/(...)1, 6) [38] | 437,070.3 us | 2,823.89 us | 2,358.08 us |  1.26 |    0.02 | 17000.0000 |         - |   278497.8 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                      | (3K4/(...)1, 6) [38] | 270,378.5 us | 2,361.96 us | 2,209.38 us |  0.78 |    0.01 | 17000.0000 |         - |  278497.45 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (3K4/(...)1, 6) [38] |  21,175.1 us |    76.07 us |    71.15 us |  0.06 |    0.00 |   312.5000 |         - |    5447.08 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (3K4/(...)1, 6) [38] |  21,442.6 us |    72.25 us |    64.05 us |  0.06 |    0.00 |   312.5000 |         - |    5447.08 KB |       0.005 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (8/p7(...)-, 6) [37] |  12,793.4 us |   232.93 us |   217.88 us |  1.00 |    0.00 |  2625.0000 |   62.5000 |   42917.22 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (8/p7(...)-, 6) [37] |  16,194.7 us |    88.10 us |    78.10 us |  1.27 |    0.02 |  1031.2500 |         - |   17115.45 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                      | (8/p7(...)-, 6) [37] |  10,225.7 us |    72.37 us |    64.15 us |  0.80 |    0.01 |  1046.8750 |   15.6250 |   17115.44 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (8/p7(...)-, 6) [37] |   4,561.7 us |    30.06 us |    28.12 us |  0.36 |    0.01 |   507.8125 |    7.8125 |    8399.77 KB |        0.20 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (8/p7(...)-, 6) [37] |   4,648.8 us |    48.80 us |    45.65 us |  0.36 |    0.01 |   507.8125 |    7.8125 |    8399.77 KB |        0.20 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (r3k2(...)1, 4) [73] | 289,780.8 us | 3,074.26 us | 2,725.25 us | 1.000 |    0.00 | 49000.0000 |  500.0000 |  801232.84 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r3k2(...)1, 4) [73] | 355,328.7 us | 1,518.35 us | 1,420.27 us | 1.227 |    0.01 |  6000.0000 |         - |  104639.09 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r3k2(...)1, 4) [73] | 242,989.5 us | 1,527.59 us | 1,428.91 us | 0.839 |    0.01 |  6333.3333 |         - |  104638.61 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r3k2(...)1, 4) [73] |     160.4 us |     0.72 us |     0.67 us | 0.001 |    0.00 |     6.1035 |         - |     102.78 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r3k2(...)1, 4) [73] |     168.7 us |     0.98 us |     0.87 us | 0.001 |    0.00 |     6.1035 |         - |     102.78 KB |       0.000 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (r4rk(...)0, 4) [77] | 257,510.9 us | 5,042.96 us | 4,717.19 us | 1.000 |    0.00 | 46500.0000 |  500.0000 |  765557.05 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r4rk(...)0, 4) [77] | 278,350.4 us |   753.56 us |   704.88 us | 1.081 |    0.02 |  5500.0000 |         - |   96332.82 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r4rk(...)0, 4) [77] | 208,272.9 us |   985.54 us |   822.97 us | 0.810 |    0.01 |  5666.6667 |         - |    96332.7 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r4rk(...)0, 4) [77] |     309.7 us |     2.04 us |     1.90 us | 0.001 |    0.00 |    12.6953 |         - |     208.53 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r4rk(...)0, 4) [77] |     357.8 us |     3.60 us |     3.37 us | 0.001 |    0.00 |    12.6953 |         - |     208.53 KB |       0.000 |
 *  |                                                    |                      |              |             |             |       |         |            |           |               |             |
 *  | NewPosition                                        | (rnbq(...)1, 4) [61] |  14,612.6 us |   221.92 us |   196.73 us |  1.00 |    0.00 |  2671.8750 |   31.2500 |   43734.68 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (rnbq(...)1, 4) [61] |  15,658.0 us |    27.86 us |    24.70 us |  1.07 |    0.02 |   593.7500 |         - |    9762.01 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                      | (rnbq(...)1, 4) [61] |  11,908.9 us |    38.41 us |    34.05 us |  0.82 |    0.01 |   593.7500 |         - |       9762 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (rnbq(...)1, 4) [61] |     225.9 us |     0.83 us |     0.70 us |  0.02 |    0.00 |    13.4277 |         - |     220.05 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (rnbq(...)1, 4) [61] |     256.0 us |     0.86 us |     0.81 us |  0.02 |    0.00 |    13.1836 |         - |     220.05 KB |       0.005 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX
 *
 *  | Method                                             | data                 | Mean         | Error        | StdDev       | Median       | Ratio | RatioSD | Gen0        | Gen1      | Allocated     | Alloc Ratio |
 *  |--------------------------------------------------- |--------------------- |-------------:|-------------:|-------------:|-------------:|------:|--------:|------------:|----------:|--------------:|------------:|
 *  | NewPosition                                        | (2K2r(...)1, 6) [38] | 540,876.0 us | 10,642.51 us | 13,459.37 us | 538,884.9 us |  1.00 |    0.00 | 174000.0000 | 2000.0000 | 1069286.11 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (2K2r(...)1, 6) [38] | 564,020.4 us |  4,597.97 us |  3,839.51 us | 562,780.9 us |  1.04 |    0.02 |  45000.0000 |         - |  278498.13 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey                      | (2K2r(...)1, 6) [38] | 466,090.7 us |  3,744.35 us |  3,502.46 us | 466,143.4 us |  0.86 |    0.02 |  45000.0000 |         - |  278498.13 KB |        0.26 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (2K2r(...)1, 6) [38] |  32,505.8 us |    618.37 us |    736.12 us |  32,372.7 us |  0.06 |    0.00 |   5687.5000 |   62.5000 |   35057.89 KB |        0.03 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (2K2r(...)1, 6) [38] |  33,390.2 us |    659.56 us |  1,317.21 us |  32,919.5 us |  0.06 |    0.00 |   5687.5000 |   62.5000 |   35057.89 KB |        0.03 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (3K4/(...)1, 6) [38] | 548,045.1 us | 10,821.74 us | 15,862.37 us | 549,152.9 us |  1.00 |    0.00 | 174000.0000 | 2000.0000 | 1069286.11 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (3K4/(...)1, 6) [38] | 556,688.0 us |  5,922.58 us |  4,945.62 us | 555,281.0 us |  1.02 |    0.02 |  45000.0000 |         - |  278498.13 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey                      | (3K4/(...)1, 6) [38] | 463,567.3 us |  2,885.32 us |  2,698.93 us | 463,340.7 us |  0.86 |    0.02 |  45000.0000 |         - |  278498.13 KB |       0.260 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (3K4/(...)1, 6) [38] |  31,099.1 us |    603.40 us |    991.40 us |  30,695.8 us |  0.06 |    0.00 |    875.0000 |         - |    5448.71 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (3K4/(...)1, 6) [38] |  30,692.5 us |    575.28 us |  1,411.18 us |  30,427.9 us |  0.06 |    0.00 |    833.3333 |         - |    5448.63 KB |       0.005 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (8/p7(...)-, 6) [37] |  20,937.3 us |    387.25 us |    362.23 us |  20,862.1 us |  1.00 |    0.00 |   7000.0000 |  125.0000 |   42917.24 KB |        1.00 |
 *  | MakeUnmakeMove_Original                            | (8/p7(...)-, 6) [37] |  21,898.1 us |    419.94 us |    372.27 us |  21,922.7 us |  1.05 |    0.03 |   2781.2500 |   31.2500 |   17115.46 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey                      | (8/p7(...)-, 6) [37] |  18,003.7 us |    334.82 us |    313.19 us |  18,006.5 us |  0.86 |    0.02 |   2781.2500 |   31.2500 |   17115.46 KB |        0.40 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (8/p7(...)-, 6) [37] |   6,907.5 us |    137.10 us |    192.20 us |   6,890.6 us |  0.33 |    0.01 |   1367.1875 |   23.4375 |    8401.25 KB |        0.20 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (8/p7(...)-, 6) [37] |   9,188.5 us |    495.81 us |  1,461.91 us |   9,247.5 us |  0.41 |    0.07 |   1367.1875 |   23.4375 |    8401.25 KB |        0.20 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (r3k2(...)1, 4) [73] | 572,501.0 us | 28,498.69 us | 84,029.03 us | 573,923.6 us | 1.000 |    0.00 | 130000.0000 | 1000.0000 |  801233.53 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r3k2(...)1, 4) [73] | 663,197.3 us | 33,037.67 us | 97,412.33 us | 645,325.6 us | 1.188 |    0.27 |  17000.0000 |         - |  104639.41 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r3k2(...)1, 4) [73] | 419,634.7 us | 23,843.13 us | 68,792.89 us | 393,369.1 us | 0.749 |    0.17 |  17000.0000 |         - |  104639.41 KB |       0.131 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r3k2(...)1, 4) [73] |     267.1 us |     13.87 us |     40.25 us |     256.7 us | 0.000 |    0.00 |     16.6016 |         - |     102.81 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r3k2(...)1, 4) [73] |     232.9 us |      4.65 us |     10.58 us |     231.6 us | 0.000 |    0.00 |     16.6016 |         - |     102.81 KB |       0.000 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (r4rk(...)0, 4) [77] | 485,734.5 us |  9,694.63 us | 25,708.79 us | 482,125.0 us | 1.000 |    0.00 | 124000.0000 | 1000.0000 |  765557.73 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (r4rk(...)0, 4) [77] | 511,174.7 us | 10,162.18 us | 24,347.94 us | 515,608.8 us | 1.059 |    0.07 |  15000.0000 |         - |   96333.51 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey                      | (r4rk(...)0, 4) [77] | 324,566.1 us |  6,379.08 us | 11,981.46 us | 321,145.3 us | 0.675 |    0.05 |  15500.0000 |         - |   96332.98 KB |       0.126 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (r4rk(...)0, 4) [77] |     535.0 us |     10.60 us |      9.92 us |     535.7 us | 0.001 |    0.00 |     33.2031 |         - |      208.6 KB |       0.000 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (r4rk(...)0, 4) [77] |     531.5 us |      8.47 us |      7.92 us |     530.2 us | 0.001 |    0.00 |     33.2031 |         - |      208.6 KB |       0.000 |
 *  |                                                    |                      |              |              |              |              |       |         |             |           |               |             |
 *  | NewPosition                                        | (rnbq(...)1, 4) [61] |  25,527.8 us |    353.02 us |    312.94 us |  25,563.4 us |  1.00 |    0.00 |   7125.0000 |   93.7500 |    43734.7 KB |       1.000 |
 *  | MakeUnmakeMove_Original                            | (rnbq(...)1, 4) [61] |  26,917.2 us |    524.57 us |    515.20 us |  26,961.6 us |  1.05 |    0.03 |   1593.7500 |         - |    9763.26 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey                      | (rnbq(...)1, 4) [61] |  17,795.6 us |    305.66 us |    386.56 us |  17,654.9 us |  0.70 |    0.01 |   1593.7500 |         - |    9763.26 KB |       0.223 |
 *  | MakeUnmakeMove_WithZobristKey_PreSwitchSpecialMove | (rnbq(...)1, 4) [61] |     363.8 us |      5.50 us |      5.14 us |     363.5 us |  0.01 |    0.00 |     35.6445 |         - |     220.12 KB |       0.005 |
 *  | MakeUnmakeMove_WithZobristKey_SwitchSpecialMove    | (rnbq(...)1, 4) [61] |     371.4 us |      7.35 us |      8.17 us |     370.2 us |  0.01 |    0.00 |     35.6445 |         - |     220.12 KB |       0.005 |
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
