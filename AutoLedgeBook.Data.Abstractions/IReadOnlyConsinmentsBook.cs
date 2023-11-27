using System;
using System.Collections.Generic;

namespace AutoLedgeBook.Data.Abstractions;

/// <summary>
///     Книга с накладными.
/// </summary>
public interface  IReadOnlyConsinmentsBook : IDisposable
{
    /// <summary>
    ///     Даты, на которые ориентирована накладная книга.
    /// </summary>
    IReadOnlyList<DateOnly> Dates { get; }

    /// <summary>
    ///     Получить коллекцию накладных на один день.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    IReadOnlyConsinmentsCollection GetConsinmentsByDate(DateOnly day);
}
