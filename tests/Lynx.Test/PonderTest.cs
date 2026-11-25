using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading.Channels;

namespace Lynx.Test;

[Category(Categories.Configuration)]
[NonParallelizable]
public class PonderTest
{
    private const string GoPonder = "go ponder wtime 180000 btime 180000 winc 2000 binc 2000";
    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan _noBestMoveExpectedTimeout = TimeSpan.FromSeconds(1);

    /// <summary>
    /// stop is processed in the UCI listener thread, while go goes through a channel and it's processed in the search thread.
    /// A stop that is processed before the search thread picks up the go must not get lost
    /// </summary>
    [TestCase(1)]
    [TestCase(2)]
    public async Task StopRightAfterGoPonder(int threads)
    {
        await using var scenario = new Scenario(threads);

        await scenario.Send("position startpos");
        await scenario.Send(GoPonder);
        await scenario.Send(StopCommand.Id);

        Assert.That(await scenario.NextBestMove(_timeout), Is.Not.Null, "No 'bestmove' after 'go ponder' + 'stop'");
    }

    [Test]
    public async Task StopRightAfterGoPonderIsNotAppliedToNextGo()
    {
        await using var scenario = new Scenario(threads: 1);

        // The 2nd search has to run on its own, and return a move for the position sent after the stop
        await scenario.Send("position startpos");
        await scenario.Send(GoPonder);
        await scenario.Send(StopCommand.Id);
        await scenario.Send("position startpos moves e2e4");
        await scenario.Send("go movetime 100");

        Assert.That(await scenario.NextBestMove(_timeout), Is.Not.Null, "No 'bestmove' after 'go ponder' + 'stop'");

        var secondBestMove = await scenario.NextBestMove(_timeout);
        Assert.That(secondBestMove, Is.Not.Null, "No 'bestmove' for the 'go' following 'go ponder' + 'stop'");

        // Black to move after 1.e4: origin square in rank 7 or 8
        Assert.That(secondBestMove![1], Is.AnyOf('7', '8'), $"Second search didn't run on the position sent after 'stop': {secondBestMove}");
    }

    [Test]
    public async Task StopWithoutPendingGoDoesNotAffectNextGo()
    {
        await using var scenario = new Scenario(threads: 1, ponder: false);

        // A stray 'stop' (nothing to stop) must not cancel a later 'go'
        await scenario.Send(StopCommand.Id);
        await scenario.Send("position startpos");
        await scenario.Send("go depth 8");

        Assert.That(await scenario.NextBestMove(_timeout), Is.Not.Null);
    }

    [TestCase(1)]
    [TestCase(2)]
    public async Task PonderHitRightAfterGoPonder(int threads)
    {
        await using var scenario = new Scenario(threads);

        await scenario.Send("position startpos moves e2e4 e7e5");
        await scenario.Send("go ponder wtime 3000 btime 3000");
        await scenario.Send(PonderHitCommand.Id);

        Assert.That(await scenario.NextBestMove(_timeout), Is.Not.Null, "No 'bestmove' after 'go ponder' + 'ponderhit'");
    }

    /// <summary>
    /// A ponderhit with no go ponder in progress must be ignored, instead of staying around
    /// and making the next go ponder behave as if it had been hit
    /// </summary>
    [Test]
    public async Task StrayPonderHitDoesNotAffectNextGoPonder()
    {
        await using var scenario = new Scenario(threads: 1);

        await scenario.Send(PonderHitCommand.Id);
        await scenario.Send("position startpos");

        // With such a short time, a search that thinks it was hit would end almost immediately
        await scenario.Send("go ponder wtime 300 btime 300");

        Assert.That(await scenario.NextBestMove(_noBestMoveExpectedTimeout), Is.Null, "'bestmove' sent while pondering, before 'ponderhit' or 'stop'");

        await scenario.Send(StopCommand.Id);

        Assert.That(await scenario.NextBestMove(_timeout), Is.Not.Null, "No 'bestmove' after 'go ponder' + 'stop'");
    }

    [Test]
    public async Task PonderHitDuringNonPonderSearchIsIgnored()
    {
        await using var scenario = new Scenario(threads: 1);

        await scenario.Send("position startpos");
        await scenario.Send("go infinite");

        // The search needs to be running for this to not be the 'ponderhit before go' scenario
        await Task.Delay(200);
        await scenario.Send(PonderHitCommand.Id);

        Assert.That(await scenario.NextBestMove(_noBestMoveExpectedTimeout), Is.Null, "'ponderhit' stopped a search that wasn't a 'go ponder'");

        await scenario.Send(StopCommand.Id);

        Assert.That(await scenario.NextBestMove(_timeout), Is.Not.Null);
    }

    /// <summary>
    /// A pondering search can finish on its own (i.e. mate detected, max depth reached), but the result
    /// can't be sent until ponderhit or stop are received
    /// </summary>
    [TestCase(PonderHitCommand.Id, 1)]
    [TestCase(StopCommand.Id, 1)]
    [TestCase(PonderHitCommand.Id, 2)]
    [TestCase(StopCommand.Id, 2)]
    public async Task FinishedPonderSearchWaitsForPonderHitOrStop(string resolution, int threads)
    {
        await using var scenario = new Scenario(threads);

        await scenario.Send("position startpos");
        await scenario.Send("go ponder depth 2");

        Assert.That(await scenario.NextBestMove(_noBestMoveExpectedTimeout), Is.Null, "'bestmove' sent while pondering, before 'ponderhit' or 'stop'");

        await scenario.Send(resolution);

        Assert.That(await scenario.NextBestMove(_timeout), Is.Not.Null, $"No 'bestmove' after '{resolution}'");
    }

    /// <summary>
    /// After stdin EOF no stop or ponderhit can arrive, so go infinite and go ponder searches need to be stopped for the engine to exit.
    /// 0 ms delay: the go command is most likely still pending when the end of input is processed. 200 ms delay: the search is running
    /// </summary>
    [TestCase("go infinite", 0, 1)]
    [TestCase("go infinite", 0, 2)]
    [TestCase("go infinite", 200, 1)]
    [TestCase("go infinite", 200, 2)]
    [TestCase(GoPonder, 0, 1)]
    [TestCase(GoPonder, 0, 2)]
    [TestCase(GoPonder, 200, 1)]
    [TestCase(GoPonder, 200, 2)]
    public async Task EndOfInputStopsUnboundedSearch(string goCommand, int delayMs, int threads)
    {
        await using var scenario = new Scenario(threads);

        await scenario.Send("position startpos");
        await scenario.Send(goCommand);
        await Task.Delay(delayMs);

        Assert.IsTrue(await scenario.EndOfInput(_timeout), $"'{goCommand}' wasn't stopped by the end of input");
        Assert.AreEqual(1, (await scenario.BestMovesUntilCompletion()).Count);
    }

    /// <summary>
    /// Searches that end on their own aren't cut short by the end of input, a go infinite queued after them is stopped once it starts
    /// </summary>
    [TestCase(1)]
    [TestCase(2)]
    public async Task EndOfInputLetsBoundedSearchesFinish(int threads)
    {
        const int depth = 8;
        await using var scenario = new Scenario(threads);

        await scenario.Send("position startpos");
        await scenario.Send($"go depth {depth}");
        await scenario.Send("go infinite");

        Assert.IsTrue(await scenario.EndOfInput(_timeout), "Searches weren't over after the end of input");

        var bestMoves = await scenario.BestMovesUntilCompletion();
        Assert.AreEqual(2, bestMoves.Count);

        // >=: with threads > 1 the final result can come from a helper thread, which searches without depth limit
        Assert.GreaterOrEqual(bestMoves[0].Depth, depth, "'go depth' search was cut short by the end of input");
    }

    /// <summary>
    /// After ponderhit a go ponder search is a regular one, which the end of input mustn't stop.
    /// 200 ms delay: the end of input arrives while the search that follows the ponderhit is running.
    /// Time-based instead of depth-based, so that it doesn't depend on the build speed (Debug + code coverage in CI)
    /// </summary>
    [TestCase(0, 1)]
    [TestCase(0, 2)]
    [TestCase(200, 1)]
    [TestCase(200, 2)]
    public async Task EndOfInputAfterPonderHitLetsSearchFinish(int delayMs, int threads)
    {
        // >= PonderHitMinTimeToContinueSearch, so that the search always restarts after the ponderhit, with movetime as hard limit
        const int moveTime = 500;

        // Timer granularity (~15.6 ms on Windows), while a search cut short by the end of input would last ~delayMs
        const int moveTimeMargin = 100;

        await using var scenario = new Scenario(threads);

        await scenario.Send("position startpos");
        await scenario.Send($"go ponder movetime {moveTime}");

        var sw = Stopwatch.StartNew();
        await scenario.Send(PonderHitCommand.Id);
        await Task.Delay(delayMs);

        Assert.IsTrue(await scenario.EndOfInput(_timeout), "Search wasn't over after the end of input");
        sw.Stop();

        Assert.AreEqual(1, (await scenario.BestMovesUntilCompletion()).Count);
        Assert.GreaterOrEqual(sw.ElapsedMilliseconds, moveTime - moveTimeMargin, "Search after ponderhit was cut short by the end of input");
    }

    private sealed class Scenario : IAsyncDisposable
    {
        private readonly bool _previousIsPonder = Configuration.EngineSettings.IsPonder;
        private readonly int _previousThreads = Configuration.EngineSettings.Threads;

        private readonly Channel<object> _engineChannel = Channel.CreateUnbounded<object>();
        private readonly Searcher _searcher;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _searcherTask;
        private readonly UCIHandler _handler;

        public Scenario(int threads, bool ponder = true)
        {
            Configuration.EngineSettings.IsPonder = ponder;
            Configuration.EngineSettings.Threads = threads;

            var guiChannel = Channel.CreateUnbounded<string>();

            _searcher = new Searcher(guiChannel.Reader, _engineChannel.Writer);
            _searcherTask = Task.Run(() => _searcher.Run(_cts.Token));
            _handler = new UCIHandler(guiChannel, _engineChannel, _searcher);
        }

        public Task Send(string command) => _handler.Handle(command, CancellationToken.None);

        /// <returns>The move of the next bestmove sent by the engine, or <see langword="null"/> if there's none within <paramref name="timeout"/></returns>
        public async Task<string?> NextBestMove(TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);

            try
            {
                await foreach (var item in _engineChannel.Reader.ReadAllAsync(cts.Token))
                {
                    if (item is BestMoveCommand bestMoveCommand)
                    {
                        return bestMoveCommand.ToString().Split(' ')[1];
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Timeout
            }

            return null;
        }

        /// <summary>
        /// Stdin EOF, handled like the listener does: <see cref="UCIHandler.HandleEndOfInput"/> blocks until the searches are over
        /// and completes the engine channel
        /// </summary>
        /// <returns>Whether it returned within <paramref name="timeout"/></returns>
        public async Task<bool> EndOfInput(TimeSpan timeout)
        {
            var endOfInput = Task.Run(_handler.HandleEndOfInput);

            return await Task.WhenAny(endOfInput, Task.Delay(timeout)) == endOfInput;
        }

        /// <summary>
        /// Only after <see cref="EndOfInput(TimeSpan)"/> returned <see langword="true"/>: reads the engine output until the channel completion
        /// </summary>
        /// <returns>Each bestmove move, with the depth of the last search result (info line) sent before it</returns>
        public async Task<List<(string Move, int Depth)>> BestMovesUntilCompletion()
        {
            var bestMoves = new List<(string Move, int Depth)>();
            var lastDepth = 0;

            await foreach (var item in _engineChannel.Reader.ReadAllAsync())
            {
                switch (item)
                {
                    case SearchResult searchResult:
                        lastDepth = searchResult.Depth;
                        break;
                    case BestMoveCommand bestMoveCommand:
                        bestMoves.Add((bestMoveCommand.ToString().Split(' ')[1], lastDepth));
                        lastDepth = 0;
                        break;
                }
            }

            return bestMoves;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                // Searches must be over before the Searcher (and its TT) gets disposed, even if the test failed
                _searcher.StopSearching();
                await _cts.CancelAsync();
                await Task.WhenAny(_searcherTask, Task.Delay(2_000));

                _searcher.Dispose();
                _cts.Dispose();
            }
            finally
            {
                Configuration.EngineSettings.IsPonder = _previousIsPonder;
                Configuration.EngineSettings.Threads = _previousThreads;
            }
        }
    }
}
