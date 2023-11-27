using System.Collections.Immutable;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Data.CanteenBook;

public class CanteenBookConsinmentsProductBuilder
{
    private readonly ExcelCanteenBook _canteenBook;

    private readonly Dictionary<string, double> _productToValue = new();

    internal CanteenBookConsinmentsProductBuilder(ExcelCanteenBook canteenBook)
    {
        _canteenBook = canteenBook;
    }

    public string[] ProductNames => _canteenBook.GetProductNames();

    public CanteenBookConsinmentsProductBuilder SetProductValue(string productName, double value)
    {
        if (_canteenBook.Products.FirstOrDefault(p => p.Name == productName) == default)
            throw new Exception($"Продукт с наименованием \"{ productName }\" не найден");

        _productToValue[productName] = value;
        return this;
    }
    

    public IReadOnlyDictionary<string, double> Build()
        => _productToValue.ToImmutableDictionary();

    private record class VirtualAccountingProduct : IAccountingProduct
    {
        private readonly CanteenProduct _product;
        private double _value;

        public VirtualAccountingProduct(CanteenProduct product, double value = 0)
        {
            _product = product;
            Value = value;
        }

        public double Value 
        {
            get => _value;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _value = value;
            }
        }

        public string Name => _product.Name;

        public ProductUnits Units => _product.Units;

        double IReadOnlyAccountingProduct.Value => Value;
    }
}