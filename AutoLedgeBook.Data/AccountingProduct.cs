using System;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Data;

/// <summary>
///     Класс продукта по умолчанию.
/// </summary>
/// <inheritdoc cref="IAccountingProduct"/>
public sealed class AccountingProduct : IAccountingProduct
{
    private double _value;
    
    public AccountingProduct(string name, double value, ProductUnits units)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
        Value = value;
        Units = Enum.IsDefined(units) ? units : throw new ArgumentException("Неверное значение аргумента", nameof(units));
    }


    public string Name { get; }

    public double Value
    {
        get => _value;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Значение веса не может быть отрицательным", nameof(value));
            _value = value;
        }
    }

    public ProductUnits Units { get; }
}