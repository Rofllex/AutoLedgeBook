using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Mock;

internal class MockConsinmentsBook : IConsinmentsBook
{
    private readonly IConsinmentsCollection _consinments;
    private readonly IEnumerable<IAccountingProduct> _products;

    public MockConsinmentsBook(IEnumerable<IConsinmentNote> consinments, IEnumerable<IAccountingProduct> products)
    {
        _consinments = new MockConsinmentsCollection(consinments, products);
        _products = products;
    }

    public IConsinmentNote this[string consinmentNumber] => _consinments[consinmentNumber];

    public IReadOnlyList<DateOnly> Dates => ((IEnumerable<IReadOnlyConsinmentNote>)_consinments).Select(c => c.Day).Distinct().ToList();

    public bool ContainsConsinment(string consinmentNumber)
        => _consinments.Contains(consinmentNumber);

    public void Dispose()
    {
    }

    public IConsinmentsCollection GetConsinmentsByDate(DateOnly day)
    {
        if (!Dates.Contains(day))
            throw new ArgumentException("Выбранная дата не найдена", nameof(day));
        var consinments = ((IEnumerable<IConsinmentNote>)_consinments).Where(c => c.Day == day);
        return new MockConsinmentsCollection(consinments, _products);
    }

    public string[] GetProductNames()
        => _products.Select(p => p.Name).ToArray();


    IReadOnlyConsinmentsCollection IReadOnlyConsinmentsBook.GetConsinmentsByDate(DateOnly day)
        => GetConsinmentsByDate(day);
}

internal sealed class MockConsinment : IConsinmentNote
{
    public MockConsinment(DateOnly day, string number, MockConsinmentDescription description, IEnumerable<IAccountingProduct> products)
    {
        Day = day;
        Number = number;
        Description = description;
        Products = new Data.AccountingProductCollection<IAccountingProduct>(products);
    }

    public IConsinmentDescription Description { get; }

    public IAccountingProductCollection<IAccountingProduct> Products { get; }

    public string Number { get; }

    public DateOnly Day { get; }

    IReadOnlyConsinmentDescription IReadOnlyConsinmentNote.Description => Description;

    IAccountingProductCollection<IReadOnlyAccountingProduct> IReadOnlyConsinmentNote.Products => Products;

    public double GetTotalProductsPcs()
        => GetProductsSum(Products, ProductUnits.Pcs);

    public double GetTotalProductsWeight()
        => GetProductsSum(Products, ProductUnits.Kilo);

    private double GetProductsSum(IEnumerable<IReadOnlyAccountingProduct> products, ProductUnits units)
        => products.Where(p => p.Units == units).Sum(p => p.Value);
}

internal sealed class MockConsinmentsCollection : IConsinmentsCollection
{
    private readonly List<IConsinmentNote> _consinments;
    private readonly IEnumerable<IAccountingProduct> _products;

    public MockConsinmentsCollection(IEnumerable<IConsinmentNote> consinments, IEnumerable<IAccountingProduct> products)
    {
        _consinments = new List<IConsinmentNote>(consinments ?? throw new ArgumentNullException(nameof(consinments)));
        _products = products ?? throw new ArgumentNullException(nameof(products));
    }

    public MockConsinmentsCollection(IEnumerable<IAccountingProduct> products) : this(Array.Empty<IConsinmentNote>(), products)
    {
    }

    public IConsinmentNote this[string consinmentNumber] 
    {
        get
        {
            if (string.IsNullOrWhiteSpace(consinmentNumber))
                throw new ArgumentNullException(nameof(consinmentNumber));

            IConsinmentNote findedConsinment = _consinments.FirstOrDefault(c => c.Number == consinmentNumber) ?? throw new ConsinmentNotFoundException(consinmentNumber);
            return findedConsinment;
        }
    }

    IReadOnlyConsinmentNote IReadOnlyConsinmentsCollection.this[string consinmentNumber] => this[consinmentNumber];

    public int MaxCount => int.MaxValue;

    public DateOnly Day { get; }

    public int Count => _consinments.Count;

    public IConsinmentNote Add(string consinmentNumber)
    {
        if (string.IsNullOrWhiteSpace(consinmentNumber))
            throw new ArgumentNullException(nameof(consinmentNumber));

        if (Contains(consinmentNumber))
            throw new InvalidOperationException($"Накладная с номером \"{ consinmentNumber }\" уже занята.");
        IEnumerable<IAccountingProduct> products = _products.Select(p => (IAccountingProduct)MockConsinmentProduct.FromAccountingProduct(p, false));
        MockConsinment mockConsinment = new MockConsinment(Day,
                                                           consinmentNumber,
                                                           new MockConsinmentDescription(),
                                                           products);
        return mockConsinment;
    }

    public bool Contains(string consinmentNumber)
        => _consinments.FirstOrDefault(c => c.Number == consinmentNumber) is not null;
    

    public void Delete(string consinmentNumber)
    {
        if (string.IsNullOrWhiteSpace(nameof(consinmentNumber)))
            throw new ArgumentNullException(nameof(consinmentNumber));
        
        IConsinmentNote? consinment = _consinments.FirstOrDefault(c => c.Number == consinmentNumber);
        if (consinment is null)
            throw new ConsinmentNotFoundException(consinmentNumber);
        _consinments.Remove(consinment);
    }

    public IEnumerator<IReadOnlyConsinmentNote> GetEnumerator()
        => _consinments.GetEnumerator();

    IEnumerator<IConsinmentNote> IEnumerable<IConsinmentNote>.GetEnumerator()
        => _consinments.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}


internal struct MockConsinmentDescription : IConsinmentDescription
{
    public MockConsinmentDescription(string destination, string type, int personsCount)
        => (Destination, Type, PersonsCount) = (destination, type, personsCount);

    public MockConsinmentDescription() : this(string.Empty, string.Empty, default) { }

    public string Destination { get; set; }
    
    public string Type { get; set; }
    
    public int PersonsCount { get; set; }

    string IReadOnlyConsinmentDescription.Destination => Destination;

    string IReadOnlyConsinmentDescription.Type => Type;

    int IReadOnlyConsinmentDescription.PersonsCount => PersonsCount;
}


internal struct MockConsinmentProduct : IAccountingProduct
{
    public static MockConsinmentProduct FromAccountingProduct(IReadOnlyAccountingProduct product, bool copyValue = false)
        => new(product.Name, copyValue ? product.Value : 0, product.Units);
    
    public static IEnumerable<MockConsinmentProduct> FromEnumerable(IEnumerable<IReadOnlyAccountingProduct> products, bool copyValue = false)
        => products.Select(p => FromAccountingProduct(p, copyValue));
    

    public MockConsinmentProduct(string name, double value, ProductUnits units)
    {
        Name = name;
        Value = value;
        Units = units;
    }

    public double Value { get; set; }

    public string Name { get; }

    public ProductUnits Units { get; }

    double IReadOnlyAccountingProduct.Value => Value;
}

