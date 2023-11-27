#nullable enable

namespace AutoLedgeBook.Utils.Extensions;

using xl = Microsoft.Office.Interop.Excel;

internal static class ExcelExtensions
{
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
        if (findedCell is null)
            return null;
        
        (int Column, int Row) firstFindedCoords = new(findedCell.Column, findedCell.Row);
        do
        {
            if (comparePredicate(findedCell))
                return findedCell;
            findedCell = range.Find(What: whatFind, After: findedCell, MatchCase: matchCase);
        } while (findedCell.Row != firstFindedCoords.Row && findedCell.Column != firstFindedCoords.Column);
        return null;
    }
}
