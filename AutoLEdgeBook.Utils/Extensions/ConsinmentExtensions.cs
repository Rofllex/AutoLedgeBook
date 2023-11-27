using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Utils.Matches;

namespace AutoLedgeBook.Utils.Extensions;

public static class ConsinmentExtensions
{
    /// <summary>
    ///     Сравнить контрольные значения в накладных.
    /// </summary>
    public static bool CompareCheckSum(this IReadOnlyConsinmentNote currentConsinment, IReadOnlyConsinmentNote consinmentNote, int decimalsRound = 1)
          {
        if (consinmentNote == null)
            throw new ArgumentNullException(nameof(consinmentNote));
        if (decimalsRound < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalsRound));

        double currentKilo = Math.Round(currentConsinment.GetTotalProductsWeight(), decimalsRound);
        double consinmentKilo = Math.Round(consinmentNote.GetTotalProductsWeight(), decimalsRound);
        double currentPcs = Math.Round(currentConsinment.GetTotalProductsPcs(), decimalsRound);
        double consinmentPcs = Math.Round(consinmentNote.GetTotalProductsPcs(), decimalsRound);

        return currentKilo == consinmentKilo
                    && currentPcs == consinmentPcs;
    }

    /// <summary>
    ///     Грубое сравнение значений в накладных
    /// </summary>
    public static bool BruteCompareCheckSum(this IReadOnlyConsinmentNote currentConsinment, IReadOnlyConsinmentNote consinmentNote, int decimalsRound = 1)
    {
        if (consinmentNote is null)
            throw new ArgumentNullException(nameof(consinmentNote));
        if (decimalsRound < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalsRound));
        double currentConsinmentControlValue = Math.Round(currentConsinment.GetTotalProductsWeight() + currentConsinment.GetTotalProductsPcs(), decimalsRound);
        double conisnmentControlValue = Math.Round(consinmentNote.GetTotalProductsWeight() + consinmentNote.GetTotalProductsPcs(), decimalsRound);
        return currentConsinmentControlValue == conisnmentControlValue;
    }

    /// <summary>
    ///     Автоматическое автоматическое сопоставление коллекций по именам.
    ///     Если коллекция будет пустая, то вернет пустое перечисление сопоставлений.
    /// </summary>
    /// <exception cref="CompareMatchException" />
    /// <inheritdoc cref="DefaultProductMatcher.Match(IEnumerable{IAccountingProduct}, IReadOnlyCollection{IReadOnlyAccountingProduct}, IMatchesCollection{string, string})"/>
    public static IEnumerable<IMatch<IAccountingProduct, IReadOnlyAccountingProduct>> AutoMatch(this IAccountingProductCollection<IAccountingProduct> accountingProducts, IAccountingProductCollection<IReadOnlyAccountingProduct> otherProduct, IMatchesCollection<string,string> nameMatches)
    {
        if (otherProduct is null)
            throw new ArgumentNullException(nameof(otherProduct));
        if (nameMatches is null)
            throw new ArgumentNullException(nameof(nameMatches));
        
        IMatchesCollection<IAccountingProduct, IReadOnlyAccountingProduct> matches = DefaultProductMatcher.Match(accountingProducts, otherProduct, nameMatches);
        return matches;
    }

    /// <summary>
    ///     Очиста значения всех продуктов.
    /// </summary>
    public static void FillZero(this IEnumerable<IAccountingProduct> products)
    {
        if (products is null)
            throw new ArgumentNullException(nameof(products));
        
        foreach (IAccountingProduct product in products)
            product.Value = 0;
    }
}

