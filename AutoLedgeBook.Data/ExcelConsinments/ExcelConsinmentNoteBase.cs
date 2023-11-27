﻿#nullable enable

using System;
using System.Linq;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Базовый класс накладной.
/// </summary>
/// <inheritdoc cref="IReadOnlyConsinmentNote"/>
public abstract class ExcelConsinmentNoteBase : IReadOnlyConsinmentNote
{
    public ExcelConsinmentNoteBase()
    {
    }

    public abstract string Number { get; }

    public abstract DateOnly Day { get; }

    public abstract IReadOnlyConsinmentDescription Description { get; }

    public abstract IAccountingProductCollection<IReadOnlyAccountingProduct> Products { get; }

    public virtual double GetTotalProductsPcs() => Products.Where(p => p.Units == ProductUnits.Pcs).Sum(p => p.Value);

    public virtual double GetTotalProductsWeight() => Products.Where(p => p.Units == ProductUnits.Kilo).Sum(p => p.Value);
}