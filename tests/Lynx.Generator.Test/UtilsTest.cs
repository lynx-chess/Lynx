namespace Lynx.Generator.Test;

public class UtilsTest
{
    [TestCase(-1, 2, 131071)]
    [TestCase(1, -2, -131073)]
    [TestCase(-1, -2, 131073)]
    public void Pack(short mg, short eg, int packedValue)
    {
        Assert.AreEqual(packedValue, Utils.Pack(mg, eg));
    }
}
