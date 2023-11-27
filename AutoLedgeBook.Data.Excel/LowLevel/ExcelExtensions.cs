using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.Excel.LowLevel;

public static class ExcelExtensions
{
    /// <summary>
    ///     Делегат незамкнутого поиска по диапазону ячеек.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns>
    ///     Если вернет <c>true</c>, значит ячейка найдена и поиск можно останавливать.
    /// </returns>
    public delegate bool FindNextCellPredicate(xl.Range cell);

    public static xl.Range? Find(this xl.Range range,
                                 string what,
                                 FindNextCellPredicate predicate,
                                 xl.XlSearchDirection searchDirection = xl.XlSearchDirection.xlNext,
                                 bool matchCase = false)
    {
        xl.Range findedCell = range.Find(What: what);
        if (findedCell is null)
            return null;
        (int Column, int Row) firstFindedCell = new(findedCell.Column, findedCell.Row);
        do
        {
            findedCell = range.Find(What: what,
                                    After: findedCell,
                                    SearchDirection: searchDirection,
                                    MatchCase: matchCase);
            // Если найденная ячейка идентична первой найденной.
            if (findedCell.Column == firstFindedCell.Column && findedCell.Row == firstFindedCell.Row)
                break;

            if (predicate(findedCell))
                return findedCell;
        } while (true);

        return default;
    }

    /// <summary>
    ///     Метод поиска ячейки в диапазоне.
    ///     Находит значение если значение в найденной ячейке полностью соответствует <paramref name="whatFind"/> 
    /// </summary>
    /// <param name="range"></param>
    /// <param name="whatFind"></param>
    /// <returns></returns>
    public static xl.Range? FindCellByValue(this xl.Range range, string whatFind, bool matchCase = false)
    {
        return FindCellByValue(range, whatFind, (xl.Range cell) =>
        {
            string cellValueString = Convert.ToString(cell.Value);
            return string.Compare(whatFind, cellValueString, !matchCase) == 0;
        }, matchCase);
    }

    public static xl.Range? FindCellByValue(this xl.Range range, string whatFind, Func<xl.Range, bool> comparePredicate, bool matchCase)
    {
        if (string.IsNullOrWhiteSpace(whatFind))
            throw new ArgumentNullException(nameof(whatFind));
        
        if (comparePredicate is null)
            throw new ArgumentNullException(nameof(comparePredicate));
        
        xl.Range findedCell = range.Find(What: whatFind, MatchCase: matchCase);
        if (findedCell is not null)
        {
            (int Column, int Row) firstFindedCoords = new(findedCell.Column, findedCell.Row);
            do
            {
                if (comparePredicate(findedCell))
                    return findedCell;

                findedCell = range.Find(What: whatFind, After: findedCell, MatchCase: matchCase);
            } while (findedCell.Row != firstFindedCoords.Row && findedCell.Column != firstFindedCoords.Column);
        }
        return default;
    }
}
