using System.Diagnostics;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Модель продукта, представляющая наименование и столбцы.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal sealed record class CanteenProduct
{
    public CanteenProduct(string name, int arrivalColumnIndex, int consumptionColumnIndex, int consinmentNumberColumnIndex, int remaindColumnIndex, ProductUnits units)
    {
        Name = name;
        ArrivalColumnIndex = arrivalColumnIndex;
        RemaindColumnIndex = remaindColumnIndex;
        ConsumptionColumnIndex = consumptionColumnIndex;
        ConsinmentNumberColumnIndex = consinmentNumberColumnIndex;
        Units = units;
    }

    /// <summary>
    ///     Индекс столбца расхода.
    /// </summary>
    public int ConsumptionColumnIndex { get; }

    /// <summary>
    ///     Индекс столбца номера наклданой
    /// </summary>
    public int ConsinmentNumberColumnIndex { get; }

    /// <summary>
    ///     Индекс столбца остатка
    /// </summary>
    public int RemaindColumnIndex { get; }

    /// <summary>
    ///     Индекс столбца прихода
    /// </summary>
    public int ArrivalColumnIndex { get; }

    /// <summary>
    ///     Наименование
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Единицы измерения.
    /// </summary>
    public ProductUnits Units { get; }

    internal xl.Range GetArrivalCell(xl.Worksheet dataWorksheet, int rowIndex) => GetCell(dataWorksheet, rowIndex, ArrivalColumnIndex);

    internal xl.Range GetConsumptionCell(xl.Worksheet dataWorksheet, int rowIndex) => GetCell(dataWorksheet, rowIndex, ConsumptionColumnIndex);

    internal xl.Range GetConsinmentNumberCell(xl.Worksheet dataWorksheet, int rowIndex) => GetCell(dataWorksheet, rowIndex, ConsinmentNumberColumnIndex);

    internal xl.Range GetRemaindCell(xl.Worksheet dataWorksheet, int rowIndex) => GetCell(dataWorksheet, rowIndex, RemaindColumnIndex);

    private xl.Range GetCell(xl.Worksheet dataWorksheet, int rowIndex, int columnIndex) => dataWorksheet.Cells[RowIndex: rowIndex, ColumnIndex: columnIndex];
    

    private string GetDebuggerDisplay() => $"\"{Name}\" {Units}";
}
