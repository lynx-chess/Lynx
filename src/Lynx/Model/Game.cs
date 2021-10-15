using NLog;
using System.Collections.Generic;
using System.Linq;

namespace Lynx.Model
{
    public class Game
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public List<Move> MoveHistory { get; }
        public List<Position> PositionHistory { get; }
        public Dictionary<long, int> PositionHashHistory { get; }

        public int MovesWithoutCaptureOrPawnMove { get; set; }

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
                if (!Move.TryParseFromUCIString(moveString, GetAllMoves(), out var parsedMove))
                {
                    _logger.Error($"Error parsing game with fen {fen} and moves {string.Join(' ', movesUCIString)}");
                    break;
                }

                MakeMove(parsedMove.Value);
            }
        }

        public IOrderedEnumerable<Move> GetAllMoves() => MoveGenerator.GenerateAllMoves(CurrentPosition);
        public IOrderedEnumerable<Move> GetAllMovesWithCaptures() => MoveGenerator.GenerateAllMoves(CurrentPosition, capturesOnly: true);

        public void RevertLastMove()
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

            MovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(moveToPlay, MovesWithoutCaptureOrPawnMove);

            return true;
        }
    }
}
