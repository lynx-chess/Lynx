using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Lynx.Generator.Test;

public class GeneratedPackTest
{
    //[Test]
    //public Task Driver()
    //{
    //    var driver = BuildDriver();

    //    return Verify(driver);
    //}

    //[Test]
    //public Task RunResults()
    //{
    //    var driver = BuildDriver();

    //    var results = driver.GetRunResult();
    //    return Verify(results);
    //}

    //[Test]
    //public Task RunResult()
    //{
    //    var driver = BuildDriver();

    //    var result = driver.GetRunResult().Results.Single();
    //    return Verify(result);
    //}

    [Test]
    public Task GeneratesGeneratedPackCorrectly()
    {
        // The source code to test
        const string source = @"
using namespace Lynx.Generator;

public static partial class EvaluationParams
{
    [GeneratedPack(-1, 2)]
    private static readonly int _IsolatedPawnPenalty;
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }

    private static GeneratorDriver BuildDriver()
    {
        var compilation = CSharpCompilation.Create(nameof(GeneratedPackTest));
        var generator = new GeneratedPackGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);
        return driver.RunGenerators(compilation);
    }
}