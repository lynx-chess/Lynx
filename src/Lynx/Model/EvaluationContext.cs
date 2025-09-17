using System.Buffers;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public sealed class EvaluationContext : IDisposable
{
    public BitBoard[] Attacks;
    public BitBoard[] AttacksBySide;

    private bool _disposedValue;

    public EvaluationContext()
    {
        Attacks = ArrayPool<BitBoard>.Shared.Rent(12);
        AttacksBySide = ArrayPool<BitBoard>.Shared.Rent(2);

        Array.Clear(Attacks, 0, 12);
        AttacksBySide[0] = 0;
        AttacksBySide[1] = 0;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ArrayPool<BitBoard>.Shared.Return(Attacks);
                ArrayPool<BitBoard>.Shared.Return(AttacksBySide);
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);

#pragma warning disable S3234, IDISP024 // "GC.SuppressFinalize" should not be invoked for types without destructors
        GC.SuppressFinalize(this);
#pragma warning restore S3234, IDISP024 // "GC.SuppressFinalize" should not be invoked for types without destructors
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
