#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using AutoLedgeBook.Data.Abstractions;


using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.ExcelConsinments.Underweight;


/// </summary>
/// <summary>
/// ДМТ накладная.
public class UnderweightConsinmentNote : ExcelConsinmentNoteBase
{
    private readonly UnderweightConsinmentNoteDescription _description;
    private readonly xl.Worksheet _consinmentWorksheet;

    internal UnderweightConsinmentNote(xl.Worksheet underweightConsinmentNote)
    {
        _consinmentWorksheet = underweightConsinmentNote;
        _description = new UnderweightConsinmentNoteDescription(_consinmentWorksheet);
        Products = new UnderweightAccountingProductCollection(_consinmentWorksheet);

    }


    public override string Number => _description.Number;

    public override DateOnly Day => _description.Day;

    public override IReadOnlyConsinmentDescription Description => _description;

    public override IAccountingProductCollection<IReadOnlyAccountingProduct> Products { get; }




    private class UnderweightAccountingProductCollection : IAccountingProductCollection<IReadOnlyAccountingProduct>
    {
        private readonly xl.Worksheet _consinmentWorksheet;
        private readonly IDictionary<string, xl.Range> _productNameToCell;

        public UnderweightAccountingProductCollection(xl.Worksheet consinmentWorksheet)
        {
            _consinmentWorksheet = consinmentWorksheet;
            _productNameToCell = CreateProductNameToCell(consinmentWorksheet);
        }

        public IReadOnlyAccountingProduct this[string productName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(productName))
                    throw new ArgumentNullException(nameof(productName));

                xl.Range? productNameCell;
                if (_productNameToCell.TryGetValue(productName, out productNameCell))
                    return BuildAccountingProduct(productNameCell);
                else
                    throw new KeyNotFoundException($"Продукт с наименованием \"{productName}\" не найден");
            }
        }

        public int Count => _productNameToCell.Count;

        public IEnumerator<IReadOnlyAccountingProduct> GetEnumerator()
            => _productNameToCell.Select(p => BuildAccountingProduct(p.Value)).GetEnumerator();

        public bool ContainsProduct(string productName)
            => GetProductNames().Contains(productName);

        public string[] GetProductNames()
            => _productNameToCell.Keys.ToArray();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();


        private IReadOnlyAccountingProduct BuildAccountingProduct(xl.Range productNameCell)
        {
            xl.Range unitsCell = productNameCell.Offset[0, 1];
            xl.Range valueCell = productNameCell.Offset[0, 6];
            return new ReadOnlyAccountingProduct(productNameCell.Name, valueCell.Value, ParseUnits(unitsCell.Value));
        }

        private ProductUnits ParseUnits(string unitsString)
        {
            string normalizedUnitsString = unitsString.ToLower().Trim();

            for (var i = 0; i < normalizedUnitsString.Length; i++)
            {
                if (!char.IsLetter(normalizedUnitsString[i]))
                {
                    normalizedUnitsString = normalizedUnitsString.Remove(i, 1);
                    i--;
                }
            }

            return normalizedUnitsString switch
            {
                "кг" => ProductUnits.Kilo,
                "л" => ProductUnits.Kilo,
                "шт" => ProductUnits.Pcs,
                _ => throw new InvalidDataException()
            };
        }

        private IDictionary<string, xl.Range> CreateProductNameToCell(xl.Worksheet consinmentWorksheet)
        {
            xl.Range signatureCell = consinmentWorksheet.UsedRange.Find(What: "№ п/п");
            xl.Range currentProductNameCell = signatureCell.Offset[1, 2];
            Dictionary<string, xl.Range> productNameToCell = new Dictionary<string, xl.Range>();
            do
            {
                productNameToCell[(string)currentProductNameCell.Value] = currentProductNameCell;
            } while (currentProductNameCell.Value != null);
            return productNameToCell;
        }
    }
}

public class UnderweightConsinmentNoteDescription : IReadOnlyConsinmentDescription
{
    private const string DESTINATION_CELL_ADDRESS = "F4",
                         CONSINMENT_NUMBER_CELL_ADDRESS = "A5",
                         PERSONS_COUNT_CELL_ADDRESS = "C12",
                         CONSINMENT_DATE_CELL_ADDRESS = "D8";

    private readonly xl.Worksheet _consinmentWorksheet;

    internal UnderweightConsinmentNoteDescription(xl.Worksheet consinmentWorksheet)
    {
        _consinmentWorksheet = consinmentWorksheet;

        Destination = ParseDestination(Convert.ToString(_consinmentWorksheet.Range[DESTINATION_CELL_ADDRESS].Value));
        Number = ParseConsinmentNumber(Convert.ToString(_consinmentWorksheet.Range[CONSINMENT_NUMBER_CELL_ADDRESS].Value));
        PersonsCount = Convert.ToInt32(_consinmentWorksheet.Range[PERSONS_COUNT_CELL_ADDRESS].Value);
        Day = ParseDate(_consinmentWorksheet.Range[CONSINMENT_DATE_CELL_ADDRESS].Value);
    }

    public bool IsReadOnly => true;

    public string Destination { get; }

    public string Number { get; }

    public int PersonsCount { get; }

    public string Type => "ДМТ";

    public DateOnly Day { get; }


    private string ParseDestination(string destinationCellValue)
    {
        string destination = destinationCellValue.Replace("  ", "");
        string[] splittedDestination = destination.Split(' ');
        return splittedDestination.Last();
    }

    private string ParseConsinmentNumber(string consinmentNumberCellValue)
    {
        string consinmentNumber = consinmentNumberCellValue.Replace("  ", " ");
        return consinmentNumber.Split(' ').Last();
    }

    private DateOnly ParseDate(object dateCellValue)
    {
        DateTime? date = dateCellValue as DateTime?;
        if (!date.HasValue)
            throw new ArgumentException("Неверное значение ячейки с датой накладной");

        return new DateOnly(date.Value.Year, date.Value.Month, date.Value.Day);
    }
}

// TODO: implement this
public class UnderweightProductsEnumerator : ExcelProductsEnumerator<IReadOnlyAccountingProduct>
{
    public UnderweightProductsEnumerator(xl.Range productsRange) : base(productsRange)
    {

    }

    protected override IReadOnlyAccountingProduct Build(xl.Range productRow)
    {
        throw new NotImplementedException();
    }

    protected override double GetProductValue(xl.Range productRow)
    {
        throw new NotImplementedException();
    }
}