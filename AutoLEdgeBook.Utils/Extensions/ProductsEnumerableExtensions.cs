using System.Collections;

using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Utils.Extensions;

public static class ProductsEnumerableExtensions
{
    public static IEnumerable<TProduct> NonZeroProducts<TProduct>(this IEnumerable<TProduct> enumerable) where TProduct : IReadOnlyAccountingProduct => new NonZeroProductsEnumerable<TProduct>(enumerable);

    private class NonZeroProductsEnumerable<TProduct> : IEnumerable<TProduct> where TProduct : IReadOnlyAccountingProduct
    {
        private readonly IEnumerator<TProduct> _originalEnumerator;

        public NonZeroProductsEnumerable(IEnumerable<TProduct> original) => _originalEnumerator = original.GetEnumerator();
        

        public IEnumerator<TProduct> GetEnumerator()
        {
            while (_originalEnumerator.MoveNext())
                if (_originalEnumerator.Current.Value > 0)
                    yield return _originalEnumerator.Current;
        }
        

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
