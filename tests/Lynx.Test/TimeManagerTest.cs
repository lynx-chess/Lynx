using Lynx.Model;
using NUnit.Framework;

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

        var game = new Game(Constants.InitialPositionFEN);
        const int nodes = 666;

        var restrictions = TimeManager.CalculateTimeManagement(game, new UCI.Commands.GUI.GoCommand($"go nodes {nodes}"));
        Assert.AreEqual(nodes, restrictions.MaxNodes);
        Assert.AreEqual(Configuration.EngineSettings.Datagen_GenFens_MinHardTimeBound, restrictions.HardLimitTimeBound);
    }

    [Test]
    public void CalculateTimeManagementTest_GoNodes_SoftNodesEnabled()
    {
        Configuration.EngineSettings.SoftNodes = true;

        var game = new Game(Constants.InitialPositionFEN);
        const int nodes = 666;

        var restrictions = TimeManager.CalculateTimeManagement(game, new UCI.Commands.GUI.GoCommand($"go nodes {nodes}"));
        Assert.AreEqual(nodes, restrictions.MaxNodes);
        Assert.AreEqual(Configuration.EngineSettings.Datagen_GenFens_MinHardTimeBound, restrictions.HardLimitTimeBound);
    }
}
