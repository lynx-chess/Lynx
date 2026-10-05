# Lynx self-gen

Documentation of the data self-generation process used for tuning the HCE evaluation of Lynx chess engine.

Inspired by [Sirius datagen document](https://docs.google.com/document/d/1l6mcAjVCdsicTlojfxbAhw7cTnpx1t8yb59UuWP_t-U).

## Published data

### lynx-1.0

[lynx-1.0](https://huggingface.co/datasets/eduherminio/lynx-1.0)

<details>

#### Datagen

~2M games of no-book self-play datagen (OB runs 3020, 3030):
- 20knpm (soft-nodes), 1s hard time bound

~2M games of book self-play datagen using `UHO_Lichess_4852_v1.epd` (OB runs 3046 and 3047):
- 15knpm (soft-nodes), 1s hard time bound

All data:
- No draw or win adjudications
- 1-4 random half moves on top of the book moves
- Move counters set to 0 after those initial random moves
- Max allowed score: 1000 cp (verified using a depth 10 search)

#### Filtering

- No positions where the best move is a check or a 'tactical' move (captures or promotions)
- Min ply: 20
- Min pieces: 7
- Max eval: 20_000cp
- 40 pos/game, sampling done by alternating phases (`[0, 1, 1, 2, 4, 0]`) in descending order

#### Notes

- Games interleaved before filtering
- Positions shuffled after filtering
- Dataset equivalent to `3020-3030-20kn_3046-3047-15kn-4M-interleaved.vf` in the tests

#### Data analysis

Data produced by [pawnocchio](https://github.com/JonathanHallstrom/pawnocchio) `analyse` tool. Score distribution can be found on Hugging Face.

```
games: 4247458
positions: 541854023
positions/game: 127.57
average exit: 118.71
unique positions: 503872222/541854023 (92%)
wins: 1549405 (36%)
draws: 1196043 (28%)
losses: 1502010 (35%)
king bucket distr:
 0.46  2.15  3.37  1.53 15.31  3.09 18.26  2.97
 0.73  1.39  1.78  2.58  3.33  4.17  4.51  2.91
 0.48  1.00  1.46  2.08  2.58  2.67  2.24  1.08
 0.31  0.70  1.06  1.41  1.62  1.58  1.22  0.58
 0.25  0.48  0.62  0.65  0.70  0.72  0.64  0.35
 0.18  0.36  0.40  0.41  0.40  0.43  0.40  0.21
 0.12  0.22  0.24  0.22  0.22  0.23  0.22  0.12
 0.05  0.09  0.09  0.09  0.09  0.09  0.09  0.04
king bucket distr (mirrored):
 3.43 20.41  6.46 16.85
 3.63  5.90  5.95  5.91
 1.56  3.24  4.12  4.65
 0.89  1.92  2.63  3.03
 0.59  1.12  1.34  1.34
 0.40  0.76  0.84  0.80
 0.25  0.43  0.47  0.44
 0.09  0.17  0.19  0.18
```

</details>

### lynx-1.0-extended

This is a superset of `lynx-1.0` with twice as much data. Same filtering conditions, just moar data that ended up being neutral for Lynx

[lynx-1.0-extended](https://huggingface.co/datasets/eduherminio/lynx-1.0-extended)

<details>

#### Datagen

~4M games of no-book self-play datagen (OB runs 3020, 3030, 3053 and 3061):
- 20knpm (soft-nodes), 1s hard time bound

~2M games of book self-play datagen using `UHO_Lichess_4852_v1.epd` (OB runs 3046 and 3047):
- 15knpm (soft-nodes), 1s hard time bound

~2M games of book self-play datagen using `UHO_Lichess_4852_v1.epd` (OB runs 3048 and 3062):
- 20knpm (soft-nodes), 1s hard time bound

All data:
- No draw or win adjudications
- 1-4 random half moves on top of the book moves
- Move counters set to 0 after those initial random moves
- Max allowed score: 1000 cp (verified using a depth 10 search)

#### Filtering

- No positions where the best move is a check or a 'tactical' move (captures or promotions)
- Min ply: 20
- Min pieces: 7
- Max eval: 20_000cp
- 40 pos/game, sampling done by alternating phases (`[0, 1, 1, 2, 4, 0]`) in descending order

#### Notes

- Games interleaved before filtering
- Positions shuffled after filtering
- Dataset equivalent to `3020-3030-3046-3047-3048-3053-3061-3062-8M-interleaved.vf` in the tests

#### Data analysis

Data produced by [pawnocchio](https://github.com/JonathanHallstrom/pawnocchio) `analyse` tool. Score distribution can be found on Hugging Face.

```
games: 8356728
positions: 1066630687
positions/game: 127.64
average exit: 118.75
unique positions: 979806889/1066630687 (91%)
wins: 3042606 (36%)
draws: 2364860 (28%)
losses: 2949262 (35%)
king bucket distr:
 0.45  2.11  3.38  1.53 15.31  3.08 18.13  3.04
 0.72  1.40  1.78  2.58  3.32  4.19  4.54  2.94
 0.47  1.00  1.45  2.08  2.57  2.67  2.26  1.09
 0.30  0.70  1.06  1.41  1.62  1.58  1.22  0.58
 0.25  0.48  0.62  0.65  0.70  0.73  0.65  0.35
 0.18  0.36  0.40  0.41  0.40  0.43  0.41  0.21
 0.12  0.22  0.24  0.22  0.22  0.23  0.22  0.12
 0.04  0.09  0.09  0.09  0.09  0.09  0.09  0.04

king bucket distr (mirrored):
 3.49 20.24  6.46 16.84
 3.66  5.93  5.97  5.90
 1.57  3.26  4.13  4.65
 0.89  1.92  2.64  3.02
 0.59  1.13  1.34  1.35
 0.40  0.77  0.84  0.80
 0.24  0.43  0.47  0.44
 0.09  0.17  0.19  0.18
```

</details>

### lynx-dfrc-1.0

[lynx-dfrc-1.0](https://huggingface.co/datasets/eduherminio/lynx-dfrc-1.0)

<details>

#### Datagen

~4M games of book self-play datagen using `UHO_Lichess_4852_v1.epd` (OB runs 3102, 3103, 3115 and 3116):
- 15knpm (soft-nodes), 1s hard time bound

All data:
- No draw or win adjudications
- 1-4 random half moves on top of the book moves
- Move counters set to 0 after those initial random moves
- Max allowed score: 1000 cp (verified using a depth 10 search)

#### Filtering

- No positions where the best move is a check or a 'tactical' move (captures or promotions)
- Min ply: 20
- Min pieces: 7
- Max eval: 20_000cp
- 40 pos/game, sampling done by alternating phases (`[0, 1, 1, 2, 4, 0]`) in descending order

#### Notes

- Games interleaved before filtering
- Positions shuffled after filtering
- Dataset equivalent to `3102-3103-3115-3116-4M-interleaved.vf` in the tests

## Data analysis

Data produced by [pawnocchio](https://github.com/JonathanHallstrom/pawnocchio) `analyse` tool. Score distribution can be found on Hugging Face.

```
games: 4197508
positions: 557170545
positions/game: 132.74
average exit: 65.84
unique positions: 505893822/557170545 (90%)
wins: 1683801 (40%)
draws: 1253377 (29%)
losses: 1260330 (30%)
king bucket distr:
 0.82  7.13  8.97  5.43  5.19  5.24 14.49  2.15
 1.49  2.75  2.73  2.52  2.55  3.07  3.12  1.95
 0.76  1.38  1.76  2.11  2.26  2.09  1.65  0.86
 0.40  0.85  1.20  1.41  1.46  1.34  1.02  0.47
 0.28  0.55  0.65  0.66  0.66  0.66  0.58  0.29
 0.20  0.38  0.43  0.41  0.40  0.42  0.38  0.19
 0.13  0.22  0.24  0.23  0.22  0.23  0.21  0.12
 0.05  0.09  0.10  0.10  0.09  0.09  0.09  0.04
king bucket distr (mirrored):
 2.97 21.62 14.21 10.62
 3.43  5.87  5.79  5.06
 1.63  3.02  3.85  4.37
 0.87  1.88  2.54  2.87
 0.58  1.13  1.31  1.32
 0.39  0.76  0.84  0.80
 0.25  0.43  0.48  0.46
 0.09  0.18  0.19  0.19
```

</details>

## self-gen progress

### Data sources

Games generated using [OpenBench](https://github.com/AndyGrant/OpenBench) [data generation workloads](https://github.com/AndyGrant/OpenBench/wiki/Data-Generation-Workloads).

### Data processing

* Extract .pgn files ([extract-pgns.sh](https://github.com/lynx-chess/OpenBench/blob/lynx/lynx-scripts/datagen/extract-pgns.sh))

* ~~Merge .pgn files into a single one ([merge-pgn-files.sh](https://github.com/lynx-chess/OpenBench/blob/lynx/lynx-scripts/datagen/merge-pgn-files.sh))~~

* ~~Convert merged .pgn file into [viriformat](https://github.com/cosmobobak/viriformat) using [pawnocchio](https://github.com/JonathanHallstrom/pawnocchio) `pgntovf` tool~~

* Convert individual .pgn file into [viriformat](https://github.com/cosmobobak/viriformat) using [pawnocchio](https://github.com/JonathanHallstrom/pawnocchio) `pgntovf` tool

* Interleave viriformat files using [bullet-utils](https://github.com/jw1912/bullet) `viribinpack interleave` tool

* Sanitise the viriformat file if needed using [pawnocchio](https://github.com/JonathanHallstrom/pawnocchio) `sanitise` tool

* Filter viriformat file using Lynx `vftoepd` tool

* Shuffle using [`shuf`](https://man7.org/linux/man-pages/man1/shuf.1.html) (GNU coreutils)

* Tune using [Lynx fork of texel-tuner](https://github.com/lynx-chess/texel-tuner)

### Baseline

We use `eval/only-lichessbig3` as base, tuned with 7M positions of `lichess-big3-resolved.book` (most of it).

We tune all other branches using 7M positions too, unless otherwise specified, to try to estimate data quality.

Once data quality surpasses lichess-big3, self-gen data can start to be compared vs `main`.
17M positions can be used, the equivalent size of the `main` dataset. But an even more realistic test is to use 12.43M of self-generated positions mixed with the current DFRC dataset (4.57M external positions)

For DFRC, we use `eval/only-dfrc` as base, tuned using all (D)FRC data currently used in `main` (4.57M)

<details>

```
Test  | eval/only-lichessbig3 (vs main)
Elo   | -22.03 +- 2.96 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 20006: +4776 -6043 =9187
Penta | [476, 2831, 4384, 2108, 204]
https://openbench.lynx-chess.com/test/2777/
```

```
Test  | eval/only-dfrc (vs main)
Elo   | -57.50 +- 13.99 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 1000: +188 -352 =460
Penta | [41, 177, 192, 85, 5]
https://openbench.lynx-chess.com/test/3085/
```

```
Test  | eval/only-dfrc (vs main, DFRC book)
Elo   | -15.99 +- 14.25 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 1000: +243 -289 =468
Penta | [26, 143, 202, 109, 20]
https://openbench.lynx-chess.com/test/3086/
```

For reference, this is how far is other engines data from lichess-big3 when being used for tuning Lynx (still using 7M positions):

```
Test  | eval/only-cw-v5-25knpm (vs eval/only-lichessbig3)
Elo   | -13.97 +- 3.08 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 20002: +5382 -6186 =8434
Penta | [480, 2696, 4353, 2092, 380]
https://openbench.lynx-chess.com/test/2781/
```

```
Test  | eval/only-sirius-40ppg (vs eval/only-lichessbig3)
Elo   | -26.21 +- 5.24 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 7012: +1743 -2271 =2998
Penta | [200, 1043, 1442, 727, 94]
https://openbench.lynx-chess.com/test/2795/
```

```
Test  | eval/only-stash-v35-16knpm (vs eval/only-lichessbig3)
Elo   | -44.91 +- 6.46 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5002: +1147 -1790 =2065
Penta | [201, 815, 978, 440, 67]
https://openbench.lynx-chess.com/test/2796/
```

</details>

### Tests and experiments

<details>

#### `selfgen/base`

**MinPly 0, MinPieces 0, MaxEval 20000, filter checks but no tactical**

Using https://openbench.lynx-chess.com/datagen/2772/

```json
Source file: 2772-5kn-100k.pgn.vf.vf
Total games: 100677
Total positions: 12948612
Positions after filtering: 11232680 (86.75%)
Total time: 17.37 s
```

Progress test
```
Test  | selfgen/base (vs eval/only-lichessbig3)
Elo   | -235.95 +- 12.19 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 3002: +282 -2056 =664
Penta | [674, 490, 282, 46, 9]
https://openbench.lynx-chess.com/test/2779/
```

#### ✅ `selfgen/1-filter-tactical`

MinPly 0, MinPieces 0, MaxEval 20000, filter checks **and tactical**

```json
Source file: 2772-5kn-100k.pgn.vf.vf
Total games: 100677
Total positions: 12948612
Positions after filtering: 8950066 (69.12%)
Total time: 15.61 s
```

SPRT
```
Test  | selfgen/1-filter-tactical (vs selfgen/base)
Elo   | 40.37 +- 10.15 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 1954: +659 -433 =862
Penta | [16, 189, 410, 277, 85]
https://openbench.lynx-chess.com/test/2780/
```

#### ✅ `selfgen/2-minpieces-4`

MinPly 0, **MinPieces 4**, MaxEval 20000, filter checks and tactical

```json
Source file: 2772-5kn-100k.pgn.vf.vf
Total games: 100677
Total positions: 12948612
Positions after filtering: 8433035 (65.13%)
Total time: 15.45 s
```

SPRT
```
Test  | selfgen/2-minpieces-4 (vs selfgen/1-filter-tactical)
Elo   | 14.48 +- 5.81 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 5450: +1668 -1441 =2341
Penta | [91, 575, 1198, 738, 123]
https://openbench.lynx-chess.com/test/2784/
```

#### ✅ `selfgen/2.1-minpieces-4-1M`

**Switch to 1M games dataset**

A bigger initial dataset will allow us to experiment with extra filtering: random position skipping limiting positions per game, etc.

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical

```json
Source file: 2773-5kn-1M.pgn.vf
Total games: 983058
Total positions: 126453028
Positions after filtering: 82371060 (65.14%)
Positions/game: 84
Total time: 2 min 27 s
```

SPRT (non-reg)
```
Test  | selfgen/2.1-minpieces-4-1M (vs selfgen/2-minpieces-4)
Elo   | 82.89 +- 17.30 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.89 (-2.25, 2.89) [-3.00, 1.00]
Games | 820: +356 -164 =300
Penta | [9, 50, 151, 140, 60]
https://openbench.lynx-chess.com/test/2786/
```

Progress test
```
Test  | selfgen/2.1-minpieces-4-1M (vs eval/only-lichessbig3)
Elo   | -118.44 +- 7.26 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5008: +860 -2504 =1644
Penta | [527, 867, 866, 211, 33]
https://openbench.lynx-chess.com/test/2788/
```

#### ❌ `selfgen/3-minpieces-3`

MinPly 0, **MinPieces 3**, MaxEval 20000, filter checks and tactical

```json
Source file: 2773-5kn-1M.pgn.vf
Total games: 983058
Total positions: 126453028
Positions after filtering: 41188799 (32.57%)
Positions/game: 42
Total time: 1 min 52 
```

SPRT
```
Test  | selfgen/3-minpieces-3 (vs selfgen/2.1-minpieces-4-1M)
Elo   | -22.09 +- 8.52 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [0.00, 3.00]
Games | 2504: +609 -768 =1127
Penta | [59, 360, 551, 245, 37]
https://openbench.lynx-chess.com/test/2789/
```

#### ❌ `selfgen/4-randomfenskipping-0.5`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, **random fen skipping 0.5**

```json
Source file: 2773-5kn-1M.pgn.vf
Total games: 983058
Total positions: 126453028
Positions after filtering: 41188799 (32.57%)
Total time: 1 min 52 
```

SPRT
```
Test  | selfgen/4-randomfenskipping-0.5 (vs selfgen/2.1-minpieces-4-1M)
Elo   | -3.35 +- 3.69 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [0.00, 3.00]
Games | 13294: +3507 -3635 =6152
Penta | [244, 1687, 2905, 1575, 236]
https://openbench.lynx-chess.com/test/2790/
```

#### ✅ `selfgen/5-limit-pos-per-game-40`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, **40 pos/game random sampling**

```json
Source file: merged-5kn-1M.pgn.vf
Total games: 983058
Total positions: 126453028
Positions after filtering: 38488314 (30.44%)
Positions/game: 39
Total time: 1 min 58 s
```

SPRT
```
Test  | selfgen/5-limit-pos-per-game-40 (vs selfgen/2.1-minpieces-4-1M)
Elo   | 84.03 +- 14.22 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 1058: +445 -194 =419
Penta | [6, 65, 193, 202, 63]
https://openbench.lynx-chess.com/test/2794/
```

#### ✅ `selfgen/6-genfens-scorethreshold-fix`

Switch dataset to 2797 datagen run, which includes a `genfens` fix that esentially enables depth 10 score <= 1000cp validation for start positions

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game random sampling

```json
Source file: 2797-5kn-1M.pgn.vf
Total games: 1000408
Total positions: 132201949
Positions after filtering: 39563411 (29.93%)
Positions/game: 40
Shortest game: 3 moves, longest game: 369 moves
Total time: 1 min 44 s
```

SPRT (non-reg)
```
Test  | selfgen/6-genfens-scorethreshold-fix (vs selfgen/5-limit-pos-per-game-40)
Elo   | 4.54 +- 4.14 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [-3.00, 1.00]
Games | 10630: +2979 -2840 =4811
Penta | [200, 1228, 2310, 1387, 190]
https://openbench.lynx-chess.com/test/2802/
```


#### ✅ `selfgen/7-limit-pos-per-game-20`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, **20 pos/game** random sampling

```json
Source file: 2797-5kn-1M.pgn.vf
Total games: 1000408
Total positions: 132201949
Positions after filtering: 19989845 (15.12%)
Positions/game: 20
Shortest game: 3 moves, longest game: 369 moves
Total time: 1 min 27 s
```

SPRT
```
Test  | selfgen/7-limit-pos-per-game-20 (vs selfgen/6-genfens-scorethreshold-fix)
Elo   | 2.62 +- 2.03 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 41632: +11320 -11006 =19306
Penta | [674, 4855, 9429, 5199, 659]
https://openbench.lynx-chess.com/test/2803/
```

#### ❌ `selfgen/8-max-initial-eval-500`

MinPly 0, MinPieces 4, **MaxInitialEval 500** MaxEval 20000, filter checks and tactical, 40 pos/game random sampling

```json
Source file: 2797-5kn-1M.pgn.vf
Total games: 1000408
Total positions: 132201949
Positions after filtering: 39563411 (29.93%)
Positions/game: 40
Shortest game: 3 moves, longest game: 369 moves
Total time: 1 min 42 s
```

SPRT
```
Test  | selfgen/8-max-initial-eval-500 (vs selfgen/6-genfens-scorethreshold-fix)
Elo   | -3.96 +- 3.87 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 11306: +2957 -3086 =5263
Penta | [185, 1421, 2541, 1350, 156]
https://openbench.lynx-chess.com/test/2804/
```

#### ✅ `selfgen/9-limit-pos-per-game-phase-1`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling **alternating phases in descending order**

```JSON
Source file: 2797-5kn-1M.pgn.vf
Total games: 1000408
Total positions: 132201949
Positions after filtering: 39563411 (29.93%)
Positions/game: 40
Shortest game: 3 moves, longest game: 369 moves
Total time: 1 min 52 s
```

SPRT
```
Test  | selfgen/9-limit-pos-per-game-phase-1 (vs selfgen/6-genfens-scorethreshold-fix)
Elo   | 15.13 +- 5.94 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 5146: +1500 -1276 =2370
Penta | [66, 585, 1096, 711, 115]
https://openbench.lynx-chess.com/test/2805/
```

#### ❌ `selfgen/10-refactored-genfens`

Swith to 2801 dataset, generated from a refactored version of genfens

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2801-5kn-1M.pgn.vf
Total games: 1000764
Total positions: 132372651
Positions after filtering: 39581928 (29.90%)
Positions/game: 40
Shortest game: 3 moves, longest game: 309 moves
Total time: 4 min 9 s
```

SPRT (non-reg)
```
Test  | selfgen/10-refactored-genfens (vs selfgen/9-limit-pos-per-game-phase-1)
Elo   | -2.81 +- 3.03 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [-3.00, 1.00]
Games | 19434: +5227 -5384 =8823
Penta | [366, 2380, 4366, 2255, 350]
https://openbench.lynx-chess.com/test/2806/
```

#### ❌ `selfgen/11-1-phasepos-per-game`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order, **1 pos/phase/game**

```json
Source file: 2801-5kn-1M.pgn.vf
Total games: 1000764
Total positions: 132372651
Positions after filtering: 8576111 (6.48%)
Positions/game: 9
Shortest game: 3 moves, longest game: 309 moves
Total time: 2 min 31 s
```

SPRT
```
Test  | selfgen/11-1-phasepos-per-game (vs selfgen/10-refactored-genfens)
Elo   | -38.95 +- 11.10 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 1460: +332 -495 =633
Penta | [40, 237, 315, 122, 16]
https://openbench.lynx-chess.com/test/2807/
```

#### ❌ `selfgen/12-2-phasepos-per-game`


MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order, **2** pos/phase/game

```json
Source file: 2801-5kn-1M.pgn.vf
Total games: 1000764
Total positions: 132372651
Positions after filtering: 14586930 (11.02%)
Positions/game: 15
Shortest game: 3 moves, longest game: 309 moves
Total time: 2 min 49 s
```
SPRT
```
Test  | selfgen/12-2-phasepos-per-game (vs selfgen/10-refactored-genfens)
Elo   | -2.89 +- 3.44 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 14570: +3891 -4012 =6667
Penta | [238, 1812, 3307, 1689, 239]
https://openbench.lynx-chess.com/test/2808/
```

#### ❌ `selfgen/13-3-phasepos-per-game`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order, **3** pos/phase/game

```json
Source file: 2801-5kn-1M.pgn.vf
Total games: 1000764
Total positions: 132372651
Positions after filtering: 20049839 (15.15%)
Positions/game: 20
Shortest game: 3 moves, longest game: 309 moves
Total time: 3 min 28 s
```

SPRT
```
Test  | selfgen/13-3-phasepos-per-game  (vs selfgen/10-refactored-genfens)
Elo   | -4.38 +- 4.04 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 10386: +2683 -2814 =4889
Penta | [175, 1291, 2369, 1206, 152]
https://openbench.lynx-chess.com/test/2810/
```

#### 🟡 `selfgen/14-limit-pos-per-game-20`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, **20** pos/game sampling alternating phases in descending order
This is `selfgen/7-limit-pos-per-game-20` after `selfgen/9-limit-pos-per-game-phase-1` (both were green, the latter with a much better SPRT result)

```json
Source file: 2801-5kn-1M.pgn.vf
Total games: 1000764
Total positions: 132372651
Positions after filtering: 19996905 (15.11%)
Positions/game: 20
Shortest game: 3 moves, longest game: 309 moves
Total time: 6 min 44 s
```

SPRT
```
Test  | selfgen/14-limit-pos-per-game-20
Elo   | 0.49 +- 1.15 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 129556: +34552 -34371 =60633
Penta | [1970, 15830, 29200, 15605, 2173]
https://openbench.lynx-chess.com/test/2811/
```

#### ❌ `selfgen/15-8-phasepos-per-game-no-pos-limit`  (vs selfgen/10-refactored-genfens)

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, **8** pos/phase/game
Removing the 40 pos/game limitation, and increaseing pos/phase/game to match the number of positions after filtering

```json
Source file: 2801-5kn-1M.pgn.vf
Total games: 1000764
Total positions: 132372651
Positions after filtering: 40940787 (30.93%)
Positions/game: 41
Shortest game: 3 moves, longest game: 309 moves
Total time: 5 min 32 s
```

SPRT
```
Test  | selfgen/15-8-phasepos-per-game-no-pos-limit
Elo   | -7.01 +- 7.89 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -0.93 (-2.25, 2.89) [0.00, 3.00]
Games | 2974: +797 -857 =1320
Penta | [69, 368, 651, 352, 47]
https://openbench.lynx-chess.com/test/2812/
```

#### ✅ `selfgen/16-genfens-seed`

Swith to 2809 dataset, generated from a refactored version of genfens that uses the provided seed

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 39574492 (29.91%)
Positions/game: 40
Shortest game: 3 moves, longest game: 318 moves
Total time: 5 min 8 s
```

SPRT (non-reg)
```
Test  | selfgen/16-genfens-seed (vs selfgen/9-limit-pos-per-game-phase-1)
Elo   | 13.27 +- 6.86 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.89 (-2.25, 2.89) [-3.00, 1.00]
Games | 3982: +1183 -1031 =1768
Penta | [54, 464, 853, 516, 104]
https://openbench.lynx-chess.com/test/2818/
```

Progress test

```
Test  | selfgen/16-genfens-seed (vs eval/only-lichessbig3)
Elo   | -24.03 +- 6.28 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5010: +1288 -1634 =2088
Penta | [172, 663, 1081, 517, 72]
https://openbench.lynx-chess.com/test/2819/
```

Progress test after retuning using 17M positions (roughly the size of the current dataset)
```
Test  | selfgen/16-genfens-seed-17M (vs eval/only-lichessbig3)
Elo   | -21.92 +- 6.27 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5000: +1272 -1587 =2141
Penta | [158, 675, 1068, 522, 77]
https://openbench.lynx-chess.com/test/2827/
```

#### ❌ `selfgen/17-low-phase-first`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases, **ascending order**

```json
Source file: 2809-5kn-1M.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 39574492 (29.91%)
Positions/game: 40
Shortest game: 3 moves, longest game: 318 moves
Total time: 3 min 49 s
```

SPRT
```
Test  | selfgen/17-low-phase-first (vs selfgen/16-genfens-seed)
Elo   | -8.77 +- 5.40 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.28 (-2.25, 2.89) [0.00, 3.00]
Games | 5706: +1432 -1576 =2698
Penta | [79, 781, 1278, 635, 80]
https://openbench.lynx-chess.com/test/2820/
```

#### ❌ `selfgen/18-alterning-ascending-descending-phase`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases, **alternating ascending and descending order**

```json
Total games: 1000690
Total positions: 132322604
Positions after filtering: 39574492 (29.91%)
Positions/game: 40
Shortest game: 3 moves, longest game: 318 moves
Total time: 4 min 3 s
```

SPRT
```
Test  | selfgen/18-alterning-ascending-descending-phase (vs selfgen/16-genfens-seed)
Elo   | -3.76 +- 3.82 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 11908: +3062 -3191 =5655
Penta | [200, 1519, 2623, 1434, 178]
https://openbench.lynx-chess.com/test/2821/
``` 

#### ❌ `selfgen/19-max-initial-eval-500`

MinPly 0, MinPieces 4, MaxEval 20000, **MaxIntialEval 500**, filter checks and tactical, 40 pos/game sampling alternating phases in descending order
This is equivalent to `selfgen/8-max-initial-eval-500`, but with the implementation fixed

```json
Source file: 2809-5kn-1M.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 37654830 (28.46%)
Positions/game: 38
Shortest game: 3 moves, longest game: 318 moves
Total time: 4 min 37 s
```

SPRT
```
Test  | selfgen/19-max-initial-eval-500 (vs selfgen/16-genfens-seed)
Elo   | -4.32 +- 4.01 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [0.00, 3.00]
Games | 10536: +2704 -2835 =4997
Penta | [168, 1333, 2379, 1238, 150]
https://openbench.lynx-chess.com/test/2822/
```

#### ❌ `selfgen/20-max-initial-eval-300`

MinPly 0, MinPieces 4, MaxEval 20000, **MaxIntialEval 300**, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 33812853 (25.55%)
Positions/game: 34
Shortest game: 3 moves, longest game: 318 moves
Total time: 11 min 0 
```

SPRT
```
Test  | selfgen/20-max-initial-eval-300 (vs selfgen/16-genfens-seed)
Elo   | -4.42 +- 4.06 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 10306: +2662 -2793 =4851
Penta | [176, 1280, 2343, 1207, 147]
https://openbench.lynx-chess.com/test/2823/
```

#### ❌ `selfgen/21-adjudications`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order, win adj (1000cp, 5 moves), draw adj (10cp, 8 moves, from move 40)

```json
Source file: 2809-5kn-1M.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 38675421 (29.23%)
Positions/game: 39
Shortest game: 3 moves, longest game: 318 moves
Games adjudicated as a draw: 64686 (6.00%, 1211541 potential positions saved)
Games adjudicated as a win: 598064 (59.00%, 8572461 potential positions saved)
Total time: 4 min 27 
```

SPRT
```
Test  | selfgen/21-adjudications (vs selfgen/16-genfens-seed)
Elo   | -0.94 +- 2.38 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 29688: +7841 -7921 =13926
Penta | [453, 3638, 6732, 3578, 443]
https://openbench.lynx-chess.com/test/2824/
```

#### ❌ `selfgen/21-shuffle-positionsbyphase`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases **in random order**

```json
Source file: 2809-5kn-1M.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 39574492 (29.91%)
Positions/game: 40
Shortest game: 3 moves, longest game: 318 moves
Total time: 4 min 36 s
```

SPRT
```
Test  | selfgen/21-shuffle-positionsbyphase (vs selfgen/16-genfens-seed)
Elo   | -4.21 +- 3.96 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 10656: +2731 -2860 =5065
Penta | [152, 1378, 2394, 1255, 149]
https://openbench.lynx-chess.com/test/2825/
```

#### ❌ `selfgen/21-positionsbyphase-descending-randomstart`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order **with random start**

```json
Source file: 2809-5kn-1M.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 39574492 (29.91%)
Positions/game: 40
Shortest game: 3 moves, longest game: 318 moves
Total time: 5 min 2 s
```

SPRT
```
Test  | selfgen/21-positionsbyphase-descending-randomstart (vs selfgen/16-genfens-seed)
Elo   | -3.57 +- 3.74 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 12344: +3228 -3355 =5761
Penta | [213, 1529, 2791, 1450, 189]
https://openbench.lynx-chess.com/test/2826/
```

#### ❌ `selfgen/22-half-full-moves`

Switch to 2836 dataset, which includes half and full move counters in genfens starting positions

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

> [!NOTE]
>I can't really understand why this didn't pass, the only Lynx functional changes were adding 50mr half-move counter and full move counter, other than adding filtering that was disabled ([a84aba7](https://github.com/lynx-chess/Lynx/commit/a84aba731c7870ad343ecad89f7566660df8f9c6)).
>
>An important OB update was required to reflect the counters in the startpos fen of the PGNs though.
>
>Most of the following tests have been run against this branch, which subsequent non-reg tests vs `selfgen/16-genfens-seed` to try to unify both branches.

```json
Source file: 2836-5kn-1M.pgn.vf
Total games: 997687
Total positions: 131933001
Positions after filtering: 39458212 (29.91%)
Positions/game: 40
Shortest game: 7 moves, longest game: 323 moves
Total time: 3 min 31 s
```

SPRT (non-reg)
```
Test  | selfgen/22-half-full-moves (vs selfgen/16-genfens-seed)
Elo   | -2.68 +- 2.91 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.30 (-2.25, 2.89) [-3.00, 1.00]
Games | 20624: +5510 -5669 =9445
Penta | [363, 2527, 4664, 2422, 336]
https://openbench.lynx-chess.com/test/2840/
```

#### ✅ `selfgen/23-minply-16`

**MinPly 16**, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2836-5kn-1M.pgn.vf
Total games: 997687
Total positions: 131933001
Positions after filtering: 39059704 (29.61%)
Positions/game: 39
Shortest game: 7 moves, longest game: 323 moves
Total time: 3 min 38 s
```

SPRT
```
Test  | selfgen/23-minply-16 (vs selfgen/22-half-full-moves)
Elo   | 3.10 +- 2.30 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 32240: +8662 -8374 =15204
Penta | [477, 3843, 7233, 4049, 518]
https://openbench.lynx-chess.com/test/2841/
```

#### ❌ `selfgen/24-piece-probabilities`

Switch to 2838 dataset, which includes half and full move counters in genfens starting positions + piece probatilities

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2838-5kn-1M.pgn.vf
Total games: 994977
Total positions: 131672274
Positions after filtering: 39364994 (29.90%)
Positions/game: 40
Shortest game: 7 moves, longest game: 330 moves
Total time: 3 min 59 s
```

SPRT
```
Test  | selfgen/24-piece-probabilities (vs selfgen/16-genfens-seed)
Elo   | -2.05 +- 3.05 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 18830: +5053 -5164 =8613
Penta | [341, 2284, 4239, 2247, 304]
https://openbench.lynx-chess.com/test/2842/
```

#### 🟡 `selfgen/25-adj-maxpospergame-20`

Combine `selfgen/21-adjudications` + `selfgen/14-limit-pos-per-game-20`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, **20** pos/game sampling alternating phases in descending order, **win adj (500cp, 5 moves), draw adj (10cp, 8 moves, from move 40)**

```json
Source file: 2836-5kn-1M.pgn.vf
Total games: 997687
Total positions: 131933001
Positions after filtering: 18606005 (14.10%)
Positions/game: 19
Shortest game: 7 moves, longest game: 323 moves
Games adjudicated as a draw: 63865 (6.00%, 1206536 potential positions saved)
Games adjudicated as a win: 697149 (69.00%, 29347586 potential positions saved)
Total time: 2 min 34 s
```

SPRT
```
Test  | selfgen/25-adj-maxpospergame-20
Elo   | 1.18 +- 1.67 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 0.68 (-2.25, 2.89) [0.00, 3.00]
Games | 62208: +16786 -16574 =28848
Penta | [954, 7566, 13936, 7610, 1038]
https://openbench.lynx-chess.com/test/2843/
```

#### ❌ `selfgen/26-10-pos-per-game`

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, **10** pos/game sampling alternating phases in descending order

```json
Source file: 2836-5kn-1M.pgn.vf
Total games: 997687
Total positions: 131933001
Positions after filtering: 9974304 (7.56%)
Positions/game: 10
Shortest game: 7 moves, longest game: 323 moves
Total time: 2 min 32 s
```

SPRT
```
Test  | selfgen/26-10-pos-per-game  (vs selfgen/22-half-full-moves)
Elo   | -8.46 +- 5.52 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 6120: +1588 -1737 =2795
Penta | [142, 778, 1327, 713, 100]
https://openbench.lynx-chess.com/test/2844/
```

#### ✅ `selfgen/27-interleave-before-shuf`

Interleave using bullet-utils before shuffling

MinPly 0, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```JSON
Source file: 2836-5kn-1M-interleaved.vf
Total games: 997687
Total positions: 131933001
Positions after filtering: 39458212 (29.91%)
Positions/game: 40
Shortest game: 7 moves, longest game: 323 moves
Total time: 3 min 50 s
```

SPRT

```
Test  | selfgen/27-interleave-before-shuf (vs selfgen/22-half-full-moves)
Elo   | 4.27 +- 2.84 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.89 (-2.25, 2.89) [0.00, 3.00]
Games | 20650: +5550 -5296 =9804
Penta | [280, 2416, 4718, 2592, 319]
https://openbench.lynx-chess.com/test/2846/
```

SPRT (non-reg)
```
Test  | selfgen/27-interleave-before-shuf (vs selfgen/16-genfens-seed)
Elo   | -3.89 +- 3.69 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.30 (-2.25, 2.89) [-3.00, 1.00]
Games | 13232: +3537 -3685 =6010
Penta | [272, 1619, 2921, 1593, 211]
https://openbench.lynx-chess.com/test/2847/
```

#### ❌ `selfgen/28-interleave-minply-16`

`selfgen/27-interleave-before-shuf` + `selfgen/23-minply-16` on top of `selfgen/22-half-full-moves`

MinPly 16, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2836-5kn-1M-interleaved.vf
Total games: 997687
Total positions: 131933001
Positions after filtering: 39059704 (29.61%)
Positions/game: 39
Shortest game: 7 moves, longest game: 323 moves
Total time: 4 min 14 s
```

```
Test  | selfgen/28-interleave-minply-16 (vs selfgen/16-2-interleave-minply)
Elo   | -2.93 +- 3.10 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [-3.00, 1.00]
Games | 18254: +4865 -5019 =8370
Penta | [317, 2300, 4019, 2202, 289]
https://openbench.lynx-chess.com/test/2852/
```

#### ✅ `selfgen/16-2-interleave-minply`

`selfgen/27-interleave-before-shuf` + `selfgen/23-minply-16` on top of `selfgen/16-genfens-seed`

2809 but interleaving individual .vf files instead of merging individual .pgn into a single one first, before .pgn -> .vf conversion

MinPly 16, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 38521793 (29.11%)
Positions/game: 38
Shortest game: 3 moves, longest game: 318 moves
Total time: 4 min 0 s
```

SPRT
```
Test  | selfgen/16-2-interleave-minply (vs selfgen/16-genfens-seed)
Elo   | 2.81 +- 2.14 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.89 (-2.25, 2.89) [0.00, 3.00]
Games | 36850: +9882 -9584 =17384
Penta | [548, 4351, 8344, 4619, 563]
https://openbench.lynx-chess.com/test/2850/
```

#### ❌ `selfgen/29-limit-pos-per-game-20`

Same as `selfgen/14-limit-pos-per-game-20` and `selfgen/7-limit-pos-per-game-20`.

MinPly 16, MinPieces 4, MaxEval 20000, filter checks and tactical, **20** pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 19885895 (15.03%)
Positions/game: 20
Shortest game: 3 moves, longest game: 318 moves
Total time: 3 min 60 s
```

SPRT
```
Test  | selfgen/29-limit-pos-per-game-20 (vs selfgen/16-2-interleave-minply)
Elo   | -5.01 +- 4.24 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [-3.00, 1.00]
Games | 9290: +2416 -2550 =4324
Penta | [134, 1193, 2135, 1039, 144]
https://openbench.lynx-chess.com/test/2879/
```

#### ✅ `selfgen/30-minpieces-5`

MinPly 16, MinPieces **5**, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 38508121 (29.10%)
Positions/game: 38
Shortest game: 3 moves, longest game: 318 moves
Total time: 4 min 59 s
```

SPRT
```
Test  | selfgen/30-minpieces-5 (vs selfgen/16-2-interleave-minply)
Elo   | 4.50 +- 2.92 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 19378: +5232 -4981 =9165
Penta | [275, 2229, 4433, 2474, 278]
https://openbench.lynx-chess.com/test/2888/
```

#### ✅ `selfgen/31-minply-20`

MinPly **20**, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 38111848 (28.80%)
Positions/game: 38
Shortest game: 3 moves, longest game: 318 moves
Total time: 5 min 1 s
```

SPRT
```
Test  | selfgen/31-minply-20 (vs selfgen/16-2-interleave-minply)
Elo   | 1.93 +- 1.57 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 68594: +18253 -17872 =32469
Penta | [949, 8350, 15410, 8547, 1041]
https://openbench.lynx-chess.com/test/2889/
```

#### ❌ `selfgen/32-minply-12`

MinPly **12**, MinPieces 4, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 38875472 (29.38%)
Positions/game: 39
Shortest game: 3 moves, longest game: 318 moves
Total time: 5 min 18 s
```

SPRT

#### ✅ `selfgen/33-minpieces-5-minply-20`

`selfgen/30-minpieces-5` + `selfgen/31-minply-20`

MinPly 20, MinPieces 5, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 38088733 (28.78%)
Positions/game: 38
Shortest game: 3 moves, longest game: 318 moves
Total time: 5 min 28 s
```

SPRT non-reg
```
Test  | selfgen/33-minpieces-5-minply-20 (vs selfgen/30-minpieces-5)
Elo   | 1.69 +- 2.09 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 4.89 (-2.25, 2.89) [-3.00, 1.00]
Games | 38834: +10275 -10086 =18473
Penta | [580, 4639, 8831, 4746, 621]
https://openbench.lynx-chess.com/test/2892/
```

#### ✅ `selfgen/34-minpieces-6`

MinPly 20, MinPieces **6**, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 38016760 (28.73%)
Positions/game: 38
Shortest game: 3 moves, longest game: 318 moves
Total time: 4 min 28 s
```

SPRT
```
Test  | selfgen/34-minpieces-6 (vs selfgen/33-minpieces-5-minply-20)
Elo   | 8.51 +- 4.24 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 9148: +2512 -2288 =4348
Penta | [123, 997, 2133, 1175, 146]
https://openbench.lynx-chess.com/test/2896/
```

#### ❌ `selfgen/35-genfens-verif-15`

Adopt 2883 dataset, which searches depth 15 instead of depth 10 for genfens startpos verification

MinPly 20, MinPieces 6, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 37845885 (28.60%)
Positions/game: 38
Shortest game: 3 moves, longest game: 318 moves
Total time: 4 min 28 s
```

SPRT
```
Test  | selfgen/35-genfens-verif-15
Elo   | -7.09 +- 4.96 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 6862: +1743 -1883 =3236
Penta | [120, 875, 1546, 805, 85]
https://openbench.lynx-chess.com/test/2897/
```

#### ✅ `selfgen/36-minpieces-7`

MinPly 20, MinPieces **7**, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Total positions: 132322604
Positions after filtering: 38016760 (28.73%)
Positions/game: 38
Shortest game: 3 moves, longest game: 318 moves
Total time: 4 min 37 s
```

SPRT
```
Test  | selfgen/36-minpieces-7 (vs selfgen/34-minpieces-6)
Elo   | 2.16 +- 1.73 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 55050: +14459 -14116 =26475
Penta | [758, 6556, 12597, 6813, 801]
https://openbench.lynx-chess.com/test/2898/
```

#### 🟡 `selfgen/37-mingamesply-30`

MinPly 20, **MinGamePly 30**, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2809-5kn-1M-interleaved.pgn.vf
Total games: 1000690
Games after filtering: 998301 (99.76%)
Total positions: 132322604
Positions after filtering: 37840355 (28.60%)
Positions/game: 38
Shortest game: 16 moves, longest game: 318 moves
Total time: 4 min 26 s
```

SPRT
```
Test  | selfgen/37-mingamesply-30
Elo   | 0.26 +- 1.37 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.28 (-2.25, 2.89) [0.00, 3.00]
Games | 86432: +22413 -22348 =41671
Penta | [1144, 10580, 19728, 10595, 1169]
https://openbench.lynx-chess.com/test/2902/
```

#### ✅ `selfgen/38-no-move-counters`

Adopt 2884 dataset, which removes half and full move counters from genfens start positions, trying to fix the reversion identified in `selfgen/22-half-full-moves`.

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2884-5kn-1M-interleaved.pgn.vf
Total games: 1001354
Total positions: 132372988
Positions after filtering: 37868557 (28.61%)
Positions/game: 38
Shortest game: 3 moves, longest game: 308 moves
Total time: 5 min 19 s
```

non-reg SPRT
```
Test  | selfgen/38-no-move-counters (vs selfgen/36-minpieces-7)
Elo   | 2.79 +- 3.33 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [-3.00, 1.00]
Games | 15790: +4369 -4242 =7179
Penta | [260, 1862, 3539, 1959, 275]
https://openbench.lynx-chess.com/test/2903/
```

#### ❌ `selfgen/39-genfens-verif-15`

Same as `selfgen/35-genfens-verif-15`, but the full 2883 dataset (1M games).

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2883-5kn-1M-interleaved.pgn.vf
Total games: 1001093
Total positions: 132397902
Positions after filtering: 38795490 (29.30%)
Positions/game: 39
Shortest game: 7 moves, longest game: 318 moves
Total time: 5 min 31 s
```

SPRT
```
Test  | selfgen/39-genfens-verif-15 (vs selfgen/36-minpieces-7)
Elo   | -2.21 +- 3.14 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 17918: +4768 -4882 =8268
Penta | [307, 2233, 4000, 2105, 314]
https://openbench.lynx-chess.com/test/2906/
```

#### ❌ `selfgen/40-piece-probabilities`

Adopt 2905 dataset, which uses piece probablities instead of randomly selecting moves.

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

Same as `selfgen/24-piece-probabilities`

```json
Source file: 2905-5kn-1M-interleaved.pgn.vf
Total games: 996560
Total positions: 131986946
Positions after filtering: 37755124 (28.61%)
Positions/game: 38
Shortest game: 3 moves, longest game: 291 moves
Total time: 3 min 55 s
```

SPRT
```
Test  | selfgen/40-piece-probabilities (vs selfgen/38-no-move-counters)
Elo   | -0.37 +- 1.99 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.28 (-2.25, 2.89) [0.00, 3.00]
Games | 43680: +11660 -11706 =20314
Penta | [706, 5343, 9818, 5237, 736]
https://openbench.lynx-chess.com/test/2908/
```

#### ❌ `selfgen/41-genfens-adj`

Adopt 2907 dataset, generated using adjudication directly in OB.

Adjudication concept already explored and failed during filtering in `selfgen/21-adjudications`, but just in case...

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2907-5kn-1M-interleaved.pgn.vf
Total games: 1000458
Total positions: 98187652
Positions after filtering: 31249987 (31.83%)
Positions/game: 31
Shortest game: 3 moves, longest game: 303 moves
Total time: 3 min 53 s
```

SPRT
```
Test  | selfgen/41-genfens-adj
Elo   | -1.37 +- 2.68 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 24414: +6496 -6592 =11326
Penta | [425, 2997, 5419, 2981, 385]
https://openbench.lynx-chess.com/test/2909/
```

#### ✅ `selfgen/42-book-datagen-1-4`

Adopt 2904 dataset, generated using `UHO_Lichess_4852_v1.epd` + 1-4 moves

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2904-5kn-1M-interleaved.pgn.vf
Total games: 1000602
Total positions: 123602411
Positions after filtering: 36884362 (29.84%)
Positions/game: 37
Shortest game: 3 moves, longest game: 317 moves
Total time: 4 min 26 s
```

SPRT
```
Test  | selfgen/42-book-datagen-1-4 (vs selfgen/38-no-move-counters)
Elo   | 2.62 +- 2.03 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 42478: +11631 -11311 =19536
Penta | [701, 5042, 9482, 5264, 750]
https://openbench.lynx-chess.com/test/2912/
```

#### ✅ `selfgen/43-book-datagen-1-4-50mix`

Use 2884 + 2904 (book datagen 1-4) datasets

This effectively mixes `selfgen/38-no-move-counters` + `selfgen/42-book-datagen-1-4`, which since this point will use as a reference for improvements of non-book and book data.

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2884-2904-5kn-1M-interleaved.pgn.vf
Total games: 2001956
Total positions: 255975399
Positions after filtering: 74752919 (29.20%)
Positions/game: 37
Shortest game: 3 moves, longest game: 317 moves
Total time: 8 min 17 s
```

SPRT
```
Test  | selfgen/43-book-datagen-1-4-50mix (vs selfgen/42-book-datagen-1-4)
Elo   | 9.39 +- 4.49 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 8330: +2309 -2084 =3937
Penta | [107, 951, 1841, 1142, 124]
https://openbench.lynx-chess.com/test/2915/
```

#### 🟡 `selfgen/44-2884-2905-50mix`

2884 + 2905 datasets (using 2905 instead of 2884 was neutral, see `selfgen/40-piece-probabilities`)

The only point of this branch is proving that `selfgen/43-book-datagen-1-4-50mix` is good due to mixing book and non-book data, instead of just due to having more data together.

The results suggest that: adding more data even of only picking 7M pos is good, but not as good as mixing book + non-book

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2884-2905-5kn-1M-interleaved.pgn.vf
Total games: 1997914
Total positions: 264359934
Positions after filtering: 75623681 (28.61%)
Positions/game: 38
Shortest game: 3 moves, longest game: 308 moves
Total time: 8 min 12 s
```

SPRT
```
Test  | selfgen/44-2884-2905-50mix (vs selfgen/38-no-move-counters)
Elo   | 3.52 +- 3.58 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 1.41 (-2.25, 2.89) [0.00, 3.00]
Games | 12850: +3406 -3276 =6168
Penta | [195, 1450, 2988, 1614, 178]
https://openbench.lynx-chess.com/test/2916/
```

#### ❌ `selfgen/45-datagen-maxeval-1500`

2911 dataset, which allows positions with evals <= 1500 (instead of 1000)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2911-5kn-1M-interleaved.pgn.vf
Total games: 1000526
Total positions: 131588158
Positions after filtering: 37752374 (28.69%)
Positions/game: 38
Shortest game: 3 moves, longest game: 317 moves
Total time: 4 min 1 s
```

SPRT
```
Test  | selfgen/45-datagen-maxeval-1500 (vs selfgen/38-no-move-counters)
Elo   | -3.87 +- 3.85 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 11578: +3035 -3164 =5379
Penta | [187, 1452, 2644, 1315, 191]
https://openbench.lynx-chess.com/test/2917/
```

#### ❌ `selfgen/46-datagen-maxeval-500`

2918 dataset, which allows positions with evals <= 500 (instead of 1000)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2918-5kn-1M-interleaved.pgn.vf
Total games: 1000682
Total positions: 134680474
Positions after filtering: 37931994 (28.16%)
Positions/game: 38
Shortest game: 3 moves, longest game: 324 moves
Total time: 4 min 24 s
```

SPRT
```
Test  | selfgen/46-datagen-maxeval-500 (vs selfgen/38-no-move-counters)
Elo   | -1.19 +- 2.54 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 26310: +6904 -6994 =12412
Penta | [421, 3205, 5975, 3151, 403]
https://openbench.lynx-chess.com/test/2933/
```

#### ❌ `selfgen/47-book-datagen-4-5`

2914 dataset, which uses `UHO_Lichess_4852_v1.epd` + 4-5 moves (instead of 1-4)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2914-5kn-1M-interleaved.pgn.vf
Total games: 997801
Total positions: 119240037
Positions after filtering: 36260493 (30.41%)
Positions/game: 36
Shortest game: 2 moves, longest game: 302 moves
Total time: 4 min 33 s
```

SPRT
```
Test  | selfgen/47-book-datagen-4-5 (vs selfgen/42-book-datagen-1-4)
Elo   | -9.96 +- 5.85 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 5164: +1309 -1457 =2398
Penta | [95, 689, 1150, 565, 83]
https://openbench.lynx-chess.com/test/2921/
```

#### ❌ `selfgen/48-random-moves-6`

2922 dataset, which uses 6/7 random moves instead of 8/9

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2922-5kn-1M-interleaved.pgn.vf
Total games: 1000347
Total positions: 132247677
Positions after filtering: 37827710 (28.60%)
Positions/game: 38
Shortest game: 3 moves, longest game: 410 moves
Total time: 4 min 47 s
```

SPRT
```
Test  | selfgen/48-random-moves-6 (vs selfgen/38-no-move-counters)
Elo   | -2.49 +- 3.25 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 16340: +4269 -4386 =7685
Penta | [247, 2112, 3572, 1989, 250]
https://openbench.lynx-chess.com/test/2934/
```

#### ❌ `selfgen/49-random-moves-10`

2923 dataset, which uses 10/11 random moves instead of 8/9

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2923-5kn-1M-interleaved.pgn.vf
Total games: 1000452
Total positions: 132267753
Positions after filtering: 37834938 (28.60%)
Positions/game: 38
Shortest game: 2 moves, longest game: 314 moves
Total time: 3 min 43 s
```

SPRT
```
Test  | selfgen/49-random-moves-10 (vs selfgen/38-no-move-counters)
Elo   | -1.72 +- 2.86 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 21066: +5607 -5711 =9748
Penta | [361, 2540, 4804, 2498, 330]
https://openbench.lynx-chess.com/test/2938/
```

#### ❌ `selfgen/50-random-moves-6-11`

2939 dataset, which uses 6-11 random moves instead of 8/9

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2939-5kn-1M-interleaved.pgn.vf
Total games: 1000409
Total positions: 132388232
Positions after filtering: 37833960 (28.58%)
Positions/game: 38
Shortest game: 3 moves, longest game: 315 moves
Total time: 3 min 47 s
```

SPRT
```
Test  | selfgen/50-random-moves-6-11 (vs selfgen/38-no-move-counters)
Elo   | -2.12 +- 3.07 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [0.00, 3.00]
Games | 18328: +4809 -4921 =8598
Penta | [301, 2289, 4082, 2205, 287]
https://openbench.lynx-chess.com/test/2940/
```

#### ✅ `selfgen/51-book-10kn`

2937 dataset, generated using 10k nodes and `UHO_Lichess_4852_v1.epd` + 1-4 moves. Same filtering as `selfgen/42-book-datagen-1-4`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2937-10kn-1M-interleaved.pgn.vf
Total games: 1000365
Total positions: 123387973
Positions after filtering: 37049892 (30.03%)
Positions/game: 37
Shortest game: 3 moves, longest game: 298 moves
Total time: 3 min 38 s
```

SPRT
```
Test  | selfgen/51-book-10kn (vs selfgen/42-book-datagen-1-4)
Elo   | 8.23 +- 4.21 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 9790: +2734 -2502 =4554
Penta | [146, 1101, 2198, 1275, 175]
https://openbench.lynx-chess.com/test/2943/
```

#### ❌ `selfgen/52-random-moves-7-10`

2941 dataset, which uses 7-10 random moves instead of 8/9

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2941-5kn-1M-interleaved.pgn.vf
Total games: 975506
Total positions: 129082605
Positions after filtering: 36887654 (28.58%)
Positions/game: 38
Shortest game: 3 moves, longest game: 323 moves
Total time: 4 min 11 s
```

SPRT
```
Test  | selfgen/52-random-moves-7-10 (vs selfgen/38-no-move-counters)
Elo   | -6.68 +- 4.86 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [0.00, 3.00]
Games | 7334: +1892 -2033 =3409
Penta | [144, 900, 1682, 835, 106]
https://openbench.lynx-chess.com/test/2950/
```

#### ❌ `selfgen/53-moves-3-6-2`

3033 dataset, which uses 3-6 random moves on top of `UHO_Lichess_4852_v1.epd` (instead of 1-4)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3033-5kn-1M-interleaved.vf
Total games: 1000602
Total positions: 119484916
Positions after filtering: 36339029 (30.41%)
Positions/game: 36
Shortest game: 2 moves, longest game: 279 moves
Total time: 3 min 50 s
```

SPRT
```
Test  | selfgen/53-moves-3-6-2
Elo   | -11.43 +- 6.25 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [0.00, 3.00]
Games | 4592: +1154 -1305 =2133
Penta | [104, 589, 1020, 520, 63]
https://openbench.lynx-chess.com/test/3041/
```

#### ✅ `selfgen/54-nobook-10kn`

2942 dataset, generated using 10k nodes. Same filtering as `selfgen/38-no-move-counters`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2942-10kn-1M-interleaved.pgn.vf
Total games: 849233
Total positions: 112379373
Positions after filtering: 32255874 (28.70%)
Positions/game: 38
Shortest game: 3 moves, longest game: 291 moves
Total time: 3 min 46 s
```

SPRT
```
Test  | selfgen/54-nobook-10kn (vs selfgen/38-no-move-counters)
Elo   | 5.22 +- 3.22 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 16314: +4468 -4223 =7623
Penta | [235, 1891, 3678, 2100, 253]
https://openbench.lynx-chess.com/test/2954/
```

#### ❌ `selfgen/55-mingamesply-30-minply-16`

Min game ply idea from `selfgen/37-mingamesply-30` which might help avoiding low quality games and allow us to reduce min ply

**MinPly 16**, **MinGamePly 30**, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2942-10kn-1M-interleaved.pgn.vf
Total games: 849233
Games after filtering: 847208 (99.76%)
Total positions: 112379373
Positions after filtering: 32652349 (29.06%)
Positions/game: 39
Shortest game: 16 moves, longest game: 291 moves
Total time: 4 min 26 s
```

SPRT
```
Test  | selfgen/55-mingamesply-30-minply-16 (vs selfgen/54-nobook-10kn)
Elo   | -1.63 +- 2.75 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [0.00, 3.00]
Games | 21492: +5454 -5555 =10483
Penta | [301, 2635, 4961, 2562, 287]
https://openbench.lynx-chess.com/test/2956/
```

#### ❌ `selfgen/56-cw-phase-filtering`

Apply CW phase filtering on top of `selfgen/54-nobook-10kn` + shuffling.

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

`32255874 -> 10284092`

```bash
Filtering file 54-nobook-10kn.epd and writing to 54-nobook-10kn_after-cw-phase-filtering.epd
Phase 0 has keep probability 0.17113422856636476
Phase 1 has keep probability 0.30081664487701837
Phase 2 has keep probability 0.1579951553239074
Phase 3 has keep probability 0.2379316998081618
Phase 4 has keep probability 0.13267695458954187
Phase 5 has keep probability 0.1980546566323134
Phase 6 has keep probability 0.15361728554285686
Phase 7 has keep probability 0.33628924874212796
Phase 8 has keep probability 0.2649849324376741
Phase 9 has keep probability 0.47519356469195967
Phase 10 has keep probability 0.32074075345004033
Phase 11 has keep probability 0.6526286683082533
Phase 12 has keep probability 0.37150921112581936
Phase 13 has keep probability 0.7986133587784457
Phase 14 has keep probability 0.4507432804135693
Phase 15 has keep probability 1.0
Phase 16 has keep probability 0.6091668070423616
Phase 17 has keep probability 0.7469076991496346
Phase 18 has keep probability 0.43993399485829454
Phase 19 has keep probability 0.40962749360005773
Phase 20 has keep probability 0.2408143503970063
Phase 21 has keep probability 0.2307397901740613
Phase 22 has keep probability 0.15274627531718135
Phase 23 has keep probability 0.13224375344773753
Phase 24 has keep probability 0.08408328165605822
Finished
```

SPRT
```
Test  | selfgen/56-cw-phase-filtering (vs selfgen/54-nobook-10kn)
Elo   | -3.26 +- 3.54 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 12886: +3267 -3388 =6231
Penta | [173, 1625, 2950, 1540, 155]
https://openbench.lynx-chess.com/test/2955/
```

#### ❌ `selfgen/57-random-sampling`

Just making sure the phase sampling is good

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game **random** sampling

```json
Source file: 2942-10kn-1M-interleaved.pgn.vf
Total games: 849233
Total positions: 112379373
Positions after filtering: 32255874 (28.70%)
Positions/game: 38
Shortest game: 3 moves, longest game: 291 moves
Total time: 1 min 24 s
```

SPRT
```
Test  | selfgen/57-random-sampling (vs selfgen/54-nobook-10kn)
Elo   | -1.60 +- 2.72 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.31 (-2.25, 2.89) [0.00, 3.00]
Games | 22120: +5588 -5690 =10842
Penta | [330, 2696, 5060, 2694, 280]
https://openbench.lynx-chess.com/test/2959/
```

#### ❌ `selfgen/58-nodefudging-5k`

2952 dataset, generated using 10k +-2k nodes.

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2952-10kn-pm2k-600k-interleaved.pgn.vf
Total games: 646826
Total positions: 85545119
Positions after filtering: 24560470 (28.71%)
Positions/game: 38
Shortest game: 3 moves, longest game: 287 moves
Total time: 2 min 48 s
```

```
Test  | selfgen/58-nodefudging-5k (vs selfgen/54-nobook-10kn-647k)
Elo   | -5.54 +- 4.47 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 8600: +2220 -2357 =4023
Penta | [153, 1075, 1956, 988, 128]
https://openbench.lynx-chess.com/test/2974/
```

#### ❌ `selfgen/59-phases-random-order`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in **random** order

```json
Source file: 2942-10kn-1M-interleaved.pgn.vf
Total games: 849233
Total positions: 112379373
Positions after filtering: 32255874 (28.70%)
Positions/game: 38
Shortest game: 3 moves, longest game: 291 moves
Total time: 3 min 37 s
```

SPRT
```
Test  | selfgen/59-phases-random-order (vs selfgen/54-nobook-10kn)
Elo   | -0.56 +- 2.10 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 37162: +9544 -9604 =18014
Penta | [496, 4609, 8457, 4497, 522]
https://openbench.lynx-chess.com/test/2975/
```

#### 🟡 `selfgen/60-random-sampling-20pos`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, **20** pos/game **random** sampling

```json
Source file: 2942-10kn-1M-interleaved.pgn.vf
Total games: 849233
Total positions: 112379373
Positions after filtering: 16835116 (14.98%)
Positions/game: 20
Shortest game: 3 moves, longest game: 291 moves
Total time: 2 min 53 s
```

SPRT
```
Test  | selfgen/60-random-sampling-20pos (vs selfgen/54-nobook-10kn)
Elo   | 0.27 +- 1.37 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 87210: +22714 -22646 =41850
Penta | [1184, 10634, 19876, 10752, 1159]
https://openbench.lynx-chess.com/test/2976/
```

#### ❌ `selfgen/61-20-pos-per-game`

Same as `selfgen/29-limit-pos-per-game-20`, `selfgen/14-limit-pos-per-game-20` and `selfgen/7-limit-pos-per-game-20`.

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, **20** pos/game sampling alternating phases in descending order

```json
Source file: 2948-15kn-1M-interleaved.vf
Total games: 984489
Total positions: 121588547
Positions after filtering: 19386615 (15.94%)
Positions/game: 20
Shortest game: 3 moves, longest game: 332 moves
Total time: 3 min 13 s
```

SPRT
```
Test  | selfgen/61-20-pos-per-game (vs selfgen/62-book-15kn)
Elo   | -2.95 +- 3.41 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 14012: +3603 -3722 =6687
Penta | [210, 1700, 3282, 1627, 187]
https://openbench.lynx-chess.com/test/2985/
```

#### ✅ `selfgen/62-book-15kn`

Adopt 2948 dataset, generated using 15k nodes and `UHO_Lichess_4852_v1.epd` + 1-4 moves

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2948-15kn-1M-interleaved.vf
Total games: 984489
Total positions: 121588547
Positions after filtering: 36559232 (30.07%)
Positions/game: 37
Shortest game: 3 moves, longest game: 332 moves
Total time: 4 min 17 s
```

SPRT
```
Test  | selfgen/62-book-15kn (vs selfgen/51-book-10kn)
Elo   | 3.70 +- 2.59 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 25080: +6809 -6542 =11729
Penta | [361, 2945, 5674, 3186, 374]
https://openbench.lynx-chess.com/test/2982/
```

#### ❌ `selfgen/63-10-pos-per-game`

Same as `selfgen/26-10-pos-per-game`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, **10** pos/game sampling alternating phases in descending order

```json
Source file: 2948-15kn-1M-interleaved.vf
Total games: 984489
Total positions: 121588547
Positions after filtering: 9791802 (8.05%)
Positions/game: 10
Shortest game: 3 moves, longest game: 332 moves
Total time: 3 min 36 s
```

SPRT
```
Test  | selfgen/63-10-pos-per-game (vs selfgen/62-book-15kn)
Elo   | -2.66 +- 3.98 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -1.58 (-2.25, 2.89) [0.00, 3.00]
Games | 10848: +2844 -2927 =5077
Penta | [160, 1389, 2426, 1272, 177]
https://openbench.lynx-chess.com/test/2983/
```

#### ❌`selfgen/64-random-sampling-10pos`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, **10** pos/game **random** sampling

```json
Source file: 2948-15kn-1M-interleaved.vf
Total games: 984489
Total positions: 121588547
Positions after filtering: 9791802 (8.05%)
Positions/game: 10
Shortest game: 3 moves, longest game: 332 moves
Total time: 3 min 4 s
```

SPRT
```
Test  | selfgen/64-random-sampling-10pos (vs selfgen/62-book-15kn)
Elo   | -3.97 +- 3.87 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 11214: +2898 -3026 =5290
Penta | [175, 1411, 2549, 1311, 161]
https://openbench.lynx-chess.com/test/2986/
```

#### ✅ `selfgen/65-nobook-15kn`

Adopt 2981 dataset, generated using 15kn per move, no book

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2981-15kn-1M-interleaved.vf
Total games: 962961
Total positions: 127712642
Positions after filtering: 36642151 (28.69%)
Positions/game: 38
Shortest game: 3 moves, longest game: 319 moves
Total time: 4 min 8 s
```

SPRT
```
Test  | selfgen/65-nobook-15kn (vs selfgen/54-nobook-10kn)
Elo   | 4.21 +- 2.81 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 21318: +5682 -5424 =10212
Penta | [300, 2496, 4841, 2690, 332]
https://openbench.lynx-chess.com/test/2988/
```

#### ❌ `selfgen/66-phase-include-pawns`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases **(including pawns)** in descending order

```json
Source file: 2981-15kn-1M-interleaved.vf
Total games: 962961
Total positions: 127712642
Positions after filtering: 36642151 (28.69%)
Positions/game: 38
Shortest game: 3 moves, longest game: 319 moves
Total time: 4 min 52 s
```

SPRT
```
Test  | selfgen/66-phase-include-pawns
Elo   | -1.27 +- 2.55 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 24888: +6342 -6433 =12113
Penta | [355, 3019, 5732, 3038, 300]
https://openbench.lynx-chess.com/test/2994/
```

#### ❌ `selfgen/66-book-8-9-randomply`

Adopt 2989 dataset, generated applying 8-9 random moves on top of `UHO_Lichess_4852_v1.epd`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2989-5kn-1M-interleaved.pgn.vf
Total games: 1000214
Total positions: 113029990
Positions after filtering: 35253148 (31.19%)
Positions/game: 35
Shortest game: 3 moves, longest game: 279 moves
Total time: 3 min 57 s
```

SPRT
```
Test  | selfgen/66-book-8-9-randomply (vs selfgen/42-book-datagen-1-4)
Elo   | -14.36 +- 7.04 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [0.00, 3.00]
Games | 3800: +963 -1120 =1717
Penta | [86, 535, 787, 434, 58]
https://openbench.lynx-chess.com/test/2996/
```

```
Test  | selfgen/66-book-8-9-randomply (vs selfgen/42-book-datagen-1-4 using a different book than the one used for training)
Elo   | -16.28 +- 9.15 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -1.47 (-2.25, 2.89) [0.00, 3.00]
Games | 2136: +535 -635 =966
Penta | [50, 283, 482, 223, 30]
https://openbench.lynx-chess.com/test/2997/
```

#### ✅ `selfgen/67-nobook-20kn`

Adopt 2995 dataset, generated using 20kn per move, no book

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2995-20kn-1M-interleaved.pgn.vf
Total games: 948766
Total positions: 126190156
Positions after filtering: 36170057 (28.66%)
Positions/game: 38
Shortest game: 3 moves, longest game: 323 moves
Total time: 4 min 10 s
```

SPRT
```
Test  | selfgen/67-nobook-20kn (vs selfgen/65-nobook-15kn)
Elo   | 2.99 +- 2.22 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.92 (-2.25, 2.89) [0.00, 3.00]
Games | 33420: +8855 -8567 =15998
Penta | [450, 3974, 7599, 4212, 475]
https://openbench.lynx-chess.com/test/3000/
```

#### `selfgen/67-nobook-20kn-250k`

PoC using first 250k games of 2995 dataset vs the first 250k games of 2981 dataset.

This proves that this approach can be used to avoid having to generate 1M games at higher node counts

```json
Source file: 2995-20kn-1M-interleaved.pgn.vf
Total games: 250000
Total positions: 33231145
Positions after filtering: 9528414 (28.67%)
Positions/game: 38
Shortest game: 4 moves, longest game: 272 moves
Total time: 1 min 7 s
```

vs

```json
Source file: 2981-15kn-1M-interleaved.vf
Total games: 250000
Total positions: 33135335
Positions after filtering: 9510551 (28.70%)
Positions/game: 38
Shortest game: 4 moves, longest game: 294 moves
Total time: 1 min 17 s
```

SPRT
```
Test  | selfgen/67-nobook-20kn-250k (vs selfgen/65-nobook-15kn-250k)
Elo   | 8.81 +- 4.41 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.89 (-2.25, 2.89) [0.00, 3.00]
Games | 9154: +2628 -2396 =4130
Penta | [134, 1049, 2037, 1165, 192]
https://openbench.lynx-chess.com/test/3010/
```

#### ❌ `selfgen/68-nobook-30kn-250k`

Adopt 3006 dataset, generated using 30kn per move, no book

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3006-30kn-250k-interleaved.pgn.vf
Total games: 250000
Total positions: 33432249
Positions after filtering: 9548381 (28.56%)
Positions/game: 38
Shortest game: 3 moves, longest game: 282 moves
Total time: 1 min 6 s
```

SPRT
```
Test  | selfgen/68-nobook-30kn-250k (vs selfgen/67-nobook-20kn-250k)
Elo   | -4.65 +- 4.23 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 10166: +2742 -2878 =4546
Penta | [198, 1289, 2224, 1195, 177]
https://openbench.lynx-chess.com/test/3007/
```

#### 🟡 `selfgen/69-nobook-25kn-250k`

Adopt 3009 dataset, generated using 25kn per move, no book

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3009-25kn-250k-interleaved.vf
Total games: 238572
Total positions: 31814931
Positions after filtering: 9107552 (28.63%)
Positions/game: 38
Shortest game: 4 moves, longest game: 269 moves
Total time: 1 min 10 s
```

vs

```json
Source file: 2995-20kn-1M-interleaved.pgn.vf
Total games: 238572
Total positions: 31710521
Positions after filtering: 9092737 (28.67%)
Positions/game: 38
Shortest game: 4 moves, longest game: 272 moves
Total time: 1 min 15 s
```

SPRT
```
Test  | selfgen/69-nobook-25kn-250k (vs selfgen/67-nobook-20kn-238k)
Elo   | -0.15 +- 1.88 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 52276: +14464 -14487 =23325
Penta | [1040, 6324, 11426, 6315, 1033]
https://openbench.lynx-chess.com/test/3012/
```

#### 🟡 `selfgen/70-book-20kn`

Adopt 2991 dataset, generated using 20k nodes and `UHO_Lichess_4852_v1.epd` + 1-4 moves

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 2991-20kn-250M-interleaved.vf
Total games: 250000
Total positions: 30905443
Positions after filtering: 9298665 (30.09%)
Positions/game: 37
Shortest game: 3 moves, longest game: 279 moves
Total time: 1 min 1 s
```

vs

```json
Source file: 2948-15kn-1M-interleaved.vf
Total games: 250000
Total positions: 30902549
Positions after filtering: 9287968 (30.06%)
Positions/game: 37
Shortest game: 3 moves, longest game: 332 moves
Total time: 1 min 10 s
```

SPRT
```
Test  | selfgen/70-book-20kn-250k
Elo   | 0.02 +- 1.72 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.28 (-2.25, 2.89) [0.00, 3.00]
Games | 62584: +17272 -17268 =28044
Penta | [1222, 7556, 13758, 7508, 1248]
https://openbench.lynx-chess.com/test/3022/
```

#### ❌ `selfgen/71-book-datagen-4-9`

Adopt 3032 dataset, generated using 5k nodes and `UHO_Lichess_4852_v1.epd` + 4-9 moves

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3032/3032-5kn-interleaved.vf
Total games: 1000602
Total positions: 116398692
Positions after filtering: 35824002 (30.78%)
Positions/game: 36
Shortest game: 2 moves, longest game: 285 moves
Total time: 3 min 32 s
```

SPRT using `Pohl.pgn`

```
Test  | selfgen/71-book-datagen-4-9 (vs selfgen/42-book-datagen-1-4, using Pohl)
Elo   | -9.34 +- 5.55 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.30 (-2.25, 2.89) [0.00, 3.00]
Games | 5430: +1379 -1525 =2526
Penta | [85, 717, 1253, 579, 81]
https://openbench.lynx-chess.com/test/3037/
```

```
Test  | selfgen/71-book-datagen-4-9 (vs selfgen/42-book-datagen-1-4, using UHO_Lichess_4852_v1.epd)
Elo   | -9.62 +- 5.72 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 5240: +1322 -1467 =2451
Penta | [102, 664, 1203, 579, 72]
https://openbench.lynx-chess.com/test/3036/
```

#### ❌ `selfgen/72-book-datagen-1-6`

Adopt 3043 dataset, generated using 5k nodes and `UHO_Lichess_4852_v1.epd` + 1-6 moves

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3043-5kn-1M-interleaved.vf
Total games: 1000602
Total positions: 121249993
Positions after filtering: 36598663 (30.18%)
Positions/game: 37
Shortest game: 3 moves, longest game: 306 moves
Total time: 3 min 32 s
```

SPRT (non-reg, following the logic of more moves == more variety long term)
```
Test  | selfgen/72-book-datagen-1-6 (vs selfgen/42-book-datagen-1-4, using Pohl)
Elo   | -10.46 +- 6.38 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.27 (-2.25, 2.89) [-3.00, 1.00]
Games | 4252: +1076 -1204 =1972
Penta | [75, 569, 954, 465, 63]
https://openbench.lynx-chess.com/test/3045/
```

SPRT (non-reg, following the logic of more moves == more variety long term)

#### ✅ `selfgen/73-nobook-20kn-2M`

3020 + 3030 datasets, both no book. Double checking that moar data (aka more random skipping) is good

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3020_3030-20kn-2M-interleaved.vf
Total games: 2056175
Games after filtering: 2054321 (99.91%)
Total positions: 272325405
Positions after filtering: 78319960 (28.76%)
Positions/game: 38
Shortest game: 11 moves, longest game: 303 moves
Total time: 8 min 14 s
```

vs only 3020 dataset (~1M pos)

```json
Source file: 3020-20kn-1M-interleaved.vf
Total games: 1074481
Games after filtering: 1073504 (99.91%)
Total positions: 142312426
Positions after filtering: 40928648 (28.76%)
Positions/game: 38
Shortest game: 11 moves, longest game: 292 moves
Total time: 4 min 20 s
```

SPRT
```
Test  | selfgen/73-nobook-20kn-2M (vs selfgen/73-nobook-20kn-1M)
Elo   | 5.20 +- 3.18 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 15898: +4221 -3983 =7694
Penta | [200, 1820, 3681, 2038, 210]
https://openbench.lynx-chess.com/test/3049/
```

#### ✅ `selfgen/74-book-nobook-20kn-2M`

3020 + 3046 datasets (no book 20kn + book 15kn). Double checking that `book + no book` > `2 x no book`

```json
Source file: 3020-20kn_3046-15kn-2M-interleaved.vf
Total games: 2056175
Games after filtering: 2053348 (99.86%)
Total positions: 262595242
Positions after filtering: 77290169 (29.43%)
Positions/game: 38
Shortest game: 11 moves, longest game: 294 moves
Total time: 7 min 41 s
```

SPRT vs `selfgen/73-nobook-20kn-2` (3020 + 3030 datasets, both no book 20kn)
```
Test  | selfgen/74-book-nobook-20kn-2M (vs selfgen/73-nobook-20kn-2)
Elo   | 3.21 +- 2.34 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 29660: +7787 -7513 =14360
Penta | [379, 3493, 6839, 3713, 406]
https://openbench.lynx-chess.com/test/3050/
```

#### 🟡 `selfgen/75-book-20kn`

Adopt 3048 dataset, generated using 20k nodes and `UHO_Lichess_4852_v1.epd` + 1-4 moves
This double checks the methodology of `selfgen/70-book-20kn`, using 1M 20kn games vs 1M 15kn games instead if 250k generated games vs the first 250k games of the existing 1M 15kn games

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3048-20kn-1M-interleaved.vf
Total games: 1024248
Games after filtering: 1022433 (99.82%)
Total positions: 126110873
Positions after filtering: 38049050 (30.17%)
Positions/game: 37
Shortest game: 11 moves, longest game: 314 moves
Total time: 4 min 17 s
```

vs 3046 dataset (book 15kn) - `selfgen/75-book-15kn`

```json
Source file: 3046-15kn-1M-interleaved.vf
Total games: 1024248
Games after filtering: 1022360 (99.82%)
Total positions: 125962315
Positions after filtering: 37987677 (30.16%)
Positions/game: 37
Shortest game: 11 moves, longest game: 294 moves
Total time: 4 min 3 s
```

SPRT
```
Test  | selfgen/75-book-20kn
Elo   | 0.39 +- 1.40 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -1.81 (-2.25, 2.89) [0.00, 3.00]
Games | 85424: +22427 -22331 =40666
Penta | [1246, 10397, 19339, 10475, 1255]
https://openbench.lynx-chess.com/test/3054/
```

#### ✅ `selfgen/76-nobook-20kn-book-15kn-mix-4M` -> `lynx-1.0`

3020 + 3030 (no book, 20kn) + 3046 + 3047 (`UHO_Lichess_4852_v1.epd`, 15kn) datasets

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3020-3030-20kn_3046-3047-15kn-4M-interleaved.vf
Total games: 4247458
Games after filtering: 4241571 (99.86%)
Total positions: 541854023
Positions after filtering: 159593873 (29.45%)
Positions/game: 38
Shortest game: 11 moves, longest game: 311 moves
Total time: 17 min 21 s
```

SPRT
```
Test  | selfgen/76-nobook-20kn-book-15kn-mix-4M (vs selfgen/74-book-nobook-20kn-2M)
Elo   | 9.44 +- 4.49 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.89 (-2.25, 2.89) [0.00, 3.00]
Games | 8100: +2168 -1948 =3984
Penta | [95, 921, 1814, 1109, 111]
https://openbench.lynx-chess.com/test/3055/
```

Progress test
```
Test  | selfgen/76-nobook-20kn-book-15kn-mix-4M (vs eval/only-lichessbig3-2)
Elo   | 18.29 +- 5.92 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5000: +1498 -1235 =2267
Penta | [61, 530, 1101, 701, 107]
https://openbench.lynx-chess.com/test/3056/
```

Progress test after retuning using 17M positions (roughly the size of the current dataset)
```
Test  | selfgen/76-nobook-20kn-book-15kn-mix-4M-17M (vs main)
Elo   | 3.40 +- 5.79 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5000: +1359 -1310 =2331
Penta | [73, 584, 1139, 629, 75]
https://openbench.lynx-chess.com/test/3058/
```

```
Test  | selfgen/76-nobook-20kn-book-15kn-mix-4M-17M (vs main)
Elo   | 5.27 +- 7.41 (95%)
Conf  | 40.0+0.40s Threads=1 Hash=128MB
Games | 2504: +624 -586 =1294
Penta | [23, 269, 621, 325, 14]
https://openbench.lynx-chess.com/test/3060/
```

Logically, this sucks at DFRC, given we are removing all the DFRC data

```
Test  | selfgen/76-nobook-20kn-book-15kn-mix-4M-17M
Elo   | -83.59 +- 16.14 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 1004: +192 -429 =383
Penta | [80, 168, 175, 67, 12]
https://openbench.lynx-chess.com/test/3082/
```

Using 12.43M and keeping external DFRC data

DFRC
```
Test  | selfgen/76-nobook-20kn-book-15kn-mix-4M-12.4M (vs main)
Elo   | 8.82 +- 6.30 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5006: +1468 -1341 =2197
Penta | [104, 574, 1039, 663, 123]
https://openbench.lynx-chess.com/test/3083/
```

Standard
```
Test  | selfgen/76-nobook-20kn-book-15kn-mix-4M-12.4M
Elo   | -1.25 +- 5.72 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5012: +1284 -1302 =2426
Penta | [77, 602, 1151, 614, 62]
https://openbench.lynx-chess.com/test/3084/
```

#### 🟡 `selfgen/77-nobook-20kn-book-15kn-20kn-mix-6M`

3020 + 3030 + 3053 (no book, 20kn) + 3046 + 3047 (`UHO_Lichess_4852_v1.epd`, 15kn) + 3048 (`UHO_Lichess_4852_v1.epd`, 20kn) datasets

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3020-3030-3046-3047-3048-3053-6M-interleaved.vf
Total games: 6304673
Games after filtering: 6296002 (99.86%)
Total positions: 804732441
Positions after filtering: 236972963 (29.45%)
Positions/game: 38
Shortest game: 11 moves, longest game: 336 moves
Total time: 27 min 4 s
```

SPRT
```
Test  | selfgen/77-nobook-20kn-book-15kn-20kn-mix-6M (vs selfgen/76-nobook-20kn-book-15kn-mix-4M)
Elo   | 0.48 +- 2.19 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -0.59 (-2.25, 2.89) [0.00, 3.00]
Games | 34176: +8924 -8877 =16375
Penta | [479, 4138, 7788, 4223, 460]
https://openbench.lynx-chess.com/test/3063/
```

After retuning using 17M positions (roughly the size of the current dataset)
```
Test  | selfgen/77-nobook-20kn-book-15kn-20kn-mix-6M-17M (vs selfgen/76-nobook-20kn-book-15kn-mix-4M-17M)
Elo   | -0.65 +- 3.56 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -0.80 (-2.25, 2.89) [0.00, 3.00]
Games | 12312: +3091 -3114 =6107
Penta | [154, 1477, 2907, 1474, 144]
https://openbench.lynx-chess.com/test/3065/
```

#### 🟡 `selfgen/78-nobook-20kn-book-15kn-20kn-mix-8M-17M` -> `lynx-1.0-extended`

3020 + 3030 + 3053 + 3061 (no book, 20kn) + 3046 + 3047 (`UHO_Lichess_4852_v1.epd`, 15kn) + 3048 + 3062 (`UHO_Lichess_4852_v1.epd`, 20kn) datasets

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3020-3030-3046-3047-3048-3053-3061-3062-8M-interleaved.vf
Total games: 8356728
Games after filtering: 8345164 (99.86%)
Total positions: 1066630687
Positions after filtering: 314129778 (29.45%)
Positions/game: 38
Shortest game: 11 moves, longest game: 336 moves
Total time: 34 min 14 s
```

SPRT
```
Test  | selfgen/78-nobook-20kn-book-15kn-20kn-mix-8M-17M (vs selfgen/76-nobook-20kn-book-15kn-mix-4M-17M)
Elo   | 0.38 +- 1.21 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.26 (-2.25, 2.89) [0.00, 3.00]
Games | 107404: +27330 -27213 =52861
Penta | [1257, 13022, 25044, 13105, 1274]
https://openbench.lynx-chess.com/test/3077/
```

#### ❌ `selfgen/79-nobook-20kn-book-20kn-mix-4M-17M`

3020 + 3030 + (no book, 20kn) + 3048 + 3062 (`UHO_Lichess_4852_v1.epd`, 20kn) datasets
This essentially replaces 15kn book data with 20kn one

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3020-3030-20kn_3048-3062-20kn-4M-interleaved.vf
Total games: 4151045
Games after filtering: 4145414 (99.86%)
Total positions: 530294678
Positions after filtering: 156143844 (29.44%)
Positions/game: 38
Shortest game: 11 moves, longest game: 332 moves
Total time: 18 min 10 s
```

SPRT
```
Test  | selfgen/79-nobook-20kn-book-20kn-mix-4M-17M (vs selfgen/76-nobook-20kn-book-15kn-mix-4M-17M)
Elo   | -2.23 +- 3.03 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.31 (-2.25, 2.89) [0.00, 3.00]
Games | 17572: +4488 -4601 =8483
Penta | [240, 2166, 4073, 2081, 226]
https://openbench.lynx-chess.com/test/3079/
```

#### `selfgen/80-frc-book-5k`

Adopt 3067 dataset (`DFRC_4852_v1.epd` + 1-4 moves, 5kn)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3067-frc-5kn-250K-interleaved.vf
Total games: 250000
Games after filtering: 249711 (99.88%)
Total positions: 32905628
Positions after filtering: 9400065 (28.57%)
Positions/game: 38
Shortest game: 11 moves, longest game: 291 moves
Total time: 1 min 5 s
```

SPRT
```
Test  | selfgen/80-frc-book-5k (vs eval/only-dfrc)
Elo   | -30.35 +- 7.01 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5004: +1390 -1826 =1788
Penta | [265, 636, 995, 482, 124]
https://openbench.lynx-chess.com/test/3092/
```

Mixed with standard data from `main`
```
Test  | selfgen/80-frc-book-5k-with-std (vs main)
Elo   | -3.47 +- 6.46 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5004: +1409 -1459 =2136
Penta | [129, 632, 1032, 578, 131]
https://openbench.lynx-chess.com/test/3088/
```

#### ❌ `selfgen/81-frc-nobook-5k`

Adopt 3071 dataset (`DFRC.epd` + 8-9 moves, 5kn), which contains all the possible DFRC starting positions (so it's the equivalent of no-book)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending orde

```json
Source file: 3071-frc-5kn-250K-interleaved.vf_sanitised
Total games: 250000
Games after filtering: 249567 (99.83%)
Total positions: 31832132
Positions after filtering: 9260240 (29.09%)
Positions/game: 37
Shortest game: 11 moves, longest game: 292 moves
Total time: 56.20 s
```

SPRT
```
Test  | selfgen/81-frc-nobook-5k (vs selfgen/80-frc-book-5k)
Elo   | -78.87 +- 17.36 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.29 (-2.25, 2.89) [0.00, 3.00]
Games | 932: +200 -408 =324
Penta | [77, 153, 151, 71, 14]
https://openbench.lynx-chess.com/test/3091/
```

#### ✅ `selfgen/82-frc-book-15k`

Adopt 3089 dataset (`DFRC_4852_v1.epd` + 1-4 moves, 15kn)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3089-frc-15kn-1M-interleaved.vf_sanitised
Total games: 1000000
Games after filtering: 999087 (99.91%)
Total positions: 132811624
Positions after filtering: 37898141 (28.54%)
Positions/game: 38
Shortest game: 11 moves, longest game: 299 moves
Total time: 4 min 11 s
```

SPRT
```
Test  | selfgen/82-frc-book-15k (vs selfgen/80-frc-book-5k)
Elo   | 60.22 +- 12.73 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 1480: +588 -334 =558
Penta | [22, 121, 262, 251, 84]
https://openbench.lynx-chess.com/test/3094/
```

Mixed with standard data from `main`
```
Test  | selfgen/82-frc-book-15k-with-std (vs main)
Elo   | 1.74 +- 14.21 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 1000: +281 -276 =443
Penta | [22, 124, 205, 125, 24]
https://openbench.lynx-chess.com/test/3098/
```

#### 🟡 `selfgen/83-frc-book-20k`

Adopt 3097 dataset (`DFRC_4852_v1.epd` + 1-4 moves, 20kn)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3097-frc-20kn-470K-interleaved.vf
Total games: 473289
Games after filtering: 472917 (99.92%)
Total positions: 63008544
Positions after filtering: 17961263 (28.51%)
Positions/game: 38
Shortest game: 11 moves, longest game: 284 moves
Total time: 2 min 1 s
```

vs 3089 473K
```json
Source file: 3089-frc-15kn-1M-interleaved.vf_sanitised
Total games: 473289
Games after filtering: 472854 (99.91%)
Total positions: 62915098
Positions after filtering: 17938267 (28.51%)
Positions/game: 38
Shortest game: 11 moves, longest game: 299 moves
Total time: 4 min 23 s
```

```
Test  | selfgen/83-frc-book-20k
Elo   | -0.10 +- 2.81 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -1.12 (-2.25, 2.89) [0.00, 3.00]
Games | 27294: +7991 -7999 =11304
Penta | [770, 3278, 5535, 3318, 746]
https://openbench.lynx-chess.com/test/3101/
```

#### ✅ `selfgen/84-frc-book-15k-fix`

Adopt 3102 dataset (`DFRC_4852_v1.epd` + 1-4 moves, 15kn), which includes a `genfens` fix

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3102-frc-15kn-1M-interleaved.vf
Total games: 1000000
Games after filtering: 999104 (99.91%)
Total positions: 132700147
Positions after filtering: 37898544 (28.56%)
Positions/game: 38
Shortest game: 11 moves, longest game: 309 moves
Total time: 3 min 60 s
```

SPRT
```
Test  | selfgen/84-frc-book-15k-fix (vs selfgen/82-frc-book-15k)
Elo   | 0.88 +- 4.85 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 1.37 (-2.25, 2.89) [-5.00, 0.00]
Games | 8652: +2491 -2469 =3692
Penta | [199, 1066, 1798, 1040, 223]
https://openbench.lynx-chess.com/test/3112/
```

#### ✅ `selfgen/85-frc-book-15k-2M`

3102 + 3103 datasets (`DFRC_4852_v1.epd` + 1-4 moves, 15kn)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3102-3103-2M-interleaved.vf
Total games: 2000000
Games after filtering: 1998176 (99.91%)
Total positions: 265465786
Positions after filtering: 75797082 (28.55%)
Positions/game: 38
Shortest game: 11 moves, longest game: 309 moves
Total time: 7 min 0 s
```

SPRT
```
Test  | selfgen/85-frc-book-15k-2M (vs selfgen/84-frc-book-15k-fix)
Elo   | 6.07 +- 3.64 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 15066: +4374 -4111 =6581
Penta | [321, 1767, 3160, 1898, 387]
https://openbench.lynx-chess.com/test/3117/
```

#### ✅ `selfgen/86-frc-book-15k-4M` -> `lynx-dfrc-1.0`

3102 + 3103 + 3115 + 3116 datasets (`DFRC_4852_v1.epd` + 1-4 moves, 15kn)

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order

```json
Source file: 3102-3103-3115-3116-4M-interleaved.vf
Total games: 4000000
Games after filtering: 3996393 (99.91%)
Total positions: 530959980
Positions after filtering: 151596145 (28.55%)
Positions/game: 38
Shortest game: 11 moves, longest game: 330 moves
Total time: 12 min 59 s
```

SPRT
```
Test  | selfgen/86-frc-book-15k-4M (vs selfgen/85-frc-book-15k-2M)
Elo   | 2.67 +- 2.10 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.91 (-2.25, 2.89) [0.00, 3.00]
Games | 46004: +13191 -12837 =19976
Penta | [1097, 5446, 9607, 5710, 1142]
https://openbench.lynx-chess.com/test/3119/
```

Progress test
```
Test  | selfgen/86-frc-book-15k-4M (vs only-dfrc)
Elo   | 29.35 +- 6.59 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5008: +1703 -1281 =2024
Penta | [82, 523, 999, 691, 209]
https://openbench.lynx-chess.com/test/3139/
```

DFRC
```
Test  | selfgen/86-frc-book-15k-4M-with-std (vs main)
Elo   | 14.66 +- 6.31 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5002: +1475 -1264 =2263
Penta | [99, 543, 1038, 690, 131]
https://openbench.lynx-chess.com/test/3142/
```

Standard
```
Test  | selfgen/86-frc-book-15k-4M-with-std
Elo   | 3.40 +- 5.64 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 5002: +1288 -1239 =2475
Penta | [64, 581, 1157, 640, 59]
https://openbench.lynx-chess.com/test/3143/
```

#### ❌ `selfgen/88-eval-mismatch-47-400`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order, **eval mismatch < 0.45 w/ eval scale 400**

```json
Source file: 3102-3103-3115-3116-4M-interleaved.vf
Total games: 4000000
Games after filtering: 1618112 (40.45%)
Total positions: 530959980
Positions after filtering: 47890401 (9.02%)
Positions/game: 30
Shortest game: 11 moves, longest game: 330 moves
Total time: 7 min 21 s
```

SPRT
```
Test  | selfgen/87-eval-mismatch-47-400 (vs selfgen/86-frc-book-15k-4M)
Elo   | -1199.83 +- 0.00 (95%)
Conf  | 8.0+0.08s Threads=1 Hash=32MB
Games | 508: +0 -508 =0
Penta | [254, 0, 0, 0, 0]
https://openbench.lynx-chess.com/test/3122/
```

#### ❌ `selfgen/88-eval-mismatch-90-400`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order, **eval mismatch < 0.90 w/ eval scale 400**

```json
Source file: 3102-3103-3115-3116-4M-interleaved.vf
Total games: 4000000
Games after filtering: 3986040 (99.65%)
Total positions: 530959980
Positions after filtering: 143750556 (27.07%)
Positions/game: 36
Shortest game: 11 moves, longest game: 330 moves
Total time: 12 min 41 s
```

SPRT
```
Test  | selfgen/88-eval-mismatch-90-400 (vs selfgen/86-frc-book-15k-4M)
Elo   | -0.22 +- 1.98 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.28 (-2.25, 2.89) [0.00, 3.00]
Games | 49990: +13846 -13877 =22267
Penta | [1131, 6050, 10646, 6055, 1113]
https://openbench.lynx-chess.com/test/3124/
```

#### 🟡 `selfgen/88-eval-mismatch-80-400`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order, **eval mismatch < 0.80 w/ eval scale 400**

```json
Source file: 3102-3103-3115-3116-4M-interleaved.vf
Total games: 4000000
Games after filtering: 3912677 (97.82%)
Total positions: 530959980
Positions after filtering: 133304474 (25.11%)
Positions/game: 34
Shortest game: 11 moves, longest game: 330 moves
Total time: 12 min 10 s
```

SPRT
```
Test  | selfgen/88-eval-mismatch-80-400
Elo   | 0.78 +- 1.43 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -0.73 (-2.25, 2.89) [0.00, 3.00]
Games | 96834: +27263 -27045 =42526
Penta | [2233, 11667, 20396, 11891, 2230]
https://openbench.lynx-chess.com/test/3125/
```

#### ❌ `selfgen/88-eval-mismatch-70-400`

MinPly 20, MinPieces 7, MaxEval 20000, filter checks and tactical, 40 pos/game sampling alternating phases in descending order, **eval mismatch < 0.70 w/ eval scale 400**

```json
Source file: 3102-3103-3115-3116-4M-interleaved.vf
Total games: 4000000
Games after filtering: 3724922 (93.12%)
Total positions: 530959980
Positions after filtering: 116240281 (21.89%)
Positions/game: 31
Shortest game: 11 moves, longest game: 330 moves
Total time: 12 min 13 s
```

SPRT
```
Test  | selfgen/88-eval-mismatch-70-400 (vs selfgen/86-frc-book-15k-4M)
Elo   | -12.88 +- 6.44 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.25 (-2.25, 2.89) [0.00, 3.00]
Games | 3886: +952 -1096 =1838
Penta | [68, 505, 913, 417, 40]
https://openbench.lynx-chess.com/test/3128/
```

#### 🟡 `eval/selfgen-lynx-v1`

```csv
lynx-1.0.epd,0,12430000
lynx-dfrc-1.0.epd,0,4570000
```

Progress test by itself

Standard
```
Test  | eval/selfgen-lynx-v1 (vs main)
Elo   | -1.12 +- 2.49 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | -2.28 (-2.25, 2.89) [0.00, 3.00]
Games | 27412: +7186 -7274 =12952
Penta | [458, 3336, 6114, 3432, 366]
https://openbench.lynx-chess.com/test/3132/
```

Standard non-reg
```
Test  | eval/selfgen-lynx-v1 (vs main)
Elo   | -1.92 +- 2.26 (95%)
SPRT  | 40.0+0.40s Threads=1 Hash=128MB
LLR   | -2.25 (-2.25, 2.89) [-3.00, 1.00]
Games | 26982: +6492 -6641 =13849
Penta | [212, 3296, 6565, 3265, 153]
https://openbench.lynx-chess.com/test/3134/
```

DFRC
```
Test  | eval/selfgen-lynx-v1 (vs main)
Elo   | 10.89 +- 5.13 (95%)
SPRT  | 8.0+0.08s Threads=1 Hash=32MB
LLR   | 2.90 (-2.25, 2.89) [0.00, 3.00]
Games | 7946: +2445 -2196 =3305
Penta | [187, 888, 1606, 1073, 219]
https://openbench.lynx-chess.com/test/3133/
```

</details>
