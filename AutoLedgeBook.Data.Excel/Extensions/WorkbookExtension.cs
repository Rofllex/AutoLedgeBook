
using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.Excel.Extensions
{
    /// <summary>
    ///     Расширение рабочей книги.
    /// </summary>
    public static class WorkbookExtension
    {
        /// <summary>
        ///     Безопасное сохранение. 
        /// </summary>
        /// <param name="workbook">Рабочая книга</param>
        /// <remarks>
        ///     По умолчанию при вызове в рабочей книге экселя метода <see cref="xl.WorkbookClass.Save"/> выполняется асинхронно.
        ///     <br />При окончании сохранения эксель вызывает событие <see cref="xl.WorkbookClass.AfterSave"/>
        ///     <br />Данное расширение синхронизирует вызов метода <c>Save</c>
        /// </remarks>
        public static void SyncSave(this xl.Workbook workbook)
        {
            s_SyncSave(workbook, wb => 
            {
                wb.Save();
            });
        }

        public static void SyncSaveAs(this xl.Workbook workbook, string fileName)
        {
            s_SyncSave(workbook, wb => 
            {
                wb.SaveAs(Filename: fileName);
            });
        }

        private static void s_SyncSave(xl.Workbook workbook, Action<xl.Workbook> saveAct)
        {
            using EventWaitHandle waitHandle = new(true, EventResetMode.ManualReset);

            workbook.BeforeSave += (bool _, ref bool __) => waitHandle.Reset();
            workbook.AfterSave += (_) => waitHandle.Set();

            saveAct(workbook);

            waitHandle.WaitOne();
        }
    }
}
