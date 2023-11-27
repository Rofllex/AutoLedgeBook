using System.Linq;
using System.Collections;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Data;

/// <summary>
///      Коллекция накладных.
/// </summary>
/// <inheritdoc cref=""/>
public class ReadOnlyConsinmentNoteCollection : IReadOnlyCollection<IReadOnlyConsinmentNote>
{
    private readonly List<IReadOnlyConsinmentNote> _consinments;
    
    public ReadOnlyConsinmentNoteCollection(IEnumerable<IReadOnlyConsinmentNote> consinmentNotes)
    {
        _consinments = consinmentNotes.ToList();
    }

    public ReadOnlyConsinmentNoteCollection(IReadOnlyConsinmentNote consinmentNote) : this(new IReadOnlyConsinmentNote[] { consinmentNote })
    {
    }

    
    public int Count => _consinments.Count;

    public IEnumerator<IReadOnlyConsinmentNote> GetEnumerator() => _consinments.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
