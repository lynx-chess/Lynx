using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Threading.Channels;

namespace Lynx;

public sealed class LynxDriver
{
    private readonly ChannelReader<string> _uciReader;
    private readonly Channel<string> _engineWriter;
    private readonly Engine _engine;
    private readonly Logger _logger;
    private static readonly string[] _none = new[] { "none" };

    public LynxDriver(ChannelReader<string> uciReader, Channel<string> engineWriter, Engine engine)
    {
        _uciReader = uciReader;
        _engineWriter = engineWriter;
        _engine = engine;
        _logger = LogManager.GetCurrentClassLogger();

        InitializeStaticClasses();
    }

    private static void InitializeStaticClasses()
    {
        _ = PVTable.Indexes[0];
        _ = Attacks.KingAttacks;
        _ = ZobristTable.SideHash();
        _ = Masks.FileMasks;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            while (await _uciReader.WaitToReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_uciReader.TryRead(out var rawCommand) && !string.IsNullOrWhiteSpace(rawCommand))
                    {
                        _logger.Debug("[GUI]\t{0}", rawCommand);

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
                                return;
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
                                await HandlePerft(rawCommand);
                                break;
                            case "divide":
                                await HandleDivide(rawCommand);
                                break;
                            case "bench":
                                await HandleBench();
                                break;
                            default:
                                _logger.Warn("Unknown command received: {0}", rawCommand);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error trying to read/parse UCI command");
                }
            }
        }
        catch (Exception e)
        {
            _logger.Fatal(e);
        }
        finally
        {
            _logger.Info("Finishing {0}", nameof(LynxDriver));
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
        await SendCommand(ReadyOKCommand.Id, cancellationToken);
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
                    if (commandItems.Length > 4 && bool.TryParse(commandItems[4], out var value))
                    {
                        Configuration.IsPonder = value;
                    }
                    _logger.Warn("Ponder not supported yet");
                    break;
                }
            case "uci_analysemode":
                {
                    if (commandItems.Length > 4 && bool.TryParse(commandItems[4], out var value))
                    {
                        Configuration.UCI_AnalyseMode = value;
                    }
                    break;
                }
            case "depth":
                {
                    if (commandItems.Length > 4 && int.TryParse(commandItems[4], out var value))
                    {
                        Configuration.EngineSettings.DefaultMaxDepth = value;
                    }
                    break;
                }
            case "hash":
                {
                    if (commandItems.Length > 4 && int.TryParse(commandItems[4], out var value))
                    {
                        Configuration.Hash = value * 1024 * 1024;
                    }
                    break;
                }
            case "uci_opponent":
                {
                    if (commandItems.Length > 4)
                    {
                        _logger.Info("Game against {0}", string.Join(' ', commandItems.Skip(4).Except(_none)));
                    }
                    break;
                }
            case "uci_engineabout":
                {
                    if (commandItems.Length > 4)
                    {
                        _logger.Info("UCI_EngineAbout: {0}", string.Join(' ', commandItems.Skip(4)));
                    }
                    break;
                }
            case "onlinetablebaseinrootpositions":
                {
                    if (commandItems.Length > 4 && bool.TryParse(commandItems[4], out var value))
                    {
                        Configuration.EngineSettings.UseOnlineTablebaseInRootPositions = value;
                    }
                    break;
                }
            case "onlinetablebaseinsearch":
                {
                    if (commandItems.Length > 4 && bool.TryParse(commandItems[4], out var value))
                    {
                        Configuration.EngineSettings.UseOnlineTablebaseInSearch = value;
                    }
                    break;
                }
            case "threads":
                {
                    if (commandItems.Length > 4 && int.TryParse(commandItems[4], out var value))
                    {
                        if (value != 1)
                        {
                            _logger.Warn("Unsopported threads value: {0}", value);
                        }
                    }
                    break;
                }
            default:
                _logger.Warn("Unsupported option: {0}", command);
                break;
        }
    }

    private void HandleNewGame()
    {
        if (_engine.AverageDepth > 0 && _engine.AverageDepth < int.MaxValue)
        {
            _logger.Info("Average depth: {0}", _engine.AverageDepth);
        }
        _engine.NewGame();
    }

    private static void HandleDebug(string command) => Configuration.IsDebug = DebugCommand.Parse(command);

    private void HandleQuit()
    {
        if (_engine.AverageDepth > 0 && _engine.AverageDepth < int.MaxValue)
        {
            _logger.Info("Average depth: {0}", _engine.AverageDepth);
        }
        _engineWriter.Writer.Complete();
    }

    private void HandleRegister(string rawCommand) => _engine.Registration = new RegisterCommand(rawCommand);

    private async Task HandlePerft(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length >= 2 && int.TryParse(items[1], out int depth) && depth >= 1)
        {
            var results = Perft.Results(_engine.Game.CurrentPosition, depth);
            await Perft.PrintPerftResult(depth, results, str => _engineWriter.Writer.WriteAsync(str));
        }
    }

    private async ValueTask HandleDivide(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length >= 2 && int.TryParse(items[1], out int depth) && depth >= 1)
        {
            var results = Perft.Divide(_engine.Game.CurrentPosition, depth);
            await Perft.PrintPerftResult(depth, results, str => _engineWriter.Writer.WriteAsync(str));
        }
    }

    private async ValueTask HandleBench()
    {
        var results = await OpenBench.Bench(_engineWriter);
        await OpenBench.PrintBenchResults(results, str => _engineWriter.Writer.WriteAsync(str));
    }

    #endregion

    private async Task SendCommand(string command, CancellationToken cancellationToken)
    {
        await _engineWriter.Writer.WriteAsync(command, cancellationToken);
    }
}
