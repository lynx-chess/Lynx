namespace Lynx.UCI.Commands.GUI
{
    /// <summary>
    /// stop
    ///	stop calculating as soon as possible,
    ///	don't forget the "bestmove" and possibly the "ponder" token when finishing the search
    /// </summary>
    public class StopCommand : GUIBaseCommand
    {
        public const string Id = "stop";
    }
}
