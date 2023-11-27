using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLedgeBook.Models
{
    public class RecentConsinmentsBookModel
    {
        public RecentConsinmentsBookModel(RecentConsinmentsBook origin)
        {
            Origin = origin;
        }

        [Browsable(false)] public RecentConsinmentsBook Origin { get; init; }

        [DisplayName("Последнее использование")] public DateTime LastOpened => Origin.LastOpened;

        [DisplayName("Путь до файла")] public string FilePath => Origin.FilePath;

        [DisplayName("Тип книги")] public ConsinmentsBookType BookType => Origin.BookType;
    
        public static explicit operator RecentConsinmentsBook (RecentConsinmentsBookModel model) => model.Origin;
    }
}
