namespace AutoLedgeBook.Data.Abstractions
{
    /// <summary>
    ///     Подведение итогов для коллекции продуктов.
    /// </summary>
    public interface IConsinmentsCollectionStatistics
    {
        /// <summary>
        ///     Коллекция остатка продуктов на конец дня.
        /// </summary>
        IAccountingProductCollection<IReadOnlyAccountingProduct> RemainProducts { get; }

        /// <summary>
        ///     Всего расход продуктов за день.
        /// </summary>
        IAccountingProductCollection<IReadOnlyAccountingProduct> TotalProductsConsumption { get; }
    }
}
