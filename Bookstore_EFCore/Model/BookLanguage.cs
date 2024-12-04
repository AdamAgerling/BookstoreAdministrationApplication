using System.ComponentModel.DataAnnotations;

namespace BookstoreAdmin.Model
{
    public class BookLanguage
    {
        [Key]
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }

        public List<Book> Books { get; set; }
    }
}