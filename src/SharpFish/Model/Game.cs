using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFish.Model
{
    public class Game
    {
        public List<Move> MoveHistory { get; }
        public List<Position> PositionHistory { get; }

        public Position CurrentPosition { get; private set; }

        public Game() : this(Constants.InitialPositionFEN)
        {
        }

        public Game(string fen): this(new Position(fen))
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

        public void RevertMove()
        {
            if (PositionHistory.Count != 0)
            {
                CurrentPosition = PositionHistory.Last();
                PositionHistory.Remove(CurrentPosition);
            }
        }

        public void MakeMove(Move moveToPlay)
        {
            PositionHistory.Add(CurrentPosition);
            CurrentPosition = new Position(CurrentPosition, moveToPlay);
            MoveHistory.Add(moveToPlay);
        }
    }
}
