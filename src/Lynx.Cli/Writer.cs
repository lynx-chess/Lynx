using NLog;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lynx.Cli
{
    public class Writer
    {
        private readonly ChannelReader<string> _engineOutputReader;
        private readonly ILogger _logger;

        public Writer(ChannelReader<string> engineOutputReader)
        {
            _engineOutputReader = engineOutputReader;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            try
            {
                while (await _engineOutputReader.WaitToReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
                {
                    _engineOutputReader.TryRead(out var output);

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        _logger.Debug($"[Lynx]\t{output}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }
            finally
            {
                _logger.Info($"Finishing {nameof(Writer)}");
            }
        }
    }
}
