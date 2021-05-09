using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lynx.Cli
{
    public class Listener
    {
        private readonly Channel<string> _guiInputReader;

        public Listener(Channel<string> guiInputReader)
        {
            _guiInputReader = guiInputReader;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = Console.ReadLine();
                await _guiInputReader.Writer.WriteAsync(input!, cancellationToken);
            }

            Console.WriteLine($"Finishing {nameof(Listener)}");
        }
    }
}
