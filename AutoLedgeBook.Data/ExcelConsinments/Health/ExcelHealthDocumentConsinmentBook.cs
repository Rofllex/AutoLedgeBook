using System;
using System.IO;
using System.Linq;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.LowLevel;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments.Health;

/// <summary>
///     Накладные мед роты и госпиталя.
/// </summary>
/// <inheritdoc cref="IReadOnlyConsinmentsBook"/>
public class ExcelHealthDocumentConsinmentBook : ExcelOnceConsinmentsBookBase
{
    /// <summary>
    ///     Загрузить накладную сан части. госпиталя из файла.
    /// </summary>
    /// <param name="filePath">Путь до файла</param>
    /// <param name="applicationContext">Контекст excel`я</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static ExcelHealthDocumentConsinmentBook FromFile(string filePath, ExcelApplicationContext applicationContext = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);
        if (applicationContext is null)
            applicationContext = new ExcelApplicationContext();

        xl.Workbook workbook = applicationContext.OpenWorkbook(filePath);
        return new ExcelHealthDocumentConsinmentBook(workbook);
    }

    private const string CONSINMENT_WORKSHEET_NAME = "Накладная";


    private ExcelHealthDocumentConsinmentBook(xl.Workbook workbook)
    {
        Workbook = workbook;
        Worksheet = workbook.Worksheets.Cast<xl.Worksheet>().FirstOrDefault(ws => ws.Name == CONSINMENT_WORKSHEET_NAME) ?? throw new Exception($"Неверная структура документа, лист с названием \"{CONSINMENT_WORKSHEET_NAME}\" не найден");
        ConsinmentNote = new ExcelHealthDocumentConsinmentNote(Worksheet);
    }

    public override ExcelHealthDocumentConsinmentNote ConsinmentNote { get; }

    internal xl.Workbook Workbook { get; }
    internal xl.Worksheet Worksheet { get; private set; }


    protected override void DisposeProtected()
    {
        try
        {
            Workbook.Close(SaveChanges: false);
            Marshal.ReleaseComObject(Workbook);
        }
        catch { }
    }
}
