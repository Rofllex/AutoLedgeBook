
using AutoLedgeBook.Data.Abstractions;

#nullable enable

namespace AutoLedgeBook.Utils.Matches;

/// <summary>
///     Исключение ошибки сопоставления.
/// </summary>
public class CompareMatchException : Exception
{
    public CompareMatchException(string productName, CompareMatchError result)
    {
        ProductName = productName;
        Result = result;

        Message = $"Произошла ошибка при сопоставлении продукта \"{ productName }\"";
        Message += "\n" + Result switch
        {
            CompareMatchError.ProductNotFound => "Продукт в книге накладных не найден",
            CompareMatchError.ProductMatchNotFound => "Сопоставление для продукта не найдено",
            _ => throw new InvalidDataException()
        };
    }

    /// <summary>
    ///     Ошибка сопоставления
    /// </summary>
    public CompareMatchError Result { get; init; }

    /// <summary>
    ///     Наименование продукта.
    /// </summary>
    public string ProductName { get; }

    public override string Message { get; }

    
}

