namespace Lynx.UCI.Commands.Engine;

/// <summary>
/// copyprotection
///	this is needed for copyprotected engines. After the uciok command the engine can tell the GUI,
///	that it will check the copy protection now. This is done by "copyprotection checking".
///	If the check is ok the engine should sent "copyprotection ok", otherwise "copyprotection error".
///	If there is an error the engine should not function properly but should not quit alone.
///	If the engine reports "copyprotection error" the GUI should not use this engine
///	and display an error message instead!
///	The code in the engine can look like this
///	    TellGUI("copyprotection checking\n");
///     ... check the copy protection here ...
///     if (ok)
///	        TellGUI("copyprotection ok\n");
///     else
///	        TellGUI("copyprotection error\n");
/// </summary>
public sealed class CopyProtectionCommand : EngineBaseCommand
{
    public const string Id = "copyprotection";
}
