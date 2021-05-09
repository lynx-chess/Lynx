namespace Lynx.UCI.Commands.Engine
{
    /// <summary>
    /// uciok
    ///	Must be sent after the id and optional options to tell the GUI that the engine
    ///	has sent all infos and is ready in uci mode.
    /// </summary>
    public class UciOKCommand : EngineBaseCommand
    {
        public const string Id = "uciok";
    }
}
