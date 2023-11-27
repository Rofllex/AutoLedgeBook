#nullable enable

namespace AutoLedgeBook.Utils.Matches
{
    /// <summary>
    ///     Результат сравнения.
    /// </summary>
    public enum CompareMatchError
    {
        /// <summary>
        ///     Продукт не найден в книге накладных
        /// </summary>
        ProductNotFound,
        /// <summary>
        ///     Сопоставление для продукта не найдено.
        /// </summary>
        ProductMatchNotFound,
    }
}
