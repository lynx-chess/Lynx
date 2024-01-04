/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                       | fen                  | Mean       | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |----------------------------- |--------------------- |-----------:|---------:|---------:|------:|-------:|----------:|------------:|
 *  | ParseFEN_Original            | 8/k7/(...)- 0 1 [39] | 2,441.4 ns |  7.51 ns |  6.27 ns |  1.00 | 0.0343 |    2960 B |        1.00 |
 *  | ParseFEN_Improved1           | 8/k7/(...)- 0 1 [39] | 2,509.8 ns | 12.38 ns | 11.58 ns |  1.03 | 0.0305 |    2704 B |        0.91 |
 *  | ParseFEN_Base2               | 8/k7/(...)- 0 1 [39] | 2,332.2 ns | 13.59 ns | 12.72 ns |  0.95 | 0.0305 |    2760 B |        0.93 |
 *  | ParseFEN_NoRegex             | 8/k7/(...)- 0 1 [39] | 1,074.7 ns |  4.83 ns |  4.03 ns |  0.44 | 0.0057 |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard | 8/k7/(...)- 0 1 [39] |   332.4 ns |  2.58 ns |  2.41 ns |  0.14 | 0.0019 |     168 B |        0.06 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | r2q1r(...)- 0 9 [68] | 3,254.5 ns |  7.77 ns |  7.27 ns |  1.00 | 0.0343 |    3160 B |        1.00 |
 *  | ParseFEN_Improved1           | r2q1r(...)- 0 9 [68] | 2,981.4 ns | 16.73 ns | 15.65 ns |  0.92 | 0.0343 |    2904 B |        0.92 |
 *  | ParseFEN_Base2               | r2q1r(...)- 0 9 [68] | 2,958.6 ns |  8.61 ns |  8.05 ns |  0.91 | 0.0343 |    3016 B |        0.95 |
 *  | ParseFEN_NoRegex             | r2q1r(...)- 0 9 [68] | 1,524.1 ns |  6.46 ns |  6.04 ns |  0.47 | 0.0057 |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard | r2q1r(...)- 0 9 [68] |   380.5 ns |  2.13 ns |  1.88 ns |  0.12 | 0.0019 |     168 B |        0.05 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | r3k2r(...)- 0 1 [68] | 2,959.7 ns | 12.97 ns | 12.13 ns |  1.00 | 0.0343 |    3080 B |        1.00 |
 *  | ParseFEN_Improved1           | r3k2r(...)- 0 1 [68] | 2,884.5 ns |  8.71 ns |  8.14 ns |  0.97 | 0.0305 |    2816 B |        0.91 |
 *  | ParseFEN_Base2               | r3k2r(...)- 0 1 [68] | 2,846.0 ns |  5.74 ns |  5.09 ns |  0.96 | 0.0343 |    2928 B |        0.95 |
 *  | ParseFEN_NoRegex             | r3k2r(...)- 0 1 [68] | 1,405.5 ns |  4.79 ns |  4.48 ns |  0.47 | 0.0057 |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard | r3k2r(...)- 0 1 [68] |   380.0 ns |  2.50 ns |  2.34 ns |  0.13 | 0.0019 |     168 B |        0.05 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | r3k2r(...)- 0 1 [68] | 2,930.6 ns |  8.95 ns |  7.93 ns |  1.00 | 0.0343 |    3080 B |        1.00 |
 *  | ParseFEN_Improved1           | r3k2r(...)- 0 1 [68] | 2,850.1 ns |  8.67 ns |  8.11 ns |  0.97 | 0.0305 |    2816 B |        0.91 |
 *  | ParseFEN_Base2               | r3k2r(...)- 0 1 [68] | 2,787.1 ns |  2.02 ns |  1.58 ns |  0.95 | 0.0343 |    2928 B |        0.95 |
 *  | ParseFEN_NoRegex             | r3k2r(...)- 0 1 [68] | 1,411.7 ns |  4.99 ns |  4.42 ns |  0.48 | 0.0057 |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard | r3k2r(...)- 0 1 [68] |   383.2 ns |  3.05 ns |  2.85 ns |  0.13 | 0.0019 |     168 B |        0.05 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | rnbqk(...)6 0 1 [67] | 2,839.4 ns |  7.78 ns |  6.49 ns |  1.00 | 0.0343 |    3072 B |        1.00 |
 *  | ParseFEN_Improved1           | rnbqk(...)6 0 1 [67] | 2,673.7 ns | 13.69 ns | 12.13 ns |  0.94 | 0.0305 |    2800 B |        0.91 |
 *  | ParseFEN_Base2               | rnbqk(...)6 0 1 [67] | 2,707.6 ns |  9.94 ns |  9.29 ns |  0.95 | 0.0343 |    2904 B |        0.95 |
 *  | ParseFEN_NoRegex             | rnbqk(...)6 0 1 [67] | 1,341.3 ns |  4.73 ns |  4.42 ns |  0.47 | 0.0057 |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard | rnbqk(...)6 0 1 [67] |   385.4 ns |  0.55 ns |  0.43 ns |  0.14 | 0.0019 |     168 B |        0.05 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | rnbqk(...)- 0 1 [56] | 2,022.2 ns |  3.98 ns |  3.53 ns |  1.00 | 0.0305 |    2760 B |        1.00 |
 *  | ParseFEN_Improved1           | rnbqk(...)- 0 1 [56] | 1,989.9 ns |  4.35 ns |  3.40 ns |  0.98 | 0.0267 |    2496 B |        0.90 |
 *  | ParseFEN_Base2               | rnbqk(...)- 0 1 [56] | 1,941.0 ns |  7.01 ns |  6.22 ns |  0.96 | 0.0305 |    2584 B |        0.94 |
 *  | ParseFEN_NoRegex             | rnbqk(...)- 0 1 [56] |   754.1 ns |  3.40 ns |  3.18 ns |  0.37 | 0.0029 |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard | rnbqk(...)- 0 1 [56] |   385.8 ns |  0.84 ns |  0.74 ns |  0.19 | 0.0019 |     168 B |        0.06 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | rq2k2(...)- 0 1 [71] | 3,171.1 ns |  6.61 ns |  5.86 ns |  1.00 | 0.0343 |    3168 B |        1.00 |
 *  | ParseFEN_Improved1           | rq2k2(...)- 0 1 [71] | 3,102.0 ns |  6.98 ns |  6.18 ns |  0.98 | 0.0343 |    2904 B |        0.92 |
 *  | ParseFEN_Base2               | rq2k2(...)- 0 1 [71] | 3,082.6 ns | 10.31 ns |  9.14 ns |  0.97 | 0.0343 |    3024 B |        0.95 |
 *  | ParseFEN_NoRegex             | rq2k2(...)- 0 1 [71] | 1,583.2 ns |  5.20 ns |  4.86 ns |  0.50 | 0.0057 |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard | rq2k2(...)- 0 1 [71] |   408.8 ns |  0.56 ns |  0.47 ns |  0.13 | 0.0019 |     168 B |        0.05 |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                       | fen                  | Mean       | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |----------------------------- |--------------------- |-----------:|---------:|---------:|------:|-------:|----------:|------------:|
 *  | ParseFEN_Original            | 8/k7/(...)- 0 1 [39] | 2,118.2 ns | 28.81 ns | 25.54 ns |  1.00 | 0.1755 |    2961 B |        1.00 |
 *  | ParseFEN_Improved1           | 8/k7/(...)- 0 1 [39] | 2,093.2 ns |  5.88 ns |  5.21 ns |  0.99 | 0.1602 |    2704 B |        0.91 |
 *  | ParseFEN_Base2               | 8/k7/(...)- 0 1 [39] | 2,024.0 ns | 10.40 ns |  9.73 ns |  0.96 | 0.1640 |    2760 B |        0.93 |
 *  | ParseFEN_NoRegex             | 8/k7/(...)- 0 1 [39] |   957.6 ns |  3.09 ns |  2.74 ns |  0.45 | 0.0286 |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard | 8/k7/(...)- 0 1 [39] |   307.3 ns |  1.03 ns |  0.97 ns |  0.15 | 0.0100 |     168 B |        0.06 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | r2q1r(...)- 0 9 [68] | 2,676.8 ns | 10.22 ns |  9.06 ns |  1.00 | 0.1869 |    3161 B |        1.00 |
 *  | ParseFEN_Improved1           | r2q1r(...)- 0 9 [68] | 2,655.8 ns |  8.83 ns |  7.83 ns |  0.99 | 0.1717 |    2905 B |        0.92 |
 *  | ParseFEN_Base2               | r2q1r(...)- 0 9 [68] | 2,570.6 ns | 11.12 ns | 10.40 ns |  0.96 | 0.1793 |    3017 B |        0.95 |
 *  | ParseFEN_NoRegex             | r2q1r(...)- 0 9 [68] | 1,421.3 ns |  7.09 ns |  6.63 ns |  0.53 | 0.0362 |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard | r2q1r(...)- 0 9 [68] |   378.9 ns |  0.76 ns |  0.67 ns |  0.14 | 0.0100 |     168 B |        0.05 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | r3k2r(...)- 0 1 [68] | 2,346.9 ns | 15.15 ns | 13.43 ns |  1.00 | 0.1831 |    3081 B |        1.00 |
 *  | ParseFEN_Improved1           | r3k2r(...)- 0 1 [68] | 2,458.3 ns |  7.03 ns |  6.23 ns |  1.05 | 0.1678 |    2817 B |        0.91 |
 *  | ParseFEN_Base2               | r3k2r(...)- 0 1 [68] | 2,514.6 ns | 13.30 ns | 11.11 ns |  1.07 | 0.1717 |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex             | r3k2r(...)- 0 1 [68] | 1,286.1 ns |  3.21 ns |  2.68 ns |  0.55 | 0.0324 |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard | r3k2r(...)- 0 1 [68] |   391.6 ns |  3.84 ns |  3.59 ns |  0.17 | 0.0100 |     168 B |        0.05 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | r3k2r(...)- 0 1 [68] | 2,594.7 ns |  8.83 ns |  7.83 ns |  1.00 | 0.1831 |    3081 B |        1.00 |
 *  | ParseFEN_Improved1           | r3k2r(...)- 0 1 [68] | 2,502.9 ns |  9.25 ns |  7.72 ns |  0.96 | 0.1678 |    2817 B |        0.91 |
 *  | ParseFEN_Base2               | r3k2r(...)- 0 1 [68] | 2,444.2 ns |  9.58 ns |  8.96 ns |  0.94 | 0.1717 |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex             | r3k2r(...)- 0 1 [68] | 1,283.2 ns |  4.44 ns |  4.15 ns |  0.49 | 0.0324 |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard | r3k2r(...)- 0 1 [68] |   385.2 ns |  0.83 ns |  0.73 ns |  0.15 | 0.0100 |     168 B |        0.05 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | rnbqk(...)6 0 1 [67] | 2,475.0 ns | 13.47 ns | 12.60 ns |  1.00 | 0.1831 |    3073 B |        1.00 |
 *  | ParseFEN_Improved1           | rnbqk(...)6 0 1 [67] | 2,393.5 ns | 10.55 ns |  9.35 ns |  0.97 | 0.1640 |    2800 B |        0.91 |
 *  | ParseFEN_Base2               | rnbqk(...)6 0 1 [67] | 2,405.1 ns |  7.30 ns |  6.10 ns |  0.97 | 0.1717 |    2905 B |        0.95 |
 *  | ParseFEN_NoRegex             | rnbqk(...)6 0 1 [67] | 1,158.9 ns |  2.26 ns |  2.11 ns |  0.47 | 0.0305 |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard | rnbqk(...)6 0 1 [67] |   357.7 ns |  1.13 ns |  1.00 ns |  0.14 | 0.0100 |     168 B |        0.05 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | rnbqk(...)- 0 1 [56] | 1,729.6 ns |  6.36 ns |  5.64 ns |  1.00 | 0.1640 |    2760 B |        1.00 |
 *  | ParseFEN_Improved1           | rnbqk(...)- 0 1 [56] | 1,667.3 ns | 10.49 ns |  9.30 ns |  0.96 | 0.1488 |    2496 B |        0.90 |
 *  | ParseFEN_Base2               | rnbqk(...)- 0 1 [56] | 1,678.4 ns |  5.03 ns |  4.46 ns |  0.97 | 0.1545 |    2584 B |        0.94 |
 *  | ParseFEN_NoRegex             | rnbqk(...)- 0 1 [56] |   755.3 ns |  2.44 ns |  2.16 ns |  0.44 | 0.0153 |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard | rnbqk(...)- 0 1 [56] |   357.6 ns |  2.64 ns |  2.47 ns |  0.21 | 0.0100 |     168 B |        0.06 |
 *  |                              |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original            | rq2k2(...)- 0 1 [71] | 2,756.1 ns | 12.76 ns | 11.93 ns |  1.00 | 0.1869 |    3169 B |        1.00 |
 *  | ParseFEN_Improved1           | rq2k2(...)- 0 1 [71] | 2,754.7 ns | 12.91 ns | 12.07 ns |  1.00 | 0.1717 |    2905 B |        0.92 |
 *  | ParseFEN_Base2               | rq2k2(...)- 0 1 [71] | 2,662.5 ns |  8.62 ns |  8.06 ns |  0.97 | 0.1793 |    3025 B |        0.95 |
 *  | ParseFEN_NoRegex             | rq2k2(...)- 0 1 [71] | 1,427.8 ns |  4.57 ns |  4.27 ns |  0.52 | 0.0362 |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard | rq2k2(...)- 0 1 [71] |   378.3 ns |  1.33 ns |  1.18 ns |  0.14 | 0.0100 |     168 B |        0.05 |
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                       | fen                  | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |----------------------------- |--------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original            | 8/k7/(...)- 0 1 [39] | 3,794.2 ns | 100.96 ns | 288.04 ns | 3,783.3 ns |  1.00 |    0.00 | 0.4692 |      - |    2961 B |        1.00 |
 *  | ParseFEN_Improved1           | 8/k7/(...)- 0 1 [39] | 3,443.8 ns |  90.46 ns | 263.88 ns | 3,439.0 ns |  0.91 |    0.09 | 0.4311 |      - |    2705 B |        0.91 |
 *  | ParseFEN_Base2               | 8/k7/(...)- 0 1 [39] | 3,703.3 ns | 194.13 ns | 553.86 ns | 3,607.0 ns |  0.98 |    0.15 | 0.4387 |      - |    2761 B |        0.93 |
 *  | ParseFEN_NoRegex             | 8/k7/(...)- 0 1 [39] | 1,773.6 ns |  34.80 ns |  83.37 ns | 1,759.6 ns |  0.47 |    0.04 | 0.0763 |      - |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard | 8/k7/(...)- 0 1 [39] |   554.2 ns |  12.98 ns |  37.03 ns |   557.1 ns |  0.15 |    0.02 | 0.0267 |      - |     168 B |        0.06 |
 *  |                              |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original            | r2q1r(...)- 0 9 [68] | 3,915.4 ns |  76.41 ns |  78.47 ns | 3,912.0 ns |  1.00 |    0.00 | 0.5035 |      - |    3162 B |        1.00 |
 *  | ParseFEN_Improved1           | r2q1r(...)- 0 9 [68] | 3,695.3 ns |  68.21 ns |  63.80 ns | 3,704.8 ns |  0.94 |    0.02 | 0.4616 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2               | r2q1r(...)- 0 9 [68] | 3,646.7 ns |  62.66 ns |  52.32 ns | 3,653.7 ns |  0.93 |    0.02 | 0.4807 |      - |    3017 B |        0.95 |
 *  | ParseFEN_NoRegex             | r2q1r(...)- 0 9 [68] | 1,993.2 ns |  37.52 ns |  38.53 ns | 1,976.9 ns |  0.51 |    0.02 | 0.0992 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard | r2q1r(...)- 0 9 [68] |   561.4 ns |   6.67 ns |   5.57 ns |   559.7 ns |  0.14 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  |                              |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original            | r3k2r(...)- 0 1 [68] | 3,604.1 ns |  62.98 ns | 148.44 ns | 3,549.7 ns |  1.00 |    0.00 | 0.4883 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1           | r3k2r(...)- 0 1 [68] | 3,339.2 ns |  40.44 ns |  31.58 ns | 3,337.8 ns |  0.91 |    0.05 | 0.4463 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2               | r3k2r(...)- 0 1 [68] | 3,446.9 ns |  67.95 ns |  66.74 ns | 3,421.1 ns |  0.94 |    0.04 | 0.4654 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex             | r3k2r(...)- 0 1 [68] | 1,733.0 ns |  15.48 ns |  13.72 ns | 1,732.0 ns |  0.47 |    0.03 | 0.0877 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard | r3k2r(...)- 0 1 [68] |   591.4 ns |  10.09 ns |  11.62 ns |   588.7 ns |  0.16 |    0.01 | 0.0267 |      - |     168 B |        0.05 |
 *  |                              |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original            | r3k2r(...)- 0 1 [68] | 3,515.3 ns |  60.58 ns |  50.59 ns | 3,528.7 ns |  1.00 |    0.00 | 0.4883 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1           | r3k2r(...)- 0 1 [68] | 3,295.8 ns |  27.15 ns |  22.67 ns | 3,288.3 ns |  0.94 |    0.02 | 0.4463 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2               | r3k2r(...)- 0 1 [68] | 3,397.7 ns |  38.98 ns |  32.55 ns | 3,391.7 ns |  0.97 |    0.01 | 0.4654 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex             | r3k2r(...)- 0 1 [68] | 1,773.9 ns |  16.04 ns |  15.01 ns | 1,766.1 ns |  0.50 |    0.01 | 0.0877 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard | r3k2r(...)- 0 1 [68] |   585.8 ns |   3.94 ns |   3.07 ns |   585.4 ns |  0.17 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  |                              |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original            | rnbqk(...)6 0 1 [67] | 3,537.1 ns |  68.15 ns |  72.92 ns | 3,528.8 ns |  1.00 |    0.00 | 0.4883 |      - |    3073 B |        1.00 |
 *  | ParseFEN_Improved1           | rnbqk(...)6 0 1 [67] | 3,225.5 ns |  38.78 ns |  32.38 ns | 3,220.2 ns |  0.91 |    0.02 | 0.4463 |      - |    2801 B |        0.91 |
 *  | ParseFEN_Base2               | rnbqk(...)6 0 1 [67] | 3,319.0 ns |  31.29 ns |  29.27 ns | 3,311.9 ns |  0.94 |    0.02 | 0.4616 | 0.0038 |    2905 B |        0.95 |
 *  | ParseFEN_NoRegex             | rnbqk(...)6 0 1 [67] | 1,677.7 ns |  18.83 ns |  14.70 ns | 1,670.1 ns |  0.47 |    0.01 | 0.0839 |      - |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard | rnbqk(...)6 0 1 [67] |   536.6 ns |   7.37 ns |   6.90 ns |   534.2 ns |  0.15 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  |                              |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original            | rnbqk(...)- 0 1 [56] | 2,506.3 ns |  42.48 ns |  35.47 ns | 2,497.0 ns |  1.00 |    0.00 | 0.4387 |      - |    2761 B |        1.00 |
 *  | ParseFEN_Improved1           | rnbqk(...)- 0 1 [56] | 2,329.3 ns |  44.57 ns |  41.69 ns | 2,319.3 ns |  0.93 |    0.02 | 0.3967 |      - |    2497 B |        0.90 |
 *  | ParseFEN_Base2               | rnbqk(...)- 0 1 [56] | 2,319.6 ns |  45.21 ns |  69.04 ns | 2,291.5 ns |  0.95 |    0.01 | 0.4120 |      - |    2585 B |        0.94 |
 *  | ParseFEN_NoRegex             | rnbqk(...)- 0 1 [56] |   867.6 ns |  13.83 ns |  20.70 ns |   863.9 ns |  0.35 |    0.01 | 0.0420 |      - |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard | rnbqk(...)- 0 1 [56] |   564.1 ns |   7.06 ns |   5.90 ns |   561.6 ns |  0.23 |    0.00 | 0.0267 |      - |     168 B |        0.06 |
 *  |                              |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original            | rq2k2(...)- 0 1 [71] | 3,832.2 ns |  74.22 ns |  82.50 ns | 3,817.5 ns |  1.00 |    0.00 | 0.5035 |      - |    3169 B |        1.00 |
 *  | ParseFEN_Improved1           | rq2k2(...)- 0 1 [71] | 3,589.3 ns |  27.62 ns |  23.07 ns | 3,589.4 ns |  0.94 |    0.02 | 0.4616 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2               | rq2k2(...)- 0 1 [71] | 3,687.5 ns |  39.47 ns |  34.99 ns | 3,685.2 ns |  0.96 |    0.02 | 0.4807 | 0.0038 |    3025 B |        0.95 |
 *  | ParseFEN_NoRegex             | rq2k2(...)- 0 1 [71] | 2,004.8 ns |  39.24 ns |  36.70 ns | 1,984.7 ns |  0.52 |    0.02 | 0.0992 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard | rq2k2(...)- 0 1 [71] |   577.0 ns |   7.93 ns |   7.41 ns |   576.4 ns |  0.15 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using ParseResult = (bool Success, ulong[] PieceBitBoards, ulong[] OccupancyBitBoards, Lynx.Model.Side Side, byte Castle, Lynx.Model.BoardSquare EnPassant,
            int HalfMoveClock, int FullMoveCounter);

namespace Lynx.Benchmark;
public partial class ParseFENBenchmark : BaseBenchmark
{
    public static IEnumerable<string> Data => new[] {
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.CmkTestPositionFEN,
        Constants.ComplexPositionFEN,
        Constants.KillerTestPositionFEN,
        Constants.TTPositionFEN
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Original(string fen) => ParseFEN_FENParser_Original.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Improved1(string fen) => ParseFEN_FENParser_Improved1.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_Base2(string fen) => ParseFEN_FENParser_Base2.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_NoRegex(string fen) => ParseFEN_FENParser_NoRegex.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_OptimizedParseBoard(string fen) => ParseFEN_FENParser_OptimizedParseBoard.ParseFEN(fen);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseResult ParseFEN_OptimizedPopulateOccupancies(string fen) => ParseFEN_FENParser_OptimizedPopulateOccupancies.ParseFEN(fen);

    public static partial class ParseFEN_FENParser_Original
    {
        [GeneratedRegex("(?<=^|\\/)[P|N|B|R|Q|K|p|n|b|r|q|k|\\d]{1,8}", RegexOptions.Compiled)]
        private static partial Regex RanksRegex();

        private static readonly Regex _ranksRegex = RanksRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ParseResult ParseFEN(string fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12] {
                default, default, default, default,
                default, default, default, default,
                default, default, default, default};

            var occupancyBitBoards = new BitBoard[3] { default, default, default };

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                MatchCollection matches;
                (matches, success) = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedString = fen[(matches[^1].Index + matches[^1].Length)..];
                var parts = unparsedString.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(parts[0]);

                castle = ParseCastlingRights(parts[1]);

                (enPassant, success) = ParseEnPassant(parts[2], pieceBitBoards, side);

                if (parts.Length < 4 || !int.TryParse(parts[3], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (parts.Length < 5 || !int.TryParse(parts[4], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen);
                _logger.Error(e.Message);
                success = false;
            }

            if (!success)
            {
                throw new();
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        private static (MatchCollection Matches, bool Success) ParseBoard(string fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;

            var rankIndex = 0;
            var matches = _ranksRegex.Matches(fen);

            if (matches.Count != 8)
            {
                return (matches, false);
            }

            foreach (var match in matches)
            {
                var fileIndex = 0;
                foreach (var ch in ((Group)match).Value)
                {
                    if (Constants.PiecesByChar.TryGetValue(ch, out Piece piece))
                    {
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else if (int.TryParse($"{ch}", out int emptySquares))
                    {
                        fileIndex += emptySquares;
                    }
                    else
                    {
                        _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, ((Group)match).Value);
                        success = false;
                        break;
                    }
                }

                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                ++rankIndex;
            }

            return (matches, success);

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                var limit = (int)Piece.K;
                for (int piece = (int)Piece.P; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                }

                limit = (int)Piece.k;
                for (int piece = (int)Piece.p; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
            }
        }

        private static Side ParseSide(string sideString)
        {
#pragma warning disable S3358 // Ternary operators should not be nested
            bool isWhite = sideString.Equals("w", StringComparison.OrdinalIgnoreCase);

            return isWhite || sideString.Equals("b", StringComparison.OrdinalIgnoreCase)
                ? isWhite ? Side.White : Side.Black
                : throw new($"Unrecognized side: {sideString}");
#pragma warning restore S3358 // Ternary operators should not be nested
        }

        private static byte ParseCastlingRights(string castleString)
        {
            byte castle = 0;

            foreach (var ch in castleString)
            {
                castle |= ch switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {ch}")
                };
            }

            return castle;
        }

        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(string enPassantString, BitBoard[] PieceBitBoards, Side side)
        {
            bool success = true;
            BoardSquare enPassant = BoardSquare.noSquare;

            if (Enum.TryParse(enPassantString, ignoreCase: true, out BoardSquare result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant >> 3);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    _logger.Error("Invalid en passant square: {0}", enPassantString);
                }

                // Check that there's an actual pawn to be captured
                var pawnOffset = side == Side.White
                    ? +8
                    : -8;

                var pawnSquare = (int)enPassant + pawnOffset;

                var pawnBitBoard = side == Side.White
                    ? PieceBitBoards[(int)Piece.p]
                    : PieceBitBoards[(int)Piece.P];

                if (!pawnBitBoard.GetBit(pawnSquare))
                {
                    success = false;
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantString, side, pawnSquare);
                }
            }
            else if (enPassantString != "-")
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantString);
            }

            return (enPassant, success);
        }
    }

    public static partial class ParseFEN_FENParser_Improved1
    {
        [GeneratedRegex("(?<=^|\\/)[P|N|B|R|Q|K|p|n|b|r|q|k|\\d]{1,8}", RegexOptions.Compiled)]
        private static partial Regex RanksRegex();

        private static readonly Regex _ranksRegex = RanksRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ParseResult ParseFEN(string fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12] {
                default, default, default, default,
                default, default, default, default,
                default, default, default, default};

            var occupancyBitBoards = new BitBoard[3] { default, default, default };

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                MatchCollection matches;
                (matches, success) = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedString = fen[(matches[^1].Index + matches[^1].Length)..].AsSpan();
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedString.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedString, parts[0]);

                castle = ParseCastlingRights(unparsedString, parts[1]);

                (enPassant, success) = ParseEnPassant(unparsedString, parts[2], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedString[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (partsLength < 5 || !int.TryParse(unparsedString[parts[4]], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen);
                _logger.Error(e.Message);
                success = false;
            }

            if (!success)
            {
                throw new();
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        private static (MatchCollection Matches, bool Success) ParseBoard(string fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;

            var rankIndex = 0;
            var matches = _ranksRegex.Matches(fen);

            if (matches.Count != 8)
            {
                return (matches, false);
            }

            foreach (var match in matches)
            {
                var fileIndex = 0;
                foreach (var ch in ((Group)match).Value)
                {
                    if (Constants.PiecesByChar.TryGetValue(ch, out Piece piece))
                    {
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else if (int.TryParse($"{ch}", out int emptySquares))
                    {
                        fileIndex += emptySquares;
                    }
                    else
                    {
                        _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, ((Group)match).Value);
                        success = false;
                        break;
                    }
                }

                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                ++rankIndex;
            }

            return (matches, success);

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                var limit = (int)Piece.K;
                for (int piece = (int)Piece.P; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                }

                limit = (int)Piece.k;
                for (int piece = (int)Piece.p; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
            }
        }

        private static Side ParseSide(ReadOnlySpan<char> unparsedString, Range sideRange)
        {
            var sidePart = unparsedString[sideRange];
#pragma warning disable S3358 // Ternary operators should not be nested
            bool isWhite = sidePart[0].Equals('w');

            return isWhite || sidePart[0].Equals('b')
                ? isWhite ? Side.White : Side.Black
                : throw new($"Unrecognized side: {sidePart}");
#pragma warning restore S3358 // Ternary operators should not be nested
        }

        private static byte ParseCastlingRights(ReadOnlySpan<char> unparsedString, Range castleRange)
        {
            byte castle = 0;

            foreach (var ch in unparsedString[castleRange])
            {
                castle |= ch switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {ch}")
                };
            }

            return castle;
        }

        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> unparsedString, Range enPassantRange, BitBoard[] PieceBitBoards, Side side)
        {
            var enPassantPart = unparsedString[enPassantRange];
            bool success = true;
            BoardSquare enPassant = BoardSquare.noSquare;

            if (Enum.TryParse(enPassantPart, ignoreCase: true, out BoardSquare result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant >> 3);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    _logger.Error("Invalid en passant square: {0}", enPassantPart.ToString());
                }

                // Check that there's an actual pawn to be captured
                var pawnOffset = side == Side.White
                    ? +8
                    : -8;

                var pawnSquare = (int)enPassant + pawnOffset;

                var pawnBitBoard = side == Side.White
                    ? PieceBitBoards[(int)Piece.p]
                    : PieceBitBoards[(int)Piece.P];

                if (!pawnBitBoard.GetBit(pawnSquare))
                {
                    success = false;
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantPart.ToString(), side, pawnSquare);
                }
            }
            else if (enPassantPart[0] != '-')
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantPart.ToString());
            }

            return (enPassant, success);
        }
    }

    public static partial class ParseFEN_FENParser_Base2
    {
        [GeneratedRegex("(?<=^|\\/)[P|N|B|R|Q|K|p|n|b|r|q|k|\\d]{1,8}", RegexOptions.Compiled)]
        private static partial Regex RanksRegex();

        private static readonly Regex _ranksRegex = RanksRegex();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12];
            var occupancyBitBoards = new BitBoard[3];

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                MatchCollection matches;
                (matches, success) = ParseBoard(fen.ToString(), pieceBitBoards, occupancyBitBoards);

                var unparsedStringAsSpan = fen[(matches[^1].Index + matches[^1].Length)..];
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedStringAsSpan[parts[0]]);

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen.ToString());
                _logger.Error(e.Message);
                success = false;
            }

            if (!success)
            {
                throw new();
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (MatchCollection Matches, bool Success) ParseBoard(string fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;

            var rankIndex = 0;
            var matches = _ranksRegex.Matches(fen);

            if (matches.Count != 8)
            {
                return (matches, false);
            }

            foreach (var match in matches)
            {
                var fileIndex = 0;
                foreach (var ch in ((Group)match).Value)
                {
                    if (Constants.PiecesByChar.TryGetValue(ch, out Piece piece))
                    {
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else if (int.TryParse($"{ch}", out int emptySquares))
                    {
                        fileIndex += emptySquares;
                    }
                    else
                    {
                        _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, ((Group)match).Value);
                        success = false;
                        break;
                    }
                }

                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                ++rankIndex;
            }

            return (matches, success);

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                var limit = (int)Piece.K;
                for (int piece = (int)Piece.P; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                }

                limit = (int)Piece.k;
                for (int piece = (int)Piece.p; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Side ParseSide(ReadOnlySpan<char> side)
        {
            return side[0] switch
            {
                'w' or 'W' => Side.White,
                'b' or 'B' => Side.Black,
                _ => throw new($"Unrecognized side: {side}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ParseCastlingRights(ReadOnlySpan<char> castling)
        {
            byte castle = 0;

            for (int i = 0; i < castling.Length; ++i)
            {
                castle |= castling[i] switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {castling[i]}")
                };
            }

            return castle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, BitBoard[] PieceBitBoards, Side side)
        {
            bool success = true;
            BoardSquare enPassant = BoardSquare.noSquare;

            if (Enum.TryParse(enPassantSpan, ignoreCase: true, out BoardSquare result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant >> 3);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
                }

                // Check that there's an actual pawn to be captured
                var pawnOffset = side == Side.White
                    ? +8
                    : -8;

                var pawnSquare = (int)enPassant + pawnOffset;

                var pawnBitBoard = side == Side.White
                    ? PieceBitBoards[(int)Piece.p]
                    : PieceBitBoards[(int)Piece.P];

                if (!pawnBitBoard.GetBit(pawnSquare))
                {
                    success = false;
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
                }
            }
            else if (enPassantSpan[0] != '-')
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
            }

            return (enPassant, success);
        }
    }

    public static class ParseFEN_FENParser_NoRegex
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12];
            var occupancyBitBoards = new BitBoard[3];

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                success = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedStringAsSpan[parts[0]]);

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen.ToString());
                _logger.Error(e.Message);
                success = false;
            }

            if (!success)
            {
                throw new();
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;
            var rankIndex = 0;
            var end = fen.IndexOf('/');

            while (success && end != -1)
            {
                var match = fen[..end];

                ParseBoardSection(pieceBitBoards, ref success, rankIndex, match);
                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                fen = fen[(end + 1)..];
                end = fen.IndexOf('/');
                ++rankIndex;
            }

            ParseBoardSection(pieceBitBoards, ref success, rankIndex, fen[..fen.IndexOf(' ')]);
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            return success;

            static void ParseBoardSection(ulong[] pieceBitBoards, ref bool success, int rankIndex, ReadOnlySpan<char> boardfenSection)
            {
                int fileIndex = 0;

                foreach (var ch in boardfenSection)
                {
                    if (Constants.PiecesByChar.TryGetValue(ch, out Piece piece))
                    {
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else if (int.TryParse($"{ch}", out int emptySquares))
                    {
                        fileIndex += emptySquares;
                    }
                    else
                    {
                        _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, boardfenSection.ToString());
                        success = false;
                        break;
                    }
                }
            }

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                var limit = (int)Piece.K;
                for (int piece = (int)Piece.P; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                }

                limit = (int)Piece.k;
                for (int piece = (int)Piece.p; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Side ParseSide(ReadOnlySpan<char> side)
        {
            return side[0] switch
            {
                'w' or 'W' => Side.White,
                'b' or 'B' => Side.Black,
                _ => throw new($"Unrecognized side: {side}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ParseCastlingRights(ReadOnlySpan<char> castling)
        {
            byte castle = 0;

            for (int i = 0; i < castling.Length; ++i)
            {
                castle |= castling[i] switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {castling[i]}")
                };
            }

            return castle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, BitBoard[] PieceBitBoards, Side side)
        {
            bool success = true;
            BoardSquare enPassant = BoardSquare.noSquare;

            if (Enum.TryParse(enPassantSpan, ignoreCase: true, out BoardSquare result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant >> 3);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
                }

                // Check that there's an actual pawn to be captured
                var pawnOffset = side == Side.White
                    ? +8
                    : -8;

                var pawnSquare = (int)enPassant + pawnOffset;

                var pawnBitBoard = side == Side.White
                    ? PieceBitBoards[(int)Piece.p]
                    : PieceBitBoards[(int)Piece.P];

                if (!pawnBitBoard.GetBit(pawnSquare))
                {
                    success = false;
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
                }
            }
            else if (enPassantSpan[0] != '-')
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
            }

            return (enPassant, success);
        }
    }

    public static class ParseFEN_FENParser_OptimizedParseBoard
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12];
            var occupancyBitBoards = new BitBoard[3];

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                success = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedStringAsSpan[parts[0]]);

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen.ToString());
                _logger.Error(e.Message);
                success = false;
            }

            if (!success)
            {
                throw new();
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;
            var rankIndex = 0;
            var end = fen.IndexOf('/');

            while (
                end != -1
#if DEBUG
                && success
#endif
                )
            {
                var match = fen[..end];

                ParseBoardSection(pieceBitBoards, rankIndex, match
#if DEBUG
                , ref success
#endif
                    );
                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                fen = fen[(end + 1)..];
                end = fen.IndexOf('/');
                ++rankIndex;
            }

            ParseBoardSection(pieceBitBoards, rankIndex, fen[..fen.IndexOf(' ')]
#if DEBUG
                , ref success
#endif
                );
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            return success;

            static void ParseBoardSection(ulong[] pieceBitBoards, int rankIndex, ReadOnlySpan<char> boardfenSection
#if DEBUG
                , ref bool success
#endif
                )
            {
                int fileIndex = 0;

                foreach (var ch in boardfenSection)
                {
                    var piece = ch switch
                    {
                        'P' => Piece.P,
                        'N' => Piece.N,
                        'B' => Piece.B,
                        'R' => Piece.R,
                        'Q' => Piece.Q,
                        'K' => Piece.K,

                        'p' => Piece.p,
                        'n' => Piece.n,
                        'b' => Piece.b,
                        'r' => Piece.r,
                        'q' => Piece.q,
                        'k' => Piece.k,

                        _ => Piece.None
                    };

                    if (piece != Piece.None)
                    {
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else
                    {
                        fileIndex += ch - '0';
#if DEBUG
                        System.Diagnostics.Debug.Assert(fileIndex >= 1 && fileIndex <= 8, $"Error parsing char {ch} in fen {boardfenSection.ToString()}");
                        success = false;
#endif
                    }
                }
            }

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                var limit = (int)Piece.K;
                for (int piece = (int)Piece.P; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                }

                limit = (int)Piece.k;
                for (int piece = (int)Piece.p; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Side ParseSide(ReadOnlySpan<char> side)
        {
            return side[0] switch
            {
                'w' or 'W' => Side.White,
                'b' or 'B' => Side.Black,
                _ => throw new($"Unrecognized side: {side}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ParseCastlingRights(ReadOnlySpan<char> castling)
        {
            byte castle = 0;

            for (int i = 0; i < castling.Length; ++i)
            {
                castle |= castling[i] switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {castling[i]}")
                };
            }

            return castle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, BitBoard[] PieceBitBoards, Side side)
        {
            bool success = true;
            BoardSquare enPassant = BoardSquare.noSquare;

            if (Enum.TryParse(enPassantSpan, ignoreCase: true, out BoardSquare result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant >> 3);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
                }

                // Check that there's an actual pawn to be captured
                var pawnOffset = side == Side.White
                    ? +8
                    : -8;

                var pawnSquare = (int)enPassant + pawnOffset;

                var pawnBitBoard = side == Side.White
                    ? PieceBitBoards[(int)Piece.p]
                    : PieceBitBoards[(int)Piece.P];

                if (!pawnBitBoard.GetBit(pawnSquare))
                {
                    success = false;
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
                }
            }
            else if (enPassantSpan[0] != '-')
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
            }

            return (enPassant, success);
        }
    }

    public static class ParseFEN_FENParser_OptimizedPopulateOccupancies
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            var pieceBitBoards = new BitBoard[12];
            var occupancyBitBoards = new BitBoard[3];

            bool success;
            Side side = Side.Both;
            byte castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquare enPassant = BoardSquare.noSquare;

            try
            {
                success = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedStringAsSpan[parts[0]]);

                castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                {
                    _logger.Debug("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error parsing FEN string {0}", fen.ToString());
                _logger.Error(e.Message);
                success = false;
            }

            if (!success)
            {
                throw new();
            }

            return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;
            var rankIndex = 0;
            var end = fen.IndexOf('/');

            while (
                end != -1
#if DEBUG
                && success
#endif
                )
            {
                var match = fen[..end];

                ParseBoardSection(pieceBitBoards, rankIndex, match
#if DEBUG
                , ref success
#endif
                    );
                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                fen = fen[(end + 1)..];
                end = fen.IndexOf('/');
                ++rankIndex;
            }

            ParseBoardSection(pieceBitBoards, rankIndex, fen[..fen.IndexOf(' ')]
#if DEBUG
                , ref success
#endif
                );
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            return success;

            static void ParseBoardSection(ulong[] pieceBitBoards, int rankIndex, ReadOnlySpan<char> boardfenSection
#if DEBUG
                , ref bool success
#endif
                )
            {
                int fileIndex = 0;

                foreach (var ch in boardfenSection)
                {
                    var piece = ch switch
                    {
                        'P' => Piece.P,
                        'N' => Piece.N,
                        'B' => Piece.B,
                        'R' => Piece.R,
                        'Q' => Piece.Q,
                        'K' => Piece.K,

                        'p' => Piece.p,
                        'n' => Piece.n,
                        'b' => Piece.b,
                        'r' => Piece.r,
                        'q' => Piece.q,
                        'k' => Piece.k,

                        _ => Piece.None
                    };

                    if (piece != Piece.None)
                    {
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else
                    {
                        fileIndex += ch - '0';
#if DEBUG
                        System.Diagnostics.Debug.Assert(fileIndex >= 1 && fileIndex <= 8, $"Error parsing char {ch} in fen {boardfenSection.ToString()}");
                        success = false;
#endif
                    }
                }
            }

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                    occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece + 6];
                }

                occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Side ParseSide(ReadOnlySpan<char> side)
        {
            return side[0] switch
            {
                'w' or 'W' => Side.White,
                'b' or 'B' => Side.Black,
                _ => throw new($"Unrecognized side: {side}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ParseCastlingRights(ReadOnlySpan<char> castling)
        {
            byte castle = 0;

            for (int i = 0; i < castling.Length; ++i)
            {
                castle |= castling[i] switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {castling[i]}")
                };
            }

            return castle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, BitBoard[] PieceBitBoards, Side side)
        {
            bool success = true;
            BoardSquare enPassant = BoardSquare.noSquare;

            if (Enum.TryParse(enPassantSpan, ignoreCase: true, out BoardSquare result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant >> 3);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
                }

                // Check that there's an actual pawn to be captured
                var pawnOffset = side == Side.White
                    ? +8
                    : -8;

                var pawnSquare = (int)enPassant + pawnOffset;

                var pawnBitBoard = side == Side.White
                    ? PieceBitBoards[(int)Piece.p]
                    : PieceBitBoards[(int)Piece.P];

                if (!pawnBitBoard.GetBit(pawnSquare))
                {
                    success = false;
                    _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
                }
            }
            else if (enPassantSpan[0] != '-')
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
            }

            return (enPassant, success);
        }
    }
}
