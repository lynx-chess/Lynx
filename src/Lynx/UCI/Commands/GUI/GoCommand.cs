using NLog.LayoutRenderers.Wrappers;
using System.Text.RegularExpressions;

namespace Lynx.UCI.Commands.GUI;

/// <summary>
/// go
///	start calculating on the current position set up with the "position" command.
///	There are a number of commands that can follow this command, all will be sent in the same string.
///	If one command is not send its value should be interpreted as it would not influence the search.
///	* searchmoves....
///		restrict search to this moves only
///		Example: After "position startpos" and "go infinite searchmoves e2e4 d2d4"
///		the engine should only search the two moves e2e4 and d2d4 in the initial position.
///	* ponder
///		start searching in pondering mode.
///		Do not exit the search in ponder mode, even if it's mate!
///		This means that the last move sent in the position string is the ponder move.
///		The engine can do what it wants to do, but after a "ponderhit" command
///		it should execute the suggested move to ponder on. This means that the ponder move sent by
///		the GUI can be interpreted as a recommendation about which move to ponder. However, if the
///		engine decides to ponder on a different move, it should not display any mainlines as they are
///		likely to be misinterpreted by the GUI because the GUI expects the engine to ponder
///	    on the suggested move.
///	* wtime <x>
///		white has x msec left on the clock
///	* btime <x>
///		black has x msec left on the clock
///	* winc <x>
///		white increment per move in mseconds if x > 0
///	* binc <x>
///		black increment per move in mseconds if x > 0
///	* movestogo <x>
///	    there are x moves to the next time control,
///		this will only be sent if x > 0,
///		if you don't get this and get the wtime and btime it's sudden death
///	* depth <x>
///		search x plies only
///	* nodes <x>
///	   search x nodes only
///	* mate <x>
///		search for a mate in x moves
///	* movetime <x>
///		search exactly x mseconds
///	* infinite
///		search until the "stop" command. Do not exit the search without being told so in this mode!
/// </summary>
public sealed partial class GoCommand : GUIBaseCommand
{
    public const string Id = "go";

    /// <summary>
    /// Note that wtime and btime need to support negative numbers because of cutechess
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(?<wtime>(?<=wtime\s+)-?\d+)|(?<btime>(?<=btime\s+)-?\d+)|(?<winc>(?<=winc\s+)\d+)|(?<binc>(?<=binc\s+)\d+)|(?<movestogo>(?<=movestogo\s+)\d+)|(?<depth>(?<=depth\s+)\d+)|(?<movetime>(?<=movetime\s+)\d+)|(?<infinite>infinite)|(?<ponder>ponder)")]
    private static partial Regex Regex();

    public int WhiteTime { get; }
    public int BlackTime { get; }
    public int WhiteIncrement { get; }
    public int BlackIncrement { get; }
    public int MovesToGo { get; }
    public int Depth { get; }
    public int MoveTime { get; }
    public bool Infinite { get; }
    public bool Ponder { get; private set; }

    public int Nodes => throw new NotImplementedException();
    public int Mate => throw new NotImplementedException();
    public List<string> SearchMoves => throw new NotImplementedException();

    /// <summary>
    /// Invokes <see cref="Parse(string)" synchronously/>
    /// </summary>
    /// <param name="command"></param>
    public GoCommand(string command)
    {
        foreach (var match in Regex().Matches(command).Cast<Match>())
        {
            for (int i = 1; i < match.Groups.Count; ++i)
            {
                var group = match.Groups[i];
                if (group.Success)
                {
                    switch (group.Name)
                    {
                        case "wtime":
                            WhiteTime = int.Parse(group.Value);
                            break;
                        case "btime":
                            BlackTime = int.Parse(group.Value);
                            break;
                        case "winc":
                            WhiteIncrement = int.Parse(group.Value);
                            break;
                        case "binc":
                            BlackIncrement = int.Parse(group.Value);
                            break;
                        case "movestogo":
                            MovesToGo = int.Parse(group.Value);
                            break;
                        case "infinite":
                            Infinite = true;
                            break;
                        case "ponder":
                            if (Configuration.EngineSettings.IsPonder)
                            {
                                Ponder = true;
                            }
                            break;
                        case "depth":
                            Depth = int.Parse(group.Value);
                            break;
                        case "movetime":
                            MoveTime = int.Parse(group.Value);
                            break;
                    }

                    break;
                }
            }
        }
    }

    public static string Init() => Id;

    public void DisablePonder() => Ponder = false;
}
