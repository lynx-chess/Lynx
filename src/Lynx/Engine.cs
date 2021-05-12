using Lynx.Internal;
using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lynx
{
    public class Engine
    {
        private Logger _logger;
        private bool _isNewGameCommandSupported;
        private bool _isNewGameComing;
        private bool _isPondering;

        public RegisterCommand? Registration { get; set; }

        public Game Game { get; private set; }

        private bool _isReady;
        /// <summary>
        /// Ready for <see cref="IsReadyCommand"/> purposes. Normally true
        /// </summary>
        public bool IsReady
        {
            get => _isReady;
            private set
            {
                _isReady = value;
                if (value)
                {
                    OnReady?.Invoke();
                }
            }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                if (_isSearching && !value)
                {
                    _isSearching = value;
                    OnSearchFinished?.Invoke(BestMove(), MoveToPonder()).Wait();
                }
                else
                {
                    _isSearching = value;
                }
            }
        }

        public bool PendingConfirmation { get; set; }

        public delegate Task NotifyReadyOKHandler();
        public event NotifyReadyOKHandler? OnReady;

        public delegate Task NotifyBestMove(Move move, Move? moveToPonder);
        public event NotifyBestMove? OnSearchFinished;

        public Engine()
        {
            Game = new Game();
            IsReady = true;
            _isNewGameComing = true;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void NewGame()
        {
            _isNewGameComing = true;
            _isNewGameCommandSupported = true;
        }

        public void AdjustPosition(string rawPositionCommand)
        {
            if (_isNewGameComing || !_isNewGameCommandSupported)
            {
                ParseWholeGame(rawPositionCommand);
            }
            else
            {
                if (!PositionCommand.TryParseLastMove(rawPositionCommand, Game, out var lastMove)
                    || !Game.MakeMove(lastMove.Value))
                {
                    _logger.Warn(
                        $"Position couldn't be adjusted using last move in position command: {rawPositionCommand}" + Environment.NewLine +
                        "Retrying parsing the whole game");

                    ParseWholeGame(rawPositionCommand);
                }
            }

            void ParseWholeGame(string rawPositionCommand)
            {
                Game = PositionCommand.ParseGame(rawPositionCommand);
                _isNewGameComing = false;
            }
        }

        public void PonderHit()
        {
            Game.MakeMove(MoveToPonder()!.Value);   // TODO: do we also receive the position command? If so, remove this line
            _isPondering = false;
        }

        public Move BestMove()
        {
            //return FindRandomMove();
            var game = FindBestMove_Naive(Game.CurrentPosition);
            return game.MoveHistory.Last();
        }

        public Move? MoveToPonder()
        {
            // TODO
            return default;
        }

        public void StartSearching(GoCommand goCommand)
        {
            IsSearching = true;
            _isPondering = goCommand.Ponder;

            // TODO
            StopSearching();
        }

        public void StopSearching()
        {
            IsSearching = false;
            // TODO
        }


        private Move FindRandomMove()
        {
            foreach (var move in Game.GetAllMoves().OrderBy(_ => Guid.NewGuid()))
            {
                if (Game.MakeMove(move))
                {
                    return move;
                }
            }

            return default;
        }

        private Game FindBestMove_Naive(Position position, int depth = 3, Game? game = null)
        {
            if (game is null)
            {
                game = new Game();
            }

            if (depth == 0)
            {
                game.PositionHistory.Add(position);
                return game;
            }

            var positionMoveList = new List<(Move Move, Position Position)>(150);

            foreach (var move in MoveGenerator.GenerateAllMoves(position))
            {
                var newPosition = new Position(position, move);
                if (newPosition.IsValid())
                {
                    positionMoveList.Add((move, newPosition));
                }
            }

            var optimalPair = positionMoveList.OrderBy(pair => EvaluatePosition_Naive(pair.Position, depth - 1)).First();
            game.MoveHistory.Add(optimalPair.Move);
            game.PositionHistory.Add(optimalPair.Position);

            return game;
        }

        private int EvaluatePosition_Naive(Position position, int depth)
        {
            if (depth == 0)
            {
                return position.Evaluate();
            }

            var positions = new List<Position>(150);

            foreach (var move in MoveGenerator.GenerateAllMoves(position))
            {
                var newPosition = new Position(position, move);
                if (newPosition.IsValid())
                {
                    positions.Add(newPosition);
                }
            }

            if (positions.Count == 0)
            {
                if (Attacks.IsSquaredAttackedBySide(
                    position.PieceBitBoards[(int)Piece.K + Utils.PieceOffset(position.Side)].GetLS1BIndex(),
                    position,
                    (Side)Utils.OppositeSide(position.Side)))
                {
                    return int.MinValue;
                }
                else
                {
                    return 0;
                }
            }

            return positions.Max(p => EvaluatePosition_Naive(p, depth - 1));
        }
    }
}
