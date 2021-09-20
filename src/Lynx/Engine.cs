using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NLog;
using System;
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
        private Move? _moveToPonder;
        public double AverageDepth { get; private set; }

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
                if (!_isSearching && value)
                {
                    _isSearching = value;
                    //_searchCancellationTokenSource.TryReset();
                    //OnSearchFinished?.Invoke(BestMove(_lastGoCommand, _searchCancellationTokenSource.Token), MoveToPonder()).Wait();
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

        public delegate Task NotifyEndOfSearch(SearchResult searchResult, Move? moveToPonder);
        public event NotifyEndOfSearch? OnSearchFinished;

        private CancellationTokenSource _searchCancellationTokenSource;
        private CancellationTokenSource _absoluteSearchCancellationTokenSource;

        public Engine()
        {
            AverageDepth = 0;
            Game = new Game();
            IsReady = true;
            _isNewGameComing = true;
            _logger = LogManager.GetCurrentClassLogger();
            _searchCancellationTokenSource = new();
            _absoluteSearchCancellationTokenSource = new();
        }

        internal void SetGame(Game game)
        {
            Game = game;
        }

        public void NewGame()
        {
            _isNewGameComing = true;
            _isNewGameCommandSupported = true;
        }

        public void AdjustPosition(string rawPositionCommand)
        {
            Game = PositionCommand.ParseGame(rawPositionCommand);
            _isNewGameComing = false;
        }

        public void PonderHit()
        {
            Game.MakeMove(_moveToPonder!.Value);   // TODO: do we also receive the position command? If so, remove this line
            _isPondering = false;
        }

        public SearchResult BestMove() => BestMove(null);

        public SearchResult BestMove(GoCommand? goCommand)
        {
            _searchCancellationTokenSource = new CancellationTokenSource();
            _absoluteSearchCancellationTokenSource = new CancellationTokenSource();
            int? millisecondsLeft;
            int? millisecondsIncrement;
            int minDepth = Configuration.EngineSettings.MinDepth;
            int? maxDepth = null;

            if (Game.CurrentPosition.Side == Side.White)
            {
                millisecondsLeft = goCommand?.WhiteTime;
                millisecondsIncrement = goCommand?.WhiteIncrement;
            }
            else
            {
                millisecondsLeft = goCommand?.BlackTime;
                millisecondsIncrement = goCommand?.BlackIncrement;
            }

            if (goCommand is not null && millisecondsLeft != 0)
            {
                int decisionTime = Convert.ToInt32(CalculateDecisionTime(goCommand.MovesToGo, millisecondsLeft ?? 0, millisecondsIncrement ?? 0));

                if (decisionTime > Configuration.EngineSettings.MinMoveTime)
                {
                    _logger.Info($"Time to move: {0.001 * decisionTime}s, min. {minDepth} plies");
                    _searchCancellationTokenSource.CancelAfter(decisionTime);
                }
                else // Ignore decisionTime and limit search to MinDepthWhenLessThanMinMoveTime plies
                {
                    _logger.Info($"Depth limited to {Configuration.EngineSettings.DepthWhenLessThanMinMoveTime} plies due to time trouble");
                    maxDepth = Configuration.EngineSettings.DepthWhenLessThanMinMoveTime;
                }
            }
            else // EngineTest
            {
                maxDepth = Configuration.EngineSettings.MinDepth;
            }

            var result = NegaMax_AlphaBeta_Quiescence_IDDFS(Game.CurrentPosition, minDepth, maxDepth, _searchCancellationTokenSource.Token, _absoluteSearchCancellationTokenSource.Token);
            _logger.Debug($"Evaluation: {result.Evaluation} (depth: {result.TargetDepth}, refutation: {string.Join(", ", result.Moves)})");

            if (!result.isCancelled)
            {
                Game.MakeMove(result.BestMove);
            }
            AverageDepth += (result.DepthReached - AverageDepth) / Game.MoveHistory.Count;

            return result;
        }

        internal double CalculateDecisionTime(int movesToGo, int millisecondsLeft, int millisecondsIncrement)
        {
            double decisionTime = 0;
            millisecondsLeft -= millisecondsIncrement; // Since we're going to spend them, shouldn't take into account for our calculations

            if (movesToGo == default)
            {
                int movesLeft = Configuration.EngineSettings.TotalMovesWhenNoMovesToGoProvided - (Game.MoveHistory.Count >> 1);

                if (movesLeft > 0)
                {
                    if (millisecondsLeft >= Configuration.EngineSettings.FirstTimeLimitWhenNoMovesToGoProvided)
                    {
                        decisionTime = Configuration.EngineSettings.FirstCoefficientWhenNoMovesToGoProvided * millisecondsLeft / movesLeft;
                    }
                    else if (millisecondsLeft >= Configuration.EngineSettings.SecondTimeLimitWhenNoMovesToGoProvided)
                    {
                        decisionTime = Configuration.EngineSettings.SecondCoefficientWhenNoMovesToGoProvided * millisecondsLeft / movesLeft;
                    }
                    else
                    {
                        decisionTime = millisecondsLeft / movesLeft;
                    }
                }
            }
            else
            {
                if (movesToGo > Configuration.EngineSettings.KeyMovesBeforeMovesToGo)
                {
                    decisionTime = Configuration.EngineSettings.CoefficientBeforeKeyMovesBeforeMovesToGo * millisecondsLeft / movesToGo;
                }
                else
                {
                    decisionTime = Configuration.EngineSettings.CoefficientAfterKeyMovesBeforeMovesToGo * millisecondsLeft / movesToGo;
                }
            }

            decisionTime += millisecondsIncrement;

            //if (millisecondsLeft > Configuration.Parameters.MinTimeToClamp)
            //{
            //    decisionTime = Math.Clamp(decisionTime, Configuration.Parameters.MinMoveTime, Configuration.Parameters.MaxMoveTime);
            //}

            if (millisecondsLeft + millisecondsIncrement - decisionTime < 1_000)    // i.e. x + 10s, 10s left in the clock
            {
                decisionTime *= 0.9;
            }

            return decisionTime;
        }

        public void StartSearching(GoCommand goCommand)
        {
            _isPondering = goCommand.Ponder;
            IsSearching = true;
            Task.Run(() =>
            {
                try
                {
                    var searchResult = BestMove(goCommand);
                    _moveToPonder = searchResult.Moves.Count >= 2 ? searchResult.Moves[1] : null;
                    OnSearchFinished?.Invoke(searchResult, _moveToPonder);
                }
                catch (Exception e)
                {
                    _logger.Fatal(e.Message + Environment.NewLine + e.StackTrace);
                }
            });
            // TODO: if ponder, continue with PonderAction, which is searching indefinitely for a move
        }

        public void StopSearching()
        {
            _absoluteSearchCancellationTokenSource.Cancel();
            IsSearching = false;
            // TODO
        }
    }
}
