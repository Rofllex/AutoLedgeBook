using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.LowLevel;

using xl = Microsoft.Office.Interop.Excel;

#nullable enable

namespace AutoLedgeBook.Data.ExcelConsinments.Guard;



public class ExcelGuardConsinmentsBook : ExcelOnceConsinmentsBookBase
{
    public static ExcelGuardConsinmentsBook FromFile(string filePath, ExcelApplicationContext? xlAppContext = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        xlAppContext = xlAppContext ?? ExcelApplicationContext.SingleInstance;

        xl.Workbook workbook = xlAppContext.OpenWorkbook(filePath, true);
        return FromWorkbook(workbook, xlAppContext);
    }

    internal static ExcelGuardConsinmentsBook FromWorkbook(xl.Workbook workbook, ExcelApplicationContext? xlAppContext = null)
    {
        IEnumerable<xl.Worksheet> worksheets = workbook.Worksheets.Cast<xl.Worksheet>();

        const string DATA_WORKSHEET_NAME = "Караул";
        xl.Worksheet? guardWorksheet = worksheets.FirstOrDefault(ws => ws.Name == DATA_WORKSHEET_NAME);

        if (guardWorksheet is null)
            throw new InvalidDataException($"В книге excel не найден лист с наименованием \"{DATA_WORKSHEET_NAME}\"");

        return new ExcelGuardConsinmentsBook(workbook, guardWorksheet, xlAppContext ?? ExcelApplicationContext.SingleInstance);
    }

    private ExcelApplicationContext? _xlAppContext;
    private xl.Worksheet? _worksheet;
    private xl.Workbook? _workbook;

    private ExcelGuardConsinmentNote? _consinment;

    private ExcelGuardConsinmentsBook(xl.Workbook workbook, xl.Worksheet guardSheet, ExcelApplicationContext xlAppContext)
    {
        _workbook = workbook;
        _worksheet = guardSheet;
        _xlAppContext = xlAppContext;

        _consinment = new(guardSheet);
    }

    public override IReadOnlyConsinmentNote ConsinmentNote => _consinment!;

    protected override void DisposeProtected()
    {
        _consinment = null;

        Marshal.ReleaseComObject(_worksheet!);
        _worksheet = null;

        _workbook!.Close(SaveChanges: false);
        Marshal.ReleaseComObject(_workbook);
        _workbook = null;

        _xlAppContext = null;
    }
}

public class ExcelGuardConsinmentNote : ExcelConsinmentNoteBase
{
    private xl.Worksheet _guardWorksheet;

    internal ExcelGuardConsinmentNote(xl.Worksheet guardWorksheet)
    {
        _guardWorksheet = guardWorksheet ?? throw new ArgumentNullException(nameof(guardWorksheet));

        Number = ParseConsinmentNumber(Convert.ToString(guardWorksheet.Range["A3"].Value));
        Day = ParseDateOnly(guardWorksheet.Range["D6"]);
        Description = new GuardConsinmentDescription(guardWorksheet);
        Products = new ReadOnlyAccountingProductCollection<IReadOnlyAccountingProduct>(GetProducts(guardWorksheet.Range["A17:H94"]));
    }

    public override string Number { get; }

    public override DateOnly Day { get; }

    public override IReadOnlyConsinmentDescription Description { get; }

    public override IAccountingProductCollection<IReadOnlyAccountingProduct> Products { get; }


    private string ParseConsinmentNumber(string consinmentNumberHeader)
    {
        string parsedConsinmentNumber = new(consinmentNumberHeader);

        while (parsedConsinmentNumber.IndexOf("  ") > -1)
            parsedConsinmentNumber = parsedConsinmentNumber.Replace("  ", " ");

        string[] headerParts = parsedConsinmentNumber.Split(' ');
        if (headerParts.Length >= 3)
            return headerParts[2];
        else
            throw new InvalidDataException("Заголовок накладной состоит больше чем из 3-х частей");
    }

    private DateOnly ParseDateOnly(xl.Range cell)
    {
        if (cell.Value is DateTime dateTime)
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        throw new ArgumentException("В ячейке не содержится даты", nameof(cell));
    }

    private class GuardConsinmentDescription : IReadOnlyConsinmentDescription
    {
        private xl.Worksheet _guardWorksheet;

        public GuardConsinmentDescription(xl.Worksheet guardWorksheet)
        {
            _guardWorksheet = guardWorksheet;
        }

        public string Destination => "22/6-2";

        public string Type => "Караул";

        public int PersonsCount => Convert.ToInt32(_guardWorksheet.Range["C12"].Value);
    }

    private List<IReadOnlyAccountingProduct> GetProducts(xl.Range productsRange)
    {
        List<ReadOnlyAccountingProduct> products = new();
        using GuardProductEnumerator enumerator = new(productsRange);
        while (enumerator.MoveNext())
            products.Add(enumerator.Current);
        return products.Cast<IReadOnlyAccountingProduct>().ToList();
    }

    private class GuardProductEnumerator : ExcelProductsEnumerator<ReadOnlyAccountingProduct>
    {
        public GuardProductEnumerator(xl.Range productsRange) : base(productsRange)
        {
        }

        protected override ReadOnlyAccountingProduct Build(xl.Range productRow)
        {
            string? productName = GetProductName(productRow);
            if (string.IsNullOrWhiteSpace(productName))
                throw new InvalidOperationException();
            ReadOnlyAccountingProduct product = new(productName, GetProductValue(productRow), ProductUnits.Kilo);
            return product;
        }

        protected override double GetProductValue(xl.Range productRow)
        {
            if (string.IsNullOrWhiteSpace(GetProductName(productRow)))
                return default;
            return Convert.ToDouble(productRow.Range["H1"].Value);
        }

        private string? GetProductName(xl.Range nameCell)
        {
            string? valueString = Convert.ToString(nameCell.Range["B1"].Value);
            return valueString;
        }
    }
}

public class ExcelGuardConsinmentsBookFactory : IExcelConsinmentBookFactory
{
    private readonly xl.Workbook _workbook;
    private readonly ExcelApplicationContext _xlAppContext;

    private ExcelGuardConsinmentsBook? _book;

    public ExcelGuardConsinmentsBookFactory(xl.Workbook workbook, ExcelApplicationContext xlAppContext)
    {
        _workbook = workbook;
        _xlAppContext = xlAppContext;
    }

    public Type TypeToBuild => typeof(ExcelGuardConsinmentsBook);

    public IReadOnlyConsinmentsBook Build()
        => _book ?? throw new InvalidOperationException($"Перед использованием данного метода необходимо вызвать {nameof(Check)}");

    public bool Check()
    {
        IEnumerable<xl.Worksheet> worksheets = _workbook.Worksheets.Cast<xl.Worksheet>();
        xl.Worksheet? guardWorksheet = worksheets.FirstOrDefault(ws => ws.Name == "Караул");
        if (guardWorksheet is null)
            return false;
        try
        {
            _book = ExcelGuardConsinmentsBook.FromWorkbook(_workbook, _xlAppContext);
            return true;
        }
        catch { return false; }
    }
}
