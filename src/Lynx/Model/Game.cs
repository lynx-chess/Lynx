using NLog;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public sealed class Game
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#if DEBUG
    public List<Move> MoveHistory { get; }
#endif

    public HashSet<long> PositionHashHistory { get; }

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

        PositionHashHistory = new(1024) { CurrentPosition.UniqueIdentifier };
        HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;

#if DEBUG
        MoveHistory = new(1024);
#endif
    }

    [Obsolete("Just intended for testing purposes")]
    internal Game(Position position)
    {
        CurrentPosition = position;
        _gameInitialPosition = new Position(CurrentPosition);

        PositionHashHistory = new(1024) { position.UniqueIdentifier };

#if DEBUG
        MoveHistory = new(1024);
#endif
    }

    [Obsolete("Just intended for testing purposes")]
    internal Game(string fen, string[] movesUCIString) : this(fen)
    {
        foreach (var moveString in movesUCIString)
        {
            var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition);

            if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
            {
                _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen, string.Join(' ', movesUCIString), moveString);
                break;
            }

            MakeMove(parsedMove.Value);
        }

        _gameInitialPosition = new Position(CurrentPosition);
    }

    [Obsolete("Just intended for testing purposes")]
    internal Game(string fen, ReadOnlySpan<char> rawMoves, Span<Range> rangeSpan) : this(fen)
    {
        for (int i = 0; i < rangeSpan.Length; ++i)
        {
            var range = rangeSpan[i];
            if (range.Start.Equals(range.End))
            {
                break;
            }
            var moveString = rawMoves[range];
            var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition);

            if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
            {
                _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen, string.Join(' ', rawMoves.ToString()), moveString.ToString());
                break;
            }

            MakeMove(parsedMove.Value);
        }

        _gameInitialPosition = new Position(CurrentPosition);
    }

    [Obsolete("Just intended for testing purposes")]
    public Game(ReadOnlySpan<char> fen, ReadOnlySpan<char> rawMoves, Span<Range> rangeSpan, Move[] movePool) : this(fen)
    {
        for (int i = 0; i < rangeSpan.Length; ++i)
        {
            if (rangeSpan[i].Start.Equals(rangeSpan[i].End))
            {
                break;
            }
            var moveString = rawMoves[rangeSpan[i]];
            var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition, movePool);

            if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
            {
                _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen.ToString(), rawMoves.ToString(), moveString.ToString());
                break;
            }

            MakeMove(parsedMove.Value);
        }

        _gameInitialPosition = new Position(CurrentPosition);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsThreefoldRepetition(Position position) => PositionHashHistory.Contains(position.UniqueIdentifier);

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
    /// To be used in online tb proving only, with a copy of <see cref="PositionHashHistory"/>
    /// </summary>
    /// <param name="positionHashHistory"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsThreefoldRepetition(HashSet<long> positionHashHistory, Position position) => positionHashHistory.Contains(position.UniqueIdentifier);

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
