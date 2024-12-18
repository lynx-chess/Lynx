using System.Threading.Channels;

namespace Lynx;

public sealed class SilentChannelWriter<T> : ChannelWriter<T>
{
    public static SilentChannelWriter<T> Instance { get; } = new();

    /// <summary>
    /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
    /// https://csharpindepth.com/articles/singleton
    /// </summary>
#pragma warning disable S3253 // Constructor and destructor declarations should not be redundant
    static SilentChannelWriter() { }
#pragma warning restore S3253 // Constructor and destructor declarations should not be redundant

    private SilentChannelWriter() { }

    /// <summary>
    /// Returns <see langword="true"/>
    /// </summary>
    public override bool TryWrite(T item) => true;

    /// <summary>
    /// Returns a static <see cref="ValueTask"/> wrapping <see langword="true"/>
    /// </summary>
    /// <returns>A non-usable <see cref="ValueTask"/></returns>
    public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default) => default;
}
