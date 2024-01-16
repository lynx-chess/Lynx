using System.Collections.Immutable;

namespace Lynx.UCI.Commands.Engine;

#pragma warning disable RCS1243 // Duplicate word in a comment.
/// <summary>
/// option
///	This command tells the GUI which parameters can be changed in the engine.
///	This should be sent once at engine startup after the "uci" and the "id" commands
///	if any parameter can be changed in the engine.
///	The GUI should parse this and build a dialog for the user to change the settings.
///	Note that not every option needs to appear in this dialog as some options like
///	"Ponder", "UCI_AnalyseMode", etc. are better handled elsewhere or are set automatically.
///	If the user wants to change some settings, the GUI will send a "setoption" command to the engine.
///	Note that the GUI need not send the setoption command when starting the engine for every option if
///	it doesn't want to change the default value.
///	For all allowed combinations see the examples below,
///	as some combinations of this tokens don't make sense.
///	One string will be sent for each parameter.
///	* name <id>
///		The option has the name id.
///		Certain options have a fixed value for <id>, which means that the semantics of this option is fixed.
///		Usually those options should not be displayed in the normal engine options window of the GUI but
///		get a special treatment. "Pondering" for example should be set automatically when pondering is
///		enabled or disabled in the GUI options. The same for "UCI_AnalyseMode" which should also be set
///		automatically by the GUI. All those certain options have the prefix "UCI_" except for the
///		first 6 options below. If the GUI gets an unknown Option with the prefix "UCI_", it should just
///		ignore it and not display it in the engine's options dialog.
///		* <id> = Hash, type is spin
///			the value in MB for memory for hash tables can be changed,
///			this should be answered with the first "setoptions" command at program boot
///			if the engine has sent the appropriate "option name Hash" command,
///			which should be supported by all engines!
///			So the engine should use a very small hash first as default.
///		* <id> = NalimovPath, type string
///			this is the path on the hard disk to the Nalimov compressed format.
///			Multiple directories can be concatenated with ";"
///		* <id> = NalimovCache, type spin
///			this is the size in MB for the cache for the nalimov table bases
///			These last two options should also be present in the initial options exchange dialog
///			when the engine is booted if the engine supports it
///		* <id> = Ponder, type check
///			this means that the engine is able to ponder.
///			The GUI will send this whenever pondering is possible or not.
///			Note: The engine should not start pondering on its own if this is enabled, this option is only
///			needed because the engine might change its time management algorithm when pondering is allowed.
///		* <id> = OwnBook, type check
///			this means that the engine has its own book which is accessed by the engine itself.
///			if this is set, the engine takes care of the opening book and the GUI will never
///			execute a move out of its book for the engine. If this is set to false by the GUI,
///			the engine should not access its own book.
///		* <id> = MultiPV, type spin
///			the engine supports multi best line or k-best mode. the default value is 1
///		* <id> = UCI_ShowCurrLine, type check, should be false by default,
///			the engine can show the current line it is calculating. see "info currline" above.
///		* <id> = UCI_ShowRefutations, type check, should be false by default,
///			the engine can show a move and its refutation in a line. see "info refutations" above.
///		* <id> = UCI_LimitStrength, type check, should be false by default,
///			The engine is able to limit its strength to a specific Elo number,
///		    This should always be implemented together with "UCI_Elo".
///		* <id> = UCI_Elo, type spin
///			The engine can limit its strength in Elo within this interval.
///			If UCI_LimitStrength is set to false, this value should be ignored.
///			If UCI_LimitStrength is set to true, the engine should play with this specific strength.
///		   This should always be implemented together with "UCI_LimitStrength".
///		* <id> = UCI_AnalyseMode, type check
///		   The engine wants to behave differently when analysing or playing a game.
///		   For example when playing it can use some kind of learning.
///		   This is set to false if the engine is playing a game, otherwise it is true.
///		 * <id> = UCI_Opponent, type string
///		   With this command the GUI can send the name, title, elo and if the engine is playing a human
///		   or computer to the engine.
///		   The format of the string has to be [GM|IM|FM|WGM|WIM|none][<elo>|none][computer|human] <name>
///		   Examples:
///		   "setoption name UCI_Opponent value GM 2800 human Gary Kasparov"
///		   "setoption name UCI_Opponent value none none computer Shredder"
///		 * <id> = UCI_EngineAbout, type string
///		   With this command, the engine tells the GUI information about itself, for example a license text,
///		   usually it doesn't make sense that the GUI changes this text with the setoption command.
///		   Example:
///			"option name UCI_EngineAbout type string default Shredder by Stefan Meyer-Kahlen, see www.shredderchess.com"
///		* <id> = UCI_ShredderbasesPath, type string
///			this is either the path to the folder on the hard disk containing the Shredder endgame databases or
///			the path and filename of one Shredder endgame datbase.
///	   * <id> = UCI_SetPositionValue, type string
///		    the GUI can send this to the engine to tell the engine to use a certain value in centipawns from white's
///		    point of view if evaluating this specifix position.
///		    The string can have the formats:
///	            <value> + <fen> | clear + <fen> | clearall
///	* type <t>
///		The option has type t.
///		There are 5 different types of options the engine can send
///		* check
///			a checkbox that can either be true or false
///		* spin
///			a spin wheel that can be an integer in a certain range
///		* combo
///			a combo box that can have different predefined strings as a value
///		* button
///			a button that can be pressed to send a command to the engine
///		* string
///			a text field that has a string as a value,
///			an empty string has the value "<empty>"
///	* default <x>
///		the default value of this parameter is x
///	* min <x>
///		the minimum value of this parameter is x
///	* max <x>
///		the maximum value of this parameter is x
///	* var <x>
///		a predefined value of this parameter is x
///	Examples:
///	Here are 5 strings for each of the 5 possible types of options
///	    "option name Nullmove type check default true\n"
///	    "option name Selectivity type spin default 2 min 0 max 4\n"
///	    "option name Style type combo default Normal var Solid var Normal var Risky\n"
///	    "option name NalimovPath type string default c:\\n"
///	    "option name Clear Hash type button\n"
/// </summary>
#pragma warning restore RCS1243 // Duplicate word in a comment.

public sealed class OptionCommand : EngineBaseCommand
{
    public const string Id = "option";

    public static readonly ImmutableArray<string> AvailableOptions =
    [
        "option name UCI_Opponent type string",
        $"option name UCI_EngineAbout type string default {IdCommand.EngineName} by {IdCommand.EngineAuthor}, see https://github.com/lynx-chess/Lynx",
        $"option name UCI_ShowWDL type check default {Configuration.EngineSettings.ShowWDL}",
        $"option name Hash type spin default {Configuration.EngineSettings.TranspositionTableSize} min 0 max 1024",
        $"option name OnlineTablebaseInRootPositions type check default {Configuration.EngineSettings.UseOnlineTablebaseInRootPositions}",
        //$"option name OnlineTablebaseInSearch type check default {Configuration.EngineSettings.UseOnlineTablebaseInSearch}",
        "option name Threads type spin default 1 min 1 max 1",
        $"option name Ponder type check default {Configuration.EngineSettings.IsPonder}",

        #region Search tuning

        //$"option name {nameof(Configuration.EngineSettings.LMR_MinDepth)} type spin default {Configuration.EngineSettings.LMR_MinDepth} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves)} type spin default {Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.LMR_Base)} type spin default {100 * Configuration.EngineSettings.LMR_Base} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.LMR_Divisor)} type spin default {100 * Configuration.EngineSettings.LMR_Divisor} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.NMP_MinDepth)} type spin default {Configuration.EngineSettings.NMP_MinDepth} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.NMP_BaseDepthReduction)} type spin default {Configuration.EngineSettings.NMP_BaseDepthReduction} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.AspirationWindow_Delta)} type spin default {Configuration.EngineSettings.AspirationWindow_Delta} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.AspirationWindow_MinDepth)} type spin default {Configuration.EngineSettings.AspirationWindow_MinDepth} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.RFP_MaxDepth)} type spin default {Configuration.EngineSettings.RFP_MaxDepth} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.RFP_DepthScalingFactor)} type spin default {Configuration.EngineSettings.RFP_DepthScalingFactor} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.Razoring_MaxDepth)} type spin default {Configuration.EngineSettings.Razoring_MaxDepth} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.Razoring_Depth1Bonus)} type spin default {Configuration.EngineSettings.Razoring_Depth1Bonus} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.Razoring_NotDepth1Bonus)} type spin default {Configuration.EngineSettings.Razoring_NotDepth1Bonus} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.IIR_MinDepth)} type spin default {Configuration.EngineSettings.IIR_MinDepth} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.LMP_MaxDepth)} type spin default {Configuration.EngineSettings.LMP_MaxDepth} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.LMP_BaseMovesToTry)} type spin default {Configuration.EngineSettings.LMP_BaseMovesToTry} min 0 max 1024",
        //$"option name {nameof(Configuration.EngineSettings.LMP_MovesDepthMultiplier)} type spin default {Configuration.EngineSettings.LMP_MovesDepthMultiplier} min 0 max 1024",

        #endregion
    ];

    //"option name UCI_AnalyseMode type check",
    //"option name NalimovPath type string default C:/...",
    //"option name NalimovCache type spin default 1 min 1 max 32
    //"option name OwnBook type check",
    //"option name MultiPV type spin default 1",
    //"option name UCI_ShowCurrLine type check default false",      // Interesting
    //"option name UCI_ShowRefutations type check default false",
    //"option name UCI_LimitStrength type check default false",
    //"option name UCI_Elo type spin",
}
