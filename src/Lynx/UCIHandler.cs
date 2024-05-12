using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace Lynx;

public sealed class UCIHandler
{
    private readonly Channel<string> _uciToEngine;
    private readonly Channel<string> _engineToUci;

    private readonly Engine _engine;
    private readonly Logger _logger;

    public UCIHandler(Channel<string> uciToEngine, Channel<string> engineToUci, Engine engine)
    {
        _uciToEngine = uciToEngine;
        _engineToUci = engineToUci;

        _engine = engine;
        _logger = LogManager.GetCurrentClassLogger();
    }

    public async Task Handle(string rawCommand, CancellationToken cancellationToken)
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
            _logger.Debug("[GUI]\t{0}", rawCommand);

            switch (ExtractCommandItems(rawCommand))
            {
                case GoCommand.Id:
                    await _uciToEngine.Writer.WriteAsync(rawCommand, cancellationToken);
                    break;

                case DebugCommand.Id:
                    HandleDebug(rawCommand);
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
                    await HandleBench(rawCommand);
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
                case "eval":
                    await HandleEval(cancellationToken);
                    break;
                case "fen":
                    await HandleFEN(cancellationToken);
                    break;
                case "ob_spsa":
                    await HandleOpenBenchSPSA(cancellationToken);
                    break;
                case "ob_spsa_pretty":
                    await HandleOpenBenchSPSAPretty(cancellationToken);
                    break;
                case "wf_spsa":
                    await HandleWeatherFactorySPSA(cancellationToken);
                    break;
                default:
                    _logger.Warn("Unknown command received: {0}", rawCommand);
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error trying to read/parse UCI command");
        }
    }

    #region Command handlers

    private void HandlePosition(ReadOnlySpan<char> command)
    {
        var sw = Stopwatch.StartNew();
#if DEBUG
        _engine.Game.CurrentPosition.Print();
#endif

        _engine.AdjustPosition(command);
#if DEBUG
        _engine.Game.CurrentPosition.Print();
#endif

        _logger.Info("Position parsing took {0}ms", sw.ElapsedMilliseconds);
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

    private async Task HandleIsReady(CancellationToken cancellationToken) => await SendCommand(ReadyOKCommand.Id, cancellationToken);

    private void HandlePonderHit()
    {
        if (Configuration.IsPonder)
        {
            _engine.PonderHit();
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
                        Configuration.IsPonder = value;
                    }
                    _logger.Warn("Ponder not supported yet");
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
            case "hash":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.Hash = Math.Clamp(value, Constants.AbsoluteMinTTSize, Constants.AbsoluteMaxTTSize);
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
#pragma warning disable S1066 // Collapsible "if" statements should be merged
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        if (value != 1)
                        {
                            _logger.Warn("Unsopported threads value: {0}", value);
                        }
                    }
                    break;
#pragma warning restore S1066 // Collapsible "if" statements should be merged
                }
            case "uci_showwdl":
                {
                    if (length > 4 && bool.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.ShowWDL = value;
                    }
                    break;
                }

            #region Search tuning

            case "lmr_mindepth":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.LMR_MinDepth = value;
                    }
                    break;
                }
            case "lmr_minfulldepthsearchedmoves":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves = value;
                    }
                    break;
                }
            case "lmr_base":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.LMR_Base = (value * 0.01);
                    }
                    break;
                }
            case "lmr_divisor":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.LMR_Divisor = (value * 0.01);
                    }
                    break;
                }

            case "nmp_mindepth":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.NMP_MinDepth = value;
                    }
                    break;
                }
            case "nmp_basedepthreduction":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.NMP_BaseDepthReduction = value;
                    }
                    break;
                }
            case "nmp_depthincrement":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.NMP_DepthIncrement = value;
                    }
                    break;
                }
            case "nmp_depthdivisor":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.NMP_DepthDivisor = value;
                    }
                    break;
                }

            case "aspirationwindow_delta":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.AspirationWindow_Delta = value;
                    }
                    break;
                }
            case "aspirationwindow_mindepth":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.AspirationWindow_MinDepth = value;
                    }
                    break;
                }

            case "rfp_maxdepth":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.RFP_MaxDepth = value;
                    }
                    break;
                }
            case "rfp_depthscalingfactor":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.RFP_DepthScalingFactor = value;
                    }
                    break;
                }

            case "razoring_maxdepth":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.Razoring_MaxDepth = value;
                    }
                    break;
                }
            case "razoring_depth1bonus":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.Razoring_Depth1Bonus = value;
                    }
                    break;
                }
            case "razoring_notdepth1bonus":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.Razoring_NotDepth1Bonus = value;
                    }
                    break;
                }

            case "iir_mindepth":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.IIR_MinDepth = value;
                    }
                    break;
                }

            case "lmp_maxdepth":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.LMP_MaxDepth = value;
                    }
                    break;
                }
            case "lmp_basemovestotry":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.LMP_BaseMovesToTry = value;
                    }
                    break;
                }
            case "lmp_movesdepthmultiplier":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.LMP_MovesDepthMultiplier = value;
                    }
                    break;
                }
            case "see_badcapturereduction":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.SEE_BadCaptureReduction = value;
                    }
                    break;
                }

            #endregion

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
        _engineToUci.Writer.Complete();
    }

    private void HandleRegister(ReadOnlySpan<char> rawCommand) => _engine.Registration = new RegisterCommand(rawCommand);

    private async Task HandlePerft(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length >= 2 && int.TryParse(items[1], out int depth) && depth >= 1)
        {
            var results = Perft.Results(_engine.Game.CurrentPosition, depth);
            await Perft.PrintPerftResult(depth, results, str => _engineToUci.Writer.WriteAsync(str));
        }
    }

    private async ValueTask HandleDivide(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length >= 2 && int.TryParse(items[1], out int depth) && depth >= 1)
        {
            var results = Perft.Divide(_engine.Game.CurrentPosition, depth, str => _engineToUci.Writer.TryWrite(str));
            await Perft.PrintPerftResult(depth, results, str => _engineToUci.Writer.WriteAsync(str));
        }
    }

    private async ValueTask HandleBench(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length < 2 || !int.TryParse(items[1], out int depth))
        {
            depth = Configuration.EngineSettings.BenchDepth;
        }
        var results = _engine.Bench(depth);
        await _engine.PrintBenchResults(results);
    }

    private async ValueTask HandleSettings()
    {
        var engineSettings = JsonSerializer.Serialize(Configuration.EngineSettings, EngineSettingsJsonSerializerContext.Default.EngineSettings);

        var message = $"{nameof(Configuration)}.{nameof(Configuration.EngineSettings)}:{Environment.NewLine}{engineSettings}";

        await _engineToUci.Writer.WriteAsync(message);
    }

    private async ValueTask HandleSystemInfo()
    {
        try
        {
            var simd = Bmi2.X64.IsSupported
                ? "Bmi2.X64 supported, PEXT BitBoards will be used"
                : "Bmi2.X64 not supported";

            await _engineToUci.Writer.WriteAsync(simd);
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

                await _engineToUci.Writer.WriteAsync($"{line}: {eval}", cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + e.StackTrace);
            await _engineToUci.Writer.WriteAsync(e.Message + e.StackTrace, cancellationToken);
        }
    }

    private async Task HandleEval(CancellationToken cancellationToken)
    {
        var score = _engine.Game.CurrentPosition.StaticEvaluation().Score;

        await _engineToUci.Writer.WriteAsync(score.ToString(), cancellationToken);
    }

    private async Task HandleFEN(CancellationToken cancellationToken)
    {
        const string fullMoveCounterString = " 1";

        var fen = _engine.Game.CurrentPosition.FEN()[..^3] + _engine.Game.HalfMovesWithoutCaptureOrPawnMove + fullMoveCounterString;

        await _engineToUci.Writer.WriteAsync(fen, cancellationToken);
    }

    private async ValueTask HandleOpenBenchSPSA(CancellationToken cancellationToken)
    {
        foreach(var tunableValue in SPSAAttributeHelpers.GenerateOpenBenchStrings())
        {
            await SendCommand(tunableValue, cancellationToken);
        }
    }

    private async ValueTask HandleOpenBenchSPSAPretty(CancellationToken cancellationToken)
    {
        await SendCommand(
            $"{"param name",-35} {"type",-5} {"def",-5} {"min",-5} {"max",-5} {"step",-5} {"R_end",-5}"
                + Environment.NewLine
                + "-----------------------------------------------------------------------",
            cancellationToken);

        foreach (var tunableValue in SPSAAttributeHelpers.GenerateOpenBenchPrettyStrings())
        {
            await SendCommand(tunableValue, cancellationToken);
        }
    }

    private async ValueTask HandleWeatherFactorySPSA(CancellationToken cancellationToken)
    {
        var tunableValues = SPSAAttributeHelpers.GenerateWeatherFactoryStrings();

        await SendCommand(new JsonObject(tunableValues).ToString(), cancellationToken);
    }

    #endregion

    private async Task SendCommand(string command, CancellationToken cancellationToken)
    {
        await _engineToUci.Writer.WriteAsync(command, cancellationToken);
    }
}
