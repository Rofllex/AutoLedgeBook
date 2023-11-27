using System;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Data.ExcelConsinments;

/// <summary>
///     Фабрика создания накладной
/// </summary>
public interface IExcelConsinmentBookFactory
{
    /// <summary>
    ///     Тип на который ориентирована фабрика
    /// </summary>
    Type TypeToBuild { get; }

    /// <summary>
    ///     Проверить возможность создания документа исходя из входных данных.
    /// </summary>
    /// <returns></returns>
    bool Check();
    /// <summary>
    ///     Создать экземпляр накладной
    /// </summary>
    /// <returns></returns>
    IReadOnlyConsinmentsBook Build();
}

