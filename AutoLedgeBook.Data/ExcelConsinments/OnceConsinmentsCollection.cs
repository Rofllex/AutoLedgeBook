using System;
using System.Collections;
using System.Collections.Generic;
using AutoLedgeBook.Data.Abstractions;

#nullable enable

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Коллекция накладных представляющая всего одну накладную.
/// </summary>
public class OnceConsinmentsCollection : IReadOnlyConsinmentsCollection
{
    private readonly IReadOnlyList<IReadOnlyConsinmentNote> _consinments;

    internal OnceConsinmentsCollection(IReadOnlyConsinmentNote consinment)
    {
        _consinments = new IReadOnlyConsinmentNote[] { consinment };
    }

    public IReadOnlyConsinmentNote this[string consinmentNumber]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(consinmentNumber))
                throw new ArgumentNullException(nameof(consinmentNumber));
            if (_consinments[0].Number != consinmentNumber)
                throw new ConsinmentNotFoundException(consinmentNumber);
            return _consinments[0];
        }
    }

    public DateOnly Day => _consinments[0].Day;

    public int Count => _consinments.Count;

    public bool Contains(string consinmentNumber) => _consinments[0].Number == consinmentNumber;


    public IEnumerator<IReadOnlyConsinmentNote> GetEnumerator() => _consinments.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
