using System.Collections;
using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;

public abstract class ExcelProductValueEnumeratorBase<T> : IEnumerator<T> where T : class
{
    public ExcelProductValueEnumeratorBase(xl.Range productValuesRange)
    {
        _productValueCellEnumerator = productValuesRange.Cast<xl.Range>().GetEnumerator();
    }

    public T Current => _current ?? throw new InvalidOperationException($"Перед использованием необходимо вызвать метод \"{ nameof(MoveNext) }\"");

    object IEnumerator.Current => Current;

    public abstract void Dispose();

    public bool MoveNext()
    {
        if (!_productValueCellEnumerator.MoveNext())
            return false;
        xl.Range currentValueCell = _productValueCellEnumerator.Current;
        T? product = CreateByValueCell(currentValueCell);
        if (product is null)
            return MoveNext();
        _current = product;
        return true;
    }

    public void Reset()
    {
        _current = null;
        _productValueCellEnumerator.Reset();
    }

    protected abstract T? CreateByValueCell(xl.Range valueCell);

    private T? _current;
    private readonly IEnumerator<xl.Range> _productValueCellEnumerator;
}
