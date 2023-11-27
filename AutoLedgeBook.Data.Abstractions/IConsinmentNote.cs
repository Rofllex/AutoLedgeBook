using System;
using System.Collections;
using System.Collections.Generic;

namespace AutoLedgeBook.Data.Abstractions;

/// <summary>
///     Расход по накладной.
/// </summary>
public interface IConsinmentNote : IReadOnlyConsinmentNote
{
    /// <summary>
    ///     Описание накладной.
    /// </summary>
    new IConsinmentDescription Description { get; }

    /// <summary>
    ///     Продукты для перезаписи.
    /// </summary>
    new IAccountingProductCollection<IAccountingProduct> Products { get; }
}

/// <summary>
///     Накладная на приход.
/// </summary>
public interface IArrivalConsinmentNote
{
    /// <summary>
    ///     Номер накладной
    /// </summary>
    string ConsinmentNumber { get; }

    /// <summary>
    ///     Коллекция продуктов.
    /// </summary>
    IAccountingProductCollection<IAccountingProduct> Products { get; }
}

/// <summary>
///     Коллекция накладных прихода
/// </summary>
public interface IArrivalConsinmentNotesCollection : IReadOnlyList<IArrivalConsinmentNote>
{
    /// <summary>
    ///     Получить накладную по номеру накладной.
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной</param>
    /// <returns></returns>
    IArrivalConsinmentNote this[string consinmentNumber] { get; }

    /// <summary>
    ///     Добавить накладную.
    /// </summary>
    /// <param name="consinmentNumber"></param>
    IArrivalConsinmentNote Add(string consinmentNumber);

    bool ContainsConsinment(string consinmentNumber);

    /// <summary>
    ///     Удалить накладную по номеру
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной</param>
    void Remove(string consinmentNumber);
    
    /// <summary>
    ///     Удалить накладную
    /// </summary>
    /// <param name="consinment">Экземпляр накладной</param>
    void Remove(IArrivalConsinmentNote consinment);
}
