using System.Diagnostics;

using AutoLedgeBook.Data.CanteenBook.Infrastructure;


namespace AutoLedgeBook.Data.CanteenBook;

public class CanteenBookConsinmentBuilder
{
    private readonly ExcelCanteenBook _parentBook;
    private readonly int _buildItRow;

    private CanteenBookConsinmentsProductBuilder? _productsBuilder;
    private int? _personsCount;
    private string? _destination;
    private string? _consinmentType;
    private string? _consinmentNumber;
    
    internal CanteenBookConsinmentBuilder(ExcelCanteenBook parentBook, int buildItRow)
    {
        _parentBook = parentBook;
        _buildItRow = buildItRow;
    }

    public CanteenBookConsinmentBuilder SetConsinmentNumber(string consinmentNumber)
    {
        _consinmentNumber = consinmentNumber;
        return this;
    }

    public CanteenBookConsinmentBuilder SetPersonsCount(int personsCount)
    {
        if (personsCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(personsCount));

        _personsCount = personsCount;
        return this;
    }

    public CanteenBookConsinmentBuilder SetDestination(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentNullException(nameof(destination));
        _destination = destination;
        return this;
    }

    public CanteenBookConsinmentBuilder SetType(string consinmentType)
    {
        if (string.IsNullOrWhiteSpace(consinmentType))
            throw new ArgumentNullException(nameof(consinmentType));
        _consinmentType = consinmentType;
        return this;
    }

    public CanteenBookConsinmentBuilder SetProducts(Action<CanteenBookConsinmentsProductBuilder> productsBuildAction)
    {
        CanteenBookConsinmentsProductBuilder productsBuilder = new CanteenBookConsinmentsProductBuilder(_parentBook);
        productsBuildAction(productsBuilder);
        _productsBuilder = productsBuilder;
        return this;
    }

    public ExcelCanteenConsinment Build()
    {
        if (string.IsNullOrWhiteSpace(_consinmentNumber))
            throw new Exception("Номер накладной не может быть пустым");

        if (_parentBook.ConsinmentsMetaCollection.ContainsMetadata(_consinmentNumber))
            throw new InvalidOperationException($"Номер накладной \"{ _consinmentNumber }\"уже занят");
        
        CanteenBookConsinmentMetadata? metadata = OverwriteMetadata();
        
        DateOnly date = _parentBook.GetDateByRowIndex(_buildItRow);

        ExcelCanteenConsinment consinment = new ExcelCanteenConsinment(_parentBook, date, _buildItRow, metadata);
        if (_productsBuilder is not null)
        {
            consinment.Products.ClearAll();

            foreach (KeyValuePair<string, double> productNameToValue in _productsBuilder.Build())
            {
                ExcelCanteenProduct product = consinment.Products.FirstOrDefault(p => p.Name == productNameToValue.Key)!;
                Debug.Assert(product is not null);
                product.Value = productNameToValue.Value;
                product.ConsinmentNumber = _consinmentNumber;
            }
        }
        
        consinment.Description = new ExcelCanteenConsinment.CanteenConsinmentMetaDescription(metadata);
        return consinment;
    }

    private CanteenBookConsinmentMetadata OverwriteMetadata()
    {
        CanteenBookConsinmentMetadata? metadata = _parentBook.ConsinmentsMetaCollection.GetByConsinmentRowIndex(_buildItRow);
        if (metadata is not null)
            metadata.Delete();
        metadata = _parentBook.ConsinmentsMetaCollection.CreateNew(_consinmentNumber!);
        metadata.RowIndex = _buildItRow;
        metadata.Destination = _destination!;
        metadata.Type = _consinmentType!;
        if (_personsCount.HasValue)
            metadata.PersonsCount = _personsCount!.Value;
        return metadata;
    }

}
