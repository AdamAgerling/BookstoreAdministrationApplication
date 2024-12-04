using System.ComponentModel.DataAnnotations;

namespace BookstoreAdmin.Model
{
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }
        public string PublisherCountry { get; set; }

        public List<Book> Books { get; set; }
    }
}