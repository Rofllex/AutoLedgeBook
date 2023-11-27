namespace AutoLedgeBook.Data.Abstractions
{
    /// <summary>
    ///     Продукт в режиме "только для чтения"
    /// </summary>
    public interface IReadOnlyAccountingProduct
    {
        /// <summary>
        ///     Наименование продукта.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Значение расхода.
        /// </summary>
        double Value { get; }
        /// <summary>
        ///     Ед. учета продукта
        /// </summary>
        ProductUnits Units { get; }
    }    
}
