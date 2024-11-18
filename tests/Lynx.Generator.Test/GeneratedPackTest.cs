using static Lynx.Generator.Test.TestHelpers;

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

        return VerifyGenerator(new GeneratedPackGenerator(), source);
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

        return VerifyGenerator(new GeneratedPackGenerator(), source);
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

        return VerifyGenerator(new GeneratedPackGenerator(), source);
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

        return VerifyGenerator(new GeneratedPackGenerator(), source);
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

        return VerifyGenerator(new GeneratedPackGenerator(), source);
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

        return VerifyGenerator(new GeneratedPackGenerator(), source);
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

        return VerifyGenerator(new GeneratedPackGenerator(), source);
    }
}
