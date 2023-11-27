#nullable enable

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Перечислитель Excel продукта.
/// </summary>
/// <typeparam name="TProduct"></typeparam>
public abstract class ExcelProductsEnumerator<TProduct> : IEnumerator<TProduct> where TProduct : class, IReadOnlyAccountingProduct
{
    private readonly IEnumerator<xl.Range> _currentRowEnumerator;
    private TProduct? _current;

    /// <summary>
    ///     Конструктор перечислителя продуктов.
    /// </summary>
    /// <param name="productsRange">Диапазон продуктов</param>
    public ExcelProductsEnumerator(xl.Range productsRange)
    {
        _currentRowEnumerator = productsRange.Rows.Cast<xl.Range>().GetEnumerator();
    }

    public TProduct Current => _current ?? throw new InvalidOperationException($"Перед использованием необходимо вызвать метод \"{ nameof(MoveNext) }\"");

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        _currentRowEnumerator.Dispose();
        DisposeProtected();
    }

    public bool MoveNext()
    {
        if (!_currentRowEnumerator.MoveNext())
            return false;
        xl.Range currentRow = _currentRowEnumerator.Current;

        if (!IsValidProduct(currentRow) || GetProductValue(currentRow) == 0)
            return MoveNext();
        _current = Build(currentRow);
        return true;
    }

    public void Reset()
    {
        _currentRowEnumerator.Reset();
        _current = null;
    }

    protected abstract TProduct Build(xl.Range productRow);

    protected abstract double GetProductValue(xl.Range productRow);

    /// <summary>
    ///     Проверка на валидность продукта.
    /// </summary>
    /// <param name="productRow">Строка продукта.</param>
    /// <returns></returns>
    protected virtual bool IsValidProduct(xl.Range productRow)
    {
        object firstCellValue = productRow.Cells[1, 1].Value;
        if (firstCellValue is null)
            return false;
        int productIndex = Convert.ToInt32(firstCellValue);
        return productIndex > 0;
    }

    /// <summary>
    ///     Освободить ресурсы подкласса.
    /// </summary>
    protected virtual void DisposeProtected() { }
}
