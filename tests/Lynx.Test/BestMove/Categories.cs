using NUnit.Framework;

public class SampleClass
{

    [Test]
    [Explicit]
    [Category("LongRunning")]
    public void __________________________SampleTest(){ Assert.True(true); }
}
