using System.Reflection;

namespace Lynx.UCI.Commands.Engine;

/// <summary>
/// id
///	* name <x>
///		this must be sent after receiving the "uci" command to identify the engine,
///		e.g. "id name Shredder X.Y\n"
///	* author <x>
///		this must be sent after receiving the "uci" command to identify the engine,
///		e.g. "id author Stefan MK\n"
/// </summary>
public sealed class IdCommand
{
    public const string IdString = "id";

    public const string EngineName = "Lynx";

    public const string EngineAuthor = "Eduardo Caceres";

    public static string GetLynxVersion()
    {
        var fullVersion = Assembly.GetAssembly(typeof(IdCommand))
            !.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        var parts = fullVersion?.Split('+');

#if LYNX_RELEASE
        return parts?[0] ?? "Unknown";
#else
        return parts?.Length switch
        {
            >= 2 => $"{parts[0]}-dev-{parts[1][..Math.Min(8, parts[1].Length)]}",
            1 => parts[0],
            _ => "Unknown"
        };
#endif
    }

    public static string NameString => $"id name {EngineName} {GetLynxVersion()}";

    public static string AuthorString => $"id author {EngineAuthor}";
}
