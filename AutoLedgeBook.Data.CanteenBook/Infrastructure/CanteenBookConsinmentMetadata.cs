using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook.Infrastructure;

/// <summary>
///     Модель, представляющая метаданные накладной
/// </summary>
internal class CanteenBookConsinmentMetadata
{
    private 
        xl.Range _rowIndexCell,
                                 _consinmentNumberCell,
                                 _personsCountCell,
                                 _typeCell,
                                 _destinationCell;

    public CanteenBookConsinmentMetadata(xl.Range consinmentNumberCell, xl.Range personsCountCell, xl.Range typeCell, xl.Range destinationCell, xl.Range rowIndexCell)
    {
        _rowIndexCell = rowIndexCell;
        _consinmentNumberCell = consinmentNumberCell;
        _personsCountCell = personsCountCell;
        _typeCell = typeCell;
        _destinationCell = destinationCell;
    }

    public CanteenBookConsinmentMetadata(string consinmentNumber, xl.Range consinmentNumberCell, xl.Range personsCountCell, xl.Range typeCell, xl.Range destinationCell, xl.Range rowIndexCell)
        : this(consinmentNumberCell: consinmentNumberCell, personsCountCell: personsCountCell, typeCell: typeCell, destinationCell: destinationCell, rowIndexCell: rowIndexCell)
    {
        ConsinmentNumber = consinmentNumber;
    }

    public int RowIndex
    {
        get => _rowIndexCell.Value;
        set => ChangeCell(_rowIndexCell, value);
    }

    public string ConsinmentNumber
    {
        get => Convert.ToString(_consinmentNumberCell.Value) ?? string.Empty;
        private set => ChangeCell(_consinmentNumberCell, value);
    }

    public int PersonsCount
    {
        get
        {
            object personsCountCellValue = _personsCountCell.Value;
            if (personsCountCellValue is null)
                return default;
            return Convert.ToInt32((double)personsCountCellValue);
        }
        
        set => ChangeCell(_personsCountCell, value);
    }

    public string Type
    {
        get => Convert.ToString(_typeCell.Value) ?? string.Empty;
        set => ChangeCell(_typeCell, value);
    }

    public string Destination
    {
        get => Convert.ToString(_destinationCell.Value) ?? string.Empty;
        set => ChangeCell(_destinationCell, value);
    }

    public void Delete()
    {
        int row = _rowIndexCell.Row;
        _ = _rowIndexCell.Worksheet.Range[$"{row}:{row}"].Delete(xl.XlDeleteShiftDirection.xlShiftUp);
    }

    private void ChangeCell(xl.Range cell, object value)
        => cell.Value = value;
}
