using System;
using System.Linq;
using System.Collections.Generic;

namespace AutoLedgeBook.Data.Abstractions
{
    /// <summary>
    ///     Коллекция продуктов.
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    public interface IAccountingProductCollection<out TProduct> : IReadOnlyCollection<TProduct> where TProduct : class
    {
        /// <summary>
        ///     Получить продукт по наименованию.
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        TProduct this[string productName] { get; }
        /// <summary>
        ///     Получить наименования всех продуктов.
        /// </summary>
        /// <returns></returns>
        string[] GetProductNames();
        /// <summary>
        ///     Содержится ли в этой коллекции продукт
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool ContainsProduct(string productName);
    }
}
