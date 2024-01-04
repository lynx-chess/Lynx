/*
 * No clear conclusions
 *
 *  BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1237 (20H2/October2020Update)
 *  Intel Core i7-5500U CPU 2.40GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK=6.0.100-rc.1.21463.6
 *    [Host]     : .NET 6.0.0 (6.0.21.45113), X64 RyuJIT
 *    DefaultJob : .NET 6.0.0 (6.0.21.45113), X64 RyuJIT
 *
 *  |                                               Method |                  fen |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |  Gen 0 | Allocated |
 *  |----------------------------------------------------- |--------------------- |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|
 *  |                         Struct_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 22.07 us | 0.845 us | 2.411 us | 21.24 us |  1.00 |    0.00 | 4.9133 |     10 KB |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 23.47 us | 0.936 us | 2.685 us | 22.65 us |  1.08 |    0.18 | 5.5847 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 23.32 us | 0.891 us | 2.557 us | 22.52 us |  1.06 |    0.14 | 4.9133 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 22.61 us | 0.582 us | 1.623 us | 22.11 us |  1.04 |    0.12 | 5.5847 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 23.58 us | 0.652 us | 1.882 us | 23.39 us |  1.08 |    0.13 | 4.9744 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 27.12 us | 1.023 us | 2.952 us | 26.85 us |  1.24 |    0.20 | 5.6458 |     12 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 29.00 us | 1.114 us | 3.231 us | 27.96 us |  1.33 |    0.20 | 4.9744 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 28.08 us | 0.688 us | 1.847 us | 27.96 us |  1.29 |    0.16 | 5.6458 |     12 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 29.54 us | 1.124 us | 3.224 us | 28.57 us |  1.35 |    0.19 | 4.8828 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 28.90 us | 0.801 us | 2.259 us | 28.20 us |  1.33 |    0.18 | 5.5847 |     11 KB |
 *  |                                                      |                      |          |          |          |          |       |         |        |           |
 *  |                         Struct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 30.97 us | 1.183 us | 3.395 us | 29.80 us |  1.00 |    0.00 | 4.8218 |     10 KB |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 30.66 us | 1.001 us | 2.921 us | 29.59 us |  1.00 |    0.14 | 5.4626 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 25.40 us | 1.694 us | 4.994 us | 22.31 us |  0.83 |    0.17 | 4.8218 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 21.86 us | 0.401 us | 0.612 us | 21.85 us |  0.70 |    0.08 | 5.4626 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 21.71 us | 0.430 us | 0.906 us | 21.60 us |  0.70 |    0.08 | 4.8828 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 20.18 us | 0.385 us | 0.811 us | 20.23 us |  0.65 |    0.07 | 5.4932 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 19.90 us | 0.388 us | 0.626 us | 19.94 us |  0.63 |    0.07 | 4.8828 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 20.25 us | 0.403 us | 0.705 us | 20.20 us |  0.65 |    0.07 | 5.4932 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 19.92 us | 0.395 us | 0.947 us | 19.90 us |  0.64 |    0.08 | 4.8218 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 22.06 us | 0.436 us | 0.679 us | 22.08 us |  0.70 |    0.08 | 5.4626 |     11 KB |
 *  |                                                      |                      |          |          |          |          |       |         |        |           |
 *  |                         Struct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 24.47 us | 0.470 us | 0.482 us | 24.46 us |  1.00 |    0.00 | 4.8523 |     10 KB |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 24.74 us | 0.485 us | 0.726 us | 24.63 us |  1.00 |    0.04 | 5.4932 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 24.43 us | 0.474 us | 0.680 us | 24.52 us |  1.00 |    0.03 | 4.8523 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 24.70 us | 0.473 us | 0.563 us | 24.70 us |  1.01 |    0.03 | 5.4932 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 24.06 us | 0.474 us | 0.694 us | 24.04 us |  0.99 |    0.04 | 4.9133 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 24.18 us | 0.464 us | 0.476 us | 24.26 us |  0.99 |    0.03 | 5.5542 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 24.08 us | 0.470 us | 0.503 us | 24.13 us |  0.98 |    0.04 | 4.9133 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 24.10 us | 0.472 us | 0.631 us | 23.95 us |  0.99 |    0.04 | 5.5542 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 24.38 us | 0.471 us | 0.629 us | 24.29 us |  1.00 |    0.03 | 4.8523 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 24.40 us | 0.481 us | 0.705 us | 24.39 us |  1.00 |    0.03 | 5.4932 |     11 KB |
 *  |                                                      |                      |          |          |          |          |       |         |        |           |
 *  |                         Struct_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 21.22 us | 0.418 us | 0.600 us | 21.12 us |  1.00 |    0.00 | 4.7607 |     10 KB |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 22.43 us | 0.447 us | 0.669 us | 22.40 us |  1.06 |    0.04 | 5.4626 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 25.17 us | 1.345 us | 3.965 us | 23.39 us |  1.03 |    0.06 | 4.7913 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 21.76 us | 0.418 us | 0.558 us | 21.73 us |  1.03 |    0.04 | 5.4626 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 21.38 us | 0.420 us | 0.431 us | 21.35 us |  1.01 |    0.04 | 4.8523 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 22.08 us | 0.421 us | 0.352 us | 22.09 us |  1.04 |    0.03 | 5.4932 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 21.26 us | 0.411 us | 0.440 us | 21.38 us |  1.00 |    0.03 | 4.8523 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 21.64 us | 0.430 us | 0.617 us | 21.73 us |  1.02 |    0.04 | 5.4932 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 21.62 us | 0.423 us | 0.718 us | 21.77 us |  1.02 |    0.05 | 4.7913 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 22.15 us | 0.427 us | 0.475 us | 22.19 us |  1.04 |    0.04 | 5.4626 |     11 KB |
 *  |                                                      |                      |          |          |          |          |       |         |        |           |
 *  |                         Struct_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 15.97 us | 0.317 us | 0.619 us | 15.94 us |  1.00 |    0.00 | 4.3335 |      9 KB |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 15.89 us | 0.316 us | 0.519 us | 15.97 us |  1.00 |    0.05 | 4.9133 |     10 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 16.00 us | 0.319 us | 0.599 us | 16.03 us |  1.00 |    0.06 | 4.3335 |      9 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 15.50 us | 0.331 us | 0.950 us | 15.47 us |  1.01 |    0.06 | 4.9133 |     10 KB |
 *  |                          Class_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 14.66 us | 0.292 us | 0.488 us | 14.76 us |  0.92 |    0.06 | 4.3945 |      9 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 14.73 us | 0.292 us | 0.602 us | 14.73 us |  0.92 |    0.05 | 4.9896 |     10 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 14.65 us | 0.292 us | 0.689 us | 14.69 us |  0.92 |    0.06 | 4.3945 |      9 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 14.76 us | 0.291 us | 0.461 us | 14.70 us |  0.93 |    0.04 | 4.9896 |     10 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 14.72 us | 0.292 us | 0.577 us | 14.58 us |  0.92 |    0.05 | 4.3335 |      9 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 15.18 us | 0.301 us | 0.709 us | 15.10 us |  0.95 |    0.06 | 4.9133 |     10 KB |
 *
 *
 *  BenchmarkDotNet=v0.13.1, OS=Windows 10.0.17763.2183 (1809/October2018Update/Redstone5)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK=6.0.100-rc.1.21458.32
 *    [Host]     : .NET 6.0.0 (6.0.21.45113), X64 RyuJIT
 *    DefaultJob : .NET 6.0.0 (6.0.21.45113), X64 RyuJIT
 *
 *
 *  |                                               Method |                  fen |     Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
 *  |----------------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|
 *  |                         **Struct_FENCalculatedOnTheFly** | **r2q1r(...)- 0 1 [68]** | **15.04 μs** | **0.103 μs** | **0.097 μs** |  **1.00** |    **0.00** | **0.5341** |     **10 KB** |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 15.18 μs | 0.110 μs | 0.103 μs |  1.01 |    0.01 | 0.6104 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 15.28 μs | 0.106 μs | 0.099 μs |  1.02 |    0.01 | 0.5341 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 15.25 μs | 0.090 μs | 0.080 μs |  1.01 |    0.01 | 0.6104 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 15.44 μs | 0.127 μs | 0.119 μs |  1.03 |    0.01 | 0.5493 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 15.27 μs | 0.091 μs | 0.085 μs |  1.02 |    0.01 | 0.6104 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 14.97 μs | 0.113 μs | 0.106 μs |  1.00 |    0.01 | 0.5493 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 15.24 μs | 0.099 μs | 0.083 μs |  1.01 |    0.01 | 0.6104 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 15.13 μs | 0.113 μs | 0.106 μs |  1.01 |    0.01 | 0.5341 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 15.61 μs | 0.120 μs | 0.112 μs |  1.04 |    0.01 | 0.6104 |     11 KB |
 *  |                                                      |                      |          |          |          |       |         |        |           |
 *  |                         **Struct_FENCalculatedOnTheFly** | **r3k2r(...)- 0 1 [68]** | **15.29 μs** | **0.119 μs** | **0.111 μs** |  **1.00** |    **0.00** | **0.5341** |     **10 KB** |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 15.01 μs | 0.129 μs | 0.120 μs |  0.98 |    0.01 | 0.5798 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 15.17 μs | 0.134 μs | 0.126 μs |  0.99 |    0.01 | 0.5341 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 15.35 μs | 0.167 μs | 0.156 μs |  1.00 |    0.01 | 0.5798 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 15.24 μs | 0.080 μs | 0.071 μs |  1.00 |    0.01 | 0.5341 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 15.37 μs | 0.127 μs | 0.119 μs |  1.01 |    0.01 | 0.6104 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 15.12 μs | 0.106 μs | 0.099 μs |  0.99 |    0.01 | 0.5341 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 15.51 μs | 0.224 μs | 0.209 μs |  1.01 |    0.02 | 0.6104 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 15.37 μs | 0.103 μs | 0.097 μs |  1.01 |    0.01 | 0.5188 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 15.32 μs | 0.200 μs | 0.187 μs |  1.00 |    0.01 | 0.5951 |     11 KB |
 *  |                                                      |                      |          |          |          |       |         |        |           |
 *  |                         Struct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 17.01 μs | 0.122 μs | 0.108 μs |  1.00 |    0.00 | 0.5188 |     10 KB |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.30 μs | 0.178 μs | 0.158 μs |  1.02 |    0.01 | 0.5798 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 17.41 μs | 0.164 μs | 0.154 μs |  1.02 |    0.01 | 0.5188 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.13 μs | 0.147 μs | 0.138 μs |  1.01 |    0.01 | 0.5798 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 17.30 μs | 0.102 μs | 0.091 μs |  1.02 |    0.01 | 0.5188 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.28 μs | 0.095 μs | 0.089 μs |  1.02 |    0.01 | 0.6104 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 17.37 μs | 0.096 μs | 0.085 μs |  1.02 |    0.01 | 0.5188 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.45 μs | 0.141 μs | 0.132 μs |  1.03 |    0.01 | 0.6104 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 17.27 μs | 0.116 μs | 0.108 μs |  1.02 |    0.01 | 0.5188 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.31 μs | 0.148 μs | 0.139 μs |  1.02 |    0.01 | 0.5798 |     11 KB |
 *  |                                                      |                      |          |          |          |       |         |        |           |
 *  |                         **Struct_FENCalculatedOnTheFly** | **rnbqk(...)6 0 1 [67]** | **15.53 μs** | **0.107 μs** | **0.089 μs** |  **1.00** |    **0.00** | **0.5188** |     **10 KB** |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 15.79 μs | 0.186 μs | 0.174 μs |  1.02 |    0.01 | 0.5798 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 15.20 μs | 0.066 μs | 0.051 μs |  0.98 |    0.01 | 0.5188 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 15.49 μs | 0.119 μs | 0.111 μs |  1.00 |    0.01 | 0.5798 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 15.65 μs | 0.203 μs | 0.190 μs |  1.01 |    0.01 | 0.5188 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 15.73 μs | 0.197 μs | 0.184 μs |  1.01 |    0.01 | 0.6104 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 15.87 μs | 0.151 μs | 0.141 μs |  1.02 |    0.01 | 0.5188 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 15.99 μs | 0.139 μs | 0.123 μs |  1.03 |    0.01 | 0.6104 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 16.02 μs | 0.117 μs | 0.109 μs |  1.03 |    0.01 | 0.5188 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 15.87 μs | 0.141 μs | 0.132 μs |  1.02 |    0.01 | 0.5798 |     11 KB |
 *  |                                                      |                      |          |          |          |       |         |        |           |
 *  |                         **Struct_FENCalculatedOnTheFly** | **rnbqk(...)- 0 1 [56]** | **11.43 μs** | **0.211 μs** | **0.197 μs** |  **1.00** |    **0.00** | **0.4730** |      **9 KB** |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 10.93 μs | 0.084 μs | 0.075 μs |  0.96 |    0.02 | 0.5341 |     10 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 11.19 μs | 0.083 μs | 0.077 μs |  0.98 |    0.02 | 0.4730 |      9 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 11.61 μs | 0.228 μs | 0.253 μs |  1.02 |    0.03 | 0.5341 |     10 KB |
 *  |                          Class_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 11.58 μs | 0.230 μs | 0.299 μs |  1.02 |    0.04 | 0.4730 |      9 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 11.47 μs | 0.147 μs | 0.115 μs |  1.01 |    0.02 | 0.5493 |     10 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 11.27 μs | 0.058 μs | 0.052 μs |  0.99 |    0.02 | 0.4730 |      9 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 12.12 μs | 0.233 μs | 0.229 μs |  1.06 |    0.03 | 0.5493 |     10 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 11.79 μs | 0.125 μs | 0.117 μs |  1.03 |    0.02 | 0.4730 |      9 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 11.28 μs | 0.079 μs | 0.074 μs |  0.99 |    0.02 | 0.5341 |     10 KB |
 *
 *
 *  BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK=6.0.100-rc.1.21458.32
 *    [Host]     : .NET 6.0.0 (6.0.21.45113), X64 RyuJIT
 *    DefaultJob : .NET 6.0.0 (6.0.21.45113), X64 RyuJIT
 *
 *
 *  ```
 *  |                                               Method |                  fen |     Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
 *  |----------------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|
 *  |                         **Struct_FENCalculatedOnTheFly** | **r2q1r(...)- 0 1 [68]** | **16.79 μs** | **0.225 μs** | **0.211 μs** |  **1.00** |    **0.00** | **0.5188** |     **10 KB** |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 16.88 μs | 0.186 μs | 0.165 μs |  1.01 |    0.02 | 0.6104 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 16.79 μs | 0.163 μs | 0.152 μs |  1.00 |    0.02 | 0.5188 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 17.03 μs | 0.116 μs | 0.103 μs |  1.02 |    0.01 | 0.6104 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 17.09 μs | 0.135 μs | 0.119 μs |  1.02 |    0.02 | 0.5493 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 17.12 μs | 0.091 μs | 0.076 μs |  1.02 |    0.01 | 0.6104 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 17.22 μs | 0.094 μs | 0.088 μs |  1.03 |    0.01 | 0.5493 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 17.34 μs | 0.116 μs | 0.097 μs |  1.03 |    0.01 | 0.6104 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r2q1r(...)- 0 1 [68] | 16.97 μs | 0.198 μs | 0.186 μs |  1.01 |    0.02 | 0.5188 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r2q1r(...)- 0 1 [68] | 17.36 μs | 0.095 μs | 0.084 μs |  1.03 |    0.01 | 0.6104 |     11 KB |
 *  |                                                      |                      |          |          |          |       |         |        |           |
 *  |                         **Struct_FENCalculatedOnTheFly** | **r3k2r(...)- 0 1 [68]** | **17.29 μs** | **0.202 μs** | **0.189 μs** |  **1.00** |    **0.00** | **0.5188** |     **10 KB** |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.40 μs | 0.323 μs | 0.302 μs |  1.01 |    0.02 | 0.5798 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 17.27 μs | 0.188 μs | 0.176 μs |  1.00 |    0.01 | 0.5188 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.57 μs | 0.176 μs | 0.164 μs |  1.02 |    0.02 | 0.5798 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 16.95 μs | 0.255 μs | 0.239 μs |  0.98 |    0.01 | 0.5188 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 16.94 μs | 0.082 μs | 0.072 μs |  0.98 |    0.01 | 0.6104 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 16.90 μs | 0.223 μs | 0.209 μs |  0.98 |    0.01 | 0.5188 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.01 μs | 0.168 μs | 0.157 μs |  0.98 |    0.01 | 0.6104 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 16.78 μs | 0.141 μs | 0.125 μs |  0.97 |    0.01 | 0.5188 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 17.04 μs | 0.116 μs | 0.108 μs |  0.99 |    0.01 | 0.5798 |     11 KB |
 *  |                                                      |                      |          |          |          |       |         |        |           |
 *  |                         Struct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 18.65 μs | 0.114 μs | 0.096 μs |  1.00 |    0.00 | 0.5188 |     10 KB |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 18.82 μs | 0.116 μs | 0.103 μs |  1.01 |    0.01 | 0.5798 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 18.74 μs | 0.088 μs | 0.078 μs |  1.00 |    0.01 | 0.5188 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 18.86 μs | 0.074 μs | 0.065 μs |  1.01 |    0.00 | 0.5798 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 18.70 μs | 0.109 μs | 0.091 μs |  1.00 |    0.01 | 0.5188 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 18.80 μs | 0.077 μs | 0.060 μs |  1.01 |    0.01 | 0.6104 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 18.61 μs | 0.096 μs | 0.090 μs |  1.00 |    0.01 | 0.5188 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 19.27 μs | 0.177 μs | 0.165 μs |  1.03 |    0.01 | 0.6104 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | r3k2r(...)- 0 1 [68] | 18.66 μs | 0.072 μs | 0.060 μs |  1.00 |    0.01 | 0.5188 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | r3k2r(...)- 0 1 [68] | 18.85 μs | 0.126 μs | 0.111 μs |  1.01 |    0.01 | 0.5798 |     11 KB |
 *  |                                                      |                      |          |          |          |       |         |        |           |
 *  |                         **Struct_FENCalculatedOnTheFly** | **rnbqk(...)6 0 1 [67]** | **17.05 μs** | **0.108 μs** | **0.101 μs** |  **1.00** |    **0.00** | **0.5188** |     **10 KB** |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 17.22 μs | 0.197 μs | 0.175 μs |  1.01 |    0.01 | 0.5798 |     11 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 17.05 μs | 0.103 μs | 0.096 μs |  1.00 |    0.01 | 0.5188 |     10 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 17.00 μs | 0.200 μs | 0.187 μs |  1.00 |    0.01 | 0.5798 |     11 KB |
 *  |                          Class_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 16.96 μs | 0.084 μs | 0.079 μs |  0.99 |    0.01 | 0.5188 |     10 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 17.33 μs | 0.249 μs | 0.233 μs |  1.02 |    0.02 | 0.6104 |     11 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 17.56 μs | 0.314 μs | 0.293 μs |  1.03 |    0.02 | 0.5188 |     10 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 17.42 μs | 0.335 μs | 0.329 μs |  1.02 |    0.02 | 0.6104 |     11 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | rnbqk(...)6 0 1 [67] | 16.97 μs | 0.193 μs | 0.180 μs |  1.00 |    0.01 | 0.5188 |     10 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)6 0 1 [67] | 17.24 μs | 0.183 μs | 0.163 μs |  1.01 |    0.01 | 0.5798 |     11 KB |
 *  |                                                      |                      |          |          |          |       |         |        |           |
 *  |                         **Struct_FENCalculatedOnTheFly** | **rnbqk(...)- 0 1 [56]** | **12.45 μs** | **0.226 μs** | **0.211 μs** |  **1.00** |    **0.00** | **0.4730** |      **9 KB** |
 *  |         Struct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 12.43 μs | 0.141 μs | 0.132 μs |  1.00 |    0.02 | 0.5341 |     10 KB |
 *  |                 ReadonlyStruct_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 12.51 μs | 0.186 μs | 0.174 μs |  1.01 |    0.01 | 0.4730 |      9 KB |
 *  | ReadonlyStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 12.59 μs | 0.108 μs | 0.101 μs |  1.01 |    0.02 | 0.5341 |     10 KB |
 *  |                          Class_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 12.42 μs | 0.188 μs | 0.176 μs |  1.00 |    0.01 | 0.4730 |      9 KB |
 *  |          Class_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 12.53 μs | 0.148 μs | 0.131 μs |  1.01 |    0.02 | 0.5493 |     10 KB |
 *  |                    RecordClass_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 12.46 μs | 0.134 μs | 0.126 μs |  1.00 |    0.02 | 0.4730 |      9 KB |
 *  |    RecordClass_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 12.44 μs | 0.138 μs | 0.129 μs |  1.00 |    0.02 | 0.5493 |     10 KB |
 *  |                   RecordStruct_FENCalculatedOnTheFly | rnbqk(...)- 0 1 [56] | 12.36 μs | 0.240 μs | 0.236 μs |  0.99 |    0.01 | 0.4730 |      9 KB |
 *  |   RecordStruct_FENCalculatedWithinTheMoveConstructor | rnbqk(...)- 0 1 [56] | 12.43 μs | 0.119 μs | 0.112 μs |  1.00 |    0.02 | 0.5341 |     10 KB |
 *
*/

#pragma warning disable RCS1163, IDE0060 // Unused parameter.

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Text;

namespace Lynx.Benchmark;

public class FENGeneration_Benchmark : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public string Struct_FENCalculatedOnTheFly(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new StructCustomPosition(fen);
        var newPosition = new StructCustomPosition(position, moves.First());
        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string Struct_FENCalculatedWithinTheMoveConstructor(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new StructCustomPosition(fen);
        var newPosition = new StructCustomPosition(position, moves.First(), default);

        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string ReadonlyStruct_FENCalculatedOnTheFly(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new ReadonlyStructCustomPosition(fen);
        var newPosition = new ReadonlyStructCustomPosition(position, moves.First());

        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string ReadonlyStruct_FENCalculatedWithinTheMoveConstructor(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new ReadonlyStructCustomPosition(fen);
        var newPosition = new ReadonlyStructCustomPosition(position, moves.First(), default);

        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string Class_FENCalculatedOnTheFly(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new ClassCustomPosition(fen);
        var newPosition = new ClassCustomPosition(position, moves.First());

        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string Class_FENCalculatedWithinTheMoveConstructor(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new ClassCustomPosition(fen);
        var newPosition = new ClassCustomPosition(position, moves.First(), default);

        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string RecordClass_FENCalculatedOnTheFly(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new RecordClassCustomPosition(fen);
        var newPosition = new RecordClassCustomPosition(position, moves.First());

        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string RecordClass_FENCalculatedWithinTheMoveConstructor(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new RecordClassCustomPosition(fen);
        var newPosition = new RecordClassCustomPosition(position, moves.First(), default);

        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string RecordStruct_FENCalculatedOnTheFly(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new RecordStructCustomPosition(fen);
        var newPosition = new RecordStructCustomPosition(position, moves.First());

        return newPosition.FEN;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public string RecordStruct_FENCalculatedWithinTheMoveConstructor(string fen)
    {
        var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

        var position = new RecordStructCustomPosition(fen);
        var newPosition = new RecordStructCustomPosition(position, moves.First(), default);

        return newPosition.FEN;
    }

    public static IEnumerable<string> Data => new[] {
            //Constants.EmptyBoardFEN,
            Constants.InitialPositionFEN,
            Constants.TrickyTestPositionFEN,
            "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1",
            "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1",
            "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 1"
        };
}

internal struct StructCustomPosition
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private string? _fen;

    public string FEN
    {
        get => _fen ??= CalculateFEN();
        init => _fen = value;
    }

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards { get; }

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards { get; }

    public Side Side { get; }

    public BoardSquare EnPassant { get; }

    public int Castle { get; }

    public StructCustomPosition(string fen)
    {
        _fen = fen;

        var parsedFEN = FENParser.ParseFEN(fen);

        PieceBitBoards = parsedFEN.PieceBitBoards;
        OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
        Side = parsedFEN.Side;
        Castle = parsedFEN.Castle;
        EnPassant = parsedFEN.EnPassant;
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    /// <param name="position"></param>
    public StructCustomPosition(StructCustomPosition position)
    {
        _fen = position.FEN;

        PieceBitBoards = new BitBoard[12];
        Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

        OccupancyBitBoards = new BitBoard[3];
        Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

        Side = position.Side;
        Castle = position.Castle;
        EnPassant = position.EnPassant;
    }

    public StructCustomPosition(StructCustomPosition position, Move move) : this(position)
    {
        _fen = null;
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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];
    }

    /// <summary>
    /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <param name="calculateFen"></param>
    public StructCustomPosition(StructCustomPosition position, Move move, bool calculateFen) : this(position)

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

        StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

        EnPassant = BoardSquare.noSquare;

        PieceBitBoards[piece].PopBit(sourceSquare);
        OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

        PieceBitBoards[newPiece].SetBit(targetSquare);
        OccupancyBitBoards[(int)Side].SetBit(targetSquare);

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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        _fen = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
    }

    private readonly string CalculateFEN()
    {
        var sb = new StringBuilder(100);

        var squaresPerRow = 0;

        int squaresWithoutPiece = 0;
        int lengthBeforeSlash = sb.Length;
        for (int square = 0; square < 64; ++square)
        {
            int foundPiece = -1;
            for (var pieceBoardIndex = 0; pieceBoardIndex < 12; ++pieceBoardIndex)
            {
                if (PieceBitBoards[pieceBoardIndex].GetBit(square))
                {
                    foundPiece = pieceBoardIndex;
                    break;
                }
            }

            if (foundPiece != -1)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                sb.Append(Constants.AsciiPieces[foundPiece]);
            }
            else
            {
                ++squaresWithoutPiece;
            }

            squaresPerRow = (squaresPerRow + 1) % 8;
            if (squaresPerRow == 0)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                if (square != 63)
                {
                    if (sb.Length == lengthBeforeSlash)
                    {
                        sb.Append('8');
                    }
                    sb.Append('/');
                    lengthBeforeSlash = sb.Length;
                    squaresWithoutPiece = 0;
                }
            }
        }

        sb.Append(' ');
        sb.Append(Side == Side.White ? 'w' : 'b');

        sb.Append(' ');
        var length = sb.Length;

        if ((Castle & (int)CastlingRights.WK) != default)
        {
            sb.Append('K');
        }
        if ((Castle & (int)CastlingRights.WQ) != default)
        {
            sb.Append('Q');
        }
        if ((Castle & (int)CastlingRights.BK) != default)
        {
            sb.Append('k');
        }
        if ((Castle & (int)CastlingRights.BQ) != default)
        {
            sb.Append('q');
        }

        if (sb.Length == length)
        {
            sb.Append('-');
        }

        sb.Append(' ');

        sb.Append(EnPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)EnPassant]);

        sb.Append(" 0 1");

        return sb.ToString();
    }
}

internal readonly struct ReadonlyStructCustomPosition
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public string FEN { get; private init; }

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards { get; }

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards { get; }

    public Side Side { get; }

    public BoardSquare EnPassant { get; }

    public int Castle { get; }

    public ReadonlyStructCustomPosition(string fen)
    {
        FEN = fen;

        var parsedFEN = FENParser.ParseFEN(fen);

        PieceBitBoards = parsedFEN.PieceBitBoards;
        OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
        Side = parsedFEN.Side;
        Castle = parsedFEN.Castle;
        EnPassant = parsedFEN.EnPassant;
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    /// <param name="position"></param>
    public ReadonlyStructCustomPosition(ReadonlyStructCustomPosition position)
    {
        FEN = position.FEN;

        PieceBitBoards = new BitBoard[12];
        Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

        OccupancyBitBoards = new BitBoard[3];
        Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

        Side = position.Side;
        Castle = position.Castle;
        EnPassant = position.EnPassant;
    }

    public ReadonlyStructCustomPosition(ReadonlyStructCustomPosition position, Move move) : this(position)
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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        FEN = CalculateFEN();
    }

    /// <summary>
    /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <param name="calculateFen"></param>
    public ReadonlyStructCustomPosition(ReadonlyStructCustomPosition position, Move move, bool calculateFen) : this(position)
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

        StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

        EnPassant = BoardSquare.noSquare;

        PieceBitBoards[piece].PopBit(sourceSquare);
        OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

        PieceBitBoards[newPiece].SetBit(targetSquare);
        OccupancyBitBoards[(int)Side].SetBit(targetSquare);

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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        FEN = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
    }

    private readonly string CalculateFEN()
    {
        var sb = new StringBuilder(100);

        var squaresPerRow = 0;

        int squaresWithoutPiece = 0;
        int lengthBeforeSlash = sb.Length;
        for (int square = 0; square < 64; ++square)
        {
            int foundPiece = -1;
            for (var pieceBoardIndex = 0; pieceBoardIndex < 12; ++pieceBoardIndex)
            {
                if (PieceBitBoards[pieceBoardIndex].GetBit(square))
                {
                    foundPiece = pieceBoardIndex;
                    break;
                }
            }

            if (foundPiece != -1)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                sb.Append(Constants.AsciiPieces[foundPiece]);
            }
            else
            {
                ++squaresWithoutPiece;
            }

            squaresPerRow = (squaresPerRow + 1) % 8;
            if (squaresPerRow == 0)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                if (square != 63)
                {
                    if (sb.Length == lengthBeforeSlash)
                    {
                        sb.Append('8');
                    }
                    sb.Append('/');
                    lengthBeforeSlash = sb.Length;
                    squaresWithoutPiece = 0;
                }
            }
        }

        sb.Append(' ');
        sb.Append(Side == Side.White ? 'w' : 'b');

        sb.Append(' ');
        var length = sb.Length;

        if ((Castle & (int)CastlingRights.WK) != default)
        {
            sb.Append('K');
        }
        if ((Castle & (int)CastlingRights.WQ) != default)
        {
            sb.Append('Q');
        }
        if ((Castle & (int)CastlingRights.BK) != default)
        {
            sb.Append('k');
        }
        if ((Castle & (int)CastlingRights.BQ) != default)
        {
            sb.Append('q');
        }

        if (sb.Length == length)
        {
            sb.Append('-');
        }

        sb.Append(' ');

        sb.Append(EnPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)EnPassant]);

        sb.Append(" 0 1");

        return sb.ToString();
    }
}

internal class ClassCustomPosition
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private string? _fen;

    public string FEN
    {
        get => _fen ??= CalculateFEN();
        init => _fen = value;
    }

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards { get; }

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards { get; }

    public Side Side { get; }

    public BoardSquare EnPassant { get; }

    public int Castle { get; }

    public ClassCustomPosition(string fen)
    {
        _fen = fen;

        var parsedFEN = FENParser.ParseFEN(fen);

        PieceBitBoards = parsedFEN.PieceBitBoards;
        OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
        Side = parsedFEN.Side;
        Castle = parsedFEN.Castle;
        EnPassant = parsedFEN.EnPassant;
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    /// <param name="position"></param>
    public ClassCustomPosition(ClassCustomPosition position)
    {
        _fen = position.FEN;

        PieceBitBoards = new BitBoard[12];
        Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

        OccupancyBitBoards = new BitBoard[3];
        Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

        Side = position.Side;
        Castle = position.Castle;
        EnPassant = position.EnPassant;
    }

    public ClassCustomPosition(ClassCustomPosition position, Move move) : this(position)
    {
        _fen = null;
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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];
    }

    /// <summary>
    /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <param name="calculateFen"></param>
    public ClassCustomPosition(ClassCustomPosition position, Move move, bool calculateFen) : this(position)
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

        StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

        EnPassant = BoardSquare.noSquare;

        PieceBitBoards[piece].PopBit(sourceSquare);
        OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

        PieceBitBoards[newPiece].SetBit(targetSquare);
        OccupancyBitBoards[(int)Side].SetBit(targetSquare);

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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        _fen = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
    }

    private string CalculateFEN()
    {
        var sb = new StringBuilder(100);

        var squaresPerRow = 0;

        int squaresWithoutPiece = 0;
        int lengthBeforeSlash = sb.Length;
        for (int square = 0; square < 64; ++square)
        {
            int foundPiece = -1;
            for (var pieceBoardIndex = 0; pieceBoardIndex < 12; ++pieceBoardIndex)
            {
                if (PieceBitBoards[pieceBoardIndex].GetBit(square))
                {
                    foundPiece = pieceBoardIndex;
                    break;
                }
            }

            if (foundPiece != -1)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                sb.Append(Constants.AsciiPieces[foundPiece]);
            }
            else
            {
                ++squaresWithoutPiece;
            }

            squaresPerRow = (squaresPerRow + 1) % 8;
            if (squaresPerRow == 0)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                if (square != 63)
                {
                    if (sb.Length == lengthBeforeSlash)
                    {
                        sb.Append('8');
                    }
                    sb.Append('/');
                    lengthBeforeSlash = sb.Length;
                    squaresWithoutPiece = 0;
                }
            }
        }

        sb.Append(' ');
        sb.Append(Side == Side.White ? 'w' : 'b');

        sb.Append(' ');
        var length = sb.Length;

        if ((Castle & (int)CastlingRights.WK) != default)
        {
            sb.Append('K');
        }
        if ((Castle & (int)CastlingRights.WQ) != default)
        {
            sb.Append('Q');
        }
        if ((Castle & (int)CastlingRights.BK) != default)
        {
            sb.Append('k');
        }
        if ((Castle & (int)CastlingRights.BQ) != default)
        {
            sb.Append('q');
        }

        if (sb.Length == length)
        {
            sb.Append('-');
        }

        sb.Append(' ');

        sb.Append(EnPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)EnPassant]);

        sb.Append(" 0 1");

        return sb.ToString();
    }
}

internal record class RecordClassCustomPosition
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private string? _fen;

    public string FEN
    {
        get => _fen ??= CalculateFEN();
        init => _fen = value;
    }

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards { get; }

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards { get; }

    public Side Side { get; }

    public BoardSquare EnPassant { get; }

    public int Castle { get; }

    public RecordClassCustomPosition(string fen)
    {
        _fen = fen;

        var parsedFEN = FENParser.ParseFEN(fen);

        PieceBitBoards = parsedFEN.PieceBitBoards;
        OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
        Side = parsedFEN.Side;
        Castle = parsedFEN.Castle;
        EnPassant = parsedFEN.EnPassant;
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    /// <param name="position"></param>
    public RecordClassCustomPosition(RecordClassCustomPosition position)
    {
        _fen = position.FEN;

        PieceBitBoards = new BitBoard[12];
        Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

        OccupancyBitBoards = new BitBoard[3];
        Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

        Side = position.Side;
        Castle = position.Castle;
        EnPassant = position.EnPassant;
    }

    public RecordClassCustomPosition(RecordClassCustomPosition position, Move move) : this(position)
    {
        _fen = null;
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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];
    }

    /// <summary>
    /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <param name="calculateFen"></param>
    public RecordClassCustomPosition(RecordClassCustomPosition position, Move move, bool calculateFen) : this(position)
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

        StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

        EnPassant = BoardSquare.noSquare;

        PieceBitBoards[piece].PopBit(sourceSquare);
        OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

        PieceBitBoards[newPiece].SetBit(targetSquare);
        OccupancyBitBoards[(int)Side].SetBit(targetSquare);

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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        _fen = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
    }

    private string CalculateFEN()
    {
        var sb = new StringBuilder(100);

        var squaresPerRow = 0;

        int squaresWithoutPiece = 0;
        int lengthBeforeSlash = sb.Length;
        for (int square = 0; square < 64; ++square)
        {
            int foundPiece = -1;
            for (var pieceBoardIndex = 0; pieceBoardIndex < 12; ++pieceBoardIndex)
            {
                if (PieceBitBoards[pieceBoardIndex].GetBit(square))
                {
                    foundPiece = pieceBoardIndex;
                    break;
                }
            }

            if (foundPiece != -1)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                sb.Append(Constants.AsciiPieces[foundPiece]);
            }
            else
            {
                ++squaresWithoutPiece;
            }

            squaresPerRow = (squaresPerRow + 1) % 8;
            if (squaresPerRow == 0)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                if (square != 63)
                {
                    if (sb.Length == lengthBeforeSlash)
                    {
                        sb.Append('8');
                    }
                    sb.Append('/');
                    lengthBeforeSlash = sb.Length;
                    squaresWithoutPiece = 0;
                }
            }
        }

        sb.Append(' ');
        sb.Append(Side == Side.White ? 'w' : 'b');

        sb.Append(' ');
        var length = sb.Length;

        if ((Castle & (int)CastlingRights.WK) != default)
        {
            sb.Append('K');
        }
        if ((Castle & (int)CastlingRights.WQ) != default)
        {
            sb.Append('Q');
        }
        if ((Castle & (int)CastlingRights.BK) != default)
        {
            sb.Append('k');
        }
        if ((Castle & (int)CastlingRights.BQ) != default)
        {
            sb.Append('q');
        }

        if (sb.Length == length)
        {
            sb.Append('-');
        }

        sb.Append(' ');

        sb.Append(EnPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)EnPassant]);

        sb.Append(" 0 1");

        return sb.ToString();
    }
}

internal record struct RecordStructCustomPosition
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private string? _fen;

    public string FEN
    {
        get => _fen ??= CalculateFEN();
        init => _fen = value;
    }

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards { get; }

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards { get; }

    public Side Side { get; }

    public BoardSquare EnPassant { get; }

    public int Castle { get; }

    public RecordStructCustomPosition(string fen)
    {
        _fen = fen;

        var parsedFEN = FENParser.ParseFEN(fen);

        PieceBitBoards = parsedFEN.PieceBitBoards;
        OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
        Side = parsedFEN.Side;
        Castle = parsedFEN.Castle;
        EnPassant = parsedFEN.EnPassant;
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    /// <param name="position"></param>
    public RecordStructCustomPosition(RecordStructCustomPosition position)
    {
        _fen = position.FEN;

        PieceBitBoards = new BitBoard[12];
        Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

        OccupancyBitBoards = new BitBoard[3];
        Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

        Side = position.Side;
        Castle = position.Castle;
        EnPassant = position.EnPassant;
    }

    public RecordStructCustomPosition(RecordStructCustomPosition position, Move move) : this(position)
    {
        _fen = null;
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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];
    }

    /// <summary>
    /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
    /// <param name="calculateFen"></param>
    public RecordStructCustomPosition(RecordStructCustomPosition position, Move move, bool calculateFen) : this(position)
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

        StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

        EnPassant = BoardSquare.noSquare;

        PieceBitBoards[piece].PopBit(sourceSquare);
        OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

        PieceBitBoards[newPiece].SetBit(targetSquare);
        OccupancyBitBoards[(int)Side].SetBit(targetSquare);

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
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        _fen = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
    }

    private readonly string CalculateFEN()
    {
        var sb = new StringBuilder(100);

        var squaresPerRow = 0;

        int squaresWithoutPiece = 0;
        int lengthBeforeSlash = sb.Length;
        for (int square = 0; square < 64; ++square)
        {
            int foundPiece = -1;
            for (var pieceBoardIndex = 0; pieceBoardIndex < 12; ++pieceBoardIndex)
            {
                if (PieceBitBoards[pieceBoardIndex].GetBit(square))
                {
                    foundPiece = pieceBoardIndex;
                    break;
                }
            }

            if (foundPiece != -1)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                sb.Append(Constants.AsciiPieces[foundPiece]);
            }
            else
            {
                ++squaresWithoutPiece;
            }

            squaresPerRow = (squaresPerRow + 1) % 8;
            if (squaresPerRow == 0)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                if (square != 63)
                {
                    if (sb.Length == lengthBeforeSlash)
                    {
                        sb.Append('8');
                    }
                    sb.Append('/');
                    lengthBeforeSlash = sb.Length;
                    squaresWithoutPiece = 0;
                }
            }
        }

        sb.Append(' ');
        sb.Append(Side == Side.White ? 'w' : 'b');

        sb.Append(' ');
        var length = sb.Length;

        if ((Castle & (int)CastlingRights.WK) != default)
        {
            sb.Append('K');
        }
        if ((Castle & (int)CastlingRights.WQ) != default)
        {
            sb.Append('Q');
        }
        if ((Castle & (int)CastlingRights.BK) != default)
        {
            sb.Append('k');
        }
        if ((Castle & (int)CastlingRights.BQ) != default)
        {
            sb.Append('q');
        }

        if (sb.Length == length)
        {
            sb.Append('-');
        }

        sb.Append(' ');

        sb.Append(EnPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)EnPassant]);

        sb.Append(" 0 1");

        return sb.ToString();
    }
}

internal static class FENHelpers
{
    public static StringBuilder UpdateFirstPartOfFEN(StructCustomPosition position, int sourceSquare, int targetSquare, int piece)
    {
        var fenSegments = position.FEN.Split('/');

        int sourceSegmentIndex = sourceSquare / 8;
        int sourceFile = sourceSquare % 8;
        var expandedSourceSegment = new StringBuilder();

        foreach (var item in fenSegments[sourceSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedSourceSegment.Append('1', item - '0');
            }
            else
            {
                expandedSourceSegment.Append(item);
            }
        }

        expandedSourceSegment[sourceFile] = '1';

        var sourceSegment = new StringBuilder(8);
        int ones = 0;
        foreach (var item in expandedSourceSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    sourceSegment.Append(ones);
                    ones = 0;

                    sourceSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            sourceSegment.Append(ones);
        }

        fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

        int targetSegmentIndex = targetSquare / 8;
        int targetFile = targetSquare % 8;
        var expandedTargetSegment = new StringBuilder(8);
        foreach (var item in fenSegments[targetSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedTargetSegment.Append('1', item - '0');
            }
            else
            {
                expandedTargetSegment.Append(item);
            }
        }

        expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

        var targetSegment = new StringBuilder();
        ones = 0;
        foreach (var item in expandedTargetSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    targetSegment.Append(ones);
                    ones = 0;

                    targetSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            targetSegment.Append(ones);
        }

        fenSegments[targetSegmentIndex] = targetSegment.ToString();

        fenSegments[7] = fenSegments[7].Split(' ')[0];

        var fenSb = new StringBuilder(string.Join('/', fenSegments));
        return fenSb;
    }

    public static StringBuilder UpdateFirstPartOfFEN(ReadonlyStructCustomPosition position, int sourceSquare, int targetSquare, int piece)
    {
        var fenSegments = position.FEN.Split('/');

        int sourceSegmentIndex = sourceSquare / 8;
        int sourceFile = sourceSquare % 8;
        var expandedSourceSegment = new StringBuilder();

        foreach (var item in fenSegments[sourceSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedSourceSegment.Append('1', item - '0');
            }
            else
            {
                expandedSourceSegment.Append(item);
            }
        }

        expandedSourceSegment[sourceFile] = '1';

        var sourceSegment = new StringBuilder(8);
        int ones = 0;
        foreach (var item in expandedSourceSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    sourceSegment.Append(ones);
                    ones = 0;

                    sourceSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            sourceSegment.Append(ones);
        }

        fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

        int targetSegmentIndex = targetSquare / 8;
        int targetFile = targetSquare % 8;
        var expandedTargetSegment = new StringBuilder(8);
        foreach (var item in fenSegments[targetSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedTargetSegment.Append('1', item - '0');
            }
            else
            {
                expandedTargetSegment.Append(item);
            }
        }

        expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

        var targetSegment = new StringBuilder();
        ones = 0;
        foreach (var item in expandedTargetSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    targetSegment.Append(ones);
                    ones = 0;

                    targetSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            targetSegment.Append(ones);
        }

        fenSegments[targetSegmentIndex] = targetSegment.ToString();

        fenSegments[7] = fenSegments[7].Split(' ')[0];

        var fenSb = new StringBuilder(string.Join('/', fenSegments));
        return fenSb;
    }

    public static StringBuilder UpdateFirstPartOfFEN(ClassCustomPosition position, int sourceSquare, int targetSquare, int piece)
    {
        var fenSegments = position.FEN.Split('/');

        int sourceSegmentIndex = sourceSquare / 8;
        int sourceFile = sourceSquare % 8;
        var expandedSourceSegment = new StringBuilder();

        foreach (var item in fenSegments[sourceSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedSourceSegment.Append('1', item - '0');
            }
            else
            {
                expandedSourceSegment.Append(item);
            }
        }

        expandedSourceSegment[sourceFile] = '1';

        var sourceSegment = new StringBuilder(8);
        int ones = 0;
        foreach (var item in expandedSourceSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    sourceSegment.Append(ones);
                    ones = 0;

                    sourceSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            sourceSegment.Append(ones);
        }

        fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

        int targetSegmentIndex = targetSquare / 8;
        int targetFile = targetSquare % 8;
        var expandedTargetSegment = new StringBuilder(8);
        foreach (var item in fenSegments[targetSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedTargetSegment.Append('1', item - '0');
            }
            else
            {
                expandedTargetSegment.Append(item);
            }
        }

        expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

        var targetSegment = new StringBuilder();
        ones = 0;
        foreach (var item in expandedTargetSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    targetSegment.Append(ones);
                    ones = 0;

                    targetSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            targetSegment.Append(ones);
        }

        fenSegments[targetSegmentIndex] = targetSegment.ToString();

        fenSegments[7] = fenSegments[7].Split(' ')[0];

        var fenSb = new StringBuilder(string.Join('/', fenSegments));
        return fenSb;
    }

    public static StringBuilder UpdateFirstPartOfFEN(RecordClassCustomPosition position, int sourceSquare, int targetSquare, int piece)
    {
        var fenSegments = position.FEN.Split('/');

        int sourceSegmentIndex = sourceSquare / 8;
        int sourceFile = sourceSquare % 8;
        var expandedSourceSegment = new StringBuilder();

        foreach (var item in fenSegments[sourceSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedSourceSegment.Append('1', item - '0');
            }
            else
            {
                expandedSourceSegment.Append(item);
            }
        }

        expandedSourceSegment[sourceFile] = '1';

        var sourceSegment = new StringBuilder(8);
        int ones = 0;
        foreach (var item in expandedSourceSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    sourceSegment.Append(ones);
                    ones = 0;

                    sourceSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            sourceSegment.Append(ones);
        }

        fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

        int targetSegmentIndex = targetSquare / 8;
        int targetFile = targetSquare % 8;
        var expandedTargetSegment = new StringBuilder(8);
        foreach (var item in fenSegments[targetSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedTargetSegment.Append('1', item - '0');
            }
            else
            {
                expandedTargetSegment.Append(item);
            }
        }

        expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

        var targetSegment = new StringBuilder();
        ones = 0;
        foreach (var item in expandedTargetSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    targetSegment.Append(ones);
                    ones = 0;

                    targetSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            targetSegment.Append(ones);
        }

        fenSegments[targetSegmentIndex] = targetSegment.ToString();

        fenSegments[7] = fenSegments[7].Split(' ')[0];

        var fenSb = new StringBuilder(string.Join('/', fenSegments));
        return fenSb;
    }

    public static StringBuilder UpdateFirstPartOfFEN(RecordStructCustomPosition position, int sourceSquare, int targetSquare, int piece)
    {
        var fenSegments = position.FEN.Split('/');

        int sourceSegmentIndex = sourceSquare / 8;
        int sourceFile = sourceSquare % 8;
        var expandedSourceSegment = new StringBuilder();

        foreach (var item in fenSegments[sourceSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedSourceSegment.Append('1', item - '0');
            }
            else
            {
                expandedSourceSegment.Append(item);
            }
        }

        expandedSourceSegment[sourceFile] = '1';

        var sourceSegment = new StringBuilder(8);
        int ones = 0;
        foreach (var item in expandedSourceSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    sourceSegment.Append(ones);
                    ones = 0;

                    sourceSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            sourceSegment.Append(ones);
        }

        fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

        int targetSegmentIndex = targetSquare / 8;
        int targetFile = targetSquare % 8;
        var expandedTargetSegment = new StringBuilder(8);
        foreach (var item in fenSegments[targetSegmentIndex])
        {
            if (char.IsDigit(item))
            {
                expandedTargetSegment.Append('1', item - '0');
            }
            else
            {
                expandedTargetSegment.Append(item);
            }
        }

        expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

        var targetSegment = new StringBuilder();
        ones = 0;
        foreach (var item in expandedTargetSegment.ToString())
        {
            if (item == '1')
            {
                ++ones;
            }
            else
            {
                if (ones != 0)
                {
                    targetSegment.Append(ones);
                    ones = 0;

                    targetSegment.Append(item);
                }
            }
        }

        if (ones != 0)
        {
            targetSegment.Append(ones);
        }

        fenSegments[targetSegmentIndex] = targetSegment.ToString();

        fenSegments[7] = fenSegments[7].Split(' ')[0];

        var fenSb = new StringBuilder(string.Join('/', fenSegments));
        return fenSb;
    }

    /// <summary>
    /// Neeeds to be invoked with the updated <paramref name="side"/>, <paramref name="castle"/> and <paramref name="enPassant"/> properties for the new position
    /// </summary>
    /// <param name="fenSb">Result of <see cref="UpdateFirstPartOfFEN(Position, int, int, int)"/></param>
    /// <param name="side">Update <see cref="Position.Side"/></param>
    /// <param name="castle">Updated <see cref="Position.Castle"/></param>
    /// <param name="enPassant">Updated <see cref="Position.EnPassant"/></param>
    /// <returns></returns>
    public static string UpdateSecondPartOfFEN(StringBuilder fenSb, Side side, int castle, BoardSquare enPassant)
    {
        fenSb.Append(' ');
        fenSb.Append(side == Side.White ? 'w' : 'b');

        fenSb.Append(' ');
        var length = fenSb.Length;

        if ((castle & (int)CastlingRights.WK) != default)
        {
            fenSb.Append('K');
        }
        if ((castle & (int)CastlingRights.WQ) != default)
        {
            fenSb.Append('Q');
        }
        if ((castle & (int)CastlingRights.BK) != default)
        {
            fenSb.Append('k');
        }
        if ((castle & (int)CastlingRights.BQ) != default)
        {
            fenSb.Append('q');
        }

        if (fenSb.Length == length)
        {
            fenSb.Append('-');
        }

        fenSb.Append(' ');

        fenSb.Append(enPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)enPassant]);

        fenSb.Append(" 0 1");

        return fenSb.ToString();
    }
}

#pragma warning restore RCS1163, IDE0060 // Unused parameter.