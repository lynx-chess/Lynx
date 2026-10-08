using System.Runtime.InteropServices.JavaScript;
using System.Threading.Channels;

namespace Lynx.Wasm;

public partial class UciInterop
{
    private static readonly StringWriter _outputBuffer = new();
    internal static readonly OutputCapture OutputWriter = new();

    private static Lynx.Searcher? _searcher;
    private static Lynx.UCIHandler? _uciHandler;
    private static System.Threading.Channels.Channel<string>? _uciChannel;
    private static System.Threading.Channels.Channel<object>? _engineChannel;
    private static CancellationTokenSource? _cts;
    private static bool _initialized;

    [JSExport]
    public static async Task<string> Initialize()
    {
        Console.WriteLine("Initializing Lynx engine...");

        if (_initialized)
        {
            return "already initialized";
        }

        try
        {
            // Force single-threaded mode, small hash table, disable online tablebases
            Configuration.EngineSettings.Threads = 1;
            Configuration.EngineSettings.MaxDepth = 64; // Reduce from 128 to halve array sizes
            //Configuration.EngineSettings.TranspositionTableSize = 4;

            // Disable logging (NLog would fail in WASM)
            Configuration.GeneralSettings.EnableLogging = false;

            _uciChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = true, FullMode = BoundedChannelFullMode.Wait });
            _engineChannel = Channel.CreateBounded<object>(new BoundedChannelOptions(2 * Configuration.EngineSettings.MaxDepth) { SingleReader = true, SingleWriter = false, FullMode = BoundedChannelFullMode.DropOldest });

            _cts = new CancellationTokenSource();
            try
            {
                _searcher = new Lynx.Searcher(_uciChannel, _engineChannel);
            }
            catch (Exception searcherEx)
            {
                return $"error: {searcherEx.GetType().Name}: {searcherEx.Message} | {searcherEx.StackTrace?.Split('\n').FirstOrDefault()}";
            }
            _uciHandler = new Lynx.UCIHandler(_uciChannel, _engineChannel, _searcher);

            // Start the searcher and output writer as background tasks
            _ = Task.Run(() => _searcher.Run(_cts.Token));
            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var output in _engineChannel.Reader.ReadAllAsync(_cts.Token))
                    {
                        OutputWriter.EnqueueLine(output.ToString() ?? "");
                    }
                }
                catch (OperationCanceledException) { }
            });

            _initialized = true;
            return "ok";
        }
        catch (Exception ex)
        {
            return $"error: {ex.Message}";
        }
    }

    [JSExport]
    public static async Task<string> SendCommand(string uciCommand)
    {
        if (!_initialized || _uciHandler == null)
            return "error: not initialized";

        try
        {
            OutputWriter.Clear();

            await _uciHandler.Handle(uciCommand, _cts!.Token);

            // Wait for expected sentinel based on command type
            var trimmed = uciCommand.Trim();
            string? sentinel = null;
            if (trimmed == "uci") sentinel = "uciok";
            else if (trimmed == "isready") sentinel = "readyok";

            if (sentinel != null)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < 5000)
                {
                    await Task.Delay(20);
                    if (OutputWriter.Peek().Contains(sentinel))
                        break;
                }
            }
            else
            {
                // For other commands (position, setoption, stop), brief wait
                await Task.Delay(50);
            }

            return OutputWriter.Flush();
        }
        catch (Exception ex)
        {
            return $"error: {ex.Message}";
        }
    }

    /// <summary>
    /// Sends a command that triggers a search (like "go depth N") and waits for "bestmove".
    /// </summary>
    [JSExport]
    public static async Task<string> SendSearchCommand(string goCommand)
    {
        if (!_initialized || _uciHandler == null)
            return "error: not initialized";

        try
        {
            OutputWriter.Clear();

            await _uciHandler.Handle(goCommand, _cts!.Token);

            // Wait for bestmove output (search runs async on the Searcher task)
            var timeout = TimeSpan.FromSeconds(120);
            var sw = System.Diagnostics.Stopwatch.StartNew();

            while (sw.Elapsed < timeout)
            {
                await Task.Delay(50);
                var current = OutputWriter.Peek();
                if (current.Contains("bestmove"))
                    break;
                if (current.Contains("error"))
                    break;
            }

            return OutputWriter.Flush();
        }
        catch (Exception ex)
        {
            return $"error: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
        }
    }

    [JSExport]
    public static string PollOutput()
    {
        return OutputWriter.Flush();
    }

    [JSExport]
    public static bool IsReady()
    {
        return _initialized;
    }
}
