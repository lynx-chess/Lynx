using Lynx.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Lynx.Test;

[Explicit]
[Category(Categories.Configuration)]
[NonParallelizable]
public class TimeManagerTest
{
    [Test]
    public void CalculateTimeManagementTest_GoNodes_SoftNodesDisabled()
    {
        Configuration.EngineSettings.SoftNodes = false;
        Configuration.EngineSettings.SoftNodeFudging = 0;

        var game = new Game(Constants.InitialPositionFEN);
        const int nodes = 666;

        var restrictions = TimeManager.CalculateTimeManagement(game, new UCI.Commands.GUI.GoCommand($"go nodes {nodes}"), []);
        Assert.AreEqual(nodes, restrictions.MaxNodes);
        Assert.AreEqual(Configuration.EngineSettings.Datagen_GenFens_MinHardTimeBound, restrictions.HardLimitTimeBound);
        Assert.IsEmpty(restrictions.SearchMoves);
    }

    [Test]
    public void CalculateTimeManagementTest_GoNodes_SoftNodesEnabled()
    {
        Configuration.EngineSettings.SoftNodes = true;

        var game = new Game(Constants.InitialPositionFEN);
        const int nodes = 666;

        var restrictions = TimeManager.CalculateTimeManagement(game, new UCI.Commands.GUI.GoCommand($"go nodes {nodes}"), []);
        Assert.AreEqual(nodes, restrictions.MaxNodes);
        Assert.AreEqual(Configuration.EngineSettings.Datagen_GenFens_MinHardTimeBound, restrictions.HardLimitTimeBound);
    }

    [TestCase("go depth 5")]
    [TestCase("go infinite")]
    [TestCase("go wtime 60000 btime 60000 winc 1000 binc 1000")]
    [TestCase("go movetime 500")]
    [TestCase("go nodes 666")]
    public void CalculateTimeManagementTest_SearchMoves(string goCommand)
    {
        var game = new Game(Constants.InitialPositionFEN);
        Move[] searchMoves = [MoveGenerator.GenerateAllMoves(game.CurrentPosition).First()];

        var restrictions = TimeManager.CalculateTimeManagement(game, new UCI.Commands.GUI.GoCommand(goCommand), searchMoves);

        CollectionAssert.AreEqual(searchMoves, restrictions.SearchMoves);
    }
}
