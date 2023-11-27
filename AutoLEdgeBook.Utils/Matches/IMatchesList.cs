#nullable enable

namespace AutoLedgeBook.Utils.Matches;

/// <summary>
///     Список сопоставлений.
/// </summary>
public interface IMatchesList<TDestination, TSource> : IMatchesCollection<TDestination, TSource>
                                                        where TDestination : class
                                                        where TSource : class
{
    /// <summary>
    ///     Добавить новое сопоставление.
    /// </summary>
    void Add(TDestination destination, TSource[] sources);

    /// <summary>
    ///     Удалить сопоставление
    /// </summary>
    void Remove(TDestination destination);

    /// <summary>
    ///     Проверить наличие ключа сопоставления
    /// </summary>
    bool ContainsKey(TDestination destination);
}
