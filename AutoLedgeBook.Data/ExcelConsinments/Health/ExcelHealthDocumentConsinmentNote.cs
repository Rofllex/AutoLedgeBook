#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.LowLevel;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments.Health;

/// <summary>
///     Накладная лечебного пайка(мед рота + госпиталь)
/// </summary>
public class ExcelHealthDocumentConsinmentNote : IReadOnlyConsinmentNote
{
    #region public static

    /// <summary>
    ///     Загрузить накладную из файла.
    /// </summary>
    /// <param name="filePath">Путь до файла excel</param>
    /// <param name="findWorksheet">Делегат позволяющий найти необходимый лист.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public static ExcelHealthDocumentConsinmentNote FromFile(string filePath, Func<IEnumerable<string>, string> findWorksheet, ExcelApplicationContext? applicationContext = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        if (findWorksheet == null)
            throw new ArgumentNullException(nameof(findWorksheet));

        if (applicationContext == null)
            applicationContext = ExcelApplicationContext.SingleInstance;

        xl.Workbook workbook = applicationContext.OpenWorkbook(filePath);

        string[] worksheetNames = workbook.Worksheets.Cast<xl.Worksheet>().Select(w => w.Name).ToArray();

        string worksheetName = findWorksheet(worksheetNames);
        if (worksheetName == string.Empty || !worksheetNames.Contains(worksheetName))
            throw new Exception("Лист не найден");

        return new ExcelHealthDocumentConsinmentNote(workbook.Worksheets[worksheetName]);
    }

    /// <summary>
    ///     Загрузить накладную из файла и указать название листа excel для поиска
    /// </summary>
    /// <param name="filePath">Путь до файла</param>
    /// <param name="worksheetName">Название листа с накладной</param>
    /// <returns></returns>
    /// <inheritdoc cref="FromFile(string, Func{IEnumerable{string}, string})"/>
    public static ExcelHealthDocumentConsinmentNote FromFile(string filePath, string worksheetName = "Накладная")
        => FromFile(filePath, e =>
        {
            return e.Contains(worksheetName) ? worksheetName : string.Empty;
        });

    #endregion

    #region excel addresses 
    private const string TOTAL_PCS_CELL_ADDRESS = "G150";
    private const string TOTAL_WEIGHT_CELL_ADDRESS = "G149";

    private const string DATE_CELL_ADDRESS = "D5";
    private const string DESTINATION_CELL_ADDRESS = "E2";
    private const string CONSINMENT_NUMBER_CELL_ADDRESS = "A3";
    private const string PERSONS_COUNT_CELL_ADDRESS = "C11";
    private const string TYPE_CELL_ADDRESS = "G11";
    #endregion

    private readonly xl.Worksheet _consinmentWorksheet;

    private readonly double _totalPcs,
                            _totalWeight;

    internal ExcelHealthDocumentConsinmentNote(xl.Worksheet consinmentWorksheet)
    {
        _consinmentWorksheet = consinmentWorksheet;
        Description = new ExcelHealthDocumentConsinmentNoteDescription(consinmentWorksheet);

        Products = new ExcelDocumentConsinmentProductCollection(_consinmentWorksheet.Range["A16:H148"].Rows);

        _totalPcs = Convert.ToDouble(_consinmentWorksheet.Range[TOTAL_PCS_CELL_ADDRESS].Value);
        _totalWeight = Convert.ToDouble(_consinmentWorksheet.Range[TOTAL_WEIGHT_CELL_ADDRESS].Value);

        Number = GetConsinmentNumber(_consinmentWorksheet.Range[CONSINMENT_NUMBER_CELL_ADDRESS]);
    }


    public ExcelHealthDocumentConsinmentNoteDescription Description { get; }

    public IAccountingProductCollection<IReadOnlyAccountingProduct> Products { get; }

    public string Number { get; }

    public DateOnly Day => Description.Date;


    public double GetTotalProductsPcs() => _totalPcs;

    public double GetTotalProductsWeight() => _totalWeight;

    IReadOnlyConsinmentDescription IReadOnlyConsinmentNote.Description => Description;

    private string GetConsinmentNumber(xl.Range consinmentNumberCell)
    {
        string? cellValue = Convert.ToString(consinmentNumberCell.Value).Trim();
        if (string.IsNullOrWhiteSpace(cellValue))
            throw new InvalidDataException($"Ячейка с адресом накладной не содержит контента для парсинга. Row: {consinmentNumberCell.Row} Column: {consinmentNumberCell.Column}");

        if (cellValue.IndexOf("Накладная № ") > -1)
            cellValue = cellValue.Replace("Накладная № ", string.Empty);
        return cellValue;
    }
}
