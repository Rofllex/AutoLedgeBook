#nullable disable

namespace AutoLedgeBook.Logging;

/// <summary>
///     Файловый логгер.
/// </summary>
public class TextWriterLogger : Logger, IDisposable
{
    private readonly TextWriter _textWriter;
    private readonly object _syncMutex = new();
    private const string TIMESTAMP_FORMAT = "dd.MM.yyyy HH:mm:ss";

    private bool _disposed = false;

    public TextWriterLogger(TextWriter textWriter)
    {
        _textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
    }

    ~TextWriterLogger()
    {
        if (_disposed)
            return;
        Dispose();
    }


    public override void Error(string message) => WriteLine(nameof(Error), message);

    public override void Error(Exception ex) => Error(ex.ToString());

    public override void Fatal(string message) => WriteLine(nameof(Fatal), message);

    public override void Fatal(Exception ex) => Fatal(ex.ToString());

    public override void Info(string message) => WriteLine(nameof(Info), message);

    public override void Warning(string message) => WriteLine(nameof(Warning), message);

    public override void Warning(Exception ex) => Warning(ex.ToString());

    private void WriteLine(string prefix, string message)
    {
        lock (_syncMutex)
        {
            string messageToWrite = $"{ DateTime.Now.ToString(TIMESTAMP_FORMAT) } [{ prefix }] => { message }";
            _textWriter.WriteLine(messageToWrite);
            _textWriter.Flush();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _textWriter.Close();
        _textWriter.Dispose();

        GC.SuppressFinalize(this);
    }
}
