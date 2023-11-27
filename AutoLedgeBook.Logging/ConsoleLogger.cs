using System.Runtime.CompilerServices;

using static System.Console;

#nullable disable

namespace AutoLedgeBook.Logging;

public class ConsoleLogger : Logger
{
    public override void Error(string message)
        => WriteLog(message, ConsoleColor.Red);

    public override void Error(Exception ex)
        => Error(ex.ToString());

    public override void Fatal(string message)
        => WriteLog(message, ConsoleColor.DarkRed);

    public override void Fatal(Exception ex)
        => Fatal(ex.ToString());

    public override void Info(string message)
        => WriteLog(message, ConsoleColor.Green);

    public override void Warning(string message)
        => WriteLog(message, ConsoleColor.Yellow);

    public override void Warning(Exception ex)
        => Warning(ex.ToString());


    private static object _consoleMutex = new object();

    private void WriteLog(string logType, string message, ConsoleColor foregroundColor)
    {
        lock (_consoleMutex)
        {
            ConsoleColor previousColor = ForegroundColor;

            Write("[");
            ForegroundColor = foregroundColor;
            Write(logType);
            ForegroundColor = previousColor;
            Write("] ");
            WriteLine($"{{{DateTime.Now}}} " + message);
        }
    }

    private void WriteLog(string message, ConsoleColor foregroundColor, [CallerMemberName] string callerName = "") => WriteLog(callerName, message, foregroundColor);
}
