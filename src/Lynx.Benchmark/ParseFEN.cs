/*
 * Span Split()
 *
 *  | enchmarkDotNet v0.13.8, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  | tel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
 *  | ET SDK 8.0.100-rc.1.23455.8
 *  | [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *  | DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *  |
 *  |
 *  | Method             | fen                  | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  | ------------------ |--------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 *  | ParseFEN_Original  | 8/k7/(...)- 0 1 [39] | 3.211 us | 0.0078 us | 0.0066 us |  1.00 | 0.1144 |    2960 B |        1.00 |
 *  | ParseFEN_Improved1 | 8/k7/(...)- 0 1 [39] | 2.860 us | 0.0076 us | 0.0071 us |  0.89 | 0.1068 |    2704 B |        0.91 |
 *  | ParseFEN_Base2     | 8/k7/(...)- 0 1 [39] | 2.934 us | 0.0074 us | 0.0062 us |  0.91 | 0.1068 |    2760 B |        0.93 |
 *  | ParseFEN_NoRegex   | 8/k7/(...)- 0 1 [39] | 1.358 us | 0.0004 us | 0.0003 us |  0.42 | 0.0191 |     480 B |        0.16 |
 *  |                    |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original  | r2q1r(...)- 0 9 [68] | 3.810 us | 0.0103 us | 0.0091 us |  1.00 | 0.1221 |    3160 B |        1.00 |
 *  | ParseFEN_Improved1 | r2q1r(...)- 0 9 [68] | 3.681 us | 0.0112 us | 0.0105 us |  0.97 | 0.1144 |    2904 B |        0.92 |
 *  | ParseFEN_Base2     | r2q1r(...)- 0 9 [68] | 3.682 us | 0.0173 us | 0.0162 us |  0.97 | 0.1183 |    3016 B |        0.95 |
 *  | ParseFEN_NoRegex   | r2q1r(...)- 0 9 [68] | 2.031 us | 0.0017 us | 0.0013 us |  0.53 | 0.0229 |     624 B |        0.20 |
 *  |                    |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 3.591 us | 0.0091 us | 0.0081 us |  1.00 | 0.1221 |    3080 B |        1.00 |
 *  | ParseFEN_Improved1 | r3k2r(...)- 0 1 [68] | 3.416 us | 0.0030 us | 0.0025 us |  0.95 | 0.1106 |    2816 B |        0.91 |
 *  | ParseFEN_Base2     | r3k2r(...)- 0 1 [68] | 3.746 us | 0.0053 us | 0.0044 us |  1.04 | 0.1144 |    2928 B |        0.95 |
 *  | ParseFEN_NoRegex   | r3k2r(...)- 0 1 [68] | 1.942 us | 0.0014 us | 0.0012 us |  0.54 | 0.0210 |     552 B |        0.18 |
 *  |                    |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 3.701 us | 0.0054 us | 0.0048 us |  1.00 | 0.1221 |    3080 B |        1.00 |
 *  | ParseFEN_Improved1 | r3k2r(...)- 0 1 [68] | 3.575 us | 0.0026 us | 0.0023 us |  0.97 | 0.1106 |    2816 B |        0.91 |
 *  | ParseFEN_Base2     | r3k2r(...)- 0 1 [68] | 3.728 us | 0.0100 us | 0.0093 us |  1.01 | 0.1144 |    2928 B |        0.95 |
 *  | ParseFEN_NoRegex   | r3k2r(...)- 0 1 [68] | 1.835 us | 0.0025 us | 0.0022 us |  0.50 | 0.0210 |     552 B |        0.18 |
 *  |                    |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)6 0 1 [67] | 3.747 us | 0.0036 us | 0.0032 us |  1.00 | 0.1221 |    3072 B |        1.00 |
 *  | ParseFEN_Improved1 | rnbqk(...)6 0 1 [67] | 3.341 us | 0.0078 us | 0.0069 us |  0.89 | 0.1106 |    2800 B |        0.91 |
 *  | ParseFEN_Base2     | rnbqk(...)6 0 1 [67] | 3.360 us | 0.0070 us | 0.0062 us |  0.90 | 0.1144 |    2904 B |        0.95 |
 *  | ParseFEN_NoRegex   | rnbqk(...)6 0 1 [67] | 1.622 us | 0.0005 us | 0.0004 us |  0.43 | 0.0210 |     528 B |        0.17 |
 *  |                    |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)- 0 1 [56] | 2.786 us | 0.0098 us | 0.0087 us |  1.00 | 0.1068 |    2760 B |        1.00 |
 *  | ParseFEN_Improved1 | rnbqk(...)- 0 1 [56] | 2.618 us | 0.0057 us | 0.0053 us |  0.94 | 0.0992 |    2496 B |        0.90 |
 *  | ParseFEN_Base2     | rnbqk(...)- 0 1 [56] | 2.671 us | 0.0071 us | 0.0059 us |  0.96 | 0.0992 |    2584 B |        0.94 |
 *  | ParseFEN_NoRegex   | rnbqk(...)- 0 1 [56] | 1.016 us | 0.0004 us | 0.0003 us |  0.36 | 0.0095 |     264 B |        0.10 |
 *  |                    |                      |          |           |           |       |        |           |             |
 *  | ParseFEN_Original  | rq2k2(...)- 0 1 [71] | 3.944 us | 0.0123 us | 0.0103 us |  1.00 | 0.1221 |    3168 B |        1.00 |
 *  | ParseFEN_Improved1 | rq2k2(...)- 0 1 [71] | 3.767 us | 0.0156 us | 0.0146 us |  0.96 | 0.1144 |    2904 B |        0.92 |
 *  | ParseFEN_Base2     | rq2k2(...)- 0 1 [71] | 3.756 us | 0.0077 us | 0.0060 us |  0.95 | 0.1183 |    3024 B |        0.95 |
 *  | ParseFEN_NoRegex   | rq2k2(...)- 0 1 [71] | 2.074 us | 0.0010 us | 0.0008 us |  0.53 | 0.0229 |     624 B |        0.20 |
 *
 *
 *  BenchmarkDotNet v0.13.8, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX2
 *
 *
 *  | Method             | fen                  | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------- |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | ParseFEN_Original  | 8/k7/(...)- 0 1 [39] | 3.588 us | 0.0695 us | 0.0714 us |  1.00 |    0.00 | 0.1564 |    2960 B |        1.00 |
 *  | ParseFEN_Improved1 | 8/k7/(...)- 0 1 [39] | 3.323 us | 0.0506 us | 0.0473 us |  0.92 |    0.02 | 0.1411 |    2704 B |        0.91 |
 *  | ParseFEN_Base2     | 8/k7/(...)- 0 1 [39] | 3.324 us | 0.0602 us | 0.0563 us |  0.92 |    0.02 | 0.1450 |    2760 B |        0.93 |
 *  | ParseFEN_NoRegex   | 8/k7/(...)- 0 1 [39] | 1.549 us | 0.0247 us | 0.0219 us |  0.43 |    0.01 | 0.0248 |     480 B |        0.16 |
 *  |                    |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original  | r2q1r(...)- 0 9 [68] | 4.393 us | 0.0727 us | 0.0680 us |  1.00 |    0.00 | 0.1678 |    3161 B |        1.00 |
 *  | ParseFEN_Improved1 | r2q1r(...)- 0 9 [68] | 4.103 us | 0.0803 us | 0.0860 us |  0.93 |    0.03 | 0.1526 |    2904 B |        0.92 |
 *  | ParseFEN_Base2     | r2q1r(...)- 0 9 [68] | 4.201 us | 0.0735 us | 0.0687 us |  0.96 |    0.02 | 0.1602 |    3016 B |        0.95 |
 *  | ParseFEN_NoRegex   | r2q1r(...)- 0 9 [68] | 2.193 us | 0.0149 us | 0.0125 us |  0.50 |    0.01 | 0.0305 |     624 B |        0.20 |
 *  |                    |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 4.145 us | 0.0803 us | 0.1125 us |  1.00 |    0.00 | 0.1602 |    3080 B |        1.00 |
 *  | ParseFEN_Improved1 | r3k2r(...)- 0 1 [68] | 3.893 us | 0.0685 us | 0.0608 us |  0.92 |    0.03 | 0.1450 |    2816 B |        0.91 |
 *  | ParseFEN_Base2     | r3k2r(...)- 0 1 [68] | 3.889 us | 0.0434 us | 0.0384 us |  0.92 |    0.02 | 0.1526 |    2928 B |        0.95 |
 *  | ParseFEN_NoRegex   | r3k2r(...)- 0 1 [68] | 1.980 us | 0.0261 us | 0.0231 us |  0.47 |    0.01 | 0.0267 |     552 B |        0.18 |
 *  |                    |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 3.912 us | 0.0437 us | 0.0341 us |  1.00 |    0.00 | 0.1602 |    3080 B |        1.00 |
 *  | ParseFEN_Improved1 | r3k2r(...)- 0 1 [68] | 3.879 us | 0.0289 us | 0.0256 us |  0.99 |    0.01 | 0.1450 |    2816 B |        0.91 |
 *  | ParseFEN_Base2     | r3k2r(...)- 0 1 [68] | 4.080 us | 0.0815 us | 0.1116 us |  1.03 |    0.03 | 0.1526 |    2928 B |        0.95 |
 *  | ParseFEN_NoRegex   | r3k2r(...)- 0 1 [68] | 1.962 us | 0.0328 us | 0.0307 us |  0.50 |    0.01 | 0.0267 |     552 B |        0.18 |
 *  |                    |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)6 0 1 [67] | 4.118 us | 0.0814 us | 0.0762 us |  1.00 |    0.00 | 0.1602 |    3072 B |        1.00 |
 *  | ParseFEN_Improved1 | rnbqk(...)6 0 1 [67] | 4.115 us | 0.0822 us | 0.0913 us |  1.01 |    0.03 | 0.1450 |    2800 B |        0.91 |
 *  | ParseFEN_Base2     | rnbqk(...)6 0 1 [67] | 4.021 us | 0.0675 us | 0.0527 us |  0.98 |    0.02 | 0.1526 |    2904 B |        0.95 |
 *  | ParseFEN_NoRegex   | rnbqk(...)6 0 1 [67] | 2.008 us | 0.0388 us | 0.0363 us |  0.49 |    0.01 | 0.0267 |     528 B |        0.17 |
 *  |                    |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)- 0 1 [56] | 3.108 us | 0.0505 us | 0.0447 us |  1.00 |    0.00 | 0.1450 |    2760 B |        1.00 |
 *  | ParseFEN_Improved1 | rnbqk(...)- 0 1 [56] | 2.828 us | 0.0540 us | 0.0530 us |  0.91 |    0.03 | 0.1335 |    2496 B |        0.90 |
 *  | ParseFEN_Base2     | rnbqk(...)- 0 1 [56] | 2.842 us | 0.0482 us | 0.0451 us |  0.91 |    0.02 | 0.1373 |    2584 B |        0.94 |
 *  | ParseFEN_NoRegex   | rnbqk(...)- 0 1 [56] | 1.137 us | 0.0180 us | 0.0168 us |  0.37 |    0.01 | 0.0134 |     264 B |        0.10 |
 *  |                    |                      |          |           |           |       |         |        |           |             |
 *  | ParseFEN_Original  | rq2k2(...)- 0 1 [71] | 4.488 us | 0.0874 us | 0.1041 us |  1.00 |    0.00 | 0.1678 |    3169 B |        1.00 |
 *  | ParseFEN_Improved1 | rq2k2(...)- 0 1 [71] | 4.145 us | 0.0524 us | 0.0438 us |  0.92 |    0.02 | 0.1526 |    2904 B |        0.92 |
 *  | ParseFEN_Base2     | rq2k2(...)- 0 1 [71] | 4.264 us | 0.0804 us | 0.0826 us |  0.95 |    0.03 | 0.1602 |    3024 B |        0.95 |
 *  | ParseFEN_NoRegex   | rq2k2(...)- 0 1 [71] | 2.253 us | 0.0343 us | 0.0321 us |  0.50 |    0.02 | 0.0305 |     624 B |        0.20 |
 *
 *
 *  BenchmarkDotNet v0.13.8, macOS Monterey 12.6.8 (21G725) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-rc.1.23455.8
 *    [Host]     : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.41904), X64 RyuJIT AVX
 *
 *
 *  | Method             | fen                  | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
 *  |------------------- |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
 *  | ParseFEN_Original  | 8/k7/(...)- 0 1 [39] | 3.105 us | 0.0258 us | 0.0229 us |  1.00 |    0.00 | 0.4692 |      - |    2961 B |        1.00 |
 *  | ParseFEN_Improved1 | 8/k7/(...)- 0 1 [39] | 2.756 us | 0.0196 us | 0.0183 us |  0.89 |    0.01 | 0.4311 |      - |    2705 B |        0.91 |
 *  | ParseFEN_Base2     | 8/k7/(...)- 0 1 [39] | 2.857 us | 0.0564 us | 0.1306 us |  0.88 |    0.04 | 0.4387 |      - |    2761 B |        0.93 |
 *  | ParseFEN_NoRegex   | 8/k7/(...)- 0 1 [39] | 1.330 us | 0.0225 us | 0.0363 us |  0.43 |    0.01 | 0.0763 |      - |     480 B |        0.16 |
 *  |                    |                      |          |           |           |       |         |        |        |           |             |
 *  | ParseFEN_Original  | r2q1r(...)- 0 9 [68] | 3.793 us | 0.0753 us | 0.1128 us |  1.00 |    0.00 | 0.5035 | 0.0038 |    3161 B |        1.00 |
 *  | ParseFEN_Improved1 | r2q1r(...)- 0 9 [68] | 3.712 us | 0.0944 us | 0.2783 us |  0.92 |    0.05 | 0.4616 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2     | r2q1r(...)- 0 9 [68] | 3.938 us | 0.0761 us | 0.1067 us |  1.04 |    0.04 | 0.4807 |      - |    3017 B |        0.95 |
 *  | ParseFEN_NoRegex   | r2q1r(...)- 0 9 [68] | 1.983 us | 0.0310 us | 0.0454 us |  0.52 |    0.02 | 0.0992 |      - |     624 B |        0.20 |
 *  |                    |                      |          |           |           |       |         |        |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 3.529 us | 0.0676 us | 0.0664 us |  1.00 |    0.00 | 0.4883 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1 | r3k2r(...)- 0 1 [68] | 3.398 us | 0.0434 us | 0.0406 us |  0.96 |    0.02 | 0.4463 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2     | r3k2r(...)- 0 1 [68] | 3.330 us | 0.0262 us | 0.0245 us |  0.94 |    0.02 | 0.4654 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex   | r3k2r(...)- 0 1 [68] | 1.836 us | 0.0181 us | 0.0170 us |  0.52 |    0.01 | 0.0877 |      - |     552 B |        0.18 |
 *  |                    |                      |          |           |           |       |         |        |        |           |             |
 *  | ParseFEN_Original  | r3k2r(...)- 0 1 [68] | 3.521 us | 0.0477 us | 0.0399 us |  1.00 |    0.00 | 0.4883 |      - |    3081 B |        1.00 |
 *  | ParseFEN_Improved1 | r3k2r(...)- 0 1 [68] | 3.606 us | 0.0701 us | 0.0936 us |  1.03 |    0.02 | 0.4463 |      - |    2817 B |        0.91 |
 *  | ParseFEN_Base2     | r3k2r(...)- 0 1 [68] | 3.382 us | 0.0496 us | 0.0414 us |  0.96 |    0.02 | 0.4654 |      - |    2929 B |        0.95 |
 *  | ParseFEN_NoRegex   | r3k2r(...)- 0 1 [68] | 1.815 us | 0.0354 us | 0.0331 us |  0.51 |    0.01 | 0.0877 |      - |     552 B |        0.18 |
 *  |                    |                      |          |           |           |       |         |        |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)6 0 1 [67] | 3.626 us | 0.0618 us | 0.0607 us |  1.00 |    0.00 | 0.4883 |      - |    3073 B |        1.00 |
 *  | ParseFEN_Improved1 | rnbqk(...)6 0 1 [67] | 3.392 us | 0.0436 us | 0.0408 us |  0.94 |    0.02 | 0.4463 |      - |    2801 B |        0.91 |
 *  | ParseFEN_Base2     | rnbqk(...)6 0 1 [67] | 3.415 us | 0.0236 us | 0.0197 us |  0.94 |    0.02 | 0.4616 | 0.0038 |    2905 B |        0.95 |
 *  | ParseFEN_NoRegex   | rnbqk(...)6 0 1 [67] | 1.645 us | 0.0288 us | 0.0241 us |  0.45 |    0.01 | 0.0839 |      - |     528 B |        0.17 |
 *  |                    |                      |          |           |           |       |         |        |        |           |             |
 *  | ParseFEN_Original  | rnbqk(...)- 0 1 [56] | 2.626 us | 0.0165 us | 0.0146 us |  1.00 |    0.00 | 0.4387 |      - |    2761 B |        1.00 |
 *  | ParseFEN_Improved1 | rnbqk(...)- 0 1 [56] | 2.401 us | 0.0231 us | 0.0216 us |  0.91 |    0.01 | 0.3967 |      - |    2497 B |        0.90 |
 *  | ParseFEN_Base2     | rnbqk(...)- 0 1 [56] | 2.492 us | 0.0319 us | 0.0298 us |  0.95 |    0.01 | 0.4120 |      - |    2585 B |        0.94 |
 *  | ParseFEN_NoRegex   | rnbqk(...)- 0 1 [56] | 1.037 us | 0.0208 us | 0.0184 us |  0.39 |    0.01 | 0.0420 |      - |     264 B |        0.10 |
 *  |                    |                      |          |           |           |       |         |        |        |           |             |
 *  | ParseFEN_Original  | rq2k2(...)- 0 1 [71] | 3.952 us | 0.0759 us | 0.0960 us |  1.00 |    0.00 | 0.5035 |      - |    3169 B |        1.00 |
 *  | ParseFEN_Improved1 | rq2k2(...)- 0 1 [71] | 3.647 us | 0.0652 us | 0.0578 us |  0.92 |    0.04 | 0.4616 |      - |    2905 B |        0.92 |
 *  | ParseFEN_Base2     | rq2k2(...)- 0 1 [71] | 3.739 us | 0.0718 us | 0.0636 us |  0.94 |    0.03 | 0.4807 |      - |    3025 B |        0.95 |
 *  | ParseFEN_NoRegex   | rq2k2(...)- 0 1 [71] | 2.111 us | 0.0420 us | 0.0746 us |  0.52 |    0.02 | 0.0992 |      - |     624 B |        0.20 |
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
}
