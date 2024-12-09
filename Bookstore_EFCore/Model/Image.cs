using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreAdmin.Model
{
    [Table("Images")]
    public class Image
    {
        [Key]
        [MaxLength(13)]
        [MinLength(13)]
        public string ImageId { get; set; }
        public string ImageUrl { get; set; }
    }
}
