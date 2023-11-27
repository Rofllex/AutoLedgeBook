namespace AutoLedgeBook.Data.Abstractions;

/// <summary>
///     Интерфейс сохранения книги.
/// </summary>
public interface ISaveable
{
    /// <summary>
    ///     Сохранить
    /// </summary>
    void Save();
    /// <summary>
    ///     Сохранить по пути.
    /// </summary>
    /// <param name="filePath"></param>
    void SaveAs(string filePath);

    /// <summary>
    ///     Сохранить с настройками.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="saveSettings"></param>
    void SaveAs(string filePath, ISaveSettings saveSettings);
}

public interface ISaveSettings { }
