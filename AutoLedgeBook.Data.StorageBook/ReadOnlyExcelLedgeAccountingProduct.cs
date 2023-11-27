
using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.Extensions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;

/// <summary>
///     Продукт книги накладных в режиме "только для чтения"
/// </summary>
/// <inheritdoc cref="IReadOnlyAccountingProduct"/>
public class ReadOnlyExcelLedgeAccountingProduct : IReadOnlyAccountingProduct, IDisposable
{
    private readonly LedgeProduct _ledgeProduct;
    private readonly xl.Range _productValueCell;

    private bool _disposed = false;

    internal ReadOnlyExcelLedgeAccountingProduct(LedgeProduct ledgeProduct, xl.Range productValueCell)
    {
        _ledgeProduct = ledgeProduct;
        _productValueCell = productValueCell;
    }

    ~ReadOnlyExcelLedgeAccountingProduct()
    {
        if (_disposed)
            return;
        _disposed = true;
        Dispose();
    }

    public string Name => _ledgeProduct.ProductName;

    public double Value
    {
        get
        {
            ThrowIfDisposed();

            object cellValue = _productValueCell.Value;
            if (cellValue is null)
                return 0;
            return Convert.ToDouble(cellValue);
        }
    }

    public ProductUnits Units => _ledgeProduct.Units;

    public void Dispose()
    {
        if (_disposed)
            return;

        _productValueCell.Release();

        GC.SuppressFinalize(this);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ReadOnlyAccountingProduct));
    }
}
