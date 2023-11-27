using System.Collections.Immutable;

#nullable disable

namespace AutoLedgeBook.Logging;

public class LoggerRepeater : ILogger
{
    public LoggerRepeater(IEnumerable<ILogger> loggers)
    {
        if (loggers == null)
            throw new ArgumentNullException(nameof(loggers));

        this.Loggers = ImmutableList.CreateRange(loggers);
    }

    public LoggerRepeater(params ILogger[] loggers) : this((IEnumerable<ILogger>)loggers)
    {
    }

    public IImmutableList<ILogger> Loggers { get; }

    #region ILoggerImplementation

    void ILogger.Error(string message)
        => repeat(l => l.Error(message));

    void ILogger.Error(Exception ex)
        => repeat(l => l.Error(ex));

    void ILogger.Fatal(string message)
        => repeat(l => l.Fatal(message));

    void ILogger.Fatal(Exception ex)
        => repeat(l => l.Fatal(ex));

    void ILogger.Info(string message)
        => repeat(l => l.Info(message));

    void ILogger.Warning(string message)
        => repeat(l => l.Warning(message));

    void ILogger.Warning(Exception ex)
        => repeat(l => l.Warning(ex));

    #endregion

    private void repeat(Action<ILogger> invokeLogger)
    {
        foreach (ILogger logger in Loggers)
            invokeLogger(logger);
    }

}
