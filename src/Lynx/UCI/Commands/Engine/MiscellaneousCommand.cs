
namespace Lynx.UCI.Commands.Engine;


public sealed class MiscellaneousCommand : EngineBaseCommand
{
    private readonly string _message;

    public MiscellaneousCommand(string message)
    {
        _message = message;
    }

    public override string ToString() => _message;
}
