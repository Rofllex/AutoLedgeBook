using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.LowLevel;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Коллекция накладных столовой.
///     Реализует интерфейс <see cref="IConsinmentNoteCollection"/>
/// </summary>
/// <inheritdoc cref="IConsinmentsCollection"/>
public class ExcelCanteenConsinmentsCollection : IConsinmentsCollection, IEnumerable<ExcelCanteenConsinment>
{
    private readonly xl.Range _consinmentsRange;
    private readonly ExcelCanteenBook _parentBook;
    private readonly Lazy<RemaindCanteenBookAccountingProductCollection> _remainProducts;
    private List<ExcelCanteenConsinment> _consinmentsList = new();

    internal ExcelCanteenConsinmentsCollection(ExcelCanteenBook parentBook, DateOnly day, xl.Range consinmentsRange)
    {
        Day = day;

        Debug.Assert(consinmentsRange is not null);
        _consinmentsRange = consinmentsRange;

        Debug.Assert(parentBook is not null);
        _parentBook = parentBook;

        _consinmentsList = GetConsinments(new ExcelCanteenConsinmentEnumerator(_parentBook, Day, _consinmentsRange));
        _remainProducts = new(GetRemaindProducts);
    }

    public int MaxCount => 32;

    public int Count => 32;

    public DateOnly Day { get; }

    public ExcelCanteenConsinment this[string consinmentNumber]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(consinmentNumber))
                throw new ArgumentNullException(nameof(consinmentNumber));
            xl.Range? findedCell = _consinmentsRange.FindCellByValue(consinmentNumber);
            if (findedCell is null)
                throw new ConsinmentNotFoundException(consinmentNumber);
            return _parentBook.GetConsinmentByRow(findedCell.Row);
        }
    }

    public RemaindCanteenBookAccountingProductCollection RemaindProducts => _remainProducts.Value;

    public bool Contains(string consinmentNumber)
    {
        xl.Range? findedCell = _consinmentsRange.FindCellByValue(consinmentNumber);
        bool contains = findedCell is not null;
        if (contains)
            Marshal.ReleaseComObject(findedCell!);

        return contains;
    }

    public void Delete(string consinmentNumber)
    {
        if (string.IsNullOrWhiteSpace(consinmentNumber))
            throw new ArgumentNullException(nameof(consinmentNumber));

        this[consinmentNumber].Delete();
    }
    
    public IEnumerator<ExcelCanteenConsinment> GetEnumerator() => _consinmentsList.GetEnumerator();

    #region interfaces implicit implementation

    IReadOnlyConsinmentNote IReadOnlyConsinmentsCollection.this[string consinmentNumber] => this[consinmentNumber];

    IConsinmentNote IConsinmentsCollection.Add(string _) => throw new NotSupportedException("Добавление новых накладных не поддерживается.");

    IConsinmentNote IConsinmentsCollection.this[string consinmentNumber] => this[consinmentNumber];

    IEnumerator<IConsinmentNote> IEnumerable<IConsinmentNote>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<IReadOnlyConsinmentNote> IEnumerable<IReadOnlyConsinmentNote>.GetEnumerator() => GetEnumerator();

    #endregion

    private List<ExcelCanteenConsinment> GetConsinments(IEnumerator<ExcelCanteenConsinment> enumerator)
    {
        List<ExcelCanteenConsinment> consinmentsList = new();
        if (enumerator.MoveNext())
        {
            do
            {
                consinmentsList.Add(enumerator.Current);
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (enumerator.MoveNext());
        }

        return consinmentsList;
    }

    private RemaindCanteenBookAccountingProductCollection GetRemaindProducts()
    {
        if (_parentBook.Dates[_parentBook.Dates.Count - 1] == Day)
            return new RemaindCanteenBookAccountingProductCollection(_parentBook, ExcelCanteenBook.LOWER_CELLS_BORDER_ROW);
        int nextDateRowIndex = _parentBook.RowIndexToDateDictionary.First(p => p.Value > Day).Key;
        return new RemaindCanteenBookAccountingProductCollection(_parentBook, nextDateRowIndex);
    }
}

