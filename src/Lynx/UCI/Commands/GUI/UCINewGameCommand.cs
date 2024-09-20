namespace Lynx.UCI.Commands.GUI;

/// <summary>
/// ucinewgame
/// this is sent to the engine when the next search (started with "position" and "go") will be from
/// a different game. This can be a new game the engine should play or a new game it should analyse but
/// also the next position from a testsuite with positions only.
/// If the GUI hasn't sent a "ucinewgame" before the first "position" command, the engine shouldn't
/// expect any further ucinewgame commands as the GUI is probably not supporting the ucinewgame command.
/// So the engine should not rely on this command even though all new GUIs should support it.
/// As the engine's reaction to "ucinewgame" can take some time the GUI should always send "isready"
/// after "ucinewgame" to wait for the engine to finish its operation.
/// </summary>
public sealed class UCINewGameCommand : IGUIBaseCommand
{
    public const string Id = "ucinewgame";
}
