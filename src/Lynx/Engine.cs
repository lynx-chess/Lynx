// TODOs:
// Si empieza con blancas, hace a3 y entabla vs v0.1 sin hacer nada
//
// Esto falla, devolviendo un movimiento ilegal:
// position startpos moves a2a3 a7a5 b2b3 a8a6 c2c3 e7e6 f2f3 h7h6 g2g3 g8f6 h2h3 b7b6 e1f2 g7g5 f2e1 f6e4 c1b2 c8b7 b2c1 b7c6 c1b2 d7d6 b2c1 c6b7 c1b2 b7d5 b2c1 e4d2 e1f2 g5g4 f2e1 a5a4 e1f2 d5a8 f2e1 h8g8 e1f2 g4h3 f2e1 a8b7 e1f2 b8d7 f2e1 h3h2 e1f2 b7c6 f2e1 f8g7 e1f2 a6a7 f2e1 d7b8 e1f2 e8e7 f2e1 d2e4 c1g5 e7d7 g5f4 e6e5 f1h3 f7f5 f4c1 g8f8 h3g2 f8g8 g2h3 d8f8 h3g2 g7f6 g2h3 a4b3 h3g2 g8g5 g2h3 a7a4 h3g2 g5h5 g2h3 f8e7 h3g2 f5f4 g2h3 h5f5 h3g2 d7c8 g2h3 e7e6 h3g2 e4g5 g2h3 c6b5 h3g2 b8c6 g2h3 g5f3 e1f2 f5g5 h3g2 g5h5 g2h3 f3e1 h3g2 a4c4 g2h3 b5a6 h3g2 c4c3 d1d3 f6h4 g2h3 c6a7 h3g2 h4d8 g2h3 d6d5 h3g2 c3c1 d3c3 e6f7 c3c7 c8c7 g2h3 c1c4 h3f1 h2g1r a1a2 c7d7 f1h3 h5h3 e2e3 e5e4 f2e2 g1h1 e2d2 d8f6 d2e2 b3b2 e2d2 c4c7 b1c3 f7e6 a2a1 e6d6 d2d1 f6h4 c3e2 d6c5 e2g1 c5b5 d1d2 c7c6 a1a2 b2b1r
// go

// position startpos moves c2c4 e7e5 b1c3 f8b4 g2g3 g8f6 f1g2 e8g8 e2e3 d7d5 c4d5 c8g4 g1e2 f8e8 e1g1 b8d7 d2d3 d7c5 a2a3 -> go, avoid playing bb4a5

// position startpos moves a2a3 e7e5 g2g3 d7d5 b2b3 b8c6 c2c3 g8f6 f1h3 c8h3 capture the bishop

using Lynx.Internal;
using Lynx.Model;
using Lynx.Search;
using Lynx.UCI.Commands.GUI;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Lynx.Search.SearchAlgorithms;

namespace Lynx
{
    public class Engine
    {
        private readonly Logger _logger;
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
            // var bestMove =  FindRandomMove();

            //var game = FindBestMove_Naive(Game.CurrentPosition);
            //var bestMove = game.MoveHistory.Last();

            //var bestMove = FindBestMove_Depth1();

            PrintMovesStaticEval();
            //var result = new Result();
            //var evaluation = MiniMax_InitialImplementation(Game.CurrentPosition, Configuration.Parameters?.Depth ?? 3, result);
            //_logger.Debug($"Evaluation: {evaluation}");

            var result = MiniMax_InitialImplementation_2(Game.CurrentPosition, Configuration.Parameters?.Depth ?? 3);
            _logger.Debug($"Evaluation: {result.Evaluation}");

            var bestMove = result.MoveList!.Moves.Last();   // TODO: MoveList can be empty if the initial position is stalement or checkmate

            Game.MakeMove(bestMove);

            return bestMove;
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
                return position.MaterialEvaluation();
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

        private Move FindBestMove_Depth1()
        {
            var evalMoveList = new List<(Move Move, int eval)>(150);

            foreach (var move in MoveGenerator.GenerateAllMoves(Game.CurrentPosition))
            {
                var newPosition = new Position(Game.CurrentPosition, move);
                if (newPosition.IsValid())
                {
                    var eval = -newPosition.MaterialAndPositionalEvaluation();
                    Console.WriteLine($"{move,-6} | {-newPosition.MaterialEvaluation(),-5} | {eval,-5}");
                    evalMoveList.Add((move, eval));
                }
            }

            return evalMoveList.OrderByDescending(l => l.eval).First().Move;
        }

        private void PrintMovesStaticEval()
        {
            foreach (var move in MoveGenerator.GenerateAllMoves(Game.CurrentPosition))
            {
                var newPosition = new Position(Game.CurrentPosition, move);
                if (newPosition.IsValid())
                {
                    var eval = -newPosition.MaterialAndPositionalEvaluation();
                    Console.WriteLine($"{move,-6} | {-newPosition.MaterialEvaluation(),-5} | {eval,-5}");
                }
            }
        }

    }
}
