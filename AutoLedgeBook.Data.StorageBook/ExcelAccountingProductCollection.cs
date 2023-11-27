using System.Collections;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;


/// <summary>
///     Коллекция продуктов Excel`я с возможностью изменения значения.
///     Реализует интерфейс <see cref="IReadOnlyCollection{ExcelLedgeAccountingProduct}"/>
/// </summary>
public abstract class ExcelLedgeAccountingProductCollectionBase<TProduct> :  IAccountingProductCollection<TProduct> where TProduct : class, IReadOnlyAccountingProduct 
{
    private readonly List<TProduct> _productsList = new();
    
    internal ExcelLedgeAccountingProductCollectionBase(ExcelStorageBook parentBook, int productValuesColumn) 
    {
        ParentBook = parentBook;
        _productsList = GetProducts(productValuesColumn, parentBook.Products);
    }

    public int Count => ParentBook.Products.Count;

    public TProduct this[string productName]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentNullException(nameof(productName));
            return _productsList.FirstOrDefault(p => p.Name == productName) ?? throw new Exception($"Продукт с наименованием \"{ productName }\" не найден.");
        }
    }

    public IEnumerator<TProduct> GetEnumerator() => _productsList.GetEnumerator();


    public string[] GetProductNames() => _productsList.Select(p => p.Name).ToArray();

    public bool ContainsProduct(string productName) => _productsList.FirstOrDefault(p => p.Name == productName) is not null;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected ExcelStorageBook ParentBook { get; }


    protected abstract List<TProduct> GetProducts(int columnIndex, IEnumerable<LedgeProduct> ledgeProducts);
}

public class ExcelLedgeAccountingProductCollection : ExcelLedgeAccountingProductCollectionBase<ExcelLedgeAccountingProduct>
{
    public ExcelLedgeAccountingProductCollection(ExcelStorageBook parentBook, int productValuesColumn) : base(parentBook, productValuesColumn)
    {

    }

    public void ClearAll()
    {
        foreach (ExcelLedgeAccountingProduct product in this)
            product.Clear();
    }

    protected override List<ExcelLedgeAccountingProduct> GetProducts(int columnIndex, IEnumerable<LedgeProduct> ledgeProducts)
        => ledgeProducts.Select(lp => lp.CreateProduct(ParentBook.Worksheet, columnIndex)).ToList();
    
}

public class ReadOnlyExcelLedgeAccountingProductCollection : ExcelLedgeAccountingProductCollectionBase<ReadOnlyExcelLedgeAccountingProduct>
{
    public ReadOnlyExcelLedgeAccountingProductCollection(ExcelStorageBook parentBook, int productValuesColumn) : base(parentBook, productValuesColumn)
    {

    }

    protected override List<ReadOnlyExcelLedgeAccountingProduct> GetProducts(int columnIndex, IEnumerable<LedgeProduct> ledgeProducts)
        => ledgeProducts.Select(lp => lp.CreateReadOnlyProduct(ParentBook.Worksheet, columnIndex)).ToList();

}

