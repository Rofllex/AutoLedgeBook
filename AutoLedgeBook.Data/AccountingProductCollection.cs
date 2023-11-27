using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;

#nullable enable

namespace AutoLedgeBook.Data;

/// <summary>
///     Базовый класс коллекции продуктов.
/// </summary>
/// <typeparam name="TProduct"></typeparam>
public abstract class ProductCollectionBase<TProduct> : IAccountingProductCollection<TProduct> where TProduct : class
{
    private string[]? _productNames;

    public ProductCollectionBase(IEnumerable<TProduct> productsCollection)
    {
        ProductsCollection = productsCollection ?? throw new ArgumentNullException(nameof(productsCollection));
        Count = ProductsCollection.Count();
    }

    public TProduct this[string productName]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentNullException(nameof(productName));

            TProduct? product = GetProduct(ProductsCollection, productName);
            if (product is null)
                throw new InvalidOperationException($"Продукта с наименованием \"{productName}\" не существует в данной коллекции");
            return product;
        }
    }

    public virtual int Count { get; }

    public bool ContainsProduct(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentNullException(nameof(productName), "Наименование продукта не может быть пустым или состоять только из пробелов");
        return GetProductNames().Contains(productName);
    }

    public virtual IEnumerator<TProduct> GetEnumerator() => ProductsCollection.GetEnumerator();

    public string[] GetProductNames()
    {
        if (_productNames is null)
            _productNames = GetProductNames(ProductsCollection);
        return _productNames;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected readonly IEnumerable<TProduct> ProductsCollection;

    protected abstract TProduct? GetProduct(IEnumerable<TProduct> productCollection, string productName);
    
    /// <summary>
    ///     Метод получения наименования продуктов.
    /// </summary>
    /// <param name="productEnumerable">Перечисление продуктов</param>
    /// <returns></returns>
    protected abstract string[] GetProductNames(IEnumerable<TProduct> productEnumerable);
}

/// <summary>
///     Коллекция продуктов с типом <see cref="IReadOnlyAccountingProduct"/>
/// </summary>
/// <typeparam name="TProduct"></typeparam>
public class ReadOnlyAccountingProductCollection<TProduct> : ProductCollectionBase<TProduct> where TProduct : class, IReadOnlyAccountingProduct
{
    public ReadOnlyAccountingProductCollection(IEnumerable<TProduct> products) : base(products)
    {

    }


    protected override TProduct? GetProduct(IEnumerable<TProduct> productCollection, string productName) => productCollection.FirstOrDefault(p => p.Name == productName);
    
    protected override string[] GetProductNames(IEnumerable<TProduct> productCollection)  => productCollection.Select(p => p.Name).ToArray();
}

/// <summary>
///     Коллекция продуктов с типом <see cref="IAccountingProduct"/>
/// </summary>
/// <typeparam name="TProduct"></typeparam>
public class AccountingProductCollection<TProduct> : ProductCollectionBase<TProduct> where TProduct : class, IAccountingProduct
{
    public AccountingProductCollection(IEnumerable<TProduct> productCollection) : base(productCollection)
    {

    }

    protected override TProduct? GetProduct(IEnumerable<TProduct> productCollection, string productName) => productCollection.FirstOrDefault(p => p.Name == productName);

    protected override string[] GetProductNames(IEnumerable<TProduct> productCollection) => productCollection.Select(p => p.Name).ToArray();
}
