using AutoLedgeBook.Data.Excel.LowLevel;

using xl = Microsoft.Office.Interop.Excel;


namespace AutoLedgeBook.Data.CanteenBook.Infrastructure;

internal class CanteenBookConsinmentsMetadataCollection
{
    /// <summary>
    ///     Получить метаданные рабочей книги.
    ///     Если метаданные не созданы, то создает лист.
    /// </summary>
    /// <param name="workbook"></param>
    /// <returns></returns>
    public static CanteenBookConsinmentsMetadataCollection GetOrCreate(xl.Workbook workbook)
    {
        xl.Worksheet? metaWorksheet = workbook.Worksheets.Cast<xl.Worksheet>().FirstOrDefault(ws => ws.Name == s_METADATA_WORKSHEET_NAME);
        if (metaWorksheet is null)
        {
            metaWorksheet = workbook.Worksheets.Add();
            metaWorksheet.Name = s_METADATA_WORKSHEET_NAME;
            metaWorksheet.Visible = xl.XlSheetVisibility.xlSheetHidden;
        }
        return new CanteenBookConsinmentsMetadataCollection(metaWorksheet);
    }

    /// <summary>
    /// Наименование листа.
    /// </summary>
    private const string s_METADATA_WORKSHEET_NAME = "__cbm";




    public CanteenBookConsinmentsMetadataCollection(xl.Worksheet metadataWorksheet)
    {
        _metadataWorksheet = metadataWorksheet;
    }


    public CanteenBookConsinmentMetadata this[string consinmentNumber]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(consinmentNumber))
                throw new ArgumentNullException(nameof(consinmentNumber));

            xl.Range findRange = _metadataWorksheet.Range["A:A"];
            xl.Range? findedCell = s_FindCell(findRange, consinmentNumber, c =>
            {
                string cellValue = Convert.ToString(c.Value);
                return (cellValue == consinmentNumber);
            });

            if (findedCell is null)
                throw new Exception($"Накладная с номером \"{ consinmentNumber }\" не найдена");

            return GetConsinmentMeta(findedCell);
        }
    }

    public CanteenBookConsinmentMetadata? GetByConsinmentRowIndex(int consinmentRowIndex)
    {
        xl.Range? findedCell = _metadataWorksheet.Range[CONSINMENTS_ROW_INDEX_COLUMN_RANGE].FindCellByValue(consinmentRowIndex.ToString());
        return (findedCell is not null) ? GetConsinmentMeta(findedCell.Row) : null;
    }


    public CanteenBookConsinmentMetadata CreateNew(string consinmentNumber)
    {
        if (_metasDictionary.ContainsKey(consinmentNumber))
        {
            throw new InvalidOperationException("Мета с таким номером уже создана");
        }

        xl.Range? emptyCell = null;
        foreach (xl.Range currentCell in _metadataWorksheet.Range[CONSINMENTS_NUMBER_COLUMN_RANGE].Rows)
        {
            string cellValue = Convert.ToString(currentCell.Value);
            if (string.IsNullOrWhiteSpace(cellValue))
            {
                emptyCell = currentCell;
                break;
            }
        }
        // emptyCell не может быть пустым т.к. цикл выше обязательно найдет значение.
        CanteenBookConsinmentMetadata consinmentMeta = CreateConsinmentMeta(consinmentNumber, emptyCell!);
        return consinmentMeta;
    }

    public bool ContainsMetadata(string consinmentNumber)
    {
        if (string.IsNullOrWhiteSpace(consinmentNumber))
            throw new ArgumentNullException(nameof(consinmentNumber));
        xl.Range dataRange = _metadataWorksheet.Range[CONSINMENTS_NUMBER_COLUMN_RANGE];
        xl.Range findedCell = dataRange.Find(What: consinmentNumber);
        if (findedCell is null)
            return false;
        (int col, int row) firstFindedCell = new(findedCell.Column, findedCell.Row);
        do
        {
            object cellValue = findedCell.Value;
            if (cellValue is not null)
            {
                string? cellValueString = Convert.ToString(cellValue);
                if (!string.IsNullOrWhiteSpace(cellValueString) && cellValueString == consinmentNumber)
                    return true;
            }
            findedCell = dataRange.Find(What: consinmentNumber, After: findedCell);
            if (findedCell.Column == firstFindedCell.col && findedCell.Row == firstFindedCell.row)
                return false;
        } while (true);
    }


    public void DeleteAllMetas()
    {
        foreach (CanteenBookConsinmentMetadata meta in _metasList)
            meta.Delete();
    }

    private 
        xl.Worksheet _metadataWorksheet;
    private readonly Dictionary<string, CanteenBookConsinmentMetadata> _metasDictionary = new();
    private readonly List<CanteenBookConsinmentMetadata> _metasList = new();

    private const string CONSINMENTS_NUMBER_COLUMN_RANGE = "A:A";
    private const string CONSINMENTS_ROW_INDEX_COLUMN_RANGE = "E:E";

    private CanteenBookConsinmentMetadata GetConsinmentMeta(xl.Range leftCornerCell)
    {
        xl.Range currentCell = leftCornerCell;
        xl.Range getNextCell()
        {
            return currentCell = currentCell.Offset[RowOffset: 0, ColumnOffset: 1];
        }

        xl.Range consinmentNumberCell = currentCell;
        xl.Range personsCountCell = getNextCell();
        xl.Range typeCell = getNextCell();
        xl.Range destinationCell = getNextCell();
        xl.Range rowIndexCell = getNextCell();
        
        return new CanteenBookConsinmentMetadata(consinmentNumberCell, personsCountCell, typeCell, destinationCell, rowIndexCell);
    }

    private CanteenBookConsinmentMetadata GetConsinmentMeta(int rowIndex)
    {
        xl.Range leftCornerCell = _metadataWorksheet.Range[$"A{ rowIndex }"];
        return GetConsinmentMeta(leftCornerCell);
    }

    private CanteenBookConsinmentMetadata CreateConsinmentMeta(string consinmentNumber, xl.Range leftCornerCell)
    {
        xl.Range currentCell = leftCornerCell;
        xl.Range getNextCell()
        {
            currentCell = currentCell.Offset[RowOffset: 0, ColumnOffset: 1];
            return currentCell;
        }

        xl.Range consinmentNumberCell = currentCell;
        xl.Range personsCountCell = getNextCell();
        xl.Range typeCell = getNextCell();
        xl.Range destinationCell = getNextCell();
        xl.Range rowIndexCell = getNextCell();
        return new CanteenBookConsinmentMetadata(consinmentNumber, consinmentNumberCell, personsCountCell, typeCell, destinationCell, rowIndexCell); 
    }

    /// <summary>
    ///     Функция поиска ячейки.
    /// </summary>
    /// <param name="rangeToFind"></param>
    /// <param name="what"></param>
    /// <param name="cellComparerPredicate"></param>
    /// <returns></returns>
    private static xl.Range? s_FindCell(xl.Range rangeToFind, string what, Func<xl.Range, bool> cellComparerPredicate)
    {
        xl.Range? findedCell = rangeToFind.Find(What: what);
        if (findedCell is null)
            return null;
        (int rowIndex, int columnIndex) firstFindedCell = (findedCell.Row, findedCell.Column);
        do
        {
            if (cellComparerPredicate(findedCell))
                return findedCell;

            findedCell = rangeToFind.Find(What: what, After: findedCell);

            if (findedCell.Row == firstFindedCell.rowIndex && findedCell.Column == firstFindedCell.rowIndex)
                return null;
        } while (true);
    }

    private List<CanteenBookConsinmentMetadata> GetMetasList()
    {
        List<CanteenBookConsinmentMetadata> metasList = new();
        foreach (xl.Range cell in _metadataWorksheet.Range[CONSINMENTS_NUMBER_COLUMN_RANGE].Rows)
        {
            object cellValue = cell.Value;
            if (cellValue is null)
                break;
            metasList.Add(GetConsinmentMeta(cell));
        }
        return metasList;
    }
}
