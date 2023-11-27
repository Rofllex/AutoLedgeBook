using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.CanteenBook.Infrastructure;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Накладная книги столовой.
///     Наследуется от <see cref="IReadOnlyConsinmentNote"/>
/// </summary>
public class ExcelCanteenConsinment : IConsinmentNote, IConsinmentsCollectionStatistics
{
    internal static bool CanInitialize(xl.Range consinmentRow)
        => consinmentRow.Cells[1, 1].Value is not null;


    private readonly int _rowIndex;

    private CanteenBookConsinmentMetadata? _consinmentMeta;
    private string? _consinmentNumber;
    private CanteenBookAccountingProductCollection? _productsCollection;
    private Lazy<RemaindCanteenBookAccountingProductCollection> _remainProducts;


    /// <summary>
    ///     Конструктор накладной книги столовой.
    /// </summary>
    /// <param name="parentBook">Родительская книга</param>
    /// <param name="day">День, на который ориентирована накладная</param>
    /// <param name="rowIndex">Индекс строки</param>
    /// <param name="consinmentMeta">
    /// </param>
    internal ExcelCanteenConsinment(ExcelCanteenBook parentBook, DateOnly day, int rowIndex, CanteenBookConsinmentMetadata? consinmentMeta)
    {
        _consinmentMeta = consinmentMeta;
        ParentBook = parentBook;
        _rowIndex = rowIndex;

        Day = day;

        _consinmentNumber = _GetConsinmentNumber();
        if (!string.IsNullOrWhiteSpace(_consinmentNumber))
        {
            if (ParentBook.ConsinmentsMetaCollection.ContainsMetadata(_consinmentNumber))
            {
                _consinmentMeta = ParentBook.ConsinmentsMetaCollection[_consinmentNumber];
            }
            else
            {
                _consinmentMeta = ParentBook.ConsinmentsMetaCollection.CreateNew(_consinmentNumber);
                _consinmentMeta.Type = ParentBook.Worksheet.Cells[_rowIndex, 1].Value;
            }

            Description = new CanteenConsinmentMetaDescription(_consinmentMeta);
        }
        else
        {
            xl.Range typeCell = parentBook.Worksheet.Cells[rowIndex, 1];
            Description = new CanteenConsinmentDescriptionWrapper2(typeCell);
        }

        _remainProducts = new Lazy<RemaindCanteenBookAccountingProductCollection>(() => 
        {
            return new RemaindCanteenBookAccountingProductCollection(ParentBook, _rowIndex);
        });
    }
    
    public ExcelCanteenBook ParentBook { get; }

    public DateOnly Day { get; }

    public string? Number => _consinmentNumber;

    public IConsinmentDescription? Description { get; internal set; }

    public CanteenBookAccountingProductCollection Products
    {
        get
        {
            if (_productsCollection is null)
                _productsCollection = new CanteenBookAccountingProductCollection(ParentBook, this, ParentBook.Products, _rowIndex);
            return _productsCollection;
        }
    }

    
    #region implicit implementation

    IAccountingProductCollection<IAccountingProduct> IConsinmentNote.Products => Products;
    IAccountingProductCollection<IReadOnlyAccountingProduct> IReadOnlyConsinmentNote.Products => Products;
    IReadOnlyConsinmentDescription? IReadOnlyConsinmentNote.Description => Description;

    IAccountingProductCollection<IReadOnlyAccountingProduct> IConsinmentsCollectionStatistics.RemainProducts => _remainProducts.Value;

    IAccountingProductCollection<IReadOnlyAccountingProduct> IConsinmentsCollectionStatistics.TotalProductsConsumption => throw new NotImplementedException();
    
    #endregion

    public double GetTotalProductsPcs() => Products.Where(p => p.Units == ProductUnits.Pcs).Sum(p => p.Value);

    public double GetTotalProductsWeight() => Products.Where(p => p.Units == ProductUnits.Kilo).Sum(p => p.Value);

    public bool CanBuild => string.IsNullOrWhiteSpace(Number);


    public CanteenBookConsinmentBuilder GetBuilder()
    {
        if (!CanBuild)
            throw new InvalidOperationException();
        return new CanteenBookConsinmentBuilder(ParentBook, _rowIndex);
    }

    public void Delete()
    {
        foreach (ExcelCanteenProduct accountingProduct in Products)
        {
            accountingProduct.Clear();
            Thread.Sleep(TimeSpan.FromMilliseconds(1));
        }

        _consinmentMeta?.Delete();
        _consinmentNumber = null;
    }

    internal int RowIndex => _rowIndex;

    private string? _GetConsinmentNumber()
    {
        if (_consinmentMeta is not null)
            return _consinmentMeta.ConsinmentNumber;

        ExcelCanteenProduct? product = Products.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.ConsinmentNumber));
        return product?.ConsinmentNumber ?? default;
    }

    /// <summary>
    ///     Обёртка над метаданными для создания накладной
    /// </summary>
    internal class CanteenConsinmentMetaDescription : IConsinmentDescription
    {
        private readonly CanteenBookConsinmentMetadata _metadata;
 
        public CanteenConsinmentMetaDescription(CanteenBookConsinmentMetadata metadata)
        {
            _metadata = metadata;
        }

        public string Destination
        {
            get => _metadata.Destination;
            set => _metadata.Destination = value;
        }

        public string Type 
        {
            get => _metadata.Type;
            set => _metadata.Type = value;
        }

        public int PersonsCount 
        {
            get => _metadata.PersonsCount;
            set => _metadata.PersonsCount = value;
        }
    }
        
    internal sealed class CanteenConsinmentDescriptionWrapper2 : IConsinmentDescription
    {
        private readonly xl.Range _typeCell;

        public CanteenConsinmentDescriptionWrapper2(xl.Range typeCell)
        {
            _typeCell = typeCell;
        }

        public string Destination
        {
            get
            {
                ThrowInvalidOperation();
                // До этой строки компилятор не дойдёт, но он об этом не знает...
                return string.Empty;
            }
            set => ThrowInvalidOperation();
        }

        public string Type 
        {
            get => _typeCell.Value;
            set => ThrowInvalidOperation();
        }

        public int PersonsCount
        {
            get
            {
                ThrowInvalidOperation();
                return default;
            }
            set => ThrowInvalidOperation();
        }

        private void ThrowInvalidOperation() => throw new InvalidOperationException("Редактирование данной накладной невозможно т.к. не указан номер накладной. Для редактирования воспользуйся билдером.");
    }
}
