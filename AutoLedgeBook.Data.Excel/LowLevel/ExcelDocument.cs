using System.Runtime.InteropServices;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.Excel.LowLevel
{
    public abstract class ExcelDocument : IDisposable
    {
        private bool _disposed = false;
        private EventWaitHandle _saveWaitHandle = new(true, EventResetMode.ManualReset);

        protected ExcelDocument(xl.Workbook workbook)
        {
            workbook.BeforeSave += Workbook_BeforeSave;
            workbook.AfterSave += Workbook_AfterSave;
            
            Workbook = workbook;
        }

        ~ExcelDocument() => Dispose();
        
        
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            _saveWaitHandle.WaitOne();
            _saveWaitHandle.Dispose();

            DisposeProtected();

            xl.Application app = Workbook.Application;
            bool appDisplayAlerts = app.DisplayAlerts;

            app.DisplayAlerts = false;
            Workbook.Close(SaveChanges: false);

            Marshal.ReleaseComObject(Workbook);
            Workbook = null!;

            GC.SuppressFinalize(this);

            app.DisplayAlerts = appDisplayAlerts;
        }

        protected xl.Workbook Workbook { get; private set; }

        protected virtual void DisposeProtected() { }

                
        private void Workbook_AfterSave(bool Success) => _saveWaitHandle.Set();
        

        private void Workbook_BeforeSave(bool SaveAsUI, ref bool Cancel) => _saveWaitHandle.Reset();
        
    }
}
