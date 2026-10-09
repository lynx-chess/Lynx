using Lynx.Model;
using NLog;
using System.Threading.Channels;

namespace Lynx;

internal sealed class SearchWorker : IDisposable
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private static readonly TimeSpan _stopTimeout = TimeSpan.FromSeconds(1);

    private readonly Thread _thread;
    private readonly SemaphoreSlim _searchRequested = new(0);

    private SearchConstraints _searchConstraints;
    private bool _isPondering;
    private CancellationToken _absoluteSearchCancellationToken;
    private CancellationToken _searchCancellationToken;
    private TaskCompletionSource<SearchResult?>? _result;
    private volatile bool _stopRequested;
    private bool _disposed;

    public Engine Engine { get; }

    public SearchWorker(int id, ChannelWriter<object> engineWriter, in TranspositionTable tt)
    {
        Engine = new Engine(id, engineWriter, in tt);

        _thread = new Thread(Loop)
        {
            IsBackground = true,
            Name = $"Lynx search worker #{id}",
        };

        _thread.Start();
    }

    public Task<SearchResult?> Search(in SearchConstraints searchConstraints, bool isPondering, CancellationToken absoluteSearchCancellationToken, CancellationToken searchCancellationToken)
    {
        _searchConstraints = searchConstraints;
        _isPondering = isPondering;
        _absoluteSearchCancellationToken = absoluteSearchCancellationToken;
        _searchCancellationToken = searchCancellationToken;

        var result = new TaskCompletionSource<SearchResult?>(TaskCreationOptions.RunContinuationsAsynchronously);
        _result = result;

        _searchRequested.Release();

        return result.Task;
    }

    private void Loop()
    {
        while (true)
        {
            _searchRequested.Wait(CancellationToken.None);

            if (_stopRequested)
            {
                return;
            }

            var result = _result!;

            try
            {
                result.SetResult(Engine.Search(in _searchConstraints, _isPondering, _absoluteSearchCancellationToken, _searchCancellationToken));
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
        }
    }

    public void Stop()
    {
        if (_stopRequested)
        {
            return;
        }

        _stopRequested = true;
        _searchRequested.Release();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Stop();

        // Neither the semaphore nor the engine can be disposed while the thread might still be using them
        if (_thread.Join(_stopTimeout))
        {
            _searchRequested.Dispose();
            Engine.Dispose();
        }
        else
        {
            _logger.Warn("[{ThreadName}] Search didn't stop within {Timeout} ms, its engine won't be disposed", _thread.Name, _stopTimeout.TotalMilliseconds);
        }
    }
}
