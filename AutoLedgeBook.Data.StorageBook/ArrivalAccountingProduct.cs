
using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Data.StorageBook;

public class ArrivalAccountingProduct : IReadOnlyAccountingProduct
{
    private readonly LedgeProduct _product;
    private readonly int _columnIndex;

    internal ArrivalAccountingProduct(LedgeProduct product, int columnIndex)
    {
        _product = product;
        _columnIndex = columnIndex;
    }

    public string Name => throw new NotImplementedException();

    public double Value => throw new NotImplementedException();

    public ProductUnits Units => throw new NotImplementedException();
}

