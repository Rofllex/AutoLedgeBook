#nullable enable

using System;
using System.Linq;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments
{
    /// <inheritdoc cref="IConsinmentNoteHeader"/>
    public class ReadOnlyExcelDocumentConsinmentNoteDescription : IReadOnlyConsinmentDescription
    {
        internal ReadOnlyExcelDocumentConsinmentNoteDescription(xl.Worksheet consinmentWorksheet, string typeCellAddress = "G12")
        {
            Date = GetDate(consinmentWorksheet);
            Destination = GetDestination(consinmentWorksheet) ?? string.Empty;
            Number = GetConsinmentNumber(consinmentWorksheet);
            PersonsCount = GetPersonsCount(consinmentWorksheet);
            Type = (string)consinmentWorksheet.Range[typeCellAddress].Value;
        }


        internal ReadOnlyExcelDocumentConsinmentNoteDescription(string destination, string number, int personsCount, string type, DateOnly date)
        {
            Destination = destination;

            if (string.IsNullOrWhiteSpace(Number = number))
                throw new ArgumentNullException(nameof(number), "Номер накладной не может быть пустым");

            if ((PersonsCount = personsCount) < 1)
                throw new ArgumentOutOfRangeException(nameof(personsCount), "Кол-во питающихся не может быть меньше или равным 0");

            PersonsCount = personsCount;
            Type = type;
            Date = date;
        }


#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        internal protected ReadOnlyExcelDocumentConsinmentNoteDescription()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        {
        }



        #region public properties 

        public virtual string Destination { get; }

        public virtual string Number { get; }

        public virtual int PersonsCount { get; }

        public virtual string Type { get; }

        public virtual DateOnly Date { get; }

        #endregion

        protected virtual string ParseConsinmentNumber(string cellValue)
        {
            int GetFirstDigitIndex(string inputString)
            {
                for (var i = 0; i < inputString.Length; i++)
                    if (char.IsDigit(inputString[i]))
                        return i;
                return -1;
            }
            
            cellValue = cellValue.Replace('а', 'a');
            int firstDigitIndex = GetFirstDigitIndex(cellValue);
            if (firstDigitIndex > -1)
                cellValue = cellValue.Remove(0, GetFirstDigitIndex(cellValue));
            else
                return string.Empty;

            char[] allowedChars = { 'a', '/' };
 
            for (var i = 0; i < cellValue.Length; i++)
            {
                char currentChar = cellValue[i];

                if (!char.IsDigit(currentChar))
                {
                    if (!allowedChars.Contains(currentChar))
                    {
                        cellValue = cellValue.Remove(i, 1);
                        i--;
                    }
                }
            }



            return cellValue;
        }

        private string GetConsinmentNumber(xl.Worksheet consinmentWorksheet)
        {
            xl.Range? findedCell = consinmentWorksheet.UsedRange.Find(What: "Накладная");
            if (findedCell is null)
                throw new System.Data.DataException("Не удалось найти номер накладной!");

            return ParseConsinmentNumber(Convert.ToString(findedCell.Value));
        }

        private DateOnly GetDate(xl.Worksheet consinmentWorksheet)
        {
            const string DEFAULT_DATE_ADDRESS = "D8";

            DateTime? dateTime = consinmentWorksheet.Range[DEFAULT_DATE_ADDRESS].Value as DateTime?;
            if (!dateTime.HasValue)
            {
                const string DATE_SEARCH_PATTERN = "=**!AA6";

                xl.Range? findedCell = consinmentWorksheet.UsedRange.Find(What: DATE_SEARCH_PATTERN);

                if (findedCell is null)
                {
                    throw new System.Data.DataException($"Неверный формат листа excel. Дата не была найдена в стандартной ячейке \"{DEFAULT_DATE_ADDRESS}\". Так же не удалось найти ячейку по паттерну поиска \"{DATE_SEARCH_PATTERN}\"");
                }

                dateTime = (DateTime)findedCell.Value;
            }

            return DateOnly.FromDateTime(dateTime.Value);
        }

        private string? GetDestination(xl.Worksheet consinmentWorksheet)
        {
            const string SEARCH_PATTERN = "Кухня";

            xl.Range? findedCell = consinmentWorksheet.UsedRange.Find(What: SEARCH_PATTERN);
            if (findedCell is null)
                return default;

            string findedCellValue = Convert.ToString(findedCell.Value);

            while (findedCellValue.IndexOf("  ") > -1)
                findedCellValue = findedCellValue.Replace("  ", " ");

            string[] splittedValue = findedCellValue.Split(' ');
            if (splittedValue.Length == 4)
                return splittedValue[3];
            else
                return findedCellValue;
        }

        private int GetPersonsCount(xl.Worksheet consinmentWorksheet)
        {
            const string SEARCH_PATTERN = "Количество питающихся";

            xl.Range? findedCell = consinmentWorksheet.UsedRange.Find(What: SEARCH_PATTERN);
            if (findedCell is null)
                throw new System.Data.DataException($"Не удалось найти ячейку с кол-вом питающихся по паттерну \"{SEARCH_PATTERN}\"");

            return Convert.ToInt32(findedCell.Offset[0, 1].Value);
        }
    }
}
