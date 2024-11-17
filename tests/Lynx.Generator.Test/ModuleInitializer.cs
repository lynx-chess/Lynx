using System.Runtime.CompilerServices;

namespace Lynx.Generator.Test;
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init() =>
        VerifySourceGenerators.Initialize();
}