using System.Runtime.InteropServices;

using xl = Microsoft.Office.Interop.Excel;

namespace AutoLedgeBook.Data.Excel.Extensions
{
    public static class RangeExtension
    {
        /// <summary>
        ///     Освободить используемый диапазон после использования.
        /// </summary>
        /// <remarks>
        ///     Вызывает метод <see cref="Marshal.ReleaseComObject(object)"/>
        /// </remarks>
        public static void Release(this xl.Range range) => Marshal.ReleaseComObject(range);
    
        /// <summary>
        ///     Освободить используемый диапазон после использования.
        /// </summary>
        /// <param name="rangeAct">Функция, которую необходимо вызвать перед освобождением ресурса</param>
        public static void ReleaseAfter(this xl.Range range, Action<xl.Range> rangeAct)
        {
            rangeAct(range);
            range.Release();
        }

        /// <summary>
        ///     Освободить используемый диапазон после использования с возвратом значения.
        /// </summary>
        /// <param name="invokeFunc">Функция, которая вернет объект перед освобождением диапазона</param>
        /// <returns></returns>
        public static TObject ReleaseAfter<TObject>(this xl.Range range, Func<xl.Range, TObject> invokeFunc)
        {
            TObject invokeResult = invokeFunc(range);
            range.Release();
            return invokeResult;
        }
    }
}
