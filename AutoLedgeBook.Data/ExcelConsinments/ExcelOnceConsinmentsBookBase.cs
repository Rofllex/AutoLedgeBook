using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AutoLedgeBook.Data.Abstractions;

#nullable enable

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Базовый класс книги накладных, содержащая всего одну накладную.
/// </summary>
public abstract class ExcelOnceConsinmentsBookBase : IReadOnlyConsinmentsBook
{
    private readonly Lazy<IReadOnlyConsinmentsCollection> _onceConsinmentNoteCollectionLazy;
    private readonly Lazy<IReadOnlyList<DateOnly>> _datesLazy;

    private bool _disposed = false;
    private readonly object _disposeMutex = new object();

    protected ExcelOnceConsinmentsBookBase()
    {
        _onceConsinmentNoteCollectionLazy = new Lazy<IReadOnlyConsinmentsCollection>(() => new OnceConsinmentsCollection(ConsinmentNote));
        _datesLazy = new Lazy<IReadOnlyList<DateOnly>>(() => ImmutableList.Create(Day));
    }

    public DateOnly Day => ConsinmentNote.Day;

    public abstract IReadOnlyConsinmentNote ConsinmentNote { get; }


    public virtual void Dispose()
    {
        lock (_disposeMutex)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ExcelOnceConsinmentsBookBase));
            DisposeProtected();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }

    #region interface implicit implementation

    IReadOnlyList<DateOnly> IReadOnlyConsinmentsBook.Dates => _datesLazy.Value;
    
    IReadOnlyConsinmentsCollection IReadOnlyConsinmentsBook.GetConsinmentsByDate(DateOnly day)
    {
        if (day != Day)
            throw new ArgumentException("Неверная дата");
        return _onceConsinmentNoteCollectionLazy.Value;
    }

    #endregion

    protected abstract void DisposeProtected();
}