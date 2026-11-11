using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Channels;

namespace Lynx.Test;

/// <summary>
/// Modifies <see cref="Configuration.EngineSettings.MultiPV"/>, so they need to run in isolation
/// </summary>
[Category(Categories.Configuration)]
[NonParallelizable]
public class MultiPVTest : BaseTest
{
    private static readonly string[] StartPositionMoves =
    [
        "a2a3", "a2a4", "b2b3", "b2b4", "c2c3", "c2c4", "d2d3", "d2d4", "e2e3", "e2e4",
        "f2f3", "f2f4", "g2g3", "g2g4", "h2h3", "h2h4", "b1a3", "b1c3", "g1f3", "g1h3"
    ];

    private int _originalMultiPV;
    private int _originalThreads;

    [SetUp]
    public void SetUp()
    {
        _originalMultiPV = Configuration.EngineSettings.MultiPV;
        _originalThreads = Configuration.EngineSettings.Threads;
    }

    [TearDown]
    public void TearDown()
    {
        Configuration.EngineSettings.MultiPV = _originalMultiPV;
        Configuration.EngineSettings.Threads = _originalThreads;
    }

    private static SearchResult[] Lines(SearchResult searchResult) => [searchResult, .. searchResult.ExtraLines];

    [Test]
    public void AllLegalMovesAreReturnedSortedByScore()
    {
        Configuration.EngineSettings.MultiPV = Constants.MaxMultiPV;

        var lines = Lines(SearchBestMove(Constants.InitialPositionFEN, depth: 6));

        Assert.AreEqual(StartPositionMoves.Length, lines.Length);

        var moves = lines.Select(l => l.BestMove.UCIString()).ToList();
        CollectionAssert.AreEquivalent(StartPositionMoves, moves);

        for (int i = 0; i < lines.Length; ++i)
        {
            Assert.AreEqual(i + 1, lines[i].MultiPVIndex);
            Assert.AreEqual(lines[i].BestMove, lines[i].Moves[0]);
            Assert.AreEqual(6, lines[i].Depth);

            if (i > 0)
            {
                Assert.GreaterOrEqual(lines[i - 1].Score, lines[i].Score, $"Line {i} scores higher than line {i - 1}");
            }
        }
    }

    [Test]
    public void NumberOfLinesIsLimitedByMultiPV()
    {
        Configuration.EngineSettings.MultiPV = 4;

        var lines = Lines(SearchBestMove(Constants.InitialPositionFEN, depth: 8));

        Assert.AreEqual(4, lines.Length);
        Assert.AreEqual(4, lines.Select(l => l.BestMove).Distinct().Count());
    }

    [Test]
    public void NumberOfLinesIsLimitedByLegalMoves()
    {
        Configuration.EngineSettings.MultiPV = 10;

        // Black can only play h6 or h5
        var lines = Lines(SearchBestMove("7k/5Q1p/8/6K1/8/8/8/8 b - - 0 1", depth: 6));

        CollectionAssert.AreEquivalent(new[] { "h7h6", "h7h5" }, lines.Select(l => l.BestMove.UCIString()));
    }

    [Test]
    public void MateIsTheFirstLine()
    {
        Configuration.EngineSettings.MultiPV = 3;

        var lines = Lines(SearchBestMove("6k1/5ppp/8/8/8/8/1Q6/1R4K1 w - - 0 1", depth: 8));

        Assert.AreEqual(3, lines.Length);
        Assert.AreEqual("b2b8", lines[0].BestMove.UCIString());
        Assert.AreEqual(1, lines[0].Mate);
        Assert.AreNotEqual(1, lines[1].Mate);
        Assert.GreaterOrEqual(lines[1].Score, lines[2].Score);
    }

    [Test]
    public void BestMoveIsTheFirstLineOne()
    {
        const int depth = 9;

        Configuration.EngineSettings.MultiPV = 1;
        var singleLine = SearchBestMove(Constants.InitialPositionFEN, depth);

        Configuration.EngineSettings.MultiPV = 3;
        var multiLine = SearchBestMove(Constants.InitialPositionFEN, depth);

        Assert.IsEmpty(singleLine.ExtraLines);
        Assert.AreEqual(1, singleLine.MultiPVIndex);

        Assert.AreEqual(2, multiLine.ExtraLines.Length);
        Assert.AreEqual(1, multiLine.MultiPVIndex);
        Assert.AreEqual(multiLine.BestMove, multiLine.Moves[0]);
    }

    [Test]
    public void InfoCommandContainsAllTheLines()
    {
        Configuration.EngineSettings.MultiPV = 3;

        var searchResult = SearchBestMove(Constants.InitialPositionFEN, depth: 6);
        var output = searchResult.ToString().Split(Environment.NewLine);

        Assert.AreEqual(3, output.Length);

        for (int i = 0; i < output.Length; ++i)
        {
            StringAssert.StartsWith($"{InfoCommand.Id} depth 6 ", output[i]);
            StringAssert.Contains($" multipv {i + 1} ", output[i]);
            StringAssert.Contains($" pv {(i == 0 ? searchResult : searchResult.ExtraLines[i - 1]).BestMove.UCIString()}", output[i]);
        }
    }

    [Test]
    public void SingleLineInfoCommandIsNotAffected()
    {
        Configuration.EngineSettings.MultiPV = 1;

        var output = SearchBestMove(Constants.InitialPositionFEN, depth: 6).ToString();

        StringAssert.Contains(" multipv 1 ", output);
        Assert.False(output.Contains('\n', StringComparison.Ordinal));
    }

    [Test]
    public void InfoCommandsAreWrittenOncePerDepth()
    {
        Configuration.EngineSettings.MultiPV = 3;

        var channel = Channel.CreateUnbounded<object>();
        var engine = new Engine(channel.Writer);
        engine.SetGame(new Game(Constants.InitialPositionFEN));

        var searchResult = engine.BestMove(new GoCommand("go depth 5"));

        var written = new List<string>();
        while (channel.Reader.TryRead(out var item))
        {
            written.Add(item.ToString()!);
        }

        Assert.AreEqual(5, written.Count);
        Assert.True(written.All(w => w.Split(Environment.NewLine).Length == 3), "Each depth should be written as 3 lines");
        Assert.AreEqual(5, searchResult.Depth);
    }

    [Test]
    public void HelperThreadsDontChangeTheLines()
    {
        Configuration.EngineSettings.MultiPV = 3;
        Configuration.EngineSettings.Threads = 2;

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
            handler.Handle("go depth 8", cts.Token).GetAwaiter().GetResult();

            var multiPVLines = 0;
            bool bestMoveFound = false;
            while (!bestMoveFound && engineChannel.Reader.WaitToReadAsync(cts.Token).AsTask().GetAwaiter().GetResult())
            {
                while (engineChannel.Reader.TryRead(out var item))
                {
                    var text = item.ToString()!;
                    if (item is BestMoveCommand)
                    {
                        bestMoveFound = true;
                        break;
                    }

                    multiPVLines = text.Split(Environment.NewLine).Length;
                }
            }

            Assert.True(bestMoveFound);
            Assert.AreEqual(3, multiPVLines);
        }
        finally
        {
            cts.Cancel();
        }
    }
}
