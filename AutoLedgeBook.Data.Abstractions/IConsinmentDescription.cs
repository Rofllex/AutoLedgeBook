namespace AutoLedgeBook.Data.Abstractions
{
    public interface IReadOnlyConsinmentDescription
    {
        /// <summary>
        ///     Направление, куда направляются продукты (22/6-1, 22/6-2)
        /// </summary>
        string Destination { get; }

        /// <summary>
        /// 
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 
        /// </summary>
        int PersonsCount { get; }
    }


    /// <summary>
    ///     Описание накладной.
    /// </summary>
    public interface IConsinmentDescription : IReadOnlyConsinmentDescription
    {
        /// <summary>
        ///     Направление, куда направляются продукты (22/6-1, 22/6-2)
        /// </summary>
        new string Destination { get; set; }
    
        /// <summary>
        /// 
        /// </summary>
        new string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        new int PersonsCount { get; set; }
    }
}
