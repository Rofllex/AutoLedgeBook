using AutoLedgeBook.Utils.Matches;

#nullable enable

namespace AutoLedgeBook.Utils.Extensions;

public static class MatchesExtensions
{
    public static IEditableMatch<TDestination, TSource>? AsEditable<TDestination, TSource>(this IMatch<TDestination, TSource> match)
        where TDestination : class
        where TSource : class
            => match as IEditableMatch<TDestination, TSource>;

    public static IMatchesList<TDestination, TSource>? AsList<TDestination, TSource>(this IMatchesCollection<TDestination, TSource> collection)
        where TDestination : class
        where TSource : class
            => collection as IMatchesList<TDestination, TSource>;
}
