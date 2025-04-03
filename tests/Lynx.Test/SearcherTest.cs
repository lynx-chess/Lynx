using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using NUnit.Framework;
using System.Threading.Channels;

namespace Lynx.Test;

public class SearcherTest
{
    [Test]
    [Explicit]
    [NonParallelizable]
    [Category(Categories.LongRunning)]
    public async Task OnPonderHit_ShouldNotDoubleSearchOrProduceIllegalMoves()
    {
        Configuration.EngineSettings.IsPonder = true;

        // Position by toanth
        const string PositionWithSearchExplosionOnDepth1 = "r1n1n1b1/1P1P1P1P/1N1N1N2/2RnQrRq/2pKp3/3BNQbQ/k7/4Bq2 w - - 0 1";
        var positionCommand = $"position fen {PositionWithSearchExplosionOnDepth1}";

        var guiChannel = Channel.CreateUnbounded<string>();
        var engineChannel = Channel.CreateUnbounded<object>();

        var searcher = new Searcher(guiChannel.Reader, engineChannel.Writer);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Run(() => searcher.Run(CancellationToken.None));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        searcher.AdjustPosition(positionCommand);

        guiChannel.Writer.TryWrite("go ponder wtime 60000 btime 60000 binc 1000 winc 1000 ponder");
        await searcher.PonderHit();

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(30_000);

        var bestMoveCount = 0;
        SearchResult? lastInfo = null;
        SearchResult? firstBestMoveInfo = null;
        List<BestMoveCommand> bestMoveCommands = [];

        try
        {
            await foreach (var result in engineChannel.Reader.ReadAllAsync(cts.Token))
            {
                if (result is SearchResult searchResult)
                {
                    lastInfo = searchResult;
                    continue;
                }

                if (result is BestMoveCommand b)
                {
                    bestMoveCommands.Add(b);
                    ++bestMoveCount;

                    if (bestMoveCount == 1)
                    {
                        firstBestMoveInfo = lastInfo;
                        if (firstBestMoveInfo?.Depth == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
#pragma warning disable S108 // Nested blocks of code should not be left empty
        catch (OperationCanceledException)
        {
        }
#pragma warning restore S108 // Nested blocks of code should not be left empty
        catch (Exception)
        {
            Assert.Fail();
        }

        // Test https://github.com/lynx-chess/Lynx/pull/1613
        Assert.True(bestMoveCount == 1                                      // Ponderhit was processed before the first search started.
#if DEBUG
            || bestMoveCount == 0                                           // Not enough time to complete the first search
#endif
            || (bestMoveCount == 2 && firstBestMoveInfo?.Depth == 0));      // Ponderhit was processed late, after the first search started but it cancelled it early enough

        // Test https://github.com/lynx-chess/Lynx/pull/1610
        var originalPosition = new Position(PositionWithSearchExplosionOnDepth1);
        var allPseudoLegalMoves = MoveGenerator.GenerateAllMoves(originalPosition);

        foreach(var bestMove in bestMoveCommands)
        {
            var bestMoveString = bestMove.ToString().Split(' ')[1];
            Assert.True(allPseudoLegalMoves.Select(m => m.UCIString()).Contains(bestMoveString));
        }
    }
}
