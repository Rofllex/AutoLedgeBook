
#nullable enable

namespace AutoLedgeBook.Utils.Matches;

/// <summary>
///     Сопоставление с возможностью редактирования.
/// </summary>
/// <typeparam name="TDestination"></typeparam>
/// <typeparam name="TSource"></typeparam>
public interface IEditableMatch<TDestination, TSource> : IMatch<TDestination, TSource>
                                                        where TDestination : class
                                                        where TSource : class
{
    /// <summary>
    ///     Редактируемые значения
    /// </summary>
    new IList<TSource> Values { get; }
}
