namespace Lynx.UCI.Commands.Engine;

/// <summary>
/// readyok
///	This must be sent when the engine has received an "isready" command and has
///	processed all input and is ready to accept new commands now.
///	It is usually sent after a command that can take some time to be able to wait for the engine,
///	but it can be used anytime, even when the engine is searching,
///	and must always be answered with "isready".
/// </summary>
public sealed class ReadyOKCommand : EngineBaseCommand
{
    public const string Id = "readyok";
}
