using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreAdmin.Model
{
    public class Book
    {
        [Key]
        [MaxLength(13)]
        [MinLength(13)]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN must be exactly 13 digits.")]
        public string ISBN13 { get; set; }

        public string BookTitle { get; set; }
        public decimal BookPrice { get; set; }
        public DateTime BookRelease { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        public int LanguageId { get; set; }
        public BookLanguage Language { get; set; }

        public string ImageId { get; set; }
        [ForeignKey("ImageId")]
        public Image Image { get; set; }

        public ICollection<InventoryBalance> InventoryBalances { get; set; }
    }
}