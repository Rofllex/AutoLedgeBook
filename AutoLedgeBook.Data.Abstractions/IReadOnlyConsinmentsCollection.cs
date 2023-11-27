using System;
using System.Collections.Generic;

namespace AutoLedgeBook.Data.Abstractions;

/// <summary>
///     Коллекция накладных только для чтения
/// </summary>
public interface IReadOnlyConsinmentsCollection : IReadOnlyCollection<IReadOnlyConsinmentNote>
{
    /// <summary>
    ///     Получить накладную по номеру.
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной</param>
    /// <returns></returns>
    /// <exception cref="ConsinmentNotFoundException" />
    IReadOnlyConsinmentNote this[string consinmentNumber] { get; }
    
    /// <summary>
    ///     День, на который ориентирована накладная.
    /// </summary>
    DateOnly Day { get; }
    
    /// <summary>
    ///     Содержит ли данная коллекция накладную.
    /// </summary>
    /// <param name="consinmentNumber"></param>
    /// <returns></returns>
    bool Contains(string consinmentNumber);
}
