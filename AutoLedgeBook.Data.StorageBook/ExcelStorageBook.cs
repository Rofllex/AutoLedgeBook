using System.Linq;
using System.Diagnostics;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using AutoLedgeBook.Data.Excel.LowLevel;
using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

#nullable enable

namespace AutoLedgeBook.Data.StorageBook
{
    /// <summary>
    ///     Учетная книжка в формате excel.
    /// </summary>
    /// <inheritdoc cref="IReadOnlyConsinmentsBook"/>
    public class ExcelStorageBook : IConsinmentsBook, ISaveable
    {
        /// <summary>
        ///     Загрузить из файла.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        ///     Экземпляр учетной книжки.
        /// </returns>
        /// <exception cref="FileNotFoundException">Если не удалось найти файл</exception>
        /// <inheritdoc cref="ExcelStorageBook.ExcelLedgeBook(xl.Workbook)"/>
        public static ExcelStorageBook FromFile(string filePath, ExcelApplicationContext? applicationContext = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();
            if (applicationContext == null)
                applicationContext = new ExcelApplicationContext();
            xl.Workbook ledgeWorkbook = applicationContext.OpenWorkbook(filePath);
            return new ExcelStorageBook(ledgeWorkbook, applicationContext);
        }


        private readonly string[] _productNames;
        private readonly Dictionary<DateOnly, ExcelLedgeConsinmentConsumptionCollection> _dateToConsumptionCollection = new();
        
        private EventWaitHandle _saveWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);

        private readonly ExcelApplicationContext _xlContext;

        /// <exception cref="InvalidDataException">Если неверный формат документа</exception>
        private ExcelStorageBook(xl.Workbook ledgeWorkbook, ExcelApplicationContext applicationContext)
        {
            Workbook = ledgeWorkbook ?? throw new ArgumentNullException(nameof(ledgeWorkbook));
            Worksheet = (xl.Worksheet)Workbook.Worksheets[1];
            
            this._xlContext = applicationContext;
            
            FirstColumnIndexInDayToDate = ImmutableSortedDictionary.CreateRange(GetColumnIndexToDate("E4"));
            Dates = FirstColumnIndexInDayToDate.Select(d => d.Value).ToImmutableList();
            Products = GetLedgeProducts(Worksheet.Range["B7:C127"]);
            
            _productNames = Products.Select(p => p.ProductName).ToArray();

            FirstProductRowIndex = Products.First().Row;
            LastProductRowIndex = Products.Last().Row;

            TotalWeightRowIndex = LastProductRowIndex + 1;
            TotalPcsRowIndex = LastProductRowIndex + 2;

            ConsinmentHeaderRowIndex = FirstProductRowIndex - 1;

            Workbook.BeforeSave += (bool _, ref bool __) => _saveWaitHandle.Reset();
            Workbook.AfterSave += (bool _) => _saveWaitHandle.Set();

            
        }

        /// <summary>
        ///     Список дат поддерживаемых книгой.
        /// </summary>
        public IImmutableList<DateOnly> Dates { get; private set; }

        /// <summary>
        ///     Получить накладную по номеру накладной.
        /// </summary>
        /// <param name="consinmentNumber">Номер накладной</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ConsinmentNotFoundException"></exception>
        public ExcelLedgeConsinment this[string consinmentNumber]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(nameof(consinmentNumber)))
                    throw new ArgumentNullException(nameof(consinmentNumber));

                xl.Range headersRow = Worksheet.Range[$"{ConsinmentHeaderRowIndex}:{ConsinmentHeaderRowIndex}"];
                xl.Range? findedCell = headersRow.Find(What: consinmentNumber);
                if (findedCell is null)
                    throw new ConsinmentNotFoundException(consinmentNumber);
                (int r, int c) firstCellAddress = (findedCell.Row, findedCell.Column);
                do
                {
                    int? column = FirstColumnIndexInDayToDate.Keys.LastOrDefault(columnIndex => columnIndex <= findedCell.Column);
                    Debug.Assert(column.HasValue);
                    ExcelLedgeConsinment? consumption = new ExcelLedgeConsinment(this, FirstColumnIndexInDayToDate[column.Value], findedCell.Column);
                    if (consumption.Number == consinmentNumber)
                        return consumption;

                    findedCell = headersRow.Find(What: consinmentNumber, After: findedCell);
                    if (findedCell.Row == firstCellAddress.r && findedCell.Column == firstCellAddress.c)
                        throw new ConsinmentNotFoundException(consinmentNumber);
                } while (true);
            }
        }

        public ExcelLedgeConsinmentConsumptionCollection GetConsinmentsByDate(DateOnly date)
        {
            if (!Dates.Contains(date))
                throw new ArgumentException($"Накладные по дате \"{ date }\" не найдены");
            ExcelLedgeConsinmentConsumptionCollection? consinmentConsumptions;
            if (!_dateToConsumptionCollection.TryGetValue(date, out consinmentConsumptions))
            { 
                if (_dateToConsumptionCollection.ContainsKey(date))
                    return _dateToConsumptionCollection[date];

                KeyValuePair<int, DateOnly> columnToDateTime = FirstColumnIndexInDayToDate.First(p => p.Value == date);
            
                xl.Range consinmentHeaders = GetHeadersRangeByDateCell(Worksheet.Cells[4, columnToDateTime.Key]);

                consinmentConsumptions = _dateToConsumptionCollection[date] = new ExcelLedgeConsinmentConsumptionCollection(this, date, consinmentHeaders);
            }

            return consinmentConsumptions;

        }
        
        public bool ContainsConsinment(string consinmentNumber)
        {
            IConsinmentsBook book = this;
            foreach (DateOnly date in Dates)
            {
                IConsinmentsCollection consumptions = book.GetConsinmentsByDate(date);
                
                IReadOnlyConsinmentNote? consinment = ((IReadOnlyConsinmentsCollection)consumptions).FirstOrDefault(c => c.Number == consinmentNumber);
                if (consinment is not null)
                    return true;
            }
            return false;
        }
        
        public string[] GetProductNames()
            => _productNames;

        #region implicit implementation

        IConsinmentNote IConsinmentsBook.this[string consinmentNumber] => this[consinmentNumber];

        IConsinmentsCollection IConsinmentsBook.GetConsinmentsByDate(DateOnly date) => GetConsinmentsByDate(date);

        IReadOnlyConsinmentsCollection IReadOnlyConsinmentsBook.GetConsinmentsByDate(DateOnly day) => GetConsinmentsByDate(day);

        IReadOnlyList<DateOnly> IReadOnlyConsinmentsBook.Dates
            => Dates;
        
        #endregion

        #region ISaveable
        public void SaveAs(string filePath, ISaveSettings saveSettings)
            => SaveAs(filePath);

        public void SaveAs(string filePath)
            => Workbook.SaveAs(filePath);

        public void Save()
        {
            Workbook.Save();
            _saveWaitHandle.WaitOne();
        }
        #endregion

        #region internal properties

        /// <summary>
        ///     Сопоставление первого столбца накладной к дате.
        /// </summary>
        internal IImmutableDictionary<int, DateOnly> FirstColumnIndexInDayToDate { get; private set; }

        /// <summary>
        ///     Рабочий лист
        /// </summary>
        internal xl.Worksheet Worksheet { get; init; }

        /// <summary>
        ///     Рабочая книга.
        /// </summary>
        internal xl.Workbook Workbook { get; init; }

        /// <summary>
        ///     Коллекция абстрактной модели продуктов.
        /// </summary>
        internal IReadOnlyCollection<LedgeProduct> Products { get; private set; }

        /// <summary>
        ///     Индекс строки первого продукта.
        /// </summary>
        internal int FirstProductRowIndex { get; private set; }
        
        /// <summary>
        ///     Индекс строки последнего продукта.
        /// </summary>
        internal int LastProductRowIndex { get; private set; }
        
        /// <summary>
        ///     Строка с суммарным кол-вом шт.
        /// </summary>
        internal int TotalPcsRowIndex { get; private set; }

        /// <summary>
        ///     Строка с суммарным весом
        /// </summary>
        internal int TotalWeightRowIndex { get; private set; }

        /// <summary>
        ///     Номер строки с заголовком накладной
        /// </summary>
        internal int ConsinmentHeaderRowIndex { get; private set; }

        /// <summary>
        ///     Получить столбец значений продуктов. В одну строку одна ячейка.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        internal xl.Range GetProductValuesRangeByColumn(int columnIndex)
        {
            xl.Range upperCell = Worksheet.Cells[RowIndex: FirstProductRowIndex, ColumnIndex: columnIndex];
            xl.Range lowerCell = Worksheet.Cells[RowIndex: LastProductRowIndex, ColumnIndex: columnIndex];
            return Worksheet.Range[upperCell, lowerCell];
        }

        /// <summary>
        ///     Получить ячейки контрольных значений.
        /// </summary>
        /// <remarks>
        ///     Первым значением является ячейка суммарного веса накладной.
        ///     Вторым значением является ячейка суммарного кол-ва продуктов в шт.
        /// </remarks>
        internal (xl.Range totalWeightCell, xl.Range totalPcsCell) GetControlCells(int columnIndex)
        {
            return (totalWeightCell: Worksheet.Cells[TotalWeightRowIndex, columnIndex],
                    totalPcsCell: Worksheet.Cells[TotalPcsRowIndex, columnIndex]);
        }

        /// <summary>
        ///     Получить ячейку заголовка накладной.
        /// </summary>
        /// <param name="columnIndex">Индекс столбца</param>
        /// <returns></returns>
        internal xl.Range GetConsinmentHeaderCell(int columnIndex)
            => Worksheet.Cells[ConsinmentHeaderRowIndex, columnIndex];


        #endregion
        
        #region IDisposable

        public void Dispose()
        {
            _saveWaitHandle.WaitOne();
            Workbook.Close(false);
            Marshal.ReleaseComObject(Workbook);
        }

        #endregion

        
        private IDictionary<int, DateOnly> GetColumnIndexToDate(string firstDateAddress)
        {
            xl.Range currentColumnCell = Worksheet.Range[firstDateAddress];
            SortedDictionary<int, DateOnly> columnIndexToDateTime = new();

            DateOnly firstDateTime = FromDateTime((DateTime)currentColumnCell.Value);

            while (currentColumnCell.Value != null)
            {
                object currentColumnCellValue = currentColumnCell.Value;
                DateTime? currentDateTime = currentColumnCellValue as DateTime?;
                if (currentDateTime.HasValue && currentDateTime.Value.Month == firstDateTime.Month)
                {
                    columnIndexToDateTime[currentColumnCell.Column] = FromDateTime(currentDateTime.Value);
                }

                xl.Range nextCell = currentColumnCell.Offset[0, 1];
                if (nextCell.Value == null)
                    currentColumnCell = nextCell;
                currentColumnCell = currentColumnCell.Offset[0, 1];
            }
            return columnIndexToDateTime;
        }

        private static DateOnly FromDateTime(DateTime date)
            => new DateOnly(date.Year, date.Month, date.Day);

        private IDictionary<int, string> GetRowIndexToProductName(string firstProductCellAddress)
        {
            xl.Range currentProductNameCell = Worksheet.Range[firstProductCellAddress];
            SortedDictionary<int, string> rowIndexToProductName = new SortedDictionary<int, string>();
            while (currentProductNameCell.Row != 143)
            {
                object currentProductNameCellValue = currentProductNameCell.Value;
                if (currentProductNameCellValue != null
                     && currentProductNameCellValue.GetType() == typeof(string)
                     && !string.IsNullOrWhiteSpace((string)currentProductNameCellValue))
                {
                    rowIndexToProductName[currentProductNameCell.Row] = (string)currentProductNameCell.Value;
                }

                currentProductNameCell = currentProductNameCell.Offset[1, 0];
            }
            return rowIndexToProductName;
        }

        /// <summary>
        ///     Получить диапазон ячеек заголовков по ячейке даты.
        /// </summary>
        private xl.Range GetHeadersRangeByDateCell(xl.Range dateCell)
        {
            xl.Range firstConsinmentHeader = dateCell.Offset[1].Offset[0,3].Offset[1,0];
            xl.Range lastConsinmentHeader = firstConsinmentHeader.Offset[0, 1];
            while (true)
            {
                xl.Range nextCell = lastConsinmentHeader.Offset[0, 1];
                string nextCellValue = Convert.ToString(nextCell.Value);
                if (nextCellValue?.IndexOf("()") > -1)
                    break;
                lastConsinmentHeader = nextCell;
            }
            return Worksheet.Range[firstConsinmentHeader, lastConsinmentHeader];
        }

        /// <summary>
        ///     Получить остаток продуктов на конец дня по дате.
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        internal ReadOnlyExcelLedgeAccountingProductCollection GetRemainProducsByDate(DateOnly date)
        {
            int columnIndex = FirstColumnIndexInDayToDate.First(p => p.Value == date).Key;
            xl.Range dateCell = Worksheet.Cells[4, columnIndex];
            //xl.Range dbg = dateCell.Offset[RowOffset: 1];
            xl.Range remainProductsHeaderCell = dateCell.Offset[RowOffset: 1];
            for (var i = 0; i < 4; i++)
                remainProductsHeaderCell = remainProductsHeaderCell.Offset[0, 1];
            return new ReadOnlyExcelLedgeAccountingProductCollection(this, remainProductsHeaderCell.Column);
        }

        /// <summary>
        /// Получить расход продуктов за день
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal ReadOnlyExcelLedgeAccountingProductCollection GetProductsTotalConsumption(DateOnly date)
        {
            int columnIndex = FirstColumnIndexInDayToDate.First(p => p.Value == date).Key;
            xl.Range dateCell = Worksheet.Cells[4, columnIndex];
            //xl.Range dbg = dateCell.Offset[RowOffset: 1];
            xl.Range remainProductsHeaderCell = dateCell.Offset[RowOffset: 1];
            for (var i = 0; i < 4; i++)
                remainProductsHeaderCell = remainProductsHeaderCell.Offset[0, 1];
            // Такое решение т.к. "всего расход" находится как правило на 1 столбец левее остатка продуктов. Костыльно ну а хуле, мне похуй :ъ
            return new ReadOnlyExcelLedgeAccountingProductCollection(this, remainProductsHeaderCell.Column - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productInfoRange">Диапазон представляющий из себя строку со значениями: наименование продукта - тип единиц измерения</param>
        /// <returns></returns>
        private IReadOnlyCollection<LedgeProduct> GetLedgeProducts(xl.Range productInfoRange)
        {
            List<LedgeProduct> productsList = new List<LedgeProduct>();

            foreach (xl.Range productInfoRow in productInfoRange.Rows)
            {
                string productName = Convert.ToString(productInfoRow.Cells[1, 1].Value);
                if (string.IsNullOrWhiteSpace(productName))
                    continue;
                string productUnitsString = Convert.ToString(productInfoRow.Cells[1, 2].Value);
                ProductUnits productUnits = ParseProductUnits(productUnitsString);
                productsList.Add(new LedgeProduct(productInfoRow.Row, productName, productUnits));
            }
            return productsList.AsReadOnly();
        }

        private ProductUnits ParseProductUnits(string productUnitsStr)
        {
            productUnitsStr = productUnitsStr.Trim().ToLower();
            for (var charIndex = 0; charIndex < productUnitsStr.Length; charIndex++)
            {
                if (!char.IsLetter(productUnitsStr[charIndex]))
                {
                    productUnitsStr = productUnitsStr.Remove(charIndex, 1);
                    charIndex--;
                }
            }

            return productUnitsStr switch
            {
                "л" => ProductUnits.Kilo,
                "кг" => ProductUnits.Kilo,
                "шт" => ProductUnits.Pcs,
                _ => throw new InvalidProgramException()
            };
        }
    }
}
