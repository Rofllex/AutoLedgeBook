using System;

namespace AutoLedgeBook.Data.Abstractions
{
    /// <summary>
    ///     Книга учета веса.
    /// </summary>
    public interface IConsinmentsBook : IReadOnlyConsinmentsBook
    {
        /// <summary>
        ///     Получить расход накладной по номеру.    
        /// </summary>
        /// <param name="consinmentNumber">Номер накладной</param>
        /// <exception cref="ConsinmentNotFoundException">Выбрасывается когда накладная не найдена.</exception>
        IConsinmentNote this[string consinmentNumber] { get; }
        
        /// <summary>
        ///     Если содержит накладуню.
        /// </summary>
        bool ContainsConsinment(string consinmentNumber);

        /// <summary>
        ///     Получить расход за день.
        /// </summary>
        new IConsinmentsCollection GetConsinmentsByDate(DateOnly date);

        /// <summary>
        ///     Получить доступные наименования продуктов.
        /// </summary>
        string[] GetProductNames();
    }
}
