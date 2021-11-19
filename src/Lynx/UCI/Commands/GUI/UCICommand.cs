namespace Lynx.UCI.Commands.GUI;

/// <summary>
/// uci
///	tell engine to use the uci (universal chess interface),
///	this will be send once as a first command after program boot
///	to tell the engine to switch to uci mode.
///	After receiving the uci command the engine must identify itself with the "id" command
///	and sent the "option" commands to tell the GUI which engine settings the engine supports if any.
///	After that the engine should sent "uciok" to acknowledge the uci mode.
///	If no uciok is sent within a certain time period, the engine task will be killed by the GUI.
/// </summary>
public sealed class UCICommand : GUIBaseCommand
{
    public const string Id = "uci";
}
