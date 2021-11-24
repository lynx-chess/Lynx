using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.Model;

public class PVTableTest
{
    [Test]
    public void PVTable_Indexes_Support_MaxDepthPlusOne()
    {
        Assert.DoesNotThrow(() => _ = PVTable.Indexes[Configuration.EngineSettings.MaxDepth]);
    }
}
