using NLog;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lynx.Cli
{
    public sealed class Writer
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
                await foreach (var output in _engineOutputReader.ReadAllAsync(cancellationToken))
                {
                    _logger.Debug($"[Lynx]\t{output}");
                    Console.WriteLine(output);
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
