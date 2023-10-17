namespace Lynx.UCI.Commands.GUI;

/// <summary>
/// debug [ on | off ]
///	switch the debug mode of the engine on and off.
///	In debug mode the engine should sent additional infos to the GUI, e.g. with the "info string" command,
///	to help debugging, e.g. the commands that the engine has received etc.
///	This mode should be switched off by default and this command can be sent
///	any time, also when the engine is thinking.
/// </summary>
public sealed class DebugCommand : GUIBaseCommand
{
    public const string Id = "debug";

    /// <summary>
    /// Parse debug command
    /// </summary>
    /// <param name="command"></param>
    /// <returns>
    /// true if debug command sent 'on'
    /// false if debug command sent 'off'
    /// <see cref="Configuration.IsDebug"/> if something else was sent
    ///
    /// </returns>
    public static bool Parse(ReadOnlySpan<char> command)
    {
        const string on = "on";
        const string off= "off";

        Span<Range> items = stackalloc Range[2];
        command.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries);

        var debugValue = command[items[1]];

        return debugValue.Equals(on, StringComparison.OrdinalIgnoreCase)
            || (!debugValue.Equals(off, StringComparison.OrdinalIgnoreCase)
                && Configuration.IsDebug);
    }
}
