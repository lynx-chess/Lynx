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

| Lynx version                                                    | CCRL Blitz elo                                                                                                                         |
|-----------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------|
| [0.14.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.14.0) | [1804](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.14.0%2064-bit#Lynx_0_14_0_64-bit) |
| [0.13.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.13.0) | [1762](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.13.0%2064-bit#Lynx_0_13_0_64-bit) |
| [0.11.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.11.0) | [1618](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.11.0%2064-bit#Lynx_0_11_0_64-bit) |
| [0.10.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.10.0) | [1573](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.10.0%2064-bit#Lynx_0_10_0_64-bit) |
| [0.9.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.9.0) | [1591](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.9.0%2064-bit#Lynx_0_9_0_64-bit) |
| [0.6.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.6.0) | [1419](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.6.0%2064-bit#Lynx_0_6_0_64-bit) |
| [0.4.0](https://github.com/lynx-chess/Lynx/releases/tag/v0.4.0) | [1367](http://ccrl.chessdom.com/ccrl/404/cgi/engine_details.cgi?print=Details&each_game=1&eng=Lynx%200.4.0%2064-bit#Lynx_0_4_0_64-bit) |

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

## Credits

Lynx development wouldn't have been possible without:

- [Chess Programming Wiki](https://www.chessprogramming.org/)

-  [`BitBoard Chess Engine in C` YouTube playlist](https://www.youtube.com/playlist?list=PLmN0neTso3Jxh8ZIylk74JpwfiWNI76Cs), where [@maksimKorzh](https://github.com/maksimKorzh) explains how he developed his [BBC](https://github.com/maksimKorzh/bbc) engine

[buildlink]: https://github.com/lynx-chess/Lynx/actions/workflows/ci.yml
[buildlogo]: https://github.com/lynx-chess/Lynx/actions/workflows/ci.yml/badge.svg
[releaselink]: https://github.com/lynx-chess/Lynx/releases/latest
[releaselogo]: https://img.shields.io/github/v/release/lynx-chess/Lynx
