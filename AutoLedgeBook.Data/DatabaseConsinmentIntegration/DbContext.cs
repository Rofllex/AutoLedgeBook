
using AutoLedgeBook.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;

namespace AutoLedgeBook.Data.DatabaseConsinmentIntegration;

internal class ConsinmentsBookDbContext : DbContext
{
    private readonly string _filePath;

    public ConsinmentsBookDbContext(string dbPath)
    {
        _filePath = dbPath;
    }

    
    public DbSet<DbProduct> Products { get; private set; }

    public DbSet<DbProductValue> ProductValues { get; private set; }

    public DbSet<DbConsinmentDescription> ConsinmentDescriptions { get; private set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={ _filePath }");
        
        base.OnConfiguring(optionsBuilder);
    }
}

public class SQLiteConsinmentsBook : IConsinmentsBook, ISaveable
{
    private readonly ConsinmentsBookDbContext _dbContext;

    public SQLiteConsinmentsBook(string filePath)
    {
        _dbContext = new ConsinmentsBookDbContext(filePath);
        _dbContext.Database.EnsureCreated();
    }



    public IConsinmentNote this[string consinmentNumber] => throw new NotImplementedException();

    public IReadOnlyList<DateOnly> Dates { get; private set; }

    public bool ContainsConsinment(string consinmentNumber)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public IConsinmentsCollection GetConsinmentsByDate(DateOnly date)
    {
        throw new NotImplementedException();
    }

    public string[] GetProductNames()
    {
        throw new NotImplementedException();
    }

    public void Save() => _dbContext.SaveChanges();

    public void Clear()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

    public DbConsinmentsCollection CreateConsinmentCollection(DateOnly date) => throw new NotImplementedException();


    IReadOnlyConsinmentsCollection IReadOnlyConsinmentsBook.GetConsinmentsByDate(DateOnly day)
    {
        throw new NotImplementedException();
    }
    
    void ISaveable.SaveAs(string _) => throw new NotSupportedException();

    void ISaveable.SaveAs(string _, ISaveSettings __) => throw new NotSupportedException();

}



public class DbConsinmentsCollection : IConsinmentsCollection
{
    public DbConsinmentsCollection()
    {

    }

    public IConsinmentNote this[string consinmentNumber] => throw new NotImplementedException();

    IReadOnlyConsinmentNote IReadOnlyConsinmentsCollection.this[string consinmentNumber] => throw new NotImplementedException();

    public int MaxCount => throw new NotImplementedException();

    public DateOnly Day => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public IConsinmentNote Add(string consinmentNumber)
    {
        throw new NotImplementedException();
    }

    public bool Contains(string consinmentNumber)
    {
        throw new NotImplementedException();
    }

    public void Delete(string consinmentNumber)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<IReadOnlyConsinmentNote> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator<IConsinmentNote> IEnumerable<IConsinmentNote>.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

public class DbConsinment : IConsinmentNote
{
    internal DbConsinment(string consinmentNumber, DbConsinmentDescription description)
    {
        if (string.IsNullOrWhiteSpace(consinmentNumber))
            throw new ArgumentNullException(nameof(consinmentNumber));

        Number = consinmentNumber;
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    

    public DbProductsCollection Products { get; private set; }

    public DbConsinmentDescription Description { get; private set; } 

    [Key, Required(AllowEmptyStrings = false)] public string Number { get; private set; }
    
    IConsinmentDescription IConsinmentNote.Description => Description;

    IReadOnlyConsinmentDescription IReadOnlyConsinmentNote.Description => Description;

    IAccountingProductCollection<IAccountingProduct> IConsinmentNote.Products => Products;

    IAccountingProductCollection<IReadOnlyAccountingProduct> IReadOnlyConsinmentNote.Products => Products;

    DateOnly IReadOnlyConsinmentNote.Day => throw new NotImplementedException();

    public double GetTotalProductsPcs() => GetSum(ProductUnits.Pcs);

    public double GetTotalProductsWeight() => GetSum(ProductUnits.Kilo);

    private double GetSum(ProductUnits units) => Products.Where(p => p.Product.Units == units).Sum(p => p.Value);
}

public class DbConsinmentDescription : IConsinmentDescription
{
    public static DbConsinmentDescription CreateEmpty(int personsCount) => new DbConsinmentDescription(String.Empty, string.Empty, personsCount);

    public DbConsinmentDescription(string destination, string type, int personsCount)
    {
        Destination = destination;
        Type = type;

        if (personsCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(personsCount), "Кол-во питающихся не может быть меньше 0");
        PersonsCount = personsCount;
    }

    private DbConsinmentDescription()
    {
    }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public uint Index { get; private set; }

    [Required(AllowEmptyStrings = true)] public string Destination { get; set; }
    
    [Required(AllowEmptyStrings = true)] public string Type { get; set; }

    [Required(AllowEmptyStrings = false)] public int PersonsCount { get; set; }

    string IReadOnlyConsinmentDescription.Destination => Destination;

    string IReadOnlyConsinmentDescription.Type => Type;

    int IReadOnlyConsinmentDescription.PersonsCount => PersonsCount;
}

public class DbProductsCollection : IAccountingProductCollection<DbProductValue>
{
    private Dictionary<string, DbProductValue> _products;

    public DbProductsCollection(IEnumerable<DbProductValue> products)
    {
        _products = products.ToDictionary(k => k.Product.Name);
    }

    public DbProductValue this[string productName]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentNullException(nameof(productName));

            if (_products.TryGetValue(productName, out DbProductValue product))
                return product;

            throw new InvalidOperationException($"Продукт с наименованием \"{ productName }\"");
        }
    }

    public int Count => _products.Count;

    public bool ContainsProduct(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentNullException(nameof(productName));
        return _products.ContainsKey(productName);
    }

    public IEnumerator<DbProductValue> GetEnumerator() => _products.Values.GetEnumerator();

    public string[] GetProductNames() => _products.Keys.ToArray();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class DbProductValue : IAccountingProduct
{
    public DbProductValue(DbProduct product, double value)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(product));

        Product = product;

        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Значение не может быть меньше 0.");

        Value = value;
    }

    private DbProductValue()
    {
    }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public uint Index { get; private set; }

    [Required] public DbProduct Product { get; private set; }

    public double Value { get; private set; }

    double IAccountingProduct.Value
    {
        get => Value;
        set => Value = value;
    }

    string IReadOnlyAccountingProduct.Name => Product.Name;

    ProductUnits IReadOnlyAccountingProduct.Units => Product.Units;
}

public class DbProduct
{
    public DbProduct(string name, ProductUnits units)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        Name = name;

        if (!Enum.IsDefined(units))
            throw new ArgumentException($"Значение { units } не предусмотрено перечислением { typeof(ProductUnits).FullName }", nameof(units));

        Units = units;
    }

    private DbProduct()
    {
    }

    [Key, Required(AllowEmptyStrings = false)] public string Name { get; private set; }

    [Required(AllowEmptyStrings = false)] public ProductUnits Units { get; private set; }
}