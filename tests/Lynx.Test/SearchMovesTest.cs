using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace Lynx.Test;

/// <summary>
/// Modifies <see cref="Configuration.EngineSettings"/> values, so they need to run in isolation
/// </summary>
[Category(Categories.Configuration)]
[NonParallelizable]
public partial class SearchMovesTest : BaseTest
{
    private static readonly string[] CornerPawnMoves = ["a2a3", "h2h3"];

    private int _originalMultiPV;
    private int _originalThreads;
    private bool _originalIsPonder;

    [SetUp]
    public void SetUp()
    {
        _originalMultiPV = Configuration.EngineSettings.MultiPV;
        _originalThreads = Configuration.EngineSettings.Threads;
        _originalIsPonder = Configuration.EngineSettings.IsPonder;

        Configuration.EngineSettings.MultiPV = 1;
        Configuration.EngineSettings.Threads = 1;
    }

    [TearDown]
    public void TearDown()
    {
        Configuration.EngineSettings.MultiPV = _originalMultiPV;
        Configuration.EngineSettings.Threads = _originalThreads;
        Configuration.EngineSettings.IsPonder = _originalIsPonder;
    }

    [Test]
    public void BestMoveAndPVAreAmongTheSearchMoves()
    {
        var unrestricted = SearchBestMove(Constants.InitialPositionFEN, depth: 8);
        CollectionAssert.DoesNotContain(CornerPawnMoves, unrestricted.BestMove.UCIString(), "Test position should make the engine pick another move");

        var engine = GetEngine(Constants.InitialPositionFEN);
        var searchResult = engine.BestMove(new GoCommand("go depth 8 searchmoves a2a3 h2h3"));

        Assert.AreEqual(8, searchResult.Depth);
        CollectionAssert.Contains(CornerPawnMoves, searchResult.BestMove.UCIString());
        Assert.AreEqual(searchResult.BestMove, searchResult.Moves[0]);
    }

    [Test]
    public void SingleSearchMove()
    {
        var engine = GetEngine(Constants.InitialPositionFEN);
        var searchResult = engine.BestMove(new GoCommand("go depth 6 searchmoves g1f3"));

        Assert.AreEqual("g1f3", searchResult.BestMove.UCIString());
        Assert.AreEqual(6, searchResult.Depth);
        Assert.AreNotEqual(EvaluationConstants.SingleMoveScore, Math.Abs(searchResult.Score), "A real evaluation is expected, not the single legal move one");
    }

    [Test]
    public void NodesAreReducedWhenRestricting()
    {
        var unrestricted = GetEngine(Constants.InitialPositionFEN).BestMove(new GoCommand("go depth 8"));
        var restricted = GetEngine(Constants.InitialPositionFEN).BestMove(new GoCommand("go depth 8 searchmoves a2a3"));

        Assert.Less(restricted.Nodes, unrestricted.Nodes);
    }

    [TestCase("go depth 8 searchmoves e2e5")]               // Not a legal move
    [TestCase("go depth 8 searchmoves e7e5")]               // Opponent's move
    [TestCase("go depth 8 searchmoves e1g1")]               // Illegal castling
    [TestCase("go depth 8 searchmoves z9z9 a1a1 e2e4k")]    // Not even moves
    [TestCase("go depth 8 searchmoves")]                    // Empty
    public void InvalidSearchMovesDontRestrictTheSearch(string goCommand)
    {
        var unrestricted = GetEngine(Constants.InitialPositionFEN).BestMove(new GoCommand("go depth 8"));
        var searchResult = GetEngine(Constants.InitialPositionFEN).BestMove(new GoCommand(goCommand));

        Assert.AreEqual(unrestricted.BestMove, searchResult.BestMove);
        Assert.AreEqual(unrestricted.Nodes, searchResult.Nodes);
        Assert.AreEqual(unrestricted.Score, searchResult.Score);
    }

    [Test]
    public void GetSearchMovesIgnoresInvalidIllegalAndDuplicatedMoves()
    {
        var engine = GetEngine(Constants.InitialPositionFEN);

        var moves = engine.GetSearchMoves(new GoCommand("go searchmoves h2h3 e2e5 h2h3 e7e5 e1g1 g1f3"));

        CollectionAssert.AreEqual(new[] { "h2h3", "g1f3" }, moves.Select(m => m.UCIString()));
    }

    [Test]
    public void GetSearchMovesWithoutSearchMoves()
    {
        var engine = GetEngine(Constants.InitialPositionFEN);

        Assert.IsEmpty(engine.GetSearchMoves(new GoCommand("go infinite")));
        Assert.IsEmpty(engine.GetSearchMoves(new GoCommand("go depth 5 searchmoves")));
        Assert.IsEmpty(engine.GetSearchMoves(new GoCommand("go depth 5 searchmoves e2e5")));
    }

    [Test]
    public void GetSearchMovesIgnoresPseudoLegalMoves()
    {
        // The rook on a2 covers the second rank, so Ke1-e2 is pseudolegal but not legal
        var engine = GetEngine("4k3/8/8/8/8/8/r7/4K3 w - - 0 1");

        var moves = engine.GetSearchMoves(new GoCommand("go searchmoves e1e2 e1d1"));

        CollectionAssert.AreEqual(new[] { "e1d1" }, moves.Select(m => m.UCIString()));

        Assert.IsEmpty(engine.GetSearchMoves(new GoCommand("go searchmoves e1e2")));
    }

    [Test]
    public void GetSearchMovesPromotions()
    {
        var white = GetEngine("8/P6k/8/8/8/8/6K1/8 w - - 0 1");

        CollectionAssert.AreEqual(
            new[] { "a7a8n", "a7a8q" },
            white.GetSearchMoves(new GoCommand("go searchmoves a7a8n a7a8Q")).Select(m => m.UCIString()));

        var black = GetEngine("8/6k1/8/8/8/8/p5K1/8 b - - 0 1");

        CollectionAssert.AreEqual(
            new[] { "a2a1r", "a2a1b" },
            black.GetSearchMoves(new GoCommand("go searchmoves a2a1r a2a1B")).Select(m => m.UCIString()));

        Assert.AreEqual("a2a1b", black.BestMove(new GoCommand("go depth 4 searchmoves a2a1b")).BestMove.UCIString());
    }

    [Test]
    public void GetSearchMovesCastling()
    {
        var engine = GetEngine("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");

        var moves = engine.GetSearchMoves(new GoCommand("go searchmoves e1g1 e1c1"));

        CollectionAssert.AreEqual(new[] { "e1g1", "e1c1" }, moves.Select(m => m.UCIString()));
    }

    [Test]
    public void MultiPVLinesAreLimitedBySearchMoves()
    {
        Configuration.EngineSettings.MultiPV = 4;

        var engine = GetEngine(Constants.InitialPositionFEN);
        var searchResult = engine.BestMove(new GoCommand("go depth 8 searchmoves a2a3 h2h3"));

        SearchResult[] lines = [searchResult, .. searchResult.ExtraLines];

        Assert.AreEqual(2, lines.Length);
        CollectionAssert.AreEquivalent(CornerPawnMoves, lines.Select(l => l.BestMove.UCIString()));
        Assert.GreaterOrEqual(lines[0].Score, lines[1].Score);
    }

    [Test]
    public void MultiPVWithMoreSearchMovesThanLines()
    {
        Configuration.EngineSettings.MultiPV = 2;

        var engine = GetEngine(Constants.InitialPositionFEN);
        var searchResult = engine.BestMove(new GoCommand("go depth 8 searchmoves a2a3 h2h3 b2b3 g2g3"));

        SearchResult[] lines = [searchResult, .. searchResult.ExtraLines];

        Assert.AreEqual(2, lines.Length);
        Assert.AreEqual(2, lines.Select(l => l.BestMove).Distinct().Count());
        CollectionAssert.IsSubsetOf(lines.Select(l => l.BestMove.UCIString()).ToArray(), new[] { "a2a3", "h2h3", "b2b3", "g2g3" });
    }

    private static SearchResult[] Lines(SearchResult searchResult) => [searchResult, .. searchResult.ExtraLines];

    [Test]
    public void MultiPVLinesAreIndexedAndSortedByScore()
    {
        Configuration.EngineSettings.MultiPV = 3;

        string[] searchMoves = ["a2a3", "h2h3", "b2b3", "g2g3"];

        var engine = GetEngine(Constants.InitialPositionFEN);
        var lines = Lines(engine.BestMove(new GoCommand($"go depth 7 searchmoves {string.Join(' ', searchMoves)}")));

        Assert.AreEqual(3, lines.Length);
        Assert.AreEqual(3, lines.Select(l => l.BestMove).Distinct().Count());

        for (int i = 0; i < lines.Length; ++i)
        {
            Assert.AreEqual(i + 1, lines[i].MultiPVIndex);
            Assert.AreEqual(lines[i].BestMove, lines[i].Moves[0]);
            Assert.AreEqual(7, lines[i].Depth);
            CollectionAssert.Contains(searchMoves, lines[i].BestMove.UCIString());

            if (i > 0)
            {
                Assert.GreaterOrEqual(lines[i - 1].Score, lines[i].Score, $"Line {i} scores higher than line {i - 1}");
            }
        }
    }

    [Test]
    public void MultiPVEqualToTheNumberOfSearchMoves()
    {
        Configuration.EngineSettings.MultiPV = 3;

        string[] searchMoves = ["a2a3", "h2h3", "g1f3"];

        var engine = GetEngine(Constants.InitialPositionFEN);
        var lines = Lines(engine.BestMove(new GoCommand($"go depth 6 searchmoves {string.Join(' ', searchMoves)}")));

        CollectionAssert.AreEquivalent(searchMoves, lines.Select(l => l.BestMove.UCIString()));
    }

    [Test]
    public void MultiPVWithASingleSearchMove()
    {
        Configuration.EngineSettings.MultiPV = 4;

        var engine = GetEngine(Constants.InitialPositionFEN);
        var searchResult = engine.BestMove(new GoCommand("go depth 6 searchmoves g1f3"));

        Assert.IsEmpty(searchResult.ExtraLines);
        Assert.AreEqual(1, searchResult.MultiPVIndex);
        Assert.AreEqual("g1f3", searchResult.BestMove.UCIString());
        Assert.AreEqual(6, searchResult.Depth);
    }

    [Test]
    public void MultiPVIgnoresInvalidIllegalAndDuplicatedSearchMoves()
    {
        Configuration.EngineSettings.MultiPV = 4;

        var engine = GetEngine(Constants.InitialPositionFEN);
        var lines = Lines(engine.BestMove(new GoCommand("go depth 6 searchmoves a2a3 a2a3 e2e5 h2h3 e7e5")));

        CollectionAssert.AreEquivalent(CornerPawnMoves, lines.Select(l => l.BestMove.UCIString()));
    }

    [Test]
    public void MultiPVWithInvalidSearchMovesDoesntRestrictTheLines()
    {
        Configuration.EngineSettings.MultiPV = 3;

        var unrestricted = Lines(GetEngine(Constants.InitialPositionFEN).BestMove(new GoCommand("go depth 7")));
        var lines = Lines(GetEngine(Constants.InitialPositionFEN).BestMove(new GoCommand("go depth 7 searchmoves e2e5 e7e5")));

        Assert.AreEqual(3, lines.Length);
        CollectionAssert.AreEqual(unrestricted.Select(l => l.BestMove), lines.Select(l => l.BestMove));
        CollectionAssert.AreEqual(unrestricted.Select(l => l.Score), lines.Select(l => l.Score));
    }

    /// <summary>
    /// Lines are built from the root moves list, which can't contain the moves that aren't allowed no matter how big MultiPV is
    /// </summary>
    [Test]
    public void MultiPVBiggerThanSearchMovesAndLegalMoves()
    {
        Configuration.EngineSettings.MultiPV = Constants.MaxMultiPV;

        string[] searchMoves = ["a2a3", "h2h3", "b2b3", "g2g3", "g1f3"];

        var engine = GetEngine(Constants.InitialPositionFEN);
        var lines = Lines(engine.BestMove(new GoCommand($"go depth 5 searchmoves {string.Join(' ', searchMoves)}")));

        CollectionAssert.AreEquivalent(searchMoves, lines.Select(l => l.BestMove.UCIString()));
    }

    /// <summary>
    /// MultiPV is capped by the number of allowed moves, not by the number of legal ones.
    /// Asking for more lines than allowed moves has to be exactly the same as asking for as many lines as allowed moves.
    /// </summary>
    [Test]
    public void MultiPVBiggerThanSearchMovesIsEquivalentToMultiPVEqualToSearchMoves()
    {
        const string goCommand = "go depth 9 searchmoves a2a3 h2h3";

        Configuration.EngineSettings.MultiPV = 2;
        var expected = GetEngine(Constants.InitialPositionFEN).BestMove(new GoCommand(goCommand));

        Configuration.EngineSettings.MultiPV = 5;
        var actual = GetEngine(Constants.InitialPositionFEN).BestMove(new GoCommand(goCommand));

        var expectedLines = Lines(expected);
        var actualLines = Lines(actual);

        Assert.AreEqual(2, actualLines.Length);
        Assert.AreEqual(expected.Nodes, actual.Nodes);

        for (int i = 0; i < expectedLines.Length; ++i)
        {
            Assert.AreEqual(expectedLines[i].MultiPVIndex, actualLines[i].MultiPVIndex);
            Assert.AreEqual(expectedLines[i].Score, actualLines[i].Score, $"Line {i + 1} score");
            CollectionAssert.AreEqual(expectedLines[i].Moves, actualLines[i].Moves, $"Line {i + 1} PV");
        }
    }

    [Test]
    public void MultiPVInfoCommandOnlyContainsSearchMoves()
    {
        Configuration.EngineSettings.MultiPV = 4;

        var engine = GetEngine(Constants.InitialPositionFEN);
        var searchResult = engine.BestMove(new GoCommand("go depth 6 searchmoves a2a3 h2h3"));

        var output = searchResult.ToString().Split(Environment.NewLine);

        Assert.AreEqual(2, output.Length);

        for (int i = 0; i < output.Length; ++i)
        {
            StringAssert.StartsWith($"{InfoCommand.Id} depth 6 ", output[i]);
            StringAssert.Contains($" multipv {i + 1} ", output[i]);
            StringAssert.Contains($" pv {Lines(searchResult)[i].BestMove.UCIString()}", output[i]);
        }

        StringAssert.DoesNotContain(" multipv 3 ", searchResult.ToString());
    }

    /// <summary>
    /// A mate in 1 is the first line when it's an allowed move, and not available when it isn't
    /// </summary>
    [Test]
    public void MultiPVMateAndSearchMoves()
    {
        const string fen = "6k1/5ppp/8/8/8/8/1Q6/1R4K1 w - - 0 1";

        Configuration.EngineSettings.MultiPV = 3;

        var withMate = Lines(GetEngine(fen).BestMove(new GoCommand("go depth 8 searchmoves b2b8 b2c3 g1h1")));

        Assert.AreEqual(3, withMate.Length);
        Assert.AreEqual("b2b8", withMate[0].BestMove.UCIString());
        Assert.AreEqual(1, withMate[0].Mate);

        string[] withoutMateMoves = ["b2c3", "g1h1", "b2a1"];
        var withoutMate = Lines(GetEngine(fen).BestMove(new GoCommand($"go depth 8 searchmoves {string.Join(' ', withoutMateMoves)}")));

        CollectionAssert.AreEquivalent(withoutMateMoves, withoutMate.Select(l => l.BestMove.UCIString()));
        Assert.True(withoutMate.All(l => l.Mate != 1), "Mate in 1 reported without the mating move being allowed");
    }

    /// <summary>
    /// A search cancelled before the first iteration finishes returns an emergency move, which must respect the restriction as well
    /// </summary>
    [Test]
    public void EmergencyMoveIsAmongTheSearchMoves()
    {
        using var cancelledCts = new CancellationTokenSource();
        cancelledCts.Cancel();

        SearchResult EmergencySearch(string goCommand)
        {
            var engine = GetEngine(Constants.InitialPositionFEN);
            var constraints = SearchConstraints.Infinite(engine.GetSearchMoves(new GoCommand(goCommand)));

            return engine.BestMove(in constraints, isPondering: false, cancelledCts.Token, CancellationToken.None);
        }

        var unrestrictedMove = EmergencySearch("go infinite").BestMove.UCIString();

        // Restricting to anything but the move chosen without restrictions
        var allowedMove = CornerPawnMoves.First(m => m != unrestrictedMove);

        var restricted = EmergencySearch($"go infinite searchmoves {allowedMove}");

        Assert.AreEqual(allowedMove, restricted.BestMove.UCIString());
    }

    /// <summary>
    /// The root score and best move of a restricted search can't be reused by unrestricted ones
    /// </summary>
    [Test]
    public void RestrictedSearchDoesntRecordRootInTT()
    {
        var tt = new TranspositionTable();

        try
        {
            tt.Clear();

            _ = GetEngine(Constants.InitialPositionFEN, in tt).BestMove(new GoCommand("go depth 6 searchmoves a2a3"));

            using var game = new Game(Constants.InitialPositionFEN);

            // A static eval only entry (NodeType.Unknown, no best move) is always saved, and it's ignored by the search, so it's harmless
            if (tt.ProbeHash(game.CurrentPosition, 0, 0, out var restrictedEntry))
            {
                Assert.AreEqual(NodeType.Unknown, restrictedEntry.NodeType, "Root position score recorded by a restricted search");
                Assert.AreEqual(0, restrictedEntry.BestMove, "Root best move recorded by a restricted search");
            }

            // Control: an unrestricted search does record the root score and best move
            _ = GetEngine(Constants.InitialPositionFEN, in tt).BestMove(new GoCommand("go depth 6"));

            Assert.True(tt.ProbeHash(game.CurrentPosition, 0, 0, out var unrestrictedEntry), "Root position not recorded by an unrestricted search");
            Assert.AreNotEqual(NodeType.Unknown, unrestrictedEntry.NodeType);
            Assert.AreNotEqual(0, unrestrictedEntry.BestMove);
        }
        finally
        {
            tt.Release();
        }
    }

    [TestCase(1)]
    [TestCase(2)]
    public void FullUCIFlowWithMultiPV(int threads)
    {
        Configuration.EngineSettings.Threads = threads;
        Configuration.EngineSettings.MultiPV = 3;

        var (bestMove, pvFirstMoves, multiPVIndexes) = RunGo("go depth 7 searchmoves a2a3 h2h3", stopAfterMilliseconds: null);

        CollectionAssert.Contains(CornerPawnMoves, bestMove);
        CollectionAssert.IsSubsetOf(pvFirstMoves.Distinct().ToArray(), CornerPawnMoves);

        // Only 2 allowed moves, so there are never more than 2 lines
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, multiPVIndexes.Distinct());
    }

    [TestCase(1)]
    [TestCase(2)]
    public void FullUCIFlowPondering(int threads)
    {
        Configuration.EngineSettings.Threads = threads;
        Configuration.EngineSettings.IsPonder = true;

        var (bestMove, pvFirstMoves, _) = RunGo("go ponder searchmoves h2h3 a2a3", stopAfterMilliseconds: 300);

        CollectionAssert.Contains(CornerPawnMoves, bestMove);
        CollectionAssert.IsSubsetOf(pvFirstMoves.Distinct().ToArray(), CornerPawnMoves);
    }

    [GeneratedRegex(@" pv (\S+)")]
    private static partial Regex PVFirstMoveRegex();

    [GeneratedRegex(@" multipv (\d+) ")]
    private static partial Regex MultiPVIndexRegex();

    /// <summary>
    /// Sends <paramref name="goCommand"/> from the starting position through the UCI handler, waits for the bestmove
    /// (sending stop after <paramref name="stopAfterMilliseconds"/> if provided) and
    /// returns its move together with the first move and the multipv index of every PV printed
    /// </summary>
    private static (string BestMove, List<string> PVFirstMoves, List<int> MultiPVIndexes) RunGo(string goCommand, int? stopAfterMilliseconds)
    {
        var engineChannel = Channel.CreateUnbounded<object>();
        var guiChannel = Channel.CreateUnbounded<string>();

        using var searcher = new Searcher(guiChannel.Reader, engineChannel.Writer);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var handler = new UCIHandler(guiChannel, engineChannel, searcher);

        var run = Task.Run(() => searcher.Run(cts.Token), cts.Token);

        try
        {
            handler.Handle("ucinewgame", cts.Token).GetAwaiter().GetResult();
            handler.Handle("position startpos", cts.Token).GetAwaiter().GetResult();
            handler.Handle(goCommand, cts.Token).GetAwaiter().GetResult();

            if (stopAfterMilliseconds.HasValue)
            {
                Thread.Sleep(stopAfterMilliseconds.Value);
                handler.Handle("stop", cts.Token).GetAwaiter().GetResult();
            }

            var pvFirstMoves = new List<string>();
            var multiPVIndexes = new List<int>();
            string? bestMove = null;

            while (bestMove is null && engineChannel.Reader.WaitToReadAsync(cts.Token).AsTask().GetAwaiter().GetResult())
            {
                while (engineChannel.Reader.TryRead(out var item))
                {
                    if (item is BestMoveCommand bestMoveCommand)
                    {
                        bestMove = bestMoveCommand.ToString().Split(' ')[1];
                        break;
                    }

                    foreach (var line in item.ToString()!.Split(Environment.NewLine))
                    {
                        var match = PVFirstMoveRegex().Match(line);
                        if (match.Success)
                        {
                            pvFirstMoves.Add(match.Groups[1].Value);
                        }

                        var multiPVMatch = MultiPVIndexRegex().Match(line);
                        if (multiPVMatch.Success)
                        {
                            multiPVIndexes.Add(int.Parse(multiPVMatch.Groups[1].Value));
                        }
                    }
                }
            }

            Assert.IsNotNull(bestMove, "No bestmove received");

            return (bestMove!, pvFirstMoves, multiPVIndexes);
        }
        finally
        {
            // Searches must be over before the Searcher (and its TT) gets disposed, even if the test failed
            searcher.StopSearching();
            cts.Cancel();
            _ = Task.WhenAny(run, Task.Delay(2_000)).GetAwaiter().GetResult();
        }
    }
}
