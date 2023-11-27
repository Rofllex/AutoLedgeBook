using System;

namespace AutoLedgeBook.Data.Abstractions
{
    /// <summary>
    ///     Исключение если накладной не было найдено.
    /// </summary>
    public class ConsinmentNotFoundException : Exception
    {
        public ConsinmentNotFoundException(string consinmentNumber) : this (consinmentNumber, $"Накладная с номером \"{ consinmentNumber }\" не найдена")
        {
        }

        public ConsinmentNotFoundException(string consinmentNumber, string message) : base(message)
        {
            ConsinmentNumber = consinmentNumber;
        }
        
        /// <summary>
        ///     Номер накладной.
        /// </summary>
        public string ConsinmentNumber { get; }
    }
}
