using System.Buffers;

namespace Lynx.Model;

public class EvaluationContext : IDisposable
{
    private bool _disposedValue;

    public BitBoard[] Attacks { get; }
    public BitBoard[] AttacksBySide { get; }

    public static EvaluationContext Empty { get; } = new(false);

    public EvaluationContext()
    {
        Attacks = ArrayPool<BitBoard>.Shared.Rent(12);
        AttacksBySide = ArrayPool<BitBoard>.Shared.Rent(2);
    }

    private EvaluationContext(bool _)
    {
        Attacks = new BitBoard[12];
        AttacksBySide = new BitBoard[2];
    }

    public void FreeResources()
    {
        ArrayPool<BitBoard>.Shared.Return(Attacks, clearArray: true);
        ArrayPool<BitBoard>.Shared.Return(AttacksBySide, clearArray: true);

        _disposedValue = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                FreeResources();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
