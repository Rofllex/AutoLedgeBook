using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.LowLevel;

using xl = Microsoft.Office.Interop.Excel;
using AutoLedgeBook.Data.ExcelConsinments.Buffet;

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Книга накладных шведского стола.
/// </summary>
/// <inheritdoc cref="IReadOnlyConsinmentsBook"/>
public partial class ExcelBuffetDocumentConsinmentBook : IReadOnlyConsinmentsBook
{
    /// <summary>
    ///     Загрузить книгу из файла.
    /// </summary>
    /// <param name="filePath">Путь до файла</param>
    /// <remarks>
    ///     Если <paramref name="xlAppContext"/> будет null, функция инициализирует новый экземпляр.
    /// </remarks>
    /// <returns>
    ///     Экземпляр типа <see cref="ExcelBuffetDocumentConsinmentBook"/>
    /// </returns>
    /// <exception cref="FileNotFoundException">Если файл по заданному пути не найден.</exception>
    public static ExcelBuffetDocumentConsinmentBook FromFile(string filePath, ref ExcelApplicationContext xlAppContext)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл не найден", filePath);

        if (xlAppContext == null)
            xlAppContext = new ExcelApplicationContext();
        
        xl.Workbook workbook = xlAppContext.OpenWorkbook(filePath);
        return FromExcelWorkbook(workbook);
    }

    
    internal static ExcelBuffetDocumentConsinmentBook FromExcelWorkbook(xl.Workbook workbook) 
    {
        IEnumerable<xl.Worksheet> worksheets = workbook.Worksheets.Cast<xl.Worksheet>()
                .Where(ws => ws.Visible == xl.XlSheetVisibility.xlSheetVisible && ws.Name.Contains("накл"));

        ImmutableDictionary<DateOnly, ExcelBuffetDocumentConsinmentNote> dateToConsinmentDictionary;
        {
            ImmutableDictionary<DateOnly, ExcelBuffetDocumentConsinmentNote>.Builder dateToConsinmentBuilder = ImmutableDictionary.CreateBuilder<DateOnly, ExcelBuffetDocumentConsinmentNote>();

            foreach (xl.Worksheet worksheet in worksheets)
            {
                ExcelBuffetDocumentConsinmentNote consinment;
                try
                {
                    consinment = new ExcelBuffetDocumentConsinmentNote(worksheet);
                }
                catch
                {
                    if (!Debugger.IsAttached && !Debugger.Launch())
                        throw;
                    Debugger.Break();
                    continue;
                }
#if RELEASE
                catch
                {
                    continue;
                }
#endif


                if (consinment.Description.PersonsCount < 1 || !consinment.Description.Type.Contains("шведск"))
                    continue;

                dateToConsinmentBuilder.Add(consinment.Day, consinment);
            }

            dateToConsinmentDictionary = dateToConsinmentBuilder.ToImmutable();
        }


        if (dateToConsinmentDictionary.Count == 0)
            throw new InvalidDataException("Неверная структура документа, кол-во дат 0");
        
        return new ExcelBuffetDocumentConsinmentBook(dateToConsinmentDictionary);
    }

    /// <summary>
    ///     Неизменяемый словарь дата - лист excel`я.
    /// </summary>
    private readonly IReadOnlyDictionary<DateOnly, ExcelBuffetDocumentConsinmentNote> _dateToConsinmentDictionary;
    
    private ExcelBuffetDocumentConsinmentBook(IReadOnlyDictionary<DateOnly, ExcelBuffetDocumentConsinmentNote> dateToConsinmentDictionary)
    {
        _dateToConsinmentDictionary = dateToConsinmentDictionary ?? throw new ArgumentNullException(nameof(dateToConsinmentDictionary));

        Dates = dateToConsinmentDictionary.Keys.ToImmutableList();
    }

    /// <summary>
    ///     Получить накладную по дате.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public ExcelBuffetDocumentConsinmentNote this[DateOnly day]
    {
        get
        {
            if (!_dateToConsinmentDictionary.TryGetValue(day, out ExcelBuffetDocumentConsinmentNote consinment))
                throw new KeyNotFoundException($"Накладная по дате { day } не найдена");
            
            return consinment;
        }
    }
      
    public IReadOnlyList<DateOnly> Dates { get; }


    public void Dispose() { }

#region implicit interfaces implementation

    IReadOnlyList<DateOnly> IReadOnlyConsinmentsBook.Dates => Dates;

    IReadOnlyConsinmentsCollection IReadOnlyConsinmentsBook.GetConsinmentsByDate(DateOnly day)
    {
        if (!_dateToConsinmentDictionary.ContainsKey(day))
            throw new ArgumentOutOfRangeException($"Данная дата \"{ day }\" не поддерживается");
        var consinment = _dateToConsinmentDictionary[day];
        return new OnceConsinmentsCollection(consinment);
    }

#endregion
}

#nullable enable

partial class ExcelBuffetDocumentConsinmentBook
{
    /// <summary>
    ///     Фабрика для автоматического создания накладной шведского стола.
    ///     Реализует интерфейс <see cref="IExcelConsinmentBookFactory"/>
    /// </summary>
    public class ExcelBuffetDocumentConsinmentBookFactory : IExcelConsinmentBookFactory
    {
        private readonly xl.Workbook _workbook;
        private ExcelBuffetDocumentConsinmentBook? _consinmentBook;

        public ExcelBuffetDocumentConsinmentBookFactory(xl.Workbook workbook)
        {
            _workbook = workbook ?? throw new ArgumentNullException(nameof(workbook));
        }

        public Type TypeToBuild { get; } = typeof(ExcelBuffetDocumentConsinmentBook);

        public IReadOnlyConsinmentsBook Build() => _consinmentBook ?? throw new InvalidOperationException("Перед началом построения объекта необходимо проверить его на возможность создания..");

        public bool Check()
        {
            try
            {
                _consinmentBook = FromExcelWorkbook(_workbook);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}

