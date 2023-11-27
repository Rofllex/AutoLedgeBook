using System.Diagnostics;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.Extensions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Класс, представляющий реализцию <see cref="IAccountingProduct"/> для книги столовой.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ExcelCanteenProduct : IAccountingProduct
{
    private readonly CanteenProduct _product;
    private readonly xl.Range _valueCell;

    private xl.Range? _consinmentNumberCell;

    private readonly ExcelCanteenConsinment _parentConsinment;

    private double _productValue = default;
    private string _consinmentNumber = string.Empty;

    internal ExcelCanteenProduct(ExcelCanteenConsinment parentConsinment, CanteenProduct product, xl.Range valueCell)
    {
        Debug.Assert(product is not null);
        _product = product;

        Debug.Assert(valueCell is not null);
        _valueCell = valueCell;

        _parentConsinment = parentConsinment ?? throw new ArgumentNullException(nameof(parentConsinment));

        _productValue = GetProductValue();
        _consinmentNumber = GetConsinmentNumber();
    }

    ~ExcelCanteenProduct()
    {
        _valueCell.Release();
        _consinmentNumberCell?.Release();
    }

    /// <summary>
    ///     Значение расхода.
    /// </summary>
    public double Value
    {
        get => _productValue;

        set
        {
            if (_productValue == value)
                return;

            if (value == 0)
            {
                _valueCell.ClearContents();
                ConsinmentNumberCell.ClearContents();
                
                _consinmentNumber = string.Empty;
                _productValue = value;
            }
            else
            {
                _valueCell.Value = _productValue = value;
                ConsinmentNumber = _parentConsinment.Number;
            }
        }
    }

    public string Name => _product.Name;

    public ProductUnits Units => _product.Units;

    /// <summary>
    ///     Номер накладной.
    /// </summary>
    /// <remarks>
    ///     Так случилось, что книга накладных столовой придумана <u>Долбоёбами</u>, придётся реализовать это здесь...
    /// </remarks>
    public string? ConsinmentNumber
    {
        get => _consinmentNumber;

        internal set
        {
            if (Value > 0 && _consinmentNumber != value)
            {
                ConsinmentNumberCell.Value = _consinmentNumber = (value ?? string.Empty);
            }
        }
    }

    /// <summary>
    ///     Очистка значения продукта и номера накладной.
    /// </summary>
    public void Clear()
    {
        _valueCell.ClearContents();

        ConsinmentNumberCell.ClearContents();

        _productValue = default;
        _consinmentNumber = string.Empty;
    }

    internal CanteenProduct Product => _product;

    private xl.Range ConsinmentNumberCell
    {
        get => _consinmentNumberCell ?? (_consinmentNumberCell = GetConsinmentNumberCell());
    }

    private double GetProductValue()
    {
        object? cellValue = _valueCell.Value;
        if (cellValue is null)
            return default;

        return Convert.ToDouble(cellValue);
    }

    private string GetConsinmentNumber()
    {
        if (Value == 0)
            return string.Empty;
                
        return Convert.ToString(ConsinmentNumberCell.Value) ?? string.Empty;
    }

    private xl.Range GetConsinmentNumberCell() => _product.GetConsinmentNumberCell(_parentConsinment.ParentBook.Worksheet, _valueCell.Row);

    private string GetDebuggerDisplay() => $"{Name} => {Value} {Units}";
}

public class ReadOnlyExcelCanteenProduct : IReadOnlyAccountingProduct
{
    private readonly xl.Range _valueCell;

    internal ReadOnlyExcelCanteenProduct(CanteenProduct canteenProduct, xl.Range valueCell)
    {
        Product = canteenProduct;
        _valueCell = valueCell;
    }

    ~ReadOnlyExcelCanteenProduct()
    {
        _valueCell.Release();
    }

    public string Name => Product.Name;

    public double Value
    {
        get
        {
            object cellValue = _valueCell.Value;
            if (cellValue is null)
                return default;
            return Convert.ToDouble(cellValue);
        }
    }

    public ProductUnits Units => Product.Units;

    internal CanteenProduct Product { get; }
}