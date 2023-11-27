using System.Collections.Generic;

namespace AutoLedgeBook.Data.Abstractions;

/// <summary>
///     Коллекция расхода по накладным.
/// </summary>
public interface IConsinmentsCollection : IReadOnlyConsinmentsCollection, IReadOnlyCollection<IConsinmentNote>
{
    /// <summary>
    ///     Возможность получения накладной по номеру накладных.
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной</param>
    /// <exception cref="ConsinmentNotFoundException" />
    new IConsinmentNote this[string consinmentNumber] { get; }

    /// <summary>
    ///     Максимальное кол-во накладных.
    /// </summary>
    int MaxCount { get; }

    /// <summary>
    ///     Добавить новую накладную.
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной</param>
    /// <returns></returns>
    IConsinmentNote Add(string consinmentNumber);
    
    /// <summary>
    ///     Удалить накладную.
    /// </summary>
    /// <param name="consinmentNumber">Номер накладной</param>
    void Delete(string consinmentNumber);
}