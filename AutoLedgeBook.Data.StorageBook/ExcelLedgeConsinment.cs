using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;

/// <inheritdoc cref="IReadOnlyConsinmentNote"/>
public class ExcelLedgeConsinment : IConsinmentNote
{
    private const int TOTAL_PCS_ROW_INDEX = 129;
    private const int TOTAL_WEIGHT_ROW_INDEX = 128;

    private readonly xl.Range _totalPcsCell,
                                _totalWeightCell;
    private readonly ExcelStorageBook _parentBook;

    internal ExcelLedgeConsinment(ExcelStorageBook parentBook, DateOnly day, int columnIndex)
    {
        _parentBook = parentBook;
        Day = day;

        Products = new ExcelLedgeAccountingProductCollection(parentBook, columnIndex);
        Description = new ExcelLedgeConsinmentDescription(date: day,
                                                            headerCell: parentBook.GetConsinmentHeaderCell(columnIndex));
        (_totalWeightCell, _totalPcsCell) = parentBook.GetControlCells(columnIndex);
    }

    public ExcelLedgeConsinmentDescription Description { get; private set; }

    public string Number => Description.Number;

    public DateOnly Day { get; }

    public ExcelLedgeAccountingProductCollection Products { get; }

    
    #region implicit interface implementation
    IReadOnlyConsinmentDescription IReadOnlyConsinmentNote.Description => Description;

    IAccountingProductCollection<IReadOnlyAccountingProduct> IReadOnlyConsinmentNote.Products => Products;
    
    IAccountingProductCollection<IAccountingProduct> IConsinmentNote.Products => Products;

    IConsinmentDescription IConsinmentNote.Description => Description;
    #endregion

    public double GetTotalProductsPcs() => Convert.ToDouble(_totalPcsCell.Value);
    
    public double GetTotalProductsWeight() => Convert.ToDouble(_totalWeightCell.Value);
    
    public void Clear()
    {
        Description.Clear();
        Products.ClearAll();
    }
}

