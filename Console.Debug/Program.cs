using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.DatabaseConsinmentIntegration;
using AutoLedgeBook.Data.Excel.LowLevel;


using xl = Microsoft.Office.Interop.Excel;
using AutoLedgeBook.Data.ExcelConsinments.Health;

namespace AutoLedgeBook.ConsoleDebug;


using static Console;

static class Program
{
    static Program()
    {
        ExecutableDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    }

    public static readonly string ExecutableDirectoryPath;

    [STAThread]
    static void Main(string[] _)
    {
        var consinmentsBook = ExcelHealthDocumentConsinmentBook.FromFile(@"D:\Downloads\накл\Новая папка\22-6 №5 2 неделя 2 блюда 16.01.xls");   

    }

    private static void ShowProducts(IAccountingProductCollection<IReadOnlyAccountingProduct> products)
    {
        int maxNameLength = products.Max(p => p.Name.Length);

        foreach (IReadOnlyAccountingProduct product in products)
        {
            string productName = new string(' ', maxNameLength - product.Name.Length) + product.Name;
            WriteLine($"{productName} => {Math.Round(product.Value, 4)}");
        }
    }

    private static void ShowProducts(IAccountingProductCollection<IAccountingProduct> products)
    {
        int maxNameLength = products.Max(p => p.Name.Length);
        foreach (IReadOnlyAccountingProduct product in products)
        {
            string productName = new string(' ', maxNameLength - product.Name.Length) + product.Name;
            WriteLine($"{productName} => {Math.Round(product.Value, 4)}");
        }
    }

    private static string FormatArray(string[] array, string separator = ", ")
    {
        if (array.Length == 1)
            return $"\"{array[0]}\"";

        return array.Aggregate((a, b) =>
        {
            if (a == b)
                return $"\"{b}\"";

            return $"{a}{separator}\"{b}\"";
        });
    }
}
