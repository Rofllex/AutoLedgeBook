using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Коллекция продуктов.
/// </summary>
public class CanteenBookAccountingProductCollection : IAccountingProductCollection<ExcelCanteenProduct>
{
    private readonly ExcelCanteenBook _parentBook;
    private readonly IReadOnlyCollection<CanteenProduct> _canteenProducts;
    private readonly int _rowIndex;

    private readonly IReadOnlyCollection<ExcelCanteenProduct> _products;


    internal CanteenBookAccountingProductCollection(ExcelCanteenBook parentBook, ExcelCanteenConsinment parentConsinment, IReadOnlyCollection<CanteenProduct> canteenProducts, int rowIndex)
    {
        _parentBook = parentBook;
        _canteenProducts = canteenProducts;
        Debug.Assert(_canteenProducts.Count > 0, "Кол-во обнаруженных продуктов не может быть нулевым..."); 
        _rowIndex = rowIndex;
        Debug.Assert(_rowIndex > 0, "В экселе индекс строки не может быть меньше 0...");

        _products = ProductsHelper.GetProducts(parentConsinment, parentBook.Worksheet, canteenProducts, rowIndex);

        Debug.Assert(_products.Count == canteenProducts.Count, "Количество продуктов не может различаться...");
    }

    public ExcelCanteenProduct this[string productName]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentNullException(nameof(productName));
            return _products.FirstOrDefault(p => p.Name == productName) ?? throw new Exception($"Продукт с наименованием \"{ productName }\" не найден.");
        }
    }

    
    public int Count => _products.Count;

    public bool ContainsProduct(string productName)
        => _products.FirstOrDefault(p => p.Name == productName) != default;
    
    public string[] GetProductNames()
        => _parentBook.GetProductNames();
    
    public void ClearAll()
    {
        foreach (ExcelCanteenProduct product in this)
            product.Clear();
    }


    public IEnumerator<ExcelCanteenProduct> GetEnumerator()
        => _products.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

public abstract class CanteenBookAccountingProductCollectionBase<TProduct> : IAccountingProductCollection<TProduct> where TProduct : class, IReadOnlyAccountingProduct
{
    private readonly ExcelCanteenBook _parentBook;
    private string[]? _productNames;

    private IReadOnlyDictionary<string, TProduct> _productNameToProductDictionary;

    internal CanteenBookAccountingProductCollectionBase(ExcelCanteenBook parentBook, int rowIndex)
    {
        Debug.Assert(parentBook is not null);
        _parentBook = parentBook;
        _productNameToProductDictionary = ImmutableDictionary.CreateRange(GetProducts(parentBook, rowIndex));
    }

    public TProduct this[string productName] 
    {
        get
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentNullException(nameof(productName));

            return _productNameToProductDictionary[productName];
        }
    }

    public int Count => _parentBook.Products.Count;

    public bool ContainsProduct(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentNullException(nameof(productName));
        
        return _parentBook.Products.FirstOrDefault(p => p.Name == productName) != default;
    }

    public IEnumerator<TProduct> GetEnumerator() => _productNameToProductDictionary.Values.GetEnumerator();

    public string[] GetProductNames()
    {
        if (_productNames is null)
            _productNames = _parentBook.Products.Select(p => p.Name).ToArray();
        return _productNames;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected virtual Dictionary<string, TProduct> GetProducts(ExcelCanteenBook canteenBook, int rowIndex) 
    {
        Dictionary<string, TProduct> productNameToProduct = new();

        foreach (CanteenProduct product in canteenBook.Products)
        {
            if (productNameToProduct.ContainsKey(product.Name))
                continue;

            TProduct readOnlyProduct = GetProduct(canteenBook, product, rowIndex);
            productNameToProduct[readOnlyProduct.Name] = readOnlyProduct;
        }

        return productNameToProduct;
    }

    private protected abstract TProduct GetProduct(ExcelCanteenBook canteenBook, CanteenProduct product, int rowIndex);
}

public class ReadOnlyCanteenBookAccountingProductCollection : CanteenBookAccountingProductCollectionBase<ReadOnlyExcelCanteenProduct>
{
    internal ReadOnlyCanteenBookAccountingProductCollection(ExcelCanteenBook canteenBook, int rowIndex) : base(canteenBook, rowIndex)
    {
    }

    private protected override ReadOnlyExcelCanteenProduct GetProduct(ExcelCanteenBook canteenBook, CanteenProduct product, int rowIndex)
    {
        xl.Range valueCell = product.GetConsumptionCell(canteenBook.Worksheet, rowIndex);
        return new(product, valueCell);
    }
}

/// <summary>
///     Коллекция остаточных прдуктов.
/// </summary>
public class RemaindCanteenBookAccountingProductCollection : CanteenBookAccountingProductCollectionBase<ReadOnlyExcelCanteenProduct>
{
    internal RemaindCanteenBookAccountingProductCollection(ExcelCanteenBook canteenBook, int rowIndex) : base (canteenBook, rowIndex)
    {
    }

    private protected override ReadOnlyExcelCanteenProduct GetProduct(ExcelCanteenBook canteenBook, CanteenProduct product, int rowIndex)
    {
        xl.Range remaindCell = product.GetRemaindCell(canteenBook.Worksheet, rowIndex);
        return new(product, remaindCell);
    }
}