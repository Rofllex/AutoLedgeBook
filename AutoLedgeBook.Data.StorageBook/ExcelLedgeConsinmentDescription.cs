using System.Text;
using System.Diagnostics;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.StorageBook;

/// <inheritdoc cref="IConsinmentNoteHeader"/>
[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class ExcelLedgeConsinmentDescription : IConsinmentDescription
{
    private readonly xl.Range headerCell;
    
    private string _destination = string.Empty;
    private string _number = string.Empty;
    private string _type = string.Empty;
    private int _personsCount = 0;
    private bool _suspendUpdateCellValue = false;

    internal ExcelLedgeConsinmentDescription(DateOnly date, xl.Range headerCell)
    {
        Date = date;
        this.headerCell = headerCell ?? throw new ArgumentNullException(nameof(headerCell));
        ParseHeaderCellValue(this.headerCell.Value);
    }

    internal ExcelLedgeConsinmentDescription(DateOnly date, xl.Range headerCell, string destination, string number, int personsCount, string type)
    {
        Date = date;
        this.headerCell = headerCell ?? throw new ArgumentNullException(nameof(headerCell));
        UpdateHeader(h => 
        {
            this._destination = destination;
            this._destination = destination;
            this._number = number;
            this._personsCount = personsCount;
            this._type = type;
        });
    }

    public string Destination
    {
        get => _destination;
        set => UpdateHeader(l => l._destination = value);
    }
    public string Number
    {
        get => _number;
        set => UpdateHeader(l => l._number = value);
    }

    public int PersonsCount
    {
        get => _personsCount;
        set => UpdateHeader(l => l._personsCount = value);
    }

    public string Type
    {
        get => _type;
        set => UpdateHeader(l => l._type = value);
    }


    public DateOnly Date { get; init; }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(Destination);
        stringBuilder.AppendLine(Number);
        stringBuilder.AppendLine(PersonsCount.ToString());
        stringBuilder.AppendLine(Type);
        return stringBuilder.ToString();
    }
    
    internal void Clear()
    {
        _suspendUpdateCellValue = true;
            this._destination = this._type = this._number = String.Empty;
            _personsCount = default;
            _ = headerCell.ClearContents();
        _suspendUpdateCellValue = false;
        
    }


    private void UpdateCellValue()
    {
        if (!_suspendUpdateCellValue)
            headerCell.Value = $"{Destination}\n{Number}\n{PersonsCount}\n{Type}";
    }

    private void ParseHeaderCellValue(string headerCellValue)
    {
        UpdateHeader(h =>
        {
            if (!string.IsNullOrWhiteSpace(headerCellValue))
            {
                while (headerCellValue.IndexOf("  ") > -1)
                    headerCellValue = headerCellValue.Replace("  ", " ");
                headerCellValue = headerCellValue.Replace('\n', ' ');
                string[] splitted = headerCellValue.Split(' ', 4);
                if (splitted.Length > 0)
                {
                    _destination = splitted[0];
                    if (splitted.Length > 1)
                    {
                        _number = splitted[1];

                        if (splitted.Length > 2)
                        {
                            if (!int.TryParse(OnlyDigitsInString(splitted[2]), out _personsCount))
                                _personsCount = int.MinValue;

                            if (splitted.Length > 3)
                            {
                                _type = splitted[3];
                            }
                        }
                    }
                }
            }
        });
    }

    private string OnlyDigitsInString(string originalString)
        => new string(originalString.Where(c => char.IsDigit(c)).ToArray());
    
    private void UpdateHeader(Action<ExcelLedgeConsinmentDescription> header)
    {
        _suspendUpdateCellValue = true;
        header(this);
        _suspendUpdateCellValue = false;
        UpdateCellValue();
    }
    
    private string GetDebuggerDisplay()
        => $"[{Destination}] [{ Number }] [{ PersonsCount }] [{ Type }]";
}
