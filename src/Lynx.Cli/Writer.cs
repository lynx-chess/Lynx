using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lynx.Cli
{
    public class Writer
    {
        private readonly ChannelReader<string> _engineOutputReader;

        public Writer(ChannelReader<string> engineOutputReader)
        {
            _engineOutputReader = engineOutputReader;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (await _engineOutputReader.WaitToReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                _engineOutputReader.TryRead(out var output);

                if (!string.IsNullOrWhiteSpace(output))
                {
                    Console.WriteLine($"[Lynx]\t{output}");
                }
            }

            Console.WriteLine($"Finishing {nameof(Writer)}");
        }
    }
}
