using System.Collections;
using System.Diagnostics;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;

/// <summary>
///     Коллекция накладных книги склада.
/// </summary>
/// <inheritdoc cref="IConsinmentsCollection"/>
/// <inheritdoc cref="IConsinmentsCollectionStatistics"/>
public class ExcelLedgeConsinmentConsumptionCollection : IConsinmentsCollection, IConsinmentsCollectionStatistics
{
    private readonly ExcelStorageBook _parentBook;
    private readonly IEnumerable<xl.Range> _headerCells;
    private readonly Dictionary<string, ExcelLedgeConsinment> _consinmentNumberToConsinment;

    internal ExcelLedgeConsinmentConsumptionCollection(ExcelStorageBook parentBook, DateOnly day, xl.Range headersRange)
    {
        Debug.Assert(headersRange is not null);
        _parentBook = parentBook;
        
        Day = day;
                
        _headerCells = headersRange.Cast<xl.Range>();
        List<xl.Range>? cellsList = _headerCells.ToList();
        MaxCount = _headerCells.Count();

        _consinmentNumberToConsinment = GetConsinments(_headerCells).ToDictionary(k => k.Number);
        _consinmentNumberToConsinment.EnsureCapacity(MaxCount);

        RemainProducts = parentBook.GetRemainProducsByDate(Day);
        TotalProductsConsumption = parentBook.GetProductsTotalConsumption(Day);
    }

    public int MaxCount { get; }

    public DateOnly Day { get; }

    public int Count => _consinmentNumberToConsinment.Count;


    public ExcelLedgeConsinment this[string consinmentNumber] 
    {
        get
        {
            if (string.IsNullOrWhiteSpace(consinmentNumber))
                throw new ArgumentNullException(nameof(consinmentNumber));

            ExcelLedgeConsinment? ledgeConsinment;
            if (_consinmentNumberToConsinment.TryGetValue(consinmentNumber, out ledgeConsinment))
                return ledgeConsinment!;

            throw new ConsinmentNotFoundException(consinmentNumber);
        }

        private set
        {
            _consinmentNumberToConsinment[consinmentNumber] = value;
        }
    }

    /// <inheritdoc cref="IConsinmentsCollectionStatistics.RemainProducts"/>
    public ReadOnlyExcelLedgeAccountingProductCollection RemainProducts { get; }

    /// <inheritdoc cref="IConsinmentsCollectionStatistics.TotalProductsConsumption"/>
    public ReadOnlyExcelLedgeAccountingProductCollection TotalProductsConsumption { get; }

    /// <summary>
    ///     Добавить накладную
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException">При нехватке места для новой накладной или если накладная уже существует в книге.</exception>
    public ExcelLedgeConsinment Add(string consinmentNumber)
    {
        if (string.IsNullOrWhiteSpace(consinmentNumber))
            throw new ArgumentNullException(nameof(consinmentNumber));

        if (Count == MaxCount)
            throw new InvalidOperationException("Нет места для новой накладной");

        xl.Range? headerCell = _headerCells.FirstOrDefault(c => c.Value == null || string.IsNullOrWhiteSpace(Convert.ToString(c.Value)));
        if (headerCell is null)
            throw new InvalidOperationException("Нет места для новой накладной");

        if (Contains(consinmentNumber))
            throw new InvalidOperationException($"Накладная с номером \"{ consinmentNumber }\" уже существует");

        ExcelLedgeConsinment consinmentConsumption = new ExcelLedgeConsinment(_parentBook, Day, headerCell.Column);
        consinmentConsumption.Description.Number = consinmentNumber;
        this[consinmentNumber] = consinmentConsumption;
        
        return consinmentConsumption;

    }

    public bool Contains(string consinmentNumber)
    {
        if (string.IsNullOrWhiteSpace(consinmentNumber))
            throw new ArgumentNullException(nameof(consinmentNumber));
        
        return _consinmentNumberToConsinment.ContainsKey(consinmentNumber);
    }

    public void Delete(string consinmentNumber)
    {
        if (string.IsNullOrWhiteSpace(consinmentNumber))
            throw new ArgumentNullException(nameof(consinmentNumber));
        
        if (!_consinmentNumberToConsinment.TryGetValue(consinmentNumber, out ExcelLedgeConsinment? consinment))
            throw new ConsinmentNotFoundException(consinmentNumber);

        consinment.Clear();
        _consinmentNumberToConsinment.Remove(consinmentNumber);
    }

    /// <summary>
    ///     Удалить накладную
    /// </summary>
    /// <param name="consinment">Номер накладной</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <inheritdoc cref="IConsinmentsCollection.Delete(string)"/>
    public void Delete(ExcelLedgeConsinment consinment)
    {
        if (consinment is null)
            throw new ArgumentNullException(nameof(consinment));

        Delete(consinment.Number);
    }

    public IEnumerator<IConsinmentNote> GetEnumerator() => _consinmentNumberToConsinment.Values.GetEnumerator();

    #region interfaces implicit implementation

    IAccountingProductCollection<IReadOnlyAccountingProduct> IConsinmentsCollectionStatistics.RemainProducts => RemainProducts;

    IAccountingProductCollection<IReadOnlyAccountingProduct> IConsinmentsCollectionStatistics.TotalProductsConsumption => TotalProductsConsumption;

    IReadOnlyConsinmentNote IReadOnlyConsinmentsCollection.this[string consinmentNumber] => this[consinmentNumber];
    
    IConsinmentNote IConsinmentsCollection.this[string consinmentNumber] => this[consinmentNumber];

    IConsinmentNote IConsinmentsCollection.Add(string consinmentNumber) => Add(consinmentNumber);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    IEnumerator<IReadOnlyConsinmentNote> IEnumerable<IReadOnlyConsinmentNote>.GetEnumerator() => GetEnumerator();

    #endregion

    private ExcelLedgeConsinment CreateConsinment(int columnIndex) => new ExcelLedgeConsinment(_parentBook, Day, columnIndex);

    private List<ExcelLedgeConsinment> GetConsinments(IEnumerable<xl.Range> headerCells)
    {
        return headerCells.Where(c => (c.Value is not null) && (!string.IsNullOrEmpty(Convert.ToString(c.Value))))
                               .Select(c => CreateConsinment(c.Column))
                               .ToList();
    }
}