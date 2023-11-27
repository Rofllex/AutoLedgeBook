#nullable enable

using System.Collections;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Перечислитель накладных за один день.
///     Наследуется реализует интерфейс <see cref="IEnumerator{ExcelCanteenConsinmentConsumptionCollection}"/>
/// </summary>
public sealed class ExcelCanteenConsinmentConsumptionCollectionEnumerator : IEnumerator<ExcelCanteenConsinmentsCollection>
{
    private readonly ExcelCanteenBook _book;
    private readonly IEnumerator<DateOnly> _datesEnumerator;

    private ExcelCanteenConsinmentsCollection? _current = null;

    internal ExcelCanteenConsinmentConsumptionCollectionEnumerator(ExcelCanteenBook canteenBook) 
    {
        _book = canteenBook;
        _datesEnumerator = canteenBook.Dates.GetEnumerator();
    }

    public ExcelCanteenConsinmentsCollection Current => _current ?? throw new InvalidOperationException();

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        _datesEnumerator.Dispose();
    }

    public bool MoveNext()
    {
        if (!_datesEnumerator.MoveNext())
            return false;
        _current = _book.GetConsinmentsByDate(_datesEnumerator.Current);
        return true;
    }

    public void Reset()
    {
        _datesEnumerator.Reset();
        _current = null;
    }
}