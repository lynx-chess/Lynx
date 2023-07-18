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

    private const string GoSubcommands = "searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite";

    [GeneratedRegex(@$"(?<=searchmoves).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SearchMoveRegex();

    [GeneratedRegex(@$"(?<=wtime).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WhiteTimeRegex();

    [GeneratedRegex(@$"(?<=btime).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex BlackTimeRegex();

    [GeneratedRegex(@$"(?<=winc).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WhiteIncrementRegex();

    [GeneratedRegex(@$"(?<=binc).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex BlackIncrementRegex();

    [GeneratedRegex(@$"(?<=movestogo).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MovesToGoRegex();

    [GeneratedRegex(@$"(?<=depth).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DepthRegex();

    [GeneratedRegex(@$"(?<=nodes).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex NodesRegex();

    [GeneratedRegex(@$"(?<=mate).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MateRegex();

    [GeneratedRegex(@$"(?<=movetime).+?(?={GoSubcommands}|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MoveTimeRegex();

    private static readonly Regex _searchMovesRegex = SearchMoveRegex();
    private static readonly Regex _whiteTimeRegex = WhiteTimeRegex();
    private static readonly Regex _blackTimeRegex = BlackTimeRegex();
    private static readonly Regex _whiteIncrementRegex = WhiteIncrementRegex();
    private static readonly Regex _blackIncrementRegex = BlackIncrementRegex();
    private static readonly Regex _movesToGoRegex = MovesToGoRegex();
    private static readonly Regex _depthRegex = DepthRegex();
    private static readonly Regex _nodesRegex = NodesRegex();
    private static readonly Regex _mateRegex = MateRegex();
    private static readonly Regex _moveTimeRegex = MoveTimeRegex();

    public List<string> SearchMoves { get; private set; } = default!;
    public int WhiteTime { get; private set; } = default!;
    public int BlackTime { get; private set; } = default!;
    public int WhiteIncrement { get; private set; } = default!;
    public int BlackIncrement { get; private set; } = default!;
    public int MovesToGo { get; private set; } = default!;
    public int Depth { get; private set; } = default!;
    public int Nodes { get; private set; } = default!;  // Not implemented
    public int Mate { get; private set; } = default!;   // Not implemented
    public int MoveTime { get; private set; } = default!;
    public bool Infinite { get; private set; } = default!;
    public bool Ponder { get; private set; } = default!;

    /// <summary>
    /// Requires invoking <see cref="Parse(string)", allowing the user to make it asynchronously/>
    /// </summary>
    public GoCommand() { }

    /// <summary>
    /// Invokes <see cref="Parse(string)" synchronously/>
    /// </summary>
    /// <param name="command"></param>
    internal GoCommand(string command)
    {
        Parse(command).Wait();
    }

    public async Task Parse(string command)
    {
        var taskList = new List<Task>
            {
                Task.Run(() =>
                {
                    var match = _searchMovesRegex.Match(command);

                    SearchMoves = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                }),
                Task.Run(() =>
                {
                    var match = _whiteTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _blackTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _whiteIncrementRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _blackIncrementRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _movesToGoRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MovesToGo = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _depthRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Depth = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _nodesRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Nodes = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _mateRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Mate = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = _moveTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MoveTime = value;
                    }
                }),
                Task.Run(() => Infinite = command.Contains("infinite", StringComparison.OrdinalIgnoreCase)),
                Task.Run(() => Ponder = command.Contains("ponder", StringComparison.OrdinalIgnoreCase))
            };

        await Task.WhenAll(taskList);
    }
}
