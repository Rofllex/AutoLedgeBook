using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AutoLedgeBook.Data.Excel.LowLevel;
using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

#nullable enable

namespace AutoLedgeBook.Data.ExcelConsinments.SUC;

/// <summary>
///     Класс накладной СУЦ`а.
///     Данный класс реализует интерфейс <see cref="IReadOnlyConsinmentNote"/>
/// </summary>
public class ExcelSUCConsinmentNote : IReadOnlyConsinmentNote
{
    /// <summary>
    ///     Метод загрузки накладной из файла.
    /// </summary>
    /// <param name="filePath">Путь до файла</param>
    /// <param name="xlAppContext">Контекст взаимодействия с Excel`ем</param>
    /// <param name="findWorksheetByName">Функция поиска наименования листа документа</param>
    /// <returns>
    ///     Экземпляр класса <see cref="ExcelSUCConsinmentNote"/>
    /// </returns>
    /// <exception cref="FileNotFoundException">Если файл не найден</exception>
    /// <exception cref="Exception"></exception>
    public static ExcelSUCConsinmentNote FromFile(string filePath, ref ExcelApplicationContext? xlAppContext, Func<string, bool>? findWorksheetByName = null)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        if (findWorksheetByName == null)
            findWorksheetByName = n => n == "Лист1";

        if (xlAppContext is null)
            xlAppContext = new();

        xl.Workbook workbook = xlAppContext.OpenWorkbook(filePath);

        xl.Worksheet? consinmentWorksheet = workbook.Worksheets.Cast<xl.Worksheet>().FirstOrDefault(w => findWorksheetByName(w.Name));
        if (consinmentWorksheet == null)
            throw new Exception("Лист не найден");

        return new ExcelSUCConsinmentNote(consinmentWorksheet, xlAppContext);
    }

    private const string CONSINMENT_NUMBER_CELL_ADDRESS = "D4";
    private const string DATE_CELL_ADDRESS = "D5";

    /// <summary>
    ///     Контекст приложения excel.
    /// </summary>
    private readonly ExcelApplicationContext _appContext;

    private readonly double _totalWeight;
    private readonly xl.Worksheet worksheet;
    private readonly ReadOnlyAccountingProductCollection<IReadOnlyAccountingProduct> _products;

    internal ExcelSUCConsinmentNote(xl.Worksheet worksheet, ExcelApplicationContext xlAppContext)
    {
        this.worksheet = worksheet;

        Products = GetProducts(worksheet);
        Description = new SUCDescription(worksheet);

        _appContext = xlAppContext;

        _totalWeight = GetTotalProductsWeight(worksheet);
        _products = new ReadOnlyAccountingProductCollection<IReadOnlyAccountingProduct>(GetProductsFromWorksheet(worksheet));
    }


    IReadOnlyConsinmentDescription IReadOnlyConsinmentNote.Description => Description;

    public IAccountingProductCollection<IReadOnlyAccountingProduct> Products { get; }

    public SUCDescription Description { get; }

    public string Number
    {
        get
        {
            string cellValue = (string)worksheet.Range[CONSINMENT_NUMBER_CELL_ADDRESS].Value;
            return cellValue.Split(' ')[2];
        }
    }

    public DateOnly Day => Description.Day;

    /// <summary>
    ///     Получить сумму количественных продуктов. Всегда равно 0.
    /// </summary>
    /// <returns></returns>
    public double GetTotalProductsPcs() => 0;

    public double GetTotalProductsWeight() => _totalWeight;

    protected virtual IAccountingProductCollection<IReadOnlyAccountingProduct> GetProducts(xl.Worksheet worksheet)
        => new ReadOnlyAccountingProductCollection<IReadOnlyAccountingProduct>(GetProductsFromWorksheet(worksheet));

    private double GetTotalProductsWeight(xl.Worksheet consinmentWorksheet)
    {
        xl.Range totalCell = consinmentWorksheet.UsedRange.Find("Всего:");
        xl.Range totalWeightCell = totalCell.Offset[0, 1];

        double totalWeight = (double)totalWeightCell.Value;
        Marshal.ReleaseComObject(totalWeightCell);
        Marshal.ReleaseComObject(totalCell);
        return totalWeight;
    }

    private IReadOnlyAccountingProduct[] GetProductsFromWorksheet(xl.Worksheet sucWorksheet)
    {
        List<IReadOnlyAccountingProduct> products = new List<IReadOnlyAccountingProduct>();
        const string START_ADDRESS = "B25";
        xl.Range currentCell = sucWorksheet.Range[START_ADDRESS];
        while (currentCell.Value != null)
        {
            xl.Range productNameCell = currentCell.Offset[0, 1];
            string productName = Convert.ToString(productNameCell.Value);
            Marshal.ReleaseComObject(productNameCell);

            xl.Range productValueCell = currentCell.Offset[0, 6];
            double productValue = (double)productValueCell.Value;
            Marshal.ReleaseComObject(productValueCell);

            if (productValue > 0)
                products.Add(new ReadOnlyAccountingProduct(productName, productValue, ProductUnits.Kilo));

            xl.Range oldCell = currentCell;
            currentCell = currentCell.Offset[1];
            Marshal.ReleaseComObject(oldCell);
        }
        Marshal.ReleaseComObject(currentCell);
        return products.ToArray();
    }
}

public class SUCDescription : IReadOnlyConsinmentDescription
{
    internal SUCDescription(xl.Worksheet sucWorksheet)
    {
        DateTime dateTime = (DateTime)sucWorksheet.Range["D5"].Value;

        Day = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);

        PersonsCount = (int)sucWorksheet.Range["J16"].Value;
        Number = ((string)sucWorksheet.Range["D4"].Value).Split(' ')[2];
    }

    internal SUCDescription(DateOnly day, string number, int personsCount)
        => (Day, Number, PersonsCount) = (day, number, personsCount);

    public string Destination { get; } = "22/6-2";

    public string Number { get; }

    public int PersonsCount { get; }

    public string Type => "СУЦ";

    public DateOnly Day { get; }
}
