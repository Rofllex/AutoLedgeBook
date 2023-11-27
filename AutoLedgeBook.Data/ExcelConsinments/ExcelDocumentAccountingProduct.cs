using System;
using System.Diagnostics;
using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Продукт накладной excel.
/// </summary>
/// <inheritdoc cref="IAccountingProduct"/>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ExcelDocumentAccountingProduct : ExcelDocumentReadOnlyAccountingProduct, IAccountingProduct
{
    public ExcelDocumentAccountingProduct(string name, xl.Range valueCell, ProductUnits units) : base(name, valueCell, units)
    {
        _changeValueAction = valueCell.HasFormula
            ? ((_) => throw new InvalidOperationException("Запрещенно редактировать ячейки в которых содержатся формулы."))
            : ((value) => this.ValueCell.Value = value);
    }

    public new double Value
    {
        get => base.Value;
        set => ValueCell.Value = value;
    }

    private readonly Action<double> _changeValueAction;

    private string GetDebuggerDisplay()
        => $"{Name} {Value} " + Units switch
        {
            ProductUnits.Pcs => "шт.",
            ProductUnits.Kilo => "кг.",
            _ => "unknown"
        };
}