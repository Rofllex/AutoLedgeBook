using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;

/// <summary>
///     Запись, описывающая продукт.
/// </summary>
/// <param name="Row">Номер строки</param>
/// <param name="ProductName">Наименование продукта</param>
/// <param name="Units">Единицы измерения продукта</param>
public record class LedgeProduct
{
    internal LedgeProduct(int row, string productName, ProductUnits units)
    {
        Row = row;
        ProductName = productName;
        Units = units;
    }

    /// <summary>
    ///     Строка продукта.
    /// </summary>
    public int Row { get; }

    /// <summary>
    ///     Наименование продукта.
    /// </summary>
    public string ProductName { get; }
    
    /// <summary>
    ///     Единицы измерения продукта.
    /// </summary>
    public ProductUnits Units { get; }

    internal ExcelLedgeAccountingProduct CreateProduct(xl.Worksheet worksheet, int columnIndex) 
        => new ExcelLedgeAccountingProduct(this, worksheet.Cells[Row, columnIndex]);

    internal ReadOnlyExcelLedgeAccountingProduct CreateReadOnlyProduct(xl.Worksheet worksheet, int columnIndex)
        => new(this, worksheet.Cells[Row, columnIndex]);
}
