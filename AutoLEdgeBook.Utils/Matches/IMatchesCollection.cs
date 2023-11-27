namespace AutoLedgeBook.Utils.Matches;


/// <summary>
///     Коллекция сопоставлений.
/// </summary>
/// <typeparam name="Destination"></typeparam>
/// <typeparam name="Source"></typeparam>
public interface IMatchesCollection<Destination, Source> : IReadOnlyCollection<IMatch<Destination, Source>>, IDisposable where Destination : class
                                                                                                                    where Source : class
{
    /// <summary>
    ///     Получить сопоставления по ключу.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IMatch<Destination, Source> this[Destination key] { get; }
    
    /// <summary>
    ///     Получить сопоставления по значению.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IMatch<Destination, Source> GetBySourceValue(Source key);
    
    /// <summary>
    ///     Получить ключевые значения в сопоставлениях.
    /// </summary>
    /// <returns></returns>
    Destination[] GetKeys();
}


