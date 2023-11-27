#nullable enable

using System;
using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Учетный продукт в формате Excel`я.
/// </summary>
/// <inheritdoc cref="IReadOnlyAccountingProduct"
internal class ReadOnlyAccountingProduct : IReadOnlyAccountingProduct
{
    public ReadOnlyAccountingProduct(string name, double value, ProductUnits units)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        Name = name;
        Value = value;

        if (!Enum.IsDefined(units))
            throw new ArgumentException($"Значение \"{ units }\" не определено в перечислении \"{ nameof(ProductUnits) }\"", nameof(units));
        Units = units;
    }

    public string Name { get; }

    public double Value { get; }

    public ProductUnits Units { get; }
}