using NLog;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public sealed class Game
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Move[] MovePool { get; } = new Move[Constants.MaxNumberOfPossibleMovesInAPosition];

    public List<Move> MoveHistory { get; }
    public HashSet<long> PositionHashHistory { get; }

    public int HalfMovesWithoutCaptureOrPawnMove { get; set; }

    public Position CurrentPosition { get; private set; }

    public Game() : this(Constants.InitialPositionFEN)
    {
    }

    public Game(string fen)
    {
        var parsedFen = FENParser.ParseFEN(fen);
        CurrentPosition = new Position(parsedFen);

        MoveHistory = new(150);
        PositionHashHistory = new(150) { CurrentPosition.UniqueIdentifier };

        HalfMovesWithoutCaptureOrPawnMove = parsedFen.HalfMoveClock;
    }

    internal Game(Position position)
    {
        CurrentPosition = position;

        MoveHistory = new(150);
        PositionHashHistory = new(150) { position.UniqueIdentifier };
    }

    public Game(string fen, List<string> movesUCIString) : this(fen)
    {
        foreach (var moveString in movesUCIString)
        {
            var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition, MovePool);

            if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
            {
                _logger.Error("Error parsing game with fen {0} and moves {1}: error detected in {2}", fen, string.Join(' ', movesUCIString), moveString);
                break;
            }

            MakeMove(parsedMove.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsThreefoldRepetition(Position position) => PositionHashHistory.Contains(position.UniqueIdentifier);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Is50MovesRepetition() => HalfMovesWithoutCaptureOrPawnMove >= 100;

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

    public GameState MakeMove(Move moveToPlay)
    {
        var gameState = CurrentPosition.MakeMove(moveToPlay);
        MoveHistory.Add(moveToPlay);

        if (!CurrentPosition.WasProduceByAValidMove())
        {
            RevertLastMove(moveToPlay, gameState);
        }

        PositionHashHistory.Add(CurrentPosition.UniqueIdentifier);

        HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveToPlay, HalfMovesWithoutCaptureOrPawnMove);

        return gameState;
    }

    internal void RevertLastMove(Move playedMove, GameState gameState)
    {
        CurrentPosition.UnmakeMove(playedMove, gameState);

        if (MoveHistory.Count != 0)
        {
            MoveHistory.RemoveAt(MoveHistory.Count - 1);
        }
    }
}
