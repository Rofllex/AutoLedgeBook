using System;

namespace AutoLedgeBook.Data.Abstractions;

/// <summary>
///     Накладная.
/// </summary>
public interface IReadOnlyConsinmentNote
{
    /// <summary>
    ///      Номер накладной.
    /// </summary>
    string Number { get; }

    /// <summary>
    ///     День на который ориентирована накладная.
    /// </summary>
    DateOnly Day { get;  }

    /// <summary>
    ///     Описание накладной.
    /// </summary>
    IReadOnlyConsinmentDescription Description { get; }

    /// <summary>
    ///     Продукты в режиме только для чтения
    /// </summary>
    IAccountingProductCollection<IReadOnlyAccountingProduct> Products { get; }

    /// <summary>
    ///     Получить вес продуктов.
    /// </summary>
    /// <returns></returns>
    double GetTotalProductsWeight();
    
    /// <summary>
    ///     Получить количество продуктов(только для продуктов, которые измеряются в единицах)
    /// </summary>
    /// <returns></returns>
    double GetTotalProductsPcs();
}