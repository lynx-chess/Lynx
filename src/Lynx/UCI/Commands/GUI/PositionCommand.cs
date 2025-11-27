using Lynx.Model;
using NLog;
using System.Diagnostics.CodeAnalysis;

namespace Lynx.UCI.Commands.GUI;

/// <summary>
/// position[fen | startpos] moves  ....
/// set up the position described in fenstring on the internal board and
///	play the moves on the internal chess board.
///	if the game was played from the start position the string "startpos" will be sent
///	Note: no "new" command is needed. However, if this position is from a different game than
///	the last position sent to the engine, the GUI should have sent a "ucinewgame" inbetween.
/// </summary>
public sealed class PositionCommand
{
    public const string Id = "position";

    public const string StartPositionString = "startpos";
    public const string MovesString = "moves";

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static bool TryParseLastMove(string positionCommand, Game game, [NotNullWhen(true)] out Move? lastMove)
    {
        var moveString = positionCommand
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)[^1];

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        if (!MoveExtensions.TryParseFromUCIString(
            moveString,
            MoveGenerator.GenerateAllMoves(game.CurrentPosition, ref evaluationContext, movePool),
            out lastMove))
        {
            _logger.Warn("Error parsing last move {0} from position command {1}", lastMove, positionCommand);
            return false;
        }

        return true;
    }
}
