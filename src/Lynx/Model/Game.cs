﻿using NLog;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public sealed class Game
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#if DEBUG
    public List<Move> MoveHistory { get; }
#endif

    public List<long> PositionHashHistory { get; }

    public int HalfMovesWithoutCaptureOrPawnMove { get; set; }

    public Position CurrentPosition { get; private set; }
    private Position _gameInitialPosition;

    public Game() : this(Constants.InitialPositionFEN)
    {
    }

    public Game(ReadOnlySpan<char> fen)
    {
        var parsedFen = FENParser.ParseFEN(fen);
        CurrentPosition = new Position(parsedFen);
        _gameInitialPosition = new Position(CurrentPosition);

        if (!CurrentPosition.IsValid())
        {
            _logger.Warn($"Invalid position detected: {fen.ToString()}");
        }

        PositionHashHistory = new(Constants.MaxNumberMovesInAGame) { CurrentPosition.UniqueIdentifier };
        HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;

#if DEBUG
        MoveHistory = new(Constants.MaxNumberMovesInAGame);
#endif
    }

    public Game(ReadOnlySpan<char> fen, ReadOnlySpan<char> rawMoves, Span<Range> rangeSpan, Span<Move> movePool) : this(fen)
    {
        for (int i = 0; i < rangeSpan.Length; ++i)
        {
            if (rangeSpan[i].Start.Equals(rangeSpan[i].End))
            {
                break;
            }
            var moveString = rawMoves[rangeSpan[i]];
            var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition, movePool);

            // TODO: consider creating moves on the fly
            if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
            {
                _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen.ToString(), rawMoves.ToString(), moveString.ToString());
                break;
            }

            MakeMove(parsedMove.Value);
        }

        _gameInitialPosition = new Position(CurrentPosition);
    }

    /// <summary>
    /// Basic algorithm described in https://web.archive.org/web/20201107002606/https://marcelk.net/2013-04-06/paper/upcoming-rep-v2.pdf
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsThreefoldRepetition()
    {
        var currentHash = CurrentPosition.UniqueIdentifier;

        // Count - 1 would be the last one, we want to start searching 2 ealier
        for (int i = PositionHashHistory.Count - 3; i >= 0; i -= 2)
        {
            var diff = currentHash ^ PositionHashHistory[i];
            if (diff == 0)
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Is50MovesRepetition()
    {
        if (HalfMovesWithoutCaptureOrPawnMove < 100)
        {
            return false;
        }

        return !CurrentPosition.IsInCheck() || MoveGenerator.CanGenerateAtLeastAValidMove(CurrentPosition);
    }

    /// <summary>
    /// To be used in online tb proving only, with a copy of <see cref="PositionHashHistory"/> that hasn't been updated with <paramref name="position"/>
    /// </summary>
    /// <param name="positionHashHistory"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsThreefoldRepetition(List<long> positionHashHistory, Position position)
    {
        var currentHash = position.UniqueIdentifier;

        // Since positionHashHistory hasn't been updated with position, Count would be the last one, so we want to start searching 2 ealier
        for (int i = positionHashHistory.Count - 2; i >= 0; i -= 2)
        {
            var diff = currentHash ^ positionHashHistory[i];
            if (diff == 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// To be used in online tb proving only, with a copy of <see cref="HalfMovesWithoutCaptureOrPawnMove"/>
    /// </summary>
    /// <param name="halfMovesWithoutCaptureOrPawnMove"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is50MovesRepetition(int halfMovesWithoutCaptureOrPawnMove) => halfMovesWithoutCaptureOrPawnMove >= 100;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameState MakeMove(Move moveToPlay)
    {
        var gameState = CurrentPosition.MakeMove(moveToPlay);

        if (CurrentPosition.WasProduceByAValidMove())
        {
#if DEBUG
            MoveHistory.Add(moveToPlay);
#endif
        }
        else
        {
            _logger.Warn("Error trying to play {0}", moveToPlay.UCIString());
            CurrentPosition.UnmakeMove(moveToPlay, gameState);
        }

        PositionHashHistory.Add(CurrentPosition.UniqueIdentifier);
        HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveToPlay, HalfMovesWithoutCaptureOrPawnMove);

        return gameState;
    }

    /// <summary>
    /// Cleans <see cref="CurrentPosition"/> value, since in case of search cancellation
    /// (either by the engine time management logic or by external stop command)
    /// currentPosition won't be the initial one
    /// </summary>
    public void ResetCurrentPositionToBeforeSearchState() => CurrentPosition = new(_gameInitialPosition);

    public void UpdateInitialPosition() => _gameInitialPosition = new(CurrentPosition);
}
