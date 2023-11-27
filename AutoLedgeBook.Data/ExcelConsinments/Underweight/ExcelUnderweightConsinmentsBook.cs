#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.LowLevel;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments.Underweight;

/// <summary>
///     Накладная ДМТ.
///     Данный класс не наследуется.
/// </summary>
public sealed class ExcelUnderweightConsinmentsBook : IReadOnlyConsinmentsBook
{
    /// <summary>
    ///     Загрузить накладные из файла.
    /// </summary>
    /// <param name="filePath">Путь до файла</param>
    /// <param name="xlAppContext">Контекст взаимодействия с приложением Excel</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static ExcelUnderweightConsinmentsBook FromFile([NotNull] string filePath, ExcelApplicationContext? xlAppContext = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException(nameof(filePath));
        if (xlAppContext is null)
            xlAppContext = new();

        xl.Workbook workbook = xlAppContext.OpenWorkbook(filePath);
        return new ExcelUnderweightConsinmentsBook(workbook: workbook,
            worksheets: workbook.Worksheets.Cast<xl.Worksheet>()
                                          .Where(ws => ws.Visible == xl.XlSheetVisibility.xlSheetVisible && ws.Name.Contains("ДМТ", StringComparison.InvariantCultureIgnoreCase))
        );
    }

    private const string DATE_CELL_ADDRESS = "D8";
    private const string PERSONS_COUNT_CELL_ADDRESS = "C12";

    private readonly xl.Workbook _workbook;

    private readonly IReadOnlyDictionary<DateOnly, Lazy<UnderweightConsinmentNote>> _dateToConsinment;


    private ExcelUnderweightConsinmentsBook(xl.Workbook workbook, [NotNull] IEnumerable<xl.Worksheet> worksheets)
    {
        Debug.Assert(worksheets != null);
        _dateToConsinment = GetDatesToConsinment(worksheets);
        Dates = _dateToConsinment.Keys.ToImmutableList();
        _workbook = workbook;
    }

    public IReadOnlyCollection<IReadOnlyConsinmentNote> this[DateOnly day]
    {
        get
        {
            if (!_dateToConsinment.TryGetValue(day, out var consinmentFactory))
                throw new ArgumentOutOfRangeException(nameof(day), "Данная книга не поддерживает выбранную дату.");

            return new ReadOnlyConsinmentNoteCollection(consinmentFactory.Value);
        }
    }

    public IImmutableList<DateOnly> Dates { get; }

    public IReadOnlyConsinmentsCollection GetConsinmentsByDate(DateOnly day)
    {
        if (_dateToConsinment.TryGetValue(day, out var consinmentFactory))
            throw new ArgumentException($"Дата \"{day}\" не найдена");
        return new OnceConsinmentsCollection(consinmentFactory!.Value);
    }

    IReadOnlyList<DateOnly> IReadOnlyConsinmentsBook.Dates => Dates;


    public void Dispose()
    {
        Marshal.FinalReleaseComObject(_workbook);

        _workbook.Close(SaveChanges: false);

    }


    private IReadOnlyDictionary<DateOnly, Lazy<UnderweightConsinmentNote>> GetDatesToConsinment([NotNull] IEnumerable<xl.Worksheet> worksheetsCollection)
    {
        Dictionary<DateOnly, Lazy<UnderweightConsinmentNote>> dateToWorksheet = new();
        foreach (xl.Worksheet worksheet in worksheetsCollection)
        {
            if (worksheet.Visible == xl.XlSheetVisibility.xlSheetVisible
                && worksheet.Name.Contains("ДМТ"))
            {
                int personsCount = Convert.ToInt32(worksheet.Range[PERSONS_COUNT_CELL_ADDRESS].Value);
                if (personsCount > 1)
                {
                    DateTime? dateTime = worksheet.Range[DATE_CELL_ADDRESS].Value as DateTime?;
                    if (dateTime.HasValue)
                    {
                        dateToWorksheet[new DateOnly(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day)] = new(() => new(worksheet));
                    }
                }
            }
        }

        if (dateToWorksheet.Count == 0)
            throw new ArgumentException("Кол-во дат 0.");

        return dateToWorksheet.ToImmutableSortedDictionary();
    }
}
