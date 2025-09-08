/*
 * Using Span.Split()
 *
 *  BenchmarkDotNet v0.13.8, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method              | positionCommand      | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------- |--------------------- |-----------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | ParseGame_Original  | position startpos    |   5.437 us | 0.0792 us | 0.0741 us |  1.00 |    0.00 | 1.2131 | 0.1144 |   29.8 KB |        1.00 |
 *  | ParseGame_Improved1 | position startpos    |   5.339 us | 0.0611 us | 0.0542 us |  0.98 |    0.02 | 1.2054 | 0.1068 |  29.68 KB |        1.00 |
 *  | ParseGame_Improved2 | position startpos    |   5.265 us | 0.0925 us | 0.0865 us |  0.97 |    0.01 | 1.2054 | 0.1068 |  29.68 KB |        1.00 |
 *  | ParseGame_Improved3 | position startpos    |   5.423 us | 0.0604 us | 0.0535 us |  1.00 |    0.01 | 1.2131 | 0.1144 |   29.8 KB |        1.00 |
 *  | ParseGame_Improved4 | position startpos    |   5.745 us | 0.1125 us | 0.1104 us |  1.06 |    0.02 | 1.2131 | 0.1144 |  29.77 KB |        1.00 |
 *  | ParseGame_Improved5 | position startpos    |   5.429 us | 0.1045 us | 0.1499 us |  1.01 |    0.03 | 1.2131 | 0.1144 |  29.77 KB |        1.00 |
 *  |                     |                      |            |           |           |       |         |        |        |           |             |
 *  | ParseGame_Original  | posi(...)b7b6 [193]  |  26.556 us | 0.1636 us | 0.1450 us |  1.00 |    0.00 | 1.6479 | 0.1526 |  40.55 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)b7b6 [193]  |  26.311 us | 0.2117 us | 0.1980 us |  0.99 |    0.01 | 1.5869 | 0.1526 |  39.07 KB |        0.96 |
 *  | ParseGame_Improved2 | posi(...)b7b6 [193]  |  25.852 us | 0.2180 us | 0.2039 us |  0.97 |    0.01 | 1.4954 | 0.1221 |  37.06 KB |        0.91 |
 *  | ParseGame_Improved3 | posi(...)b7b6 [193]  |  27.533 us | 0.2404 us | 0.2249 us |  1.04 |    0.01 | 1.5564 | 0.1526 |  38.54 KB |        0.95 |
 *  | ParseGame_Improved4 | posi(...)b7b6 [193]  |  26.357 us | 0.2285 us | 0.2138 us |  0.99 |    0.01 | 1.4954 | 0.1221 |  36.94 KB |        0.91 |
 *  | ParseGame_Improved5 | posi(...)b7b6 [193]  |  25.903 us | 0.1491 us | 0.1395 us |  0.98 |    0.01 | 1.4954 | 0.1221 |  36.94 KB |        0.91 |
 *  |                     |                      |            |           |           |       |         |        |        |           |             |
 *  | ParseGame_Original  | posi(...)f3g3 [353]  |  43.593 us | 0.2400 us | 0.2245 us |  1.00 |    0.00 | 2.0142 | 0.1831 |  50.37 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)f3g3 [353]  |  42.431 us | 0.1899 us | 0.1776 us |  0.97 |    0.01 | 1.8921 | 0.1831 |  47.63 KB |        0.95 |
 *  | ParseGame_Improved2 | posi(...)f3g3 [353]  |  41.193 us | 0.1674 us | 0.1565 us |  0.94 |    0.01 | 1.7700 | 0.1221 |  43.81 KB |        0.87 |
 *  | ParseGame_Improved3 | posi(...)f3g3 [353]  |  42.969 us | 0.4555 us | 0.4038 us |  0.99 |    0.01 | 1.8921 | 0.1831 |  46.54 KB |        0.92 |
 *  | ParseGame_Improved4 | posi(...)f3g3 [353]  |  41.829 us | 0.2377 us | 0.2107 us |  0.96 |    0.01 | 1.7700 | 0.1221 |  43.69 KB |        0.87 |
 *  | ParseGame_Improved5 | posi(...)f3g3 [353]  |  41.780 us | 0.3129 us | 0.2927 us |  0.96 |    0.01 | 1.7700 | 0.1221 |  43.69 KB |        0.87 |
 *  |                     |                      |            |           |           |       |         |        |        |           |             |
 *  | ParseGame_Original  | posi(...)g4g8 [979]  |  96.149 us | 0.6131 us | 0.5735 us |  1.00 |    0.00 | 3.5400 | 0.3662 |  88.81 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)g4g8 [979]  |  98.144 us | 0.4949 us | 0.4387 us |  1.02 |    0.01 | 3.2959 | 0.3662 |  81.19 KB |        0.91 |
 *  | ParseGame_Improved2 | posi(...)g4g8 [979]  |  97.826 us | 0.3846 us | 0.3597 us |  1.02 |    0.01 | 2.8076 | 0.2441 |  70.29 KB |        0.79 |
 *  | ParseGame_Improved3 | posi(...)g4g8 [979]  |  97.370 us | 0.3111 us | 0.2910 us |  1.01 |    0.01 | 3.1738 | 0.2441 |  77.91 KB |        0.88 |
 *  | ParseGame_Improved4 | posi(...)g4g8 [979]  |  91.612 us | 0.3654 us | 0.3418 us |  0.95 |    0.01 | 2.8076 | 0.2441 |  70.17 KB |        0.79 |
 *  | ParseGame_Improved5 | posi(...)g4g8 [979]  |  88.534 us | 0.3452 us | 0.3229 us |  0.92 |    0.01 | 2.8076 | 0.2441 |  70.17 KB |        0.79 |
 *  |                     |                      |            |           |           |       |         |        |        |           |             |
 *  | ParseGame_Original  | posi(...)h3f1 [2984] | 276.602 us | 0.8504 us | 0.7538 us |  1.00 |    0.00 | 8.3008 | 0.9766 | 211.79 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)h3f1 [2984] | 262.196 us | 0.8526 us | 0.7975 us |  0.95 |    0.00 | 7.3242 | 0.9766 |  188.5 KB |        0.89 |
 *  | ParseGame_Improved2 | posi(...)h3f1 [2984] | 246.968 us | 0.9181 us | 0.8588 us |  0.89 |    0.00 | 5.8594 | 0.4883 | 154.89 KB |        0.73 |
 *  | ParseGame_Improved3 | posi(...)h3f1 [2984] | 260.619 us | 0.9472 us | 0.8860 us |  0.94 |    0.00 | 6.8359 | 0.4883 | 178.17 KB |        0.84 |
 *  | ParseGame_Improved4 | posi(...)h3f1 [2984] | 251.254 us | 2.8627 us | 2.6778 us |  0.91 |    0.01 | 5.8594 | 0.4883 | 154.77 KB |        0.73 |
 *  | ParseGame_Improved5 | posi(...)h3f1 [2984] | 250.528 us | 0.9854 us | 0.9217 us |  0.91 |    0.00 | 5.8594 | 0.4883 | 154.77 KB |        0.73 |
 *
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method              | positionCommand      | Mean       | Error     | StdDev     | Median     | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------- |--------------------- |-----------:|----------:|-----------:|-----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | ParseGame_Original  | position startpos    |   7.131 us | 0.1044 us |  0.0925 us |   7.135 us |  1.00 |    0.00 |  1.6251 | 0.1450 |   29.8 KB |        1.00 |
 *  | ParseGame_Improved1 | position startpos    |   6.985 us | 0.1332 us |  0.1246 us |   7.029 us |  0.98 |    0.02 |  1.6251 | 0.1602 |  29.68 KB |        1.00 |
 *  | ParseGame_Improved2 | position startpos    |   6.939 us | 0.1108 us |  0.1037 us |   6.910 us |  0.97 |    0.02 |  1.6251 | 0.1602 |  29.68 KB |        1.00 |
 *  | ParseGame_Improved3 | position startpos    |   6.999 us | 0.1370 us |  0.1829 us |   7.017 us |  0.97 |    0.03 |  1.6251 | 0.1450 |   29.8 KB |        1.00 |
 *  | ParseGame_Improved4 | position startpos    |   7.350 us | 0.1380 us |  0.1291 us |   7.329 us |  1.03 |    0.02 |  1.6251 | 0.1602 |  29.77 KB |        1.00 |
 *  | ParseGame_Improved5 | position startpos    |   7.292 us | 0.0724 us |  0.0678 us |   7.279 us |  1.02 |    0.01 |  1.6251 | 0.1602 |  29.77 KB |        1.00 |
 *  |                     |                      |            |           |            |            |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)b7b6 [193]  |  29.191 us | 0.1627 us |  0.1522 us |  29.125 us |  1.00 |    0.00 |  2.1973 | 0.2136 |  40.55 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)b7b6 [193]  |  28.624 us | 0.1355 us |  0.1202 us |  28.629 us |  0.98 |    0.01 |  2.1362 | 0.2136 |  39.07 KB |        0.96 |
 *  | ParseGame_Improved2 | posi(...)b7b6 [193]  |  27.901 us | 0.1893 us |  0.1581 us |  27.874 us |  0.96 |    0.01 |  2.0142 | 0.1831 |  37.06 KB |        0.91 |
 *  | ParseGame_Improved3 | posi(...)b7b6 [193]  |  28.070 us | 0.2160 us |  0.2020 us |  28.027 us |  0.96 |    0.01 |  2.1057 | 0.1831 |  38.54 KB |        0.95 |
 *  | ParseGame_Improved4 | posi(...)b7b6 [193]  |  28.338 us | 0.1551 us |  0.1375 us |  28.311 us |  0.97 |    0.01 |  2.0142 | 0.1831 |  36.94 KB |        0.91 |
 *  | ParseGame_Improved5 | posi(...)b7b6 [193]  |  28.230 us | 0.0971 us |  0.0810 us |  28.261 us |  0.97 |    0.01 |  2.0142 | 0.1831 |  36.94 KB |        0.91 |
 *  |                     |                      |            |           |            |            |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)f3g3 [353]  |  47.699 us | 0.3330 us |  0.2952 us |  47.611 us |  1.00 |    0.00 |  2.7466 | 0.2441 |  50.37 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)f3g3 [353]  |  44.214 us | 0.2395 us |  0.2240 us |  44.106 us |  0.93 |    0.01 |  2.5635 | 0.2441 |  47.63 KB |        0.95 |
 *  | ParseGame_Improved2 | posi(...)f3g3 [353]  |  44.423 us | 0.2258 us |  0.2002 us |  44.444 us |  0.93 |    0.01 |  2.3804 | 0.1831 |  43.81 KB |        0.87 |
 *  | ParseGame_Improved3 | posi(...)f3g3 [353]  |  46.899 us | 0.1712 us |  0.1336 us |  46.884 us |  0.98 |    0.01 |  2.5024 | 0.2441 |  46.55 KB |        0.92 |
 *  | ParseGame_Improved4 | posi(...)f3g3 [353]  |  46.007 us | 0.3615 us |  0.3382 us |  45.990 us |  0.96 |    0.01 |  2.3804 | 0.1831 |  43.69 KB |        0.87 |
 *  | ParseGame_Improved5 | posi(...)f3g3 [353]  |  45.505 us | 0.3338 us |  0.3122 us |  45.474 us |  0.95 |    0.01 |  2.3804 | 0.1831 |  43.69 KB |        0.87 |
 *  |                     |                      |            |           |            |            |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)g4g8 [979]  | 104.450 us | 0.9623 us |  0.8530 us | 104.297 us |  1.00 |    0.00 |  4.7607 | 0.4883 |  88.81 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)g4g8 [979]  | 101.191 us | 0.5946 us |  0.5562 us | 101.197 us |  0.97 |    0.01 |  4.3945 | 0.4883 |   81.2 KB |        0.91 |
 *  | ParseGame_Improved2 | posi(...)g4g8 [979]  | 100.221 us | 0.8324 us |  0.7379 us | 100.007 us |  0.96 |    0.01 |  3.7842 | 0.3662 |  70.29 KB |        0.79 |
 *  | ParseGame_Improved3 | posi(...)g4g8 [979]  | 107.524 us | 0.4763 us |  0.4455 us | 107.478 us |  1.03 |    0.01 |  4.1504 | 0.2441 |  77.91 KB |        0.88 |
 *  | ParseGame_Improved4 | posi(...)g4g8 [979]  |  99.805 us | 0.4638 us |  0.4111 us |  99.675 us |  0.96 |    0.01 |  3.7842 | 0.3662 |  70.18 KB |        0.79 |
 *  | ParseGame_Improved5 | posi(...)g4g8 [979]  | 102.839 us | 0.4866 us |  0.4314 us | 102.638 us |  0.98 |    0.01 |  3.7842 | 0.3662 |  70.18 KB |        0.79 |
 *  |                     |                      |            |           |            |            |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)h3f1 [2984] | 282.097 us | 0.9685 us |  0.8586 us | 281.797 us |  1.00 |    0.00 | 11.2305 | 1.4648 |  211.8 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)h3f1 [2984] | 262.435 us | 2.5436 us |  2.3793 us | 261.823 us |  0.93 |    0.01 | 10.2539 | 1.4648 | 188.51 KB |        0.89 |
 *  | ParseGame_Improved2 | posi(...)h3f1 [2984] | 267.611 us | 2.9339 us |  2.6008 us | 266.801 us |  0.95 |    0.01 |  8.3008 | 0.4883 |  154.9 KB |        0.73 |
 *  | ParseGame_Improved3 | posi(...)h3f1 [2984] | 273.903 us | 0.8124 us |  0.6784 us | 274.134 us |  0.97 |    0.00 |  9.2773 | 0.4883 | 178.18 KB |        0.84 |
 *  | ParseGame_Improved4 | posi(...)h3f1 [2984] | 274.281 us | 5.4806 us | 10.0216 us | 269.509 us |  1.02 |    0.01 |  8.3008 | 0.4883 | 154.78 KB |        0.73 |
 *  | ParseGame_Improved5 | posi(...)h3f1 [2984] | 269.832 us | 3.3739 us |  3.1559 us | 269.159 us |  0.96 |    0.01 |  8.3008 | 0.4883 | 154.78 KB |        0.73 |
 *
 *
 *  BenchmarkDotNet v0.13.8, macOS Monterey 12.6.8 (21G725) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *
 *
 *  | Method              | positionCommand      | Mean       | Error     | StdDev     | Median     | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------- |--------------------- |-----------:|----------:|-----------:|-----------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | ParseGame_Original  | position startpos    |   6.105 us | 0.1220 us |  0.3082 us |   6.138 us |  1.00 |    0.00 |  4.8523 | 0.1755 |  29.81 KB |        1.00 |
 *  | ParseGame_Improved1 | position startpos    |   6.104 us | 0.1190 us |  0.0993 us |   6.097 us |  0.96 |    0.04 |  4.8370 | 0.2823 |  29.69 KB |        1.00 |
 *  | ParseGame_Improved2 | position startpos    |   6.589 us | 0.1916 us |  0.5650 us |   6.582 us |  1.10 |    0.11 |  4.8370 | 0.2823 |  29.69 KB |        1.00 |
 *  | ParseGame_Improved3 | position startpos    |   7.067 us | 0.1363 us |  0.1674 us |   7.089 us |  1.13 |    0.06 |  4.8523 | 0.1755 |  29.81 KB |        1.00 |
 *  | ParseGame_Improved4 | position startpos    |   6.645 us | 0.1136 us |  0.1007 us |   6.662 us |  1.04 |    0.03 |  4.8523 | 0.4196 |  29.78 KB |        1.00 |
 *  | ParseGame_Improved5 | position startpos    |   6.238 us | 0.1190 us |  0.3278 us |   6.177 us |  1.03 |    0.08 |  4.8523 | 0.4196 |  29.78 KB |        1.00 |
 *  |                     |                      |            |           |            |            |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)b7b6 [193]  |  29.603 us | 0.4740 us |  0.3958 us |  29.719 us |  1.00 |    0.00 |  6.5918 | 0.3052 |  40.57 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)b7b6 [193]  |  28.784 us | 0.5684 us |  0.6546 us |  28.758 us |  0.97 |    0.03 |  6.3477 | 0.5188 |  39.08 KB |        0.96 |
 *  | ParseGame_Improved2 | posi(...)b7b6 [193]  |  29.156 us | 0.3219 us |  0.2853 us |  29.118 us |  0.98 |    0.02 |  6.0120 | 0.3662 |  37.07 KB |        0.91 |
 *  | ParseGame_Improved3 | posi(...)b7b6 [193]  |  31.017 us | 0.6139 us |  1.5625 us |  30.538 us |  1.11 |    0.05 |  6.2866 | 0.3052 |  38.56 KB |        0.95 |
 *  | ParseGame_Improved4 | posi(...)b7b6 [193]  |  29.579 us | 0.4176 us |  0.3487 us |  29.619 us |  1.00 |    0.01 |  6.0120 | 0.2747 |  36.96 KB |        0.91 |
 *  | ParseGame_Improved5 | posi(...)b7b6 [193]  |  31.900 us | 0.6327 us |  1.4538 us |  32.075 us |  1.09 |    0.04 |  5.9814 | 0.3052 |  36.96 KB |        0.91 |
 *  |                     |                      |            |           |            |            |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)f3g3 [353]  |  51.769 us | 1.0324 us |  1.6074 us |  51.707 us |  1.00 |    0.00 |  8.1787 | 0.6714 |  50.38 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)f3g3 [353]  |  48.668 us | 0.9140 us |  1.0526 us |  48.698 us |  0.96 |    0.03 |  7.7515 | 0.6714 |  47.65 KB |        0.95 |
 *  | ParseGame_Improved2 | posi(...)f3g3 [353]  |  50.798 us | 0.9196 us |  0.8602 us |  50.849 us |  0.99 |    0.03 |  7.1411 | 0.6104 |  43.83 KB |        0.87 |
 *  | ParseGame_Improved3 | posi(...)f3g3 [353]  |  46.783 us | 0.7563 us |  0.6704 us |  46.839 us |  0.92 |    0.03 |  7.5684 | 0.5493 |  46.56 KB |        0.92 |
 *  | ParseGame_Improved4 | posi(...)f3g3 [353]  |  48.164 us | 0.9535 us |  1.7908 us |  48.609 us |  0.93 |    0.04 |  7.0801 | 0.4272 |  43.71 KB |        0.87 |
 *  | ParseGame_Improved5 | posi(...)f3g3 [353]  |  47.805 us | 0.9146 us |  1.1566 us |  47.799 us |  0.93 |    0.03 |  7.0801 | 0.4272 |  43.71 KB |        0.87 |
 *  |                     |                      |            |           |            |            |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)g4g8 [979]  | 116.992 us | 2.2891 us |  2.7250 us | 116.574 us |  1.00 |    0.00 | 14.4043 | 1.4648 |  88.85 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)g4g8 [979]  | 108.038 us | 2.1497 us |  4.1928 us | 106.080 us |  0.93 |    0.04 | 13.1836 | 1.2207 |  81.23 KB |        0.91 |
 *  | ParseGame_Improved2 | posi(...)g4g8 [979]  | 100.069 us | 1.9363 us |  2.4488 us |  99.096 us |  0.86 |    0.03 | 11.3525 | 0.8545 |  70.32 KB |        0.79 |
 *  | ParseGame_Improved3 | posi(...)g4g8 [979]  | 101.941 us | 0.5298 us |  0.4424 us | 101.996 us |  0.88 |    0.02 | 12.6953 | 0.9766 |  77.94 KB |        0.88 |
 *  | ParseGame_Improved4 | posi(...)g4g8 [979]  | 100.997 us | 1.8846 us |  1.7629 us | 100.144 us |  0.87 |    0.03 | 11.3525 | 0.8545 |  70.21 KB |        0.79 |
 *  | ParseGame_Improved5 | posi(...)g4g8 [979]  | 102.337 us | 2.0326 us |  2.2593 us | 101.967 us |  0.87 |    0.03 | 11.3525 | 0.8545 |  70.21 KB |        0.79 |
 *  |                     |                      |            |           |            |            |       |         |         |        |           |             |
 *  | ParseGame_Original  | posi(...)h3f1 [2984] | 303.278 us | 6.0550 us | 11.3727 us | 309.165 us |  1.00 |    0.00 | 34.1797 | 3.9063 | 211.88 KB |        1.00 |
 *  | ParseGame_Improved1 | posi(...)h3f1 [2984] | 275.779 us | 5.5037 us |  6.5517 us | 275.073 us |  0.93 |    0.05 | 30.7617 | 4.3945 | 188.59 KB |        0.89 |
 *  | ParseGame_Improved2 | posi(...)h3f1 [2984] | 269.410 us | 4.1198 us |  4.2307 us | 268.815 us |  0.92 |    0.04 | 24.9023 | 1.9531 | 154.96 KB |        0.73 |
 *  | ParseGame_Improved3 | posi(...)h3f1 [2984] | 279.300 us | 4.1479 us |  3.8800 us | 279.708 us |  0.96 |    0.03 | 28.8086 | 2.4414 | 178.25 KB |        0.84 |
 *  | ParseGame_Improved4 | posi(...)h3f1 [2984] | 268.161 us | 3.8298 us |  3.5824 us | 268.346 us |  0.92 |    0.03 | 24.9023 | 1.9531 | 154.84 KB |        0.73 |
 *  | ParseGame_Improved5 | posi(...)h3f1 [2984] | 263.004 us | 3.1736 us |  2.9686 us | 263.624 us |  0.91 |    0.04 | 24.9023 | 1.9531 | 154.84 KB |        0.73 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark;

#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both

public partial class ParseGame_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
    [
        "position startpos",    // No moves
        "position startpos moves d2d4 d7d5 b1c3 g8f6 f2f3 c7c5 e2e3 b8c6 a2a3 e7e6 f1e2 c8d7 d4c5 f8c5 h2h3 f6h5 h1h2 d8h4 e1d2 h4f2 c3d5 e6d5 d2c3 c5e3 c1d2 c6a5 a3a4 e3d4 c3b4 d4c5 b4a5 a7a6 a1a2 b7b6",    // 17 moves
        "position startpos moves d2d4 d7d5 g1f3 g8f6 e2e3 b8c6 f1e2 c8f5 b1c3 f6e4 e1g1 e4c3 b2c3 e7e6 a1b1 a8b8 c1b2 b7b5 f3e5 c6e5 d4e5 f8c5 e2f3 e8g8 f1e1 d8h4 g2g4 f5g6 b1c1 f7f6 e5f6 h4f6 g1g2 g6e4 f3e4 f6f2 g2h1 d5e4 d1e2 f2f3 e2f3 e4f3 e3e4 f3f2 e1f1 f8f4 c1d1 f4e4 g4g5 e4g4 b2c1 b8f8 a2a3 e6e5 h2h3 g4g3 c1b2 f8f3 d1d8 g8f7 d8d7 f7e6 d7c7 g3h3 h1g2 f3g3",    // 36 moves
        "position startpos moves g1f3 e7e6 e2e4 b7b6 d2d4 g8f6 e4e5 f6d5 c2c4 f8b4 b1d2 d5e7 a2a3 b4d2 c1d2 d7d6 d2c3 d6e5 f3e5 c8b7 f1e2 e8g8 e2f3 b7f3 d1f3 b8d7 a1d1 f7f6 e5c6 e7c6 f3c6 f8e8 e1g1 d7f8 f1e1 d8d7 c6f3 f8g6 f3b7 a7a5 c4c5 a8b8 b7e4 d7b5 d4d5 e6e5 d5d6 b5c5 d6d7 e8d8 e1e3 g6f4 g2g3 f4h3 g1g2 h3g5 e4a4 g5f7 e3d3 f7d6 d3d5 c5c4 a4c4 d6c4 b2b4 a5a4 d1c1 c4d6 c3e5 d8d7 e5d6 d7d6 d5d6 c7d6 c1c6 d6d5 c6d6 b8c8 d6d5 c8c3 b4b5 c3b3 d5d4 b3a3 d4d6 a3a1 d6b6 a4a3 b6b8 g8f7 b5b6 a1b1 b8a8 b1b6 a8a3 b6b8 a3a7 f7g8 h2h4 b8e8 g3g4 e8d8 g2f3 d8d3 f3e4 d3d8 e4f4 d8d4 f4g3 d4d8 h4h5 d8f8 g3f4 f8b8 f4f3 b8b3 f3g2 b3b8 a7c7 b8a8 g2f3 a8a3 f3f4 a3a4 f4g3 a4a8 c7b7 a8f8 g3f4 f8c8 f4e4 c8c4 e4f3 c4c3 f3g2 c3c8 f2f4 c8c2 g2f3 c2c3 f3e4 c3c4 e4f5 c4c5 f5e6 c5c6 e6d5 c6c8 h5h6 g7h6 d5e6 c8c4 e6f5 c4c6 b7a7 c6d6 a7c7 d6d4 c7b7 d4a4 b7e7 a4a6 e7e6 a6e6 f5e6 g8g7 f4f5 h6h5 g4h5 h7h6 e6e7 g7g8 e7f6 g8f8 f6e6 f8g7 e6e7 g7h8 f5f6 h8g8 f6f7 g8h8 f7f8Q h8h7 f8f4 h7g8 f4g4 g8h8 e7f8 h8h7 g4g8",  // 96 movws
        Constants.LongPositionCommand, // 296 moves
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public OriginalGame ParseGame_Original(string positionCommand) => ParseGame_OriginalClass.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public OriginalGame ParseGame_Improved1(string positionCommand) => ParseGame_ImprovedClass1.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ImprovedGame ParseGame_Improved2(string positionCommand) => ParseGame_ImprovedClass2.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ImprovedGame ParseGame_Improved3(string positionCommand) => ParseGame_ImprovedClass3.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ImprovedGame2 ParseGame_Improved4(string positionCommand) => ParseGame_ImprovedClass4.ParseGame(positionCommand);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ImprovedGame2 ParseGame_Improved5(string positionCommand) => ParseGame_ImprovedClass5.ParseGame(positionCommand);

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

        public static OriginalGame ParseGame(string positionCommand)
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

                return new OriginalGame(initialPosition, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                throw;
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

        public static OriginalGame ParseGame(string positionCommand)
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

                return new OriginalGame(initialPosition, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                throw;
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

        public static ImprovedGame ParseGame(string positionCommand)
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

                return new ImprovedGame(initialPosition, movesRegexResultAsSpan, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                throw;
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

        public static ImprovedGame ParseGame(string positionCommand)
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

                return new ImprovedGame(initialPosition, movesRegexResultAsSpan, moves);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                throw;
            }
        }
    }

    public static class ParseGame_ImprovedClass4
    {
        private static readonly Move[] _movePool = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        public const string Id = "position";

        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ImprovedGame2 ParseGame(string positionCommand)
        {
            try
            {
                var positionCommandSpan = positionCommand.AsSpan();

                // We divide the position command in these two sections:
                // "position startpos                       ||"
                // "position startpos                       || moves e2e4 e7e5"
                // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  ||"
                // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  || moves e2e4 e7e5"
                Span<Range> items = stackalloc Range[2];
                positionCommandSpan.Split(items, "moves", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var initialPositionSection = positionCommandSpan[items[0]];

                // We divide in these two parts
                // "position startpos ||"       <-- If "fen" doesn't exist in the section
                // "position || (fen) 8/8/8/8/8/8/8/8 w - - 0 1"  <-- If "fen" does exist
                Span<Range> initialPositionParts = stackalloc Range[2];
                initialPositionSection.Split(initialPositionParts, "fen", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                ReadOnlySpan<char> fen = initialPositionSection[initialPositionParts[0]].Length == Id.Length   // "position" o "position startpos"
                    ? initialPositionSection[initialPositionParts[1]]
                    : Constants.InitialPositionFEN.AsSpan();

                var movesSection = positionCommandSpan[items[1]];

                Span<Range> moves = stackalloc Range[2048]; // Number of potential half-moves provided in the string
                movesSection.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new ImprovedGame2(fen, movesSection, moves, _movePool);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                throw;
            }
        }
    }

    public static class ParseGame_ImprovedClass5
    {
        private static readonly Move[] _movePool = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        public const string Id = "position";

        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ImprovedGame2 ParseGame(string positionCommand)
        {
            try
            {
                var positionCommandSpan = positionCommand.AsSpan();

                // We divide the position command in these two sections:
                // "position startpos                       ||"
                // "position startpos                       || moves e2e4 e7e5"
                // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  ||"
                // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  || moves e2e4 e7e5"
                Span<Range> items = stackalloc Range[2];
                positionCommandSpan.Split(items, "moves", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var initialPositionSection = positionCommandSpan[items[0]];

                // We divide in these two parts
                // "position startpos ||"       <-- If "fen" doesn't exist in the section
                // "position || (fen) 8/8/8/8/8/8/8/8 w - - 0 1"  <-- If "fen" does exist
                Span<Range> initialPositionParts = stackalloc Range[2];
                initialPositionSection.Split(initialPositionParts, "fen", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                ReadOnlySpan<char> fen = initialPositionSection[initialPositionParts[0]].Length == Id.Length   // "position" o "position startpos"
                    ? initialPositionSection[initialPositionParts[1]]
                    : Constants.InitialPositionFEN.AsSpan();

                var movesSection = positionCommandSpan[items[1]];

                Span<Range> moves = stackalloc Range[(movesSection.Length / 5) + 1]; // Number of potential half-moves provided in the string
                movesSection.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new ImprovedGame2(fen, movesSection, moves, _movePool);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
                throw;
            }
        }
    }

#pragma warning disable RCS1169, S2933, S4487, IDE0044, IDE0052, RCS1170 // Readonly, not used

    public sealed class OriginalGame
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#if DEBUG
        public List<Move> MoveHistory { get; }
#endif

        public HashSet<ulong> PositionHashHistory { get; }

        public int HalfMovesWithoutCaptureOrPawnMove { get; set; }

        public Position CurrentPosition { get; private set; }

        private Position _gameInitialPosition;

        public OriginalGame(ReadOnlySpan<char> fen)
        {
            var parsedFen = FENParser.ParseFEN(fen);
            CurrentPosition = new Position(parsedFen);
            if (!CurrentPosition.IsValid())
            {
                _logger.Warn($"Invalid position detected: {fen.ToString()}");
            }

            PositionHashHistory = new(1024) { CurrentPosition.UniqueIdentifier };
            HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;
            _gameInitialPosition = new Position(CurrentPosition);
#if DEBUG
            MoveHistory = new(1024);
#endif
        }

        internal OriginalGame(string fen, string[] movesUCIString) : this(fen)
        {
            foreach (var moveString in movesUCIString)
            {
                var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition);

                if (!MoveExtensions.TryParseFromUCIString(CurrentPosition, moveString, moveList, out var parsedMove))
                {
                    _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen, string.Join(' ', movesUCIString), moveString);
                    break;
                }

                MakeMove(parsedMove.Value);
            }

            _gameInitialPosition = new Position(CurrentPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameState MakeMove(Move moveToPlay)
        {
            var gameState = CurrentPosition.MakeMove(moveToPlay);

            if (CurrentPosition.WasProduceByAValidMove())
            {
#if DEBUG
                MoveHistory.Add(moveToPlay);
#endif
            }
            else
            {
                _logger.Warn("Error trying to play {0}", moveToPlay.UCIString());
                CurrentPosition.UnmakeMove(moveToPlay, gameState);
            }

            PositionHashHistory.Add(CurrentPosition.UniqueIdentifier);
            HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveToPlay, HalfMovesWithoutCaptureOrPawnMove);

            return gameState;
        }
    }

    public sealed class ImprovedGame
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#if DEBUG
        public List<Move> MoveHistory { get; }
#endif

        public HashSet<ulong> PositionHashHistory { get; }

        public int HalfMovesWithoutCaptureOrPawnMove { get; set; }

        public Position CurrentPosition { get; private set; }

        private Position _gameInitialPosition;

        public ImprovedGame(ReadOnlySpan<char> fen)
        {
            var parsedFen = FENParser.ParseFEN(fen);
            CurrentPosition = new Position(parsedFen);
            if (!CurrentPosition.IsValid())
            {
                _logger.Warn($"Invalid position detected: {fen.ToString()}");
            }

            PositionHashHistory = new(1024) { CurrentPosition.UniqueIdentifier };
            HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;
            _gameInitialPosition = new Position(CurrentPosition);
#if DEBUG
            MoveHistory = new(1024);
#endif
        }

        internal ImprovedGame(string fen, ReadOnlySpan<char> rawMoves, Span<Range> rangeSpan) : this(fen)
        {
            for (int i = 0; i < rangeSpan.Length; ++i)
            {
                var range = rangeSpan[i];
                if (range.Start.Equals(range.End))
                {
                    break;
                }
                var moveString = rawMoves[range];
                var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition);

                if (!MoveExtensions.TryParseFromUCIString(CurrentPosition, moveString, moveList, out var parsedMove))
                {
                    _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen, string.Join(' ', rawMoves.ToString()), moveString.ToString());
                    break;
                }

                MakeMove(parsedMove.Value);
            }

            _gameInitialPosition = new Position(CurrentPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameState MakeMove(Move moveToPlay)
        {
            var gameState = CurrentPosition.MakeMove(moveToPlay);

            if (CurrentPosition.WasProduceByAValidMove())
            {
#if DEBUG
                MoveHistory.Add(moveToPlay);
#endif
            }
            else
            {
                _logger.Warn("Error trying to play {0}", moveToPlay.UCIString());
                CurrentPosition.UnmakeMove(moveToPlay, gameState);
            }

            PositionHashHistory.Add(CurrentPosition.UniqueIdentifier);
            HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveToPlay, HalfMovesWithoutCaptureOrPawnMove);

            return gameState;
        }
    }

    public sealed class ImprovedGame2
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#if DEBUG
        public List<Move> MoveHistory { get; }
#endif

        public HashSet<ulong> PositionHashHistory { get; }

        public int HalfMovesWithoutCaptureOrPawnMove { get; set; }

        public Position CurrentPosition { get; private set; }

        private Position _gameInitialPosition;

        public ImprovedGame2(ReadOnlySpan<char> fen)
        {
            var parsedFen = FENParser.ParseFEN(fen);
            CurrentPosition = new Position(parsedFen);
            if (!CurrentPosition.IsValid())
            {
                _logger.Warn($"Invalid position detected: {fen.ToString()}");
            }

            PositionHashHistory = new(1024) { CurrentPosition.UniqueIdentifier };
            HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;
            _gameInitialPosition = new Position(CurrentPosition);
#if DEBUG
            MoveHistory = new(1024);
#endif
        }

        public ImprovedGame2(ReadOnlySpan<char> fen, ReadOnlySpan<char> rawMoves, Span<Range> rangeSpan, Move[] movePool) : this(fen)
        {
            Span<BitBoard> attacks = stackalloc BitBoard[12];
            Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
            var evaluationContext = new EvaluationContext(attacks, attacksBySide);

            for (int i = 0; i < rangeSpan.Length; ++i)
            {
                if (rangeSpan[i].Start.Equals(rangeSpan[i].End))
                {
                    break;
                }
                var moveString = rawMoves[rangeSpan[i]];

                var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition, ref evaluationContext, movePool);

                if (!MoveExtensions.TryParseFromUCIString(CurrentPosition, moveString, moveList, out var parsedMove))
                {
                    _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen.ToString(), rawMoves.ToString(), moveString.ToString());
                    break;
                }

                MakeMove(parsedMove.Value);
            }

            _gameInitialPosition = new Position(CurrentPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameState MakeMove(Move moveToPlay)
        {
            var gameState = CurrentPosition.MakeMove(moveToPlay);

            if (CurrentPosition.WasProduceByAValidMove())
            {
#if DEBUG
                MoveHistory.Add(moveToPlay);
#endif
            }
            else
            {
                _logger.Warn("Error trying to play {0}", moveToPlay.UCIString());
                CurrentPosition.UnmakeMove(moveToPlay, gameState);
            }

            PositionHashHistory.Add(CurrentPosition.UniqueIdentifier);
            HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveToPlay, HalfMovesWithoutCaptureOrPawnMove);

            return gameState;
        }
    }

#pragma warning restore RCS1169, S2933, S4487, IDE0044, IDE0052, RCS1170,  // Readonly, not used
}

#pragma warning restore S2139 // Exceptions should be either logged or rethrown but not both