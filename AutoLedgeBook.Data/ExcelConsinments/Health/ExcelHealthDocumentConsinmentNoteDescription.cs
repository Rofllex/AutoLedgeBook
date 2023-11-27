#nullable enable

using System;
using System.IO;
using System.Linq;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments.Health;

public class ExcelHealthDocumentConsinmentNoteDescription : IReadOnlyConsinmentDescription
{
    public ExcelHealthDocumentConsinmentNoteDescription(xl.Worksheet consinmentWorksheet)
    {
        Number = GetConsinmentNumber(consinmentWorksheet);
        Date = GetConsinmentDate(consinmentWorksheet);
        Destination = GetDestination(consinmentWorksheet) ?? string.Empty;
        PersonsCount = GetPersonsCount(consinmentWorksheet);
    }

    public string Number { get; init; }

    public DateOnly Date { get; init; }

    public string Destination { get; init; }

    public int PersonsCount { get; init; }

    public string Type => "Норма № 5";

    private string GetConsinmentNumber(xl.Worksheet consinmentWorksheet)
    {
        xl.Range? findedCell = consinmentWorksheet.UsedRange.Find(What: "Накладная №");
        if (findedCell is null)
            throw new InvalidDataException("Не удалось найти номер накладной");

        string cellValue = Convert.ToString(findedCell.Value).Replace('а', 'a');
        int firstDigitIndex = s_GetFirstDigitIndex(cellValue);

        if (firstDigitIndex == -1)
            throw new InvalidDataException("Неверный формат номера накладной");

        cellValue = cellValue.Remove(0, firstDigitIndex);

        char[] allowedChars = { 'a', '/' };

        return new string(cellValue.Where(c => char.IsDigit(c) || allowedChars.Contains(c)).ToArray());
    }

    private static int s_GetFirstDigitIndex(string inputString)
    {
        for (var i = 0; i < inputString.Length; i++)
        {
            if (char.IsDigit(inputString[i]))
            {
                return i;
            }
        }
        return -1;
    }

    private DateOnly GetConsinmentDate(xl.Worksheet consinmentWorksheet)
    {
        xl.Range? dateCell = s_FindCellByValue(consinmentWorksheet.UsedRange, "2023", (c) =>
        {
            return c.Value is DateTime;
        });

        if (dateCell is null)
            throw new InvalidDataException("Не удалось найти дату на листе с накладной");

        return DateOnly.FromDateTime((DateTime)dateCell.Value);
    }

    private static xl.Range? s_FindCellByValue(xl.Range range, string whatFind, Predicate<xl.Range> predicate)
    {
        xl.Range? findedCell = range.Find(What: whatFind);

        if (findedCell is null)
            return findedCell;
        (int Column, int Row) firstFindedCell = (findedCell.Column, findedCell.Row);

        do
        {
            if (predicate(findedCell))
                return findedCell;

            findedCell = range.Find(What: whatFind, After: findedCell);
        } while (findedCell.Column != firstFindedCell.Column && findedCell.Row != firstFindedCell.Row);

        return null;
    }

    private string? GetDestination(xl.Worksheet consinmentWorksheet)
    {
        xl.Range? findedCell = consinmentWorksheet.UsedRange.Find(What: "Получил");
        if (findedCell is null)
            return null;

        string cellValue = Convert.ToString(findedCell.Value);
        while (cellValue.IndexOf("  ") > -1)
            cellValue = cellValue.Replace("  ", " ");

        int firstDigitIndex = s_GetFirstDigitIndex(cellValue);
        if (firstDigitIndex < 0)
            return null;

        cellValue = cellValue.Remove(0, firstDigitIndex);
        return new string(cellValue.Where(c => char.IsDigit(c) || c == '/').ToArray());
    }

    private int GetPersonsCount(xl.Worksheet consinmentWorksheet)
    {
        xl.Range? findedCell = consinmentWorksheet.UsedRange.Find(What: "Питающихся");
        if (findedCell is null)
            throw new InvalidDataException("Не удалось найти значение кол-во питающихся");

        return Convert.ToInt32(findedCell.Offset[0, 1].Value);
    }
}