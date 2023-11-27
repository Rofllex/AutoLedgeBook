using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Models;

/// <summary>
///     Модель накладной по умолчанию.
/// </summary>
public class ConsinmentModel
{
    public static ConsinmentModel[] FromArray(IReadOnlyConsinmentNote[] consinments)
    {
        ConsinmentModel[] models = new ConsinmentModel[consinments.Length];
        for (var i = 0; i < models.Length; i++)
            models[i] = new(consinments[i]);
        return models;
    }

    public static IEnumerable<ConsinmentModel> FromEnumerable(IEnumerable<IReadOnlyConsinmentNote> consinmentsEnumerable) => consinmentsEnumerable.Select(c => new ConsinmentModel(c));


    public ConsinmentModel(IReadOnlyConsinmentNote consinment) 
    {
        Original = consinment ?? throw new ArgumentNullException(nameof(consinment));
    }

    [Browsable(false)] public IReadOnlyConsinmentNote Original { get; }

    [DisplayName("№ накладной")] public string Number => Original.Number;

    [DisplayName("Тип")] public string Type => Original.Description.Type;

    [DisplayName("Кол-во питающихся")] public int PersonsCount => string.IsNullOrWhiteSpace(Number) ? 0 : Original.Description.PersonsCount;
}
