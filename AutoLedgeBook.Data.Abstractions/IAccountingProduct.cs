namespace AutoLedgeBook.Data.Abstractions
{
    /// <summary>
    ///     Учитываемый продукт
    /// </summary>
    public interface IAccountingProduct : IReadOnlyAccountingProduct
    {
        /// <summary>
        ///     Значение продукта.
        /// </summary>
        new double Value { get; set; }
    }
}
