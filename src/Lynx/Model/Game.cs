using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Model
{
    public class Game
    {
        public List<Move> MoveHistory { get; }
        public List<Position> PositionHistory { get; }

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
        }

        public List<Move> GetAllMoves() => MovesGenerator.GenerateAllMoves(CurrentPosition);
        public List<Move> GetAllMovesWithCaptures() => MovesGenerator.GenerateAllMoves(CurrentPosition, capturesOnly: true);

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

            if (!CurrentPosition.IsValid())
            {
                RevertLastMove();
                return false;
            }

            return true;
        }
    }
}
