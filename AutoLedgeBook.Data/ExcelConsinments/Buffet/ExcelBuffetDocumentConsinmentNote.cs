#nullable enable

using System;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.Extensions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments.Buffet;

/// <inheritdoc cref="IReadOnlyConsinmentNote"/>
public class ExcelBuffetDocumentConsinmentNote : IReadOnlyConsinmentNote
{
    private readonly xl.Range _productsRange;

    private xl.Range? _totalProductsWeightCell,
                        _totalProductsPcsCell;

    internal ExcelBuffetDocumentConsinmentNote(xl.Worksheet consinmentWorksheet)
    {
        Description = new ReadOnlyExcelDocumentConsinmentNoteDescription(consinmentWorksheet);
        Worksheet = consinmentWorksheet;

        (_totalProductsWeightCell, _totalProductsPcsCell) = GetControlCells(consinmentWorksheet);


        //_productsRange = consinmentWorksheet.Range["A17:H149"].Rows;
        _productsRange = GetProductsRange(consinmentWorksheet);

        Products = new ExcelDocumentConsinmentProductCollection(_productsRange);
        UpdateWeight();
    }

    /// <inheritdoc cref="IReadOnlyConsinmentNote.Header"/>
    public ReadOnlyExcelDocumentConsinmentNoteDescription Description { get; }


    public IAccountingProductCollection<IReadOnlyAccountingProduct> Products { get; }

    public string Number => Description.Number;

    public DateOnly Day => Description.Date;



    public double GetTotalProductsPcs() => TotalProductsPcs;

    public double GetTotalProductsWeight() => TotalProductsWeight;


    #region interface implicit implementation

    IReadOnlyConsinmentDescription IReadOnlyConsinmentNote.Description => Description;

    #endregion



    /// <summary>
    ///     Метод обновления веса(локально)
    /// </summary>
    protected virtual void UpdateWeight()
    {
        // TODO: пофиксить если накладная битая...TODO!!!!

        if (_totalProductsWeightCell?.Value != null)
        {
            try
            {
                TotalProductsWeight = Convert.ToDouble(_totalProductsWeightCell.Value);
            }
            catch
            {
                TotalProductsWeight = double.NaN;
            }
        }

        if (_totalProductsPcsCell?.Value != null)
        {
            try
            {
                TotalProductsPcs = Convert.ToDouble(_totalProductsPcsCell.Value);
            }
            catch
            {
                TotalProductsPcs = double.NaN;
            }
        }
    }

    private (xl.Range? totalWeightCell, xl.Range? totalPcsCell) GetControlCells(in xl.Worksheet worksheet)
    {
        xl.Range? findedCell = worksheet.Cells.Find(What: "Всего");
        if (findedCell is null)
            return (null, null);

        (int Column, int Row) firstFindedCellPos = (findedCell.Column, findedCell.Row);
        xl.Range? totalWeightCell = null,
                  totalPcsCell = null;
        do
        {
            string? currentCellValue = Convert.ToString(findedCell.Value);
            if (currentCellValue?.Trim() == "Всего")
            {
                totalWeightCell = findedCell.Offset[0, 1];
                totalPcsCell = findedCell.Offset[1, 1];

                findedCell.Release();
                break;
            }

            findedCell = worksheet.Cells.Find(What: "Всего", After: findedCell);
        } while (findedCell.Column != firstFindedCellPos.Column && findedCell.Row != firstFindedCellPos.Row);

        return (totalWeightCell, totalPcsCell);
    }

    /// <summary>
    ///     Текущий рабочий лист
    /// </summary>
    internal xl.Worksheet Worksheet { get; }


    protected double TotalProductsWeight { get; set; }
    protected double TotalProductsPcs { get; set; }



    private xl.Range GetProductsRange(xl.Worksheet consinmentWorksheet)
    {
        xl.Range patternCell = consinmentWorksheet.UsedRange.Find(What: "№ п/п");


        xl.Range startProductsRange = patternCell.Offset[1, 0];
        xl.Range endProductsRange = startProductsRange.End[xl.XlDirection.xlDown];
        endProductsRange = consinmentWorksheet.Range[$"H{endProductsRange.Row}"];
        return consinmentWorksheet.Range[startProductsRange, endProductsRange];
    }
}
