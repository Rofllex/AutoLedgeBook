
using AutoLedgeBook.Data.Abstractions;
using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;

/// <summary>
///     Перечислитель типа <see cref="ExcelLedgeReadOnlyAccountingProductEnumerator"/>
/// </summary>
public class ExcelLedgeReadOnlyAccountingProductEnumerator : ExcelProductValueEnumeratorBase<IReadOnlyAccountingProduct>
{
    internal ExcelLedgeReadOnlyAccountingProductEnumerator(xl.Range productValuesRange, IReadOnlyCollection<LedgeProduct> products) : base(productValuesRange)
    {
        _products = products;
    }

    public override void Dispose()
    {
    
    }

    protected override IReadOnlyAccountingProduct? CreateByValueCell(xl.Range valueCell)
    {
        LedgeProduct? product = _products.FirstOrDefault(p => p.Row == valueCell.Row);
        if (product is null)
            return null;
        return new ReadOnlyAccountingProduct(product.ProductName, Convert.ToDouble(valueCell.Value), product.Units);
    }

    private readonly IReadOnlyCollection<LedgeProduct> _products;
}