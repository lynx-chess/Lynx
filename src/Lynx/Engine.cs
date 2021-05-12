using Lynx.Internal;
using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NLog;
using System;
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
                Game = PositionCommand.ParseGame(rawPositionCommand);
                _isNewGameComing = false;
            }
            else
            {
                var lastMove = PositionCommand.ParseLastMoveOnly(rawPositionCommand, Game);

                if (lastMove is null || !Game.MakeMove(lastMove.Value))
                {
                    _logger.Warn($"Position couldn't be adjusted using last move in position command: {rawPositionCommand}" +
                        "Retrying parsing the whole game");

                    _isNewGameComing = false;
                    AdjustPosition(rawPositionCommand);
                }
            }
        }

        public void PonderHit()
        {
            Game.MakeMove(MoveToPonder()!.Value);   // TODO: do we also receive the position command? If so, remove this line
            _isPondering = false;
        }

        public Move BestMove()
        {
            // TODO
            foreach (var move in Game.GetAllMoves().OrderBy(_ => Guid.NewGuid()))
            {
                if (Game.MakeMove(move))
                {
                    return move;
                }
            }

            return default;
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
            Thread.Sleep(100);
            StopSearching();
        }

        public void StopSearching()
        {
            IsSearching = false;
            // TODO
        }
    }
}
