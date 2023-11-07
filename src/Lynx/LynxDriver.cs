using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using System.Threading.Channels;

namespace Lynx;

public sealed class LynxDriver
{
    private readonly ChannelReader<string> _uciReader;
    private readonly Channel<string> _engineWriter;
    private readonly Engine _engine;
    private readonly Logger _logger;

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
        static ReadOnlySpan<char> ExtractCommandItems(string rawCommand)
        {
            var span = rawCommand.AsSpan();
            Span<Range> items = stackalloc Range[2];
            span.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries);

            return span[items[0]];
        }
        try
        {
            while (await _uciReader.WaitToReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_uciReader.TryRead(out var rawCommand) && !string.IsNullOrWhiteSpace(rawCommand))
                    {
                        _logger.Debug("[GUI]\t{0}", rawCommand);

                        switch (ExtractCommandItems(rawCommand))
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
                                HandleSetOption(rawCommand);
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
                                HandleQuit();
                                break;
                            case "printsettings":
                                await HandleSettings();
                                break;
                            case "printsysteminfo":
                                await HandleSystemInfo();
                                break;
                            case "staticeval":
                                await HandleStaticEval(rawCommand, cancellationToken);
                                HandleQuit();
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

    private void HandlePosition(ReadOnlySpan<char> command)
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
        if (Configuration.EngineSettings.IsPonder)
        {
            _engine.PonderHit();
        }
        else
        {
            _logger.Warn("Unexpected ponderhit command, ponder is disabled");
        }
    }

    private void HandleSetOption(ReadOnlySpan<char> command)
    {
        Span<Range> commandItems = stackalloc Range[5];
        var length = command.Split(commandItems, ' ', StringSplitOptions.RemoveEmptyEntries);

        if (commandItems[2].Start.Equals(commandItems[2].End) || !command[commandItems[1]].Equals("name", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Span<char> lowerCaseFirstWord = stackalloc char[command[commandItems[2]].Length];
        command[commandItems[2]].ToLowerInvariant(lowerCaseFirstWord);

        switch (lowerCaseFirstWord)
        {
            case "ponder":
                {
                    if (length > 4 && bool.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.IsPonder = value;
                    }
                    break;
                }
            case "uci_analysemode":
                {
                    if (length > 4 && bool.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.UCI_AnalyseMode = value;
                    }
                    break;
                }
            case "depth":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.DefaultMaxDepth = value;
                    }
                    break;
                }
            case "hash":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.Hash = Math.Clamp(value, 0, 1024);
                    }
                    break;
                }
            case "uci_opponent":
                {
                    const string none = "none ";
                    if (length > 4)
                    {
                        var opponent = command[commandItems[4].Start.Value..].ToString();

                        _logger.Info("Game against {0}", opponent.Replace(none, string.Empty));
                    }
                    break;
                }
            case "uci_engineabout":
                {
                    if (length > 4)
                    {
                        _logger.Info("UCI_EngineAbout: {0}", command[commandItems[4].Start.Value..].ToString());
                    }
                    break;
                }
            case "onlinetablebaseinrootpositions":
                {
                    if (length > 4 && bool.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.UseOnlineTablebaseInRootPositions = value;
                    }
                    break;
                }
            case "onlinetablebaseinsearch":
                {
                    if (length > 4 && bool.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.UseOnlineTablebaseInSearch = value;
                    }
                    break;
                }
            case "threads":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        if (value != 1)
                        {
                            _logger.Warn("Unsopported threads value: {0}", value);
                        }
                    }
                    break;
                }
            case "uci_showwdl":
                {
                    if (length > 4 && bool.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.ShowWDL = value;
                    }
                }
                break;
            default:
                _logger.Warn("Unsupported option: {0}", command.ToString());
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

    private static void HandleDebug(ReadOnlySpan<char> command) => Configuration.IsDebug = DebugCommand.Parse(command);

    private void HandleQuit()
    {
        if (_engine.AverageDepth > 0 && _engine.AverageDepth < int.MaxValue)
        {
            _logger.Info("Average depth: {0}", _engine.AverageDepth);
        }
        _engineWriter.Writer.Complete();
    }

    private void HandleRegister(ReadOnlySpan<char> rawCommand) => _engine.Registration = new RegisterCommand(rawCommand);

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
            var results = await Perft.Divide(_engine.Game.CurrentPosition, depth, str => _engineWriter.Writer.WriteAsync(str));
            await Perft.PrintPerftResult(depth, results, str => _engineWriter.Writer.WriteAsync(str));
        }
    }

    private async ValueTask HandleBench()
    {
        var results = await OpenBench.Bench(_engineWriter);
        await OpenBench.PrintBenchResults(results, str => _engineWriter.Writer.WriteAsync(str));
    }

    private async ValueTask HandleSettings()
    {
        var engineSettings = JsonSerializer.Serialize(Configuration.EngineSettings, EngineSettingsJsonSerializerContext.Default.EngineSettings);

        var message = $"{nameof(Configuration)}.{nameof(Configuration.EngineSettings)}:{Environment.NewLine}{engineSettings}";

        await _engineWriter.Writer.WriteAsync(message);
    }

    private async ValueTask HandleSystemInfo()
    {
        try
        {
            var simd = Bmi2.X64.IsSupported
                ? "Bmi2.X64 supported, PEXT BitBoards will be used"
                : "Bmi2.X64 not supported";

            await _engineWriter.Writer.WriteAsync(simd);
        }
        catch (Exception e)
        {
            _logger.Error(e);
        }
    }

    private async ValueTask HandleStaticEval(string rawCommand, CancellationToken cancellationToken)
    {
        try
        {
            var fullPath = Path.GetFullPath(rawCommand[(rawCommand.IndexOf(' ') + 1)..]);
            if (!File.Exists(fullPath))
            {
                _logger.Warn("File {0} not found in (1), ignoring command", rawCommand, fullPath);
                return;
            }

            foreach (var line in await File.ReadAllLinesAsync(fullPath, cancellationToken))
            {
                var fen = line[..line.IndexOfAny([';', '[', '"'])];

                var position = new Position(fen);
                if (!position.IsValid())
                {
                    _logger.Warn("Position {0}, parsed as {1} and then {2} not valid, skipping it", line, fen, position.FEN());
                    continue;
                }

                var ourFen = position.FEN();
                if (ourFen != fen)
                {
                    _logger.Debug("Raw fen: {0}, parsed fen: {1}", fen, ourFen);
                }

                var eval = WDL.NormalizeScore(position.StaticEvaluation().Score);
                if (position.Side == Side.Black)
                {
                    eval = -eval;   // White perspective
                }

                await _engineWriter.Writer.WriteAsync($"{line}: {eval}", cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + e.StackTrace);
            await _engineWriter.Writer.WriteAsync(e.Message + e.StackTrace, cancellationToken);
        }
    }

    #endregion

    private async Task SendCommand(string command, CancellationToken cancellationToken)
    {
        await _engineWriter.Writer.WriteAsync(command, cancellationToken);
    }
}
