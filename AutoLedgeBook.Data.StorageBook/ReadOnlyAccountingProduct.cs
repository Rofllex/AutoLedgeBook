using AutoLedgeBook.Data.Abstractions;


namespace AutoLedgeBook.Data.StorageBook;

public record class ReadOnlyAccountingProduct : IReadOnlyAccountingProduct
{
    public static ReadOnlyAccountingProduct FromAccountingProduct(IAccountingProduct product)
        => new(product.Name, product.Value, product.Units);

    public static IEnumerable<ReadOnlyAccountingProduct> FromEnumerable(IEnumerable<IAccountingProduct> enumerable)
        => enumerable.Select(p => FromAccountingProduct(p));

    public ReadOnlyAccountingProduct(string name, double value, ProductUnits units)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));
        
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));
        
        if (!Enum.IsDefined(units))
            throw new ArgumentException(nameof(units));
        
        Name = name;
        Value = value;
        Units = units;
    }

    public string Name { get; }

    public double Value { get; }

    public ProductUnits Units { get; }
}