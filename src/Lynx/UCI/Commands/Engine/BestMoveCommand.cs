using Lynx.Model;

namespace Lynx.UCI.Commands.Engine;

/// <summary>
/// bestmove <move1> [ ponder <move2> ]
///	the engine has stopped searching and found the best move in this position.
///	the engine can send the move it likes to ponder on. The engine must not start pondering automatically.
///	this command must always be sent if the engine stops searching, also in pondering mode if there is a
///	"stop" command, so for every "go" command a "bestmove" command is needed!
///	Directly before that the engine should send a final "info" command with the final search information,
///	the GUI has the complete statistics about the last search.
/// </summary>
public sealed class BestMoveCommand : EngineBaseCommand
{
    public const string Id = "bestmove";

    private readonly Move _move;
    private readonly Move? _moveToPonder;

    public BestMoveCommand(Move move, Move? moveToPonder)
    {
        _move = move;
        _moveToPonder = moveToPonder;
    }

    public override string ToString()
    {
        return $"bestmove {_move.UCIString()}" +
            (_moveToPonder.HasValue
                ? $" ponder {_moveToPonder!.Value.UCIString()}"
                : string.Empty);
    }
}
