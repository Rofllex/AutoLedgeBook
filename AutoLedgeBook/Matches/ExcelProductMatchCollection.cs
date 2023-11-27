#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AutoLedgeBook.Utils.Matches;
using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Matches;

/// <summary>
///     Коллекция сопоставлений в Excel документе.
/// </summary>
public class ExcelProductMatchCollection : IMatchesList<string, string>, ISaveable
{
    /// <summary>
    ///     Загрузить коллекцию сопоставлений из файла excel.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static ExcelProductMatchCollection FromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        xl.Application excelApplication = new()
        {
            DisplayAlerts = false
        };

        xl.Workbook workbook = excelApplication.Workbooks.Open(filePath, ReadOnly: false);
        xl.Worksheet worksheet = workbook.Worksheets[1];
        return new ExcelProductMatchCollection(workbook, worksheet);
    }

    private readonly xl.Workbook _workbook;
    private readonly xl.Worksheet _worksheet;
    private readonly Dictionary<string, ExcelProductMatch> _matchesDictionary;
    private readonly EventWaitHandle _saveWaitHandle = new(true, EventResetMode.ManualReset);

    /// <summary>
    /// последняя занятая ячейка
    /// </summary>
    private int _lastBusyHeaderCell;

    private bool _disposed = false;

    private ExcelProductMatchCollection(xl.Workbook workbook, xl.Worksheet worksheet)
    {
        _workbook = workbook;
        _worksheet = worksheet;
        _matchesDictionary = GetMatches(worksheet, out _lastBusyHeaderCell).ToDictionary(m => m.Key);

        workbook.BeforeSave += (bool SaveAsUI, ref bool Cancel) =>
        {
            SaveAsUI = false;
            _saveWaitHandle.Reset();
        };

        workbook.AfterSave += (success) =>
        {
            _saveWaitHandle.Set();
        };
    }

    /// <summary>
    ///     Получить сопоставление по ключу.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    public ExcelProductMatch this[string key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (!_matchesDictionary.ContainsKey(key))
                throw new KeyNotFoundException(key);

            return _matchesDictionary[key];
        }

        private set => _matchesDictionary[key] = value;
    }

    public int Count => _matchesDictionary.Count;

    public ExcelProductMatch GetBySourceValue(string sourceValue)
    {
        if (string.IsNullOrWhiteSpace(sourceValue))
            throw new ArgumentNullException(nameof(sourceValue));

        KeyValuePair<string, ExcelProductMatch> match = _matchesDictionary.FirstOrDefault(m => m.Value.Values.Contains(sourceValue));
        return match.Value ?? throw new CompareMatchException(sourceValue, CompareMatchError.ProductNotFound);
    }

    public IEnumerator GetEnumerator()
        => GetEnumerator();

    public void Dispose()
    {
        if (_disposed)
            return;

        _workbook.Close(SaveChanges: false);
        _workbook.Application.Quit();

        xl.Application? application = _workbook.Application;
        Marshal.ReleaseComObject(_workbook);
        Marshal.ReleaseComObject(application);
        GC.SuppressFinalize(this);

        _disposed = true;
    }


    public void Add(string destination, string[] sources)
    {
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentNullException(nameof(destination));

        if (sources is null || sources.Length == 0)
            throw new ArgumentNullException(nameof(sources));

        if (ContainsKey(destination))
            throw new InvalidOperationException($"Ключ \"{destination}\" уже присутствует в системе");

        xl.Range headerCell = GetNextFreeHeaderCell();
        headerCell.EntireColumn.ClearContents();

        headerCell.Value = destination;

        xl.Range[] valueCells = new xl.Range[sources.Length];
        xl.Range currentValueCell = headerCell.Offset[RowOffset: 1, ColumnOffset: 0];

        for (var i = 0; i < valueCells.Length; i++)
        {
            valueCells[i] = currentValueCell;
            currentValueCell.Value = sources[i];

            currentValueCell = currentValueCell.Offset[1, 0];
        }

        ExcelProductMatch match = new(headerCell, valueCells);
        this[destination] = match;
    }

    public void Remove(string destination)
    {
        if (!ContainsKey(destination))
            throw new KeyNotFoundException(destination);
        ExcelProductMatch? match = this[destination];
        match.KeyCell.EntireColumn.Delete();
        _matchesDictionary.Remove(destination);
    }

    public bool ContainsKey(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentNullException(nameof(destination));
        return _matchesDictionary.ContainsKey(destination);
    }

    public void Save()
    {
        _workbook.Save();
        _saveWaitHandle.WaitOne();
    }


    public void SaveAs(string filePath) => _workbook.SaveAs(Filename: filePath);

    public void SaveAs(string filePath, ISaveSettings saveSettings) => SaveAs(filePath);

    #region interfaces implicit implementation

    IMatch<string, string> IMatchesCollection<string, string>.this[string key] => this[key];

    string[] IMatchesCollection<string, string>.GetKeys()
        => _matchesDictionary.Select(m => m.Key).ToArray();

    IEnumerator<IMatch<string, string>> IEnumerable<IMatch<string, string>>.GetEnumerator()
        => _matchesDictionary.Values.GetEnumerator();

    IMatch<string, string> IMatchesCollection<string, string>.GetBySourceValue(string key)
        => GetBySourceValue(key);

    #endregion

    private List<ExcelProductMatch> GetMatches(xl.Worksheet _worksheet, out int lastHeaderBusyColumnIndex)
    {
        lastHeaderBusyColumnIndex = int.MinValue;

        xl.Range currentCell = _worksheet.Range["B1"];
        List<ExcelProductMatch>? matchesList = new();

        while (!string.IsNullOrWhiteSpace(currentCell.Value))
        {
            xl.Range productNameCell = currentCell;

            List<xl.Range> matches = GetColumnCellsByValue(productNameCell.Offset[1, 0]);
            if (matches.Count > 0)
            {
                matchesList.Add(new ExcelProductMatch(productNameCell, matches.ToArray()));
                lastHeaderBusyColumnIndex = currentCell.Column;
            }

            currentCell = currentCell.Offset[0, 1];
        }

        return matchesList;
    }

    private static List<xl.Range> GetColumnCellsByValue(xl.Range startCell)
    {
        xl.Range currentCell = startCell;

        List<xl.Range> cells = new();
        while (currentCell.Value is not null)
        {
            cells.Add(currentCell);
            currentCell = currentCell.Offset[1, 0];
        }
        return cells;
    }

    private xl.Range GetNextFreeHeaderCell() => (xl.Range)_worksheet.Cells[1, ++_lastBusyHeaderCell];
}
