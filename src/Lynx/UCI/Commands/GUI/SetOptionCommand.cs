namespace Lynx.UCI.Commands.GUI;

/// <summary>
/// setoption name [value]
///	this is sent to the engine when the user wants to change the internal parameters
///	of the engine. For the "button" type no value is needed.
///	One string will be sent for each parameter and this will only be sent when the engine is waiting.
///	The name of the option in should not be case sensitive and can inludes spaces like also the value.
///	The substrings "value" and "name" should be avoided in and to allow unambiguous parsing,
///	for example do not use = "draw value".
///	Here are some strings for the example below:
///	   "setoption name Nullmove value true\n"
///    "setoption name Selectivity value 3\n"
///	   "setoption name Style value Risky\n"
///	   "setoption name Clear Hash\n"
///	   "setoption name NalimovPath value c:\chess\tb\4;c:\chess\tb\5\n"
/// </summary>
public sealed class SetOptionCommand
{
    public const string Id = "setoption";
}
