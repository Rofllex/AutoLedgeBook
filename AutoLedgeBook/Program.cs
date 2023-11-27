using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;

using AutoLedgeBook.Logging;
using AutoLedgeBook.Data.Excel.LowLevel;

namespace AutoLedgeBook;

internal static class Program
{
    public static readonly string ExecutableDirectoryPath;

    public static readonly string CacheDirectoryPath;



    private const string LICENSE_FILE_NAME = "license.bin";

    static Program()
    {
        ExecutableDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        CacheDirectoryPath = Path.Combine(ExecutableDirectoryPath, "cache");
        s_LogsDirectoryLocation = ExecutableDirectoryPath;

    }


    private const string LOGS_DIRECTORY_NAME = "logs";
    private static readonly string s_LogsDirectoryLocation;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        InitializeLogger();

        var logger = Logger.Instance; 

        logger.Info("Программа запущена");
        logger.Info("Проверка лицензии");

#if DEBUG
        
        
#elif RELEASE
#if DEMBELCHORD
        /*
         *  Смотри мужик. Вероятнее всего ты уже понял почему программа перестала работать.
         *  Просто сделай вид что пиздец как все исправляешь. 
         *  
         *  Первый коммит 03.01.2022.
         *  Начало разработки ~ 19.10.2021.
         *  
         *  Программа была сделана by rofllex за время службы.
         *  Специально для: * Кныш О.Б. (зав. складом);
         *                  * Калашников А.А. (упр. столовой);
         *                  * Кудашов А.А. (склад. кладовщик);
         *                  * Настя :D (делопроизводитель)
         *                  * Махнев А.Н. (столовский писарь)
         *  
         *  09.07.2021 - 08.07.2022.
         *  Хорошей службы!
         */

        DateTime dembelDate = new DateTime(year: 2022,
                                           month: 7,
                                           day: 8,
                                           hour: 5,
                                           minute: 55,
                                           second: 0);

        if (DateTime.UtcNow > dembelDate)
        {
            while (true)
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
        }
        else
            License = new LicenseInfo("infinity", new DateTime(3000, 1, 1), DateTime.MinValue);
#else
        try
        {
            License = LicenseManager.ValidateLicense(Path.Combine(ExecutableDirectoryPath, LICENSE_FILE_NAME));
        }
        catch (Exception ex)
        {
            Logger.Instance.Error("Ошибка при валидации лицензии");
            Logger.Instance.Error(ex);
            return;
        }
#endif
#endif
        
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Forms.MainForm());

        Logger.Instance.Info("Завершение приложения");

        Logger.Instance.Info("Освобождение ресурсов Excel хоста");
        ExcelApplicationContext.SingleInstance.Dispose();
        Logger.Instance.Info("Освобождение завершено");
    }

    public static void CloseApplication() => Application.Exit();

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Instance.Fatal("Необработанное исключение!");
        Logger.Instance.Fatal((Exception)e.ExceptionObject);

        MessageBox.Show("Необработанное исключение\n" + e.ExceptionObject);
    }


    /// <exception cref="LicenseValidationException" />

    private static string GetLogFileName() => $"{DateTime.Now:dd.MM.yyyy HH.mm}.txt";

    private static void InitializeLogger()
    {
        string logsDirectoryPath = Path.Combine(s_LogsDirectoryLocation, LOGS_DIRECTORY_NAME);
        if (!Directory.Exists(logsDirectoryPath))
            Directory.CreateDirectory(logsDirectoryPath);
        string logsFilePath = Path.Combine(logsDirectoryPath, GetLogFileName());
        StreamWriter logsWriter = File.CreateText(logsFilePath);

        Logger.Instance = new LoggerRepeater(
            new TextWriterLogger(logsWriter)
#if DEBUG
            , new EventLogger()
#endif
            );
    }
}