using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using AutoLedgeBook.Data.Excel.LowLevel;
using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;
using AutoLedgeBook.Data.ExcelConsinments.SUC;

#nullable enable

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Реализация документа СУЦ накладной.
/// </summary>
public partial class ExcelSUCDocumentConsinmentBook : ExcelOnceConsinmentsBookBase
{
    /// <summary>
    ///     Загрузить книгу СУЦ из файла Excel.
    /// </summary>
    /// <param name="filePath">Путь до файла</param>
    /// <param name="xlAppContext">Контекст приложения Excel`я. Если значение null, то параметру будет присвоено значение new</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <inheritdoc cref="ExcelSUCDocumentConsinmentBook.ExcelSUCDocumentConsinmentBook(xl.Worksheet, ref ExcelApplicationContext)"/>
    public static ExcelSUCDocumentConsinmentBook FromFile(string filePath, ref ExcelApplicationContext xlAppContext)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);
        if (xlAppContext == null)
            xlAppContext = new ExcelApplicationContext();
        xl.Workbook workbook = xlAppContext.OpenWorkbook(filePath);
        return s_FromWorkbook(workbook, ref xlAppContext);
    }

    internal static ExcelSUCDocumentConsinmentBook s_FromWorkbook(xl.Workbook workbook, ref ExcelApplicationContext xlAppContext)
    {
        if (workbook == null)
            throw new ArgumentNullException(nameof(workbook));
        IEnumerable<xl.Worksheet> worksheets = workbook.Worksheets.Cast<xl.Worksheet>();
        // Наименование листа Excel который содержит накладную
        const string CONSINMENT_WORKSHEET_NAME = "Лист1";
        xl.Worksheet? consinmentWorksheet = worksheets.FirstOrDefault(ws => ws.Name == CONSINMENT_WORKSHEET_NAME);
        if (consinmentWorksheet == default)
        {
            throw new Exception($"Лист с названием \"{ CONSINMENT_WORKSHEET_NAME }\" не найден");
        }
        return new ExcelSUCDocumentConsinmentBook(consinmentWorksheet, ref xlAppContext);
    }

    /// <exception cref="InvalidDataException"></exception>
    private ExcelSUCDocumentConsinmentBook(xl.Worksheet worksheet, ref ExcelApplicationContext xlAppContext) : base()
    {
        DateTime? consinmentNumber = worksheet.Range["D5"].Value as DateTime?;
        if (!consinmentNumber.HasValue)
            throw new InvalidDataException($"Неверная структура документа. В ячейке \"D5\" не содержится дата документа.");
        SUCConsinment = new ExcelSUCConsinmentNote(worksheet, xlAppContext);
    }

    protected override void DisposeProtected()
    {
    }

    public ExcelSUCConsinmentNote SUCConsinment { get; }

    public override IReadOnlyConsinmentNote ConsinmentNote => SUCConsinment;
}

partial class ExcelSUCDocumentConsinmentBook
{
    /// <summary>
    ///     Фабрика создания документа.
    /// </summary>
    public class ExcelSUCDocumentFactory : IExcelConsinmentBookFactory
    {
        public ExcelSUCDocumentFactory(xl.Workbook workbook, ref ExcelApplicationContext xlAppContext)
        {
            _workbook = workbook ?? throw new ArgumentNullException(nameof(workbook));
            _xlAppContext = xlAppContext;
        }

        public Type TypeToBuild { get; } = typeof(ExcelSUCDocumentConsinmentBook);

        public IReadOnlyConsinmentsBook Build()
        {
            if (_consinmentBook == null)
                throw new InvalidOperationException($"Перед началом построения, должен успешно пройти метод \"{ nameof(Check) }\"");
            return _consinmentBook;
        }

        public bool Check()
        {
            try
            {
                _consinmentBook = s_FromWorkbook(_workbook, ref _xlAppContext);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private readonly xl.Workbook _workbook;
        private ExcelSUCDocumentConsinmentBook? _consinmentBook;
        private ExcelApplicationContext _xlAppContext;
    }
}

