//using NLog;
using System.Text;

namespace Lynx.Cli;

public sealed class Listener
{
    //private readonly Logger _logger;
    private readonly UCIHandler _uciHandler;

    public Listener(UCIHandler uCIHandler)
    {
        _uciHandler = uCIHandler;
        //_logger = LogManager.GetCurrentClassLogger();
    }

    public async Task Run(CancellationToken cancellationToken, params string[] args)
    {
        try
        {
            foreach (var arg in args)
            {
                await _uciHandler.Handle(arg, cancellationToken);
            }

            IncreaseInputBufferSize();

            while (!cancellationToken.IsCancellationRequested)
            {
                var input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    await _uciHandler.Handle(input, cancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            //_logger.Fatal(e);
        }
        finally
        {
            //_logger.Info("Finishing {0}", nameof(Listener));
        }
    }

    /// <summary>
    /// By default, the method reads input from a 256-character input buffer.
    /// Because this includes the Environment.NewLine character(s), the method can read lines that contain up to 254 characters.
    /// To read longer lines, call the OpenStandardInput(Int32) method.
    /// Source: https://learn.microsoft.com/en-us/dotnet/api/system.console.readline?view=net-8.0
    /// </summary>
    private static void IncreaseInputBufferSize()
    {
        // Based on Lizard's solution. 4096 * 4 is enough to accept position commands with at least 500 moves
        Console.SetIn(new StreamReader(Console.OpenStandardInput(), Encoding.UTF8, false, 4096 * 4));
    }

    /// <summary>
    /// Something like this would work as well to read input without the input buffer limitation
    /// </summary>
    /// <returns></returns>
    private static string ReadInput()
    {
        Span<byte> bytes = stackalloc byte[4096 * 4];
        Stream inputStream = Console.OpenStandardInput();
        int outputLength = inputStream.Read(bytes);

        return Encoding.UTF8.GetString(bytes[..outputLength]);
    }
}
