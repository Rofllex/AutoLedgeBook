using System;

using Newtonsoft.Json;

namespace AutoLedgeBook;

public class RecentConsinmentsBook
{
    public RecentConsinmentsBook(DateTime lastOpened, string filePath, ConsinmentsBookType bookType)
        => (LastOpened, FilePath, BookType) = (lastOpened, filePath, bookType);

    public RecentConsinmentsBook(string filePath, ConsinmentsBookType bookType) : this (DateTime.Now, filePath, bookType)
    {
    }

    [JsonConstructor]
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    private RecentConsinmentsBook()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    {
    }

    [JsonProperty("lastOpened")] public DateTime LastOpened { get; set; }

    [JsonProperty("filePath", Required = Required.Always)] public string FilePath { get; set; }

    [JsonProperty("bookType", Required = Required.Always)] public ConsinmentsBookType BookType { get; set; }
}


