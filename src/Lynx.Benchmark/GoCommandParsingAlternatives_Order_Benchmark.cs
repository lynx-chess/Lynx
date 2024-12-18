/*
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.101
 *    [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *
 *  | Method      | Count | Command              | Mean         | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------ |------ |--------------------- |-------------:|----------:|----------:|------:|----------:|------------:|
 *  | Naive       | 1     | go bi(...)onder [49] |     153.6 ns |   1.56 ns |   1.30 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 1     | go bi(...)onder [49] |     160.5 ns |   0.86 ns |   0.80 ns |  1.05 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 1     | go po(...)c 500 [49] |     156.2 ns |   0.51 ns |   0.48 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 1     | go po(...)c 500 [49] |     145.4 ns |   0.48 ns |   0.42 ns |  0.93 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 1     | go wt(...)c 500 [42] |     144.8 ns |   0.13 ns |   0.11 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 1     | go wt(...)c 500 [42] |     139.0 ns |   0.16 ns |   0.12 ns |  0.96 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 1     | go wt(...)onder [49] |     155.4 ns |   0.57 ns |   0.48 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 1     | go wt(...)onder [49] |     149.2 ns |   1.25 ns |   1.10 ns |  0.96 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 10    | go bi(...)onder [49] |   1,546.7 ns |   4.22 ns |   3.74 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 10    | go bi(...)onder [49] |   1,604.3 ns |   5.51 ns |   4.89 ns |  1.04 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 10    | go po(...)c 500 [49] |   1,536.3 ns |   5.62 ns |   4.69 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 10    | go po(...)c 500 [49] |   1,459.2 ns |   4.64 ns |   3.88 ns |  0.95 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 10    | go wt(...)c 500 [42] |   1,448.5 ns |   1.32 ns |   1.03 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 10    | go wt(...)c 500 [42] |   1,363.1 ns |   4.48 ns |   3.97 ns |  0.94 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 10    | go wt(...)onder [49] |   1,537.3 ns |   4.39 ns |   3.66 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 10    | go wt(...)onder [49] |   1,476.0 ns |   4.58 ns |   4.28 ns |  0.96 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 100   | go bi(...)onder [49] |  15,406.1 ns |  87.03 ns |  77.15 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 100   | go bi(...)onder [49] |  15,924.5 ns |  73.29 ns |  64.97 ns |  1.03 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 100   | go po(...)c 500 [49] |  15,159.1 ns |  14.93 ns |  11.66 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 100   | go po(...)c 500 [49] |  14,304.8 ns |  60.75 ns |  53.86 ns |  0.94 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 100   | go wt(...)c 500 [42] |  14,549.0 ns |  45.96 ns |  40.74 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 100   | go wt(...)c 500 [42] |  14,210.1 ns |  26.92 ns |  23.87 ns |  0.98 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 100   | go wt(...)onder [49] |  15,410.1 ns |  31.46 ns |  26.27 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 100   | go wt(...)onder [49] |  14,500.1 ns |  53.36 ns |  49.91 ns |  0.94 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 1000  | go bi(...)onder [49] | 151,754.1 ns | 463.92 ns | 411.25 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 1000  | go bi(...)onder [49] | 157,853.6 ns | 156.02 ns | 130.28 ns |  1.04 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 1000  | go po(...)c 500 [49] | 154,193.1 ns | 504.00 ns | 471.44 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 1000  | go po(...)c 500 [49] | 143,428.2 ns | 310.06 ns | 258.92 ns |  0.93 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 1000  | go wt(...)c 500 [42] | 144,971.6 ns | 602.77 ns | 534.34 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 1000  | go wt(...)c 500 [42] | 134,861.6 ns | 580.61 ns | 514.70 ns |  0.93 |         - |          NA |
 *  |             |       |                      |              |           |           |       |           |             |
 *  | Naive       | 1000  | go wt(...)onder [49] | 151,643.2 ns | 811.92 ns | 719.74 ns |  1.00 |         - |          NA |
 *  | Wtime_First | 1000  | go wt(...)onder [49] | 152,425.0 ns | 622.47 ns | 582.26 ns |  1.01 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2849) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.101
 *    [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *
 *  | Method      | Count | Command              | Mean         | Error       | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------ |------ |--------------------- |-------------:|------------:|------------:|------:|--------:|----------:|------------:|
 *  | Naive       | 1     | go bi(...)onder [49] |     161.9 ns |     2.81 ns |     2.49 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 1     | go bi(...)onder [49] |     163.2 ns |     0.24 ns |     0.22 ns |  1.01 |    0.02 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 1     | go po(...)c 500 [49] |     165.6 ns |     2.55 ns |     2.26 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 1     | go po(...)c 500 [49] |     150.7 ns |     0.32 ns |     0.27 ns |  0.91 |    0.01 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 1     | go wt(...)c 500 [42] |     148.1 ns |     2.29 ns |     2.15 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 1     | go wt(...)c 500 [42] |     134.0 ns |     0.16 ns |     0.14 ns |  0.90 |    0.01 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 1     | go wt(...)onder [49] |     162.4 ns |     2.31 ns |     2.16 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 1     | go wt(...)onder [49] |     150.9 ns |     0.20 ns |     0.18 ns |  0.93 |    0.01 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 10    | go bi(...)onder [49] |   1,554.2 ns |     1.88 ns |     1.76 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 10    | go bi(...)onder [49] |   1,627.4 ns |     3.90 ns |     3.46 ns |  1.05 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 10    | go po(...)c 500 [49] |   1,580.2 ns |     2.74 ns |     2.57 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 10    | go po(...)c 500 [49] |   1,496.5 ns |     3.52 ns |     2.94 ns |  0.95 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 10    | go wt(...)c 500 [42] |   1,437.2 ns |     1.23 ns |     1.09 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 10    | go wt(...)c 500 [42] |   1,345.0 ns |     2.19 ns |     1.94 ns |  0.94 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 10    | go wt(...)onder [49] |   1,559.4 ns |     1.41 ns |     1.25 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 10    | go wt(...)onder [49] |   1,515.2 ns |     4.39 ns |     3.43 ns |  0.97 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 100   | go bi(...)onder [49] |  15,486.2 ns |    25.10 ns |    20.96 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 100   | go bi(...)onder [49] |  16,142.9 ns |    14.52 ns |    13.58 ns |  1.04 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 100   | go po(...)c 500 [49] |  15,579.0 ns |    12.05 ns |    10.06 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 100   | go po(...)c 500 [49] |  15,206.8 ns |    13.89 ns |    12.32 ns |  0.98 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 100   | go wt(...)c 500 [42] |  14,264.2 ns |    19.28 ns |    15.05 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 100   | go wt(...)c 500 [42] |  13,460.1 ns |    14.28 ns |    12.66 ns |  0.94 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 100   | go wt(...)onder [49] |  15,554.8 ns |    14.62 ns |    13.68 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 100   | go wt(...)onder [49] |  14,825.9 ns |    13.79 ns |    12.90 ns |  0.95 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 1000  | go bi(...)onder [49] | 156,281.3 ns |   306.35 ns |   271.57 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 1000  | go bi(...)onder [49] | 162,792.2 ns |   117.87 ns |   104.49 ns |  1.04 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 1000  | go po(...)c 500 [49] | 156,847.8 ns |   251.70 ns |   223.12 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 1000  | go po(...)c 500 [49] | 148,838.4 ns |   381.89 ns |   318.90 ns |  0.95 |    0.00 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 1000  | go wt(...)c 500 [42] | 142,851.4 ns |   109.88 ns |    91.76 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 1000  | go wt(...)c 500 [42] | 153,534.2 ns | 1,769.17 ns | 1,568.33 ns |  1.07 |    0.01 |         - |          NA |
 *  |             |       |                      |              |             |             |       |         |           |             |
 *  | Naive       | 1000  | go wt(...)onder [49] | 155,240.0 ns |   165.77 ns |   146.95 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 1000  | go wt(...)onder [49] | 148,515.9 ns |    62.10 ns |    51.86 ns |  0.96 |    0.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.101
 *    [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
 *
 *  | Method      | Count | Command              | Mean          | Error        | StdDev       | Median        | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------ |------ |--------------------- |--------------:|-------------:|-------------:|--------------:|------:|--------:|----------:|------------:|
 *  | Naive       | 1     | go bi(...)onder [49] |      96.88 ns |     0.474 ns |     0.420 ns |      96.79 ns |  1.00 |    0.01 |         - |          NA |
 *  | Wtime_First | 1     | go bi(...)onder [49] |     100.34 ns |     0.457 ns |     0.427 ns |     100.11 ns |  1.04 |    0.01 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 1     | go po(...)c 500 [49] |      97.50 ns |     1.532 ns |     2.385 ns |      96.63 ns |  1.00 |    0.03 |         - |          NA |
 *  | Wtime_First | 1     | go po(...)c 500 [49] |     102.16 ns |     2.062 ns |     3.445 ns |     101.81 ns |  1.05 |    0.04 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 1     | go wt(...)c 500 [42] |      89.76 ns |     1.832 ns |     3.617 ns |      90.54 ns |  1.00 |    0.06 |         - |          NA |
 *  | Wtime_First | 1     | go wt(...)c 500 [42] |      96.22 ns |     2.294 ns |     6.619 ns |      94.62 ns |  1.07 |    0.08 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 1     | go wt(...)onder [49] |     107.01 ns |     2.042 ns |     1.910 ns |     106.87 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 1     | go wt(...)onder [49] |     105.69 ns |     2.169 ns |     6.396 ns |     104.35 ns |  0.99 |    0.06 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 10    | go bi(...)onder [49] |   1,077.78 ns |    20.032 ns |    17.758 ns |   1,072.37 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 10    | go bi(...)onder [49] |   1,102.21 ns |    20.390 ns |    25.041 ns |   1,099.17 ns |  1.02 |    0.03 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 10    | go po(...)c 500 [49] |   1,054.03 ns |     9.869 ns |     9.231 ns |   1,051.55 ns |  1.00 |    0.01 |         - |          NA |
 *  | Wtime_First | 10    | go po(...)c 500 [49] |     981.19 ns |    10.813 ns |     9.029 ns |     979.42 ns |  0.93 |    0.01 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 10    | go wt(...)c 500 [42] |     903.25 ns |    17.800 ns |    39.444 ns |     882.93 ns |  1.00 |    0.06 |         - |          NA |
 *  | Wtime_First | 10    | go wt(...)c 500 [42] |     893.80 ns |    13.903 ns |    17.583 ns |     885.64 ns |  0.99 |    0.05 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 10    | go wt(...)onder [49] |   1,097.32 ns |    20.927 ns |    27.211 ns |   1,096.25 ns |  1.00 |    0.03 |         - |          NA |
 *  | Wtime_First | 10    | go wt(...)onder [49] |     985.91 ns |    19.706 ns |    20.237 ns |     983.12 ns |  0.90 |    0.03 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 100   | go bi(...)onder [49] |  10,545.51 ns |   209.468 ns |   349.974 ns |  10,551.52 ns |  1.00 |    0.05 |         - |          NA |
 *  | Wtime_First | 100   | go bi(...)onder [49] |  10,475.98 ns |   206.308 ns |   521.366 ns |  10,170.74 ns |  0.99 |    0.06 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 100   | go po(...)c 500 [49] |  10,066.17 ns |   150.867 ns |   133.739 ns |  10,123.95 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 100   | go po(...)c 500 [49] |   9,483.95 ns |   139.329 ns |   123.512 ns |   9,500.67 ns |  0.94 |    0.02 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 100   | go wt(...)c 500 [42] |   8,652.80 ns |    10.686 ns |     8.923 ns |   8,650.90 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 100   | go wt(...)c 500 [42] |   8,124.43 ns |    78.497 ns |    69.586 ns |   8,091.75 ns |  0.94 |    0.01 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 100   | go wt(...)onder [49] |   9,748.07 ns |    22.296 ns |    19.765 ns |   9,743.84 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 100   | go wt(...)onder [49] |   9,306.79 ns |    84.905 ns |    70.900 ns |   9,275.81 ns |  0.95 |    0.01 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 1000  | go bi(...)onder [49] | 103,853.30 ns | 1,947.968 ns | 1,913.165 ns | 104,085.09 ns |  1.00 |    0.03 |         - |          NA |
 *  | Wtime_First | 1000  | go bi(...)onder [49] | 108,393.58 ns |   527.856 ns |   440.784 ns | 108,401.36 ns |  1.04 |    0.02 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 1000  | go po(...)c 500 [49] | 105,285.22 ns | 1,481.100 ns | 1,236.786 ns | 104,670.29 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 1000  | go po(...)c 500 [49] |  91,811.02 ns | 1,396.738 ns | 1,306.510 ns |  91,259.33 ns |  0.87 |    0.02 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 1000  | go wt(...)c 500 [42] |  86,470.59 ns |   160.486 ns |   142.267 ns |  86,427.65 ns |  1.00 |    0.00 |         - |          NA |
 *  | Wtime_First | 1000  | go wt(...)c 500 [42] |  81,103.86 ns |   217.290 ns |   169.646 ns |  81,072.24 ns |  0.94 |    0.00 |         - |          NA |
 *  |             |       |                      |               |              |              |               |       |         |           |             |
 *  | Naive       | 1000  | go wt(...)onder [49] |  98,201.45 ns | 1,272.329 ns | 1,190.138 ns |  97,661.84 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 1000  | go wt(...)onder [49] |  95,086.29 ns | 1,874.662 ns | 2,688.584 ns |  93,375.23 ns |  0.97 |    0.03 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.1 (22H221) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.101
 *    [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *
 *  | Method      | Count | Command              | Mean         | Error       | StdDev       | Median       | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------ |------ |--------------------- |-------------:|------------:|-------------:|-------------:|------:|--------:|----------:|------------:|
 *  | Naive       | 1     | go bi(...)onder [49] |     239.3 ns |    11.21 ns |     32.52 ns |     231.9 ns |  1.02 |    0.19 |         - |          NA |
 *  | Wtime_First | 1     | go bi(...)onder [49] |     301.0 ns |    18.16 ns |     52.09 ns |     287.1 ns |  1.28 |    0.28 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 1     | go po(...)c 500 [49] |     298.6 ns |    15.80 ns |     44.81 ns |     302.3 ns |  1.02 |    0.22 |         - |          NA |
 *  | Wtime_First | 1     | go po(...)c 500 [49] |     279.5 ns |    17.29 ns |     49.90 ns |     276.0 ns |  0.96 |    0.22 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 1     | go wt(...)c 500 [42] |     296.8 ns |    16.92 ns |     49.88 ns |     301.7 ns |  1.03 |    0.26 |         - |          NA |
 *  | Wtime_First | 1     | go wt(...)c 500 [42] |     201.8 ns |     4.64 ns |     13.32 ns |     200.8 ns |  0.70 |    0.13 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 1     | go wt(...)onder [49] |     217.0 ns |     4.39 ns |     10.09 ns |     216.7 ns |  1.00 |    0.07 |         - |          NA |
 *  | Wtime_First | 1     | go wt(...)onder [49] |     215.9 ns |     3.63 ns |      3.88 ns |     216.2 ns |  1.00 |    0.05 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 10    | go bi(...)onder [49] |   2,172.4 ns |    42.74 ns |     94.70 ns |   2,162.4 ns |  1.00 |    0.06 |         - |          NA |
 *  | Wtime_First | 10    | go bi(...)onder [49] |   2,154.6 ns |    80.72 ns |    234.20 ns |   2,076.0 ns |  0.99 |    0.12 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 10    | go po(...)c 500 [49] |   1,916.3 ns |    38.05 ns |    101.55 ns |   1,893.9 ns |  1.00 |    0.07 |         - |          NA |
 *  | Wtime_First | 10    | go po(...)c 500 [49] |   2,021.2 ns |    56.34 ns |    164.34 ns |   2,038.9 ns |  1.06 |    0.10 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 10    | go wt(...)c 500 [42] |   2,097.0 ns |    46.97 ns |    136.27 ns |   2,131.2 ns |  1.00 |    0.09 |         - |          NA |
 *  | Wtime_First | 10    | go wt(...)c 500 [42] |   2,031.6 ns |    65.05 ns |    183.48 ns |   2,051.9 ns |  0.97 |    0.11 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 10    | go wt(...)onder [49] |   2,253.6 ns |    44.77 ns |     86.25 ns |   2,233.1 ns |  1.00 |    0.05 |         - |          NA |
 *  | Wtime_First | 10    | go wt(...)onder [49] |   2,084.3 ns |    62.07 ns |    181.06 ns |   2,095.2 ns |  0.93 |    0.09 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 100   | go bi(...)onder [49] |  20,672.7 ns |   560.24 ns |  1,543.07 ns |  20,733.4 ns |  1.01 |    0.11 |         - |          NA |
 *  | Wtime_First | 100   | go bi(...)onder [49] |  23,811.9 ns |   519.56 ns |  1,413.50 ns |  23,413.4 ns |  1.16 |    0.11 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 100   | go po(...)c 500 [49] |  22,391.0 ns |   447.76 ns |    772.36 ns |  22,302.4 ns |  1.00 |    0.05 |         - |          NA |
 *  | Wtime_First | 100   | go po(...)c 500 [49] |  22,345.2 ns |   785.43 ns |  2,189.46 ns |  22,190.5 ns |  1.00 |    0.10 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 100   | go wt(...)c 500 [42] |  20,332.2 ns |   459.38 ns |  1,325.41 ns |  20,444.4 ns |  1.00 |    0.09 |         - |          NA |
 *  | Wtime_First | 100   | go wt(...)c 500 [42] |  18,974.6 ns |   586.68 ns |  1,729.83 ns |  18,708.4 ns |  0.94 |    0.11 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 100   | go wt(...)onder [49] |  18,215.7 ns |   343.60 ns |    701.89 ns |  18,047.3 ns |  1.00 |    0.05 |         - |          NA |
 *  | Wtime_First | 100   | go wt(...)onder [49] |  18,357.5 ns |   365.79 ns |    722.03 ns |  18,201.4 ns |  1.01 |    0.05 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 1000  | go bi(...)onder [49] | 176,648.6 ns | 2,839.43 ns |  2,517.08 ns | 176,192.9 ns |  1.00 |    0.02 |         - |          NA |
 *  | Wtime_First | 1000  | go bi(...)onder [49] | 189,931.2 ns | 3,757.38 ns |  5,267.33 ns | 188,648.9 ns |  1.08 |    0.03 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 1000  | go po(...)c 500 [49] | 180,580.6 ns | 3,604.50 ns |  3,371.65 ns | 181,157.9 ns |  1.00 |    0.03 |         - |          NA |
 *  | Wtime_First | 1000  | go po(...)c 500 [49] | 172,708.5 ns | 3,256.45 ns |  3,344.14 ns | 171,903.2 ns |  0.96 |    0.03 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 1000  | go wt(...)c 500 [42] | 183,192.9 ns | 3,649.72 ns |  5,573.50 ns | 182,498.4 ns |  1.00 |    0.04 |         - |          NA |
 *  | Wtime_First | 1000  | go wt(...)c 500 [42] | 163,547.1 ns | 3,224.16 ns |  5,474.87 ns | 161,570.8 ns |  0.89 |    0.04 |         - |          NA |
 *  |             |       |                      |              |             |              |              |       |         |           |             |
 *  | Naive       | 1000  | go wt(...)onder [49] | 191,297.9 ns | 3,798.62 ns |  9,174.10 ns | 189,930.3 ns |  1.00 |    0.07 |         - |          NA |
 *  | Wtime_First | 1000  | go wt(...)onder [49] | 190,973.6 ns | 5,066.37 ns | 14,938.31 ns | 186,032.1 ns |  1.00 |    0.09 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Utilities;
using NLog;

namespace Lynx.Benchmark;

/// <summary>
/// Implementation chosen from <see cref="GoCommandParsingAlternatives_Benchmark"/>
/// </summary>
public class GoCommandParsingAlternatives_Order_Benchmark : BaseBenchmark
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [Params(1, 10, 100, 1000)]
    public int Count { get; set; }

    [ParamsSource(nameof(Data))]
    public string? Command { get; set; }

    public static IEnumerable<string> Data =>
    [
        "go wtime 7000 winc 500 btime 8000 binc 500",
        "go ponder wtime 7000 winc 500 btime 8000 binc 500",
        "go wtime 7000 winc 500 btime 8000 binc 500 ponder",
        "go binc 500 winc 500 btime 8000 wtime 7000 ponder",
    ];

    [Benchmark(Baseline = true)]
    public void Naive()
    {
        var command = Command!;

        for (int i = 0; i < Count; ++i)
        {
            ParseNaive(command);
        }
    }

    [Benchmark]
    public void Wtime_First()
    {
        var command = Command!;

        for (int i = 0; i < Count; ++i)
        {
            ParseWtime_First(command);
        }
    }

    public List<string> SearchMoves { get; } = default!;
    public int WhiteTime { get; private set; } = default!;
    public int BlackTime { get; private set; } = default!;
    public int WhiteIncrement { get; private set; } = default!;
    public int BlackIncrement { get; private set; } = default!;
    public int MovesToGo { get; private set; } = default!;
    public int Depth { get; private set; } = default!;
    public int Nodes { get; } = default!;
    public int Mate { get; } = default!;
    public int MoveTime { get; private set; } = default!;
    public bool Infinite { get; private set; } = default!;
    public bool Ponder { get; private set; } = default!;
    private static ReadOnlySpan<char> WtimeSpan => "wtime".AsSpan();
    private static ReadOnlySpan<char> BtimeSpan => "btime".AsSpan();
    private static ReadOnlySpan<char> WincSpan => "winc".AsSpan();
    private static ReadOnlySpan<char> BincSpan => "binc".AsSpan();
    private static ReadOnlySpan<char> MovestogoSpan => "movestogo".AsSpan();
    private static ReadOnlySpan<char> MovetimeSpan => "movetime".AsSpan();
    private static ReadOnlySpan<char> DepthSpan => "depth".AsSpan();
    private static ReadOnlySpan<char> InfiniteSpan => "infinite".AsSpan();
    private static ReadOnlySpan<char> PonderSpan => "ponder".AsSpan();
    private static ReadOnlySpan<char> NodesSpan => "nodes".AsSpan();
    private static ReadOnlySpan<char> MateSpan => "mate".AsSpan();
    private static ReadOnlySpan<char> SearchmovesSpan => "searchmoves".AsSpan();

    private void ParseNaive(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];

            if (key.Equals(WtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteTime = value;
                }
            }
            else if (key.Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackTime = value;
                }
            }
            else if (key.Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteIncrement = value;
                }
            }
            else if (key.Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackIncrement = value;
                }
            }
            else if (key.Equals(MovestogoSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MovesToGo = value;
                }
            }
            else if (key.Equals(MovetimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MoveTime = value;
                }
            }
            else if (key.Equals(DepthSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    Depth = value;
                }
            }
            else if (key.Equals(InfiniteSpan, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderSpan, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;
            }
            else if (key.Equals(NodesSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchmovesSpan, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new InvalidDataException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    private void ParseWtime_First(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];

            if (key.Equals(WtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteTime = value;
                }

                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteIncrement = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackIncrement = value;
                    }
                }
            }
            else if (key.Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackTime = value;
                }
            }
            else if (key.Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteIncrement = value;
                }
            }
            else if (key.Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackIncrement = value;
                }
            }
            else if (key.Equals(MovestogoSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MovesToGo = value;
                }
            }
            else if (key.Equals(MovetimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MoveTime = value;
                }
            }
            else if (key.Equals(DepthSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    Depth = value;
                }
            }
            else if (key.Equals(InfiniteSpan, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderSpan, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;
                int value;

                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteIncrement = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackIncrement = value;
                    }
                }
            }
            else if (key.Equals(NodesSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchmovesSpan, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new InvalidDataException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }
}
