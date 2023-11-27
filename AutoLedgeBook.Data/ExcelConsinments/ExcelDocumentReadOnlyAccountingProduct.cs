using System;
using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Продукт документа в режиме только для чтения.
/// </summary>
public class ExcelDocumentReadOnlyAccountingProduct : IReadOnlyAccountingProduct
{
    public ExcelDocumentReadOnlyAccountingProduct(string name, xl.Range valueCell, ProductUnits units)
    {
        ValueCell = valueCell ?? throw new ArgumentNullException(nameof(valueCell));

        if (string.IsNullOrWhiteSpace(Name = name))
            throw new ArgumentNullException(nameof(name));

        if (!Enum.IsDefined(Units = units))
            throw new ArgumentException($"Значение \"{units}\" не определено в типе \"{typeof(ProductUnits).FullName}\"", nameof(units));

        Units = units;
    }

    protected ExcelDocumentReadOnlyAccountingProduct()
    {
    }

    public virtual string Name { get; }

    public virtual double Value
    {
        get
        {
            object cellValue = ValueCell.Value;
            if (cellValue is null)
                return 0;
            return Convert.ToDouble(cellValue);
        }
    }

    public virtual ProductUnits Units { get; }

    protected readonly xl.Range ValueCell;
}
