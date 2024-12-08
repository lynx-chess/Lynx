# Lynx

[![Lynx build][buildlogo]][buildlink]
[![Lynx release][releaselogo]][releaselink]

## Introduction

<img align="right" width="200" height="200" src="resources/lynx.png">

Lynx is a chess engine developed by [@eduherminio](https://github.com/eduherminio).

It's written in C# (.NET 9).

You can find Lynx:

- As a [lichess bot](https://lichess.org/@/Lynx_BOT).

- As a self-contained executable, downloadable from [Releases](https://github.com/lynx-chess/Lynx/releases).

Lichess bot can be played directly, but a chess GUI that supports UCI protocol is needed to play against the self-contained version.

## Strength

See [Releases](https://github.com/lynx-chess/Lynx/releases) for the complete list of versions.

Here are the ones 'properly' rated over at least a few hundred of games:

| Version | Date | Estimated<br>elo<sup>0</sup>| [CCRL<br>40/15](https://www.computerchess.org.uk/ccrl/4040/cgi/compare_engines.cgi?class=Single-CPU+engines&only_best_in_class=on&num_best_in_class=1&print=Rating+list) | [CCRL<br>Blitz](https://www.computerchess.org.uk/ccrl/404/cgi/compare_engines.cgi?class=Single-CPU+engines&only_best_in_class=on&num_best_in_class=1&print=Rating+list) | [MCERL](https://www.chessengeria.eu/mcerl) | [CEGT<br>40/20](http://www.cegt.net/40_40%20Rating%20List/40_40%20All%20Versions/rangliste.html) | [CEGT<br>40/4](http://www.cegt.net/40_4_Ratinglist/40_4_AllVersion/rangliste.html) | [CEGT<br>5+3 pb](http://www.cegt.net/5Plus3Rating/5Plus3AllVersion/rangliste.html) |
|---|---|---|---|---|---|---|---|---|
| [1.7.0](https://github.com/lynx-chess/Lynx/releases/tag/v1.7.0) | 2024-10-05 | [3101](https://github.com/lynx-chess/Lynx/commit/06da9363b7f38dce5690e8c2c0dcd2914cdfaa30#commitcomment-147596793) | [3111](https://www.computerchess.org.uk/ccrl/4040/cgi/engine_details.cgi?print=Details&each_game=0&eng=Lynx%201.7.0%2064-bit#Lynx_1_7_0_64-bit) | [3127](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%201.7.0%2064-bit#Lynx_1_7_0_64-bit) | 3206 | 2974 | 2936 | 2959 |
| [1.6.0](https://github.com/lynx-chess/Lynx/releases/tag/v1.6.0) | 2024-08-15 | [2952](https://github.com/lynx-chess/Lynx/commit/a230d0518bf2743ec0dd27931928719e43ac5334#commitcomment-145399551) | [2981](https://www.computerchess.org.uk/ccrl/4040/cgi/engine_details.cgi?print=Details&each_game=0&eng=Lynx%201.6.0%2064-bit#Lynx_1_6_0_64-bit)* |  | 3039 |  |  |  |
| [1.5.1](https://github.com/lynx-chess/Lynx/releases/tag/v1.5.1) | 2024-06-21 | [2830](https://github.com/lynx-chess/Lynx/commit/47e7b8799cfac433c1004213e51daf35ae0fcd97#commitcomment-143384223) | [2851](https://www.computerchess.org.uk/ccrl/4040/cgi/engine_details.cgi?print=Details&each_game=0&eng=Lynx%201.5.1%2064-bit#Lynx_1_5_1_64-bit) |  |  |  | 2660 | 2690 |
| [1.5.0](https://github.com/lynx-chess/Lynx/releases/tag/v1.5.0) | 2024-06-09 | [2817](https://github.com/lynx-chess/Lynx/commit/70f23d96a2789ef22440cd0955a8b9557eb2682f#commitcomment-142930835) |  | [2817](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%201.5.0%2064-bit#Lynx_1_5_0_64-bit) |  |  |  |  |
| [1.4.0](https://github.com/lynx-chess/Lynx/releases/tag/v1.4.0) | 2024-03-21 | [2747](https://github.com/lynx-chess/Lynx/commit/70a81b9d08482c691b8c8cd6885e3e1eaf2c16b2#commitcomment-140146920) | [2753](https://www.computerchess.org.uk/ccrl/4040/cgi/engine_details.cgi?print=Details&each_game=0&eng=Lynx%201.4.0%2064-bit#Lynx_1_4_0_64-bit) |  |  |  |  |  |
| [1.3.0](https://github.com/lynx-chess/Lynx/releases/tag/v1.3.0) | 2024-02-04 | [2651](https://github.com/lynx-chess/Lynx/commit/1f2384804f69ad68a58a5d363225a809b7c1b0d9#commitcomment-138257203) | [2682](https://www.computerchess.org.uk/ccrl/4040/cgi/engine_details.cgi?print=Details&each_game=0&eng=Lynx%201.3.0%2064-bit#Lynx_1_3_0_64-bit) | [2653](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%201.3.0%2064-bit#Lynx_1_3_0_64-bit) | 2831 |  |  |  |
| [1.2.0](https://github.com/lynx-chess/Lynx/releases/tag/v1.2.0) | 2024-01-11 | [2611](https://github.com/lynx-chess/Lynx/commit/38f0d147fe049c89e56e6ea66ce28f9fa29907c2) / [2551](https://github.com/lynx-chess/Lynx/commit/38f0d147fe049c89e56e6ea66ce28f9fa29907c2#commitcomment-137001006)<sup>1</sup> | [2586](https://www.computerchess.org.uk/ccrl/4040/cgi/engine_details.cgi?print=Details&each_game=0&eng=Lynx%201.2.0%2064-bit#Lynx_1_2_0_64-bit) |  | 2862 |  |  |  |
| [1.1.0](https://github.com/lynx-chess/Lynx/releases/tag/v1.1.0) | 2023-12-14 | [2533](https://github.com/lynx-chess/Lynx/commit/b7d0131909977fe7c398f70e29daf3dc02f9fdcb#commitcomment-134947966) | [2507](https://www.computerchess.org.uk/ccrl/4040/cgi/engine_details.cgi?print=Details&each_game=0&eng=Lynx%201.1.0%2064-bit#Lynx_1_1_0_64-bit) | [2426](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%201.1.0%2064-bit#Lynx_1_1_0_64-bit) | 2597 |  |  |  |
| [1.0.1](https://github.com/lynx-chess/Lynx/releases/tag/v1.0.1) | 2023-11-20 | [2511](https://github.com/lynx-chess/Lynx/commit/66d340232298768bba57d6876f59831645a6dffb#commitcomment-132727293) | [2433](https://www.computerchess.org.uk/ccrl/4040/cgi/engine_details.cgi?print=Details&each_game=0&eng=Lynx%201.0.1%2064-bit#Lynx_1_0_1_64-bit) | [2430](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%201.0.1%2064-bit#Lynx_1_0_1_64-bit) | 2566 |  |  |  |
| [0.19.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.19.0) | 2023-10-27 | [2434](https://github.com/lynx-chess/Lynx/commit/b42d235a2815ddb989c5d83218750167c43be7bb#commitcomment-131057706) |  | [2348](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.19.0%2064-bit#Lynx_0_19_0_64-bit) | 2508 |  |  |  |
| [0.18.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.18.0) | 2023-10-21 | [2283](https://github.com/lynx-chess/Lynx/commit/3397c86c27bccb521f08306564325ff3cd64335d#commitcomment-130585961) |  |  | 2395 |  |  |  |
| [0.17.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.17.0) | 2023-09-19 | [2178](https://github.com/lynx-chess/Lynx/commit/ecd462bf48923deb7fe7449ff74da3bcc8afe75c#commitcomment-127755063) |  |  | 2368 |  |  |  |
| [0.16.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.16.0) | 2023-08-26 | [2053](https://github.com/lynx-chess/Lynx/commit/8743436f4e0cca508dc9fd419a5498c46f15866c#commitcomment-125145952) |  | [1978](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.16.0%2064-bit#Lynx_0_16_0_64-bit) |  |  |  |  |
| [0.15.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.15.0) | 2023-08-13 | [2039](https://github.com/lynx-chess/Lynx/commit/519d69302f855971d502724de0cdfef5e56ffed2#commitcomment-124397606) |  |  | 2098 |  |  |  |
| [0.14.1](https://github.com/lynx-chess/Lynx/releases/tag/v0.14.0) | 2023-07-30 |  |  | [1670](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.14.0%2064-bit#Lynx_0_14_0_64-bit) |  |  |  |  |
| [0.13.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.13.0) | 2022-11-25 |  |  | [1637](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.13.0%2064-bit#Lynx_0_13_0_64-bit) | 1773 |  |  |  |
| [0.11.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.11.0) | 2022-09-18 |  |  | [1477](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.11.0%2064-bit#Lynx_0_11_0_64-bit) |  |  |  |  |
| [0.10.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.10.0) | 2022-05-09 |  |  | [1426](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.10.0%2064-bit#Lynx_0_10_0_64-bit) |  |  |  |  |
| [0.9.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.9.0) | 2021-11-29 |  |  | [1449](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.9.0%2064-bit#Lynx_0_9_0_64-bit) |  |  |  |  |
| [0.6.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.6.0) | 2021-10-19 |  |  | [1263](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.6.0%2064-bit#Lynx_0_6_0_64-bit) |  |  |  |  |
| [0.4.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.4.0) | 2021-09-20 |  |  | [1208](https://www.computerchess.org.uk/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.4.0%2064-bit#Lynx_0_4_0_64-bit) |  |  |  |  |

\* Not enough games

<sup>0</sup> CCRL Blitz elo estimation, based on 40+0.4 gauntlets vs other engines

<sup>1</sup> After 2024-01-13 CCRL blitz elo recalculation, where Lynx 1.0.1 went from 2497 to 2432

## Building Lynx

[Lynx release artifacts](https://github.com/lynx-chess/Lynx/releases) are self-contained and require no dependencies to be run.

However, you can also choose to build Lynx yourself.

### Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0). You can find instructions about how to install it in your preferred OS/Distro either [here](https://docs.microsoft.com/en-us/dotnet/core/install/) or [here](https://github.com/dotnet/core/tree/main/release-notes/9.0).

If you're a Linux user and are new to .NET ecosystem, the conversation in [this issue](https://github.com/lynx-chess/Lynx/issues/33) may help.

### Instructions

- Clone the repo and preferably checkout one of the [tagged commits](https://github.com/lynx-chess/Lynx/tags).

- Run `make` to build a self-contained binary similar to the pre-compiled ones.

  Disclaimer: I do not use the Makefile myself, which means it is not fully tested and may occasionally get out of date.

- Alternatively, you can get the exact `dotnet publish (...)` command from [`release.yml`](https://github.com/lynx-chess/Lynx/blob/main/.github/workflows/release.yml) that is used by the CI to create the binaries and run it yourself (with the right [runtime identifier](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#using-rids)).

  Examples:

  ```bash
  dotnet publish src/Lynx.Cli/Lynx.Cli.csproj -c Release --runtime linux-x64 --self-contained /p:Optimized=true -o /home/your_user/engines/Lynx
  dotnet publish src/Lynx.Cli/Lynx.Cli.csproj -c Release --runtime win-x64 --self-contained /p:Optimized=true -o C:/Users/your_user/engines/Lynx
  ```

- The previous steps will generate an executable named `Lynx.Cli(.exe)` and a settings file named `appsettings.json`, which are enough to run Lynx chess engine.

## Features

<details>

<Summary>Feature list</Summary>

_Beware, most of the provided links contain outdated information and don't reflect the current implementation or the state of the art of computer chess programming, at this point they remain here mostly for historical reasons_.

### Search

- NegaMax [[1](https://www.chessprogramming.org/Negamax)]

- Quiescence Search [[1](https://www.chessprogramming.org/Quiescence_Search)]

- Iterative Deepening Depth-First Search (IDDFS) [[1](https://en.wikipedia.org/wiki/Iterative_deepening_depth-first_search)] [[2](https://www.chessprogramming.org/Iterative_Deepening)]

- Aspiration Windows [[1](https://web.archive.org/web/20071031095918/http://www.brucemo.com/compchess/programming/aspiration.htm)] [[2](https://www.chessprogramming.org/Aspiration_Windows)]

- Principal Variation Search (PVS) [[1](https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm)]

- Null-move pruning (NMP) [[1](https://web.archive.org/web/20071031095933/http://www.brucemo.com/compchess/programming/nullmove.htm)] [[2](https://www.chessprogramming.org/Null_Move_Pruning)]

- Late Move Pruning (LMP)

- Futility Pruning (FP)

- Reverse Futility Pruning (RFP)

- History pruning

- Late Move Reductions (LMR) [[1](https://web.archive.org/web/20150212051846/http://www.glaurungchess.com/lmr.html)] [[2](https://www.chessprogramming.org/Late_Move_Reductions)] [[3](https://talkchess.com/forum3/viewtopic.php?f=7&t=75056#p860118)]

- Internal Iterative Reduction (IIR)

- Check extensions [[1](https://www.chessprogramming.org/Check_Extensions)]

- Static Exchange Evaluation (SEE) for move ordering, reduction and QSearch pruning

- Razoring [[1](https://www.chessprogramming.org/Razoring)]

- Killer heuristic [[1](https://www.chessprogramming.org/Killer_Heuristic)]

- History heuristic: quiet history, capture history, continuation history, history malus [[1](https://www.chessprogramming.org/History_Heuristic)]

- Countermoves

### Evaluation

- Piece-Square Tables (PSQT) [[1](https://www.chessprogramming.org/Piece-Square_Tables)]

- King-bucketed PSQT

- Enemy king PSQT

- Mobility (knight, bishop, rook, queen)

- Bishop pair

- Bishop penalty for same color pawns

- Bishop penalty for blocked central pawns

- Rook in open and semi-open files

- King pawn shield, king virtual mobility, king in open and semi-open files

- Isolated pawns

- Passed pawns, including bonus for not opponent pieces ahead and friend/opponent king distance to it

- Pawn phalanx

- Pieces protected and attacked by pawns

- Pieces capable of deliverying checks

- Eval scaling with pawn count and 50 moves rule

- 50 moves rule eval scaling

### Miscellaneous

- PEXT Bitboards [[1](https://www.chessprogramming.org/BMI2#PEXTBitboards)]  [[2](https://analog-hors.github.io/site/magic-bitboards/)]

- Zobrist hashing [[1](https://www.chessprogramming.org/Zobrist_Hashing)]

- Transposition Table [[1](https://web.archive.org/web/20071031100051/http://www.brucemo.com/compchess/programming/hashing.htm)]

- Triangular PV-Table [[1](https://www.chessprogramming.org/Triangular_PV-Table)]

- Most Valuable Victim - Least Valuable Aggressor (MVV-LVA) [[1](https://www.chessprogramming.org/MVV-LVA)]

- Incremental move sorting

- Hard/Soft time management

- Expected moves to go [[1](https://expositor.dev/pdf/movetime.pdf)]

</details>

## Credits

Lynx development would simply not have been possible without:

- [`BitBoard Chess Engine in C` YouTube playlist](https://www.youtube.com/playlist?list=PLmN0neTso3Jxh8ZIylk74JpwfiWNI76Cs), where [@maksimKorzh](https://github.com/maksimKorzh) explains how he developed his [BBC](https://github.com/maksimKorzh/bbc) engine

- [Chess Programming Wiki](https://www.chessprogramming.org/)

I would also like to extend my gratitude to:

- Engine Programming discord group. Without it, Lynx wouldn't be as strong as it is nowadays. Especial mention for Jamie Whiting
([Akimbo](https://github.com/JacquesRW/akimbo)), Antares ([Altair](https://github.com/Alex2262/AltairChessEngine)), Ciekce ([Stormphrax](https://github.com/Ciekce/Stormphrax/)), Rak ([Mess](https://github.com/raklaptudirm/mess)), AspectOfTheNoob ([Sirius](https://github.com/mcthouacbb/Sirius)), etc.

- The community Discord around [SebLague/Chess-Challenge](https://github.com/SebLague/Chess-Challenge/), which allowed me to discover EP discord and to revisit the basics, this time explained by very knowledgeable developers (such as the ones above) to people without any previous chess engine programming knowledge

- Marcel Vanthoor and [his blog about how he created his engine, Rustic](https://rustic-chess.org/)

- Gedas for his [texel-tuner](https://github.com/GediminasMasaitis/texel-tuner) tool

- SF developers for their [WDL_model](https://github.com/official-stockfish/WDL_model) tool

- Andrew Grant for [OpenBench](https://github.com/AndyGrant/OpenBench)

- lichess developers for [lichess-bot](https://github.com/lichess-bot-devs/lichess-bot)

- Open source chess engines with permissive licenses. Their existence encourages knowledge sharing and really helps pushing the Chess Engine Developer community forward. Some engines are credited inside the codebase itself, where relevant

- Countless other developers and online resources, who/which I should probably remember, but don't come to my mind right now

Thanks also to all the testers that invest their time in computer chess, especially those ones that test lower rated engines (as opposed to only top ones).

[buildlink]: https://github.com/lynx-chess/Lynx/actions/workflows/ci.yml
[buildlogo]: https://github.com/lynx-chess/Lynx/actions/workflows/ci.yml/badge.svg
[releaselink]: https://github.com/lynx-chess/Lynx/releases/latest
[releaselogo]: https://img.shields.io/github/v/release/lynx-chess/Lynx
