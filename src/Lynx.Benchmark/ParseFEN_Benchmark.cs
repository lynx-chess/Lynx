/*
 *
 *  BenchmarkDotNet v0.14.0, Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *
 *  | Method                                | fen                  | Mean       | Error    | StdDev   | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------------------------- |--------------------- |-----------:|---------:|---------:|------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original                     | 8/k7/(...)- 0 1 [39] | 2,163.7 ns | 19.92 ns | 18.63 ns |  1.00 | 0.1755 |      - |    2961 B |        1.00 |
 *  | ParseFEN_Improved1                    | 8/k7/(...)- 0 1 [39] | 1,955.1 ns | 20.14 ns | 18.84 ns |  0.90 | 0.1602 |      - |    2705 B |        0.91 |
 *  | ParseFEN_Base2                        | 8/k7/(...)- 0 1 [39] | 1,993.1 ns | 23.42 ns | 21.91 ns |  0.92 | 0.1640 |      - |    2761 B |        0.93 |
 *  | ParseFEN_NoRegex                      | 8/k7/(...)- 0 1 [39] |   885.5 ns |  8.15 ns |  7.63 ns |  0.41 | 0.0286 |      - |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard          | 8/k7/(...)- 0 1 [39] |   309.3 ns |  1.96 ns |  1.74 ns |  0.14 | 0.0100 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | 8/k7/(...)- 0 1 [39] |   328.5 ns |  0.65 ns |  0.58 ns |  0.15 | 0.0100 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedParseEnPassant      | 8/k7/(...)- 0 1 [39] |   272.4 ns |  1.56 ns |  1.46 ns |  0.13 | 0.0348 | 0.0010 |     584 B |        0.20 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | r2q1r(...)- 0 9 [68] | 2,612.0 ns | 21.50 ns | 20.11 ns |  1.00 | 0.1869 |      - |    3161 B |        1.00 |
 *  | ParseFEN_Improved1                    | r2q1r(...)- 0 9 [68] | 2,434.7 ns |  9.81 ns |  9.17 ns |  0.93 | 0.1717 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | r2q1r(...)- 0 9 [68] | 2,504.4 ns |  6.07 ns |  5.38 ns |  0.96 | 0.1793 |      - |    3017 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r2q1r(...)- 0 9 [68] | 1,255.2 ns |  1.94 ns |  1.72 ns |  0.48 | 0.0362 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | r2q1r(...)- 0 9 [68] |   393.0 ns |  0.59 ns |  0.53 ns |  0.15 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r2q1r(...)- 0 9 [68] |   413.8 ns |  0.41 ns |  0.32 ns |  0.16 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r2q1r(...)- 0 9 [68] |   333.7 ns |  0.62 ns |  0.52 ns |  0.13 | 0.0348 | 0.0010 |     584 B |        0.18 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 2,477.0 ns |  8.02 ns |  7.11 ns |  1.00 | 0.1831 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 2,587.7 ns |  5.15 ns |  4.57 ns |  1.04 | 0.1678 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,384.0 ns | 25.45 ns | 21.25 ns |  0.96 | 0.1717 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,125.9 ns | 11.10 ns | 10.38 ns |  0.45 | 0.0324 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   394.8 ns |  2.32 ns |  2.17 ns |  0.16 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   419.6 ns |  2.05 ns |  1.82 ns |  0.17 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r3k2r(...)- 0 1 [68] |   326.0 ns |  1.72 ns |  1.61 ns |  0.13 | 0.0348 | 0.0010 |     584 B |        0.19 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 2,442.3 ns |  5.34 ns |  4.73 ns |  1.00 | 0.1831 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 2,441.4 ns | 20.05 ns | 18.75 ns |  1.00 | 0.1678 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,395.8 ns | 16.90 ns | 14.98 ns |  0.98 | 0.1717 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,154.2 ns |  2.26 ns |  2.00 ns |  0.47 | 0.0324 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   395.0 ns |  1.84 ns |  1.72 ns |  0.16 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   410.1 ns |  0.53 ns |  0.45 ns |  0.17 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r3k2r(...)- 0 1 [68] |   345.8 ns |  1.32 ns |  1.17 ns |  0.14 | 0.0348 | 0.0010 |     584 B |        0.19 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)6 0 1 [67] | 2,436.6 ns |  2.64 ns |  2.34 ns |  1.00 | 0.1831 |      - |    3073 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)6 0 1 [67] | 2,289.6 ns |  9.62 ns |  8.04 ns |  0.94 | 0.1640 |      - |    2801 B |        0.91 |
 *  | ParseFEN_Base2                        | rnbqk(...)6 0 1 [67] | 2,329.7 ns | 20.01 ns | 18.71 ns |  0.96 | 0.1717 |      - |    2905 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)6 0 1 [67] | 1,105.0 ns |  5.54 ns |  5.18 ns |  0.45 | 0.0305 |      - |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)6 0 1 [67] |   412.3 ns |  0.99 ns |  0.87 ns |  0.17 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)6 0 1 [67] |   388.8 ns |  2.03 ns |  1.90 ns |  0.16 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | rnbqk(...)6 0 1 [67] |   335.5 ns |  2.57 ns |  2.28 ns |  0.14 | 0.0348 | 0.0010 |     584 B |        0.19 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)- 0 1 [56] | 1,802.3 ns |  3.97 ns |  3.32 ns |  1.00 | 0.1640 | 0.0019 |    2761 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)- 0 1 [56] | 1,709.8 ns |  3.78 ns |  3.35 ns |  0.95 | 0.1488 | 0.0019 |    2496 B |        0.90 |
 *  | ParseFEN_Base2                        | rnbqk(...)- 0 1 [56] | 1,798.2 ns |  9.45 ns |  8.84 ns |  1.00 | 0.1545 | 0.0019 |    2584 B |        0.94 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)- 0 1 [56] |   634.8 ns |  1.00 ns |  0.83 ns |  0.35 | 0.0153 |      - |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)- 0 1 [56] |   386.9 ns |  0.53 ns |  0.49 ns |  0.21 | 0.0100 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)- 0 1 [56] |   357.9 ns |  0.89 ns |  0.74 ns |  0.20 | 0.0100 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedParseEnPassant      | rnbqk(...)- 0 1 [56] |   340.7 ns |  0.59 ns |  0.49 ns |  0.19 | 0.0348 | 0.0010 |     584 B |        0.21 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | rq2k2(...)- 0 1 [71] | 2,746.8 ns |  3.55 ns |  3.15 ns |  1.00 | 0.1869 |      - |    3169 B |        1.00 |
 *  | ParseFEN_Improved1                    | rq2k2(...)- 0 1 [71] | 2,478.9 ns |  5.30 ns |  4.43 ns |  0.90 | 0.1717 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | rq2k2(...)- 0 1 [71] | 2,597.1 ns |  5.84 ns |  5.17 ns |  0.95 | 0.1793 |      - |    3025 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rq2k2(...)- 0 1 [71] | 1,271.5 ns |  9.36 ns |  8.76 ns |  0.46 | 0.0362 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | rq2k2(...)- 0 1 [71] |   397.2 ns |  1.51 ns |  1.26 ns |  0.14 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rq2k2(...)- 0 1 [71] |   411.8 ns |  0.47 ns |  0.42 ns |  0.15 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | rq2k2(...)- 0 1 [71] |   346.1 ns |  3.78 ns |  3.54 ns |  0.13 | 0.0348 | 0.0010 |     584 B |        0.18 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.4052) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *
 *  | Method                                | fen                  | Mean       | Error    | StdDev   | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------------------------- |--------------------- |-----------:|---------:|---------:|------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original                     | 8/k7/(...)- 0 1 [39] | 1,943.1 ns | 12.57 ns | 11.75 ns |  1.00 | 0.1755 |      - |    2961 B |        1.00 |
 *  | ParseFEN_Improved1                    | 8/k7/(...)- 0 1 [39] | 1,856.3 ns | 14.68 ns | 13.73 ns |  0.96 | 0.1602 |      - |    2705 B |        0.91 |
 *  | ParseFEN_Base2                        | 8/k7/(...)- 0 1 [39] | 1,880.6 ns | 11.08 ns |  9.82 ns |  0.97 | 0.1640 |      - |    2761 B |        0.93 |
 *  | ParseFEN_NoRegex                      | 8/k7/(...)- 0 1 [39] |   806.5 ns |  2.16 ns |  2.02 ns |  0.42 | 0.0286 |      - |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard          | 8/k7/(...)- 0 1 [39] |   348.3 ns |  1.13 ns |  1.06 ns |  0.18 | 0.0100 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | 8/k7/(...)- 0 1 [39] |   340.9 ns |  0.72 ns |  0.64 ns |  0.18 | 0.0100 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedParseEnPassant      | 8/k7/(...)- 0 1 [39] |   280.1 ns |  2.10 ns |  1.96 ns |  0.14 | 0.0348 | 0.0010 |     584 B |        0.20 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | r2q1r(...)- 0 9 [68] | 2,447.4 ns | 14.02 ns | 12.43 ns |  1.00 | 0.1869 |      - |    3161 B |        1.00 |
 *  | ParseFEN_Improved1                    | r2q1r(...)- 0 9 [68] | 2,334.1 ns | 10.08 ns |  9.43 ns |  0.95 | 0.1717 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | r2q1r(...)- 0 9 [68] | 2,321.8 ns | 12.74 ns | 11.92 ns |  0.95 | 0.1793 |      - |    3017 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r2q1r(...)- 0 9 [68] | 1,084.8 ns |  2.54 ns |  2.25 ns |  0.44 | 0.0362 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | r2q1r(...)- 0 9 [68] |   422.6 ns |  1.00 ns |  0.88 ns |  0.17 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r2q1r(...)- 0 9 [68] |   407.0 ns |  1.12 ns |  1.04 ns |  0.17 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r2q1r(...)- 0 9 [68] |   330.7 ns |  2.53 ns |  2.36 ns |  0.14 | 0.0348 | 0.0010 |     584 B |        0.18 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 2,343.7 ns | 11.56 ns | 10.82 ns |  1.00 | 0.1831 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 2,275.1 ns | 13.01 ns | 12.17 ns |  0.97 | 0.1678 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,279.9 ns |  5.66 ns |  5.02 ns |  0.97 | 0.1717 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,004.1 ns |  1.78 ns |  1.66 ns |  0.43 | 0.0324 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   387.9 ns |  1.20 ns |  1.06 ns |  0.17 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   372.9 ns |  1.37 ns |  1.22 ns |  0.16 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r3k2r(...)- 0 1 [68] |   320.6 ns |  2.62 ns |  2.46 ns |  0.14 | 0.0348 | 0.0010 |     584 B |        0.19 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 2,328.9 ns | 14.27 ns | 13.35 ns |  1.00 | 0.1831 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 2,247.6 ns | 13.35 ns | 12.49 ns |  0.97 | 0.1678 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,221.3 ns | 10.52 ns |  9.84 ns |  0.95 | 0.1717 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,013.5 ns |  1.77 ns |  1.57 ns |  0.44 | 0.0324 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   388.7 ns |  1.13 ns |  1.01 ns |  0.17 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   389.4 ns |  0.69 ns |  0.61 ns |  0.17 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r3k2r(...)- 0 1 [68] |   335.5 ns |  1.86 ns |  1.74 ns |  0.14 | 0.0348 | 0.0010 |     584 B |        0.19 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)6 0 1 [67] | 2,243.5 ns |  9.02 ns |  7.99 ns |  1.00 | 0.1831 |      - |    3073 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)6 0 1 [67] | 2,169.6 ns | 10.65 ns |  9.97 ns |  0.97 | 0.1640 |      - |    2801 B |        0.91 |
 *  | ParseFEN_Base2                        | rnbqk(...)6 0 1 [67] | 2,187.3 ns |  9.47 ns |  8.86 ns |  0.97 | 0.1717 |      - |    2905 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)6 0 1 [67] |   995.4 ns |  2.67 ns |  2.36 ns |  0.44 | 0.0305 |      - |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)6 0 1 [67] |   394.4 ns |  1.12 ns |  0.99 ns |  0.18 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)6 0 1 [67] |   397.1 ns |  1.12 ns |  0.99 ns |  0.18 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | rnbqk(...)6 0 1 [67] |   326.9 ns |  2.19 ns |  1.82 ns |  0.15 | 0.0348 | 0.0010 |     584 B |        0.19 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)- 0 1 [56] | 1,720.7 ns |  9.79 ns |  8.68 ns |  1.00 | 0.1640 | 0.0019 |    2761 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)- 0 1 [56] | 1,601.6 ns |  9.60 ns |  8.98 ns |  0.93 | 0.1488 |      - |    2496 B |        0.90 |
 *  | ParseFEN_Base2                        | rnbqk(...)- 0 1 [56] | 1,636.7 ns | 10.35 ns |  9.68 ns |  0.95 | 0.1545 |      - |    2584 B |        0.94 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)- 0 1 [56] |   633.6 ns |  1.22 ns |  1.08 ns |  0.37 | 0.0153 |      - |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)- 0 1 [56] |   361.1 ns |  0.79 ns |  0.74 ns |  0.21 | 0.0100 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)- 0 1 [56] |   360.4 ns |  0.55 ns |  0.46 ns |  0.21 | 0.0100 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedParseEnPassant      | rnbqk(...)- 0 1 [56] |   349.0 ns |  1.74 ns |  1.54 ns |  0.20 | 0.0348 | 0.0010 |     584 B |        0.21 |
 *  |                                       |                      |            |          |          |       |        |        |           |             |
 *  | ParseFEN_Original                     | rq2k2(...)- 0 1 [71] | 2,522.9 ns | 11.00 ns | 10.29 ns |  1.00 | 0.1869 |      - |    3169 B |        1.00 |
 *  | ParseFEN_Improved1                    | rq2k2(...)- 0 1 [71] | 2,383.1 ns | 13.18 ns | 12.33 ns |  0.94 | 0.1717 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | rq2k2(...)- 0 1 [71] | 2,419.7 ns | 13.33 ns | 12.47 ns |  0.96 | 0.1793 |      - |    3025 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rq2k2(...)- 0 1 [71] | 1,138.6 ns |  2.89 ns |  2.41 ns |  0.45 | 0.0362 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | rq2k2(...)- 0 1 [71] |   382.5 ns |  1.63 ns |  1.44 ns |  0.15 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rq2k2(...)- 0 1 [71] |   371.8 ns |  1.22 ns |  1.14 ns |  0.15 | 0.0100 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | rq2k2(...)- 0 1 [71] |   328.4 ns |  2.23 ns |  2.09 ns |  0.13 | 0.0348 | 0.0010 |     584 B |        0.18 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.6 (22H625) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *
 *
 *  | Method                                | fen                  | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |-------------------------------------- |--------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original                     | 8/k7/(...)- 0 1 [39] | 2,777.7 ns |  45.24 ns |  40.11 ns | 2,757.0 ns |  1.00 |    0.02 | 0.4692 |      - |    2961 B |        1.00 |
 *  | ParseFEN_Improved1                    | 8/k7/(...)- 0 1 [39] | 2,559.3 ns |  48.29 ns |  45.17 ns | 2,537.5 ns |  0.92 |    0.02 | 0.4311 |      - |    2705 B |        0.91 |
 *  | ParseFEN_Base2                        | 8/k7/(...)- 0 1 [39] | 2,546.9 ns |  35.67 ns |  31.62 ns | 2,543.6 ns |  0.92 |    0.02 | 0.4387 |      - |    2761 B |        0.93 |
 *  | ParseFEN_NoRegex                      | 8/k7/(...)- 0 1 [39] | 1,209.0 ns |   5.63 ns |   4.70 ns | 1,207.0 ns |  0.44 |    0.01 | 0.0763 |      - |     480 B |        0.16 |
 *  | ParseFEN_OptimizedParseBoard          | 8/k7/(...)- 0 1 [39] |   383.7 ns |   2.67 ns |   2.37 ns |   382.8 ns |  0.14 |    0.00 | 0.0267 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | 8/k7/(...)- 0 1 [39] |   380.9 ns |   2.09 ns |   1.63 ns |   380.6 ns |  0.14 |    0.00 | 0.0267 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedParseEnPassant      | 8/k7/(...)- 0 1 [39] |   314.6 ns |   2.35 ns |   2.08 ns |   314.9 ns |  0.11 |    0.00 | 0.0930 | 0.0029 |     584 B |        0.20 |
 *  |                                       |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | r2q1r(...)- 0 9 [68] | 3,351.3 ns |  18.48 ns |  15.43 ns | 3,349.0 ns |  1.00 |    0.01 | 0.5035 |      - |    3162 B |        1.00 |
 *  | ParseFEN_Improved1                    | r2q1r(...)- 0 9 [68] | 3,179.1 ns |  51.43 ns |  45.59 ns | 3,154.2 ns |  0.95 |    0.01 | 0.4616 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | r2q1r(...)- 0 9 [68] | 3,127.7 ns |  31.87 ns |  29.81 ns | 3,113.8 ns |  0.93 |    0.01 | 0.4807 | 0.0038 |    3018 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r2q1r(...)- 0 9 [68] | 1,710.3 ns |  11.10 ns |  10.39 ns | 1,708.1 ns |  0.51 |    0.00 | 0.0992 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | r2q1r(...)- 0 9 [68] |   536.3 ns |   1.63 ns |   1.45 ns |   536.4 ns |  0.16 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r2q1r(...)- 0 9 [68] |   541.8 ns |   4.25 ns |   3.97 ns |   543.1 ns |  0.16 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r2q1r(...)- 0 9 [68] |   498.9 ns |   4.60 ns |   4.08 ns |   497.7 ns |  0.15 |    0.00 | 0.0925 | 0.0029 |     584 B |        0.18 |
 *  |                                       |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 3,133.6 ns |  44.65 ns |  34.86 ns | 3,117.0 ns |  1.00 |    0.02 | 0.4883 |      - |    3082 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 3,030.9 ns |  22.14 ns |  18.49 ns | 3,025.9 ns |  0.97 |    0.01 | 0.4463 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 2,936.1 ns |  35.35 ns |  31.34 ns | 2,921.4 ns |  0.94 |    0.01 | 0.4654 | 0.0038 |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,514.0 ns |   5.18 ns |   4.33 ns | 1,513.6 ns |  0.48 |    0.01 | 0.0877 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   562.0 ns |   1.84 ns |   1.54 ns |   561.8 ns |  0.18 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   556.3 ns |   5.25 ns |   4.38 ns |   556.7 ns |  0.18 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r3k2r(...)- 0 1 [68] |   524.7 ns |  10.43 ns |  21.55 ns |   519.2 ns |  0.17 |    0.01 | 0.0925 | 0.0029 |     584 B |        0.19 |
 *  |                                       |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | r3k2r(...)- 0 1 [68] | 3,319.1 ns |  47.31 ns |  39.51 ns | 3,312.4 ns |  1.00 |    0.02 | 0.4883 |      - |    3082 B |        1.00 |
 *  | ParseFEN_Improved1                    | r3k2r(...)- 0 1 [68] | 3,455.2 ns | 115.86 ns | 322.98 ns | 3,324.5 ns |  1.04 |    0.10 | 0.4463 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2                        | r3k2r(...)- 0 1 [68] | 3,112.7 ns |  60.80 ns | 169.48 ns | 3,078.8 ns |  0.94 |    0.05 | 0.4654 | 0.0038 |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex                      | r3k2r(...)- 0 1 [68] | 1,524.2 ns |  14.22 ns |  11.88 ns | 1,520.3 ns |  0.46 |    0.01 | 0.0877 |      - |     552 B |        0.18 |
 *  | ParseFEN_OptimizedParseBoard          | r3k2r(...)- 0 1 [68] |   566.5 ns |   5.42 ns |   4.81 ns |   564.7 ns |  0.17 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | r3k2r(...)- 0 1 [68] |   555.5 ns |   4.91 ns |   4.59 ns |   553.5 ns |  0.17 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | r3k2r(...)- 0 1 [68] |   470.3 ns |   2.98 ns |   2.49 ns |   470.7 ns |  0.14 |    0.00 | 0.0930 | 0.0029 |     584 B |        0.19 |
 *  |                                       |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)6 0 1 [67] | 3,105.8 ns |  23.61 ns |  19.72 ns | 3,113.0 ns |  1.00 |    0.01 | 0.4883 | 0.0038 |    3074 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)6 0 1 [67] | 2,916.6 ns |  40.86 ns |  38.22 ns | 2,913.1 ns |  0.94 |    0.01 | 0.4463 |      - |    2801 B |        0.91 |
 *  | ParseFEN_Base2                        | rnbqk(...)6 0 1 [67] | 2,870.8 ns |  37.71 ns |  35.27 ns | 2,870.4 ns |  0.92 |    0.01 | 0.4616 | 0.0038 |    2905 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)6 0 1 [67] | 1,485.7 ns |   6.61 ns |   5.52 ns | 1,484.2 ns |  0.48 |    0.00 | 0.0839 |      - |     528 B |        0.17 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)6 0 1 [67] |   519.1 ns |   3.23 ns |   3.03 ns |   518.1 ns |  0.17 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)6 0 1 [67] |   509.9 ns |   3.54 ns |   3.32 ns |   507.7 ns |  0.16 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | rnbqk(...)6 0 1 [67] |   438.6 ns |   3.17 ns |   2.97 ns |   439.4 ns |  0.14 |    0.00 | 0.0930 | 0.0029 |     584 B |        0.19 |
 *  |                                       |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | rnbqk(...)- 0 1 [56] | 2,289.9 ns |  32.22 ns |  30.14 ns | 2,295.0 ns |  1.00 |    0.02 | 0.4387 |      - |    2761 B |        1.00 |
 *  | ParseFEN_Improved1                    | rnbqk(...)- 0 1 [56] | 2,113.1 ns |  22.55 ns |  21.09 ns | 2,105.6 ns |  0.92 |    0.01 | 0.3967 |      - |    2497 B |        0.90 |
 *  | ParseFEN_Base2                        | rnbqk(...)- 0 1 [56] | 2,149.3 ns |  34.28 ns |  28.62 ns | 2,153.1 ns |  0.94 |    0.02 | 0.4120 |      - |    2585 B |        0.94 |
 *  | ParseFEN_NoRegex                      | rnbqk(...)- 0 1 [56] |   771.0 ns |  15.22 ns |  16.92 ns |   765.6 ns |  0.34 |    0.01 | 0.0420 |      - |     264 B |        0.10 |
 *  | ParseFEN_OptimizedParseBoard          | rnbqk(...)- 0 1 [56] |   500.5 ns |   7.61 ns |   6.36 ns |   498.7 ns |  0.22 |    0.00 | 0.0267 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rnbqk(...)- 0 1 [56] |   494.2 ns |   3.52 ns |   3.12 ns |   493.2 ns |  0.22 |    0.00 | 0.0267 |      - |     168 B |        0.06 |
 *  | ParseFEN_OptimizedParseEnPassant      | rnbqk(...)- 0 1 [56] |   463.8 ns |   7.68 ns |   7.18 ns |   461.4 ns |  0.20 |    0.00 | 0.0930 | 0.0029 |     584 B |        0.21 |
 *  |                                       |                      |            |           |           |            |       |         |        |        |           |             |
 *  | ParseFEN_Original                     | rq2k2(...)- 0 1 [71] | 3,472.7 ns |  27.23 ns |  24.14 ns | 3,480.3 ns |  1.00 |    0.01 | 0.5035 |      - |    3170 B |        1.00 |
 *  | ParseFEN_Improved1                    | rq2k2(...)- 0 1 [71] | 3,278.9 ns |  33.31 ns |  31.15 ns | 3,284.4 ns |  0.94 |    0.01 | 0.4616 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2                        | rq2k2(...)- 0 1 [71] | 3,271.0 ns |  41.79 ns |  37.05 ns | 3,264.5 ns |  0.94 |    0.01 | 0.4807 | 0.0038 |    3026 B |        0.95 |
 *  | ParseFEN_NoRegex                      | rq2k2(...)- 0 1 [71] | 1,762.6 ns |  23.54 ns |  22.02 ns | 1,762.0 ns |  0.51 |    0.01 | 0.0992 |      - |     624 B |        0.20 |
 *  | ParseFEN_OptimizedParseBoard          | rq2k2(...)- 0 1 [71] |   537.5 ns |   4.11 ns |   3.65 ns |   537.3 ns |  0.15 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedPopulateOccupancies | rq2k2(...)- 0 1 [71] |   549.1 ns |  10.99 ns |  11.28 ns |   545.1 ns |  0.16 |    0.00 | 0.0267 |      - |     168 B |        0.05 |
 *  | ParseFEN_OptimizedParseEnPassant      | rq2k2(...)- 0 1 [71] |   552.8 ns |   9.93 ns |   8.81 ns |   548.8 ns |  0.16 |    0.00 | 0.0925 | 0.0029 |     584 B |        0.18 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using ParseResult = (bool Success, ulong[] PieceBitBoards, ulong[] OccupancyBitBoards, Lynx.Model.Side Side, byte Castle, Lynx.Model.BoardSquare EnPassant,
            int HalfMoveClock, int FullMoveCounter);

namespace Lynx.Benchmark;

#pragma warning disable S112, S6667 // General or reserved exceptions should never be thrown

public partial class ParseFENBenchmark_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
    [
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.CmkTestPositionFEN,
        Constants.ComplexPositionFEN,
        Constants.KillerTestPositionFEN,
        Constants.TTPositionFEN
    ];

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

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ParseFENResult ParseFEN_OptimizedParseEnPassant(string fen) => ParseFEN_FENParser_OptimizedParseEnPassant.ParseFEN(fen);

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
            var rankIndex = 0;
            var end = fen.IndexOf('/');

            while (end != -1)
            {
                var match = fen[..end];

                ParseBoardSection(pieceBitBoards, rankIndex, match);
                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                fen = fen[(end + 1)..];
                end = fen.IndexOf('/');
                ++rankIndex;
            }

            ParseBoardSection(pieceBitBoards, rankIndex, fen[..fen.IndexOf(' ')]);
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            return true;

            static void ParseBoardSection(ulong[] pieceBitBoards, int rankIndex, ReadOnlySpan<char> boardfenSection)
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
            var rankIndex = 0;
            var end = fen.IndexOf('/');

            while (end != -1)
            {
                var match = fen[..end];

                ParseBoardSection(pieceBitBoards, rankIndex, match);
                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                fen = fen[(end + 1)..];
                end = fen.IndexOf('/');
                ++rankIndex;
            }

            ParseBoardSection(pieceBitBoards, rankIndex, fen[..fen.IndexOf(' ')]);
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            return true;

            static void ParseBoardSection(ulong[] pieceBitBoards, int rankIndex, ReadOnlySpan<char> boardfenSection)
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

    public static class ParseFEN_FENParser_OptimizedParseEnPassant
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#pragma warning disable IDE1006 // Naming Styles
        private static readonly SearchValues<char> _DFRCCastlingRightsChars =
#pragma warning restore IDE1006 // Naming Styles
            SearchValues.Create(
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParseFENResult ParseFEN(ReadOnlySpan<char> fen)
        {
            fen = fen.Trim();

            // Arrays will be be returned as part of Position cleanaup
            var pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
            var occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
            var board = ArrayPool<int>.Shared.Rent(64);
            Array.Fill(board, (int)Piece.None);

            bool success;
            Side side;
            byte castlingRights = 0;
            int halfMoveClock = 0/*, fullMoveCounter = 1*/;
            BoardSquare enPassant = BoardSquare.noSquare;
            CastlingData castlingData;

            try
            {
                ParseBoard(fen, pieceBitBoards, occupancyBitBoards, board);

                var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
                Span<Range> parts = stackalloc Range[5];
                var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

                if (partsLength < 3)
                {
                    throw new LynxException($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(unparsedStringAsSpan[parts[0]]);

                castlingData = !Configuration.EngineSettings.IsChess960
                        ? ParseStandardChessCastlingRights(unparsedStringAsSpan[parts[1]], pieceBitBoards, out castlingRights)
                        : ParseDFRCCastlingRights(unparsedStringAsSpan[parts[1]], pieceBitBoards, out castlingRights);

                (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

                if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
                {
                    _logger.Debug("No half move clock detected");
                }

                //if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
                //{
                //    _logger.Debug("No full move counter detected");
                //}

                if (pieceBitBoards[(int)Piece.K].CountBits() != 1
                    || pieceBitBoards[(int)Piece.k].CountBits() != 1)
                {
                    throw new LynxException("Missing or extra kings");
                }
            }
#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both - meh
            catch (Exception e)
            {
                _logger.Error(e, "Error parsing FEN {Fen}", fen.ToString());
                success = false;
                throw;
            }
#pragma warning restore S2139 // Exceptions should be either logged or rethrown but not both

            return success
                ? new(pieceBitBoards, occupancyBitBoards, board, side, castlingRights, enPassant,
                    castlingData,
                    halfMoveClock/*, fullMoveCounter*/)
                : throw new LynxException($"Error parsing {fen.ToString()}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards, int[] board)
        {
            var rankIndex = 0;
            var end = fen.IndexOf('/');

            while (end != -1)
            {
                var match = fen[..end];

                ParseBoardSection(pieceBitBoards, board, rankIndex, match);

                fen = fen[(end + 1)..];
                end = fen.IndexOf('/');
                ++rankIndex;
            }

            ParseBoardSection(pieceBitBoards, board, rankIndex, fen[..fen.IndexOf(' ')]);

            // Populate occupancies
            for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
            {
                occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece + 6];
            }

            occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void ParseBoardSection(BitBoard[] pieceBitBoards, int[] board, int rankIndex, ReadOnlySpan<char> boardfenSection)
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
                        var square = BitBoardExtensions.SquareIndex(rankIndex, fileIndex);
                        pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(square);
                        board[square] = (int)piece;
                        ++fileIndex;
                    }
                    else
                    {
                        fileIndex += ch - '0';
                        Debug.Assert(fileIndex >= 1 && fileIndex <= 8, $"Error parsing char {ch} in fen {boardfenSection.ToString()}");
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Side ParseSide(ReadOnlySpan<char> side)
        {
            return side[0] switch
            {
                'w' or 'W' => Side.White,
                'b' or 'B' => Side.Black,
                _ => throw new LynxException($"Unrecognized side: {side}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static CastlingData ParseStandardChessCastlingRights(ReadOnlySpan<char> castlingChars, ReadOnlySpan<BitBoard> pieceBitboards, out byte castlingRights)
        {
            if (castlingChars.ContainsAny(_DFRCCastlingRightsChars))
            {
                _logger.Warn("DFRC position detected without UCI_Chess960 set. Enabling it as a fallback");
                Configuration.EngineSettings.IsChess960 = true;

                return ParseDFRCCastlingRights(castlingChars, pieceBitboards, out castlingRights);
            }

            castlingRights = 0;
            for (int i = 0; i < castlingChars.Length; ++i)
            {
                castlingRights |= castlingChars[i] switch
                {
                    'K' => (byte)CastlingRights.WK,
                    'Q' => (byte)CastlingRights.WQ,
                    'k' => (byte)CastlingRights.BK,
                    'q' => (byte)CastlingRights.BQ,
                    '-' => castlingRights,
                    _ => throw new LynxException($"Unrecognized castling char: {castlingChars[i]}")
                };
            }

            return new CastlingData(
                Constants.InitialWhiteKingsideRookSquare, Constants.InitialWhiteQueensideRookSquare,
                Constants.InitialBlackKingsideRookSquare, Constants.InitialBlackQueensideRookSquare);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static CastlingData ParseDFRCCastlingRights(ReadOnlySpan<char> castlingChars, ReadOnlySpan<BitBoard> pieceBitboards, out byte castlingRights)
        {
            // X-FEN uses KQkq notation when not ambiguous, with the letters referring to "the outermost rook of the affected side"

            int whiteKingsideRook = CastlingData.DefaultValues, whiteQueensideRook = CastlingData.DefaultValues, blackKingsideRook = CastlingData.DefaultValues, blackQueensideRook = CastlingData.DefaultValues;

            var whiteKing = pieceBitboards[(int)Piece.K].GetLS1BIndex();
            var blackKing = pieceBitboards[(int)Piece.k].GetLS1BIndex();

            Debug.Assert(whiteKing != 0);
            Debug.Assert(blackKing != 0);

            castlingRights = 0;

            for (int i = 0; i < castlingChars.Length; ++i)
            {
                var ch = castlingChars[i];
                switch (ch)
                {
                    case 'K':
                        {
                            Debug.Assert(whiteKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white kingside rooks detected {whiteKingsideRook}");

                            castlingRights |= (byte)CastlingRights.WK;

                            for (int potentialRookSquareIndex = Constants.InitialWhiteKingsideRookSquare; potentialRookSquareIndex > whiteKing; --potentialRookSquareIndex)
                            {
                                if (pieceBitboards[(int)Piece.R].GetBit(potentialRookSquareIndex))
                                {
                                    whiteKingsideRook = potentialRookSquareIndex;
                                    break;
                                }
                            }

                            if (whiteKingsideRook == CastlingData.DefaultValues)
                            {
                                throw new LynxException($"Invalid castling rights: {ch} specified, but no rook found");
                            }

                            break;
                        }
                    case 'Q':
                        {
                            Debug.Assert(whiteQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white queenside rooks detected {whiteQueensideRook}");

                            castlingRights |= (byte)CastlingRights.WQ;

                            for (int potentialRookSquareIndex = Constants.InitialWhiteQueensideRookSquare; potentialRookSquareIndex < whiteKing; ++potentialRookSquareIndex)
                            {
                                if (pieceBitboards[(int)Piece.R].GetBit(potentialRookSquareIndex))
                                {
                                    whiteQueensideRook = potentialRookSquareIndex;
                                    break;
                                }
                            }

                            if (whiteQueensideRook == CastlingData.DefaultValues)
                            {
                                throw new LynxException($"Invalid castling rights: {ch} specified, but no rook found");
                            }

                            break;
                        }
                    case 'k':
                        {
                            Debug.Assert(blackKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black kingside rooks detected {blackKingsideRook}");

                            castlingRights |= (byte)CastlingRights.BK;

                            for (int potentialRookSquareIndex = Constants.InitialBlackKingsideRookSquare; potentialRookSquareIndex > blackKing; --potentialRookSquareIndex)
                            {
                                if (pieceBitboards[(int)Piece.r].GetBit(potentialRookSquareIndex))
                                {
                                    blackKingsideRook = potentialRookSquareIndex;
                                    break;
                                }
                            }

                            if (blackKingsideRook == CastlingData.DefaultValues)
                            {
                                throw new LynxException($"Invalid castling rights: {ch} specified, but no rook found");
                            }

                            break;
                        }
                    case 'q':
                        {
                            Debug.Assert(blackQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black queenside rooks detected {blackQueensideRook}");

                            castlingRights |= (byte)CastlingRights.BQ;

                            for (int potentialRookSquareIndex = Constants.InitialBlackQueensideRookSquare; potentialRookSquareIndex < blackKing; ++potentialRookSquareIndex)
                            {
                                if (pieceBitboards[(int)Piece.r].GetBit(potentialRookSquareIndex))
                                {
                                    blackQueensideRook = potentialRookSquareIndex;
                                    break;
                                }
                            }

                            if (blackQueensideRook == CastlingData.DefaultValues)
                            {
                                throw new LynxException($"Invalid castling rights: {ch} specified, but no rook found");
                            }

                            break;
                        }
                    case '-':
                        {
                            //castle |= (byte)CastlingRights.None;
                            break;
                        }
                    default:
                        {
                            if (ch >= 'A' && ch <= 'H')
                            {
                                var square = BitBoardExtensions.SquareIndex(rank: 7, file: ch - 'A');

                                if (square < whiteKing)
                                {
                                    Debug.Assert(whiteQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white queenside rooks detected {whiteQueensideRook}");
                                    castlingRights |= (byte)CastlingRights.WQ;
                                    whiteQueensideRook = square;
                                }
                                else if (square > whiteKing)
                                {
                                    Debug.Assert(whiteKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple white kingside rooks detected {whiteKingsideRook}");
                                    castlingRights |= (byte)CastlingRights.WK;
                                    whiteKingsideRook = square;
                                }
                                else
                                {
                                    throw new LynxException($"Invalid castle character {ch}: it matches white king square ({square})");
                                }

                                break;
                            }
                            else if (ch >= 'a' && ch <= 'h')
                            {
                                var square = BitBoardExtensions.SquareIndex(rank: 0, file: ch - 'a');

                                if (square < blackKing)
                                {
                                    Debug.Assert(blackQueensideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black queenside rooks detected {blackQueensideRook}");
                                    castlingRights |= (byte)CastlingRights.BQ;
                                    blackQueensideRook = square;
                                }
                                else if (square > blackKing)
                                {
                                    Debug.Assert(blackKingsideRook == CastlingData.DefaultValues, $"Invalid castle character {ch}", $"Multiple black kingside rooks detected {blackKingsideRook}");
                                    castlingRights |= (byte)CastlingRights.BK;
                                    blackKingsideRook = square;
                                }
                                else
                                {
                                    throw new LynxException($"Invalid castle character {ch}: it matches black king square ({square})");
                                }

                                break;
                            }

                            throw new LynxException($"Unrecognized castle character: {ch}");
                        }
                }
            }

            return new CastlingData(whiteKingsideRook, whiteQueensideRook, blackKingsideRook, blackQueensideRook);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, BitBoard[] PieceBitBoards, Side side)
        {
            bool success = true;

            if (TryParseEnPassantSquare(enPassantSpan, out var enPassant))
            {
                if (enPassant != BoardSquare.noSquare)
                {
                    // Check that there's an actual pawn to be captured
                    var pawnOffset = ((int)side << 4) - 8; // side == Side.White ? +8 : -8
                    var pawnSquare = (int)enPassant + pawnOffset;

                    var pawnBitBoard = PieceBitBoards[(int)Piece.P + Utils.PieceOffset(Utils.OppositeSide(side))];

                    if (!pawnBitBoard.GetBit(pawnSquare))
                    {
                        success = false;
                        enPassant = BoardSquare.noSquare;
                        _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
                    }
                }
            }
            else if (enPassantSpan.Length != 1 || enPassantSpan[0] != '-')
            {
                success = false;
                enPassant = BoardSquare.noSquare;
                _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
            }

            return (enPassant, success);

            /// <summary>
            /// Fast alternative to Enum.TryParse
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool TryParseEnPassantSquare(ReadOnlySpan<char> enPassantSpan, out BoardSquare square)
            {
                if (enPassantSpan.Length == 1 && enPassantSpan[0] == '-')
                {
                    square = BoardSquare.noSquare;
                    return true;
                }

                if (enPassantSpan.Length != 2)
                {
                    square = BoardSquare.noSquare;
                    return false;
                }

                // Normalize to lowercase without branching
                // https://blog.cloudflare.com/the-oldest-trick-in-the-ascii-book/
                // Lowercase ASCII is uppercase ASCII + 0x20
                var fileChar = (char)(enPassantSpan[0] | 0x20);
                int file = fileChar - 'a'; // 0-7
                int rank = enPassantSpan[1] - '0';  // 1-8

                // Only ranks 3 and 6 are legal en passant target squares.
                if ((uint)file >= 8 || (rank != 3 && rank != 6))
                {
                    square = BoardSquare.noSquare;
                    _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());

                    return false;
                }

                square = (BoardSquare)BitBoardExtensions.SquareIndex(8 - rank, file);
                return true;
            }
        }
    }
}

#pragma warning restore S112, S6667 // General or reserved exceptions should never be thrown
