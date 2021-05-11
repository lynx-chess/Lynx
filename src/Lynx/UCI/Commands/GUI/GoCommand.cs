using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lynx.UCI.Commands.GUI
{
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
    public class GoCommand : GUIBaseCommand
    {
        public const string Id = "go";

        private const string GoSubcommands = "searchmoves|wtime|btime|winc|binc|movestogo|depth|nodes|mate|movetime|ponder|infinite";

        private static readonly Regex SearchMovesRegex = new(
            $"(?<=searchmoves).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex WhiteTimeRegex = new(
            $"(?<=wtime).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex BlackTimeRegex = new(
            $"(?<=btime).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex WhiteIncrementRegex = new(
            $"(?<=winc).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex BlackIncrementRegex = new(
            $"(?<=binc).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex MovesToGoRegex = new(
            $"(?<=movestogo).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex DepthRegex = new(
            $"(?<=depth).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex NodesRegex = new(
            $"(?<=nodes).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex MateRegex = new(
            $"(?<=mate).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex MoveTimeRegex = new(
            $"(?<=movetime).+?(?={GoSubcommands}|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public List<string> SearchMoves { get; private set; } = default!;
        public int WhiteTime { get; private set; } = default!;
        public int BlackTime { get; private set; } = default!;
        public int WhiteIncrement { get; private set; } = default!;
        public int BlackIncrement { get; private set; } = default!;
        public int MovesToGo { get; private set; } = default!;
        public int Depth { get; private set; } = default!;
        public int Nodes { get; private set; } = default!;
        public int Mate { get; private set; } = default!;
        public int MoveTime { get; private set; } = default!;
        public bool Infinite { get; private set; } = default!;
        public bool Ponder { get; private set; } = default!;

        public async Task Parse(string command)
        {
            var taskList = new List<Task>
            {
                Task.Run(() =>
                {
                    var match = SearchMovesRegex.Match(command);

                    SearchMoves = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                }),
                Task.Run(() =>
                {
                    var match = WhiteTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = BlackTimeRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackTime = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = WhiteIncrementRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        WhiteIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = BlackIncrementRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        BlackIncrement = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = MovesToGoRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        MovesToGo = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = DepthRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Depth = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = NodesRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Nodes = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = MateRegex.Match(command);

                    if(int.TryParse(match.Value, out var value))
                    {
                        Mate = value;
                    }
                }),
                Task.Run(() =>
                {
                    var match = MoveTimeRegex.Match(command);

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
}
