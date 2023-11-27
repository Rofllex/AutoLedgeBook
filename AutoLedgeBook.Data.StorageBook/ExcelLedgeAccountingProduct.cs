using System.Diagnostics;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;

/// <summary>
///     Учетный продукт в формате excel.
/// </summary>
[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class ExcelLedgeAccountingProduct : IAccountingProduct
{
    private readonly LedgeProduct _product;
    private double _value;

    internal ExcelLedgeAccountingProduct(LedgeProduct product, xl.Range valueCell)
    {
        _product = product;
        ValueCell = valueCell;
    }


    /// <summary>
    ///     Название продукта.
    /// </summary>
    public string Name => _product.ProductName;

    /// <summary>
    ///     Вес продукта.
    /// </summary>
    public double Value
    {
        get => _value;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Значение продукта не может быть меньше 0",nameof(value));

            ValueCell.Value = _value = value;
        }
    }

    /// <summary>
    ///     Единица измерения продукта.
    /// </summary>
    public ProductUnits Units => _product.Units;

    internal xl.Range ValueCell { get; }

    internal void Clear()
    {
        ValueCell.ClearContents();
        _value = 0;    
    }

    private double GetProductValue()
    {
        object value = ValueCell.Value;
        if (value != null)
        {
            Type valueType = value.GetType();
            if (valueType == typeof(double))
                return (double)value;
            else
            {
                try
                {
                    double weight = Convert.ToDouble(value);
                    return weight;
                }
                catch
                {
                    return double.NaN;
                }
            }
        }
        else
            return 0d;
    }

    private string GetDebuggerDisplay()
        => $"[{Name}] = {Value}";
}
