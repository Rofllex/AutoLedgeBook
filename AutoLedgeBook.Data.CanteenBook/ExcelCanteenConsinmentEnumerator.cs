using System.Collections;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Перечислитель накладных книги столовой.
/// </summary>
public class ExcelCanteenConsinmentEnumerator : IEnumerator<ExcelCanteenConsinment>
{
    private readonly IEnumerator<xl.Range> _consinmentRowsEnumerator;
    private readonly ExcelCanteenBook _canteenBook;
    private readonly DateOnly _day;

    private ExcelCanteenConsinment? _current;

    internal ExcelCanteenConsinmentEnumerator(ExcelCanteenBook canteenBook, DateOnly day, xl.Range consinmentRows)
    {
        _consinmentRowsEnumerator = consinmentRows.Rows.Cast<xl.Range>().GetEnumerator();
        _canteenBook = canteenBook;
        _day = day;
    }

    public event Action<ExcelCanteenConsinment> ConsinmentCreated = _ => { };

    public ExcelCanteenConsinment Current => _current ?? throw new InvalidOperationException($"Перед использованием данного метода необходимо вызвать метод { nameof(MoveNext) }");
    
    object IEnumerator.Current => Current;

    public void Dispose() => _consinmentRowsEnumerator.Dispose();
    
    public bool MoveNext()
    {
        if (!_consinmentRowsEnumerator.MoveNext())
            return false;
        xl.Range currentRow = _consinmentRowsEnumerator.Current;
        if (!ExcelCanteenConsinment.CanInitialize(currentRow))
            return MoveNext();
        _current = _canteenBook.GetConsinmentByRow(currentRow.Row);
        InvokeConsinmentCreated(_current);
        Thread.Sleep(TimeSpan.FromMilliseconds(10));
        return true;
    }

    public void Reset()
    {
        _consinmentRowsEnumerator.Reset();
        _current = null;
    }

    private void InvokeConsinmentCreated(ExcelCanteenConsinment consinment) => ConsinmentCreated(consinment);
}
 
