using Lynx.Datagen;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;

namespace Lynx.Test.Datagen;

[Explicit]
[Category(Categories.Datagen)]
public class ViriformatFilterConfigBindTest
{
    [Test]
    public void AllProperties_AreBoundFromJson()
    {
        var json = @"{
  ""MinPly"": 5,
  ""MinPieces"": 6,
  ""MaxEval"": 12345,
  ""FilterTactical"": false,
  ""FilterCheck"": false,
  ""FilterCastling"": true,
  ""MaxEvalIncorrectness"": 77,
  ""RandomFenSkipping"": true,
  ""RandomFenSkipProbability"": 0.25,
  ""MaterialCountFiltered"": true,
  ""MaterialCountProbabilities"": [0.0, 0.1, 0.2, 0.3],
  ""WdlFiltered"": true,
  ""WdlModelParamsA"": [1.1, 2.2, 3.3, 4.4],
  ""WdlModelParamsB"": [5.5, 6.6, 7.7, 8.8],
  ""MaterialMin"": 10,
  ""MaterialMax"": 20,
  ""MomTarget"": 42,
  ""WdlHeuristicScale"": 2.5
}";

        var tmp = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tmp, json);

            var config = new ConfigurationBuilder()
                .AddJsonFile(tmp, optional: false, reloadOnChange: false)
                .Build();

            var filter = new ViriformatFilter();
            config.Bind(filter);

            Assert.AreEqual(5, filter.MinPly);
            Assert.AreEqual(6, filter.MinPieces);
            Assert.AreEqual(12345u, filter.MaxEval);
            Assert.AreEqual(false, filter.FilterTactical);
            Assert.AreEqual(false, filter.FilterCheck);
            Assert.AreEqual(true, filter.FilterCastling);
            Assert.AreEqual(77u, filter.MaxEvalIncorrectness);
            Assert.AreEqual(true, filter.RandomFenSkipping);
            Assert.AreEqual(0.25, filter.RandomFenSkipProbability, 1e-9);
            Assert.AreEqual(true, filter.MaterialCountFiltered);
            Assert.IsNotNull(filter.MaterialCountProbabilities);
            Assert.AreEqual(4, filter.MaterialCountProbabilities.Length);
            Assert.AreEqual(0.2, filter.MaterialCountProbabilities[2], 1e-9);

            Assert.AreEqual(true, filter.WdlFiltered);
            Assert.IsNotNull(filter.WdlModelParamsA);
            Assert.IsNotNull(filter.WdlModelParamsB);
            Assert.AreEqual(4, filter.WdlModelParamsA.Length);
            Assert.AreEqual(4, filter.WdlModelParamsB.Length);
            Assert.AreEqual(1.1, filter.WdlModelParamsA[0], 1e-9);
            Assert.AreEqual(8.8, filter.WdlModelParamsB[3], 1e-9);

            Assert.AreEqual(10, filter.MaterialMin);
            Assert.AreEqual(20, filter.MaterialMax);
            Assert.AreEqual(42, filter.MomTarget);
            Assert.AreEqual(2.5, filter.WdlHeuristicScale, 1e-9);
        }
        finally
        {
            try { File.Delete(tmp); } catch { }
        }
    }
}
