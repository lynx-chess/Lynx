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
