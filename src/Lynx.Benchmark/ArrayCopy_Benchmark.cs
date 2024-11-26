/*
 * Since we're copying such small arrays, there's little effect in trying to optimize it
 * https://devblogs.microsoft.com/dotnet/hardware-intrinsics-in-net-core/
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean          | Error       | StdDev      | Ratio  | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
 *  |------------------- |------------- |--------------:|------------:|------------:|-------:|--------:|--------:|--------:|--------:|----------:|------------:|
 *  | ArrayDotCopy       | Int32[1024]  |    341.232 ns |   1.6986 ns |   1.5058 ns |   1.00 |    0.01 |  0.0491 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoop         | Int32[1024]  |    600.480 ns |   4.0319 ns |   3.5742 ns |   1.76 |    0.01 |  0.0486 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoopSpan     | Int32[1024]  |    597.033 ns |   4.1905 ns |   3.7148 ns |   1.75 |    0.01 |  0.0486 |       - |       - |    4120 B |       1.000 |
 *  | UnrolledManualLoop | Int32[1024]  |    610.104 ns |   4.0727 ns |   3.8096 ns |   1.79 |    0.01 |  0.0486 |       - |       - |    4120 B |       1.000 |
 *  | CopyBlock          | Int32[1024]  |    255.873 ns |   2.1683 ns |   1.8106 ns |   0.75 |    0.01 |  0.0491 |       - |       - |    4120 B |       1.000 |
 *  | ArrayDotCopy       | Int32[128]   |     47.756 ns |   0.2212 ns |   0.1961 ns |   0.14 |    0.00 |  0.0064 |       - |       - |     536 B |       0.130 |
 *  | ManualLoop         | Int32[128]   |     86.177 ns |   0.6830 ns |   0.6389 ns |   0.25 |    0.00 |  0.0063 |       - |       - |     536 B |       0.130 |
 *  | ManualLoopSpan     | Int32[128]   |     86.613 ns |   0.5919 ns |   0.5536 ns |   0.25 |    0.00 |  0.0063 |       - |       - |     536 B |       0.130 |
 *  | UnrolledManualLoop | Int32[128]   |     83.328 ns |   0.3954 ns |   0.3505 ns |   0.24 |    0.00 |  0.0063 |       - |       - |     536 B |       0.130 |
 *  | CopyBlock          | Int32[128]   |     43.495 ns |   0.3532 ns |   0.3131 ns |   0.13 |    0.00 |  0.0064 |       - |       - |     536 B |       0.130 |
 *  | ArrayDotCopy       | Int32[12]    |     13.866 ns |   0.1039 ns |   0.0921 ns |   0.04 |    0.00 |  0.0008 |       - |       - |      72 B |       0.017 |
 *  | ManualLoop         | Int32[12]    |     13.768 ns |   0.0557 ns |   0.0521 ns |   0.04 |    0.00 |  0.0008 |       - |       - |      72 B |       0.017 |
 *  | ManualLoopSpan     | Int32[12]    |     14.950 ns |   0.0917 ns |   0.0858 ns |   0.04 |    0.00 |  0.0008 |       - |       - |      72 B |       0.017 |
 *  | UnrolledManualLoop | Int32[12]    |     15.384 ns |   0.0775 ns |   0.0687 ns |   0.05 |    0.00 |  0.0008 |       - |       - |      72 B |       0.017 |
 *  | CopyBlock          | Int32[12]    |     13.771 ns |   0.1692 ns |   0.1583 ns |   0.04 |    0.00 |  0.0008 |       - |       - |      72 B |       0.017 |
 *  | ArrayDotCopy       | Int32[192]   |     68.307 ns |   0.5091 ns |   0.4513 ns |   0.20 |    0.00 |  0.0094 |       - |       - |     792 B |       0.192 |
 *  | ManualLoop         | Int32[192]   |    121.866 ns |   0.9016 ns |   0.8434 ns |   0.36 |    0.00 |  0.0093 |       - |       - |     792 B |       0.192 |
 *  | ManualLoopSpan     | Int32[192]   |    123.016 ns |   1.0373 ns |   0.9196 ns |   0.36 |    0.00 |  0.0093 |       - |       - |     792 B |       0.192 |
 *  | UnrolledManualLoop | Int32[192]   |    121.386 ns |   0.7138 ns |   0.6328 ns |   0.36 |    0.00 |  0.0093 |       - |       - |     792 B |       0.192 |
 *  | CopyBlock          | Int32[192]   |     59.361 ns |   0.5927 ns |   0.4949 ns |   0.17 |    0.00 |  0.0094 |       - |       - |     792 B |       0.192 |
 *  | ArrayDotCopy       | Int32[256]   |     87.352 ns |   0.5702 ns |   0.5055 ns |   0.26 |    0.00 |  0.0125 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoop         | Int32[256]   |    158.689 ns |   1.3192 ns |   1.2340 ns |   0.47 |    0.00 |  0.0124 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoopSpan     | Int32[256]   |    160.477 ns |   1.0026 ns |   0.8888 ns |   0.47 |    0.00 |  0.0124 |       - |       - |    1048 B |       0.254 |
 *  | UnrolledManualLoop | Int32[256]   |    159.179 ns |   0.9639 ns |   0.8545 ns |   0.47 |    0.00 |  0.0124 |       - |       - |    1048 B |       0.254 |
 *  | CopyBlock          | Int32[256]   |     74.531 ns |   0.5833 ns |   0.5170 ns |   0.22 |    0.00 |  0.0125 |       - |       - |    1048 B |       0.254 |
 *  | ArrayDotCopy       | Int32[35328] | 79,237.983 ns | 406.1927 ns | 360.0793 ns | 232.22 |    1.42 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoop         | Int32[35328] | 81,760.738 ns | 457.4256 ns | 427.8762 ns | 239.61 |    1.59 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoopSpan     | Int32[35328] | 81,690.325 ns | 713.7939 ns | 667.6832 ns | 239.40 |    2.15 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | UnrolledManualLoop | Int32[35328] | 82,031.897 ns | 409.3806 ns | 382.9349 ns | 240.40 |    1.49 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | CopyBlock          | Int32[35328] | 18,018.080 ns | 354.1344 ns | 331.2575 ns |  52.80 |    0.97 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ArrayDotCopy       | Int32[3]     |     14.013 ns |   0.0624 ns |   0.0553 ns |   0.04 |    0.00 |  0.0005 |       - |       - |      40 B |       0.010 |
 *  | ManualLoop         | Int32[3]     |      9.016 ns |   0.0645 ns |   0.0604 ns |   0.03 |    0.00 |  0.0005 |       - |       - |      40 B |       0.010 |
 *  | ManualLoopSpan     | Int32[3]     |     10.974 ns |   0.0314 ns |   0.0263 ns |   0.03 |    0.00 |  0.0005 |       - |       - |      40 B |       0.010 |
 *  | UnrolledManualLoop | Int32[3]     |     10.328 ns |   0.0315 ns |   0.0279 ns |   0.03 |    0.00 |  0.0005 |       - |       - |      40 B |       0.010 |
 *  | CopyBlock          | Int32[3]     |     11.700 ns |   0.0951 ns |   0.0890 ns |   0.03 |    0.00 |  0.0005 |       - |       - |      40 B |       0.010 |
 *  | ArrayDotCopy       | Int32[512]   |    164.367 ns |   1.3324 ns |   1.1811 ns |   0.48 |    0.00 |  0.0246 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoop         | Int32[512]   |    308.328 ns |   2.6109 ns |   2.3145 ns |   0.90 |    0.01 |  0.0243 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoopSpan     | Int32[512]   |    304.989 ns |   1.0601 ns |   0.9916 ns |   0.89 |    0.00 |  0.0243 |       - |       - |    2072 B |       0.503 |
 *  | UnrolledManualLoop | Int32[512]   |    315.490 ns |   1.5028 ns |   1.3322 ns |   0.92 |    0.01 |  0.0243 |       - |       - |    2072 B |       0.503 |
 *  | CopyBlock          | Int32[512]   |    136.954 ns |   1.1618 ns |   1.0867 ns |   0.40 |    0.00 |  0.0246 |       - |       - |    2072 B |       0.503 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2655) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean          | Error         | StdDev        | Median        | Ratio  | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
 *  |------------------- |------------- |--------------:|--------------:|--------------:|--------------:|-------:|--------:|--------:|--------:|--------:|----------:|------------:|
 *  | ArrayDotCopy       | Int32[1024]  |    166.478 ns |     2.4900 ns |     2.2073 ns |    166.496 ns |   1.00 |    0.02 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoop         | Int32[1024]  |    452.750 ns |     3.7316 ns |     3.4906 ns |    452.694 ns |   2.72 |    0.04 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoopSpan     | Int32[1024]  |    446.079 ns |     4.2916 ns |     4.0144 ns |    444.948 ns |   2.68 |    0.04 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | UnrolledManualLoop | Int32[1024]  |    489.411 ns |     5.5378 ns |     5.1801 ns |    490.750 ns |   2.94 |    0.05 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | CopyBlock          | Int32[1024]  |    156.339 ns |     2.9746 ns |     3.0547 ns |    157.210 ns |   0.94 |    0.02 |  0.2460 |       - |       - |    4120 B |       1.000 |
 *  | ArrayDotCopy       | Int32[128]   |     27.210 ns |     0.5076 ns |     0.4748 ns |     27.142 ns |   0.16 |    0.00 |  0.0320 |       - |       - |     536 B |       0.130 |
 *  | ManualLoop         | Int32[128]   |     67.539 ns |     1.0309 ns |     0.9644 ns |     67.284 ns |   0.41 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | ManualLoopSpan     | Int32[128]   |     67.219 ns |     0.5582 ns |     0.5221 ns |     67.368 ns |   0.40 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | UnrolledManualLoop | Int32[128]   |     65.285 ns |     0.5297 ns |     0.4955 ns |     65.228 ns |   0.39 |    0.01 |  0.0319 |       - |       - |     536 B |       0.130 |
 *  | CopyBlock          | Int32[128]   |     26.329 ns |     0.2134 ns |     0.1996 ns |     26.376 ns |   0.16 |    0.00 |  0.0320 |       - |       - |     536 B |       0.130 |
 *  | ArrayDotCopy       | Int32[12]    |     10.478 ns |     0.0806 ns |     0.0754 ns |     10.453 ns |   0.06 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ManualLoop         | Int32[12]    |     11.961 ns |     0.2393 ns |     0.2121 ns |     11.965 ns |   0.07 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ManualLoopSpan     | Int32[12]    |     10.686 ns |     0.1371 ns |     0.1283 ns |     10.679 ns |   0.06 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | UnrolledManualLoop | Int32[12]    |     12.080 ns |     0.0733 ns |     0.0686 ns |     12.065 ns |   0.07 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | CopyBlock          | Int32[12]    |     12.709 ns |     0.0731 ns |     0.0611 ns |     12.718 ns |   0.08 |    0.00 |  0.0043 |       - |       - |      72 B |       0.017 |
 *  | ArrayDotCopy       | Int32[192]   |     38.813 ns |     0.8150 ns |     0.8005 ns |     38.579 ns |   0.23 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ManualLoop         | Int32[192]   |     93.773 ns |     0.7433 ns |     0.6589 ns |     93.741 ns |   0.56 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ManualLoopSpan     | Int32[192]   |     91.330 ns |     0.3684 ns |     0.3076 ns |     91.385 ns |   0.55 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | UnrolledManualLoop | Int32[192]   |     91.901 ns |     0.5426 ns |     0.5075 ns |     91.911 ns |   0.55 |    0.01 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | CopyBlock          | Int32[192]   |     36.832 ns |     0.5232 ns |     0.4638 ns |     36.845 ns |   0.22 |    0.00 |  0.0473 |       - |       - |     792 B |       0.192 |
 *  | ArrayDotCopy       | Int32[256]   |     47.911 ns |     0.3103 ns |     0.2903 ns |     47.908 ns |   0.29 |    0.00 |  0.0626 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoop         | Int32[256]   |    121.821 ns |     1.4008 ns |     1.2418 ns |    121.757 ns |   0.73 |    0.01 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoopSpan     | Int32[256]   |    122.810 ns |     2.5303 ns |     4.0860 ns |    120.784 ns |   0.74 |    0.03 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | UnrolledManualLoop | Int32[256]   |    126.058 ns |     2.0677 ns |     1.9341 ns |    125.530 ns |   0.76 |    0.01 |  0.0625 |       - |       - |    1048 B |       0.254 |
 *  | CopyBlock          | Int32[256]   |     42.192 ns |     0.8895 ns |     1.0244 ns |     42.108 ns |   0.25 |    0.01 |  0.0626 |       - |       - |    1048 B |       0.254 |
 *  | ArrayDotCopy       | Int32[35328] | 60,919.514 ns |   533.0634 ns |   498.6279 ns | 60,907.214 ns | 365.99 |    5.53 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | ManualLoop         | Int32[35328] | 60,547.013 ns |   369.6494 ns |   345.7703 ns | 60,613.208 ns | 363.75 |    5.09 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | ManualLoopSpan     | Int32[35328] | 60,955.413 ns |   342.4178 ns |   320.2978 ns | 60,897.559 ns | 366.21 |    5.06 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | UnrolledManualLoop | Int32[35328] | 61,596.587 ns | 1,219.1673 ns | 2,135.2733 ns | 60,606.055 ns | 370.06 |   13.54 | 43.4570 | 43.4570 | 43.4570 |  141351 B |      34.308 |
 *  | CopyBlock          | Int32[35328] |  8,415.391 ns |    90.8827 ns |    85.0117 ns |  8,400.739 ns |  50.56 |    0.82 | 43.4723 | 43.4723 | 43.4723 |  141351 B |      34.308 |
 *  | ArrayDotCopy       | Int32[3]     |      9.544 ns |     0.0391 ns |     0.0365 ns |      9.535 ns |   0.06 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ManualLoop         | Int32[3]     |      6.025 ns |     0.0836 ns |     0.0782 ns |      5.989 ns |   0.04 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ManualLoopSpan     | Int32[3]     |      7.565 ns |     0.1122 ns |     0.1050 ns |      7.576 ns |   0.05 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | UnrolledManualLoop | Int32[3]     |      7.689 ns |     0.0386 ns |     0.0361 ns |      7.686 ns |   0.05 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | CopyBlock          | Int32[3]     |      9.225 ns |     0.0863 ns |     0.0808 ns |      9.227 ns |   0.06 |    0.00 |  0.0024 |       - |       - |      40 B |       0.010 |
 *  | ArrayDotCopy       | Int32[512]   |     85.079 ns |     1.3087 ns |     1.2241 ns |     84.831 ns |   0.51 |    0.01 |  0.1239 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoop         | Int32[512]   |    228.827 ns |     2.1208 ns |     1.9838 ns |    229.436 ns |   1.37 |    0.02 |  0.1237 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoopSpan     | Int32[512]   |    232.315 ns |     1.4715 ns |     1.3765 ns |    232.118 ns |   1.40 |    0.02 |  0.1237 |       - |       - |    2072 B |       0.503 |
 *  | UnrolledManualLoop | Int32[512]   |    251.282 ns |     2.2536 ns |     2.1081 ns |    251.971 ns |   1.51 |    0.02 |  0.1235 |       - |       - |    2072 B |       0.503 |
 *  | CopyBlock          | Int32[512]   |     76.438 ns |     1.5000 ns |     1.4031 ns |     76.142 ns |   0.46 |    0.01 |  0.1239 |       - |       - |    2072 B |       0.503 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *
 *  | Method             | array        | Mean          | Error       | StdDev      | Median        | Ratio  | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
 *  |------------------- |------------- |--------------:|------------:|------------:|--------------:|-------:|--------:|--------:|--------:|--------:|----------:|------------:|
 *  | ArrayDotCopy       | Int32[1024]  |    199.989 ns |   0.7623 ns |   0.6365 ns |    199.611 ns |   1.00 |    0.00 |  0.6564 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoop         | Int32[1024]  |    463.312 ns |   1.1367 ns |   0.9492 ns |    463.127 ns |   2.32 |    0.01 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoopSpan     | Int32[1024]  |    476.869 ns |   2.0572 ns |   1.8236 ns |    476.698 ns |   2.38 |    0.01 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | UnrolledManualLoop | Int32[1024]  |    339.966 ns |   1.0548 ns |   0.9350 ns |    339.967 ns |   1.70 |    0.01 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | CopyBlock          | Int32[1024]  |    121.339 ns |   0.7548 ns |   0.6303 ns |    121.366 ns |   0.61 |    0.00 |  0.6564 |       - |       - |    4120 B |       1.000 |
 *  | ArrayDotCopy       | Int32[128]   |     27.314 ns |   0.4401 ns |   0.4117 ns |     27.496 ns |   0.14 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ManualLoop         | Int32[128]   |     67.059 ns |   0.1738 ns |   0.1541 ns |     67.055 ns |   0.34 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ManualLoopSpan     | Int32[128]   |     67.516 ns |   0.1477 ns |   0.1233 ns |     67.509 ns |   0.34 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | UnrolledManualLoop | Int32[128]   |     45.162 ns |   0.0737 ns |   0.0654 ns |     45.184 ns |   0.23 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | CopyBlock          | Int32[128]   |     19.525 ns |   0.0261 ns |   0.0218 ns |     19.520 ns |   0.10 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ArrayDotCopy       | Int32[12]    |      7.985 ns |   0.2023 ns |   0.3435 ns |      7.978 ns |   0.04 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ManualLoop         | Int32[12]    |     10.083 ns |   0.2442 ns |   0.3873 ns |      9.994 ns |   0.05 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ManualLoopSpan     | Int32[12]    |     10.091 ns |   0.1949 ns |   0.1823 ns |     10.080 ns |   0.05 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | UnrolledManualLoop | Int32[12]    |      7.860 ns |   0.1794 ns |   0.1590 ns |      7.805 ns |   0.04 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | CopyBlock          | Int32[12]    |      6.778 ns |   0.0795 ns |   0.0705 ns |      6.747 ns |   0.03 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ArrayDotCopy       | Int32[192]   |     38.627 ns |   0.2451 ns |   0.2293 ns |     38.636 ns |   0.19 |    0.00 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ManualLoop         | Int32[192]   |    104.075 ns |   2.1325 ns |   5.6552 ns |    101.966 ns |   0.52 |    0.03 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ManualLoopSpan     | Int32[192]   |    102.622 ns |   0.7561 ns |   0.7072 ns |    102.875 ns |   0.51 |    0.00 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | UnrolledManualLoop | Int32[192]   |     67.313 ns |   0.2843 ns |   0.2659 ns |     67.271 ns |   0.34 |    0.00 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | CopyBlock          | Int32[192]   |     27.161 ns |   0.4798 ns |   0.4712 ns |     27.005 ns |   0.14 |    0.00 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ArrayDotCopy       | Int32[256]   |     50.585 ns |   0.8960 ns |   0.7943 ns |     50.622 ns |   0.25 |    0.00 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoop         | Int32[256]   |    127.951 ns |   0.3253 ns |   0.2883 ns |    127.935 ns |   0.64 |    0.00 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoopSpan     | Int32[256]   |    130.715 ns |   0.9158 ns |   0.8566 ns |    130.823 ns |   0.65 |    0.00 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | UnrolledManualLoop | Int32[256]   |     89.334 ns |   1.7641 ns |   1.8875 ns |     88.494 ns |   0.45 |    0.01 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | CopyBlock          | Int32[256]   |     34.845 ns |   0.4448 ns |   0.4161 ns |     34.909 ns |   0.17 |    0.00 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | ArrayDotCopy       | Int32[35328] | 14,172.682 ns |  32.6831 ns |  25.5168 ns | 14,166.293 ns |  70.87 |    0.25 | 43.4723 | 43.4723 | 43.4723 |  141365 B |      34.312 |
 *  | ManualLoop         | Int32[35328] | 22,370.898 ns | 405.3837 ns | 359.3621 ns | 22,185.706 ns | 111.86 |    1.77 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoopSpan     | Int32[35328] | 22,316.615 ns | 340.8624 ns | 350.0406 ns | 22,205.228 ns | 111.59 |    1.74 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | UnrolledManualLoop | Int32[35328] | 18,232.040 ns | 129.4978 ns | 101.1034 ns | 18,207.483 ns |  91.17 |    0.56 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | CopyBlock          | Int32[35328] |  5,555.777 ns | 106.2812 ns | 244.1986 ns |  5,428.293 ns |  27.78 |    1.22 | 43.4723 | 43.4723 | 43.4723 |  141339 B |      34.306 |
 *  | ArrayDotCopy       | Int32[3]     |      6.153 ns |   0.0367 ns |   0.0325 ns |      6.158 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | ManualLoop         | Int32[3]     |      5.080 ns |   0.0234 ns |   0.0219 ns |      5.083 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | ManualLoopSpan     | Int32[3]     |      5.478 ns |   0.0220 ns |   0.0184 ns |      5.478 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | UnrolledManualLoop | Int32[3]     |      5.752 ns |   0.0534 ns |   0.0417 ns |      5.759 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | CopyBlock          | Int32[3]     |      5.774 ns |   0.0334 ns |   0.0296 ns |      5.777 ns |   0.03 |    0.00 |  0.0064 |       - |       - |      40 B |       0.010 |
 *  | ArrayDotCopy       | Int32[512]   |    101.976 ns |   1.6567 ns |   1.5497 ns |    102.194 ns |   0.51 |    0.01 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoop         | Int32[512]   |    240.174 ns |   1.0839 ns |   0.9608 ns |    240.230 ns |   1.20 |    0.01 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoopSpan     | Int32[512]   |    243.819 ns |   1.1068 ns |   0.9812 ns |    243.938 ns |   1.22 |    0.01 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | UnrolledManualLoop | Int32[512]   |    176.572 ns |   0.5168 ns |   0.4834 ns |    176.371 ns |   0.88 |    0.00 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *  | CopyBlock          | Int32[512]   |     60.887 ns |   0.5053 ns |   0.4220 ns |     60.797 ns |   0.30 |    0.00 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.6.9 (22G830) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method             | array        | Mean         | Error        | StdDev       | Median       | Ratio  | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
 *  |------------------- |------------- |-------------:|-------------:|-------------:|-------------:|-------:|--------:|--------:|--------:|--------:|----------:|------------:|
 *  | ArrayDotCopy       | Int32[1024]  |    312.46 ns |     4.794 ns |     4.484 ns |    313.03 ns |   1.00 |    0.02 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoop         | Int32[1024]  |    954.31 ns |    14.583 ns |    12.177 ns |    952.59 ns |   3.05 |    0.06 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | ManualLoopSpan     | Int32[1024]  |    529.51 ns |     9.835 ns |     9.200 ns |    526.95 ns |   1.69 |    0.04 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | UnrolledManualLoop | Int32[1024]  |    710.51 ns |    11.459 ns |    10.158 ns |    709.73 ns |   2.27 |    0.04 |  0.6561 |       - |       - |    4120 B |       1.000 |
 *  | CopyBlock          | Int32[1024]  |    227.94 ns |     2.611 ns |     2.180 ns |    227.13 ns |   0.73 |    0.01 |  0.6564 |       - |       - |    4120 B |       1.000 |
 *  | ArrayDotCopy       | Int32[128]   |     47.00 ns |     0.949 ns |     0.793 ns |     47.14 ns |   0.15 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ManualLoop         | Int32[128]   |    133.54 ns |     1.224 ns |     1.145 ns |    133.57 ns |   0.43 |    0.01 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ManualLoopSpan     | Int32[128]   |     82.37 ns |     1.040 ns |     0.922 ns |     82.04 ns |   0.26 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | UnrolledManualLoop | Int32[128]   |    100.56 ns |     0.822 ns |     0.687 ns |    100.37 ns |   0.32 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | CopyBlock          | Int32[128]   |     48.32 ns |     0.500 ns |     0.467 ns |     48.20 ns |   0.15 |    0.00 |  0.0854 |       - |       - |     536 B |       0.130 |
 *  | ArrayDotCopy       | Int32[12]    |     19.50 ns |     0.264 ns |     0.234 ns |     19.42 ns |   0.06 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ManualLoop         | Int32[12]    |     22.39 ns |     0.262 ns |     0.219 ns |     22.38 ns |   0.07 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ManualLoopSpan     | Int32[12]    |     18.25 ns |     0.246 ns |     0.218 ns |     18.22 ns |   0.06 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | UnrolledManualLoop | Int32[12]    |     20.68 ns |     0.206 ns |     0.193 ns |     20.71 ns |   0.07 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | CopyBlock          | Int32[12]    |     24.39 ns |     0.311 ns |     0.243 ns |     24.39 ns |   0.08 |    0.00 |  0.0115 |       - |       - |      72 B |       0.017 |
 *  | ArrayDotCopy       | Int32[192]   |     63.93 ns |     1.337 ns |     3.474 ns |     62.42 ns |   0.20 |    0.01 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ManualLoop         | Int32[192]   |    203.23 ns |     4.017 ns |     4.782 ns |    202.13 ns |   0.65 |    0.02 |  0.1261 |       - |       - |     792 B |       0.192 |
 *  | ManualLoopSpan     | Int32[192]   |    121.68 ns |     2.501 ns |     3.743 ns |    120.34 ns |   0.39 |    0.01 |  0.1261 |       - |       - |     792 B |       0.192 |
 *  | UnrolledManualLoop | Int32[192]   |    150.91 ns |     3.021 ns |     3.711 ns |    150.31 ns |   0.48 |    0.01 |  0.1261 |       - |       - |     792 B |       0.192 |
 *  | CopyBlock          | Int32[192]   |     66.96 ns |     1.361 ns |     1.908 ns |     67.14 ns |   0.21 |    0.01 |  0.1262 |       - |       - |     792 B |       0.192 |
 *  | ArrayDotCopy       | Int32[256]   |     82.99 ns |     1.706 ns |     2.755 ns |     82.07 ns |   0.27 |    0.01 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoop         | Int32[256]   |    277.52 ns |     5.609 ns |    15.820 ns |    272.75 ns |   0.89 |    0.05 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | ManualLoopSpan     | Int32[256]   |    154.58 ns |     3.005 ns |     3.340 ns |    155.63 ns |   0.49 |    0.01 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | UnrolledManualLoop | Int32[256]   |    197.95 ns |     3.961 ns |     5.150 ns |    198.74 ns |   0.63 |    0.02 |  0.1669 |       - |       - |    1048 B |       0.254 |
 *  | CopyBlock          | Int32[256]   |     80.56 ns |     1.671 ns |     2.053 ns |     80.90 ns |   0.26 |    0.01 |  0.1670 |       - |       - |    1048 B |       0.254 |
 *  | ArrayDotCopy       | Int32[35328] | 64,731.30 ns | 1,255.478 ns | 1,343.347 ns | 64,547.61 ns | 207.20 |    5.07 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoop         | Int32[35328] | 80,422.79 ns | 1,276.042 ns | 1,193.610 ns | 80,293.55 ns | 257.43 |    5.13 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | ManualLoopSpan     | Int32[35328] | 67,971.67 ns | 1,330.762 ns | 1,908.540 ns | 67,585.10 ns | 217.58 |    6.71 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | UnrolledManualLoop | Int32[35328] | 72,846.98 ns | 1,396.043 ns | 1,237.556 ns | 72,584.08 ns | 233.18 |    5.00 | 43.4570 | 43.4570 | 43.4570 |  141365 B |      34.312 |
 *  | CopyBlock          | Int32[35328] | 10,714.51 ns |   206.358 ns |   211.914 ns | 10,644.77 ns |  34.30 |    0.81 | 43.4723 | 43.4723 | 43.4723 |  141363 B |      34.311 |
 *  | ArrayDotCopy       | Int32[3]     |     17.39 ns |     0.361 ns |     0.320 ns |     17.31 ns |   0.06 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | ManualLoop         | Int32[3]     |     14.33 ns |     0.195 ns |     0.182 ns |     14.30 ns |   0.05 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | ManualLoopSpan     | Int32[3]     |     13.64 ns |     0.278 ns |     0.260 ns |     13.56 ns |   0.04 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | UnrolledManualLoop | Int32[3]     |     14.87 ns |     0.290 ns |     0.271 ns |     14.85 ns |   0.05 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | CopyBlock          | Int32[3]     |     22.81 ns |     0.223 ns |     0.198 ns |     22.77 ns |   0.07 |    0.00 |  0.0063 |       - |       - |      40 B |       0.010 |
 *  | ArrayDotCopy       | Int32[512]   |    140.71 ns |     1.622 ns |     1.355 ns |    140.29 ns |   0.45 |    0.01 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoop         | Int32[512]   |    480.80 ns |     5.349 ns |     5.003 ns |    479.43 ns |   1.54 |    0.03 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | ManualLoopSpan     | Int32[512]   |    273.54 ns |     5.297 ns |     4.696 ns |    272.61 ns |   0.88 |    0.02 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | UnrolledManualLoop | Int32[512]   |    360.95 ns |     3.048 ns |     2.546 ns |    361.07 ns |   1.16 |    0.02 |  0.3300 |       - |       - |    2072 B |       0.503 |
 *  | CopyBlock          | Int32[512]   |    125.44 ns |     1.825 ns |     1.524 ns |    125.65 ns |   0.40 |    0.01 |  0.3302 |       - |       - |    2072 B |       0.503 |
 *
 */

using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class ArrayCopy_Benchmark : BaseBenchmark
{
#pragma warning disable S2365 // Properties should not make collection or array copies
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
#pragma warning restore S2365 // Properties should not make collection or array copies

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int[] ArrayDotCopy(int[] array)
    {
        return ArrayDotCopyImpl(array);
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
