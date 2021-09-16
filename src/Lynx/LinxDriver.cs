using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Lynx.Search.SearchAlgorithms;

namespace Lynx
{
    public class LinxDriver
    {
        private readonly ChannelReader<string> _uciReader;
        private readonly Channel<string> _engineWriter;
        private readonly Engine _engine;
        private readonly ILogger _logger;

        public LinxDriver(ChannelReader<string> uciReader, Channel<string> engineWriter, Engine engine)
        {
            _uciReader = uciReader;
            _engineWriter = engineWriter;
            _engine = engine;
            _logger = LogManager.GetCurrentClassLogger();
            _engine.OnReady += NotifyReadyOK;
            _engine.OnSearchFinished += NotifyBestMove;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            try
            {
                while (await _uciReader.WaitToReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
                {
                    if (_uciReader.TryRead(out var rawCommand) && !string.IsNullOrWhiteSpace(rawCommand))
                    {
                        _logger.Debug($"[GUI]\t{rawCommand}");

                        var commandItems = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        switch (commandItems[0].ToLowerInvariant())
                        {
                            case DebugCommand.Id:
                                HandleDebug(rawCommand);
                                break;
                            case GoCommand.Id:
                                await HandleGo(rawCommand);
                                break;
                            case IsReadyCommand.Id:
                                await HandleIsReady(cancellationToken);
                                break;
                            case PonderHitCommand.Id:
                                HandlePonderHit();
                                break;
                            case PositionCommand.Id:
                                HandlePosition(rawCommand);
                                break;
                            case QuitCommand.Id:
                                HandleQuit();
                                break;
                            case RegisterCommand.Id:
                                HandleRegister(rawCommand);
                                break;
                            case SetOptionCommand.Id:
                                HandleSetOption(rawCommand, commandItems);
                                break;
                            case StopCommand.Id:
                                HandleStop();
                                break;
                            case UCICommand.Id:
                                await HandleUCI(cancellationToken);
                                break;
                            case UCINewGameCommand.Id:
                                HandleNewGame();
                                break;
                            case "perft":
                                HandlePerft(rawCommand);
                                break;
                            case "divide":
                                HandleDivide(rawCommand);
                                break;

                            default:
                                _logger.Warn($"Unknown command received: {rawCommand}");
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }
            finally
            {
                _logger.Info($"Finishing {nameof(LinxDriver)}");
            }
        }

        #region Command handlers

        private void HandlePosition(string command)
        {
#if DEBUG
            _engine.Game.CurrentPosition.Print();
#endif

            _engine.AdjustPosition(command);
#if DEBUG
            _engine.Game.CurrentPosition.Print();

            //foreach (var move in MoveGenerator.GenerateAllMoves(_engine.Game.CurrentPosition))
            //{
            //    var newBlackPosition = new Position(_engine.Game.CurrentPosition, move);
            //    if (newBlackPosition.IsValid())
            //    {
            //        var newWhitePosition = new Position(newBlackPosition, newBlackPosition.AllPossibleMoves()[0]);
            //        if (newBlackPosition.IsValid())
            //        {
            //            Console.WriteLine($"{move,-6} | {newWhitePosition.MaterialEvaluation(),-5} | {newWhitePosition.MaterialAndPositionalEvaluation(),-5}");
            //        }
            //    }
            //}
#endif
        }

        private async Task HandleGo(string command)
        {
            var goCommand = new GoCommand();
            await goCommand.Parse(command);
            _engine.StartSearching(goCommand);
        }

        private void HandleStop() => _engine.StopSearching();

        private async Task HandleUCI(CancellationToken cancellationToken)
        {
            await SendCommand(IdCommand.Name, cancellationToken);
            await SendCommand(IdCommand.Version, cancellationToken);

            foreach (var availableOption in OptionCommand.AvailableOptions)
            {
                await SendCommand(availableOption, cancellationToken);
            }

            await SendCommand(UciOKCommand.Id, cancellationToken);
        }

        private async Task HandleIsReady(CancellationToken cancellationToken)
        {
            if (_engine.IsReady)
            {
                await SendCommand(ReadyOKCommand.Id, cancellationToken);
            }
        }

        private void HandlePonderHit()
        {
            if (Configuration.IsPonder)
            {
                _engine.PonderHit();
            }
        }

        private void HandleSetOption(string command, string[] commandItems)
        {
            if (commandItems.Length < 3 || !string.Equals(commandItems[1], "name", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            switch (commandItems[2].ToLowerInvariant())
            {
                case "ponder":
                    {
                        if (bool.TryParse(commandItems[4], out var value))
                        {
                            Configuration.IsPonder = value;
                        }
                        _logger.Warn("Ponder not supported yet");
                        break;
                    }
                case "uci_analysemode":
                    {
                        if (bool.TryParse(commandItems[4], out var value))
                        {
                            Configuration.UCI_AnalyseMode = value;
                        }
                        break;
                    }
                case "depth":
                    {
                        if (int.TryParse(commandItems[4], out var value))
                        {
                            Configuration.Parameters.Depth = value;
                        }
                        break;
                    }
                case "hash":
                    {
                        if (int.TryParse(commandItems[4], out var value))
                        {
                            Configuration.Parameters.Depth = value;
                        }
                        _logger.Warn("Hash size modification not supported yet");
                        break;
                    }
                default:
                    _logger.Warn($"Unsupported option: {command}");
                    break;
            }
        }

        private void HandleNewGame() => _engine.NewGame();

        private static void HandleDebug(string command) => Configuration.IsDebug = DebugCommand.Parse(command);

        private void HandleQuit()
        {
            _logger.Info($"Average depth: {_engine.AverageDepth}");
            _engineWriter.Writer.Complete();
        }

        private void HandleRegister(string rawCommand) => _engine.Registration = new RegisterCommand(rawCommand);

        private void HandlePerft(string rawCommand)
        {
            var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (items.Length >= 2 && int.TryParse(items[1], out int depth) && depth >= 1)
            {
                Perft.Results(_engine.Game.CurrentPosition, depth);
            }
        }

        private void HandleDivide(string rawCommand)
        {
            var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (items.Length >= 2 && int.TryParse(items[1], out int depth) && depth >= 1)
            {
                Perft.Results(_engine.Game.CurrentPosition, depth);
            }
        }

        #endregion

        private async Task NotifyReadyOK()
        {
            await _engineWriter.Writer.WriteAsync(ReadyOKCommand.Id);
        }

        private async Task NotifyBestMove(SearchResult searchResult, Move? moveToPonder)
        {
            await _engineWriter.Writer.WriteAsync(InfoCommand.SearchResultInfo(searchResult));
            await _engineWriter.Writer.WriteAsync(BestMoveCommand.BestMove(searchResult.BestMove, moveToPonder));
        }

        private async Task SendCommand(string command, CancellationToken cancellationToken)
        {
            await _engineWriter.Writer.WriteAsync(command, cancellationToken);
        }
    }
}
