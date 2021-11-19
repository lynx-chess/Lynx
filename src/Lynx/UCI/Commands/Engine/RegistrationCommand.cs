namespace Lynx.UCI.Commands.Engine;

/// <summary>
/// registration
///	this is needed for engines that need a username and / or a code to function with all features.
///	Analog to the "copyprotection" command the engine can send "registration checking"
///	after the uciok command followed by either "registration ok" or "registration error".
///	Also after every attempt to register the engine it should answer with "registration checking"
///	and then either "registration ok" or "registration error".
///	In contrast to the "copyprotection" command, the GUI can use the engine after the engine has
///	reported an error, but should inform the user that the engine is not properly registered
///	and might not use all its features.
///	In addition the GUI should offer to open a dialog to
///	enable registration of the engine.To try to register an engine the GUI can send
///	the "register" command.
///	The GUI has to always answer with the "register" command if the engine sends "registration error"
///	at engine startup (this can also be done with "register later")
///	and tell the user somehow that the engine is not registered.
///	This way the engine knows that the GUI can deal with the registration procedure and the user
///	will be informed that the engine is not properly registered.
/// </summary>
public sealed class RegistrationCommand : EngineBaseCommand
{
    public const string Id = "uciok";
}
