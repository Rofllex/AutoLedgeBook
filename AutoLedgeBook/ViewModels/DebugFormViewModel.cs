using System.Linq;
using System.ComponentModel;

using AutoLedgeBook.Models;
using AutoLedgeBook.Logging;

namespace AutoLedgeBook.ViewModels;

public class DebugFormViewModel : ViewModel
{
    private readonly EventLogger? _eventLogger;

    public DebugFormViewModel() 
    {
        _eventLogger = GetEventLogger();
        if (_eventLogger is not null)
        {
            _eventLogger.Log += _eventLogger_Log;
            LoggingEnabled = true;
        }
    }


    public void FormLoaded()
    {

    }

    public BindingList<LogModel> Logs { get; init; } = new BindingList<LogModel>();

    public bool LoggingEnabled { get; init; }

    private void _eventLogger_Log(object sender, EventLogger.LogEventArgs args) => Logs.Add(new LogModel(args));
    

    private EventLogger? GetEventLogger()
    {
        if (Logger.Instance is LoggerRepeater logRepeater)
            return logRepeater.Loggers.OfType<EventLogger>().FirstOrDefault();

        return Logger.Instance as EventLogger;
    }

}