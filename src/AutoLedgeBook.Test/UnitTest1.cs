using AutoLedgeBook.Data.ExcelConsinments;
using AutoLedgeBook.Data.ExcelConsinments.Health;

namespace AutoLedgeBook.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var book = ExcelHealthDocumentConsinmentBook.FromFile(@"D:\Downloads\����\����� �����\22-6 �5 2 ������ 2 ����� 16.01.xls");
        }
    }
}