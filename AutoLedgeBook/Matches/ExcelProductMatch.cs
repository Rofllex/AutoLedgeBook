#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using AutoLedgeBook.Utils.Matches;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Matches;

/// <summary>
///     Текстовое сопоставление в excel`е.
/// </summary>
/// <inheritdoc cref="IEditableMatch{TDestination, TSource}"/>
public class ExcelProductMatch : IEditableMatch<string, string>
{
    private readonly List<xl.Range> _valueCells;
    private readonly ObservableCollection<string> _valuesCollection;

    internal ExcelProductMatch(xl.Range keyCell, xl.Range[] valueCells)
    {
        KeyCell = keyCell;
        Key = Convert.ToString(keyCell.Value);

        _valueCells = valueCells.ToList();

        _valuesCollection = new ObservableCollection<string>(valueCells.Select(c => Convert.ToString(c.Value)).Cast<string>());
        _valuesCollection.CollectionChanged += ValuesCollectionChanged;
    }


    public IList<string> Values => _valuesCollection;

    public string Key { get; }

    #region interface implicit implementation

    string[] IMatch<string, string>.Values => Values.ToArray();

    #endregion

    internal xl.Range KeyCell { get; private set; }

    internal IReadOnlyCollection<xl.Range> ValueCells => _valueCells;

    protected xl.Range GetNextCell() => ValueCells.Last().Offset[RowOffset: 1, ColumnOffset: 0];


    private void ValuesCollectionChanged(object? _, NotifyCollectionChangedEventArgs __) => SyncValuesAndCollection(_valueCells, Values);

    /// <summary>
    ///     Синхронизация значений и коллекций.
    /// </summary>
    /// <param name="valueCells"></param>
    /// <param name="values"></param>
    private void SyncValuesAndCollection(List<xl.Range> valueCells, IEnumerable<string> values)
    {
        int valuesCount = values.Count();
                
        while (valueCells.Count < valuesCount)
            valueCells.Add(GetNextCell());

        while (valueCells.Count > valuesCount)
        {
            xl.Range lastCell = valueCells.Last();
            lastCell.ClearContents();
            valueCells.Remove(lastCell);
        }

        IEnumerator<string> valuesEnumerator = values.GetEnumerator();

        for (var i = 0; valuesEnumerator.MoveNext(); i++)
            valueCells[i].Value = valuesEnumerator.Current;
    }
}
