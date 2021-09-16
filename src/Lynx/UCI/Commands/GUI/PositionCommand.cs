using Lynx.Model;
using NLog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lynx.UCI.Commands.GUI
{
    /// <summary>
    /// position[fen | startpos] moves  ....
    /// set up the position described in fenstring on the internal board and
    ///	play the moves on the internal chess board.
    ///	if the game was played from the start position the string "startpos" will be sent
    ///	Note: no "new" command is needed. However, if this position is from a different game than
    ///	the last position sent to the engine, the GUI should have sent a "ucinewgame" inbetween.
    /// </summary>
    public class PositionCommand : GUIBaseCommand
    {
        public const string Id = "position";

        public const string StartPositionString = "startpos";
        public const string MovesString = "moves";

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex FenRegex = new(
            "(?<=fen).+?(?=moves|$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex MovesRegex = new(
            "(?<=moves).+?(?=$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Game ParseGame(string positionCommand)
        {
            var items = positionCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            bool isInitialPosition = string.Equals(items.ElementAtOrDefault(1), StartPositionString, StringComparison.OrdinalIgnoreCase);

            var initialPosition = isInitialPosition
                    ? Constants.InitialPositionFEN
                    : FenRegex.Match(positionCommand).Value.Trim();

            if (string.IsNullOrEmpty(initialPosition))
            {
                Logger.Error($"Error parsing position command {positionCommand}: no initial position found");
            }

            var moves = MovesRegex.Match(positionCommand).Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            return new Game(initialPosition, moves);
        }

        public static bool TryParseLastMove(string positionCommand, Game game, [NotNullWhen(true)] out Move? lastMove)
        {
            var moveString = positionCommand
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Last();

            if (!Move.TryParseFromUCIString(
                moveString,
                game.CurrentPosition.AllPossibleMoves(),
                out lastMove))
            {
                Logger.Warn($"Error parsing last move {lastMove} from position command {positionCommand}");
                return false;
            }

            return true;
        }
    }
}
