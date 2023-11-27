#nullable enable

using System.Diagnostics;
using System.Runtime.InteropServices;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.Excel.LowLevel;

/// <summary>
///     Контекст взаимодействия с Excel приложением.   
/// </summary>
public sealed class ExcelApplicationContext : IDisposable
{
    public bool SingleInitialized => s_SingleInstance.IsValueCreated;

    public static ExcelApplicationContext SingleInstance => s_SingleInstance.Value;

    private static Lazy<ExcelApplicationContext> s_SingleInstance = new Lazy<ExcelApplicationContext>(() => new ExcelApplicationContext());


    private bool disposed = false;
    private object mutex = new object();
    
    public ExcelApplicationContext() 
    {
        Application.Visible = false;
        Application.DisplayAlerts = false;
    }

    ~ExcelApplicationContext()
    {
        if (disposed)
            return;
        Dispose();
    }
    
    public xl.Application Application { get; private set; } = new xl.Application();

    /// <summary>
    ///     Освобождение ресурсов приложения Excel.
    ///     Автоматически закрывает все книги.
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Dispose()
    {
        lock (mutex)
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(ExcelApplicationContext));
            
            Application.WorkbookBeforeClose += (xl.Workbook workbook, ref bool cancel) => 
            {
                cancel = false;
            };
            Application.DisplayAlerts = false;

            while (Application.Workbooks.Count > 0)
            {
                xl.Workbook currentWorkbook = Application.Workbooks.Cast<xl.Workbook>().First();
                currentWorkbook.BeforeClose += (ref bool cancel) => cancel = false;
                currentWorkbook.Close(SaveChanges: false);
            }

            Application.Quit();
            Marshal.ReleaseComObject(Application);
            
            foreach (Process xlProcess in Process.GetProcesses().Where(p => p.ProcessName == "EXCEL" && p.MainWindowTitle.Length == 0))
                xlProcess.Kill();

            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    ///     Открыть книгу Excel документа
    /// </summary>
    /// <param name="workbookPath">
    ///     Путь до документа.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public xl.Workbook OpenWorkbook(string workbookPath) => OpenWorkbook(workbookPath, false);

    public xl.Workbook OpenWorkbook(string workbookPath, bool readOnly)
    {
        lock (mutex)
        {
            if (string.IsNullOrWhiteSpace(workbookPath))
                throw new ArgumentNullException(nameof(workbookPath));

            if (!File.Exists(workbookPath))
                throw new FileNotFoundException(workbookPath);
            
            try
            {
                return Application.Workbooks.Open(workbookPath, ReadOnly: readOnly);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при открытии книги", ex);
            }
        }
    }

    /// <summary>
    ///     Асинхронное открытие Excel документа.
    /// </summary>
    /// <param name="workbookPath"></param>
    /// <returns></returns>
    public Task<xl.Workbook> OpenWorkbookAsync(string workbookPath) => Task.Run(() => OpenWorkbook(workbookPath));

    /// <summary>
    ///     Синхронизированный вызов доступа к <see cref="Application"/>
    /// </summary>
    /// <param name="act">Экземпляр приложения excel.</param>
    /// <exception cref="ArgumentNullException" />
    public void SyncInvoke(Action<xl.Application> act)
    {
        if (act is null)
            throw new ArgumentNullException(nameof(act));

        lock (mutex)
        {
            act(Application);
        }
    }


}
