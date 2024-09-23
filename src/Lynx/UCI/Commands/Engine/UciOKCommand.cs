namespace Lynx.UCI.Commands.Engine;

/// <summary>
/// uciok
///	Must be sent after the id and optional options to tell the GUI that the engine
///	has sent all infos and is ready in uci mode.
/// </summary>
public sealed class UciOKCommand : IEngineBaseCommand
{
    public const string Id = "uciok";
}
