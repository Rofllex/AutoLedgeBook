using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using AutoLedgeBook.Data.Abstractions;

#nullable enable

namespace AutoLedgeBook.Utils.Matches;

/// <summary>
///     Сравнение по умолчанию.
/// </summary>
public static class DefaultProductMatcher
{
    /// <summary>
    ///     Сопоставить перезаписываемую коллекцию продуктов с неперезаписываемой.
    /// </summary>
    /// <param name="bookProducts">Перезаписывоемое перечисление продуктов</param>
    /// <param name="consinmentProducts"></param>
    /// <param name="bookToConsinmentProductNamesMatches"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IMatchesCollection<TDestinationProduct, TSourceProduct> Match<TDestinationProduct, TSourceProduct>(IEnumerable<TDestinationProduct> bookProducts,
                                                                                                                     IReadOnlyCollection<TSourceProduct> consinmentProducts,
                                                                                                                     IMatchesCollection<string, string> bookToConsinmentProductNamesMatches)
        where TDestinationProduct : class, IAccountingProduct
        where TSourceProduct : class, IReadOnlyAccountingProduct
    {
        if (bookProducts is null)
            throw new ArgumentNullException(nameof(bookProducts));
        if (consinmentProducts is null)
            throw new ArgumentNullException(nameof(consinmentProducts));
        if (bookToConsinmentProductNamesMatches is null)
            throw new ArgumentNullException(nameof(bookToConsinmentProductNamesMatches));

        return AccountingProductMatchesCollection<TDestinationProduct, TSourceProduct>.Match(bookProducts, consinmentProducts, bookToConsinmentProductNamesMatches);
    }

    /// <summary>
    ///     Сопоставить строковые литералы при помощи словаря сопоставлений
    /// </summary>
    /// <param name="destinationNames">Наименования получателя</param>
    /// <param name="sourceNames">Наименования источника</param>
    /// <param name="nameToNameMatches"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="AggregateException" />
    public static IMatchesCollection<string, string> Match(IEnumerable<string> destinationNames, IReadOnlyCollection<string> sourceNames, IMatchesCollection<string, string> nameToNameMatches)
    {
        Dictionary<string, IMatch<string, string>> matches = new(capacity: sourceNames.Count);

        List<CompareMatchException> cmeList = new();

        foreach (string sourceName in sourceNames)
        {
            IMatch<string, string> match = nameToNameMatches.GetBySourceValue(sourceName);
            string? destinationName = destinationNames.FirstOrDefault(n => n == match.Key, null);

            if (destinationName is null)
            {
                cmeList.Add(new(match.Key, CompareMatchError.ProductNotFound));
                continue;
            }

            if (matches.ContainsKey(destinationName))
                continue;

            string[] sources;
            if (match.Values.Length == 1)
                sources = new string[] { sourceName };
            else
                sources = sourceNames.Where(s => match.Values.Contains(s)).ToArray();

            IMatch<string, string> findedMatch = new MemoryMatch<string, string>(destinationName, sources);
            matches[findedMatch.Key] = findedMatch;
        }

        if (cmeList.Count > 0)
            throw new AggregateException(cmeList);

        return new MatchesCollection<string, string>(matches);
    }


}

public class MatchesCollection<TDestination, TSource> : IMatchesCollection<TDestination, TSource> where TDestination : class where TSource : class
{
    private readonly IReadOnlyDictionary<TDestination, IMatch<TDestination, TSource>> _matchedDictionary;
    private readonly TDestination[] _keys;

    public MatchesCollection(IReadOnlyDictionary<TDestination, IMatch<TDestination, TSource>> matchedDictionary)
    {
        _keys = matchedDictionary.Keys.ToArray();
        _matchedDictionary = matchedDictionary;
    }

    public IMatch<TDestination, TSource> this[TDestination key] => _matchedDictionary[key];

    public int Count => _keys.Length;

    public void Dispose()
    {
    }

    public IMatch<TDestination, TSource> GetBySourceValue(TSource value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return _matchedDictionary.Values.FirstOrDefault(m => m.Values.Contains(value)) ?? throw new Exception($"Ни одно сопоставление не содержит ключа \"{value}\"");
    }

    public IEnumerator<IMatch<TDestination, TSource>> GetEnumerator() => _matchedDictionary.Values.GetEnumerator();

    public TDestination[] GetKeys() => _keys;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal class ProductNamesEqualityComparer : IEqualityComparer<IReadOnlyAccountingProduct>
{
    public static ProductNamesEqualityComparer Instance { get; } = new ProductNamesEqualityComparer();
    
    private ProductNamesEqualityComparer() { }

    public bool Equals(IReadOnlyAccountingProduct? x, IReadOnlyAccountingProduct? y)
    {
        if (x is null || y is null)
            return false;
        return x.Name == y.Name;
    }

    public int GetHashCode([DisallowNull] IReadOnlyAccountingProduct obj)
        => HashCode.Combine(obj);
}



public class AccountingProductMatchesCollection<TProduct, TReadOnlyProduct> : IMatchesCollection<TProduct, TReadOnlyProduct>
                                                          where TProduct : class, IAccountingProduct
                                                          where TReadOnlyProduct : class, IReadOnlyAccountingProduct
{
    public static AccountingProductMatchesCollection<TProduct, TReadOnlyProduct> Match(IEnumerable<TProduct> destinationProducts, IReadOnlyCollection<TReadOnlyProduct> sourceProducts, IMatchesCollection<string, string> nameMatches)
    {
        return new AccountingProductMatchesCollection<TProduct, TReadOnlyProduct>(destinationProducts, sourceProducts, nameMatches);
    }

    private readonly List<IMatch<TProduct, TReadOnlyProduct>> _matchesList;
    private readonly Lazy<TProduct[]> _keys;

    private AccountingProductMatchesCollection(IEnumerable<TProduct> destinationProducts, IReadOnlyCollection<TReadOnlyProduct> sourceProducts, IMatchesCollection<string, string> nameMatches)
    {
        _matchesList = MatchAll(destinationProducts, sourceProducts, nameMatches);
        _keys = new Lazy<TProduct[]>(() => { return _matchesList.Select(m => m.Key).ToArray(); });
    }

    public int Count => _keys.Value.Length;

    public IMatch<TProduct, TReadOnlyProduct> this[TProduct key]
    {
        get
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            return _matchesList.FirstOrDefault(m => m.Key.Name == key.Name) ?? throw new CompareMatchException(key.Name, CompareMatchError.ProductMatchNotFound);
        }
    }

    public IMatch<TProduct, TReadOnlyProduct> GetBySourceValue(TReadOnlyProduct sourceProduct)
    {
        if (sourceProduct is null)
            throw new ArgumentNullException(nameof(sourceProduct));
        return _matchesList.FirstOrDefault(m => m.Values.Contains(sourceProduct, ProductNamesEqualityComparer.Instance)) ?? throw new CompareMatchException(sourceProduct.Name, CompareMatchError.ProductMatchNotFound);
    }

    public TProduct[] GetKeys() => _keys.Value;

    public IEnumerator<IMatch<TProduct, TReadOnlyProduct>> GetEnumerator() => _matchesList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
    }

    private List<IMatch<TProduct, TReadOnlyProduct>> MatchAll(IEnumerable<TProduct> destinationProducts,
                                                              IReadOnlyCollection<TReadOnlyProduct> sourceProducts,
                                                              IMatchesCollection<string, string> nameMatches)
    {
        StepMatchEnumerable<TProduct, TReadOnlyProduct> stepMatchEnumerable = new StepMatchEnumerable<TProduct, TReadOnlyProduct>(destinationProducts, sourceProducts, nameMatches);
        return stepMatchEnumerable.ToList();
    }
}

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class MemoryMatch<Destination, Source> : IMatch<Destination, Source> where Destination : class
                                                                            where Source : class
{
    public MemoryMatch(Destination key, Source[] values)
        => (Key, Values) = (key, values);

    public Destination Key { get; }

    public Source[] Values { get; }

    protected virtual string GetDebuggerDisplay()
    {
        string valuesString;
        IEnumerator valuesEnumerator = Values.GetEnumerator();
        if (valuesEnumerator.MoveNext())
        {
            valuesString = "[ ";
            valuesString += $"\"{valuesEnumerator.Current}\"";
            do
            {
                valuesString += $", \"{valuesEnumerator.Current}\"";
            } while (valuesEnumerator.MoveNext());
        }
        else
        {
            valuesString = "[ ]";
        }


        return $"{Key} => {valuesString}";
    }
}

public class StepMatchEnumerable<TProduct, TReadOnlyProduct> : IEnumerable<IMatch<TProduct, TReadOnlyProduct>>
                                                               where TProduct : class, IAccountingProduct
                                                               where TReadOnlyProduct : class, IReadOnlyAccountingProduct
{
    private readonly IEnumerable<TProduct> _destinationProducts;
    private readonly IReadOnlyCollection<TReadOnlyProduct> _sourceProducts;
    private readonly IMatchesCollection<string, string> _nameMatches;


    public StepMatchEnumerable(IEnumerable<TProduct> destinationProducts, IReadOnlyCollection<TReadOnlyProduct> sourceProducts, IMatchesCollection<string, string> nameMatches)
    {
        _destinationProducts = destinationProducts;
        _sourceProducts = sourceProducts;
        _nameMatches = nameMatches;
    }

    public IEnumerator<IMatch<TProduct, TReadOnlyProduct>> GetEnumerator() => new Enumerator(_destinationProducts, _sourceProducts, _nameMatches);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<IMatch<TProduct, TReadOnlyProduct>>
    {
        private readonly IEnumerable<TProduct> _destinationProducts;
        private readonly IEnumerable<TReadOnlyProduct> _sourceProducts;
        private readonly IMatchesCollection<string, string> _nameMatches;

        private readonly IEnumerator<TReadOnlyProduct> _sourceProductsEnumerator;

        private readonly HashSet<TReadOnlyProduct> _enumeratedSourceProducts;

        private IMatch<TProduct, TReadOnlyProduct>? _current;

        public Enumerator(IEnumerable<TProduct> destinationProducts, IReadOnlyCollection<TReadOnlyProduct> sourceProducts, IMatchesCollection<string, string> nameMatches)
        {
            _destinationProducts = destinationProducts;

            _sourceProductsEnumerator = sourceProducts.GetEnumerator();

            _enumeratedSourceProducts = new HashSet<TReadOnlyProduct>(capacity: sourceProducts.Count);

            _sourceProducts = sourceProducts;

            _nameMatches = nameMatches;

            _current = null;
        }

        public IMatch<TProduct, TReadOnlyProduct> Current => _current ?? throw new InvalidOperationException();

        object IEnumerator.Current => Current;

        public void Dispose() => _sourceProductsEnumerator.Dispose();


        public bool MoveNext()
        {
            if (!_sourceProductsEnumerator.MoveNext())
                return false;

            TReadOnlyProduct sourceProduct = _sourceProductsEnumerator.Current;
            if (_enumeratedSourceProducts.FirstOrDefault(p => p.Name == sourceProduct.Name) is not null)
                return MoveNext();

            IMatch<string, string> nameMatch = _nameMatches.GetBySourceValue(sourceProduct.Name);

            TProduct destinationProduct = _destinationProducts.FirstOrDefault(p => p.Name == nameMatch.Key) ?? throw new CompareMatchException(sourceProduct.Name, CompareMatchError.ProductNotFound);

            TReadOnlyProduct[] sourceProducts;
            if (nameMatch.Values.Length == 1)
            {
                sourceProducts = new TReadOnlyProduct[] { sourceProduct };
            }
            else
            {
                sourceProducts = _sourceProducts.Where(p => nameMatch.Values.Contains(p.Name)).ToArray();
            }

            foreach (var mathedProduct in sourceProducts)
                _enumeratedSourceProducts.Add(mathedProduct);

            _current = new MemoryMatch<TProduct, TReadOnlyProduct>(destinationProduct, sourceProducts);
            return true;
        }

        public void Reset()
        {
            _sourceProductsEnumerator.Reset();
            _current = null;
        }
    }
}