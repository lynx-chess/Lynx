using NLog;

namespace Lynx.Model;

public sealed class Game
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public Move[] MovePool { get; } = new Move[Constants.MaxNumberOfPossibleMovesInAPosition];

    public List<Move> MoveHistory { get; }
    public List<Position> PositionHistory { get; }
    public Dictionary<long, int> PositionHashHistory { get; }

    public int HalfMovesWithoutCaptureOrPawnMove { get; private set; }

    public Position CurrentPosition { get; private set; }

    public Game() : this(Constants.InitialPositionFEN)
    {
    }

    public Game(string fen) : this(new Position(fen))
    {
    }

    public Game(Position position)
    {
        CurrentPosition = position;

        MoveHistory = new(150);
        PositionHistory = new(150);
        PositionHashHistory = new() { [position.UniqueIdentifier] = 1 };
    }

    public Game(string fen, List<string> movesUCIString) : this(fen)
    {
        foreach (var moveString in movesUCIString)
        {
            var moveList = MoveGenerator.GenerateAllMoves(CurrentPosition, MovePool);

            if (!MoveExtensions.TryParseFromUCIString(moveString, moveList, out var parsedMove))
            {
                _logger.Error($"Error parsing game with fen {fen} and moves {string.Join(' ', movesUCIString)}: error detected in {moveString}");
                break;
            }

            MakeMove(parsedMove.Value);
        }
    }

    public bool MakeMove(Move moveToPlay)
    {
        PositionHistory.Add(CurrentPosition);
        CurrentPosition = new Position(CurrentPosition, moveToPlay);
        MoveHistory.Add(moveToPlay);

        if (!CurrentPosition.WasProduceByAValidMove())
        {
            RevertLastMove();
            return false;
        }

        Utils.UpdatePositionHistory(CurrentPosition, PositionHashHistory);

        HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveToPlay, HalfMovesWithoutCaptureOrPawnMove);

        return true;
    }

    internal void RevertLastMove()
    {
        if (PositionHistory.Count != 0)
        {
            CurrentPosition = PositionHistory.Last();
            PositionHistory.Remove(CurrentPosition);
        }

        if (MoveHistory.Count != 0)
        {
            MoveHistory.RemoveAt(MoveHistory.Count - 1);
        }
    }
}
