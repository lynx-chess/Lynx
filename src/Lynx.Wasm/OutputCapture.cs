
namespace Lynx.Wasm;

/// <summary>
/// Captures lines written by the engine output channel into a queue.
/// </summary>
public sealed class OutputCapture : TextWriter
{
    private readonly List<string> _lines = new();
    private readonly object _lock = new();

    public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;

    public override void WriteLine(string? value)
    {
        if (value != null)
            EnqueueLine(value);
    }

    public override void Write(string? value)
    {
        if (value != null)
            EnqueueLine(value);
    }

    public void EnqueueLine(string line)
    {
        lock (_lock)
        {
            _lines.Add(line);
        }
    }

    public string Flush()
    {
        lock (_lock)
        {
            if (_lines.Count == 0) return "";
            var result = string.Join("\n", _lines);
            _lines.Clear();
            return result;
        }
    }

    public string Peek()
    {
        lock (_lock)
        {
            return string.Join("\n", _lines);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _lines.Clear();
        }
    }
}
