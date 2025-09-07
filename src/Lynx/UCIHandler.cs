using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace Lynx;

public sealed class UCIHandler
{
    private readonly Channel<string> _uciToEngine;
    private readonly Channel<object> _engineToUci;

    private readonly Searcher _searcher;
    private readonly Logger _logger;

    public UCIHandler(Channel<string> uciToEngine, Channel<object> engineToUci, Searcher searcher)
    {
        _uciToEngine = uciToEngine;
        _engineToUci = engineToUci;

        _searcher = searcher;
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
            if (_logger.IsInfoEnabled)
            {
                _logger.Info("[GUI]\t{0}", rawCommand);
            }

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
                    await HandlePonderHit();
                    break;
                case PositionCommand.Id:
                    HandlePosition(rawCommand);
                    break;
                case QuitCommand.Id:
                    HandleQuit();
                    return;
                case SetOptionCommand.Id:
                    HandleSetOption(rawCommand);
                    break;
                case StopCommand.Id:
                    await HandleStop();
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
                case "bench":
                    await HandleBench(rawCommand);
                    HandleQuit();
                    break;
                case "verbosebench":
                    await HandleVerboseBench(rawCommand);
                    HandleQuit();
                    break;
                case "printsettings":
                    await HandleSettings();
                    break;
                case "runtimeconfig":
                    await HandleRuntimeConfig();
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
                case "print":
                    HandlePrint();
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
#if DEBUG
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _searcher.PrintCurrentPosition();
#endif

        _searcher.AdjustPosition(command);

#if DEBUG
        _searcher.PrintCurrentPosition();
        _logger.Info("Position parsing took {0}ms", sw.ElapsedMilliseconds);
#endif
    }

    private async Task HandleStop() => await _searcher.StopSearching();

    private async Task HandleUCI(CancellationToken cancellationToken)
    {
        await SendCommand(IdCommand.NameString, cancellationToken);
        await SendCommand(IdCommand.AuthorString, cancellationToken);

        foreach (var availableOption in OptionCommand.AvailableOptions)
        {
            await SendCommand(availableOption, cancellationToken);
        }

        await SendCommand(UciOKCommand.Id, cancellationToken);
    }

    private async Task HandleIsReady(CancellationToken cancellationToken) => await SendCommand(ReadyOKCommand.Id, cancellationToken);

    private async Task HandlePonderHit()
    {
        if (Configuration.EngineSettings.IsPonder)
        {
            await _searcher.PonderHit();
        }
        else
        {
            _logger.Warn("Unexpected 'ponderhit' command, given pondering is disabled. Ignoring it");
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

#pragma warning disable S1479 // "switch" statements should not have too many "case" clauses
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
            case "hash":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.Hash = value;
                        _searcher.UpdateHash();
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
                        Configuration.EngineSettings.Threads = value;
                        _searcher.UpdateThreads();
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
            case "uci_cess960":
                {
                    if (length > 4 && bool.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.IsChess960 = value;
                        UpdateCurrentInstance();
                    }
                    break;
                }
            case "moveoverhead":
                {
                    if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                    {
                        Configuration.EngineSettings.MoveOverhead = value;
                    }
                    break;
                }

            default:
                if (!SPSAAttributeHelpers.ParseUCIOption(command, commandItems, lowerCaseFirstWord, length))
                {
                    _logger.Warn("Unsupported option: {0}", command.ToString());
                }
                break;
        }
#pragma warning restore S1479 // "switch" statements should not have too many "case" clauses
    }

    private void HandleNewGame()
    {
        _searcher.NewGame();
    }

    private static void HandleDebug(ReadOnlySpan<char> command) => Configuration.IsDebug = DebugCommand.Parse(command);

    private void HandleQuit()
    {
        _searcher.Quit();
        _engineToUci.Writer.Complete();
    }

    private void HandlePerft(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length >= 2 && int.TryParse(items[1], out int depth) && depth >= 1)
        {
            Perft.RunPerft(_searcher.CurrentPosition, depth, str => _engineToUci.Writer.TryWrite(str));
        }
    }

    private void HandleDivide(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length >= 2 && int.TryParse(items[1], out int depth) && depth >= 1)
        {
            Perft.RunDivide(_searcher.CurrentPosition, depth, str => _engineToUci.Writer.TryWrite(str));
        }
    }

    private async ValueTask HandleBench(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length < 2 || !int.TryParse(items[1], out int depth))
        {
            depth = Configuration.EngineSettings.BenchDepth;
        }

        await _searcher.RunBench(depth);
    }

    private async ValueTask HandleVerboseBench(string rawCommand)
    {
        var items = rawCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length < 2 || !int.TryParse(items[1], out int depth))
        {
            depth = Configuration.EngineSettings.BenchDepth;
        }

        await _searcher.RunVerboseBench(depth);
    }

    private async ValueTask HandleSettings()
    {
        var engineSettings = JsonSerializer.Serialize(Configuration.EngineSettings, EngineSettingsJsonSerializerContext.Default.EngineSettings);

        var message = $"{nameof(Configuration)}.{nameof(Configuration.EngineSettings)}:{Environment.NewLine}{engineSettings}";

        await _engineToUci.Writer.WriteAsync(message);
    }

    private async ValueTask HandleRuntimeConfig()
    {
        try
        {
            await _engineToUci.Writer.WriteAsync("CPU flags:");
            string[] intrinsics =
            [
                $"BMI2 = {Bmi2.IsSupported}",
                $"SSE4.2 = {Sse42.IsSupported}",
                $"AVX2 = {Avx2.IsSupported}",
                $"Avx512BW = {Avx512BW.IsSupported}",
            ];

            foreach (var instructionSet in intrinsics)
            {
                await _engineToUci.Writer.WriteAsync($"\t- {instructionSet}");
            }

            if (Sse.IsSupported)
            {
                await _engineToUci.Writer.WriteAsync("SSE supported, Prefetch0 will be used for TT prefetching");
            }

            if (Bmi1.IsSupported)
            {
                await _engineToUci.Writer.WriteAsync("BMI1 supported, ExtractLowestSetBit will be used for BitBoard LSB operations");
            }

            if (Bmi2.IsSupported)
            {
                await _engineToUci.Writer.WriteAsync("BMI2 supported, ParallelBitExtract (PEXT) will be used for slider pieces move generation");
            }

            await _engineToUci.Writer.WriteAsync("Garbage Collector (GC) settings:");

            foreach (var pair in GC.GetConfigurationVariables())
            {
                await _engineToUci.Writer.WriteAsync($"\t- {pair.Key} = {pair.Value}");
            }
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
            var fullPath = Path.GetFullPath(rawCommand[(rawCommand.IndexOf(' ') + 1)..].Replace("\"", string.Empty));
            if (!File.Exists(fullPath))
            {
                _logger.Warn("File {0} not found in (1), ignoring command", rawCommand, fullPath);
                return;
            }

            int lineCounter = 0;
            await foreach (var line in File.ReadLinesAsync(fullPath, cancellationToken))
            {
                var fen = line[..line.IndexOfAny([';', '[', '"'])];

                using var position = new Position(fen);
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

                ++lineCounter;
                if (lineCounter % 100 == 0)
                {
#pragma warning disable CA1849 // Call async methods when in an async method - intended
                    Thread.Sleep(50);
#pragma warning restore CA1849 // Call async methods when in an async method
                }
            }
        }
        catch (Exception e)
        {
            var sb = new StringBuilder(1_024);
            var errorMessage = ComposeExceptionMessage(e, sb).ToString();

#pragma warning disable S106, S2228 // Standard outputs should not be used directly to log anything
            Console.WriteLine(errorMessage);
#pragma warning restore S106, S2228 // Standard outputs should not be used directly to log anything

            await _engineToUci.Writer.WriteAsync(errorMessage, cancellationToken);

            static StringBuilder ComposeExceptionMessage(Exception e, StringBuilder sb)
            {
                sb.AppendLine();
                sb.AppendLine(e.Message);
                sb.AppendLine(e.StackTrace);

                if (e.InnerException is not null)
                {
                    ComposeExceptionMessage(e.InnerException, sb);
                }

                return sb;
            }
        }
    }

    private async Task HandleEval(CancellationToken cancellationToken)
    {
        var normalizedScore = WDL.NormalizeScore(_searcher.CurrentPosition.StaticEvaluation().Score);
        await _engineToUci.Writer.WriteAsync(normalizedScore, cancellationToken);
    }

    private async Task HandleFEN(CancellationToken cancellationToken)
    {
        await _engineToUci.Writer.WriteAsync(_searcher.FEN, cancellationToken);
    }

    private void HandlePrint()
    {
        _searcher.PrintCurrentPosition();
    }

    private async ValueTask HandleOpenBenchSPSA(CancellationToken cancellationToken)
    {
        foreach (var tunableValue in SPSAAttributeHelpers.GenerateOpenBenchStrings())
        {
            await SendCommand(tunableValue, cancellationToken);
        }
    }

    private async ValueTask HandleOpenBenchSPSAPretty(CancellationToken cancellationToken)
    {
        await SendCommand(
            $"{"param name",-35} {"type",-5} {"def",-5} {"min",-5} {"max",-5} {"step",-5} {"step %",-7} {"R_end",-5}"
                + Environment.NewLine
                + "----------------------------------------------------------------------------------------",
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
