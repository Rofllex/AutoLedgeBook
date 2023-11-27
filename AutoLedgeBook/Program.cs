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

        logger.Info("��������� ��������");
        logger.Info("�������� ��������");

#if DEBUG
        
        
#elif RELEASE
#if DEMBELCHORD
        /*
         *  ������ �����. ��������� ����� �� ��� ����� ������ ��������� ��������� ��������.
         *  ������ ������ ��� ��� ������ ��� ��� �����������. 
         *  
         *  ������ ������ 03.01.2022.
         *  ������ ���������� ~ 19.10.2021.
         *  
         *  ��������� ���� ������� by rofllex �� ����� ������.
         *  ���������� ���: * ���� �.�. (���. �������);
         *                  * ���������� �.�. (���. ��������);
         *                  * ������� �.�. (�����. ���������);
         *                  * ����� :D (�����������������)
         *                  * ������ �.�. (���������� ������)
         *  
         *  09.07.2021 - 08.07.2022.
         *  ������� ������!
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
            Logger.Instance.Error("������ ��� ��������� ��������");
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

        Logger.Instance.Info("���������� ����������");

        Logger.Instance.Info("������������ �������� Excel �����");
        ExcelApplicationContext.SingleInstance.Dispose();
        Logger.Instance.Info("������������ ���������");
    }

    public static void CloseApplication() => Application.Exit();

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Instance.Fatal("�������������� ����������!");
        Logger.Instance.Fatal((Exception)e.ExceptionObject);

        MessageBox.Show("�������������� ����������\n" + e.ExceptionObject);
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