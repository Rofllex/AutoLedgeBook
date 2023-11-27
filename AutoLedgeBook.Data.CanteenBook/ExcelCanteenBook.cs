using System.Diagnostics;
using System.Collections;
using System.Collections.Immutable;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.LowLevel;
using AutoLedgeBook.Data.CanteenBook.Infrastructure;

using xl = Microsoft.Office.Interop.Excel;
using AutoLedgeBook.Data.Excel.Extensions;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Класс взаимодействия с книгой кладовщика столовой. 
/// </summary>
/// <inheritdoc cref="IReadOnlyConsinmentsBook"/>
public class ExcelCanteenBook : XlConsinmentsBook, IEnumerable<ExcelCanteenConsinmentsCollection>
{
    /// <summary>
    ///     Загрузить книгу накладных из файла Excel./
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="applicationContext"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static ExcelCanteenBook FromFile(string filePath, ExcelApplicationContext? applicationContext = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл по пути \"{ filePath }\" не найден", filePath);
        
        if (applicationContext is null)
            applicationContext = new ExcelApplicationContext();
        
        xl.Workbook workbook = applicationContext.OpenWorkbook(filePath);

        const string WORKSHEET_NAME = "Книга кладовщика";
        xl.Worksheet? worksheet = workbook.Worksheets.Cast<xl.Worksheet>().FirstOrDefault(ws => ws.Name == WORKSHEET_NAME);
        if (worksheet is null)
        {
            workbook.Close(SaveChanges: false);
            throw new InvalidDataException($"Лист с названием \"{ WORKSHEET_NAME }\" не найден.");
        }
        
        CanteenBookConsinmentsMetadataCollection consinmentsMeta = CanteenBookConsinmentsMetadataCollection.GetOrCreate(workbook);
        return new ExcelCanteenBook(worksheet, consinmentsMeta);
    }


    // Верхняя граница возможно допустимых ячеек.
    internal const int UPPER_CELLS_BORDER_ROW = 5;
    // Нижняя граница возможно допустимых ячеек
    internal const int LOWER_CELLS_BORDER_ROW = 2053;

    private readonly string AllCellsRangeString = $"A{UPPER_CELLS_BORDER_ROW}:ZH{LOWER_CELLS_BORDER_ROW}";
    private readonly string[] _productNames;

    private readonly Dictionary<int, ExcelCanteenConsinment> _rowIndexToConsinment = new();
    private readonly Dictionary<DateOnly, ExcelCanteenConsinmentsCollection> _dateToConsinmentCollection = new();


    private ExcelCanteenBook(xl.Worksheet worksheet, CanteenBookConsinmentsMetadataCollection consinmentsMeta) : base((xl.Workbook)worksheet.Parent)
    {
        Debug.Assert(worksheet is not null);
        this.Worksheet = worksheet;

        // Анализ книги и загрузка словаря номер столбца - название продукта.
        ColumnIndexToProductName = GetColumnIndexToProductName();

        xl.Range firstColumnDataRange = Worksheet.Range["A1:A2383"];
        RowIndexToDateDictionary = GetRowIndexToDateDictionary(firstColumnDataRange);
        Products = GetProducts();
        
        _productNames = Products.Select(p => p.Name).ToArray();
        Debug.Assert(consinmentsMeta != null);
        ConsinmentsMetaCollection = consinmentsMeta;
    }

    /// <summary>
    ///     Получение наклданой по номеру.
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной</param>
    /// <returns></returns>
    /// <exception cref="ConsinmentNotFoundException">
    ///     Исключение если накладную по номеру не удалось найти.
    /// </exception>
    public override ExcelCanteenConsinment this[string consinmentNumber] 
    {
        get
        {
            if (!ContainsConsinment(consinmentNumber))
                throw new ConsinmentNotFoundException(consinmentNumber);
            
            CanteenBookConsinmentMetadata consinmentMeta = ConsinmentsMetaCollection[consinmentNumber];
            xl.Range findedCell = Worksheet.Range[AllCellsRangeString].FindCellByValue(consinmentNumber)!;
            Debug.Assert(findedCell is not null);

            KeyValuePair<int, DateOnly> defaultValue = new(-1, DateOnly.MinValue);
            KeyValuePair<int, DateOnly> rowIndexToDate = RowIndexToDateDictionary.FirstOrDefault(p => p.Key > findedCell.Row, defaultValue);
            
            Debug.Assert(!rowIndexToDate.Equals(defaultValue));
            return GetConsinmentByRow(findedCell.Row);
        }
    }

    public override void SaveAs(string filePath, ISaveSettings saveSettings) => SaveAs(filePath);

    /// <summary>
    ///     Проверить, содержится ли накладная в книге.
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException" />
    public override bool ContainsConsinment(string consinmentNumber)
    {
        if (string.IsNullOrWhiteSpace(consinmentNumber))
            throw new ArgumentNullException(nameof(consinmentNumber));
        return ConsinmentsMetaCollection.ContainsMetadata(consinmentNumber);
    }

    public override ExcelCanteenConsinmentsCollection GetConsinmentsByDate(DateOnly date)
    {
        ExcelCanteenConsinmentsCollection? consinmentsCollection;
        if (!_dateToConsinmentCollection.TryGetValue(date, out consinmentsCollection))
        {
            consinmentsCollection = new ExcelCanteenConsinmentsCollection(this, date, GetConsinmentsRangeByDate(date));
            _dateToConsinmentCollection[date] = consinmentsCollection;
        }

        return consinmentsCollection;
    }

    /// <summary>
    ///     Получить названия доступных продуктов в книге.
    /// </summary>
    /// <returns></returns>
    public override string[] GetProductNames() => _productNames;

    /// <summary>
    ///     Получение перечисления накладных.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<ExcelCanteenConsinmentsCollection> GetEnumerator() => new ExcelCanteenConsinmentConsumptionCollectionEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    protected override void DisposeProtected()
    {
    }

    protected override IReadOnlyList<DateOnly> GetDates() => RowIndexToDateDictionary.Values.ToList().AsReadOnly();

    #region internal properties

    /// <summary>
    /// Рабочий лист на котором находятся все накладные.
    /// </summary>
    internal xl.Worksheet Worksheet { get; }

    /// <summary>
    ///     Сопоставление: индекс столбца - название продукта.
    /// </summary>
    internal IReadOnlyDictionary<int, string> ColumnIndexToProductName { get; private set; }

    /// <summary>
    ///     Сопоставление: индект строки - дата.
    ///     ДАННЫЕ БЕРУТСЯ ИЗ ПЕРВОГО СТОЛБЦА.
    /// </summary>
    internal IReadOnlyDictionary<int, DateOnly> RowIndexToDateDictionary { get; private set; }

    internal CanteenBookConsinmentsMetadataCollection ConsinmentsMetaCollection { get; private set; }

    /// <summary>
    ///      Индекс строки с нименованием продукта.
    /// </summary>
    internal const int ProductNameRowIndex = 2;

    // A1:ZH2053
    /// <summary>
    ///  Коллекция моделей, описывающих продукты.
    /// </summary>
    internal IReadOnlyCollection<CanteenProduct> Products { get; }

    /// <summary>
    ///     Получить диапазон всех ячеек рабочего листа.
    /// </summary>
    /// <returns></returns>
    internal xl.Range GetAllCells()
        => Worksheet.Range[AllCellsRangeString];

    /// <summary>
    ///     Получить диапазон строк накладных по дате.
    /// </summary>
    /// <param name="date">Необходимая дата</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">
    ///     Если дата не поддерживается
    /// </exception>
    internal xl.Range GetConsinmentsRangeByDate(DateOnly date)
    {
        const int DEFAULT_KEY_VALUE = -1;
        KeyValuePair<int, DateOnly> pair = RowIndexToDateDictionary.FirstOrDefault(p => p.Value == date, new KeyValuePair<int, DateOnly>(DEFAULT_KEY_VALUE, DateOnly.MinValue));
        if (pair.Key == DEFAULT_KEY_VALUE)
            throw new ArgumentException("Данная дата не поддерживается текущей книгой...");
        xl.Range dateCell = Worksheet.Cells[RowIndex: pair.Key, ColumnIndex: 1];
        xl.Range firstConsinmentCell = dateCell.Offset[RowOffset: 26];
        xl.Range lastConsinmentCell = firstConsinmentCell.Offset[RowOffset: 32];
        xl.Range consinmentsRange = Worksheet.Range[$"{firstConsinmentCell.Row}:{lastConsinmentCell.Row}"];
        return consinmentsRange;
    }

    /// <summary>
    ///     Получить накладную по номеру строки
    /// </summary>
    /// <param name="rowIndex">Номер строки</param>
    /// <returns></returns>
    internal ExcelCanteenConsinment GetConsinmentByRow(int rowIndex)
    {
        ExcelCanteenConsinment? consinment;
        if (!_rowIndexToConsinment.TryGetValue(rowIndex, out consinment))
        {
            DateOnly consinmentDate = GetDateByRowIndex(rowIndex);
            CanteenBookConsinmentMetadata? meta = ConsinmentsMetaCollection.GetByConsinmentRowIndex(rowIndex);
            consinment = new ExcelCanteenConsinment(this, consinmentDate, rowIndex, meta);
            _rowIndexToConsinment[rowIndex] = consinment;
        }
        return consinment;
    }

    /// <summary>
    ///     Получить дату по номеру строки
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal DateOnly GetDateByRowIndex(int rowIndex)
    {
        Debug.Assert(rowIndex > 1, "Значение индекса не может быть меньше 1");
        KeyValuePair<int, DateOnly> pair = RowIndexToDateDictionary.LastOrDefault(p => rowIndex > p.Key);
        if (pair.Key == 0)
            throw new ArgumentOutOfRangeException("Invalid row index", nameof(rowIndex));
        return pair.Value;
    }

    #endregion

    /// <summary>
    ///     Создание словаря индекс столбца - наименование продукта.
    /// </summary>
    /// <returns></returns>
    private IImmutableDictionary<int, string> GetColumnIndexToProductName()
    {
        xl.Range currentCell = Worksheet.Range["A2"];
        ImmutableDictionary<int, string>.Builder productNameToColumnIndexBuilder = ImmutableDictionary.CreateBuilder<int, string>();
        while (!string.IsNullOrWhiteSpace(currentCell.Value))
        {
            string? currentCellValue = Convert.ToString(currentCell.Value);
            if (currentCellValue != "Дата" && !productNameToColumnIndexBuilder.ContainsKey(currentCell.Column))
            {
                productNameToColumnIndexBuilder.Add(currentCell.Column, currentCellValue!);
            }

            xl.Range oldCell = currentCell;
            currentCell = currentCell.Offset[0, 1];
            oldCell.Release();
        }
        currentCell.Release();

        return productNameToColumnIndexBuilder.ToImmutable();
    }

    /// <summary>
    ///     Получить словарь индекс строки - дата.
    /// </summary>
    /// <param name="rangeWithDates"></param>
    /// <returns></returns>
    private IReadOnlyDictionary<int, DateOnly> GetRowIndexToDateDictionary(xl.Range rangeWithDates)
    {
        ImmutableSortedDictionary<int,DateOnly>.Builder dictionary = ImmutableSortedDictionary.CreateBuilder<int,DateOnly>();
        foreach (xl.Range cell in rangeWithDates)
        {
            object? cellValue = cell.Value;
            if (cellValue is DateTime dateTime)
            {
                DateOnly date = DateOnly.FromDateTime(dateTime);
                dictionary.Add(cell.Row, date);
            }
        }

        return dictionary.ToImmutable();
    }
    
    /// <summary>
    ///     Получить коллекцию условных наименований продуктов.
    /// </summary>
    /// <returns></returns>
    private IReadOnlyList<CanteenProduct> GetProducts()
    {
        xl.Range currentCell = Worksheet.Range["B2"];

        List<CanteenProduct> productsList = new();
        do
        {
            object? cellValue = currentCell.Value;
            if (cellValue is not null)
            {
                string cellValueString = Convert.ToString(cellValue)!;
                if (!cellValueString.Contains("Дата"))
                {
                    int arrivalColumnIndex = currentCell.Column,
                        consinmentNumberColumnIndex = arrivalColumnIndex + 1,
                        consumptionColumnIndex = consinmentNumberColumnIndex + 1,
                        remainColumnIndex = consumptionColumnIndex + 1;

                    CanteenProduct product = new(name: cellValueString,
                                                 arrivalColumnIndex: arrivalColumnIndex,
                                                 consumptionColumnIndex: consumptionColumnIndex,
                                                 consinmentNumberColumnIndex: consinmentNumberColumnIndex,
                                                 remaindColumnIndex: remainColumnIndex,
                                                 units: ProductUnits.Kilo);
                    productsList.Add(product);
                }    
            }
            else
            {
                currentCell = currentCell.Offset[0, 1];
            }
            currentCell = currentCell.Offset[0, 1];
        } while (currentCell.Value is not null);

        return productsList.AsReadOnly();
    }
}
