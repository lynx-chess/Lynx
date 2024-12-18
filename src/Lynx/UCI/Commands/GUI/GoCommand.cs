using NLog;

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
public sealed class GoCommand : IGUIBaseCommand
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public const string Id = "go";

    private static ReadOnlySpan<char> WtimeSpan => "wtime".AsSpan();
    private static ReadOnlySpan<char> BtimeSpan => "btime".AsSpan();
    private static ReadOnlySpan<char> WincSpan => "winc".AsSpan();
    private static ReadOnlySpan<char> BincSpan => "binc".AsSpan();
    private static ReadOnlySpan<char> MovestogoSpan => "movestogo".AsSpan();
    private static ReadOnlySpan<char> MovetimeSpan => "movetime".AsSpan();
    private static ReadOnlySpan<char> DepthSpan => "depth".AsSpan();
    private static ReadOnlySpan<char> InfiniteSpan => "infinite".AsSpan();
    private static ReadOnlySpan<char> PonderSpan => "ponder".AsSpan();
    private static ReadOnlySpan<char> NodesSpan => "nodes".AsSpan();
    private static ReadOnlySpan<char> MateSpan => "mate".AsSpan();
    private static ReadOnlySpan<char> SearchmovesSpan => "searchmoves".AsSpan();

    public int WhiteTime { get; }
    public int BlackTime { get; }
    public int WhiteIncrement { get; }
    public int BlackIncrement { get; }
    public int MovesToGo { get; }
    public int Depth { get; }
    public int MoveTime { get; }
    public bool Infinite { get; }
    public bool Ponder { get; }

    public static int Nodes => throw new NotSupportedException();
    public static int Mate => throw new NotSupportedException();
    public static List<string> SearchMoves => throw new NotSupportedException();

    public GoCommand(string command)
    {
        var commandAsSpan = command.AsSpan();

        Span<Range> ranges = stackalloc Range[commandAsSpan.Length];
        var rangesLength = commandAsSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable S127 // "for" loop stop conditions should be invariant
        for (int i = 1; i < rangesLength; i++)
        {
            var key = commandAsSpan[ranges[i]];

            if (key.Equals(WtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteTime = value;
                }

                // Following cutechess (and kinda logical) order of go wtime btime winc binc
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteIncrement = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackIncrement = value;
                    }
                }
            }
            else if (key.Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackTime = value;
                }
            }
            else if (key.Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    WhiteIncrement = value;
                }
            }
            else if (key.Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    BlackIncrement = value;
                }
            }
            else if (key.Equals(MovestogoSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MovesToGo = value;
                }
            }
            else if (key.Equals(MovetimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    MoveTime = value;
                }
            }
            else if (key.Equals(DepthSpan, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandAsSpan[ranges[++i]], out var value))
                {
                    Depth = value;
                }
            }
            else if (key.Equals(InfiniteSpan, StringComparison.OrdinalIgnoreCase))
            {
                Infinite = true;
            }
            else if (key.Equals(PonderSpan, StringComparison.OrdinalIgnoreCase))
            {
                Ponder = true;

                // Following cutechess order of go ponder wtime btime winc binc
                int value;

                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(WincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        WhiteIncrement = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BtimeSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackTime = value;
                    }
                }
                if (i + 1 < rangesLength && commandAsSpan[ranges[i + 1]].Equals(BincSpan, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (int.TryParse(commandAsSpan[ranges[++i]], out value))
                    {
                        BlackIncrement = value;
                    }
                }
            }
            else if (key.Equals(NodesSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("nodes not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(MateSpan, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warn("mate not supported in go command, it will be safely ignored");
                ++i;
            }
            else if (key.Equals(SearchmovesSpan, StringComparison.OrdinalIgnoreCase))
            {
                const string message = "searchmoves not supported in go command";
                _logger.Error(message);
                throw new NotSupportedException(message);
            }
            else
            {
                _logger.Warn("{0} not supported in go command, attempting to continue command parsing", key.ToString());
            }
        }
#pragma warning restore S127 // "for" loop stop conditions should be invariant
    }

    public static string Init() => Id;
}
