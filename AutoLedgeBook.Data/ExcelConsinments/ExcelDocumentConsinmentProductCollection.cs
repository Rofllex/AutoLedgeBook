#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <inheritdoc cref="IReadOnlyAccountingProductCollection"/>
public class ExcelDocumentConsinmentProductCollection : IAccountingProductCollection<ExcelDocumentAccountingProduct>
{
    private readonly IReadOnlyList<ExcelDocumentAccountingProduct> _products;

    public ExcelDocumentConsinmentProductCollection(xl.Range productsRange)
    {
        _products = GetProducts(productsRange);
    }


    public ExcelDocumentAccountingProduct this[string productName] => _products.FirstOrDefault(p => p.Name == productName) ?? throw new Exception($"Продукт с названием \"{ productName }\" не найден");

    public int Count => _products.Count;

    public string[] GetProductNames() => this.Select(p => p.Name).ToArray();

    public bool ContainsProduct(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentNullException(nameof(productName));
        return GetProductNames().Contains(productName);
    }

    public IEnumerator<ExcelDocumentAccountingProduct> GetEnumerator() => _products.GetEnumerator();

    #region implicit interface implementations

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    private List<ExcelDocumentAccountingProduct> GetProducts(xl.Range productsRange)
    {
        using ExcelDocumentAccountingProductEnumerator productsEnumerator = new(productsRange);
        List<ExcelDocumentAccountingProduct> productsList = new();
        while (productsEnumerator.MoveNext())
            productsList.Add(productsEnumerator.Current);
        return productsList;
    }
}


/// <summary>
///     Перечислитель продуктов в Excel документе по умолчанию.
///     <br />
///     Перечисляет и парсит строки продуктов в следующем порядке:
///     <br /><b>B1</b> - наименование продукта
///     <br /><b>C1</b> - единицы измерения продукта
///     <br /><b>H1</b> - значение продукта
/// </summary>
internal class ExcelDocumentAccountingProductEnumerator : ExcelProductsEnumerator<ExcelDocumentAccountingProduct>
{
    public ExcelDocumentAccountingProductEnumerator(xl.Range productsRange) : base(productsRange)
    {
    }

    protected override ExcelDocumentAccountingProduct Build(xl.Range productRow)
    {
        string name = Convert.ToString(productRow.Range["B1"].Value);
        string unitsString = Convert.ToString(productRow.Range["C1"].Value);
        ProductUnits units = ParseUnits(unitsString);
        return new ExcelDocumentAccountingProduct(name, productRow.Range["H1"], units);
    }

    protected override double GetProductValue(xl.Range productRow)
    {
        object cellValue = productRow.Range["H1"].Value;
        if (cellValue is null || string.IsNullOrWhiteSpace(Convert.ToString(cellValue)))
            return 0;
        return Convert.ToDouble(cellValue);
    }

    private ProductUnits ParseUnits(string unitsString)
    {
        string normalizedUnitsString = new string(unitsString.Where(c => char.IsLetter(c)).ToArray());
        
        return normalizedUnitsString switch
        {
            "кг" => ProductUnits.Kilo,
            "л" => ProductUnits.Kilo,
            "шт" => ProductUnits.Pcs,
            _ => throw new InvalidDataException()
        };
    }
}
