namespace Lynx.UCI.Commands.GUI
{
    /// <summary>
    /// debug [ on | off ]
    ///	switch the debug mode of the engine on and off.
    ///	In debug mode the engine should sent additional infos to the GUI, e.g. with the "info string" command,
    ///	to help debugging, e.g. the commands that the engine has received etc.
    ///	This mode should be switched off by default and this command can be sent
    ///	any time, also when the engine is thinking.
    /// </summary>
    public class DebugCommand : GUIBaseCommand
    {
        public const string Id = "debug";

        public static bool Parse(string command)
        {
            return string.Equals(
                "on",
                command.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)[1],
                System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
