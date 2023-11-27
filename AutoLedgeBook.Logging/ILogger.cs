#nullable disable

namespace AutoLedgeBook.Logging;

public interface ILogger
{
    void Info(string message);
    void Warning(string message);
    void Warning(Exception ex);
    void Error(string message);
    void Error(Exception ex);
    void Fatal(string message);
    void Fatal(Exception ex);
}
