using System;
using System.ComponentModel;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Models;

/// <summary>
///     Модель накладной к добавлению.
/// </summary>
public class ConsinmentToAddModel : Model
{
    private bool _errorOccured = false;
    private string _errorMessage = string.Empty;

    private double _totalPcs = default,
                   _totalWeight = default;

    public ConsinmentToAddModel(IConsinmentNote originalConsinment, IReadOnlyConsinmentNote sourceConsinment)
    {
        OriginalConsinment = originalConsinment ?? throw new ArgumentNullException(nameof(originalConsinment));
        SourceConsinment = sourceConsinment ?? throw new ArgumentNullException(nameof(sourceConsinment));
    }

    
    [Browsable(false)] public IConsinmentNote OriginalConsinment { get; }

    [Browsable(false)] public IReadOnlyConsinmentNote SourceConsinment { get; }

    [DisplayName("Дата"), ReadOnly(true)] public DateOnly Date => OriginalConsinment.Day;

    [DisplayName("№ накл."), ReadOnly(true)] public string ConsinmentNumber => OriginalConsinment.Number;

    [DisplayName("Тип")]
    public string ConsinmentType
    {
        get => OriginalConsinment.Description.Type ?? String.Empty;
        set => ChangeProperty(OriginalConsinment.Description, d => d.Type, value);
        
    }

    [DisplayName("Кол-во питающихся")] public int PersonsCount
    {
        get => OriginalConsinment.Description.PersonsCount;
        set => ChangeProperty(OriginalConsinment.Description, d => d.PersonsCount, value);
    }

    [DisplayName("Направление")] public string Destination
    {
        get => OriginalConsinment.Description.Destination;
        set => ChangeProperty(OriginalConsinment.Description, d => d.Destination, value);
    }


    [DisplayName("Всего шт."), ReadOnly(true)] public double TotalPcs 
    {
        get => _totalPcs;
        set => ChangeProperty(ref _totalPcs, value);
    }

    [DisplayName("Всего кг."), ReadOnly(true)] public double TotalWeight 
    {
        get => _totalWeight;
        set => ChangeProperty(ref _totalWeight, value);
    }

    [Browsable(false)] public bool ErrorOccured
    {
        get => _errorOccured;
        set
        {
            if (_errorOccured != value)
            {
                _errorOccured = value;
                InvokePropertyChanged();
            }
        }
    }

    [Browsable(false)] public string ErrorMessage 
    {
        get => _errorMessage ?? string.Empty;
        set => ChangeProperty(ref _errorMessage, value);
    }

    /// <summary>
    ///     Обновить свойства <see cref="TotalPcs"/> и <see cref="TotalWeight"/> исходя из оригинальной накладной <see cref="OriginalConsinment"/>
    /// </summary>
    public void UpdateWeight()
    {
        TotalPcs = OriginalConsinment.GetTotalProductsPcs();
        TotalWeight = OriginalConsinment.GetTotalProductsWeight();
    }
}
