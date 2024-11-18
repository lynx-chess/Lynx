using static Lynx.Generator.Test.TestHelpers;

namespace Lynx.Generator.Test;

public class GeneratedPackArrayTest
{
    [Test]
    public Task StaticClass()
    {
        const string source = @"
using namespace Lynx.Generator;

public static partial class TestClass
{
	[GeneratedPackArray(
	[
		""0, 0"",
		""1, 2"",
		""10, 10"",
		""22, 24"",
		""55, 77"",
		""208, 187"",
		""-54, 403""
	])]
	public static readonly int[] TestArray;";

        return VerifyGenerator(new GeneratedPackArrayGenerator(), source);
    }

    [Test]
    public Task Nullable()
    {
        const string source = @"
using namespace Lynx.Generator;

public partial class TestClass
{
	[GeneratedPackArray(
	[
		""0, 0"",
		""1, 2"",
		""10, 10"",
		""22, 24"",
		""55, 77"",
		""208, 187"",
		""-54, 403""
	])]
	public static readonly int[]? TestArray;";

        return VerifyGenerator(new GeneratedPackArrayGenerator(), source);
    }
}
