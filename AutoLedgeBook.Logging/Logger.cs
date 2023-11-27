#nullable disable

namespace AutoLedgeBook.Logging;

/// <inheritdoc cref="ILogger"/>
public abstract class Logger : ILogger
{
    public static ILogger Instance { get; set; }


    public abstract void Error(string message);
    public abstract void Error(Exception ex);
    public abstract void Fatal(string message);
    public abstract void Fatal(Exception ex);
    public abstract void Info(string message);
    public abstract void Warning(string message);
    public abstract void Warning(Exception ex);
}