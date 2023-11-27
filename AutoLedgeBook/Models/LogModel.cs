using System.ComponentModel;

using AutoLedgeBook.Logging;

namespace AutoLedgeBook.Models;

/// <summary>
///     Модель лога.
/// </summary>
public class LogModel : Model
{
    public LogModel(EventLogger.LogEventArgs logEvent)
    {
        Original = logEvent;
    }

    [Browsable(false)] public EventLogger.LogEventArgs Original { get; init; }

    [DisplayName("Сообщение")] public string? Message => Original.Message;

    [DisplayName("Тип лога")] public EventLogger.LogType LogType => Original.LogType;

    public static explicit operator EventLogger.LogEventArgs(LogModel model) => model.Original;
}
