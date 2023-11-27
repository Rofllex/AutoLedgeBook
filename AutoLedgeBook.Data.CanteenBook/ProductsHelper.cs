using System.Collections;
using System.Diagnostics;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook;

/// <summary>
///     Помощник создания экземпляров классов продуктов.
/// </summary>
internal class ProductsHelper
{
    /// <summary>
    ///     Получить продукты с листа.
    /// </summary>
    /// <param name="dataWorksheet"></param>
    /// <param name="canteenProducts"></param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    public static ExcelCanteenProduct[] GetProducts(ExcelCanteenConsinment parentConsinment, xl.Worksheet dataWorksheet, IReadOnlyCollection<CanteenProduct> canteenProducts, int rowIndex)
    {
        using IEnumerator<ExcelCanteenProduct> productsEnumerator = new ExcelCanteenProductsEnumerator(parentConsinment, dataWorksheet, canteenProducts, rowIndex);
        ExcelCanteenProduct[] products = new ExcelCanteenProduct[canteenProducts.Count];

        var productIndex = -1;
        while (productsEnumerator.MoveNext())
            products[++productIndex] = productsEnumerator.Current;
        // По завершению цикла, значение должно быть равно кол-ву продуктов в массиве.
        Debug.Assert(productIndex + 1 == products.Length, "Кол-во перечисленных продуктов не соответствует кол-ву заявленных.."); 

        return products;
    }

    /// <summary>
    ///     Перечислитель продуктов.
    /// </summary>
    /// <remarks>
    ///     Данный перечислитель на каждую итерацию создает новый экземпляр объекта продукта.
    ///     От сложной инициализации(нескольких операций за раз) эксель может охуевать и в рандомном месте(где к нему обращаешься), выкидывать исключение
    ///     ЭТО ЗЛО. НАДО ОСТАНОВИТЬ. НЕ ТРОГАЙ ЭТО. ЭТО ВОНЯЕТ ХУЯМИ!
    /// </remarks>
    private class ExcelCanteenProductsEnumerator : IEnumerator<ExcelCanteenProduct>
    {
        private readonly IEnumerator<CanteenProduct> _canteenProductsEnumerator;
        private readonly int _rowIndex;
        private readonly xl.Worksheet _dataWorksheet;

        private ExcelCanteenProduct? _current = null;
        private readonly ExcelCanteenConsinment _parentConsinment;

        internal ExcelCanteenProductsEnumerator(ExcelCanteenConsinment parentConsinment, xl.Worksheet dataWorksheet, IEnumerable<CanteenProduct> productsEnumerable, int rowIndex)
        {
            _canteenProductsEnumerator = productsEnumerable.GetEnumerator();
            _dataWorksheet = dataWorksheet;
            _rowIndex = rowIndex;
            _parentConsinment = parentConsinment;
        }

        public ExcelCanteenProduct Current => _current ?? throw new InvalidOperationException($"Перед использованием необходимо вызвать метод \"{ nameof(MoveNext) }\"");

        object IEnumerator.Current => Current;

        public void Dispose() => _canteenProductsEnumerator.Dispose();


        public bool MoveNext()
        {
            if (!_canteenProductsEnumerator.MoveNext())
                return false;
            CanteenProduct currentProduct = _canteenProductsEnumerator.Current;
            xl.Range consumptionCell = currentProduct.GetConsumptionCell(_dataWorksheet, _rowIndex);
            _current = new ExcelCanteenProduct(_parentConsinment, currentProduct, consumptionCell);
            //Thread.Sleep(TimeSpan.FromMilliseconds(50));
            return true;
        }

        public void Reset()
        {
            _current = null;
            _canteenProductsEnumerator.Reset();
        }
    }
}
