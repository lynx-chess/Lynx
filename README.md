# Lynx

[![Lynx build][buildlogo]][buildlink]
[![Lynx release][releaselogo]][releaselink]

## Introduction

<img align="right" width="200" height="200" src="resources/lynx.png">

Lynx is a chess engine developed by [@eduherminio](https://github.com/eduherminio).

It's written in C# (.NET 8).

You can find Lynx:

- As a [lichess bot](https://lichess.org/@/Lynx_BOT).

- As a self-contained executable, downloadable from [Releases](https://github.com/lynx-chess/Lynx/releases).

Lichess bot can be played directly, but a chess GUI that supports UCI protocol is needed to play against the self-contained version.

## Strength

| Lynx version | Release date | Estimated elo | [CCRL Blitz](https://ccrl.chessdom.com/ccrl/404/) | [CCRL](https://ccrl.chessdom.com/ccrl/4040/) | [MCERL](https://www.chessengeria.eu/mcerl) |
|---|---|---|---|---|---|
| [0.17.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.17.0)  | 2023-09-19  | [2178](https://github.com/lynx-chess/Lynx/commit/ecd462bf48923deb7fe7449ff74da3bcc8afe75c#commitcomment-127755063) |  |  |  |
| [0.16.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.16.0)  | 2023-08-26 | [2053](https://github.com/lynx-chess/Lynx/commit/8743436f4e0cca508dc9fd419a5498c46f15866c#commitcomment-125145952) | [2083](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.16.0%2064-bit#Lynx_0_16_0_64-bit) |  |  |
| [0.15.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.15.0)| 2023-08-13 | [2039](https://github.com/lynx-chess/Lynx/commit/519d69302f855971d502724de0cdfef5e56ffed2#commitcomment-124397606) | |  | 2081 |
| [0.14.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.14.0)  | 2023-07-26 | | [1797](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.14.0%2064-bit#Lynx_0_14_0_64-bit)  |  |
| [0.13.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.13.0)  | 2022-11-25 |   | [1767](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.13.0%2064-bit#Lynx_0_13_0_64-bit) |  | 1718 |
| [0.11.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.11.0) | 2022-09-18 |  | [1620](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.11.0%2064-bit#Lynx_0_11_0_64-bit) |  |  |
| [0.10.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.10.0)| 2022-05-09 |  | [1575](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.10.0%2064-bit#Lynx_0_10_0_64-bit) |  |  |
| [0.9.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.9.0) | 2021-11-29 |   | [1594](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.9.0%2064-bit#Lynx_0_9_0_64-bit) |  |  |
| [0.6.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.6.0)| 2021-10-19 |   | [1423](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.6.0%2064-bit#Lynx_0_6_0_64-bit) |  |  |
| [0.4.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.4.0)| 2021-09-20  |   | [1372](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.4.0%2064-bit#Lynx_0_4_0_64-bit) |  |  |

## Building Lynx

[Lynx release artifacts](https://github.com/lynx-chess/Lynx/releases) are self-contained and require no dependencies to be run.

However, you can also choose to build Lynx yourself.

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0). You can find instructions about how to install it in your preferred OS/Distro either [here](https://docs.microsoft.com/en-us/dotnet/core/install/) or [here](https://github.com/dotnet/core/tree/main/release-notes/8.0).

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

- The previous steps will generate an executable named `Lynx.Cli(.exe)` and a (required) settings file named `appsettings.json`, which are enough to run Lynx chess engine.

## Features

- Static, material evaluation

- Static, positional evaluation

- NegaMax [[1](https://www.chessprogramming.org/Negamax)]

- Iterative Deepening Depth-First Search (IDDFS) [[1](https://en.wikipedia.org/wiki/Iterative_deepening_depth-first_search)] [[2](https://www.chessprogramming.org/Iterative_Deepening)]

- Most Valuable Victim - Least Valuable Aggressor (MVV-LVA) [[1](https://www.chessprogramming.org/MVV-LVA)]

- Killer heuristic [[1](https://www.chessprogramming.org/Killer_Heuristic)]

- History heuristic [[1](https://www.chessprogramming.org/History_Heuristic)]

- Zobrist hashing [[1](https://www.chessprogramming.org/Zobrist_Hashing)]

- Triangular PV-Table [[1](https://www.chessprogramming.org/Triangular_PV-Table)]

- Principal Variation Search [[1](https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm)]

- Late Move Reductions [[1](https://web.archive.org/web/20150212051846/http://www.glaurungchess.com/lmr.html)] [[2](https://www.chessprogramming.org/Late_Move_Reductions)] [[3](https://talkchess.com/forum3/viewtopic.php?f=7&t=75056#p860118)]

- Verified null-move pruning [[1](https://www.researchgate.net/publication/297377298_Verified_Null-Move_Pruning)] [[2](https://web.archive.org/web/20071031095933/http://www.brucemo.com/compchess/programming/nullmove.htm)] [[3](https://www.chessprogramming.org/Null_Move_Pruning)]

- Aspiration Windows [[1](https://web.archive.org/web/20071031095918/http://www.brucemo.com/compchess/programming/aspiration.htm)] [[2](https://www.chessprogramming.org/Aspiration_Windows)]

- Transposition Table [[1](https://web.archive.org/web/20071031100051/http://www.brucemo.com/compchess/programming/hashing.htm)]

- Check extensions [[1](https://www.chessprogramming.org/Check_Extensions)]

## Credits

Lynx development wouldn't have been possible without:

-  [`BitBoard Chess Engine in C` YouTube playlist](https://www.youtube.com/playlist?list=PLmN0neTso3Jxh8ZIylk74JpwfiWNI76Cs), where [@maksimKorzh](https://github.com/maksimKorzh) explains how he developed his [BBC](https://github.com/maksimKorzh/bbc) engine

- [Chess Programming Wiki](https://www.chessprogramming.org/)

- Engine Programming discord group, especially Jamie Whiting
([Akimbo](https://github.com/JacquesRW/akimbo)), Antares ([Altair](https://github.com/Alex2262/AltairChessEngine)), Ciekce ([Stormphrax](https://github.com/Ciekce/Stormphrax/)), Rak ([Mess](https://github.com/raklaptudirm/mess)), etc.

- The community Discord around [SebLague/Chess-Challenge](https://github.com/SebLague/Chess-Challenge/), which allowed me to discover EP discord and to revisit the basics, this time explained by very knowledgeable developers (such as the ones above) to people without any previous chess engine programming knowledge

- [Marcel Vanthoor's blog](https://rustic-chess.org/) about creating his engine Rustic

- Countless other developers and online resources, who/which I should probably remember, but don't come to my mind right now

Thanks also to all the testers that invest their time in computer chess, especially those ones that test lower rated engines (as opposed to only top ones).

[buildlink]: https://github.com/lynx-chess/Lynx/actions/workflows/ci.yml
[buildlogo]: https://github.com/lynx-chess/Lynx/actions/workflows/ci.yml/badge.svg
[releaselink]: https://github.com/lynx-chess/Lynx/releases/latest
[releaselogo]: https://img.shields.io/github/v/release/lynx-chess/Lynx
