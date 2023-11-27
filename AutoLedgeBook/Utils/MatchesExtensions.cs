using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoLedgeBook.Utils.Matches;
using AutoLedgeBook.Data.Abstractions;


namespace AutoLedgeBook.Utils
{
    public static class MatchesExtensions
    {
        public static IMatchesCollection<TDestinationProduct, TSourceProduct> Match<TDestinationProduct, TSourceProduct>(this IAccountingProductCollection<TDestinationProduct> products, IAccountingProductCollection<TSourceProduct> matches, IMatchesCollection<string,string> nameMatches) where TDestinationProduct : class, IAccountingProduct
                                                                                                                           where TSourceProduct : class, IReadOnlyAccountingProduct
            => DefaultProductMatcher.Match(products, matches, nameMatches);            
    }
}
