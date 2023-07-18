using Lynx.Model;
using NLog;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Lynx.UCI.Commands.GUI;

/// <summary>
/// position[fen | startpos] moves  ....
/// set up the position described in fenstring on the internal board and
///	play the moves on the internal chess board.
///	if the game was played from the start position the string "startpos" will be sent
///	Note: no "new" command is needed. However, if this position is from a different game than
///	the last position sent to the engine, the GUI should have sent a "ucinewgame" inbetween.
/// </summary>
public sealed partial class PositionCommand : GUIBaseCommand
{
    public const string Id = "position";

    public const string StartPositionString = "startpos";
    public const string MovesString = "moves";

    [GeneratedRegex("(?<=fen).+?(?=moves|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex FenRegex();

    [GeneratedRegex("(?<=moves).+?(?=$)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MovesRegex();

    private static readonly Regex _fenRegex = FenRegex();
    private static readonly Regex _movesRegex = MovesRegex();

    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public static Game ParseGame(string positionCommand)
    {
        try
        {
            var items = positionCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            bool isInitialPosition = string.Equals(items.ElementAtOrDefault(1), StartPositionString, StringComparison.OrdinalIgnoreCase);

            var initialPosition = isInitialPosition
                    ? Constants.InitialPositionFEN
                    : _fenRegex.Match(positionCommand).Value.Trim();

            if (string.IsNullOrEmpty(initialPosition))
            {
                _logger.Error("Error parsing position command '{0}': no initial position found", positionCommand);
            }

            var moves = _movesRegex.Match(positionCommand).Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            return new Game(initialPosition, moves);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error parsing position command '{0}'", positionCommand);
            return new Game();
        }
    }

    public static bool TryParseLastMove(string positionCommand, Game game, [NotNullWhen(true)] out Move? lastMove)
    {
        var moveString = positionCommand
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)[^1];

        if (!MoveExtensions.TryParseFromUCIString(
            moveString,
            game.CurrentPosition.AllPossibleMoves(game.MovePool),
            out lastMove))
        {
            _logger.Warn("Error parsing last move {0} from position command {1}", lastMove, positionCommand);
            return false;
        }

        return true;
    }
}
