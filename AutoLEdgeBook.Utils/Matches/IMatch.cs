namespace AutoLedgeBook.Utils.Matches;

/// <summary>
///     Сопоставление.
/// </summary>
/// <typeparam name="Destination"></typeparam>
/// <typeparam name="Source"></typeparam>
public interface IMatch<Destination, Source> where Destination : class
                                                where Source : class
{
    /// <summary>
    ///     Ключ.
    /// </summary>
    Destination Key { get; }

    /// <summary>
    ///     Значения для ключа.
    /// </summary>
    Source[] Values { get; }
}


