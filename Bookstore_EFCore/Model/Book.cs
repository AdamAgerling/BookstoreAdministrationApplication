using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreAdmin.Model
{
    public class Book
    {
        [Key]
        public string ISBN13 { get; set; }
        public required string BookTitle { get; set; }
        public decimal BookPrice { get; set; }
        public DateTime BookRelease { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        [ForeignKey("BookLanguage")]
        public int LanguageId { get; set; }
        public BookLanguage BookLanguage { get; set; }

        [ForeignKey("Publisher")]
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
    }
}