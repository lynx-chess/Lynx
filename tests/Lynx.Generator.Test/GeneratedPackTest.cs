using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Lynx.Generator.Test;

public class GeneratedPackTest
{
    [Test]
    public Task StaticClass()
    {
        // The source code to test
        const string source = @"
using namespace Lynx.Generator;

public static partial class TestClass
{
    [GeneratedPack(1, 2)]
    public static readonly int _TestConstant;
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }

    [Test]
    public Task NonStaticClass()
    {
        // The source code to test
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    [GeneratedPack(-1, 2)]
    public static readonly int _TestConstant;
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }

    [Test]
    public Task PrivateStaticReadonlyField()
    {
        // The source code to test
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    [GeneratedPack(-1, -2)]
    private static readonly int _TestConstant;
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }

    [Test]
    public Task NoNamespaceImport_ShouldNotGenerateConstant()
    {
        // The source code to test
        const string source = @"
public partial class TestClass
{
    [GeneratedPack(1, 2)]
    private static readonly int _TestConstant;
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }

    [Test]
    public Task TwoAttributes_ShouldNotGenerateConstant()
    {
        // The source code to test
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    [GeneratedPack(1, 2)]
    [GeneratedPack(1, 3)]
    private static readonly int _TestConstant;
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }

    [Test]
    public Task NoAttributes_ShouldNotGenerateConstant()
    {
        // The source code to test
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
    private static readonly int _TestConstant;
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }
}