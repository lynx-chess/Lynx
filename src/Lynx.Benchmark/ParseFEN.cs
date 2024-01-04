/*
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                                | fen                  | Mean       | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |-------------------------------------- |--------------------- |-----------:|---------:|---------:|------:|-------:|----------:|------------:|
 *  | ParseFEN_Original                     | 8/k7/(...)- 0 1 [39] | 2,524.7 ns | 12.02 ns | 10.66 ns |  1.00 | 0.0343 |    2960 B |        1.00 |
 *  | ParseFEN_Improved1                    | 8/k7/(...)- 0 1 [39] | 2,288.4 ns | 18.94 ns | 17.72 ns |  0.91 | 0.0305 |    2704 B |        0.91 |
 *  | ParseFEN_Base2                        | 8/k7/(...)- 0 1 [39] | 2,367.6 ns |  7.51 ns |  6.27 ns |  0.94 | 0.0305 |    2760 B |        0.93 |
 *  | ParseFEN_NoRegex                      | 8/k7/(...)- 0 1 [39] | 1,079.5 ns |  4.92 ns |  4.36 ns |  0.43 | 0.0057 |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard          | 8/k7/(...)- 0 1 [39] |   332.3 ns |  0.76 ns |  0.59 ns |  0.13 | 0.0019 |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | 8/k7/(...)- 0 1 [39] |   317.7 ns |  2.06 ns |  1.92 ns |  0.13 | 0.0019 |     168 B |        0.06 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | r2q1r(...)- 0 9 [68] | 3,169.5 ns | 11.35 ns | 10.62 ns |  1.00 | 0.0343 |    3160 B |        1.00 |
 *  | ParseFEN_Improved1                    | r2q1r(...)- 0 9 [68] | 3,135.9 ns |  4.78 ns |  4.24 ns |  0.99 | 0.0343 |    2904 B |        0.92 |
 *  | ParseFEN_Base2                        | r2q1r(...)- 0 9 [68] | 3,170.2 ns | 13.62 ns | 12.74 ns |  1.00 | 0.0343 |    3016 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r2q1r(...)- 0 9 [68] | 1,565.7 ns |  1.15 ns |  0.96 ns |  0.49 | 0.0057 |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | r2q1r(...)- 0 9 [68] |   380.9 ns |  2.34 ns |  2.08 ns |  0.12 | 0.0019 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r2q1r(...)- 0 9 [68] |   390.8 ns |  1.56 ns |  1.39 ns |  0.12 | 0.0019 |     168 B |        0.05 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 3,030.9 ns |  4.14 ns |  3.46 ns |  1.00 | 0.0343 |    3080 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 2,855.1 ns |  8.86 ns |  8.29 ns |  0.94 | 0.0305 |    2816 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,723.6 ns | 15.72 ns | 14.70 ns |  0.90 | 0.0343 |    2928 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,441.1 ns |  1.23 ns |  1.03 ns |  0.48 | 0.0057 |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   400.5 ns |  2.11 ns |  1.87 ns |  0.13 | 0.0019 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   371.0 ns |  1.56 ns |  1.46 ns |  0.12 | 0.0019 |     168 B |        0.05 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 2,884.8 ns | 12.03 ns | 10.67 ns |  1.00 | 0.0343 |    3080 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 2,834.7 ns | 10.18 ns |  9.52 ns |  0.98 | 0.0305 |    2816 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,782.7 ns |  6.39 ns |  5.33 ns |  0.96 | 0.0343 |    2928 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,396.6 ns |  2.03 ns |  1.80 ns |  0.48 | 0.0057 |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   409.4 ns |  1.83 ns |  1.71 ns |  0.14 | 0.0019 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   374.5 ns |  3.72 ns |  3.30 ns |  0.13 | 0.0019 |     168 B |        0.05 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)6 0 1 [67] | 3,089.3 ns | 12.13 ns | 10.75 ns |  1.00 | 0.0343 |    3072 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)6 0 1 [67] | 2,699.1 ns | 10.40 ns |  9.73 ns |  0.87 | 0.0305 |    2800 B |        0.91 |
 *  | ParseFEN_Base2                        | rnbqk(...)6 0 1 [67] | 2,795.3 ns |  8.82 ns |  7.82 ns |  0.90 | 0.0343 |    2904 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)6 0 1 [67] | 1,328.5 ns |  1.40 ns |  1.17 ns |  0.43 | 0.0057 |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)6 0 1 [67] |   400.5 ns |  0.84 ns |  0.74 ns |  0.13 | 0.0019 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)6 0 1 [67] |   394.7 ns |  1.76 ns |  1.65 ns |  0.13 | 0.0019 |     168 B |        0.05 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)- 0 1 [56] | 2,059.5 ns |  4.90 ns |  4.59 ns |  1.00 | 0.0305 |    2760 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)- 0 1 [56] | 2,007.0 ns |  5.08 ns |  4.75 ns |  0.97 | 0.0267 |    2496 B |        0.90 |
 *  | ParseFEN_Base2                        | rnbqk(...)- 0 1 [56] | 1,916.2 ns |  6.67 ns |  5.57 ns |  0.93 | 0.0305 |    2584 B |        0.94 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)- 0 1 [56] |   776.1 ns |  0.99 ns |  0.83 ns |  0.38 | 0.0029 |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)- 0 1 [56] |   374.3 ns |  0.60 ns |  0.50 ns |  0.18 | 0.0019 |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)- 0 1 [56] |   356.9 ns |  1.50 ns |  1.26 ns |  0.17 | 0.0019 |     168 B |        0.06 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | rq2k2(...)- 0 1 [71] | 3,124.3 ns | 10.80 ns | 10.10 ns |  1.00 | 0.0343 |    3168 B |        1.00 |
 *  | ParseFEN_Improved1                    | rq2k2(...)- 0 1 [71] | 3,033.7 ns |  6.11 ns |  5.72 ns |  0.97 | 0.0343 |    2904 B |        0.92 |
 *  | ParseFEN_Base2                        | rq2k2(...)- 0 1 [71] | 3,127.4 ns |  5.19 ns |  4.33 ns |  1.00 | 0.0343 |    3024 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rq2k2(...)- 0 1 [71] | 1,590.6 ns |  8.02 ns |  7.50 ns |  0.51 | 0.0057 |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | rq2k2(...)- 0 1 [71] |   380.7 ns |  1.12 ns |  1.05 ns |  0.12 | 0.0019 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rq2k2(...)- 0 1 [71] |   371.5 ns |  0.64 ns |  0.53 ns |  0.12 | 0.0019 |     168 B |        0.05 |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                                | fen                  | Mean       | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |-------------------------------------- |--------------------- |-----------:|---------:|---------:|------:|-------:|----------:|------------:|
 *  | ParseFEN_Original                     | 8/k7/(...)- 0 1 [39] | 2,125.6 ns | 15.09 ns | 12.60 ns |  1.00 | 0.1755 |    2961 B |        1.00 |
 *  | ParseFEN_Improved1                    | 8/k7/(...)- 0 1 [39] | 2,116.1 ns | 15.77 ns | 13.17 ns |  1.00 | 0.1602 |    2704 B |        0.91 |
 *  | ParseFEN_Base2                        | 8/k7/(...)- 0 1 [39] | 2,047.4 ns | 26.62 ns | 24.90 ns |  0.96 | 0.1640 |    2760 B |        0.93 |
 *  | ParseFEN_NoRegex                      | 8/k7/(...)- 0 1 [39] |   972.4 ns |  3.72 ns |  3.30 ns |  0.46 | 0.0286 |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard          | 8/k7/(...)- 0 1 [39] |   308.4 ns |  0.93 ns |  0.78 ns |  0.15 | 0.0100 |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | 8/k7/(...)- 0 1 [39] |   315.1 ns |  1.44 ns |  1.28 ns |  0.15 | 0.0100 |     168 B |        0.06 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | r2q1r(...)- 0 9 [68] | 2,686.8 ns | 11.73 ns | 10.97 ns |  1.00 | 0.1869 |    3161 B |        1.00 |
 *  | ParseFEN_Improved1                    | r2q1r(...)- 0 9 [68] | 2,686.6 ns | 15.47 ns | 14.47 ns |  1.00 | 0.1717 |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | r2q1r(...)- 0 9 [68] | 2,590.5 ns |  5.99 ns |  5.60 ns |  0.96 | 0.1793 |    3017 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r2q1r(...)- 0 9 [68] | 1,393.3 ns |  3.90 ns |  3.46 ns |  0.52 | 0.0362 |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | r2q1r(...)- 0 9 [68] |   377.1 ns |  1.54 ns |  1.44 ns |  0.14 | 0.0100 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r2q1r(...)- 0 9 [68] |   375.4 ns |  0.91 ns |  0.80 ns |  0.14 | 0.0100 |     168 B |        0.05 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 2,554.5 ns | 19.55 ns | 17.33 ns |  1.00 | 0.1831 |    3081 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 2,541.3 ns | 14.49 ns | 11.31 ns |  1.00 | 0.1678 |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,491.1 ns | 27.74 ns | 25.95 ns |  0.98 | 0.1717 |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,297.3 ns |  7.77 ns |  7.27 ns |  0.51 | 0.0324 |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   386.0 ns |  0.66 ns |  0.59 ns |  0.15 | 0.0100 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   383.5 ns |  1.15 ns |  1.02 ns |  0.15 | 0.0100 |     168 B |        0.05 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 2,543.6 ns | 16.06 ns | 14.24 ns |  1.00 | 0.1831 |    3081 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 2,465.2 ns | 12.26 ns | 10.87 ns |  0.97 | 0.1678 |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,479.2 ns | 20.19 ns | 18.89 ns |  0.97 | 0.1717 |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,275.2 ns |  1.49 ns |  1.24 ns |  0.50 | 0.0324 |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   374.7 ns |  1.66 ns |  1.56 ns |  0.15 | 0.0100 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   358.2 ns |  1.76 ns |  1.47 ns |  0.14 | 0.0100 |     168 B |        0.05 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)6 0 1 [67] | 2,528.8 ns |  7.25 ns |  6.78 ns |  1.00 | 0.1831 |    3073 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)6 0 1 [67] | 2,431.1 ns | 12.50 ns | 11.08 ns |  0.96 | 0.1640 |    2800 B |        0.91 |
 *  | ParseFEN_Base2                        | rnbqk(...)6 0 1 [67] | 2,418.0 ns |  6.77 ns |  5.29 ns |  0.96 | 0.1717 |    2905 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)6 0 1 [67] | 1,205.9 ns |  4.79 ns |  4.48 ns |  0.48 | 0.0305 |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)6 0 1 [67] |   350.5 ns |  0.98 ns |  0.82 ns |  0.14 | 0.0100 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)6 0 1 [67] |   336.5 ns |  0.53 ns |  0.50 ns |  0.13 | 0.0100 |     168 B |        0.05 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)- 0 1 [56] | 1,790.1 ns | 14.62 ns | 12.96 ns |  1.00 | 0.1640 |    2760 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)- 0 1 [56] | 1,628.0 ns |  6.50 ns |  6.08 ns |  0.91 | 0.1488 |    2496 B |        0.90 |
 *  | ParseFEN_Base2                        | rnbqk(...)- 0 1 [56] | 1,645.9 ns |  5.81 ns |  4.85 ns |  0.92 | 0.1545 |    2584 B |        0.94 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)- 0 1 [56] |   743.8 ns |  1.82 ns |  1.70 ns |  0.42 | 0.0153 |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)- 0 1 [56] |   366.6 ns |  1.36 ns |  1.27 ns |  0.20 | 0.0100 |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)- 0 1 [56] |   361.1 ns |  0.99 ns |  0.88 ns |  0.20 | 0.0100 |     168 B |        0.06 |
 *  |                                       |                      |            |          |          |       |        |           |             |
 *  | ParseFEN_Original                     | rq2k2(...)- 0 1 [71] | 2,756.2 ns |  6.30 ns |  5.58 ns |  1.00 | 0.1869 |    3169 B |        1.00 |
 *  | ParseFEN_Improved1                    | rq2k2(...)- 0 1 [71] | 2,686.1 ns |  9.56 ns |  8.94 ns |  0.97 | 0.1717 |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | rq2k2(...)- 0 1 [71] | 2,689.8 ns |  9.50 ns |  8.42 ns |  0.98 | 0.1793 |    3025 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rq2k2(...)- 0 1 [71] | 1,418.5 ns |  7.53 ns |  7.04 ns |  0.51 | 0.0362 |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | rq2k2(...)- 0 1 [71] |   401.6 ns |  0.53 ns |  0.41 ns |  0.15 | 0.0100 |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rq2k2(...)- 0 1 [71] |   383.2 ns |  1.64 ns |  1.45 ns |  0.14 | 0.0100 |     168 B |        0.05 |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX
 *
 *  | Method                                | fen                  | Mean       | Error    | StdDev    | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------------------------- |--------------------- |-----------:|---------:|----------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original                     | 8/k7/(...)- 0 1 [39] | 3,027.5 ns | 60.25 ns | 154.45 ns | 2,978.2 ns |  1.00 |    0.00 | 0.4692 |      - |    2961 B |        1.00 |
 *  | ParseFEN_Improved1                    | 8/k7/(...)- 0 1 [39] | 2,646.7 ns | 49.90 ns |  57.47 ns | 2,655.1 ns |  0.88 |    0.06 | 0.4311 |      - |    2705 B |        0.91 |
 *  | ParseFEN_Base2                        | 8/k7/(...)- 0 1 [39] | 2,757.1 ns | 18.09 ns |  16.92 ns | 2,753.3 ns |  0.92 |    0.05 | 0.4387 |      - |    2761 B |        0.93 |
 *  | ParseFEN_NoRegex                      | 8/k7/(...)- 0 1 [39] | 1,283.2 ns |  6.19 ns |   5.48 ns | 1,282.6 ns |  0.43 |    0.02 | 0.0763 |      - |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard          | 8/k7/(...)- 0 1 [39] |   439.4 ns |  8.51 ns |   7.10 ns |   439.8 ns |  0.15 |    0.01 | 0.0267 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | 8/k7/(...)- 0 1 [39] |   410.6 ns |  3.45 ns |   3.23 ns |   409.8 ns |  0.14 |    0.01 | 0.0267 |      - |     168 B |        0.06 |
 *  |                                       |                      |            |          |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | r2q1r(...)- 0 9 [68] | 3,698.7 ns | 60.75 ns |  50.73 ns | 3,710.6 ns |  1.00 |    0.00 | 0.5035 |      - |    3161 B |        1.00 |
 *  | ParseFEN_Improved1                    | r2q1r(...)- 0 9 [68] | 3,528.1 ns | 51.56 ns |  45.71 ns | 3,538.6 ns |  0.96 |    0.02 | 0.4616 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | r2q1r(...)- 0 9 [68] | 3,747.1 ns | 74.17 ns |  88.29 ns | 3,755.4 ns |  1.01 |    0.03 | 0.4807 |      - |    3017 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r2q1r(...)- 0 9 [68] | 1,942.1 ns | 29.20 ns |  25.89 ns | 1,940.5 ns |  0.53 |    0.01 | 0.0992 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | r2q1r(...)- 0 9 [68] |   579.5 ns |  8.81 ns |   8.24 ns |   578.0 ns |  0.16 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r2q1r(...)- 0 9 [68] |   565.6 ns | 11.17 ns |  10.44 ns |   564.4 ns |  0.15 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  |                                       |                      |            |          |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 3,870.9 ns | 76.82 ns | 140.47 ns | 3,821.1 ns |  1.00 |    0.00 | 0.4883 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 3,421.6 ns | 64.83 ns |  77.18 ns | 3,406.2 ns |  0.87 |    0.05 | 0.4463 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 3,741.3 ns | 71.90 ns |  90.93 ns | 3,730.3 ns |  0.96 |    0.04 | 0.4654 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,687.8 ns | 33.75 ns |  38.87 ns | 1,669.9 ns |  0.43 |    0.02 | 0.0877 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   609.5 ns |  3.78 ns |   3.35 ns |   607.8 ns |  0.15 |    0.01 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   591.3 ns |  5.60 ns |   5.23 ns |   590.6 ns |  0.15 |    0.01 | 0.0267 |      - |     168 B |        0.05 |
 *  |                                       |                      |            |          |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 3,608.2 ns | 30.71 ns |  27.22 ns | 3,611.8 ns |  1.00 |    0.00 | 0.4883 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 3,377.4 ns | 60.42 ns |  56.52 ns | 3,374.1 ns |  0.93 |    0.02 | 0.4463 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 3,791.0 ns | 37.73 ns |  35.29 ns | 3,800.8 ns |  1.05 |    0.01 | 0.4654 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,793.3 ns | 30.26 ns |  28.30 ns | 1,784.9 ns |  0.50 |    0.01 | 0.0877 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   635.5 ns | 12.54 ns |  23.87 ns |   642.7 ns |  0.17 |    0.01 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   601.3 ns |  8.07 ns |   7.16 ns |   603.7 ns |  0.17 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  |                                       |                      |            |          |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)6 0 1 [67] | 3,908.1 ns | 58.65 ns |  54.86 ns | 3,890.9 ns |  1.00 |    0.00 | 0.4883 |      - |    3073 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)6 0 1 [67] | 3,512.7 ns | 35.79 ns |  29.88 ns | 3,510.1 ns |  0.90 |    0.01 | 0.4463 |      - |    2801 B |        0.91 |
 *  | ParseFEN_Base2                        | rnbqk(...)6 0 1 [67] | 3,573.5 ns | 70.68 ns | 121.92 ns | 3,578.9 ns |  0.93 |    0.04 | 0.4616 | 0.0038 |    2905 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)6 0 1 [67] | 1,808.6 ns | 27.75 ns |  25.96 ns | 1,804.9 ns |  0.46 |    0.01 | 0.0839 |      - |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)6 0 1 [67] |   573.3 ns |  3.23 ns |   2.86 ns |   573.4 ns |  0.15 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)6 0 1 [67] |   566.5 ns |  5.64 ns |   5.28 ns |   564.6 ns |  0.14 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  |                                       |                      |            |          |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)- 0 1 [56] | 2,689.6 ns | 35.42 ns |  29.57 ns | 2,679.4 ns |  1.00 |    0.00 | 0.4387 |      - |    2761 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)- 0 1 [56] | 2,500.6 ns | 16.87 ns |  15.78 ns | 2,497.4 ns |  0.93 |    0.01 | 0.3967 |      - |    2497 B |        0.90 |
 *  | ParseFEN_Base2                        | rnbqk(...)- 0 1 [56] | 2,546.0 ns | 23.64 ns |  20.96 ns | 2,552.9 ns |  0.95 |    0.02 | 0.4120 |      - |    2585 B |        0.94 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)- 0 1 [56] | 1,008.9 ns | 14.43 ns |  13.50 ns | 1,011.0 ns |  0.38 |    0.00 | 0.0420 |      - |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)- 0 1 [56] |   542.9 ns |  8.88 ns |   8.31 ns |   538.9 ns |  0.20 |    0.00 | 0.0267 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)- 0 1 [56] |   562.2 ns |  6.47 ns |   6.05 ns |   560.8 ns |  0.21 |    0.00 | 0.0267 |      - |     168 B |        0.06 |
 *  |                                       |                      |            |          |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | rq2k2(...)- 0 1 [71] | 3,823.7 ns | 75.62 ns |  84.05 ns | 3,846.4 ns |  1.00 |    0.00 | 0.5035 |      - |    3170 B |        1.00 |
 *  | ParseFEN_Improved1                    | rq2k2(...)- 0 1 [71] | 3,661.7 ns | 57.23 ns |  50.73 ns | 3,655.0 ns |  0.96 |    0.03 | 0.4616 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | rq2k2(...)- 0 1 [71] | 3,800.6 ns | 72.78 ns |  83.81 ns | 3,793.9 ns |  0.99 |    0.03 | 0.4807 |      - |    3025 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rq2k2(...)- 0 1 [71] | 2,023.4 ns | 24.08 ns |  22.52 ns | 2,019.7 ns |  0.53 |    0.01 | 0.0992 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | rq2k2(...)- 0 1 [71] |   631.5 ns |  6.95 ns |   6.50 ns |   629.9 ns |  0.16 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rq2k2(...)- 0 1 [71] |   600.8 ns |  5.86 ns |   4.57 ns |   601.4 ns |  0.16 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
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
