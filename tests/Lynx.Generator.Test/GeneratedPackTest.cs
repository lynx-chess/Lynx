﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Lynx.Generator.Test;

public class GeneratedPackTest
{
    [Test]
    public Task StaticClass()
    {
        const string source = @"
using namespace Lynx.Generator;

public static partial class TestClass
{
    [GeneratedPack(1, 2)]
    public static readonly int _TestConstant;
}";

        return Verify(source);
    }

    [Test]
    public Task NonStaticClass()
    {
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    [GeneratedPack(-1, 2)]
    public static readonly int _TestConstant;
}";

        return Verify(source);
    }

    [Test]
    public Task PrivateStaticReadonlyField()
    {
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    [GeneratedPack(-1, -2)]
    private static readonly int _TestConstant;
}";

        return Verify(source);
    }

    [Test]
    public Task NamedArguments()
    {
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    [GeneratedPack(mg : -1, eg : -2)]
    private static readonly int _TestConstant;
}";

        return Verify(source);
    }

    [Test]
    public Task MultipleNamespacesAndClasses()
    {
        const string source = @"
using namespace Lynx.Generator;

namespace Namespace1
{
    public partial class TestClass1_1
    {
        [GeneratedPack(0, 1)]
        private static readonly int _TestConstant1_1;
    }

    public partial class TestClass1_2
    {
        [GeneratedPack(2, 3)]
        private static readonly int _TestConstant1_2;
    }
}

namespace Namespace2
{
    public partial class TestClass2_1
    {
        [GeneratedPack(4, 5)]
        private static readonly int _TestConstant2_1;
    }
}";

        return Verify(source);
    }

    [Test]
    public Task NoNamespaceImport_ShouldNotGenerateConstant()
    {
        const string source = @"
public partial class TestClass
{
    [GeneratedPack(1, 2)]
    private static readonly int _TestConstant;
}";

        return Verify(source);
    }

    [Test]
    public Task TwoAttributes_ShouldNotGenerateConstant()
    {
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    [GeneratedPack(1, 2)]
    [GeneratedPack(1, 3)]
    private static readonly int _TestConstant;
}";

        return Verify(source);
    }

    [Test]
    public Task NoAttributes_ShouldNotGenerateConstant()
    {
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    private static readonly int _TestConstant;
}";

        return Verify(source);
    }

    /// <summary>
    /// Based on: https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/
    /// https://github.com/andrewlock/blog-examples/blob/c35edf1c1f0e1f9adf84c215e2ce7ab644b374f5/NetEscapades.EnumGenerators2/tests/NetEscapades.EnumGenerators.Tests/cs
    /// </summary>
    private static Task Verify(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create references for assemblies we require
        // We could add multiple references if required
        IEnumerable<PortableExecutableReference> references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        ];

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references);

        GeneratedPackGenerator generator = new GeneratedPackGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        return Verifier
            .Verify(driver)
            .UseDirectory("Snapshots");
    }
}
