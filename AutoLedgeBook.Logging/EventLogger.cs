#nullable enable

namespace AutoLedgeBook.Logging;

public class EventLogger : Logger
{
    public enum LogType
    {
        Info,
        Warning,
        Error,
        Fatal
    };

    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(LogType logType, string message, Exception? exception = null) : this (DateTime.Now, logType, message, exception)
        {
        }

        public LogEventArgs(DateTime timestamp, LogType logType, string message, Exception? exception = null)
                => (Timestamp, LogType, Message, this.Exception) = (timestamp, logType, message, exception);

        public LogType LogType { get; init; }

        public string Message { get; init; }

        public Exception? Exception { get; init; }
    
        public DateTime Timestamp { get; init; }
    }

    public delegate void LogEventHandler(object sender, LogEventArgs args);


    public event LogEventHandler Log = (_, __) => { };

    public override void Error(string message)
        => InvokeLog(LogType.Error, message);

    public override void Error(Exception ex)
        => InvokeLog(LogType.Error, string.Empty, ex);

    public override void Fatal(string message)
        => InvokeLog(LogType.Fatal, message);

    public override void Fatal(Exception ex)
        => InvokeLog(LogType.Fatal, string.Empty, ex);

    public override void Info(string message)
        => InvokeLog(LogType.Info, message);

    public override void Warning(string message)
        => InvokeLog(LogType.Warning, message);

    public override void Warning(Exception ex)
        => InvokeLog(LogType.Warning, string.Empty, ex);

    private void InvokeLog(LogType logType, string message, Exception? exception = null)
        => Log(this, new LogEventArgs(logType, message, exception));
}
