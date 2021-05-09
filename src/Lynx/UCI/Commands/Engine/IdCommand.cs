using System;
using System.IO;
using System.Reflection;

namespace Lynx.UCI.Commands.Engine
{
    /// <summary>
    /// id
    ///	* name <x>
    ///		this must be sent after receiving the "uci" command to identify the engine,
    ///		e.g. "id name Shredder X.Y\n"
    ///	* author <x>
    ///		this must be sent after receiving the "uci" command to identify the engine,
    ///		e.g. "id author Stefan MK\n"
    /// </summary>
    public class IdCommand : EngineBaseCommand
    {
        public const string IdString = "id";

        private static string GetVersion()
        {
            return Assembly.GetAssembly(typeof(IdCommand))!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? "Uknown";
        }

        public static string Name => $"id name Lynx {GetVersion()}";

        public static string Version => "id author Eduardo Cáceres";
    }
}