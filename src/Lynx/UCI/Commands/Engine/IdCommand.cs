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
public sealed class IdCommand : IEngineBaseCommand
{
    public const string IdString = "id";

    public const string EngineName = "Lynx";

    public const string EngineAuthor = "Eduardo Caceres";

    public static string GetLynxVersion()
    {
        return
            Assembly.GetAssembly(typeof(IdCommand))
                !.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion?.Split('+')[0]
                ?? "Unknown";
    }

    public static string NameString => $"id name {EngineName} {GetLynxVersion()}";

    public static string AuthorString => $"id author {EngineAuthor}";
}
