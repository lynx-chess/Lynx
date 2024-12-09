/*
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.101
 *    [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *
 *
 *  | Method                                   | command              | Mean       | Error    | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |----------------------------------------- |--------------------- |-----------:|---------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | Sequential                               | go infinite          | 1,483.0 ns | 20.03 ns |  18.74 ns |  1.00 |    0.02 | 0.0019 |     272 B |        1.00 |
 *  | Parallel                                 | go infinite          | 3,223.2 ns | 64.17 ns | 154.98 ns |  2.17 |    0.11 | 0.0153 |    1822 B |        6.70 |
 *  | CapturingGroups                          | go infinite          | 1,271.1 ns |  5.60 ns |   4.97 ns |  0.86 |    0.01 | 0.0172 |    1504 B |        5.53 |
 *  | NoRegex                                  | go infinite          |   734.4 ns |  9.31 ns |   8.71 ns |  0.50 |    0.01 | 0.0029 |     272 B |        1.00 |
 *  | NoRegex_DictionaryAction                 | go infinite          |   720.0 ns |  9.13 ns |   8.54 ns |  0.49 |    0.01 | 0.0029 |     312 B |        1.15 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go infinite          |   868.7 ns |  3.73 ns |   3.30 ns |  0.59 |    0.01 | 0.0029 |     312 B |        1.15 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go infinite          |   748.9 ns |  6.02 ns |   5.63 ns |  0.51 |    0.01 | 0.0029 |     312 B |        1.15 |
 *  | NoRegex_ReadOnlySpanComparison           | go infinite          |   724.8 ns |  6.53 ns |   5.78 ns |  0.49 |    0.01 | 0.0029 |     272 B |        1.00 |
 *  |                                          |                      |            |          |           |       |         |        |           |             |
 *  | Sequential                               | go wt(...)c 500 [42] | 3,569.1 ns | 35.79 ns |  33.48 ns |  1.00 |    0.01 | 0.0114 |    1248 B |        1.00 |
 *  | Parallel                                 | go wt(...)c 500 [42] | 4,096.2 ns | 70.57 ns | 151.91 ns |  1.15 |    0.04 | 0.0305 |    2800 B |        2.24 |
 *  | CapturingGroups                          | go wt(...)c 500 [42] | 2,873.7 ns | 45.78 ns |  42.82 ns |  0.81 |    0.01 | 0.0572 |    4800 B |        3.85 |
 *  | NoRegex                                  | go wt(...)c 500 [42] |   900.5 ns |  9.23 ns |   8.63 ns |  0.25 |    0.00 | 0.0019 |     272 B |        0.22 |
 *  | NoRegex_DictionaryAction                 | go wt(...)c 500 [42] | 1,087.4 ns |  7.07 ns |   6.26 ns |  0.30 |    0.00 | 0.0038 |     400 B |        0.32 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)c 500 [42] | 1,659.2 ns | 11.66 ns |  10.34 ns |  0.46 |    0.01 | 0.0076 |     784 B |        0.63 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)c 500 [42] | 1,062.2 ns |  9.55 ns |   8.93 ns |  0.30 |    0.00 | 0.0038 |     400 B |        0.32 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)c 500 [42] |   912.1 ns |  7.88 ns |   7.37 ns |  0.26 |    0.00 | 0.0029 |     272 B |        0.22 |
 *  |                                          |                      |            |          |           |       |         |        |           |             |
 *  | Sequential                               | go wt(...)00000 [78] | 3,827.4 ns | 63.06 ns |  58.99 ns |  1.00 |    0.02 | 0.0191 |    1728 B |        1.00 |
 *  | Parallel                                 | go wt(...)00000 [78] | 4,666.2 ns | 93.19 ns | 196.57 ns |  1.22 |    0.05 | 0.0381 |    3280 B |        1.90 |
 *  | CapturingGroups                          | go wt(...)00000 [78] | 4,536.0 ns | 88.17 ns | 123.60 ns |  1.19 |    0.04 | 0.0839 |    7064 B |        4.09 |
 *  | NoRegex                                  | go wt(...)00000 [78] |   920.6 ns |  8.38 ns |   7.43 ns |  0.24 |    0.00 | 0.0019 |     272 B |        0.16 |
 *  | NoRegex_DictionaryAction                 | go wt(...)00000 [78] | 1,187.0 ns |  3.34 ns |   2.96 ns |  0.31 |    0.00 | 0.0057 |     504 B |        0.29 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)00000 [78] | 2,409.8 ns | 48.02 ns |  58.97 ns |  0.63 |    0.02 | 0.0114 |    1088 B |        0.63 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)00000 [78] | 1,195.9 ns |  4.15 ns |   3.68 ns |  0.31 |    0.00 | 0.0057 |     504 B |        0.29 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)00000 [78] | 1,062.3 ns |  5.13 ns |   4.79 ns |  0.28 |    0.00 | 0.0019 |     272 B |        0.16 |
 *  |                                          |                      |            |          |           |       |         |        |           |             |
 *  | Sequential                               | go wt(...)go 40 [62] | 3,340.7 ns | 66.03 ns |  78.60 ns |  1.00 |    0.03 | 0.0153 |    1488 B |        1.00 |
 *  | Parallel                                 | go wt(...)go 40 [62] | 4,440.2 ns | 88.39 ns | 197.71 ns |  1.33 |    0.07 | 0.0305 |    3040 B |        2.04 |
 *  | CapturingGroups                          | go wt(...)go 40 [62] | 4,256.4 ns | 81.40 ns | 111.43 ns |  1.27 |    0.04 | 0.0839 |    7032 B |        4.73 |
 *  | NoRegex                                  | go wt(...)go 40 [62] |   891.4 ns |  3.82 ns |   3.58 ns |  0.27 |    0.01 | 0.0029 |     272 B |        0.18 |
 *  | NoRegex_DictionaryAction                 | go wt(...)go 40 [62] | 1,102.6 ns |  4.03 ns |   3.77 ns |  0.33 |    0.01 | 0.0057 |     480 B |        0.32 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)go 40 [62] | 2,107.2 ns | 13.83 ns |  12.94 ns |  0.63 |    0.02 | 0.0114 |     968 B |        0.65 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)go 40 [62] | 1,125.3 ns |  3.62 ns |   3.02 ns |  0.34 |    0.01 | 0.0057 |     480 B |        0.32 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)go 40 [62] |   917.6 ns |  4.75 ns |   4.21 ns |  0.27 |    0.01 | 0.0029 |     272 B |        0.18 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2849) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.101
 *    [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *
 *  | Method                                   | command              | Mean       | Error    | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |----------------------------------------- |--------------------- |-----------:|---------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Sequential                               | go infinite          | 1,550.1 ns | 18.80 ns |  17.59 ns |  1.00 |    0.02 | 0.0153 |      - |     271 B |        1.00 |
 *  | Parallel                                 | go infinite          | 2,536.8 ns | 17.51 ns |  15.52 ns |  1.64 |    0.02 | 0.1068 |      - |    1820 B |        6.72 |
 *  | CapturingGroups                          | go infinite          | 1,068.2 ns |  5.45 ns |   5.10 ns |  0.69 |    0.01 | 0.0896 |      - |    1504 B |        5.55 |
 *  | NoRegex                                  | go infinite          |   662.5 ns |  2.72 ns |   2.55 ns |  0.43 |    0.01 | 0.0162 |      - |     272 B |        1.00 |
 *  | NoRegex_DictionaryAction                 | go infinite          |   705.7 ns |  5.98 ns |   5.60 ns |  0.46 |    0.01 | 0.0172 |      - |     312 B |        1.15 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go infinite          |   741.8 ns |  5.34 ns |   5.00 ns |  0.48 |    0.01 | 0.0181 |      - |     312 B |        1.15 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go infinite          |   708.4 ns |  3.07 ns |   2.87 ns |  0.46 |    0.01 | 0.0181 |      - |     312 B |        1.15 |
 *  | NoRegex_ReadOnlySpanComparison           | go infinite          |   689.2 ns |  3.24 ns |   3.03 ns |  0.44 |    0.01 | 0.0153 |      - |     272 B |        1.00 |
 *  |                                          |                      |            |          |           |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)c 500 [42] | 3,204.6 ns | 23.94 ns |  22.39 ns |  1.00 |    0.01 | 0.0725 |      - |    1247 B |        1.00 |
 *  | Parallel                                 | go wt(...)c 500 [42] | 3,616.0 ns | 72.25 ns | 132.11 ns |  1.13 |    0.04 | 0.1678 |      - |    2800 B |        2.25 |
 *  | CapturingGroups                          | go wt(...)c 500 [42] | 2,424.8 ns |  9.21 ns |   8.61 ns |  0.76 |    0.01 | 0.2861 | 0.0038 |    4799 B |        3.85 |
 *  | NoRegex                                  | go wt(...)c 500 [42] |   844.1 ns |  3.31 ns |   2.94 ns |  0.26 |    0.00 | 0.0162 |      - |     272 B |        0.22 |
 *  | NoRegex_DictionaryAction                 | go wt(...)c 500 [42] |   853.8 ns |  1.70 ns |   1.42 ns |  0.27 |    0.00 | 0.0229 |      - |     400 B |        0.32 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)c 500 [42] | 1,399.0 ns |  6.58 ns |   5.14 ns |  0.44 |    0.00 | 0.0458 |      - |     784 B |        0.63 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)c 500 [42] |   871.8 ns |  2.50 ns |   1.95 ns |  0.27 |    0.00 | 0.0229 |      - |     400 B |        0.32 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)c 500 [42] |   839.4 ns |  2.87 ns |   2.69 ns |  0.26 |    0.00 | 0.0153 |      - |     272 B |        0.22 |
 *  |                                          |                      |            |          |           |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)00000 [78] | 4,220.3 ns | 12.18 ns |  10.17 ns |  1.00 |    0.00 | 0.0992 |      - |    1727 B |        1.00 |
 *  | Parallel                                 | go wt(...)00000 [78] | 4,378.3 ns | 87.02 ns | 245.43 ns |  1.04 |    0.06 | 0.1907 |      - |    3280 B |        1.90 |
 *  | CapturingGroups                          | go wt(...)00000 [78] | 3,620.8 ns | 44.23 ns |  41.37 ns |  0.86 |    0.01 | 0.4196 | 0.0076 |    7063 B |        4.09 |
 *  | NoRegex                                  | go wt(...)00000 [78] |   866.4 ns |  3.90 ns |   3.64 ns |  0.21 |    0.00 | 0.0162 |      - |     272 B |        0.16 |
 *  | NoRegex_DictionaryAction                 | go wt(...)00000 [78] | 1,222.8 ns |  4.58 ns |   4.29 ns |  0.29 |    0.00 | 0.0286 |      - |     503 B |        0.29 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)00000 [78] | 1,828.1 ns |  6.08 ns |   5.39 ns |  0.43 |    0.00 | 0.0648 |      - |    1088 B |        0.63 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)00000 [78] | 1,227.5 ns |  3.80 ns |   3.37 ns |  0.29 |    0.00 | 0.0286 |      - |     503 B |        0.29 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)00000 [78] |   934.1 ns |  6.21 ns |   5.81 ns |  0.22 |    0.00 | 0.0153 |      - |     272 B |        0.16 |
 *  |                                          |                      |            |          |           |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)go 40 [62] | 3,916.9 ns | 30.81 ns |  28.82 ns |  1.00 |    0.01 | 0.0839 |      - |    1486 B |        1.00 |
 *  | Parallel                                 | go wt(...)go 40 [62] | 3,916.9 ns | 63.58 ns |  59.47 ns |  1.00 |    0.02 | 0.1755 |      - |    3040 B |        2.05 |
 *  | CapturingGroups                          | go wt(...)go 40 [62] | 3,271.6 ns |  5.10 ns |   4.77 ns |  0.84 |    0.01 | 0.4196 | 0.0076 |    7031 B |        4.73 |
 *  | NoRegex                                  | go wt(...)go 40 [62] |   871.0 ns |  2.65 ns |   2.35 ns |  0.22 |    0.00 | 0.0153 |      - |     272 B |        0.18 |
 *  | NoRegex_DictionaryAction                 | go wt(...)go 40 [62] |   973.3 ns |  4.96 ns |   4.64 ns |  0.25 |    0.00 | 0.0286 |      - |     480 B |        0.32 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)go 40 [62] | 1,639.3 ns |  3.96 ns |   3.31 ns |  0.42 |    0.00 | 0.0572 |      - |     968 B |        0.65 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)go 40 [62] | 1,019.8 ns |  5.61 ns |   5.25 ns |  0.26 |    0.00 | 0.0286 |      - |     480 B |        0.32 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)go 40 [62] |   838.1 ns |  2.75 ns |   2.44 ns |  0.21 |    0.00 | 0.0153 |      - |     272 B |        0.18 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.101
 *    [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
 *
 *  | Method                                   | command              | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |----------------------------------------- |--------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Sequential                               | go infinite          | 1,279.9 ns |  25.37 ns |  47.02 ns | 1,257.4 ns |  1.00 |    0.05 | 0.0420 |      - |     272 B |        1.00 |
 *  | Parallel                                 | go infinite          | 3,306.9 ns |  62.92 ns | 121.22 ns | 3,308.9 ns |  2.59 |    0.13 | 0.2899 |      - |    1824 B |        6.71 |
 *  | CapturingGroups                          | go infinite          | 1,129.0 ns |   9.11 ns |   7.11 ns | 1,126.4 ns |  0.88 |    0.03 | 0.2403 |      - |    1504 B |        5.53 |
 *  | NoRegex                                  | go infinite          |   783.9 ns |  15.16 ns |  19.18 ns |   776.9 ns |  0.61 |    0.03 | 0.0429 |      - |     272 B |        1.00 |
 *  | NoRegex_DictionaryAction                 | go infinite          |   807.0 ns |  14.54 ns |  23.88 ns |   795.7 ns |  0.63 |    0.03 | 0.0496 |      - |     312 B |        1.15 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go infinite          |   785.9 ns |   4.79 ns |   4.48 ns |   786.6 ns |  0.61 |    0.02 | 0.0496 |      - |     312 B |        1.15 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go infinite          |   782.9 ns |   3.06 ns |   2.71 ns |   782.7 ns |  0.61 |    0.02 | 0.0496 |      - |     312 B |        1.15 |
 *  | NoRegex_ReadOnlySpanComparison           | go infinite          |   754.6 ns |   4.44 ns |   3.71 ns |   753.7 ns |  0.59 |    0.02 | 0.0429 |      - |     272 B |        1.00 |
 *  |                                          |                      |            |           |           |            |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)c 500 [42] | 2,397.1 ns |  36.15 ns |  37.12 ns | 2,383.2 ns |  1.00 |    0.02 | 0.1984 |      - |    1248 B |        1.00 |
 *  | Parallel                                 | go wt(...)c 500 [42] | 6,004.7 ns | 318.08 ns | 912.64 ns | 5,822.3 ns |  2.51 |    0.38 | 0.4425 |      - |    2798 B |        2.24 |
 *  | CapturingGroups                          | go wt(...)c 500 [42] | 2,649.4 ns | 157.13 ns | 455.88 ns | 2,518.8 ns |  1.11 |    0.19 | 0.7629 | 0.0076 |    4800 B |        3.85 |
 *  | NoRegex                                  | go wt(...)c 500 [42] | 1,187.6 ns |  98.99 ns | 291.88 ns | 1,082.3 ns |  0.50 |    0.12 | 0.0420 |      - |     272 B |        0.22 |
 *  | NoRegex_DictionaryAction                 | go wt(...)c 500 [42] | 1,739.4 ns |  34.47 ns |  47.19 ns | 1,753.1 ns |  0.73 |    0.02 | 0.0629 |      - |     400 B |        0.32 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)c 500 [42] | 2,474.3 ns |  49.52 ns | 126.05 ns | 2,478.2 ns |  1.03 |    0.05 | 0.1221 |      - |     784 B |        0.63 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)c 500 [42] | 1,738.4 ns |  52.54 ns | 154.93 ns | 1,732.9 ns |  0.73 |    0.07 | 0.0610 |      - |     400 B |        0.32 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)c 500 [42] |   807.2 ns |  16.16 ns |  37.78 ns |   791.2 ns |  0.34 |    0.02 | 0.0420 |      - |     272 B |        0.22 |
 *  |                                          |                      |            |           |           |            |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)00000 [78] | 4,220.4 ns | 217.97 ns | 618.35 ns | 4,171.4 ns |  1.02 |    0.21 | 0.2747 |      - |    1728 B |        1.00 |
 *  | Parallel                                 | go wt(...)00000 [78] | 6,456.2 ns | 299.17 ns | 882.12 ns | 6,228.8 ns |  1.56 |    0.31 | 0.5188 |      - |    3280 B |        1.90 |
 *  | CapturingGroups                          | go wt(...)00000 [78] | 3,207.1 ns |  63.48 ns | 147.13 ns | 3,256.4 ns |  0.78 |    0.11 | 1.1292 | 0.0153 |    7064 B |        4.09 |
 *  | NoRegex                                  | go wt(...)00000 [78] |   905.8 ns |   9.26 ns |   8.21 ns |   905.1 ns |  0.22 |    0.03 | 0.0420 |      - |     272 B |        0.16 |
 *  | NoRegex_DictionaryAction                 | go wt(...)00000 [78] |   948.7 ns |   4.33 ns |   3.84 ns |   949.3 ns |  0.23 |    0.03 | 0.0801 |      - |     504 B |        0.29 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)00000 [78] | 1,750.8 ns |  34.60 ns |  63.26 ns | 1,721.5 ns |  0.42 |    0.06 | 0.1717 |      - |    1088 B |        0.63 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)00000 [78] |   968.0 ns |   7.41 ns |   6.57 ns |   966.5 ns |  0.23 |    0.03 | 0.0801 |      - |     504 B |        0.29 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)00000 [78] |   963.3 ns |   8.31 ns |   7.77 ns |   962.4 ns |  0.23 |    0.03 | 0.0420 |      - |     272 B |        0.16 |
 *  |                                          |                      |            |           |           |            |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)go 40 [62] | 3,146.3 ns |  60.14 ns |  80.29 ns | 3,154.8 ns |  1.00 |    0.04 | 0.2365 |      - |    1488 B |        1.00 |
 *  | Parallel                                 | go wt(...)go 40 [62] | 4,920.4 ns |  98.05 ns | 100.69 ns | 4,897.8 ns |  1.56 |    0.05 | 0.4730 |      - |    3040 B |        2.04 |
 *  | CapturingGroups                          | go wt(...)go 40 [62] | 3,004.4 ns |  58.40 ns |  51.77 ns | 3,013.6 ns |  0.96 |    0.03 | 1.1215 | 0.0267 |    7032 B |        4.73 |
 *  | NoRegex                                  | go wt(...)go 40 [62] |   859.0 ns |   7.83 ns |   7.33 ns |   857.7 ns |  0.27 |    0.01 | 0.0420 |      - |     272 B |        0.18 |
 *  | NoRegex_DictionaryAction                 | go wt(...)go 40 [62] |   971.5 ns |  13.72 ns |  12.16 ns |   969.8 ns |  0.31 |    0.01 | 0.0763 |      - |     480 B |        0.32 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)go 40 [62] | 1,509.1 ns |  13.32 ns |  11.81 ns | 1,504.8 ns |  0.48 |    0.01 | 0.1545 |      - |     968 B |        0.65 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)go 40 [62] |   967.4 ns |  16.52 ns |  14.64 ns |   960.2 ns |  0.31 |    0.01 | 0.0763 |      - |     480 B |        0.32 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)go 40 [62] |   937.5 ns |  18.69 ns |  52.11 ns |   926.5 ns |  0.30 |    0.02 | 0.0420 |      - |     272 B |        0.18 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.1 (22H221) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.101
 *    [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
 *  s
 *  | Method                                   | command              | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |----------------------------------------- |--------------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | Sequential                               | go infinite          |  1.786 us | 0.0332 us | 0.0554 us |  1.765 us |  1.00 |    0.04 | 0.0420 |      - |     272 B |        1.00 |
 *  | Parallel                                 | go infinite          | 22.265 us | 0.4398 us | 0.6165 us | 22.340 us | 12.48 |    0.50 | 0.2441 |      - |    1824 B |        6.71 |
 *  | CapturingGroups                          | go infinite          |  1.685 us | 0.0334 us | 0.0539 us |  1.677 us |  0.94 |    0.04 | 0.2403 |      - |    1504 B |        5.53 |
 *  | NoRegex                                  | go infinite          |  1.107 us | 0.0220 us | 0.0548 us |  1.107 us |  0.62 |    0.04 | 0.0420 |      - |     272 B |        1.00 |
 *  | NoRegex_DictionaryAction                 | go infinite          |  1.161 us | 0.0231 us | 0.0511 us |  1.162 us |  0.65 |    0.03 | 0.0496 |      - |     312 B |        1.15 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go infinite          |  1.229 us | 0.0243 us | 0.0473 us |  1.217 us |  0.69 |    0.03 | 0.0496 |      - |     312 B |        1.15 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go infinite          |  1.175 us | 0.0233 us | 0.0491 us |  1.171 us |  0.66 |    0.03 | 0.0496 |      - |     312 B |        1.15 |
 *  | NoRegex_ReadOnlySpanComparison           | go infinite          |  1.359 us | 0.0463 us | 0.1312 us |  1.332 us |  0.76 |    0.08 | 0.0420 |      - |     272 B |        1.00 |
 *  |                                          |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)c 500 [42] |  3.776 us | 0.1164 us | 0.3263 us |  3.681 us |  1.01 |    0.12 | 0.1984 |      - |    1248 B |        1.00 |
 *  | Parallel                                 | go wt(...)c 500 [42] | 30.027 us | 0.5963 us | 1.3089 us | 29.792 us |  8.01 |    0.73 | 0.4272 |      - |    2800 B |        2.24 |
 *  | CapturingGroups                          | go wt(...)c 500 [42] |  3.485 us | 0.0833 us | 0.2295 us |  3.382 us |  0.93 |    0.10 | 0.7629 | 0.0076 |    4800 B |        3.85 |
 *  | NoRegex                                  | go wt(...)c 500 [42] |  1.149 us | 0.0311 us | 0.0887 us |  1.126 us |  0.31 |    0.03 | 0.0381 |      - |     272 B |        0.22 |
 *  | NoRegex_DictionaryAction                 | go wt(...)c 500 [42] |  1.890 us | 0.1562 us | 0.4532 us |  1.743 us |  0.50 |    0.13 | 0.0629 |      - |     400 B |        0.32 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)c 500 [42] |  1.961 us | 0.0388 us | 0.0556 us |  1.948 us |  0.52 |    0.04 | 0.1221 |      - |     784 B |        0.63 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)c 500 [42] |  1.653 us | 0.0811 us | 0.2366 us |  1.618 us |  0.44 |    0.07 | 0.0629 |      - |     400 B |        0.32 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)c 500 [42] |  1.224 us | 0.0541 us | 0.1527 us |  1.175 us |  0.33 |    0.05 | 0.0381 |      - |     272 B |        0.22 |
 *  |                                          |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)00000 [78] |  4.444 us | 0.0675 us | 0.0598 us |  4.432 us |  1.00 |    0.02 | 0.2747 |      - |    1728 B |        1.00 |
 *  | Parallel                                 | go wt(...)00000 [78] | 33.309 us | 0.6608 us | 1.4082 us | 33.202 us |  7.50 |    0.33 | 0.4883 |      - |    3280 B |        1.90 |
 *  | CapturingGroups                          | go wt(...)00000 [78] |  5.002 us | 0.0992 us | 0.1957 us |  4.948 us |  1.13 |    0.05 | 1.1215 | 0.0229 |    7064 B |        4.09 |
 *  | NoRegex                                  | go wt(...)00000 [78] |  1.632 us | 0.0858 us | 0.2477 us |  1.629 us |  0.37 |    0.06 | 0.0420 |      - |     272 B |        0.16 |
 *  | NoRegex_DictionaryAction                 | go wt(...)00000 [78] |  1.813 us | 0.0894 us | 0.2593 us |  1.743 us |  0.41 |    0.06 | 0.0801 |      - |     504 B |        0.29 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)00000 [78] |  2.954 us | 0.1064 us | 0.3000 us |  3.022 us |  0.66 |    0.07 | 0.1678 |      - |    1088 B |        0.63 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)00000 [78] |  2.169 us | 0.1363 us | 0.3888 us |  2.111 us |  0.49 |    0.09 | 0.0763 |      - |     504 B |        0.29 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)00000 [78] |  1.792 us | 0.1027 us | 0.2997 us |  1.785 us |  0.40 |    0.07 | 0.0420 |      - |     272 B |        0.16 |
 *  |                                          |                      |           |           |           |           |       |         |        |        |           |             |
 *  | Sequential                               | go wt(...)go 40 [62] |  4.705 us | 0.1885 us | 0.5285 us |  4.736 us |  1.01 |    0.16 | 0.2365 |      - |    1488 B |        1.00 |
 *  | Parallel                                 | go wt(...)go 40 [62] | 40.416 us | 1.3169 us | 3.8828 us | 40.841 us |  8.70 |    1.27 | 0.4883 |      - |    3040 B |        2.04 |
 *  | CapturingGroups                          | go wt(...)go 40 [62] |  5.624 us | 0.1568 us | 0.4345 us |  5.527 us |  1.21 |    0.16 | 1.1215 | 0.0229 |    7032 B |        4.73 |
 *  | NoRegex                                  | go wt(...)go 40 [62] |  1.595 us | 0.0611 us | 0.1732 us |  1.576 us |  0.34 |    0.05 | 0.0420 |      - |     272 B |        0.18 |
 *  | NoRegex_DictionaryAction                 | go wt(...)go 40 [62] |  1.455 us | 0.0167 us | 0.0148 us |  1.450 us |  0.31 |    0.03 | 0.0763 |      - |     480 B |        0.32 |
 *  | NoRegex_DictionaryActionAndMemoryValues  | go wt(...)go 40 [62] |  2.291 us | 0.0217 us | 0.0203 us |  2.285 us |  0.49 |    0.05 | 0.1526 |      - |     968 B |        0.65 |
 *  | NoRegex_DictionaryActionAndMemoryValues2 | go wt(...)go 40 [62] |  1.462 us | 0.0292 us | 0.0273 us |  1.451 us |  0.31 |    0.03 | 0.0763 |      - |     480 B |        0.32 |
 *  | NoRegex_ReadOnlySpanComparison           | go wt(...)go 40 [62] |  1.264 us | 0.0304 us | 0.0887 us |  1.262 us |  0.27 |    0.04 | 0.0420 |      - |     272 B |        0.18 |
 *
 */

using BenchmarkDotNet.Attributes;
using NLog;
using System.Text.RegularExpressions;

namespace Lynx.Benchmark;

public partial class GoCommandParsingAlternatives_Benchmark : BaseBenchmark
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static IEnumerable<string> Data => [
            "go infinite",
            "go wtime 7000 winc 500 btime 8000 binc 500",
            "go wtime 7000 winc 500 btime 8000 binc 500 ponder movestogo 40",
            "go wtime 7000 winc 500 btime 8000 binc 500 movestogo 40 depth 14 nodes 1000000",
        ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public async Task Sequential(string command)
    {
        await Task.Run(() => ParseSequentially(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task Parallel(string command)
    {
        await ParseInParallel(command);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task CapturingGroups(string command)
    {
        await Task.Run(() => ParseRegexCapturingGroups(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex(string command)
    {
        await Task.Run(() => ParseNoRegex(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex_DictionaryAction(string command)
    {
        await Task.Run(() => ParseNoRegex_DictionaryAction(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex_DictionaryActionAndMemoryValues(string command)
    {
        await Task.Run(() => ParseNoRegex_DictionaryActionAndMemoryValues(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex_DictionaryActionAndMemoryValues2(string command)
    {
        await Task.Run(() => ParseNoRegex_DictionaryActionAndMemoryValues_2(command));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public async Task NoRegex_ReadOnlySpanComparison(string command)
    {
        await Task.Run(() => ParseNoRegex_ReadOnlySpanComparison(command));
    }

    [GeneratedRegex("(?<=wtime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex WhiteTimeRegex();

    [GeneratedRegex("(?<=btime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex BlackTimeRegex();

    [GeneratedRegex("(?<=winc).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex WhiteIncrementRegex();

    [GeneratedRegex("(?<=binc).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex BlackIncrementRegex();

    [GeneratedRegex("(?<=movestogo).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex MovesToGoRegex();

    [GeneratedRegex("(?<=movetime).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex MoveTimeRegex();

    [GeneratedRegex("(?<=depth).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex DepthRegex();

    [GeneratedRegex("(?<=nodes).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex NodesRegex();

    [GeneratedRegex("(?<=mate).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex MateRegex();

    [GeneratedRegex("(?<=searchmoves).+?(?=searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex SearchMovesRegex();

    [GeneratedRegex(@"(?<wtime>(?<=wtime\s+)\d+)|(?<btime>(?<=btime\s+)\d+)|(?<winc>(?<=winc\s+)\d+)|(?<binc>(?<=binc\s+)\d+)|(?<movestogo>(?<=movestogo\s+)\d+)|(?<depth>(?<=depth\s+)\d+)|(?<movetime>(?<=movetime\s+)\d+)|(?<infinite>infinite)|(?<ponder>ponder)")]
    private static partial Regex CapturingGroups();

    public List<string> SearchMoves { get; private set; } = default!;
    public int WhiteTime { get; private set; } = default!;
    public int BlackTime { get; private set; } = default!;
    public int WhiteIncrement { get; private set; } = default!;
    public int BlackIncrement { get; private set; } = default!;
    public int MovesToGo { get; private set; } = default!;
    public int Depth { get; private set; } = default!;
    public int Nodes { get; private set; } = default!;
    public int Mate { get; private set; } = default!;
    public int MoveTime { get; private set; } = default!;
    public bool Infinite { get; private set; } = default!;
    public bool Ponder { get; private set; } = default!;

    private void ParseSequentially(string command)
    {
        var match = WhiteTimeRegex().Match(command);
        if (int.TryParse(match.Value, out var value))
        {
            WhiteTime = value;
        }

        match = BlackTimeRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            BlackTime = value;
        }

        match = WhiteIncrementRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            WhiteIncrement = value;
        }

        match = BlackIncrementRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            BlackIncrement = value;
        }

        match = MovesToGoRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            MovesToGo = value;
        }

        match = MoveTimeRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            MoveTime = value;
        }

        match = DepthRegex().Match(command);
        if (int.TryParse(match.Value, out value))
        {
            Depth = value;
        }

        Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase);

        Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase);

        //match = NodesRegex().Match(command);
        //if (int.TryParse(match.Value, out value))
        //{
        //    Nodes = value;
        //}

        //match = MateRegex().Match(command);
        //if (int.TryParse(match.Value, out value))
        //{
        //    Mate = value;
        //}

        //var match = SearchMovesRegex().Match(command);
        //SearchMoves = [.. match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)];ç
    }

    private async Task ParseInParallel(string command)
    {
        var taskList = new List<Task>
            {
                Task.Run(() =>
                {
                    var match = WhiteTimeRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = BlackTimeRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = WhiteIncrementRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = BlackIncrementRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = MovesToGoRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MovesToGo = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = MoveTimeRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MoveTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = DepthRegex().Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Depth = value;
                    }
                }),
                Task.Run(() => Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase)),
                Task.Run(() => Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase)),
                //Task.Run(() =>
                //{
                //    var match = NodesRegex().Match(command);

                //    if(int.TryParse(match.Value, out var value))
                //    {
                //        Nodes = value;
                //    }
                //}),
                //Task.Run(() =>
                //{
                //    var match = MateRegex().Match(command);

                //    if(int.TryParse(match.Value, out var value))
                //    {
                //        Mate = value;
                //    }
                //}),
                //Task.Run(() =>
                //{
                //    var match = SearchMovesRegex().Match(command);

                //    SearchMoves = Enumerable.ToList<string>(match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                //}),
            };

        await Task.WhenAll(taskList);
    }

    private void ParseRegexCapturingGroups(string command)
    {
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (var match in CapturingGroups().Matches(command).Cast<Match>())
        {
            for (int i = 1; i < match.Groups.Count; ++i)
            {
                var group = match.Groups[i];
                if (group.Success)
                {
                    switch (group.Name)
                    {
                        case "wtime":
                            WhiteTime = int.Parse(group.Value);
                            break;
                        case "btime":
                            BlackTime = int.Parse(group.Value);
                            break;
                        case "winc":
                            WhiteIncrement = int.Parse(group.Value);
                            break;
                        case "binc":
                            BlackIncrement = int.Parse(group.Value);
                            break;
                        case "movestogo":
                            MovesToGo = int.Parse(group.Value);
                            break;
                        case "movetime":
                            MoveTime = int.Parse(group.Value);
                            break;
                        case "depth":
                            Depth = int.Parse(group.Value);
                            break;
                        case "infinite":
                            Infinite = true;
                            break;
                        case "ponder":
                            Ponder = true;
                            break;
                            //case "nodes":
                            //    Nodes = int.Parse(group.Value);
                            //    break;
                            //case "mate":
                            //    Nodes = int.Parse(group.Value);
                            //    break;
                            //case "searchmoves":
                            //    Nodes = int.Parse(group.Value);
                            //    break;
                    }

                    break;
                }
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
    }

    private void ParseNoRegex(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            switch (commandAsSpan[ranges[i]])
            {
                case "wtime":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            WhiteTime = value;
                        }

                        break;
                    }
                case "btime":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            BlackTime = value;
                        }

                        break;
                    }
                case "winc":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            WhiteIncrement = value;
                        }

                        break;
                    }
                case "binc":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            BlackIncrement = value;
                        }

                        break;
                    }
                case "movestogo":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            MovesToGo = value;
                        }

                        break;
                    }
                case "movetime":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            MoveTime = value;
                        }

                        break;
                    }
                case "depth":
                    {
                        if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                        {
                            Depth = value;
                        }

                        break;
                    }
                case "infinite":
                    {
                        Infinite = true;
                        break;
                    }
                case "ponder":
                    {
                        Ponder = true;
                        break;
                    }
                case "nodes":
                    {
                        _logger.Warn("nodes not supported in go command, it will be safely ignored");
                        ++i;
                        break;
                    }
                case "mate":
                    {
                        _logger.Warn("mate not supported in go command, it will be safely ignored");
                        ++i;
                        break;
                    }
                case "searchmoves":
                    {
                        const string message = "searchmoves not supported in go command";
                        _logger.Error(message);
                        throw new InvalidDataException(message);
                    }
                default:
                    {
                        _logger.Warn("{0} not supported in go command", commandAsSpan[ranges[i]].ToString());
                        break;
                    }
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    private static readonly Dictionary<string, Action<GoCommandParsingAlternatives_Benchmark, int>> _commandActions = new Dictionary<string, Action<GoCommandParsingAlternatives_Benchmark, int>>
    {
        ["wtime"] = (command, value) => command.WhiteTime = value,
        ["btime"] = (command, value) => command.BlackTime = value,
        ["winc"] = (command, value) => command.WhiteIncrement = value,
        ["binc"] = (command, value) => command.BlackIncrement = value,
        ["movestogo"] = (command, value) => command.MovesToGo = value,
        ["movetime"] = (command, value) => command.MoveTime = value,
        ["depth"] = (command, value) => command.Depth = value
    };

    private void ParseNoRegex_DictionaryAction(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]].ToString();
            if (_commandActions.TryGetValue(key, out var action))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    action(this, value);
                }
            }
            else
            {
                switch (key)
                {
                    case "infinite":
                        Infinite = true;
                        break;
                    case "ponder":
                        Ponder = true;
                        break;
                    case "nodes":
                        _logger.Warn("nodes not supported in go command, it will be safely ignored");
                        ++i;
                        break;
                    case "mate":
                        _logger.Warn("mate not supported in go command, it will be safely ignored");
                        ++i;
                        break;
                    case "searchmoves":
                        const string message = "searchmoves not supported in go command";
                        _logger.Error(message);
                        throw new NotImplementedException(message);
                    default:
                        _logger.Warn("{0} not supported in go command, attempting to continue command parsing", key);
                        break;
                }
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    private static readonly ReadOnlyMemory<char> InfiniteMemory = "infinite".AsMemory();
    private static readonly ReadOnlyMemory<char> PonderMemory = "ponder".AsMemory();
    private static readonly ReadOnlyMemory<char> NodesMemory = "nodes".AsMemory();
    private static readonly ReadOnlyMemory<char> MateMemory = "mate".AsMemory();
    private static readonly ReadOnlyMemory<char> SearchMovesMemory = "searchmoves".AsMemory();

    private static readonly Dictionary<ReadOnlyMemory<char>, Action<GoCommandParsingAlternatives_Benchmark, int>> _commandActions2 = new()
    {
        ["wtime".AsMemory()] = (command, value) => command.WhiteTime = value,
        ["btime".AsMemory()] = (command, value) => command.BlackTime = value,
        ["winc".AsMemory()] = (command, value) => command.WhiteIncrement = value,
        ["binc".AsMemory()] = (command, value) => command.BlackIncrement = value,
        ["movestogo".AsMemory()] = (command, value) => command.MovesToGo = value,
        ["movetime".AsMemory()] = (command, value) => command.MoveTime = value,
        ["depth".AsMemory()] = (command, value) => command.Depth = value
    };

    private void ParseNoRegex_DictionaryActionAndMemoryValues(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];
            if (_commandActions2.TryGetValue(key.ToString().AsMemory(), out var action))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    action(this, value);
                }
            }
            else if (key.Equals(InfiniteMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;
            }
            else if (key.Equals(NodesMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchMovesMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new NotImplementedException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command, attempting to continue command parsing", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    private static readonly Dictionary<string, Action<GoCommandParsingAlternatives_Benchmark, int>> _commandActions3 = new()
    {
        ["wtime"] = (command, value) => command.WhiteTime = value,
        ["btime"] = (command, value) => command.BlackTime = value,
        ["winc"] = (command, value) => command.WhiteIncrement = value,
        ["binc"] = (command, value) => command.BlackIncrement = value,
        ["movestogo"] = (command, value) => command.MovesToGo = value,
        ["movetime"] = (command, value) => command.MoveTime = value,
        ["depth"] = (command, value) => command.Depth = value
    };

    private void ParseNoRegex_DictionaryActionAndMemoryValues_2(string command)
    {
        var commandAsSpan = command.AsSpan();
        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];
            if (_commandActions3.TryGetValue(key.ToString(), out var action))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    action(this, value);
                }
            }
            else if (key.Equals(InfiniteMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;
            }
            else if (key.Equals(NodesMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchMovesMemory.Span, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new NotImplementedException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command, attempting to continue command parsing", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

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

    private void ParseNoRegex_ReadOnlySpanComparison(string command)
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
}
