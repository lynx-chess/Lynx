using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Threading.Channels;

namespace Lynx;

public sealed partial class Engine
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ChannelWriter<string> _engineWriter;

#pragma warning disable IDE0052, CS0414, S4487 // Remove unread private members
    private bool _isNewGameCommandSupported;
    private bool _isNewGameComing;
    private bool _isPondering;
#pragma warning restore IDE0052, CS0414 // Remove unread private members

    private Move? _moveToPonder;
    public double AverageDepth { get; private set; }

    public RegisterCommand? Registration { get; set; }

    public Game Game { get; private set; }

    public bool IsSearching { get; private set; }

    public bool PendingConfirmation { get; set; }

    private CancellationTokenSource _searchCancellationTokenSource;
    private CancellationTokenSource _absoluteSearchCancellationTokenSource;

    public Engine(ChannelWriter<string> engineWriter)
    {
        AverageDepth = 0;
        Game = new Game();
        _isNewGameComing = true;
        _searchCancellationTokenSource = new();
        _absoluteSearchCancellationTokenSource = new();
        _engineWriter = engineWriter;
    }

    internal void SetGame(Game game)
    {
        Game = game;
    }

    public void NewGame()
    {
        AverageDepth = 0;
        _isNewGameComing = true;
        _isNewGameCommandSupported = true;
        if (Configuration.EngineSettings.TranspositionTableEnabled)
        {
            _transpositionTable = new TranspositionTableElement[TranspositionTableExtensions.TranspositionTableArrayLength];
        }
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
        int minDepth = Configuration.EngineSettings.MinDepth + 1;
        int? maxDepth = null;
        int? decisionTime = null;

        if (goCommand is not null)
        {
            int millisecondsLeft;
            int millisecondsIncrement;
            if (Game.CurrentPosition.Side == Side.White)
            {
                millisecondsLeft = goCommand.WhiteTime;
                millisecondsIncrement = goCommand.WhiteIncrement;
            }
            else
            {
                millisecondsLeft = goCommand.BlackTime;
                millisecondsIncrement = goCommand.BlackIncrement;
            }

            if (millisecondsLeft > 0)
            {
                millisecondsLeft -= 50;
                Math.Clamp(millisecondsLeft, 50, int.MaxValue);
                decisionTime = Convert.ToInt32(Math.Min(0.5 * millisecondsLeft, millisecondsLeft * 0.03333 + millisecondsIncrement));
                //decisionTime = Convert.ToInt32(CalculateDecisionTime(goCommand.MovesToGo, millisecondsLeft, millisecondsIncrement));

                _logger.Info("Time to move: {0}s", 0.001 * decisionTime);
                _searchCancellationTokenSource.CancelAfter(decisionTime!.Value);
            }
            else if (goCommand.MoveTime > 0)
            {
                minDepth = 0;
                decisionTime = (int)(0.95 * goCommand.MoveTime);
                _logger.Info("Time to move: {0}s, min. {1} plies", 0.001 * decisionTime, minDepth);
                _searchCancellationTokenSource.CancelAfter(decisionTime.Value);
            }
            else if (goCommand.Depth > 0)
            {
                minDepth = goCommand.Depth;
                maxDepth = goCommand.Depth;
            }
            else if (goCommand.Infinite)
            {
                minDepth = Configuration.EngineSettings.MaxDepth;
                maxDepth = Configuration.EngineSettings.MaxDepth;
                _logger.Info("Infinite search (depth {0})", minDepth);
            }
            else
            {
                _logger.Warn("Unexpected or unsupported go command");
                maxDepth = Configuration.EngineSettings.DefaultMaxDepth;
            }
        }
        else // EngineTest
        {
            maxDepth = Configuration.EngineSettings.DefaultMaxDepth;
        }

        SearchResult resultToReturn = SearchBestMove(minDepth, maxDepth, decisionTime);

        if (!resultToReturn.IsCancelled && !_absoluteSearchCancellationTokenSource.IsCancellationRequested)
        {
            Game.MakeMove(resultToReturn.BestMove);
        }

        AverageDepth += (resultToReturn.TargetDepth - AverageDepth) / Math.Ceiling(0.5 * Game.MoveHistory.Count);

        return resultToReturn;
    }

    private SearchResult SearchBestMove(int minDepth, int? maxDepth, int? decisionTime)
    {
        var tasks = new Task<SearchResult?>[] {
            ProbeOnlineTablebase(Game.CurrentPosition, new(Game.PositionHashHistory), _halfMovesWithoutCaptureOrPawnMove),
            IDDFS(minDepth, maxDepth, decisionTime),
        };

        var resultList = Task.WhenAll(tasks).Result;
        var searchResult = resultList[1];
        var tbResult = resultList[0];

        if (searchResult is not null)
        {
            _logger.Info("Search evaluation result - eval: {0}, mate: {1}, depth: {2}, refutation: {3}",
                searchResult.Evaluation, searchResult.Mate, searchResult.TargetDepth, string.Join(", ", searchResult.Moves.Select(m => m.ToMoveString())));
        }

        if (tbResult is not null)
        {
            _logger.Info("Online tb probing result - mate: {0}, moves: {1}",
                tbResult.Mate, string.Join(", ", tbResult.Moves.Select(m => m.ToMoveString())));

            if (searchResult?.Mate > 0 && searchResult.Mate <= tbResult.Mate && searchResult.Mate + _halfMovesWithoutCaptureOrPawnMove < 96)
            {
                _logger.Info("Relying on search result mate line due to dtm match and low enough dtz");
                ++searchResult.TargetDepth;
                tbResult = null;
            }
        }

        return tbResult
            ?? searchResult
                ?? throw new AssertException("Both search and online tb proving results are null. At least search one is always expected to have a value");
    }

    internal double CalculateDecisionTime(int movesToGo, int millisecondsLeft, int millisecondsIncrement)
    {
        double decisionTime = millisecondsLeft;
        //millisecondsLeft -= millisecondsIncrement; // Since we're going to spend them, shouldn't take into account for our calculations
        //millisecondsLeft = Math.Clamp(millisecondsLeft, 0, int.MaxValue);

        if (movesToGo == default)
        {
            if (Game.MoveHistory.Count < Configuration.EngineSettings.FirstMoveLimitWhenNoMovesToGoProvided)
            {
                decisionTime *= Configuration.EngineSettings.FirstMoveLimitWhenNoMovesToGoProvided;
            }
            else if (Game.MoveHistory.Count < Configuration.EngineSettings.SecondMoveLimitWhenNoMovesToGoProvided)
            {
                decisionTime *= Configuration.EngineSettings.SecondCoefficientWhenNoMovesToGoProvided;
            }
            else
            {
                decisionTime *= Configuration.EngineSettings.ThirdCoefficientWhenNoMovesToGoProvided;
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

        decisionTime += millisecondsIncrement * Configuration.EngineSettings.IncrementCoefficientToSpendPerMove;

        decisionTime = Math.Min(millisecondsLeft >> 1, decisionTime);

        //if (millisecondsLeft > Configuration.Parameters.MinTimeToClamp)
        //{
        //    decisionTime = Math.Clamp(decisionTime, Configuration.Parameters.MinMoveTime, Configuration.Parameters.MaxMoveTime);
        //}

        // If time left after taking all decision time < 1s
        if (millisecondsLeft + millisecondsIncrement - decisionTime < Configuration.EngineSettings.MinSecurityTime)    // i.e. x + 10s, 10s left in the clock
        {
            decisionTime *= Configuration.EngineSettings.CoefficientSecurityTime;
        }

        return decisionTime;
    }

    public void StartSearching(GoCommand goCommand)
    {
        _isPondering = goCommand.Ponder;
        IsSearching = true;
        Task.Run(async () =>
        {
            try
            {
                var searchResult = BestMove(goCommand);
                _moveToPonder = searchResult.Moves.Count >= 2 ? searchResult.Moves[1] : null;
                await _engineWriter.WriteAsync(BestMoveCommand.BestMove(searchResult.BestMove, _moveToPonder));
            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Error in {0} while calculating BestMove", nameof(StartSearching));
            }
        });
        // TODO: if ponder, continue with PonderAction, which is searching indefinitely for a move
    }

    public void StopSearching()
    {
        _absoluteSearchCancellationTokenSource.Cancel();
        IsSearching = false;
    }
}
