using System.Runtime.InteropServices;

using AutoLedgeBook.Data.Abstractions;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.CanteenBook
{
    /// <summary>
    ///     Базовый класс реализующий необходимый функционал для книги Excel`я.
    /// </summary>
    public abstract class XlConsinmentsBook : IConsinmentsBook, ISaveable
    {
        protected internal XlConsinmentsBook(xl.Workbook workbook)
        {
            if (workbook == null)
                throw new ArgumentNullException(nameof(workbook));
            workbook.BeforeSave += Workbook_BeforeSave;
            workbook.AfterSave += Workbook_AfterSave;
            
            Workbook = workbook;
        }

        ~XlConsinmentsBook()
        {
            if (Disposed)
                return;
            else
                Dispose();
        }

        public abstract IConsinmentNote this[string consinmentNumber] { get; }

        public abstract bool ContainsConsinment(string consinmentNumber);

        public abstract IConsinmentsCollection GetConsinmentsByDate(DateOnly date);

        IReadOnlyConsinmentsCollection IReadOnlyConsinmentsBook.GetConsinmentsByDate(DateOnly day) => GetConsinmentsByDate(day);

        public abstract string[] GetProductNames();
        

        /// <summary>
        ///     Если объект был освобожден.
        /// </summary>
        public bool Disposed { get; private set; }

        public void Dispose()
        {
            lock (DisposeMutex)
            {
                if (Disposed)
                    throw new ObjectDisposedException(null);
                _saveWaitHandle.WaitOne();
                DisposeProtected();

                Workbook.Close(SaveChanges: false);
                Marshal.ReleaseComObject(Workbook);
            }
        }


        #region ISaveable
        public void Save()
        {
            Workbook.Save();
            _saveWaitHandle.WaitOne();
        }

        public void SaveAs(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            Workbook.SaveAs(Filename: filePath);
            _saveWaitHandle.WaitOne();
        }

        public abstract void SaveAs(string filePath, ISaveSettings saveSettings);
        
        #endregion

        
        /// <summary>
        /// Мьютекс синхронизации освобождения ресурсов.
        /// </summary>
        protected readonly object DisposeMutex = new object();
        
        /// <summary>
        ///     Рабочая книга, которая открыта.
        /// </summary>
        protected xl.Workbook Workbook { get; }
        
        /// <summary>
        ///  Освободить ресурсы.
        /// </summary>
        protected abstract void DisposeProtected();


        public IReadOnlyList<DateOnly> Dates 
        {
            get
            {
                if (_dates is null)
                    _dates = GetDates();
                return _dates;
            }
        }
        
        private IReadOnlyList<DateOnly>? _dates = null;



        protected abstract IReadOnlyList<DateOnly> GetDates();


        private readonly EventWaitHandle _saveWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);

        private void Workbook_AfterSave(bool Success)
            => _saveWaitHandle.Set();

        private void Workbook_BeforeSave(bool SaveAsUI, ref bool Cancel)
            => _saveWaitHandle.Reset();
    
        
    }
}
