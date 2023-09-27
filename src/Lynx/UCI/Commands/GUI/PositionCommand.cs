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
public sealed class PositionCommand : GUIBaseCommand
{
    public const string Id = "position";

    public const string StartPositionString = "startpos";
    public const string MovesString = "moves";

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static Game ParseGame(ReadOnlySpan<char> positionCommandSpan)
    {
        try
        {
            // We divide the position command in these two sections:
            // "position startpos                       ||"
            // "position startpos                       || moves e2e4 e7e5"
            // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  ||"
            // "position fen 8/8/8/8/8/8/8/8 w - - 0 1  || moves e2e4 e7e5"
            Span<Range> items = stackalloc Range[2];
            positionCommandSpan.Split(items, "moves", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var initialPositionSection = positionCommandSpan[items[0]];

            // We divide in these two parts
            // "position startpos ||"       <-- If "fen" doesn't exist in the section
            // "position || (fen) 8/8/8/8/8/8/8/8 w - - 0 1"  <-- If "fen" does exist
            Span<Range> initialPositionParts = stackalloc Range[2];
            initialPositionSection.Split(initialPositionParts, "fen", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            ReadOnlySpan<char> fen = initialPositionSection[initialPositionParts[0]].Length == Id.Length   // "position" o "position startpos"
                ? initialPositionSection[initialPositionParts[1]]
                : Constants.InitialPositionFEN.AsSpan();

            var movesSection = positionCommandSpan[items[1]];

            Span<Range> moves = stackalloc Range[(movesSection.Length / 5) + 1]; // Number of potential half-moves provided in the string
            movesSection.Split(moves, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return new Game(fen, movesSection, moves);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error parsing position command '{0}'", positionCommandSpan.ToString());
            return new Game();
        }
    }

    public static bool TryParseLastMove(string positionCommand, Game game, [NotNullWhen(true)] out Move? lastMove)
    {
        var moveString = positionCommand
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)[^1];

        if (!MoveExtensions.TryParseFromUCIString(
            moveString,
            MoveGenerator.GenerateAllMoves(game.CurrentPosition, game.MovePool),
            out lastMove))
        {
            _logger.Warn("Error parsing last move {0} from position command {1}", lastMove, positionCommand);
            return false;
        }

        return true;
    }
}
