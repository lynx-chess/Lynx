/*
 * Since we're copying such small arrays, there's little effect in trying to optimize it
 * https://devblogs.microsoft.com/dotnet/hardware-intrinsics-in-net-core/
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.203
 *    [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean          | Error       | StdDev      | Median        | Ratio  | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
 *  |------------------- |------------- |--------------:|------------:|------------:|--------------:|-------:|--------:|--------:|--------:|--------:|----------:|------------:|
 *  | ArrayDotCopy       | Int32[1024]  |    254.937 ns |   5.0322 ns |   6.8882 ns |    256.887 ns |   1.00 |    0.04 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | CopyTo             | Int32[1024]  |    236.574 ns |   4.5065 ns |   9.6038 ns |    234.128 ns |   0.93 |    0.04 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoop         | Int32[1024]  |    522.283 ns |  10.5071 ns |  11.6786 ns |    520.483 ns |   2.05 |    0.07 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoopSpan     | Int32[1024]  |    506.400 ns |   9.3082 ns |   8.2515 ns |    505.805 ns |   1.99 |    0.06 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | UnrolledManualLoop | Int32[1024]  |    540.060 ns |  10.7099 ns |  15.3598 ns |    534.890 ns |   2.12 |    0.08 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | CopyBlock          | Int32[1024]  |    155.792 ns |   3.1857 ns |   8.7207 ns |    154.345 ns |   0.61 |    0.04 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ArrayDotCopy       | Int32[128]   |     36.375 ns |   0.7975 ns |   0.8189 ns |     36.274 ns |   0.14 |    0.00 |  0.0320 |       - |       - |     536 B |       0.130 |
 *  | CopyTo             | Int32[128]   |     34.074 ns |   0.7398 ns |   0.9086 ns |     33.801 ns |   0.13 |    0.00 |  0.0320 |       - |       - |     536 B |       0.130 |
 *  | ManualLoop         | Int32[128]   |     78.982 ns |   1.4934 ns |   1.3969 ns |     78.591 ns |   0.31 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | ManualLoopSpan     | Int32[128]   |     79.160 ns |   1.5990 ns |   1.7109 ns |     78.787 ns |   0.31 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | UnrolledManualLoop | Int32[128]   |     74.002 ns |   1.5375 ns |   2.5262 ns |     73.338 ns |   0.29 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | CopyBlock          | Int32[128]   |     30.194 ns |   0.6349 ns |   1.2382 ns |     29.825 ns |   0.12 |    0.01 |  0.0320 |       - |       - |     536 B |       0.130 |
 *  | ArrayDotCopy       | Int32[12]    |     13.409 ns |   0.3232 ns |   0.3592 ns |     13.366 ns |   0.05 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | CopyTo             | Int32[12]    |     11.225 ns |   0.2788 ns |   0.2739 ns |     11.220 ns |   0.04 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ManualLoop         | Int32[12]    |     13.808 ns |   0.2218 ns |   0.2074 ns |     13.854 ns |   0.05 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ManualLoopSpan     | Int32[12]    |     15.623 ns |   0.3757 ns |   0.3690 ns |     15.659 ns |   0.06 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | UnrolledManualLoop | Int32[12]    |     14.250 ns |   0.2756 ns |   0.2443 ns |     14.273 ns |   0.06 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | CopyBlock          | Int32[12]    |     13.033 ns |   0.3302 ns |   0.3088 ns |     13.086 ns |   0.05 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ArrayDotCopy       | Int32[192]   |     50.182 ns |   1.0207 ns |   2.4456 ns |     49.498 ns |   0.20 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | CopyTo             | Int32[192]   |     47.545 ns |   1.0146 ns |   2.4697 ns |     47.227 ns |   0.19 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ManualLoop         | Int32[192]   |    102.220 ns |   1.4524 ns |   1.2128 ns |    102.341 ns |   0.40 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ManualLoopSpan     | Int32[192]   |    106.417 ns |   1.5634 ns |   1.3859 ns |    106.369 ns |   0.42 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | UnrolledManualLoop | Int32[192]   |    102.315 ns |   2.0449 ns |   2.1880 ns |    101.683 ns |   0.40 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | CopyBlock          | Int32[192]   |     40.659 ns |   0.8753 ns |   2.1305 ns |     40.855 ns |   0.16 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ArrayDotCopy       | Int32[256]   |     65.828 ns |   1.4033 ns |   4.1375 ns |     65.118 ns |   0.26 |    0.02 |  0.0626 |       - |       - |    1048 B |       0.254 |
 *  | CopyTo             | Int32[256]   |     57.489 ns |   1.2234 ns |   2.3277 ns |     57.226 ns |   0.23 |    0.01 |  0.0626 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoop         | Int32[256]   |    135.464 ns |   2.7909 ns |   5.3099 ns |    134.674 ns |   0.53 |    0.03 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoopSpan     | Int32[256]   |    145.205 ns |   2.2656 ns |   2.0084 ns |    145.782 ns |   0.57 |    0.02 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | UnrolledManualLoop | Int32[256]   |    138.370 ns |   2.7344 ns |   2.9257 ns |    138.188 ns |   0.54 |    0.02 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | CopyBlock          | Int32[256]   |     44.231 ns |   0.8072 ns |   0.7156 ns |     44.414 ns |   0.17 |    0.01 |  0.0626 |       - |       - |    1048 B |       0.254 |
 *  | ArrayDotCopy       | Int32[35328] | 77,285.510 ns | 664.7419 ns | 621.8000 ns | 77,289.190 ns | 303.37 |    8.44 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | CopyTo             | Int32[35328] | 76,578.458 ns | 427.8499 ns | 400.2111 ns | 76,449.558 ns | 300.59 |    8.17 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoop         | Int32[35328] | 79,268.465 ns | 530.4042 ns | 496.1404 ns | 79,222.683 ns | 311.15 |    8.52 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoopSpan     | Int32[35328] | 79,513.644 ns | 876.5231 ns | 819.9003 ns | 79,171.336 ns | 312.12 |    8.90 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | UnrolledManualLoop | Int32[35328] | 79,381.776 ns | 385.9187 ns | 360.9886 ns | 79,403.938 ns | 311.60 |    8.43 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | CopyBlock          | Int32[35328] | 13,457.564 ns |  85.6251 ns |  71.5009 ns | 13,444.995 ns |  52.83 |    1.44 | 43.4723 | 43.4723 | 43.4723 |  141365 B |      34.312 |
 *  | ArrayDotCopy       | Int32[3]     |     11.879 ns |   0.1954 ns |   0.1732 ns |     11.897 ns |   0.05 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | CopyTo             | Int32[3]     |     10.328 ns |   0.1514 ns |   0.1416 ns |     10.353 ns |   0.04 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ManualLoop         | Int32[3]     |      9.245 ns |   0.1751 ns |   0.1638 ns |      9.249 ns |   0.04 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ManualLoopSpan     | Int32[3]     |     10.653 ns |   0.1990 ns |   0.1862 ns |     10.711 ns |   0.04 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | UnrolledManualLoop | Int32[3]     |     12.834 ns |   0.0955 ns |   0.0847 ns |     12.850 ns |   0.05 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | CopyBlock          | Int32[3]     |     11.375 ns |   0.2166 ns |   0.2026 ns |     11.336 ns |   0.04 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ArrayDotCopy       | Int32[512]   |    112.072 ns |   2.5947 ns |   7.5687 ns |    109.571 ns |   0.44 |    0.03 |  0.1237 |       - |       - |    2072 B |       0.503 |
 *  | CopyTo             | Int32[512]   |    114.706 ns |   2.2447 ns |   2.5850 ns |    115.365 ns |   0.45 |    0.02 |  0.1239 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoop         | Int32[512]   |    277.700 ns |   4.2346 ns |   3.9610 ns |    279.329 ns |   1.09 |    0.03 |  0.1235 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoopSpan     | Int32[512]   |    275.319 ns |   5.5379 ns |   8.7837 ns |    276.077 ns |   1.08 |    0.04 |  0.1235 |       - |       - |    2072 B |       0.503 |
 *  | UnrolledManualLoop | Int32[512]   |    278.260 ns |   5.6370 ns |  11.5150 ns |    276.258 ns |   1.09 |    0.05 |  0.1235 |       - |       - |    2072 B |       0.503 |
 *  | CopyBlock          | Int32[512]   |     84.906 ns |   1.6474 ns |   2.2550 ns |     85.274 ns |   0.33 |    0.01 |  0.1239 |       - |       - |    2072 B |       0.503 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.3328) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.203
 *    [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean          | Error         | StdDev        | Median        | Ratio  | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
 *  |------------------- |------------- |--------------:|--------------:|--------------:|--------------:|-------:|--------:|--------:|--------:|--------:|----------:|------------:|
 *  | ArrayDotCopy       | Int32[1024]  |    167.303 ns |     3.2905 ns |     4.0410 ns |    167.314 ns |   1.00 |    0.03 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | CopyTo             | Int32[1024]  |    161.119 ns |     3.2450 ns |     3.1870 ns |    161.313 ns |   0.96 |    0.03 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoop         | Int32[1024]  |    451.862 ns |     4.6027 ns |     4.3054 ns |    450.727 ns |   2.70 |    0.07 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoopSpan     | Int32[1024]  |    474.215 ns |     9.5574 ns |    15.7031 ns |    471.370 ns |   2.84 |    0.12 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | UnrolledManualLoop | Int32[1024]  |    482.340 ns |     9.7145 ns |    15.1243 ns |    479.212 ns |   2.88 |    0.11 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | CopyBlock          | Int32[1024]  |    124.461 ns |     2.5556 ns |     7.0390 ns |    123.459 ns |   0.74 |    0.05 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ArrayDotCopy       | Int32[128]   |     31.394 ns |     0.6882 ns |     1.3093 ns |     31.351 ns |   0.19 |    0.01 |  0.0320 |       - |       - |     536 B |       0.130 |
 *  | CopyTo             | Int32[128]   |     28.147 ns |     0.7565 ns |     2.2186 ns |     27.432 ns |   0.17 |    0.01 |  0.0320 |       - |       - |     536 B |       0.130 |
 *  | ManualLoop         | Int32[128]   |     67.006 ns |     1.3903 ns |     1.9490 ns |     66.337 ns |   0.40 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | ManualLoopSpan     | Int32[128]   |     69.754 ns |     1.0327 ns |     0.9660 ns |     69.325 ns |   0.42 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | UnrolledManualLoop | Int32[128]   |     63.376 ns |     0.6436 ns |     0.5706 ns |     63.350 ns |   0.38 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | CopyBlock          | Int32[128]   |     20.676 ns |     0.3304 ns |     0.3091 ns |     20.739 ns |   0.12 |    0.00 |  0.0320 |       - |       - |     536 B |       0.130 |
 *  | ArrayDotCopy       | Int32[12]    |     10.604 ns |     0.2504 ns |     0.2220 ns |     10.531 ns |   0.06 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | CopyTo             | Int32[12]    |      9.299 ns |     0.2503 ns |     0.6417 ns |      9.126 ns |   0.06 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ManualLoop         | Int32[12]    |      9.202 ns |     0.0623 ns |     0.0552 ns |      9.203 ns |   0.06 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ManualLoopSpan     | Int32[12]    |     11.325 ns |     0.2423 ns |     0.2148 ns |     11.264 ns |   0.07 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | UnrolledManualLoop | Int32[12]    |     11.850 ns |     0.1924 ns |     0.1799 ns |     11.811 ns |   0.07 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | CopyBlock          | Int32[12]    |      9.416 ns |     0.1776 ns |     0.1574 ns |      9.395 ns |   0.06 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ArrayDotCopy       | Int32[192]   |     37.388 ns |     0.3859 ns |     0.3610 ns |     37.375 ns |   0.22 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | CopyTo             | Int32[192]   |     36.826 ns |     0.4934 ns |     0.4615 ns |     36.756 ns |   0.22 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ManualLoop         | Int32[192]   |     91.077 ns |     0.7161 ns |     0.6698 ns |     90.928 ns |   0.54 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ManualLoopSpan     | Int32[192]   |     93.651 ns |     0.6078 ns |     0.5685 ns |     93.578 ns |   0.56 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | UnrolledManualLoop | Int32[192]   |     90.263 ns |     0.4877 ns |     0.4562 ns |     90.301 ns |   0.54 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | CopyBlock          | Int32[192]   |     27.954 ns |     0.5928 ns |     0.6088 ns |     28.249 ns |   0.17 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ArrayDotCopy       | Int32[256]   |     46.617 ns |     0.9028 ns |     0.8003 ns |     46.837 ns |   0.28 |    0.01 |  0.0626 |       - |       - |    1048 B |       0.254 |
 *  | CopyTo             | Int32[256]   |     49.439 ns |     1.0412 ns |     1.2395 ns |     49.552 ns |   0.30 |    0.01 |  0.0626 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoop         | Int32[256]   |    121.957 ns |     1.6444 ns |     1.5382 ns |    121.232 ns |   0.73 |    0.02 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoopSpan     | Int32[256]   |    121.772 ns |     0.8941 ns |     0.7926 ns |    121.846 ns |   0.73 |    0.02 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | UnrolledManualLoop | Int32[256]   |    120.973 ns |     1.1336 ns |     1.0049 ns |    120.856 ns |   0.72 |    0.02 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | CopyBlock          | Int32[256]   |     34.077 ns |     0.6609 ns |     0.6182 ns |     34.266 ns |   0.20 |    0.01 |  0.0626 |       - |       - |    1048 B |       0.254 |
 *  | ArrayDotCopy       | Int32[35328] | 58,967.564 ns |   285.2065 ns |   252.8282 ns | 58,982.831 ns | 352.66 |    8.62 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | CopyTo             | Int32[35328] | 59,374.512 ns |   426.5268 ns |   398.9735 ns | 59,409.662 ns | 355.09 |    8.86 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | ManualLoop         | Int32[35328] | 62,104.009 ns |   529.8950 ns |   495.6641 ns | 61,915.222 ns | 371.42 |    9.40 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | ManualLoopSpan     | Int32[35328] | 62,279.200 ns | 1,239.0397 ns | 2,137.2829 ns | 62,614.404 ns | 372.47 |   15.48 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | UnrolledManualLoop | Int32[35328] | 61,123.840 ns | 1,184.1585 ns | 1,163.0019 ns | 61,265.228 ns | 365.56 |   11.09 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | CopyBlock          | Int32[35328] |  5,972.860 ns |    66.4679 ns |    62.1741 ns |  5,990.718 ns |  35.72 |    0.93 | 43.4723 | 43.4723 | 43.4723 |  141351 B |      34.308 |
 *  | ArrayDotCopy       | Int32[3]     |      9.299 ns |     0.0797 ns |     0.0746 ns |      9.272 ns |   0.06 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | CopyTo             | Int32[3]     |      6.694 ns |     0.0930 ns |     0.0824 ns |      6.668 ns |   0.04 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ManualLoop         | Int32[3]     |      5.856 ns |     0.1814 ns |     0.2825 ns |      5.763 ns |   0.04 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ManualLoopSpan     | Int32[3]     |      7.747 ns |     0.1127 ns |     0.0880 ns |      7.722 ns |   0.05 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | UnrolledManualLoop | Int32[3]     |      8.200 ns |     0.1066 ns |     0.0997 ns |      8.183 ns |   0.05 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | CopyBlock          | Int32[3]     |      8.769 ns |     0.1145 ns |     0.1015 ns |      8.796 ns |   0.05 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ArrayDotCopy       | Int32[512]   |     84.826 ns |     1.0510 ns |     0.8776 ns |     84.745 ns |   0.51 |    0.01 |  0.1239 |       - |       - |    2072 B |       0.503 |
 *  | CopyTo             | Int32[512]   |     83.307 ns |     1.6755 ns |     2.4559 ns |     82.329 ns |   0.50 |    0.02 |  0.1239 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoop         | Int32[512]   |    233.038 ns |     3.5805 ns |     2.9899 ns |    234.109 ns |   1.39 |    0.04 |  0.1237 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoopSpan     | Int32[512]   |    235.429 ns |     4.7197 ns |     8.8648 ns |    233.006 ns |   1.41 |    0.06 |  0.1235 |       - |       - |    2072 B |       0.503 |
 *  | UnrolledManualLoop | Int32[512]   |    240.894 ns |     3.5134 ns |     3.2865 ns |    239.514 ns |   1.44 |    0.04 |  0.1235 |       - |       - |    2072 B |       0.503 |
 *  | CopyBlock          | Int32[512]   |     59.198 ns |     1.0916 ns |     0.9676 ns |     59.290 ns |   0.35 |    0.01 |  0.1239 |       - |       - |    2072 B |       0.503 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.4 (23H420) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.203
 *    [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
 *
 *  | Method             | array        | Mean          | Error       | StdDev      | Median        | Ratio  | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
 *  |------------------- |------------- |--------------:|------------:|------------:|--------------:|-------:|--------:|--------:|--------:|--------:|----------:|------------:|
 *  | ArrayDotCopy       | Int32[1024]  |    190.477 ns |   0.7414 ns |   0.6191 ns |    190.248 ns |   1.00 |    0.00 |  0.6564 |       - |       - |    4120 B |       1.000 |
 *  | CopyTo             | Int32[1024]  |    188.317 ns |   0.6225 ns |   0.4860 ns |    188.069 ns |   0.99 |    0.00 |  0.6564 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoop         | Int32[1024]  |    476.820 ns |   9.3927 ns |  16.4506 ns |    466.555 ns |   2.50 |    0.09 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoopSpan     | Int32[1024]  |    462.221 ns |   1.3285 ns |   1.1093 ns |    462.245 ns |   2.43 |    0.01 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | UnrolledManualLoop | Int32[1024]  |    350.365 ns |   6.9323 ns |  10.7927 ns |    342.147 ns |   1.84 |    0.06 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | CopyBlock          | Int32[1024]  |    125.560 ns |   0.8139 ns |   0.7215 ns |    125.388 ns |   0.66 |    0.00 |  0.6564 |       - |       - |    4120 B |       1.000 |
 *  | ArrayDotCopy       | Int32[128]   |     30.535 ns |   0.2554 ns |   0.1994 ns |     30.495 ns |   0.16 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | CopyTo             | Int32[128]   |     30.496 ns |   0.6554 ns |   1.3240 ns |     29.723 ns |   0.16 |    0.01 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ManualLoop         | Int32[128]   |     66.589 ns |   0.1909 ns |   0.1490 ns |     66.540 ns |   0.35 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ManualLoopSpan     | Int32[128]   |     67.227 ns |   0.1674 ns |   0.1398 ns |     67.204 ns |   0.35 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | UnrolledManualLoop | Int32[128]   |     46.892 ns |   0.9866 ns |   1.7019 ns |     45.734 ns |   0.25 |    0.01 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | CopyBlock          | Int32[128]   |     20.341 ns |   0.3450 ns |   0.2881 ns |     20.216 ns |   0.11 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ArrayDotCopy       | Int32[12]    |      7.681 ns |   0.0988 ns |   0.0876 ns |      7.658 ns |   0.04 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | CopyTo             | Int32[12]    |      6.159 ns |   0.1243 ns |   0.1277 ns |      6.121 ns |   0.03 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ManualLoop         | Int32[12]    |      9.636 ns |   0.0214 ns |   0.0178 ns |      9.633 ns |   0.05 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ManualLoopSpan     | Int32[12]    |     10.426 ns |   0.0257 ns |   0.0214 ns |     10.424 ns |   0.05 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | UnrolledManualLoop | Int32[12]    |      9.102 ns |   0.1083 ns |   0.0846 ns |      9.065 ns |   0.05 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | CopyBlock          | Int32[12]    |      6.863 ns |   0.1648 ns |   0.1376 ns |      6.805 ns |   0.04 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ArrayDotCopy       | Int32[192]   |     41.900 ns |   0.1687 ns |   0.1409 ns |     41.870 ns |   0.22 |    0.00 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | CopyTo             | Int32[192]   |     42.699 ns |   0.8952 ns |   1.8882 ns |     42.201 ns |   0.22 |    0.01 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ManualLoop         | Int32[192]   |     98.491 ns |   0.1844 ns |   0.1539 ns |     98.433 ns |   0.52 |    0.00 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ManualLoopSpan     | Int32[192]   |    103.952 ns |   2.1208 ns |   3.5433 ns |    101.393 ns |   0.55 |    0.02 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | UnrolledManualLoop | Int32[192]   |     65.404 ns |   0.1947 ns |   0.1625 ns |     65.356 ns |   0.34 |    0.00 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | CopyBlock          | Int32[192]   |     27.609 ns |   0.0929 ns |   0.0824 ns |     27.599 ns |   0.14 |    0.00 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ArrayDotCopy       | Int32[256]   |     53.425 ns |   0.5512 ns |   0.7545 ns |     53.218 ns |   0.28 |    0.00 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | CopyTo             | Int32[256]   |     53.354 ns |   0.2741 ns |   0.2430 ns |     53.379 ns |   0.28 |    0.00 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoop         | Int32[256]   |    130.490 ns |   2.6156 ns |   4.2975 ns |    127.819 ns |   0.69 |    0.02 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoopSpan     | Int32[256]   |    130.775 ns |   1.0417 ns |   0.8699 ns |    130.461 ns |   0.69 |    0.00 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | UnrolledManualLoop | Int32[256]   |     89.066 ns |   0.2781 ns |   0.2171 ns |     89.020 ns |   0.47 |    0.00 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | CopyBlock          | Int32[256]   |     37.881 ns |   0.7970 ns |   1.4371 ns |     38.107 ns |   0.20 |    0.01 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | ArrayDotCopy       | Int32[35328] | 14,016.992 ns |  61.9362 ns |  48.3557 ns | 13,993.120 ns |  73.59 |    0.34 | 43.4723 | 43.4723 | 43.4723 |  141365 B |      34.312 |
 *  | CopyTo             | Int32[35328] | 13,977.269 ns |  67.5290 ns |  59.8627 ns | 13,951.038 ns |  73.38 |    0.38 | 43.4723 | 43.4723 | 43.4723 |  141365 B |      34.312 |
 *  | ManualLoop         | Int32[35328] | 22,610.576 ns | 441.8748 ns | 713.5455 ns | 22,187.824 ns | 118.71 |    3.71 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoopSpan     | Int32[35328] | 22,229.023 ns | 116.9327 ns |  97.6441 ns | 22,177.252 ns | 116.70 |    0.61 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | UnrolledManualLoop | Int32[35328] | 18,344.439 ns | 110.0947 ns |  97.5961 ns | 18,305.539 ns |  96.31 |    0.58 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | CopyBlock          | Int32[35328] |  5,645.243 ns | 111.1175 ns | 182.5693 ns |  5,552.076 ns |  29.64 |    0.95 | 43.4723 | 43.4723 | 43.4723 |  141339 B |      34.306 |
 *  | ArrayDotCopy       | Int32[3]     |      6.455 ns |   0.0712 ns |   0.0631 ns |      6.436 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | CopyTo             | Int32[3]     |      4.987 ns |   0.0407 ns |   0.0318 ns |      4.981 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | ManualLoop         | Int32[3]     |      5.729 ns |   0.0163 ns |   0.0136 ns |      5.729 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | ManualLoopSpan     | Int32[3]     |      6.328 ns |   0.0493 ns |   0.0412 ns |      6.312 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | UnrolledManualLoop | Int32[3]     |      7.595 ns |   0.1827 ns |   0.2844 ns |      7.442 ns |   0.04 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | CopyBlock          | Int32[3]     |      5.969 ns |   0.0189 ns |   0.0148 ns |      5.969 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | ArrayDotCopy       | Int32[512]   |    101.080 ns |   1.9277 ns |   4.3116 ns |     98.837 ns |   0.53 |    0.02 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *  | CopyTo             | Int32[512]   |     98.103 ns |   0.4272 ns |   0.3567 ns |     97.989 ns |   0.52 |    0.00 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoop         | Int32[512]   |    247.438 ns |   4.9922 ns |   9.1286 ns |    242.526 ns |   1.30 |    0.05 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoopSpan     | Int32[512]   |    247.710 ns |   1.9794 ns |   1.6529 ns |    247.065 ns |   1.30 |    0.01 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | UnrolledManualLoop | Int32[512]   |    173.532 ns |   0.9552 ns |   0.7977 ns |    173.244 ns |   0.91 |    0.00 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *  | CopyBlock          | Int32[512]   |     68.001 ns |   1.3980 ns |   3.5837 ns |     66.823 ns |   0.36 |    0.02 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.5 (22H527) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.203
 *    [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean         | Error        | StdDev        | Median       | Ratio  | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
 *  |------------------- |------------- |-------------:|-------------:|--------------:|-------------:|-------:|--------:|--------:|--------:|--------:|----------:|------------:|
 *  | ArrayDotCopy       | Int32[1024]  |    654.15 ns |    26.694 ns |     73.968 ns |    663.93 ns |   1.01 |    0.17 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | CopyTo             | Int32[1024]  |    696.94 ns |    31.158 ns |     88.390 ns |    687.93 ns |   1.08 |    0.19 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoop         | Int32[1024]  |  1,663.42 ns |    35.142 ns |     99.691 ns |  1,668.67 ns |   2.58 |    0.36 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoopSpan     | Int32[1024]  |  1,094.38 ns |    43.426 ns |    124.598 ns |  1,103.46 ns |   1.70 |    0.29 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | UnrolledManualLoop | Int32[1024]  |  1,238.86 ns |    35.268 ns |     97.139 ns |  1,238.41 ns |   1.92 |    0.28 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | CopyBlock          | Int32[1024]  |    514.58 ns |    20.464 ns |     58.385 ns |    498.74 ns |   0.80 |    0.14 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | ArrayDotCopy       | Int32[128]   |     98.08 ns |     2.619 ns |      7.388 ns |     97.97 ns |   0.15 |    0.02 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | CopyTo             | Int32[128]   |     69.66 ns |     1.396 ns |      1.957 ns |     69.17 ns |   0.11 |    0.01 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ManualLoop         | Int32[128]   |    181.71 ns |     3.548 ns |      2.962 ns |    180.71 ns |   0.28 |    0.04 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ManualLoopSpan     | Int32[128]   |    117.55 ns |     1.475 ns |      1.307 ns |    117.47 ns |   0.18 |    0.02 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | UnrolledManualLoop | Int32[128]   |    131.15 ns |     2.505 ns |      4.046 ns |    130.09 ns |   0.20 |    0.03 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | CopyBlock          | Int32[128]   |     63.59 ns |     1.348 ns |      1.845 ns |     62.73 ns |   0.10 |    0.01 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ArrayDotCopy       | Int32[12]    |     26.90 ns |     0.573 ns |      0.536 ns |     26.84 ns |   0.04 |    0.01 |  0.0114 |       - |       - |      72 B |       0.017 |
 *  | CopyTo             | Int32[12]    |     24.50 ns |     0.552 ns |      1.077 ns |     24.16 ns |   0.04 |    0.01 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ManualLoop         | Int32[12]    |     30.46 ns |     0.641 ns |      1.667 ns |     30.13 ns |   0.05 |    0.01 |  0.0114 |       - |       - |      72 B |       0.017 |
 *  | ManualLoopSpan     | Int32[12]    |     27.56 ns |     0.567 ns |      0.502 ns |     27.49 ns |   0.04 |    0.01 |  0.0114 |       - |       - |      72 B |       0.017 |
 *  | UnrolledManualLoop | Int32[12]    |     28.62 ns |     0.628 ns |      0.901 ns |     28.26 ns |   0.04 |    0.01 |  0.0114 |       - |       - |      72 B |       0.017 |
 *  | CopyBlock          | Int32[12]    |     24.19 ns |     0.297 ns |      0.263 ns |     24.20 ns |   0.04 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ArrayDotCopy       | Int32[192]   |     97.61 ns |     2.022 ns |      3.646 ns |     97.50 ns |   0.15 |    0.02 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | CopyTo             | Int32[192]   |     98.71 ns |     3.055 ns |      8.716 ns |     95.73 ns |   0.15 |    0.02 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ManualLoop         | Int32[192]   |    254.55 ns |     4.860 ns |      8.512 ns |    252.53 ns |   0.39 |    0.05 |  0.1259 |       - |       - |     792 B |       0.192 |
 *  | ManualLoopSpan     | Int32[192]   |    161.90 ns |     6.175 ns |     17.716 ns |    161.90 ns |   0.25 |    0.04 |  0.1261 |       - |       - |     792 B |       0.192 |
 *  | UnrolledManualLoop | Int32[192]   |    187.30 ns |     3.772 ns |      4.632 ns |    186.45 ns |   0.29 |    0.04 |  0.1261 |       - |       - |     792 B |       0.192 |
 *  | CopyBlock          | Int32[192]   |     86.19 ns |     1.796 ns |      4.637 ns |     85.21 ns |   0.13 |    0.02 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ArrayDotCopy       | Int32[256]   |    116.50 ns |     4.054 ns |     11.632 ns |    119.53 ns |   0.18 |    0.03 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | CopyTo             | Int32[256]   |    119.62 ns |     2.453 ns |      5.281 ns |    119.02 ns |   0.19 |    0.02 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoop         | Int32[256]   |    340.26 ns |     6.788 ns |     10.160 ns |    337.26 ns |   0.53 |    0.07 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoopSpan     | Int32[256]   |    209.02 ns |     6.150 ns |     17.645 ns |    209.40 ns |   0.32 |    0.05 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | UnrolledManualLoop | Int32[256]   |    188.36 ns |     3.825 ns |      8.942 ns |    187.29 ns |   0.29 |    0.04 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | CopyBlock          | Int32[256]   |     72.75 ns |     2.084 ns |      6.113 ns |     70.09 ns |   0.11 |    0.02 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | ArrayDotCopy       | Int32[35328] | 62,739.88 ns |   704.207 ns |    624.261 ns | 62,721.67 ns |  97.27 |   12.24 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | CopyTo             | Int32[35328] | 68,099.77 ns | 1,979.752 ns |  5,680.279 ns | 66,221.00 ns | 105.58 |   15.91 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoop         | Int32[35328] | 90,018.18 ns | 4,121.769 ns | 11,892.246 ns | 84,789.18 ns | 139.56 |   25.46 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoopSpan     | Int32[35328] | 67,088.24 ns | 1,327.817 ns |  2,712.378 ns | 65,848.74 ns | 104.01 |   13.70 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | UnrolledManualLoop | Int32[35328] | 71,232.94 ns | 1,409.505 ns |  1,318.452 ns | 70,785.69 ns | 110.43 |   13.99 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | CopyBlock          | Int32[35328] |  9,693.57 ns |   188.412 ns |    157.332 ns |  9,696.26 ns |  15.03 |    1.90 | 43.4723 | 43.4723 | 43.4723 |  141362 B |      34.311 |
 *  | ArrayDotCopy       | Int32[3]     |     18.34 ns |     0.423 ns |      0.632 ns |     18.18 ns |   0.03 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | CopyTo             | Int32[3]     |     15.30 ns |     0.258 ns |      0.229 ns |     15.20 ns |   0.02 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | ManualLoop         | Int32[3]     |     14.76 ns |     0.294 ns |      0.261 ns |     14.67 ns |   0.02 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | ManualLoopSpan     | Int32[3]     |     15.23 ns |     0.364 ns |      0.656 ns |     14.99 ns |   0.02 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | UnrolledManualLoop | Int32[3]     |     17.44 ns |     0.213 ns |      0.189 ns |     17.39 ns |   0.03 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | CopyBlock          | Int32[3]     |     18.86 ns |     0.795 ns |      2.345 ns |     18.74 ns |   0.03 |    0.01 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | ArrayDotCopy       | Int32[512]   |    193.99 ns |     7.571 ns |     22.323 ns |    197.04 ns |   0.30 |    0.05 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *  | CopyTo             | Int32[512]   |    184.09 ns |     7.254 ns |     21.390 ns |    184.55 ns |   0.29 |    0.05 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoop         | Int32[512]   |    643.32 ns |    12.854 ns |     30.298 ns |    641.21 ns |   1.00 |    0.13 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoopSpan     | Int32[512]   |    393.31 ns |     7.928 ns |     19.595 ns |    391.85 ns |   0.61 |    0.08 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | UnrolledManualLoop | Int32[512]   |    419.72 ns |    16.536 ns |     48.498 ns |    415.80 ns |   0.65 |    0.11 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | CopyBlock          | Int32[512]   |    138.14 ns |     5.584 ns |     15.658 ns |    136.30 ns |   0.21 |    0.04 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *
 *
 */

using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class ArrayCopy_Benchmark : BaseBenchmark
{
#pragma warning disable S2365 // Properties should not make collection or array copies
#pragma warning disable IDE0305 // Simplify collection initialization
    public static IEnumerable<int[]> Data => [
        [1, 2, 3],
        Enumerable.Range(0,12).ToArray(),
        Enumerable.Range(0,128).ToArray(),
        Enumerable.Range(0,192).ToArray(),
        Enumerable.Range(0,256).ToArray(),
        Enumerable.Range(0,512).ToArray(),
        Enumerable.Range(0,1024).ToArray(),
        EvaluationPSQTs._packedPSQT
    ];
#pragma warning restore IDE0305 // Simplify collection initialization
#pragma warning restore S2365 // Properties should not make collection or array copies

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int[] ArrayDotCopy(int[] array)
    {
        return ArrayDotCopyImpl(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] CopyTo(int[] array)
    {
        return CopyToImpl(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] ManualLoop(int[] array)
    {
        return ManualLoopImpl(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] ManualLoopSpan(int[] array)
    {
        return ManualLoopImplSpan(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int[] UnrolledManualLoop(int[] array)
    {
        return UnrolledManualLoopImpl(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public unsafe int[] CopyBlock(int[] array)
    {
        return CopyBlockImpl(array);
    }

    private static int[] ArrayDotCopyImpl(int[] array)
    {
        var result = new int[array.Length];
        Array.Copy(array, result, array.Length);

        return result;
    }

    private static int[] CopyToImpl(int[] array)
    {
        var result = new int[array.Length];
        array.CopyTo(result);

        return result;
    }

    private static int[] ManualLoopImpl(int[] array)
    {
        var result = new int[array.Length];
        for (int i = 0; i < array.Length; ++i)
        {
            result[i] = array[i];
        }

        return result;
    }

    private static int[] ManualLoopImplSpan(ReadOnlySpan<int> array)
    {
        var result = new int[array.Length];
        for (int i = 0; i < array.Length; ++i)
        {
            result[i] = array[i];
        }

        return result;
    }

    private static unsafe int[] UnrolledManualLoopImpl(ReadOnlySpan<int> array)
    {
        var result = new int[array.Length];

        int i = 0;
        int lastBlockIndex = array.Length - (array.Length % 4);

        // Pin source so we can elide the bounds checks
        fixed (int* pSource = array)
        {
            while (i < lastBlockIndex)
            {
                result[i] = pSource[i];
                result[i + 1] = pSource[i + 1];
                result[i + 2] = pSource[i + 2];
                result[i + 3] = pSource[i + 3];
                i += 4;
            }
            while (i < array.Length)
            {
                result[i] = pSource[i];
                ++i;
            }
        }

        return result;
    }

    private static unsafe int[] CopyBlockImpl(ReadOnlySpan<int> array)
    {
        var result = new int[array.Length];
        fixed (int* source = array, destination = result)
        {
            Unsafe.CopyBlock(source, source, (uint)(sizeof(int) * array.Length));
        }

        return result;
    }
}
