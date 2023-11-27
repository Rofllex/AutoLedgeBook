#nullable enable

namespace AutoLedgeBook.Utils.Matches;

using System.Collections;
using System.Collections.Immutable;

using xl = Microsoft.Office.Interop.Excel;


/// <summary>
///     Коллекция сопоставлений в Excel документе.
/// </summary>
public class ExcelProductMatchCollection : IMatchesCollection<string, string>
{
    public static ExcelProductMatchCollection FromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        xl.Application excelApplication = new();
        xl.Workbook workbook = excelApplication.Workbooks.Open(filePath, ReadOnly: true);
        xl.Worksheet worksheet = workbook.Worksheets[1];

        try
        {
            ExcelProductMatchCollection matchesCollection = new(GetMatches(worksheet));
            return matchesCollection;
        }
        finally
        {
            workbook.Close(SaveChanges: false);
            excelApplication.Quit();
        }
    }

    private static IReadOnlyDictionary<string, IMatch<string, string>> GetMatches(xl.Worksheet _worksheet)
    {
        xl.Range currentCell = _worksheet.Range["B1"];

        ImmutableDictionary<string, IMatch<string, string>>.Builder matchesDictionaryBuilder = ImmutableDictionary.CreateBuilder<string, IMatch<string, string>>();

        while (!string.IsNullOrWhiteSpace(currentCell.Value))
        {
            string productName = Convert.ToString(currentCell.Value);
            if (matchesDictionaryBuilder.ContainsKey(productName))
                throw new Exception($"Наименование продукта \"{ productName }\" дублируется в наименовании источника продукта");

            xl.Range columnMatchesCell = currentCell.Offset[1, 0];

            string[] values = GetColumnValues(columnMatchesCell);

            if (values.Length > 0)
                matchesDictionaryBuilder[productName] = new MemoryMatch<string, string>(productName, values);

            currentCell = currentCell.Offset[0, 1];
        }

        return matchesDictionaryBuilder.ToImmutable();
    }

    /// <summary>
    /// Получить значения столбца.
    /// </summary>
    /// <param name="startCell">Первая ячейка с возможным значением</param>
    /// <remarks>
    ///     Ищет по столбцу до того момента, пока не будет пустая ячейка.
    /// </remarks>
    /// <returns></returns>
    private static string[] GetColumnValues(xl.Range startCell)
    {
        xl.Range currentCell = startCell;
        List<string> valuesList = new();
        do
        {
            object? currentCellValue = currentCell.Value;
            if (currentCellValue is null)
                break;

            string? currentCellValueString = Convert.ToString(currentCellValue);
            if (string.IsNullOrWhiteSpace(currentCellValueString))
                break;

            valuesList.Add(currentCellValueString);
            currentCell = currentCell.Offset[RowOffset: 1, ColumnOffset: 0];
        } while (true);

        return valuesList.ToArray();
    }

    private readonly IReadOnlyDictionary<string, IMatch<string, string>> _matchesDictionary;
    private readonly string[] _keys;



    private ExcelProductMatchCollection(IReadOnlyDictionary<string, IMatch<string, string>> matches)
    {
        _matchesDictionary = matches;
        _keys = matches.Keys.ToArray();
    }

    public IMatch<string, string> this[string key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (_matchesDictionary.TryGetValue(key, out IMatch<string, string>? match))
                return match!;
            else
                throw new KeyNotFoundException(key);
        }
    }

    public int Count => _keys.Length;

    public IMatch<string, string> GetBySourceValue(string sourceValue)
    {
        if (string.IsNullOrWhiteSpace(sourceValue))
            throw new ArgumentNullException(nameof(sourceValue));
        return _matchesDictionary.Values.FirstOrDefault(v => v.Values.Contains(sourceValue)) ?? throw new ArgumentException("Значение в сопоставлениях не найдено");
    }

    public IEnumerator GetEnumerator()
        => GetEnumerator();

    string[] IMatchesCollection<string, string>.GetKeys()
        => _keys;


    IEnumerator<IMatch<string, string>> IEnumerable<IMatch<string, string>>.GetEnumerator()
        => _matchesDictionary.Values.GetEnumerator();

    void IDisposable.Dispose()
    {
    }
}
