namespace Lynx.UCI.Commands.GUI
{
    /// <summary>
    /// isready
    ///	this is used to synchronize the engine with the GUI. When the GUI has sent a command or
    ///	multiple commands that can take some time to complete,
    ///	this command can be used to wait for the engine to be ready again or
    ///	to ping the engine to find out if it is still alive.
    ///	E.g. this should be sent after setting the path to the tablebases as this can take some time.
    ///	This command is also required once before the engine is asked to do any search
    ///	to wait for the engine to finish initializing.
    ///	This command must always be answered with "readyok" and can be sent also when the engine is calculating
    ///	in which case the engine should also immediately answer with "readyok" without stopping the search.
    /// </summary>
    public sealed class IsReadyCommand : GUIBaseCommand
    {
        public const string Id = "isready";
    }
}
